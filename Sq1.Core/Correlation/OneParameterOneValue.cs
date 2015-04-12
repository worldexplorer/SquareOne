using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.Sequencing;

namespace Sq1.Core.Correlation {
	public class OneParameterOneValue {
		OneParameterAllValuesAveraged oneParameterAllValuesAveraged;

		public string	ArtificialName		{ get; private set; }
		public bool		IsArtificialRow		{ get { return string.IsNullOrEmpty(this.ArtificialName) == false; } }

		public double	ValueSequenced		{ get; private set; }
		public string	ParameterName		{ get { return this.oneParameterAllValuesAveraged.ParameterName; } }
		public string	ParameterNameValue	{ get {
				if (this.IsArtificialRow) return this.ArtificialName;			//"Average" / "Dispersion""
				string ret = this.oneParameterAllValuesAveraged.ParameterName + "=" + this.ValueSequenced;	//"MaFast.Period=10"
				return ret;
			} }


		SortedDictionary<int, SystemPerformanceRestoreAble> backtestsWithMyValue;

		SortedDictionary<int, SystemPerformanceRestoreAble> backtestsWithMyValueAndOnlyChosenOtherValues_cached;
		SortedDictionary<int, SystemPerformanceRestoreAble> backtestsWithMyValueAndOnlyChosenOtherValues { get {
			if (this.backtestsWithMyValueAndOnlyChosenOtherValues_cached != null) return this.backtestsWithMyValueAndOnlyChosenOtherValues_cached;
			this.backtestsWithMyValueAndOnlyChosenOtherValues_cached = new SortedDictionary<int, SystemPerformanceRestoreAble>();
			foreach (SystemPerformanceRestoreAble eachRegardless in this.backtestsWithMyValue.Values) {
				if (oneParameterAllValuesAveraged.Sequencer.HasUnchosenParametersExceptFor(eachRegardless, this.oneParameterAllValuesAveraged)) continue;
				backtestsWithMyValueAndOnlyChosenOtherValues_cached.Add(eachRegardless.OptimizationIterationSerno, eachRegardless);
			}
			return this.backtestsWithMyValueAndOnlyChosenOtherValues_cached;
		} }

		public KPIs KPIsGlobal	{ get; private set; }
		public KPIs KPIsLocal	{ get; private set; }
		public bool Chosen;

		KPIs kPIsLocalMinusGlobal_cached;
		public KPIs KPIsDelta { get {
				if (this.kPIsLocalMinusGlobal_cached != null) return this.kPIsLocalMinusGlobal_cached;
				KPIs ret = new KPIs();
				ret.NetProfit			= this.KPIsLocal.NetProfit			- this.KPIsGlobal.NetProfit;
				ret.PositionsCount		= this.KPIsLocal.PositionsCount		- this.KPIsGlobal.PositionsCount;
				ret.PositionAvgProfit	= this.KPIsLocal.PositionAvgProfit	- this.KPIsGlobal.PositionAvgProfit;
				ret.WinLossRatio		= this.KPIsLocal.WinLossRatio		- this.KPIsGlobal.WinLossRatio;
				ret.ProfitFactor		= this.KPIsLocal.ProfitFactor		- this.KPIsGlobal.ProfitFactor;
				ret.RecoveryFactor		= this.KPIsLocal.RecoveryFactor		- this.KPIsGlobal.RecoveryFactor;
				ret.MaxDrawDown			= this.KPIsLocal.MaxDrawDown		- this.KPIsGlobal.MaxDrawDown;
				ret.MaxConsecWinners	= this.KPIsLocal.MaxConsecWinners	- this.KPIsGlobal.MaxConsecWinners;
				ret.MaxConsecLosers		= this.KPIsLocal.MaxConsecLosers	- this.KPIsGlobal.MaxConsecLosers;
				this.kPIsLocalMinusGlobal_cached = ret;
				return ret;
			} }

		OneParameterOneValue() {
			Chosen = true;
			backtestsWithMyValue = new SortedDictionary<int, SystemPerformanceRestoreAble>();
			KPIsGlobal = new KPIs();
			KPIsLocal = new KPIs();
		}

		public OneParameterOneValue(OneParameterAllValuesAveraged oneParameterAllValuesAveraged
				, double optimizedValue, string artificialName = null) : this() {
			this.oneParameterAllValuesAveraged = oneParameterAllValuesAveraged;
			this.ValueSequenced = optimizedValue;
			this.ArtificialName = artificialName;
		}

		internal void AddBacktestForValue_AddKPIsGlobal(SystemPerformanceRestoreAble eachRun) {
			if (this.backtestsWithMyValue.ContainsKey(eachRun.OptimizationIterationSerno)) {
				string msg = "DUPLICATE";
				Assembler.PopupException(msg);
			}
			this.backtestsWithMyValue.Add(eachRun.OptimizationIterationSerno, eachRun);
			this.KPIsGlobal.AddKPIs(eachRun);
		}

		internal void KPIsGlobal_DivideTotalsByCount() {
			int noDivisionToZero = this.backtestsWithMyValue.Count;
			if (noDivisionToZero == 0) return;
			KPIsGlobal.DivideTotalByCount(noDivisionToZero);
			KPIsLocal = KPIsGlobal.Clone();
		}

		internal void CalculateLocalsAndDeltas() {
			if (this.IsArtificialRow) {
				string msg = "USE_CalculateLocalsAndDeltasForArtificial()_INSTEAD";
				Assembler.PopupException(msg);
				return;
			}

			this.backtestsWithMyValueAndOnlyChosenOtherValues_cached = null;
			this.KPIsLocal.Reset();
			foreach (SystemPerformanceRestoreAble eachChosen in this.backtestsWithMyValueAndOnlyChosenOtherValues.Values) {
				this.KPIsLocal.AddKPIs(eachChosen);
			}

			int noDivisionToZero = this.backtestsWithMyValueAndOnlyChosenOtherValues.Count;
			if (noDivisionToZero == 0) {
				//Assembler.PopupException("AVOIDING_DIVISION_BY_ZERO");
				return;
			}
			this.KPIsLocal.DivideTotalByCount(noDivisionToZero);
			this.kPIsLocalMinusGlobal_cached = null;
		}

		internal void CalculateLocalsAndDeltasForArtificial_Average() {
			this.KPIsLocal.Reset();

			List<OneParameterOneValue> parentValues = new List<OneParameterOneValue>(this.oneParameterAllValuesAveraged.ValuesByParam.Values);
			foreach (OneParameterOneValue kpisForValue in parentValues) {
				this.KPIsLocal.AddKPIs(kpisForValue.KPIsLocal);
			}
			int noDivisionToZero = parentValues.Count;
			if (noDivisionToZero == 0) {
				Assembler.PopupException("AVOIDING_DIVISION_BY_ZERO");
				return;
			}
			this.KPIsLocal.DivideTotalByCount(noDivisionToZero);
			this.kPIsLocalMinusGlobal_cached = null;
		}
		internal void CalculateGlobalsForArtificial_Average() {
			this.KPIsGlobal.Reset();

			List<OneParameterOneValue> parentValues = new List<OneParameterOneValue>(this.oneParameterAllValuesAveraged.ValuesByParam.Values);
			foreach (OneParameterOneValue kpisForValue in parentValues) {
				this.KPIsGlobal.AddKPIs(kpisForValue.KPIsGlobal);
			}
			int noDivisionToZero = parentValues.Count;
			if (noDivisionToZero == 0) {
				Assembler.PopupException("AVOIDING_DIVISION_BY_ZERO");
				return;
			}
			this.KPIsGlobal.DivideTotalByCount(noDivisionToZero);
		}

		public override string ToString() {
			string ret = this.ParameterNameValue + ":";
			if (this.IsArtificialRow) {
				ret += this.oneParameterAllValuesAveraged.ValuesByParam.Count + "paramValues";
			} else {
				ret += this.backtestsWithMyValue.Count + "backtests";
			}
			return ret;
		}

		internal void CalculateGlobalsForArtificial_Kurtsotis() {
		}

		internal void CalculateGlobalsForArtificial_Dispersion() {
		}

		internal void CalculateLocalsAndDeltasForArtificial_Dispersion() {
		}

		internal void CalculateLocalsAndDeltasForArtificial_Kurtsotis() {
		}
	}
}
