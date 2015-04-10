using System.Collections.Generic;

using Sq1.Core.Optimization;

namespace Sq1.Widgets.Optimization {
	public class OneParameterAllValuesAveraged {
		public const string ARTIFICIAL_AVERAGE = "Average";
		public const string ARTIFICIAL_AVERAGE_DISPERSION = "Dispersion";
		public const string ARTIFICIAL_AVERAGE_EXCENTRICITY = "Excentricity";

		public string ParameterName { get; private set; }

		public SortedDictionary<double, OneParameterOneValue> AllValuesForOneParameter { get; private set; }
		public SortedDictionary<int, SystemPerformanceRestoreAble> AllUnderlyingForDispersion { get; private set; }

		public OneParameterOneValue ArtificialRowForAllKPIsAverage { get; private set; }
		public OneParameterOneValue ArtificialRowForAllKPIsAverageDispersion { get; private set; }
		public OneParameterOneValue ArtificialRowForAllKPIsAverageExcentricity { get; private set; }
		public List<OneParameterOneValue> AllValuesForOneParameterWithAverages { get; private set; }

		public OneParameterAllValuesAveraged(string parameterName) {
			this.ParameterName = parameterName;
			AllValuesForOneParameterWithAverages = new List<OneParameterOneValue>();
			AllValuesForOneParameter = new SortedDictionary<double, OneParameterOneValue>();

			ArtificialRowForAllKPIsAverage = new OneParameterOneValue(parameterName, 0, ARTIFICIAL_AVERAGE);
			ArtificialRowForAllKPIsAverageDispersion = new OneParameterOneValue(parameterName, 0, ARTIFICIAL_AVERAGE_DISPERSION);
			ArtificialRowForAllKPIsAverageExcentricity = new OneParameterOneValue(parameterName, 0, ARTIFICIAL_AVERAGE_EXCENTRICITY);
			AllUnderlyingForDispersion = new SortedDictionary<int, SystemPerformanceRestoreAble>();
		}

		internal void AddKPIsForIndicatorValue(double optimizedValue, SystemPerformanceRestoreAble eachRun) {
			if (this.AllValuesForOneParameter.ContainsKey(optimizedValue) == false) {
				this.AllValuesForOneParameter.Add(optimizedValue, new OneParameterOneValue(this.ParameterName, optimizedValue));
			}
			OneParameterOneValue kpisForValue = this.AllValuesForOneParameter[optimizedValue];
			kpisForValue.AddKPIsGlobal(eachRun);

			this.ArtificialRowForAllKPIsAverage.AddKPIsGlobal(eachRun);
			this.AllUnderlyingForDispersion.Add(eachRun.OptimizationIterationSerno, eachRun);
		}

		internal void NoMoreGlobalParameters_DivideTotalsByCount() {
			foreach (OneParameterOneValue kpisForValue in AllValuesForOneParameter.Values) {
				kpisForValue.NoMoreGlobalParameters_DivideTotalsByCount();
			}

			this.AllValuesForOneParameterWithAverages = new List<OneParameterOneValue>(this.AllValuesForOneParameter.Values);

			this.ArtificialRowForAllKPIsAverage.NoMoreGlobalParameters_DivideTotalsByCount();
			this.ArtificialRowForAllKPIsAverage.KPIsGlobal.FormatStrings();
			this.AllValuesForOneParameterWithAverages.Add(this.ArtificialRowForAllKPIsAverage);

			//this.calcDispersionKPIsGlobal();
			//this.AllValuesForOneParameterWithAverages.Add(this.ArtificialRowForAllKPIsAverageDispersion);

			//this.calcExcentricityKPIsGlobal();
			//this.AllValuesForOneParameterWithAverages.Add(this.ArtificialRowForAllKPIsAverageExcentricity);
		}

		private void calcExcentricityKPIsGlobal() {
		}

		private void calcDispersionKPIsGlobal() {
		}
	}
}
