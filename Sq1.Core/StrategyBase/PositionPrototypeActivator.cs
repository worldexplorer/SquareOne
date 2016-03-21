using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.Execution;
using Sq1.Core.DataTypes;

namespace Sq1.Core.StrategyBase {
	public partial class PositionPrototypeActivator {
		ScriptExecutor executor;
		public PositionPrototypeActivator(ScriptExecutor strategyExecutor) {
			this.executor = strategyExecutor;
		}

		public List<Alert> PositionActivator_entryPoint__alertFilled_createSlTp_orAnnihilateCounterparty(Alert alert) {
			List<Alert> ret = new List<Alert>();
			if (alert.PositionAffected == null) return ret;
			if (alert.PositionAffected.Prototype == null) return ret;
			if (alert.IsEntryAlert) {
				return this.CreateStopLossAndTakeProfitAlerts_fromPositionPrototype(alert.PositionAffected);
			} else {
				bool killed = this.AnnihilateCounterparty_forClosedPosition(alert.PositionAffected);
				return ret;
			}
		}

		public double StopLossCurrentGet_NaNunsafe(PositionPrototype proto) {
			if (proto.StopLoss_negativeOffset == 0) return double.NaN;
			Alert SLalert = proto.StopLossAlert_forMoveAndAnnihilation;
			if (SLalert == null) {
				string msg = "CHECK_UPSTACK_WHAT_LED_TO_proto.StopLossAlertForAnnihilation=NULL";
				Assembler.PopupException(msg);
				return double.NaN;
			}
			if (SLalert.MarketLimitStop != MarketLimitStop.Stop && SLalert.MarketLimitStop != MarketLimitStop.StopLimit) {
				string msg = "CHECK_UPSTACK_WHAT_LED_TO_SLalert.MarketLimitStop=" + SLalert.MarketLimitStop;
				Assembler.PopupException(msg);
				return double.NaN;
			}
			return SLalert.PriceScriptAligned;
		}
	}
}