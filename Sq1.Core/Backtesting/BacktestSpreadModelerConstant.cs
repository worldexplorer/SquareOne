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
		public override void GenerateFillAskBasedOnBid(Quote quote) {
			quote.Ask = quote.Bid + this.constant;
		}
		public override void GenerateFillBidBasedOnAsk(Quote quote) {
			quote.Bid = quote.Ask - this.constant;
		}
	}
}
