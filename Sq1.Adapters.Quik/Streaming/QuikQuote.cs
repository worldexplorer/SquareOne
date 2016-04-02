using System;

using Sq1.Core.DataTypes;

using Newtonsoft.Json;

namespace Sq1.Adapters.Quik.Streaming {
	public class QuoteQuik : Quote {
		[JsonProperty]	public	double	FortsDepositBuy;
		[JsonProperty]	public	double	FortsDepositSell;

		[JsonProperty]	public	double	FortsPriceMax;
		[JsonProperty]	public	double	FortsPriceMin;

		[JsonProperty]	public	double	PriceStepFromDde;


		[JsonProperty]	public	string	FortsDepositBuy_formatted	{ get { return string.Format("{0:" + base.PriceFormat + "}", this.FortsDepositBuy); } }
		[JsonProperty]	public	string	FortsDepositSell_formatted	{ get { return string.Format("{0:" + base.PriceFormat + "}", this.FortsDepositSell); } }
		[JsonProperty]	public	string	FortsPriceMax_formatted		{ get { return string.Format("{0:" + base.PriceFormat + "}", this.FortsPriceMax); } }
		[JsonProperty]	public	string	FortsPriceMin_formatted		{ get { return string.Format("{0:" + base.PriceFormat + "}", this.FortsPriceMin); } }
		[JsonProperty]	public	string	PriceStepFromDde_formatted	{ get { return string.Format("{0:" + base.PriceFormat + "}", this.PriceStepFromDde); } }

		//public QuoteQuik(DateTime quoteDate) : base(quoteDate) {}

		public QuoteQuik(DateTime localTime, DateTime serverTime,
						string symbol, long absno_perSymbol_perStreamingAdapter = -1,
						double bid = double.NaN, double ask = double.NaN, double size = -1,
						BidOrAsk tradedAt = BidOrAsk.UNKNOWN)
					: base(localTime, serverTime,
						symbol, absno_perSymbol_perStreamingAdapter,
						bid, ask, size,
						tradedAt) {}


		public void Enrich_fromStreamingDataSnapshotQuik(QuikStreamingDataSnapshot quikStreamingDataSnapshot) {
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
