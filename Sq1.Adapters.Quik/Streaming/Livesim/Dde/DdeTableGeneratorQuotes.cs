using System;
using System.Collections.Generic;

using NDde;
using NDde.Client;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Backtesting;

using Sq1.Adapters.Quik.Streaming.Dde;
using Sq1.Adapters.Quik.Streaming.Livesim.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Livesim.Dde {
	public class DdeTableGeneratorQuotes : XlDdeTableGenerator {
		protected override string DdeGeneratorClassName { get { return "DdeTableGeneratorQuotes"; } }

		public DdeTableGeneratorQuotes(string ddeService, string ddeTopic, QuikStreamingLivesim quikLivesimStreaming) : base(ddeService, ddeTopic, quikLivesimStreaming, true) {
			base.Initialize(TableDefinitions.XlColumnsForTable_Quotes);
		}

		public void OutgoingObjectBufferize_eachRow(object quoteAsObject) {
			string msig = " //" + this.DdeGeneratorClassName + ".OutgoingObjectBufferize_eachRow(" + quoteAsObject + ")";
			if (quoteAsObject == null) {
				Assembler.PopupException("MUST_NOT_BE_NULL" + msig);
				return;
			}
			QuoteGenerated quote = quoteAsObject as QuoteGenerated;
			if (quote == null) {
				Assembler.PopupException("MUST_BE_A_QuoteGenerated [" + quoteAsObject.GetType() + "]" + msig);
				return;
			}
			base.XlWriter.Put("CODE",		quote.Symbol);
			base.XlWriter.Put("CLASS_CODE",	quote.SymbolClass);
			base.XlWriter.Put("bid",		quote.Bid);
			//base.XlWriter.Put("biddepth",	quote.Symbol);
			base.XlWriter.Put("offer",		quote.Ask);
			//base.XlWriter.Put("offerdepth",	quote.Symbol);
			base.XlWriter.Put("last",		quote.TradedPrice);
			//base.XlWriter.Put("stepprice",	quote.Symbol);


			string dateFormat = base.ColumnsLookup["TRADE_DATE_CODE"]	.ToDateParseFormat;
			string timeFormat = base.ColumnsLookup["time"]				.ToTimeParseFormat;

			string date = quote.ServerTime.ToString(dateFormat);
			string time = quote.ServerTime.ToString(timeFormat);

			base.XlWriter.Put("TRADE_DATE_CODE",	date);
			base.XlWriter.Put("time",				time);

			//base.XlWriter.Put("changetime",		quote.Symbol);
			//base.XlWriter.Put("selldepo",			quote.Symbol);
			//base.XlWriter.Put("buydepo",			quote.Symbol);
			base.XlWriter.Put("qty",				quote.Size);
			//base.XlWriter.Put("pricemin",			quote.Symbol);
			//base.XlWriter.Put("pricemax",			quote.Symbol);
			//base.XlWriter.Put("stepprice",		quote.Symbol);
		}

		internal void Send_DdeClientPokesDdeServer_waitServerProcessed(QuoteGenerated quote) {
			base.OutgoingTableBegin();
			this.OutgoingObjectBufferize_eachRow(quote);
			base.OutgoingTableTerminate();
			base.Send_DdeClientPokesDdeServer_asynControlledByLivesim("item-quote");
		}
	}
}