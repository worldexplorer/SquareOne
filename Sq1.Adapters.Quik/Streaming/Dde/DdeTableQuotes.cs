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

		protected DateTime		quoteLastDateTimeForVolume = DateTime.MinValue;
		protected double		quoteLastSizeForVolume = 0;

		public DdeTableQuotes(string topic, QuikStreaming quikStreaming, List<XlColumn> columns) : base(topic, quikStreaming, columns, true) {}

		//protected override void IncomingTableRow_convertToDataStructure(XlRowParsed row) {
		protected override QuoteQuik IncomingTableRow_convertToDataStructure_monitoreable(XlRowParsed row) {
			//if (rowParsed["SHORTNAME"] == "LKOH") {
			//	int a = 1;
			//}

			// UPSTACK ALREADY_DOES_IT
			//string msig = " //this[" + this + "].IncomingTableRow_convertToDataStructure_monitoreable(" + row + ")";
			//foreach (string msg in row.ErrorMessages) {
			//	Assembler.PopupException(msg + msig, null, false);
			//}

			this.reconstructServerTime_useNowAndTimezoneFromMarketInfo_ifNotFoundInRow(row);	// upstack@base check rowParsed.ErrorMessages 
			DateTime serverTime		= row.GetDateTime("_ServerTime"	, DateTime.Now);
			//DateTime qChangeTime = DateTime.MinValue;
			//if (quote.ServerTime == DateTime.MinValue && qChangeTime != DateTime.MinValue) {
			//	quote.ServerTime = qChangeTime;
			//}
			string symbol			= row.Get<string>("CODE");
			double bid				= row.Get<double>("bid");
			double ask				= row.Get<double>("offer");

			double sizeParsed			= row.Get<double>("qty");
			////if (quoteLastDateTimeForVolume != quoteQuik.ServerTime) {
			////	quoteLastDateTimeForVolume  = quoteQuik.ServerTime;
			//    quoteQuik.Size = sizeParsed;
			////} else {
			////	string msg = "SHOULD_I_DELIVER_THE_DUPLIATE_QUOTE?";
			////	Assembler.PopupException(msg, null, false);
			////	return quoteQuik;
			////}
			////if (quoteLastSizeForVolume != sizeParsed) {
			////	quoteLastSizeForVolume = sizeParsed;
			////	quote.Size = sizeParsed;
			////}

			double	last				= row.Get<double>("last");
			BidOrAsk tradedAt = BidOrAsk.UNKNOWN;
			if (last == bid) tradedAt = BidOrAsk.Bid;
			if (last == ask) tradedAt = BidOrAsk.Ask;
			if (tradedAt == BidOrAsk.UNKNOWN) {
				string msg = "QUOTE_WASNT_TRADED last must NOT be bid or ask //ROUNDING_ERROR?...";
				//Assembler.PopupException(msg, null, false);
			}

			QuoteQuik quoteQuik = new QuoteQuik(DateTime.Now, serverTime, symbol, -1, bid, ask, sizeParsed, tradedAt);
			quoteQuik.Source			= this.DdeConsumerClassName + " Topic[" + base.Topic + "]";
			//quoteQuik.Symbol			= row.Get<string>("CODE");
			quoteQuik.SymbolClass		= row.Get<string>("CLASS_CODE");

			quoteQuik.FortsDepositBuy	= row.Get<double>("buydepo");
			quoteQuik.FortsDepositSell	= row.Get<double>("selldepo");
			quoteQuik.FortsPriceMax		= row.Get<double>("high");
			quoteQuik.FortsPriceMin		= row.Get<double>("low");

			quoteQuik.PriceStepFromDde	= row.Get<double>("SEC_PRICE_STEP");
			this.syncPriceStep_toSymbolInfo(quoteQuik);

			base.QuikStreaming.PushQuoteReceived(quoteQuik);	//goes to another thread via PUMP and invokes strategies letting me go
			return quoteQuik;									//one more delay is to raise and event which will go to GUI thread as well QuikStreamingMonitorForm.tableQuotes_DataStructureParsed_One()
		}

		void reconstructServerTime_useNowAndTimezoneFromMarketInfo_ifNotFoundInRow(XlRowParsed rowParsed) {
			string msig = " //this[" + this + "].reconstructServerTime_useNowAndTimezoneFromMarketInfo_ifNotFoundInRow(" + rowParsed + ")";

			string dateReceived = rowParsed.GetString("TRADE_DATE_CODE",	"QUOTE_DATE_NOT_DELIVERED_DDE");
			string timeReceived = rowParsed.GetString("time",				"QUOTE_TIME_NOT_DELIVERED_DDE");

			string dateFormat = base.ColumnDefinitionFor("TRADE_DATE_CODE").ToDateParseFormat;
			string timeFormat = base.ColumnDefinitionFor("time").ToTimeParseFormat;

			string errmsg = "DATE_NOT_FOUND_IN_rowParsed__RETURNING_DateTime.MinValue";
			DateTime serverDateTime  = base.QuikStreaming.ReconstructServerTime_useNowAndTimezoneFromMarketInfo(out errmsg, dateReceived, timeReceived, dateFormat, timeFormat);

			if (serverDateTime != DateTime.MinValue) {
				rowParsed.AddOrReplace("_ServerTime", serverDateTime);
			}

			if (string.IsNullOrEmpty(errmsg) == false) {
				rowParsed.ErrorMessages.Add(errmsg + msig);
			}
		}

		void syncPriceStep_toSymbolInfo(QuoteQuik quoteQuik) {
			if (double.IsNaN(quoteQuik.PriceStepFromDde)) return;
			if (Assembler.InstanceInitialized.RepositorySymbolInfos == null) return;

			SymbolInfo symbolInfo = Assembler.InstanceInitialized.RepositorySymbolInfos.FindSymbolInfo_nullUnsafe(quoteQuik.Symbol);
			if (symbolInfo == null) return;

			//int priceStep_fromDde_asInt = Convert.ToInt32(Math.Round(priceStep_fromDde));
			if (symbolInfo.PriceStepFromDde == quoteQuik.PriceStepFromDde) return;

			symbolInfo.PriceStepFromDde = quoteQuik.PriceStepFromDde;
			Assembler.InstanceInitialized.RepositorySymbolInfos.Serialize();	// YEAH (double)0 != (double)0... serializing as many times as many quotes we received first; but only once/symbol/session koz Infos are cached
		}
	}
}