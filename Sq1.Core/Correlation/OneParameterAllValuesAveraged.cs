using System.Collections.Generic;

using Sq1.Core.Sequencing;

namespace Sq1.Core.Correlation {
	public partial class OneParameterAllValuesAveraged {
		public const string ARTIFICIAL_AVERAGE				= "Average";
		public const string ARTIFICIAL_AVERAGE_DISPERSION	= "Dispersion";
		public const string ARTIFICIAL_AVERAGE_KURTOSIS 	= "Kurtosis";

		public Correlator					Sequencer;
		public string						ParameterName				{ get; private set; }
		public MaximizationCriterion		MaximizationCriterion;

		public SortedDictionary<double, OneParameterOneValue> ValuesByParam { get; private set; }

		public OneParameterOneValue			ArtificialRowAverage		{ get; private set; }
		public OneParameterOneValue			ArtificialRowDispersion		{ get; private set; }
		public OneParameterOneValue			ArtificialRowKurtosis		{ get; private set; }
		public List<OneParameterOneValue>	AllValuesWithArtificials	{ get; private set; }

		OneParameterAllValuesAveraged() {
			AllValuesWithArtificials	= new List<OneParameterOneValue>();
			ValuesByParam				= new SortedDictionary<double, OneParameterOneValue>();

			ArtificialRowAverage		= new OneParameterOneValue(this, 0, ARTIFICIAL_AVERAGE);
			ArtificialRowDispersion		= new OneParameterOneValue(this, 0, ARTIFICIAL_AVERAGE_DISPERSION);
			ArtificialRowKurtosis		= new OneParameterOneValue(this, 0, ARTIFICIAL_AVERAGE_KURTOSIS);
			MaximizationCriterion		= MaximizationCriterion.ProfitFactor;
		}
		public OneParameterAllValuesAveraged(Correlator sequencer, string parameterName) : this() {
			this.Sequencer = sequencer;
			this.ParameterName = parameterName;
		}

		internal void AddBacktestForValue_KPIsGlobalAddForIndicatorValue(double optimizedValue, SystemPerformanceRestoreAble eachRun) {
			if (this.ValuesByParam.ContainsKey(optimizedValue) == false) {
				this.ValuesByParam.Add(optimizedValue, new OneParameterOneValue(this, optimizedValue));
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

			this.ArtificialRowKurtosis.CalculateGlobalsForArtificial_Kurtsotis();
			this.ArtificialRowKurtosis.CalculateLocalsAndDeltasForArtificial_Kurtsotis();
			this.AllValuesWithArtificials.Add(this.ArtificialRowKurtosis);
		}

		internal void CalculateLocalsAndDeltas() {
			foreach (OneParameterOneValue eachValue in this.ValuesByParam.Values) {
				eachValue.CalculateLocalsAndDeltas();
			}

			this.ArtificialRowAverage.CalculateLocalsAndDeltasForArtificial_Average();
			this.ArtificialRowDispersion.CalculateLocalsAndDeltasForArtificial_Dispersion();
			this.ArtificialRowKurtosis.CalculateLocalsAndDeltasForArtificial_Kurtsotis();
		}

		public override string ToString() {
			return this.ParameterName + ":" + this.ValuesByParam.Count + "values";
		}
	}
}
