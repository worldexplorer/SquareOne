using System;
using System.Collections.Generic;
using System.Globalization;

using Sq1.Adapters.Quik.Dde.XlDde;
using Sq1.Core;
using Sq1.Core.DataTypes;

namespace Sq1.Adapters.Quik.Dde {
	public class DdeTableQuotes : XlDdeTable {
		protected override string DdeConsumerClassName { get { return "DdeTableQuotes"; } }

		protected DateTime		lastQuoteDateTimeForVolume = DateTime.MinValue;
		protected double		lastQuoteSizeForVolume = 0;

		public DdeTableQuotes(string topic, QuikStreaming quikStreaming, List<XlColumn> columns) : base(topic, quikStreaming, columns) {}

		protected override void IncomingRowParsedDelivered(XlRowParsed row) {
			//if (rowParsed["SHORTNAME"] == "LKOH") {
			//	int a = 1;
			//}

			foreach (string msg in row.ErrorMessages) {
				Assembler.PopupException(msg, null, false);
			}

			QuoteQuik quikQuote = new QuoteQuik(DateTime.Now);
			quikQuote.Source			= this.DdeConsumerClassName + " Topic[" + base.Topic + "]";
			quikQuote.Symbol			= row.GetString("SHORTNAME"		, "NO_SYMBOL_RECEIVED_DDE");
			quikQuote.SymbolClass		= row.GetString("CLASS_CODE"	, "NO_CLASS_CODE_RECEIVED_DDE");
			quikQuote.Bid				= row.GetDouble("bid"			, double.NaN);
			quikQuote.Ask				= row.GetDouble("offer"			, double.NaN);

			double	last				= row.GetDouble("last"			, double.NaN);
			if (last == quikQuote.Bid) quikQuote.TradedAt = BidOrAsk.Bid;
			if (last == quikQuote.Ask) quikQuote.TradedAt = BidOrAsk.Ask;
			if (quikQuote.TradedAt == BidOrAsk.UNKNOWN) {
				string msg = "ROUNDING_ERROR?... last must be bid or ask";
				Assembler.PopupException(msg);
			}

			quikQuote.FortsDepositBuy	= row.GetDouble("buydepo"		, double.NaN);
			quikQuote.FortsDepositSell	= row.GetDouble("selldepo"		, double.NaN);
			quikQuote.FortsPriceMin		= row.GetDouble("pricemin"		, double.NaN);
			quikQuote.FortsPriceMax		= row.GetDouble("pricemax"		, double.NaN);

			quikQuote.ServerTime		= row.GetDateTime("ServerTime"	, DateTime.Now);
			//DateTime qChangeTime = DateTime.MinValue;
			//if (quote.ServerTime == DateTime.MinValue && qChangeTime != DateTime.MinValue) {
			//	quote.ServerTime = qChangeTime;
			//}

			double sizeParsed			= row.GetDouble("qty"			, double.NaN);
			if (lastQuoteDateTimeForVolume != quikQuote.ServerTime) {
				lastQuoteDateTimeForVolume  = quikQuote.ServerTime;
				quikQuote.Size = sizeParsed;
			} else {
				string msg = "SHOULD_I_DELIVER_THE_DUPLIATE_QUOTE?";
				//Assembler.PopupException(msg, null, false);
				return;
			}
			//if (lastQuoteSizeForVolume != sizeParsed) {
			//	lastQuoteSizeForVolume = sizeParsed;
			//	quote.Size = sizeParsed;
			//}

			this.reconstructServerDate(row);

			base.QuikStreaming.PushQuoteReceived(quikQuote);
		}
		void reconstructServerDate(XlRowParsed rowParsed) {
			rowParsed["ServerTime"] = DateTime.MinValue;

			string dateReceived = rowParsed.GetString("date", "QUOTE_DATE_NOT_DELIVERED_DDE");
			string timeReceived = rowParsed.GetString("time", "QUOTE_TIME_NOT_DELIVERED_DDE");
			string dateTimeReceived = dateReceived + " " + timeReceived;
			if (dateTimeReceived.Contains("NOT_DELIVERED_DDE")) {
				return;
			}

			string dateFormat = base.ColumnsLookup["date"].ToDateTimeParseFormat;
			string timeFormat = base.ColumnsLookup["time"].ToDateTimeParseFormat;
			string dateTimeFormat = dateFormat + " " + timeFormat;


			try {
			    rowParsed["ServerTime"] = DateTime.ParseExact(dateTimeReceived, dateTimeFormat, CultureInfo.InvariantCulture);
			} catch (Exception ex) {
			    string errmsg = "TROWN DateTime.ParseExact(" + dateTimeReceived + ", " + dateTimeFormat + "): " + ex.Message;
			    rowParsed.ErrorMessages.Add(errmsg);
			}
		}
	}
}