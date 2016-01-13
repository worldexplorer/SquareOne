using System;
using System.Text;

using Newtonsoft.Json;

using Sq1.Core.DataTypes;

namespace Sq1.Core.StrategyBase {
	public class ContextChart {
		[JsonProperty]	public string			Name;
		[JsonProperty]	public string			Symbol;
		[JsonProperty]	public string			DataSourceName;
		[JsonProperty]	public BarScaleInterval	ScaleInterval;
		[JsonProperty]	public BarDataRange		DataRange;

		[JsonProperty]	public bool				IsStreamingTriggeringScript;	// this should be in ContextScript !...
		[JsonProperty]	public bool				ShowRangeBar;
		[JsonProperty]	public bool				IsStreaming;

		public ContextChart(string name) : this() {
			Name = name;
		}
		public ContextChart() {
			string msig = "THIS_CTOR_IS_INVOKED_BY_JSON_DESERIALIZER__KEEP_ME_PUBLIC__CREATE_[JsonIgnore]d_VARIABLES_HERE";
			
			Name						= 			"NAME_UNDEFINED__CTX_JUST_CREATED";
			Symbol						= 		  "SYMBOL_UNDEFINED__CTX_JUST_CREATED";
			DataSourceName				= "DATASOURCENAME_UNDEFINED__CTX_JUST_CREATED";
			ScaleInterval				= new BarScaleInterval(BarScale.Unknown, -1);
			DataRange					= new BarDataRange(500);
			IsStreamingTriggeringScript	= false;
			ShowRangeBar				= false;
			IsStreaming					= false;
		}
		public void AbsorbFrom(ContextChart found) {
			if (found == null) return;
			//KEEP_CLONE_UNDEFINED this.Name	= found.Name;
			this.Symbol							= found.Symbol;
			this.DataSourceName					= found.DataSourceName;
			this.ScaleInterval					= found.ScaleInterval.Clone();
			this.DataRange						= found.DataRange.Clone();
			//this.ChartBarSpacing				= found.ChartBarSpacing;
			this.IsStreamingTriggeringScript	= found.IsStreamingTriggeringScript;
			this.ShowRangeBar					= found.ShowRangeBar;
		}
		public override string ToString() {
			//return this.Name;
			//v1
			//string ret = this.DataSourceName + " :: " + this.Symbol;
			//if (typeof(ContextChart) != this.GetType()) {	//append ContextScript name, not for ContextChart
			//	ret += " ctx/" + this.Name;
			//}
			//return ret;
			//v2
			StringBuilder sb = new StringBuilder();
			sb.Append(this.DataSourceName);
			sb.Append(" :: ");
			sb.Append(this.Symbol);
			sb.Append(" [");
			if (this.ScaleInterval != null) sb.Append(this.ScaleInterval.ToString());
			sb.Append("]");
			if (typeof(ContextChart) != this.GetType()) {	//append ContextScript name, not for ContextChart
				sb.Append(" ctx/");
				sb.Append(this.Name);
			}
			return sb.ToString();
		}
	}
}