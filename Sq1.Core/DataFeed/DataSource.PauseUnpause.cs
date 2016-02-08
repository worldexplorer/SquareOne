using System;

using Sq1.Core.Streaming;
using Sq1.Core.StrategyBase;
using Sq1.Core.DataTypes;
using Sq1.Core.Livesim;
using Sq1.Core.Backtesting;
using System.Collections.Generic;

namespace Sq1.Core.DataFeed {
	public partial class DataSource {

		public bool QueuePauseIgnorePump_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(ScriptExecutor executorImBacktesting, bool wrongUsagePopup = true) {
			SymbolScaleDistributionChannel channel = this.StreamingAdapter.DataDistributor_replacedForLivesim
				.GetDistributionChannelFor_nullUnsafe(executorImBacktesting.Bars.Symbol, executorImBacktesting.Bars.ScaleInterval);
			string msig = " //QueuePauseIgnorePump_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(" + executorImBacktesting + ")";
			if (channel == null) {
				string msg = "NOT_AN_ERROR__BACKTESTER_EMPLOYS_OWN_QUEUE__NO_CHART_NOR_SOLIDIFIER_AT_TIMEFRAME_DIFFERENT_TO_DS'S_CAN_BE_POSSIBLE"
					//+ " THERE_MUSTBE_AT_LEAST_ONE_EXECUTOR_THAT_INVOKED_ME_UPSTACK"
					;
				Assembler.PopupException(msg + msig);
				return false;
			}
			if (channel.ImQueueNotPump_trueOnlyForBacktest == false ||
				channel.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim.HasSeparatePushingThread == true) {
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

			SymbolScaleDistributionChannel channel = this.StreamingAdapter.DataDistributor_replacedForLivesim
				.GetDistributionChannelFor_nullUnsafe(executorImBacktesting.Bars.Symbol, executorImBacktesting.Bars.ScaleInterval);
			if (channel == null) {
				string msg = "NOT_AN_ERROR__BACKTESTER_EMPLOYS_OWN_QUEUE__NO_CHART_NOR_SOLIDIFIER_AT_TIMEFRAME_DIFFERENT_TO_DS'S_CAN_BE_POSSIBLE"
					//+ " THERE_MUSTBE_AT_LEAST_ONE_EXECUTOR_THAT_INVOKED_ME_UPSTACK"
					;
				Assembler.PopupException(msg + msig);
				return false;
			}

			if (channel.ImQueueNotPump_trueOnlyForBacktest == false ||
				channel.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim.HasSeparatePushingThread == true) {
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
			SymbolScaleDistributionChannel channel = this.StreamingAdapter.DataDistributor_replacedForLivesim
				.GetDistributionChannelFor_nullUnsafe(executorImLivesimming.Bars.Symbol, executorImLivesimming.Bars.ScaleInterval);
			string msig = " //OwnLivesimPumpHelper_PumpPause_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(" + executorImLivesimming + ")";
			if (channel == null) {
				string msg = "NOT_AN_ERROR__BACKTESTER_EMPLOYS_OWN_QUEUE__NO_CHART_NOR_SOLIDIFIER_AT_TIMEFRAME_DIFFERENT_TO_DS'S_CAN_BE_POSSIBLE"
					//+ " THERE_MUSTBE_AT_LEAST_ONE_EXECUTOR_THAT_INVOKED_ME_UPSTACK"
					;
				Assembler.PopupException(msg + msig);
				return false;
			}
			if (channel.ImQueueNotPump_trueOnlyForBacktest == true ||
				channel.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim.HasSeparatePushingThread == false) {
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

			SymbolScaleDistributionChannel channel = this.StreamingAdapter.DataDistributor_replacedForLivesim
				.GetDistributionChannelFor_nullUnsafe(executorImLivesimming.Bars.Symbol, executorImLivesimming.Bars.ScaleInterval);
			if (channel == null) {
				string msg = "NOT_AN_ERROR__BACKTESTER_EMPLOYS_OWN_QUEUE__NO_CHART_NOR_SOLIDIFIER_AT_TIMEFRAME_DIFFERENT_TO_DS'S_CAN_BE_POSSIBLE"
					//+ " THERE_MUSTBE_AT_LEAST_ONE_EXECUTOR_THAT_INVOKED_ME_UPSTACK"
					;
				Assembler.PopupException(msg + msig);
				return false;
			}

			if (channel.ImQueueNotPump_trueOnlyForBacktest == true ||
				channel.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim.HasSeparatePushingThread == false) {
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
		//    DataDistributor distr = this.StreamingAdapter.DataDistributor;
		//    SymbolScaleDistributionChannel channel = distr.GetDistributionChannelFor_nullUnsafe(bars.Symbol, bars.ScaleInterval);
		//    bool paused = channel.QuotePump.Paused;
		//    return paused;
		//}
		//public bool PumpingWaitUntilUnpaused(Bars bars, int maxWaitingMillis = 1000) {
		//    DataDistributor distr = this.StreamingAdapter.DataDistributor;
		//    SymbolScaleDistributionChannel channel = distr.GetDistributionChannelFor_nullUnsafe(bars.Symbol, bars.ScaleInterval);
		//    bool unpaused = channel.QuotePump.WaitUntilUnpaused(maxWaitingMillis);
		//    return unpaused;
		//}
		public bool PumpingWaitUntilPaused(Bars bars, int maxWaitingMillis = 1000) {
			DataDistributor distr = this.StreamingAdapter.DataDistributor_replacedForLivesim;
			SymbolScaleDistributionChannel channel = distr.GetDistributionChannelFor_nullUnsafe(bars.Symbol, bars.ScaleInterval);
			bool paused = channel.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim.WaitUntilPaused(maxWaitingMillis);
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

		public int LivesimStreamingDefault_PumpPause_freezeOtherConsumers_forSameSymbolScale(ScriptExecutor executorImLivesimming, bool wrongUsagePopup = true) {
			string msig = " //LivesimStreamingDefault_PumpPause_freezeOtherConsumers_forSameSymbolScale(" + executorImLivesimming + ")";
			List<SymbolScaleDistributionChannel> channels = this.StreamingAdapter.DataDistributor_replacedForLivesim
				.GetDistributionChannels_allScaleIntervals_forSymbol(executorImLivesimming.Bars.Symbol);
			int channelsPaused = 0;
			foreach (SymbolScaleDistributionChannel channel in channels) {
				if (channel.ImQueueNotPump_trueOnlyForBacktest == true ||
					channel.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim.HasSeparatePushingThread == false) {
					if (wrongUsagePopup == true) {
						string msg = "WILL_PAUSE DANGEROUS_DROPPING_INCOMING_QUOTES__PUSHING_THREAD_HAVENT_STARTED (review how you use QuotePump)";
						Assembler.PopupException(msg + msig);
					}
					continue;
				}
				if (channel.QuotePump_nullUnsafe.Paused == true) {
					string msg = "PUMP_ALREADY_PAUSED_BY_ANOTHER_LIVESIM";
					Assembler.PopupException(msg, null, false);
					continue;
				}
				channel.QuotePump_nullUnsafe.PusherPause();
				channelsPaused++;
			}
			return channelsPaused;
		}
		public int LivesimStreamingDefault_PumpResume_unfreezeOtherConsumers_forSameSymbolScale(ScriptExecutor executorImLivesimming, bool wrongUsagePopup = true) {
			string msig = " //LivesimStreamingDefault_PumpResume_unfreezeOtherConsumers_forSameSymbolScale(" + executorImLivesimming + ")";
			int channelsPaused = 0;
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) {
				string msg = "I_REFUSE_TO_RESUME_PUMP_BECAUSE_IT_LEADS_TO_DEADLOCK IM_CLOSING_MAINFORM_WHILE_LIVESIM_IS_RUNNING";
				Assembler.PopupException(msg + msig, null, false);
				return channelsPaused;
			}

			List<SymbolScaleDistributionChannel> channels = this.StreamingAdapter.DataDistributor_replacedForLivesim
				.GetDistributionChannels_allScaleIntervals_forSymbol(executorImLivesimming.Bars.Symbol);
			foreach (SymbolScaleDistributionChannel channel in channels) {
				if (channel.ImQueueNotPump_trueOnlyForBacktest == true ||
					channel.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim.HasSeparatePushingThread == false) {
					if (wrongUsagePopup == true) {
						string msg = "WILL_UNPAUSE DANGEROUS_I_MIGHT_HAVE_DROPPED_ALREADY_A_FEW_QUOTES__PUSHING_THREAD_HAVENT_STARTED (review how you use QuotePump)";
						Assembler.PopupException(msg + msig, null, false);
					}
					continue;
				}
				if (channel.QuotePump_nullUnsafe.Paused == false) {
					string msg = "PUMP_ALREADY_UNPAUSED_BY_ANOTHER_LIVESIM";
					Assembler.PopupException(msg, null, false);
					continue;
				}
				channel.QuotePump_nullUnsafe.PusherUnpause();
				channelsPaused++;
			}
			return channelsPaused;
		}

	}
}
