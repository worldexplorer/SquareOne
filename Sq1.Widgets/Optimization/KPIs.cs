using Sq1.Core.DataFeed;

using Sq1.Core.Optimization;

namespace Sq1.Widgets.Optimization {
	public class KPIs {
		public string Format;

		public double NetProfit;
		public string NetProfitFormatted;

		public double PositionsCount;
		public string PositionsCountFormatted;

		public double PositionAvgProfit;
		public string PositionAvgProfitFormatted;

		public double WinLossRatio;
		public string WinLossRatioFormatted;

		public double ProfitFactor;
		public string ProfitFactorFormatted;

		public double RecoveryFactor;
		public string RecoveryFactorFormatted;

		public double MaxDrawDown;
		public string MaxDrawDownFormatted;

		public double MaxConsecWinners;
		public string MaxConsecWinnersFormatted;

		public double MaxConsecLosers;
		public string MaxConsecLosersFormatted;

		public KPIs(SystemPerformanceRestoreAble firstRun) {
			this.Format = firstRun.Format;

			this.NetProfit = firstRun.NetProfitForClosedPositionsBoth;
			this.PositionsCount = firstRun.PositionsCountBoth;
			this.PositionAvgProfit = firstRun.AvgProfitBoth;
			this.WinLossRatio = firstRun.WinLossRatio;
			this.ProfitFactor = firstRun.ProfitFactor;
			this.RecoveryFactor = firstRun.RecoveryFactor;
			this.MaxDrawDown = firstRun.MaxDrawDown;
			this.MaxConsecWinners = firstRun.MaxConsecWinners;
			this.MaxConsecLosers = firstRun.MaxConsecLosers;
		}

		internal void AddKPIs(SystemPerformanceRestoreAble anotherRun) {
			this.NetProfit += anotherRun.NetProfitForClosedPositionsBoth;
			this.PositionsCount += anotherRun.PositionsCountBoth;
			this.PositionAvgProfit += anotherRun.AvgProfitBoth;
			this.WinLossRatio += anotherRun.WinLossRatio;
			this.ProfitFactor += anotherRun.ProfitFactor;
			this.RecoveryFactor += anotherRun.RecoveryFactor;
			this.MaxDrawDown += anotherRun.MaxDrawDown;
			this.MaxConsecWinners += anotherRun.MaxConsecWinners;
			this.MaxConsecLosers += anotherRun.MaxConsecLosers;
		}

		internal void NoMoreParameters_DivideTotalByCount(int totalCountItotalled) {
			double dontRoundDivisionToInt = (double) totalCountItotalled;
			this.NetProfit /= dontRoundDivisionToInt;
			this.PositionsCount /= dontRoundDivisionToInt;
			this.PositionAvgProfit /= dontRoundDivisionToInt;
			this.WinLossRatio /= dontRoundDivisionToInt;
			this.ProfitFactor /= dontRoundDivisionToInt;
			this.RecoveryFactor /= dontRoundDivisionToInt;
			this.MaxDrawDown /= dontRoundDivisionToInt;
			this.MaxConsecWinners /= dontRoundDivisionToInt;
			this.MaxConsecLosers /= dontRoundDivisionToInt;

			this.FormatStrings();
		}
		public void FormatStrings() {
			this.NetProfitFormatted = this.NetProfit.ToString(this.Format);
			this.PositionsCountFormatted = this.PositionsCount.ToString();
			this.PositionAvgProfitFormatted = this.PositionAvgProfit.ToString(this.Format);
			this.WinLossRatioFormatted = this.WinLossRatio.ToString(this.Format);
			this.ProfitFactorFormatted = this.ProfitFactor.ToString();
			this.RecoveryFactorFormatted = this.RecoveryFactor.ToString();
			this.MaxDrawDownFormatted = this.MaxDrawDown.ToString(this.Format);
			this.MaxConsecLosersFormatted = this.MaxConsecLosers.ToString();
			this.MaxConsecWinnersFormatted = this.MaxConsecWinners.ToString();
		}
		public KPIs Clone() {
			return (KPIs)base.MemberwiseClone();
		}
	}
}
