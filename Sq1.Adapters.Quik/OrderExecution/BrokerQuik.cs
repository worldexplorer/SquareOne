using System;
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
using Sq1.Core.Support;

namespace Sq1.Adapters.Quik {
	public class BrokerQuik : BrokerProvider {
		//[JsonIgnore]	BrokerQuikEditor editor;
		[JsonIgnore]	public QuikTerminal QuikTerminal { get; protected set; }
		[JsonIgnore]	public int AskPrice { get; protected set; }
		[JsonIgnore]	public int BidPrice { get; protected set; }
		[JsonProperty]	public string QuikFolder { get; internal set; }
		[JsonProperty]	public string QuikDllName { get; internal set; }
		[JsonIgnore]	public string QuikDllAbsPath { get {return Path.Combine(this.QuikFolder, this.QuikDllName);} }
		[JsonProperty]	public string QuikClientCode { get; internal set; }
		[JsonProperty]	public int ReconnectTimeoutMillis { get; internal set; }
		[JsonProperty]	private Account AccountMicex;
		[JsonIgnore]	public Account AccountMicexAutoPopulated {
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

			this.AccountMicexAutoPopulated = new Account("ACCTNR_NOT_SET", -1001);
			//base.OrdersPending = new BrokerOrdersPending(true);
			base.OrderCallbackDupesChecker = new OrderCallbackDupesCheckerQuik(this);
		}
		public override void Initialize(DataSource dataSource, StreamingProvider streamingProvider,
				OrderProcessor orderProcessor, IStatusReporter connectionStatus) {
			base.Initialize(dataSource, streamingProvider, orderProcessor, connectionStatus);
			base.Name = "Quik BrokerProvider";

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

		public override void Connect() {
			QuikTerminal.ConnectDll();
		}
		public override void Disconnect() {
			QuikTerminal.DisconnectDll();
		}

		public void CallbackTradeStateReceivedQuik(long SernoExchange, DateTime tradeDate, 
				string classCode, string secCode, double priceFill, int qtyFill,
				double tradePrice2, double tradeTradeSysCommission, double tradeTScommission) {
			try {
				string msg = Name + "::TradeState(): ";
				Order order = this.OrderProcessor.DataSnapshot.OrdersPending.FindBySernoExchange((long)SernoExchange);
				if (order == null) {
					msg += " Order with SernoExchange[" + SernoExchange + "] was not found"
						//+ "; " + base.OrderProcessor.DataSnapshot.DataSnapshot.Serializer.SessionSernosAsString
						;
					throw new Exception(msg);
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
				base.StatusReporter.PopupException(null, exc);
			}
		}
		public void CallbackOrderStateReceivedQuik(OrderState orderState, string GUID, long SernoExchange,
				string classCode, string secCode, double fillPrice, int fillQnty) {
			string msg = Name + "::CallbackOrderStateReceivedQuik(): ";
			Order orderExecuted = null;
			try {
				orderExecuted = base.CallbackOrderStateReceivedFindOrderCheckThrow(GUID);

				if (orderExecuted.SernoExchange == 0) {
					// link GUID to SernoExchange - the main role of QUIK broker provider :)
					orderExecuted.SernoExchange = SernoExchange;
				}

				if (orderState == OrderState.KillerDone || orderState == OrderState.Rejected) {
					fillPrice = 0;
					//fillQnty = 0;
				}
				if (this.Name == "Mock BrokerProvider" 
						&& orderExecuted.Alert.MarketLimitStop == MarketLimitStop.Market 
						&& orderExecuted.Alert.MarketOrderAs == MarketOrderAs.MarketZeroSentToBroker
						&& (fillPrice != -999.99 && fillPrice != 0)) {
					fillPrice = orderExecuted.Alert.PriceScript
						+ ((orderExecuted.Alert.PositionLongShortFromDirection == PositionLongShort.Long) ? 100 : -100);
				}

				msg += " Status=[" + orderState + "] @price [" + fillPrice + "]"	//filled[" + fillQnty + "] 
					+ " SernoSession=[" + GUID + "] SernoExchange=[" + SernoExchange + "] Guid=[" + orderExecuted.GUID + "]"
					+ " " + secCode + "/" + classCode;
				OrderStateMessage omsg = new OrderStateMessage(orderExecuted, orderState, msg);
				base.OrderProcessor.UpdateOrderStateAndPostProcess(orderExecuted, omsg, fillPrice, fillQnty);
				//base.TradeManager.appendMessageAndPropagate(orderExecuted, newOrderState);
			} catch (Exception exc) {
				base.StatusReporter.PopupException(null, exc);
			}
			base.CallbackOrderStateReceived(orderExecuted);
		}
		
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
				Assembler.PopupException(Name + "::callbackConnectionUpdated(): state=[" + state + "] message=[" + message + "]");
			}
			string msg = state + " " + message;
			if (base.StatusReporter != null) {
				base.StatusReporter.UpdateConnectionStatus(ConnectionState.Connected, 0, msg);
			} else {
				//Assembler.PopupException("base.StatusReporter=null, reporting here: " + msg);
			}
		}
		public override void OrderSubmit(Order order) {
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
			base.OrderProcessor.DataSnapshot.FindStateLaneDoesntContain(order).Insert(0, order);


			msg = " orderStateFromTerminal[" + orderStateFromTerminalMustGetSubmitting + "]"
				+ " msgSumbittedFromTerminal[" + msgSumbittedFromTerminal + "]"
				+ " sernoSessionFromTerminal[" + sernoSessionFromTerminal + "]";
			//Assembler.PopupException(msig + msg);

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
			string msg;
			if (order.Alert.PriceDeposited != -1) {
				msg = "alert[" + order.Alert + "] has already PricePaid[" + order.Alert.PriceDeposited + "] != 0";
				return;
			}
			if (order.Alert.QuoteCreatedThisAlert == null) {
				order.Alert.QuoteCreatedThisAlert = this.StreamingProvider.StreamingDataSnapshot.LastQuoteGetForSymbol(order.Alert.Symbol);
			}
			QuoteQuik quikQuote = QuoteQuik.SafeUpcast(order.Alert.QuoteCreatedThisAlert);
			if (order.Alert.PositionLongShortFromDirection == PositionLongShort.Long) {
				if (quikQuote.FortsDepositBuy <= 0) {
					msg = "Quote.FortsDepositBuy[" + quikQuote.FortsDepositBuy + "] <= ZERO";
				} else {
					order.Alert.PriceDeposited = quikQuote.FortsDepositBuy;
				}
			} else {
				if (quikQuote.FortsDepositSell <= 0) {
					msg = "Quote.FortsDepositSell[" + quikQuote.FortsDepositSell + "] <= ZERO";
				} else {
					order.Alert.PriceDeposited = -quikQuote.FortsDepositSell;
				}
			}
		}
		public override string ModifyOrderTypeAccordingToMarketOrderAsBrokerSpecificInjection(Order order) {
			string msg = "";
			if (order.Alert.QuoteCreatedThisAlert == null) {
				msg = "order.Quote=null SymbolInfo[" + order.Alert.Symbol + "/" + order.Alert.SymbolClass
					+ "] //" + msg;
				OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg);
				base.OrderProcessor.UpdateOrderStateAndPostProcess(order, newOrderState);
				throw new Exception(msg);
			}
			if (order.Alert.QuoteCreatedThisAlert == null) {
				msg = "WRONG: order.Alert.QuoteCreatedThisAlert=null for alert=[" + order.Alert + "]";
				throw new Exception(msg);
			}
			if (order.Alert.QuoteCreatedThisAlert is QuoteQuik == false) {
				msg = "Should be of a type Sq1.Adapters.Quik.QuoteQuik instead of Sq1.Core.DataTypes.Quote: "
					+ order.Alert.QuoteCreatedThisAlert;
				throw new Exception(msg);
			}
			QuoteQuik quikQuote = order.Alert.QuoteCreatedThisAlert as QuoteQuik;
			if (order.Alert.PositionLongShortFromDirection == PositionLongShort.Long) {
				double priceMax = quikQuote.FortsPriceMax;
				if (priceMax <= 0) {
					msg = "Price=Quote.FortsPriceMax[" + priceMax + "]=ZERO for"
						+ " Alert.MarketOrderAs=[" + order.Alert.MarketOrderAs + "]"
						+ " (Slippage=" + order.SlippageFill + ")";
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
			return msg;
		}
	}
}