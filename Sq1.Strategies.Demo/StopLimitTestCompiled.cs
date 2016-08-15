using System;
using System.Diagnostics;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;

namespace Sq1.Strategies.Demo {
	public class StopLimitTestCompiled : Script {
		ScriptParameter					protoPlacementOffsetPrm;
		ScriptParameter					TPprm;
		ScriptParameter					SLprm;
		ScriptParameter					SLApips;
		ScriptParameter					Short0_Long1;

		public StopLimitTestCompiled() {
			protoPlacementOffsetPrm = new ScriptParameter(1, "protoPlacementOffsetPrm", 1, -10, 10, 0.5,	"Current Bid/Ask => PriceEntry offset, promiles");
			TPprm					= new ScriptParameter(2, "TPprm",					5, 1, 20, 0.5,		"PriceEntry(Limit) => TakeProfit(Limit) offset, promiles");
			SLprm					= new ScriptParameter(3, "SLprm",					-1, -1, -10, 0.5,	"PriceEntry(Limit) => StopLoss(Limit) target offset, promiles");
			SLApips					= new ScriptParameter(4, "SLApips",					-10, -10, -50, 10,	"PriceEntry(Limit) => StopLoss(Stop) activation offset, priceUnits");
			Short0_Long1			= new ScriptParameter(5, "Short0_Long1",			0, 0, 1, 1,			"Each Prototype will be Short(0) or Long (1)");

			this.TPprm	.RegisterValueConverter_meaningfulToStrategy(this.calculate_promileOfBarClose);	//,	this.format_forPriceValues);
			this.SLprm	.RegisterValueConverter_meaningfulToStrategy(this.calculate_promileOfBarClose);	//,	this.format_forPriceValues);
			this.SLApips.RegisterValueConverter_meaningfulToStrategy(this.calculate_pipsStraight);	//,		this.format_forPriceValues);
		}
		double calculate_promileOfBarClose(double TPprm_value) {
			double ret = 0;
			if (this.Bars != null && this.Bars.Count > 0) {
				ret = this.Bars.BarPreLast.Close * TPprm_value / 1000;
			}
			return ret;
		}
		double calculate_pipsStraight(double SLApips_value) {
			return SLApips_value;
		}
		//string format_forPriceValues() {
		//    return this.Bars.SymbolInfo.PriceFormat;
		//}

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

			double protoPlacementOffsetPrm_value = this.protoPlacementOffsetPrm.ValueCurrent;
			double TPprm_value = this.TPprm.ValueCurrent;
			double SLprm_value = this.SLprm.ValueCurrent;
			double SLApips_value = this.SLApips.ValueCurrent;

			double protoPlacement = 0;
			if (protoPlacementOffsetPrm_value > 0) {
				string msg = "LIMIT_ALERT_WILL_BE_CREATED_WITH_PRICE_IS_NONZERO PositionPrototypeActivator.PlaceOnce()";
				protoPlacement = bar.Close + bar.Close * protoPlacementOffsetPrm_value / 1000;
			} else {
				string msg = "MARKET_ALERT_WILL_BE_CREATED_WITH_PRICE_IS_ZERO PositionPrototypeActivator.PlaceOnce()";
			}
			//double TP = bar.Close * TPprm_value / 1000;
			//double SL = bar.Close * SLprm_value / 1000;
			//double SLactivation = bar.Close * SLApips_value;

			if (bar.Close != this.Bars.BarPreLast.Close) {
				string msg = "WRONG_LAST_BAR_WILL_BE_USED_IN_calculate_promileOfBarClose()";
				Assembler.PopupException(msg);
			}

			double TP = this.calculate_promileOfBarClose(TPprm_value);
			double SL = this.calculate_promileOfBarClose(SLprm_value);

			double SLactivation = SLApips_value;
			//double SLactivation = 0;	// when SLactivation == 0 Prototype generates Stop alert instead of StopLoss

			PositionPrototype protoLong  = new PositionPrototype(this.Bars.Symbol, PositionLongShort.Long,   protoPlacement, TP, SL, SLactivation);
			PositionPrototype protoShort = new PositionPrototype(this.Bars.Symbol, PositionLongShort.Short, -protoPlacement, TP, SL, SLactivation);
			//PositionPrototype protoFixed = new PositionPrototype(this.Bars.Symbol, PositionLongShort.Long, 158000, +150.0, -50.0, -40.0);

			//PositionPrototype proto = barNewStaticArrived.Close < 158000 ? protoLong : protoShort;
			PositionPrototype proto = this.Short0_Long1.ValueCurrentAsInteger == 0 ? protoShort : protoLong;
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


			double currentStopLossNegativeOffset = proto.StopLoss_priceEntryNegativeOffset;
			double newStopLossNegativeOffset = currentStopLossNegativeOffset - 20;
			//string msg = base.Executor.PositionPrototypeActivator.ReasonWhyNewStopLossOffsetDoesntMakeSense(positionOpenedProto, newStopLossNegativeOffset);
			//if (String.IsNullOrEmpty(msg)) {
				base.Executor.PositionPrototypeActivator.StopLoss_newNegativeOffset_updateActivate(positionOpenedProto, newStopLossNegativeOffset);
			//} else {
			//	base.Executor.PopupException(new Exception("WONT_UPDATE_STOPLOSS: " + msg));
			//}

			double newTakeProfitPositiveOffset = proto.TakeProfit_priceEntryPositiveOffset + 50;
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