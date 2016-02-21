using System;
using System.Collections.Generic;

using Sq1.Adapters.Quik.Streaming.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Dde {
	public class TableDefinitions {
		public static List<XlColumn> XlColumnsForTable_Quotes { get {

			//String:TRADE_DATE_CODE,String:prevdate,String:settledate,String:time,String:CODE,String:SHORTNAME,String:CLASS_CODE,String:qty,String:bid,String:last,String:offer,String:quotebasis,String:high,String:low
			return  new List<XlColumn>() {
				new XlColumn(XlBlockType.String,	"CODE",				true, true),	// GAZP
				new XlColumn(XlBlockType.String,	"SHORTNAME",		true),			// russian name here
				new XlColumn(XlBlockType.String,	"CLASS_CODE",		true),
				new XlColumn(XlBlockType.Float,		"bid",				true),
				new XlColumn(XlBlockType.Float,		"biddepth"),
				new XlColumn(XlBlockType.Float,		"offer",			true),
				new XlColumn(XlBlockType.Float,		"offerdepth"),
				new XlColumn(XlBlockType.Float,		"last",				true),
				//new XlColumn(, "realvmprice",	TypeExpected = XlTable.BlockType.String },
				//new XlColumn() { Names = new List<string>() {"time", "changetime"}, Type = XlTable.BlockType.String, Format = "h:mm:sstt" },

				// QuikQuote.Date,Time are not Mandatory due to reconstructServerTime_useNowAndTimezoneFromMarketInfo_ifNotFoundInRow()
				new XlColumn(XlBlockType.String,	"TRADE_DATE_CODE",	false)	{ ToDateParseFormat = "dd.MM.yyyy" },	// mention some format to indicate it's a Date, I'll try to parse from anything first and then I'll try the mentioned one
				new XlColumn(XlBlockType.String,	"time",				false)	{ ToTimeParseFormat = "HH:mm:ss" },		// mention some format to indicate it's a Time, I'll try to parse from anything first and then I'll try the mentioned one

				new XlColumn(XlBlockType.String,	"changetime")				{ ToTimeParseFormat = "h:mm:sstt" },	// mention some format to indicate it's a Time, I'll try to parse from anything first and then I'll try the mentioned one
				new XlColumn(XlBlockType.Float,		"selldepo"),
				new XlColumn(XlBlockType.Float,		"buydepo"),
				new XlColumn(XlBlockType.Float,		"qty"),
				new XlColumn(XlBlockType.Float,		"high"),
				new XlColumn(XlBlockType.Float,		"low"),
				new XlColumn(XlBlockType.Float,		"stepprice"),
				new XlColumn(XlBlockType.Float,		"SEC_PRICE_STEP"),		// => SymbolInfo 
			};
		} }

		public static List<XlColumn> XlColumnsForTable_Trades { get {
			return  new List<XlColumn>() {
				new XlColumn(XlBlockType.String,	"TRADEDATE")		{ ToDateParseFormat = "dd.MM.yyyy" },
				new XlColumn(XlBlockType.String,	"TRADETIME")		{ ToTimeParseFormat = "h:mm:sstt" },
				new XlColumn(XlBlockType.Float,		"SECCODE",			true, true),
				new XlColumn(XlBlockType.Float,		"CLASSCODE"),
				new XlColumn(XlBlockType.Float,		"PRICE"),
				new XlColumn(XlBlockType.Float,		"QTY"),
				new XlColumn(XlBlockType.Float,		"BUYSELL"),
				new XlColumn(XlBlockType.Float,		"BUY"),
				new XlColumn(XlBlockType.Float,		"SELL"),
			};
		} }

		public static List<XlColumn> XlColumnsForTable_DepthOfMarketPerSymbol { get {
			return  new List<XlColumn>() {
				new XlColumn(XlBlockType.Float,		"SELL_VOLUME",		false),		// will be null@Write/Blank@Read  for levelTwoBids
				new XlColumn(XlBlockType.Float,		"PRICE",			true, true),
				new XlColumn(XlBlockType.Float,		"BUY_VOLUME",		false),		// will be null@Write/Blank@Read  for levelTwoAsks
			};
		} }


	}
}
