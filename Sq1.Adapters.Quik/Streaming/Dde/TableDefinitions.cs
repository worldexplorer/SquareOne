using System;
using System.Collections.Generic;

using Sq1.Adapters.Quik.Streaming.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Dde {
	public class TableDefinitions {
		

		public static List<XlColumn> XlColumnsForTable_Quotes { get {

			//String:TRADE_DATE_CODE,String:prevdate,String:settledate,String:time,String:CODE,String:SHORTNAME,String:CLASS_CODE,String:qty,String:bid,String:last,String:offer,String:quotebasis,String:high,String:low
			return  new List<XlColumn>() {
				//														MAND	IF_NOT_FOUND					POPUP	PRIMARY_KEY
				new XlColumn(XlBlockType.String,	"CODE",				true,	"NO_SYMBOL_RECEIVED_DDE",		true,	true),	// GAZP
				new XlColumn(XlBlockType.String,	"SHORTNAME",		true,	"NO_SHORTNAME"),			// russian name here
				new XlColumn(XlBlockType.String,	"CLASS_CODE",		true,	"NO_CLASS_CODE_RECEIVED_DDE"),
				new XlColumn(XlBlockType.Float,		"bid",				true,	double.NaN),
				new XlColumn(XlBlockType.Float,		"offer",			true,	double.NaN),
				new XlColumn(XlBlockType.Float,		"biddepth",			false,	double.NaN),
				new XlColumn(XlBlockType.Float,		"offerdepth",		false,	double.NaN),
				new XlColumn(XlBlockType.Float,		"last",				true,	double.NaN, false),
				//new XlColumn(, "realvmprice",	TypeExpected = XlTable.BlockType.String },
				//new XlColumn() { Names = new List<string>() {"time", "changetime"}, Type = XlTable.BlockType.String, Format = "h:mm:sstt" },

				//MAYBE_FOR_OLD_QUIK new XlColumn(XlBlockType.String,	"changetime")				{ ToTimeParseFormat = "h:mm:sstt" },	// mention some format to indicate it's a Time, I'll try to parse from anything first and then I'll try the mentioned one
				// QuikQuote.Date,Time are not Mandatory due to reconstructServerTime_useNowAndTimezoneFromMarketInfo_ifNotFoundInRow()
				new XlColumn(XlBlockType.String,	"TRADE_DATE_CODE",	false,	DateTime.MinValue)	{ ToDateParseFormat = "dd.MM.yyyy" },	// mention some format to indicate it's a Date, I'll try to parse from anything first and then I'll try the mentioned one
				new XlColumn(XlBlockType.String,	"time",				false,	DateTime.MinValue)	{ ToTimeParseFormat = "HH:mm:ss" },		// mention some format to indicate it's a Time, I'll try to parse from anything first and then I'll try the mentioned one

				new XlColumn(XlBlockType.Float,		"selldepo",			false,	double.NaN,						false),
				new XlColumn(XlBlockType.Float,		"buydepo",			false,	double.NaN,						false),
				new XlColumn(XlBlockType.Float,		"qty",				true,	double.NaN),
				new XlColumn(XlBlockType.Float,		"high",				false,	double.NaN),
				new XlColumn(XlBlockType.Float,		"low",				false,	double.NaN),

				//MAYBE_FOR_OLD_QUIK new XlColumn(XlBlockType.Float,		"stepprice"),
				new XlColumn(XlBlockType.Float,		"SEC_PRICE_STEP",	true,	double.NaN),		// => SymbolInfo 
			};
		} }

		public static List<XlColumn> XlColumnsForTable_Trades { get {
			return  new List<XlColumn>() {
				//														MAND	IF_NOT_FOUND					POPUP	PRIMARY_KEY
				new XlColumn(XlBlockType.String,	"TRADEDATE")		{ ToDateParseFormat = "dd.MM.yyyy" },
				new XlColumn(XlBlockType.String,	"TRADETIME")		{ ToTimeParseFormat = "h:mm:sstt" },
				new XlColumn(XlBlockType.Float,		"SECCODE",			true,	double.NaN,						true,	true),
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
				//														MAND	IF_NOT_FOUND					POPUP	PRIMARY_KEY
				new XlColumn(XlBlockType.Float,		"SELL_VOLUME",		false,	double.NaN),										// will be null@Write/Blank@Read  for levelTwoBids
				new XlColumn(XlBlockType.Float,		"PRICE",			true,	double.NaN,						true,	true),
				new XlColumn(XlBlockType.Float,		"BUY_VOLUME",		false,	double.NaN),										// will be null@Write/Blank@Read  for levelTwoAsks
			};
		} }


	}
}
