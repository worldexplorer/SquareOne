using System;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Indicators {
	public class IndicatorParameter {
		public string Name;
		
		public float ValueCurrent;
		public float ValueMin;
		public float ValueMax;
		public float ValueIncrement;
		
		//public string ValueString;
		//public BarScaleInterval ValueBarScaleInterval;
		
		public IndicatorParameter(IndicatorParameterAttribute attr) {
			this.Name = attr.Name;
			//if (attr.ValueBarScaleInterval != null) {
			//    this.ValueBarScaleInterval = attr.ValueBarScaleInterval;
			//    return;
			//}
			//if (attr.ValueString != null) {
			//    this.ValueString = attr.ValueString;
			//    return;
			//}
			if (attr.ValueCurrent != null) {
				this.ValueCurrent = attr.ValueCurrent;
				this.ValueMin = attr.ValueMin;
				this.ValueMax = attr.ValueMax;
				this.ValueIncrement = attr.ValueIncrement;
				return;
			}
		}
		
		public IndicatorParameter(string name = "NAME_NOT_INITIALIZED",
				float valueCurrent = float.NaN, float valueMin = float.NaN, float valueMax = float.NaN, float valueIncrement = float.NaN) {
			this.Name = name;
			this.ValueCurrent = valueCurrent;
			this.ValueMin = valueMin;
			this.ValueMax = valueMax;
			this.ValueIncrement = valueIncrement;
		}
		//public IndicatorParameter(string name = "NAME_NOT_INITIALIZED", string value = "STRING_VALUE_NOT_INITIALIZED") {
		//    this.Name = name;
		//    this.ValueString = value;
		//}
		//public IndicatorParameter(string name = "NAME_NOT_INITIALIZED", BarScaleInterval value = null) {
		//    this.Name = name;
		//    this.ValueBarScaleInterval = value;
		//}
		public string ValidateSelf() {
			if (this.ValueMin > this.ValueMax)		return "ValueMin[" + this.ValueMin + "] > ValueMax[" + this.ValueMax + "]";
			if (this.ValueCurrent > this.ValueMax)	return "ValueCurrent[" + this.ValueCurrent + "] > ValueMax[" + this.ValueMax + "]";
			if (this.ValueCurrent < this.ValueMin)	return"ValueCurrent[" + this.ValueCurrent + "] < ValueMin[" + this.ValueMin + "]";
			return null;
		}
		public override string ToString() {
			return this.Name + ":" + this.ValueCurrent + "[" + this.ValueMin + ".." + this.ValueMax + "/" + this.ValueIncrement + "]";
		}
	}
}
