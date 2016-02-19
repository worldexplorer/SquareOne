using System;

using Sq1.Core;

namespace Sq1.Adapters.Quik.Streaming.Monitor {
	public partial class QuikStreamingMonitorControl  {
		void olvQuotesCustomize() {
			this.olvcQuotesAsk.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesAsk.AspectGetter: quoteQuik=null";
				return quoteQuik.AskFormatted;
			};

			this.olvcQuotesBid.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesBid.AspectGetter: quoteQuik=null";
				return quoteQuik.BidFormatted;
			};

			this.olvcQuotesFortsPriceMin.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesFortsPriceMin.AspectGetter: quoteQuik=null";
				return quoteQuik.FortsPriceMinFormatted;
			};

			this.olvcQuotesFortsPriceMax.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesFortsPriceMax.AspectGetter: quoteQuik=null";
				return quoteQuik.FortsPriceMaxFormatted;
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
				return quoteQuik.FortsDepositBuyFormatted;
			};

			this.olvcQuotesFortsDepositSell.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesFortsDepositSell.AspectGetter: quoteQuik=null";
				return quoteQuik.FortsDepositSellFormatted;
			};

			this.olvcQuotesQty.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesQty.AspectGetter: quoteQuik=null";
				return quoteQuik.SizeFormatted;
			};

			this.olvcQuotesServerTime.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesServerTime.AspectGetter: quoteQuik=null";
				//return position.MFEPercent.ToString("F2") + " %";
				return quoteQuik.ServerTime.ToString(Assembler.DateTimeFormatToMinutesSeconds);
			};

			this.olvcQuotesAbsnoPerSymbol.AspectGetter = delegate(object o) {
				QuoteQuik quoteQuik = o as QuoteQuik;
				if (quoteQuik == null) return "olvcQuotesAbsnoPerSymbol.AspectGetter: quoteQuik=null";
				return quoteQuik.AbsnoPerSymbol;
			};

		}
	}
}
