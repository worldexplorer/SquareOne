using System;
using Newtonsoft.Json;
using Sq1.Charting;
using Sq1.Core.StrategyBase;

namespace Sq1.Gui.Forms {
	public class ChartFormDataSnapshot {
		public string StrategyGuidJsonCheck;
		public string StrategyNameJsonCheck;
		
		[JsonIgnore]
		int chartSerno;
		public int ChartSerno {
			get { return this.chartSerno; }
			set {
				if (value == -1) return;
				if (value == this.chartSerno) return;
				if (this.chartSerno != -1) {
					string msg = "this.chartSerno[" + this.chartSerno + "] => value[" + value + "]"
						+ ": ChartSerno can be initialized only once in a lifetime"
						+ "(init-once from ChartFormManager.Initialize() after deserialization)";
					throw new Exception(msg);
				}
				this.chartSerno = value;
			}
		}

		public ContextChart ContextChart;
		
		//[JsonIgnore]	// not saved/restored yet; tune it up in ChartSettings.ChartSettings();
		public ChartSettings ChartSettings;

		public ChartFormDataSnapshot() {
			this.chartSerno = -1;
			this.StrategyGuidJsonCheck = "NOT_INITIALIZED ChartFormManager.Initialize()";
			this.StrategyNameJsonCheck = "NOT_INITIALIZED ChartFormManager.Initialize()";
			//this.ContextChart = new ContextChart();	// should be nullified when a strategy is loaded to the chart
		}
		
	}
}
