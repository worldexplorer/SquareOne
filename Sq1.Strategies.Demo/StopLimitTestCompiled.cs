using System;
using System.Diagnostics;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;

namespace Sq1.Strategies.Demo {
	public class StopLimitTestCompiled : Script {
		ScriptParameter					protoPlacementOffsetPct;
		ScriptParameter					TPpct;
		ScriptParameter					SLpct;
		//ScriptParameter					SLApct;
		ScriptParameter					Long0_Short1;

		public StopLimitTestCompiled() {
			protoPlacementOffsetPct = new ScriptParameter(1, "protoPlacementOffsetPct", 0.1, -1, 1, 0.05, "hopefully this will go to tooltip");
			TPpct					= new ScriptParameter(2, "TPpct",					0.5, 0.1, 2, 0.05, "hopefully this will go to tooltip");
			SLpct					= new ScriptParameter(3, "SLpct",					-0.1, -0.1, -1, 0.05, "hopefully this will go to tooltip");
			//SLApct					= new ScriptParameter(4, "SLApct",					-0.1, -0.8, -1, 0.1, "hopefully this will go to tooltip");
			Long0_Short1			= new ScriptParameter(5, "Long0_Short1",			0, 0, 1, 1, "hopefully this will go to tooltip");
		}

		public override void InitializeBacktest() {
			//this.PadBars(0);
		}
		public override void OnNewQuoteOfStreamingBar_callback(Quote quoteNewArrived) {
			//PUT_PROTOTYPE_ON_NEXT_BAR_NOT_THE_SAME_WHERE_THE_PREVIOUS_REACHED_SL_OR_TP
			//this.placePrototype_afterPositionClosed(quoteNewArrived.ParentBarStreaming);
		}
		public override void OnBarStaticLastFormed_whileStreamingBarWithOneQuoteAlreadyAppended_callback(Bar barNewStaticArrived) {
			this.placePrototype_afterPositionClosed(barNewStaticArrived);
		}
		void placePrototype_afterPositionClosed(Bar bar) {
			bool isBacktesting = this.Executor.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing;
			//WHATS_THE_DIFFERENCE? if (isBacktesting) return;

			if (bar.ParentBarsIndex == 138) {
				//Debugger.Break();
			}

			if (base.HasPositions_OpenNow) {
				string msg = "TEST_EOD_AUTOCLOSE_HERE OTHERWIZE_SL_OR_TP_WILL_GET_FILL";
				return;
			}

			if (base.HasAlertsUnfilled) {
				// only kill pending entries, but leave activated SL & TP for an open position UNTOUCHED !!!!
				ExecutorDataSnapshot snap = this.Executor.ExecutionDataSnapshot;
				List<Alert> pendings = snap.AlertsUnfilled.SafeCopy(this, "placePrototypeOncePositionClosed(WAIT)");
				if (pendings.Count > 0) {
					string msg = pendings.Count + " last AlertsPending[" + snap.AlertsUnfilled.Last_nullUnsafe(this, "placePrototypeOncePositionClosed(WAIT)") + "]";
					//PrintDebug(msg);
					foreach (Alert alert in pendings) {
						int wasntFilledDuringPastNbars = bar.ParentBarsIndex - alert.PlacedBarIndex;
						if (wasntFilledDuringPastNbars >= 30) {
							//if (alert.PositionPrototype != null) {}
							//base.Executor.CallbackAlertKilledInvokeScript(alert);
							//base.AlertPendingKill_appendToDoomed_inExecutorSnap(alert);
						}
					}
				}
				return;
			}

			//double protoPlacementOffsetPct_value = 1;
			//double TPpct_value = 2;
			//double SLpct_value = -1;
			//double SLApct_value = -0.8;

			double protoPlacementOffsetPct_value = protoPlacementOffsetPct.ValueCurrent;
			double TPpct_value = TPpct.ValueCurrent;
			double SLpct_value = SLpct.ValueCurrent;
			//double SLApct_value = SLApct.ValueCurrent;

			double protoPlacement = 0;
			if (protoPlacementOffsetPct_value > 0) {
				string msg = "LIMIT_ALERT_WILL_BE_CREATED_WITH_PRICE_IS_NONZERO PositionPrototypeActivator.PlaceOnce()";
				protoPlacement = bar.Close + bar.Close * protoPlacementOffsetPct_value / 100;
			} else {
				string msg = "MARKET_ALERT_WILL_BE_CREATED_WITH_PRICE_IS_ZERO PositionPrototypeActivator.PlaceOnce()";
			}
			double TP = bar.Close * TPpct_value / 100;
			double SL = bar.Close * SLpct_value / 100;
			//double SLactivation = bar.Close * SLApct_value / 100;
			double SLactivation = 0;	// when SLactivation == 0 Prototype generates Stop alert instead of StopLoss

			PositionPrototype protoLong = new PositionPrototype(this.Bars.Symbol, PositionLongShort.Long, protoPlacement, TP, SL, SLactivation);
			PositionPrototype protoShort = new PositionPrototype(this.Bars.Symbol, PositionLongShort.Short, -protoPlacement, TP, SL, SLactivation);
			//PositionPrototype protoFixed = new PositionPrototype(this.Bars.Symbol, PositionLongShort.Long, 158000, +150.0, -50.0, -40.0);

			//PositionPrototype proto = barNewStaticArrived.Close < 158000 ? protoLong : protoShort;
			PositionPrototype proto = this.Long0_Short1.ValueCurrentAsInteger == 0 ? protoLong : protoShort;
			base.Executor.PositionPrototypeActivator.PlaceOnce(proto);
		}
		public override void OnAlertFilled_callback(Alert alertFilled) {
			if (alertFilled.IsExitAlert) return;
			Position position = alertFilled.PositionAffected;
		}
		public override void OnAlertKilled_callback(Alert alertKilled) {
			//Debugger.Break();
		}
		public override void OnOrderReplaced_callback(Order orderKilled, Order orderReplacement) {
			//int ordersNumber_thatTried_toFillAlert = alertKilled.OrderFollowed == null ? 0 : 1;
			//ordersNumber_thatTried_toFillAlert += alertKilled.OrdersFollowed_killedAndReplaced.Count;
			string msg = "OnOrderReplaced_callback";		// ordersNumber_thatTried_toFillAlert[" + ordersNumber_thatTried_toFillAlert + "]";
			//Assembler.PopupException(msg, null, false);
		}
		public override void OnAlertNotSubmitted_callback(Alert alertNotSubmitted, int barNotSubmittedRelno) {
			string msig = " //OnAlertNotSubmittedCallback(" + alertNotSubmitted + ", " + barNotSubmittedRelno + ")";
			Assembler.PopupException("NEVER_HAPPENED_SO_FAR " + msig);
		}
		public override void OnPositionOpened_prototypeSlTpPlaced_callback(Position positionOpenedProto) {
			PositionPrototype proto = positionOpenedProto.Prototype;
			if (proto == null) return;

			string msg = "StopLoss_newNegativeOffset_updateActivate() STOP_LOSS_NOT_SUPPORTED";
			return;


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