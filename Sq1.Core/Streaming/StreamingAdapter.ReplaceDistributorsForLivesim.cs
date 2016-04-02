using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Sq1.Core.DataFeed;
using Sq1.Core.Livesim;
using Sq1.Core.DataTypes;
using Sq1.Core.Charting;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Streaming {
	public partial class StreamingAdapter {
		[JsonIgnore]	Distributor			dataDistributor_preLivesimForSymbolLivesimming;
		[JsonIgnore]	Distributor			dataDistributorSolidifiers_preLivesimForSymbolLivesimming;
		[JsonIgnore]	LivesimStreaming	livesimStreaming_forWhomDistributors_areReplaced;

		[JsonIgnore]	public bool			DistributorsAreReplacedByLivesim_ifYesDontPauseNeighborsOnBacktestContextInitRestore {
			get { return this.livesimStreaming_forWhomDistributors_areReplaced != null; } }

		internal void SubstituteDistributor_withOneSymbolLivesimming__extractChart_intoSeparateDistributor(LivesimStreaming livesimStreaming, bool chartBarsSubscribeSelected) {
			this.livesimStreaming_forWhomDistributors_areReplaced = livesimStreaming;

			string symbolLivesimming	= this.livesimStreaming_forWhomDistributors_areReplaced.Livesimulator.BarsSimulating.Symbol;
			ScriptExecutor executor		= this.livesimStreaming_forWhomDistributors_areReplaced.Livesimulator.Executor;
			string reasonForNewDistributor = Distributor.SUBSTITUTED_LIVESIM_STARTED
				+ " " + this.livesimStreaming_forWhomDistributors_areReplaced.Name
				+ "/" + executor.StrategyName
				// NO_IT_DOES_CONTAIN_GARBAGE!!! + "@" + executor.Bars.ToString()	// should not contain Static/Streaming bars since Count=0
				+ executor.Bars.SymbolIntervalScale
				;

			if (this.Distributor_substitutedDuringLivesim.ChannelsBySymbol.Count == 0 && chartBarsSubscribeSelected) {
				string nonDefaultLivesimWasntSubscribed = this.Distributor_substitutedDuringLivesim.ToString();
				if (nonDefaultLivesimWasntSubscribed.Contains("LiveStreamingDefault")) {
					string msg = "ORIGINAL_DISTRIBUTOR_MUST_HAVE_HAD_THE_CHART_YOU_WANT_TO_PAUSE AND_SOLIDIFIER";
					Assembler.PopupException(msg, null, false);
				}
			}

			this.dataDistributor_preLivesimForSymbolLivesimming = this.Distributor_substitutedDuringLivesim;
			// MOVED_UPSTACK_PAUSES_EVERYTHING_FOR_SYMBOL this.dataDistributor_preLivesimForSymbolLivesimming.AllQuotePumps_Pause(reasonForNewDistributor);
			this.Distributor_substitutedDuringLivesim = new Distributor(this, reasonForNewDistributor);

			if (chartBarsSubscribeSelected) {
				//BarScaleInterval scaleInterval	= livesimStreaming.Livesimulator.BarsSimulating.ScaleInterval;
				//string symbolIntervalScale		= livesimStreaming.Livesimulator.BarsSimulating.SymbolIntervalScale;
				//StreamingConsumer chartLess		= livesimStreaming.Livesimulator.LivesimQuoteBarConsumer;
				StreamingConsumer chartShadowStreamingConsumer	= this.livesimStreaming_forWhomDistributors_areReplaced.Livesimulator.Executor.ChartShadow.ChartStreamingConsumer;

				bool willPushUsingPumpInSeparateThread = true;	// I wanna know which thread is going to be used; if DDE-client then cool; YES_IT_WAS_DDE_THREAD
				//bool willPushUsingPumpInSeparateThread = true;			// and now I wanna Livesim just like it will be working with Real Quik
				//if (this.distributorCharts_preLivesimForSymbolLivesimming.ConsumerQuoteIsSubscribed(symbol, scaleInterval, chartShadow) == false) {
				//	string msg = "EXECUTOR'S_CHART_SHADOW_WASNT_QUOTECONSUMING_WHAT_YOU_GONNA_LIVESIM NONSENSE " + symbolIntervalScale;
				//	Assembler.PopupException(msg, null, false);
				//}
				// the chart will be subscribed twice to the same Symbol+ScaleInterval, yes! but the original distributor is backed up and PushQuoteReceived will only push to the new Distributor(this) with one chart only

				//v1 PARASITE_NEUTRALIZED this.Distributor_replacedForLivesim.ConsumerQuoteSubscribe(symbol, scaleInterval, chartLess, willPushUsingPumpInSeparateThread);
				//v2 CHART_CONTROL_IS_SUBSCRIBED_TO_BOTH_NOW__TO_DISTRIBUTOR_ORIGINAL_WHICH_IS_PAUSED__AND_DISTRIBUTOR_LIVESIM_OWN_IMPLEMENTATION
				this.Distributor_substitutedDuringLivesim.ConsumerQuoteSubscribe(chartShadowStreamingConsumer, willPushUsingPumpInSeparateThread);

				//if (this.distributorCharts_preLivesimForSymbolLivesimming.ConsumerBarIsSubscribed(symbol, scaleInterval, chartShadow) == false) {
				//	string msg = "EXECUTOR'S_CHART_SHADOW_WASNT_BARCONSUMING_WHAT_YOU_GONNA_LIVESIM NONSENSE " + symbolIntervalScale;
				//	Assembler.PopupException(msg, null, false);
				//}
				// the chart will be subscribed twice to the same Symbol+ScaleInterval, yes! but the original distributor is backed up and PushBarReceived will only push to the new Distributor(this) with one chart only

				//v1 PARASITE_NEUTRALIZED this.Distributor_replacedForLivesim.ConsumerBarSubscribe(symbol, scaleInterval, chartLess, willPushUsingPumpInSeparateThread);
				//v2 CHART_CONTROL_IS_SUBSCRIBED_TO_BOTH_NOW__TO_DISTRIBUTOR_ORIGINAL_WHICH_IS_PAUSED__AND_DISTRIBUTOR_LIVESIM_OWN_IMPLEMENTATION
				this.Distributor_substitutedDuringLivesim.ConsumerBarSubscribe(chartShadowStreamingConsumer, willPushUsingPumpInSeparateThread);

				this.Distributor_substitutedDuringLivesim.SetQuotePumpThreadName_sinceNoMoreSubscribersWillFollowFor(symbolLivesimming);
			}

			if (this.DistributorSolidifiers_substitutedDuringLivesim.ChannelsBySymbol.Count > 0) {
				//v1 this.DistributorSolidifiers_replacedForLivesim.AllQuotePumps_Pause(reasonForNewDistributor);
				this.DistributorSolidifiers_substitutedDuringLivesim.PumpPause_forSymbolLivesimming(symbolLivesimming, reasonForNewDistributor);
			} else {
				if (this is LivesimStreamingDefault) {
					string msg4 = "DEFAULT_LIVESIM_HAS_NO_SOLIDIFIERS";
				} else {
					string msg4 = "IM_A_REAL_STREAMING__I_MUST_HAVE_SOLIDIFIERS_TO_PAUSE";
					Assembler.PopupException(msg4, null, false);
				}
			}
			string msg3 = "REGARDLESS_WHETHER_I_HAD_SOLIDIFIERS__I_CREATE_NEW_TO_INDICATE_IM_LIVESIMMING";
			this.dataDistributorSolidifiers_preLivesimForSymbolLivesimming = this.DistributorSolidifiers_substitutedDuringLivesim;
			this.DistributorSolidifiers_substitutedDuringLivesim = new Distributor(this, reasonForNewDistributor);		// EMPTY!!! exactly what I wanted

			string msg1 = "THESE_STREAMING_CONSUMERS_PAUSED_RECEIVING_INCOMING_QUOTES_FOR_THE_DURATION_OF_LIVESIM: ";
			string msg2= this.dataDistributor_preLivesimForSymbolLivesimming.ToString();
			Assembler.PopupException(msg1 + msg2, null, false);
		}

		internal void SubstituteDistributor_withOneSymbolLivesimming_restoreOriginalDistributor() {
			if (this.livesimStreaming_forWhomDistributors_areReplaced == null) {
				string msg = "YOU_DIDNT_SUBSTITUTE_DISTRIBUTOR YOU_DIDNT_INVOKE"
					+ " SubstituteDistributorForSymbolsLivesimming_extractChartIntoSeparateDistributor()<=SimulationPreBarsSubstitute_overrideable()";
				Assembler.PopupException(msg, null, false);
				return;
			}
			ScriptExecutor executor = this.livesimStreaming_forWhomDistributors_areReplaced.Livesimulator.Executor;
			string symbolLivesimming = executor.Bars.Symbol;
			string reasonForStoppingReplacedDistributor = this.livesimStreaming_forWhomDistributors_areReplaced.Name
				+ "==RESTORING_AFTER_LIVESIM" + executor.StrategyName + "@" + executor.Bars.ToString();	// should not contain Static/Streaming bars since Count=0

			//v1 this.Distributor_substitutedDuringLivesim.AllQuotePumps_Stop(reasonForStoppingReplacedDistributor);
			this.Distributor_substitutedDuringLivesim.PumpStop_forSymbolLivesimming(symbolLivesimming, reasonForStoppingReplacedDistributor);

			this.Distributor_substitutedDuringLivesim				= this.dataDistributor_preLivesimForSymbolLivesimming;
			this.DistributorSolidifiers_substitutedDuringLivesim	= this.dataDistributorSolidifiers_preLivesimForSymbolLivesimming;

			//MOVED_UPSTACK_PAUSES_EVERYTHING_FOR_SYMBOL this.Distributor_replacedForLivesim				.AllQuotePumps_Unpause(reasonForStoppingReplacedDistributor);

			if (this.DistributorSolidifiers_substitutedDuringLivesim.ChannelsBySymbol.Count > 1) {
				//v1 this.DistributorSolidifiers_substitutedDuringLivesim.AllQuotePumps_Unpause(reasonForStoppingReplacedDistributor);
				this.DistributorSolidifiers_substitutedDuringLivesim.PumpUnpause_forSymbolLivesimming(symbolLivesimming, reasonForStoppingReplacedDistributor);
			} else {
				if (this is LivesimStreamingDefault) {
					string msg4 = "DEFAULT_LIVESIM_HAS_NO_SOLIDIFIERS";
				} else {
					string msg4 = "IM_A_REAL_STREAMING__I_MUST_HAVE_SOLIDIFIERS_TO_UNPAUSE";
					Assembler.PopupException(msg4, null, false);
				}
			}

			string msg1 = "STREAMING_CONSUMERS_RESTORED_CONNECTIVITY_TO_STREAMING_ADAPTER_AFTER_LIVESIM: "
				+ this.Distributor_substitutedDuringLivesim.ToString() + " SOLIDIFIERS:" + this.DistributorSolidifiers_substitutedDuringLivesim	;
			Assembler.PopupException(msg1, null, false);

			this.livesimStreaming_forWhomDistributors_areReplaced = null;
		}

		public string ReasonWhy_livesimCanNotBeStarted_forSymbol(string symbol, ChartShadow chartShadow) {
			string ret = null;
			List<SymbolScaleStream> channelsCloned = this.Distributor_substitutedDuringLivesim
				.GetStreams_forSymbol_exceptForChartLivesimming(symbol, null, chartShadow.ChartStreamingConsumer);
			if (channelsCloned == null) {
				ret = "YOU_MUST_HAVE_CHART_SUBSCRIBED_TO_SYMBOL_BEFORE_STARTING_LIVESIM";
			} else {
				foreach(SymbolScaleStream channelCloned in channelsCloned) {
					if (ret != null) ret += ", ";
					ret += channelCloned.ToString();
				}
				if (ret != null) ret = "STREAMING[" + this.ToString() + "] HAS_CONSUMERS[" + ret + "]";
			}
			return ret;
		}

	}
}
