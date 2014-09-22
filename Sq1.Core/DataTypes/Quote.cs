using System;
using System.Diagnostics;

namespace Sq1.Core.DataTypes {
	public class Quote {
		public string Symbol;
		public string SymbolClass;
		public string Source;
		public DateTime ServerTime;
		public DateTime LocalTimeCreatedMillis;

		public double Bid;
		public double Ask;
		public double Size;
		public BidOrAsk ItriggeredFillAtBidOrAsk;
		//MADE_READONLY_COZ_WRITING_IS_A_WRONG_CONCEPT public double PriceLastDeal;	// apparently must be PriceLastDeal==Bid || PriceLastDeal==Ask
		public double PriceLastDeal { get {
				if (this.ItriggeredFillAtBidOrAsk == BidOrAsk.UNKNOWN) {
					//v1 return -1;
					double median = (this.Ask + this.Bid) / 2;
					return median;
				}
				return (this.ItriggeredFillAtBidOrAsk == BidOrAsk.Ask) ? this.Ask : this.Bid;
			} }


		public int IntraBarSerno;
		//public int Absno { get { return AbsnoStaticCounter; } }		// I want a class var for furhter easy access despite it looks redundant
		public int Absno;

		[Obsolete("used as a reference for a single-backtest environment; otherwize Backtester and StreamingProvider compete for correct this.Absno value")]
		public static int AbsnoStaticCounter = 0;

		public Bar ParentStreamingBar { get; protected set; }
		public bool HasParentBar { get { return this.ParentStreamingBar != null; } }
		public string ParentBarIdent { get { return (this.HasParentBar) ? this.ParentStreamingBar.ParentBarsIdent : "NO_PARENT_BAR"; } }

		[Obsolete] public double PreviousClose;
		[Obsolete] public double Open;

		public static int IntraBarSernoShiftForGeneratedTowardsPendingFill = 100000;

		public Quote() {
			++AbsnoStaticCounter;
			ServerTime = DateTime.MinValue;
			LocalTimeCreatedMillis = DateTime.Now;
			IntraBarSerno = -1;
			Bid = double.NaN;
			Ask = double.NaN;
			Size = -1;
			ItriggeredFillAtBidOrAsk = BidOrAsk.UNKNOWN;
		}
		// TODO: don't be lazy and move to StreamingProvider.QuoteAbsnoForSymbol<string Symbol, int Absno> and init it on Backtester.RunSimulation
		//public void AbsnoReset() { Quote.AbsnoStaticCounter = 0; }
		public void SetParentBar(Bar parentBar) {
			if (this.Symbol != parentBar.Symbol) {
				string msg = "here is the problem for a streaming bar to carry another symbol!";
				#if DEBUG
				Debugger.Break();		//TEST_EMBEDDED
				#endif
			}
			this.ParentStreamingBar = parentBar;
		}
		public Quote Clone() {
			return (Quote)this.MemberwiseClone();
		}
		//public Quote DeriveIdenticalButFresh() {
		//    Quote identicalButFresh = new Quote();
		//    identicalButFresh.Symbol = this.Symbol;
		//    identicalButFresh.SymbolClass = this.SymbolClass;
		//    identicalButFresh.Source = this.Source;
		//    identicalButFresh.ServerTime = this.ServerTime.AddMilliseconds(911);
		//    identicalButFresh.LocalTimeCreatedMillis = this.LocalTimeCreatedMillis.AddMilliseconds(911);
		//    identicalButFresh.PriceLastDeal = this.PriceLastDeal;
		//    identicalButFresh.Bid = this.Bid;
		//    identicalButFresh.Ask = this.Ask;
		//    identicalButFresh.Size = this.Size;
		//    identicalButFresh.IntraBarSerno = this.IntraBarSerno + Quote.IntraBarSernoShiftForGeneratedTowardsPendingFill;
		//    identicalButFresh.ParentStreamingBar = this.ParentStreamingBar;
		//    return identicalButFresh;
		//}
		public override string ToString() {
			string ret = "#" + this.IntraBarSerno + "/" + this.Absno
				+ " " + this.Symbol
				+ " bid{" + Math.Round(this.Bid, 3) + "-" + Math.Round(this.Ask, 3) + "}ask"
				+ " size{" + this.Size + "@" + Math.Round(this.PriceLastDeal, 3) + "}lastDeal";
			if (ServerTime != null) ret += " SERVER[" + ServerTime.ToString("HH:mm:ss.fff") + "]";
			ret += "[" + LocalTimeCreatedMillis.ToString("HH:mm:ss.fff") + "]LOCAL";
			if (string.IsNullOrEmpty(this.Source) == false) ret += " " + Source;
			ret += " STR:" + this.ParentBarIdent;
			return ret;
		}
		public bool SameBidAsk(Quote other) {
			return (this.Bid == other.Bid && this.Ask == other.Ask);
		}
		public bool PriceBetweenBidAsk(double price) {
			return (price >= this.Bid && price <= this.Ask);
		}
		public double Spread { get { return this.Ask - this.Bid; } } 
	}
}