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
					priceForMarketAlert = this.executor.DataSource.StreamingProvider.StreamingDataSnapshot
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
			executor.OrderProcessor.KillOrderUsingKillerOrder(alert.OrderFollowed);

			//this.executor.RemovePendingExitAlert(alert, "MarketRealtime:AnnihilateCounterparty(): ");
			bool removed = this.executor.ExecutionDataSnapshot.AlertsPendingRemove(alert);
			return true;
		}
		public bool AlertKillPending(Alert alert) {
			throw new NotImplementedException();
		}
	}
}