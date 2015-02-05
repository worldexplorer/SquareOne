using Sq1.Core.Accounting;
using Sq1.Core.Broker;
using Sq1.Core.Support;

namespace Sq1.Core.Backtesting {
	[SkipInstantiationAt(Startup = true)]
	public class BacktestBroker : BrokerProvider {
		public BacktestBroker() : base() {
			base.Name = "BacktestBrokerProvider";
			base.AccountAutoPropagate = new Account("BACKTEST_ACCOUNT", -1000);
			base.AccountAutoPropagate.Initialize(this);
		}
	}
}