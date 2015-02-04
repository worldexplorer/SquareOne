using System;

using Sq1.Core.DataTypes;
using System.Diagnostics;

namespace Sq1.Core.Streaming {
	public class StreamingBarFactoryUnattached {
		public string			Symbol								{ get; private set; }
		public BarScaleInterval ScaleInterval						{ get; set; }
		public int				IntraBarSerno						{ get; private set; }
		public Bar				BarStreamingUnattached		{ get; private set; }

		public Bar				BarLastFormedUnattachedNullUnsafe	{ get; protected set; }
		public bool				BarLastFormedUnattachedNotYetFormed { get {
			return
					this.BarLastFormedUnattachedNullUnsafe == null
				||	this.BarLastFormedUnattachedNullUnsafe.DateTimeOpen == DateTime.MinValue
				||	double.IsNaN(this.BarLastFormedUnattachedNullUnsafe.Close);
		} }

		public StreamingBarFactoryUnattached(string symbol, BarScaleInterval scaleInterval) {
			Symbol = symbol;
			ScaleInterval = scaleInterval;
			BarLastFormedUnattachedNullUnsafe = null;
			BarStreamingUnattached = new Bar(this.Symbol, this.ScaleInterval, DateTime.MinValue);
			IntraBarSerno = 0;
		}
		public virtual Quote EnrichQuoteWithSernoUpdateStreamingBarCreateNewBar(Quote quoteClone) {
			if (quoteClone.LastDealBidOrAsk == BidOrAsk.UNKNOWN) {
				string msg = "CANT_FILL_STREAMING_CLOSE_FROM_BID_OR_ASK_UNKNOWN quote.PriceLastDeal[" + quoteClone.LastDealPrice + "];"
					+ "what kind of quote is that?... (" + quoteClone + ")";
				Debugger.Break();
				throw new Exception(msg);
				//return;
			}

			if (this.BarStreamingUnattached.Symbol != quoteClone.Symbol) {
				string msg = "StreamingBar.Symbol=[" + this.BarStreamingUnattached.Symbol + "]!=quote.Symbol["
					+ quoteClone.Symbol + "] (" + quoteClone + ")";
				throw new Exception(msg);
				//return;
			}

			// included in if (quoteClone.ServerTime >= StreamingBar.DateTimeNextBarOpenUnconditional) !!!
			// on very first quote StreamingBar.DateTimeNextBarOpenUnconditional = DateTime.MinValue
			//SEE_BELOW if (StreamingBar.DateTimeOpen == DateTime.MinValue)
			//SEE_BELOW 	this.initStreamingBarResetIntraBarSerno(quoteClone.ServerTime, quoteClone.PriceLastDeal, quoteClone.Size);
			//SEE_BELOW }

			if (quoteClone.ServerTime >= this.BarStreamingUnattached.DateTimeNextBarOpenUnconditional) {
				if (this.BarLastFormedUnattachedNullUnsafe != null && this.BarLastFormedUnattachedNullUnsafe.DateTimeOpen == DateTime.MinValue) {
					string msg = "beware! on very first quote LastBarFormed.DateTimeOpen == DateTime.MinValue";
				}
				this.BarLastFormedUnattachedNullUnsafe = this.BarStreamingUnattached.Clone();

				this.BarStreamingUnattached			= new Bar(this.Symbol, this.ScaleInterval, quoteClone.ServerTime);
				this.BarStreamingUnattached.Open	= quoteClone.LastDealPrice;
				this.BarStreamingUnattached.High	= quoteClone.LastDealPrice;
				this.BarStreamingUnattached.Low		= quoteClone.LastDealPrice;
				this.BarStreamingUnattached.Close	= quoteClone.LastDealPrice;
				this.BarStreamingUnattached.Volume	= quoteClone.Size;
				this.IntraBarSerno = 0;

				// quoteClone.IntraBarSerno doesn't feel new Bar; can contain 100004 for generatedQuotes;
				// I only want to reset to 0 when it's attributed to a new Bar; it's unlikely to face a new bar here for generatedQuotes;
				if (this.IntraBarSerno != 0) {
					string msg = "STREAMING_JUST_INITED_TO_ZERO WHY_NOW_IT_IS_NOT?";
					Debugger.Break();
				}
				if (quoteClone.IntraBarSerno != 0) {
					if (quoteClone.IntraBarSerno >= Quote.IntraBarSernoShiftForGeneratedTowardsPendingFill) {
						string msg = "GENERATED_QUOTES_ARENT_SUPPOSED_TO_GO_TO_NEXT_BAR";
						Debugger.Break();
					}
					// moved down as final result of "Enriching" quoteClone.IntraBarSerno = this.IntraBarSerno;
				}
				if (this.IntraBarSerno >= Quote.IntraBarSernoShiftForGeneratedTowardsPendingFill) {
					string msg = "BAR_FACTORY_INTRABAR_SERNO_NEVER_GOES_TO_SYNTHETIC_ZONE";
					Debugger.Break();
				}
			} else {
				if (double.IsNaN(this.BarStreamingUnattached.Open) || this.BarStreamingUnattached.Open == 0.0) {
					string msg = "we should've had StreamingBar already initialized with first quote of a bar"
						+ "; should only happen on first quote of first bar ever of a symbol freshly added to DataSource";
					if (double.IsNaN(quoteClone.LastDealPrice)) {
						msg = "INITIALIZED_OPEN_WITH_NAN_FROM_quoteClone.LastDealPrice " + msg;
					}
					Assembler.PopupException(msg, null, false);
					this.BarStreamingUnattached.Open = quoteClone.LastDealPrice;
					//this.StreamingBarUnattached.High = quoteClone.LastDealPrice;
					//this.StreamingBarUnattached.Low = quoteClone.LastDealPrice;
				}
				this.BarStreamingUnattached.MergeExpandHLCVforStreamingBarUnattached(quoteClone);
				this.IntraBarSerno++;
			}
			if (quoteClone.ParentBarStreaming != null) {
				string msg = "QUOTE_ALREADY_ENRICHED_WITH_PARENT_STREAMING_BAR; I think it's a pre- bindStreamingBarForQueue() atavism";
				//Assembler.PopupException(msg);
			} else {
				quoteClone.SetParentBarStreaming(this.BarStreamingUnattached);
			}
			
			if (quoteClone.IntraBarSerno == -1) {
				quoteClone.IntraBarSerno  = this.IntraBarSerno;
			} else {
				string msg = "ARE_YOU_SURE_ITS_REASONABLE_TO_SET_quoteClone.IntraBarSerno_OUTSIDE_StreamingBarFactory?..";
				Assembler.PopupException(msg, null, false);
			}
			return quoteClone;
		}
		public void InitWithStreamingBarInsteadOfEmpty(Bar StreamingBarInsteadOfEmpty) {
			string msg = "";
			if (StreamingBarInsteadOfEmpty.DateTimeOpen <= this.BarStreamingUnattached.DateTimeOpen) {
				msg += "StreamingBarInsteadOfEmpty.DateTimeOpen[" + StreamingBarInsteadOfEmpty.DateTimeOpen
					+ "] <= CurrentStreamingBar.Open[" + this.BarStreamingUnattached.Open + "]";
				//log.Warn(msg + " // " + this);
				return;
			}
			if (StreamingBarInsteadOfEmpty.DateTimeOpen == DateTime.MinValue) {
				msg += "StreamingBarInsteadOfEmpty.DateTimeOpen[" + StreamingBarInsteadOfEmpty.DateTimeOpen + "] == DateTime.MinValue ";
			}
			if (double.IsNaN(StreamingBarInsteadOfEmpty.Open)) {
				msg += "double.IsNaN(StreamingBarInsteadOfEmpty.Open[" + StreamingBarInsteadOfEmpty.Open + "]) ";
			}
			if (StreamingBarInsteadOfEmpty.Open == 0) {
				msg += "StreamingBarInsteadOfEmpty.Open[" + StreamingBarInsteadOfEmpty.Open + "] == 0 ";
			}
			this.BarStreamingUnattached = StreamingBarInsteadOfEmpty.Clone();
			if (string.IsNullOrEmpty(msg) == false) {
				//log.Warn("InitWithStreamingBarInsteadOfEmpty: " + msg + " // " + this);
			}
		}
		public override string ToString() {
			return this.Symbol + "_" + this.ScaleInterval.ToString() + ":StreamingBar[" + this.BarStreamingUnattached.ToString() + "]";
		}

		internal void AbsorbBarLastStaticFromChannelBacktesterComplete(SymbolScaleDistributionChannel channelBacktest) {
			string msg = this.BarLastFormedUnattachedNullUnsafe.ToString();
			this.BarLastFormedUnattachedNullUnsafe = channelBacktest.StreamingBarFactoryUnattached.BarLastFormedUnattachedNullUnsafe.CloneDetached();
			msg += " => " + this.BarLastFormedUnattachedNullUnsafe.ToString();
			Assembler.PopupException(msg, null, false);
		}
// KEEP_THIS_NOT_HAPPENING_BY_LEAVING_STATIC_LAST_ON_APPRESTART_NULL_ON_LIVEBACKTEST_CONTAINING_LAST_INCOMING_QUOTE
//		internal void AbsorbBarStreamingFromChannel(SymbolScaleDistributionChannel channelBacktest) {
//			this.BarStreamingUnattached = channelBacktest.StreamingBarFactoryUnattached.BarStreamingUnattached.CloneDetached();
//		}
	}
}