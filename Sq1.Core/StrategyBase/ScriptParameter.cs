using System;
using Newtonsoft.Json;
using Sq1.Core.Indicators;

namespace Sq1.Core.StrategyBase {
	public class ScriptParameter : IndicatorParameter {
		[JsonProperty]	public int Id;
		[JsonProperty]	public string ReasonToExist;
		[JsonIgnore]	public override string FullName { get { return this.Name; } }	// "MAslow.Period" for indicators, plain Name for StrategyParams

		private ScriptParameter() {
			string msg = "default initialization values for min/max/increment/current are located in ScriptParameterAttribute";
		}
		public ScriptParameter(int id, string name, double current, double min, double max, double increment, string reasonToExist)
				: base(name, current, min, max, increment) {
			this.Id = id;
			this.ReasonToExist = reasonToExist;
			base.IndicatorName = "THIS_IS_A_STRATEGY_PARAMETER";
		}
		public ScriptParameter(int id, string name, double current, double min, double max, double increment)
			: this(id, name, current, min, max, increment, "") {
		}
//		public override string ToString() {
//			return this.Name + ":" + this.ValueCurrent + "[" + this.ValueMin + ".." + this.ValueMax + "/" + this.ValueIncrement + "]";
//		}
		public ScriptParameter Clone() {
			return (ScriptParameter)base.MemberwiseClone();
		}
	}
}
