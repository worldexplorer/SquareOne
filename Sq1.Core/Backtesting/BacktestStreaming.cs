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

		public virtual void PushQuoteGenerated(QuoteGenerated quoteBoundAttached) {
			string msig = " //BacktestStreaming.PushQuoteGenerated()" + this.ToString();

			if (this.SpreadModeler == null) {
				string msg = "Don't leave quoteToReach.Bid and quoteToReach.Ask uninitialized!!!";
				throw new Exception(msg);
			}
			//ALREADY_FILLED_BY_GENERATOR this.SpreadModeler.GeneratedQuoteFillBidAsk(quote, bar2simulate, priceForSymmetricFillAtOpenOrClose);
			//TOO_MANY_WORKAROUNDS_REQUIRED base.PushQuoteReceived(quote);

			Quote quotePrev = this.StreamingDataSnapshot.GetQuotePrev_forSymbol_nullUnsafe(quoteBoundAttached.Symbol);
			if (quotePrev == null) {
				string msg = "QUIK_JUST_CONNECTED_AND_SENDS_NONSENSE[" + quoteBoundAttached + "]";
				Assembler.PopupException(msg + msig, null, false);
				this.StreamingDataSnapshot.SetQuoteCurrent_forSymbol_shiftOldToQuotePrev(quoteBoundAttached);
				return;
			}

			//v1 HAS_NO_MILLISECONDS_FROM_QUIK if (quote.ServerTime > lastQuote.ServerTime) {
			//v2 TOO_SENSITIVE_PRINTED_SAME_MILLISECONDS_BUT_STILL_DIFFERENT if (quote.ServerTime.Ticks > lastQuote.ServerTime.Ticks) {
			string quoteMillis		= quoteBoundAttached.ServerTime.ToString("HH:mm:ss.fff");
			string quotePrevMillis  = quotePrev.ServerTime.ToString("HH:mm:ss.fff");
			if (quoteMillis == quotePrevMillis) {
				string msg = quoteBoundAttached.Symbol + " SERVER_TIMESTAMP_MUST_INCREASE_EACH_NEXT_INCOMING_QUOTE QUIK_OR_BACKTESTER_FORGOT_TO_INCREASE"
					+ " quoteMillis[" + quoteMillis + "] <="
					+ " quoteLastMillis[" + quotePrevMillis + "]"
					+ ": DDE lagged somewhere?..."
					;
				Assembler.PopupException(msg + msig, null, false);
				return;
			}


			string reasonMarketIsClosedNow  = this.DataSource.MarketInfo.GetReason_ifMarket_closedOrSuspended_at(quoteBoundAttached.ServerTime);
			if (string.IsNullOrEmpty(reasonMarketIsClosedNow) == false) {
				string msg = "[" + this.DataSource.MarketInfo.Name + "]NOT_PUSHING_QUOTE " + reasonMarketIsClosedNow + " quote=[" + quoteBoundAttached + "]";
				Assembler.PopupException(msg + msig, null, false);
				Assembler.DisplayStatus(msg + msig);
				return;
			}

			if (quoteBoundAttached.Size > 0) {
				this.StreamingDataSnapshot.SetQuoteCurrent_forSymbol_shiftOldToQuotePrev(quoteBoundAttached);
			}

			try {
				this.DistributorCharts_substitutedDuringLivesim.Push_quoteUnboundUnattached_toChannel(quoteBoundAttached);
			} catch (Exception ex) {
				string msg = "CHART_OR_STRATEGY__FAILED_INSIDE Distributor.PushQuoteToDistributionChannels(" + quoteBoundAttached + ")";
				Assembler.PopupException(msg + msig, ex);
			}
			
			quoteBoundAttached.WentThroughStreamingToScript = true;
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
