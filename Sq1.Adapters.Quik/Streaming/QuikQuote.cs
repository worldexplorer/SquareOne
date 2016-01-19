using System;

using Sq1.Core.DataTypes;

using Newtonsoft.Json;

namespace Sq1.Adapters.Quik.Streaming {
	public class QuoteQuik : Quote {
		[JsonProperty]	public double	FortsDepositBuy;
		[JsonProperty]	public double	FortsDepositSell;

		[JsonProperty]	public double	FortsPriceMax;
		[JsonProperty]	public double	FortsPriceMin;


		[JsonProperty]	public string		FortsDepositBuyFormatted { get {
			string ret = this.FortsDepositBuy.ToString("N1");
			if (this.ParentBarStreaming							== null) return ret;
			if (this.ParentBarStreaming.ParentBars				== null) return ret;
			if (this.ParentBarStreaming.ParentBars.SymbolInfo	== null) return ret;
			SymbolInfo symbolInfo = this.ParentBarStreaming.ParentBars.SymbolInfo;
			ret = string.Format("{0:" + symbolInfo.PriceFormat + "}", this.FortsDepositBuy);
			return ret;
		} }

		[JsonProperty]	public string		FortsDepositSellFormatted { get {
			string ret = this.FortsDepositSell.ToString("N1");
			if (this.ParentBarStreaming							== null) return ret;
			if (this.ParentBarStreaming.ParentBars				== null) return ret;
			if (this.ParentBarStreaming.ParentBars.SymbolInfo	== null) return ret;
			SymbolInfo symbolInfo = this.ParentBarStreaming.ParentBars.SymbolInfo;
			ret = string.Format("{0:" + symbolInfo.PriceFormat + "}", this.FortsDepositSell);
			return ret;
		} }


		[JsonProperty]	public string		FortsPriceMaxFormatted { get {
			string ret = this.FortsPriceMax.ToString("N1");
			if (this.ParentBarStreaming							== null) return ret;
			if (this.ParentBarStreaming.ParentBars				== null) return ret;
			if (this.ParentBarStreaming.ParentBars.SymbolInfo	== null) return ret;
			SymbolInfo symbolInfo = this.ParentBarStreaming.ParentBars.SymbolInfo;
			ret = string.Format("{0:" + symbolInfo.PriceFormat + "}", this.FortsPriceMax);
			return ret;
		} }

		[JsonProperty]	public string		FortsPriceMinFormatted { get {
			string ret = this.FortsPriceMin.ToString("N1");
			if (this.ParentBarStreaming							== null) return ret;
			if (this.ParentBarStreaming.ParentBars				== null) return ret;
			if (this.ParentBarStreaming.ParentBars.SymbolInfo	== null) return ret;
			SymbolInfo symbolInfo = this.ParentBarStreaming.ParentBars.SymbolInfo;
			ret = string.Format("{0:" + symbolInfo.PriceFormat + "}", this.FortsPriceMin);
			return ret;
		} }



		public QuoteQuik(DateTime quoteDate) : base(quoteDate) {
		}

		public void EnrichFromStreamingDataSnapshotQuik(QuikStreamingDataSnapshot quikStreamingDataSnapshot) {
			this.FortsDepositBuy	= quikStreamingDataSnapshot.FortsGetDepositBuyForSymbol	(base.Symbol);
			this.FortsDepositSell	= quikStreamingDataSnapshot.FortsGetDepositSellForSymbol(base.Symbol);
			this.FortsPriceMin		= quikStreamingDataSnapshot.FortsGetPriceMinForSymbol	(base.Symbol);
			this.FortsPriceMax		= quikStreamingDataSnapshot.FortsGetPriceMaxForSymbol	(base.Symbol);
		}
		public static QuoteQuik SafeUpcast(Quote quote) {
			if (quote is QuoteQuik == false) {
				string msg = "Should be of a type Sq1.Adapters.Quik.QuoteQuik instead of Sq1.Core.DataTypes.Quote: "
					+ quote;
				throw new Exception(msg);
			}
			return quote as QuoteQuik;
		}
	}
}
