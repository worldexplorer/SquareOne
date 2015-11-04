using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using Sq1.Core.Sequencing;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Correlation {
	public class KPIsMomentum : KPIsAveraged {
		public const string KPIS_MOMENTUM_AVERAGE			= "KPIsAvgAverage";
		public const string KPIS_MOMENTUM_DISPERSION_GLOBAL	= "KPIsAvgDispersionGlobal";
		public const string KPIS_MOMENTUM_DISPERSION_LOCAL	= "KPIsAvgDispersionLocal";

		[JsonIgnore]		List<double>	List_PositionsCount;
		[JsonIgnore]		List<double>	List_PositionAvgProfit;
		[JsonIgnore]		List<double>	List_NetProfit;
		[JsonIgnore]		List<double>	List_WinLossRatio;
		[JsonIgnore]		List<double>	List_ProfitFactor;
		[JsonIgnore]		List<double>	List_RecoveryFactor;
		[JsonIgnore]		List<double>	List_MaxDrawDown;
		[JsonIgnore]		List<double>	List_MaxConsecWinners;
		[JsonIgnore]		List<double>	List_MaxConsecLosers;

		KPIsMomentum() : base() {
			base.ReasonToExist = "KPIsMomentum()_INVOKED_DURING_DESERIALIZATION";
			this.Reset();
		}
		public KPIsMomentum(string reasonToExist) : base(reasonToExist) {
			this.Reset();
		}

		internal override void AddKPIs(KPIs anotherRun) {
			this.List_PositionsCount		.Add(anotherRun.PositionsCount);
			if (anotherRun.PositionsCount < 0) {
				string msg = "HOW_IS_IT_POSSIBLE anotherRun.PositionsCount[" + anotherRun.PositionsCount + "]";
				Assembler.PopupException(msg, null, false);
			}
			this.List_PositionAvgProfit	.Add(anotherRun.PositionAvgProfit);
			this.List_NetProfit			.Add(anotherRun.NetProfit);

			if (double.IsNaN(anotherRun.WinLossRatio) || double.IsInfinity(anotherRun.WinLossRatio) || double.IsNegativeInfinity(anotherRun.WinLossRatio)) {
				string msg = "IGNORED_KPIs_MAY_DISTORT_anotherRun.WinLossRatio[" + anotherRun.WinLossRatio + "]";
				//Assembler.PopupException(msg);
			} else {
				this.List_WinLossRatio	.Add(anotherRun.WinLossRatio);
			}

			if (double.IsNaN(anotherRun.ProfitFactor) || double.IsInfinity(anotherRun.ProfitFactor) || double.IsNegativeInfinity(anotherRun.ProfitFactor)) {
				string msg = "IGNORED_KPIs_MAY_DISTORT_anotherRun.ProfitFactor[" + anotherRun.ProfitFactor + "]";
				//Assembler.PopupException(msg);
			} else {
				this.List_ProfitFactor	.Add(anotherRun.ProfitFactor);
			}

			if (double.IsNaN(anotherRun.RecoveryFactor) || double.IsInfinity(anotherRun.RecoveryFactor) || double.IsNegativeInfinity(anotherRun.RecoveryFactor)) {
				string msg = "IGNORED_KPIs_MAY_DISTORT_anotherRun.RecoveryFactor[" + anotherRun.RecoveryFactor + "]";
				//Assembler.PopupException(msg);
			} else {
				this.List_RecoveryFactor .Add(anotherRun.RecoveryFactor);
			}

			this.List_MaxDrawDown		.Add(anotherRun.MaxDrawDown);
			this.List_MaxConsecWinners	.Add(anotherRun.MaxConsecWinners);
			this.List_MaxConsecLosers	.Add(anotherRun.MaxConsecLosers);
		}

		protected override void Finalize_allKPIsAdded() {
			base.PositionsCount		= this.List_PositionsCount		.StdDev();
			base.PositionAvgProfit	= this.List_PositionAvgProfit	.StdDev();
			base.NetProfit			= this.List_NetProfit			.StdDev();
			base.WinLossRatio		= this.List_WinLossRatio		.StdDev();
			base.ProfitFactor		= this.List_ProfitFactor		.StdDev();
			base.RecoveryFactor		= this.List_RecoveryFactor		.StdDev();
			base.MaxDrawDown		= this.List_MaxDrawDown			.StdDev();
			base.MaxConsecWinners	= this.List_MaxConsecWinners	.StdDev();
			base.MaxConsecLosers	= this.List_MaxConsecLosers		.StdDev();
		}
		public override void Reset() {
			this.List_PositionsCount	= new List<double>();
			this.List_PositionAvgProfit	= new List<double>();
			this.List_NetProfit			= new List<double>();
			this.List_WinLossRatio		= new List<double>();
			this.List_ProfitFactor		= new List<double>();
			this.List_RecoveryFactor	= new List<double>();
			this.List_MaxDrawDown		= new List<double>();
			this.List_MaxConsecWinners	= new List<double>();
			this.List_MaxConsecLosers	= new List<double>();
		}
		public new KPIsMomentum Clone() {
			return (KPIsMomentum)base.MemberwiseClone();
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
