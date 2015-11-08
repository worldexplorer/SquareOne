using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Sq1.Core;
using Sq1.Core.Sequencing;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Correlation {
	public class OneParameterOneValue {
		[JsonIgnore]	OneParameterAllValuesAveraged OneParameterAllValuesAveraged;

		[JsonIgnore]	public string	ArtificialName		{ get; private set; }
		[JsonIgnore]	public bool		IsArtificialRow		{ get { return string.IsNullOrEmpty(this.ArtificialName) == false; } }

		[JsonProperty]	public double	ValueSequenced		{ get; private set; }
		[JsonIgnore]	public string	ParameterName		{ get { return this.OneParameterAllValuesAveraged.ParameterName; } }
		[JsonIgnore]	public string	ParameterNameValue	{ get {
				if (this.IsArtificialRow) return this.ArtificialName;			//"Average" / "Dispersion""
				string ret = this.OneParameterAllValuesAveraged.ParameterName + "=" + this.ValueSequenced;	//"MaFast.Period=10"
				return ret;
			} }


		[JsonIgnore]	internal	SortedDictionary<int, SystemPerformanceRestoreAble> BacktestsWithMyValue;
		[JsonProperty]				int													BacktestsWithMyValueCount { get { return this.BacktestsWithMyValue.Count; } }
		[JsonIgnore]				SortedDictionary<int, SystemPerformanceRestoreAble> backtestsWithMyValueAndOnlyChosenOtherValues_cached;
		[JsonIgnore]	internal	SortedDictionary<int, SystemPerformanceRestoreAble> BacktestsWithMyValueAndOnlyChosenOtherValues { get {
			if (this.backtestsWithMyValueAndOnlyChosenOtherValues_cached != null) return this.backtestsWithMyValueAndOnlyChosenOtherValues_cached;
			this.backtestsWithMyValueAndOnlyChosenOtherValues_cached = new SortedDictionary<int, SystemPerformanceRestoreAble>();
			foreach (SystemPerformanceRestoreAble eachRegardless in this.BacktestsWithMyValue.Values) {
				if (OneParameterAllValuesAveraged.Correlator.HasUnchosenParametersExceptFor(eachRegardless, this.OneParameterAllValuesAveraged)) continue;
				backtestsWithMyValueAndOnlyChosenOtherValues_cached.Add(eachRegardless.SequenceIterationSerno, eachRegardless);
			}
			return this.backtestsWithMyValueAndOnlyChosenOtherValues_cached;
		} }
		[JsonProperty]				int													BacktestsWithMyValueAndOnlyChosenOtherValuesCount { get { return this.BacktestsWithMyValueAndOnlyChosenOtherValues.Count; } }
		[JsonProperty]				string												BacktestsWithMyValueAndOnlyChosenOtherValueAsString { get { return this.keysAsString(this.BacktestsWithMyValueAndOnlyChosenOtherValues); } }

		[JsonIgnore]	public	KPIsAveraged	KPIsGlobal	{ get; private set; }
		[JsonIgnore]	public	KPIsAveraged	KPIsLocal	{ get; private set; }

		//v1 HAPPY_IDIOTS_VERSION
		//[JsonProperty]	public	bool			Chosen;
		//v2 TRYING_TO_FIX_FIRST_CLICK_RESETTING_ALL_CHECKBOXES 
		[JsonIgnore]			bool			chosen;
		[JsonProperty]	public	bool			Chosen {
			get { return this.chosen; }
			set { this.chosen = value; }
		}

		[JsonIgnore]			KPIs	kPIsLocalMinusGlobal_cached;
		[JsonIgnore]	public	KPIs	KPIsDelta { get {
				if (this.kPIsLocalMinusGlobal_cached != null) return this.kPIsLocalMinusGlobal_cached;
				KPIs ret				= new KPIs("IM_KPIsDelta:" + this.ToString());
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
		[JsonIgnore]	public	KPIsMomentum	KPIsMomentumAverage;


		[JsonIgnore]	public	KPIsMomentum	KPIsMomentumDispersionGlobal;
		[JsonIgnore]	public	KPIsMomentum	KPIsMomentumDispersionLocal;
		[JsonIgnore]			KPIsMomentum	kPIsMomentumDispersionDelta_cached;
		[JsonIgnore]	public	KPIsMomentum	KPIsMomentumDispersionDelta { get {
				if (this.kPIsMomentumDispersionDelta_cached != null) return this.kPIsMomentumDispersionDelta_cached;
				KPIsMomentum ret		= new KPIsMomentum("IM_KPIsMomentumDispersionDelta:" + this.ToString());
				ret.NetProfit			= this.KPIsMomentumDispersionLocal.NetProfit			- this.KPIsMomentumDispersionGlobal.NetProfit;
				ret.PositionsCount		= this.KPIsMomentumDispersionLocal.PositionsCount		- this.KPIsMomentumDispersionGlobal.PositionsCount;
				ret.PositionAvgProfit	= this.KPIsMomentumDispersionLocal.PositionAvgProfit	- this.KPIsMomentumDispersionGlobal.PositionAvgProfit;
				ret.WinLossRatio		= this.KPIsMomentumDispersionLocal.WinLossRatio			- this.KPIsMomentumDispersionGlobal.WinLossRatio;
				ret.ProfitFactor		= this.KPIsMomentumDispersionLocal.ProfitFactor			- this.KPIsMomentumDispersionGlobal.ProfitFactor;
				ret.RecoveryFactor		= this.KPIsMomentumDispersionLocal.RecoveryFactor		- this.KPIsMomentumDispersionGlobal.RecoveryFactor;
				ret.MaxDrawDown			= this.KPIsMomentumDispersionLocal.MaxDrawDown			- this.KPIsMomentumDispersionGlobal.MaxDrawDown;
				ret.MaxConsecWinners	= this.KPIsMomentumDispersionLocal.MaxConsecWinners		- this.KPIsMomentumDispersionGlobal.MaxConsecWinners;
				ret.MaxConsecLosers		= this.KPIsMomentumDispersionLocal.MaxConsecLosers		- this.KPIsMomentumDispersionGlobal.MaxConsecLosers;
				this.kPIsMomentumDispersionDelta_cached = ret;
				return ret;
			} }

		OneParameterOneValue() {
			//Chosen					= true;
			BacktestsWithMyValue	= new SortedDictionary<int, SystemPerformanceRestoreAble>();
			KPIsGlobal				= new KPIsAveraged("IM_KPIsGlobal");
			KPIsLocal				= new KPIsAveraged("IM_KPIsLocal");
		}

		public OneParameterOneValue(OneParameterAllValuesAveraged oneParameterAllValuesAveraged
				, double optimizedValue, string artificialName = null) : this() {
			if (oneParameterAllValuesAveraged == null) {
				string msg = "AVOIDING_NPE_DURING_DESERIALIZATION__this.ParameterNameValue";
				return;
			}

			this.OneParameterAllValuesAveraged = oneParameterAllValuesAveraged;
			this.ValueSequenced = optimizedValue;
			this.ArtificialName = artificialName;

			string momentumFor = "(" + this.ParameterNameValue + ")";
			// will be reassigned soon; just want to avoid NPE in Customiser.cs
			KPIsMomentumAverage				= new KPIsMomentum(KPIsMomentum.KPIS_MOMENTUM_AVERAGE			 + momentumFor);
			KPIsMomentumDispersionGlobal	= new KPIsMomentum(KPIsMomentum.KPIS_MOMENTUM_DISPERSION_GLOBAL	 + momentumFor);
			KPIsMomentumDispersionLocal		= new KPIsMomentum(KPIsMomentum.KPIS_MOMENTUM_DISPERSION_LOCAL	 + momentumFor);
		}

		internal void ClearBacktestsWithMyValue_step1of3() {
			this.BacktestsWithMyValue.Clear();
		}
		internal void AddBacktestForValue(SystemPerformanceRestoreAble eachRun) {
			if (this.BacktestsWithMyValue.ContainsKey(eachRun.SequenceIterationSerno)) {
				string msg = "DUPLICATE";
				Assembler.PopupException(msg);
			}
			this.BacktestsWithMyValue.Add(eachRun.SequenceIterationSerno, eachRun);
			//this.KPIsGlobal.AddKPIs(eachRun);
		}

		internal void CalculateGlobalsAndCloneToLocals() {
			string msig = " //CalculateGlobalsAndCloneToLocals()";
			if (this.IsArtificialRow) {
				string msg = "USE_CalculateLocalsAndDeltasForArtificial()_INSTEAD";
				Assembler.PopupException(msg + msig);
				return;
			}
			this.KPIsGlobal.Reset_addBacktests_getMyMembersReady(this.BacktestsWithMyValue);
			this.KPIsLocal = this.KPIsGlobal.Clone();
		}

		internal void CalculateLocalsAndDeltas() {
			string msig = " //CalculateLocalsAndDeltas()";
			if (this.IsArtificialRow) {
				string msg = "USE_CalculateLocalsAndDeltasForArtificial()_INSTEAD";
				Assembler.PopupException(msg + msig);
				return;
			}

			#if DEBUG
			SortedDictionary<int, SystemPerformanceRestoreAble> backupToSeeChanges = this.BacktestsWithMyValueAndOnlyChosenOtherValues;
			this.backtestsWithMyValueAndOnlyChosenOtherValues_cached = null;	// rebuild and cache

			string backtestsOld = this.keysAsString(backupToSeeChanges);
			string backtestsNew = this.BacktestsWithMyValueAndOnlyChosenOtherValueAsString;
			if (backtestsOld == backtestsNew) {
				string msg = "SAME_BACKTESTS_AS_BEFORE__CHOSEN_MUST_NOT_BE_THE_SAME";
				//Assembler.PopupException(msg);
			} else {
				string msg = "I_NEED_TO_BE_HERE_FOR_MOMENTUMS_CALCULATOR";
			}
			#endif

			this.KPIsLocal.Reset_addBacktests_getMyMembersReady(this.BacktestsWithMyValueAndOnlyChosenOtherValues);
			this.kPIsLocalMinusGlobal_cached = null;
			this.kPIsMomentumDispersionDelta_cached = null;
		}

		string keysAsString(SortedDictionary<int, SystemPerformanceRestoreAble> backtestsWithMyValue) {
			string ret = "";
			foreach (int eachKey in backtestsWithMyValue.Keys) {
				ret += eachKey + ",";
			}
			if (string.IsNullOrEmpty(ret) == false) ret = ret.TrimEnd(",".ToCharArray());
			return ret;
		}

		//internal void CalculateGlobalsForArtificial_Average() {
		//    string msig = " //CalculateGlobalsForArtificial_Average()";

		//    if (this.OneParameterAllValuesAveraged.OneParamOneValues.Count == 0) {
		//        this.KPIsGlobal.Reset();
		//        Assembler.PopupException("AVOIDING_DIVISION_BY_ZERO" + msig);
		//        return;
		//    }

		//    List<KPIs> KPIsList = new List<KPIs>();
		//    foreach (OneParameterOneValue kpisForValue in this.OneParameterAllValuesAveraged.OneParamOneValues) {
		//        KPIsList.Add(kpisForValue.KPIsGlobal);
		//    }
		//    this.KPIsGlobal.Reset_addKPIs_getMyMembersReady(KPIsList);
		//}
		//internal void CalculateLocalsAndDeltasForArtificial_Average() {
		//    string msig = " //CalculateLocalsAndDeltasForArtificial_Average()";

		//    if (this.OneParameterAllValuesAveraged.OneParamValuesChosen.Count == 0) {
		//        this.KPIsLocal.Reset();
		//        this.kPIsLocalMinusGlobal_cached = null;
		//        Assembler.PopupException("AVOIDING_DIVISION_BY_ZERO" + msig);
		//        return;
		//    }

		//    List<KPIs> KPIsList = new List<KPIs>();
		//    foreach (OneParameterOneValue kpisForValue in this.OneParameterAllValuesAveraged.OneParamOneValues) {
		//        KPIsList.Add(kpisForValue.KPIsLocal);
		//    }
		//    this.KPIsLocal.Reset_addKPIs_getMyMembersReady(KPIsList);
		//    this.kPIsLocalMinusGlobal_cached = null;

		//    double checkLocalNetProfit = this.KPIsLocal.NetProfit;
		//}

		//public override string ToString() {
		//    string ret = this.ParameterNameValue + ":";
		//    if (this.IsArtificialRow) {
		//        ret += this.OneParameterAllValuesAveraged.OneParamOneValueByValues.Count + "paramValues";
		//    } else {
		//        ret += this.BacktestsWithMyValue.Count + "backtests";
		//    }
		//    return ret;
		//}

		//internal void CalculateGlobalsForArtificial_Variance() {
		//    List<double> positionsCount		= new List<double>();
		//    List<double> positionAvgProfit	= new List<double>();
		//    List<double> netProfit			= new List<double>();
		//    List<double> winLossRatio		= new List<double>();
		//    List<double> profitFactor		= new List<double>();
		//    List<double> recoveryFactor		= new List<double>();
		//    List<double> maxDrawDown		= new List<double>();
		//    List<double> maxConsecWinners	= new List<double>();
		//    List<double> maxConsecLosers	= new List<double>();
		//    foreach (OneParameterOneValue eachValue in this.OneParameterAllValuesAveraged.OneParamOneValueByValues.Values) {
		//        positionsCount		.Add(eachValue.KPIsGlobal.PositionsCount);
		//        positionAvgProfit	.Add(eachValue.KPIsGlobal.PositionAvgProfit);
		//        netProfit			.Add(eachValue.KPIsGlobal.NetProfit);
		//        winLossRatio		.Add(eachValue.KPIsGlobal.WinLossRatio);
		//        profitFactor		.Add(eachValue.KPIsGlobal.ProfitFactor);
		//        recoveryFactor		.Add(eachValue.KPIsGlobal.RecoveryFactor);
		//        maxDrawDown			.Add(eachValue.KPIsGlobal.MaxDrawDown);
		//        maxConsecWinners	.Add(eachValue.KPIsGlobal.MaxConsecWinners);
		//        maxConsecLosers		.Add(eachValue.KPIsGlobal.MaxConsecLosers);
		//    }
		//    this.KPIsGlobal.PositionsCount		= positionsCount	.Variance();
		//    this.KPIsGlobal.PositionAvgProfit	= positionAvgProfit	.Variance();
		//    this.KPIsGlobal.NetProfit			= netProfit			.Variance();
		//    this.KPIsGlobal.WinLossRatio		= winLossRatio		.Variance();
		//    this.KPIsGlobal.ProfitFactor		= profitFactor		.Variance();
		//    this.KPIsGlobal.RecoveryFactor		= recoveryFactor	.Variance();
		//    this.KPIsGlobal.MaxDrawDown			= maxDrawDown		.Variance();
		//    this.KPIsGlobal.MaxConsecWinners	= maxConsecWinners	.Variance();
		//    this.KPIsGlobal.MaxConsecLosers		= maxConsecLosers	.Variance();
		//}

		//internal void CalculateGlobalsForArtificial_Dispersion() {
		//    List<double> positionsCount		= new List<double>();
		//    List<double> positionAvgProfit	= new List<double>();
		//    List<double> netProfit			= new List<double>();
		//    List<double> winLossRatio		= new List<double>();
		//    List<double> profitFactor		= new List<double>();
		//    List<double> recoveryFactor		= new List<double>();
		//    List<double> maxDrawDown		= new List<double>();
		//    List<double> maxConsecWinners	= new List<double>();
		//    List<double> maxConsecLosers	= new List<double>();
		//    foreach (OneParameterOneValue eachValue in this.OneParameterAllValuesAveraged.OneParamOneValueByValues.Values) {
		//        positionsCount		.Add(eachValue.KPIsGlobal.PositionsCount);
		//        positionAvgProfit	.Add(eachValue.KPIsGlobal.PositionAvgProfit);
		//        netProfit			.Add(eachValue.KPIsGlobal.NetProfit);
		//        winLossRatio		.Add(eachValue.KPIsGlobal.WinLossRatio);
		//        profitFactor		.Add(eachValue.KPIsGlobal.ProfitFactor);
		//        recoveryFactor		.Add(eachValue.KPIsGlobal.RecoveryFactor);
		//        maxDrawDown			.Add(eachValue.KPIsGlobal.MaxDrawDown);
		//        maxConsecWinners	.Add(eachValue.KPIsGlobal.MaxConsecWinners);
		//        maxConsecLosers		.Add(eachValue.KPIsGlobal.MaxConsecLosers);
		//    }
		//    this.KPIsGlobal.PositionsCount		= positionsCount	.StdDev();
		//    this.KPIsGlobal.PositionAvgProfit	= positionAvgProfit	.StdDev();
		//    this.KPIsGlobal.NetProfit			= netProfit			.StdDev();
		//    this.KPIsGlobal.WinLossRatio		= winLossRatio		.StdDev();
		//    this.KPIsGlobal.ProfitFactor		= profitFactor		.StdDev();
		//    this.KPIsGlobal.RecoveryFactor		= recoveryFactor	.StdDev();
		//    this.KPIsGlobal.MaxDrawDown			= maxDrawDown		.StdDev();
		//    this.KPIsGlobal.MaxConsecWinners	= maxConsecWinners	.StdDev();
		//    this.KPIsGlobal.MaxConsecLosers		= maxConsecLosers	.StdDev();
		//}

		//internal void CalculateLocalsAndDeltasForArtificial_Dispersion() {
		//    List<double> positionsCount		= new List<double>();
		//    List<double> positionAvgProfit	= new List<double>();
		//    List<double> netProfit			= new List<double>();
		//    List<double> winLossRatio		= new List<double>();
		//    List<double> profitFactor		= new List<double>();
		//    List<double> recoveryFactor		= new List<double>();
		//    List<double> maxDrawDown		= new List<double>();
		//    List<double> maxConsecWinners	= new List<double>();
		//    List<double> maxConsecLosers	= new List<double>();
		//    foreach (OneParameterOneValue eachValue in this.OneParameterAllValuesAveraged.OneParamOneValueByValues.Values) {
		//        positionsCount		.Add(eachValue.KPIsLocal.PositionsCount);
		//        positionAvgProfit	.Add(eachValue.KPIsLocal.PositionAvgProfit);
		//        netProfit			.Add(eachValue.KPIsLocal.NetProfit);
		//        winLossRatio		.Add(eachValue.KPIsLocal.WinLossRatio);
		//        profitFactor		.Add(eachValue.KPIsLocal.ProfitFactor);
		//        recoveryFactor		.Add(eachValue.KPIsLocal.RecoveryFactor);
		//        maxDrawDown			.Add(eachValue.KPIsLocal.MaxDrawDown);
		//        maxConsecWinners	.Add(eachValue.KPIsLocal.MaxConsecWinners);
		//        maxConsecLosers		.Add(eachValue.KPIsLocal.MaxConsecLosers);
		//    }
		//    this.KPIsLocal.PositionsCount		= positionsCount	.StdDev();
		//    this.KPIsLocal.PositionAvgProfit	= positionAvgProfit	.StdDev();
		//    this.KPIsLocal.NetProfit			= netProfit			.StdDev();
		//    this.KPIsLocal.WinLossRatio			= winLossRatio		.StdDev();
		//    this.KPIsLocal.ProfitFactor			= profitFactor		.StdDev();
		//    this.KPIsLocal.RecoveryFactor		= recoveryFactor	.StdDev();
		//    this.KPIsLocal.MaxDrawDown			= maxDrawDown		.StdDev();
		//    this.KPIsLocal.MaxConsecWinners		= maxConsecWinners	.StdDev();
		//    this.KPIsLocal.MaxConsecLosers		= maxConsecLosers	.StdDev();
		//    this.kPIsLocalMinusGlobal_cached	= null;

		//    if (this.KPIsDelta.NetProfit == 0) {
		//        int a = 0;
		//    }

		//    if (this.KPIsDelta.PositionsCount < 0) {
		//        int a = 0;
		//    }

		//}

		//internal void CalculateLocalsAndDeltasForArtificial_Variance() {
		//    List<double> positionsCount		= new List<double>();
		//    List<double> positionAvgProfit	= new List<double>();
		//    List<double> netProfit			= new List<double>();
		//    List<double> winLossRatio		= new List<double>();
		//    List<double> profitFactor		= new List<double>();
		//    List<double> recoveryFactor		= new List<double>();
		//    List<double> maxDrawDown = new List<double>();
		//    List<double> maxConsecWinners = new List<double>();
		//    List<double> maxConsecLosers = new List<double>();
		//    foreach (OneParameterOneValue eachValue in this.OneParameterAllValuesAveraged.OneParamOneValueByValues.Values) {
		//        positionsCount		.Add(eachValue.KPIsLocal.PositionsCount);
		//        positionAvgProfit	.Add(eachValue.KPIsLocal.PositionAvgProfit);
		//        netProfit			.Add(eachValue.KPIsLocal.NetProfit);
		//        winLossRatio		.Add(eachValue.KPIsLocal.WinLossRatio);
		//        profitFactor		.Add(eachValue.KPIsLocal.ProfitFactor);
		//        recoveryFactor		.Add(eachValue.KPIsLocal.RecoveryFactor);
		//        maxDrawDown			.Add(eachValue.KPIsLocal.MaxDrawDown);
		//        maxConsecWinners	.Add(eachValue.KPIsLocal.MaxConsecWinners);
		//        maxConsecLosers		.Add(eachValue.KPIsLocal.MaxConsecLosers);
		//    }
		//    this.KPIsLocal.PositionsCount		= positionsCount	.Variance();
		//    this.KPIsLocal.PositionAvgProfit	= positionAvgProfit	.Variance();
		//    this.KPIsLocal.NetProfit			= netProfit			.Variance();
		//    this.KPIsLocal.WinLossRatio			= winLossRatio		.Variance();
		//    this.KPIsLocal.ProfitFactor			= profitFactor		.Variance();
		//    this.KPIsLocal.RecoveryFactor		= recoveryFactor	.Variance();
		//    this.KPIsLocal.MaxDrawDown			= maxDrawDown		.Variance();
		//    this.KPIsLocal.MaxConsecWinners		= maxConsecWinners	.Variance();
		//    this.KPIsLocal.MaxConsecLosers		= maxConsecLosers	.Variance();
		//    this.kPIsLocalMinusGlobal_cached	= null;

		//    if (this.KPIsDelta.NetProfit == 0) {
		//        int a = 0;
		//    }
		//}
	}
}
