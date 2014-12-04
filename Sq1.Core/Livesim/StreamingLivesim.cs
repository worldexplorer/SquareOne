using System;

using Sq1.Core.Backtesting;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;
using Sq1.Core.Support;

namespace Sq1.Core.Livesim {
	[SkipInstantiationAt(Startup = true)]
	public class StreamingLivesim : StreamingProvider {
		public BacktestSpreadModeler SpreadModeler;
		public const double PERCENTAGE_DEFAULT= 0.005;

		public StreamingLivesim() : base() {
			base.Name = "StreamingLivesim";
			this.SpreadModeler = new BacktestSpreadModelerPercentage(PERCENTAGE_DEFAULT);
		}

		public void GeneratedQuoteEnrichSymmetricallyAndPush(QuoteGenerated quote, Bar bar2simulate, double priceForSymmetricFillAtOpenOrClose = -1) {
			if (this.SpreadModeler == null) {
				string msg = "Don't leave quoteToReach.Bid and quoteToReach.Ask uninitialized!!!";
				throw new Exception(msg);
			}
			//ALREADY_FILLED_BY_GENERATOR this.SpreadModeler.GeneratedQuoteFillBidAsk(quote, bar2simulate, priceForSymmetricFillAtOpenOrClose);
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
