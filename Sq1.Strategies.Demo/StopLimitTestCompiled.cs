using System;
using System.Diagnostics;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;
using Sq1.Core;

namespace Sq1.Strategies.Demo {
	public class StopLimitTestCompiled : Script {
		public override void InitializeBacktest() {
			//this.PadBars(0);
		}
		public override void OnNewQuoteOfStreamingBar_callback(Quote quoteNewArrived) {
			this.placePrototypeOncePositionClosed(quoteNewArrived.ParentBarStreaming);
		}
		public override void OnBarStaticLastFormed_whileStreamingBarWithOneQuoteAlreadyAppended_callback(Bar barNewStaticArrived) {
			//this.placePrototypeOncePositionClosed(barNewStaticArrived);
		}
		private void placePrototypeOncePositionClosed(Bar bar) {
			bool isBacktesting = this.Executor.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing;
			//WHATS_THE_DIFFERENCE? if (isBacktesting) return;

			if (bar.ParentBarsIndex == 138) {
				//Debugger.Break();
			}

			if (base.HasPositions_OpenNow) return;

			if (base.HasAlertsPending_flashyWhenReplacingUnfilled) {
				// only kill pending entries, but leave activated SL & TP for an open position UNTOUCHED !!!!
				ExecutorDataSnapshot snap = this.Executor.ExecutionDataSnapshot;
				List<Alert> pendings = snap.AlertsPending_havingOrderFollowed_notYetFilled.SafeCopy(this, "placePrototypeOncePositionClosed(WAIT)");
				if (pendings.Count > 0) {
					string msg = pendings.Count + " last AlertsPending[" + snap.AlertsPending_havingOrderFollowed_notYetFilled.Last_nullUnsafe(this, "placePrototypeOncePositionClosed(WAIT)") + "]";
					//PrintDebug(msg);
					foreach (Alert alert in pendings) {
						int wasntFilledDuringPastNbars = bar.ParentBarsIndex - alert.PlacedBarIndex;
						if (wasntFilledDuringPastNbars >= 30) {
							//if (alert.PositionPrototype != null) {}
							//base.Executor.CallbackAlertKilledInvokeScript(alert);
							base.AlertPending_kill(alert);
						}
					}
				}
				return;
			}

			double protoPlacementOffsetPct = 1;
			double TPpct = 2;
			double SLpct = -1;
			double SLApct = -0.8;

			double protoPlacement = bar.Close + bar.Close * protoPlacementOffsetPct / 100;
			double TP = bar.Close * TPpct / 100;
			double SL = bar.Close * SLpct / 100;
			double SLactivation = bar.Close * SLApct / 100;
			SLactivation = 0;	// when SLactivation == 0 Prototype generates Stop alert instead of StopLoss

			PositionPrototype protoLong = new PositionPrototype(this.Bars.Symbol, PositionLongShort.Long, protoPlacement, TP, SL, SLactivation);
			PositionPrototype protoShort = new PositionPrototype(this.Bars.Symbol, PositionLongShort.Short, -protoPlacement, TP, SL, SLactivation);
			//PositionPrototype protoFixed = new PositionPrototype(this.Bars.Symbol, PositionLongShort.Long, 158000, +150.0, -50.0, -40.0);

			//PositionPrototype proto = barNewStaticArrived.Close < 158000 ? protoLong : protoShort;
			PositionPrototype proto = protoLong;
			base.Executor.PositionPrototypeActivator.PlaceOnce(proto);
		}
		public override void OnAlertFilled_callback(Alert alertFilled) {
			if (alertFilled.IsExitAlert) return;
			Position position = alertFilled.PositionAffected;
		}
		public override void OnAlertKilled_callback(Alert alertKilled) {
			//Debugger.Break();
		}
		public override void OnAlertNotSubmitted_callback(Alert alertNotSubmitted, int barNotSubmittedRelno) {
			string msig = " //OnAlertNotSubmittedCallback(" + alertNotSubmitted + ", " + barNotSubmittedRelno + ")";
			Assembler.PopupException("NEVER_HAPPENED_SO_FAR " + msig);
		}
		public override void OnPositionOpened_prototypeSlTpPlaced_callback(Position positionOpenedProto) {
			PositionPrototype proto = positionOpenedProto.Prototype;
			if (proto == null) return;

			double currentStopLossNegativeOffset = proto.StopLoss_negativeOffset;
			double newStopLossNegativeOffset = currentStopLossNegativeOffset - 20;
			//string msg = base.Executor.PositionPrototypeActivator.ReasonWhyNewStopLossOffsetDoesntMakeSense(positionOpenedProto, newStopLossNegativeOffset);
			//if (String.IsNullOrEmpty(msg)) {
				base.Executor.PositionPrototypeActivator.StopLoss_newNegativeOffset_updateActivate(positionOpenedProto, newStopLossNegativeOffset);
			//} else {
			//	base.Executor.PopupException(new Exception("WONT_UPDATE_STOPLOSS: " + msg));
			//}

			double newTakeProfitPositiveOffset = proto.TakeProfit_positiveOffset + 50;
			//msg = base.Executor.PositionPrototypeActivator.ReasonWhyNewTakeProfitOffsetDoesntMakeSense(positionOpenedProto, newTakeProfitPositiveOffset);
			//if (String.IsNullOrEmpty(msg)) {
				base.Executor.PositionPrototypeActivator.TakeProfit_newPositiveOffset_updateActivate(positionOpenedProto, newTakeProfitPositiveOffset);
			//} else {
			//	base.Executor.PopupException(new Exception("WONT_UPDATE_TAKEPROFIT: " + msg));
			//}
		}
		public override void OnPositionClosed_callback(Position positionClosed) {
			//Debugger.Break();
		}
		public override void OnPositionOpened_callback(Position positionOpened) {
			string msg = " NEVER_INVOKED_SINCE_I_USE_POSITION_PROTOTYPES_ONLY no direct BuyAt* or SellAt*";
			Assembler.PopupException(msg);
		}
	}
}