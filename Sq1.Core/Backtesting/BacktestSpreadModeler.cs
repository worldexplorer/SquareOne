using System;
using Sq1.Core.DataTypes;
using System.Diagnostics;

namespace Sq1.Core.Backtesting {
	public abstract class BacktestSpreadModeler {
		public abstract void GenerateFillBidAskSymmetrically(Quote quote, double medianPrice);
		public abstract void GenerateFillAskBasedOnBid(Quote quote);
		public abstract void GenerateFillBidBasedOnAsk(Quote quote);
		public abstract string ToString();

		public void GeneratedQuoteFillBidAsk(Quote quote, Bar barSimulated, double priceForSymmetricFillAtOpenOrClose = -1) {
			if (quote == null) return;

			if (quote.ItriggeredFillAtBidOrAsk != BidOrAsk.UNKNOWN) {
				#if DEBUG
				string msg = "I_REFUSE_TO_GENERATE_BIDASK THIS_QUOTE_ALREADY_TRIGGERED_SOME_FILL";
				Debugger.Break();
				#endif
				return;
			}

			if (double.IsNaN(quote.Bid) == false && double.IsNaN(quote.Ask) == false) {
				#if DEBUG
				string msg = "I_REFUSE_TO_GENERATE_BIDASK THIS_QUOTE_ALREADY_HAS_BID_AND_ASK";
				Debugger.Break();
				#endif
				Assembler.PopupException(msg);
				return;
			}

			if (double.IsNaN(quote.Bid) && double.IsNaN(quote.Ask)) {
				string msg = "WARNING_IMPRECISE_QUOTE_MODELING: at Open or Close stroke when I don't have to keep bar boundaries very precise; check generateNewQuoteChildrenHelper() for BidOrAsk=UNKNOWN";
				#if DEBUG
				//Debugger.Break();
				#endif
				//Assembler.PopupException(msg);
				
				this.GenerateFillBidAskSymmetrically(quote, priceForSymmetricFillAtOpenOrClose);
			}

			if (double.IsNaN(quote.Bid)) {
				this.GenerateFillAskBasedOnBid(quote);
				return;
			}
			if (double.IsNaN(quote.Ask)) {
				this.GenerateFillBidBasedOnAsk(quote);
				return;
			}			
		}
	}
}
