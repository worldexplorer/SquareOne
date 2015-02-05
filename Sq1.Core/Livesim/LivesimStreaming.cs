using System;
using System.Threading;

using Sq1.Core.Backtesting;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;
using Sq1.Core.Support;

namespace Sq1.Core.Livesim {
	[SkipInstantiationAt(Startup = true)]
	public class LivesimStreaming : BacktestStreaming {
		public ManualResetEvent StreamingLivesimUnpaused { get; private set; }

		public LivesimStreaming() : base() {
			base.Name = "StreamingLivesim";
			this.StreamingLivesimUnpaused = new ManualResetEvent(true);
		}

		public override void GeneratedQuoteEnrichSymmetricallyAndPush(QuoteGenerated quote, Bar bar2simulate, double priceForSymmetricFillAtOpenOrClose = -1) {
			bool isUnpaused = this.StreamingLivesimUnpaused.WaitOne(0);
			if (isUnpaused == false) {
				this.StreamingLivesimUnpaused.WaitOne();
			}
			Thread.Sleep(10);
			base.GeneratedQuoteEnrichSymmetricallyAndPush(quote, bar2simulate, priceForSymmetricFillAtOpenOrClose);
		}

	}
}
