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
		[JsonIgnore]	public	QuikDllConnector	QuikDllConnector		{ get; protected set; }

		[JsonProperty]	public	string				QuikFolder				{ get; internal set; }		// internal <= POPULATED_IN_EDITOR
		[JsonProperty]	public	string				Trans2QuikDllName		{ get; protected set; }
		[JsonProperty]	public	string				Trans2QuikDllUrl		{ get; protected set; }

		[JsonIgnore]	public	bool				QuikFolderExists		{ get { return Directory.Exists(this.QuikFolder); } }
		[JsonIgnore]	public	string				Trans2QuikDllAbsPath	{ get { return Path.Combine(Assembler.InstanceInitialized.AppStartupPath, this.Trans2QuikDllName); } }
		[JsonIgnore]	public	bool				Trans2QuikDllFound		{ get { return File.Exists(this.Trans2QuikDllAbsPath); } }


		[JsonProperty]	public	string				QuikClientCode			{ get; protected set; }
		[JsonProperty]	public	int					ReconnectTimeoutMillis	{ get; internal set; }		// internal <= POPULATED_IN_EDITOR
		[JsonProperty]			Account				AccountMicex;			//{ get; internal set; }		// internal <= POPULATED_IN_EDITOR
		[JsonIgnore]	public	Account				AccountMicexAutoPopulated {
			get { return AccountMicex; }
			internal set {
				this.AccountMicex = value;
				this.AccountMicex.Initialize(this);
			}
		}
		[JsonProperty]	public	bool				GoRealDontUseOwnLivesim	{ get; internal set; }		// internal <= POPULATED_IN_EDITOR

		[JsonIgnore]	public	bool				CanSendToTerminal	{ get { return this.QuikDllConnector != null && this.QuikDllConnector.CanSend; } }
		[JsonIgnore]	public	string				DllConnectionStatus_oppositeAction { get {
			return this.CanSendToTerminal ? "Disconnect from QUIK (now connected)" : "Connect to QUIK via DLL (now disconnected)";
			} }

		[JsonIgnore]	private	int					identicalConnection_statesReported = 0;
		[JsonIgnore]	private	int					identicalConnection_statesReported_limit = 3;


	
		public QuikBroker() : base() {		// base() will be invoked anyways by .NET, just wanna make it obvious (reminder)
			base.Name				= "QuikBroker-DllScanned-NOT_INITIALIZED";
			base.Icon				= (Bitmap)Sq1.Adapters.Quik.Properties.Resources.imgQuikStreamingAdapter;
			this.QuikDllConnector	= new QuikDllConnector(this);
			this.Trans2QuikDllName	= this.QuikDllConnector.DllName;
			this.Trans2QuikDllUrl	= this.QuikDllConnector.DllUrl;

			this.AccountMicexAutoPopulated = new Account("QUIK_MICEX_ACCTNR_NOT_SET", -1001);
			//base.OrderCallbackDupesChecker = new OrderCallbackDupesCheckerQuik(this);

			this.QuikFolder					= @"C:\Program Files (x86)\QUIK-Junior";
			this.ReconnectTimeoutMillis		= 3000;
			this.UpstreamConnectionState = ConnectionState.UnknownConnectionState;
		}

		public override BrokerEditor BrokerEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.BrokerEditorInitializeHelper(dataSourceEditor);
			base.BrokerEditorInstance = new QuikBrokerEditorControl(this, dataSourceEditor);
			return base.BrokerEditorInstance;
		}

		public void TradeState_callbackFromQuikDll(long sernoExchange, DateTime tradeDate, 
				string classCode, string secCode, double priceFill, int qtyFill,
				double tradePrice2, double tradeTradeSysCommission, double tradeTScommission) {
			string msig = " //" + Name + "::TradeState_callbackFromQuikDll()";
			try {
				OrderLane pendings = this.OrderProcessor.DataSnapshot.OrdersPending;	// when TradeState(), both Market and Limit orders are Pending
				string		suggestion		= "PASS_suggestLane=TRUE";
				OrderLane	suggestedLane	= null;
				Order order = pendings.ScanRecent_forSernoExchange((long)sernoExchange, out suggestedLane, out suggestion);
				if (order == null) {
					string msg_findOrder = "FAILED_TO_FIND_ORDER_IN_OrdersPending sernoExchange[" + sernoExchange + "]"
						+ " suggestedLane[" + suggestedLane + "] suggestion[" + suggestion + "]";
					throw new Exception(msg_findOrder + msig);
				}

				string filled = qtyFill == 0 ? "NOT_FILLED" : "FILLED";
				if (qtyFill > 0 && qtyFill != order.QtyRequested) {
					filled = "PARTIAL_FILL_OF[" + order.QtyRequested + "]";
				}
				filled += " " + order.Alert.MarketLimitStopAsString;
				string msg = filled + " [" + qtyFill + "]@[" + priceFill + "]"
					+ " " + secCode + "/" + classCode
					+ " tradeDate[" + tradeDate + "]"
					+ " tradePrice2[" + tradePrice2 + "]"
					+ " tradeTradeSysCommission[" + tradeTradeSysCommission + "] tradeTScommission[" + tradeTScommission + "]";

				// QUIK tells commission in TradeState callback; in OrderState callback there is no way;
				// for Market* tradeTScommission>0
				// for Limit*  tradeTScommission=0
				double commissionSum = tradeTradeSysCommission + tradeTScommission;
				if (order.CommissionFill != commissionSum && commissionSum > 0) {
					//order.CommissionFill  = commissionSum;
				}

				if (order.Alert.Bars.SymbolInfo.MarketOrders_priceFill_bringBackFromOutrageous) {
				    // for Market* orders (Futures), Quik declares Fill in TradeState(); Fill for Limit* (Futures) orders I should expect in OrderState()
				    // but the price is outrageous (beoynd the bar High/Low) and looks like PriceMin/PriceMax (CONFIRM?)
				    bool outOfBarPriceFilled = order.Alert.MarketOrderAs == MarketOrderAs.MarketUnchanged_DANGEROUS
				                            || order.Alert.MarketOrderAs == MarketOrderAs.MarketZeroSentToBroker
				                            || order.Alert.MarketOrderAs == MarketOrderAs.MarketMinMaxSentToBroker;
				    if (outOfBarPriceFilled && order.Alert.SymbolClass == "SPBFUT") {
				        if (order.Alert.PositionLongShortFromDirection == PositionLongShort.Long) {
				            priceFill = order.PriceRequested + commissionSum;
				        } else {
				            priceFill = order.PriceRequested - commissionSum;
				        }
				    }
				}

			    // Quik declares Fill in TradeState() only for Market* orders (Futures), Fill for Limit* (Futures) orders I should expect in OrderState()
				bool fillHappened = priceFill > 0 && qtyFill > 0;
				if (fillHappened) {
					order.DateServerLastFillUpdate = tradeDate;
				}

				OrderState statusChanged_onlyIfFilled = fillHappened ? OrderState.Filled : order.State;
				OrderStateMessage sameStateOmsg = new OrderStateMessage(order, statusChanged_onlyIfFilled, msg);
				this.OrderProcessor.Order_updateState_mustBeDifferent_postProcess(sameStateOmsg, priceFill, qtyFill);
			} catch (Exception exc) {
				string msg = "THROWN_SOMEWHERE_SORRY_IN_CallbackTradeStateReceivedQuik";
				Assembler.PopupException(msg + msig, exc);
			}
		}

		public void OrderState_callbackFromQuikDll(OrderState newOrderStateReceived, string GUID, long SernoExchange,
												string classCode, string secCode, double priceFill, int qtyFill) {
			string msig = " //" + Name + "::OrderState_callbackFromQuikDll()";

			Order order = base.ScanEvidentLanes_forGuid_nullUnsafe(GUID);
			if (order == null) {
				// already reported "PENDING_ORDER_NOT_FOUND__RETURNING_ORDER_NULL" in base.FindOrderLaneOptimizedNullUnsafe() 
				return;
			}

			string filled = qtyFill == 0 ? "NOT_FILLED" : "FILLED";
			if (qtyFill > 0) {
				if (qtyFill != order.QtyRequested) {
					filled = "PARTIAL_FILL_OF[" + order.QtyRequested + "]";
					if (newOrderStateReceived != OrderState.FilledPartially) {
						filled = "WRONG_STATE[" + newOrderStateReceived + "] " + filled;
					}
				} else {
					if (newOrderStateReceived != OrderState.Filled) {
						filled = "WRONG_STATE[" + newOrderStateReceived + "] " + filled;
					}
				}
			}
			filled += " " + order.Alert.MarketLimitStopAsString;

			string msg = filled + " [" + qtyFill + "]@[" + priceFill + "]"
				+ " " + secCode + "/" + classCode
				+ " [" + newOrderStateReceived + "]"
				+ " sernoExchange[" + SernoExchange + "] GUID=[" + GUID + "]";

			if (order.SernoExchange == 0) {
				order.SernoExchange  = SernoExchange;	// linking GUID to SernoExchange otherwize we'll never find it again
			}

			if (newOrderStateReceived == OrderState.KillerDone || newOrderStateReceived == OrderState.Rejected) {
				if (priceFill != 0) {
					string msg1 = "QUIK_HINTS_ON_SOMETHING fillPrice[" + priceFill + "]!=0 for newOrderStateReceived[" + newOrderStateReceived + "]";
					this.OrderProcessor.AppendOrderMessage_propagateToGui(order, msg1);
					priceFill = 0;
				}
				if (qtyFill != 0) {
					string msg1 = "QUIK_HINTS_ON_SOMETHING fillQnty[" + priceFill + "]!=0 for newOrderStateReceived[" + newOrderStateReceived + "]";
					this.OrderProcessor.AppendOrderMessage_propagateToGui(order, msg1);
					qtyFill = 0;
				}
			}

			if (newOrderStateReceived == OrderState.FilledPartially || newOrderStateReceived == OrderState.Filled) {
				OrderStateMessage omsg1 = new OrderStateMessage(order, newOrderStateReceived, "LIMIT_ORDER_FILLED " + msg);
				this.OrderProcessor.Order_updateState_mustBeDifferent_postProcess(omsg1, priceFill, qtyFill);
			} else {
				OrderStateMessage omsg = new OrderStateMessage(order, newOrderStateReceived, msg);
				base.OrderProcessor.Order_updateState_mustBeTheSame_dontPostProcess(omsg);
			}
		}


		public void ConnectionStateUpdated_callbackFromQuikDll(ConnectionState state, string message) {
			this.UpstreamConnectionState = state;
			Assembler.DisplayConnectionStatus(state, message);
		}
		public override string ModifyOrderType_accordingToMarketOrder_asBrokerSpecificInjection(Order order) {
			string msg = "";
			if (order.Alert.QuoteCreatedThisAlert == null) {
				msg = "ORDER_MARKED_INCONSISTENT__order.Alert.QuoteCreatedThisAlert=null SymbolInfo["
					 + order.Alert.Symbol + "/" + order.Alert.SymbolClass + "]";
				OrderStateMessage newOrderState = new OrderStateMessage(order, OrderState.ErrorOrderInconsistent, msg);
				base.OrderProcessor.Order_updateState_mustBeDifferent_postProcess(newOrderState);
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
							this.OrderProcessor.Order_updateState_mustBeDifferent_postProcess(newOrderState);
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
							this.OrderProcessor.Order_updateState_mustBeDifferent_postProcess(newOrderState);
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