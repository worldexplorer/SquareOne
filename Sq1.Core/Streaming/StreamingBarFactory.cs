using System;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public class StreamingBarFactory {
		public int IntraBarSerno { get; private set; }
		public Bar StreamingBar { get; private set; }
		public Bars Bars { get; private set; }
		
		public StreamingBarFactory(Bars bars) {
			Bars = bars;
			StreamingBarUnattached = this.Bars.;
			IntraBarSerno = 0;
		}
		public virtual Quote EnrichQuoteWithSernoUpdateStreamingBarCreateNewBar(Quote quoteClone) {
			if (quoteClone.PriceLastDeal == 0) {
				string msg = "quote.PriceLastDeal[" + quoteClone.PriceLastDeal + "] == 0;"
					+ "what kind of quote is that?... (" + quoteClone + ")";
				throw new Exception(msg);
				//return;
			}

			if (StreamingBar.Symbol != quoteClone.Symbol) {
				string msg = "StreamingBar.Symbol=[" + StreamingBar.Symbol + "]!=quote.Symbol["
					+ quoteClone.Symbol + "] (" + quoteClone + ")";
				throw new Exception(msg);
				//return;
			}

			// included in if (quoteClone.ServerTime >= StreamingBar.DateTimeNextBarOpenUnconditional) !!!
			// on very first quote StreamingBar.DateTimeNextBarOpenUnconditional = DateTime.MinValue
			//SEE_BELOW if (StreamingBar.DateTimeOpen == DateTime.MinValue)
			//SEE_BELOW 	this.initStreamingBarResetIntraBarSerno(quoteClone.ServerTime, quoteClone.PriceLastDeal, quoteClone.Size);
			//SEE_BELOW }

			if (quoteClone.ServerTime >= StreamingBar.DateTimeNextBarOpenUnconditional) {
				LastBarFormedUnattached = StreamingBar.Clone();	//beware! on very first quote LastBarFormed.DateTimeOpen == DateTime.MinValue
				initStreamingBarResetIntraBarSerno(quoteClone.ServerTime, quoteClone.PriceLastDeal, quoteClone.Size);
			} else {
				if (Double.IsNaN(StreamingBar.Open) || StreamingBar.Open == 0.0) {
					throw new Exception("nonsense! we should've had StreamingBar already initialized with first quote of a bar");
					//log.Warn("Initializing OHL as quote.PriceLastDeal[" + quoteClone.PriceLastDeal + "];"
					//	+ " following previous InitWithStreamingBarInsteadOfEmpty message"
					//	+ " (if absent then never initialized)");
					//StreamingBar.Open = quoteClone.PriceLastDeal;
					//StreamingBar.High = quoteClone.PriceLastDeal;
					//StreamingBar.Low = quoteClone.PriceLastDeal;
				}
				if (quoteClone.PriceLastDeal > StreamingBar.High) StreamingBar.High = quoteClone.PriceLastDeal;
				if (quoteClone.PriceLastDeal < StreamingBar.Low) StreamingBar.Low = quoteClone.PriceLastDeal;
				StreamingBar.Close = quoteClone.PriceLastDeal;
				StreamingBar.Volume += quoteClone.Size;
				IntraBarSerno++;
			}
			quoteClone.IntraBarSerno = IntraBarSerno;
			quoteClone.SetParentBar(StreamingBar);
			return quoteClone;
		}
		public override string ToString() {
			return Symbol + "_" + ScaleInterval + ":StreamingBar[" + StreamingBar + "]";
		}
	}
}
