using System;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Backtesting {
	public class BacktestQuotesGeneratorFourStroke : BacktestQuotesGenerator {
		public BacktestQuotesGeneratorFourStroke() : base(BacktestStrokesPerBar.FourStrokeOHLC) { }
		protected override void Assign_priceAndBidOrAsk_dependingOnQuotesPerBar_forStroke(
				Bar barSimulated, int stroke, out double priceFromBar_granularStrokes_willBeAligned_upstack, out BidOrAsk bidOrAsk) {
			switch (stroke) {
				case 0:
					priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.Open;
					bidOrAsk = BidOrAsk.UNKNOWN;
					// QUOTEGEN_PROBLEM#1 : here we don't know the derived.Spread, trying to avoid SymmetricFill 
					// don't fail Bar.ContainsBidAskForQuoteGenerated() check CANT_BE_LESS_OR_GREATER_ONLY_EQUAL_BUT_DOUBLE_VALUES_HAVE_TINY_TAILS
					if (barSimulated.Open <= barSimulated.Low) bidOrAsk = BidOrAsk.Bid;
					if (barSimulated.Open >= barSimulated.High) bidOrAsk = BidOrAsk.Ask;
					break;
				case 1:
					priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.IsWhiteCandle ? barSimulated.Low : barSimulated.High;
					bidOrAsk = barSimulated.IsWhiteCandle ? BidOrAsk.Bid : BidOrAsk.Ask;
					break;
				case 2:
					priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.IsWhiteCandle ? barSimulated.High : barSimulated.Low;
					bidOrAsk = barSimulated.IsWhiteCandle ? BidOrAsk.Ask : BidOrAsk.Bid;
					break;
				case 3:
					priceFromBar_granularStrokes_willBeAligned_upstack = barSimulated.Close;
					bidOrAsk = BidOrAsk.UNKNOWN;
					// QUOTEGEN_PROBLEM#1 : here we don't know the derived.Spread, trying to avoid SymmetricFill 
					// don't fail Bar.ContainsBidAskForQuoteGenerated() check CANT_BE_LESS_OR_GREATER_ONLY_EQUAL_BUT_DOUBLE_VALUES_HAVE_TINY_TAILS
					if (barSimulated.Close <= barSimulated.Low) bidOrAsk = BidOrAsk.Bid;
					if (barSimulated.Close >= barSimulated.High) bidOrAsk = BidOrAsk.Ask;
					break;
				default:
					throw new Exception("Stroke[" + stroke + "] isn't supported in 4-stroke QuotesGenerator");
			}
		}
	}
}
