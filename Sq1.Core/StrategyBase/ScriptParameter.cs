using System;
using Newtonsoft.Json;

namespace Sq1.Core.StrategyBase {
	public class ScriptParameter {
		[JsonProperty]	public int Id;
		[JsonProperty]	public string Name;
		[JsonProperty]	public string ReasonToExist;
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
		private ScriptParameter() {}
		public ScriptParameter(int id, string name, double current, double min, double max, double increment, string reasonToExist) : this() {
			this.Id = id;
			this.Name = name;
			this.ValueMin = min;
			this.ValueMax = max;
			this.ValueIncrement = increment;
			this.ValueCurrent = current;
			this.ReasonToExist = reasonToExist;
		}
		public ScriptParameter(int id, string name, double current, double min, double max, double increment)
			: this(id, name, current, min, max, increment, "") {
		}
		public override string ToString() {
			return this.Name + ":" + this.ValueCurrent + "[" + this.ValueMin + ".." + this.ValueMax + "/" + this.ValueIncrement + "]";
		}
		public ScriptParameter Clone() {
			return (ScriptParameter)base.MemberwiseClone();
		}
	}
}
