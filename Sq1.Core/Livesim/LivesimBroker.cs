using Sq1.Core.Accounting;
using Sq1.Core.Broker;
using Sq1.Core.Support;

namespace Sq1.Core.Livesim {
	[SkipInstantiationAt(Startup = true)]
	public class LivesimBroker : BrokerProvider {
		public LivesimBroker() : base() {
			base.Name = "LivesimBroker";
			base.AccountAutoPropagate = new Account("LIVESIM_ACCOUNT", -1000);
			base.AccountAutoPropagate.Initialize(this);
		}
	}
}