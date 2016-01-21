using System;

using Sq1.Core.Streaming;
using Sq1.Core.DataTypes;
using Sq1.Core.StrategyBase;
using Sq1.Core.Execution;

namespace Sq1.Core.Charting {
	public class ChartStreamingConsumer : StreamingConsumer {
				ChartShadow chartShadow;

		public ChartStreamingConsumer(ChartShadow chartShadow) {
			this.chartShadow = chartShadow;
		}

		public void StreamingUnsubscribe(string reason = "NO_REASON_FOR_STREAMING_UNSUBSCRIBE") {
			base.MsigForNpExceptions = " //ChartStreamingConsumer.StreamingUnsubscribe(" + this.ToString() + ")";

			var executorSafe			= base.Executor_nullReported;
			var symbolSafe				= base.Symbol_nullReported;
			var scaleIntervalSafe		= base.ScaleInterval_nullReported;
			var streaming_nullReported	= this.StreamingAdapter_nullReported;

			bool downstreamSubscribed = this.DownstreamSubscribed;
			if (downstreamSubscribed == false) {
				string msg = "CHART_STREAMING_ALREADY_UNSUBSCRIBED_QUOTES_AND_BARS";
				Assembler.PopupException(msg + base.MsigForNpExceptions, null, false);
				// RESET_IsStreaming=subscribed return;
			}

			if (streaming_nullReported != null) {
				string branch = " DATA_SOURCE_HAS_STREAMING_ASSIGNED_1/2";
				streaming_nullReported.UnsubscribeChart(symbolSafe, scaleIntervalSafe, this, branch + base.MsigForNpExceptions);

				//re-reading to be 100% sure
				downstreamSubscribed = this.DownstreamSubscribed;
				if (downstreamSubscribed) {
					string msg = "ERROR_CHART_STREAMING_STILL_SUBSCRIBED_QUOTES_OR_BARS";
					Assembler.PopupException(msg + base.MsigForNpExceptions);
					return;
				}
			} else {
				string msg = "ChartForm: BARS=>UNSUBSCRIBE_SHOULD_DISABLED_KOZ_NO_STREAMING_IN_DATASOURCE in PopulateBtnStreamingTriggersScript_afterBarsLoaded()";
				Assembler.PopupException(msg);
			}

			this.ContextCurrentChartOrStrategy_nullReported.DownstreamSubscribed = downstreamSubscribed;
			this.Strategy_nullReported.Serialize();

			string msg2 = "CHART_STREAMING_UNSUBSCRIBED[" + downstreamSubscribed + "] due to [" + reason + "]";
			Assembler.PopupException(msg2 + base.MsigForNpExceptions, null, false);

			// TUNNELLED_TO_CHART_CONTROL base.ChartShadow_nullReported.ChartControl.ScriptExecutorObjects.QuoteLast = null;
		}
		public void StreamingSubscribe(string reason = "NO_REASON_FOR_STREAMING_SUBSCRIBE") {
			base.ReasonToExist = "Chart[" + base.Symbol_nullReported + "]";
			if (this.Strategy_nullReported != null) this.ReasonToExist += "[" + this.Strategy_nullReported.Name + "]";

			if (this.CanSubscribeToStreamingAdapter() == false) return;	// NULL_POINTERS_ARE_ALREADY_REPORTED_TO_EXCEPTIONS_FORM
			base.MsigForNpExceptions = " //ChartStreamingConsumer.StreamingSubscribe(" + this.ToString() + ")";

			var executorSafe				= base.Executor_nullReported;
			var symbolSafe					= base.Symbol_nullReported;
			var scaleIntervalSafe			= base.ScaleInterval_nullReported;
			var streaming_nullReported		= this.StreamingAdapter_nullReported;
			var streamingBarSafeCloneSafe	= this.StreamingBarSafeClone_nullReported;

			bool downstreamSubscribed = this.DownstreamSubscribed;
			if (downstreamSubscribed) {
				string msg = "CHART_STREAMING_ALREADY_SUBSCRIBED_OR_FORGOT_TO_DISCONNECT REMOVE_INVOCATION_UPSTACK";
				Assembler.PopupException(msg + base.MsigForNpExceptions, null, false);
				// RESET_IsStreaming=subscribed return;
			}

			if (streaming_nullReported != null) {
				string branch = " DATA_SOURCE_HAS_STREAMING_ASSIGNED_1/2";

				// I dont remember what I was testing disabled
				//Assembler.PopupException("Subscribing BarsConsumer [" + this + "] to " + this.ToString() + " (wasn't registered)");
				//if (executorSafe.Bars == null) {
				//    string msg = "in Initialize() this.ChartForm is requesting bars in a separate thread";
				//    Assembler.PopupException(msg);
				//} else {
				//    string msg = "fully initialized, after streaming was stopped for a moment and resumed - append into PartialBar";
				//    Assembler.PopupException(msg);
				//}

				streaming_nullReported.SubscribeChart(symbolSafe, scaleIntervalSafe, this, branch + base.MsigForNpExceptions);

				//re-reading to be 100% sure
				downstreamSubscribed = this.DownstreamSubscribed;
				if (downstreamSubscribed == false) {
					string msg = "CHART_STREAMING_FAILED_SUBSCRIBE_BAR_OR_QUOTE_OR_BOTH StreamingAdapter[" + streaming_nullReported.ToString() + "]";
					Assembler.PopupException(msg + base.MsigForNpExceptions);
					return;
				}
			} else {
				string msg = "ChartForm: BARS=>SUBSCRIBE_SHOULD_BE_DISABLED_KOZ_NO_STREAMING_IN_DATASOURCE in PopulateBtnStreamingTriggersScript_afterBarsLoaded()";
				Assembler.PopupException(msg);
			}

			this.ContextCurrentChartOrStrategy_nullReported.DownstreamSubscribed = downstreamSubscribed;
			this.Strategy_nullReported.Serialize();

			string msg2 = "CHART_STREAMING_SUBSCRIBED[" + downstreamSubscribed + "] due to [" + reason + "]";
			Assembler.PopupException(msg2 + base.MsigForNpExceptions, null, false);
		}

		public void StreamingTriggeringScriptStart() {
			base.Executor_nullReported.IsStreamingTriggeringScript = true;
		}
		public void StreamingTriggeringScriptStop() {
			base.Executor_nullReported.IsStreamingTriggeringScript = false;
		}

		#region StreamingConsumer
		public	override ScriptExecutor	Executor			{ get {
				var ret = this.chartShadow.Executor;
				base.ActionForNullPointer(ret, "this.chartShadow.Executor=null");
				return ret;
			} }

		public override void UpstreamSubscribedToSymbolNotification(Quote quoteFirstAfterStart) {
		}
		public override void UpstreamUnSubscribedFromSymbolNotification(Quote quoteLastBeforeStop) {
		}
		public override void ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(Bar barLastFormed, Quote quoteForAlertsCreated) {
			if (barLastFormed == null) {
				string msg = "WRONG_SHOW_BRO";
				Assembler.PopupException(msg);
			}
			base.MsigForNpExceptions = " //ChartStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(" + barLastFormed.ToString() + ")";

			#if DEBUG	// TEST_INLINE
			var barsSafe = this.Bars_nullReported;
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
					bool wrongUsagePopup = executorSafe.Livesimulator.IsBacktestingNoLivesimNow;
					bool thereWereNeighbours = dataSourceSafe.PumpPauseNeighborsIfAnyFor(executorSafe, wrongUsagePopup);		// NOW_FOR_LIVE_MOCK_BUFFERING
					// TESTED BACKLOG_GREWUP Thread.Sleep(450);	// 10,000msec = 10sec
					ReporterPokeUnit pokeUnitNullUnsafe = executorSafe.ExecuteOnNewBarOrNewQuote(quoteForAlertsCreated, false);	//new Quote());
					//UNFILLED_POSITIONS_ARE_USELESS chartFormManager.ReportersFormsManager.BuildIncrementalAllReports(pokeUnit);
				} finally {
					bool thereWereNeighbours = dataSourceSafe.PumpResumeNeighborsIfAnyFor(executorSafe);	// NOW_FOR_LIVE_MOCK_BUFFERING
				}
			}

			#if DEBUG
			if (base.Executor_nullReported.BacktesterOrLivesimulator.IsBacktestingNoLivesimNow) {
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

			#if DEBUG	// TEST_INLINE_BEGIN
			var barsSafe = this.Bars_nullReported;
			if (barsSafe.ScaleInterval != quote.ParentBarStreaming.ScaleInterval) {
				string msg = "SCALEINTERVAL_RECEIVED_DOESNT_MATCH_CHARTS ChartForm[" + base.ChartShadow_nullReported.Text + "]"
					+ " bars[" + barsSafe.ScaleInterval + "] quote.ParentStreamingBar[" + quote.ParentBarStreaming.ScaleInterval + "]";
				Assembler.PopupException(msg + base.MsigForNpExceptions);
				return;
			}
			if (barsSafe.Symbol != quote.ParentBarStreaming.Symbol) {
				string msg = "SYMBOL_RECEIVED_DOESNT_MATCH_CHARTS ChartForm[" + base.ChartShadow_nullReported.Text + "]"
					+ " bars[" + barsSafe.Symbol + "] quote.ParentStreamingBar[" + quote.ParentBarStreaming.Symbol + "]";
				Assembler.PopupException(msg + base.MsigForNpExceptions);
				return;
			}
			string msg2 = "BARS_IDENTICAL";
			bool sameDOHLCV = barsSafe.BarStreamingNullUnsafe.HasSameDOHLCVas(quote.ParentBarStreaming, "quote.ParentStreamingBar", "barsSafe.BarStreaming", ref msg2);
			if (sameDOHLCV == false) {
				string msg = "FIXME_MUST_BE_THE_SAME EARLY_BINDER_DIDNT_DO_ITS_JOB#3 [" + msg2 + "] this.Executor.Bars.BarStreaming[" + barsSafe.BarStreamingNullUnsafe
					+ "].HasSameDOHLCVas(quote.ParentStreamingBar[" + quote.ParentBarStreaming + "])=false";
				Assembler.PopupException(msg + base.MsigForNpExceptions);
				return;
			}
			if (barsSafe.BarStreamingNullUnsafe != quote.ParentBarStreaming) {
				string msg = "SHOULD_THEY_BE_CLONES_OR_SAME? EARLY_BINDER_DIDNT_DO_ITS_JOB#3 bars[" + barsSafe
					+ "] quote.ParentStreamingBar[" + quote.ParentBarStreaming + "]";
				Assembler.PopupException(msg + base.MsigForNpExceptions);
				return;
			}
			#endif	// TEST_INLINE_END

			var streamingSafe	= this.StreamingAdapter_nullReported;
			var chartFormSafe	= base.ChartShadow_nullReported;
			var executorSafe	= base.Executor_nullReported;
			var dataSourceSafe	= this.DataSource_nullReported;

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
			executorSafe.EventGenerator.RaiseOnStrategyPreExecuteOneQuote(quote);

			// #2/4 execute strategy in the thread of a StreamingAdapter (DDE server for MockQuickProvider)
			if (executorSafe.Strategy != null) {
				if (executorSafe.IsStreamingTriggeringScript) {
					try {
						bool wrongUsagePopup = executorSafe.Livesimulator.IsBacktestingNoLivesimNow;
						bool thereWereNeighbours = dataSourceSafe.PumpPauseNeighborsIfAnyFor(executorSafe, wrongUsagePopup);		// NOW_FOR_LIVE_MOCK_BUFFERING
						// TESTED BACKLOG_GREWUP Thread.Sleep(450);	// 10,000msec = 10sec
						ReporterPokeUnit pokeUnitNullUnsafe1 = executorSafe.ExecuteOnNewBarOrNewQuote(quote, true);
						//UNFILLED_POSITIONS_ARE_USELESS chartFormManager.ReportersFormsManager.BuildIncrementalAllReports(pokeUnit);
					} finally {
						bool thereWereNeighbours = dataSourceSafe.PumpResumeNeighborsIfAnyFor(executorSafe);	// NOW_FOR_LIVE_MOCK_BUFFERING
					}
				} else {
					// UPDATE_REPORTS_OPEN_POSITIONS_WITH_EACH_QUOTE_DESPITE_STRATEGY_IS_NOT_TRIGGERED
					// copypaste from Executor.ExecuteOnNewBarOrNewQuote()
					ReporterPokeUnit pokeUnit = new ReporterPokeUnit(quote,
						executorSafe.ExecutionDataSnapshot.AlertsNewAfterExec		.Clone(this, "ConsumeQuoteOfStreamingBar(WAIT)"),
						executorSafe.ExecutionDataSnapshot.PositionsOpenedAfterExec	.Clone(this, "ConsumeQuoteOfStreamingBar(WAIT)"),
						executorSafe.ExecutionDataSnapshot.PositionsClosedAfterExec	.Clone(this, "ConsumeQuoteOfStreamingBar(WAIT)"),
						executorSafe.ExecutionDataSnapshot.PositionsOpenNow			.Clone(this, "ConsumeQuoteOfStreamingBar(WAIT)")
					);

					// FROM_ChartStreamingConsumer.ConsumeQuoteOfStreamingBar() #4/4 notify Positions that it should update open positions, I wanna see current profit/loss and relevant red/green background
					if (pokeUnit.PositionsOpenNow.Count > 0) {
						executorSafe.PerformanceAfterBacktest.BuildIncrementalOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(executorSafe.ExecutionDataSnapshot.PositionsOpenNow);
						executorSafe.EventGenerator.RaiseOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(pokeUnit);
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

		public override string ToString() {
			var symbolSafe			= base.Symbol_nullReported;
			var chartShadowSafe		= base.ChartShadow_nullReported;
			var scaleIntervalSafe	= base.ScaleInterval_nullReported;
			string ret = "ChartShadow.Symbol[" + symbolSafe + "](" + scaleIntervalSafe + ")";

			//HANGS_ON_STARTUP__#D_STACK_IS_BLANK__VS2010_HINTED_IM_ACCESSING_this.ChartForm.Text_FROM_DDE_QUOTE_GENERATOR (!?!?!)
			if (chartShadowSafe.InvokeRequired == false) {
				ret += " CHART.TEXT[" + chartShadowSafe.Text + "]";
			} else {
//				ChartFormDataSnapshot snap = this.chartFormManager.DataSnapshot;
//				if (snap == null) {
//					Assembler.PopupException(null);
//				}
//				ContextChart ctx = this.chartFormManager.DataSnapshot.ContextChart;
				ret += (base.Executor_nullReported.Strategy != null)
					? " ScriptContextCurrent[" + base.Executor_nullReported.Strategy.ScriptContextCurrent.ToString() + "]"
					//: " ContextChart[" + this.chartFormManager.DataSnapshot.ContextChart.ToString() + "]"
					: " ContextChart[UNACCESSIBLE]"
					;
			}

			return "{" + ret + "}";
		}
	}
}
