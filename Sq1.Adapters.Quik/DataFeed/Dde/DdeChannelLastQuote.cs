using System;
using System.Collections.Generic;

using Sq1.Adapters.Quik.Dde.XlDde;

namespace Sq1.Adapters.Quik.Dde {
	public class DdeChannelLastQuote : XlDdeChannel {
		protected StreamingQuik quikStreamingProvider;
		protected string quoteSource = "QUIK_DDE";
		protected DateTime lastQuoteDateTimeForVolume = DateTime.MinValue;
		protected double lastQuoteSizeForVolume = 0;

		public DdeChannelLastQuote(string topic, StreamingQuik quikStreamingProvider) : base(topic) {
			this.quikStreamingProvider = quikStreamingProvider;
			base.columns = new List<XlColumn>() {
				new XlColumn() { Name = "SHORTNAME", TypeExpected = XlTable.BlockType.String, Mandatory = true },
				new XlColumn() { Name = "CLASS_CODE", TypeExpected = XlTable.BlockType.String, Mandatory = true },
				new XlColumn() { Name = "bid", TypeExpected = XlTable.BlockType.Float, Mandatory = true },
				new XlColumn() { Name = "biddepth", TypeExpected = XlTable.BlockType.Float },
				new XlColumn() { Name = "offer", TypeExpected = XlTable.BlockType.Float, Mandatory = true },
				new XlColumn() { Name = "offerdepth", TypeExpected = XlTable.BlockType.Float },
				new XlColumn() { Name = "last", TypeExpected = XlTable.BlockType.Float, Mandatory = true },
				//new XlColumn() { Name = "realvmprice", TypeExpected = XlTable.BlockType.String },
				//new XlColumn() { Names = new List<string>() {"time", "changetime"}, Type = XlTable.BlockType.String, Format = "h:mm:sstt" },
				new XlColumn() { Name = "time", TypeExpected = XlTable.BlockType.String, ToDateTimeParseFormat = "h:mm:sstt", Mandatory = true },
				new XlColumn() { Name = "changetime", TypeExpected = XlTable.BlockType.String, ToDateTimeParseFormat = "h:mm:sstt" },
				new XlColumn() { Name = "selldepo", TypeExpected = XlTable.BlockType.Float },
				new XlColumn() { Name = "buydepo", TypeExpected = XlTable.BlockType.Float },
				new XlColumn() { Name = "qty", TypeExpected = XlTable.BlockType.Float, Mandatory = true },
				new XlColumn() { Name = "pricemin", TypeExpected = XlTable.BlockType.Float },
				new XlColumn() { Name = "pricemax", TypeExpected = XlTable.BlockType.Float },
				new XlColumn() { Name = "stepprice", TypeExpected = XlTable.BlockType.Float },
			};
		}
		protected override void processNonHeaderRowParsed(XlRowParsed rowParsed) {
			//if (rowParsed["SHORTNAME"] == "LKOH") {
			//	int a = 1;
			//}
			QuoteQuik quikQuote = new QuoteQuik(DateTime.Now);
			quikQuote.Source = this.quoteSource + " Topic[" + base.Topic + "]";
			quikQuote.Symbol = (string)rowParsed["SHORTNAME"];
			quikQuote.SymbolClass = (string)rowParsed["CLASS_CODE"];
			quikQuote.Bid = (double)rowParsed["bid"];
			quikQuote.Ask = (double)rowParsed["offer"];
			//quikQuote.PriceLastDeal = (double)rowParsed["last"];
			quikQuote.FortsDepositBuy = (double)rowParsed["buydepo"];
			quikQuote.FortsDepositSell = (double)rowParsed["selldepo"];
			quikQuote.FortsPriceMin = (double)rowParsed["pricemin"];
			quikQuote.FortsPriceMax = (double)rowParsed["pricemax"];

			quikQuote.ServerTime = (DateTime)rowParsed["time"];
			//DateTime qChangeTime = DateTime.MinValue;
			//if (quote.ServerTime == DateTime.MinValue && qChangeTime != DateTime.MinValue) {
			//	quote.ServerTime = qChangeTime;
			//}

			double sizeParsed = (double)rowParsed["qty"];
			if (lastQuoteDateTimeForVolume != quikQuote.ServerTime) {
				lastQuoteDateTimeForVolume = quikQuote.ServerTime;
				quikQuote.Size = sizeParsed;
			}
			//if (lastQuoteSizeForVolume != sizeParsed) {
			//	lastQuoteSizeForVolume = sizeParsed;
			//	quote.Size = sizeParsed;
			//}
			quikStreamingProvider.FilterAndDistributeDdeQuote(quikQuote);
		}
		public override string ToString() {
			return "ChannelQuotes{Symbols[" + quikStreamingProvider.StreamingDataSnapshot.SymbolsSubscribedAndReceiving + "] " + base.ToString() + "}";
		}
	}
}