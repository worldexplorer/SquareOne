using Sq1.Core.Sequencing;

using Newtonsoft.Json;

namespace Sq1.Core.StrategyBase {
	public class KPIs {
		[JsonProperty]	public	string	ReasonToExist		{ get; protected set; }

		[JsonProperty]	public	double	PositionsCount		;//CORRELATOR_WRITES_DIRECTLY{ get; protected set; }
		[JsonProperty]	public	double	PositionAvgProfit	;//CORRELATOR_WRITES_DIRECTLY{ get; protected set; }
		[JsonProperty]	public	double	NetProfit			;//CORRELATOR_WRITES_DIRECTLY{ get; protected set; }
		[JsonProperty]	public	double	WinLossRatio		;//CORRELATOR_WRITES_DIRECTLY{ get; protected set; }
		[JsonProperty]	public	double	ProfitFactor		;//CORRELATOR_WRITES_DIRECTLY{ get; protected set; }
		[JsonProperty]	public	double	RecoveryFactor		;//CORRELATOR_WRITES_DIRECTLY{ get; protected set; }
		[JsonProperty]	public	double	MaxDrawDown			;//CORRELATOR_WRITES_DIRECTLY{ get; protected set; }
		[JsonProperty]	public	double	MaxConsecWinners	;//CORRELATOR_WRITES_DIRECTLY{ get; protected set; }
		[JsonProperty]	public	double	MaxConsecLosers		;//CORRELATOR_WRITES_DIRECTLY{ get; protected set; }

		public KPIs() {
			string msg = "IM_INVOKED_DURING_DESERIALIZATION";
			this.ReasonToExist = "NOT_INITIALIZED";
		}
		public KPIs(string reasonToExist) : this() {
			this.ReasonToExist		= reasonToExist;
		}

		public KPIs(string reasonToExist, int positionsCountBoth, double netProfitForClosedPositionsBoth
					, double winLossRatio, double profitFactor, double recoveryFactor
					, double maxDrawDown, double maxConsecWinners, double maxConsecLosers) : this(reasonToExist) {
			this.PositionsCount		= positionsCountBoth;
			this.NetProfit			= netProfitForClosedPositionsBoth;
			if (this.PositionsCount != 0) {
				this.PositionAvgProfit = this.NetProfit / this.PositionsCount;
			} else {
				string msg = "WHEN_YOU_HAVE_TWO_MAs_SAME_PERIOD_THERE_WILL_BE_NO_POSITIONS_AND_ALL_NANs_INFINITIES_AND_ZEROES";
			}
			this.WinLossRatio		= winLossRatio;
			this.ProfitFactor		= profitFactor;
			this.RecoveryFactor		= recoveryFactor;
			this.MaxDrawDown		= maxDrawDown;
			this.MaxConsecWinners	= maxConsecWinners;
			this.MaxConsecLosers	= maxConsecLosers;
		}
		internal void AddKPIs(KPIs kpis) {
			if (this.PositionsCount < 0) {
				string msg = "HOW_IS_IT_POSSIBLE this.PositionsCount[" + this.PositionsCount + "]";
				Assembler.PopupException(msg, null, false);
			}
			if (this.PositionsCount < 0) {
				string msg = "HOW_IS_IT_POSSIBLE kpis.PositionsCount[" + kpis.PositionsCount + "]";
				Assembler.PopupException(msg, null, false);
			}
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
		internal void AddKPIs(SystemPerformanceRestoreAble anotherRun) {
			this.PositionsCount		+= anotherRun.PositionsCount;
			if (anotherRun.PositionsCount < 0) {
				string msg = "HOW_IS_IT_POSSIBLE anotherRun.PositionsCount[" + anotherRun.PositionsCount + "]";
				Assembler.PopupException(msg, null, false);
			}
			this.PositionAvgProfit	+= anotherRun.PositionAvgProfit;
			this.NetProfit			+= anotherRun.NetProfit;

			if (double.IsNaN(anotherRun.WinLossRatio) || double.IsInfinity(anotherRun.WinLossRatio) || double.IsNegativeInfinity(anotherRun.WinLossRatio)) {
				string msg = "IGNORED_KPIs_MAY_DISTORT_anotherRun.WinLossRatio[" + anotherRun.WinLossRatio + "]";
				//Assembler.PopupException(msg);
			} else {
				this.WinLossRatio	+= anotherRun.WinLossRatio;
			}

			if (double.IsNaN(anotherRun.ProfitFactor) || double.IsInfinity(anotherRun.ProfitFactor) || double.IsNegativeInfinity(anotherRun.ProfitFactor)) {
				string msg = "IGNORED_KPIs_MAY_DISTORT_anotherRun.ProfitFactor[" + anotherRun.ProfitFactor + "]";
				//Assembler.PopupException(msg);
			} else {
				this.ProfitFactor	+= anotherRun.ProfitFactor;
			}

			if (double.IsNaN(anotherRun.RecoveryFactor) || double.IsInfinity(anotherRun.RecoveryFactor) || double.IsNegativeInfinity(anotherRun.RecoveryFactor)) {
				string msg = "IGNORED_KPIs_MAY_DISTORT_anotherRun.RecoveryFactor[" + anotherRun.RecoveryFactor + "]";
				//Assembler.PopupException(msg);
			} else {
				this.RecoveryFactor += anotherRun.RecoveryFactor;
			}

			this.MaxDrawDown		+= anotherRun.MaxDrawDown;
			this.MaxConsecWinners	+= anotherRun.MaxConsecWinners;
			this.MaxConsecLosers	+= anotherRun.MaxConsecLosers;
		}
		public void Reset() {
			this.PositionsCount		= 0;
			this.PositionAvgProfit	= 0;
			this.NetProfit			= 0;
			this.WinLossRatio		= 0;
			this.ProfitFactor		= 0;
			this.RecoveryFactor		= 0;
			this.MaxDrawDown		= 0;
			this.MaxConsecWinners	= 0;
			this.MaxConsecLosers	= 0;
		}
		protected void AbsorbFrom(KPIs finalForSubset) {
			this.PositionsCount			= finalForSubset.PositionsCount;
			this.PositionAvgProfit		= finalForSubset.PositionAvgProfit;
			this.NetProfit				= finalForSubset.NetProfit;
			this.WinLossRatio			= finalForSubset.WinLossRatio;
			this.ProfitFactor			= finalForSubset.ProfitFactor;
			this.RecoveryFactor			= finalForSubset.RecoveryFactor;
			this.MaxDrawDown			= finalForSubset.MaxDrawDown;
			this.MaxConsecWinners		= finalForSubset.MaxConsecWinners;
			this.MaxConsecLosers		= finalForSubset.MaxConsecLosers;
		}
		public void Invert() {
			this.PositionsCount			*= -1;
			this.PositionAvgProfit		*= -1;
			this.NetProfit				*= -1;
			this.WinLossRatio			*= -1;
			this.ProfitFactor			*= -1;
			this.RecoveryFactor			*= -1;
			this.MaxDrawDown			*= -1;
			this.MaxConsecWinners		*= -1;
			this.MaxConsecLosers		*= -1;
		}
		public KPIs Clone() {
			return (KPIs)this.MemberwiseClone();
		}
		public override string ToString() {
			string netFormatted = this.NetProfit.ToString("N2").Replace(",", "");	//copypaste from NetProfitRecoveryForScriptContextNewName
			string ret = this.ReasonToExist + ":Net[" + netFormatted + "]"
				+ " PF[" + this.ProfitFactor.ToString("N2") + "]"
				+ " RF[" + this.RecoveryFactor.ToString("N2") + "]";
			return ret;

		}
	}
}
