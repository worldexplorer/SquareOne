using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using Sq1.Core.Sequencing;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Correlation {
	public class KPIsAveraged : KPIs {
		[JsonIgnore]	protected	double	TotalCount;			// divider must be double in C# otherwize truncated result
		[JsonProperty]	public		bool	IgnoreBacktestRunsWithZeroPositionCount		{ get; protected set; }	// each backtest is also a KPI; so ignoring starts making sense only for averaged and momentums

		[JsonIgnore]	double	cumulativePositionsCount;
		[JsonIgnore]	double	cumulativePositionAvgProfit;
		[JsonIgnore]	double	cumulativeNetProfit;
		[JsonIgnore]	double	cumulativeWinLossRatio;
		[JsonIgnore]	double	cumulativeProfitFactor;
		[JsonIgnore]	double	cumulativeRecoveryFactor;
		[JsonIgnore]	double	cumulativeMaxDrawDown;
		[JsonIgnore]	double	cumulativeMaxConsecWinners;
		[JsonIgnore]	double	cumulativeMaxConsecLosers;

		protected KPIsAveraged() : base() {
			base.ReasonToExist = "KPIsAveraged()_INVOKED_DURING_DESERIALIZATION";
		}
		public KPIsAveraged(string reasonToExist, bool ignoreBacktestRunsWithZeroPositionCount = true) : base(reasonToExist) {
			this.IgnoreBacktestRunsWithZeroPositionCount	= ignoreBacktestRunsWithZeroPositionCount;
		}

		internal void Reset_addBacktests_getMyMembersReady(SortedDictionary<int, SystemPerformanceRestoreAble> backtestsWithMyValue) {
			this.Reset_addKPIs_getMyMembersReady(new List<KPIs>(backtestsWithMyValue.Values));
		}
		internal void Reset_addBacktests_getMyMembersReady(List<SystemPerformanceRestoreAble> backtestsWithMyValue) {
			List<KPIs> KPIsList = new List<KPIs>();
			foreach (SystemPerformanceRestoreAble eachBacktest in backtestsWithMyValue) {
				KPIsList.Add(eachBacktest);
			}
			this.Reset_addKPIs_getMyMembersReady(KPIsList);
		}
		internal void Reset_addKPIs_getMyMembersReady(List<KPIs> backtestsWithMyValue) {
			this.Reset();

			int noDivisionToZero = backtestsWithMyValue.Count;
			if (noDivisionToZero == 0) {
				this.TotalCount = -1;
				return;
			}
			this.TotalCount = (double) noDivisionToZero;

			foreach (KPIs eachChosen in backtestsWithMyValue) {
				this.AddKPIs(eachChosen);
			}
			this.Finalize_allKPIsAdded();

			if (this.PositionsCount == 0) {
				string msg = "DONT_BLAME_YOURSELF__HAPPENS_WHEN_MaFast[14]&&MaSlow[14] => base.PositionsCount == 0 ?";
				//Assembler.PopupException(msg, null, false);
			}
		}
		internal override void AddKPIs(KPIs anotherRun) {
			if (this.IgnoreBacktestRunsWithZeroPositionCount && anotherRun.PositionsCount == 0) {
				string msg = "I_DIDNT_INCLUDE_BACKTEST_RUN_DUE_TO_ZERO_POSITIONS " + anotherRun;
				//Assembler.PopupException(msg, null, false);
				return;
			}
			this.cumulativePositionsCount		+= anotherRun.PositionsCount;
			if (anotherRun.PositionsCount < 0) {
				string msg = "HOW_IS_IT_POSSIBLE anotherRun.PositionsCount[" + anotherRun.PositionsCount + "]";
				Assembler.PopupException(msg, null, false);
			}
			this.cumulativePositionAvgProfit	+= anotherRun.PositionAvgProfit;
			this.cumulativeNetProfit			+= anotherRun.NetProfit;

			if (double.IsNaN(anotherRun.WinLossRatio) || double.IsInfinity(anotherRun.WinLossRatio) || double.IsNegativeInfinity(anotherRun.WinLossRatio)) {
				string msg = "IGNORED_KPIs_MAY_DISTORT_anotherRun.WinLossRatio[" + anotherRun.WinLossRatio + "]";
				//Assembler.PopupException(msg);
			} else {
				this.cumulativeWinLossRatio	+= anotherRun.WinLossRatio;
			}

			if (double.IsNaN(anotherRun.ProfitFactor) || double.IsInfinity(anotherRun.ProfitFactor) || double.IsNegativeInfinity(anotherRun.ProfitFactor)) {
				string msg = "IGNORED_KPIs_MAY_DISTORT_anotherRun.ProfitFactor[" + anotherRun.ProfitFactor + "]";
				//Assembler.PopupException(msg);
			} else {
				this.cumulativeProfitFactor	+= anotherRun.ProfitFactor;
			}

			if (double.IsNaN(anotherRun.RecoveryFactor) || double.IsInfinity(anotherRun.RecoveryFactor) || double.IsNegativeInfinity(anotherRun.RecoveryFactor)) {
				string msg = "IGNORED_KPIs_MAY_DISTORT_anotherRun.RecoveryFactor[" + anotherRun.RecoveryFactor + "]";
				//Assembler.PopupException(msg);
			} else {
				this.cumulativeRecoveryFactor += anotherRun.RecoveryFactor;
			}

			this.cumulativeMaxDrawDown		+= anotherRun.MaxDrawDown;
			this.cumulativeMaxConsecWinners	+= anotherRun.MaxConsecWinners;
			this.cumulativeMaxConsecLosers	+= anotherRun.MaxConsecLosers;
		}
		protected virtual void Finalize_allKPIsAdded() {
			base.PositionsCount		= this.cumulativePositionsCount		/ this.TotalCount;
			base.PositionAvgProfit	= this.cumulativePositionAvgProfit	/ this.TotalCount;
			base.NetProfit			= this.cumulativeNetProfit			/ this.TotalCount;
			base.WinLossRatio		= this.cumulativeWinLossRatio		/ this.TotalCount;
			base.ProfitFactor		= this.cumulativeProfitFactor		/ this.TotalCount;
			base.RecoveryFactor		= this.cumulativeRecoveryFactor		/ this.TotalCount;
			base.MaxDrawDown		= this.cumulativeMaxDrawDown		/ this.TotalCount;
			base.MaxConsecWinners	= this.cumulativeMaxConsecWinners	/ this.TotalCount;
			base.MaxConsecLosers	= this.cumulativeMaxConsecLosers	/ this.TotalCount;
		}
		public override void Reset() {
			this.cumulativePositionsCount		= 0;
			this.cumulativePositionAvgProfit	= 0;
			this.cumulativeNetProfit			= 0;
			this.cumulativeWinLossRatio			= 0;
			this.cumulativeProfitFactor			= 0;
			this.cumulativeRecoveryFactor		= 0;
			this.cumulativeMaxDrawDown			= 0;
			this.cumulativeMaxConsecWinners		= 0;
			this.cumulativeMaxConsecLosers		= 0;

			this.TotalCount						= -1;
		}
		public new KPIsAveraged Clone() {
			return (KPIsAveraged)base.MemberwiseClone();
		}
	}
}
