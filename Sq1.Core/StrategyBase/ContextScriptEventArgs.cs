using System;

namespace Sq1.Core.StrategyBase {
	public class ContextScriptEventArgs : EventArgs {
		public ContextScript ContextScript {get; private set;}
		public ContextScriptEventArgs(ContextScript ContextScript) {
			this.ContextScript=ContextScript;
		}
	}
}
