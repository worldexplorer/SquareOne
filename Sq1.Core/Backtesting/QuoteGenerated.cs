using System;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Backtesting {
	public class QuoteGenerated : Quote {
		public Bar ParentBarSimulated;
		public bool HasParentBarSimulated { get { return this.ParentBarSimulated != null; } }
		public new string ParentBarIdent { get { return (this.HasParentBarSimulated) ? this.ParentBarSimulated.ParentBarsIdent : "NO_PARENT_BAR_SIMULTED"; } }
		public bool WentThroughStreamingToScript;

		//public QuoteGenerated Clone() {
		//    return (QuoteGenerated)this.MemberwiseClone();
		//}
		public QuoteGenerated DeriveIdenticalButFresh() {
			QuoteGenerated identicalButFresh = new QuoteGenerated();
			identicalButFresh.Symbol = this.Symbol;
			identicalButFresh.SymbolClass = this.SymbolClass;
			identicalButFresh.Source = this.Source;
			identicalButFresh.ServerTime = this.ServerTime.AddMilliseconds(911);
			identicalButFresh.LocalTimeCreatedMillis = this.LocalTimeCreatedMillis.AddMilliseconds(911);
			//identicalButFresh.PriceLastDeal = this.PriceLastDeal;
			identicalButFresh.ItriggeredFillAtBidOrAsk = this.ItriggeredFillAtBidOrAsk;
			identicalButFresh.Bid = this.Bid;
			identicalButFresh.Ask = this.Ask;
			identicalButFresh.Size = this.Size;
			identicalButFresh.IntraBarSerno = this.IntraBarSerno + Quote.IntraBarSernoShiftForGeneratedTowardsPendingFill;
			identicalButFresh.ParentBarSimulated = this.ParentBarSimulated;
			return identicalButFresh;
		}
		public override string ToString() {
			string ret = "#" + this.IntraBarSerno + "/" + this.Absno
				+ " " + this.Symbol
				+ " bid{" + Math.Round(this.Bid, 3) + "-" + Math.Round(this.Ask, 3) + "}ask"
				+ " size{" + this.Size + "@" + Math.Round(this.PriceLastDeal, 3) + "}lastDeal";
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
