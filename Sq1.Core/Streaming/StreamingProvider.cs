using System;
using System.Collections.Generic;
using System.Drawing;

using Newtonsoft.Json;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Support;

namespace Sq1.Core.Streaming {
	// TODO: it's not an abstract class because....
	public partial class StreamingProvider {
		[JsonIgnore]	public const string				NO_STREAMING_PROVIDER = "--- No Streaming Provider ---";
		
		[JsonIgnore]	public string					Name								{ get; protected set; }
		[JsonIgnore]	public string					Description							{ get; protected set; }
		[JsonIgnore]	public Bitmap					Icon								{ get; protected set; }
		[JsonIgnore]	public bool						IsConnected							{ get; protected set; }
		[JsonIgnore]	public string					PreferredStaticProviderName 		{ get; protected set; }
		[JsonIgnore]	public StreamingSolidifier		StreamingSolidifier					{ get; protected set; }
		[JsonIgnore]	public DataSource				DataSource;
		[JsonIgnore]	public string					marketName							{ get { return this.DataSource.MarketInfo.Name; } }
		[JsonIgnore]	public DataDistributor			DataDistributor						{ get; protected set; }
		[JsonProperty]	public StreamingDataSnapshot	StreamingDataSnapshot				{ get; protected set; }
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
		[JsonIgnore]	protected object				BarsConsumersLock;
		[JsonIgnore]	public ConnectionState			ConnectionState						{ get; protected set; }

		// public for assemblyLoader: Streaming-derived.CreateInstance();
		public StreamingProvider() {
			SymbolsSubscribedLock = new object();
			BarsConsumersLock = new object();
			SymbolsUpstreamSubscribed = new List<string>();
			DataDistributor = new DataDistributor(this);
			StreamingDataSnapshot = new StreamingDataSnapshot(this);
			StreamingSolidifier = new StreamingSolidifier();
		}
		public virtual void Initialize(DataSource dataSource) {
			this.InitializeFromDataSource(dataSource);
			this.SubscribeSolidifier();
		}
		public virtual void InitializeFromDataSource(DataSource dataSource) {
			this.DataSource = dataSource;
			this.StreamingDataSnapshot.InitializeLastQuoteReceived(this.DataSource.Symbols);
		}
		protected void SubscribeSolidifier() {
			//SOLIDIFIER_REPLACES_STATIC_PROVIDER_TO_STORE_REALTIME_QUOTES if (this.DataSource.StaticProvider == null) return;
			this.StreamingSolidifier.Initialize(this.DataSource);
			foreach (string symbol in this.DataSource.Symbols) {
				string ident = "[" + this.StreamingSolidifier.ToString()
					+ "] <= " + this.DataSource.Name + " :: " + symbol + " (" + this.DataSource.ScaleInterval + ")";

				if (this.ConsumerBarIsSubscribed(symbol, this.DataSource.ScaleInterval, this.StreamingSolidifier) == true) {
					string msg = "WONT_DUPLICATE_SUBSCRIPTION_SOLIDIFIER_BARS " + ident;
					Assembler.PopupException(msg, null, false);
				} else {
					this.ConsumerBarSubscribe(symbol, this.DataSource.ScaleInterval, this.StreamingSolidifier);
					string msg = "SUBSCRIBED_SOLIDIFIER_BARS " + ident;
					//Assembler.PopupException(msg, null, false);
				}

				if (this.ConsumerQuoteIsSubscribed(symbol, this.DataSource.ScaleInterval, this.StreamingSolidifier) == true) {
					string msg = "WONT_DUPLICATE_SUBSCRIPTION_SOLIDIFIER_QUOTES " + ident;
					Assembler.PopupException(msg, null, false);
				} else {
					this.ConsumerQuoteSubscribe(symbol, this.DataSource.ScaleInterval, this.StreamingSolidifier);
					string msg = "SUBSCRIBED_SOLIDIFIER_QUOTES " + ident;
					//Assembler.PopupException(msg, null, false);
				}
			}
		}

		#region the essence#1 of streaming provider
		public virtual void UpstreamConnect() {
			//StatusReporter.UpdateConnectionStatus(ConnectionState.ErrorConnecting, 0, "ConnectStreaming(): NOT_OVERRIDEN_IN_CHILD");
			Assembler.DisplayStatus("ConnectStreaming(): NOT_OVERRIDEN_IN_CHILD " + this.ToString());
		}
		public virtual void UpstreamDisconnect() {
			//StatusReporter.UpdateConnectionStatus(ConnectionState.ErrorDisconnecting, 0, "DisconnectStreaming(): NOT_OVERRIDEN_IN_CHILD");
			Assembler.DisplayStatus("DisconnectStreaming(): NOT_OVERRIDEN_IN_CHILD " + this.ToString());
		}
		#endregion

		#region the essence#2 of streaming provider
		public virtual void UpstreamSubscribe(string symbol) {
			throw new Exception("please override StreamingProvider::UpstreamSubscribe()");
			//CHILDRENT_TEMPLATE: base.UpstreamSubscribeRegistryHelper(symbol);
		}
		public virtual void UpstreamUnSubscribe(string symbol) {
			throw new Exception("please override StreamingProvider::UpstreamUnSubscribe()");
			//CHILDRENT_TEMPLATE: base.UpstreamUnSubscribeRegistryHelper(symbol);
		}
		public virtual bool UpstreamIsSubscribed(string symbol) {
			throw new Exception("please override StreamingProvider::UpstreamIsSubscribed()");
			//CHILDRENT_TEMPLATE: return base.UpstreamIsSubscribedRegistryHelper(symbol);
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

		#region overridable proxy methods routed by default to DataDistributor
		public virtual void ConsumerBarSubscribe(string symbol, BarScaleInterval scaleInterval, IStreamingConsumer consumer, bool quotePumpSeparatePushingThreadEnabled = true) {
			if (scaleInterval.Scale == BarScale.Unknown) {
				string msg = "Failed to ConsumerBarRegister(): scaleInterval.Scale=Unknown; returning";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			bool alreadyRegistered = this.ConsumerBarIsSubscribed(symbol, scaleInterval, consumer);
			if (alreadyRegistered) {
				string msg = "AVOIDING_MULTIPLE_SUBSCRIPTION BAR_CONSUMER_ALREADY_REGISTERED[" + consumer.ToString() + "] symbol[" + symbol + "] scaleInterval[" + scaleInterval.ToString() + "] ";
				Assembler.PopupException(msg, null, false);
				return;
			}

			this.DataDistributor.ConsumerBarSubscribe(symbol, scaleInterval, consumer, quotePumpSeparatePushingThreadEnabled);
		}
		public virtual void ConsumerBarUnSubscribe(string symbol, BarScaleInterval scaleInterval, IStreamingConsumer consumer) {
			this.DataDistributor.ConsumerBarUnSubscribe(symbol, scaleInterval, consumer);
		}
		public virtual bool ConsumerBarIsSubscribed(string symbol, BarScaleInterval scaleInterval, IStreamingConsumer consumer) {
			return this.DataDistributor.ConsumerBarIsSubscribed(symbol, scaleInterval, consumer);
		}
		#endregion

		#region overridable proxy methods routed by default to DataDistributor
		public virtual void ConsumerQuoteSubscribe(string symbol, BarScaleInterval scaleInterval, IStreamingConsumer consumer, bool quotePumpSeparatePushingThreadEnabled = true) {
			bool alreadyRegistered = this.ConsumerQuoteIsSubscribed(symbol, scaleInterval, consumer);
			if (alreadyRegistered) {
				string msg = "AVOIDING_MULTIPLE_SUBSCRIPTION QUOTE_CONSUMER_ALREADY_REGISTERED[" + consumer.ToString() + "] symbol[" + symbol + "] scaleInterval[" + scaleInterval.ToString() + "] ";
				Assembler.PopupException(msg, null, false);
				return;
			}
			this.DataDistributor.ConsumerQuoteSubscribe(symbol, scaleInterval, consumer, quotePumpSeparatePushingThreadEnabled);
			this.StreamingDataSnapshot.LastQuoteInitialize(symbol);
		}
		public virtual void ConsumerQuoteUnSubscribe(string symbol, BarScaleInterval scaleInterval, IStreamingConsumer streamingConsumer) {
			this.DataDistributor.ConsumerQuoteUnSubscribe(symbol, scaleInterval, streamingConsumer);
			this.StreamingDataSnapshot.LastQuoteInitialize(symbol);
		}
		public virtual bool ConsumerQuoteIsSubscribed(string symbol, BarScaleInterval scaleInterval, IStreamingConsumer consumer) {
			return this.DataDistributor.ConsumerQuoteIsSubscribed(symbol, scaleInterval, consumer);
		}
		//public virtual void ConsumerUnregisterDead(IStreamingConsumer streamingConsumer) {
		//    this.DataDistributor.ConsumerBarUnSubscribeDying(streamingConsumer);
		//    this.DataDistributor.ConsumerQuoteUnSubscribeDying(streamingConsumer);
		//}
		#endregion

		public virtual void PushQuoteReceived(Quote quote) {
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
				this.UpdateConnectionStatus(503, mainFormStatus);

				//string msg = "HACK!!! FILLING_LAST_BIDASK_FOR_DUPE_QUOTE_IS_UNJUSTIFIED: PREV_QUOTE_ABSNO_MUST_BE_LINEAR_WITHOUT_HOLES Backtester.generateQuotesForBarAndPokeStreaming()";
				//Assembler.PopupException(msg, null, false);
				//essence of PushQuoteReceived(); all above were pre-checks ensuring successfull completion of two lines below
				// ALREADY_ENRICHED this.EnrichQuoteWithStreamingDependantDataSnapshot(quote);
				//LOTS_OF_NOISE this.StreamingDataSnapshot.LastQuoteCloneSetForSymbol(quote);
				return;
			}

			int absnoPerSymbolNext = 0;

			Quote lastQuote = this.StreamingDataSnapshot.LastQuoteCloneGetForSymbol(quote.Symbol);
			if (lastQuote == null) {
				string msg = "RECEIVED_FIRST_QUOTE_EVER_FOR symbol[" + quote.Symbol + "] SKIPPING_LASTQUOTE_ABSNO_CHECK SKIPPING_QUOTE<=LASTQUOTE_NEXT_CHECK";
				Assembler.PopupException(msg, null, false);
			} else {
				if (lastQuote.AbsnoPerSymbol == -1) {
					string msg = "LAST_QUOTE_DIDNT_HAVE_ABSNO_SET_BY_STREAMING_PROVIDER_ON_PREV_ITERATION";
					Assembler.PopupException(msg, null, true);
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
					Assembler.PopupException(msg);
				}

				absnoPerSymbolNext = lastQuote.AbsnoPerSymbol + 1;
			}

			//QUOTE_ABSNO_MUST_BE_-1__HERE_NOT_MODIFIED_AFTER_QUOTE.CTOR()
			if (quote.AbsnoPerSymbol != -1) {
				string msg = "THIS_WAS_REFACTORED__QUOTE_ABSNO_MUST_BE_SEQUENTIAL_PER_SYMBOL__INITIALIZED_IN_STREAMING_PROVIDER";
				Assembler.PopupException(msg, null, true);

				//QUOTE_ABSNO_MUST_BE_SEQUENTIAL_PER_SYMBOL INITIALIZED_IN_STREAMING_PROVIDER
				//if (quote.AbsnoPerSymbol >= absnoPerSymbolNext) {
				//    string msg1 = "DONT_FEED_ME_WITH_SAME_QUOTE_BACKTESTER quote.Absno[" + quote.AbsnoPerSymbol + "] >= lastQuote.Absno[" + lastQuote.AbsnoPerSymbol + "] + 1";
				//    Assembler.PopupException(msg1, null, true);
				//}
			}
			quote.AbsnoPerSymbol = absnoPerSymbolNext;

			//OUTDATED?... BacktestStreamingProvider.EnrichGeneratedQuoteSaveSpreadInStreaming has updated lastQuote alredy...
			//essence of PushQuoteReceived(); all above were pre-checks ensuring successfull completion of two lines below
			// ALREADY_ENRICHED this.EnrichQuoteWithStreamingDependantDataSnapshot(quote);
			this.StreamingDataSnapshot.LastQuoteCloneSetForSymbol(quote.Clone());

			try {
				this.DataDistributor.PushQuoteToDistributionChannels(quote);
			} catch (Exception e) {
				string msg = "SOME_CONSUMERS_SOME_SCALEINTERVALS_FAILED_ON_ONE StreamingProvider.PushQuoteReceived()";
				Assembler.PopupException(msg, e);
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
		public void UpdateConnectionStatus(int code, string msg) {
			//StatusReporter.UpdateConnectionStatus(this.ConnectionState, code, msg);
			//StatusReporter.PopupException(msg, null, false);
			//Assembler.PopupException(this.ConnectionState + ", " + code + ", " + msg, null, false);
			Assembler.DisplayStatus(this.ConnectionState + ", " + code + ", " + msg);
		}
		public void InitializeStreamingOHLCVfromStreamingProvider(Bars chartBars) {
			SymbolScaleDistributionChannel distributionChannel = this.DataDistributor
				.GetDistributionChannelFor(chartBars.Symbol, chartBars.ScaleInterval);
			//v1 
			//Bar streamingBar = distributionChannel.StreamingBarFactoryUnattached.StreamingBarUnattached;
			Bar streamingBar = chartBars.BarStreaming;
			if (streamingBar == null) {
				string msg = "STREAMING_NEVER_STARTED BarFactory.StreamingBar=null for distributionChannel[" + distributionChannel + "]";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			if (streamingBar.DateTimeOpen == DateTime.MinValue) {
				string msg = "STREAMING_NEVER_STARTED streamingBar.DateTimeOpen=MinValue [" + streamingBar
					+ "] for distributionChannel[" + distributionChannel + "]";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			if (chartBars.BarStaticLastNullUnsafe == null) {
				// FAILED_FIXING_IN_DataDistributor
				string msg = "BAR_STATIC_LAST_IS_NULL while streamingBar[" + streamingBar
					+ "] for distributionChannel[" + distributionChannel + "]";
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
			string msg1 = "StreamingOHLCV Overwritten: Bars.StreamingBar[" + chartBars.BarStreamingCloneReadonly + "] taken from streamingBar[" + streamingBar + "]";
			//Assembler.PopupException(msg1, null, false);
		}
		public virtual void EnrichQuoteWithStreamingDependantDataSnapshot(Quote quote) {
			// in Market-dependant StreamingProviders, put in the Quote-derived quote anything like QuikQuote.FortsDepositBuy ;
		}

		public override string ToString() {
			return this.Name + "/[" + this.ConnectionState + "]: UpstreamSymbols[" + this.SymbolsUpstreamSubscribedAsString + "]";
		}

		internal void AbsorbStreamingBarFactoryFrom(StreamingProvider streamingBacktest, string symbol, BarScaleInterval barScaleInterval) {
			SymbolScaleDistributionChannel channelBacktest = streamingBacktest.DataDistributor.GetDistributionChannelFor(symbol, barScaleInterval);
			Bar barLastFormedBacktest = channelBacktest.StreamingBarFactoryUnattached.BarLastFormedUnattached;
			Bar barStreamingBacktest = channelBacktest.StreamingBarFactoryUnattached.BarStreamingUnattached;

			SymbolScaleDistributionChannel channelOriginal = this.DataDistributor.GetDistributionChannelFor(symbol, barScaleInterval);
			Bar barLastFormedOriginal = channelOriginal.StreamingBarFactoryUnattached.BarLastFormedUnattached;
			Bar barStreamingOriginal = channelOriginal.StreamingBarFactoryUnattached.BarStreamingUnattached;

			if (barLastFormedOriginal == null || barLastFormedOriginal.DateTimeOpen == DateTime.MinValue) {
				string msg = "SYNCHRONIZING_STREAMING_BAR_FACTORY_FOR_FIRST_TIME_BACKTEST";
				Assembler.PopupException(msg, null, false);
				channelOriginal.StreamingBarFactoryUnattached.AbsorbBarLastStaticFromChannel(channelBacktest);
				channelOriginal.StreamingBarFactoryUnattached.AbsorbBarStreamingFromChannel(channelBacktest);
			} else {
				string difference = "";
				if (barLastFormedOriginal.HasSameDOHLCVas(barLastFormedBacktest, "barLastBacktest", "barLastOriginal", ref difference)) {
					string msg = "SHOULD_NOT_BE_DIFFERENT_FOR_MID_LIVE_BACKTEST_SINCE_QUOTES_MUST_HAVE_BEEN_PAUSED " + difference;
					//Assembler.PopupException(msg);
				}
			}
		}
	}
}