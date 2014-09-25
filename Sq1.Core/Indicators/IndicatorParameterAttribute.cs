using System;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Indicators {
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class IndicatorParameterAttribute : Attribute {
		public string Name;
		
		public double ValueCurrent;
		public double ValueMin;
		public double ValueMax;
		public double ValueIncrement;
		
		//public string ValueString;
		//public BarScaleInterval ValueBarScaleInterval;
		
		public IndicatorParameterAttribute(string name = "NAME_NOT_INITIALIZED",
				double valueCurrent = double.NaN, double valueMin = double.NaN, double valueMax = double.NaN, double valueIncrement = double.NaN) {
			Name = name;
			ValueCurrent = valueCurrent;
			ValueMin = valueMin;
			ValueMax = valueMax;
			ValueIncrement = valueIncrement;
		}
		//public IndicatorParameterAttribute(string name = "NAME_NOT_INITIALIZED", string value = "STRING_VALUE_NOT_INITIALIZED") {
		//    this.Name = name;
		//    this.ValueString = value;
		//}
		//public IndicatorParameterAttribute(string name = "NAME_NOT_INITIALIZED", BarScaleInterval value = null) {
		//    this.Name = name;
		//    this.ValueBarScaleInterval = value;
		//}
		public override string ToString() {
			return this.Name + ":" + this.ValueCurrent + "[" + this.ValueMin + ".." + this.ValueMax + "/" + this.ValueIncrement + "]";
		}
	}
}
