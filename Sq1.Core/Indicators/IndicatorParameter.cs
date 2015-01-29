using System;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Sq1.Core.Indicators {
	public class IndicatorParameter {
		[JsonIgnore]	public string IndicatorName;
		[JsonIgnore]	public virtual string FullName { get { return this.IndicatorName + "." + this.Name; } } // "MAslow.Period" for indicators, plain Name for StrategyParams
		
		[JsonProperty]	public string Name;	// unlike user-editable ScriptParameter, IndicatorParameter.Name is compiled and remains constant (no need for Id)
		[JsonProperty]	public double ValueMin;
		[JsonProperty]	public double ValueMax;
		[JsonProperty]	public double ValueIncrement;
		[JsonProperty]	public double ValueCurrent;
		
		// NOT_USED_YET; waiting for optimizer
//		[JsonIgnore]	public bool IsInteger { get {
//				return this.ValueMin == (double)((int)this.ValueMin)
//					&& this.ValueMax == (double)((int)this.ValueMax)
//					&& this.ValueIncrement == (double)((int)this.ValueIncrement)
//					&& this.ValueCurrent == (double)this.ValueCurrent;
//			} }
		[JsonIgnore]	public int NumberOfRuns { get {
				if (this.ValueIncrement <= 0.0) return 1;
				int ret = (int)Math.Floor((this.ValueMax - this.ValueMin) / this.ValueIncrement) + 1;
                return ret;
			} }

		//public string ValueString;
		//public BarScaleInterval ValueBarScaleInterval;

		[JsonProperty]	public bool BorderShown;
		[JsonProperty]	public bool NumericUpdownShown;

		// DESPITE_NOT_INVOKED_EXPLICITLY__I_GUESS_INITIALIZING_VALUES_USING_OTHER_CONSTRUCTOR_MAY_CORRUPT_JSON_DESERIALIZATION
		public IndicatorParameter() {
			BorderShown = false;
			NumericUpdownShown = true;
		}
		public IndicatorParameter(string name = "NAME_NOT_INITIALIZED",
		                          double valueCurrent = double.NaN, double valueMin = double.NaN, double valueMax = double.NaN, double valueIncrement = double.NaN) : this() {
			Name = name;
			ValueCurrent = valueCurrent;
			ValueMin = valueMin;
			ValueMax = valueMax;
			ValueIncrement = valueIncrement;
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
			return this.FullName + ":" + this.ValuesAsString;
		}
		public string ValuesAsString { get {
				return this.ValueCurrent + "[" + this.ValueMin + ".." + this.ValueMax + "/" + this.ValueIncrement + "]";
			} }
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
            if (ctxParamToAbsorbCurrentAndFixBoundaries.ValueIncrement != this.ValueIncrement) {
				#if DEBUG
				//Debugger.Break();
				#endif
                ctxParamToAbsorbCurrentAndFixBoundaries.ValueIncrement = this.ValueIncrement;
			}
		}
		// USED_TO_SEPARATE_LONG_LIVING_SCRIPT_INDICATOR_PARAMETER_INSTANCE__FROM_SWITCHING_CONTEXT_INDICATOR_SETTINGS
		public IndicatorParameter Clone() {
			return (IndicatorParameter)base.MemberwiseClone();
		}
	}
}
