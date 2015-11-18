using Sq1.Adapters.Quik;
using Sq1.Core.Support;

namespace Sq1.Adapters.QuikLivesim {
	[SkipInstantiationAt(Startup = true)]		// I'm an internally-used proxy that must not appear in DataSourceEditor's list of available StreamingAdapters
	public class QuikStreamingPuppet : QuikStreaming {
		string ddeTopicsPrefix;

		public QuikStreamingPuppet(string ddeTopicsPrefix) : base() {
			this.ddeTopicsPrefix	= ddeTopicsPrefix;
			base.DdeServiceName		= this.ddeTopicsPrefix + base.DdeServiceName;
			//base.DdeTopicPrefixDom	= this.ddeTopicsPrefix + base.DdeTopicPrefixDom;
			//base.DdeTopicQuotes		= this.ddeTopicsPrefix + base.DdeTopicQuotes;
			//base.DdeTopicTrades		= this.ddeTopicsPrefix + base.DdeTopicTrades;
		}

		protected override void SubscribeSolidifier() {
			string msg = "OTHERWIZE_BASE_WILL_SUBSCRIBE_SOLIDIFIER PUPPET_ONLY_REGISTERS_DDE_SERVER_AND_PUSHES_QUOTES_TO_STRATEGY";
		}
	}
}
