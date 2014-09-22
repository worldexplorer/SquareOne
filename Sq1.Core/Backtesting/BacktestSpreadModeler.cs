using System;
using Sq1.Core.DataTypes;
using System.Diagnostics;

namespace Sq1.Core.Backtesting {
	public abstract class BacktestSpreadModeler {
		public abstract void GenerateFillBidAskSymmetrically(Quote quote, double medianPrice);
		public abstract void GenerateFillAskBasedOnBid(Quote quote);
		public abstract void GenerateFillBidBasedOnAsk(Quote quote);

		[Obsolete("quoteNotBeoyndBarLowHigh is moved upstack to BacktestQuoteGenerator.generateNewQuoteChildrenHelper()")]
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
				string msg2 = "I_REFUSE_TO_GENERATE_BIDASK THIS_QUOTE_ALREADY_HAS_BID_AND_ASK";
				Debugger.Break();
				#endif
				return;
			}

			if (double.IsNaN(quote.Bid) && double.IsNaN(quote.Ask)) {
				//string msg1 = "MUST_BE_BID_OR_ASK";
				//throw new Exception(msg1);
				// UNKNOWN, at Open or Close stroke
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
