using System;

using Sq1.Core.Backtesting;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Livesim {
	public class LivesimDataSource : BacktestDataSource, IDisposable {
		public ScriptExecutor Executor;

		public LivesimStreaming		StreamingAsLivesimNullUnsafe	{ get { return base.StreamingAdapter	as LivesimStreaming; } }
		public LivesimBroker		BrokerAsLivesimNullUnsafe		{ get { return base.BrokerAdapter		as LivesimBroker; } }

		LivesimDataSource() {
			base.Name				= "LivesimDataSource";
			base.StreamingAdapter	= new LivesimStreaming(this);
			base.BrokerAdapter		= new LivesimBroker(this);
		}

		public LivesimDataSource(ScriptExecutor executor) : this() {
			this.Executor = executor;
		}

		public void SubstituteAdapters(LivesimStreaming liveStreamingChild, LivesimBroker liveBrokerChild) {
			base.StreamingAdapter	= liveStreamingChild;
			base.BrokerAdapter		= liveBrokerChild;
		}

		public void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE__" + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
			if (this.StreamingAsLivesimNullUnsafe != null) {
				this.StreamingAsLivesimNullUnsafe.Dispose();
				base.StreamingAdapter = null;
			}
			if (this.BrokerAsLivesimNullUnsafe != null) {
				this.BrokerAsLivesimNullUnsafe.Dispose();
				base.BrokerAdapter = null;
			}
			this.IsDisposed = true;
		}
		public bool IsDisposed { get; private set; }
	}
}