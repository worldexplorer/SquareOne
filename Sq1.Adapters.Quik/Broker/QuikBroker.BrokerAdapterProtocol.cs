using System;
using System.IO;

using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.Execution;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Streaming;

using Sq1.Adapters.Quik.Streaming;
using Sq1.Adapters.Quik.Broker.Livesim;

namespace Sq1.Adapters.Quik.Broker {
	public partial class QuikBroker : BrokerAdapter {
		public override void InitializeDataSource_inverse(DataSource dataSource, StreamingAdapter streamingAdapter, OrderProcessor orderProcessor) {
			base.Name			= "QuikBroker";
			base.ReasonToExist	= "INSTANCE_FOR[" + dataSource.Name + "]";

			if (base.LivesimBroker_ownImplementation == null) {
				base.LivesimBroker_ownImplementation	= new QuikBrokerLivesim("OWN_IMPLEMENTATION_USED_FOR_LIVESIM_NOT_DUMMY");
			} else {
				string msg = "ALREADY_INITIALIZED_OWN_LIVESIM MUST_NEVER_HAPPEN_BUT_CRITICAL_WHEN_IT_DOES";
				Assembler.PopupException(msg);
			}

			if (base.UpstreamConnectedOnAppRestart) {
				this.Connect();
			}

			base.InitializeDataSource_inverse(dataSource, streamingAdapter, orderProcessor);
		}

		public override void Connect() {
			string msig = " //QuikBroker.Connect()";
			if (string.IsNullOrEmpty(this.QuikFolder)) {
				string msg = "I_REFUSE_TO_CONNECT_WITH_NULL_FOLDER QuikBroker.QuikFolder[" + this.QuikFolder + "]";
				throw new Exception(msg + msig);
			}

			if (this.QuikFolderExists == false) {
				string msg = "I_REFUSE_TO_CONNECT__NON_EXISTING_FOLDER QuikBroker.QuikFolder[" + this.QuikFolder + "]";
				throw new Exception(msg + msig);
			}

			if (this.Trans2QuikDllFound == false) {
				string msg = "I_REFUSE_TO_DONNECT__DLL_CAN_NOT_BE_FOUND QuikBroker.trans2QuikDllAbsPath[" + this.Trans2QuikDllAbsPath + "]";
				throw new Exception(msg + msig);
			}
			this.QuikDllConnector.ConnectDll();
		}
		public override void Disconnect() { this.QuikDllConnector.DisconnectDll(); }

		public override void Submit(Order order) {
			//Debugger.Break();
			string msig = " //" + Name + "::OrderSubmit():"
				+ " Guid[" + order.GUID + "]" + " SernoExchange[" + order.SernoExchange + "]"
				+ " SernoSession[" + order.SernoSession + "]";
			string msg = "";

			// was the reason of TP/SL "sequenced" submit here?...
			//if (this.Name == "Mock BrokerAdapter") Thread.Sleep(1000);

			char typeMarketLimitStop = '?';
			switch (order.Alert.MarketLimitStop) {
				case MarketLimitStop.Market:
					typeMarketLimitStop = 'M';
					break;
				case MarketLimitStop.Limit:
					typeMarketLimitStop = 'L';
					break;
				case MarketLimitStop.Stop:
					typeMarketLimitStop = 'S';
					break;
				case MarketLimitStop.StopLimit:
					typeMarketLimitStop = 'S';
					break;
				default:
					msg = " No MarketLimitStop[" + order.Alert.MarketLimitStop + "] handler for order[" + order.ToString() + "]"
						+ "; must be one of those: Market/Limit/Stop";
					this.OrderProcessor.UpdateOrderState_postProcess(order,
						new OrderStateMessage(order, OrderState.Error, msig + msg));
					throw new Exception(msig + msg);
			}

			char opBuySell = (order.Alert.PositionLongShortFromDirection == PositionLongShort.Long) ? 'B' : 'S';
			int sernoSessionFromTerminal = -999;
			string msgSubmittedFromTerminal = "";
			OrderState orderStateFromTerminalMustGetSubmitted = OrderState.Unknown;

			double priceFill = order.PriceRequested;
			this.QuikDllConnector.OrderSubmit_sendTransaction_async(opBuySell, typeMarketLimitStop,
				order.Alert.Symbol, order.Alert.SymbolClass,
				order.PriceRequested, (int)order.QtyRequested, order.GUID,
				out sernoSessionFromTerminal, out msgSubmittedFromTerminal, out orderStateFromTerminalMustGetSubmitted);

			msg = msgSubmittedFromTerminal + "order.SernoSession[" + order.SernoSession + "]=>[" + sernoSessionFromTerminal + "] ";
			order.SernoSession = sernoSessionFromTerminal;

			OrderStateMessage newState = new OrderStateMessage(order, orderStateFromTerminalMustGetSubmitted, msg + msig);
			base.OrderProcessor.UpdateOrderState_postProcess(order, newState);
		}
		public override void Kill(Order victimOrder) {
			string msig = Name + "::OrderKillSubmit():"
				+ "State[" + victimOrder.State + "]"
				+ " [" + victimOrder.Alert.Symbol + "/" + victimOrder.Alert.SymbolClass + "]"
				+ " VictimToBeKilled.SernoExchange[" + victimOrder.SernoExchange + "] ";

			bool victimWasStopOrder = victimOrder.Alert.MarketLimitStop == MarketLimitStop.Stop;
			
			string msgSubmittedFromTerminal = "";
			int sernoSessionFromTerminal = -999;
			OrderState killerStateFromTerminal = OrderState.Unknown;

			Order killerOrder = victimOrder.KillerOrder;

			QuikDllConnector.KillOrder_sendTransaction_async(victimOrder.Alert.Symbol, victimOrder.Alert.SymbolClass,
				killerOrder.GUID.ToString(),
				victimOrder.GUID.ToString(),
				victimOrder.SernoExchange, victimWasStopOrder,
				out msgSubmittedFromTerminal, out sernoSessionFromTerminal, out killerStateFromTerminal);

			killerOrder.SernoSession = sernoSessionFromTerminal;

			string msg = "killerStateFromTerminal[" + killerStateFromTerminal + "]"
				+ " msgSubmittedFromTerminal[" + msgSubmittedFromTerminal + "]"
				+ " sernoSessionFromTerminal[" + sernoSessionFromTerminal + "]";
			base.OrderProcessor.AppendOrderMessage_propagateToGui_checkThrowOrderNull(killerOrder, msig + msg);

			// don't set State.KillPending to Killer!!! Killer has KillSubmitting->BulletFlying->KillerDone
			//base.OrderManager.UpdateOrderStateAndPostProcess(killerOrder,
			//	new OrderStateMessage(killerOrder, killerStateFromTerminal, msgSubmittedFromTerminal));
		}
		public override void CancelReplace(Order order, Order newOrder) {
			throw new Exception("TODO: don't forget to implement before going live!");
		}
		public override void OrderEnrich_preSubmit_brokerSpecificInjection(Order order) {
			string msig = " //BrokerQuik.OrderPreSubmitEnrichBrokerSpecificInjection(" + order.ToString() + ")";
			string msg = "";
			
			if (order.Alert.QuoteCreatedThisAlert == null) {
				Quote lastMayNotBeTheCreatorHereHavingNoParentBars = this.StreamingAdapter.StreamingDataSnapshot
					.LastQuote_getForSymbol(order.Alert.Symbol);
				order.Alert.QuoteCreatedThisAlert = lastMayNotBeTheCreatorHereHavingNoParentBars;
				string msg2 = "AVOIDING_ORDER_MARKED_INCONSISTENT: " + order.Alert.QuoteCreatedThisAlert;
				Assembler.PopupException(msg2, null, false);
			}
			
			if (order.Alert.PriceDeposited != -1) {
				msg = "I_REFISE_TO_ENRICH_FILLED_ALERT alert[" + order.Alert + "].PriceDeposited[" + order.Alert.PriceDeposited + "] != 0";
				Assembler.PopupException(msg + msig, null, false);
				return;
			}
			QuoteQuik quikQuote = QuoteQuik.SafeUpcast(order.Alert.QuoteCreatedThisAlert);
			if (order.Alert.PositionLongShortFromDirection == PositionLongShort.Long) {
				if (quikQuote.FortsDepositBuy <= 0) {
					//DISABLED_FOR_ORDER_TO_SHOWUP_IN_ORDER_EXECUTION_FORM msg = "Quote.FortsDepositBuy[" + quikQuote.FortsDepositBuy + "] <= ZERO";
				} else {
					order.Alert.PriceDeposited = quikQuote.FortsDepositBuy;
				}
			} else {
				if (quikQuote.FortsDepositSell <= 0) {
					//DISABLED_FOR_ORDER_TO_SHOWUP_IN_ORDER_EXECUTION_FORM msg = "Quote.FortsDepositSell[" + quikQuote.FortsDepositSell + "] <= ZERO";
				} else {
					order.Alert.PriceDeposited = -quikQuote.FortsDepositSell;
				}
			}
			if (string.IsNullOrEmpty(msg)) return;
			Assembler.PopupException(msg + msig, null, false);
		}

	}
}