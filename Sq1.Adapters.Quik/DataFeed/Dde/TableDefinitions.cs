using System;
using System.Collections.Generic;

using Sq1.Adapters.Quik.Dde.XlDde;

namespace Sq1.Adapters.Quik.Dde {
	public class TableDefinitions {
		public static List<XlColumn> XlColumnsForTable_Quotes { get {
			return  new List<XlColumn>() {
				new XlColumn() { Name = "SHORTNAME",	TypeExpected = XlBlockType.String,	Mandatory = true },
				new XlColumn() { Name = "CLASS_CODE",	TypeExpected = XlBlockType.String,	Mandatory = true },
				new XlColumn() { Name = "bid",			TypeExpected = XlBlockType.Float,	Mandatory = true },
				new XlColumn() { Name = "biddepth",		TypeExpected = XlBlockType.Float },
				new XlColumn() { Name = "offer",		TypeExpected = XlBlockType.Float,	Mandatory = true },
				new XlColumn() { Name = "offerdepth",	TypeExpected = XlBlockType.Float },
				new XlColumn() { Name = "last",			TypeExpected = XlBlockType.Float,	Mandatory = false },
				//new XlColumn() { Name = "realvmprice",	TypeExpected = XlTable.BlockType.String },
				//new XlColumn() { Names = new List<string>() {"time", "changetime"}, Type = XlTable.BlockType.String, Format = "h:mm:sstt" },
				new XlColumn() { Name = "date",			TypeExpected = XlBlockType.String,	Mandatory = true,	ToDateTimeParseFormat = "dd.MM.yyyy" },
				new XlColumn() { Name = "time",			TypeExpected = XlBlockType.String,	Mandatory = true,	ToDateTimeParseFormat = "HH:mm:ss" },
				new XlColumn() { Name = "changetime",	TypeExpected = XlBlockType.String,	Mandatory = false,	ToDateTimeParseFormat = "h:mm:sstt" },
				new XlColumn() { Name = "selldepo",		TypeExpected = XlBlockType.Float },
				new XlColumn() { Name = "buydepo",		TypeExpected = XlBlockType.Float },
				new XlColumn() { Name = "qty",			TypeExpected = XlBlockType.Float,	Mandatory = true },
				new XlColumn() { Name = "pricemin",		TypeExpected = XlBlockType.Float },
				new XlColumn() { Name = "pricemax",		TypeExpected = XlBlockType.Float },
				new XlColumn() { Name = "stepprice",	TypeExpected = XlBlockType.Float },
			};
		} }

		public static List<XlColumn> XlColumnsForTable_Trades { get {
			return  new List<XlColumn>() {
				new XlColumn() { Name = "TRADEDATE",	TypeExpected = XlBlockType.String,	ToDateTimeParseFormat = "h:mm:sstt", Mandatory = true },
				new XlColumn() { Name = "TRADETIME",	TypeExpected = XlBlockType.String,	ToDateTimeParseFormat = "h:mm:sstt", Mandatory = true },
				new XlColumn() { Name = "SECCODE",		TypeExpected = XlBlockType.String,	Mandatory = true },
				new XlColumn() { Name = "CLASSCODE",	TypeExpected = XlBlockType.String,	Mandatory = true },
				new XlColumn() { Name = "PRICE",		TypeExpected = XlBlockType.Float,	Mandatory = true },
				new XlColumn() { Name = "QTY",			TypeExpected = XlBlockType.Float,	Mandatory = true },
				new XlColumn() { Name = "BUYSELL",		TypeExpected = XlBlockType.String,	Mandatory = true },
				new XlColumn() { Name = "BUY",			TypeExpected = XlBlockType.Float,	Mandatory = true },
				new XlColumn() { Name = "SELL",			TypeExpected = XlBlockType.Float,	Mandatory = true },
			};
		} }

		public static List<XlColumn> XlColumnsForTable_DepthOfMarketPerSymbol { get {
			return  new List<XlColumn>() {
				new XlColumn() { Name = "SELL_VOLUME",	TypeExpected = XlBlockType.Float,	Mandatory = true },
				new XlColumn() { Name = "PRICE",		TypeExpected = XlBlockType.Float,	Mandatory = true },
				new XlColumn() { Name = "BUY_VOLUME",	TypeExpected = XlBlockType.Float,	Mandatory = true },
			};
		} }


	}
}
