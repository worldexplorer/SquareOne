using System;

using Sq1.Core.DataTypes;
using System.Text;

namespace Sq1.Core.Backtesting {
	public class QuoteGenerated : Quote {
		public Bar			ParentBarSimulated;
		public bool			HasParentBarSimulated { get { return this.ParentBarSimulated != null; } }
		public new string	ParentBarIdent { get { return (this.HasParentBarSimulated) ? this.ParentBarSimulated.ParentBarsIdent : "NO_PARENT_BAR_SIMULATED"; } }
		public bool			WentThroughStreamingToScript;
		public double		SpreadAligned { get {
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
		
		public QuoteGenerated(DateTime localTimeEqualsToServerTimeForGenerated) : base(localTimeEqualsToServerTimeForGenerated) {
		}

		#region SORRY_FOR_THE_MESS__I_NEED_TO_DERIVE_IDENTICAL_ONLY_FOR_GENERATED__IF_YOU_NEED_IT_IN_BASE_QUOTE_MOVE_IT_THERE
		//public QuoteGenerated Clone() {
		//	return (QuoteGenerated)this.MemberwiseClone();
		//}
		public QuoteGenerated DeriveIdenticalButFresh() {
			QuoteGenerated identicalButFresh = new QuoteGenerated(this.ServerTime);
			identicalButFresh.Symbol = this.Symbol;
			identicalButFresh.SymbolClass = this.SymbolClass;
			identicalButFresh.Source = this.Source;
			identicalButFresh.ServerTime = this.ServerTime.AddMilliseconds(911);
			identicalButFresh.LocalTimeCreated = this.LocalTimeCreated.AddMilliseconds(911);
			identicalButFresh.LastDealBidOrAsk = this.LastDealBidOrAsk;
			identicalButFresh.ItriggeredFillAtBidOrAsk = this.ItriggeredFillAtBidOrAsk;
			identicalButFresh.Bid = this.Bid;
			identicalButFresh.Ask = this.Ask;
			identicalButFresh.Size = this.Size;
			identicalButFresh.IntraBarSerno = this.IntraBarSerno + Quote.IntraBarSernoShiftForGeneratedTowardsPendingFill;
			identicalButFresh.ParentBarSimulated = this.ParentBarSimulated;	// was there before I noticed "injected quotes don't seem to have ParentBarSimulated"
			identicalButFresh.ParentBarStreaming = this.ParentBarStreaming;	// this may fix it injected quotes don't seem to have ParentBarSimulated
			return identicalButFresh;
		}
		#endregion

		//public override string ToString() {
		//    string ret = "G#" + this.IntraBarSerno + "/" + this.Absno + " " + this.Symbol
		//        + " bid{" + this.Bid + "-" + this.Ask + "}ask"
		//        + " size{" + this.Size + "}";
		//    ret += " SIM:" + this.ParentBarSimulated;		// ALLOW_NULL .ToString()
		//    ret += " STR:" + this.ParentStreamingBar;		// ALLOW_NULL .ToString()
		//    if (string.IsNullOrEmpty(this.Source) == false) ret += " " + Source;
		//    return ret;
		//}
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.Append("G#");
			sb.Append(this.IntraBarSerno);
			sb.Append("/");
			sb.Append(this.AbsnoPerSymbol);
			sb.Append(" ");
			sb.Append(this.Symbol);
			sb.Append(" bid{");
			sb.Append(this.Bid);
			sb.Append("-");
			sb.Append(this.Ask);
			sb.Append("}ask size{");
			sb.Append(this.Size);
			sb.Append("} SIM:");
			sb.Append(this.ParentBarSimulated);
			sb.Append(" STR:");
			sb.Append(this.ParentBarStreaming);
			if (string.IsNullOrEmpty(this.Source) == false) {
				sb.Append(" ");
				sb.Append(Source);
			}
			return sb.ToString();
		}
		//public string ToStringLong() {
		//    string ret = "#" + this.IntraBarSerno + "/" + this.Absno + " " + this.Symbol
		//        + " bid{" + Math.Round(this.Bid, 3) + "-" + Math.Round(this.Ask, 3) + "}ask"
		//        + " size{" + this.Size + "@" + Math.Round(this.LastDealPrice, 3) + "}lastDeal";
		//    if (ServerTime != null) ret += " SERVER[" + ServerTime.ToString("HH:mm:ss.fff") + "]";
		//    ret += "[" + LocalTimeCreatedMillis.ToString("HH:mm:ss.fff") + "]LOCAL";
		//    if (string.IsNullOrEmpty(this.Source) == false) ret += " " + Source;
		//    ret += " WentThroughStreamingToScript[" + this.WentThroughStreamingToScript + "]";
		//    ret += " SIM:" + this.ParentBarSimulated;
		//    ret += " STR:" + this.ParentStreamingBar;
		//    return ret;
		//}
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
			sb.Append(Math.Round(this.LastDealPrice, 3));
			sb.Append("}lastDeal");
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
			sb.Append("]");
			sb.Append(" SIM:");
			sb.Append(this.ParentBarSimulated);
			sb.Append(" STR:");
			sb.Append(this.ParentBarStreaming);
			return sb.ToString();
		}
	}
}
