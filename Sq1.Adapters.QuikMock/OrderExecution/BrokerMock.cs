using System;
using System.Drawing;

using Newtonsoft.Json;
using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.DataFeed;
using Sq1.Core.Execution;
using Sq1.Core.Streaming;

using Sq1.Adapters.Quik;
using Sq1.Adapters.QuikMock.Terminal;
using System.Diagnostics;

namespace Sq1.Adapters.QuikMock {
	public class BrokerMock : BrokerQuik {
		[JsonIgnore]	public	QuikTerminalMock	MockTerminal;
		[JsonProperty]	public	int					ExecutionDelayMillis	{ get; internal set; }		// internal <= POPULATED_IN_EDITOR
		[JsonProperty]	public	int					RejectFirstNOrders		{ get; internal set; }		// internal <= POPULATED_IN_EDITOR
		[JsonProperty]	public	bool				RejectRandomly			{ get; internal set; }		// internal <= POPULATED_IN_EDITOR
		[JsonProperty]	public	bool				RejectAllUpcoming		{ get; internal set; }		// internal <= POPULATED_IN_EDITOR

		public BrokerMock() : base() {
			base.Name = "BrokerQuikMockDummy";
			base.Icon = (Bitmap)Sq1.Adapters.QuikMock.Properties.Resources.imgMockQuikStreamingAdapter;
			base.QuikTerminal = new QuikTerminalMock(this);
			this.ExecutionDelayMillis = 1000;
			this.RejectFirstNOrders = 5;
			this.RejectRandomly = true;
			this.RejectAllUpcoming = false;
		}
		public override void Initialize(DataSource dataSource, StreamingAdapter streamingAdapter, OrderProcessor orderProcessor) {
			base.Initialize(dataSource, streamingAdapter, orderProcessor);
			base.QuikTerminal.ConnectDll();
			base.Name = "BrokerQuikMock";
		}
		public override BrokerEditor BrokerEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.BrokerEditorInitializeHelper(dataSourceEditor);
			base.brokerEditorInstance = new BrokerMockEditor(this, dataSourceEditor);
			return base.brokerEditorInstance;
		}
		public override void CancelReplace(Order order, Order newOrder) {
			Assembler.PopupException("NYI: CancelReplace(" + order.ToString() + ", " + newOrder.ToString() + ")");
		}
		public override void OrderSubmit(Order order) {
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
					this.OrderProcessor.UpdateOrderStateAndPostProcess(order,
						new OrderStateMessage(order, OrderState.Error, msig + msg));
					throw new Exception(msig + msg);
			}

			char opBuySell = (order.Alert.PositionLongShortFromDirection == PositionLongShort.Long) ? 'B' : 'S';
			int sernoSessionFromTerminal = -999;
			string msgSubmittedFromTerminal = "";
			OrderState orderStateFromTerminalMustGetSubmitted = OrderState.Unknown;

			double priceFill = order.PriceRequested;

			if (order.Alert.MarketLimitStop == MarketLimitStop.Market) {
				// TODO: paste link where did you take this piece of silliness (I promise!!)
				StreamingMock quickBrokerAcceptsMarketOrdersOnlyWithMinOrMaxPrice = base.StreamingAdapter as StreamingMock;
				if (quickBrokerAcceptsMarketOrdersOnlyWithMinOrMaxPrice != null) {
					double fortsPriceMin = quickBrokerAcceptsMarketOrdersOnlyWithMinOrMaxPrice.StreamingDataSnapshotQuik.FortsGetPriceMinForSymbol(order.Alert.Symbol);
					double fortsPriceMax = quickBrokerAcceptsMarketOrdersOnlyWithMinOrMaxPrice.StreamingDataSnapshotQuik.FortsGetPriceMaxForSymbol(order.Alert.Symbol);
					bool isQuickMinOrMax = priceFill == fortsPriceMin || priceFill == fortsPriceMax;
					if (isQuickMinOrMax == true) {
						priceFill = order.Alert.PriceScriptAligned;
					} else {
						priceFill = order.Alert.PriceScriptAligned;
						string msg2 = "MOCKSTREAMING's FORTS MIN AND MAX FLOAT EACH QUOTE, IGNORING";
						Assembler.PopupException(msg2);
					}
				}
			}

			bool pausedToFinishBacktest = base.StreamingAdapter.DataSource.PumpingPausedGet(order.Alert.Bars);
			bool backtestIsRunning = order.Alert.Strategy.Script.Executor.Backtester.IsBacktestingNow;

			if (pausedToFinishBacktest) {
				if (backtestIsRunning == false) {
					string msg3 = "__SUBMIT_ORDERS_OR_ANOTHER_PARALLEL_BACKTEST_PAUSED_QUOTE_PUMP__OR_PAUSE_PUMP_WHILE_BACKTEST_RUNNING_IMPLEMENTATION_DOESNT_DO_ITS_JOB";
					//Assembler.PopupException(msg3, null, false);
				}

				Stopwatch waitedFor = new Stopwatch();
				waitedFor.Start();
				bool unpaused = base.StreamingAdapter.DataSource.PumpingWaitUntilUnpaused(order.Alert.Bars, 120000);
				waitedFor.Stop();
				string waited = "__WAITED_TOO_LONG_FOR_UNPAUSE_CONFIRMATION[" + waitedFor.ElapsedMilliseconds + "]ms ";
				if (waitedFor.ElapsedMilliseconds > 1000) {
					string msg2 = "I_WISH_I_KNEW_WHO_TOOK_THAT_LONG_TO_KEEP_QUOTE_PUMP_PAUSED";
					Assembler.PopupException(waited + msg2);
				} else {
					string msg4 = "NICE_CATCH__DONT_INVOKE_ORDER_UPDATE_BEFORE_POSITION_WAS_REGISTERED_IN_OrderProcessor.DataSnapshot.OrdersPending.ScanRecentForGUID()";
					//Assembler.PopupException(waited + msg4, null, false);
				}
			}

			this.QuikTerminal.SendTransactionOrderAsync(opBuySell, typeMarketLimitStop,
				order.Alert.Symbol, order.Alert.SymbolClass,
				priceFill, (int)order.QtyRequested, order.GUID,
				out sernoSessionFromTerminal, out msgSubmittedFromTerminal, out orderStateFromTerminalMustGetSubmitted);

			msg = msgSubmittedFromTerminal + "order.SernoSession[" + order.SernoSession + "]=>[" + sernoSessionFromTerminal + "] ";
			order.SernoSession = sernoSessionFromTerminal;

			OrderStateMessage newState = new OrderStateMessage(order, orderStateFromTerminalMustGetSubmitted, msg + msig);
			base.OrderProcessor.UpdateOrderStateAndPostProcess(order, newState);
		}
	}
}