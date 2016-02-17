using System;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Streaming;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Livesim {
	public class LivesimQuoteBarConsumer : StreamingConsumer {
				Livesimulator livesimulator;
		public	LivesimQuoteBarConsumer(Livesimulator livesimulatorPassed) {
			this.livesimulator = livesimulatorPassed;
		}

		#region StreamingConsumer
		public	override ScriptExecutor	Executor			{ get {
				var ret = this.livesimulator.Executor;
				base.ActionForNullPointer(ret, "this.Livesimulator.Executor=null");
				return ret;
			} }

		public override Bars ConsumerBarsToAppendInto { get { return this.livesimulator.BarsSimulating; } }
		public override void UpstreamSubscribedToSymbolNotification(Quote quoteFirstAfterStart) {
			base.ReasonToExist = "Livesim[" + base.Symbol_nullReported + "]";
			if (this.Strategy_nullReported != null) this.ReasonToExist += "[" + this.Strategy_nullReported.Name + "]";
		}
		public override void UpstreamUnSubscribedFromSymbolNotification(Quote quoteLastBeforeStop) {
		}
		public override void ConsumeQuoteOfStreamingBar(Quote quote) {
			bool guiHasTime = this.livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild;
			ScriptExecutor executor = this.livesimulator.Executor;
			ReporterPokeUnit pokeUnit_nullUnsafe = this.livesimulator.Executor.ExecuteOnNewBarOrNewQuote(quote);
			if (pokeUnit_nullUnsafe != null && pokeUnit_nullUnsafe.PositionsOpenNow.Count > 0) {
				executor.PerformanceAfterBacktest.BuildIncrementalOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(executor.ExecutionDataSnapshot.PositionsOpenNow);
				if (guiHasTime) {
					executor.EventGenerator.RaiseOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(pokeUnit_nullUnsafe);
				}
			}
			if (guiHasTime) {
				// ALREADY_HANDLED_BY chartControl_BarStreamingUpdatedMerged_ShouldTriggerRepaint_WontUpdateBtnTriggeringScriptTimeline
				//executor.ChartShadow.Invalidate();
				//executor.ChartShadow.InvalidateAllPanels();
				//executor.ChartShadow.RefreshAllPanelsWaitFinishedSoLivesimCouldGenerateNewQuote(0);
			}
		}
		public override void ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(Bar barLastFormed, Quote quoteForAlertsCreated) {
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
			ReporterPokeUnit pokeUnit_nullUnsafe = this.livesimulator.Executor.ExecuteOnNewBarOrNewQuote(quoteForAlertsCreated, false);
		}
		#endregion

		public override string ToString() {
			string ret = "CHARTLESS_CONSUMER_FOR-" + this.livesimulator.ToString();
			return ret;
		}
	}
}
