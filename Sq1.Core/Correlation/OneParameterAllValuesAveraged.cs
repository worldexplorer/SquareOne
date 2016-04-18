using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Sq1.Core.Sequencing;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Correlation {
	public partial class OneParameterAllValuesAveraged : NamedObjectJsonSerializable {
		[JsonIgnore]	public const string ARTIFICIAL_ROW_MEAN			= "Mean";
		[JsonIgnore]	public const string ARTIFICIAL_ROW_DISPERSION	= "Standard Deviation";
		[JsonIgnore]	public const string ARTIFICIAL_ROW_VARIANCE 	= "Variance";

		[JsonIgnore]	public	Correlator					Correlator;
		[JsonProperty]	public	string						ParameterName				{ get; private set; }

		[Obsolete("DONT_USE_ME_IM_HERE_FOR_DESERIALIZER_ONLY")]
		[JsonProperty]	public	string						Name						{
			get { return this.ParameterName; }
			set { this.ParameterName = value; }
		}
		[JsonProperty]	public	MaximizationCriterion		MaximizationCriterion;

		[JsonProperty]	public	SortedDictionary<double, OneParameterOneValue>	OneParamOneValueByValues	{ get; private set; }
		[JsonIgnore]	public	List<OneParameterOneValue>						OneParamOneValues			{ get { return new List<OneParameterOneValue>(this.OneParamOneValueByValues.Values); } }

		[JsonIgnore]	public	OneParameterOneValue		ArtificialRowMean			{ get; private set; }
		[JsonIgnore]	public	OneParameterOneValue		ArtificialRowDispersion		{ get; private set; }
		[JsonIgnore]	public	OneParameterOneValue		ArtificialRowVariance		{ get; private set; }
		[JsonIgnore]	public	List<OneParameterOneValue>	AllValuesWithArtificials	{ get; private set; }

		[JsonIgnore]			List<OneParameterOneValue>	oneParamValuesChosen_cached;
		[JsonIgnore]	public	List<OneParameterOneValue>	OneParamValuesChosen	{ get {
			//SINCE_WE_MANIPULATE_CHOSEN_ON_STARTUP_I_GET_WRONG_NUMBERS if (this.oneParamValuesChosen_cached != null) return this.oneParamValuesChosen_cached;
			List<OneParameterOneValue> ret = new List<OneParameterOneValue>();
			foreach (var eachValue in this.OneParamOneValueByValues.Values) {
				if (eachValue.Chosen == false) continue;
				ret.Add(eachValue);
			}
			this.oneParamValuesChosen_cached = ret;
			return ret;
		} }
		[JsonProperty]	public	int							ChosenCount		{ get { return this.OneParamValuesChosen.Count; } }
		[JsonProperty]	public	string						ChosenAsString	{ get {
			string ret = "";
			foreach (var eachValue in OneParamValuesChosen) {
				ret += eachValue.ValueSequenced + ",";
			}
			if (string.IsNullOrEmpty(ret) == false) ret = ret.TrimEnd(",".ToCharArray());
			return ret;
		} }


		public OneParameterAllValuesAveraged() {
			string msig = "THIS_CTOR_IS_REQUIRED_BY {public for RepositoryBlaBlaBla where T : new()}";

			AllValuesWithArtificials	= new List<OneParameterOneValue>();
			OneParamOneValueByValues	= new SortedDictionary<double, OneParameterOneValue>();

			ArtificialRowMean			= new OneParameterOneValue(this, 0, ARTIFICIAL_ROW_MEAN);
			ArtificialRowDispersion		= new OneParameterOneValue(this, 0, ARTIFICIAL_ROW_DISPERSION);
			ArtificialRowVariance		= new OneParameterOneValue(this, 0, ARTIFICIAL_ROW_VARIANCE);

			MaximizationCriterion		= MaximizationCriterion.UNKNOWN;
		}
		public OneParameterAllValuesAveraged(Correlator correlator, string parameterName) : this() {
			this.Correlator = correlator;
			this.ParameterName = parameterName;
		}

		internal void ClearBacktestsForAllMyValue_step1of3() {
			foreach (OneParameterOneValue paramValue in this.OneParamOneValueByValues.Values) {
				paramValue.ClearBacktestsWithMyValue_step1of3();
			}
			this.oneParamValuesChosen_cached = null;
		}

		internal void AddBacktestForValue_KPIsGlobalAddForIndicatorValue_step2of3(double optimizedValue, SystemPerformanceRestoreAble eachRun) {
			if (this.OneParamOneValueByValues.ContainsKey(optimizedValue) == false) {
				this.OneParamOneValueByValues		  .Add(optimizedValue, new OneParameterOneValue	(this, optimizedValue));
				//this.AvgMomentumsByParam  .Add(optimizedValue, new AvgCorMomentums		(this, optimizedValue));
			}
			OneParameterOneValue paramValue = this.OneParamOneValueByValues[optimizedValue];
			paramValue.AddBacktestForValue(eachRun);
			this.oneParamValuesChosen_cached = null;
		}

		internal void KPIsGlobalNoMoreParameters_CalculateGlobalsAndCloneToLocals_step3of3() {
			foreach (OneParameterOneValue kpisForValue in this.OneParamOneValueByValues.Values) {
				kpisForValue.CalculateGlobalsAndCloneToLocals();
			}

			this.AllValuesWithArtificials = new List<OneParameterOneValue>(this.OneParamOneValueByValues.Values);

			//this.ArtificialRowMean.CalculateGlobalsForArtificial_Average();
			//this.ArtificialRowMean.CalculateLocalsAndDeltasForArtificial_Average();
			//this.AllValuesWithArtificials.Add(this.ArtificialRowMean);

			//this.ArtificialRowDispersion.CalculateGlobalsForArtificial_Dispersion();
			//this.ArtificialRowDispersion.CalculateLocalsAndDeltasForArtificial_Dispersion();
			//this.AllValuesWithArtificials.Add(this.ArtificialRowDispersion);

			//this.ArtificialRowVariance.CalculateGlobalsForArtificial_Variance();
			//this.ArtificialRowVariance.CalculateLocalsAndDeltasForArtificial_Variance();
			//REPLACE_WITH_max(avg(Net)),min(StDev(Net))_ALIGNED_WITH_MaximizationCriterion this.AllValuesWithArtificials.Add(this.ArtificialRowVariance);

			this.oneParamValuesChosen_cached = null;
		}

		internal void CalculateLocalsAndDeltas_forEachValue_and3artificials() {
			if (this.OneParamOneValueByValues.Count <= 1) {
				string msg = "I_HAVE_ONLY_ONE_VALUE_ACROSS_ALL_BACKTESTS__IM_NOT_DISPLAYED__SKIP_ME_UPSTACK";
				Assembler.PopupException(msg);
			}
			foreach (OneParameterOneValue eachValue in this.OneParamOneValueByValues.Values) {
				eachValue.CalculateLocalsAndDeltas();
			}

			//this.ArtificialRowMean			.CalculateLocalsAndDeltasForArtificial_Average();
			//this.ArtificialRowDispersion	.CalculateLocalsAndDeltasForArtificial_Dispersion();
			//this.ArtificialRowVariance		.CalculateLocalsAndDeltasForArtificial_Variance();
		}

		public override string ToString() {
			string ret = this.ParameterName + ":" + this.OneParamOneValueByValues.Count + "values";
			ret += ";chosen[" + this.ChosenAsString + "]";
			return ret;
		}

		internal void chooseThisUnchooseOthers(OneParameterOneValue thisCheckedOthersOff) {
			int found = 0;
			foreach (OneParameterOneValue eachValue in this.OneParamOneValueByValues.Values) {
				if (eachValue != thisCheckedOthersOff) {
					eachValue.Chosen = false;
					this.oneParamValuesChosen_cached = null;
					continue;
				}
				eachValue.Chosen = true;
				found++;
			}
			if (found != 1) {
				string msg = "MUST_BE_FOUND_EXACTLY_ONCE[" + found + "]";
				Assembler.PopupException(msg);
			}
			string visualCheck = this.ToString();
		}

		internal void chooseAllValues() {
			foreach (OneParameterOneValue eachValue in this.OneParamOneValueByValues.Values) {
				eachValue.Chosen = true;
				this.oneParamValuesChosen_cached = null;
			}
		}

	}
}
