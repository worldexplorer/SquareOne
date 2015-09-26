using System;
using Sq1.Core.StrategyBase;

namespace Sq1.Reporters {
	public partial class Performance : Reporter {
		protected override void SymbolInfo_PriceDecimalsChanged(object sender, EventArgs e) {
			this.propagatePerformanceReport();
		}
	}
}
