using System;

using Sq1.Core.Backtesting;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Livesim {
	public class LivesimDataSource : BacktestDataSource, IDisposable {
		public ScriptExecutor		Executor						{ get; private set; }

		public LivesimStreaming		StreamingAsLivesimNullUnsafe	{ get { return base.StreamingAdapter	as LivesimStreaming; } }
		public LivesimBroker		BrokerAsLivesimNullUnsafe		{ get { return base.BrokerAdapter		as LivesimBroker; } }

		LivesimDataSource() {
			base.Name				= "LivesimDataSource";
		}

		public LivesimDataSource(ScriptExecutor executor) : this() {
			this.Executor = executor;
		}

		public void PropagatePreInstantiatedLivesimAdapter_intoLivesimDataSource() {
			string msig = " //PushPreInstantiatedLivesimAdaptersToLivesimDataSource() [" + this.ToString() + "]";

			//TOO_MANY_ALREADY_DISPOSED_EXCEPTIONS SEEMS_TO_BE_SAME_DUMMY_ACROSS_MANY_DATASOURCES_POINTING_TO_IT
			//this.StreamingAsLivesimNullUnsafe.Dispose();
			//this.   BrokerAsLivesimNullUnsafe.Dispose();

			base.StreamingAdapter	= base.StreamingAdapter.LivesimStreaming;
			base.BrokerAdapter		= base.BrokerAdapter.LivesimBroker;

		    string msg1 = "ADAPTERS_SUBSTITUTED_FOR_LIVESIM_DATASOURCE";
		    Assembler.PopupException(msg1 + msig, null, false);
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