using System;

namespace Sq1.Core.StrategyBase {
	public class ScriptParameterEventArgs : EventArgs {
		public ScriptParameter ScriptParameter { get; protected set; }
		public ScriptParameterEventArgs(ScriptParameter scriptParameter) {
			this.ScriptParameter = scriptParameter;
		}
	}
}
