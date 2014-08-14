using System;
using Newtonsoft.Json;

namespace Sq1.Core.StrategyBase {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class ScriptParameterAttribute : Attribute {
		public int Id;
		public string Name;
		public string ReasonToExist;
		public double ValueMin;
		public double ValueMax;
		public double ValueIncrement;
		public double ValueCurrent;

		public override string ToString() {
			return this.Name + ":" + this.ValueCurrent + "[" + this.ValueMin + ".." + this.ValueMax + "/" + this.ValueIncrement + "]";
		}
	}
}
