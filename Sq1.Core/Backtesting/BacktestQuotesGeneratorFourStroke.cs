using System;
using System.Diagnostics;
using System.Collections.Generic;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Backtesting {
	public class BacktestQuotesGeneratorFourStroke : BacktestQuotesGenerator {
		public BacktestQuotesGeneratorFourStroke(Backtester backtester)
			: base(backtester, 4, BacktestMode.FourStrokeOHLC) {

//			string thisGeneratorName = this.GetType().Name;
//			string baseGeneratorName = base.GetType().Name;
//			if (thisGeneratorName.Length > baseGeneratorName.Length) {
//				string subclassName = baseGeneratorName.Substring(0, baseGeneratorName.Length);
//				if (string.IsNullOrEmpty(subclassName) == false) {
//					base.GeneratorName = subclassName;
//				}
//			}
		}
	}
}