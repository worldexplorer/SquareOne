using System;
using System.Globalization;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.DataTypes;

using Sq1.Adapters.Quik.Streaming.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Dde {
	//public class DdeTableQuotes : XlDdeTable {
	public class DdeTableQuotes : XlDdeTableMonitoreable<QuoteQuik> {
		protected override string DdeConsumerClassName { get { return "DdeTableQuotes"; } }

		protected DateTime		lastQuoteDateTimeForVolume = DateTime.MinValue;
		protected double		lastQuoteSizeForVolume = 0;

		public DdeTableQuotes(string topic, QuikStreaming quikStreaming, List<XlColumn> columns) : base(topic, quikStreaming, columns, true) {}

		//protected override void IncomingTableRow_convertToDataStructure(XlRowParsed row) {
		protected override QuoteQuik IncomingTableRow_convertToDataStructure_monitoreable(XlRowParsed row) {
			//if (rowParsed["SHORTNAME"] == "LKOH") {
			//	int a = 1;
			//}

			foreach (string msg in row.ErrorMessages) {
				Assembler.PopupException(msg, null, false);
			}

			QuoteQuik quikQuote = new QuoteQuik(DateTime.Now);
			quikQuote.Source			= this.DdeConsumerClassName + " Topic[" + base.Topic + "]";
			quikQuote.Symbol			= row.GetString("CODE"			, "NO_SYMBOL_RECEIVED_DDE");
			quikQuote.SymbolClass		= row.GetString("CLASS_CODE"	, "NO_CLASS_CODE_RECEIVED_DDE");
			quikQuote.Bid				= row.GetDouble("bid"			, double.NaN);
			quikQuote.Ask				= row.GetDouble("offer"			, double.NaN);

			double	last				= row.GetDouble("last"			, double.NaN);
			if (last == quikQuote.Bid) quikQuote.TradedAt = BidOrAsk.Bid;
			if (last == quikQuote.Ask) quikQuote.TradedAt = BidOrAsk.Ask;
			if (quikQuote.TradedAt == BidOrAsk.UNKNOWN) {
				string msg = "QUOTE_WASNT_TRADED last must NOT be bid or ask //ROUNDING_ERROR?...";
				Assembler.PopupException(msg, null, false);
			}

			quikQuote.FortsDepositBuy	= row.GetDouble("buydepo"		, double.NaN);
			quikQuote.FortsDepositSell	= row.GetDouble("selldepo"		, double.NaN);
			quikQuote.FortsPriceMax		= row.GetDouble("high"			, double.NaN);
			quikQuote.FortsPriceMin		= row.GetDouble("low"			, double.NaN);

			this.reconstructServerTime_useNowAndTimezoneFromMarketInfo_ifNotFoundInRow(row);	// upstack@base check rowParsed.ErrorMessages 
			quikQuote.ServerTime		= row.GetDateTime("ServerTime"	, DateTime.Now);
			//DateTime qChangeTime = DateTime.MinValue;
			//if (quote.ServerTime == DateTime.MinValue && qChangeTime != DateTime.MinValue) {
			//	quote.ServerTime = qChangeTime;
			//}

			double sizeParsed			= row.GetDouble("qty"			, double.NaN);
			//if (lastQuoteDateTimeForVolume != quikQuote.ServerTime) {
			//    lastQuoteDateTimeForVolume  = quikQuote.ServerTime;
			    quikQuote.Size = sizeParsed;
			//} else {
			//    string msg = "SHOULD_I_DELIVER_THE_DUPLIATE_QUOTE?";
			//    Assembler.PopupException(msg, null, false);
			//    return quikQuote;
			//}
			//if (lastQuoteSizeForVolume != sizeParsed) {
			//	lastQuoteSizeForVolume = sizeParsed;
			//	quote.Size = sizeParsed;
			//}

			quikQuote.PriceStepFromDde	= row.GetDouble("SEC_PRICE_STEP"	, double.NaN);
			this.syncPriceStep_toSymbolInfo(quikQuote);

			base.QuikStreaming.PushQuoteReceived(quikQuote);	//goes to another thread via PUMP and invokes strategies letting me go
			return quikQuote;									//one more delay is to raise and event which will go to GUI thread as well QuikStreamingMonitorForm.tableQuotes_DataStructureParsed_One()
		}

		void reconstructServerTime_useNowAndTimezoneFromMarketInfo_ifNotFoundInRow(XlRowParsed rowParsed) {
			DateTime ret = DateTime.MinValue;
			string errmsg = "DATE_NOT_FOUND_IN_rowParsed__RETURNING_DateTime.MinValue";

			MarketInfo marketInfo = this.QuikStreaming.DataSource.MarketInfo;
			if (marketInfo == null) {
				ret = TimeZoneInfo.ConvertTime(DateTime.Now, marketInfo.TimeZoneInfo);
				errmsg = "DATE_NOT_FOUND_IN_rowParsed__RETURNING_DateTime.Now=>marketInfo[" + this.QuikStreaming.DataSource.MarketName + "]"
					+ ".TimeZoneInfo.BaseUtcOffset[" + marketInfo.TimeZoneInfo.BaseUtcOffset + "]";
			}

			string dateReceived = rowParsed.GetString("TRADE_DATE_CODE", "QUOTE_DATE_NOT_DELIVERED_DDE");
			string timeReceived = rowParsed.GetString("time", "QUOTE_TIME_NOT_DELIVERED_DDE");
			if (dateReceived == "QUOTE_DATE_NOT_DELIVERED_DDE" || timeReceived == "QUOTE_TIME_NOT_DELIVERED_DDE") {
				rowParsed["ServerTime"] = ret;
			    rowParsed.ErrorMessages.Add(errmsg);
				return;
			}

			string dateTimeReceived = dateReceived + " " + timeReceived;

			try {
			    ret = DateTime.Parse(dateTimeReceived);
				rowParsed["ServerTime"] = ret;
				return;		// if not Parse()d fromAnyFormat then it'll throw and I'll continue with ParseExact()
			} catch (Exception ex) {
			    errmsg = "TROWN DateTime.Parse(" + dateTimeReceived + "): " + ex.Message;
			    rowParsed.ErrorMessages.Add(errmsg);
			}

			string dateFormat = base.ColumnDefinitionsByNameLookup["TRADE_DATE_CODE"]	.ToDateParseFormat;
			string timeFormat = base.ColumnDefinitionsByNameLookup["time"]				.ToTimeParseFormat;
			string dateTimeFormat = dateFormat + " " + timeFormat;
			try {
			    ret = DateTime.ParseExact(dateTimeReceived, dateTimeFormat, CultureInfo.InvariantCulture);
				rowParsed["ServerTime"] = ret;
			} catch (Exception ex) {
			    errmsg = "TROWN DateTime.ParseExact(" + dateTimeReceived + ", " + dateTimeFormat + "): " + ex.Message;
			    rowParsed.ErrorMessages.Add(errmsg);
			}
		}

		void syncPriceStep_toSymbolInfo(QuoteQuik quikQuote) {
			if (double.IsNaN(quikQuote.PriceStepFromDde)) return;
			if (Assembler.InstanceInitialized.RepositorySymbolInfos == null) return;

			SymbolInfo symbolInfo = Assembler.InstanceInitialized.RepositorySymbolInfos.FindSymbolInfo_nullUnsafe(quikQuote.Symbol);
			if (symbolInfo == null) return;

			//int priceStep_fromDde_asInt = Convert.ToInt32(Math.Round(priceStep_fromDde));
			if (symbolInfo.PriceStepFromDde == quikQuote.PriceStepFromDde) return;

			symbolInfo.PriceStepFromDde = quikQuote.PriceStepFromDde;
			Assembler.InstanceInitialized.RepositorySymbolInfos.Serialize();	// YEAH (double)0 != (double)0... serializing as many times as many quotes we received first; but only once/symbol/session koz Infos are cached
		}
	}
}