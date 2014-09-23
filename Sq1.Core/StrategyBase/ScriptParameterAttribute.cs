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

		public ScriptParameterAttribute() {
			ValueMin = 0;
			ValueMax = 10;
			ValueIncrement = 1;
			ValueCurrent = 5;
		}
		
		public override string ToString() {
			return this.Name + ":" + this.ValueCurrent + "[" + this.ValueMin + ".." + this.ValueMax + "/" + this.ValueIncrement + "]";
		}
	}
}
