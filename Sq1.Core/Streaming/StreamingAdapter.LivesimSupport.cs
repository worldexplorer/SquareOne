using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Sq1.Core.DataFeed;
using Sq1.Core.Livesim;
using Sq1.Core.DataTypes;
using Sq1.Core.Charting;

namespace Sq1.Core.Streaming {
	public partial class StreamingAdapter {
		[JsonIgnore]	DataDistributor		distributorCharts_preLivesimForSymbolLivesimming = null;
		[JsonIgnore]	DataDistributor		distributorSolidifier_preLivesimForSymbolLivesimming = null;

		internal void SubstituteDistributorForSymbolsLivesimming_extractChartIntoSeparateDistributor(LivesimStreaming livesimStreaming) {
			this.distributorCharts_preLivesimForSymbolLivesimming = this.DataDistributor_replacedForLivesim;
			this.DataDistributor_replacedForLivesim = new DataDistributor(this);

			string symbol					= livesimStreaming.Livesimulator.BarsSimulating.Symbol;
			BarScaleInterval scaleInterval	= livesimStreaming.Livesimulator.BarsSimulating.ScaleInterval;
			string symbolIntervalScale		= livesimStreaming.Livesimulator.BarsSimulating.SymbolIntervalScale;
			IStreamingConsumer chartShadow	= livesimStreaming.Livesimulator.Executor.ChartShadow.ChartStreamingConsumer;

			bool willPushUsingPumpInSeparateThread = false;	// I wanna know which thread is going to be used; if DDE-client then cool; YES_IT_WAS_DDE_THREAD
			//bool willPushUsingPumpInSeparateThread = true;			// and now I wanna Livesim just like it will be working with Real Quik
			//if (this.distributorCharts_preLivesimForSymbolLivesimming.ConsumerQuoteIsSubscribed(symbol, scaleInterval, chartShadow) == false) {
			//    string msg = "EXECUTOR'S_CHART_SHADOW_WASNT_QUOTECONSUMING_WHAT_YOU_GONNA_LIVESIM NONSENSE " + symbolIntervalScale;
			//    Assembler.PopupException(msg, null, false);
			//}
			// the chart will be subscribed twice to the same Symbol+ScaleInterval, yes! but the original distributor is backed up and PushQuoteReceived will only push to the new DataDistributor(this) with one chart only
			this.DataDistributor_replacedForLivesim.ConsumerQuoteSubscribe(symbol, scaleInterval, chartShadow, willPushUsingPumpInSeparateThread);

			//if (this.distributorCharts_preLivesimForSymbolLivesimming.ConsumerBarIsSubscribed(symbol, scaleInterval, chartShadow) == false) {
			//    string msg = "EXECUTOR'S_CHART_SHADOW_WASNT_BARCONSUMING_WHAT_YOU_GONNA_LIVESIM NONSENSE " + symbolIntervalScale;
			//    Assembler.PopupException(msg, null, false);
			//}
			// the chart will be subscribed twice to the same Symbol+ScaleInterval, yes! but the original distributor is backed up and PushBarReceived will only push to the new DataDistributor(this) with one chart only
			this.DataDistributor_replacedForLivesim.ConsumerBarSubscribe(symbol, scaleInterval, chartShadow, willPushUsingPumpInSeparateThread);

			this.distributorSolidifier_preLivesimForSymbolLivesimming = this.DataDistributorSolidifiers_replacedForLivesim;
			this.DataDistributorSolidifiers_replacedForLivesim = new DataDistributor(this);		// EMPTY!!! exactly what I wanted

			string msg1 = "THESE_STREAMING_CONSUMERS_LOST_INCOMING_QUOTES_FOR_THE_DURATION_OF_LIVESIM: ";
			string msg2= this.distributorCharts_preLivesimForSymbolLivesimming.ToString();
			Assembler.PopupException(msg1 + msg2, null, false);
		}

		internal void SubstituteDistributorForSymbolsLivesimming_restoreOriginalDistributor() {
			this.DataDistributor_replacedForLivesim			= this.distributorCharts_preLivesimForSymbolLivesimming;
			this.DataDistributorSolidifiers_replacedForLivesim	= this.distributorSolidifier_preLivesimForSymbolLivesimming;

			string msg1 = "STREAMING_CONSUMERS_RESTORED_CONNECTIVITY_TO_STREAMING_ADAPTER_AFTER_LIVESIM: "
				+ this.DataDistributor_replacedForLivesim.ToString() + " SOLIDIFIERS:" + this.DataDistributorSolidifiers_replacedForLivesim	;
			Assembler.PopupException(msg1, null, false);
		}

		public string ReasonWhyLivesimCanNotBeStartedForSymbol(string symbol, ChartShadow chartShadow) {
			string ret = null;
			List<SymbolScaleDistributionChannel> channelsCloned = this.DataDistributor_replacedForLivesim
				.GetDistributionChannels_forSymbol_exceptForChartLivesimming(symbol, null, chartShadow.ChartStreamingConsumer);
			if (channelsCloned == null) {
				ret = "YOU_MUST_HAVE_CHART_SUBSCRIBED_TO_SYMBOL_BEFORE_STARTING_LIVESIM";
			} else {
				foreach(SymbolScaleDistributionChannel channelCloned in channelsCloned) {
					if (ret != null) ret += ", ";
					ret += channelCloned.ToString();
				}
				if (ret != null) ret = "STREAMING[" + this.ToString() + "] HAS_CONSUMERS[" + ret + "]";
			}
			return ret;
		}

	}
}
