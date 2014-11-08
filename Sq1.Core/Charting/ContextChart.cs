using System;

using Sq1.Core.DataTypes;
using Newtonsoft.Json;

namespace Sq1.Core.StrategyBase {
	public class ContextChart {
		[JsonProperty]	public string Name;
		[JsonProperty]	public string Symbol;
		[JsonProperty]	public string DataSourceName;
		[JsonProperty]	public BarScaleInterval ScaleInterval;
		[JsonProperty]	public BarDataRange DataRange;

		//public int ChartBarSpacing;
		[JsonProperty]	public bool ChartStreaming;
		[JsonProperty]	public bool ShowRangeBar;

		public ContextChart(string name = "UNDEFINED") : this() {
			Name = name;
		}
		protected ContextChart() {
			Name = "UNDEFINED";
			Symbol = "UNDEFINED";
			DataSourceName = "UNDEFINED";
			ScaleInterval = new BarScaleInterval();
			DataRange = new BarDataRange(500);
			//ChartBarSpacing = 6;
			ChartStreaming = false;
			ShowRangeBar = false;
		}
		public void AbsorbFrom(ContextChart found) {
			if (found == null) return;
			//KEEP_CLONE_UNDEFINED this.Name = found.Name;
			this.Symbol = found.Symbol;
			this.DataSourceName = found.DataSourceName;
			this.ScaleInterval = found.ScaleInterval.Clone();
			this.DataRange = found.DataRange.Clone();
			//this.ChartBarSpacing = found.ChartBarSpacing;
			this.ChartStreaming = found.ChartStreaming;
			this.ShowRangeBar = found.ShowRangeBar;
		}
//		public ChartContext MemberwiseClone() {
//			return (ChartContext)base.MemberwiseClone();
//		}
		public override string ToString() {
			//return this.Name;
			string ret = this.DataSourceName + " :: " + this.Symbol;
			if (typeof(ContextChart) != this.GetType()) {	//append ContextScript name, not for ContextChart
				ret += " ctx/" + this.Name;
			}
			return ret;
		}
	}
}