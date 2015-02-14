using System;
using System.Diagnostics;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;

namespace Sq1.Strategies.Demo {
	public class StopLimitTestCompiled : Script {
		public override void InitializeBacktest() {
			//this.PadBars(0);
		}
		public override void OnNewQuoteOfStreamingBarCallback(Quote quoteNewArrived) {
			this.placePrototypeOncePositionClosed(quoteNewArrived.ParentBarStreaming);
		}
		public override void OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(Bar barNewStaticArrived) {
			//this.placePrototypeOncePositionClosed(barNewStaticArrived);
		}
		private void placePrototypeOncePositionClosed(Bar bar) {
			bool isBacktesting = this.Executor.Backtester.IsBacktestingNoLivesimNow;
			//WHATS_THE_DIFFERENCE? if (isBacktesting) return;

			if (bar.ParentBarsIndex == 138) {
				//Debugger.Break();
			}

			if (base.HasPositionsOpenNow) return;

			if (base.HasAlertsPending) {
				// only kill pending entries, but leave activated SL & TP for an open position UNTOUCHED !!!!
				ExecutionDataSnapshot snap = this.Executor.ExecutionDataSnapshot;
				List<Alert> pendings = snap.AlertsPending.InnerListSafeCopy;
				if (pendings.Count > 0) {
					string msg = pendings.Count + " last AlertsPending[" + snap.AlertsPending.InnerList[pendings.Count - 1] + "]";
					//PrintDebug(msg);
					foreach (Alert alert in pendings) {
						int wasntFilledDuringPastNbars = bar.ParentBarsIndex - alert.PlacedBarIndex;
						if (wasntFilledDuringPastNbars >= 30) {
							//if (alert.PositionAffected.Prototype != null) {}
							//base.Executor.CallbackAlertKilledInvokeScript(alert);
							base.AlertKillPending(alert);
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
		public override void OnAlertFilledCallback(Alert alertFilled) {
			if (alertFilled.IsExitAlert) return;
			Position position = alertFilled.PositionAffected;
		}
		public override void OnAlertKilledCallback(Alert alertKilled) {
			//Debugger.Break();
		}
		public override void OnAlertNotSubmittedCallback(Alert alertNotSubmitted, int barNotSubmittedRelno) {
			Debugger.Break();
		}
		public override void OnPositionOpenedPrototypeSlTpPlacedCallback(Position positionOpenedProto) {
			PositionPrototype proto = positionOpenedProto.Prototype;
			if (proto == null) return;

			double currentStopLossNegativeOffset = proto.StopLossNegativeOffset;
			double newStopLossNegativeOffset = currentStopLossNegativeOffset - 20;
			//string msg = base.Executor.PositionPrototypeActivator.ReasonWhyNewStopLossOffsetDoesntMakeSense(positionOpenedProto, newStopLossNegativeOffset);
			//if (String.IsNullOrEmpty(msg)) {
				base.Executor.PositionPrototypeActivator.StopLossNewNegativeOffsetUpdateActivate(positionOpenedProto, newStopLossNegativeOffset);
			//} else {
			//	base.Executor.PopupException(new Exception("WONT_UPDATE_STOPLOSS: " + msg));
			//}

			double newTakeProfitPositiveOffset = proto.TakeProfitPositiveOffset + 50;
			//msg = base.Executor.PositionPrototypeActivator.ReasonWhyNewTakeProfitOffsetDoesntMakeSense(positionOpenedProto, newTakeProfitPositiveOffset);
			//if (String.IsNullOrEmpty(msg)) {
				base.Executor.PositionPrototypeActivator.TakeProfitNewPositiveOffsetUpdateActivate(positionOpenedProto, newTakeProfitPositiveOffset);
			//} else {
			//	base.Executor.PopupException(new Exception("WONT_UPDATE_TAKEPROFIT: " + msg));
			//}
		}
		public override void OnPositionClosedCallback(Position positionClosed) {
			//Debugger.Break();
		}
		public override void OnPositionOpenedCallback(Position positionOpened) {
			// NEVER_INVOKED_SINCE_I_USE_POSITION_PROTOTYPES_ONLY no direct BuyAt* or SellAt*
			Debugger.Break();
		}
	}
}