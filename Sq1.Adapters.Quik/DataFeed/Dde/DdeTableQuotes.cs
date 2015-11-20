using System;
using System.Collections.Generic;

using Sq1.Adapters.Quik.Dde.XlDde;

namespace Sq1.Adapters.Quik.Dde {
	public class DdeTableQuotes : XlDdeTable {
		protected override string DdeConsumerClassName { get { return "DdeTableQuotes"; } }

		protected DateTime		lastQuoteDateTimeForVolume = DateTime.MinValue;
		protected double		lastQuoteSizeForVolume = 0;

		public DdeTableQuotes(string topic, QuikStreaming quikStreaming) : base(topic, quikStreaming) {
			base.Columns = TableDefinitions.XlColumnsForTable_Quotes;
		}
		protected override void IncomingRowParsedDelivered(XlRowParsed row) {
			//if (rowParsed["SHORTNAME"] == "LKOH") {
			//	int a = 1;
			//}
			QuoteQuik quikQuote = new QuoteQuik(DateTime.Now);
			quikQuote.Source			= this.DdeConsumerClassName + " Topic[" + base.Topic + "]";
			quikQuote.Symbol			= (string)row["SHORTNAME"];
			quikQuote.SymbolClass		= (string)row["CLASS_CODE"];
			quikQuote.Bid				= (double)row["bid"];
			quikQuote.Ask				= (double)row["offer"];
			//quikQuote.PriceLastDeal	= (double)rowParsed["last"];
			quikQuote.FortsDepositBuy	= (double)row["buydepo"];
			quikQuote.FortsDepositSell	= (double)row["selldepo"];
			quikQuote.FortsPriceMin		= (double)row["pricemin"];
			quikQuote.FortsPriceMax		= (double)row["pricemax"];

			quikQuote.ServerTime		= (DateTime)row["time"];
			//DateTime qChangeTime = DateTime.MinValue;
			//if (quote.ServerTime == DateTime.MinValue && qChangeTime != DateTime.MinValue) {
			//	quote.ServerTime = qChangeTime;
			//}

			double sizeParsed = (double)row["qty"];
			if (lastQuoteDateTimeForVolume != quikQuote.ServerTime) {
				lastQuoteDateTimeForVolume = quikQuote.ServerTime;
				quikQuote.Size = sizeParsed;
			}
			//if (lastQuoteSizeForVolume != sizeParsed) {
			//	lastQuoteSizeForVolume = sizeParsed;
			//	quote.Size = sizeParsed;
			//}
			base.QuikStreaming.PushQuoteReceived(quikQuote);
		}
	}
}