using Sq1.Core.DataTypes;

namespace Sq1.Core.Backtesting {
	public abstract class BacktestSpreadModeler {
		public abstract void GenerateFillBidAskSymmetrically(Quote quote, double medianPrice);
		public abstract void GenerateFillAskBasedOnBid(Quote quote, double bidPrice);
		public abstract void GenerateFillBidBasedOnAsk(Quote quote, double askPrice);

		public void GeneratedQuoteFillBidAsk(Quote quote, Bar barSimulated) {
			if (quote == null) return;
			this.GenerateFillBidAskSymmetrically(quote, quote.PriceLastDeal);

			if (quote.Ask > barSimulated.High) {
				double pushDown = quote.Ask - barSimulated.High;
				quote.Ask -= pushDown;
				quote.Bid -= pushDown;
				return;
			}
			if (quote.Bid < barSimulated.Low) {
				double pushUp = barSimulated.Low - quote.Bid;
				quote.Ask += pushUp;
				quote.Bid += pushUp;
				return;
			}

			
		}
	}
}
