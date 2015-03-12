using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Backtesting {
	public class BacktestQuotesGeneratorSixteenStroke : BacktestQuotesGenerator {
		public BacktestQuotesGeneratorSixteenStroke() : base(BacktestStrokesPerBar.SixteenStroke) { }
		protected override void AssignPriceAndBidOrAskDependingOnQuotesPerBarForStroke(
				Bar barSimulated, int stroke_0to15, out double price, out BidOrAsk bidOrAsk) {

			List<double> legPatternExcludingBoundaries = new List<double>()
				//{ 0/5d, 2/5d, 1/5d, 4/5d, 3/5d, 5/5d };	// 0/5 and 5/5 are never used below
				{ 0, 0.4, 0.2, 0.8, 0.6, 1 };	// 0/5 and 5/5 are never used below
			List<double> openToLow = legPatternExcludingBoundaries;
			List<double> lowToHigh = legPatternExcludingBoundaries;
			List<double> highToClose = legPatternExcludingBoundaries;

			int strokesForOneLeg	= this.BacktestStrokesPerBarAsInt / 3;	// 15 /3 = 5
			int currentLeg			= stroke_0to15 / strokesForOneLeg;		// 6 / 5 = 1
			int strokeWithinLeg		= stroke_0to15 % strokesForOneLeg;		// 6 % 5 = 1


			if (stroke_0to15 == 0) {							// stroke_0to15=0
				price = barSimulated.Open;
				bidOrAsk = BidOrAsk.UNKNOWN;
				// QUOTEGEN_PROBLEM#1 : here we don't know the derived.Spread, trying to avoid SymmetricFill 
				// don't fail Bar.ContainsBidAskForQuoteGenerated() check CANT_BE_LESS_OR_GREATER_ONLY_EQUAL_BUT_DOUBLE_VALUES_HAVE_TINY_TAILS
				if (barSimulated.Open <= barSimulated.Low) bidOrAsk = BidOrAsk.Bid;
				if (barSimulated.Open >= barSimulated.High) bidOrAsk = BidOrAsk.Ask;
			} else if (stroke_0to15 < strokesForOneLeg) {		// stroke_0to15=1,2,3,4
				// first leg openToLow
				double distancePart = openToLow[strokeWithinLeg];
				if (barSimulated.IsWhiteCandle) {
					double distance = barSimulated.Open - barSimulated.Low;
					double nFifths = distance * distancePart;
					price = barSimulated.Open - nFifths;
					bidOrAsk = BidOrAsk.Bid;
				} else {
					double distance = barSimulated.High - barSimulated.Open;
					double nFifths = distance * distancePart;
					price = barSimulated.Open + nFifths;
					bidOrAsk = BidOrAsk.Ask;
				}
			} else if (stroke_0to15 == strokesForOneLeg) { 		// stroke_0to15=5
				// Low
				price = barSimulated.IsWhiteCandle ? barSimulated.Low : barSimulated.High;
				bidOrAsk = barSimulated.IsWhiteCandle ? BidOrAsk.Bid : BidOrAsk.Ask;
			} else if (stroke_0to15 < strokesForOneLeg * 2) {	// stroke_0to15=6,7,8,9
				// second leg lowToHigh
				double distancePart = lowToHigh[strokeWithinLeg];
				if (barSimulated.IsWhiteCandle) {
					double nFifths = barSimulated.HighLowDistance * distancePart;
					price = barSimulated.Low + nFifths;
					bidOrAsk = BidOrAsk.Ask;
				} else {
					double nFifths = barSimulated.HighLowDistance * distancePart;
					price = barSimulated.High - nFifths;
					bidOrAsk = BidOrAsk.Bid;
				}
			} else if (stroke_0to15 == strokesForOneLeg * 2) {	// stroke_0to15=10
				// High
				price = barSimulated.IsWhiteCandle ? barSimulated.High : barSimulated.Low;
				bidOrAsk = barSimulated.IsWhiteCandle ? BidOrAsk.Ask : BidOrAsk.Bid;
			} else if (stroke_0to15 < strokesForOneLeg * 3) {	// stroke_0to15=11,12,13,14
				// third leg highToClose
				double distancePart = highToClose[strokeWithinLeg];
				if (barSimulated.IsWhiteCandle) {
					double distance = barSimulated.High - barSimulated.Close;
					double nFifths = distance * distancePart;
					price = barSimulated.High - nFifths;
					bidOrAsk = BidOrAsk.Ask;
				} else {
					double distance = barSimulated.Close - barSimulated.Low;
					double nFifths = distance * distancePart;
					price = barSimulated.Low + nFifths;
					bidOrAsk = BidOrAsk.Bid;
				}
			} else if (stroke_0to15 == strokesForOneLeg * 3) {		// stroke_0to15=15
				price = barSimulated.Close;
				bidOrAsk = BidOrAsk.UNKNOWN;
				// QUOTEGEN_PROBLEM#1 : here we don't know the derived.Spread, trying to avoid SymmetricFill 
				// don't fail Bar.ContainsBidAskForQuoteGenerated() check CANT_BE_LESS_OR_GREATER_ONLY_EQUAL_BUT_DOUBLE_VALUES_HAVE_TINY_TAILS
				if (barSimulated.Close <= barSimulated.Low) bidOrAsk = BidOrAsk.Bid;
				if (barSimulated.Close >= barSimulated.High) bidOrAsk = BidOrAsk.Ask;
			} else {
				string msg = "Stroke[" + stroke_0to15 + "] isn't supported in " + base.ToString();
				throw new Exception(msg);
			}
		}
	}
}
