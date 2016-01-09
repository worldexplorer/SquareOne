using System;

namespace Sq1.Adapters.Quik {
	public partial class QuikStreamingMonitorControl  {
		void olvQuotesCustomize() {
			this.olvcQuotesAsk.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesAsk.AspectGetter: quoteQuik=null";
				//return position.MFEPercent.ToString("F2") + " %";
				return quoteQuik.Ask;
			};

			this.olvcQuotesBid.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesBid.AspectGetter: quoteQuik=null";
				//return position.MFEPercent.ToString("F2") + " %";
				return quoteQuik.Bid;
			};

			this.olvcQuotesFortsPriceMin.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesFortsPriceMin.AspectGetter: quoteQuik=null";
				//return position.MFEPercent.ToString("F2") + " %";
				return quoteQuik.FortsPriceMin;
			};

			this.olvcQuotesFortsPriceMax.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesFortsPriceMax.AspectGetter: quoteQuik=null";
				//return position.MFEPercent.ToString("F2") + " %";
				return quoteQuik.FortsPriceMax;
			};

			this.olvcQuotesSymbol.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesSymbol.AspectGetter: quoteQuik=null";
				//return position.MFEPercent.ToString("F2") + " %";
				return quoteQuik.Symbol;
			};

			this.olvcQuotesSymbolClass.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesSymbolClass.AspectGetter: quoteQuik=null";
				//return position.MFEPercent.ToString("F2") + " %";
				return quoteQuik.SymbolClass;
			};

			this.olvcQuotesFortsDepositBuy.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesFortsDepositBuy.AspectGetter: quoteQuik=null";
				//return position.MFEPercent.ToString("F2") + " %";
				return quoteQuik.FortsDepositBuy;
			};

			this.olvcQuotesFortsDepositSell.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesFortsDepositSell.AspectGetter: quoteQuik=null";
				//return position.MFEPercent.ToString("F2") + " %";
				return quoteQuik.FortsDepositSell;
			};

			this.olvcQuotesQty.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesQty.AspectGetter: quoteQuik=null";
				//return position.MFEPercent.ToString("F2") + " %";
				return quoteQuik.Size;
			};

			this.olvcQuotesServerTime.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesServerTime.AspectGetter: quoteQuik=null";
				//return position.MFEPercent.ToString("F2") + " %";
				return quoteQuik.ServerTime;
			};

			this.olvcQuotesAbsnoPerSymbol.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesAbsnoPerSymbol.AspectGetter: quoteQuik=null";
				//return position.MFEPercent.ToString("F2") + " %";
				return quoteQuik.AbsnoPerSymbol;
			};

		}
	}
}
