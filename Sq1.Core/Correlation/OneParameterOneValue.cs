using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Sq1.Core;
using Sq1.Core.Sequencing;

namespace Sq1.Core.Correlation {
	public class OneParameterOneValue {
		[JsonIgnore]	OneParameterAllValuesAveraged oneParameterAllValuesAveraged;

		[JsonIgnore]	public string	ArtificialName		{ get; private set; }
		[JsonIgnore]	public bool		IsArtificialRow		{ get { return string.IsNullOrEmpty(this.ArtificialName) == false; } }

		[JsonProperty]	public double	ValueSequenced		{ get; private set; }
		[JsonIgnore]	public string	ParameterName		{ get { return this.oneParameterAllValuesAveraged.ParameterName; } }
		[JsonIgnore]	public string	ParameterNameValue	{ get {
				if (this.IsArtificialRow) return this.ArtificialName;			//"Average" / "Dispersion""
				string ret = this.oneParameterAllValuesAveraged.ParameterName + "=" + this.ValueSequenced;	//"MaFast.Period=10"
				return ret;
			} }


		[JsonIgnore]	SortedDictionary<int, SystemPerformanceRestoreAble> backtestsWithMyValue;
		[JsonProperty]	int													BacktestsWithMyValueCount { get {
			return this.backtestsWithMyValue.Count; } }

		[JsonIgnore]	SortedDictionary<int, SystemPerformanceRestoreAble> backtestsWithMyValueAndOnlyChosenOtherValues_cached;
		[JsonIgnore]	SortedDictionary<int, SystemPerformanceRestoreAble> backtestsWithMyValueAndOnlyChosenOtherValues { get {
			if (this.backtestsWithMyValueAndOnlyChosenOtherValues_cached != null) return this.backtestsWithMyValueAndOnlyChosenOtherValues_cached;
			this.backtestsWithMyValueAndOnlyChosenOtherValues_cached = new SortedDictionary<int, SystemPerformanceRestoreAble>();
			foreach (SystemPerformanceRestoreAble eachRegardless in this.backtestsWithMyValue.Values) {
				if (oneParameterAllValuesAveraged.Correlator.HasUnchosenParametersExceptFor(eachRegardless, this.oneParameterAllValuesAveraged)) continue;
				backtestsWithMyValueAndOnlyChosenOtherValues_cached.Add(eachRegardless.SequenceIterationSerno, eachRegardless);
			}
			return this.backtestsWithMyValueAndOnlyChosenOtherValues_cached;
		} }
		[JsonProperty]	int													BacktestsWithMyValueAndOnlyChosenOtherValuesCount { get {
			return this.backtestsWithMyValueAndOnlyChosenOtherValues.Count; } }
		[JsonProperty]	string												BacktestsWithMyValueAndOnlyChosenOtherValueAsString { get {
			return this.keysAsString(this.backtestsWithMyValueAndOnlyChosenOtherValues); } }

		[JsonIgnore]	public	KPIsAveraged	KPIsGlobal	{ get; private set; }
		[JsonIgnore]	public	KPIsAveraged	KPIsLocal	{ get; private set; }
		[JsonProperty]	public	bool			Chosen;


		[JsonIgnore]			KPIsAveraged	kPIsLocalMinusGlobal_cached;
		[JsonIgnore]	public	KPIsAveraged	KPIsDelta { get {
				if (this.kPIsLocalMinusGlobal_cached != null) return this.kPIsLocalMinusGlobal_cached;
				KPIsAveraged ret		= new KPIsAveraged();
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

		// controlled by AvgCorMomentumsCalculator => MomentumsAveragedByParameter.getter => AvgCorMomentums.ctor()
		[JsonIgnore]	public	KPIsAveraged	KPIsMomentumsAverage;
		[JsonIgnore]	public	KPIsAveraged	KPIsMomentumsDispersion;
		[JsonIgnore]	public	KPIsAveraged	KPIsMomentumsVariance;

		OneParameterOneValue() {
			Chosen					= true;
			backtestsWithMyValue	= new SortedDictionary<int, SystemPerformanceRestoreAble>();
			KPIsGlobal				= new KPIsAveraged();
			KPIsLocal				= new KPIsAveraged();

			// will be reassigned soon; just want to avoid NPE in Customiser.cs
			KPIsMomentumsAverage	= new KPIsAveraged();
			KPIsMomentumsDispersion	= new KPIsAveraged();
			KPIsMomentumsVariance	= new KPIsAveraged();
		}

		public OneParameterOneValue(OneParameterAllValuesAveraged oneParameterAllValuesAveraged
				, double optimizedValue, string artificialName = null) : this() {
			this.oneParameterAllValuesAveraged = oneParameterAllValuesAveraged;
			this.ValueSequenced = optimizedValue;
			this.ArtificialName = artificialName;
		}

		internal void ClearBacktestsWithMyValue_step1of3() {
			this.backtestsWithMyValue.Clear();
		}
		internal void AddBacktestForValue_AddKPIsGlobal_step2of3(SystemPerformanceRestoreAble eachRun) {
			if (this.backtestsWithMyValue.ContainsKey(eachRun.SequenceIterationSerno)) {
				string msg = "DUPLICATE";
				Assembler.PopupException(msg);
			}
			this.backtestsWithMyValue.Add(eachRun.SequenceIterationSerno, eachRun);
			this.KPIsGlobal.AddKPIs(eachRun);
		}

		internal void KPIsGlobal_DivideTotalsByCount_step3of3() {
			int noDivisionToZero = this.backtestsWithMyValue.Count;
			if (noDivisionToZero == 0) return;
			KPIsGlobal.DivideTotalByCount(noDivisionToZero);
			KPIsLocal = KPIsGlobal.Clone();
		}

		internal void CalculateLocalsAndDeltas() {
			string msig = " //CalculateLocalsAndDeltas()";
			if (this.IsArtificialRow) {
				string msg = "USE_CalculateLocalsAndDeltasForArtificial()_INSTEAD";
				Assembler.PopupException(msg + msig);
				return;
			}

			SortedDictionary<int, SystemPerformanceRestoreAble> backupToSeeChanges = this.backtestsWithMyValueAndOnlyChosenOtherValues;

			this.backtestsWithMyValueAndOnlyChosenOtherValues_cached = null;	// rebuild and cache

			#if DEBUG
			string backtestsOld = this.keysAsString(backupToSeeChanges);
			string backtestsNew = this.BacktestsWithMyValueAndOnlyChosenOtherValueAsString;
			if (backtestsOld == backtestsNew) {
				string msg = "SAME_BACKTESTS_AS_BEFORE__CHOSEN_MUST_NOT_BE_THE_SAME";
				//Assembler.PopupException(msg);
			} else {
				string msg = "I_NEED_TO_BE_HERE_FOR_MOMENTUMS_CALCULATOR";
			}
			#endif

			this.KPIsLocal.Reset();
			foreach (SystemPerformanceRestoreAble eachChosen in this.backtestsWithMyValueAndOnlyChosenOtherValues.Values) {
				this.KPIsLocal.AddKPIs(eachChosen);
			}

			int noDivisionToZero = this.backtestsWithMyValueAndOnlyChosenOtherValues.Count;
			if (noDivisionToZero == 0) {
				//Assembler.PopupException("AVOIDING_DIVISION_BY_ZERO" + msig, null, false);
				this.kPIsLocalMinusGlobal_cached = null;
				return;
			}
			this.KPIsLocal.DivideTotalByCount(noDivisionToZero);
			this.kPIsLocalMinusGlobal_cached = null;
		}

		string keysAsString(SortedDictionary<int, SystemPerformanceRestoreAble> backtestsWithMyValue) {
			string ret = "";
			foreach (int eachKey in backtestsWithMyValue.Keys) {
				ret += eachKey + ",";
			}
			if (string.IsNullOrEmpty(ret) == false) ret = ret.TrimEnd(",".ToCharArray());
			return ret;
		}

		internal void CalculateLocalsAndDeltasForArtificial_Average() {
			string msig = " //CalculateLocalsAndDeltasForArtificial_Average()";
			this.KPIsLocal.Reset();

			List<OneParameterOneValue> parentValues = new List<OneParameterOneValue>(this.oneParameterAllValuesAveraged.ValuesByParam.Values);
			foreach (OneParameterOneValue kpisForValue in parentValues) {
				this.KPIsLocal.AddKPIs(kpisForValue.KPIsLocal);
			}
			int noDivisionToZero = parentValues.Count;
			if (noDivisionToZero == 0) {
				Assembler.PopupException("AVOIDING_DIVISION_BY_ZERO" + msig);
				return;
			}
			this.KPIsLocal.DivideTotalByCount(noDivisionToZero);
			this.kPIsLocalMinusGlobal_cached = null;
		}
		internal void CalculateGlobalsForArtificial_Average() {
			string msig = " //CalculateGlobalsForArtificial_Average()";
			this.KPIsGlobal.Reset();

			List<OneParameterOneValue> parentValues = new List<OneParameterOneValue>(this.oneParameterAllValuesAveraged.ValuesByParam.Values);
			foreach (OneParameterOneValue kpisForValue in parentValues) {
				this.KPIsGlobal.AddKPIs(kpisForValue.KPIsGlobal);
			}
			int noDivisionToZero = parentValues.Count;
			if (noDivisionToZero == 0) {
				Assembler.PopupException("AVOIDING_DIVISION_BY_ZERO" + msig);
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

		internal void CalculateGlobalsForArtificial_Variance() {
			List<double> positionsCount		= new List<double>();
			List<double> positionAvgProfit	= new List<double>();
			List<double> netProfit			= new List<double>();
			List<double> winLossRatio		= new List<double>();
			List<double> profitFactor		= new List<double>();
			List<double> recoveryFactor		= new List<double>();
			List<double> maxDrawDown		= new List<double>();
			List<double> maxConsecWinners	= new List<double>();
			List<double> maxConsecLosers	= new List<double>();
			foreach (OneParameterOneValue eachValue in this.oneParameterAllValuesAveraged.ValuesByParam.Values) {
				positionsCount		.Add(eachValue.KPIsGlobal.PositionsCount);
				positionAvgProfit	.Add(eachValue.KPIsGlobal.PositionAvgProfit);
				netProfit			.Add(eachValue.KPIsGlobal.NetProfit);
				winLossRatio		.Add(eachValue.KPIsGlobal.WinLossRatio);
				profitFactor		.Add(eachValue.KPIsGlobal.ProfitFactor);
				recoveryFactor		.Add(eachValue.KPIsGlobal.RecoveryFactor);
				maxDrawDown			.Add(eachValue.KPIsGlobal.MaxDrawDown);
				maxConsecWinners	.Add(eachValue.KPIsGlobal.MaxConsecWinners);
				maxConsecLosers		.Add(eachValue.KPIsGlobal.MaxConsecLosers);
			}
			this.KPIsGlobal.PositionsCount		= positionsCount	.Variance();
			this.KPIsGlobal.PositionAvgProfit	= positionAvgProfit	.Variance();
			this.KPIsGlobal.NetProfit			= netProfit			.Variance();
			this.KPIsGlobal.WinLossRatio		= winLossRatio		.Variance();
			this.KPIsGlobal.ProfitFactor		= profitFactor		.Variance();
			this.KPIsGlobal.RecoveryFactor		= recoveryFactor	.Variance();
			this.KPIsGlobal.MaxDrawDown			= maxDrawDown		.Variance();
			this.KPIsGlobal.MaxConsecWinners	= maxConsecWinners	.Variance();
			this.KPIsGlobal.MaxConsecLosers		= maxConsecLosers	.Variance();
		}

		internal void CalculateGlobalsForArtificial_Dispersion() {
			List<double> positionsCount		= new List<double>();
			List<double> positionAvgProfit	= new List<double>();
			List<double> netProfit			= new List<double>();
			List<double> winLossRatio		= new List<double>();
			List<double> profitFactor		= new List<double>();
			List<double> recoveryFactor		= new List<double>();
			List<double> maxDrawDown		= new List<double>();
			List<double> maxConsecWinners	= new List<double>();
			List<double> maxConsecLosers	= new List<double>();
			foreach (OneParameterOneValue eachValue in this.oneParameterAllValuesAveraged.ValuesByParam.Values) {
				positionsCount		.Add(eachValue.KPIsGlobal.PositionsCount);
				positionAvgProfit	.Add(eachValue.KPIsGlobal.PositionAvgProfit);
				netProfit			.Add(eachValue.KPIsGlobal.NetProfit);
				winLossRatio		.Add(eachValue.KPIsGlobal.WinLossRatio);
				profitFactor		.Add(eachValue.KPIsGlobal.ProfitFactor);
				recoveryFactor		.Add(eachValue.KPIsGlobal.RecoveryFactor);
				maxDrawDown			.Add(eachValue.KPIsGlobal.MaxDrawDown);
				maxConsecWinners	.Add(eachValue.KPIsGlobal.MaxConsecWinners);
				maxConsecLosers		.Add(eachValue.KPIsGlobal.MaxConsecLosers);
			}
			this.KPIsGlobal.PositionsCount		= positionsCount	.StdDev();
			this.KPIsGlobal.PositionAvgProfit	= positionAvgProfit	.StdDev();
			this.KPIsGlobal.NetProfit			= netProfit			.StdDev();
			this.KPIsGlobal.WinLossRatio		= winLossRatio		.StdDev();
			this.KPIsGlobal.ProfitFactor		= profitFactor		.StdDev();
			this.KPIsGlobal.RecoveryFactor		= recoveryFactor	.StdDev();
			this.KPIsGlobal.MaxDrawDown			= maxDrawDown		.StdDev();
			this.KPIsGlobal.MaxConsecWinners	= maxConsecWinners	.StdDev();
			this.KPIsGlobal.MaxConsecLosers		= maxConsecLosers	.StdDev();
		}

		internal void CalculateLocalsAndDeltasForArtificial_Dispersion() {
			List<double> positionsCount		= new List<double>();
			List<double> positionAvgProfit	= new List<double>();
			List<double> netProfit			= new List<double>();
			List<double> winLossRatio		= new List<double>();
			List<double> profitFactor		= new List<double>();
			List<double> recoveryFactor		= new List<double>();
			List<double> maxDrawDown		= new List<double>();
			List<double> maxConsecWinners	= new List<double>();
			List<double> maxConsecLosers	= new List<double>();
			foreach (OneParameterOneValue eachValue in this.oneParameterAllValuesAveraged.ValuesByParam.Values) {
				positionsCount		.Add(eachValue.KPIsLocal.PositionsCount);
				positionAvgProfit	.Add(eachValue.KPIsLocal.PositionAvgProfit);
				netProfit			.Add(eachValue.KPIsLocal.NetProfit);
				winLossRatio		.Add(eachValue.KPIsLocal.WinLossRatio);
				profitFactor		.Add(eachValue.KPIsLocal.ProfitFactor);
				recoveryFactor		.Add(eachValue.KPIsLocal.RecoveryFactor);
				maxDrawDown			.Add(eachValue.KPIsLocal.MaxDrawDown);
				maxConsecWinners	.Add(eachValue.KPIsLocal.MaxConsecWinners);
				maxConsecLosers		.Add(eachValue.KPIsLocal.MaxConsecLosers);
			}
			this.KPIsLocal.PositionsCount		= positionsCount	.StdDev();
			this.KPIsLocal.PositionAvgProfit	= positionAvgProfit	.StdDev();
			this.KPIsLocal.NetProfit			= netProfit			.StdDev();
			this.KPIsLocal.WinLossRatio			= winLossRatio		.StdDev();
			this.KPIsLocal.ProfitFactor			= profitFactor		.StdDev();
			this.KPIsLocal.RecoveryFactor		= recoveryFactor	.StdDev();
			this.KPIsLocal.MaxDrawDown			= maxDrawDown		.StdDev();
			this.KPIsLocal.MaxConsecWinners		= maxConsecWinners	.StdDev();
			this.KPIsLocal.MaxConsecLosers		= maxConsecLosers	.StdDev();
			this.kPIsLocalMinusGlobal_cached	= null;
		}

		internal void CalculateLocalsAndDeltasForArtificial_Variance() {
			List<double> positionsCount		= new List<double>();
			List<double> positionAvgProfit	= new List<double>();
			List<double> netProfit			= new List<double>();
			List<double> winLossRatio		= new List<double>();
			List<double> profitFactor		= new List<double>();
			List<double> recoveryFactor		= new List<double>();
			List<double> maxDrawDown = new List<double>();
			List<double> maxConsecWinners = new List<double>();
			List<double> maxConsecLosers = new List<double>();
			foreach (OneParameterOneValue eachValue in this.oneParameterAllValuesAveraged.ValuesByParam.Values) {
				positionsCount		.Add(eachValue.KPIsLocal.PositionsCount);
				positionAvgProfit	.Add(eachValue.KPIsLocal.PositionAvgProfit);
				netProfit			.Add(eachValue.KPIsLocal.NetProfit);
				winLossRatio		.Add(eachValue.KPIsLocal.WinLossRatio);
				profitFactor		.Add(eachValue.KPIsLocal.ProfitFactor);
				recoveryFactor		.Add(eachValue.KPIsLocal.RecoveryFactor);
				maxDrawDown			.Add(eachValue.KPIsLocal.MaxDrawDown);
				maxConsecWinners	.Add(eachValue.KPIsLocal.MaxConsecWinners);
				maxConsecLosers		.Add(eachValue.KPIsLocal.MaxConsecLosers);
			}
			this.KPIsLocal.PositionsCount		= positionsCount	.Variance();
			this.KPIsLocal.PositionAvgProfit	= positionAvgProfit	.Variance();
			this.KPIsLocal.NetProfit			= netProfit			.Variance();
			this.KPIsLocal.WinLossRatio			= winLossRatio		.Variance();
			this.KPIsLocal.ProfitFactor			= profitFactor		.Variance();
			this.KPIsLocal.RecoveryFactor		= recoveryFactor	.Variance();
			this.KPIsLocal.MaxDrawDown			= maxDrawDown		.Variance();
			this.KPIsLocal.MaxConsecWinners		= maxConsecWinners	.Variance();
			this.KPIsLocal.MaxConsecLosers		= maxConsecLosers	.Variance();
			this.kPIsLocalMinusGlobal_cached	= null;
		}
	}
}
