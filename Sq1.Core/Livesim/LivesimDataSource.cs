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
			base.StreamingAdapter	= new LivesimStreaming(true);
			base.BrokerAdapter		= new LivesimBroker(true);
		}

		public LivesimDataSource(ScriptExecutor executor) : this() {
			this.Executor = executor;
		}

		public void PropagatePreInstantiatedLivesimAdapter_intoLivesimDataSource() {
			string msig = " //PushPreInstantiatedLivesimAdaptersToLivesimDataSource() [" + this.ToString() + "]";
			if (this.Executor.DataSource_fromBars.StreamingAdapter != null) {
				base.StreamingAdapter	= this.Executor.DataSource_fromBars.StreamingAdapter.LivesimStreaming;
			    string msg1 = "STREAMING_SUBSTITUTED_FOR_LIVESIM_DATASOURCE";
			    Assembler.PopupException(msg1 + msig, null, false);
			} else {
				if (this.StreamingAsLivesimNullUnsafe == null) {
					string msg1 = "I_REFUSE_TO_RUN_LIVESIM_WITHOUT_LIVESIMSTREAMING"
						+ " LivesimDataSource.ctor() should have created its own basic LivesimStreaming<=BacktestStreaming, now NULL";
					Assembler.PopupException(msg1);
				} else {
					string msg1 = "USING_LivesimStreaming (no streaming chosen in DataSourceEditor), will use QuoteGen=>Pump straight;"
						+ " StreamingAsLivesimNullUnsafe[" + this.StreamingAsLivesimNullUnsafe + "]";
				}
			}
			if (this.Executor.DataSource_fromBars.BrokerAdapter != null) {
				base.BrokerAdapter		= this.Executor.DataSource_fromBars.BrokerAdapter.LivesimBroker;
			    string msg1 = "BROKER_SUBSTITUTED_FOR_LIVESIM_DATASOURCE";
			    Assembler.PopupException(msg1 + msig, null, false);
			} else {
				if (this.BrokerAsLivesimNullUnsafe == null) {
					string msg2 = "I_REFUSE_TO_RUN_LIVESIM_WITHOUT_LIVESIMBROKER"
						+ " LivesimDataSource.ctor() should have created its own basic LivesimBroker<=BacktestBroker, now NULL";
					Assembler.PopupException(msg2);
				} else {
					string msg1 = "USING_LivesimBroker (no broker chosen in DataSourceEditor), will use OrderProcessor straight;"
						+ " BrokerAsLivesimNullUnsafe[" + this.BrokerAsLivesimNullUnsafe + "]";
				}
			}
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