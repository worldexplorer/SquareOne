using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using Newtonsoft.Json;

using Sq1.Core.Accounting;
using Sq1.Core.Broker;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;
using Sq1.Core.Backtesting;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Support;

namespace Sq1.Core.Livesim {
	// I_WANT_LIVESIM_STREAMING_BROKER_BE_AUTOASSIGNED_AND_VISIBLE_IN_DATASOURCE_EDITOR [SkipInstantiationAt(Startup = true)]
	[SkipInstantiationAt(Startup = true)]
	public abstract partial class LivesimBroker : BrokerAdapter, IDisposable {
		[JsonIgnore]	public		ScriptExecutor				ScriptExecutor							{ get; private set; }
		[JsonIgnore]	public		BacktestMarketsim			LivesimMarketsim						{ get; protected set; }

		[JsonIgnore]	public		List<Order>					OrdersSubmitted_forOneLivesimBacktest	{ get; private set; }
		[JsonIgnore]	protected	LivesimDataSource			LivesimDataSource						{ get { return base.DataSource as LivesimDataSource; } }
		[JsonIgnore]	internal	LivesimBrokerSettings		LivesimBrokerSettings					{ get { return this.ScriptExecutor.Strategy.LivesimBrokerSettings; } }
		[JsonIgnore]	public		LivesimBrokerDataSnapshot	DataSnapshot;
		[JsonIgnore]	protected	LivesimBrokerSpoiler		LivesimBrokerSpoiler;

		[JsonIgnore]	public		bool						IsDisposed								{ get; private set; }

		public LivesimBroker(string reasonToExist) : base(reasonToExist) {
			base.Name									= "LivesimBroker";
			this.LivesimMarketsim						= new BacktestMarketsim(this);
			base.AccountAutoPropagate					= new Account("LIVESIM_ACCOUNT", -1000);
			base.AccountAutoPropagate.Initialize(this);
			this.OrdersSubmitted_forOneLivesimBacktest	= new List<Order>();
			this.LivesimBrokerSpoiler					= new LivesimBrokerSpoiler(this);
		}
		public virtual void InitializeLivesim(LivesimDataSource livesimDataSource, OrderProcessor orderProcessor) {
			base.DataSource		= livesimDataSource;
			this.DataSnapshot	= new LivesimBrokerDataSnapshot(this.LivesimDataSource);
			//v1 base.InitializeDataSource_inverse(livesimDataSource, this.LivesimDataSource.StreamingAsLivesim_nullUnsafe,  orderProcessor);
			//v3
			if (this.LivesimDataSource.StreamingAsLivesim_nullUnsafe == null) {
				if (this.LivesimDataSource.StreamingAdapter == null) {
					string msg1 = "I_REFUSE_TO_INITIALIZE_BROKER_WITH_NULL_STREAMING";
					Assembler.PopupException(msg1, null, true);
					return;
				}
				if (this.LivesimDataSource.StreamingAdapter is LivesimBroker) {
					string msg1 = "I_REFUSE_TO_INITIALIZE_BROKER_MUST_BE_BROKER_ORIGINAL_HERE";
					Assembler.PopupException(msg1, null, true);
					return;
				}
				string msg = "LIVESIM_BROKER_ALREADY_REFERRING_TO_STREAMING_ORIGINAL_DDE THATS_WHAT_I_WANTED";
				Assembler.PopupException(msg, null, false);
				base.InitializeDataSource_inverse(this.LivesimDataSource, this.LivesimDataSource.StreamingAdapter, orderProcessor);
				return;
			}
			//v2
			if (this.LivesimDataSource.StreamingAsLivesim_nullUnsafe.StreamingOriginal != null) {
				base.InitializeDataSource_inverse(this.LivesimDataSource, this.LivesimDataSource.StreamingAsLivesim_nullUnsafe.StreamingOriginal, orderProcessor);
			} else {
				base.InitializeDataSource_inverse(this.LivesimDataSource, this.LivesimDataSource.StreamingAsLivesim_nullUnsafe, orderProcessor);
			}
		}
		internal void InitializeMarketsim(ScriptExecutor scriptExecutor) {
			this.ScriptExecutor = scriptExecutor;
			this.LivesimMarketsim.Initialize(this.ScriptExecutor);
		}
		public override BrokerEditor BrokerEditorInitialize(IDataSourceEditor dataSourceEditor) {
			LivesimBrokerEditorEmpty emptyEditor = new LivesimBrokerEditorEmpty();
			emptyEditor.Initialize(this, dataSourceEditor);
			this.BrokerEditorInstance = emptyEditor;
			return emptyEditor;
		}

		public void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE  " + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
			if (this.LivesimDataSource.IsDisposed == false) {
				this.LivesimDataSource	.Dispose();
			} else {
				string msg = "ITS_OKAY this.livesimDataSource might have been already disposed by LivesimStreaming.Dispose()";
			}
			this.DataSnapshot	.Dispose();
			base.DataSource		= null;
			this.DataSnapshot	= null;
			this.IsDisposed = true;
		}

		public override void Broker_connect() {
			string msig = " //UpstreamConnect(" + this.ToString() + ")";
			string msg = "LIVESIM_CHILDREN_SHOULD_NEVER_RECEIVE_UpstreamConnect()";
			Assembler.PopupException(msg + msig, null, false);
		}
		public override void Broker_disconnect() {
			string msig = " //UpstreamDisconnect(" + this.ToString() + ")";
			string msg = "LIVESIM_CHILDREN_SHOULD_NEVER_RECEIVE_UpstreamDisonnect()";
			Assembler.PopupException(msg + msig, null, false);
		}
		
	}
}