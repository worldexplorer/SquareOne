using Sq1.Core.Sequencing;

namespace Sq1.Core.Correlation {
	public class KPIs {
		public double PositionsCount;
		public double PositionAvgProfit;
		public double NetProfit;
		public double WinLossRatio;
		public double ProfitFactor;
		public double RecoveryFactor;
		public double MaxDrawDown;
		public double MaxConsecWinners;
		public double MaxConsecLosers;

		public void Reset() {
			this.PositionsCount = 0;
			this.PositionAvgProfit = 0;
			this.NetProfit = 0;
			this.WinLossRatio = 0;
			this.ProfitFactor = 0;
			this.RecoveryFactor = 0;
			this.MaxDrawDown = 0;
			this.MaxConsecWinners = 0;
			this.MaxConsecLosers = 0;
		}

		public KPIs() {
		}

		internal void AddKPIs(SystemPerformanceRestoreAble anotherRun) {
			this.PositionsCount += anotherRun.PositionsCountBoth;
			this.PositionAvgProfit += anotherRun.AvgProfitBoth;
			this.NetProfit += anotherRun.NetProfitForClosedPositionsBoth;

			if (double.IsNaN(anotherRun.WinLossRatio) || double.IsInfinity(anotherRun.WinLossRatio) || double.IsNegativeInfinity(anotherRun.WinLossRatio)) {
				//System.Diagnostics.Debugger.Break();
			} else {
				this.WinLossRatio += anotherRun.WinLossRatio;
			}

			if (double.IsNaN(anotherRun.ProfitFactor) || double.IsInfinity(anotherRun.ProfitFactor) || double.IsNegativeInfinity(anotherRun.ProfitFactor)) {
				//System.Diagnostics.Debugger.Break();
			} else {
				this.ProfitFactor += anotherRun.ProfitFactor;
			}

			if (double.IsNaN(anotherRun.RecoveryFactor) || double.IsInfinity(anotherRun.RecoveryFactor) || double.IsNegativeInfinity(anotherRun.RecoveryFactor)) {
				//System.Diagnostics.Debugger.Break();
			} else {
				this.RecoveryFactor += anotherRun.RecoveryFactor;
			}

			this.MaxDrawDown += anotherRun.MaxDrawDown;
			this.MaxConsecWinners += anotherRun.MaxConsecWinners;
			this.MaxConsecLosers += anotherRun.MaxConsecLosers;
		}

		internal void DivideTotalByCount(int totalCountItotalled) {
			double dontRoundDivisionToInt = (double) totalCountItotalled;
			this.PositionsCount		/= dontRoundDivisionToInt;
			this.PositionAvgProfit	/= dontRoundDivisionToInt;
			this.NetProfit			/= dontRoundDivisionToInt;
			this.WinLossRatio		/= dontRoundDivisionToInt;
			this.ProfitFactor		/= dontRoundDivisionToInt;
			this.RecoveryFactor		/= dontRoundDivisionToInt;
			this.MaxDrawDown		/= dontRoundDivisionToInt;
			this.MaxConsecWinners	/= dontRoundDivisionToInt;
			this.MaxConsecLosers	/= dontRoundDivisionToInt;

			//if (double.IsNaN(this.WinLossRatio)) System.Diagnostics.Debugger.Break();
			//if (double.IsNaN(this.ProfitFactor)) System.Diagnostics.Debugger.Break();
			//if (double.IsNaN(this.RecoveryFactor)) System.Diagnostics.Debugger.Break();
		}
		public KPIs Clone() {
			return (KPIs)base.MemberwiseClone();
		}


		internal void AddKPIs(KPIs kpis) {
			this.PositionsCount		+= kpis.PositionsCount;
			this.PositionAvgProfit	+= kpis.PositionAvgProfit;
			this.NetProfit			+= kpis.NetProfit;
			this.WinLossRatio		+= kpis.WinLossRatio;
			this.ProfitFactor		+= kpis.ProfitFactor;
			this.RecoveryFactor		+= kpis.RecoveryFactor;
			this.MaxDrawDown		+= kpis.MaxDrawDown;
			this.MaxConsecWinners	+= kpis.MaxConsecWinners;
			this.MaxConsecLosers	+= kpis.MaxConsecLosers;
		}
	}
}
