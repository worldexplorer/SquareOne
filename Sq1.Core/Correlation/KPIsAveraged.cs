using Sq1.Core.Sequencing;
using Sq1.Core.StrategyBase;
using System;

namespace Sq1.Core.Correlation {
	public class KPIsAveraged : KPIs {
		public const string KPIS_AVG_AVERAGE	= "KPIsAvgAverage";
		public const string KPIS_AVG_DISPERSION	= "KPIsAvgDispersion";
		public const string KPIS_AVG_VARIANCE	= "KPIsAvgVariance";

		public KPIsAveraged() : base() {
			string msg = "IM_INVOKED_DURING_DESERIALIZATION";
		}
		public KPIsAveraged(string reasonToExist) : base(reasonToExist) {
		}

		internal void DivideTotalByCount(int totalCountItotalled) {
			double dontRoundDivisionToInt = (double) totalCountItotalled;
			if (base.PositionsCount == 0) {
				string msg = "WHY?";
			}
			base.PositionsCount		/= dontRoundDivisionToInt;
			base.PositionAvgProfit	/= dontRoundDivisionToInt;
			base.NetProfit			/= dontRoundDivisionToInt;
			base.WinLossRatio		/= dontRoundDivisionToInt;
			base.ProfitFactor		/= dontRoundDivisionToInt;
			base.RecoveryFactor		/= dontRoundDivisionToInt;
			base.MaxDrawDown		/= dontRoundDivisionToInt;
			base.MaxConsecWinners	/= dontRoundDivisionToInt;
			base.MaxConsecLosers	/= dontRoundDivisionToInt;

			//if (double.IsNaN(base.WinLossRatio)) System.Diagnostics.Debugger.Break();
			//if (double.IsNaN(base.ProfitFactor)) System.Diagnostics.Debugger.Break();
			//if (double.IsNaN(base.RecoveryFactor)) System.Diagnostics.Debugger.Break();
		}
		public KPIsAveraged Clone() {
			return (KPIsAveraged)base.MemberwiseClone();
		}
	}
}
