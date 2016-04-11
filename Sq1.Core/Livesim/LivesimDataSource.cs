using System;

using Sq1.Core.Backtesting;
using Sq1.Core.StrategyBase;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Livesim {
	public class LivesimDataSource : BacktestDataSource, IDisposable {
		public ScriptExecutor		Executor						{ get; private set; }

		LivesimDataSource() {
			base.Name				= "LivesimDataSource";
			//base.StreamingAdapter	= new LivesimStreamingDefault("WILL_BE_OVERWRITTEN_FROM_REAL_DATASOURCE");	//USED_FOR_LIVESIM_ON_DATASOURCES_WITHOUT_ASSIGNED_STREAMING
			//base.BrokerAdapter		= new LivesimBrokerDefault	 ("WILL_BE_OVERWRITTEN_FROM_REAL_DATASOURCE");	//USED_FOR_LIVESIM_ON_DATASOURCES_WITHOUT_ASSIGNED_BROKER
		}

		public LivesimDataSource(ScriptExecutor executor) : this() {
			this.Executor = executor;
		}

		public void Propagate_preInstantiated_LivesimStreaming_ownImplementation_intoLivesimDataSource() {
			string msig = " //Propagate_preInstantiated_ownImplementationStreamingLivesimAdapters_intoLivesimDataSource() [" + this.ToString() + "]";
			if (this.Executor.DataSource_fromBars.StreamingAdapter.LivesimStreaming_ownImplementation != null) {
				base.StreamingAdapter = this.Executor.DataSource_fromBars.StreamingAdapter.LivesimStreaming_ownImplementation;
				string msg1 = "STREAMING_SUBSTITUTED_FOR_LIVESIM_DATASOURCE";
				Assembler.PopupException(msg1 + msig, null, false);
				return;
			}
			#region DIAGNOSTICS_IF_WAS_NOT_PRE_INSTANTIATED
			if (base.StreamingAsLivesim_nullUnsafe == null) {
				if (this.Executor.DataSource_fromBars.StreamingAdapter is LivesimStreamingDefault) {
					if (base.StreamingAdapter != this.Executor.DataSource_fromBars.StreamingAdapter) {
						base.StreamingAdapter  = this.Executor.DataSource_fromBars.StreamingAdapter;
					}
				}
			}
			if (base.StreamingAsLivesim_nullUnsafe == null) {
				string msg1 = "I_REFUSE_TO_RUN_LIVESIM_WITHOUT_LIVESIMSTREAMING"
					+ " LivesimDataSource.ctor() should have created its own basic LivesimStreaming<=BacktestStreaming, now NULL";
				Assembler.PopupException(msg1);
			} else {
				//base.StreamingAsLivesim_nullUnsafe.CreateDistributors_onlyWhenNecessary("DD_FOR_ONE_LIVESIM");
				string msg1 = "WILL_LIVESIM_VIA"
					+ " StreamingAsLivesim_nullUnsafe[" + base.StreamingAsLivesim_nullUnsafe + "]"
					+ ".Distributor_replacedForLivesim[" + base.StreamingAsLivesim_nullUnsafe.DistributorCharts_substitutedDuringLivesim.ReasonIwasCreated + "]"
					//+ " (LivesimStreamingDefault was probably chosen in DataSourceEditor which QuoteGen=>Pumps)"
					;
				Assembler.PopupException(msg1 + msig, null, false);
			}
			#endregion
		}

		public void Propagate_preInstantiated_LivesimBroker_ownImplementation_intoLivesimDataSource() {
			string msig = " //Propagate_preInstantiated_LivesimBroker_ownImplementation_intoLivesimDataSource() [" + this.ToString() + "]";
			if (this.Executor.DataSource_fromBars.BrokerAdapter.LivesimBroker_ownImplementation != null) {
				base.BrokerAdapter = this.Executor.DataSource_fromBars.BrokerAdapter.LivesimBroker_ownImplementation;
				string msg1 = "BROKER_SUBSTITUTED_FOR_LIVESIM_DATASOURCE";
				Assembler.PopupException(msg1 + msig, null, false);
				return;
			}
			#region DIAGNOSTICS_IF_WAS_NOT_PRE_INSTANTIATED
			if (base.BrokerAsLivesim_nullUnsafe == null) {
				if (this.Executor.DataSource_fromBars.BrokerAdapter is LivesimBrokerDefault) {
					if (base.BrokerAdapter != this.Executor.DataSource_fromBars.BrokerAdapter) {
						base.BrokerAdapter  = this.Executor.DataSource_fromBars.BrokerAdapter;
					}
				}
			}
			if (base.BrokerAsLivesim_nullUnsafe == null) {
				string msg2 = "I_REFUSE_TO_RUN_LIVESIM_WITHOUT_LIVESIMBROKER"
					+ " LivesimDataSource.ctor() should have created its own basic LivesimBroker<=BacktestBroker, now NULL";
				Assembler.PopupException(msg2);
			} else {
				string msg1 = "WILL_LIVESIM_VIA"
					+ " BrokerAsLivesim_nullUnsafe[" + base.BrokerAsLivesim_nullUnsafe + "]"
					;
				Assembler.PopupException(msg1 + msig, null, false);
			}
			#endregion
		}

		public void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE__" + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
			if (base.StreamingAsLivesim_nullUnsafe != null) {
				base.StreamingAsLivesim_nullUnsafe.Dispose();
				base.StreamingAdapter = null;
			}
			if (this.BrokerAsLivesim_nullUnsafe != null) {
				this.BrokerAsLivesim_nullUnsafe.Dispose();
				base.BrokerAdapter = null;
			}
			this.IsDisposed = true;
		}
		public bool IsDisposed { get; private set; }

		internal void InitializeLivesim(string executorImServing, Bars bars, BacktestSpreadModeler spreadModeler) {
			//v1 base.InitializeBacktest(executorImServing, bars, spreadModeler);
			//v2 not tunnelling to BacktestDataSource anymore
			base.Name += " FOR_LIVESIM:" + executorImServing;
			base.MarketInfo = bars.MarketInfo;
			base.ScaleInterval = bars.ScaleInterval;
			base.Symbols.Clear();
			base.Symbols.Add(bars.Symbol);
			base.StreamingAdapter.InitializeFromDataSource(this);
			this.StreamingAsBacktest_nullUnsafe.SpreadModeler = spreadModeler;
			// livesimulator.PreBarsSubstitute will invoke LivesimBroker.InitializedLivesim - there I will invoke InitializseDataSource_inverse()
		}

	}
}