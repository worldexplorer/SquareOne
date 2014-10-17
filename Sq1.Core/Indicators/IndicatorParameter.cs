using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Indicators {
	public class IndicatorParameter {
		[JsonProperty]	public string Name;	// unlike user-editable ScriptParameter, IndicatorParameter.Name is compiled and remains constant (no need for Id)
		[JsonProperty]	public double ValueMin;
		[JsonProperty]	public double ValueMax;
		[JsonProperty]	public double ValueIncrement;
		[JsonProperty]	public double ValueCurrent;
		
		[JsonIgnore]	public bool IsInteger { get {
				return this.ValueMin == (double)((int)this.ValueMin)
					&& this.ValueMax == (double)((int)this.ValueMax)
					&& this.ValueIncrement == (double)((int)this.ValueIncrement)
					&& this.ValueCurrent == (double)this.ValueCurrent;
			} }
		[JsonIgnore]	public int NumberOfRuns { get {
				if (this.ValueIncrement <= 0.0) return 1;
				return (int)Math.Round(((this.ValueMax - this.ValueMin) / this.ValueIncrement) + 1.0);
			} }

		//public string ValueString;
		//public BarScaleInterval ValueBarScaleInterval;

		[JsonProperty]	public bool BorderShown;
		[JsonProperty]	public bool NumericUpdownShown;

		
		public IndicatorParameter(string name = "NAME_NOT_INITIALIZED",
				double valueCurrent = double.NaN, double valueMin = double.NaN, double valueMax = double.NaN, double valueIncrement = double.NaN) {
			Name = name;
			ValueCurrent = valueCurrent;
			ValueMin = valueMin;
			ValueMax = valueMax;
			ValueIncrement = valueIncrement;
			BorderShown = false;
			NumericUpdownShown = true;
		}
		//public IndicatorParameter(string name = "NAME_NOT_INITIALIZED", string value = "STRING_VALUE_NOT_INITIALIZED") {
		//	this.Name = name;
		//	this.ValueString = value;
		//}
		//public IndicatorParameter(string name = "NAME_NOT_INITIALIZED", BarScaleInterval value = null) {
		//	this.Name = name;
		//	this.ValueBarScaleInterval = value;
		//}
		public string ValidateSelf() {
			if (this.ValueMin > this.ValueMax)		return "ValueMin[" + this.ValueMin + "] > ValueMax[" + this.ValueMax + "]";
			if (this.ValueCurrent > this.ValueMax)	return "ValueCurrent[" + this.ValueCurrent + "] > ValueMax[" + this.ValueMax + "]";
			if (this.ValueCurrent < this.ValueMin)	return "ValueCurrent[" + this.ValueCurrent + "] < ValueMin[" + this.ValueMin + "]";
			return null;
		}
		public override string ToString() {
			return this.Name + ":" + this.ValueCurrent + "[" + this.ValueMin + ".." + this.ValueMax + "/" + this.ValueIncrement + "]";
		}
		public void AbsorbCurrentFixBoundariesIfChanged(IndicatorParameter ctxParamToAbsorbCurrentAndFixBoundaries) {
			if (this.ValueCurrent != ctxParamToAbsorbCurrentAndFixBoundaries.ValueCurrent) {
				string msg = "we collapsed IndicatorParameters into a single instance thing; are we back to duplicates?...";
				//Debugger.Break();
				this.ValueCurrent = ctxParamToAbsorbCurrentAndFixBoundaries.ValueCurrent;
			}
			if (this.ValueCurrent < this.ValueMin || this.ValueCurrent > this.ValueMax)  {
				#if DEBUG
				Debugger.Break();
				#endif
				this.ValueCurrent = this.ValueMin;
			}
			if (ctxParamToAbsorbCurrentAndFixBoundaries.ValueMin != this.ValueMin) {
				#if DEBUG
				Debugger.Break();
				#endif
				ctxParamToAbsorbCurrentAndFixBoundaries.ValueMin  = this.ValueMin;
			}
			if (ctxParamToAbsorbCurrentAndFixBoundaries.ValueMax != this.ValueMax) {
				#if DEBUG
				Debugger.Break();
				#endif
				ctxParamToAbsorbCurrentAndFixBoundaries.ValueMax  = this.ValueMax;
			}
		}
	}
}
