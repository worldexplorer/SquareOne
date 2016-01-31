using System;

using Newtonsoft.Json;

using Sq1.Core.Streaming;
using Sq1.Core.Support;

namespace Sq1.Core.Backtesting {
	[SkipInstantiationAt(Startup = true)]
	public class BacktestStreaming : StreamingAdapter {
		// without [JsonIgnore] Livesim children will have these properties in JSON
		[JsonIgnore] public BacktestSpreadModeler SpreadModeler;
		[JsonIgnore] public const double PERCENTAGE_DEFAULT= 0.005;

		public BacktestStreaming(string reasonToExist) : base(reasonToExist) {
			base.Name = "BacktestStreamingAdapter";
//			this.InitializeSpreadModelerPercentage(PERCENTAGE_DEFAULT);
//		}
//		public void InitializeSpreadModelerPercentage(double pct) {
			//greater than BacktestSpreadModelerPercentageOfMedian(0.01) will make ATRband inconsistent! you'll see in TooltipPrice (Close+ATR != C+Upper) & SPREAD_MODELER_SHOULD_GENERATE_TIGHTER_SPREADS
			//for medianPrice[80.36],percentageOfMedian[0.01] => spread[0.008036] => Bid[~80.35598],Ask[~80.36402]
			this.SpreadModeler = new BacktestSpreadModelerPercentage(PERCENTAGE_DEFAULT);
			base.QuotePumpSeparatePushingThreadEnabled = false;
		}

		public virtual void PushQuoteGenerated(QuoteGenerated quote) {
			if (this.SpreadModeler == null) {
				string msg = "Don't leave quoteToReach.Bid and quoteToReach.Ask uninitialized!!!";
				throw new Exception(msg);
			}
			//ALREADY_FILLED_BY_GENERATOR this.SpreadModeler.GeneratedQuoteFillBidAsk(quote, bar2simulate, priceForSymmetricFillAtOpenOrClose);
			base.PushQuoteReceived(quote);
			quote.WentThroughStreamingToScript = true;
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

		//public override string ToString() {
		//	string dataSourceAsString = this.DataSource != null ? this.DataSource.ToString() : "NOT_INITIALIZED_YET";
		//	string ret = this.Name + ": "
		//		//+ "DataSource[" + dataSourceAsString + "]"
		//		+ base.ToString()
		//		;
		//	return ret;
		//}

	}
}
