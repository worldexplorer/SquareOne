using System;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;
using Sq1.Core.Support;

namespace Sq1.Core.Backtesting {
	[SkipInstantiationAt(Startup = true)]
	public class BacktestStreamingProvider : StreamingProvider {
		public BacktestSpreadModeler SpreadModeler;

		public BacktestStreamingProvider() {
			string msg = "We should never be here; skip instantiation by Activator in MainModule::InitializeProviders()";
			//throw new Exception(msg);
		}
		public BacktestStreamingProvider(string symbol) : base() {
			base.Name = "BacktestStreamingProvider";
			this.SpreadModeler = new BacktestSpreadModelerConstant(10);
		}

		public void GeneratedQuoteEnrichSymmetricallyAndPush(QuoteGenerated quote) {
			if (this.SpreadModeler == null) {
				string msg = "Don't leave quoteToReach.Bid and quoteToReach.Ask uninitialized!!!";
				throw new Exception(msg);
			}
			this.SpreadModeler.GeneratedQuoteFillBidAsk(quote);
			base.PushQuoteReceived(quote);
		}

		public override void UpstreamSubscribe(string symbol) {
			base.UpstreamSubscribeRegistryHelper(symbol);
		}
		public override void UpstreamUnSubscribe(string symbol) {
			base.UpstreamUnSubscribeRegistryHelper(symbol);
		}
		public override bool UpstreamIsSubscribed(string symbol) {
			return base.UpstreamIsSubscribedRegistryHelper(symbol);
		}

		public override string ToString() {
			return Name + ": DataSource[" + this.DataSource + "]";
		}

	}
}
