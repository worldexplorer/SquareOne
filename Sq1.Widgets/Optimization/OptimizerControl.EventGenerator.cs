using System;

using Sq1.Core;
using Sq1.Core.StrategyBase;

namespace Sq1.Widgets.Optimization {
	public partial class OptimizerControl {
		public event EventHandler<ContextScriptEventArgs> OnCopyToContextDefault;
		public event EventHandler<ContextScriptEventArgs> OnCopyToContextDefaultBacktest;
		public event EventHandler<ContextScriptEventArgs> OnCopyToContextNew;
		public event EventHandler<ContextScriptEventArgs> OnCopyToContextNewBacktest;
		
		public void RaiseOnCopyToContextDefault(ContextScript ctxFromOptimizer) {
			if (this.OnCopyToContextDefault == null) return;
			try {
				this.OnCopyToContextDefault(this, new ContextScriptEventArgs(ctxFromOptimizer));
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnCopyToContextDefault(" + ctxFromOptimizer + ")", ex);
			}
		}
		public void RaiseOnCopyToContextDefaultBacktest(ContextScript ctxFromOptimizer) {
			if (this.OnCopyToContextDefaultBacktest == null) return;
			try {
				this.OnCopyToContextDefaultBacktest(this, new ContextScriptEventArgs(ctxFromOptimizer));
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnCopyToContextDefaultBacktest(" + ctxFromOptimizer + ")", ex);
			}
		}
		public void RaiseOnCopyToContextNew(ContextScript ctxFromOptimizer) {
			if (this.OnCopyToContextNew == null) return;
			try {
				this.OnCopyToContextNew(this, new ContextScriptEventArgs(ctxFromOptimizer));
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnCopyToContextNew(" + ctxFromOptimizer + ")", ex);
			}
		}
		public void RaiseOnCopyToContextNewBacktest(ContextScript ctxFromOptimizer) {
			if (this.OnCopyToContextNewBacktest == null) return;
			try {
				this.OnCopyToContextNewBacktest(this, new ContextScriptEventArgs(ctxFromOptimizer));
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnCopyToContextNewBacktest(" + ctxFromOptimizer + ")", ex);
			}
		}
	}
}
