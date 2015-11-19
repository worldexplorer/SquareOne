using System;

using Sq1.Adapters.Quik.Dde.XlDde;

namespace Sq1.Adapters.Quik.Dde {
	public class DdeTableTrades : XlDdeTable {
		const string cnDate			= "TRADEDATE";
		const string cnTime			= "TRADETIME";
		const string cnSecCode		= "SECCODE";
		const string cnClassCode	= "CLASSCODE";
		const string cnPrice		= "PRICE";
		const string cnQuantity		= "QTY";
		const string cnOperation	= "BUYSELL";
		const string strBuyOp		= "BUY";
		const string strSellOp		= "SELL";
		
		QuikStreaming quikStreamingAdapter;
		int cDate;
		int cTime;
		int cPrice;
		int cQuantity;
		int cOp;
		int cSecCode;
		int cClassCode;
		bool columnsNotDetected;

		public DdeTableTrades(string topic, QuikStreaming streamingAdapter) : base(topic) {
			this.quikStreamingAdapter = streamingAdapter;
			this.columnsNotDetected = true;
		}
		public override bool ReceivingDataDde {
			get { return base.ReceivingDataDde; }
			set {
				this.columnsNotDetected = true;
				base.ReceivingDataDde = value;
			}
		}
		protected override void PushIncomingRowParsed(XlRowParsed row) {
			int a = 1;
		}
		protected override void PutDdeTable(XlTable xt) {
			int row = 0;
			if (this.columnsNotDetected) {
				cDate = -1;
				cTime = -1;
				cPrice = -1;
				cQuantity = -1;
				cOp = -1;
				cSecCode = -1;
				cClassCode = -1;

				for (int col = 0 ; col < xt.ColumnsCount ; col++) {
					xt.ReadValue();

					if (xt.ValueType == XlTable.BlockType.String)
						switch (xt.StringValue) {
							case cnDate:
								cDate = col;
								break;
							case cnTime:
								cTime = col;
								break;
							case cnPrice:
								cPrice = col;
								break;
							case cnQuantity:
								cQuantity = col;
								break;
							case cnOperation:
								cOp = col;
								break;
							case cnSecCode:
								cSecCode = col;
								break;
							case cnClassCode:
								cClassCode = col;
								break;
						}
				}

				if (cDate < 0 || cTime < 0 || cPrice < 0 || cQuantity < 0 || cOp < 0 || cSecCode < 0 || cClassCode < 0) {
					ErrorParsing = true;
					return;
				}

				row++;
				columnsNotDetected = false;
			}

			while (row++ < xt.RowsCount) {
				bool rowCorrect = true;

				string secCode = string.Empty;
				string classCode = string.Empty;

				string date = string.Empty;
				string time = string.Empty;

				DdeTrade t = new DdeTrade();

				for (int col = 0 ; col < xt.ColumnsCount ; col++) {
					xt.ReadValue();

					if (col == cDate) {
						if (xt.ValueType == XlTable.BlockType.String)
							date = xt.StringValue;
						else
							rowCorrect = false;
					} else if (col == cTime) {
						if (xt.ValueType == XlTable.BlockType.String)
							time = xt.StringValue;
						else
							rowCorrect = false;
					} else if (col == cPrice) {
						if (xt.ValueType == XlTable.BlockType.Float)
							t.RawPrice = xt.FloatValue;
						else
							rowCorrect = false;
					} else if (col == cQuantity) {
						if (xt.ValueType == XlTable.BlockType.Float)
							t.Quantity = (int)xt.FloatValue;
						else
							rowCorrect = false;
					} else if (col == cOp) {
						if (xt.ValueType == XlTable.BlockType.String)
							switch (xt.StringValue) {
								case strBuyOp:
									t.Op = DdeTradeOp.Buy;
									break;
								case strSellOp:
									t.Op = DdeTradeOp.Sell;
									break;
							} else
							rowCorrect = false;
					} else if (col == cSecCode) {
						if (xt.ValueType == XlTable.BlockType.String)
							secCode = xt.StringValue;
						else
							rowCorrect = false;
					} else if (col == cClassCode) {
						if (xt.ValueType == XlTable.BlockType.String)
							classCode = xt.StringValue;
						else
							rowCorrect = false;
					}
				}

				if (DateTime.TryParse(date + " " + time, out t.DateTime)) {
					t.IntPrice = (int) t.RawPrice;
					//BrokerQuik.PutLastPrice(tp.IntPrice);
				} else {
						rowCorrect = false;
				}
				if (rowCorrect) {
					quikStreamingAdapter.TradeDeliveredDdeCallback(secCode + classCode, t);
				} else {
					ErrorParsing = true;
				}
			}
		}
	}
}
