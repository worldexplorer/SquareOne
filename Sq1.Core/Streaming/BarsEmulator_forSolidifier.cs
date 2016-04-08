using System;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public class BarsEmulator_forSolidifier {
				string				symbol;									//{ get; private set; }
				BarScaleInterval	scaleInterval;							//{ get; set; }
				int					intraBarSerno;							//{ get; private set; }
		public	Bar					PseudoBarStreaming_unattached			{ get; private set; }

		public Bar					BarLastFormedUnattached_nullUnsafe		{ get; protected set; }
		public bool					BarLastFormed_unattached_notYetFormed	{ get {
			return
					this.BarLastFormedUnattached_nullUnsafe == null
				||	this.BarLastFormedUnattached_nullUnsafe.DateTimeOpen == DateTime.MinValue
				||	double.IsNaN(this.BarLastFormedUnattached_nullUnsafe.Close);
		} }

		public BarsEmulator_forSolidifier(string symbol_passed, BarScaleInterval scaleInterval_passed) {
			symbol			= symbol_passed;
			scaleInterval	= scaleInterval_passed;
			BarLastFormedUnattached_nullUnsafe = null;
			PseudoBarStreaming_unattached = new Bar(this.symbol, this.scaleInterval, DateTime.MinValue);
			intraBarSerno	= 0;
		}
		public virtual Quote Quote_cloneBind_toUnattachedPseudoStreamingBar_enrichWithIntrabarSerno_updateStreamingBar_createNewBar(Quote quote2bCloned) {
			//if (quote2bCloned.TradedAt == BidOrAsk.UNKNOWN) {
			if (quote2bCloned.TradedPrice == double.NaN || quote2bCloned.TradedPrice <= 0) {
				string msg = "CANT_FILL_STREAMING_CLOSE_FROM_BID_OR_ASK_UNKNOWN quote.PriceLastDeal[" + quote2bCloned.TradedPrice + "];"
					+ "what kind of quote is that?... (" + quote2bCloned + ")";
				Assembler.PopupException(msg, null, false);
				//return null;
			}

			if (this.PseudoBarStreaming_unattached.Symbol != quote2bCloned.Symbol) {
				string msg = "StreamingBar.Symbol=[" + this.PseudoBarStreaming_unattached.Symbol + "]!=quote.Symbol["
					+ quote2bCloned.Symbol + "] (" + quote2bCloned + ")";
				Assembler.PopupException(msg, null, false);
				return null;
			}

			string source = quote2bCloned.Source;
			Quote quoteClone_unbound = quote2bCloned.Clone_asCoreQuote();
			quoteClone_unbound.Source = source + " QCLONE_FOR_" + this.scaleInterval;

			if (quoteClone_unbound.ServerTime >= this.PseudoBarStreaming_unattached.DateTimeNextBarOpenUnconditional) {
				if (this.BarLastFormedUnattached_nullUnsafe != null && this.BarLastFormedUnattached_nullUnsafe.DateTimeOpen == DateTime.MinValue) {
					string msg = "beware! on the very first quote LastBarFormed.DateTimeOpen == DateTime.MinValue";
				}
				this.BarLastFormedUnattached_nullUnsafe = this.PseudoBarStreaming_unattached.Clone();

				this.PseudoBarStreaming_unattached		= new Bar(this.symbol, this.scaleInterval, quoteClone_unbound.ServerTime);
				this.PseudoBarStreaming_unattached.Open	= quoteClone_unbound.TradedPrice;
				this.PseudoBarStreaming_unattached.High	= quoteClone_unbound.TradedPrice;
				this.PseudoBarStreaming_unattached.Low	= quoteClone_unbound.TradedPrice;
				this.PseudoBarStreaming_unattached.Close	= quoteClone_unbound.TradedPrice;
				this.PseudoBarStreaming_unattached.Volume	= quoteClone_unbound.Size;
				this.intraBarSerno = 0;

				// quoteClone.IntraBarSerno doesn't feel new Bar; can contain 100004 for generatedQuotes;
				// I only want to reset to 0 when it's attributed to a new Bar; it's unlikely to face a new bar here for generatedQuotes;
				if (quoteClone_unbound.IntraBarSerno != 0) {
					if (quoteClone_unbound.IamInjectedToFillPendingAlerts) {
						string msg = "NEVER_HAPPENED_SO_FAR GENERATED_QUOTES_ARENT_SUPPOSED_TO_GO_TO_NEXT_BAR";
						Assembler.PopupException(msg);
					}
					// moved down as a final result of "Enriching": quoteClone.IntraBarSerno = this.IntraBarSerno;
				}
				if (this.intraBarSerno >= Quote.IntraBarSernoShift_forGenerated_towardsPendingFill) {
					string msg = "NEVER_HAPPENED_SO_FAR BAR_FACTORY_INTRABAR_SERNO_NEVER_GOES_TO_SYNTHETIC_ZONE";
					Assembler.PopupException(msg);
				}
			} else {
				if (double.IsNaN(this.PseudoBarStreaming_unattached.Open) || this.PseudoBarStreaming_unattached.Open == 0.0) {
					string msg = "we should've had StreamingBar already initialized with first quote of a bar"
						+ "; should only happen on first quote of first bar ever of a symbol freshly added to DataSource";
					if (double.IsNaN(quoteClone_unbound.TradedPrice)) {
						msg = "INITIALIZED_OPEN_WITH_NAN_FROM_quoteClone.LastDealPrice " + msg;
					}
					Assembler.PopupException(msg, null, false);
					this.PseudoBarStreaming_unattached.Open = quoteClone_unbound.TradedPrice;
					//this.StreamingBarUnattached.High = quoteClone.LastDealPrice;
					//this.StreamingBarUnattached.Low = quoteClone.LastDealPrice;
				}
				if (quoteClone_unbound.Size > 0) {
					// spread can be anywhere outside the bar; but a bar freezes only traded spreads inside (Quotes DDE table from Quik, not Level2-generated with Size=0)
					bool barExpanded = this.PseudoBarStreaming_unattached.MergeExpandHLCV_forStreamingBarUnattached(quoteClone_unbound);
				}
				this.intraBarSerno++;
			}

			if (quoteClone_unbound.IntraBarSerno != -1) {
				string msg = "ARE_YOU_SURE_ITS_REASONABLE_TO_SET_quoteClone.IntraBarSerno_OUTSIDE_StreamingBarFactory?..";
				Assembler.PopupException(msg, null, true);
			}

			quoteClone_unbound.IntraBarSerno  = this.intraBarSerno;

			if (quoteClone_unbound.ParentBarStreaming != null) {
				string msg = "ARE_YOU_SURE_ITS_REASONABLE_TO_SET_quoteClone.IntraBarSerno_OUTSIDE_StreamingBarFactory?..";
					//+ " WILL_BIND_THIS_QUOTE_LATER_FOR_EACH_CONSUMER_TO_ITS_OWN_BAR_STREAMING_AND_ITS_PARENT_BARS"
					//+ " QUOTE_ALREADY_ENRICHED_WITH_PARENT_STREAMING_BAR; I think it's a pre- bindStreamingBarForQueue() atavism"
					;
				Assembler.PopupException(msg);
			}
			Quote quoteClone_bound = quoteClone_unbound;
			quoteClone_bound.StreamingBar_Replace(this.PseudoBarStreaming_unattached, true);
	
			return quoteClone_bound;
		}
		public void InitWithStreamingBar_insteadOfEmpty(Bar StreamingBar_insteadOfEmpty) {
			string msg = "";
			if (StreamingBar_insteadOfEmpty.DateTimeOpen <= this.PseudoBarStreaming_unattached.DateTimeOpen) {
				msg += "StreamingBar_insteadOfEmpty.DateTimeOpen[" + StreamingBar_insteadOfEmpty.DateTimeOpen
					+ "] <= CurrentStreamingBar.Open[" + this.PseudoBarStreaming_unattached.Open + "]";
				//log.Warn(msg + " // " + this);
				return;
			}
			if (StreamingBar_insteadOfEmpty.DateTimeOpen == DateTime.MinValue) {
				msg += "StreamingBar_insteadOfEmpty.DateTimeOpen[" + StreamingBar_insteadOfEmpty.DateTimeOpen + "] == DateTime.MinValue ";
			}
			if (double.IsNaN(StreamingBar_insteadOfEmpty.Open)) {
				msg += "double.IsNaN(StreamingBar_insteadOfEmpty.Open[" + StreamingBar_insteadOfEmpty.Open + "]) ";
			}
			if (StreamingBar_insteadOfEmpty.Open == 0) {
				msg += "StreamingBar_insteadOfEmpty.Open[" + StreamingBar_insteadOfEmpty.Open + "] == 0 ";
			}
			this.PseudoBarStreaming_unattached = StreamingBar_insteadOfEmpty.Clone();
			if (string.IsNullOrEmpty(msg) == false) {
				Assembler.PopupException("InitWithStreamingBar_insteadOfEmpty: " + msg + " // " + this);
			}
		}
		public override string ToString() {
			return this.symbol + "_" + this.scaleInterval.ToString() + ":StreamingBar[" + this.PseudoBarStreaming_unattached.ToString() + "]";
		}

	}
}