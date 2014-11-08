using System;
using System.Collections.Generic;
using System.Drawing;

using Newtonsoft.Json;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Support;
using System.Diagnostics;

namespace Sq1.Core.Streaming {
	// TODO: it's not an abstract class because....
	public partial class StreamingProvider {
		public const string NO_STREAMING_PROVIDER = "--- No Streaming Provider ---";
		
		[JsonIgnore]	public string Name { get; protected set; }
		[JsonIgnore]	public string Description { get; protected set; }
		[JsonIgnore]	public Bitmap Icon { get; protected set; }
		[JsonIgnore]	public bool IsConnected { get; protected set; }
		[JsonIgnore]	public string PreferredStaticProviderName { get; protected set; }
		[JsonIgnore]	public StreamingSolidifier StreamingSolidifier { get; protected set; }
		[JsonIgnore]	public DataSource DataSource;
		[JsonIgnore]	public string marketName { get { return this.DataSource.MarketInfo.Name; } }
		[JsonIgnore]	public IStatusReporter StatusReporter { get; protected set; }
		[JsonIgnore]	public DataDistributor DataDistributor { get; protected set; }
		[JsonProperty]	public StreamingDataSnapshot StreamingDataSnapshot { get; protected set; }
		[JsonIgnore]	public virtual List<string> SymbolsUpstreamSubscribed { get; private set; }
		[JsonIgnore]	protected Object SymbolsSubscribedLock = new Object();
		[JsonIgnore]	public virtual string SymbolsUpstreamSubscribedAsString { get {
				string ret = "";
				lock (SymbolsSubscribedLock) {
					foreach (string symbol in SymbolsUpstreamSubscribed) ret += symbol + ",";
				}
				ret = ret.TrimEnd(',');
				return ret;
			} }
		[JsonIgnore]	protected Object BarsConsumersLock = new Object();
		[JsonIgnore]	public bool SaveToStatic;
		[JsonIgnore]	public ConnectionState ConnectionState { get; protected set; }

		// public for assemblyLoader: Streaming-derived.CreateInstance();
		public StreamingProvider() {
			SymbolsUpstreamSubscribed = new List<string>();
			DataDistributor = new DataDistributor(this);
			StreamingDataSnapshot = new StreamingDataSnapshot(this);
		}
		public virtual void Initialize(DataSource dataSource, IStatusReporter statusReporter) {
			this.StatusReporter = statusReporter;
			this.InitializeFromDataSource(dataSource);
			this.SubscribeSolidifier();
		}
		public virtual void InitializeFromDataSource(DataSource dataSource) {
			this.DataSource = dataSource;
			this.StreamingDataSnapshot.InitializeLastQuoteReceived(this.DataSource.Symbols);
		}
		protected void SubscribeSolidifier() {
			if (this.DataSource.StaticProvider == null) return;
			this.StreamingSolidifier = new StreamingSolidifier(this.DataSource);
			foreach (string symbol in this.DataSource.Symbols) {
				string ident = "[" + this.StreamingSolidifier.ToString()
					+ "] <= " + this.DataSource.Name + " :: " + symbol + " (" + this.DataSource.ScaleInterval + ")";

				if (this.ConsumerBarIsRegistered(symbol, this.DataSource.ScaleInterval, this.StreamingSolidifier) == true) {
					string msg = "DUPLICATE_SUBSCRIPTION_BARS " + ident;
					Assembler.PopupException(msg, null, false);
				} else {
					string msg = "FIRST_SUBSCRIPTION_BARS " + ident;
					Assembler.PopupException(msg, null, false);
					this.ConsumerBarRegister(symbol, this.DataSource.ScaleInterval, this.StreamingSolidifier);
				}

				if (this.ConsumerQuoteIsRegistered(symbol, this.DataSource.ScaleInterval, this.StreamingSolidifier) == true) {
					string msg = "DUPLICATE_SUBSCRIPTION_QUOTE " + ident;
					Assembler.PopupException(msg, null, false);
				} else {
					string msg = "FIRST_SUBSCRIPTION_QUOTE " + ident;
					Assembler.PopupException(msg, null, false);
					this.ConsumerQuoteRegister(symbol, this.DataSource.ScaleInterval, this.StreamingSolidifier);
				}
			}
		}

		// the essence#1 of streaming provider
		public virtual void Connect() {
			StatusReporter.UpdateConnectionStatus(ConnectionState.ErrorConnecting, 0, "ConnectStreaming(): NOT_OVERRIDEN_IN_CHILD");
		}
		public virtual void Disconnect() {
			StatusReporter.UpdateConnectionStatus(ConnectionState.ErrorDisconnecting, 0, "DisconnectStreaming(): NOT_OVERRIDEN_IN_CHILD");
		}

		// BEGIN the essence#2 of streaming provider
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
		// END the essence#2 of streaming provider

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

		// overridable proxy methods routed by default to DataDistributor
		public virtual void ConsumerBarRegister(string symbol, BarScaleInterval scaleInterval, IStreamingConsumer consumer, Bar PartialBarInsteadOfEmpty) {
			this.ConsumerBarRegister(symbol, scaleInterval, consumer);
			SymbolScaleDistributionChannel channel = this.DataDistributor.GetDistributionChannelFor(symbol, scaleInterval);
			channel.StreamingBarFactoryUnattached.InitWithStreamingBarInsteadOfEmpty(PartialBarInsteadOfEmpty);
		}
		public virtual void ConsumerBarRegister(string symbol, BarScaleInterval scaleInterval, IStreamingConsumer consumer) {
			if (scaleInterval.Scale == BarScale.Unknown) {
				string msg = "Failed to ConsumerBarRegister(): scaleInterval.Scale=Unknown; returning";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			this.DataDistributor.ConsumerBarRegister(symbol, scaleInterval, consumer);
		}
		public virtual void ConsumerBarUnRegister(string symbol, BarScaleInterval scaleInterval, IStreamingConsumer consumer) {
			this.DataDistributor.ConsumerBarUnRegister(symbol, scaleInterval, consumer);
		}
		public virtual bool ConsumerBarIsRegistered(string symbol, BarScaleInterval scaleInterval, IStreamingConsumer consumer) {
			return this.DataDistributor.ConsumerBarIsRegistered(symbol, scaleInterval, consumer);
		}

		// overridable proxy methods routed by default to DataDistributor
		public virtual void ConsumerQuoteRegister(string symbol, BarScaleInterval scaleInterval, IStreamingConsumer streamingConsumer) {
			this.DataDistributor.ConsumerQuoteRegister(symbol, scaleInterval, streamingConsumer);
			this.StreamingDataSnapshot.LastQuotePutNull(symbol);
		}
		public virtual void ConsumerQuoteUnRegister(string symbol, BarScaleInterval scaleInterval, IStreamingConsumer streamingConsumer) {
			this.DataDistributor.ConsumerQuoteUnRegister(symbol, scaleInterval, streamingConsumer);
			this.StreamingDataSnapshot.LastQuotePutNull(symbol);
		}
		public virtual bool ConsumerQuoteIsRegistered(string symbol, BarScaleInterval scaleInterval, IStreamingConsumer consumer) {
			return this.DataDistributor.ConsumerQuoteIsRegistered(symbol, scaleInterval, consumer);
		}
		public virtual void ConsumerUnregisterDead(IStreamingConsumer streamingConsumer) {
			this.DataDistributor.ConsumerBarUnregisterDying(streamingConsumer);
			this.DataDistributor.ConsumerQuoteUnregisterDying(streamingConsumer);
		}

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

				string msg = "HACK!!! FILLING_LAST_BIDASK_FOR_DUPE_QUOTE_IS_UNJUSTIFIED: PREV_QUOTE_ABSNO_MUST_BE_LINEAR_WITHOUT_HOLES Backtester.generateQuotesForBarAndPokeStreaming()";
				#if DEBUG	// TEST_EMBEDDED
				//Debugger.Break();
				#endif
				Assembler.PopupException(msg, null, false);
				this.EnrichQuoteWithStreamingDependantDataSnapshot(quote);
				this.StreamingDataSnapshot.UpdateLastBidAskSnapFromQuote(quote);

				return;
			}

			Quote lastQuote = this.StreamingDataSnapshot.LastQuoteGetForSymbol(quote.Symbol);
			if (lastQuote == null) {
				string msg = "RECEIVED_FIRST_QUOTE_EVER_FOR symbol[" + quote.Symbol + "] NOTHING_TO_ADJUST_OR_TEST";
				//Assembler.PopupException(msg);
				//throw new Exception(msg);
			} else {
				if (quote.Absno != lastQuote.Absno + 1) {
					string msg = "DONT_FEED_ME_WITH_SAME_QUOTE_BACKTESTER quote.Absno[" + quote.Absno + "] != lastQuote.Absno[" + lastQuote.Absno + "] + 1";
					#if DEBUG
					//Debugger.Break();	// TEST_EMBEDDED
					#endif
					//Assembler.PopupException(msg);
				}
				quote.Absno = lastQuote.Absno + 1;

				//TODO WHAT_PRESICISION_DO_I_NEED_HERE?
				//v1 HAS_NO_MILLISECONDS_FROM_QUIK if (quote.ServerTime > lastQuote.ServerTime) {
				//v2 TOO_SENSITIVE_PRINTED_SAME_MILLISECONDS_BUT_STILL_DIFFERENT if (quote.ServerTime.Ticks > lastQuote.ServerTime.Ticks) {
				string quoteMillis = quote.ServerTime.ToString("HH:mm:ss.fff");
				string lastQuoteMillis = lastQuote.ServerTime.ToString("HH:mm:ss.fff");
				if (quoteMillis == lastQuoteMillis) {
					string msg = "DONT_FEED_ME_WITH_SAME_SERVER_TIME BACKTESTER_FORGOT_TO_INCREASE_SERVER_TIMESTAMP"
						+ " upcoming quote.LocalTimeCreatedMillis[" + quote.LocalTimeCreatedMillis.ToString("HH:mm:ss.fff")
						+ "] <= lastQuoteReceived.Symbol." + quote.Symbol + "["
						+ lastQuote.LocalTimeCreatedMillis.ToString("HH:mm:ss.fff") + "]: DDE lagged somewhere?...";
					Debugger.Break();
					Assembler.PopupException(msg);
				}
			}

			this.EnrichQuoteWithStreamingDependantDataSnapshot(quote);

			//BacktestStreamingProvider.EnrichGeneratedQuoteSaveSpreadInStreaming has updated lastQuote alredy...
			this.StreamingDataSnapshot.UpdateLastBidAskSnapFromQuote(quote);
			try {
				this.DataDistributor.PushQuoteToDistributionChannels(quote);
			} catch (Exception e) {
				string msg = "StreamingProvider.PushQuoteReceived()";
				Assembler.PopupException(msg, e);
				//throw e;
			}
		}

		public void UpdateConnectionStatus(int code, string msg) {
			if (this.StatusReporter == null) return;
			StatusReporter.UpdateConnectionStatus(this.ConnectionState, code, msg);
		}
		public void InitializeStreamingOHLCVfromStreamingProvider(Bars chartBars) {
			SymbolScaleDistributionChannel distributionChannel = this.DataDistributor
				.GetDistributionChannelFor(chartBars.Symbol, chartBars.ScaleInterval);
			Bar streamingBar = distributionChannel.StreamingBarFactoryUnattached.StreamingBarUnattached;
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
			chartBars.OverrideStreamingDOHLCVwith(streamingBar);
			string msg1 = "StreamingOHLCV Overwritten: Bars.StreamingBar[" + chartBars.BarStreamingCloneReadonly + "] taken from streamingBar[" + streamingBar + "]";
			Assembler.PopupException(msg1, null, false);
		}
		public virtual void EnrichQuoteWithStreamingDependantDataSnapshot(Quote quote) {
			// in Market-dependant StreamingProviders, put in the Quote-derived quote anything like QuikQuote.FortsDepositBuy ;
		}

		public override string ToString() {
			return Name + "/[" + this.ConnectionState + "]: UpstreamSymbols[" + this.SymbolsUpstreamSubscribedAsString + "]";
		}
	}
}