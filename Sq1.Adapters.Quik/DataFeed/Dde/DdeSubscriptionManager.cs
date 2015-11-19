using System;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.DataTypes;

using Sq1.Adapters.Quik.Dde.XlDde;

namespace Sq1.Adapters.Quik.Dde {
	public class DdeSubscriptionManager {
					QuikStreaming							quikStreamingAdapter;
		
		public		DdeTableQuotes							TableQuotes		{ get; protected set; }
		public		DdeTableTrades							TableTrades		{ get; protected set; }

					Dictionary<string, List<XlDdeTable>>	individualTablesBySymbol;
		public		List<string>							SymbolsHavingIndividualTables { get { return new List<string>(this.individualTablesBySymbol.Keys); } }
		
		public DdeSubscriptionManager(QuikStreaming streamingAdapter) {
			this.quikStreamingAdapter		= streamingAdapter;
			this.individualTablesBySymbol	= new Dictionary<string, List<XlDdeTable>>();
			this.tables_CommonForAllSymbols_Add();
		}
		public void TableIndividual_DepthOfMarket_ForSymbolAdd(string symbol) {
			if (this.individualTablesBySymbol.ContainsKey(symbol)) return;
			string domTopic				= this.quikStreamingAdapter.DdeServiceName + "-" + this.quikStreamingAdapter.DdeTopicPrefixDom + "-" + symbol;
			DdeTableDepth channelDom	= new DdeTableDepth(domTopic, this.quikStreamingAdapter, symbol);
			this.quikStreamingAdapter.DdeServer.TableAdd(domTopic, channelDom);
			this.individualTablesBySymbol.Add(symbol, new List<XlDdeTable>() { channelDom });
			channelDom.ReceivingDataDde = true;
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
			string ret = "DdeServiceName[" + this.quikStreamingAdapter.DdeServiceName + "]/[" + this.quikStreamingAdapter.ConnectionState + "]:";
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

		internal void tables_CommonForAllSymbols_Add() {
			this.TableQuotes = new DdeTableQuotes(this.quikStreamingAdapter.DdeServiceName + "-" + this.quikStreamingAdapter.DdeTopicQuotes, this.quikStreamingAdapter);
			this.TableTrades = new DdeTableTrades(this.quikStreamingAdapter.DdeServiceName + "-" + this.quikStreamingAdapter.DdeTopicTrades, this.quikStreamingAdapter);

			this.quikStreamingAdapter.DdeServer.TableAdd(this.TableQuotes.Topic, this.TableQuotes);
			this.quikStreamingAdapter.DdeServer.TableAdd(this.TableTrades.Topic, this.TableTrades);
		}

		public string TopicsAsString { get {
			string ret = "";
			ret +=		this.TableQuotes.Topic;
			ret += ","+ this.TableTrades.Topic;
			foreach (List<XlDdeTable> tables in this.individualTablesBySymbol.Values) {
				foreach (XlDdeTable table in tables) {
					ret += "," + table.Topic;
				}
			}
			return ret;
		} }
	}
}
