using System;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Indicators {
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class IndicatorParameterAttribute : Attribute {
		public string Name;
		
		public float ValueCurrent;
		public float ValueMin;
		public float ValueMax;
		public float ValueIncrement;
		
		//public string ValueString;
		//public BarScaleInterval ValueBarScaleInterval;
		
		public IndicatorParameterAttribute(string name = "NAME_NOT_INITIALIZED",
				float valueCurrent = float.NaN, float valueMin = float.NaN, float valueMax = float.NaN, float valueIncrement = float.NaN) {
			this.Name = name;
			this.ValueCurrent = valueCurrent;
			this.ValueMin = valueMin;
			this.ValueMax = valueMax;
			this.ValueIncrement = valueIncrement;
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
