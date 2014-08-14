using Sq1.Core.DataTypes;

namespace Sq1.Core.Backtesting {
	public abstract class BacktestSpreadModeler {
		public abstract void GenerateFillBidAskSymmetrically(Quote quote, double medianPrice);
		public abstract void GenerateFillAskBasedOnBid(Quote quote, double bidPrice);
		public abstract void GenerateFillBidBasedOnAsk(Quote quote, double askPrice);

		public void GenerateFillBidAskSymmetricallyFromLastPrice(Quote quote) {
			if (quote == null) return;
			this.GenerateFillBidAskSymmetrically(quote, quote.PriceLastDeal);
		}
	}
}
