using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Sq1.Core.Livesim;
using Sq1.Core.Charting;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Streaming {
	public partial class StreamingAdapter {
		[JsonIgnore]	DistributorCharts		dataDistributorCharts_preLivesimForSymbolLivesimming;
		[JsonIgnore]	DistributorSolidifier	dataDistributorSolidifier_preLivesimForSymbolLivesimming;
		[JsonIgnore]	LivesimStreaming		livesimStreaming_forWhomDistributors_areReplaced;

		[JsonIgnore]	public bool			DistributorsAreReplacedByLivesim_ifYesDontPauseNeighborsOnBacktestContextInitRestore {
			get { return this.livesimStreaming_forWhomDistributors_areReplaced != null; } }

		internal void SubstituteDistributor_withOneSymbolLivesimming__extractChart_intoSeparateDistributor(LivesimStreaming livesimStreaming, bool chartBarsSubscribeSelected) {
			this.livesimStreaming_forWhomDistributors_areReplaced = livesimStreaming;

			string symbolLivesimming	= this.livesimStreaming_forWhomDistributors_areReplaced.Livesimulator.BarsSimulating.Symbol;
			ScriptExecutor executor		= this.livesimStreaming_forWhomDistributors_areReplaced.Livesimulator.Executor;
			string reasonForNewDistributor = DistributorCharts.SUBSTITUTED_LIVESIM_STARTED
				+ " " + this.livesimStreaming_forWhomDistributors_areReplaced.Name
				+ "/" + executor.StrategyName
				// NO_IT_DOES_CONTAIN_GARBAGE!!! + "@" + executor.Bars.ToString()	// should not contain Static/Streaming bars since Count=0
				+ executor.Bars.SymbolIntervalScale
				;

			if (this.DistributorCharts_substitutedDuringLivesim.ChannelsBySymbol.Count == 0 && chartBarsSubscribeSelected) {
				string nonDefaultLivesimWasntSubscribed = this.DistributorCharts_substitutedDuringLivesim.ToString();
				if (nonDefaultLivesimWasntSubscribed.Contains("LiveStreamingDefault")) {
					string msg = "ORIGINAL_DISTRIBUTOR_MUST_HAVE_HAD_THE_CHART_YOU_WANT_TO_PAUSE AND_SOLIDIFIER";
					Assembler.PopupException(msg, null, false);
				}
			}

			this.dataDistributorCharts_preLivesimForSymbolLivesimming = this.DistributorCharts_substitutedDuringLivesim;
			this.DistributorCharts_substitutedDuringLivesim = new DistributorCharts(this, reasonForNewDistributor);

			if (chartBarsSubscribeSelected) {
				StreamingConsumerChart streamingConsumer_chartShadow	= this.livesimStreaming_forWhomDistributors_areReplaced.Livesimulator.Executor.ChartShadow.ChartStreamingConsumer;

				bool willPushUsingPumpInSeparateThread = true;	// I wanna know which thread is going to be used; if DDE-client then cool; YES_IT_WAS_DDE_THREAD
				this.DistributorCharts_substitutedDuringLivesim.ConsumerQuoteSubscribe(streamingConsumer_chartShadow, willPushUsingPumpInSeparateThread);
				this.DistributorCharts_substitutedDuringLivesim.ConsumerBarSubscribe(streamingConsumer_chartShadow, willPushUsingPumpInSeparateThread);
				this.DistributorCharts_substitutedDuringLivesim.SetQuotePumpThreadName_sinceNoMoreSubscribersWillFollowFor(symbolLivesimming);
			}

			if (this.DistributorSolidifiers_substitutedDuringLivesim.ChannelsBySymbol.Count > 0) {
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
			this.dataDistributorSolidifier_preLivesimForSymbolLivesimming = this.DistributorSolidifiers_substitutedDuringLivesim;
			this.DistributorSolidifiers_substitutedDuringLivesim = new DistributorSolidifier(this, reasonForNewDistributor);		// EMPTY!!! exactly what I wanted

			string msg1 = "THESE_STREAMING_CONSUMERS_PAUSED_RECEIVING_INCOMING_QUOTES_FOR_THE_DURATION_OF_LIVESIM: ";
			string msg2= this.dataDistributorCharts_preLivesimForSymbolLivesimming.ToString();
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

			this.DistributorCharts_substitutedDuringLivesim.PumpStop_forSymbolLivesimming(symbolLivesimming, reasonForStoppingReplacedDistributor);

			this.DistributorCharts_substitutedDuringLivesim				= this.dataDistributorCharts_preLivesimForSymbolLivesimming;
			this.DistributorSolidifiers_substitutedDuringLivesim	= this.dataDistributorSolidifier_preLivesimForSymbolLivesimming;

			if (this.DistributorSolidifiers_substitutedDuringLivesim.ChannelsBySymbol.Count > 0) {
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
				+ this.DistributorCharts_substitutedDuringLivesim.ToString() + " SOLIDIFIERS:" + this.DistributorSolidifiers_substitutedDuringLivesim	;
			Assembler.PopupException(msg1, null, false);

			this.livesimStreaming_forWhomDistributors_areReplaced = null;
		}

		public string ReasonWhy_livesimCanNotBeStarted_forSymbol(string symbol, ChartShadow chartShadow) {
			string ret = null;
			List<SymbolScaleStream<StreamingConsumerChart>> channelsCloned = this.DistributorCharts_substitutedDuringLivesim
				.GetStreams_forSymbol_exceptForChartLivesimming(symbol, null, chartShadow.ChartStreamingConsumer);
			if (channelsCloned == null) {
				ret = "YOU_MUST_HAVE_CHART_SUBSCRIBED_TO_SYMBOL_BEFORE_STARTING_LIVESIM";
			} else {
				foreach(SymbolScaleStream<StreamingConsumerChart> channelCloned in channelsCloned) {
					if (ret != null) ret += ", ";
					ret += channelCloned.ToString();
				}
				if (ret != null) ret = "STREAMING[" + this.ToString() + "] HAS_CONSUMERS[" + ret + "]";
			}
			return ret;
		}

	}
}
