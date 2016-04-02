using System;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Backtesting {
	public class BacktestQuotesGeneratorTenStroke : BacktestQuotesGenerator {
		public BacktestQuotesGeneratorTenStroke() : base( BacktestStrokesPerBar.TenStroke) { }

		protected override void Assign_priceAndBidOrAsk_dependingOnQuotesPerBar_forStroke(
				Bar barSimulated, int stroke_0to9, out double priceFromBar_granularStrokes_willBeAligned_upstack, out BidOrAsk bidOrAsk) {
			switch (stroke_0to9) {
				case 0:
					priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.Open;
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
						priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.Open - twoThirds;
						bidOrAsk = BidOrAsk.Bid;
					} else {
						double distance = barSimulated.High - barSimulated.Open;
						double twoThirds = distance * 2 / 3;
						priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.Open + twoThirds;
						bidOrAsk = BidOrAsk.Ask;
					}
					break;
				case 2:
					if (barSimulated.IsWhiteCandle) {
						double distance = barSimulated.Open - barSimulated.Low;
						double oneThird = distance * 1 / 3;
						priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.Open - oneThird;
						bidOrAsk = BidOrAsk.Bid;
					} else {
						double distance = barSimulated.High - barSimulated.Open;
						double oneThird = distance * 1 / 3;
						priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.Open + oneThird;
						bidOrAsk = BidOrAsk.Ask;
					}
					break;
				case 3:
					priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.IsWhiteCandle ? barSimulated.Low : barSimulated.High;
					bidOrAsk = barSimulated.IsWhiteCandle ? BidOrAsk.Bid : BidOrAsk.Ask;
					break;
				case 4:
					if (barSimulated.IsWhiteCandle) {
						double oneThird = barSimulated.HighLowDistance * 1 / 3;
						priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.High - oneThird;
						bidOrAsk = BidOrAsk.Ask;
					} else {
						double oneThird = barSimulated.HighLowDistance * 1 / 3;
						priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.Low + oneThird;
						bidOrAsk = BidOrAsk.Bid;
					}
					break;
				case 5:
					if (barSimulated.IsWhiteCandle) {
						double twoThirds = barSimulated.HighLowDistance * 2 / 3;
						priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.High - twoThirds;
						bidOrAsk = BidOrAsk.Ask;
					} else {
						double twoThirds = barSimulated.HighLowDistance * 2 / 3;
						priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.Low + twoThirds;
						bidOrAsk = BidOrAsk.Bid;
					}
					break;
				case 6:
					priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.IsWhiteCandle ? barSimulated.High : barSimulated.Low;
					bidOrAsk = barSimulated.IsWhiteCandle ? BidOrAsk.Ask : BidOrAsk.Bid;
					break;
				case 7:
					if (barSimulated.IsWhiteCandle) {
						double distance = barSimulated.High - barSimulated.Close;
						double twoThirds = distance * 2 / 3;
						priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.High - twoThirds;
						bidOrAsk = BidOrAsk.Ask;
					} else {
						double distance = barSimulated.Close - barSimulated.Low;
						double twoThirds = distance * 2 / 3;
						priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.Low + twoThirds;
						bidOrAsk = BidOrAsk.Bid;
					}
					break;
				case 8:
					if (barSimulated.IsWhiteCandle) {
						double distance = barSimulated.High - barSimulated.Close;
						double oneThird = distance * 1 / 3;
						priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.High - oneThird;
						bidOrAsk = BidOrAsk.Ask;
					} else {
						double distance = barSimulated.Close - barSimulated.Low;
						double oneThird = distance * 1 / 3;
						priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.Low + oneThird;
						bidOrAsk = BidOrAsk.Bid;
					}
					break;
				case 9:
					priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.Close;
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
