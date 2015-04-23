using System;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.Sequencing;

namespace Sq1.Core.Correlation {
	public class OneParameterOneValueForMomentums {
		OneParameterAllValuesAveraged oneParameterAllValuesAveraged;

		public double	ValueSequenced		{ get; private set; }
		public string	ParameterName		{ get { return this.oneParameterAllValuesAveraged.ParameterName; } }
		public string	ParameterNameValue	{ get {
				if (this.IsArtificialRow) return this.ArtificialName;			//"Average" / "Dispersion""
				string ret = this.oneParameterAllValuesAveraged.ParameterName + "=" + this.ValueSequenced;	//"MaFast.Period=10"
				return ret;
			} }


		SortedDictionary<int, SystemPerformanceRestoreAble> backtestsWithMyValue;
		int													BacktestsWithMyValueCount { get { return this.backtestsWithMyValue.Count; } }

		SortedDictionary<int, SystemPerformanceRestoreAble> backtestsWithMyValueAndOnlyChosenOtherValues_cached;
		SortedDictionary<int, SystemPerformanceRestoreAble> backtestsWithMyValueAndOnlyChosenOtherValues { get {
			if (this.backtestsWithMyValueAndOnlyChosenOtherValues_cached != null) return this.backtestsWithMyValueAndOnlyChosenOtherValues_cached;
			this.backtestsWithMyValueAndOnlyChosenOtherValues_cached = new SortedDictionary<int, SystemPerformanceRestoreAble>();
			foreach (SystemPerformanceRestoreAble eachRegardless in this.backtestsWithMyValue.Values) {
				if (oneParameterAllValuesAveraged.Correlator.HasUnchosenParametersExceptFor(eachRegardless, this.oneParameterAllValuesAveraged)) continue;
				backtestsWithMyValueAndOnlyChosenOtherValues_cached.Add(eachRegardless.OptimizationIterationSerno, eachRegardless);
			}
			return this.backtestsWithMyValueAndOnlyChosenOtherValues_cached;
		} }
		int													BacktestsWithMyValueAndOnlyChosenOtherValuesCount { get { return this.backtestsWithMyValueAndOnlyChosenOtherValues.Count; } }

		public	KPIs	KPIsGlobal	{ get; private set; }
		public	KPIs	KPIsLocal	{ get; private set; }
		public	bool	Chosen;

		OneParameterOneValueForMomentums() {
			Chosen = true;
			backtestsWithMyValue = new SortedDictionary<int, SystemPerformanceRestoreAble>();
			KPIsGlobal = new KPIs();
			KPIsLocal = new KPIs();
		}

		public OneParameterOneValueForMomentums(OneParameterAllValuesAveraged oneParameterAllValuesAveraged
				, double optimizedValue) : this() {
			this.oneParameterAllValuesAveraged = oneParameterAllValuesAveraged;
			this.ValueSequenced = optimizedValue;
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
			ret += this.backtestsWithMyValue.Count + "backtests";
			return ret;
		}

	}
}
