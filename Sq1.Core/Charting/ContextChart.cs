using System;
using System.Runtime.Serialization;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.StrategyBase {
	[DataContract]
	public class ContextChart {
		[DataMember] public string Name;
		[DataMember] public string Symbol;
		[DataMember] public string DataSourceName;
		[DataMember] public BarScaleInterval ScaleInterval;
		[DataMember] public BarDataRange DataRange;

		//public int ChartBarSpacing;
		[DataMember] public bool ChartStreaming;
		[DataMember] public bool ShowRangeBar;

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
			return this.DataSourceName + " :: " + this.Symbol;
		}
	}
}