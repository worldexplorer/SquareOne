using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;

namespace Sq1.Core {
	public abstract class CommissionCalculator {
		public ScriptExecutor ScriptExecutor;
		public CommissionCalculator(ScriptExecutor scriptExecutor) {
			this.ScriptExecutor = scriptExecutor;
		}
		public virtual string Name { get { return this.GetType().Name; } }
		public abstract string Description { get; }
		public abstract double CalculateCommission(Direction direction, MarketLimitStop marketLimitStop, double orderPrice, double shares, Bars bars);
		public override string ToString() {
			return this.Name;
		}
	}
}
