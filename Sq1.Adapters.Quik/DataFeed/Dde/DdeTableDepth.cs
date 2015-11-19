using Sq1.Core;
using Sq1.Core.DataTypes;

using Sq1.Adapters.Quik.Dde.XlDde;

namespace Sq1.Adapters.Quik.Dde {
	public class DdeTableDepth : XlDdeTable {
		const string cnAskVolume	= "SELL_VOLUME";
		const string cnBidVolume	= "BUY_VOLUME";
		const string cnPrice		= "PRICE";
		
		QuikStreaming	quikStreamingAdapter;
		string			symbol;

		public DdeTableDepth(string topic, QuikStreaming receiver, string SymbolSubscribing) : base (topic) {
			this.quikStreamingAdapter = receiver;
			this.symbol = SymbolSubscribing;
			this.columnsIdentified = false;
		}
		protected override void PushIncomingRowParsed(XlRowParsed row) {
			int a = 1;
		}
		protected override void PutDdeTable(XlTable xt) {
			if (xt.RowsCount < 3) {
				this.ErrorParsing = true;
				return;
			}
			int cAskVolume = -1, cBidVolume = -1, cPrice = -1;
			for (int col = 0 ; col < xt.ColumnsCount ; col++) {
				xt.ReadValue();
				if (xt.ValueType == XlTable.BlockType.String)
					switch (xt.StringValue) {
						case cnAskVolume:
							cAskVolume = col;
							break;
						case cnBidVolume:
							cBidVolume = col;
							break;
						case cnPrice:
							cPrice = col;
							break;
					}
			}
			if (cAskVolume < 0 || cBidVolume < 0 || cPrice < 0) {
				this.ErrorParsing = true;
				return;
			}
			DdeQuote[] ddeQuotes = new DdeQuote[xt.RowsCount - 1];
			int ask = -1, bid = -1;
			for (int row = 0 ; row < ddeQuotes.Length ; row++) {
				int priceLevel = 0, askVolume = 0, bidVolume = 0, sc = 0;
				for (int col = 0 ; col < xt.ColumnsCount ; col++) {
					xt.ReadValue();
					switch (xt.ValueType) {
						case XlTable.BlockType.Float:
							if (col == cAskVolume) {
								askVolume = (int)xt.FloatValue;
							} else if (col == cBidVolume) {
								bidVolume = (int)xt.FloatValue;
							} else if (col == cPrice) {
								priceLevel = (int)xt.FloatValue;
							}
							break;
						case XlTable.BlockType.String:
							sc++;
							break;
					}
				}
				if (priceLevel <= 0) {
					if (sc == xt.ColumnsCount) {
						break;
					} else {
						this.ErrorParsing = true;
						return;
					}
				}
				if (askVolume > 0) {
					ask = row;
					ddeQuotes[row] = new DdeQuote(priceLevel, bidVolume, DdeQuoteType.Ask);
				} else if (bidVolume > 0) {
					if (bid == -1) {
						bid = row;
					}
					ddeQuotes[row] = new DdeQuote(priceLevel, bidVolume, DdeQuoteType.Bid);
				} else {
					this.ErrorParsing = true;
					return;
				}
			}
			if (ask == -1 || bid == -1 || ddeQuotes[0].Price <= ddeQuotes[1].Price) {
				ErrorParsing = true;
				return;
			}
			ddeQuotes[ask].Type = DdeQuoteType.BestAsk;
			ddeQuotes[bid].Type = DdeQuoteType.BestBid;
			//DdeSpread s = new DdeSpread(ddeQuotes[ask].Price, ddeQuotes[bid].Price);
			//quikStreamingAdapter.DdeDeliveredSpread(s, SymbolSubscribing);
			//quikStreamingAdapter.StreamingDataSnapshot.BestBidAskPutForSymbol(symbol, ddeQuotes[bid].Price, ddeQuotes[ask].Price);
			string msg = "THIS_CLASS_SHOULD_BE_REFACTORER__ITS_DISABLED_IN_DDE_SERVER_HERE";
			Assembler.PopupException(msg);
		}
	}
}