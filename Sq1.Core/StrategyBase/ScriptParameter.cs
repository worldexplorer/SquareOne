using System;

using Newtonsoft.Json;

using Sq1.Core.Indicators;

namespace Sq1.Core.StrategyBase {
	public class ScriptParameter : IndicatorParameter {
		[JsonProperty]	public int				Id;
		[JsonIgnore]	public override string	FullName { get { return this.Name; } }	// "MAslow.Period" for indicators, plain Name for StrategyParams

		ScriptParameter() {
			//string msg = "default initialization values for min/max/increment/current are located in ScriptParameterAttribute";
			string msg = "invoked by JSON_DESERIALIZER, I guess...";
		}
		public ScriptParameter(int id, string name, double current, double min, double max, double increment, string reasonToExist)
				: base(name, current, min, max, increment, reasonToExist) {
			this.Id = id;
			base.IndicatorName = "THIS_IS_A_STRATEGY_PARAMETER";
			base.Owner_asString = "DESERIALIZED_ScriptParameter";
		}

		public ScriptParameter Clone_asScriptParameter(string reasonToClone, string owner_asString) {
			ScriptParameter ret = (ScriptParameter)base.MemberwiseClone();
			ret.ReasonToClone = "CLONE[" + reasonToClone + "]_" + ret.ReasonToClone;
			ret.Owner_asString = owner_asString;
			ret.ClonedFrom = this;		// if ctx.Indicator parameters would be peeled off from Script-reflected, pushing from Sliders would have a backreference... nemishe?... 
			return ret;
		}
	}
}
