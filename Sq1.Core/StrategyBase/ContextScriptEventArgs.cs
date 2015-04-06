using System;

namespace Sq1.Core.StrategyBase {
	public class ContextScriptEventArgs : EventArgs {
		public ContextScript CtxCloneOfCurrentAbsorbedFromOptimizer {get; private set;}
		
		public ContextScriptEventArgs(ContextScript ContextScript) {
			this.CtxCloneOfCurrentAbsorbedFromOptimizer=ContextScript;
		}
	}
}
