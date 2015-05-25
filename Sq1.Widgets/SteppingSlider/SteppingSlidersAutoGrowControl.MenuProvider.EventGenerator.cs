using System;

using Sq1.Core.StrategyBase;

namespace Sq1.Widgets.SteppingSlider {
	public partial class SteppingSlidersAutoGrowControl {
		public event EventHandler<StrategyEventArgs> ScriptContextLoadRequestedSubscriberImplementsCurrentSwitch;
		public event EventHandler<StrategyEventArgs> ScriptContextRenamed;
		public event EventHandler<StrategyEventArgs> ScriptContextDuplicated;
		public event EventHandler<StrategyEventArgs> ScriptContextDeleted;
		public event EventHandler<StrategyEventArgs> ScriptContextNewDefaultCreated;

		void RaiseOnScriptContextLoadRequested(string ctxToLoadName) {
			if (this.ScriptContextLoadRequestedSubscriberImplementsCurrentSwitch == null) return;
			StrategyEventArgs args = new StrategyEventArgs(this.Strategy.StoredInFolderRelName, this.Strategy, ctxToLoadName);
			this.ScriptContextLoadRequestedSubscriberImplementsCurrentSwitch(this, args);
		}
		void RaiseOnScriptContextRenamed(string ctxRenamedName) {
			if (this.ScriptContextRenamed == null) return;
			StrategyEventArgs args = new StrategyEventArgs(this.Strategy.StoredInFolderRelName, this.Strategy, ctxRenamedName);
			this.ScriptContextRenamed(this, args);
		}
		void RaiseOnScriptContextDuplicated(string ctxRenamedName) {
			if (this.ScriptContextDuplicated == null) return;
			StrategyEventArgs args = new StrategyEventArgs(this.Strategy.StoredInFolderRelName, this.Strategy, ctxRenamedName);
			this.ScriptContextDuplicated(this, args);
		}
		void RaiseOnScriptContextDeleted(string ctxRenamedName) {
			if (this.ScriptContextDeleted == null) return;
			StrategyEventArgs args = new StrategyEventArgs(this.Strategy.StoredInFolderRelName, this.Strategy, ctxRenamedName);
			this.ScriptContextDeleted(this, args);
		}
		void RaiseOnScriptContextCreated(string ctxRenamedName) {
			if (this.ScriptContextNewDefaultCreated == null) return;
			StrategyEventArgs args = new StrategyEventArgs(this.Strategy.StoredInFolderRelName, this.Strategy, ctxRenamedName);
			this.ScriptContextNewDefaultCreated(this, args);
		}

	}
}