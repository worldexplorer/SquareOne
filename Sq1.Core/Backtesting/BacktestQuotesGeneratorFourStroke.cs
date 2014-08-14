using System;
using System.Collections.Generic;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Backtesting {
	public class BacktestQuotesGeneratorFourStroke : BacktestQuotesGenerator {
		public BacktestQuotesGeneratorFourStroke(Backtester backtester)
			: base(backtester, 4, BacktestMode.FourStrokeOHLC) {
		}

		public override List<Quote> GenerateQuotesFromBar(Bar bar) {
			base.QuotesGeneratedForOneBar.Clear();
			TimeSpan whole = bar.DateTimeNextBarOpenUnconditional - bar.DateTimeOpen;
			for (int stroke = 0; stroke < 4; stroke++) {
				double price = 0;
				TimeSpan interStrokeAdjustment = new TimeSpan(0);
				switch (stroke) {
					case 0:
						price = bar.Open;
						break;
					case 1:
						interStrokeAdjustment = new TimeSpan((whole.Ticks * 1 / base.QuotePerBarGenerates) - 1);
						price = (bar.Close > bar.Open) ? bar.Low : bar.High;
						break;
					case 2:
						interStrokeAdjustment = new TimeSpan((whole.Ticks * 2 / base.QuotePerBarGenerates) - 1);
						price = (bar.Close > bar.Open) ? bar.High : bar.Low;
						break;
					case 3:
						interStrokeAdjustment = new TimeSpan((whole.Ticks * 3 / base.QuotePerBarGenerates) - 1);
						price = bar.Close;
						break;
					default:
						throw new Exception("Stroke[" + stroke + "] isn't supported in 4-stroke QuotesGenerator");
				}
				DateTime serverTime = bar.DateTimeOpen + interStrokeAdjustment;
				Quote quote = base.generateNewQuoteChildrenHelper(stroke, "RunSimulationFourStrokeOHLC", bar.Symbol, serverTime, price, (int)bar.Volume / 4);
				base.QuotesGeneratedForOneBar.Add(quote);
			}
			List<Quote> clone = new List<Quote>(base.QuotesGeneratedForOneBar);
			return clone;
		}

	}
}