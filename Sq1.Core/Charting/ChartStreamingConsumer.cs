using System;

using Sq1.Core.Streaming;
using Sq1.Core.DataTypes;
using Sq1.Core.StrategyBase;
using Sq1.Core.Execution;
using Sq1.Core.Livesim;
using Sq1.Core.Backtesting;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Charting {
	public class ChartStreamingConsumer : StreamingConsumer {
		public	ChartShadow ChartShadow	{ get; private set; }

		public ChartStreamingConsumer(ChartShadow chartShadow) {
			this.ChartShadow = chartShadow;
		}

		public void StreamingUnsubscribe(string reason = "NO_REASON_FOR_STREAMING_UNSUBSCRIBE") {
			base.MsigForNpExceptions = " //ChartStreamingConsumer.StreamingUnsubscribe(" + this.ToString() + ")";

			var executorSafe			= base.Executor_nullReported;
			var symbolSafe				= base.Symbol_nullReported;
			var scaleIntervalSafe		= base.ScaleInterval_nullReported;
			var streaming_nullReported	= this.StreamingAdapter_nullReported;
			var strategy_nullUnsafe		= this.Strategy_nullReported;

			bool downstream_mustBeSubscribed = this.DownstreamSubscribed == false;
			if (downstream_mustBeSubscribed) {
				string msg = "CHART_STREAMING_ALREADY_UNSUBSCRIBED_QUOTES_AND_BARS";
				Assembler.PopupException(msg + base.MsigForNpExceptions, null, false);
				// RESET_IsStreaming=subscribed return;
			}

			if (streaming_nullReported != null) {
				string branch = " DATA_SOURCE_HAS_STREAMING_ASSIGNED_1/2";
				streaming_nullReported.UnsubscribeChart(symbolSafe, scaleIntervalSafe, this, branch + base.MsigForNpExceptions);

				//
				bool downstreamMustBeUnSubscribed_reReadingToBe100sure = this.DownstreamSubscribed == false;
				if (downstreamMustBeUnSubscribed_reReadingToBe100sure == false) {
					string msg = "ERROR_CHART_STREAMING_STILL_SUBSCRIBED_QUOTES_OR_BARS";
					Assembler.PopupException(msg + base.MsigForNpExceptions);
					return;
				}
			} else {
				string msg = "ChartForm: BARS=>UNSUBSCRIBE_SHOULD_DISABLED_KOZ_NO_STREAMING_IN_DATASOURCE in PopulateBtnStreamingTriggersScript_afterBarsLoaded()";
				Assembler.PopupException(msg);
			}

			this.ContextCurrentChartOrStrategy_nullReported.DownstreamSubscribed = false;

			//v1
			//this.Strategy_nullReported.Serialize();
			//v2
			bool serialized = false;
			ContextChart ctxChartOrStrategy = this.ContextCurrentChartOrStrategy_nullReported;
			if (ctxChartOrStrategy != null) {
				ctxChartOrStrategy.DownstreamSubscribed = false;
				// did you find it? now you have to save it :(
				if (strategy_nullUnsafe != null) {
					//strategy_nullUnsafe.Serialize();
					this.ChartShadow.RaiseOnContextScriptChanged_containerShouldSerialize();
					serialized = true;
				} else {
					//string msg = "RAISE_EVENT_SO_THAT_MAIN_FORM_SAVES_CHART_CONTEXT_WITHOUT_STRATEGY";
					//Assembler.PopupException(msg, null, false);
					this.ChartShadow.RaiseOnChartSettingsChanged_containerShouldSerialize();
					serialized = true;
				}
			}

			string msg2 = "CHART_STREAMING_UNSUBSCRIBED[" + downstream_mustBeSubscribed + "] due to [" + reason + "]";
#if DEBUG_STREAMING
			Assembler.PopupException(msg2 + base.MsigForNpExceptions, null, false);
#endif

			// TUNNELLED_TO_CHART_CONTROL base.ChartShadow_nullReported.ChartControl.ScriptExecutorObjects.QuoteLast = null;
		}
		public void StreamingSubscribe(string reason = "NO_REASON_FOR_STREAMING_SUBSCRIBE") {
			base.ReasonToExist = "Chart";
			this.ReasonToExist += (this.Executor.Strategy != null) ? "[" + this.Executor.ToString() + "]" : "[" + base.Symbol_nullReported + "]";

			if (this.NPEs_handled() == false) return;	// NULL_POINTERS_ARE_ALREADY_REPORTED_TO_EXCEPTIONS_FORM
			base.MsigForNpExceptions = " //ChartStreamingConsumer.StreamingSubscribe(" + this.ToString() + ")";

			var executorSafe			= base.Executor_nullReported;
			var symbolSafe				= base.Symbol_nullReported;
			var scaleIntervalSafe		= base.ScaleInterval_nullReported;
			var streaming_nullReported	= this.StreamingAdapter_nullReported;

			bool downstreamBeforeWeStarted_mustBeUnSubscribed = this.DownstreamSubscribed;
			if (downstreamBeforeWeStarted_mustBeUnSubscribed
						&& Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == true) {
				string msg = "CHART_STREAMING_ALREADY_SUBSCRIBED_OR_FORGOT_TO_DISCONNECT REMOVE_INVOCATION_UPSTACK"
					+ " HAPPENS_AFTER_USER_LOADED_ANOTHER_STRATEGY_FOR_CHART_WITHOUT_UNSUBSCRIBING";
				Assembler.PopupException(msg + base.MsigForNpExceptions, null, false);
				// RESET_IsStreaming=subscribed return;
			}

			bool downstreamAfterWeAttemptedToSubscribe_mustBeSubscribed = false;
			if (streaming_nullReported != null) {
				string branch = " DATA_SOURCE_HAS_STREAMING_ASSIGNED_1/2";

				// I dont remember what I was testing disabled
				//Assembler.PopupException("Subscribing BarsConsumer [" + this + "] to " + this.ToString() + " (wasn't registered)");
				//if (executorSafe.Bars == null) {
				//	string msg = "in Initialize() this.ChartForm is requesting bars in a separate thread";
				//	Assembler.PopupException(msg);
				//} else {
				//	string msg = "fully initialized, after streaming was stopped for a moment and resumed - append into PartialBar";
				//	Assembler.PopupException(msg);
				//}

				streaming_nullReported.SubscribeChart(symbolSafe, scaleIntervalSafe, this, branch + base.MsigForNpExceptions);

				downstreamAfterWeAttemptedToSubscribe_mustBeSubscribed = this.DownstreamSubscribed == true;	// reReadingToBe100sure => will go to final msg2
				if (downstreamAfterWeAttemptedToSubscribe_mustBeSubscribed == false) {
					string msg = "CHART_STREAMING_FAILED_SUBSCRIBE_BAR_OR_QUOTE_OR_BOTH StreamingAdapter[" + streaming_nullReported.ToString() + "]";
					Assembler.PopupException(msg + base.MsigForNpExceptions);
					return;
				}
			} else {
				string msg = "ChartForm: BARS=>SUBSCRIBE_SHOULD_BE_DISABLED_KOZ_NO_STREAMING_IN_DATASOURCE in PopulateBtnStreamingTriggersScript_afterBarsLoaded()";
				Assembler.PopupException(msg);
			}

			bool serialized = false;
			ContextChart ctxChartOrStrategy = this.ContextCurrentChartOrStrategy_nullReported;
			if (ctxChartOrStrategy != null) {
				ctxChartOrStrategy.DownstreamSubscribed = true;
				// did you find it? now you have to save it :(
				if (this.Executor.Strategy != null) {
					//strategy_nullUnsafe.Serialize();
					this.ChartShadow.RaiseOnContextScriptChanged_containerShouldSerialize();
					serialized = true;
				} else {
					//string msg = "RAISE_EVENT_SO_THAT_MAIN_FORM_SAVES_CHART_CONTEXT_WITHOUT_STRATEGY";
					//Assembler.PopupException(msg, null, false);
					this.ChartShadow.RaiseOnChartSettingsChanged_containerShouldSerialize();
					serialized = true;
				}
			}

			string msg2 = "CHART_STREAMING_SUBSCRIBED[" + downstreamAfterWeAttemptedToSubscribe_mustBeSubscribed
				+ "] SAVED_FOR_APPRESTART[" + serialized + "] due to [" + reason + "]";
#if DEBUG_STREAMING
			Assembler.PopupException(msg2 + base.MsigForNpExceptions, null, false);
#endif
		}

		public void StreamingTriggeringScriptStart() {
			base.Executor_nullReported.IsStreamingTriggeringScript = true;
		}
		public void StreamingTriggeringScriptStop() {
			base.Executor_nullReported.IsStreamingTriggeringScript = false;
		}

		#region StreamingConsumer
		public	override ScriptExecutor	Executor			{ get {
				var ret = this.ChartShadow.Executor;
				base.ActionForNullPointer(ret, "this.chartShadow.Executor=null");
				return ret;
			} }

		public override void UpstreamSubscribedToSymbolNotification(Quote quoteFirstAfterStart) {
		}
		public override void UpstreamUnSubscribedFromSymbolNotification(Quote quoteLastBeforeStop) {
		}
		public override void ConsumeBarLastStatic_justFormed_whileStreamingBarWithOneQuote_alreadyAppended(Bar barLastFormed, Quote quoteForAlertsCreated) {
			if (barLastFormed == null) {
				string msg = "WRONG_SHOW_BRO";
				Assembler.PopupException(msg);
			}
			base.MsigForNpExceptions = " //ChartStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(" + barLastFormed.ToString() + ")";

			var barsSafe = this.Bars_nullReported;
			#if DEBUG	// TEST_INLINE
			if (barsSafe.ScaleInterval != barLastFormed.ScaleInterval) {
				string msg = "SCALEINTERVAL_RECEIVED_DOESNT_MATCH_CHARTS ChartForm[" + base.ChartShadow_nullReported.Text + "]"
					+ " bars[" + barsSafe.ScaleInterval + "] barLastFormed[" + barLastFormed.ScaleInterval + "]";
				Assembler.PopupException(msg + base.MsigForNpExceptions);
				return;
			}
			if (barsSafe.Symbol != barLastFormed.Symbol) {
				string msg = "SYMBOL_RECEIVED_DOESNT_MATCH_CHARTS ChartForm[" + base.ChartShadow_nullReported.Text + "]"
					+ " bars[" + barsSafe.Symbol + "] barLastFormed[" + barLastFormed.Symbol + "]";
				Assembler.PopupException(msg + base.MsigForNpExceptions);
				return;
			}
			#endif

			var chartFormSafe		= base.ChartShadow_nullReported;
			var executorSafe		= base.Executor_nullReported;
			var dataSourceSafe		= this.DataSource_nullReported;

			if (barLastFormed == null) {
				string msg = "Streaming starts generating quotes => first StreamingBar is added; for first four Quotes there's no static barsFormed yet!! Isi";
				Assembler.PopupException(msg + base.MsigForNpExceptions);
				return;
			}

			if (executorSafe.Strategy != null && executorSafe.IsStreamingTriggeringScript) {
				try {
					//v1
					//bool wrongUsagePopup = executorSafe.Livesimulator.IsBacktestingNoLivesimNow;
					//bool thereWereNeighbours = dataSourceSafe.QueuePauseIgnorePump_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(executorSafe, wrongUsagePopup);		// NOW_FOR_LIVE_MOCK_BUFFERING
					bool thereWereNeighbours = dataSourceSafe
						.QueuePauseIgnorePump_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst_WRAPPER(executorSafe, barsSafe);

					// TESTED BACKLOG_GREWUP Thread.Sleep(450);	// 10,000msec = 10sec
					ReporterPokeUnit pokeUnit_nullUnsafe_dontForgetToDispose = executorSafe.InvokeScript_onNewBar_onNewQuote(quoteForAlertsCreated, false);	//new Quote());
					//UNFILLED_POSITIONS_ARE_USELESS chartFormManager.ReportersFormsManager.BuildIncrementalAllReports(pokeUnit);
					if (pokeUnit_nullUnsafe_dontForgetToDispose != null) {
						pokeUnit_nullUnsafe_dontForgetToDispose.Dispose();
					}
				} finally {
					//v1 bool thereWereNeighbours = dataSourceSafe.QueueResumeIgnorePump_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(executorSafe);	// NOW_FOR_LIVE_MOCK_BUFFERING
					bool thereWereNeighbours = dataSourceSafe
						.QueueResumeIgnorePump_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst_WRAPPER(executorSafe);
				}
			}

			#if DEBUG
			if (base.Executor_nullReported.BacktesterOrLivesimulator.ImRunningChartlessBacktesting) {
				string msg = "SHOULD_NEVER_HAPPEN IsBacktestingNoLivesimNow[true] //ChartStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended()";
				Assembler.PopupException(msg);
				return;
			}
			#endif

			if (this.ContextCurrentChartOrStrategy_nullReported.DownstreamSubscribed) {
				chartFormSafe.InvalidateAllPanels();
			}
		}
		public override void ConsumeQuoteOfStreamingBar(Quote quote) {
			base.MsigForNpExceptions = " //ChartStreamingConsumer.ConsumeQuoteOfStreamingBar(" + quote.ToString() + ")";

			var barsSafe = this.Bars_nullReported;
			#if DEBUG	// TEST_INLINE_BEGIN
			if (barsSafe.ScaleInterval != quote.ParentBarStreaming.ScaleInterval) {
				string msg = "SCALEINTERVAL_RECEIVED_DOESNT_MATCH_CHARTS ChartForm[" + base.ChartShadow_nullReported.Text + "]"
					+ " bars[" + barsSafe.ScaleInterval + "] quote.ParentStreamingBar[" + quote.ParentBarStreaming.ScaleInterval + "]";
				Assembler.PopupException(msg + base.MsigForNpExceptions, null, false);
				return;
			}
			if (barsSafe.Symbol != quote.ParentBarStreaming.Symbol) {
				string msg = "SYMBOL_RECEIVED_DOESNT_MATCH_CHARTS ChartForm[" + base.ChartShadow_nullReported.Text + "]"
					+ " bars[" + barsSafe.Symbol + "] quote.ParentStreamingBar[" + quote.ParentBarStreaming.Symbol + "]";
				Assembler.PopupException(msg + base.MsigForNpExceptions);
				return;
			}
			string msg2 = "BARS_IDENTICAL";
			bool sameDOHLCV = barsSafe.BarStreaming_nullUnsafe.HasSameDOHLCVas(quote.ParentBarStreaming, "quote.ParentStreamingBar", "barsSafe.BarStreaming", ref msg2);
			if (sameDOHLCV == false) {
				string msg = "FIXME_MUST_BE_THE_SAME EARLY_BINDER_DIDNT_DO_ITS_JOB#3 [" + msg2 + "] this.Executor.Bars.BarStreaming[" + barsSafe.BarStreaming_nullUnsafe
					+ "].HasSameDOHLCVas(quote.ParentStreamingBar[" + quote.ParentBarStreaming + "])=false";
				Assembler.PopupException(msg + base.MsigForNpExceptions);
				return;
			}
			if (barsSafe.BarStreaming_nullUnsafe != quote.ParentBarStreaming) {
				string msg = "SHOULD_THEY_BE_CLONES_OR_SAME? EARLY_BINDER_DIDNT_DO_ITS_JOB#3 bars[" + barsSafe
					+ "] quote.ParentStreamingBar[" + quote.ParentBarStreaming + "]";
				Assembler.PopupException(msg + base.MsigForNpExceptions);
				return;
			}
			#endif	// TEST_INLINE_END

			StreamingAdapter	streamingSafe	= this.StreamingAdapter_nullReported;
			var					chartFormSafe	= base.ChartShadow_nullReported;
			ScriptExecutor		executorSafe	= base.Executor_nullReported;
			DataSource			dataSourceSafe	= this.DataSource_nullReported;

			// STREAMING_BAR_IS_ALREADY_MERGED_IN_EARLY_BINDER_WITH_QUOTE_RECIPROCALLY
			//try {
			//	streamingSafe.InitializeStreamingOHLCVfromStreamingAdapter(this.chartFormManager.Executor.Bars);
			//} catch (Exception e) {
			//	Assembler.PopupException("didn't merge with Partial, continuing", e, false);
			//}

			if (quote.ParentBarStreaming.ParentBarsIndex > quote.ParentBarStreaming.ParentBars.Count) {
				string msg = "should I add a bar into Chart.Bars?... NO !!! already added";
			}

			// #1/4 launch update in GUI thread
			//MOVED_TO_chartControl_BarAddedUpdated_ShouldTriggerRepaint chartFormSafe.ChartControl.ScriptExecutorObjects.QuoteLast = quote.Clone();
			// TUNNELLED_TO_CHART_FORMS_MANAGER chartFormSafe.PrintQuoteTimestampOnStrategyTriggeringButton_beforeExecution_switchToGuiThread(quote);
			executorSafe.EventGenerator.RaiseOnStrategyPreExecute_oneQuote(quote);

			// #2/4 execute strategy in the thread of a StreamingAdapter (DDE server for MockQuickProvider)
			if (executorSafe.Strategy != null) {
				if (executorSafe.IsStreamingTriggeringScript) {
					try {
						//v1
						//bool wrongUsagePopup = executorSafe.Livesimulator.IsBacktestingNoLivesimNow;
						//bool thereWereNeighbours = dataSourceSafe.QueuePauseIgnorePump_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(executorSafe, wrongUsagePopup);		// NOW_FOR_LIVE_MOCK_BUFFERING
						//v2
						bool thereWereNeighbours = dataSourceSafe
							.QueuePauseIgnorePump_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst_WRAPPER(executorSafe, barsSafe);

						// TESTED BACKLOG_GREWUP Thread.Sleep(450);	// 10,000msec = 10sec
						ReporterPokeUnit pokeUnit_nullUnsafe_dontForgetToDispose = executorSafe.InvokeScript_onNewBar_onNewQuote(quote, true);
						//UNFILLED_POSITIONS_ARE_USELESS chartFormManager.ReportersFormsManager.BuildIncrementalAllReports(pokeUnit);
						if (pokeUnit_nullUnsafe_dontForgetToDispose != null) {
							pokeUnit_nullUnsafe_dontForgetToDispose.Dispose();
						}
					} finally {
						//v1 bool thereWereNeighbours = dataSourceSafe.QueueResumeIgnorePump_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(executorSafe);	// NOW_FOR_LIVE_MOCK_BUFFERING
						//v2
						bool thereWereNeighbours = dataSourceSafe
							.QueueResumeIgnorePump_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst_WRAPPER(executorSafe);
					}
				} else {
					// UPDATE_REPORTS_OPEN_POSITIONS_WITH_EACH_QUOTE_DESPITE_STRATEGY_IS_NOT_TRIGGERED
					// copypaste from Executor.ExecuteOnNewBarOrNewQuote()
					ReporterPokeUnit pokeUnit_dontForgetToDispose = new ReporterPokeUnit(quote,
						executorSafe.ExecutionDataSnapshot.AlertsNewAfterExec		.Clone(this, "ConsumeQuoteOfStreamingBar(WAIT)"),
						executorSafe.ExecutionDataSnapshot.PositionsOpenedAfterExec	.Clone(this, "ConsumeQuoteOfStreamingBar(WAIT)"),
						executorSafe.ExecutionDataSnapshot.PositionsClosedAfterExec	.Clone(this, "ConsumeQuoteOfStreamingBar(WAIT)"),
						executorSafe.ExecutionDataSnapshot.PositionsOpenNow			.Clone(this, "ConsumeQuoteOfStreamingBar(WAIT)")
					);
					using(pokeUnit_dontForgetToDispose) {
						// FROM_ChartStreamingConsumer.ConsumeQuoteOfStreamingBar() #4/4 notify Positions that it should update open positions, I wanna see current profit/loss and relevant red/green background
						if (pokeUnit_dontForgetToDispose.PositionsOpenNow.Count > 0) {
							executorSafe.PerformanceAfterBacktest.BuildIncrementalOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(executorSafe.ExecutionDataSnapshot.PositionsOpenNow);
							executorSafe.EventGenerator.RaiseOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(pokeUnit_dontForgetToDispose);
						}
					}
				}
			}

			// #3/4 trigger ChartControl to repaint candles with new positions and bid/ask lines
			// ALREADY_HANDLED_BY_chartControl_BarAddedUpdated_ShouldTriggerRepaint
			//if (this.ChartFormManager.ContextCurrentChartOrStrategy.IsStreaming) {
			//	chartFormSafe.ChartControl.InvalidateAllPanels();
			//}

			// MOVED_TO_ScriptExecutor_USING_RaiseOpenPositionsUpdatedDueToStreamingNewQuote_step2of3() #4/4 notify Positions that it should update open positions, I wanna see current profit/loss and relevant red/green background
			//List<Position> positionsOpenNowSafeCopy = executorSafe.ExecutionDataSnapshot.PositionsOpenNowSafeCopy;
			//if (positionsOpenNowSafeCopy.Count > 0) {
			//	this.ChartFormManager.ReportersFormsManager.UpdateOpenPositionsDueToStreamingNewQuote_step2of3(positionsOpenNowSafeCopy);
			//}
		}
		#endregion

		public override void PumpPaused_notification_overrideMe_switchLivesimmingThreadToGui() {
			this.ChartShadow.PumpPaused_notification_switchLivesimmingThreadToGui();
		}
		public override void PumpUnPaused_notification_overrideMe_switchLivesimmingThreadToGui() {
			this.ChartShadow.PumpUnPaused_notification_switchLivesimmingThreadToGui();
		}
	
		public override string ToString() {
			ChartShadow			chartShadowSafe		= base.ChartShadow_nullReported;
			string ret = chartShadowSafe != null ? chartShadowSafe.ToString() : "base.ChartShadow_nullReported=NULL";
			return "{" + ret + "}";
		}
	}
}
