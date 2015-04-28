using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Sq1.Core.Sequencing;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Correlation {
	public partial class OneParameterAllValuesAveraged : NamedObjectJsonSerializable {
		[JsonIgnore]	public const string ARTIFICIAL_AVERAGE				= "Mean";
		[JsonIgnore]	public const string ARTIFICIAL_AVERAGE_DISPERSION	= "Stdandard Deviation";
		[JsonIgnore]	public const string ARTIFICIAL_AVERAGE_VARIANCE 	= "Variance";

		[JsonIgnore]	public Correlator					Correlator;
		[JsonIgnore]	public string						ParameterName				{ get; private set; }

		[Obsolete("DONT_USE_ME_IM_HERE_FOR_DESERIALIZER_ONLY")]
		[JsonProperty]	public string						Name						{
			get { return this.ParameterName; }
			set { this.ParameterName = value; }
		}
		[JsonProperty]	public MaximizationCriterion		MaximizationCriterion;

		[JsonProperty]	public SortedDictionary<double, OneParameterOneValue>	ValuesByParam	{ get; private set; }
		[JsonIgnore]	public List<OneParameterOneValue>						Values			{ get { return new List<OneParameterOneValue>(this.ValuesByParam.Values); } }

		[JsonIgnore]	public OneParameterOneValue			ArtificialRowAverage		{ get; private set; }
		[JsonIgnore]	public OneParameterOneValue			ArtificialRowDispersion		{ get; private set; }
		[JsonIgnore]	public OneParameterOneValue			ArtificialRowVariance		{ get; private set; }
		[JsonIgnore]	public List<OneParameterOneValue>	AllValuesWithArtificials	{ get; private set; }

		public string ChosenAsString { get {
			string ret = "";
			foreach (var eachValue in this.ValuesByParam.Values) {
				if (eachValue.Chosen == false) continue;
				ret += eachValue.ValueSequenced + ",";
			}
			if (string.IsNullOrEmpty(ret) == false) ret = ret.TrimEnd(",".ToCharArray());
			return ret;
		} }
		public int ChosenCount { get {
			int ret = 0;
			foreach (var eachValue in this.ValuesByParam.Values) {
				if (eachValue.Chosen == false) continue;
				ret ++;
			}
			return ret;
		} }

		OneParameterAllValuesAveraged() {
			AllValuesWithArtificials	= new List<OneParameterOneValue>();
			ValuesByParam				= new SortedDictionary<double, OneParameterOneValue>();

			ArtificialRowAverage		= new OneParameterOneValue(this, 0, ARTIFICIAL_AVERAGE);
			ArtificialRowDispersion		= new OneParameterOneValue(this, 0, ARTIFICIAL_AVERAGE_DISPERSION);
			ArtificialRowVariance		= new OneParameterOneValue(this, 0, ARTIFICIAL_AVERAGE_VARIANCE);
			MaximizationCriterion		= MaximizationCriterion.UNKNOWN;
		}
		public OneParameterAllValuesAveraged(Correlator correlator, string parameterName) : this() {
			this.Correlator = correlator;
			this.ParameterName = parameterName;
		}

		internal void AddBacktestForValue_KPIsGlobalAddForIndicatorValue(double optimizedValue, SystemPerformanceRestoreAble eachRun) {
			if (this.ValuesByParam.ContainsKey(optimizedValue) == false) {
				this.ValuesByParam		  .Add(optimizedValue, new OneParameterOneValue	(this, optimizedValue));
				//this.AvgMomentumsByParam  .Add(optimizedValue, new AvgCorMomentums		(this, optimizedValue));
			}
			OneParameterOneValue paramValue = this.ValuesByParam[optimizedValue];
			paramValue.AddBacktestForValue_AddKPIsGlobal(eachRun);
		}

		internal void KPIsGlobalNoMoreParameters_DivideTotalsByCount() {
			foreach (OneParameterOneValue kpisForValue in this.ValuesByParam.Values) {
				kpisForValue.KPIsGlobal_DivideTotalsByCount();
			}

			this.AllValuesWithArtificials = new List<OneParameterOneValue>(this.ValuesByParam.Values);

			this.ArtificialRowAverage.CalculateGlobalsForArtificial_Average();
			this.ArtificialRowAverage.CalculateLocalsAndDeltasForArtificial_Average();
			this.AllValuesWithArtificials.Add(this.ArtificialRowAverage);

			this.ArtificialRowDispersion.CalculateGlobalsForArtificial_Dispersion();
			this.ArtificialRowDispersion.CalculateLocalsAndDeltasForArtificial_Dispersion();
			this.AllValuesWithArtificials.Add(this.ArtificialRowDispersion);

			this.ArtificialRowVariance.CalculateGlobalsForArtificial_Variance();
			this.ArtificialRowVariance.CalculateLocalsAndDeltasForArtificial_Variance();
			this.AllValuesWithArtificials.Add(this.ArtificialRowVariance);
		}

		internal void CalculateLocalsAndDeltas_forEachValue_and3artificials() {
			if (this.ValuesByParam.Count <= 1) {
				string msg = "I_HAVE_ONLY_ONE_VALUE_ACROSS_ALL_BACKTESTS__IM_NOT_DISPLAYED__SKIP_ME_UPSTACK";
				Assembler.PopupException(msg);
			}
			foreach (OneParameterOneValue eachValue in this.ValuesByParam.Values) {
				eachValue.CalculateLocalsAndDeltas();
			}

			this.ArtificialRowAverage		.CalculateLocalsAndDeltasForArtificial_Average();
			this.ArtificialRowDispersion	.CalculateLocalsAndDeltasForArtificial_Dispersion();
			this.ArtificialRowVariance		.CalculateLocalsAndDeltasForArtificial_Variance();
		}

		public override string ToString() {
			string ret = this.ParameterName + ":" + this.ValuesByParam.Count + "values";
			ret += ";chosen[" + this.ChosenAsString + "]";
			return ret;
		}

		internal void chooseThisUnchooseOthers(OneParameterOneValue thisCheckedOthersOff) {
			int found = 0;
			foreach (OneParameterOneValue eachValue in this.ValuesByParam.Values) {
				if (eachValue != thisCheckedOthersOff) {
					eachValue.Chosen = false;
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
			foreach (OneParameterOneValue eachValue in this.ValuesByParam.Values) {
				eachValue.Chosen = true;
			}
		}

	}
}
