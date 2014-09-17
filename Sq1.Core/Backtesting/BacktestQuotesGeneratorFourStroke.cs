using System;
using System.Diagnostics;
using System.Collections.Generic;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Backtesting {
	public class BacktestQuotesGeneratorFourStroke : BacktestQuotesGenerator {
		public BacktestQuotesGeneratorFourStroke(Backtester backtester)
			: base(backtester, 4, BacktestMode.FourStrokeOHLC) {
		}

		//public TimeSpan getIntrabarMarketClearingResumesOffset(DateTime assumed, Bar barSimulated) {
		//    TimeSpan ret = new TimeSpan(0);

		//    MarketInfo marketInfo = barSimulated.ParentBars.MarketInfo;
		//    if (marketInfo.IsMarketSuspendedForClearing(assumed)) {
		//        return ret;
		//    }

		//    MarketClearingTimespan clearingTimespanOut;
		//    DateTime dateTimeNextBarOpenConditional = marketInfo.GetNextMarketServerTimeStamp(
		//        assumed, barSimulated.ScaleInterval, out clearingTimespanOut);

		//    if (dateTimeNextBarOpenConditional > barSimulated.DateTimeNextBarOpenUnconditional) {
		//        string msg = "CLEARING_INTERVAL_LONGER_THAN_BAR OR BAR_AT_MARKET_CLOSE; shorten clearing interval or shift market close forward";
		//        Debugger.Break();
		//    }

		//    if (clearingTimespanOut == null) return ret;

		//    DateTime clearingEndsDateTime = Bars.CombineBarDateWithMarketOpenTime(assumed, clearingTimespanOut.ResumeServerTimeOfDay);
		//    ret = clearingEndsDateTime.Subtract(assumed);
		//    if (ret.TotalSeconds > 0) {
		//        string msg1 = "[" + marketInfo.Name + "]Market is CLEARING, resumes["
		//            + dateTimeNextBarOpenConditional.ToString("HH:mm") + "], +[" + ret.TotalSeconds + "]sec for [" + assumed + "]";
		//        Debugger.Break();
		//    }
		//    return ret;
		//}

		public override SortedList<int, QuoteGenerated> GenerateQuotesFromBar(Bar barSimulated) {
			base.QuotesGeneratedForOneBar.Clear();

			double volumeOneQuarterOfBar = barSimulated.Volume / 4;
			if (barSimulated.ParentBars != null && barSimulated.ParentBars.SymbolInfo != null) {
				volumeOneQuarterOfBar = Math.Round(volumeOneQuarterOfBar, barSimulated.ParentBars.SymbolInfo.DecimalsVolume);
				if (volumeOneQuarterOfBar == 0) {
					//TESTED Debugger.Break();
					//double minimalValue = Math.Pow(1, -decimalsVolume);		// 1^(-2) = 0.01
					volumeOneQuarterOfBar = barSimulated.ParentBars.SymbolInfo.VolumeMinimalFromDecimal;
				}
			}
			if (volumeOneQuarterOfBar == 0) {
				Debugger.Break();
				volumeOneQuarterOfBar = 1;
			}

			TimeSpan whole = barSimulated.DateTimeNextBarOpenUnconditional - barSimulated.DateTimeOpen;
			TimeSpan increment = new TimeSpan(0, 0, ((int)(whole.TotalSeconds / base.QuotePerBarGenerates)) - 1);
			TimeSpan addToThisOpen = new TimeSpan(0);
			TimeSpan leftTillNextBar = whole;
			
			//v2 HACKY valid only for FORTS, 14:00...14:03
			DateTime initial = barSimulated.DateTimeOpen;
			MarketInfo marketInfo = barSimulated.ParentBars.MarketInfo;
			MarketClearingTimespan clearing = marketInfo.GetClearingTimespanIfMarketSuspendedDuringDateInterval(barSimulated.DateTimeOpen, barSimulated.DateTimeNextBarOpenUnconditional);
			if (clearing != null) {
				DateTime resumes = clearing.ResumeServerTimeOfDay;
				initial = new DateTime(initial.Year, initial.Month, initial.Day,
					resumes.Hour, resumes.Minute, resumes.Second);
			}

			for (int stroke = 0; stroke < 4; stroke++) {
				double price = 0;
				switch (stroke) {
					case 0: price = barSimulated.Open; break;
					case 1: price = (barSimulated.Close > barSimulated.Open) ? barSimulated.Low : barSimulated.High; break;
					case 2: price = (barSimulated.Close > barSimulated.Open) ? barSimulated.High : barSimulated.Low; break;
					case 3: price = barSimulated.Close; break;
					default: throw new Exception("Stroke[" + stroke + "] isn't supported in 4-stroke QuotesGenerator");
				}

				DateTime serverTime = initial + addToThisOpen;
				//v1
				//TimeSpan clearingResumesOffset = getIntrabarMarketClearingResumesOffset(serverTime, barSimulated);
				//if (clearingResumesOffset.TotalSeconds >= leftTillNextBar.TotalSeconds) {
				//    Debugger.Break();
				//    break;
				//}
				//if (clearingResumesOffset.TotalSeconds > 0) {
				//    addToThisOpen = clearingResumesOffset;
				//    leftTillNextBar = whole - addToThisOpen;
				//    int quotesLeft = base.QuotePerBarGenerates - stroke;
				//    increment = new TimeSpan(0, 0, ((int)(leftTillNextBar.TotalSeconds / quotesLeft)) - 1);
				//} else {
					addToThisOpen = addToThisOpen + increment;
					leftTillNextBar = whole - addToThisOpen;
				//}

				QuoteGenerated quote = base.generateNewQuoteChildrenHelper(stroke, "RunSimulationFourStrokeOHLC",
					barSimulated.Symbol, serverTime, price, volumeOneQuarterOfBar, barSimulated);
				base.QuotesGeneratedForOneBar.Add(stroke, quote);
			}
			return base.QuotesGeneratedForOneBar;
		}

	}
}