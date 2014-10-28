//    Types.cs (c) 2012 Nikolay Moroshkin, http://www.moroshkin.com/
using System;

namespace Sq1.QuikAdapter {
	public enum QuikTerminalConnectionState { None, Exception, DllConnected, ConnectedUnsubscribed, ConnectedSubscribed, Emulation }
	public enum TradeOp { Cancel, Buy, Sell, Upsize, Downsize, Close, Reverse, Wait, NotInitializedYet }
//	public enum BaseQuote { None, Counter, Similar, Absolute }
	public enum DdeQuoteType { Unknown, Free, Spread, Ask, Bid, BestAsk, BestBid }

	public struct DdeQuote {
		public int Price;
		public int Volume;
		public DdeQuoteType Type;
		public DdeQuote(int price, int volume, DdeQuoteType type) {
			this.Price = price;
			this.Volume = volume;
			this.Type = type;
		}
	}
	public struct DdeSpread {
		public readonly int Ask;
		public readonly int Bid;
		public DdeSpread(int ask, int bid) {
			this.Ask = ask;
			this.Bid = bid;
		}
	}
	public struct DdeTrade {
		public int IntPrice;
		public double RawPrice;
		public int Quantity;
		public TradeOp Op;
		public DateTime DateTime;
		//public DateTime Received;
	}
}