using Sq1.Core.DataTypes;

namespace Sq1.Core.Backtesting {
	public class BacktestSpreadModelerConstant : BacktestSpreadModeler {
		int constant;
		public BacktestSpreadModelerConstant(int constant) {
			this.constant = constant;
		}
		public override void GenerateFillBidAskSymmetrically(Quote quote, double medianPrice) {
			if (quote == null) return;
			quote.Ask = medianPrice + this.constant / 2;
			quote.Bid = medianPrice - this.constant / 2;
		}
		public override void GenerateFillAskBasedOnBid(Quote quote, double bidPrice) {
			quote.Ask = bidPrice + this.constant;
			quote.Bid = bidPrice;
		}
		public override void GenerateFillBidBasedOnAsk(Quote quote, double askPrice) {
			quote.Ask = askPrice;
			quote.Bid = askPrice - this.constant;
		}
	}
}
