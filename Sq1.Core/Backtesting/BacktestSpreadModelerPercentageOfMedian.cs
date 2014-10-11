using Sq1.Core.DataTypes;

namespace Sq1.Core.Backtesting {
	public class BacktestSpreadModelerPercentageOfMedian : BacktestSpreadModeler {
		double percentageOfMedian;
		double partOfMedian;
		public BacktestSpreadModelerPercentageOfMedian(double percentageOfMedian) {
			this.percentageOfMedian = percentageOfMedian;
			this.partOfMedian = percentageOfMedian / 100;
		}
		public override void GenerateFillBidAskSymmetrically(Quote quote, double medianPrice) {
			if (quote == null) return;
			double spread = medianPrice * this.partOfMedian;
			quote.Bid = medianPrice - spread / 2;
			quote.Ask = medianPrice + spread / 2;
			//for medianPrice[80.36],percentageOfMedian[0.01] => spread[0.008036] => Bid[~80.35598],Ask[~80.36402]
			//should I round them to Bars.SymbolInfo.DecimalsPrice or Double isn't ment to be that precise?
		}
		public override void GenerateFillAskBasedOnBid(Quote quote) {
			double spread = quote.Bid * this.partOfMedian;
			quote.Ask = quote.Bid + spread;
		}
		public override void GenerateFillBidBasedOnAsk(Quote quote) {
			double spread = quote.Ask * this.partOfMedian;
			quote.Bid = quote.Ask - spread;
		}
		public override string ToString() {
			return "BacktestSpreadModelerPercentageOfMedian(percentageOfMedian[" + partOfMedian * 100 + "]).PartOfMedian=[" + partOfMedian + "]]";
		}

	}
}
