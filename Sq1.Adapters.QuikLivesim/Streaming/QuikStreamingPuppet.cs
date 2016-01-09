using Sq1.Core;
using Sq1.Core.Support;
using Sq1.Core.Streaming;
using Sq1.Core.Backtesting;
using Sq1.Core.DataTypes;

using Sq1.Adapters.Quik;

namespace Sq1.Adapters.Quik.Broker.Livesim {
	[SkipInstantiationAt(Startup = true)]		// I'm an internally-used proxy that must not appear in DataSourceEditor's list of available StreamingAdapters
	public class QuikStreamingPuppet : QuikStreaming {
		string ddeTopicsPrefix;

		public QuikStreamingPuppet(string ddeTopicsPrefix, DataDistributor dataDistributor) : base() {
			this.ddeTopicsPrefix	= ddeTopicsPrefix;
			base.DdeServiceName		= this.ddeTopicsPrefix + base.DdeServiceName;
			//base.DdeTopicPrefixDom	= this.ddeTopicsPrefix + base.DdeTopicPrefixDom;
			//base.DdeTopicQuotes		= this.ddeTopicsPrefix + base.DdeTopicQuotes;
			//base.DdeTopicTrades		= this.ddeTopicsPrefix + base.DdeTopicTrades;
			base.DataDistributor	= dataDistributor;		// PushQuote wont be invoked for QuikLiveSim, but the puppet should invoke Exector instead
		}

		public override void PushQuoteReceived(Quote quote) {
			if (this.DataDistributor.DistributionChannels.Count == 0) {
				string msg = "MY_DISTRIBUTOR_MUST_BE_SUBSCRIBED_TO_LIVESIM_CONSUMERS__AFTER_QUOTE_IS_RECEIVED_BY_DDE_SERVER";
				Assembler.PopupException(msg);
			}
			base.PushQuoteReceived(quote);
		}

		protected override void SubscribeSolidifier() {
			string msg = "OTHERWIZE_BASE_WILL_SUBSCRIBE_SOLIDIFIER PUPPET_ONLY_REGISTERS_DDE_SERVER_AND_PUSHES_QUOTES_TO_STRATEGY";
		}
	}
}
