using System;
using System.Collections.Generic;
using System.Drawing;

using Newtonsoft.Json;

using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Livesim;

namespace Sq1.Core.Streaming {
	public abstract partial class StreamingAdapter {
		[JsonIgnore]	public		string					Name								{ get; protected set; }
		[JsonIgnore]	public		string					ReasonToExist						{ get; protected set; }
		[JsonIgnore]	public		Bitmap					Icon								{ get; protected set; }
		[JsonIgnore]	public		StreamingSolidifier		StreamingSolidifier					{ get; protected set; }
		[JsonIgnore]	public		DataSource				DataSource;
		[JsonIgnore]	public		string					marketName							{ get { return this.DataSource.MarketInfo.Name; } }
		[JsonIgnore]	public		DataDistributor			DataDistributor_replacedForLivesim						{ get; protected set; }
		[JsonIgnore]	public		DataDistributor			DataDistributorSolidifiers_replacedForLivesim			{ get; protected set; }
		[JsonIgnore]	public		StreamingDataSnapshot	StreamingDataSnapshot				{ get; protected set; }
		[JsonIgnore]	public		virtual List<string>	SymbolsUpstreamSubscribed			{ get; private set; }
		[JsonIgnore]	protected	object					SymbolsSubscribedLock;
		[JsonIgnore]	public		virtual string			SymbolsUpstreamSubscribedAsString 	{ get {
				string ret = "";
				lock (this.SymbolsSubscribedLock) {
					foreach (string symbol in this.SymbolsUpstreamSubscribed) ret += symbol + ",";
				}
				ret = ret.TrimEnd(',');
				return ret;
			} }

		[JsonIgnore]				ConnectionState			upstreamConnectionState;
		[JsonIgnore]	public		ConnectionState			UpstreamConnectionState	{
			get { return this.upstreamConnectionState; }
			protected set {
				if (this.upstreamConnectionState == value) return;	//don't invoke StateChanged if it didn't change
				if (this.upstreamConnectionState == ConnectionState.UpstreamConnected_downstreamSubscribedAll
								&& value == ConnectionState.JustInitialized_solidifiersUnsubscribed) {
					Assembler.PopupException("YOU_ARE_RESETTING_ORIGINAL_STREAMING_STATE__FROM_OWN_LIVESIM_STREAMING", null, false);
				}
				if (this.upstreamConnectionState == ConnectionState.UpstreamConnected_downstreamUnsubscribed
								&& value == ConnectionState.JustInitialized_solidifiersSubscribed) {
					Assembler.PopupException("WHAT_DID_YOU_INITIALIZE? IT_WAS_ALREADY_INITIALIZED_AND_UPSTREAM_CONNECTED", null, false);
				}
				this.upstreamConnectionState = value;
				this.RaiseOnConnectionStateChanged();	// consumed by QuikStreamingMonitorForm,QuikStreamingEditor

				try {
					if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;
					if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
					if (this.UpstreamConnectedOnAppRestart == this.UpstreamConnected) return;
					this.UpstreamConnectedOnAppRestart = this.UpstreamConnected;		// you can override this.UpstreamConnectedOnAppRestart and keep it FALSE to avoid DS serialization
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
		[JsonProperty]	public	virtual	bool				UpstreamConnectedOnAppRestart		{ get; protected set; }
		[JsonIgnore]	public		bool					UpstreamConnected					{ get {
			bool ret = false;
			switch (this.UpstreamConnectionState) {
				case ConnectionState.UnknownConnectionState:							ret = false;	break;
				case ConnectionState.JustInitialized_solidifiersUnsubscribed:			ret = false;	break;
				case ConnectionState.JustInitialized_solidifiersSubscribed:				ret = false;	break;
				case ConnectionState.DisconnectedJustConstructed:						ret = false;	break;

				// used in QuikStreamingAdapter
				case ConnectionState.UpstreamConnected_downstreamUnsubscribed:			ret = true;		break;
				case ConnectionState.UpstreamConnected_downstreamSubscribed:			ret = true;		break;
				case ConnectionState.UpstreamConnected_downstreamSubscribedAll:			ret = true;		break;
				case ConnectionState.UpstreamConnected_downstreamUnsubscribedAll:		ret = true;		break;
				case ConnectionState.UpstreamDisconnected_downstreamSubscribed:			ret = false;	break;
				case ConnectionState.UpstreamDisconnected_downstreamUnsubscribed:		ret = false;	break;

				// used in QuikBrokerAdapter
				case ConnectionState.SymbolSubscribed:					ret = true;		break;
				case ConnectionState.SymbolUnsubscribed:				ret = true;		break;
				case ConnectionState.ErrorConnectingNoRetriesAnymore:	ret = false;	break;

				// used in QuikLivesimStreaming

				case ConnectionState.ConnectFailed:						ret = false;	break;
				case ConnectionState.DisconnectFailed:					ret = false;	break;		// can still be connected but by saying NotConnected I prevent other attempt to subscribe symbols; use "Connect" button to resolve

				default:
					Assembler.PopupException("ADD_HANDLER_FOR_NEW_ENUM_VALUE this.ConnectionState[" + this.UpstreamConnectionState + "]");
					ret = false;
					break;
			}
			return ret;
		} }
		
		[JsonProperty]	public		int						Level2RefreshRateMs;
		[JsonIgnore]	public		bool					QuotePumpSeparatePushingThreadEnabled	{ get; protected set; }
		[JsonIgnore]	public		LivesimStreaming		LivesimStreaming_ownImplementation		{ get; protected set; }

		// public for assemblyLoader: Streaming-derived.CreateInstance();
		public StreamingAdapter() {
			ReasonToExist									= "DUMMY_FOR_LIST_OF_STREAMING_PROVIDERS_IN_DATASOURCE_EDITOR";
			SymbolsSubscribedLock							= new object();
			SymbolsUpstreamSubscribed						= new List<string>();
			StreamingDataSnapshot							= new StreamingDataSnapshot(this);
			StreamingSolidifier								= new StreamingSolidifier();
			Level2RefreshRateMs								= 200;
			QuotePumpSeparatePushingThreadEnabled			= true;
			LivesimStreaming_ownImplementation				= null;		// so that be careful addressing it here! valid only for StreamingAdapter-derived!!!
			//if (this is LivesimStreaming) return;
			//NULL_UNTIL_QUIK_PROVIDES_OWN_DDE_REDIRECTOR LivesimStreamingImplementation					= new LivesimStreamingDefault(true, "USED_FOR_LIVESIM_ON_DATASOURCES_WITHOUT_ASSIGNED_STREAMING");	// QuikStreaming replaces it to DdeGenerator + QuikPuppet
		}

		public StreamingAdapter(string reasonToExist) : this() {
			ReasonToExist									= reasonToExist;
			this.CreateDataDistributors_onlyWhenNecessary(reasonToExist);
		}

		public void CreateDataDistributors_onlyWhenNecessary(string reasonToExist) {
			if (this.DataDistributor_replacedForLivesim == null) {
				this.DataDistributor_replacedForLivesim			= new DataDistributorCharts		(this, reasonToExist);
			} else {
				//this.DataDistributor_replacedForLivesim.ForceUnsubscribeLeftovers_mustBeEmptyAlready(reasonToExist);
			}
			this.DataDistributorSolidifiers_replacedForLivesim	= new DataDistributorSolidifiers(this, reasonToExist);
		}
		public virtual void InitializeFromDataSource(DataSource dataSource) {
			this.DataSource = dataSource;
			this.StreamingDataSnapshot.InitializeLastQuoteReceived(this.DataSource.Symbols);
			this.UpstreamConnectionState = ConnectionState.JustInitialized_solidifiersUnsubscribed;
		}
		protected virtual void SolidifierAllSymbolsSubscribe_onAppRestart() {
			string msg = "SUBSCRIBING_SOLIDIFIER_APPRESTART " + this.DataSource.Name;
			Assembler.PopupException(msg, null, false);

			this.CreateDataDistributors_onlyWhenNecessary(this.ReasonToExist);
			this.StreamingSolidifier.Initialize(this.DataSource);
			foreach (string symbol in this.DataSource.Symbols) {
				this.solidifierSubscribeOneSymbol(symbol);
			}
			this.UpstreamConnectionState = ConnectionState.JustInitialized_solidifiersSubscribed;
		}

		//public void SolidifierSubscribeOneSymbol_iFinishedLivesimming(string symbol = null) {
		void solidifierSubscribeOneSymbol(string symbol = null) {
			if (this.DataDistributorSolidifiers_replacedForLivesim == null) {
				string msg = "DONT_SUBSCRIBE_SOLIDIFIERS_FOR_DUMMY_ADAPTERS this[" + this + "]";
				Assembler.PopupException(msg, null, false);
				return;
			}

			if (symbol == null) {
				string msg = "WHEN_AM_I_ACTIVATED??? IT_LOOKS_VERY_SUSPICIOUS_HERE NPE_RISKY";
				Assembler.PopupException(msg);
				symbol  = this.LivesimStreaming_ownImplementation.DataSource.Symbols[0];
			}

			this.DataDistributorSolidifiers_replacedForLivesim.ConsumerBarSubscribe(symbol,
				this.DataSource.ScaleInterval, this.StreamingSolidifier, this.QuotePumpSeparatePushingThreadEnabled);
			this.DataDistributorSolidifiers_replacedForLivesim.ConsumerQuoteSubscribe(symbol,
				this.DataSource.ScaleInterval, this.StreamingSolidifier, this.QuotePumpSeparatePushingThreadEnabled);

			//v1
			//SymbolScaleDistributionChannel channel = this.DataDistributorSolidifiers_replacedForLivesim.GetDistributionChannelFor_nullUnsafe(symbol, this.DataSource.ScaleInterval);
			//if (channel == null) {
			//	string msg = "NONSENSE";
			//	Assembler.PopupException(msg);
			//	return;
			//}
			//channel.QuotePump.UpdateThreadNameAfterMaxConsumersSubscribed = true;
			//MOVED_TO_SetQuotePumpThreadName_unpausePump_sinceNoMoreSubscribersWillFollowFor() channel.QuotePump.PusherUnpause();
			//v2
			this.DataDistributorSolidifiers_replacedForLivesim.SetQuotePumpThreadName_sinceNoMoreSubscribersWillFollowFor(symbol, this.DataSource.ScaleInterval);
		}
		//protected virtual void SolidifierAllSymbolsUnsubscribe(bool throwOnAlreadySubscribed = true) {
		//	List<string> oneSymbolImLivesimming = this.DataSource.Symbols;
		//	//List<string> oneSymbolImLivesimming = this.LivesimStreaming.DataSource.Symbols;
		//	foreach (string symbol in oneSymbolImLivesimming) {
		//		this.SolidifierUnsubscribeOneSymbol_imLivesimming(symbol);
		//	}
		//}

		//public void SolidifierUnsubscribeOneSymbol_imLivesimming(string symbol = null) {
		void solidifierUnsubscribeOneSymbol(string symbol = null) {
			if (symbol == null) {
				symbol  = this.LivesimStreaming_ownImplementation.DataSource.Symbols[0];
			}

			if (this.DataDistributorSolidifiers_replacedForLivesim.ConsumerBarIsSubscribed(symbol, this.DataSource.ScaleInterval, this.StreamingSolidifier, false)) {
				this.DataDistributorSolidifiers_replacedForLivesim.ConsumerBarUnsubscribe(symbol, this.DataSource.ScaleInterval, this.StreamingSolidifier);
			} else {
				string msg = "IGNORE_ME_IF_YOU_ARE_STARTED_LIVESIM SOLIDIFIER_NOT_SUBSCRIBED_BARS symbol[" + symbol + "] ScaleInterval[" + this.DataSource.ScaleInterval + "]";
				Assembler.PopupException(msg, null, false);
			}
			if (this.DataDistributorSolidifiers_replacedForLivesim.ConsumerQuoteIsSubscribed(symbol, this.DataSource.ScaleInterval, this.StreamingSolidifier, false)) {
				this.DataDistributorSolidifiers_replacedForLivesim.ConsumerQuoteUnsubscribe(symbol, this.DataSource.ScaleInterval, this.StreamingSolidifier);
			} else {
				string msg = "IGNORE_ME_IF_YOU_ARE_STARTED_LIVESIM SOLIDIFIER_NOT_SUBSCRIBED_QUOTES symbol[" + symbol + "] ScaleInterval[" + this.DataSource.ScaleInterval + "]";
				Assembler.PopupException(msg, null, false);
			}
			SymbolScaleDistributionChannel channel = this.DataDistributorSolidifiers_replacedForLivesim.GetDistributionChannelFor_nullUnsafe(symbol, this.DataSource.ScaleInterval);
			if (channel == null) {
				string msg = "I_START_LIVESIM_WITHOUT_ANY_PRIOR_SUBSCRIBED_SOLIDIFIERS symbol[" + symbol + "]";
				Assembler.PopupException(msg, null, false);
				return;
			}
			channel.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim.PusherPause();
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

		public virtual void PushQuoteReceived(Quote quote) {
			string msig = " //StreamingAdapter.PushQuoteReceived()" + this.ToString();
			
			if (this.DataDistributor_replacedForLivesim.DistributionChannels.Count == 0) {
				this.RaiseOnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_blinkDataSourceTreeWithOrange(quote);

				string msg = "I_REFUSE_TO_PUSH_QUOTE NO_SOLIDIFIER_NOR_CHARTS_SUBSCRIBED";
				if (		this.LivesimStreaming_ownImplementation != null
						 && this.LivesimStreaming_ownImplementation.Livesimulator != null
						 && this.LivesimStreaming_ownImplementation.Livesimulator.ImRunningLivesim) {
					this.LivesimStreaming_ownImplementation.Livesimulator.AbortRunningBacktestWaitAborted(msg, 0);
				}
				if (this is LivesimStreaming) return;	//already reported "USER_DIDNT_CLICK_CHART>BARS>SUBSCRIBE"
				Assembler.PopupException(msg, null, false);
				//NO_TOO_MANY_CHANGES_TO_LOOSEN_ALL_CHECKS GO_AND_DO_IT__I_WILL_SEE_ORANGE_BACKGROUNG_IN_DATASOURCE_TREE
				return;
			}

			if (quote.ServerTime == DateTime.MinValue) {
				quote.ServerTime = this.DataSource.MarketInfo.ConvertLocalTimeToServer(DateTime.Now);
			}

			MarketInfo marketInfo = this.DataSource.MarketInfo;
			//if (marketInfo.IsMarketOpenNow == false) {
			if (marketInfo.IsMarketOpenAtServerTime(quote.ServerTime) == false) {
				MarketClearingTimespan clearingTimespanOut;
				DateTime dateTimeNextBarOpenConditional = marketInfo.GetNextMarketServerTimeStamp(
					quote.ServerTime, this.DataSource.ScaleInterval, out clearingTimespanOut);
				string reason = (clearingTimespanOut != null) ? "is CLEARING" : "CLOSED";
				string mainFormStatus = "[" + marketInfo.Name + "]Market " + reason + ", resumes["
					+ dateTimeNextBarOpenConditional.ToString("HH:mm") + "]; ignoring quote[" + quote + "]";
				Assembler.DisplayStatus(mainFormStatus);

				//string msg = "HACK!!! FILLING_LAST_BIDASK_FOR_DUPE_QUOTE_IS_UNJUSTIFIED: PREV_QUOTE_ABSNO_MUST_BE_LINEAR_WITHOUT_HOLES Backtester.generateQuotesForBarAndPokeStreaming()";
				//Assembler.PopupException(msg, null, false);
				//essence of PushQuoteReceived(); all above were pre-checks ensuring successfull completion of two lines below
				// ALREADY_ENRICHED this.EnrichQuoteWithStreamingDependantDataSnapshot(quote);
				//LOTS_OF_NOISE this.StreamingDataSnapshot.LastQuoteCloneSetForSymbol(quote);
				return;
			}

			long absnoPerSymbolNext = 0;

			Quote lastQuote = this.StreamingDataSnapshot.LastQuoteClone_getForSymbol(quote.Symbol);
			if (lastQuote == null) {
				string msg = "RECEIVED_FIRST_QUOTE_EVER_FOR#1 symbol[" + quote.Symbol + "] SKIPPING_LASTQUOTE_ABSNO_CHECK SKIPPING_QUOTE<=LASTQUOTE_NEXT_CHECK";
				Assembler.PopupException(msg + msig, null, false);
			} else {
				if (lastQuote.AbsnoPerSymbol == -1) {
					string msg = "LAST_QUOTE_DIDNT_HAVE_ABSNO_SET_BY_STREAMING_ADAPDER_ON_PREV_ITERATION";
					Assembler.PopupException(msg + msig, null, true);
				}

				//v1 HAS_NO_MILLISECONDS_FROM_QUIK if (quote.ServerTime > lastQuote.ServerTime) {
				//v2 TOO_SENSITIVE_PRINTED_SAME_MILLISECONDS_BUT_STILL_DIFFERENT if (quote.ServerTime.Ticks > lastQuote.ServerTime.Ticks) {
				string quoteMillis			= quote.ServerTime.ToString("HH:mm:ss.fff");
				string lastQuoteMillis  = lastQuote.ServerTime.ToString("HH:mm:ss.fff");
				if (quoteMillis == lastQuoteMillis) {
					string msg = "DONT_FEED_ME_WITH_SAME_SERVER_TIME BACKTESTER_FORGOT_TO_INCREASE_SERVER_TIMESTAMP"
						+ " upcoming quote.LocalTimeCreatedMillis[" + quote.LocalTimeCreated.ToString("HH:mm:ss.fff")
						+ "] <= lastQuoteReceived.Symbol." + quote.Symbol + "["
						+ lastQuote.LocalTimeCreated.ToString("HH:mm:ss.fff") + "]: DDE lagged somewhere?...";
					//Assembler.PopupException(msg + msig, null, false);
				}

				absnoPerSymbolNext = lastQuote.AbsnoPerSymbol + 1;
			}

			//QUOTE_ABSNO_MUST_BE_-1__HERE_NOT_MODIFIED_AFTER_QUOTE.CTOR()
			if (quote.AbsnoPerSymbol != -1) {
				if (quote.IamInjectedToFillPendingAlerts) {
					string msg = "INJECTED_QUOTES_HAVE_AbsnoPerSymbol!=-1_AND_THIS_IS_NOT_AN_ERROR";
				} else {
					string msg = "THIS_WAS_REFACTORED__QUOTE_ABSNO_MUST_BE_SEQUENTIAL_PER_SYMBOL__INITIALIZED_IN_STREAMING_ADAPDER";
					//Assembler.PopupException(msg + msig, null, true);
				}

				//QUOTE_ABSNO_MUST_BE_SEQUENTIAL_PER_SYMBOL INITIALIZED_IN_STREAMING_ADAPDER
				//if (quote.AbsnoPerSymbol >= absnoPerSymbolNext) {
				//	string msg1 = "DONT_FEED_ME_WITH_SAME_QUOTE_BACKTESTER quote.Absno[" + quote.AbsnoPerSymbol + "] >= lastQuote.Absno[" + lastQuote.AbsnoPerSymbol + "] + 1";
				//	Assembler.PopupException(msg + msig, null, true);
				//}
			}
			quote.AbsnoPerSymbol = absnoPerSymbolNext;

			//OUTDATED?... BacktestStreamingAdapter.EnrichGeneratedQuoteSaveSpreadInStreaming has updated lastQuote alredy...
			//essence of PushQuoteReceived(); all above were pre-checks ensuring successfull completion of two lines below
			// ALREADY_ENRICHED this.EnrichQuoteWithStreamingDependantDataSnapshot(quote);
			this.StreamingDataSnapshot.LastQuoteClone_setForSymbol(quote);

			try {
				bool thisIsLivesimDataSource	= this is LivesimDataSource;
				if (thisIsLivesimDataSource) {
					if (this.DataDistributorSolidifiers_replacedForLivesim.DistributionChannels.Count > 0) {
						string msg = "LIVESIM_RUN__MUST_HAVE_REPLACED_MY_DATASOURCE_WITHOUT_SOLIDIFIERS";
						Assembler.PopupException(msg, null, false);
					}
				}
			} catch (Exception ex) {
				string msg = "SOME_CONSUMERS_SOME_SCALEINTERVALS_FAILED_INSIDE"
					+ " DataDistributorSolidifiers.PushQuoteToDistributionChannels(" + quote + ")"
					+ " StreamingAdapter.PushQuoteReceived() " + this.ToString();
				Assembler.PopupException(msg + msig, ex);
			}


			try {
				this.DataDistributor_replacedForLivesim				.PushQuoteToDistributionChannels(quote);
			} catch (Exception ex) {
				string msg = "CHART_OR_STRATEGY__FAILED_INSIDE"
					+ " DataDistributor.PushQuoteToDistributionChannels(" + quote + ")";
				Assembler.PopupException(msg + msig, ex);
			}
			try {
				this.DataDistributorSolidifiers_replacedForLivesim	.PushQuoteToDistributionChannels(quote);
			} catch (Exception ex) {
				string msg = "SOLIDIFIER__FAILED_INSIDE"
					+ " DataDistributorSolidifiers.PushQuoteToDistributionChannels(" + quote + ")";
				Assembler.PopupException(msg + msig, ex);
			}
		}
		public void UpstreamSubscribedToSymbolPokeConsumersHelper(string symbol) {
			List<SymbolScaleDistributionChannel> channels = this.DataDistributor_replacedForLivesim.GetDistributionChannels_allScaleIntervals_forSymbol(symbol);
			foreach (var channel in channels) {
				channel.UpstreamSubscribedToSymbolPokeConsumers(symbol);
			}
		}
		public void UpstreamUnSubscribedFromSymbolPokeConsumersHelper(string symbol) {
			List<SymbolScaleDistributionChannel> channels = this.DataDistributor_replacedForLivesim.GetDistributionChannels_allScaleIntervals_forSymbol(symbol);
			Quote lastQuoteReceived = this.StreamingDataSnapshot.LastQuoteClone_getForSymbol(symbol);
			foreach (var channel in channels) {
				channel.UpstreamUnSubscribedFromSymbolPokeConsumers(symbol, lastQuoteReceived);
			}
		}
		public void InitializeStreamingOHLCVfromStreamingAdapter(Bars chartBars) {
			SymbolScaleDistributionChannel channel = this.DataDistributor_replacedForLivesim
				.GetDistributionChannelFor_nullUnsafe(chartBars.Symbol, chartBars.ScaleInterval);
			if (channel == null) return;
			//v1 
			//Bar streamingBar = distributionChannel.StreamingBarFactoryUnattached.StreamingBarUnattached;
			Bar streamingBar = chartBars.BarStreaming_nullUnsafe;
			if (streamingBar == null) {
				string msg = "STREAMING_NEVER_STARTED BarFactory.StreamingBar=null for distributionChannel[" + channel + "]";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			if (streamingBar.DateTimeOpen == DateTime.MinValue) {
				string msg = "STREAMING_NEVER_STARTED streamingBar.DateTimeOpen=MinValue [" + streamingBar
					+ "] for distributionChannel[" + channel + "]";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			if (chartBars.BarStaticLast_nullUnsafe == null) {
				// FAILED_FIXING_IN_DataDistributor
				string msg = "BAR_STATIC_LAST_IS_NULL while streamingBar[" + streamingBar
					+ "] for distributionChannel[" + channel + "]";
				// Assembler.PopupException(msg, null);
				//throw new Exception(msg);
				return;
			}
			
			if (streamingBar.DateTimeOpen != chartBars.BarStaticLast_nullUnsafe.DateTimeNextBarOpenUnconditional) {
				if (streamingBar.DateTimeOpen == chartBars.BarStaticLast_nullUnsafe.DateTimeOpen) {
					string msg = "STREAMINGBAR_OVERWROTE_LASTBAR streamingBar.DateTimeOpen[" + streamingBar.DateTimeOpen
						+ "] == this.LastStaticBar.DateTimeOpen[" + chartBars.BarStaticLast_nullUnsafe.DateTimeOpen + "] " + chartBars;
					//log.Error(msg);
				} else {
					string msg = "STREAMINGBAR_OUTDATED streamingBar.DateTimeOpen[" + streamingBar.DateTimeOpen
						+ "] != chartBars.LastStaticBar.DateTimeNextBarOpenUnconditional["
						+ chartBars.BarStaticLast_nullUnsafe.DateTimeNextBarOpenUnconditional + "] " + chartBars;
					//log.Error(msg);
				}
			}
			chartBars.BarStreamingOverrideDOHLCVwith(streamingBar);
			string msg1 = "StreamingOHLCV Overwritten: Bars.StreamingBar[" + chartBars.BarStreaming_nullUnsafeCloneReadonly + "] taken from streamingBar[" + streamingBar + "]";
			//Assembler.PopupException(msg1, null, false);
		}
		public override string ToString() {
			string dataSourceAsString = this.DataSource != null ? this.DataSource.ToString() : "NOT_INITIALIZED_YET";
			string ret = this.Name + "/[" + this.UpstreamConnectionState + "]"
				+ ": UpstreamSymbols[" + this.SymbolsUpstreamSubscribedAsString + "]"
				//+ "DataSource[" + dataSourceAsString + "]"
				+ " (" + this.ReasonToExist + ")"
				;
			return ret;
		}

		internal void AbsorbStreamingBarFactoryFromBacktestComplete(StreamingAdapter streamingBacktest, string symbol, BarScaleInterval barScaleInterval) {
			SymbolScaleDistributionChannel channelBacktest = streamingBacktest.DataDistributor_replacedForLivesim.GetDistributionChannelFor_nullUnsafe(symbol, barScaleInterval);
			if (channelBacktest == null) return;
			Bar barLastFormedBacktest = channelBacktest.StreamingBarFactoryUnattached.BarLastFormedUnattached_nullUnsafe;
			if (barLastFormedBacktest == null) return;

			Bar barStreamingBacktest = channelBacktest.StreamingBarFactoryUnattached.BarStreamingUnattached;

			SymbolScaleDistributionChannel channelOriginal = this.DataDistributor_replacedForLivesim.GetDistributionChannelFor_nullUnsafe(symbol, barScaleInterval);
			if (channelOriginal == null) return;
			Bar barLastFormedOriginal = channelOriginal.StreamingBarFactoryUnattached.BarLastFormedUnattached_nullUnsafe;
			//if (barLastFormedOriginal == null) return;

			channelOriginal.StreamingBarFactoryUnattached.AbsorbBarLastStaticFromChannelBacktesterComplete(channelBacktest);
			Bar barLastFormedAbsorbed = channelOriginal.StreamingBarFactoryUnattached.BarLastFormedUnattached_nullUnsafe;
			if (barLastFormedOriginal == null || barLastFormedAbsorbed.DateTimeOpen != barLastFormedOriginal.DateTimeOpen) {
				string msg = "GUT";
			}

			Bar barStreamingOriginal = channelOriginal.StreamingBarFactoryUnattached.BarStreamingUnattached;
			channelOriginal.StreamingBarFactoryUnattached.AbsorbBarStreamingFromChannel(channelBacktest);
			Bar barStreamingAbsorbed = channelOriginal.StreamingBarFactoryUnattached.BarStreamingUnattached;
			if (barStreamingAbsorbed == null || barStreamingAbsorbed.DateTimeOpen != barStreamingOriginal.DateTimeOpen) {
				string msg = "GUT";
			}
			return;

			//if (barLastFormedOriginal == null) {
			//	string msg = "NO_NEED_TO_RESET_STREAMING_BAR_FACTORY__NO_QUOTE_WAS_PUMPED_IN_YET_SINCE_APPRESTART__FIRST_EVER_AUTOBACKTEST_COMPLETE";
			//	Assembler.PopupException(msg, null, false);
			//	//channelOriginal.StreamingBarFactoryUnattached.AbsorbBarLastStaticFromChannel(channelBacktest);
			//	//NOPE_LEAVE_IT_MINE__ON_APPRESTART_NULL__IN_MID_LIVE_BACKTEST_SHOULD_HAVE_LAST_INCOMING_QUOTE KEEP_THIS_NOT_HAPPENING_BY_LEAVING_STATIC_LAST_ON_APPRESTART_NULL_ON_LIVEBACKTEST_CONTAINING_LAST_INCOMING_QUOTE
			//	channelOriginal.StreamingBarFactoryUnattached.AbsorbBarStreamingFromChannel(channelBacktest);
			//	return;
			//}
			//if (barStreamingOriginal.DateTimeOpen == DateTime.MinValue) {
			//	string msg = "LIVE_STREAMING_NEVER_STARTED_SINCE_APPRESTART__NO_NEED_TO_FORCE_EXECUTE_ON_LASTFORMED_WILL_BE_TRIGGERED_BY_ITSELF";
			//	Assembler.PopupException(msg, null, false);
			//	return;
			//}
			//if (barLastFormedOriginal.DateTimeOpen == DateTime.MinValue) {
			//	string msg = "LIVE_STREAMING_STARTED_BUT_LAST_FORMED_NEVER_GENERATED_SINCE_APPRESTART___NO_NEED_TO_FORCE_EXECUTE_ON_LASTFORMED_WILL_BE_TRIGGERED_BY_ITSELF";
			//	Assembler.PopupException(msg, null, false);
			//	return;
			//}
			//// UNATTACHED_MEANS_NO_PARENT_BARS_AT_ALL
			//int mustBeOneToForceLastFormedExec = barLastFormedBacktest.ParentBarsIndex - barLastFormedOriginal.ParentBarsIndex;
			//if (mustBeOneToForceLastFormedExec != 1) {

			////NEW_BAR_CREATION_CONDITION: if (quoteClone.ServerTime >= this.BarStreamingUnattached.DateTimeNextBarOpenUnconditional) {
			//bool canTriggerNewBarCreation = barStreamingOriginal.DateTimeOpen > barStreamingBacktest.DateTimeOpen;
			////bool canTriggerNewBarCreation = barStreamingOriginal.DateTimeOpen < barStreamingOriginal.DateTimeNextBarOpenUnconditional;
			//if (canTriggerNewBarCreation) {
			//	string msg2 = "ANY_NEW_QUOTE_WILL_TRIGGER_LASTBAR_EXECUTION?";
			//	string msg1 = "FORCING_EXECUTE_ON_LASTFORMED_BY_RESETTING_LAST_FORMED_TO_PREVIOUSLY_EXECUTED_AFTER_BACKTEST";
			//	//Assembler.PopupException(msg2);
			//	channelOriginal.StreamingBarFactoryUnattached.AbsorbBarLastStaticFromChannelBacktesterComplete(channelBacktest);
			//}

			//string difference = "";
			//bool hasSame = barLastFormedOriginal.HasSameDOHLCVas(barLastFormedBacktest, "barLastBacktest", "barLastOriginal", ref difference);
			//if (hasSame) {
			//	string msg = "MUST_BE_DIFFERENT_FOR_MID_LIVE_BACKTEST_SINCE_QUOTES_MUST_HAVE_BEEN_PAUSED " + difference;
			//	//Assembler.PopupException(msg);
			//}

			//hasSame = barStreamingOriginal.HasSameDOHLCVas(barStreamingBacktest, "barStreamingOriginal", "barStreamingBacktest", ref difference);
			//if (hasSame) {
			//	string msg = "MUST_BE_DIFFERENT_FOR_MID_LIVE_BACKTEST_SINCE_QUOTES_MUST_HAVE_BEEN_PAUSED " + difference;
			//	Assembler.PopupException(msg);
			//}
		}

		internal void UnsubscribeChart(string symbolSafe, BarScaleInterval scaleIntervalSafe, Charting.ChartStreamingConsumer chartStreamingConsumer, string msigForNpExceptions) {
			if (this.DataDistributor_replacedForLivesim.ConsumerQuoteIsSubscribed(symbolSafe, scaleIntervalSafe, chartStreamingConsumer, false) == false) {
				Assembler.PopupException("CHART_STREAMING_WASNT_SUBSCRIBED_CONSUMER_QUOTE" + msigForNpExceptions);
			} else {
				//Assembler.PopupException("UnSubscribing QuoteConsumer [" + this + "]  to " + plug + "  (was subscribed)");
				this.DataDistributor_replacedForLivesim.ConsumerQuoteUnsubscribe(symbolSafe, scaleIntervalSafe, chartStreamingConsumer);
			}

			if (this.DataDistributor_replacedForLivesim.ConsumerBarIsSubscribed(symbolSafe, scaleIntervalSafe, chartStreamingConsumer, false) == false) {
				Assembler.PopupException("CHART_STREAMING_WASNT_SUBSCRIBED_CONSUMER_BAR" + msigForNpExceptions);
			} else {
				//Assembler.PopupException("UnSubscribing BarsConsumer [" + this + "] to " + this.ToString() + " (was subscribed)");
				this.DataDistributor_replacedForLivesim.ConsumerBarUnsubscribe(symbolSafe, scaleIntervalSafe, chartStreamingConsumer);
			}
		}
		internal void SubscribeChart(string symbolSafe, BarScaleInterval scaleIntervalSafe, Charting.ChartStreamingConsumer chartStreamingConsumer, string msigForNpExceptions) {
			bool iWantChartToConsumeQuotesInSeparateThreadToLetStreamingGoWithoutWaitingForStrategyToFinish = true;

			//NPE_AFTER_SEPARATED_SOLIDIFIERS SymbolScaleDistributionChannel channel = streamingSafe.DataDistributor.GetDistributionChannelFor_nullUnsafe(symbolSafe, scaleIntervalSafe);
			if (this.DataDistributor_replacedForLivesim.ConsumerQuoteIsSubscribed(symbolSafe, scaleIntervalSafe, chartStreamingConsumer) == true) {
				Assembler.PopupException("CHART_STREAMING_ALREADY_SUBSCRIBED_CONSUMER_QUOTE" + msigForNpExceptions);
			} else {
				//Assembler.PopupException("Subscribing QuoteConsumer [" + this + "]  to " + plug + "  (wasn't registered)");
				this.DataDistributor_replacedForLivesim.ConsumerQuoteSubscribe(symbolSafe, scaleIntervalSafe, chartStreamingConsumer,
						iWantChartToConsumeQuotesInSeparateThreadToLetStreamingGoWithoutWaitingForStrategyToFinish);
			}

			if (this.DataDistributor_replacedForLivesim.ConsumerBarIsSubscribed(symbolSafe, scaleIntervalSafe, chartStreamingConsumer) == true) {
				Assembler.PopupException("CHART_STREAMING_ALREADY_SUBSCRIBED_CONSUMER_BAR" + msigForNpExceptions);
			} else {
				//Assembler.PopupException("Subscribing BarsConsumer [" + this + "] to " + this.ToString() + " (wasn't registered)");
				this.DataDistributor_replacedForLivesim.ConsumerBarSubscribe(symbolSafe, scaleIntervalSafe, chartStreamingConsumer, true);
				//nonsense taken from ChartStreamingConsumer
				//if (executorSafe.Bars == null) {
				//	// in Initialize() this.ChartForm is requesting bars in a separate thread
				//	this.DataDistributor.ConsumerBarSubscribe(symbolSafe, scaleIntervalSafe, chartStreamingConsumer, true);
				//} else {
				//	// fully initialized, after streaming was stopped for a moment and resumed - append into PartialBar
				//	if (double.IsNaN(streamingBarSafeCloneSafe.Open) == false) {
				//		//streamingSafe.ConsumerBarRegister(symbolSafe, scaleIntervalSafe, this, streamingBarSafeCloneSafe);
				//		this.DataDistributor.ConsumerBarSubscribe(symbolSafe, scaleIntervalSafe, chartStreamingConsumer, true);
				//	} else {
				//		//streamingSafe.ConsumerBarRegister(symbolSafe, scaleIntervalSafe, this, lastStaticBarSafe);
				//		this.DataDistributor.ConsumerBarSubscribe(symbolSafe, scaleIntervalSafe, chartStreamingConsumer, true);
				//	}
				//}
			}
		}
	}
} 