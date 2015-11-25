using System;

using Sq1.Core.DataTypes;
using System.Text;

namespace Sq1.Core.Backtesting {
	public class QuoteGenerated : Quote {
		public Bar			ParentBarSimulated;
		public bool			HasParentBarSimulated			{ get { return this.ParentBarSimulated != null; } }
		public new string	ParentBarIdent					{ get { return (this.HasParentBarSimulated) ? this.ParentBarSimulated.ParentBarsIdent : "NO_PARENT_BAR_SIMULATED"; } }
		public double		SpreadAligned					{ get {
				if (this.ParentBarSimulated == null) {
					return base.Spread;
				}
				if (this.ParentBarSimulated.ParentBars == null) {
					return base.Spread;
				}
				if (this.ParentBarSimulated.ParentBars.SymbolInfo == null) {
					return base.Spread;
				}
				SymbolInfo symbolInfo = this.ParentBarSimulated.ParentBars.SymbolInfo;
				double ret = symbolInfo.AlignToPriceLevel(base.Spread, PriceLevelRoundingMode.RoundUp);	//RoundUp so I wont' get spread = 0
				return ret;
			} }
		public bool			WentThroughStreamingToScript;
		
		public QuoteGenerated(DateTime localTimeEqualsToServerTimeForGenerated) : base(localTimeEqualsToServerTimeForGenerated) {}

		// only for DDE-transformed QuoteGenerated => QuoteQuik, in the livesim to 
		public QuoteGenerated(Quote quote, Bar parentBarSimulated) : base() {
			this.Symbol					= quote.Symbol;
			this.SymbolClass			= quote.SymbolClass;
			this.Source					= "DERIVED_FROM_" + quote.ToStringShort() + " " + quote.Source;
			this.ServerTime				= quote.ServerTime.AddMilliseconds(911);
			this.LocalTimeCreated		= quote.LocalTimeCreated.AddMilliseconds(911);
			this.TradedAt				= quote.TradedAt;
			this.ItriggeredFillAtBidOrAsk = quote.ItriggeredFillAtBidOrAsk;
			this.Bid					= quote.Bid;
			this.Ask					= quote.Ask;
			this.Size					= quote.Size;
			this.IntraBarSerno			= quote.IntraBarSerno + Quote.IntraBarSernoShiftForGeneratedTowardsPendingFill;
			this.AbsnoPerSymbol			= ++quote.AbsnoPerSymbol;		// HACK_TO_ALLOW_LIVESIM_BROKER_TO_FILL_PENDING_ALERTS
			this.ParentBarSimulated		= parentBarSimulated;	// was there before I noticed "injected quotes don't seem to have ParentBarSimulated"
			this.ParentBarStreaming		= quote.ParentBarStreaming;	// this may fix it injected quotes don't seem to have ParentBarSimulated
		}

		#region SORRY_FOR_THE_MESS__I_NEED_TO_DERIVE_IDENTICAL_ONLY_FOR_GENERATED__IF_YOU_NEED_IT_IN_BASE_QUOTE_MOVE_IT_THERE
		public QuoteGenerated DeriveIdenticalButFresh() {
			QuoteGenerated identicalButFresh = new QuoteGenerated(this.ServerTime);
			identicalButFresh.Symbol				= this.Symbol;
			identicalButFresh.SymbolClass			= this.SymbolClass;
			identicalButFresh.Source				= "DERIVED_FROM_" + this.ToStringShort() + " " + this.Source;
			identicalButFresh.ServerTime			= this.ServerTime.AddMilliseconds(911);
			identicalButFresh.LocalTimeCreated		= this.LocalTimeCreated.AddMilliseconds(911);
			identicalButFresh.TradedAt				= this.TradedAt;
			identicalButFresh.ItriggeredFillAtBidOrAsk = this.ItriggeredFillAtBidOrAsk;
			identicalButFresh.Bid					= this.Bid;
			identicalButFresh.Ask					= this.Ask;
			identicalButFresh.Size					= this.Size;
			identicalButFresh.IntraBarSerno			= this.IntraBarSerno + Quote.IntraBarSernoShiftForGeneratedTowardsPendingFill;
			identicalButFresh.AbsnoPerSymbol		= ++this.AbsnoPerSymbol;		// HACK_TO_ALLOW_LIVESIM_BROKER_TO_FILL_PENDING_ALERTS
			identicalButFresh.ParentBarSimulated = this.ParentBarSimulated;	// was there before I noticed "injected quotes don't seem to have ParentBarSimulated"
			identicalButFresh.ParentBarStreaming	= this.ParentBarStreaming;	// this may fix it injected quotes don't seem to have ParentBarSimulated
			return identicalButFresh;
		}
		#endregion

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.Append("G#");
			sb.Append(this.IntraBarSerno);
			sb.Append("/");
			sb.Append(this.AbsnoPerSymbol);
			sb.Append("@");
			sb.Append(this.ServerTime.TimeOfDay);
			sb.Append(" ");
			sb.Append(this.Symbol);
			sb.Append(" bid{");
			sb.Append(this.Bid);
			sb.Append("-");
			sb.Append(this.Ask);
			sb.Append("}ask size{");
			sb.Append(this.Size);
			sb.Append("} ParentBarSimulated[");
			sb.Append(this.ParentBarSimulated);
			sb.Append("] ParentBarStreaming[");
			sb.Append(this.ParentBarStreaming);
			sb.Append("]");
			if (string.IsNullOrEmpty(this.Source) == false) {
				sb.Append(" ");
				sb.Append(Source);
			}
			return sb.ToString();
		}
		public string ToStringLong() {
			StringBuilder sb = new StringBuilder();
			sb.Append("G#");
			sb.Append(this.IntraBarSerno);
			sb.Append("/");
			sb.Append(this.AbsnoPerSymbol);
			sb.Append(" ");
			sb.Append(this.Symbol);
			sb.Append(" bid{");
			sb.Append(Math.Round(this.Bid, 3));
			sb.Append("-");
			sb.Append(Math.Round(this.Ask, 3));
			sb.Append("}ask size{");
			sb.Append(this.Size);
			sb.Append("@");
			sb.Append(Math.Round(this.TradedPrice, 3));
			sb.Append("}traded");
			if (ServerTime != null) {
				sb.Append(" SERVER[");
				sb.Append(ServerTime.ToString("HH:mm:ss.fff"));
				sb.Append("]");
			}
			sb.Append("[");
			sb.Append(LocalTimeCreated.ToString("HH:mm:ss.fff"));
			sb.Append("]LOCAL");
			if (string.IsNullOrEmpty(this.Source) == false) {
				sb.Append(" ");
				sb.Append(Source);
			}
			sb.Append(" WentThroughStreamingToScript[");
			sb.Append(this.WentThroughStreamingToScript);
			sb.Append("] ParentBarSimulated[");
			sb.Append(this.ParentBarSimulated);
			sb.Append("] ParentBarStreaming[");
			sb.Append(this.ParentBarStreaming);
			sb.Append("]");
			return sb.ToString();
		}

		//public static QuoteGenerated SafeUpcast(Quote quote) {
		//    QuoteGenerated upcasted = quote as QuoteGenerated;
		//    if (upcasted == null) {
		//        string msg = "Should be of a type Sq1.Core.Backtesting.QuoteGenerated instead of Sq1.Core.DataTypes.Quote: "
		//            + quote;
		//        throw new Exception(msg);
		//    }
		//    return upcasted;
		//}
	}
}
