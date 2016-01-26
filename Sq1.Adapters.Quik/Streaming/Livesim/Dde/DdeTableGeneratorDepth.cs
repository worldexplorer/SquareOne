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

		public DdeTableGeneratorDepth(string ddeService, string ddeTopic, QuikStreamingLivesim quikLivesimStreaming) : base(ddeService, ddeTopic, quikLivesimStreaming) {
			base.Initialize(TableDefinitions.XlColumnsForTable_DepthOfMarketPerSymbol);
		}

		internal void OutgoingObjectsBufferize_perSymbol(LevelTwoHalf levelTwoAsks, LevelTwoHalf levelTwoBids) {
			string msig = " //" + this.DdeGeneratorClassName + ".OutgoingObjectsBufferize_perSymbol(" + levelTwoAsks + "," + levelTwoBids + ")";

			//foreach (double priceLevel in levelTwoAsks.InnerDictionary.Keys) {
			//	base.XlWriter.StartNewRow();	// first row was already added as a header

			List<double> askKeys = new List<double>(levelTwoAsks.InnerDictionary.Keys);
			for (int i=0; i<askKeys.Count; i++) {
			    double priceLevel = askKeys[i];
				double volumeAtPrice = levelTwoAsks.InnerDictionary[priceLevel];
				base.XlWriter.Put("SELL_VOLUME",	null);
				base.XlWriter.Put("PRICE",			priceLevel);
				base.XlWriter.Put("BUY_VOLUME",		volumeAtPrice);
			    if (i < askKeys.Count - 1) base.XlWriter.StartNewRow();
			}
			foreach (double priceLevel in levelTwoBids.InnerDictionary.Keys) {
				base.XlWriter.StartNewRow();
				double volumeAtPrice = levelTwoBids.InnerDictionary[priceLevel];
				base.XlWriter.Put("SELL_VOLUME",	volumeAtPrice);
				base.XlWriter.Put("PRICE",			priceLevel);
				base.XlWriter.Put("BUY_VOLUME",		null);
			}
		}

		internal void Send_DdeClientPokesDdeServer_waitServerProcessed(LevelTwoHalf levelTwoAsks, LevelTwoHalf levelTwoBids) {
			base.OutgoingTableBegin();
			this.OutgoingObjectsBufferize_perSymbol(levelTwoAsks, levelTwoBids);
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