using System;
using System.Diagnostics;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.StrategyBase {
	public class MarketLive {
		ScriptExecutor executor;

		public MarketLive(ScriptExecutor executor) {
			this.executor = executor;
		}

		public Alert EntryAlertCreate(Bar entryBar, double stopOrLimitPrice, string entrySignalName,
			Direction direction, MarketLimitStop entryMarketLimitStop) {

			this.checkThrowEntryBarIsValid(entryBar);

			double priceScriptOrStreaming = stopOrLimitPrice;
			OrderSpreadSide orderSpreadSide = OrderSpreadSide.Unknown;
			if (entryMarketLimitStop == MarketLimitStop.Market) {
				priceScriptOrStreaming = this.getStreamingPriceForMarketOrder(entryMarketLimitStop, direction, out orderSpreadSide);
			}

			// ALREADY_ALIGNED_AFTER GetAlignedBidOrAskForTidalOrCrossMarketFromStreaming
			//v2
			double entryPriceScript = entryBar.ParentBars.SymbolInfo.AlignAlertToPriceLevelSimplified(priceScriptOrStreaming, direction, entryMarketLimitStop);

			//#if DEBUG
			////v1
			//PositionLongShort longShortFromDirection = MarketConverter.LongShortFromDirection(direction);
			//double entryPriceScript1 = entryBar.ParentBars.SymbolInfo.AlignAlertToPriceLevel(priceScriptOrStreaming, true, longShortFromDirection, entryMarketLimitStop);
			//if (entryPriceScript1 != entryPriceScript) {
			//	string msg = "FIX_Alert.entryPriceScript";
			//	Debugger.Break();
			//} else {
			//	string msg = "GET_RID_OF_COMPLEX_ALIGNMENT executor.AlignAlertToPriceLevel()";
			//}
			//#endif
			

			double shares = this.executor.PositionSizeCalculate(entryBar, entryPriceScript);

			Alert alert = new Alert(entryBar, shares, entryPriceScript, entrySignalName,
				direction, entryMarketLimitStop, orderSpreadSide,
				//this.executor.Script,
				this.executor.Strategy);
			alert.AbsorbFromExecutorAfterCreatedByMarketReal(executor);

			#if DEBUG	// REMOVE_ONCE_NEW_ALIGNMENT_MATURES_NOVEMBER_15TH_2014
			if (entryPriceScript != alert.PriceScriptAligned) {
				string msg = "FIX_Alert.PriceScriptAligned";
				Debugger.Break();
			} else {
				string msg = "GET_RID_OF_COMPLEX_ALIGNMENT executor.AlignAlertPriceToPriceLevel()";
			}
			#endif

			return alert;
		}
		public Alert ExitAlertCreate(Bar exitBar, Position position, double stopOrLimitPrice, string signalName,
			Direction direction, MarketLimitStop exitMarketLimitStop) {

			this.checkThrowEntryBarIsValid(exitBar);
			this.checkThrowPositionToCloseIsValid(position);

			double priceScriptOrStreaming = stopOrLimitPrice;
			OrderSpreadSide orderSpreadSide = OrderSpreadSide.Unknown;
			if (exitMarketLimitStop == MarketLimitStop.Market) {
				priceScriptOrStreaming = this.getStreamingPriceForMarketOrder(exitMarketLimitStop, direction, out orderSpreadSide);
			}

			//v2
			double exitPriceScript = exitBar.ParentBars.SymbolInfo.AlignAlertToPriceLevelSimplified(priceScriptOrStreaming, direction, exitMarketLimitStop);

			//#if DEBUG
			////v1
			//PositionLongShort longShortFromDirection = MarketConverter.LongShortFromDirection(direction);
			//double exitPriceScript1 = exitBar.ParentBars.SymbolInfo.AlignAlertToPriceLevel(priceScriptOrStreaming, true, longShortFromDirection, exitMarketLimitStop);
			//if (exitPriceScript1 != exitPriceScript) {
			//	string msg = "FIX_Alert.entryPriceScript";
			//	Debugger.Break();
			//} else {
			//	string msg = "GET_RID_OF_COMPLEX_ALIGNMENT executor.AlignAlertToPriceLevel()";
			//}
			//#endif

	
			Alert alert = new Alert(exitBar, position.Shares, exitPriceScript, signalName,
				direction, exitMarketLimitStop, orderSpreadSide,
				//this.executor.Script,
				this.executor.Strategy);
			alert.AbsorbFromExecutorAfterCreatedByMarketReal(executor);
			alert.PositionAffected = position;
			// moved to CallbackAlertFilled - we can exit by TP or SL - and position has no clue which Alert was filled!!!
			//position.ExitCopyFromAlert(alert);
			alert.PositionAffected.ExitAlertAttach(alert);

			return alert;
		}

		private void checkThrowPositionToCloseIsValid(Position position) {
			if (position == null) {
				string msg = "position=null, can't close it!";
				throw new Exception(msg);
			}
			if (position.ExitNotFilledOrStreaming == false) {
				string msg = "position.Active=false, can't close it!";
				throw new Exception(msg);
			}
		}
		void checkThrowEntryBarIsValid(Bar entryBar) {
//			if (entryBar < this.executor.Bars.Count) {
//				string msg = "use MarketSim for Backtest! MarketRealTime is for entryBars >= this.executor.Bars.Count";
//				throw new Exception(msg);
//				//this.executor.ThrowPopup(new Exception(msg));
//
//			}
//
//			if (entryBar > this.executor.Bars.Count) {
//				string msg = "entryBar[" + entryBar + "] > Bars.Count[" + this.executor.Bars.Count + "]"
//					+ " for [" + this.executor.Bars + "]"
//					+ " Bars.StreamingBarSafeClone=[" + this.executor.Bars.StreamingBarCloneReadonly + "]"
//					+ "; can't open any other position but on StreamingBar; positions postponed for tomorrow NYI";
//				throw new Exception(msg);
//				//this.executor.ThrowPopup(new Exception(msg));
//			}
			if (entryBar == null) {
				string msg = "entryBar == null";
				throw new Exception(msg);
			}
			if (entryBar.ParentBars == null) {
				string msg = "entryBar.ParentBars == null";
				throw new Exception(msg);
			}
			if (entryBar.ParentBars.SymbolInfo == null) {
				string msg = "entryBar.ParentBars.SymbolInfo == null";
				throw new Exception(msg);
			}
		}
		double getStreamingPriceForMarketOrder(MarketLimitStop entryMarketLimitStop,
				Direction direction, out OrderSpreadSide priceSpreadSide) {
			double priceForMarketAlert = -1;
			priceSpreadSide = OrderSpreadSide.Unknown;
			switch (entryMarketLimitStop) {
				case MarketLimitStop.Market:
					priceForMarketAlert = this.executor.DataSource.StreamingAdapter.StreamingDataSnapshot
						.GetAlignedBidOrAskForTidalOrCrossMarketFromStreaming(
							this.executor.Bars.Symbol, direction, out priceSpreadSide, false);
					break;
				case MarketLimitStop.AtClose:
					string msg = "[" + direction + "]At[" + entryMarketLimitStop + "]"
						+ " when LastBar[" + (this.executor.Bars.Count - 1) + "]; No way I can bring you a future price,"
						+ " even by executing your order right now"
						+ "; can't do inequivalent repacement to LastBar.Close";
					throw new Exception(msg);
					//break;
				case MarketLimitStop.Stop:
				case MarketLimitStop.Limit:
				case MarketLimitStop.StopLimit:
					string msg2 = "STREAMING_BID_ASK_IS_IRRELEVANT_FOR_STOP_OR_LIMIT_ALERT [" + direction + "]At[" + entryMarketLimitStop + "]"
						+ " when LastBar[" + (this.executor.Bars.Count - 1) + "]";
					throw new Exception(msg2);
				default:
					throw new Exception("no handler for MarketLimitStop.[" + entryMarketLimitStop + "]");
			}
			return priceForMarketAlert;
		}

		internal bool AnnihilateCounterpartyAlert(Alert alert) {
			if (alert.OrderFollowed == null) {
				string msg = "can't AnnihilateCounterparty: OrderFollowed=null for alert=[" + alert + "]";
				throw new Exception(msg);
				//this.executor.ThrowPopup(new Exception(msg));
			}
			if (alert.PositionAffected == null) {
				string msg = "can't AnnihilateCounterparty: PositionAffected=null for alert=[" + alert + "]";
				throw new Exception(msg);
				//this.executor.ThrowPopup(new Exception(msg));
			}
			if (alert.IsEntryAlert) {
				string msg = "can't AnnihilateCounterparty: alert.isEntryAlert for alert=[" + alert + "]";
				throw new Exception(msg);
				//this.executor.ThrowPopup(new Exception(msg));
			}

			OrderStateMessage newOrderState = null;
			if (alert.PositionAffected.ClosedByTakeProfitLogically) {	//copy from dispatcher (caller of a caller)
				string msg = "position ClosedByTakeProfit@" + alert.PriceScript + ", annihilating StopLoss";
				newOrderState = new OrderStateMessage(alert.OrderFollowed, OrderState.SLAnnihilated, msg);
			} else {
				string msg = "position ClosedByStopLoss@" + alert.PriceScript + ", annihilating TakeProfit";
				newOrderState = new OrderStateMessage(alert.OrderFollowed, OrderState.TPAnnihilated, msg);
			}
			executor.OrderProcessor.UpdateOrderStateDontPostProcess(alert.OrderFollowed, newOrderState);
			executor.OrderProcessor.KillPendingOrderWithoutKiller(alert.OrderFollowed);

			//string msg2 = "MARKET_LIVE_ASSUMES_A_CALLBACK_REMOVES_FROM_DATASNAPSHOT_AFTER_BROKER_SAYS_YES_HE_KILLED_PENDING";
			//Assembler.PopupException(msg2);
			//this.executor.RemovePendingExitAlert(alert, "MarketRealtime:AnnihilateCounterparty(): ");
			//bool removed = this.executor.ExecutionDataSnapshot.AlertsPending.Remove(alert, this, "AnnihilateCounterpartyAlert(WAIT)");
			return true;
		}
		public bool AlertKillPending(Alert alert) {
			string msg = "AlertKillPending";
			//OrderStateMessage newOrderState = new OrderStateMessage(alert.OrderFollowed, OrderState.KillerSubmitting, msg);
			executor.OrderProcessor.KillPendingOrderWithoutKiller(alert.OrderFollowed);
			return false;
		}
		
		public bool AlertTryFillUsingBacktest(Alert alert, out bool abortTryFill, out string abortTryFillReason) {
			abortTryFill = false;
			abortTryFillReason = "NO_REASON_TO_ABORT_TRY_FILL";
			if (this.executor.Backtester.IsBacktestingNoLivesimNow) {
				string msg = "WAIT_UNTIL_QUOTE_PUMP_RESUMES__DONT_INVOKE_ME_DURING_THE_BACKTEST AlertTryFillUsingBacktest() was designed for Sq1.Adapters.QuikMock.Terminal.QuikTerminalMock";
				Assembler.PopupException(msg, null, false);
				return false;
			}
			if (alert.DataSource.BrokerAdapterName.Contains("Mock") == false) {
				string msg = "AlertTryFillUsingBacktest() should be called only from BrokerAdaptersName.Contains(Mock)"
					+ "; here you have MOCK Realtime Streaming and Broker,"
					+ " it's not a time-insensitive QuotesFromBar-generated Streaming Backtest"
					+ " (both are routed to here, MarketSim, hypothetical order execution)";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}

			if (alert.PositionAffected == null) {
				string msg = "alertToBeKilled always has a PositionAffected, even for OnChartManual Buy/Short Market/Stop/Limit";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}

			bool filled = false;
			Quote quoteLast = this.executor.DataSource.StreamingAdapter.StreamingDataSnapshot.LastQuoteCloneGetForSymbol(alert.Symbol).Clone();
			if (quoteLast == null) {
				string msg = "MAKE_YOUR_STREAMING_SAVE_LAST_QUOTE_TO_SNAP LastQuoteGetForSymbol("
					+ alert.Symbol + ")=null StreamingAdapter[" + this.executor.DataSource.StreamingAdapter + "]"
					+ " MARKET_SIM_NEEDS_QUOTE_TO_TRY_FILL_AND_SAY_IF_FILLED";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}

			quoteLast.SetParentBarStreaming(alert.Bars.BarStreamingNullUnsafe.Clone());
			if (quoteLast.ParentBarStreaming.ParentBarsIndex == -1) {
				string msg = "EARLY_BINDER_DIDNT_DO_ITS_JOB#1 quote.ParentStreamingBar.ParentBarsIndex=-1 ";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}

			if (alert.IsEntryAlert) {
				if (alert.PositionAffected.IsEntryFilled) {
					string msg = "WONT_FILL_ENTRY_TWICE PositionAffected.EntryFilled => did you create many threads in your QuikTerminalMock?";
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception(msg);
				}
				//filled = this.CheckEntryAlertWillBeFilledByQuote(alert, quote, out priceFill, out slippageFill);
				filled = this.executor.MarketsimBacktest.SimulateFillPendingAlert(alert, quoteLast);
			} else {
				if (alert.PositionAffected.IsEntryFilled == false) {
					string msg = "WONT_FILL_EXIT_TWICE I refuse to tryFill an ExitOrder because ExitOrder.Alert.PositionAffected.EntryFilled=false";
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception(msg);
				}
				if (alert.PositionAffected.IsExitFilled) {
					string msg = null;
					if (alert.PositionAffected.IsExitFilledWithPrototypedAlert) {
						msg = "ExitAlert already filled and Counterparty.Status=["
							+ alert.PositionAffected.PrototypedExitCounterpartyAlert.OrderFollowed.State + "] (does it look OK for you?)";
					} else {
						msg = "I refuse to tryFill non-Prototype based ExitOrder having PositionAffected.IsExitFilled=true";
					}
					abortTryFill = true;
					abortTryFillReason = msg;
					// abortTryFillReason goes to the order.Message inside the caller
					//this.executor.ThrowPopup(new Exception(msg));
					return false;
				}
				try {
					//filled = this.CheckExitAlertWillBeFilledByQuote(alert, quote, out priceFill, out slippageFill);
					filled = this.executor.MarketsimBacktest.SimulateFillPendingAlert(alert, quoteLast);
				} catch (Exception ex) {
					string msig = " //AlertTryFillUsingBacktest(" + alert + ", " + abortTryFill + ", " + abortTryFillReason + ")";
					string msg = "THROWN_INSIDE_executor.MarketsimBacktest.SimulateFillPendingAlert(" + alert + ", " + quoteLast + ")";
					Assembler.PopupException(msg + msig, ex);
				}
			}
			return filled;
		}
	}
}
