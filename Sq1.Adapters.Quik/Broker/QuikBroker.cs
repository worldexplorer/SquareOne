﻿using System;
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

		[JsonIgnore]	public	bool				IsConnectedToTerminal	{ get { return this.QuikDllConnector != null && this.QuikDllConnector.DllConnected; } }
		[JsonIgnore]	public	string				DllConnectionStatus_oppositeAction { get {
			return this.IsConnectedToTerminal ? "Disconnect from DLL (now connected)" : "Connect to DLL (now disconnected)";
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
			base.OrderCallbackDupesChecker = new OrderCallbackDupesCheckerQuik(this);

			this.QuikFolder					= @"C:\Program Files (x86)\QUIK-Junior";
			this.ReconnectTimeoutMillis		= 3000;
			this.UpstreamConnectionState = ConnectionState.UnknownConnectionState;
		}

		public override BrokerEditor BrokerEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.BrokerEditorInitializeHelper(dataSourceEditor);
			base.BrokerEditorInstance = new QuikBrokerEditorControl(this, dataSourceEditor);
			return base.BrokerEditorInstance;
		}

		public void TradeState_callbackFromQuikDll(long SernoExchange, DateTime tradeDate, 
				string classCode, string secCode, double priceFill, int qtyFill,
				double tradePrice2, double tradeTradeSysCommission, double tradeTScommission) {
			string msig = Name + "::TradeState_callbackFromQuikDll(): ";
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

		public void OrderState_callbackFromQuikDll(OrderState newOrderStateReceived, string GUID, long SernoExchange,
												string classCode, string secCode, double fillPrice, int fillQnty) {
			string msig = "fillPrice[" + fillPrice + "] fillQnty[" + fillQnty + "]" 
				+ " " + secCode + "/" + classCode
				//+ " newOrderStateReceived=[" + newOrderStateReceived + "]"
				+ " SernoExchange=[" + SernoExchange + "] GUID=[" + GUID + "]"
				+ " //" + Name + "::OrderState_callbackFromQuikDll()";

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


		public void ConnectionStateUpdated_callbackFromQuikDll(ConnectionState state, string message) {
			if (this.UpstreamConnectionState == ConnectionState.UnknownConnectionState) {
				this.UpstreamConnectionState = state;
			}
			if (this.UpstreamConnectionState != state) {
				this.UpstreamConnectionState = state;
				identicalConnection_statesReported = 0;
			}
			identicalConnection_statesReported++;
			if (identicalConnection_statesReported > identicalConnection_statesReported_limit) {
				return;
			}
			//if (state != ConnectionState.UpstreamConnected_downstreamSubscribed) {
			//    string msig = " //" + Name + "::ConnectionStateUpdated_callbackFromQuikDll(): state=[" + state + "]"
			//        + " mustBe[" + ConnectionState.UpstreamConnected_downstreamSubscribed + "] message=[" + message + "]";
			//    Assembler.PopupException(message + msig);
			//}
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