using System;
using System.Drawing;
using System.IO;

using Newtonsoft.Json;

using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.Accounting;
using Sq1.Core.DataFeed;
using Sq1.Core.Streaming;
using Sq1.Core.Execution;
using Sq1.Core.DataTypes;

using Sq1.Adapters.Quik.Streaming;
using Sq1.Adapters.Quik.Broker.Livesim;
using Sq1.Adapters.Quik.Broker.Terminal;

namespace Sq1.Adapters.Quik.Broker {
	public partial class QuikBroker : BrokerAdapter {
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

		public QuikBroker() : base() {		// base() will be invoked anyways by .NET, just wanna make it obvious (reminder)
			base.Name			= "QuikBroker-DllScanned";
			base.Icon			= (Bitmap)Sq1.Adapters.Quik.Properties.Resources.imgQuikStreamingAdapter;
			this.QuikTerminal	= new QuikTerminal(this);
			this.QuikDllName	= this.QuikTerminal.DllName;

			this.AccountMicexAutoPopulated = new Account("QUIK_MICEX_ACCTNR_NOT_SET", -1001);
			base.OrderCallbackDupesChecker = new OrderCallbackDupesCheckerQuik(this);
		}
		public override void InitializeDataSource_inverse(DataSource dataSource, StreamingAdapter streamingAdapter, OrderProcessor orderProcessor) {
			base.Name = "QuikBroker";

			if (base.LivesimBroker_ownImplementation == null) {
				base.LivesimBroker_ownImplementation	= new QuikBrokerLivesim("OWN_IMPLEMENTATION_USED_FOR_LIVESIM_NOT_DUMMY");
			} else {
				string msg = "ALREADY_INITIALIZED_OWN_DISTRIBUTOR MUST_NEVER_HAPPEN_BUT_CRITICAL_WHEN_IT_DOES";
				Assembler.PopupException(msg);
			}

			base.InitializeDataSource_inverse(dataSource, streamingAdapter, orderProcessor);
		}
		public override BrokerEditor BrokerEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.BrokerEditorInitializeHelper(dataSourceEditor);
			base.BrokerEditorInstance = new BrokerQuikEditor(this, dataSourceEditor);
			return base.BrokerEditorInstance;
		}

		public void CallbackTradeStateReceivedQuik(long SernoExchange, DateTime tradeDate, 
				string classCode, string secCode, double priceFill, int qtyFill,
				double tradePrice2, double tradeTradeSysCommission, double tradeTScommission) {
			string msig = Name + "::CallbackTradeStateReceivedQuik(): ";
			try {
				string msg = "";
				Order order = this.OrderProcessor.DataSnapshot.OrdersPending.ScanRecentForSernoExchange((long)SernoExchange);
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
				this.OrderProcessor.UpdateOrderState_postProcess(order, sameStateOmsg, priceFill, qtyFill);
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
				//+ " newOrderStateReceived=[" + newOrderStateReceived + "]"
				+ " SernoExchange=[" + SernoExchange + "] GUID=[" + GUID + "]"
				+ " //" + Name + ":CallbackOrderStateReceivedQuik()";

			Order order = base.ScanEvidentLanesForGuid_nullUnsafe(GUID);
			if (order == null) {
				// already reported "PENDING_ORDER_NOT_FOUND__RETURNING_ORDER_NULL" in base.FindOrderLaneOptimizedNullUnsafe() 
				return;
			}

			if (order.SernoExchange == 0) {
				order.SernoExchange  = SernoExchange;	// link GUID to SernoExchange - the main role of QUIK broker adapter :)
			}
			if (newOrderStateReceived == OrderState.KillerDone || newOrderStateReceived == OrderState.Rejected) {
				if (fillPrice != 0) {
					string msg = "QUIK_HINTS_ON_SOMETHING fillPrice[" + fillPrice + "]!=0 for newOrderStateReceived[" + newOrderStateReceived + "]";
					this.OrderProcessor.AppendOrderMessage_propagateToGui_checkThrowOrderNull(order, msg);
					fillPrice = 0;
				}
				if (fillQnty != 0) {
					string msg = "QUIK_HINTS_ON_SOMETHING fillQnty[" + fillPrice + "]!=0 for newOrderStateReceived[" + newOrderStateReceived + "]";
					this.OrderProcessor.AppendOrderMessage_propagateToGui_checkThrowOrderNull(order, msg);
					fillQnty = 0;
				}
			}
			
			if (this.Name == "Mock BrokerAdapter" 
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

			base.OrderProcessor.UpdateOrderState_dontPostProcess(order, omsg);
			//base.CallbackOrderStateReceived(order);
		}

		//[JsonIgnore]	[JsonIgnore]	[JsonIgnore]	
		ConnectionState previousConnectionState = ConnectionState.UnknownConnectionState;
		int identicalConnectionStatesReported = 0;
		int identicalConnectionStatesReportedLimit = 3;
		public void callbackTerminalConnectionStateUpdated(ConnectionState state, string message) {
			if (this.previousConnectionState == ConnectionState.UnknownConnectionState) {
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
			if (state != ConnectionState.SymbolSubscribed) {
				string msig = " //" + Name + "::callbackConnectionUpdated(): state=[" + state + "]"
					+ " mustBe[" + ConnectionState.SymbolSubscribed + "] message=[" + message + "]";
				Assembler.PopupException(message + msig, null, false);
			}
			string msg = state + " " + message;
			Assembler.DisplayConnectionStatus(state, msg);
		}
		public override string ModifyOrderType_accordingToMarketOrder_asBrokerSpecificInjection(Order order) {
			string msg = "";
			if (order.Alert.QuoteCreatedThisAlert == null) {
				msg = "ORDER_MARKED_INCONSISTENT__order.Alert.QuoteCreatedThisAlert=null SymbolInfo["
					 + order.Alert.Symbol + "/" + order.Alert.SymbolClass + "]";
				OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg);
				base.OrderProcessor.UpdateOrderState_postProcess(order, newOrderState);
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
							OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg);
							this.OrderProcessor.UpdateOrderState_postProcess(order, newOrderState);
							#if DEBUG
							Debugger.Break();
							#endif
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
							this.OrderProcessor.UpdateOrderState_postProcess(order, newOrderState);
							#if DEBUG
							Debugger.Break();
							#endif
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