using System;

using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;
using Sq1.Core.Support;

namespace Sq1.Core.Backtesting {
	[SkipInstantiationAt(Startup = true)]
	public class BacktestStreaming : StreamingAdapter {
		public BacktestSpreadModeler SpreadModeler;
		public const double PERCENTAGE_DEFAULT= 0.005;

//		public BacktestStreamingAdapter() {
//			string msg = "We should never be here; skip instantiation by Activator in MainModule::InitializeProviders()";
//			//throw new Exception(msg);
//		}
		public BacktestStreaming() : base() {
			base.Name = "BacktestStreamingAdapter";
//			this.InitializeSpreadModelerPercentage(PERCENTAGE_DEFAULT);
//		}
//		public void InitializeSpreadModelerPercentage(double pct) {
			//greater than BacktestSpreadModelerPercentageOfMedian(0.01) will make ATRband inconsistent! you'll see in TooltipPrice (Close+ATR != C+Upper) & SPREAD_MODELER_SHOULD_GENERATE_TIGHTER_SPREADS
			//for medianPrice[80.36],percentageOfMedian[0.01] => spread[0.008036] => Bid[~80.35598],Ask[~80.36402]
			this.SpreadModeler = new BacktestSpreadModelerPercentage(PERCENTAGE_DEFAULT);
			base.QuotePumpSeparatePushingThreadEnabled = false;
		}

		public virtual void GeneratedQuoteEnrichSymmetricallyAndPush(QuoteGenerated quote, Bar bar2simulate, double priceForSymmetricFillAtOpenOrClose = -1) {
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
