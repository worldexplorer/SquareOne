using System;

using Sq1.Core;
using Sq1.Core.StrategyBase;

namespace Sq1.Widgets.Optimization {
	public partial class OptimizerControl {
		public event EventHandler<ContextScriptEventArgs> OnCopyToContextDefault;
		public event EventHandler<ContextScriptEventArgs> OnCopyToContextDefaultBacktest;
		public event EventHandler<ContextScriptEventArgs> OnCopyToContextNew;
		public event EventHandler<ContextScriptEventArgs> OnCopyToContextNewBacktest;
		
		public void RaiseOnCopyToContextDefault(ContextScript ctxCloneOfCurrentAbsorbedFromOptimizer) {
			if (this.OnCopyToContextDefault == null) return;
			try {
				this.OnCopyToContextDefault(this, new ContextScriptEventArgs(ctxCloneOfCurrentAbsorbedFromOptimizer));
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnCopyToContextDefault(" + ctxCloneOfCurrentAbsorbedFromOptimizer + ")", ex);
			}
		}
		public void RaiseOnCopyToContextDefaultBacktest(ContextScript ctxCloneOfCurrentAbsorbedFromOptimizer) {
			if (this.OnCopyToContextDefaultBacktest == null) return;
			try {
				this.OnCopyToContextDefaultBacktest(this, new ContextScriptEventArgs(ctxCloneOfCurrentAbsorbedFromOptimizer));
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnCopyToContextDefaultBacktest(" + ctxCloneOfCurrentAbsorbedFromOptimizer + ")", ex);
			}
		}
		public void RaiseOnCopyToContextNew(ContextScript ctxCloneOfCurrentAbsorbedFromOptimizer) {
			if (this.OnCopyToContextNew == null) return;
			try {
				this.OnCopyToContextNew(this, new ContextScriptEventArgs(ctxCloneOfCurrentAbsorbedFromOptimizer));
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnCopyToContextNew(" + ctxCloneOfCurrentAbsorbedFromOptimizer + ")", ex);
			}
		}
		public void RaiseOnCopyToContextNewBacktest(ContextScript ctxCloneOfCurrentAbsorbedFromOptimizer) {
			if (this.OnCopyToContextNewBacktest == null) return;
			try {
				this.OnCopyToContextNewBacktest(this, new ContextScriptEventArgs(ctxCloneOfCurrentAbsorbedFromOptimizer));
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnCopyToContextNewBacktest(" + ctxCloneOfCurrentAbsorbedFromOptimizer + ")", ex);
			}
		}
	}
}
