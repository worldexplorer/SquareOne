using System;

using Sq1.Core;
using Sq1.Core.Backtesting;
using Sq1.Core.StrategyBase;
using Sq1.Core.Livesim;

namespace Sq1.Adapters.QuikMock {
	public class LivesimDataSourceQuik : BacktestDataSource, IDisposable {
		public ScriptExecutor Executor;

		public LivesimStreaming		StreamingAsLivesimNullUnsafe	{ get { return base.StreamingAdapter	as LivesimStreaming; } }
		public LivesimBroker		BrokerAsLivesimNullUnsafe		{ get { return base.BrokerAdapter		as LivesimBroker; } }

		public LivesimDataSourceQuik() {
			base.Name				= "LivesimDataSourceQuik";
			base.StreamingAdapter	= new LivesimStreamingQuik(this);
			base.BrokerAdapter		= new LivesimBrokerQuik(this);
		}

		public LivesimDataSourceQuik(ScriptExecutor executor) : this() {
			this.Executor = executor;
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