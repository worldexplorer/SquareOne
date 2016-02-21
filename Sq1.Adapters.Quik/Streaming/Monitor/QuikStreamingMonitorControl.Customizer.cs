using System;

using Sq1.Core;

namespace Sq1.Adapters.Quik.Streaming.Monitor {
	public partial class QuikStreamingMonitorControl  {
		void olvQuotesCustomize() {
			this.olvcQuotesAsk.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesAsk.AspectGetter: quoteQuik=null";
				return quoteQuik.Ask_formatted;
			};

			this.olvcQuotesBid.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesBid.AspectGetter: quoteQuik=null";
				return quoteQuik.Bid_formatted;
			};

			this.olvcQuotesFortsPriceMin.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesFortsPriceMin.AspectGetter: quoteQuik=null";
				return quoteQuik.FortsPriceMin_formatted;
			};

			this.olvcQuotesFortsPriceMax.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesFortsPriceMax.AspectGetter: quoteQuik=null";
				return quoteQuik.FortsPriceMax_formatted;
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
				return quoteQuik.FortsDepositBuy_formatted;
			};

			this.olvcQuotesFortsDepositSell.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesFortsDepositSell.AspectGetter: quoteQuik=null";
				return quoteQuik.FortsDepositSell_formatted;
			};

			this.olvcQuotesQty.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesQty.AspectGetter: quoteQuik=null";
				return quoteQuik.Size_formatted;
			};

			this.olvcQuotesServerTime.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesServerTime.AspectGetter: quoteQuik=null";
				//return position.MFEPercent.ToString("F2") + " %";
				return quoteQuik.ServerTime.ToString(Assembler.DateTimeFormatToMinutesSeconds_noYear);
			};

			this.olvcPriceStepDde.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcPriceStepDde.AspectGetter: quoteQuik=null";
				return quoteQuik.PriceStepFromDde_formatted;
			};

			this.olvcQuotesAbsnoPerSymbol.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesAbsnoPerSymbol.AspectGetter: quoteQuik=null";
				return quoteQuik.AbsnoPerSymbol;
			};

		}
	}
}
