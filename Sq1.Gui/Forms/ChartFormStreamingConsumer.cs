using System;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.StrategyBase;
using Sq1.Core.Streaming;
using Sq1.Core.Execution;
using Sq1.Core.Livesim;

namespace Sq1.Gui.Forms {
	// ANY_STRATEGY_WILL_RUN_WITH_A_CHART_ITS_NOT_A_SERVER_APPLICATION
	public class ChartFormStreamingConsumer : IStreamingConsumer {
		//public event EventHandler<BarEventArgs> NewBar;
		//public event EventHandler<QuoteEventArgs> NewQuote;
		//public event EventHandler<BarsEventArgs> BarsLocked;
		ChartFormManager chartFormManager;
		string msigForNpExceptions = "Failed to StreamingSubscribe(): ";

		#region CASCADED_INITIALIZATION_ALL_CHECKING_CONSISTENCY_FROM_ONE_METHOD begin
		ChartFormManager ChartFormManager_nullReported { get {
				var ret = this.chartFormManager;
				this.actionForNullPointer(ret, "this.chartFormsManager=null");
				return ret;
			} }
		ScriptExecutor Executor_nullReported { get {
				var ret = this.ChartFormManager_nullReported.Executor;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor=null");
				return ret;
			} }
		Strategy Strategy_nullReported { get {
				var ret = this.Executor_nullReported.Strategy;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Strategy=null");
				return ret;
			} }
		ContextScript ScriptContextCurrent_nullReported { get {
				var ret = this.Strategy_nullReported.ScriptContextCurrent;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Strategy.ScriptContextCurrent=null");
				return ret;
			} }
		string Symbol_nullReported { get {
				string symbol = (this.Executor_nullReported.Strategy == null) ? this.Executor_nullReported.Bars.Symbol : this.ScriptContextCurrent_nullReported.Symbol;
				if (String.IsNullOrEmpty(symbol)) {
					this.action("this.chartFormsManager.Executor.Strategy.ScriptContextCurrent.Symbol IsNullOrEmpty");
				}
				return symbol;
			} }
		BarScaleInterval ScaleInterval_nullReported { get {
				var ret = (this.Executor_nullReported.Strategy == null) ? this.Executor_nullReported.Bars.ScaleInterval : this.ScriptContextCurrent_nullReported.ScaleInterval;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Strategy.ScriptContextCurrent.ScaleInterval=null");
				return ret;
			} }
		BarScale Scale_nullReported { get {
				var ret = this.ScaleInterval_nullReported.Scale;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Strategy.ScriptContextCurrent.ScaleInterval.Scale=null");
				if (ret == BarScale.Unknown) {
					this.action("this.chartFormsManager.Executor.Strategy.ScriptContextCurrent.ScaleInterval.Scale=Unknown");
				}
				return ret;
			} }
		//		BarDataRange DataRange { get {
		//				var ret = this.ScriptContextCurrent.DataRange;
		//				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Strategy.ScriptContextCurrent.DataRange=null");
		//				return ret;
		//			} }
		DataSource DataSource_nullReported { get {
				var ret = this.Executor_nullReported.DataSource;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.DataSource=null");
				return ret;
			} }
		StreamingAdapter StreamingAdapter_nullReported { get {
				StreamingAdapter ret = this.DataSource_nullReported.StreamingAdapter;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.DataSource[" + this.DataSource_nullReported + "].StreamingAdapter=null STREAMING_ADAPDER_NOT_ASSIGNED_IN_DATASOURCE");
				return ret;
			} }
		StreamingSolidifier StreamingSolidifierDeep { get {
				var ret = this.StreamingAdapter_nullReported.StreamingSolidifier;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.DataSource[" + this.DataSource_nullReported + "].StreamingAdapter.StreamingSolidifier=null");
				return ret;
			} }
		#region LIVESIM_OBEY_BARS_SUBSCRIBED__HANDLED_BY_LIVESIMULATOR 
		//Livesimulator Livesimulator { get {
		//        Livesimulator ret = this.Executor.Livesimulator;
		//        this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Livesimulator=null");
		//        return ret;
		//    } }
		//LivesimDataSource LivesimDataSource { get {
		//        LivesimDataSource ret = this.Executor.Livesimulator.DataSourceAsLivesimNullUnsafe;
		//        this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Livesimulator.DataSourceAsLivesimNullUnsafe=null");
		//        return ret;
		//    } }
		//LivesimStreaming LivesimStreamingAdapter { get {
		//        LivesimStreaming ret = this.Executor.Livesimulator.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe;
		//        this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Livesimulator.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe=null");
		//        return ret;
		//    } }
		#endregion
		ChartForm ChartForm_nullReported { get {
				var ret = this.ChartFormManager_nullReported.ChartForm;
				this.actionForNullPointer(ret, "this.chartFormsManager.ChartForm=null");
				return ret;
			} }
		Bars Bars_nullReported { get {
				var ret = this.Executor_nullReported.Bars;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Bars=null");
				return ret;
			} }
		Bar StreamingBarSafeClone_nullReported { get {
				var ret = this.Bars_nullReported.BarStreamingNullUnsafeCloneReadonly;
				//this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Bars.StreamingBarSafeClone=null");
				if (ret == null) ret = new Bar();
				return ret;
			} }
		Bar LastStaticBar_nullReported { get {
				var ret = this.Bars_nullReported.BarStaticLastNullUnsafe;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Bars.LastStaticBar=null");
				return ret;
			} }
		void actionForNullPointer(object mustBeInstance, string msgIfNull) {
			if (mustBeInstance != null) return;
			this.action(msgIfNull);
		}
		void action(string msgIfNull) {
			string msg = msigForNpExceptions + msgIfNull;
			Assembler.PopupException(msg, null, false);
			//throw new Exception(msg);
		}
		bool canSubscribeToStreamingAdapter() {
			try {
				var symbolSafe = this.Symbol_nullReported;
				var scaleSafe = this.Scale_nullReported;
				var streamingSafe = this.StreamingAdapter_nullReported;
				var staticDeepSafe = this.StreamingSolidifierDeep;
			} catch (Exception ex) {
				// already reported
				return false;
			}
			return true;
		}
		#endregion

		public ChartFormStreamingConsumer(ChartFormManager chartFormManager) {
			this.chartFormManager = chartFormManager;
		}
		public void StreamingUnsubscribe(string reason = "NO_REASON_FOR_STREAMING_UNSUBSCRIBE") {
			this.msigForNpExceptions = " //ChartFormStreamingConsumer.StreamingUnsubscribe(" + this.ToString() + ")";

			var executorSafe			= this.Executor_nullReported;
			var symbolSafe				= this.Symbol_nullReported;
			var scaleIntervalSafe		= this.ScaleInterval_nullReported;
			var streaming_nullUnsafe	= this.StreamingAdapter_nullReported;

			bool subscribed = this.Subscribed;
			if (subscribed == false) {
				string msg = "CHART_STREAMING_ALREADY_UNSUBSCRIBED_QUOTES_AND_BARS";
				Assembler.PopupException(msg + this.msigForNpExceptions, null, false);
				// RESET_IsStreaming=subscribed return;
			}

			if (streaming_nullUnsafe != null) {
				string branch = " DATA_SOURCE_HAS_STREAMING_ASSIGNED_1/2";
				if (this.DataSource_nullReported.StreamingAdapter != null) {
					if (streaming_nullUnsafe.DataDistributor.ConsumerQuoteIsSubscribed(symbolSafe, scaleIntervalSafe, this) == false) {
						Assembler.PopupException("CHART_STREAMING_WASNT_SUBSCRIBED_CONSUMER_QUOTE" + branch + this.msigForNpExceptions);
					} else {
						//Assembler.PopupException("UnSubscribing QuoteConsumer [" + this + "]  to " + plug + "  (was subscribed)");
						streaming_nullUnsafe.DataDistributor.ConsumerQuoteUnsubscribe(symbolSafe, scaleIntervalSafe, this);
					}

					if (streaming_nullUnsafe.DataDistributor.ConsumerBarIsSubscribed(symbolSafe, scaleIntervalSafe, this) == false) {
						Assembler.PopupException("CHART_STREAMING_WASNT_SUBSCRIBED_CONSUMER_BAR" + branch + this.msigForNpExceptions);
					} else {
						//Assembler.PopupException("UnSubscribing BarsConsumer [" + this + "] to " + this.ToString() + " (was subscribed)");
						streaming_nullUnsafe.DataDistributor.ConsumerBarUnsubscribe(symbolSafe, scaleIntervalSafe, this);
					}
				}

				#region 2/2 LIVESIM_OBEY_BARS_SUBSCRIBED__HANDLED_BY_LIVESIMULATOR make Livesim also obey Subscribed/Unsubscribed; Simulation of Live must be 100% alike to lifecycle of (and control over) real broker streams
				//var livesimStreamingSafe		= this.LivesimStreamingAdapter;

				//if (livesimStreamingSafe.DataDistributor.ConsumerQuoteIsSubscribed(symbolSafe, scaleIntervalSafe, this) == false) {
				//    Assembler.PopupException("CHART_LIVESIM_STREAMING_WASNT_SUBSCRIBED_CONSUMER_QUOTE" + this.msigForNpExceptions);
				//} else {
				//    //Assembler.PopupException("UnSubscribing QuoteConsumer [" + this + "]  to " + plug + "  (was subscribed)");
				//    livesimStreamingSafe.DataDistributor.ConsumerQuoteUnsubscribe(symbolSafe, scaleIntervalSafe, this);
				//}

				//if (livesimStreamingSafe.DataDistributor.ConsumerBarIsSubscribed(symbolSafe, scaleIntervalSafe, this) == false) {
				//    Assembler.PopupException("CHART_LIVESIM_STREAMING_WASNT_SUBSCRIBED_CONSUMER_BAR" + this.msigForNpExceptions);
				//} else {
				//    //Assembler.PopupException("UnSubscribing BarsConsumer [" + this + "] to " + this.ToString() + " (was subscribed)");
				//    livesimStreamingSafe.DataDistributor.ConsumerBarUnsubscribe(symbolSafe, scaleIntervalSafe, this);
				//}
				#endregion

				//re-reading to be 100% sure
				subscribed = this.Subscribed;
				if (subscribed) {
					string msg = "ERROR_CHART_STREAMING_STILL_SUBSCRIBED_QUOTES_OR_BARS";
					Assembler.PopupException(msg + this.msigForNpExceptions);
					return;
				}
			} else {
				string msg = "ChartForm: BARS=>UNSUBSCRIBE_SHOULD_DISABLED_KOZ_NO_STREAMING_IN_DATASOURCE in PopulateBtnStreamingTriggersScript_afterBarsLoaded()";
				Assembler.PopupException(msg);
			}

			this.ChartFormManager_nullReported.ContextCurrentChartOrStrategy.IsStreaming = subscribed;
			this.ChartFormManager_nullReported.Strategy.Serialize();

			string msg2 = "CHART_STREAMING_UNSUBSCRIBED[" + subscribed + "] due to [" + reason + "]";
			Assembler.PopupException(msg2 + this.msigForNpExceptions, null, false);

			var chartFormSafe = this.ChartForm_nullReported;
			chartFormSafe.ChartControl.ScriptExecutorObjects.QuoteLast = null;
		}
		public void StreamingSubscribe(string reason = "NO_REASON_FOR_STREAMING_SUBSCRIBE") {
			if (this.canSubscribeToStreamingAdapter() == false) return;	// NULL_POINTERS_ARE_ALREADY_REPORTED_TO_EXCEPTIONS_FORM
			this.msigForNpExceptions = " //ChartFormStreamingConsumer.StreamingSubscribe(" + this.ToString() + ")";

			bool subscribedAlready = this.Subscribed;

			ContextChart ctx = this.ChartFormManager_nullReported.ContextCurrentChartOrStrategy;
			//if (ctx.IsStreaming != subscribedAlready) {
			//	string msg3 = "CTX_AND_GUI_UNSYNC ctx[" + ctx.ToString() + "] ChartFormStreaming.Subscribed=[" + subscribedAlready + "]";
			//	Assembler.PopupException(msg3 + this.msigForNpExceptions);
			//	ctx.IsStreaming = subscribedAlready;
			//}

			if (subscribedAlready == true) {
				string msg = "CHART_STREAMING_ALREADY_SUBSCRIBED_OR_FORGOT_TO_DISCONNECT REMOVE_INVOCATION_UPSTACK";
				// NOT_NEEDED_WHEN_I_SWITCH_STRATEGIES_IN_EXISTING_CHART
				Assembler.PopupException(msg  + this.msigForNpExceptions);
				//channel.QuotePump.UpdateThreadNameAfterMaxConsumersSubscribed = true;
				//msg = "NOW_QUOTEPUMP_WILL_ASSIGN_THREAD_NAME afterBacktesterCompleteOnceOnWorkspaceRestore() " + msg;
				return;
			}

			var executorSafe				= this.Executor_nullReported;
			var symbolSafe					= this.Symbol_nullReported;
			var scaleIntervalSafe			= this.ScaleInterval_nullReported;
			var streaming_nullUnsafe		= this.StreamingAdapter_nullReported;
			var streamingBarSafeCloneSafe	= this.StreamingBarSafeClone_nullReported;

			bool subscribed = this.Subscribed;
			if (subscribed) {
				string msg = "CHART_STREAMING_ALREADY_SUBSCRIBED_QUOTES_AND_BARS";
				Assembler.PopupException(msg + this.msigForNpExceptions, null, false);
				// RESET_IsStreaming=subscribed return;
			}

			if (streaming_nullUnsafe != null) {
				string branch = " DATA_SOURCE_HAS_STREAMING_ASSIGNED_1/2";
				//NPE_AFTER_SEPARATED_SOLIDIFIERS SymbolScaleDistributionChannel channel = streamingSafe.DataDistributor.GetDistributionChannelForNullUnsafe(symbolSafe, scaleIntervalSafe);
				if (streaming_nullUnsafe.DataDistributor.ConsumerQuoteIsSubscribed(symbolSafe, scaleIntervalSafe, this) == true) {
					Assembler.PopupException("CHART_STREAMING_ALREADY_SUBSCRIBED_CONSUMER_QUOTE" + this.msigForNpExceptions);
				} else {
					//Assembler.PopupException("Subscribing QuoteConsumer [" + this + "]  to " + plug + "  (wasn't registered)");
					bool iWantChartToConsumeQuotesInSeparateThreadToLetStreamingGoWithoutWaitingForStrategyToFinish = true;
					streaming_nullUnsafe.DataDistributor.ConsumerQuoteSubscribe(symbolSafe, scaleIntervalSafe, this,
						 iWantChartToConsumeQuotesInSeparateThreadToLetStreamingGoWithoutWaitingForStrategyToFinish);
				}

				if (streaming_nullUnsafe.DataDistributor.ConsumerBarIsSubscribed(symbolSafe, scaleIntervalSafe, this) == true) {
					Assembler.PopupException("CHART_STREAMING_ALREADY_SUBSCRIBED_CONSUMER_BAR" + this.msigForNpExceptions);
				} else {
					//Assembler.PopupException("Subscribing BarsConsumer [" + this + "] to " + this.ToString() + " (wasn't registered)");
					if (this.chartFormManager.Executor.Bars == null) {
						// in Initialize() this.ChartForm is requesting bars in a separate thread
						streaming_nullUnsafe.DataDistributor.ConsumerBarSubscribe(symbolSafe, scaleIntervalSafe, this, true);
					} else {
						// fully initialized, after streaming was stopped for a moment and resumed - append into PartialBar
						if (double.IsNaN(streamingBarSafeCloneSafe.Open) == false) {
							//streamingSafe.ConsumerBarRegister(symbolSafe, scaleIntervalSafe, this, streamingBarSafeCloneSafe);
							streaming_nullUnsafe.DataDistributor.ConsumerBarSubscribe(symbolSafe, scaleIntervalSafe, this, true);
						} else {
							//streamingSafe.ConsumerBarRegister(symbolSafe, scaleIntervalSafe, this, lastStaticBarSafe);
							streaming_nullUnsafe.DataDistributor.ConsumerBarSubscribe(symbolSafe, scaleIntervalSafe, this, true);
						}
					}
				}

				#region 2/2 LIVESIM_OBEY_BARS_SUBSCRIBED__HANDLED_BY_LIVESIMULATOR  make Livesim also obey Subscribed/Unsubscribed; Simulation of Live must be 100% alike to lifecycle of (and control over) real broker streams
				//var livesimStreamingSafe		= this.LivesimStreamingAdapter;

				//if (livesimStreamingSafe.DataDistributor.ConsumerQuoteIsSubscribed(symbolSafe, scaleIntervalSafe, this) == true) {
				//    Assembler.PopupException("CHART_LIVESIM_STREAMING_ALREADY_SUBSCRIBED_CONSUMER_QUOTE" + this.msigForNpExceptions);
				//} else {
				//    //Assembler.PopupException("Subscribing QuoteConsumer [" + this + "]  to " + plug + "  (wasn't registered)");
				//    livesimStreamingSafe.DataDistributor.ConsumerQuoteSubscribe(symbolSafe, scaleIntervalSafe, this, true);
				//}

				//if (livesimStreamingSafe.DataDistributor.ConsumerBarIsSubscribed(symbolSafe, scaleIntervalSafe, this) == true) {
				//    Assembler.PopupException("CHART_LIVESIM_STREAMING_ALREADY_SUBSCRIBED_CONSUMER_BAR" + this.msigForNpExceptions);
				//} else {
				//    //Assembler.PopupException("Subscribing BarsConsumer [" + this + "] to " + this.ToString() + " (wasn't registered)");
				//    if (this.chartFormManager.Executor.Bars == null) {
				//        // in Initialize() this.ChartForm is requesting bars in a separate thread
				//        livesimStreamingSafe.DataDistributor.ConsumerBarSubscribe(symbolSafe, scaleIntervalSafe, this, true);
				//    } else {
				//        // fully initialized, after streaming was stopped for a moment and resumed - append into PartialBar
				//        if (double.IsNaN(streamingBarSafeCloneSafe.Open) == false) {
				//            //livesimStreamingSafe.ConsumerBarRegister(symbolSafe, scaleIntervalSafe, this, streamingBarSafeCloneSafe);
				//            livesimStreamingSafe.DataDistributor.ConsumerBarSubscribe(symbolSafe, scaleIntervalSafe, this, true);
				//        } else {
				//            //livesimStreamingSafe.ConsumerBarRegister(symbolSafe, scaleIntervalSafe, this, lastStaticBarSafe);
				//            livesimStreamingSafe.DataDistributor.ConsumerBarSubscribe(symbolSafe, scaleIntervalSafe, this, true);
				//        }
				//    }
				//}
				#endregion

				//re-reading to be 100% sure
				subscribed = this.Subscribed;
				if (subscribed == false) {
					string msg = "CHART_STREAMING_FAILED_SUBSCRIBE_BAR_OR_QUOTE_OR_BOTH StreamingAdapter[" + streaming_nullUnsafe.ToString() + "]";
					Assembler.PopupException(msg + this.msigForNpExceptions);
					return;
				}
			} else {
				string msg = "ChartForm: BARS=>SUBSCRIBE_SHOULD_BE_DISABLED_KOZ_NO_STREAMING_IN_DATASOURCE in PopulateBtnStreamingTriggersScript_afterBarsLoaded()";
				Assembler.PopupException(msg);
			}

			this.ChartFormManager_nullReported.ContextCurrentChartOrStrategy.IsStreaming = subscribed;
			this.ChartFormManager_nullReported.Strategy.Serialize();

			string msg2 = "CHART_STREAMING_SUBSCRIBED[" + subscribed + "] due to [" + reason + "]";
			Assembler.PopupException(msg2 + this.msigForNpExceptions, null, false);

			//if (ctx.IsStreaming == subscribed) {
			//    string msg3 = "ON_INITIALIZE_WITH_STRATEGY_I_CAUGHT_UP_WITH_CTX_SETTING"	// ?ALREADY_SUBSCRIBED ?CHART_STREAMING_UNSYNC_CTX"
			//        + " ctx[" + ctx.ToString() + "] ChartFormStreaming.Subscribed=[" + subscribedAlready + "]";
			//    Assembler.PopupException(msg3 + this.msigForNpExceptions, null, false);
			//}

			// MAY_NEED_TO_REFACTOR_BROKER_MOCK__AND_OrderPostProcessorSequencerCloseThenOpen
			// BROKER_MOCK_MUST_BE_PAUSED: DONT_INVOKE_ORDER_UPDATE_BEFORE_POSITION_WAS_REGISTERED_IN_OrderProcessor.DataSnapshot.OrdersPending.ScanRecentForGUID
			// IDEA_IS_TO_AVOID__WAITED_TOO_LONG_FOR_UNPAUSE_CONFIRMATION___BY_SETTING_UpdateThreadNameAfterMaxConsumersSubscribed=true
			// safeSync this.Subscribed => ContextCurrentChartOrStrategy.IsStreaming, => channel.QuotePump.UpdateThreadNameAfterMaxConsumersSubscribed
			//var chartFormSafe = this.ChartForm;
			//if (chartFormSafe.ChartControl.ScriptExecutorObjects.QuoteLast != null) {
			//	string msg = "CHART_STREAMING_SUBSCRIBED_CLEANED_UP_EXISTING_QUOTE_LAST WASNT_SHUA...";
			//	Assembler.PopupException(msg + this.msigForNpExceptions, null, false);
			//	chartFormSafe.ChartControl.ScriptExecutorObjects.QuoteLast = null;
			//}
			// don't on BacktestOnRestart && IsStreaming
			//bool runStartupBacktestDontChangePumpThreadNameKeepPaused = false;
			//ContextChart ctxChart = this.chartFormManager.ContextCurrentChartOrStrategy;
			//ContextScript ctxScript = ctxChart as ContextScript;
			//if (ctxScript != null) {
			//	runStartupBacktestDontChangePumpThreadNameKeepPaused =
			//			true == ctxScript.WillBacktestOnAppRestart
			//		&& false == Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete;

			//	if (runStartupBacktestDontChangePumpThreadNameKeepPaused
			//		&& channel.QuotePump.IshouldWaitConfirmationFromAnotherThread == true) {
			//		string msg = "ON_APPRESTART_BACKTEST_PUMP_SHOULD_WAIT_UNPAUSING_FROM_THE SAME_BACKTESTING_THREAD";
			//		Assembler.PopupException(msg);
			//		return;
			//	}
			//}
			//if (runStartupBacktestDontChangePumpThreadNameKeepPaused) return;
			//channel.QuotePump.UpdateThreadNameAfterMaxConsumersSubscribed = true;
		}
		public bool Subscribed { get {
				if (this.canSubscribeToStreamingAdapter() == false) return false;	// NULL_POINTERS_ARE_ALREADY_REPORTED_TO_EXCEPTIONS_FORM

				var streamingSafe = this.StreamingAdapter_nullReported;
				var symbolSafe = this.Symbol_nullReported;
				var scaleIntervalSafe = this.ScaleInterval_nullReported;

				bool quote	= streamingSafe.DataDistributor.ConsumerQuoteIsSubscribed(	symbolSafe, scaleIntervalSafe, this);
				bool bar	= streamingSafe.DataDistributor.ConsumerBarIsSubscribed(	symbolSafe, scaleIntervalSafe, this);
				bool ret = quote & bar;
				return ret;
			}}

		public void StreamingTriggeringScriptStart() {
			this.Executor_nullReported.IsStreamingTriggeringScript = true;
		}
		public void StreamingTriggeringScriptStop() {
			this.Executor_nullReported.IsStreamingTriggeringScript = false;
		}

		#region IStreamingConsumer
		Bars IStreamingConsumer.ConsumerBarsToAppendInto { get { return this.Bars_nullReported; } }
		void IStreamingConsumer.UpstreamSubscribedToSymbolNotification(Quote quoteFirstAfterStart) {
		}
		void IStreamingConsumer.UpstreamUnSubscribedFromSymbolNotification(Quote quoteLastBeforeStop) {
		}
		void IStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(Bar barLastFormed, Quote quoteForAlertsCreated) {
			if (barLastFormed == null) {
				string msg = "WRONG_SHOW_BRO";
				Assembler.PopupException(msg);
			}
			this.msigForNpExceptions = " //ChartFormStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(" + barLastFormed.ToString() + ")";

			#if DEBUG	// TEST_INLINE
			var barsSafe = this.Bars_nullReported;
			if (barsSafe.ScaleInterval != barLastFormed.ScaleInterval) {
				string msg = "SCALEINTERVAL_RECEIVED_DOESNT_MATCH_CHARTS ChartForm[" + this.ChartForm_nullReported.Text + "]"
					+ " bars[" + barsSafe.ScaleInterval + "] barLastFormed[" + barLastFormed.ScaleInterval + "]";
				Assembler.PopupException(msg + this.msigForNpExceptions);
				return;
			}
			if (barsSafe.Symbol != barLastFormed.Symbol) {
				string msg = "SYMBOL_RECEIVED_DOESNT_MATCH_CHARTS ChartForm[" + this.ChartForm_nullReported.Text + "]"
					+ " bars[" + barsSafe.Symbol + "] barLastFormed[" + barLastFormed.Symbol + "]";
				Assembler.PopupException(msg + this.msigForNpExceptions);
				return;
			}
			#endif

			var chartFormSafe		= this.ChartForm_nullReported;
			var executorSafe		= this.Executor_nullReported;
			var dataSourceSafe		= this.DataSource_nullReported;

			if (barLastFormed == null) {
				string msg = "Streaming starts generating quotes => first StreamingBar is added; for first four Quotes there's no static barsFormed yet!! Isi";
				Assembler.PopupException(msg + this.msigForNpExceptions);
				return;
			}

			if (executorSafe.Strategy != null && executorSafe.IsStreamingTriggeringScript) {
				try {
					bool thereWereNeighbours = dataSourceSafe.PumpPauseNeighborsIfAnyFor(executorSafe);		// NOW_FOR_LIVE_MOCK_BUFFERING
					// TESTED BACKLOG_GREWUP Thread.Sleep(450);	// 10,000msec = 10sec
					ReporterPokeUnit pokeUnitNullUnsafe = executorSafe.ExecuteOnNewBarOrNewQuote(quoteForAlertsCreated, false);	//new Quote());
					//UNFILLED_POSITIONS_ARE_USELESS chartFormManager.ReportersFormsManager.BuildIncrementalAllReports(pokeUnit);
				} finally {
					bool thereWereNeighbours = dataSourceSafe.PumpResumeNeighborsIfAnyFor(executorSafe);	// NOW_FOR_LIVE_MOCK_BUFFERING
				}
			}

			#if DEBUG
			if (this.ChartFormManager_nullReported.Executor.Backtester.IsBacktestingNoLivesimNow) {
				string msg = "SHOULD_NEVER_HAPPEN IsBacktestingNoLivesimNow[true] //ChartFormStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended()";
				Assembler.PopupException(msg);
				return;
			}
			#endif

			if (this.ChartFormManager_nullReported.ContextCurrentChartOrStrategy.IsStreaming) {
				chartFormSafe.ChartControl.InvalidateAllPanels();
			}
		}
		void IStreamingConsumer.ConsumeQuoteOfStreamingBar(Quote quote) {
			this.msigForNpExceptions = " //ChartFormStreamingConsumer.ConsumeQuoteOfStreamingBar(" + quote.ToString() + ")";

			#if DEBUG	// TEST_INLINE_BEGIN
			var barsSafe = this.Bars_nullReported;
			if (barsSafe.ScaleInterval != quote.ParentBarStreaming.ScaleInterval) {
				string msg = "SCALEINTERVAL_RECEIVED_DOESNT_MATCH_CHARTS ChartForm[" + this.ChartForm_nullReported.Text + "]"
					+ " bars[" + barsSafe.ScaleInterval + "] quote.ParentStreamingBar[" + quote.ParentBarStreaming.ScaleInterval + "]";
				Assembler.PopupException(msg + this.msigForNpExceptions);
				return;
			}
			if (barsSafe.Symbol != quote.ParentBarStreaming.Symbol) {
				string msg = "SYMBOL_RECEIVED_DOESNT_MATCH_CHARTS ChartForm[" + this.ChartForm_nullReported.Text + "]"
					+ " bars[" + barsSafe.Symbol + "] quote.ParentStreamingBar[" + quote.ParentBarStreaming.Symbol + "]";
				Assembler.PopupException(msg + this.msigForNpExceptions);
				return;
			}
			string msg2 = "BARS_IDENTICAL";
			bool sameDOHLCV = barsSafe.BarStreamingNullUnsafe.HasSameDOHLCVas(quote.ParentBarStreaming, "quote.ParentStreamingBar", "barsSafe.BarStreaming", ref msg2);
			if (sameDOHLCV == false) {
				string msg = "FIXME_MUST_BE_THE_SAME EARLY_BINDER_DIDNT_DO_ITS_JOB#3 [" + msg2 + "] this.Executor.Bars.BarStreaming[" + barsSafe.BarStreamingNullUnsafe
					+ "].HasSameDOHLCVas(quote.ParentStreamingBar[" + quote.ParentBarStreaming + "])=false";
				Assembler.PopupException(msg + this.msigForNpExceptions);
				return;
			}
			if (barsSafe.BarStreamingNullUnsafe != quote.ParentBarStreaming) {
				string msg = "SHOULD_THEY_BE_CLONES_OR_SAME? EARLY_BINDER_DIDNT_DO_ITS_JOB#3 bars[" + barsSafe
					+ "] quote.ParentStreamingBar[" + quote.ParentBarStreaming + "]";
				Assembler.PopupException(msg + this.msigForNpExceptions);
				return;
			}
			#endif	// TEST_INLINE_END

			var streamingSafe = this.StreamingAdapter_nullReported;
			var chartFormSafe = this.ChartForm_nullReported;
			var executorSafe = this.Executor_nullReported;
			var dataSourceSafe = this.DataSource_nullReported;

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
			chartFormSafe.PrintQuoteTimestampOnStrategyTriggeringButton_beforeExecution_switchToGuiThread(quote);

			// #2/4 execute strategy in the thread of a StreamingAdapter (DDE server for MockQuickProvider)
			if (executorSafe.Strategy != null) {
				if (executorSafe.IsStreamingTriggeringScript) {
					try {
						bool thereWereNeighbours = dataSourceSafe.PumpPauseNeighborsIfAnyFor(executorSafe);		// NOW_FOR_LIVE_MOCK_BUFFERING
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

					// FROM_ChartFormStreamingConsumer.ConsumeQuoteOfStreamingBar() #4/4 notify Positions that it should update open positions, I wanna see current profit/loss and relevant red/green background
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
			//v1
			//this.msigForNpExceptions = "ToString(): ";
			//if (this.chartFormManager == null) return "ChartStreamingConsumer::chartFormsManager=null";
			//if (this.chartFormManager.ChartForm == null) return "ChartStreamingConsumer::chartFormsManager.ChartForm=null";
			//if (this.chartFormManager.ChartForm.IsDisposed) return "CHARTFORM_DISPOSED";
			//if (this.chartFormManager.Executor == null) return "ChartStreamingConsumer::chartFormsManager.Executor=null";
			//if (this.chartFormManager.Executor.Strategy == null) return "ChartStreamingConsumer::chartFormsManager.Executor.Strategy=null";
			//if (this.chartFormManager.Executor.Strategy.ScriptContextCurrent == null) return "ChartStreamingConsumer::chartFormsManager.Executor.Strategy.ScriptContextCurrent=null";
			//if (String.IsNullOrEmpty(this.chartFormManager.Executor.Strategy.ScriptContextCurrent.Symbol)) return "SYMBOL_EMPTY_NOT_SUBSCRIBED";
			//return this.ChartFormManager.StreamingButtonIdent
			//	//+ " [" + this.Strategy.ScriptContextCurrent.Symbol + " " + this.Strategy.ScriptContextCurrent.ScaleInterval + "]"
			//	////+ " chart[" + this.ChartContainer.Text + "]"
			//	//+ " streaming[" + this.chartFormsManager.Executor.DataSource.StreamingAdapter.Name + "]"
			//	//+ " static[" + this.chartFormsManager.Executor.DataSource.StaticProvider.Name + "]"
			//	;
			//v2
			var symbolSafe = this.Symbol_nullReported;
			var chartFormSafe = this.ChartForm_nullReported;
			var scaleIntervalSafe = this.ScaleInterval_nullReported;
			string ret = "ChartForm.Symbol[" + symbolSafe + "](" + scaleIntervalSafe + ")";

			//HANGS_ON_STARTUP__#D_STACK_IS_BLANK__VS2010_HINTED_IM_ACCESSING_this.ChartForm.Text_FROM_DDE_QUOTE_GENERATOR (!?!?!)
			if (chartFormSafe.InvokeRequired == false) {
				ret += " CHART.TEXT[" + chartFormSafe.Text + "]";
			} else {
//				ChartFormDataSnapshot snap = this.chartFormManager.DataSnapshot;
//				if (snap == null) {
//					Assembler.PopupException(null);
//				}
//				ContextChart ctx = this.chartFormManager.DataSnapshot.ContextChart;
				ret += (this.Executor_nullReported.Strategy != null)
					? " ScriptContextCurrent[" + this.Executor_nullReported.Strategy.ScriptContextCurrent.ToString() + "]"
					: " ContextChart[" + this.chartFormManager.DataSnapshot.ContextChart.ToString() + "]"
					;
			}

			return "{" + ret + "}";
		}
	}
}