using System;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Backtesting {
	public class QuoteGenerated : Quote {
		public Bar ParentBarSimulated;
		public bool HasParentBarSimulated { get { return this.ParentBarSimulated != null; } }
		public new string ParentBarIdent { get { return (this.HasParentBarSimulated) ? this.ParentBarSimulated.ParentBarsIdent : "NO_PARENT_BAR_SIMULATED"; } }
		public bool WentThroughStreamingToScript;
		public double SpreadAligned { get {
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

		//public QuoteGenerated Clone() {
		//	return (QuoteGenerated)this.MemberwiseClone();
		//}
		public QuoteGenerated DeriveIdenticalButFresh() {
			QuoteGenerated identicalButFresh = new QuoteGenerated(this.ServerTime);
			identicalButFresh.Symbol = this.Symbol;
			identicalButFresh.SymbolClass = this.SymbolClass;
			identicalButFresh.Source = this.Source;
			identicalButFresh.ServerTime = this.ServerTime.AddMilliseconds(911);
			identicalButFresh.LocalTimeCreatedMillis = this.LocalTimeCreatedMillis.AddMilliseconds(911);
			identicalButFresh.LastDealBidOrAsk = this.LastDealBidOrAsk;
			identicalButFresh.ItriggeredFillAtBidOrAsk = this.ItriggeredFillAtBidOrAsk;
			identicalButFresh.Bid = this.Bid;
			identicalButFresh.Ask = this.Ask;
			identicalButFresh.Size = this.Size;
			identicalButFresh.IntraBarSerno = this.IntraBarSerno + Quote.IntraBarSernoShiftForGeneratedTowardsPendingFill;
			identicalButFresh.ParentBarSimulated = this.ParentBarSimulated;	// was there before I noticed "injected quotes don't seem to have ParentBarSimulated"
			identicalButFresh.ParentStreamingBar = this.ParentStreamingBar;	// this may fix it injected quotes don't seem to have ParentBarSimulated
			return identicalButFresh;
		}
		public override string ToString() {
			string ret = "G#" + this.IntraBarSerno + "/" + this.Absno + " " + this.Symbol
				+ " bid{" + this.Bid + "-" + this.Ask + "}ask"
				+ " size{" + this.Size + "}";
			ret += " SIM:" + this.ParentBarSimulated;		// ALLOW_NULL .ToString()
			ret += " STR:" + this.ParentStreamingBar;		// ALLOW_NULL .ToString()
			if (string.IsNullOrEmpty(this.Source) == false) ret += " " + Source;
			return ret;
		}
		public string ToStringLong() {
			string ret = "#" + this.IntraBarSerno + "/" + this.Absno + " " + this.Symbol
				+ " bid{" + Math.Round(this.Bid, 3) + "-" + Math.Round(this.Ask, 3) + "}ask"
				+ " size{" + this.Size + "@" + Math.Round(this.LastDealPrice, 3) + "}lastDeal";
			if (ServerTime != null) ret += " SERVER[" + ServerTime.ToString("HH:mm:ss.fff") + "]";
			ret += "[" + LocalTimeCreatedMillis.ToString("HH:mm:ss.fff") + "]LOCAL";
			if (string.IsNullOrEmpty(this.Source) == false) ret += " " + Source;
			ret += " WentThroughStreamingToScript[" + this.WentThroughStreamingToScript + "]";
			ret += " SIM:" + this.ParentBarSimulated;
			ret += " STR:" + this.ParentStreamingBar;
			return ret;
		}
	}
}
