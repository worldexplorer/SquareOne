using System.Collections.Generic;

using Sq1.Core.Optimization;

namespace Sq1.Widgets.Optimization {
	public class OneParameterOneValue {
		public bool UserSelected;

		public string ParameterName { get; private set; }
		public string ArtificialName { get; private set; }
		public bool IsArtificialRow { get { return string.IsNullOrEmpty(this.ArtificialName) == false; } }

		public double OptimizedValue { get; private set; }
		public string ParameterNameValue { get {
				if (this.IsArtificialRow) return this.ArtificialName;			//"Average" / "Dispersion""
				string ret = this.ParameterName + "=" + this.OptimizedValue;	//"MaFast.Period=10"
				return ret;
			} }
		public List<int> OptimizationIterationsAveraged { get; private set; }

		public KPIs KPIsGlobal { get; private set; }
		public KPIs KPIsLocal { get; private set; }

		public OneParameterOneValue() {
			UserSelected = true;
			OptimizationIterationsAveraged = new List<int>();
		}

		public OneParameterOneValue(string parameterName, double optimizedValue, string artificialName = null) : this() {
			this.ParameterName = parameterName;
			this.OptimizedValue = optimizedValue;
			this.ArtificialName = artificialName;
		}

		internal void AddKPIsGlobal(SystemPerformanceRestoreAble eachRun) {
			if (this.KPIsGlobal == null) {
				this.KPIsGlobal = new KPIs(eachRun);
			} else {
				this.KPIsGlobal.AddKPIs(eachRun);
			}
			if (this.OptimizationIterationsAveraged.Contains(eachRun.OptimizationIterationSerno)) {
				int a = 1;
			}
			this.OptimizationIterationsAveraged.Add(eachRun.OptimizationIterationSerno);
		}

		internal void NoMoreGlobalParameters_DivideTotalsByCount() {
			KPIsGlobal.NoMoreParameters_DivideTotalByCount(this.OptimizationIterationsAveraged.Count);
			KPIsLocal = KPIsGlobal.Clone();
		}
	}
}
