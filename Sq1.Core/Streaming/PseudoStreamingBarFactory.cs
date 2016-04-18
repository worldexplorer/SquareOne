using System;

using Sq1.Core.DataTypes;
using Sq1.Core.Backtesting;

namespace Sq1.Core.Streaming {
	public class PseudoStreamingBarFactory {
				string				symbol;									//{ get; private set; }
				BarScaleInterval	scaleInterval;							//{ get; set; }
				int					intraBarSerno;							//{ get; private set; }
		public	Bar					PseudoStreamingBar_unattached			{ get; private set; }
				SymbolInfo			symbolInfo;

		public Bar					BarLastFormedUnattached_nullUnsafe		{ get; protected set; }
		public bool					BarLastFormed_unattached_notYetFormed	{ get {
			return
					this.BarLastFormedUnattached_nullUnsafe == null
				||	this.BarLastFormedUnattached_nullUnsafe.DateTimeOpen == DateTime.MinValue
				||	double.IsNaN(this.BarLastFormedUnattached_nullUnsafe.Close);
		} }

		public PseudoStreamingBarFactory(string symbol_passed, BarScaleInterval scaleInterval_passed) {
			symbol			= symbol_passed;
			scaleInterval	= scaleInterval_passed;
			intraBarSerno	= 0;
			symbolInfo		= Assembler.InstanceInitialized.RepositorySymbolInfos.FindSymbolInfoOrNew(this.symbol);

			BarLastFormedUnattached_nullUnsafe = null;
			PseudoStreamingBar_unattached		= new Bar(this.symbol, this.scaleInterval, DateTime.MinValue);
		}
		public Quote Quote_cloneBind_toUnattachedPseudoStreamingBar_enrichWithIntrabarSerno_updateStreamingBar_createNewBar(Quote quoteDequeued_singleInstance) {
			bool imNotForming_NaNbar = double.IsNaN(quoteDequeued_singleInstance.TradedPrice)	//SAME_BUT_NAN_IS_CLEARER || quote2bCloned.TradedAt == BidOrAsk.UNKNOWN
				|| quoteDequeued_singleInstance.TradedPrice <= 0;
			if (imNotForming_NaNbar) {
				string msg = "CANT_FILL_STREAMING_CLOSE_FROM_BID_OR_ASK_UNKNOWN quote.PriceLastDeal[" + quoteDequeued_singleInstance.TradedPrice + "];"
					+ "what kind of quote is that?... (" + quoteDequeued_singleInstance + ")";
				Assembler.PopupException(msg, null, false);
				return null;
			}

			if (this.PseudoStreamingBar_unattached.Symbol != quoteDequeued_singleInstance.Symbol) {
				string msg = "StreamingBar.Symbol=[" + this.PseudoStreamingBar_unattached.Symbol + "]!=quote.Symbol["
					+ quoteDequeued_singleInstance.Symbol + "] (" + quoteDequeued_singleInstance + ")";
				Assembler.PopupException(msg, null, false);
				return null;
			}

			if (quoteDequeued_singleInstance is QuoteGenerated) {
				string msg = "IM_NOT_SERVING_BACKTESTS_AND_LIVEIMS_WITHOUT_OWN_IMPLEMENTATION"
					+ " QUIK_LIVESIM_OWN_IMPLEMENTATION_USES_DDE_AND_I_PROCESS_QuoteQuik_HERE";
				Assembler.PopupException(msg);
				return null;
			}

			string source = quoteDequeued_singleInstance.Source;
			Quote quoteCloneUU = quoteDequeued_singleInstance.Clone_asCoreQuote();
			quoteCloneUU.Source = source + " QCLONE_FOR_" + this.scaleInterval;

			if (quoteCloneUU.ServerTime >= this.PseudoStreamingBar_unattached.DateTime_nextBarOpen_unconditional) {
				if (this.BarLastFormedUnattached_nullUnsafe != null && this.BarLastFormedUnattached_nullUnsafe.DateTimeOpen == DateTime.MinValue) {
					string msg = "beware! on the very first quote LastBarFormed.DateTimeOpen == DateTime.MinValue";
				}
				this.BarLastFormedUnattached_nullUnsafe = this.PseudoStreamingBar_unattached.Clone();

				//v1
				//this.PseudoStreamingBar_unattached			= new Bar(this.symbol, this.scaleInterval, quoteCloneUU.ServerTime);
				//this.PseudoStreamingBar_unattached.Open		= quoteCloneUU.TradedPrice;
				//this.PseudoStreamingBar_unattached.High		= quoteCloneUU.TradedPrice;
				//this.PseudoStreamingBar_unattached.Low		= quoteCloneUU.TradedPrice;
				//this.PseudoStreamingBar_unattached.Close	= quoteCloneUU.TradedPrice;
				//this.PseudoStreamingBar_unattached.Volume	= quoteCloneUU.Size;
				//v2
				this.PseudoStreamingBar_unattached = new Bar(this.symbol, this.scaleInterval,
					quoteCloneUU.ServerTime,
					quoteCloneUU.TradedPrice,
					quoteCloneUU.Size,
					this.symbolInfo);

				this.intraBarSerno = 0;

			} else {
				if (double.IsNaN(this.PseudoStreamingBar_unattached.Open) || this.PseudoStreamingBar_unattached.Open == 0.0) {
					string msg = "we should've had StreamingBar already initialized with first quote of a bar"
						+ "; should only happen on first quote of first bar ever of a symbol freshly added to DataSource";
					if (double.IsNaN(quoteCloneUU.TradedPrice)) {
						msg = "INITIALIZED_OPEN_WITH_NAN_FROM_quoteClone.LastDealPrice " + msg;
					}
					Assembler.PopupException(msg, null, false);
					this.PseudoStreamingBar_unattached.Open = quoteCloneUU.TradedPrice;
				}
				if (quoteCloneUU.Size <= 0) {
					string msg = "QUOTE_WITHOUT_SIZE_SHOULD_NOT_GO_TO_BAR";
					Assembler.PopupException(msg);
				}
				// spread can be anywhere outside the bar; but a bar freezes only traded spreads inside (Quotes DDE table from Quik, not Level2-generated with Size=0)
				bool barExpanded = this.PseudoStreamingBar_unattached.MergeExpandHLCV_forStreamingBarUnattached(quoteCloneUU);
				this.intraBarSerno++;
			}

			if (quoteCloneUU.IntraBarSerno != -1) {
				string msg = "ARE_YOU_SURE_ITS_REASONABLE_TO_SET_quoteClone.IntraBarSerno_OUTSIDE_PseudoStreamingBarFactory?..";
				Assembler.PopupException(msg, null, true);
			}

			quoteCloneUU.Set_IntraBarSerno__onlyInConsumer(this.intraBarSerno);

			if (quoteCloneUU.ParentBarStreaming != null) {
				string msg = "ARE_YOU_SURE_ITS_REASONABLE_TO_SET_quoteClone.IntraBarSerno_OUTSIDE_PseudoStreamingBarFactory?..";
					//+ " WILL_BIND_THIS_QUOTE_LATER_FOR_EACH_CONSUMER_TO_ITS_OWN_BAR_STREAMING_AND_ITS_PARENT_BARS"
					//+ " QUOTE_ALREADY_ENRICHED_WITH_PARENT_STREAMING_BAR; I think it's a pre- bindStreamingBarForQueue() atavism"
					;
				Assembler.PopupException(msg);
			}
			Quote quoteClone_bound = quoteCloneUU;
			quoteClone_bound.StreamingBar_Replace(this.PseudoStreamingBar_unattached, true);
	
			return quoteClone_bound;
		}
		public void InitWithStreamingBar_insteadOfEmpty(Bar StreamingBar_insteadOfEmpty) {
			string msg = "";
			if (StreamingBar_insteadOfEmpty.DateTimeOpen <= this.PseudoStreamingBar_unattached.DateTimeOpen) {
				msg += "StreamingBar_insteadOfEmpty.DateTimeOpen[" + StreamingBar_insteadOfEmpty.DateTimeOpen
					+ "] <= CurrentStreamingBar.Open[" + this.PseudoStreamingBar_unattached.Open + "]";
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
			this.PseudoStreamingBar_unattached = StreamingBar_insteadOfEmpty.Clone();
			if (string.IsNullOrEmpty(msg) == false) {
				Assembler.PopupException("InitWithStreamingBar_insteadOfEmpty: " + msg + " // " + this);
			}
		}
		public override string ToString() {
			return this.symbol + "_" + this.scaleInterval.ToString() + ":StreamingBar[" + this.PseudoStreamingBar_unattached.ToString() + "]";
		}

	}
}