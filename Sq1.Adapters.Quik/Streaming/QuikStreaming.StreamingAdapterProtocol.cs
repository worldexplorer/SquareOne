using System;
using System.Drawing;

using Newtonsoft.Json;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;

using Sq1.Adapters.Quik.Streaming.Dde;
using Sq1.Adapters.Quik.Streaming.Livesim;
using Sq1.Adapters.Quik.Streaming.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming {
	public partial class QuikStreaming : StreamingAdapter {
		public override void InitializeDataSource_inverse(DataSource dataSource, bool subscribeSolidifier = true) {
			base.Name					= "QuikStreaming";
			base.ReasonToExist			= "INSTANCE_FOR[" + dataSource.Name + "]";
			base.Level2RefreshRateMs	= this.DdeMonitorRefreshRateMs;	// DataSourceEditor.btnSave.click () will come here and RETURN if DdeServer has already been started => propagating user input before 

			if (base.LivesimStreaming_ownImplementation == null) {
				base.LivesimStreaming_ownImplementation	= new QuikStreamingLivesim("LIVESIM_OWN_IMPLEMENTATION " + base.ReasonToExist);
				//base.LivesimStreaming_ownImplementation.CreateDistributors_onlyWhenNecessary("USED_FOR_OWN_IMPLEMENTATION");
			} else {
				string msg = "ALREADY_INITIALIZED_OWN_DISTRIBUTOR MUST_NEVER_HAPPEN_BUT_CRITICAL_WHEN_IT_DOES";
				Assembler.PopupException(msg);
			}

			if (this.DdeServer != null) {
				string msg = "FIXME_IF_YOU_SEE_ME_NOT_AFTER_SAVING_DATASOURCE"
					+ " DO_YOU_NEED_TO_INITIALIZE_DATASOURCE_INVERSE?"
					//v1 + "QUIK_STREAMING_INITIALIZING_WITH_LIVESIM_DATASOURCE_AND_UNSUBSCRIBING_SOLIDIFIER__OR_RESTORING_BACK_AND_SUBSCRIBING"
					;
				Assembler.PopupException(msg, null, false);
				return;
			}

			this.DdeServer		= new XlDdeServer(this.DdeServiceName);	// MOVED_FROM_CTOR_TO_HAVE_QuikStreamingPuppet_PREFIX_SERVICE_AND_TOPICS DUMMY_STREAMING_ISNT_INITIALIZED_WITH_DATASOURCE_SO_IN_CTOR_IT_WOULD_HAVE_OCCUPIED_SERVICE_NAME_FOR_NO_USE
			if (this.DdeBatchSubscriber != null) {
				string msg = "RETHINK_INITIALIZATION_AND_DdeTables_LIFECYCLE";
				Assembler.PopupException(msg);
				this.DdeServerUnregister();
			} else {
				this.DdeBatchSubscriber = new DdeBatchSubscriber(this);
			}
			base.InitializeDataSource_inverse(dataSource, subscribeSolidifier);
			//MOVED_TO_MainForm.WorkspaceLoad() this.Connect();
			// here definitely this Streaming was fully deserialized and LevelTwo hasn't started yet (at least after apprestart)

			//MOVED_TO_CTOR_DdeBatchSubscriber.Tables_CommonForAllSymbols_Add() from UpstreamConnect/Disconnect to avoid NPE in Monitor
			this.DdeBatchSubscriber.Tables_CommonForAllSymbols_Add();
		}

		public override void UpstreamConnect() { lock (base.SymbolsSubscribedLock) {
			if (base.UpstreamConnected == true) return;
			// MOVED_TO_CTOR_TO_AVOID_NPE_IN_MONITOR this.DdeBatchSubscriber.Tables_CommonForAllSymbols_Add();
			string symbolsSubscribed = this.upstreamSubscribeAllDataSourceSymbols();
			if (string.IsNullOrEmpty(symbolsSubscribed)) {
				string msg = "AREADY_STARTED_IN_SubstituteDistributorForSymbolsLivesimming_extractChartIntoSeparateDistributor()"
					+ " NO_SYMBOLS_IN_DATASORCE??? YOU_WERE_NOT_CONNECTED_BUT_AFTER_CONNECT_symbolsSubscribed[" + symbolsSubscribed + "]_EMPTY???"
					+ " this.DataSource.SymbolsCSV[" + this.DataSource.SymbolsCSV + "]"
					//+ " upstreamSubscribeAllDataSourceSymbols() didn't subscribe any symbol"
					;
				//Assembler.PopupException(msg, null, false);
			}
			this.DdeServerRegister();	// ConnectionState.UpstreamConnected_downstreamUnsubscribed;		// will result in StreamingConnected=true
			this.DdeBatchSubscriber.AllDdeMessagesReceivedCounter_reset();
			this.UpstreamConnectionState = ConnectionState.Streaming_UpstreamConnected_downstreamSubscribedAll;
			Assembler.DisplayConnectionStatus(base.UpstreamConnectionState, this.Name + " started DdeChannels[" + this.DdeBatchSubscriber.ToString() + "]");
		} }
		public override void UpstreamDisconnect() { lock (base.SymbolsSubscribedLock) {
			if (base.UpstreamConnected == false) {
				string msg = "HERE??? FIRST_LIVESIM_DOESNT_REMOVE_DEPTH_TABLE";
				Assembler.PopupException(msg);
				return;
			}
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms == false) {
				Assembler.PopupException("QUIK stopping DdeChannels[" + this.DdeBatchSubscriber.ToString() + "]", null, false);
			}
			string symbolsUnsubscribed = this.upstreamUnsubscribeAllDataSourceSymbols();
			if (string.IsNullOrEmpty(symbolsUnsubscribed)) {
				string msg = "NO_SYMBOLS_IN_DATASORCE??? YOU_WERE_CONNECTED_BUT_AFTER_DISCONNECT_symbolsUnsubscribed[" + symbolsUnsubscribed + "]_EMPTY???"
					+ " this.DataSource.SymbolsCSV[" + this.DataSource.SymbolsCSV + "]"
					//+ " upstreamSubscribeAllDataSourceSymbols() didn't subscribe any symbol"
					;
				Assembler.PopupException(msg, null, false);
			}
			this.UpstreamConnectionState = ConnectionState.Streaming_UpstreamConnected_downstreamUnsubscribedAll;
			Assembler.DisplayConnectionStatus(base.UpstreamConnectionState, this.Name + " symbolsUnsubscribedAll[" + symbolsUnsubscribed + "]");
			this.DdeServerUnregister();
			// MOVED_TO_CTOR_TO_AVOID_NPE_IN_MONITOR this.DdeBatchSubscriber.Tables_CommonForAllSymbols_Add();
			Assembler.DisplayConnectionStatus(base.UpstreamConnectionState, this.Name + " stopped DdeChannels[" + this.DdeBatchSubscriber.ToString() + "]");
		} }

		public override void UpstreamSubscribe(string symbol) { lock (base.SymbolsSubscribedLock) {
			if (string.IsNullOrEmpty(symbol)) {
				Assembler.PopupException("can't subscribe empty symbol=[" + symbol + "]; returning");
				return;
			}
			if (this.DdeBatchSubscriber.SymbolIsSubscribedForLevel2(symbol)) {
				String msg = "QUIK: ALREADY SymbolHasIndividualChannels(" + symbol + ")=[" + this.DdeBatchSubscriber.Level2ForSymbol(symbol) + "]";
				Assembler.PopupException(msg);
				//this.StatusReporter.UpdateConnectionStatus(ConnectionState.OK, 0, msg);
				return;
			}
			// NO_SERVER_ISNOT_STARTED_HERE_YET NB adding another DdeConversation into the registered DDE server - is NDDE capable of registering receiving topics on-the-fly?
			this.DdeBatchSubscriber.TableIndividual_DepthOfMarket_ForSymbolAdd(symbol);
			this.UpstreamConnectionState = this.UpstreamConnected
				?	ConnectionState.Streaming_UpstreamConnected_downstreamSubscribed
				: ConnectionState.Streaming_UpstreamDisconnected_downstreamSubscribed;
		} }
		public override void UpstreamUnSubscribe(string symbol) { lock (base.SymbolsSubscribedLock) {
			if (string.IsNullOrEmpty(symbol)) {
				Assembler.PopupException("can't unsubscribe empty symbol=[" + symbol + "]; returning");
				return;
			}
			if (this.DdeBatchSubscriber.SymbolIsSubscribedForLevel2(symbol) == false) {
				string errormsg = "QUIK: NOTHING TO REMOVE SymbolHasIndividualChannels(" + symbol + ")=[" + this.DdeBatchSubscriber.Level2ForSymbol(symbol) + "]";
				Assembler.PopupException(errormsg);
				return;
			}
			this.DdeBatchSubscriber.TableIndividual_DepthOfMarket_ForSymbolRemove(symbol);
			this.UpstreamConnectionState = this.UpstreamConnected
				?	ConnectionState.Streaming_UpstreamConnected_downstreamUnsubscribed
				: ConnectionState.Streaming_UpstreamDisconnected_downstreamUnsubscribed;
		} }
		public override bool UpstreamIsSubscribed(string symbol) { lock (base.SymbolsSubscribedLock) {
			if (String.IsNullOrEmpty(symbol)) {
				Assembler.PopupException("IsSubscribed() symbol=[" + symbol + "]=IsNullOrEmpty; returning");
				return false;
			}
			return this.DdeBatchSubscriber.SymbolIsSubscribedForLevel2(symbol);
		} }

		public override void PushQuoteReceived(Quote quoteQuk_unboundUnattached) {
			bool preMarket_orPreClearingResume = 
				//quote.Size == 0 ||
				quoteQuk_unboundUnattached.Bid == 0 ||
				quoteQuk_unboundUnattached.Ask == 0;
			if (preMarket_orPreClearingResume) {
				string msg = "SKIPPING_QUOTE_PRE_MARKET"
					//+ " since CHARTS will screw up painting price=0 and orders with LimitPrice=0 will be stuck forever;"
					+ " quote=[" + quoteQuk_unboundUnattached + "]";
				Assembler.PopupException(msg, null, false);
				return;
			}

			if (string.IsNullOrEmpty(quoteQuk_unboundUnattached.Source)) quoteQuk_unboundUnattached.Source = "Quik.SourceUnknown_PLEASE_ELABORATE";
			QuoteQuik quoteQuik = QuoteQuik.SafeUpcast(quoteQuk_unboundUnattached);
			this.StreamingDataSnapshotQuik.StoreFortsSpecifics(quoteQuik);
			this.syncSymbolClass_toSymbolInfo(quoteQuik);

			base.PushQuoteReceived(quoteQuk_unboundUnattached);
		}

		void syncSymbolClass_toSymbolInfo(QuoteQuik quoteQuik) {
			if (string.IsNullOrEmpty(quoteQuik.Symbol		)) return;
			if (string.IsNullOrEmpty(quoteQuik.SymbolClass	)) return;
			SymbolInfo symbolInfo = Assembler.InstanceInitialized.RepositorySymbolInfos.FindSymbolInfo_nullUnsafe(quoteQuik.Symbol);
			if (symbolInfo == null) return;
			if (symbolInfo.SymbolClass == quoteQuik.SymbolClass) return;
			symbolInfo.SymbolClass = quoteQuik.SymbolClass;
			Assembler.InstanceInitialized.RepositorySymbolInfos.Serialize();
		}

		public override void EnrichQuote_withStreamingDependant_dataSnapshot(Quote quote) {
			QuoteQuik quikQuote = QuoteQuik.SafeUpcast(quote);
			quikQuote.Enrich_fromStreamingDataSnapshotQuik(this.StreamingDataSnapshotQuik);
		}

		public override StreamingEditor StreamingEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.StreamingEditorInitializeHelper(dataSourceEditor);
			base.StreamingEditorInstance = new QuikStreamingEditorControl(this, dataSourceEditor);
			return base.StreamingEditorInstance;
		}

	}
}