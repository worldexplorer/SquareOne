using System;

using Sq1.Core.Streaming;
using Sq1.Core.DataTypes;
using Sq1.Core.StrategyBase;
using Sq1.Core.Execution;

namespace Sq1.Core.Charting {
	public class StreamingConsumerChart : StreamingConsumer {
		public	ChartShadow							ChartShadow		{ get; private set; }

		public StreamingConsumerChart(ChartShadow chartShadow) {
			this.ChartShadow = chartShadow;
		}

		public void StreamingUnsubscribe(string reason = "NO_REASON_FOR_STREAMING_UNSUBSCRIBE") {
			base.MsigForNpExceptions = " //ChartStreamingConsumer.StreamingUnsubscribe(" + this.ToString() + ")";

			var executorSafe			= base.Executor_nullReported;
			var symbolSafe				= base.Symbol_nullReported;
			var scaleIntervalSafe		= base.ScaleInterval_nullReported;
			var streaming_nullReported	= base.StreamingAdapter_nullReported;
			//var strategy_nullUnsafe		= base.Strategy_nullReported;
			var strategy_nullUnsafe		= executorSafe.Strategy;

			bool downstream_mustBeSubscribed = this.DownstreamSubscribed == false;
			if (downstream_mustBeSubscribed) {
				string msg = "CHART_STREAMING_ALREADY_UNSUBSCRIBED_QUOTES_AND_BARS";
				Assembler.PopupException(msg + base.MsigForNpExceptions, null, false);
				// RESET_IsStreaming=subscribed return;
			}

			if (streaming_nullReported != null) {
				string branch = " DATA_SOURCE_HAS_STREAMING_ASSIGNED_1/2";
				//streaming_nullReported.UnsubscribeChart(symbolSafe, scaleIntervalSafe, this, branch + base.MsigForNpExceptions);
				streaming_nullReported.ChartStreamingConsumer_Unsubscribe(this, branch + base.MsigForNpExceptions);

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
				} else {
					//string msg = "RAISE_EVENT_SO_THAT_MAIN_FORM_SAVES_CHART_CONTEXT_WITHOUT_STRATEGY";
					//Assembler.PopupException(msg, null, false);
					this.ChartShadow.RaiseOnChartSettingsIndividualChanged_chartManagerShouldSerialize_ChartFormDataSnapshot();
				}
				serialized = true;
			}

#if DEBUG_STREAMING
			string msg2 = "CHART_STREAMING_UNSUBSCRIBED[" + downstream_mustBeSubscribed + "] due to [" + reason + "]";
			Assembler.PopupException(msg2 + base.MsigForNpExceptions, null, false);
#endif

			// TUNNELLED_TO_CHART_CONTROL base.ChartShadow_nullReported.ChartControl.ScriptExecutorObjects.QuoteLast = null;
		}
		public void StreamingSubscribe(string reason = "NO_REASON_FOR_STREAMING_SUBSCRIBE") {
			base.ReasonToExist = "Chart";
			base.ReasonToExist += (this.Executor.Strategy != null) ? "[" + this.Executor.ToString() + "]" : "[" + base.Symbol_nullReported + "]";

			if (this.NPEs_handled() == false) return;	// NULL_POINTERS_ARE_ALREADY_REPORTED_TO_EXCEPTIONS_FORM
			base.MsigForNpExceptions = " //ChartStreamingConsumer.StreamingSubscribe(" + this.ToString() + ")";

			var executorSafe			= base.Executor_nullReported;
			var symbolSafe				= base.Symbol_nullReported;
			var scaleIntervalSafe		= base.ScaleInterval_nullReported;
			var streaming_nullReported	= base.StreamingAdapter_nullReported;

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

				streaming_nullReported.ChartStreamingConsumer_Subscribe(this, branch + base.MsigForNpExceptions);

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
					this.ChartShadow.RaiseOnChartSettingsIndividualChanged_chartManagerShouldSerialize_ChartFormDataSnapshot();
					serialized = true;
				}
			}

#if DEBUG_STREAMING
			string msg2 = "CHART_STREAMING_SUBSCRIBED[" + downstreamAfterWeAttemptedToSubscribe_mustBeSubscribed
				+ "] SAVED_FOR_APPRESTART[" + serialized + "] due to [" + reason + "]";
			Assembler.PopupException(msg2 + base.MsigForNpExceptions, null, false);
#endif
		}

		#region StreamingConsumer
		//CREATES_NPE_WHEN_EXECUTOR_PUSHES_BARS_TO_CHART_SHADOW__BASE_WILL_ROUTE_VIA_EXECUTOR public	override	Bars			ConsumerBars_toAppendInto	{ get { return this.ChartShadow.Bars; } }
		public	override	ScriptExecutor	Executor			{ get {
				ScriptExecutor ret = this.ChartShadow.Executor;
				base.ActionForNullPointer(ret, "this.chartShadow.Executor=null");
				return ret;
			} }

		public override void UpstreamSubscribed_toSymbol_streamNotifiedMe(Quote quoteFirst_afterStart) {
		}
		public override void UpstreamUnsubscribed_fromSymbol_streamNotifiedMe(Quote quoteLast_beforeStop) {
		}
		public override void Consume_barLastStatic_justFormed_whileStreamingBarWithOneQuote_alreadyAppended(Bar barLastFormed, Quote quoteForAlertsCreated) {
			base.MsigForNpExceptions = " //ChartStreamingConsumer.Consume_barLastStatic(" + barLastFormed + ")"
				+ " ChartForm[" + base.ChartShadow_nullReported.Text + "]";
			
			Bars barsSafe = this.Bars_nullReported;		// Livesim-clone inside the Executor on receiving side? Is this right?
			#region  PARANOID TEST_INLINE
			#if DEBUG
			if (barsSafe.ScaleInterval != barLastFormed.ScaleInterval) {
				string msg = "SCALEINTERVAL_RECEIVED_DOESNT_MATCH_CHARTS"
					+ " bars[" + barsSafe.ScaleInterval + "] barLastFormed[" + barLastFormed.ScaleInterval + "]";
				Assembler.PopupException(msg + base.MsigForNpExceptions);
				return;
			}
			if (barsSafe.Symbol != barLastFormed.Symbol) {
				string msg = "SYMBOL_RECEIVED_DOESNT_MATCH_CHARTS"
					+ " bars[" + barsSafe.Symbol + "] barLastFormed[" + barLastFormed.Symbol + "]";
				Assembler.PopupException(msg + base.MsigForNpExceptions);
				return;
			}
			if (barsSafe.BarStaticLast_nullUnsafe != barLastFormed) {
				string msg = "I_MUST_HAVE_BEEN_INVOKED_WITH_MY_OWN_LAST_BAR"
					+ " barsSafe.BarStaticLast_nullUnsafe[" + barsSafe.BarStaticLast_nullUnsafe + "] barLastFormed[" + barLastFormed + "]";
				Assembler.PopupException(msg + base.MsigForNpExceptions);
				return;
			}
			#endif
			#endregion

			if (this.Executor.Strategy == null) {
				string msg = "CHART_WITHOUT_ANY_STRATEGY__JUST_INVALIDATE_AND_EXIT__NO_SCRIPT_TO_INVOKE";
				//Assembler.PopupException(msg);
				base.ChartShadow_nullReported.PushQuote_toExecutorObjects_fromStreamingDataSnapshot_triggerInvalidateAll();
				return;
			}
	
			this.Executor.InvokeIndicators_onNewBar_onNewQuote(barLastFormed, quoteForAlertsCreated, false);

			if (this.Executor.IsStreamingTriggeringScript) {
				try {
					//v1
					//bool wrongUsagePopup = this.Executor.Livesimulator.IsBacktestingNoLivesimNow;
					//bool thereWereNeighbours = dataSourceSafe.QueuePauseIgnorePump_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(this.Executor, wrongUsagePopup);		// NOW_FOR_LIVE_MOCK_BUFFERING
					bool thereWereNeighbours = base.DataSource_nullReported
						.QueuePauseIgnorePump_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst_WRAPPER(this.Executor, barsSafe);

					// TESTED BACKLOG_GREWUP Thread.Sleep(450);	// 10,000msec = 10sec
					ReporterPokeUnit pokeUnit_nullUnsafe_dontForgetToDispose = this.Executor.InvokeScript_onNewBar_onNewQuote(quoteForAlertsCreated, false);	//new Quote());
					//UNFILLED_POSITIONS_ARE_USELESS chartFormManager.ReportersFormsManager.BuildIncrementalAllReports(pokeUnit);
					if (pokeUnit_nullUnsafe_dontForgetToDispose != null) {
						pokeUnit_nullUnsafe_dontForgetToDispose.Dispose();
					}
				} catch (Exception ex) {
					string msg = "Positions_Pending_orOpenNow.Add_step1or2()???...";
					Assembler.PopupException(msg);
				} finally {
					//v1 bool thereWereNeighbours = dataSourceSafe.QueueResumeIgnorePump_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(this.Executor);	// NOW_FOR_LIVE_MOCK_BUFFERING
					bool thereWereNeighbours = base.DataSource_nullReported
						.QueueResumeIgnorePump_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst_WRAPPER(this.Executor);
				}
			}

			#if DEBUG
			if (this.Executor.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing) {
				string msg = "SHOULD_NEVER_HAPPEN ImRunningChartlessBacktesting[true] AVOIDING_InvalidateAllPanels()--WHY???";
				Assembler.PopupException(msg + base.MsigForNpExceptions);
				return;
			}
			#endif

			if (this.ContextCurrentChartOrStrategy_nullReported.DownstreamSubscribed) {
				base.ChartShadow_nullReported.InvalidateAllPanels();
			}
		}
		public override void Consume_quoteOfStreamingBar(Quote quote_clonedBoundAttached) {
			base.MsigForNpExceptions = " //ChartStreamingConsumer.Consume_quoteOfStreamingBar(" + quote_clonedBoundAttached.ToString() + ")";

			Bars barsSafe = this.Bars_nullReported;
			
			#region  PARANOID TEST_INLINE
			#if DEBUG	// TEST_INLINE_BEGIN
			if (barsSafe.BarStreaming_nullUnsafe != quote_clonedBoundAttached.ParentBarStreaming) {
				string msg1 = "BARS_STREAMING_MUST_BE_THE_SAME__NOT_CLONES STREAMING_BINDER_DIDNT_DO_ITS_JOB#3 bars[" + barsSafe
					+ "] quote.ParentStreamingBar[" + quote_clonedBoundAttached.ParentBarStreaming + "]";
				Assembler.PopupException(msg1 + base.MsigForNpExceptions, null, false);

				if (barsSafe.ScaleInterval != quote_clonedBoundAttached.ParentBarStreaming.ScaleInterval) {
					string msg = "SCALEINTERVAL_RECEIVED_DOESNT_MATCH_CHARTS ChartForm[" + base.ChartShadow_nullReported.Text + "]"
						+ " bars[" + barsSafe.ScaleInterval + "] quote.ParentStreamingBar[" + quote_clonedBoundAttached.ParentBarStreaming.ScaleInterval + "]";
					Assembler.PopupException(msg + base.MsigForNpExceptions, null, false);
					return;
				}
				if (barsSafe.Symbol != quote_clonedBoundAttached.ParentBarStreaming.Symbol) {
					string msg = "SYMBOL_RECEIVED_DOESNT_MATCH_CHARTS ChartForm[" + base.ChartShadow_nullReported.Text + "]"
						+ " bars[" + barsSafe.Symbol + "] quote.ParentStreamingBar[" + quote_clonedBoundAttached.ParentBarStreaming.Symbol + "]";
					Assembler.PopupException(msg + base.MsigForNpExceptions);
					return;
				}
				return;
			}

			string msg2 = "BARS_IDENTICAL";
			bool sameDOHLCV = barsSafe.BarStreaming_nullUnsafe.HasSameDOHLCVas(quote_clonedBoundAttached.ParentBarStreaming, "quote.ParentStreamingBar", "barsSafe.BarStreaming", ref msg2);
			if (sameDOHLCV == false) {
				string msg = "FIXME_MUST_BE_THE_SAME EARLY_BINDER_DIDNT_DO_ITS_JOB#3 [" + msg2 + "] this.Executor.Bars.BarStreaming[" + barsSafe.BarStreaming_nullUnsafe
					+ "].HasSameDOHLCVas(quote.ParentStreamingBar[" + quote_clonedBoundAttached.ParentBarStreaming + "])=false";
				Assembler.PopupException(msg + base.MsigForNpExceptions, null, false);
				return;
			}
			#endif	// TEST_INLINE_END
			#endregion

			// STREAMING_BAR_IS_ALREADY_MERGED_IN_EARLY_BINDER_WITH_QUOTE_RECIPROCALLY
			//try {
			//	streamingSafe.InitializeStreamingOHLCVfromStreamingAdapter(this.chartFormManager.Executor.Bars);
			//} catch (Exception e) {
			//	Assembler.PopupException("didn't merge with Partial, continuing", e, false);
			//}

			if (quote_clonedBoundAttached.ParentBarStreaming.ParentBarsIndex > quote_clonedBoundAttached.ParentBarStreaming.ParentBars.Count) {
				string msg = "should I add a bar into Chart.Bars?... NO !!! already added";
				Assembler.PopupException(msg, null, false);
			}

			// #1/4 launch update in GUI thread
			//MOVED_TO_chartControl_BarAddedUpdated_ShouldTriggerRepaint chartFormSafe.ChartControl.ScriptExecutorObjects.QuoteLast = quote.Clone();
			// TUNNELLED_TO_CHART_FORMS_MANAGER chartFormSafe.PrintQuoteTimestampOnStrategyTriggeringButton_beforeExecution_switchToGuiThread(quote);
			this.Executor.EventGenerator.RaiseOnStrategyPreExecute_oneQuote(quote_clonedBoundAttached);

			base.Executor_nullReported.InvokeIndicators_onNewBar_onNewQuote(null, quote_clonedBoundAttached, true);

			if (this.Executor.Strategy == null) {
				string msg = "CHART_WITHOUT_ANY_STRATEGY__JUST_INVALIDATE_AND_EXIT__NO_SCRIPT_TO_INVOKE";
				//Assembler.PopupException(msg);
				base.ChartShadow_nullReported.PushQuote_toExecutorObjects_fromStreamingDataSnapshot_triggerInvalidateAll();
				return;
			}

			// #2/4 execute strategy in the thread of a StreamingAdapter (DDE server for MockQuickProvider)

			if (this.Executor.IsStreamingTriggeringScript) {
				try {
					//v1
					//bool wrongUsagePopup = this.Executor.Livesimulator.IsBacktestingNoLivesimNow;
					//bool thereWereNeighbours = dataSourceSafe.QueuePauseIgnorePump_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(this.Executor, wrongUsagePopup);		// NOW_FOR_LIVE_MOCK_BUFFERING
					//v2
					bool thereWereNeighbours = base.DataSource_nullReported
						.QueuePauseIgnorePump_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst_WRAPPER(this.Executor, barsSafe);

					// TESTED BACKLOG_GREWUP Thread.Sleep(450);	// 10,000msec = 10sec
					ReporterPokeUnit pokeUnit_nullUnsafe_dontForgetToDispose = this.Executor.InvokeScript_onNewBar_onNewQuote(quote_clonedBoundAttached, true);
					//UNFILLED_POSITIONS_ARE_USELESS chartFormManager.ReportersFormsManager.BuildIncrementalAllReports(pokeUnit);
					if (pokeUnit_nullUnsafe_dontForgetToDispose != null) {
						pokeUnit_nullUnsafe_dontForgetToDispose.Dispose();
					}
				} finally {
					//v1 bool thereWereNeighbours = dataSourceSafe.QueueResumeIgnorePump_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(this.Executor);	// NOW_FOR_LIVE_MOCK_BUFFERING
					//v2
					bool thereWereNeighbours = base.DataSource_nullReported
						.QueueResumeIgnorePump_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst_WRAPPER(this.Executor);
				}
			} else {
				// UPDATE_REPORTS_OPEN_POSITIONS_WITH_EACH_QUOTE_DESPITE_STRATEGY_IS_NOT_TRIGGERED
				// copypaste from Executor.ExecuteOnNewBarOrNewQuote()
				ReporterPokeUnit pokeUnit_dontForgetToDispose = new ReporterPokeUnit(quote_clonedBoundAttached,
					this.Executor.ExecutionDataSnapshot.AlertsNewAfterExec			.Clone(this, "ConsumeQuoteOfStreamingBar(WAIT)"),
					this.Executor.ExecutionDataSnapshot.Positions_toBeOpenedAfterExec	.Clone(this, "ConsumeQuoteOfStreamingBar(WAIT)"),
					this.Executor.ExecutionDataSnapshot.Positions_toBeClosedAfterExec	.Clone(this, "ConsumeQuoteOfStreamingBar(WAIT)"),
					this.Executor.ExecutionDataSnapshot.Positions_Pending_orOpenNow			.Clone(this, "ConsumeQuoteOfStreamingBar(WAIT)")
				);
				using(pokeUnit_dontForgetToDispose) {
					// FROM_ChartStreamingConsumer.ConsumeQuoteOfStreamingBar() #4/4 notify Positions that it should update open positions, I wanna see current profit/loss and relevant red/green background
					if (pokeUnit_dontForgetToDispose.PositionsOpenNow.Count > 0) {
						this.Executor.PerformanceAfterBacktest.BuildIncremental_openPositionsUpdated_afterChartConsumedNewQuote_step2of3(this.Executor.ExecutionDataSnapshot.Positions_Pending_orOpenNow);
						this.Executor.EventGenerator.RaiseOpenPositionsUpdated_afterChartConsumedNewQuote_reportersOnly_step2of3(pokeUnit_dontForgetToDispose);
					}
				}
			}

			// #3/4 trigger ChartControl to repaint candles with new positions and bid/ask lines
			//DOESNT_WORK_WHEN_LIVESIMMING_OWN_IMPLEMETATION_THER ALREADY_HANDLED_BY_chartControl_BarStreamingUpdatedMerged_ShouldTriggerRepaint_WontUpdateBtnTriggeringScriptTimeline
			//if (this.ChartFormManager.ContextCurrentChartOrStrategy.IsStreaming) {

			base.ChartShadow_nullReported.PushQuote_toExecutorObjects_fromStreamingDataSnapshot_triggerInvalidateAll();
			
			//}

			// MOVED_TO_ScriptExecutor_USING_RaiseOpenPositionsUpdatedDueToStreamingNewQuote_step2of3() #4/4 notify Positions that it should update open positions, I wanna see current profit/loss and relevant red/green background
			//List<Position> positionsOpenNowSafeCopy = this.Executor.ExecutionDataSnapshot.PositionsOpenNowSafeCopy;
			//if (positionsOpenNowSafeCopy.Count > 0) {
			//	this.ChartFormManager.ReportersFormsManager.UpdateOpenPositionsDueToStreamingNewQuote_step2of3(positionsOpenNowSafeCopy);
			//}
		}
		public override void Consume_levelTwoChanged(LevelTwoFrozen levelTwoFrozen) {
			if (this.Executor.Strategy == null) {
				string msg = "CHART_WITHOUT_ANY_STRATEGY__JUST_INVALIDATE_AND_EXIT__NO_SCRIPT_TO_INVOKE";
				//Assembler.PopupException(msg);
				base.ChartShadow_nullReported.PushLevelTwoFrozen_toExecutorObjects_fromStreamingDataSnapshot_triggerInvalidateAll(levelTwoFrozen);
				return;
			}

			if (base.Executor_nullReported.IsStreamingTriggeringScript) {
				try {
					//bool thereWereNeighbours = base.DataSource_nullReported
					//	.QueuePauseIgnorePump_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst_WRAPPER(base.Executor_nullReported, barsSafe);

					// TESTED BACKLOG_GREWUP Thread.Sleep(450);	// 10,000msec = 10sec
					ReporterPokeUnit pokeUnit_nullUnsafe_dontForgetToDispose = base.Executor_nullReported.InvokeScript_onLevelTwoChanged_noNewQuote(levelTwoFrozen);
					//UNFILLED_POSITIONS_ARE_USELESS chartFormManager.ReportersFormsManager.BuildIncrementalAllReports(pokeUnit);
					if (pokeUnit_nullUnsafe_dontForgetToDispose != null) {
						pokeUnit_nullUnsafe_dontForgetToDispose.Dispose();
					}
				} finally {
					//bool thereWereNeighbours = base.DataSource_nullReported
					//	.QueueResumeIgnorePump_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst_WRAPPER(base.Executor_nullReported);
				}
			}

			#if DEBUG
			if (base.Executor_nullReported.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing) {
				string msg = "SHOULD_NEVER_HAPPEN IsBacktestingNoLivesimNow[true] //ChartStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended()";
				Assembler.PopupException(msg);
				return;
			}
			#endif

			if (this.ContextCurrentChartOrStrategy_nullReported.DownstreamSubscribed == false) {
				string msg = "IM_NOT_PUSHING_LEVEL_TWO_FOR_BACKTESTS.... OR_HOW_AM_I_NOT_SUBSCRIBED_WHEN_I_RECEIVED_IT?...";
				Assembler.PopupException(msg);
				return;
			}

			base.ChartShadow_nullReported.PushLevelTwoFrozen_toExecutorObjects_fromStreamingDataSnapshot_triggerInvalidateAll(levelTwoFrozen);
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

	
		int intraBarSerno;
		public Quote Quote_cloneIncrement_bindToStreamingBar__createStreaming_whenBeyond_barCloseTime__nullWhenFirstBarEver(Quote quoteDequeued_singleInstance) {
			string msig = " //StreamingConsumerChart.Quote_cloneIncrement_bindToStreamingBar__createStreaming_whenBeyond_barCloseTime__nullWhenFirstBarEver() " + this.ToString();
			Quote ret = null;

			Quote quoteCloneUU = quoteDequeued_singleInstance.Clone_asCoreQuote();	// UU = UnboundUnattached

			bool justCreated_dontMerge = false;
			bool firstEverQuote_ofNewSymbol = this.ConsumerBars_toAppendInto.Count == 0;
			bool barsWereNeverStreamed = this.ConsumerBars_toAppendInto.BarStreaming_nullUnsafe == null;
			if (firstEverQuote_ofNewSymbol || barsWereNeverStreamed) {
				this.intraBarSerno = 0;

				Bar newBar_prototype = new Bar(
					this.ConsumerBars_toAppendInto.Symbol,
					this.ConsumerBars_toAppendInto.ScaleInterval,
					quoteCloneUU.ServerTime,
					quoteCloneUU.TradedPrice,
					quoteCloneUU.Size,
					this.ConsumerBars_toAppendInto.SymbolInfo);

				this.ConsumerBars_toAppendInto.BarStreaming_createNewAttach_orAbsorb(newBar_prototype);
				justCreated_dontMerge = true;
			}

			Bar barStreaming_expandedByQuote = this.ConsumerBars_toAppendInto.BarStreaming_nullUnsafe;
			if (barStreaming_expandedByQuote == null) {
				string msg = "I_REFUSE_TO_BIND_QUOTE_TO_NULL_STREAMING_BAR"
					+ " FIRST_BAR_OF_THE_CHART?... this.ConsumerBars_toAppendInto.Count[" + this.ConsumerBars_toAppendInto.Count + "] == 0?";
				Assembler.PopupException(msg);
				return ret;
			}

			if (quoteCloneUU.ServerTime >= barStreaming_expandedByQuote.DateTime_nextBarOpen_unconditional) {
				this.intraBarSerno = 0;
				Bar newBar_prototype = new Bar(this.ConsumerBars_toAppendInto.Symbol, this.ConsumerBars_toAppendInto.ScaleInterval, quoteCloneUU.ServerTime,
										quoteCloneUU.TradedPrice, quoteCloneUU.Size, this.ConsumerBars_toAppendInto.SymbolInfo);
				this.ConsumerBars_toAppendInto.BarStreaming_createNewAttach_orAbsorb(newBar_prototype);
				barStreaming_expandedByQuote = this.ConsumerBars_toAppendInto.BarStreaming_nullUnsafe;
			} else {
				if (justCreated_dontMerge == false) {
					// spread can be anywhere outside the bar; but a bar freezes only traded spreads inside (Quotes DDE table from Quik, not Level2-generated with Size=0)
					barStreaming_expandedByQuote.MergeExpandHLCV_forStreamingBarUnattached(quoteCloneUU);
				}
				this.intraBarSerno++;	// single source of increment per each consumer
			}


			if (quoteCloneUU.IntraBarSerno != -1) {
				string msg = "DONT_SET_quote.IntraBarSerno_UPSTACK__I_WILL_DO_IT_HERE";
				Assembler.PopupException(msg);
			}

			quoteCloneUU.Set_IntraBarSerno__onlyInConsumer(this.intraBarSerno);

			Quote bound = quoteCloneUU;
			bound.StreamingBar_Replace(barStreaming_expandedByQuote);
			ret = bound;

			return ret;
		}
	}
}
