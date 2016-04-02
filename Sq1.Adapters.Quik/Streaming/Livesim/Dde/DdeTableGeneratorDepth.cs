using System;
using System.Collections.Generic;

using NDde;
using NDde.Client;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Backtesting;
using Sq1.Core.Streaming;
using Sq1.Core.Support;

using Sq1.Adapters.Quik.Streaming.Dde;
using Sq1.Adapters.Quik.Streaming.Livesim.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Livesim.Dde {
	public class DdeTableGeneratorDepth : XlDdeTableGenerator {
		protected override string DdeGeneratorClassName { get { return "DdeTableGeneratorDepth"; } }

		public DdeTableGeneratorDepth(string ddeService, string ddeTopic, QuikStreamingLivesim quikLivesimStreaming) : base(ddeService, ddeTopic, quikLivesimStreaming, false) {
			base.Initialize(TableDefinitions.XlColumnsForTable_DepthOfMarketPerSymbol);
		}

		internal void OutgoingObjectsBufferize_perSymbol(LevelTwo levelTwo) {
			string msig = " //" + this.DdeGeneratorClassName + ".OutgoingObjectsBufferize_perSymbol("
				+ levelTwo.ToString() + ")";

			//foreach (double priceLevel in levelTwoAsks.InnerDictionary.Keys) {
			//	base.XlWriter.StartNewRow();	// first row was already added as a header
			//List<double> askKeys = new List<double>(levelTwoAsks.SafeCopy(this, msig).Keys);
			//for (int i=0; i<askKeys.Count; i++) {
			//	double priceLevel = askKeys[i];

			Dictionary<double, double> asks_safeCopy = levelTwo.Asks_safeCopy(this, msig);
			Dictionary<double, double> bids_safeCopy = levelTwo.Bids_safeCopy(this, msig);

			int serno = 0;
			foreach (KeyValuePair<double, double> volumeAskByPriceLevel in asks_safeCopy) {
				double priceLevel = volumeAskByPriceLevel.Key;
				double volumeAsk  = volumeAskByPriceLevel.Value;
				//double volumeAtPrice = levelTwoAsks.GetAtKey(priceLevel, this, msig);
				base.XlWriter.Put("BUY_VOLUME",		null);
				base.XlWriter.Put("PRICE",			priceLevel);
				base.XlWriter.Put("SELL_VOLUME",	volumeAsk);
				if (serno < asks_safeCopy.Count - 1) base.XlWriter.StartNewRow();
			}
			//foreach (double priceLevel in levelTwoBids.InnerDictionary.Keys) {
			foreach (KeyValuePair<double, double> volumeBidForPriceLevel in bids_safeCopy) {
				double priceLevel = volumeBidForPriceLevel.Key;
				double volumeBid  = volumeBidForPriceLevel.Value;
				base.XlWriter.StartNewRow();
				base.XlWriter.Put("BUY_VOLUME",		volumeBid);
				base.XlWriter.Put("PRICE",			priceLevel);
				base.XlWriter.Put("SELL_VOLUME",	null);
			}
		}

		internal void Send_DdeClientPokesDdeServer_waitServerProcessed(LevelTwo levelTwo) {
			base.OutgoingTableBegin();
			this.OutgoingObjectsBufferize_perSymbol(levelTwo);
			base.OutgoingTableTerminate();
			base.Send_DdeClientPokesDdeServer_asynControlledByLivesim("item-level2");
		}
		public override string ToString() {
			string ret = base.ToString();
			//ret += " " + this.DdeGeneratorClassName + "{Symbols[" + this.QuikStreamingLivesim.StreamingDataSnapshot.SymbolsSubscribedAndReceiving + "]";
			return ret;
		}
	}
}