using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Sq1.Core.DataTypes;
using Sq1.Core.Charting;

namespace Sq1.Core.Streaming {
	public abstract partial class StreamingAdapter {
		[JsonIgnore]	public		DistributorCharts				DistributorCharts_substitutedDuringLivesim			{ get; protected set; }
		[JsonIgnore]	public		DistributorSolidifier			DistributorSolidifiers_substitutedDuringLivesim		{ get; protected set; }
		[JsonIgnore]	public		StreamingConsumerSolidifier		StreamingSolidifier_oneForAllSymbols				{ get; protected set; }
		[JsonIgnore]	public		virtual List<string>			SymbolsUpstreamSubscribed			{ get; private set; }
		[JsonIgnore]	protected	object							SymbolsSubscribedLock;
		[JsonIgnore]	public		virtual string					SymbolsUpstreamSubscribedAsString 	{ get {
				string ret = "";
				lock (this.SymbolsSubscribedLock) {
					foreach (string symbol in this.SymbolsUpstreamSubscribed) ret += symbol + ",";
				}
				ret = ret.TrimEnd(',');
				return ret;
			} }

		[JsonIgnore]				ConnectionState					upstreamConnectionState;
		[JsonIgnore]	public		ConnectionState					UpstreamConnectionState	{
			get { return this.upstreamConnectionState; }
			protected set {
				if (this.upstreamConnectionState == value) return;	//don't invoke StateChanged if it didn't change
				if (this.upstreamConnectionState == ConnectionState.Streaming_UpstreamConnected_downstreamSubscribedAll
								&& value == ConnectionState.Streaming_JustInitialized_solidifiersUnsubscribed) {
					Assembler.PopupException("YOU_ARE_RESETTING_ORIGINAL_STREAMING_STATE__FROM_OWN_LIVESIM_STREAMING", null, false);
				}
				if (this.upstreamConnectionState == ConnectionState.Streaming_UpstreamConnected_downstreamUnsubscribed
								&& value == ConnectionState.Streaming_JustInitialized_solidifiersSubscribed) {
					Assembler.PopupException("WHAT_DID_YOU_INITIALIZE? IT_WAS_ALREADY_INITIALIZED_AND_UPSTREAM_CONNECTED", null, false);
				}
				this.upstreamConnectionState = value;
				this.RaiseOnStreamingConnectionStateChanged();	// consumed by QuikStreamingMonitorForm,QuikStreamingEditor

				try {
					if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;
					if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) return;
					if (this.UpstreamConnect_onAppRestart == this.UpstreamConnected) return;
					this.UpstreamConnect_onAppRestart = this.UpstreamConnected;		// you can override this.UpstreamConnectedOnAppRestart and keep it FALSE to avoid DS serialization
					if (this.DataSource == null) {
						string msg = "SHOULD_NEVER_HAPPEN DataSource=null for streaming[" + this + "]";
						Assembler.PopupException(msg);
						return;
					}
					Assembler.InstanceInitialized.RepositoryJsonDataSources.SerializeSingle(this.DataSource);
				} catch (Exception ex) {
					string msg = "SOMETHING_WENT_WRONG_WHILE_SAVING_DATASOURCE_AFTER_YOU_CHANGED UpstreamConnected for streaming[" + this + "]";
					Assembler.PopupException(msg);
				}
			}
		}
		[JsonProperty]	public	virtual	bool						UpstreamConnect_onAppRestart		{ get; protected set; }
		[JsonIgnore]	public		bool							UpstreamConnected					{ get {
			bool ret = false;
			switch (this.UpstreamConnectionState) {
				case ConnectionState.UnknownConnectionState:									ret = false;	break;
				case ConnectionState.Streaming_JustInitialized_solidifiersUnsubscribed:			ret = false;	break;
				case ConnectionState.Streaming_JustInitialized_solidifiersSubscribed:			ret = false;	break;
				case ConnectionState.Streaming_DisconnectedJustConstructed:						ret = false;	break;

				// used in QuikStreamingAdapter
				case ConnectionState.Streaming_UpstreamConnected_downstreamUnsubscribed:		ret = true;		break;
				case ConnectionState.Streaming_UpstreamConnected_downstreamSubscribed:			ret = true;		break;
				case ConnectionState.Streaming_UpstreamConnected_downstreamSubscribedAll:		ret = true;		break;
				case ConnectionState.Streaming_UpstreamConnected_downstreamUnsubscribedAll:		ret = true;		break;
				case ConnectionState.Streaming_UpstreamDisconnected_downstreamSubscribed:		ret = false;	break;
				case ConnectionState.Streaming_UpstreamDisconnected_downstreamUnsubscribed:		ret = false;	break;

				// used in QuikBrokerAdapter
				//case ConnectionState.SymbolSubscribed:					ret = true;		break;
				//case ConnectionState.SymbolUnsubscribed:				ret = true;		break;
				//case ConnectionState.ErrorConnectingNoRetriesAnymore:	ret = false;	break;

				// used in QuikLivesimStreaming
				case ConnectionState.FailedToConnect:						ret = false;	break;
				case ConnectionState.FailedToDisconnect:					ret = false;	break;		// can still be connected but by saying NotConnected I prevent other attempt to subscribe symbols; use "Connect" button to resolve

				default:
					Assembler.PopupException("ADD_HANDLER_FOR_NEW_ENUM_VALUE this.ConnectionState[" + this.UpstreamConnectionState + "]");
					ret = false;
					break;
			}
			return ret;
		} }
		
		public void CreateDistributors_onlyWhenNecessary(string reasonToExist) {
			if (this.DistributorCharts_substitutedDuringLivesim == null) {
				this.DistributorCharts_substitutedDuringLivesim		= new DistributorCharts(this, reasonToExist);
			} else {
				//this.Distributor_replacedForLivesim.ForceUnsubscribeLeftovers_mustBeEmptyAlready(reasonToExist);
			}
			this.DistributorSolidifiers_substitutedDuringLivesim	= new DistributorSolidifier(this, reasonToExist);
		}

		protected virtual void SolidifierSubscribe_toAllSymbols_ofDataSource_onAppRestart() {
			string msg = "SUBSCRIBING_SOLIDIFIER_APPRESTART " + this.DataSource.Name;
			//Assembler.PopupException(msg, null, false);

			this.CreateDistributors_onlyWhenNecessary(this.ReasonToExist);
			this.StreamingSolidifier_oneForAllSymbols.Initialize(this.DataSource);
			foreach (string symbol in this.DataSource.Symbols) {
				this.solidifierSubscribe_oneSymbol(symbol);
			}
			this.UpstreamConnectionState = ConnectionState.Streaming_JustInitialized_solidifiersSubscribed;
		}

		//public void SolidifierSubscribeOneSymbol_iFinishedLivesimming(string symbol = null) {
		void solidifierSubscribe_oneSymbol(string symbol) {
			if (this.DistributorSolidifiers_substitutedDuringLivesim == null) {
				string msg = "DONT_SUBSCRIBE_SOLIDIFIERS_FOR_DUMMY_ADAPTERS this[" + this + "]";
				Assembler.PopupException(msg, null, true);
				return;
			}

			if (symbol == null) {
				string msg = "WHEN_AM_I_ACTIVATED??? IT_LOOKS_VERY_SUSPICIOUS_HERE NPE_RISKY";
				Assembler.PopupException(msg);
				symbol = this.LivesimStreaming_ownImplementation.DataSource.Symbols[0];
			}

			this.DistributorSolidifiers_substitutedDuringLivesim.ConsumerBarSubscribe_solidifiers(symbol,
				this.DataSource.ScaleInterval, this.StreamingSolidifier_oneForAllSymbols, this.QuotePumpSeparatePushingThreadEnabled);
			this.DistributorSolidifiers_substitutedDuringLivesim.ConsumerQuoteSubscribe_solidifiers(symbol,
				this.DataSource.ScaleInterval, this.StreamingSolidifier_oneForAllSymbols, this.QuotePumpSeparatePushingThreadEnabled);

			this.DistributorSolidifiers_substitutedDuringLivesim.SetQuotePumpThreadName_sinceNoMoreSubscribersWillFollowFor(symbol);
		}

		void solidifierUnsubscribe_oneSymbol_useMe_whenRenamingSymbols_inDataSource(string symbol) {
			if (symbol == null) {
				symbol  = this.LivesimStreaming_ownImplementation.DataSource.Symbols[0];
			}

			if (this.DistributorSolidifiers_substitutedDuringLivesim.ConsumerBarIsSubscribed_solidifiers(symbol, this.DataSource.ScaleInterval, this.StreamingSolidifier_oneForAllSymbols)) {
				this.DistributorSolidifiers_substitutedDuringLivesim.ConsumerBarUnsubscribe_solidifiers(symbol, this.DataSource.ScaleInterval, this.StreamingSolidifier_oneForAllSymbols);
			} else {
				string msg = "IGNORE_ME_IF_YOU_ARE_STARTED_LIVESIM SOLIDIFIER_NOT_SUBSCRIBED_BARS symbol[" + symbol + "] ScaleInterval[" + this.DataSource.ScaleInterval + "]";
				Assembler.PopupException(msg, null, false);
			}
			if (this.DistributorSolidifiers_substitutedDuringLivesim.ConsumerQuoteIsSubscribed_solidifiers(symbol, this.DataSource.ScaleInterval, this.StreamingSolidifier_oneForAllSymbols)) {
				this.DistributorSolidifiers_substitutedDuringLivesim.ConsumerQuoteUnsubscribe_solidifiers(symbol, this.DataSource.ScaleInterval, this.StreamingSolidifier_oneForAllSymbols);
			} else {
				string msg = "IGNORE_ME_IF_YOU_ARE_STARTED_LIVESIM SOLIDIFIER_NOT_SUBSCRIBED_QUOTES symbol[" + symbol + "] ScaleInterval[" + this.DataSource.ScaleInterval + "]";
				Assembler.PopupException(msg, null, false);
			}
			SymbolChannel<StreamingConsumerSolidifier> channel = this.DistributorSolidifiers_substitutedDuringLivesim.GetChannelFor_nullMeansWasntSubscribed(symbol);
			if (channel == null) {
				string msg = "I_START_LIVESIM_WITHOUT_ANY_PRIOR_SUBSCRIBED_SOLIDIFIERS symbol[" + symbol + "]";
				Assembler.PopupException(msg, null, false);
				return;
			}
			channel.QueueWhenBacktesting_PumpForLiveAndLivesim.PusherPause_waitUntilPaused();
		}

		public void UpstreamSubscribeRegistryHelper(string symbol) {
			if (String.IsNullOrEmpty(symbol)) {
				string msg = "symbol[" + symbol + "]=IsNullOrEmpty, can't UpstreamSubscribeRegistryHelper()";
				throw new Exception(msg);
			}
			lock (this.SymbolsSubscribedLock) {
				if (this.SymbolsUpstreamSubscribed.Contains(symbol)) {
					string msg = "symbol[" + symbol + "] already registered as UpstreamSubscribed";
					throw new Exception(msg);
				}
				this.SymbolsUpstreamSubscribed.Add(symbol);
			}
		}
		public void UpstreamUnSubscribeRegistryHelper(string symbol) {
			if (String.IsNullOrEmpty(symbol)) {
				string msg = "symbol[" + symbol + "]=IsNullOrEmpty, can't UpstreamUnSubscribeRegistryHelper()";
				throw new Exception(msg);
			}
			lock (this.SymbolsSubscribedLock) {
				if (this.SymbolsUpstreamSubscribed.Contains(symbol) == false) {
					string msg = "symbol[" + symbol + "] is not registered as UpstreamSubscribed, can't unsubscribe";
					throw new Exception(msg);
				}
				this.SymbolsUpstreamSubscribed.Remove(symbol);
			}
		}
		public virtual bool UpstreamIsSubscribedRegistryHelper(string symbol) {
			if (String.IsNullOrEmpty(symbol)) {
				string msg = "symbol[" + symbol + "]=IsNullOrEmpty, can't UpstreamIsSubscribedRegistryHelper()";
				throw new Exception(msg);
			}
			lock (this.SymbolsSubscribedLock) {
				return this.SymbolsUpstreamSubscribed.Contains(symbol);
			}
		}

		public void UpstreamSubscribedToSymbolPokeConsumersHelper(string symbol) {
			List<SymbolScaleStream<StreamingConsumerChart>> channels = this.DistributorCharts_substitutedDuringLivesim.GetStreams_allScaleIntervals_forSymbol(symbol);
			foreach (var channel in channels) {
				channel.UpstreamSubscribedToSymbol_pokeConsumers(symbol);
			}
		}
		public void UpstreamUnSubscribedFromSymbolPokeConsumersHelper(string symbol) {
			List<SymbolScaleStream<StreamingConsumerChart>> channels = this.DistributorCharts_substitutedDuringLivesim.GetStreams_allScaleIntervals_forSymbol(symbol);
			Quote quoteLastReceived = this.StreamingDataSnapshot.GetQuoteLast_forSymbol_nullUnsafe(symbol);
			foreach (var channel in channels) {
				channel.UpstreamUnSubscribedFromSymbol_pokeConsumers(symbol, quoteLastReceived);
			}
		}

		internal void ChartStreamingConsumer_Subscribe(StreamingConsumerChart chartStreamingConsumer, string msigForNpExceptions) {
			bool iWantChartToConsumeQuotesInSeparateThreadToLetStreamingGoWithoutWaitingForStrategyToFinish = true;

			//NPE_AFTER_SEPARATED_SOLIDIFIERS SymbolScaleDistributionChannel channel = streamingSafe.Distributor.GetDistributionChannelFor_nullUnsafe(symbolSafe, scaleIntervalSafe);
			if (this.DistributorCharts_substitutedDuringLivesim.ConsumerQuoteIsSubscribed(chartStreamingConsumer) == true) {
				Assembler.PopupException("CHART_STREAMING_ALREADY_SUBSCRIBED_CONSUMER_QUOTE" + msigForNpExceptions);
			} else {
				//Assembler.PopupException("Subscribing QuoteConsumer [" + this + "]  to " + plug + "  (wasn't registered)");
				this.DistributorCharts_substitutedDuringLivesim.ConsumerQuoteSubscribe(chartStreamingConsumer,
						iWantChartToConsumeQuotesInSeparateThreadToLetStreamingGoWithoutWaitingForStrategyToFinish);
			}

			if (this.DistributorCharts_substitutedDuringLivesim.ConsumerBarIsSubscribed(chartStreamingConsumer) == true) {
				Assembler.PopupException("CHART_STREAMING_ALREADY_SUBSCRIBED_CONSUMER_BAR" + msigForNpExceptions);
			} else {
				//Assembler.PopupException("Subscribing BarsConsumer [" + this + "] to " + this.ToString() + " (wasn't registered)");
				this.DistributorCharts_substitutedDuringLivesim.ConsumerBarSubscribe(chartStreamingConsumer, true);
			}

			if (this.DistributorCharts_substitutedDuringLivesim.ConsumerLevelTwoFrozenIsSubscribed(chartStreamingConsumer) == true) {
				Assembler.PopupException("CHART_STREAMING_ALREADY_SUBSCRIBED_CONSUMER_LEVEL_TWO_FROZEN" + msigForNpExceptions);
			} else {
				//Assembler.PopupException("Subscribing LevelTwoFrozensConsumer [" + this + "] to " + this.ToString() + " (wasn't registered)");
				this.DistributorCharts_substitutedDuringLivesim.ConsumerLevelTwoFrozenSubscribe(chartStreamingConsumer, true);
			}
		}
		internal void ChartStreamingConsumer_Unsubscribe(StreamingConsumerChart chartStreamingConsumer, string msigForNpExceptions) {
			if (this.DistributorCharts_substitutedDuringLivesim.ConsumerQuoteIsSubscribed(chartStreamingConsumer) == false) {
				string msg = "CONSUMER_QUOTE__CHART_STREAMING_WASNT_SUBSCRIBED";
				Assembler.PopupException(msg + msigForNpExceptions, null, false);
			} else {
				//Assembler.PopupException("UnSubscribing QuoteConsumer [" + this + "]  to " + plug + "  (was subscribed)");
				this.DistributorCharts_substitutedDuringLivesim.ConsumerQuoteUnsubscribe(chartStreamingConsumer);
			}

			if (this.DistributorCharts_substitutedDuringLivesim.ConsumerBarIsSubscribed(chartStreamingConsumer) == false) {
				string msg = "CONSUMER_BAR__CHART_STREAMING_WASNT_SUBSCRIBED";
				Assembler.PopupException(msg + msigForNpExceptions, null, false);
			} else {
				//Assembler.PopupException("UnSubscribing BarsConsumer [" + this + "] to " + this.ToString() + " (was subscribed)");
				this.DistributorCharts_substitutedDuringLivesim.ConsumerBarUnsubscribe(chartStreamingConsumer);
			}

			if (this.DistributorCharts_substitutedDuringLivesim.ConsumerLevelTwoFrozenIsSubscribed(chartStreamingConsumer) == false) {
				string msg = "CONSUMER_LEVEL_TWO_FROZEN__CHART_STREAMING_WASNT_SUBSCRIBED";
				Assembler.PopupException(msg + msigForNpExceptions, null, false);
			} else {
				//Assembler.PopupException("UnSubscribing LevelTwoFrozensConsumer [" + this + "] to " + this.ToString() + " (was subscribed)");
				this.DistributorCharts_substitutedDuringLivesim.ConsumerLevelTwoFrozenUnsubscribe(chartStreamingConsumer);
			}
		}

	}
} 