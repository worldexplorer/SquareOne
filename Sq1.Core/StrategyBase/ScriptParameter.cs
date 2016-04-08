using System;

using Newtonsoft.Json;

using Sq1.Core.Indicators;

namespace Sq1.Core.StrategyBase {
	public class ScriptParameter : IndicatorParameter {
		[JsonProperty]	public int				Id;
		[JsonProperty]	public string			ReasonToExist;
		[JsonIgnore]	public override string	FullName { get { return this.Name; } }	// "MAslow.Period" for indicators, plain Name for StrategyParams

		private ScriptParameter() {
			string msg = "default initialization values for min/max/increment/current are located in ScriptParameterAttribute";
		}
		public ScriptParameter(int id, string name, double current, double min, double max, double increment, string reasonToExist)
				: base(name, current, min, max, increment) {
			Id = id;
			ReasonToExist = reasonToExist;
			ReasonToClone = "";
			IndicatorName = "THIS_IS_A_STRATEGY_PARAMETER";
		}
		//[Obsolete("Sorry bro no parameter without the reason to exist => hopefully will go to Slider's tooltip");
		//public ScriptParameter(int id, string name, double current, double min, double max, double increment)
		//	: this(id, name, current, min, max, increment, "") {
		//}
		//public override string ToString() {
		//	return this.Name + ":" + this.ValueCurrent + "[" + this.ValueMin + ".." + this.ValueMax + "/" + this.ValueIncrement + "]";
		//}
		public ScriptParameter CloneAsScriptParameter(string reasonToClone) {
			ScriptParameter ret = (ScriptParameter)base.MemberwiseClone();
			ret.ReasonToClone = "CLONE[" + reasonToClone + "]_" + ret.ReasonToClone;
			return ret;
		}
	}
}
