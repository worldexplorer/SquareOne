using System;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Streaming;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Livesim {
	public class LivesimStreamingConsumer : StreamingConsumer {
				Livesimulator livesimulator;
		public	LivesimStreamingConsumer(Livesimulator livesimulatorPassed) {
			this.livesimulator = livesimulatorPassed;
		}

		#region StreamingConsumer
		public	override ScriptExecutor	Executor			{ get {
				var ret = this.livesimulator.Executor;
				base.ActionForNullPointer(ret, "this.Livesimulator.Executor=null");
				return ret;
			} }

		public override Bars ConsumerBars_toAppendInto { get { return this.livesimulator.BarsSimulating; } }
		public override void UpstreamSubscribed_toSymbol_streamNotifiedMe(Quote quoteFirstAfterStart) {
			base.ReasonToExist = "Livesim[" + base.Symbol_nullReported + "]";
			if (this.Strategy_nullReported != null) this.ReasonToExist += "[" + this.Strategy_nullReported.Name + "]";
		}
		public override void UpstreamUnsubscribed_fromSymbol_streamNotifiedMe(Quote quoteCurrent_beforeStop) {
		}
		public override void Consume_quoteOfStreamingBar(Quote quoteClone_boundAttached) {
			bool guiHasTime = this.livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild;
			ScriptExecutor executor = this.livesimulator.Executor;
			ReporterPokeUnit pokeUnit_nullUnsafe_dontForgetToDispose = this.livesimulator.Executor.InvokeScript_onNewBar_onNewQuote(quoteClone_boundAttached);
			using (pokeUnit_nullUnsafe_dontForgetToDispose) {
				if (	pokeUnit_nullUnsafe_dontForgetToDispose != null
					 && pokeUnit_nullUnsafe_dontForgetToDispose.PositionsOpenNow.Count > 0) {
					executor.PerformanceAfterBacktest.BuildIncremental_openPositionsUpdated_afterChartConsumedNewQuote_step2of3(executor.ExecutionDataSnapshot.PositionsOpenNow);
					if (guiHasTime) {
						executor.EventGenerator.RaiseOpenPositionsUpdated_afterChartConsumedNewQuote_reportersOnly_step2of3(pokeUnit_nullUnsafe_dontForgetToDispose);
					}
				}
				if (guiHasTime) {
					this.Executor.ChartShadow.PushQuote_LevelTwoFrozen_toExecutorObjects_fromStreamingDataSnapshot_triggerInvalidateAll();
				}
			}
		}
		public override void Consume_barLastStatic_justFormed_whileStreamingBarWithOneQuote_alreadyAppended(Bar barLastFormed, Quote quoteForAlertsCreated) {
			string msig = " //BacktestQuoteBarConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended";
			if (barLastFormed == null) {
				string msg = "THERE_IS_NO_STATIC_BAR_DURING_FIRST_4_QUOTES_GENERATED__ONLY_STREAMING"
					+ " Backtester starts generating quotes => first StreamingBar is added;"
					+ " for first four Quotes there's no static barsFormed yet!! Isi";
				Assembler.PopupException(msg + msig, null, false);
				return;
			}
			msig += "(" + barLastFormed.ToString() + ")";
			//v1 this.backtester.Executor.Strategy.Script.OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(barLastFormed);
			ReporterPokeUnit pokeUnit_nullUnsafe_dontForgetToDispose = this.livesimulator.Executor.InvokeScript_onNewBar_onNewQuote(quoteForAlertsCreated, false);
			if (pokeUnit_nullUnsafe_dontForgetToDispose != null) {
				pokeUnit_nullUnsafe_dontForgetToDispose.Dispose();
			}
		}
		public override void Consume_levelTwoChanged_noNewQuote(LevelTwoFrozen levelTwoFrozen) {
			ReporterPokeUnit pokeUnit_nullUnsafe_dontForgetToDispose = base.Executor_nullReported.InvokeScript_onLevelTwoChanged_noNewQuote(levelTwoFrozen);
			if (this.ContextCurrentChartOrStrategy_nullReported.DownstreamSubscribed) {
				base.ChartShadow_nullReported.InvalidateAllPanels();
			}
		}
		#endregion

		public override string ToString() {
			string ret = "CHARTLESS_CONSUMER_FOR-" + this.livesimulator.ToString();
			return ret;
		}
	}
}
