using System;
using System.Collections.Generic;
using System.Drawing;

using Newtonsoft.Json;

using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Livesim;
using Sq1.Core.Charting;
using Sq1.Core.Backtesting;

namespace Sq1.Core.Streaming {
	public abstract partial class StreamingAdapter {
		[JsonIgnore]	public		string					Name								{ get; protected set; }
		[JsonIgnore]	public		string					ReasonToExist						{ get; protected set; }
		[JsonIgnore]	public		Bitmap					Icon								{ get; protected set; }
		[JsonIgnore]	public		StreamingConsumerSolidifier		StreamingSolidifier_oneForAllSymbols					{ get; protected set; }
		[JsonIgnore]	public		DataSource				DataSource							{ get; protected set; }
		[JsonIgnore]	public		string					marketName							{ get { return this.DataSource.MarketInfo.Name; } }
		[JsonIgnore]	public		DistributorCharts		DistributorCharts_substitutedDuringLivesim					{ get; protected set; }
		[JsonIgnore]	public		DistributorSolidifier	DistributorSolidifiers_substitutedDuringLivesim			{ get; protected set; }
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
		
		[JsonProperty]	public		int						Level2RefreshRateMs;
		[JsonIgnore]	public		bool					QuotePumpSeparatePushingThreadEnabled	{ get; protected set; }
		[JsonIgnore]	public		LivesimStreaming		LivesimStreaming_ownImplementation		{ get; protected set; }

		// public for assemblyLoader: Streaming-derived.CreateInstance();
		public StreamingAdapter() {
			ReasonToExist									= "DUMMY_FOR_LIST_OF_STREAMING_PROVIDERS_IN_DATASOURCE_EDITOR";
			SymbolsSubscribedLock							= new object();
			SymbolsUpstreamSubscribed						= new List<string>();

			StreamingDataSnapshot							= new StreamingDataSnapshot(this);			// not in DataSource because YOU may want to implement your own Level2, e.g. for Options (3D, containing multiple Strikes for one Symbol, to let the strategy choose "the best" strike for emiting Orders)
			StreamingSolidifier_oneForAllSymbols			= new StreamingConsumerSolidifier();		// not in DataSource because YOU may want to create a StreamingAdapter that doean't save incoming quotes directly to file, but YOU will save some crazy composite index / Synthetic-you-own Symbol (collected across multiple StreamingAdapters) separately

			Level2RefreshRateMs								= 200;
			QuotePumpSeparatePushingThreadEnabled			= true;		// set FALSE for Queue-based BacktestStreamingAdapter(s)
			LivesimStreaming_ownImplementation				= null;		// be careful addressing it! valid only for StreamingAdapter-derived!!!
			//if (this is LivesimStreaming) return;
			//NULL_UNTIL_QUIK_PROVIDES_OWN_DDE_REDIRECTOR LivesimStreamingImplementation					= new LivesimStreamingDefault(true, "USED_FOR_LIVESIM_ON_DATASOURCES_WITHOUT_ASSIGNED_STREAMING");	// QuikStreaming replaces it to DdeGenerator + QuikPuppet
		}

		public StreamingAdapter(string reasonToExist) : this() {
			ReasonToExist									= reasonToExist;
			this.CreateDistributors_onlyWhenNecessary(reasonToExist);
		}

		public void CreateDistributors_onlyWhenNecessary(string reasonToExist) {
			if (this.DistributorCharts_substitutedDuringLivesim == null) {
				this.DistributorCharts_substitutedDuringLivesim			= new DistributorCharts(this, reasonToExist);
			} else {
				//this.Distributor_replacedForLivesim.ForceUnsubscribeLeftovers_mustBeEmptyAlready(reasonToExist);
			}
			this.DistributorSolidifiers_substitutedDuringLivesim	= new DistributorSolidifier(this, reasonToExist);
		}
		public virtual void InitializeFromDataSource(DataSource dataSource) {
			this.DataSource = dataSource;
			this.StreamingDataSnapshot.Initialize_levelTwo_lastPrevQuotes_forAllSymbols_inDataSource(this.DataSource.Symbols);
			this.UpstreamConnectionState = ConnectionState.Streaming_JustInitialized_solidifiersUnsubscribed;
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

			//v1
			//SymbolScaleDistributionChannel channel = this.DistributorSolidifiers_replacedForLivesim.GetDistributionChannelFor_nullUnsafe(symbol, this.DataSource.ScaleInterval);
			//if (channel == null) {
			//	string msg = "NONSENSE";
			//	Assembler.PopupException(msg);
			//	return;
			//}
			//channel.QuotePump.UpdateThreadNameAfterMaxConsumersSubscribed = true;
			//MOVED_TO_SetQuotePumpThreadName_unpausePump_sinceNoMoreSubscribersWillFollowFor() channel.QuotePump.PusherUnpause();
			//v2
			this.DistributorSolidifiers_substitutedDuringLivesim.SetQuotePumpThreadName_sinceNoMoreSubscribersWillFollowFor(symbol);
		}
		//protected virtual void SolidifierAllSymbolsUnsubscribe(bool throwOnAlreadySubscribed = true) {
		//	List<string> oneSymbolImLivesimming = this.DataSource.Symbols;
		//	//List<string> oneSymbolImLivesimming = this.LivesimStreaming.DataSource.Symbols;
		//	foreach (string symbol in oneSymbolImLivesimming) {
		//		this.SolidifierUnsubscribeOneSymbol_imLivesimming(symbol);
		//	}
		//}

		//public void SolidifierUnsubscribeOneSymbol_imLivesimming(string symbol = null) {
		void solidifierUnsubscribe_oneSymbol_useMe_whenAddingRenamingSymbols_inDataSource(string symbol) {
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
		protected virtual int Quote_fixServerTime_absnoPerSymbol(Quote quote) {
			int changesMade = 0;
			string msig = " //StreamingAdapter.Quote_fixServerTime_absnoPerSymbol(" + quote + ")" + this.ToString();

			Quote quoteCurrent = this.StreamingDataSnapshot.GetQuoteCurrent_forSymbol_nullUnsafe(quote.Symbol);
			if (quoteCurrent == null) {
				string msg = "RECEIVED_FIRST_QUOTE_EVER_FOR#1 symbol[" + quote.Symbol + "] SKIPPING_LASTQUOTE_ABSNO_CHECK SKIPPING_QUOTE<=LASTQUOTE_NEXT_CHECK";
				Assembler.PopupException(msg + msig, null, false);
				return changesMade;
			}
			if (quote == quoteCurrent) {
				string msg = "DONT_FEED_STREAMING_WITH_SAME_QUOTE__NOT_FIXING_ANYTHING";
				Assembler.PopupException(msg + msig, null, false);
				return changesMade;
			}

			if (quote.IsGenerated) {
				string msg = "YES_ABSNO_IS_ZERO__SHOULD_SKIP_OTHER_CHECKS";
				//Assembler.PopupException(msg);
			} else {
				long absnoDiff_mustBe1 = quote.AbsnoPerSymbol - quoteCurrent.AbsnoPerSymbol;
				if (absnoDiff_mustBe1 <= 1) {
					string msg = "IT_WAS_QUOTE_INJECTED__BRUTEFORCING_TIRED WHERE_DID_YOU_SNEAK_EXTRA_ABSNO_INCREMENT??";
					//Assembler.PopupException(msg);
					quote.AbsnoPerSymbol = quoteCurrent.AbsnoPerSymbol + 1;
				}
			}

			if (quote.ServerTime == DateTime.MinValue) {		// spreadQuote arrived from DdeTableDepth without serverTime koz serverTime of Level2 changed is not transmitted over DDE
				if (this.Name.Contains("NOPE_TEST_REAL_QUIK_TOO___StreamingLivesim") == false) {	// QuikStreamingLivesim
					TimeSpan diff_inLocalTime = quote.LocalTime.Subtract(quoteCurrent.LocalTime);
					DateTime serverTime_reconstructed_fromLastQuote = quoteCurrent.ServerTime.Add(diff_inLocalTime);
					quote.ServerTime = serverTime_reconstructed_fromLastQuote;
					changesMade++;
				} else {
					if (quoteCurrent.ParentBarStreaming == null) {
						string msg = "WILL_BINDER_SET_QUOTE_TO_STREAMING_DATA_SNAPSHOT???";
						Assembler.PopupException(msg, null, false);
					} else {
						quote.ServerTime = quoteCurrent.ParentBarStreaming.ParentBars.MarketInfo.Convert_localTime_toServerTime(DateTime.Now);
						changesMade++;
					}
				}
			}
			if (quote.ServerTime == quoteCurrent.ServerTime) {
				// increase granularity of QuikQuotes (they will have the same ServerTime within the same second, while must have increasing milliseconds; I can't force QUIK print fractions of seconds via DDE export)
				TimeSpan diff_inLocalTime = quote.LocalTime.Subtract(quoteCurrent.LocalTime);
				// diff_localTime.Milliseconds will go to StreamingDataSnapshot with ServerTime fixed, and next diffMillis will be negative for the quote within same second
				int diffMillis_willBeNegative_forSecondQuote_duringSameSecond = diff_inLocalTime.Milliseconds;
				int diffMillis = Math.Abs(diffMillis_willBeNegative_forSecondQuote_duringSameSecond);
				if (diff_inLocalTime.Seconds == 0) {
					if (diffMillis == 0) {
						string msg = "ARE_WE_BACKTESTING_OR_LIVESIMMING_WITHOUT_INCREASING_SERVER_TIME_FOR_QUOTES_GENERATED?...";
						//Assembler.PopupException(msg, null, false);
					} else {
						quote.ServerTime = quote.ServerTime.AddMilliseconds(diffMillis);
						changesMade++;
					}
				}
			}

			long absnoPerSymbolNext = -1;
			if (quoteCurrent.AbsnoPerSymbol == -1) {
				string msg = "LAST_QUOTE_DIDNT_HAVE_ABSNO_SET_BY_STREAMING_ADAPDER_ON_PREV_ITERATION FORCING_ZERO";
				Assembler.PopupException(msg + msig, null, false);
				absnoPerSymbolNext = 0;
			} else {
				absnoPerSymbolNext = quoteCurrent.AbsnoPerSymbol + 1;	// you must see lock(){} upstack
			}

			if (quote.AbsnoPerSymbol == -1) {
				if (quote.IamInjectedToFillPendingAlerts) {
					//QUOTE_ABSNO_MUST_BE_-1__HERE_NOT_MODIFIED_AFTER_QUOTE.CTOR()
					string msg = "INJECTED_QUOTES_HAVE_AbsnoPerSymbol!=-1_AND_THIS_IS_NOT_AN_ERROR";
				} else {
					string msg1 = "THIS_WAS_REFACTORED__QUOTE_ABSNO_MUST_BE_SEQUENTIAL_PER_SYMBOL__INITIALIZED_IN_STREAMING_ADAPDER";
					//Assembler.PopupException(msg1 + msig, null, true);
					if (absnoPerSymbolNext == -1) {
						string msg = "I_REFUSE_TO_FIX quote.AbsnoPerSymbol[-1] && absnoPerSymbolNext[-1]";
						Assembler.PopupException(msg + msig, null, false);
					} else {
						quote.AbsnoPerSymbol = absnoPerSymbolNext;
						changesMade++;
					}
				}
			} else {
				//QUOTE_ABSNO_MUST_BE_SEQUENTIAL_PER_SYMBOL INITIALIZED_IN_STREAMING_ADAPTER
				if (absnoPerSymbolNext > -1 && absnoPerSymbolNext < quote.AbsnoPerSymbol && quoteCurrent.AbsnoPerSymbol != -1) {
					string msg = "DONT_FEED_ME_WITH_SAME_QUOTE_BACKTESTER quote.AbsnoPerSymbol[" + quote.AbsnoPerSymbol + "] MUST_BE_GREATER_THAN lastQuote.AbsnoPerSymbol[" + quoteCurrent.AbsnoPerSymbol + "]";
					Assembler.PopupException(msg + msig, null, false);
				}
			}
			return changesMade;
		}
		public virtual void PushQuoteReceived(Quote quoteUnboundUnattached) {
			string msig = " //StreamingAdapter.PushQuoteReceived()" + this.ToString();

			if (this.DistributorCharts_substitutedDuringLivesim.ChannelsBySymbol.Count == 0) {
				this.RaiseOnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_blinkDataSourceTreeWithOrange(quoteUnboundUnattached);

				string msg = "I_REFUSE_TO_PUSH_QUOTE NO_SOLIDIFIER_NOR_CHARTS_SUBSCRIBED";
				if (		this.LivesimStreaming_ownImplementation != null
						 && this.LivesimStreaming_ownImplementation.Livesimulator != null
						 && this.LivesimStreaming_ownImplementation.Livesimulator.ImRunningLivesim) {
					this.LivesimStreaming_ownImplementation.Livesimulator.AbortRunningBacktest_waitAborted(msg, 0);
				}
				if (this is LivesimStreaming) return;	//already reported "USER_DIDNT_CLICK_CHART>BARS>SUBSCRIBE"
				Assembler.PopupException(msg, null, false);
				//NO_TOO_MANY_CHANGES_TO_LOOSEN_ALL_CHECKS GO_AND_DO_IT__I_WILL_SEE_ORANGE_BACKGROUNG_IN_DATASOURCE_TREE
				return;
			}

			int changesMade = this.Quote_fixServerTime_absnoPerSymbol(quoteUnboundUnattached);

			if (quoteUnboundUnattached.ServerTime == DateTime.MinValue) {
				//v1 quote.ServerTime = this.DataSource.MarketInfo.ConvertLocalTimeToServer(DateTime.Now);
				string msg = "IM_NOT_PUSHING_FURTHER SERVER_TIME_HAS_TO_BE_FILLED_BY_STREAMING_DERIVED";
				Assembler.PopupException(msg + msig, null, false);
				return;
			}

			Quote lastQuote = this.StreamingDataSnapshot.GetQuoteCurrent_forSymbol_nullUnsafe(quoteUnboundUnattached.Symbol);
			if (lastQuote == null) {
				string msg = "I_DONT_WANT_TO_DELIVER_FIRST_EVER_QUOTE_TO_STRATEGY_AND_SOLIDIFIERS QUIK_JUST_CONNECTED_AND_SENDS_NONSENSE[" + quoteUnboundUnattached + "]";
				Assembler.PopupException(msg, null, false);
				this.StreamingDataSnapshot.SetQuoteCurrent_forSymbol_shiftOldToQuotePrev(quoteUnboundUnattached);
				return;
			}

			//v1 HAS_NO_MILLISECONDS_FROM_QUIK if (quote.ServerTime > lastQuote.ServerTime) {
			//v2 TOO_SENSITIVE_PRINTED_SAME_MILLISECONDS_BUT_STILL_DIFFERENT if (quote.ServerTime.Ticks > lastQuote.ServerTime.Ticks) {
			string quoteMillis		= quoteUnboundUnattached.ServerTime.ToString("HH:mm:ss.fff");
			string quoteLastMillis  = lastQuote.ServerTime.ToString("HH:mm:ss.fff");
			if (quoteMillis == quoteLastMillis) {
				string msg = quoteUnboundUnattached.Symbol + " SERVER_TIMESTAMP_MUST_INCREASE_EACH_NEXT_INCOMING_QUOTE QUIK_OR_BACKTESTER_FORGOT_TO_INCREASE"
					+ " quoteMillis[" + quoteMillis + "] <="
					+ " quoteLastMillis[" + quoteLastMillis + "]"
					+ ": DDE lagged somewhere?..."
					;
				Assembler.PopupException(msg + msig, null, false);
				return;
			}


			string reasonMarketIsClosedNow  = this.DataSource.MarketInfo.GetReason_ifMarket_closedOrSuspended_at(quoteUnboundUnattached.ServerTime);
			if (string.IsNullOrEmpty(reasonMarketIsClosedNow) == false) {
				string msg = "[" + this.DataSource.MarketInfo.Name + "]NOT_PUSHING_QUOTE " + reasonMarketIsClosedNow + " quote=[" + quoteUnboundUnattached + "]";
				Assembler.PopupException(msg + msig, null, false);
				Assembler.DisplayStatus(msg);
				return;
			}

			this.StreamingDataSnapshot.SetQuoteCurrent_forSymbol_shiftOldToQuotePrev(quoteUnboundUnattached);

			try {
				this.DistributorCharts_substitutedDuringLivesim			.Push_quoteUnboundUnattached_toChannel(quoteUnboundUnattached);
			} catch (Exception ex) {
				string msg = "CHART_OR_STRATEGY__FAILED_INSIDE"
					+ " Distributor.PushQuoteToDistributionChannels(" + quoteUnboundUnattached + ")";
				Assembler.PopupException(msg + msig, ex);
			}

			if (this.DistributorSolidifiers_substitutedDuringLivesim == null) {
				if (this is Backtesting.BacktestStreaming) {
					string msg = "YES_I_NULLIFY_SOLIDIFIERS_IN_BACKTEST_STREAMING";
					//Assembler.PopupException(msg, null, false);
				} else {
					string msg = "ADD_THE_CASE_HERE_FOR_NULL_DISTRIBUTOR_SOLIDIFIERS";
					Assembler.PopupException(msg);
				}
				return;
			}

			string symbol = quoteUnboundUnattached.Symbol;
			SymbolChannel<StreamingConsumerSolidifier> channelForSymbol = this.DistributorSolidifiers_substitutedDuringLivesim.GetChannelFor_nullMeansWasntSubscribed(symbol);
			bool okayForDistribSolidifiers_toBe_empty = this.DistributorSolidifiers_substitutedDuringLivesim.ReasonIwasCreated.Contains(Distributor<StreamingConsumerSolidifier>.SUBSTITUTED_LIVESIM_STARTED);
			if (channelForSymbol == null) {
				if (okayForDistribSolidifiers_toBe_empty) return;
				string msg = "YOUR_BARS_ARE_NOT_SAVED__SOLIDIFIERS_ARE_NOT_SUBSCRIBED_TO symbol[" + symbol + "]";
				Assembler.PopupException(msg);
				return;
			}
			try {
				this.DistributorSolidifiers_substitutedDuringLivesim	.Push_quoteUnboundUnattached_toChannel(quoteUnboundUnattached);
			} catch (Exception ex) {
				string msg = "SOLIDIFIER__FAILED_INSIDE"
					+ " DistributorSolidifiers.PushQuoteToDistributionChannels(" + quoteUnboundUnattached + ")";
				Assembler.PopupException(msg + msig, ex);
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
			Quote lastQuoteReceived = this.StreamingDataSnapshot.GetQuoteCurrent_forSymbol_nullUnsafe(symbol);
			foreach (var channel in channels) {
				channel.UpstreamUnSubscribedFromSymbolPokeConsumers(symbol, lastQuoteReceived);
			}
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

		//internal void FactoryStreamingBar_absorbFromStream_onBacktestComplete(StreamingAdapter streamingBacktest, string symbol, BarScaleInterval barScaleInterval) {
		//    SymbolScaleStream<StreamingConsumerChart> streamBacktest = streamingBacktest.DistributorCharts_substitutedDuringLivesim.GetStreamFor_nullUnsafe(symbol, barScaleInterval);
		//    if (streamBacktest == null) return;
		//    Bar barLastFormedBacktest = streamBacktest.UnattachedStreamingBar_factoryPerSymbolScale.BarLastFormedUnattached_nullUnsafe;
		//    if (barLastFormedBacktest == null) return;

		//    Bar barStreamingBacktest = streamBacktest.UnattachedStreamingBar_factoryPerSymbolScale.BarStreaming_unattached;

		//    SymbolScaleStream<StreamingConsumerChart> streamOriginal = this.DistributorCharts_substitutedDuringLivesim.GetStreamFor_nullUnsafe(symbol, barScaleInterval);
		//    if (streamOriginal == null) return;
		//    Bar barLastFormedOriginal = streamOriginal.UnattachedStreamingBar_factoryPerSymbolScale.BarLastFormedUnattached_nullUnsafe;
		//    //if (barLastFormedOriginal == null) return;

		//    streamOriginal.UnattachedStreamingBar_factoryPerSymbolScale.BarLastStatic_absorbFromStream_onBacktestComplete(streamBacktest);
		//    Bar barLastFormedAbsorbed = streamOriginal.UnattachedStreamingBar_factoryPerSymbolScale.BarLastFormedUnattached_nullUnsafe;
		//    if (barLastFormedOriginal == null || barLastFormedAbsorbed.DateTimeOpen != barLastFormedOriginal.DateTimeOpen) {
		//        string msg = "GUT";
		//    }

		//    Bar barStreamingOriginal = streamOriginal.UnattachedStreamingBar_factoryPerSymbolScale.BarStreaming_unattached;
		//    streamOriginal.UnattachedStreamingBar_factoryPerSymbolScale.BarStreaming_absorbFromStream_onBacktestComplete(streamBacktest);
		//    Bar barStreamingAbsorbed = streamOriginal.UnattachedStreamingBar_factoryPerSymbolScale.BarStreaming_unattached;
		//    if (barStreamingAbsorbed == null || barStreamingAbsorbed.DateTimeOpen != barStreamingOriginal.DateTimeOpen) {
		//        string msg = "GUT";
		//    }
		//    return;
		//}

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
		}
		internal void ChartStreamingConsumer_Unsubscribe(StreamingConsumerChart chartStreamingConsumer, string msigForNpExceptions) {
			if (this.DistributorCharts_substitutedDuringLivesim.ConsumerQuoteIsSubscribed(chartStreamingConsumer) == false) {
				string msg = "CHART_STREAMING_WASNT_SUBSCRIBED_CONSUMER_QUOTE";
				Assembler.PopupException(msg + msigForNpExceptions, null, true);
			} else {
				//Assembler.PopupException("UnSubscribing QuoteConsumer [" + this + "]  to " + plug + "  (was subscribed)");
				this.DistributorCharts_substitutedDuringLivesim.ConsumerQuoteUnsubscribe(chartStreamingConsumer);
			}

			if (this.DistributorCharts_substitutedDuringLivesim.ConsumerBarIsSubscribed(chartStreamingConsumer) == false) {
				string msg = "CHART_STREAMING_WASNT_SUBSCRIBED_CONSUMER_BAR";
				Assembler.PopupException(msg + msigForNpExceptions, null, true);
			} else {
				//Assembler.PopupException("UnSubscribing BarsConsumer [" + this + "] to " + this.ToString() + " (was subscribed)");
				this.DistributorCharts_substitutedDuringLivesim.ConsumerBarUnsubscribe(chartStreamingConsumer);
			}
		}

	}
} 