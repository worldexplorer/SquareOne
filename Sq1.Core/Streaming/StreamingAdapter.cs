using System;
using System.Collections.Generic;
using System.Drawing;

using Newtonsoft.Json;

using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Livesim;

namespace Sq1.Core.Streaming {
	// TODO: it's not an abstract class because....
	public partial class StreamingAdapter {
		[JsonIgnore]	public const string				NO_STREAMING_ADAPTER = "--- No Streaming Adapter ---";
		
		[JsonIgnore]	public string					Name								{ get; protected set; }
		[JsonIgnore]	public string					Description							{ get; protected set; }
		[JsonIgnore]	public Bitmap					Icon								{ get; protected set; }
		[JsonIgnore]	public bool						StreamingConnected					{ get; protected set; }
		[JsonIgnore]	public StreamingSolidifier		StreamingSolidifier					{ get; protected set; }
		[JsonIgnore]	public DataSource				DataSource;
		[JsonIgnore]	public string					marketName							{ get { return this.DataSource.MarketInfo.Name; } }
		[JsonIgnore]	public DataDistributor			DataDistributor						{ get; protected set; }
		[JsonIgnore]	public DataDistributor			DataDistributorSolidifiers			{ get; protected set; }
		[JsonIgnore]	public StreamingDataSnapshot	StreamingDataSnapshot				{ get; protected set; }
		[JsonIgnore]	public virtual List<string>		SymbolsUpstreamSubscribed			{ get; private set; }
		[JsonIgnore]	protected object				SymbolsSubscribedLock;
		[JsonIgnore]	public virtual string			SymbolsUpstreamSubscribedAsString 	{ get {
				string ret = "";
				lock (SymbolsSubscribedLock) {
					foreach (string symbol in SymbolsUpstreamSubscribed) ret += symbol + ",";
				}
				ret = ret.TrimEnd(',');
				return ret;
			} }
		[JsonProperty]	public ConnectionState			ConnectionState						{ get; protected set; }
		[JsonIgnore]	public bool						QuotePumpSeparatePushingThreadEnabled { get; protected set; }

		[JsonIgnore]	public LivesimStreaming			LivesimStreaming					{ get; protected set; }

		// public for assemblyLoader: Streaming-derived.CreateInstance();
		public StreamingAdapter() {
			SymbolsSubscribedLock					= new object();
			SymbolsUpstreamSubscribed				= new List<string>();
			DataDistributor							= new DataDistributorCharts(this);
			DataDistributorSolidifiers				= new DataDistributorSolidifiers(this);
			StreamingDataSnapshot					= new StreamingDataSnapshot(this);
			StreamingSolidifier						= new StreamingSolidifier();
			QuotePumpSeparatePushingThreadEnabled	= true;
			if (this is LivesimStreaming) return;
			LivesimStreaming						= new LivesimStreaming(true);	// QuikStreaming replaces it to DdeGenerator + QuikPuppet
		}
		public virtual void InitializeDataSource(DataSource dataSource, bool subscribeSolidifier = true) {
			this.InitializeFromDataSource(dataSource);
			if (subscribeSolidifier) {
				this.SolidifierSubscribe();
			} else {
				this.SolidifierUnsubscribe(false);
			}
		}
		public virtual void InitializeFromDataSource(DataSource dataSource) {
			this.DataSource = dataSource;
			this.StreamingDataSnapshot.InitializeLastQuoteReceived(this.DataSource.Symbols);
		}
		protected virtual void SolidifierSubscribe() {
			this.StreamingSolidifier.Initialize(this.DataSource);
			foreach (string symbol in this.DataSource.Symbols) {
				this.DataDistributorSolidifiers.ConsumerBarSubscribe(symbol,
					this.DataSource.ScaleInterval, this.StreamingSolidifier, this.QuotePumpSeparatePushingThreadEnabled);
				this.DataDistributorSolidifiers.ConsumerQuoteSubscribe(symbol,
					this.DataSource.ScaleInterval, this.StreamingSolidifier, this.QuotePumpSeparatePushingThreadEnabled);
				SymbolScaleDistributionChannel channel = this.DataDistributorSolidifiers.GetDistributionChannelForNullUnsafe(symbol, this.DataSource.ScaleInterval);
				if (channel == null) {
					string msg = "NONSENSE";
					Assembler.PopupException(msg);
					return;
				}
				channel.QuotePump.UpdateThreadNameAfterMaxConsumersSubscribed = true;
				channel.QuotePump.PusherUnpause();
			}
		}
		protected virtual void SolidifierUnsubscribe(bool throwOnAlreadySubscribed = true) {
			foreach (string symbol in this.DataSource.Symbols) {
				this.DataDistributorSolidifiers.ConsumerBarUnsubscribe(symbol,
					this.DataSource.ScaleInterval, this.StreamingSolidifier);
				this.DataDistributorSolidifiers.ConsumerQuoteUnsubscribe(symbol,
					this.DataSource.ScaleInterval, this.StreamingSolidifier);
				SymbolScaleDistributionChannel channel = this.DataDistributorSolidifiers.GetDistributionChannelForNullUnsafe(symbol, this.DataSource.ScaleInterval);
				if (channel == null) {
					string msg = "I_START_LIVESIM_WITHOUT_ANY_PRIOR_SUBSCRIBED_SOLIDIFIERS symbol[" + symbol + "]";
					Assembler.PopupException(msg, null, false);
					continue;
				}
				channel.QuotePump.PusherPause();
			}
		}

		#region the essence#1 of streaming adapter
		public virtual void UpstreamConnect() {
			//StatusReporter.UpdateConnectionStatus(ConnectionState.ErrorConnecting, 0, "ConnectStreaming(): NOT_OVERRIDEN_IN_CHILD");
			Assembler.DisplayStatus("ConnectStreaming(): NOT_OVERRIDEN_IN_CHILD " + this.ToString());
		}
		public virtual void UpstreamDisconnect() {
			//StatusReporter.UpdateConnectionStatus(ConnectionState.ErrorDisconnecting, 0, "DisconnectStreaming(): NOT_OVERRIDEN_IN_CHILD");
			Assembler.DisplayStatus("DisconnectStreaming(): NOT_OVERRIDEN_IN_CHILD " + this.ToString());
		}
		#endregion

		#region the essence#2 of streaming adapter
		public virtual void UpstreamSubscribe(string symbol) {
			throw new Exception("please override StreamingAdapter::UpstreamSubscribe()");
			//CHILDREN_TEMPLATE: base.UpstreamSubscribeRegistryHelper(symbol);
		}
		public virtual void UpstreamUnSubscribe(string symbol) {
			throw new Exception("please override StreamingAdapter::UpstreamUnSubscribe()");
			//CHILDREN_TEMPLATE: base.UpstreamUnSubscribeRegistryHelper(symbol);
		}
		public virtual bool UpstreamIsSubscribed(string symbol) {
			throw new Exception("please override StreamingAdapter::UpstreamIsSubscribed()");
			//CHILDREN_TEMPLATE: return base.UpstreamIsSubscribedRegistryHelper(symbol);
		}
		#endregion

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
				return SymbolsUpstreamSubscribed.Contains(symbol);
			}
		}


		public virtual void PushQuoteReceived(Quote quote) {
			string msig = " //StreamingAdapter.PushQuoteReceived()" + this.ToString();
			
			bool dontSolidifyAndDontPushToCharts = this.DataDistributor.DistributionChannels.Count == 0;
			if (dontSolidifyAndDontPushToCharts) return;

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

			Quote lastQuote = this.StreamingDataSnapshot.LastQuoteCloneGetForSymbol(quote.Symbol);
			if (lastQuote == null) {
				string msg = "RECEIVED_FIRST_QUOTE_EVER_FOR#1 symbol[" + quote.Symbol + "] SKIPPING_LASTQUOTE_ABSNO_CHECK SKIPPING_QUOTE<=LASTQUOTE_NEXT_CHECK";
				//Assembler.PopupException(msg + msig, null, false);
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
					Assembler.PopupException(msg + msig);
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
			this.StreamingDataSnapshot.LastQuoteCloneSetForSymbol(quote);

			try {
				if (this.DataDistributorSolidifiers.DistributionChannels.Count > 0) {
					string msg = "REMIND_ME_WHEN_SOLIDIFIERS_ARE_SUBSCRIBED? I_WANT_TO_COLLECT_AND_SAVE_BARS_FOR_SYMBOLS_NON_OPENED_AS_CHARTS";
					Assembler.PopupException(msg);
				}
				//this.DataDistributorSolidifiers.PushQuoteToDistributionChannels(quote);
			} catch (Exception ex) {
				string msg = "SOME_CONSUMERS_SOME_SCALEINTERVALS_FAILED_INSIDE"
					+ " DataDistributorSolidifiers.PushQuoteToDistributionChannels(" + quote + ")"
					+ " StreamingAdapter.PushQuoteReceived() " + this.ToString();
				Assembler.PopupException(msg + msig, ex);
			}
			try {
				this.DataDistributor.PushQuoteToDistributionChannels(quote);
			} catch (Exception ex) {
				string msg = "SOME_CONSUMERS_SOME_SCALEINTERVALS_FAILED_INSIDE"
					+ " DataDistributor.PushQuoteToDistributionChannels(" + quote + ")";
				Assembler.PopupException(msg + msig, ex);
			}
		}
		public void UpstreamSubscribedToSymbolPokeConsumersHelper(string symbol) {
			List<SymbolScaleDistributionChannel> channels = this.DataDistributor.GetDistributionChannelsFor(symbol);
			foreach (var channel in channels) {
				channel.UpstreamSubscribedToSymbolPokeConsumers(symbol);
			}
		}
		public void UpstreamUnSubscribedFromSymbolPokeConsumersHelper(string symbol) {
			List<SymbolScaleDistributionChannel> channels = this.DataDistributor.GetDistributionChannelsFor(symbol);
			Quote lastQuoteReceived = this.StreamingDataSnapshot.LastQuoteCloneGetForSymbol(symbol);
			foreach (var channel in channels) {
				channel.UpstreamUnSubscribedFromSymbolPokeConsumers(symbol, lastQuoteReceived);
			}
		}
		public void InitializeStreamingOHLCVfromStreamingAdapter(Bars chartBars) {
			SymbolScaleDistributionChannel channel = this.DataDistributor
				.GetDistributionChannelForNullUnsafe(chartBars.Symbol, chartBars.ScaleInterval);
			if (channel == null) return;
			//v1 
			//Bar streamingBar = distributionChannel.StreamingBarFactoryUnattached.StreamingBarUnattached;
			Bar streamingBar = chartBars.BarStreamingNullUnsafe;
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
			if (chartBars.BarStaticLastNullUnsafe == null) {
				// FAILED_FIXING_IN_DataDistributor
				string msg = "BAR_STATIC_LAST_IS_NULL while streamingBar[" + streamingBar
					+ "] for distributionChannel[" + channel + "]";
				// Assembler.PopupException(msg, null);
				//throw new Exception(msg);
				return;
			}
			
			if (streamingBar.DateTimeOpen != chartBars.BarStaticLastNullUnsafe.DateTimeNextBarOpenUnconditional) {
				if (streamingBar.DateTimeOpen == chartBars.BarStaticLastNullUnsafe.DateTimeOpen) {
					string msg = "STREAMINGBAR_OVERWROTE_LASTBAR streamingBar.DateTimeOpen[" + streamingBar.DateTimeOpen
						+ "] == this.LastStaticBar.DateTimeOpen[" + chartBars.BarStaticLastNullUnsafe.DateTimeOpen + "] " + chartBars;
					//log.Error(msg);
				} else {
					string msg = "STREAMINGBAR_OUTDATED streamingBar.DateTimeOpen[" + streamingBar.DateTimeOpen
						+ "] != chartBars.LastStaticBar.DateTimeNextBarOpenUnconditional["
						+ chartBars.BarStaticLastNullUnsafe.DateTimeNextBarOpenUnconditional + "] " + chartBars;
					//log.Error(msg);
				}
			}
			chartBars.BarStreamingOverrideDOHLCVwith(streamingBar);
			string msg1 = "StreamingOHLCV Overwritten: Bars.StreamingBar[" + chartBars.BarStreamingNullUnsafeCloneReadonly + "] taken from streamingBar[" + streamingBar + "]";
			//Assembler.PopupException(msg1, null, false);
		}
		public virtual void EnrichQuoteWithStreamingDependantDataSnapshot(Quote quote) {
			// in Market-dependant StreamingAdapters, put in the Quote-derived quote anything like QuikQuote.FortsDepositBuy ;
		}

		public override string ToString() {
			return this.Name + "/[" + this.ConnectionState + "]: UpstreamSymbols[" + this.SymbolsUpstreamSubscribedAsString + "]";
		}

		internal void AbsorbStreamingBarFactoryFromBacktestComplete(StreamingAdapter streamingBacktest, string symbol, BarScaleInterval barScaleInterval) {
			SymbolScaleDistributionChannel channelBacktest = streamingBacktest.DataDistributor.GetDistributionChannelForNullUnsafe(symbol, barScaleInterval);
			if (channelBacktest == null) return;
			Bar barLastFormedBacktest = channelBacktest.StreamingBarFactoryUnattached.BarLastFormedUnattachedNullUnsafe;
			if (barLastFormedBacktest == null) return;

			Bar barStreamingBacktest = channelBacktest.StreamingBarFactoryUnattached.BarStreamingUnattached;

			SymbolScaleDistributionChannel channelOriginal = this.DataDistributor.GetDistributionChannelForNullUnsafe(symbol, barScaleInterval);
			if (channelOriginal == null) return;
			Bar barLastFormedOriginal = channelOriginal.StreamingBarFactoryUnattached.BarLastFormedUnattachedNullUnsafe;
			//if (barLastFormedOriginal == null) return;

			channelOriginal.StreamingBarFactoryUnattached.AbsorbBarLastStaticFromChannelBacktesterComplete(channelBacktest);
			Bar barLastFormedAbsorbed = channelOriginal.StreamingBarFactoryUnattached.BarLastFormedUnattachedNullUnsafe;
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

		internal void SetQuotePumpThreadNameSinceNoMoreSubscribersWillFollowFor(string symbol, BarScaleInterval barScaleInterval) {
			SymbolScaleDistributionChannel channel = this.DataDistributor.GetDistributionChannelForNullUnsafe(symbol, barScaleInterval);
			if (channel == null) {
				string msg = "SPLIT_QUOTE_PUMP_TO_SINGLE_THREADED_AND_SELF_LAUNCHING";
				Assembler.PopupException(msg);
				return;
			}
			if (this.QuotePumpSeparatePushingThreadEnabled) {
				channel.QuotePump.UpdateThreadNameAfterMaxConsumersSubscribed = true;
			} else {
				channel.QuotePump.SetThreadName();
			}
		}
	}
}