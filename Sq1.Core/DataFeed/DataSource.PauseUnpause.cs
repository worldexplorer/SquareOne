using System;

using Sq1.Core.Streaming;
using Sq1.Core.StrategyBase;
using Sq1.Core.DataTypes;
using Sq1.Core.Livesim;
using Sq1.Core.Backtesting;
using Sq1.Core.Charting;

namespace Sq1.Core.DataFeed {
	public partial class DataSource {

		public bool QueuePauseIgnorePump_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(ScriptExecutor executorImBacktesting, bool wrongUsagePopup = true) {
			SymbolChannel<StreamingConsumerChart> channel = this.StreamingAdapter.DistributorCharts_substitutedDuringLivesim.GetSymbolChannelFor_nullMeansWasntSubscribed(executorImBacktesting.Bars.Symbol);
			string msig = " //QueuePauseIgnorePump_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(" + executorImBacktesting + ")";
			if (channel == null) {
				string msg = "NOT_AN_ERROR__BACKTESTER_EMPLOYS_OWN_QUEUE__NO_CHART_NOR_SOLIDIFIER_AT_TIMEFRAME_DIFFERENT_TO_DS'S_CAN_BE_POSSIBLE"
					//+ " THERE_MUSTBE_AT_LEAST_ONE_EXECUTOR_THAT_INVOKED_ME_UPSTACK"
					;
				Assembler.PopupException(msg + msig);
				return false;
			}
			if (channel.ImQueueNotPump_trueOnlyForBacktest == false ||
				channel.QueueWhenBacktesting_PumpForLiveAndLivesim.HasSeparatePushingThread == true) {
				if (wrongUsagePopup == true) {
					string msg = "WILL_NOT_PAUSE_PUMP DANGEROUS_DROPPING_INCOMING_QUOTES__PUSHING_THREAD_HAVENT_STARTED (review how you use QuotePump)";
					Assembler.PopupException(msg + msig);
				}
				return false;
			}
			channel.QueueOrPumpPause_addBacktesterLaunchingScript_eachQuote(executorImBacktesting.BacktesterOrLivesimulator);
			return true;
		}
		public bool QueueResumeIgnorePump_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(ScriptExecutor executorImBacktesting, bool wrongUsagePopup = true) {
			string msig = " //QueueResumeIgnorePump_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(" + executorImBacktesting + ")";
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) {
				string msg = "I_REFUSE_TO_RESUME_PUMP_BECAUSE_IT_LEADS_TO_DEADLOCK IM_CLOSING_MAINFORM_WHILE_LIVESIM_IS_RUNNING";
				Assembler.PopupException(msg + msig, null, false);
				return false;
			}

			SymbolChannel<StreamingConsumerChart> channel = this.StreamingAdapter.DistributorCharts_substitutedDuringLivesim.GetSymbolChannelFor_nullMeansWasntSubscribed(executorImBacktesting.Bars.Symbol);
			if (channel == null) {
				string msg = "NOT_AN_ERROR__BACKTESTER_EMPLOYS_OWN_QUEUE__NO_CHART_NOR_SOLIDIFIER_AT_TIMEFRAME_DIFFERENT_TO_DS'S_CAN_BE_POSSIBLE"
					//+ " THERE_MUSTBE_AT_LEAST_ONE_EXECUTOR_THAT_INVOKED_ME_UPSTACK"
					;
				Assembler.PopupException(msg + msig);
				return false;
			}

			if (channel.ImQueueNotPump_trueOnlyForBacktest == false ||
				channel.QueueWhenBacktesting_PumpForLiveAndLivesim.HasSeparatePushingThread == true) {
				if (wrongUsagePopup == true) {
					string msg = "WILL_NOT_UNPAUSE_PUMP DANGEROUS_I_MIGHT_HAVE_DROPPED_ALREADY_A_FEW_QUOTES__PUSHING_THREAD_HAVENT_STARTED (review how you use QuotePump)";
					Assembler.PopupException(msg + msig, null, false);
				}
				return false;
			}
			channel.QueueOrPumpResume_removeBacktesterFinishedScript_eachQuote(executorImBacktesting.BacktesterOrLivesimulator);
			return true;
		}
		public bool OwnLivesimHelper_PumpPause_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(ScriptExecutor executorImLivesimming, bool wrongUsagePopup = true) {
			SymbolChannel<StreamingConsumerChart> channel = this.StreamingAdapter.DistributorCharts_substitutedDuringLivesim.GetSymbolChannelFor_nullMeansWasntSubscribed(executorImLivesimming.Bars.Symbol);
			string msig = " //OwnLivesimPumpHelper_PumpPause_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(" + executorImLivesimming + ")";
			if (channel == null) {
				string msg = "NOT_AN_ERROR__BACKTESTER_EMPLOYS_OWN_QUEUE__NO_CHART_NOR_SOLIDIFIER_AT_TIMEFRAME_DIFFERENT_TO_DS'S_CAN_BE_POSSIBLE"
					//+ " THERE_MUSTBE_AT_LEAST_ONE_EXECUTOR_THAT_INVOKED_ME_UPSTACK"
					;
				Assembler.PopupException(msg + msig);
				return false;
			}
			if (channel.ImQueueNotPump_trueOnlyForBacktest == true ||
				channel.QueueWhenBacktesting_PumpForLiveAndLivesim.HasSeparatePushingThread == false) {
				if (wrongUsagePopup == true) {
					string msg = "WILL_PAUSE DANGEROUS_DROPPING_INCOMING_QUOTES__PUSHING_THREAD_HAVENT_STARTED (review how you use QuotePump)";
					Assembler.PopupException(msg + msig);
				}
				//return false;
			}
			channel.QueueOrPumpPause_addBacktesterLaunchingScript_eachQuote(executorImLivesimming.BacktesterOrLivesimulator);
			return true;
		}
		public bool OwnLivesimHelper_PumpResume_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(ScriptExecutor executorImLivesimming, bool wrongUsagePopup = true) {
			string msig = " //OwnLivesimHelper_PumpResume_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(" + executorImLivesimming + ")";
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) {
				string msg = "I_REFUSE_TO_RESUME_PUMP_BECAUSE_IT_LEADS_TO_DEADLOCK IM_CLOSING_MAINFORM_WHILE_LIVESIM_IS_RUNNING";
				Assembler.PopupException(msg + msig, null, false);
				return false;
			}

			SymbolChannel<StreamingConsumerChart> channel = this.StreamingAdapter.DistributorCharts_substitutedDuringLivesim.GetSymbolChannelFor_nullMeansWasntSubscribed(executorImLivesimming.Bars.Symbol);
			if (channel == null) {
				string msg = "NOT_AN_ERROR__BACKTESTER_EMPLOYS_OWN_QUEUE__NO_CHART_NOR_SOLIDIFIER_AT_TIMEFRAME_DIFFERENT_TO_DS'S_CAN_BE_POSSIBLE"
					//+ " THERE_MUSTBE_AT_LEAST_ONE_EXECUTOR_THAT_INVOKED_ME_UPSTACK"
					;
				Assembler.PopupException(msg + msig);
				return false;
			}

			if (channel.ImQueueNotPump_trueOnlyForBacktest == true ||
				channel.QueueWhenBacktesting_PumpForLiveAndLivesim.HasSeparatePushingThread == false) {
				if (wrongUsagePopup == true) {
					string msg = "WILL_UNPAUSE DANGEROUS_I_MIGHT_HAVE_DROPPED_ALREADY_A_FEW_QUOTES__PUSHING_THREAD_HAVENT_STARTED (review how you use QuotePump)";
					Assembler.PopupException(msg + msig, null, false);
				}
				//return false;
			}
			channel.QueueOrPumpResume_removeBacktesterFinishedScript_eachQuote(executorImLivesimming.BacktesterOrLivesimulator);
			return true;
		}
		//public bool PumpingPausedGet(Bars bars) {
		//	Distributor distr = this.StreamingAdapter.Distributor;
		//	SymbolScaleDistributionChannel channel = distr.GetDistributionChannelFor_nullUnsafe(bars.Symbol, bars.ScaleInterval);
		//	bool paused = channel.QuotePump.Paused;
		//	return paused;
		//}
		//public bool PumpingWaitUntilUnpaused(Bars bars, int maxWaitingMillis = 1000) {
		//	Distributor distr = this.StreamingAdapter.Distributor;
		//	SymbolScaleDistributionChannel channel = distr.GetDistributionChannelFor_nullUnsafe(bars.Symbol, bars.ScaleInterval);
		//	bool unpaused = channel.QuotePump.WaitUntilUnpaused(maxWaitingMillis);
		//	return unpaused;
		//}
		public bool PumpingWaitUntilPaused(Bars bars, int maxWaitingMillis = 1000) {
			//Distributor distr = this.StreamingAdapter.Distributor_replacedForLivesim;
			//SymbolScaleStream channel = distr.GetStreamFor_nullUnsafe(bars.Symbol, bars.ScaleInterval);
			SymbolChannel<StreamingConsumerChart> channel = this.StreamingAdapter.DistributorCharts_substitutedDuringLivesim.GetSymbolChannelFor_nullMeansWasntSubscribed(bars.Symbol);
			bool paused = channel.QueueWhenBacktesting_PumpForLiveAndLivesim.WaitUntilPaused(maxWaitingMillis);
			return paused;
		}

		internal bool QueuePauseIgnorePump_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst_WRAPPER(ScriptExecutor executor, Bars barsEmptyButWillGrow) {
			bool someoneGotPaused = false;
			if (this.StreamingAdapter is LivesimStreaming) {
				someoneGotPaused = this.StreamingAsLivesim_nullUnsafe
					.BacktestContextInitialize_pauseQueueForBacktest_leavePumpUnpausedForLivesimDefault_overrideable(executor, barsEmptyButWillGrow);
			} else {
				if (this.StreamingAdapter is BacktestStreaming) {
					someoneGotPaused = this.StreamingAsBacktest_nullUnsafe
						.BacktestContextInitialize_pauseQueueForBacktest_leavePumpUnpausedForLivesimDefault_overrideable(executor, barsEmptyButWillGrow);
				}
			}
			return someoneGotPaused;
		}

		internal bool QueueResumeIgnorePump_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst_WRAPPER(ScriptExecutor executor) {
			bool someoneGotUnPaused = false;
			if (this.StreamingAdapter is LivesimStreaming) {
				someoneGotUnPaused = this.StreamingAsLivesim_nullUnsafe
					.BacktestContextRestore_unpauseQueueForBacktest_leavePumpUnPausedForLivesimDefault_overrideable(executor);
			} else {
				if (this.StreamingAdapter is BacktestStreaming) {
					someoneGotUnPaused = this.StreamingAsBacktest_nullUnsafe
						.BacktestContextRestore_unpauseQueueForBacktest_leavePumpUnPausedForLivesimDefault_overrideable(executor);
				}
			}
			return someoneGotUnPaused;
		}

		#region v1 BROKER_IN_TEST_MODE_WILL_NOT_SEND_ORDERS_FOR_NON_LIVESIMMING_SYMBOLS
		public int LivesimStreaming_PumpPause_forSameSymbolScale_freezeNonSimmingConsumers_whenBrokerOriginalIsLivesimDefault(ScriptExecutor executorImLivesimming, bool wrongUsagePopup = true) {
		    string msig = " //LivesimStreaming_PumpPause_forSameSymbolScale_freezeNonSimmingConsumers_whenBrokerOriginalIsLivesimDefault(" + executorImLivesimming + ")";
		    int channelsPaused = 0;
		    //List<SymbolScaleStream> channels = this.StreamingAdapter.Distributor_replacedForLivesim
		    //    .GetStreams_allScaleIntervals_forSymbol(executorImLivesimming.Bars.Symbol);
		    //foreach (SymbolScaleStream channel in channels) {
		    SymbolChannel<StreamingConsumerChart> channel_nullUnsafe = this.StreamingAdapter.DistributorCharts_substitutedDuringLivesim.GetSymbolChannelFor_nullMeansWasntSubscribed(executorImLivesimming.Bars.Symbol);
		    if (channel_nullUnsafe == null) return channelsPaused;		// no charts (including the livesimming one) were subscribed

		    if (channel_nullUnsafe.ImQueueNotPump_trueOnlyForBacktest == true ||
		        channel_nullUnsafe.QueueWhenBacktesting_PumpForLiveAndLivesim.HasSeparatePushingThread == false) {
		        if (wrongUsagePopup == true) {
		            string msg = "WILL_PAUSE DANGEROUS_DROPPING_INCOMING_QUOTES__PUSHING_THREAD_HAVENT_STARTED (review how you use QuotePump)";
		            Assembler.PopupException(msg + msig);
		        }
		        return channelsPaused;
		    }

		    if (channel_nullUnsafe.PumpQuote_nullWhenBacktesting.Paused == true) {
		        string msg = "PUMP_QUOTE_ALREADY_PAUSED_BY_ANOTHER_LIVESIM";
		        Assembler.PopupException(msg, null, false);
		        return channelsPaused;
		    }
		    channel_nullUnsafe.PumpQuote_nullWhenBacktesting.PusherPause_waitUntilPaused();
		    channelsPaused++;

		    if (channel_nullUnsafe.PumpLevelTwo.Paused == true) {
		        string msg = "PUMP_LEVEL_TWO_ALREADY_PAUSED_BY_ANOTHER_LIVESIM";
		        Assembler.PopupException(msg, null, false);
		        return channelsPaused;
		    }
		    channel_nullUnsafe.PumpLevelTwo.PusherPause_waitUntilPaused();
		    channelsPaused++;

		    return channelsPaused;
		}
		public int LivesimStreaming_TwoPumpsResume_forSameSymbolScale_unfreezeOtherConsumers_whenBrokerOriginalIsLivesimDefault(ScriptExecutor executorImLivesimming, bool wrongUsagePopup = true) {
		    string msig = " //LivesimStreaming_PumpResume_forSameSymbolScale_unfreezeOtherConsumers_whenBrokerOriginalIsLivesimDefault(" + executorImLivesimming + ")";
		    int channelsPaused = 0;
		    if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) {
		        string msg = "I_REFUSE_TO_RESUME_PUMP_BECAUSE_IT_LEADS_TO_DEADLOCK IM_CLOSING_MAINFORM_WHILE_LIVESIM_IS_RUNNING";
		        Assembler.PopupException(msg + msig, null, false);
		        return channelsPaused;
		    }

		    //List<SymbolScaleStream> channels = this.StreamingAdapter.Distributor_replacedForLivesim
		    //    .GetStreams_allScaleIntervals_forSymbol(executorImLivesimming.Bars.Symbol);
		    //foreach (SymbolScaleStream channel in channels) {
		    SymbolChannel<StreamingConsumerChart> channel_nullUnsafe = this.StreamingAdapter.DistributorCharts_substitutedDuringLivesim.GetSymbolChannelFor_nullMeansWasntSubscribed(executorImLivesimming.Bars.Symbol);
		    if (channel_nullUnsafe == null) return channelsPaused;		// no charts (including the livesimming one) were subscribed

		    if (channel_nullUnsafe.ImQueueNotPump_trueOnlyForBacktest == true ||
		        channel_nullUnsafe.QueueWhenBacktesting_PumpForLiveAndLivesim.HasSeparatePushingThread == false) {
		        if (wrongUsagePopup == true) {
		            string msg = "WILL_UNPAUSE DANGEROUS_I_MIGHT_HAVE_DROPPED_ALREADY_A_FEW_QUOTES__PUSHING_THREAD_HAVENT_STARTED (review how you use QuotePump)";
		            Assembler.PopupException(msg + msig, null, false);
		        }
		        return channelsPaused;
		    }

		    if (channel_nullUnsafe.PumpQuote_nullWhenBacktesting.Paused == false) {
		        string msg = "PUMP_QUOTE_ALREADY_UNPAUSED_BY_ANOTHER_LIVESIM";
		        Assembler.PopupException(msg, null, false);
		        return channelsPaused;
		    }
		    channel_nullUnsafe.PumpQuote_nullWhenBacktesting.PusherUnpause_waitUntilUnpaused();
		    channelsPaused++;

		    if (channel_nullUnsafe.PumpLevelTwo.Paused == false) {
		        string msg = "PUMP_LEVEL2_ALREADY_UNPAUSED_BY_ANOTHER_LIVESIM";
		        Assembler.PopupException(msg, null, false);
		        return channelsPaused;
		    }
		    channel_nullUnsafe.PumpLevelTwo.PusherUnpause_waitUntilUnpaused();
		    channelsPaused++;

			return channelsPaused;
		}
		#endregion

		//v2
		public int LivesimStreaming_PumpsAllPause_forAllSymbolsScales_freezeAllConsumers_whenBrokerOriginalIsReal(ScriptExecutor executorImLivesimming) {
			string msig = " //LivesimStreaming_PumpsAllPause_forAllSymbolsScales_freezeAllConsumers_whenBrokerOriginalIsReal(" + executorImLivesimming + ")";
			DistributorCharts distributorCharts = this.StreamingAdapter.DistributorCharts_substitutedDuringLivesim;
			int pumpsPaused = distributorCharts.TwoPushingPumpsPerSymbol_Pause_forAllSymbol_duringLivesimmingOne(msig);
			return pumpsPaused;
		}
		public int LivesimStreaming_PumpsAllResume_forAllSymbolsScales_unfreezeAllConsumers_whenBrokerOriginalIsReal(ScriptExecutor executorImLivesimming) {
			string msig = " //LivesimStreaming_PumpsAllResume_forAllSymbolsScales_unfreezeAllConsumers_whenBrokerOriginalIsReal(" + executorImLivesimming + ")";
			DistributorCharts distributorCharts = this.StreamingAdapter.DistributorCharts_substitutedDuringLivesim;
			int pumpsUnpaused = distributorCharts.TwoPushingPumpsPerSymbol_Unpause_forAllSymbol_afterLivesimmingOne(msig);
			return pumpsUnpaused;
		}
	}
}
