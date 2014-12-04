using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

using Newtonsoft.Json;
using Sq1.Adapters.Quik.Terminal;
using Sq1.Core;
using Sq1.Core.Accounting;
using Sq1.Core.Broker;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Streaming;

namespace Sq1.Adapters.Quik {
	public class BrokerQuik : BrokerProvider {
		[JsonIgnore]	public	QuikTerminal	QuikTerminal			{ get; protected set; }
		[JsonProperty]	public	string			QuikFolder				{ get; internal set; }		// internal <= POPULATED_IN_EDITOR
		[JsonProperty]	public	string			QuikDllName				{ get; protected set; }
		[JsonIgnore]	public	string			QuikDllAbsPath			{ get {return Path.Combine(this.QuikFolder, this.QuikDllName);} }
		[JsonProperty]	public	string			QuikClientCode			{ get; protected set; }
		[JsonProperty]	public	int				ReconnectTimeoutMillis	{ get; internal set; }		// internal <= POPULATED_IN_EDITOR
		[JsonProperty]			Account			AccountMicex;			//{ get; internal set; }		// internal <= POPULATED_IN_EDITOR
		[JsonIgnore]	public	Account			AccountMicexAutoPopulated {
			get { return AccountMicex; }
			internal set {
				this.AccountMicex = value;
				this.AccountMicex.Initialize(this);
			}
		}

		public BrokerQuik() : base() {		// base() will be invoked anyways by .NET, just wanna make it obvious (reminder)
			base.Name = "Quik BrokerDummy";
			base.Icon = (Bitmap)Sq1.Adapters.Quik.Properties.Resources.imgQuikStreamingProvider;
			this.QuikTerminal = new QuikTerminal(this);
			this.QuikDllName = this.QuikTerminal.DllName;

			this.AccountMicexAutoPopulated = new Account("QUIK_MICEX_ACCTNR_NOT_SET", -1001);
			base.OrderCallbackDupesChecker = new OrderCallbackDupesCheckerQuik(this);
		}
		public override void Initialize(DataSource dataSource, StreamingProvider streamingProvider, OrderProcessor orderProcessor) {
			base.Initialize(dataSource, streamingProvider, orderProcessor);
			base.Name = "Quik Broker";

			if (String.IsNullOrEmpty(this.QuikFolder)) return;
			if (Directory.Exists(this.QuikFolder) == false) {
				string msg = "QuikStreamingProvider.QuikFolder[" + this.QuikFolder + "] doesn't exist; will not try to QuikTerminal.ConnectDll()";
				//Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			if (File.Exists(this.QuikDllAbsPath) == false) {
				string msg = "QuikStreamingProvider.QuikDllAbsPath[" + this.QuikDllAbsPath + "] doesn't exist; will not try to QuikTerminal.ConnectDll()";
				//Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			
			this.QuikTerminal.ConnectDll();
		}
		public override BrokerEditor BrokerEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.BrokerEditorInitializeHelper(dataSourceEditor);
			base.brokerEditorInstance = new BrokerQuikEditor(this, dataSourceEditor);
			return base.brokerEditorInstance;
		}
		public override void Connect() { QuikTerminal.ConnectDll(); }
		public override void Disconnect() { QuikTerminal.DisconnectDll(); }
		public void CallbackTradeStateReceivedQuik(long SernoExchange, DateTime tradeDate, 
				string classCode, string secCode, double priceFill, int qtyFill,
				double tradePrice2, double tradeTradeSysCommission, double tradeTScommission) {
			string msig = Name + "::CallbackTradeStateReceivedQuik(): ";
			try {
				string msg = "";
				Order order = this.OrderProcessor.DataSnapshot.OrdersPending.FindBySernoExchange((long)SernoExchange);
				if (order == null) {
					msg += " Order with SernoExchange[" + SernoExchange + "] was not found"
						//+ "; " + base.OrderProcessor.DataSnapshot.DataSnapshot.Serializer.SessionSernosAsString
						;
					throw new Exception(msig + msg);
				}

				msg += " tradeDate=[" + tradeDate + "] quantity[" + qtyFill + "] @price [" + priceFill + "]"
					+ " tradePrice2=[" + tradePrice2 + "]"
					+ " tradeTradeSysCommission=[" + tradeTradeSysCommission + "] tradeTScommission=[" + tradeTScommission + "]"
					+ " " + secCode + "/" + classCode;

				//OrderStateMessage sameStateOmsg = new OrderStateMessage(order, order.State, msg);
				//this.OrderManager.UpdateTradeStateAndPostProcess(order, sameStateOmsg, priceFill, qtyFill);
				OrderStateMessage sameStateOmsg = new OrderStateMessage(order, OrderState.TradeStatus, msg);
				this.OrderProcessor.UpdateOrderStateAndPostProcess(order, sameStateOmsg, priceFill, qtyFill);
				order.DateServerLastFillUpdate = tradeDate;

				// workaround: calc "implied" slippage from executed price, instead of assumed for LimitCrossMarket
				if (//order.Alert.MarketLimitStop == MarketLimitStop.Market && 
						order.Alert.MarketOrderAs == MarketOrderAs.MarketMinMaxSentToBroker
						&& order.SlippageFill == 0) {
					if (order.Alert.PositionLongShortFromDirection == PositionLongShort.Long) {
						order.SlippageFill = order.PriceFill - order.CurrentBid;
					} else {
						order.SlippageFill = order.PriceFill - order.CurrentAsk;
					}
				}
			} catch (Exception exc) {
				string msg = "THROWN_SOMEWHERE_SORRY_IN_CallbackTradeStateReceivedQuik";
				Assembler.PopupException(msg + msig, exc);
			}
		}
		public void CallbackOrderStateReceivedQuik(OrderState newOrderStateReceived, string GUID, long SernoExchange,
												string classCode, string secCode, double fillPrice, int fillQnty) {
			string msig = "fillPrice[" + fillPrice + "] fillQnty[" + fillQnty + "]" 
				+ " " + secCode + "/" + classCode
				+ " newOrderStateReceived=[" + newOrderStateReceived + "] SernoExchange=[" + SernoExchange + "] GUID=[" + GUID + "]"
				+ " //" + Name + ":CallbackOrderStateReceivedQuik()";

			Order order = base.FindOrderLaneOptimizedNullUnsafe(GUID);
			if (order == null) {
				// already reported "PENDING_ORDER_NOT_FOUND__RETURNING_ORDER_NULL" in base.FindOrderLaneOptimizedNullUnsafe() 
				return;
			}

			if (order.SernoExchange == 0) {
				order.SernoExchange  = SernoExchange;	// link GUID to SernoExchange - the main role of QUIK broker provider :)
			}
			if (newOrderStateReceived == OrderState.KillerDone || newOrderStateReceived == OrderState.Rejected) {
				if (fillPrice != 0) {
					string msg = "QUIK_HINTS_ON_SOMETHING fillPrice[" + fillPrice + "]!=0 for newOrderStateReceived[" + newOrderStateReceived + "]";
					this.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msg);
					fillPrice = 0;
				}
				if (fillQnty != 0) {
					string msg = "QUIK_HINTS_ON_SOMETHING fillQnty[" + fillPrice + "]!=0 for newOrderStateReceived[" + newOrderStateReceived + "]";
					this.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msg);
					fillQnty = 0;
				}
			}
			
			if (this.Name == "Mock BrokerProvider" 
					&& order.Alert.MarketLimitStop == MarketLimitStop.Market 
					&& order.Alert.MarketOrderAs == MarketOrderAs.MarketZeroSentToBroker
					&& (fillPrice != -999.99 && fillPrice != 0)) {
				Assembler.PopupException("REMIND_ME_WHAT_THIS_EXPERIMENT_WAS_ALL_ABOUT?");
				fillPrice = order.Alert.PriceScript
					+ ((order.Alert.PositionLongShortFromDirection == PositionLongShort.Long) ? 100 : -100);
			}

			OrderStateMessage omsg = new OrderStateMessage(order, newOrderStateReceived, msig);

			// if you want to bring the check up earlier than UpdateOrderStateAndPostProcess will do it or change the message added to order
			//string whyIthinkQuikIsSpammingMe = this.OrderCallbackDupesChecker.OrderCallbackIsDupeReson(
			//	orderExecuted, omsg, fillPrice, fillQnty);
			//if (string.IsNullOrEmpty(whyIthinkQuikIsSpammingMe) == false) {
			//	this.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(orderExecuted,
			//		"ORDER_CALLBACK_DUPE__SKIPPED_PROCESSING: " + whyIthinkQuikIsSpammingMe);
			//	return;
			//}

			base.OrderProcessor.UpdateOrderStateDontPostProcess(order, omsg);
			base.CallbackOrderStateReceived(order);
		}

		//[JsonIgnore]	[JsonIgnore]	[JsonIgnore]	
		QuikConnectionState previousConnectionState = QuikConnectionState.None;
		int identicalConnectionStatesReported = 0;
		int identicalConnectionStatesReportedLimit = 3;
		public void callbackTerminalConnectionStateUpdated(QuikConnectionState state, string message) {
			if (this.previousConnectionState == QuikConnectionState.None) {
				this.previousConnectionState = state;
			}
			if (this.previousConnectionState != state) {
				this.previousConnectionState = state;
				identicalConnectionStatesReported = 0;
			}
			identicalConnectionStatesReported++;
			if (identicalConnectionStatesReported > identicalConnectionStatesReportedLimit) {
				return;
			}
			if (state != QuikConnectionState.DllConnected) {
				Assembler.PopupException(Name + "::callbackConnectionUpdated(): state=[" + state + "] message=[" + message + "]", null, false);
			}
			string msg = state + " " + message;
			if (base.StatusReporter != null) {
				base.StatusReporter.UpdateConnectionStatus(ConnectionState.Connected, 0, msg);
			} else {
				//Assembler.PopupException("base.StatusReporter=null, reporting here: " + msg);
			}
		}
		public override void OrderSubmit(Order order) {
			//Debugger.Break();
			string msig = Name + "::OrderSubmit():"
				+ " Guid[" + order.GUID + "]" + " SernoExchange[" + order.SernoExchange + "]"
				+ " SernoSession[" + order.SernoSession + "]";
			string msg = "";

			// was the reason of TP/SL "sequenced" submit here?...
			//if (this.Name == "Mock BrokerProvider") Thread.Sleep(1000);

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
					this.OrderProcessor.UpdateOrderStateAndPostProcess(order,
						new OrderStateMessage(order, OrderState.Error, msig + msg));
					throw new Exception(msig + msg);
			}

			char opBuySell = (order.Alert.PositionLongShortFromDirection == PositionLongShort.Long) ? 'B' : 'S';
			int sernoSessionFromTerminal = -999;
			string msgSumbittedFromTerminal = "";
			OrderState orderStateFromTerminalMustGetSubmitting = OrderState.Unknown;
			this.QuikTerminal.SendTransactionOrderAsync(opBuySell, typeMarketLimitStop,
				order.Alert.Symbol, order.Alert.SymbolClass,
				order.PriceRequested, (int)order.QtyRequested, order.GUID,
				out sernoSessionFromTerminal, out msgSumbittedFromTerminal, out orderStateFromTerminalMustGetSubmitting);

			order.SernoSession = sernoSessionFromTerminal;

			//base.OrderSubmitPostProcessOrdersPendingAdd(order);
			// I expect here "Submitted", move it now
			//base.OrderProcessor.DataSnapshot.StateLaneAddAppropriate(order);
			OrderLaneByState olist = base.OrderProcessor.DataSnapshot.FindStateLaneDoesntContain(order);
			olist.Insert(0, order);

			msg = " orderStateFromTerminal[" + orderStateFromTerminalMustGetSubmitting + "]"
				+ " msgSumbittedFromTerminal[" + msgSumbittedFromTerminal + "]"
				+ " sernoSessionFromTerminal[" + sernoSessionFromTerminal + "]";
			//Assembler.PopupException(msig + msg);

			//Debugger.Break();
			base.OrderProcessor.UpdateOrderStateAndPostProcess(order,
				new OrderStateMessage(order, orderStateFromTerminalMustGetSubmitting, msig + msg));
		}
		public override void OrderKillSubmit(Order victimOrder) {
			string msig = Name + "::OrderKillSubmit():"
				+ "State[" + victimOrder.State + "]"
				+ " [" + victimOrder.Alert.Symbol + "/" + victimOrder.Alert.SymbolClass + "]"
				+ " VictimToBeKilled.SernoExchange[" + victimOrder.SernoExchange + "] ";

			bool victimWasStopOrder = victimOrder.Alert.MarketLimitStop == MarketLimitStop.Stop;
			
			string msgSumbittedFromTerminal = "";
			int sernoSessionFromTerminal = -999;
			OrderState killerStateFromTerminal = OrderState.Unknown;

			Order killerOrder = victimOrder.KillerOrder;

			QuikTerminal.SendTransactionOrderKillAsync(victimOrder.Alert.Symbol, victimOrder.Alert.SymbolClass,
				killerOrder.GUID.ToString(),
				victimOrder.GUID.ToString(),
				victimOrder.SernoExchange, victimWasStopOrder,
				out msgSumbittedFromTerminal, out sernoSessionFromTerminal, out killerStateFromTerminal);

			killerOrder.SernoSession = sernoSessionFromTerminal;

			string msg = "killerStateFromTerminal[" + killerStateFromTerminal + "]"
				+ " msgSumbittedFromTerminal[" + msgSumbittedFromTerminal + "]"
				+ " sernoSessionFromTerminal[" + sernoSessionFromTerminal + "]";
			base.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(killerOrder, msig + msg);

			// don't set State.KillPending to Killer!!! Killer has KillSubmitting->BulletFlying->KillerDone
			//base.OrderManager.UpdateOrderStateAndPostProcess(killerOrder,
			//	new OrderStateMessage(killerOrder, killerStateFromTerminal, msgSumbittedFromTerminal));
		}
		public override void CancelReplace(Order order, Order newOrder) {
			throw new Exception("TODO: don't forget to implement before going live!");
		}
		public override void OrderPreSubmitEnrichBrokerSpecificInjection(Order order) {
			string msig = " //BrokerQuik.OrderPreSubmitEnrichBrokerSpecificInjection(" + order.ToString() + ")";
			string msg = "";
			
			if (order.Alert.QuoteCreatedThisAlert == null) {
				Quote lastMayNotBeTheCreatorHereHavingNoParentBars = this.StreamingProvider.StreamingDataSnapshot
					.LastQuoteCloneGetForSymbol(order.Alert.Symbol);
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
		public override string ModifyOrderTypeAccordingToMarketOrderAsBrokerSpecificInjection(Order order) {
			string msg = "";
			if (order.Alert.QuoteCreatedThisAlert == null) {
				msg = "ORDER_MARKED_INCONSISTENT__order.Alert.QuoteCreatedThisAlert=null SymbolInfo["
					 + order.Alert.Symbol + "/" + order.Alert.SymbolClass + "]";
				OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg);
				base.OrderProcessor.UpdateOrderStateAndPostProcess(order, newOrderState);
				throw new Exception(msg);
			}
			QuoteQuik quikQuote = order.Alert.QuoteCreatedThisAlert as QuoteQuik;
			if (quikQuote == null) {
				msg = "QUOTE_MUST_BE_TYPE(Sq1.Adapters.Quik.QuoteQuik)_INSTEAD_OF "
					+ order.Alert.QuoteCreatedThisAlert.GetType();
				throw new Exception(msg);
			}
			switch (order.Alert.MarketLimitStop) {
				case MarketLimitStop.Market:
					if (order.Alert.PositionLongShortFromDirection == PositionLongShort.Long) {
						double priceMax = quikQuote.FortsPriceMax;
						if (priceMax <= 0) {
							msg = "Price=Quote.FortsPriceMax[" + priceMax + "]=ZERO for"
								+ " Alert.MarketOrderAs=[" + order.Alert.MarketOrderAs + "]"
								+ " (Slippage=" + order.SlippageFill + ")";
							Debugger.Break();
							OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg);
							this.OrderProcessor.UpdateOrderStateAndPostProcess(order, newOrderState);
							throw new Exception(msg);
						}
						order.PriceRequested = priceMax;
						order.SlippageFill = 0;
						msg = "Setting PriceRequested=[" + order.PriceRequested + "]=Quote.FortsPriceMax"
							+ " since MarketOrderAs=[" + order.Alert.MarketOrderAs + "]"
							//	+ "; (useless Slippage=" + order.Slippage + ")"
							;
					} else {
						double priceMin = quikQuote.FortsPriceMin;
						if (priceMin <= 0) {
							msg = "Price=Quote.FortsPriceMin[" + priceMin + "]=ZERO for"
								+ " Alert.MarketOrderAs=[" + order.Alert.MarketOrderAs + "]"
								+ " (Slippage=" + order.SlippageFill + ")";
							Debugger.Break();
							OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg);
							this.OrderProcessor.UpdateOrderStateAndPostProcess(order, newOrderState);
							throw new Exception(msg);
						}
						order.PriceRequested = priceMin;
						order.SlippageFill = 0;
						msg = "Setting PriceRequested=[" + order.PriceRequested + "]=Quote.FortsPriceMin"
							+ "; since Alert.MarketOrderAs=[" + order.Alert.MarketOrderAs + "]"
							//	+ " (useless Slippage=" + order.Slippage + ")"
							+ "";
					}
					break;
				case MarketLimitStop.Limit:
					break;
				case MarketLimitStop.Stop:
					break;
				case MarketLimitStop.StopLimit:
					break;
			}
			return msg;
		}
	}
}