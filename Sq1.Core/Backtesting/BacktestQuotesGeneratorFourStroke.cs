using System;
using System.Diagnostics;
using System.Collections.Generic;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Backtesting {
	public class BacktestQuotesGeneratorFourStroke : BacktestQuotesGenerator {
		public BacktestQuotesGeneratorFourStroke(Backtester backtester)
			: base(backtester, 4, BacktestMode.FourStrokeOHLC) {
		}
	}
}