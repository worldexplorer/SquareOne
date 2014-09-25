using System;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Indicators {
	public class IndicatorParameter {
		public string Name;
		
		public double ValueCurrent;
		public double ValueMin;
		public double ValueMax;
		public double ValueIncrement;
		
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
				ValueCurrent = attr.ValueCurrent;
				ValueMin = attr.ValueMin;
				ValueMax = attr.ValueMax;
				ValueIncrement = attr.ValueIncrement;
				return;
			}
		}
		
		public IndicatorParameter(string name = "NAME_NOT_INITIALIZED",
				double valueCurrent = double.NaN, double valueMin = double.NaN, double valueMax = double.NaN, double valueIncrement = double.NaN) {
			Name = name;
			ValueCurrent = valueCurrent;
			ValueMin = valueMin;
			ValueMax = valueMax;
			ValueIncrement = valueIncrement;
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
