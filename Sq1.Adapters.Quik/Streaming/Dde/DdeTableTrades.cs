using System;
using System.Collections.Generic;

using Sq1.Adapters.Quik.Streaming.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Dde {
	public class DdeTableTrades : XlDdeTable {
		protected override string DdeConsumerClassName { get { return "DdeTableTrades"; } }

		public DdeTableTrades(string topic, QuikStreaming quikStreaming, List<XlColumn> columns) : base(topic, quikStreaming, columns) {}

		protected override void IncomingTableRow_convertToDataStructure(XlRowParsed row) {
			QuikTrade quikTrade			= new QuikTrade(this.DdeConsumerClassName + " Topic[" + base.Topic + "]");

			quikTrade.Symbol			= (string)row["SECCODE"];
			quikTrade.SymbolClass		= (string)row["CLASSCODE"];
			//quikTrade.ServerTime		= new DateTime(rowParsed["TRADEDATE"] + " " + rowParsed["TRADETIME"]);

			quikTrade.Price				= (double)row["PRICE"];
			quikTrade.Quantity			= (double)row["QTY"];
			quikTrade.BuyTrue_SellFalse	= (string)row["BUYSELL"] == "Купля" ? true : false;

			base.QuikStreaming.TradeDeliveredDdeCallback(quikTrade);
		}
	}
}
