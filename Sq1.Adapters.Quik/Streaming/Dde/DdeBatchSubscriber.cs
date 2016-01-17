using System;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.DataTypes;

using Sq1.Adapters.Quik.Streaming.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Dde {
	public class DdeBatchSubscriber {
					QuikStreaming							quikStreamingAdapter;
		
		public		DdeTableQuotes							TableQuotes		{ get; protected set; }
		public		DdeTableTrades							TableTrades		{ get; protected set; }

					Dictionary<string, List<XlDdeTable>>	individualTablesBySymbol;
		public		List<string>							SymbolsHavingIndividualTables { get { return new List<string>(this.individualTablesBySymbol.Keys); } }
		
		public DdeBatchSubscriber(QuikStreaming streamingAdapter) {
			this.quikStreamingAdapter		= streamingAdapter;
			this.individualTablesBySymbol	= new Dictionary<string, List<XlDdeTable>>();
			this.tables_CommonForAllSymbols_Add();
		}
		public void TableIndividual_DepthOfMarket_ForSymbolAdd(string symbol) {
			if (this.individualTablesBySymbol.ContainsKey(symbol)) {
				string msg = "YOU_ALREADY_SUBSCRIBED_DOM_FOR_SYMBOL [" + symbol + "]";
				Assembler.PopupException(msg);
				return;
			}
			string domTopic = this.GetDomTopicForSymbol(symbol);
			DdeTableDepth channelDom	= new DdeTableDepth(domTopic, this.quikStreamingAdapter, TableDefinitions.XlColumnsForTable_DepthOfMarketPerSymbol, symbol);
			this.quikStreamingAdapter.DdeServer.TableAdd(domTopic, channelDom);
			this.individualTablesBySymbol.Add(symbol, new List<XlDdeTable>() { channelDom });
			channelDom.ReceivingDataDde = true;
		}

		public string GetDomTopicForSymbol(string symbol) {
			if (string.IsNullOrEmpty(symbol)) {
				Assembler.PopupException("SYMBOL_MUST_NOT_BE_NULL //GetDomTopicForSymbol(" + symbol + ")");
				return null;
			}
			return this.quikStreamingAdapter.DdeServiceName + "-" + this.quikStreamingAdapter.DdeTopicPrefixDom + "-" + symbol;
		}
		public void TableIndividual_DepthOfMarket_ForSymbolRemove(string symbol) {
			if (this.individualTablesBySymbol.ContainsKey(symbol) == false) return;
			List<XlDdeTable> tables = this.individualTablesBySymbol[symbol];
			if (tables == null) return;
			if (tables.Count == 0) return;
			foreach (XlDdeTable table in tables) {
				this.quikStreamingAdapter.DdeServer.TableRemove(table.Topic);
				table.ReceivingDataDde = false;
			}
		}
		public bool SymbolHasIndividualChannels(string symbol) {
			return this.individualTablesBySymbol.ContainsKey(symbol);
		}
		public string IndividualChannelsForSymbol(string symbol) {
			string ret = "Symbol[" + symbol + "]channels: ";
			if (this.SymbolHasIndividualChannels(symbol) == false) return ret + " NO_INDIVIDUAL_CHANNELS";
			List<XlDdeTable> individualTables = this.individualTablesBySymbol[symbol];
			if (individualTables == null) return ret + " NULL_INDIVIDUAL_CHANNELS";
			if (individualTables.Count == 0) return ret + " ZERO_INDIVIDUAL_CHANNELS";
			foreach (XlDdeTable table in individualTables) {
				ret += " " + table.ToString();
			}
			return ret;
		}
		public override string ToString() {
			string ret = "DdeServiceName[" + this.quikStreamingAdapter.DdeServiceName + "]/[" + this.quikStreamingAdapter.UpstreamConnectionState + "]:";
			if (this.TableQuotes != null)	ret += " " + this.TableQuotes.ToString() + " ";
			if (this.TableTrades != null)	ret += " " + this.TableTrades.ToString();
			string individualChannels = "";
			foreach (string symbol in this.SymbolsHavingIndividualTables) {
				ret += " {"
					//+ " Symbol[" + symbol + "]"
					+ this.IndividualChannelsForSymbol(symbol) + "}";
			}
			if (individualChannels == "") individualChannels = " NO_DEPTHS_OF_MARKET";
			return ret;
		}
		//public string TopicsAsString { get {
		//    string ret = "";
		//    ret +=		this.TableQuotes.Topic;
		//    ret += ","+ this.TableTrades.Topic;
		//    foreach (List<XlDdeTable> tables in this.individualTablesBySymbol.Values) {
		//        foreach (XlDdeTable table in tables) {
		//            ret += "," + table.Topic;
		//        }
		//    }
		//    return ret;
		//} }

		void tables_CommonForAllSymbols_Add() {
			this.TableQuotes = new DdeTableQuotes(this.quikStreamingAdapter.DdeServiceName + "-" + this.quikStreamingAdapter.DdeTopicQuotes
				, this.quikStreamingAdapter, TableDefinitions.XlColumnsForTable_Quotes);
			this.TableTrades = new DdeTableTrades(this.quikStreamingAdapter.DdeServiceName + "-" + this.quikStreamingAdapter.DdeTopicTrades
				, this.quikStreamingAdapter, TableDefinitions.XlColumnsForTable_Trades);

			this.quikStreamingAdapter.DdeServer.TableAdd(this.TableQuotes.Topic, this.TableQuotes);
			this.quikStreamingAdapter.DdeServer.TableAdd(this.TableTrades.Topic, this.TableTrades);
		}

		internal void AllDdeTablesReceivedCountersReset() {
			this.TableQuotes.DdeTablesReceived = 0;
			this.TableTrades.DdeTablesReceived = 0;
			foreach (List<XlDdeTable> tablesPerSymbol in this.individualTablesBySymbol.Values) {
				foreach (XlDdeTable eachTable in tablesPerSymbol) {
					eachTable.DdeTablesReceived = 0;
				}
			}
		}
		internal long AllDdeTablesReceivedCountersTotal { get {
			long ret = 0;
			ret += this.TableQuotes.DdeTablesReceived;
			ret += this.TableTrades.DdeTablesReceived;
			foreach (List<XlDdeTable> tablesPerSymbol in this.individualTablesBySymbol.Values) {
				foreach (XlDdeTable eachTable in tablesPerSymbol) {
					ret += eachTable.DdeTablesReceived;
				}
			}
			return ret;
		} }
	}
}
