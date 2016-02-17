using System;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Widgets.Level2;

using Sq1.Adapters.Quik.Streaming.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Dde {
	public partial class DdeBatchSubscriber {
					QuikStreaming							quikStreamingAdapter;
		
		public		DdeTableQuotes							TableQuotes		{ get; protected set; }
		public		DdeTableTrades							TableTrades		{ get; protected set; }

					Dictionary<string, XlDdeTableMonitoreable<LevelTwoOlv>>	level2BySymbol;
		public		List<string>							SymbolsDOMsSubscribed { get { return new List<string>(this.level2BySymbol.Keys); } }
		
		public DdeBatchSubscriber(QuikStreaming streamingAdapter) {
			this.quikStreamingAdapter		= streamingAdapter;
			this.level2BySymbol	= new Dictionary<string, XlDdeTableMonitoreable<LevelTwoOlv>>();
			//this.Tables_CommonForAllSymbols_Add();
		}
		public string GetDomTopicForSymbol(string symbol) {
			if (string.IsNullOrEmpty(symbol)) {
				Assembler.PopupException("SYMBOL_MUST_NOT_BE_NULL //GetDomTopicForSymbol(" + symbol + ")");
				return null;
			}
			return this.quikStreamingAdapter.DdeServiceName + "-" + symbol + "-" + this.quikStreamingAdapter.DdeTopicSuffixDom;
		}
		public void TableIndividual_DepthOfMarket_ForSymbolAdd(string symbol) {
			if (this.level2BySymbol.ContainsKey(symbol)) {
				string msg = "YOU_ALREADY_SUBSCRIBED_DOM_FOR_SYMBOL [" + symbol + "]";
				Assembler.PopupException(msg);
				return;
			}
			string domTopic = this.GetDomTopicForSymbol(symbol);
			DdeTableDepth ddeTableLevel2adding	= new DdeTableDepth(domTopic, this.quikStreamingAdapter, TableDefinitions.XlColumnsForTable_DepthOfMarketPerSymbol, symbol);
			this.quikStreamingAdapter.DdeServer.TableAdd(domTopic, ddeTableLevel2adding);
			this.level2BySymbol.Add(symbol, ddeTableLevel2adding);
			ddeTableLevel2adding.OnDataStructuresParsed_Table += new EventHandler<XlDdeTableMonitoringEventArg<List<LevelTwoOlv>>>(level2_OnDataStructuresParsed_Table_butAlwaysOneElementInList);
			this.quikStreamingAdapter.MonitorForm.QuikStreamingMonitorControl.DomUserControl_createAddFor(ddeTableLevel2adding);
		}
		public void TableIndividual_DepthOfMarket_ForSymbolRemove(string symbol) {
			string msig = " //TableIndividual_DepthOfMarket_ForSymbolRemove()";
			if (this.level2BySymbol.ContainsKey(symbol) == false) return;
			XlDdeTableMonitoreable<LevelTwoOlv> ddeTableLevel2removing = this.level2BySymbol[symbol];
			if (ddeTableLevel2removing == null) {
				string msg = "MUST_BE_NOT_NULL this.level2BySymbol[" + symbol + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			DdeTableDepth ddeTableLevel2removing_asDdeTableDepth = ddeTableLevel2removing as DdeTableDepth;
			if (ddeTableLevel2removing_asDdeTableDepth == null) {
				string msg = "MUST_BE_OF_TYPE_DdeTableDepth this.level2BySymbol[" + symbol + "].GetType()=[" + ddeTableLevel2removing.GetType() + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (ddeTableLevel2removing_asDdeTableDepth == null) return;
			ddeTableLevel2removing_asDdeTableDepth.OnDataStructuresParsed_Table -= new EventHandler<XlDdeTableMonitoringEventArg<List<LevelTwoOlv>>>(level2_OnDataStructuresParsed_Table_butAlwaysOneElementInList);
			this.quikStreamingAdapter.DdeServer.TableRemove(ddeTableLevel2removing_asDdeTableDepth.Topic);
			this.quikStreamingAdapter.MonitorForm.QuikStreamingMonitorControl.DomUserControl_deleteFor(ddeTableLevel2removing_asDdeTableDepth);
			//WILL_IT_USE_ManualResetEvent(s)? level2.Dispose();
		}
		public bool SymbolIsSubscribedForLevel2(string symbol) {
			return this.level2BySymbol.ContainsKey(symbol);
		}
		public string Level2ForSymbol(string symbol) {
			string ret = "Symbol[" + symbol + "]channels: ";
			if (this.SymbolIsSubscribedForLevel2(symbol) == false) return ret + " NO_INDIVIDUAL_CHANNELS";
			XlDdeTableMonitoreable<LevelTwoOlv> level2 = this.level2BySymbol[symbol];
			if (level2 == null) return ret + " NULL_INDIVIDUAL_CHANNELS";
			ret += " " + level2.ToString();
			return ret;
		}
		public override string ToString() {
			string ret = "DdeServiceName[" + this.quikStreamingAdapter.DdeServiceName + "]/[" + this.quikStreamingAdapter.UpstreamConnectionState + "]:";
			if (this.TableQuotes != null)	ret += " " + this.TableQuotes.ToString() + " ";
			if (this.TableTrades != null)	ret += " " + this.TableTrades.ToString();
			string individualChannels = "";
			foreach (string symbol in this.SymbolsDOMsSubscribed) {
				individualChannels += " {"
					//+ " Symbol[" + symbol + "]"
					+ this.Level2ForSymbol(symbol) + "}";
			}
			if (individualChannels == "") individualChannels = " NO_DEPTHS_OF_MARKET";
			return ret + " " + individualChannels;
		}
		public string WindowTitle { get {
			string ret = "DdeServiceName[" + this.quikStreamingAdapter.DdeServiceName + "]/[" + this.quikStreamingAdapter.UpstreamConnectionState + "] ";
			ret += " " + this.AllDdeMessagesReceivedCounters_total;
			ret += ":" + this.AllDdeRowsReceivedCounters_total;
			return ret;
		} }
		public string DomGroupboxTitle { get {
			string ret = "";
			ret += " Symbols[" + this.quikStreamingAdapter.StreamingDataSnapshot.SymbolsSubscribedAndReceiving + "]";
			ret += " " + this.AllDOMsMessagesReceivedCounters_total;
			ret += ":" + this.AllDOMsRowsReceivedCounters_total;
			return ret;
		} }
		public void Tables_CommonForAllSymbols_Add() {
			this.TableQuotes = new DdeTableQuotes(this.quikStreamingAdapter.DdeServiceName + "-" + this.quikStreamingAdapter.DdeTopicQuotes
				, this.quikStreamingAdapter, TableDefinitions.XlColumnsForTable_Quotes);
			this.TableTrades = new DdeTableTrades(this.quikStreamingAdapter.DdeServiceName + "-" + this.quikStreamingAdapter.DdeTopicTrades
				, this.quikStreamingAdapter, TableDefinitions.XlColumnsForTable_Trades);

			this.quikStreamingAdapter.DdeServer.TableAdd(this.TableQuotes.Topic, this.TableQuotes);
			this.quikStreamingAdapter.DdeServer.TableAdd(this.TableTrades.Topic, this.TableTrades);
		}

		// EXPECT_NPE_AFTER_YOU_INVOKED_IT
		//public void Tables_CommonForAllSymbols_Remove() {
		//    this.quikStreamingAdapter.DdeServer.TableRemove(this.TableQuotes.Topic);	// and they are going to GarbageCollector?... only because user renamed one symbol?...
		//    this.quikStreamingAdapter.DdeServer.TableRemove(this.TableTrades.Topic);
		//}

		internal void AllDdeMessagesReceivedCounter_reset() {
			this.TableQuotes.ResetCounters();
			this.TableTrades.ResetCounters();
			foreach (XlDdeTableMonitoreable<LevelTwoOlv> eachLevel2 in this.level2BySymbol.Values) {
				eachLevel2.ResetCounters();
			}
		}
		internal long AllDdeMessagesReceivedCounters_total { get {
			long ret = 0;
			ret += this.TableQuotes.DdeMessagesReceived;	//MUST_NOT_BE_NULL_AFTER_I_MOVED_TO_CTOR_DdeBatchSubscriber.Tables_CommonForAllSymbols_Add()
			ret += this.TableTrades.DdeMessagesReceived;	//MUST_NOT_BE_NULL_AFTER_I_MOVED_TO_CTOR_DdeBatchSubscriber.Tables_CommonForAllSymbols_Add()
			ret += this.AllDOMsMessagesReceivedCounters_total;
			return ret;
		} }

		internal long AllDdeRowsReceivedCounters_total { get {
			long ret = 0;
			ret += this.TableQuotes.DdeRowsReceived;	//MUST_NOT_BE_NULL_AFTER_I_MOVED_TO_CTOR_DdeBatchSubscriber.Tables_CommonForAllSymbols_Add()
			ret += this.TableTrades.DdeRowsReceived;	//MUST_NOT_BE_NULL_AFTER_I_MOVED_TO_CTOR_DdeBatchSubscriber.Tables_CommonForAllSymbols_Add()
			ret += this.AllDOMsRowsReceivedCounters_total;
			return ret;
		} }

		public long AllDOMsMessagesReceivedCounters_total { get {
			long ret = 0;
			foreach (XlDdeTableMonitoreable<LevelTwoOlv> eachLevel2 in this.level2BySymbol.Values) {
				ret += eachLevel2.DdeMessagesReceived;
			}
			return ret;
		} }
		public long AllDOMsRowsReceivedCounters_total { get {
			long ret = 0;
			foreach (XlDdeTableMonitoreable<LevelTwoOlv> eachLevel2 in this.level2BySymbol.Values) {
				ret += eachLevel2.DdeRowsReceived;
			}
			return ret;
		} }
	}
}
