using System;

using Sq1.Core.DataTypes;

namespace Sq1.Core.StrategyBase {
	public partial class ScriptExecutor {
		public event EventHandler<QuoteEventArgs> StrategyExecutionComplete;
		
		public void RaiseStrategyExecutionComplete(Quote quote) {
			if (this.StrategyExecutionComplete == null) return;
			try {
				this.StrategyExecutionComplete(this, new QuoteEventArgs(quote));
			} catch (Exception e) {
				string msg = "EVENT_CONSUMER(USED_ONLY_FOR_LIVE_SIMULATOR)_THROWN //DataDistributor.RaiseQuotePushedToDistributor(" + quote + ")";
				Assembler.PopupException(msg, e);
			}
		}
	}
}
