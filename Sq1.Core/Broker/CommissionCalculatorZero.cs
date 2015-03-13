using System;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Broker {
	public class CommissionCalculatorZero : CommissionCalculator {
		public CommissionCalculatorZero(ScriptExecutor scriptExecutor) : base(scriptExecutor) { }
		public override string Description { get { return "ALWAYS_RETURN_ZERO_COMMISSION"; } }
		public override double CalculateCommission(Direction direction, MarketLimitStop marketLimitStop,
												   double orderPrice, double shares, Bars bars) {
			return 0;
		}
	}
}
