using System;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Backtesting {
	public class BacktestQuotesGeneratorTenStroke : BacktestQuotesGenerator {
		public BacktestQuotesGeneratorTenStroke() : base( BacktestStrokesPerBar.TenStroke) { }

		protected override void AssignPriceAndBidOrAskDependingOnQuotesPerBarForStroke(
				Bar barSimulated, int stroke_0to9, out double price, out BidOrAsk bidOrAsk) {
			switch (stroke_0to9) {
				case 0:
					price = barSimulated.Open;
					bidOrAsk = BidOrAsk.UNKNOWN;
					// QUOTEGEN_PROBLEM#1 : here we don't know the derived.Spread, trying to avoid SymmetricFill 
					// don't fail Bar.ContainsBidAskForQuoteGenerated() check CANT_BE_LESS_OR_GREATER_ONLY_EQUAL_BUT_DOUBLE_VALUES_HAVE_TINY_TAILS
					if (barSimulated.Open <= barSimulated.Low) bidOrAsk = BidOrAsk.Bid;
					if (barSimulated.Open >= barSimulated.High) bidOrAsk = BidOrAsk.Ask;
					break;
				case 1:
					if (barSimulated.IsWhiteCandle) {
						double distance = barSimulated.Open - barSimulated.Low;
						double twoThirds = distance * 2 / 3;
						price = barSimulated.Open - twoThirds;
						bidOrAsk = BidOrAsk.Bid;
					} else {
						double distance = barSimulated.High - barSimulated.Open;
						double twoThirds = distance * 2 / 3;
						price = barSimulated.Open + twoThirds;
						bidOrAsk = BidOrAsk.Ask;
					}
					break;
				case 2:
					if (barSimulated.IsWhiteCandle) {
						double distance = barSimulated.Open - barSimulated.Low;
						double oneThird = distance * 1 / 3;
						price = barSimulated.Open - oneThird;
						bidOrAsk = BidOrAsk.Bid;
					} else {
						double distance = barSimulated.High - barSimulated.Open;
						double oneThird = distance * 1 / 3;
						price = barSimulated.Open + oneThird;
						bidOrAsk = BidOrAsk.Ask;
					}
					break;
				case 3:
					price = barSimulated.IsWhiteCandle ? barSimulated.Low : barSimulated.High;
					bidOrAsk = barSimulated.IsWhiteCandle ? BidOrAsk.Bid : BidOrAsk.Ask;
					break;
				case 4:
					if (barSimulated.IsWhiteCandle) {
						double oneThird = barSimulated.HighLowDistance * 1 / 3;
						price = barSimulated.High - oneThird;
						bidOrAsk = BidOrAsk.Ask;
					} else {
						double oneThird = barSimulated.HighLowDistance * 1 / 3;
						price = barSimulated.Low + oneThird;
						bidOrAsk = BidOrAsk.Bid;
					}
					break;
				case 5:
					if (barSimulated.IsWhiteCandle) {
						double twoThirds = barSimulated.HighLowDistance * 2 / 3;
						price = barSimulated.High - twoThirds;
						bidOrAsk = BidOrAsk.Ask;
					} else {
						double twoThirds = barSimulated.HighLowDistance * 2 / 3;
						price = barSimulated.Low + twoThirds;
						bidOrAsk = BidOrAsk.Bid;
					}
					break;
				case 6:
					price = barSimulated.IsWhiteCandle ? barSimulated.High : barSimulated.Low;
					bidOrAsk = barSimulated.IsWhiteCandle ? BidOrAsk.Ask : BidOrAsk.Bid;
					break;
				case 7:
					if (barSimulated.IsWhiteCandle) {
						double distance = barSimulated.High - barSimulated.Close;
						double twoThirds = distance * 2 / 3;
						price = barSimulated.High - twoThirds;
						bidOrAsk = BidOrAsk.Ask;
					} else {
						double distance = barSimulated.Close - barSimulated.Low;
						double twoThirds = distance * 2 / 3;
						price = barSimulated.Low + twoThirds;
						bidOrAsk = BidOrAsk.Bid;
					}
					break;
				case 8:
					if (barSimulated.IsWhiteCandle) {
						double distance = barSimulated.High - barSimulated.Close;
						double oneThird = distance * 1 / 3;
						price = barSimulated.High - oneThird;
						bidOrAsk = BidOrAsk.Ask;
					} else {
						double distance = barSimulated.Close - barSimulated.Low;
						double oneThird = distance * 1 / 3;
						price = barSimulated.Low + oneThird;
						bidOrAsk = BidOrAsk.Bid;
					}
					break;
				case 9:
					price = barSimulated.Close;
					bidOrAsk = BidOrAsk.UNKNOWN;
					// QUOTEGEN_PROBLEM#1 : here we don't know the derived.Spread, trying to avoid SymmetricFill 
					// don't fail Bar.ContainsBidAskForQuoteGenerated() check CANT_BE_LESS_OR_GREATER_ONLY_EQUAL_BUT_DOUBLE_VALUES_HAVE_TINY_TAILS
					if (barSimulated.Close <= barSimulated.Low) bidOrAsk = BidOrAsk.Bid;
					if (barSimulated.Close >= barSimulated.High) bidOrAsk = BidOrAsk.Ask;
					break;
				default:
					throw new Exception("Stroke[" + stroke_0to9 + "] isn't supported in " +  base.ToString());
			}
		}
	}
}
