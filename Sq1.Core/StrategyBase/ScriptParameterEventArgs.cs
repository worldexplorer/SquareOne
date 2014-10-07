using System;
using Sq1.Core.Indicators;

namespace Sq1.Core.StrategyBase {
	public class ScriptParameterEventArgs : IndicatorParameterEventArgs {
		public ScriptParameter ScriptParameter { get { return base.IndicatorParameter as ScriptParameter; } }
		public ScriptParameterEventArgs(ScriptParameter scriptParameter) : base(scriptParameter) {
		}
	}
}
