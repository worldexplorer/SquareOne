using System;
using System.Collections.Generic;

using Sq1.Adapters.QuikLivesim;
using Sq1.Adapters.Quik.Dde;
using Sq1.Adapters.QuikLiveism.Dde;
using Sq1.Core.DataTypes;
using Sq1.Core;
using Sq1.Core.Backtesting;

namespace Sq1.Adapters.QuikLivesim.Dde {
	public class DdeTableGeneratorQuotes : XlDdeTableGenerator {
		protected override string DdeGeneratorClassName { get { return "DdeTableGeneratorQuotes"; } }

		public DdeTableGeneratorQuotes(string topic, QuikLivesimStreaming quikLivesimStreaming) : base(topic, quikLivesimStreaming) {
			base.Initialize(TableDefinitions.XlColumnsForTable_Quotes);
		}

		public override void OutgoingObjectBufferize_eachRow(object quoteAsObject) {
			string msig = " //OutgoingObjectBufferizeAndSend(" + quoteAsObject + ")";
			if (quoteAsObject == null) {
				Assembler.PopupException("MUST_NOT_BE_NULL" + msig);
				return;
			}
			QuoteGenerated quote = quoteAsObject as QuoteGenerated;
			if (quote == null) {
				Assembler.PopupException("MUST_BE_A_QUOTE [" + quoteAsObject.GetType() + "]" + msig);
				return;
			}
			base.XlWriter.Put("SHORTNAME",	quote.Symbol);
			base.XlWriter.Put("CLASS_CODE",	quote.SymbolClass);
			base.XlWriter.Put("bid",		quote.Bid);
			//base.XlWriter.Put("biddepth",	quote.Symbol);
			base.XlWriter.Put("offer",		quote.Ask);
			//base.XlWriter.Put("offerdepth",	quote.Symbol);
			base.XlWriter.Put("last",		quote.TradedPrice);
			//base.XlWriter.Put("stepprice",	quote.Symbol);


			string dateFormat = base.ColumnsLookup["date"].ToDateTimeParseFormat;
			string timeFormat = base.ColumnsLookup["time"].ToDateTimeParseFormat;

			string date = quote.ServerTime.ToString(dateFormat);
			string time = quote.ServerTime.ToString(timeFormat);

			base.XlWriter.Put("date",		date);
			base.XlWriter.Put("time",		time);

			//base.XlWriter.Put("changetime",	quote.Symbol);
			//base.XlWriter.Put("selldepo",	quote.Symbol);
			//base.XlWriter.Put("buydepo",	quote.Symbol);
			base.XlWriter.Put("qty",		quote.Size);
			//base.XlWriter.Put("pricemin",	quote.Symbol);
			//base.XlWriter.Put("pricemax",	quote.Symbol);
			//base.XlWriter.Put("stepprice",	quote.Symbol);
		}

		internal byte[] GetXlDdeMessage() {
			return this.XlWriter.ConvertToXlDdeMessage();
		}
	}
}