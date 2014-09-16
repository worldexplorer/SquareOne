using System;
using Newtonsoft.Json;
using Sq1.Charting;
using Sq1.Core;
using Sq1.Core.StrategyBase;

namespace Sq1.Gui.Forms {
	public class ChartFormDataSnapshot {
		[JsonProperty]	public string StrategyGuidJsonCheck;
		[JsonProperty]	public string StrategyNameJsonCheck;
		[JsonProperty]	public string StrategyAbsPathJsonCheck;
		
		[JsonIgnore]	int chartSerno;
		[JsonProperty]	public int ChartSerno {
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

		[JsonProperty]	public ContextChart ContextChart;
		[JsonProperty]	public string ContextChartCommentJsonCheck { get {
				string ret = "THIS_CHARTFORM_HAS_NO_STRATEGY_ATTACHED_SO_this.ContextChart_CONTAINS_CURRENT_BARSCALEINTEVAL_DATARANGE";

				// KISS lazy to figure out if I assigned a derived-casted-to-parent-before-assignment - will AS work or I have to use GetType() / typeof....
				//ContextScript contextScript = this.ContextChart as ContextScript;
				//if (contextScript == null) return ret;

				if (this.ContextChart == null) return ret;

				ret = "THIS_CHARTFORM_CONTAINS_STRATEGY_SO_this.ContextChart_IS_NULL check [" + this.StrategyAbsPathJsonCheck + "], ScriptContextCurrentName[ScriptContextCurrentName]";
				return ret;
			} }
		
		//[JsonIgnore]	// not saved/restored during severe storms in ChartSettings' structure (avoiding Json deserialization exceptions by JsonIgnoring it)
		public ChartSettings ChartSettings;

		public ChartFormDataSnapshot() {
			this.chartSerno = -1;
			this.StrategyGuidJsonCheck		= "NOT_INITIALIZED ChartFormManager.Initialize()";
			this.StrategyNameJsonCheck		= "NOT_INITIALIZED ChartFormManager.Initialize()";
			this.StrategyAbsPathJsonCheck	= "NOT_INITIALIZED ChartFormManager.Initialize()";
			//this.ContextChart = new ContextChart();	// should be nullified when a strategy is loaded to the chart
		}
		
	}
}
