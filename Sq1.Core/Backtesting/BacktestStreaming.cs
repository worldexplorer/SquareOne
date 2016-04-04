using System;

using Newtonsoft.Json;

using Sq1.Core.Streaming;
using Sq1.Core.Support;
using Sq1.Core.DataTypes;
using Sq1.Core.StrategyBase;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Backtesting {
	[SkipInstantiationAt(Startup = true)]
	public class BacktestStreaming : StreamingAdapter {
		// without [JsonIgnore] Livesim children will have these properties in JSON
		[JsonIgnore] public BacktestSpreadModeler SpreadModeler;
		[JsonIgnore] public const double PERCENTAGE_DEFAULT= 0.005;
		//[JsonIgnore] internal DistributorBacktest DistributorBacktest;

		public BacktestStreaming(string reasonToExist) : base(reasonToExist) {
			base.Name = "BacktestStreamingAdapter";
//			this.InitializeSpreadModelerPercentage(PERCENTAGE_DEFAULT);
//		}
//		public void InitializeSpreadModelerPercentage(double pct) {
			//greater than BacktestSpreadModelerPercentageOfMedian(0.01) will make ATRband inconsistent! you'll see in TooltipPrice (Close+ATR != C+Upper) & SPREAD_MODELER_SHOULD_GENERATE_TIGHTER_SPREADS
			//for medianPrice[80.36],percentageOfMedian[0.01] => spread[0.008036] => Bid[~80.35598],Ask[~80.36402]
			this.SpreadModeler = new BacktestSpreadModelerPercentage(PERCENTAGE_DEFAULT);
			base.QuotePumpSeparatePushingThreadEnabled = false;
			//this.DistributorBacktest = new DistributorBacktest();
			base.DistributorSolidifiers_substitutedDuringLivesim = null;
		}

		public virtual void PushQuoteGenerated(QuoteGenerated quote) {
			if (this.SpreadModeler == null) {
				string msg = "Don't leave quoteToReach.Bid and quoteToReach.Ask uninitialized!!!";
				throw new Exception(msg);
			}
			//ALREADY_FILLED_BY_GENERATOR this.SpreadModeler.GeneratedQuoteFillBidAsk(quote, bar2simulate, priceForSymmetricFillAtOpenOrClose);
			base.PushQuoteReceived(quote);
			quote.WentThroughStreamingToScript = true;
		}

		public override void UpstreamSubscribe(string symbol) {
			base.UpstreamSubscribeRegistryHelper(symbol);
		}
		public override void UpstreamUnSubscribe(string symbol) {
			base.UpstreamUnSubscribeRegistryHelper(symbol);
		}
		public override bool UpstreamIsSubscribed(string symbol) {
			return base.UpstreamIsSubscribedRegistryHelper(symbol);
		}

		public virtual bool BacktestContextInitialize_pauseQueueForBacktest_leavePumpUnpausedForLivesimDefault_overrideable(ScriptExecutor executor, Bars barsEmptyButWillGrow) {
			//return false;	// NOTHING_WAS_DONE, nooneGotPaused
			bool thereWereNeighbours = this.DataSource
				.QueuePauseIgnorePump_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(
					executor, false);
#if DEBUG
			//Debugger.Break();	// CONFIRM_THAT_LIVESIM_QUIK_IS_OKAY_BETWEEN_HERE
#endif
			return thereWereNeighbours;
		}

		public virtual bool BacktestContextRestore_unpauseQueueForBacktest_leavePumpUnPausedForLivesimDefault_overrideable(ScriptExecutor executor) {
			//return false;	// NOTHING_WAS_DONE, nooneGotUnPaused
			bool thereWereNeighbours = this.DataSource
				.QueueResumeIgnorePump_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(
					executor, false);
#if DEBUG
			//Debugger.Break();	// CONFIRM_THAT_LIVESIM_QUIK_IS_OKAY_BETWEEN_HERE
#endif
			return thereWereNeighbours;
		}

		// Livesimulator.ctor() when instantiating LivesimDS with its own dummy LivesimStreaming/BrokerDefaults does not make ChartShadows PAUSED
		// base.BacktestDataSource			= new LivesimDataSource(executor);
		#region DISABLING_SOLIDIFIER__NOT_REALLY_USED_WHEN_STREAMING_ADAPTER_PROVIDES_ITS_OWN_LIVESIM_STREAMING
		public override void InitializeDataSource_inverse(DataSource dataSource, bool subscribeSolidifier = true) {
			base.InitializeFromDataSource(dataSource);
			base.Name						= "LivesimStreaming_IAM_ABSTRACT_ALWAYS_OVERRIDE_IN_CHILDREN";
			if (subscribeSolidifier) {
				string msg = "RELAX_IM_NOT_FORWARING_IT_TO_BASE_BUT_I_HANDLE_InitializeDataSource()_IN_LivesimStreaming";
			}
		}
		protected override void SolidifierSubscribe_toAllSymbols_ofDataSource_onAppRestart() {
			string msg = "BACKTESTER_MUST_NOT_SAVE_ANY_BARS EMPTY_HERE_TO_PREVENT_BASE_FROM_SUBSCRIBING_SOLIDIFIER";
		}
		#endregion
	}
}
