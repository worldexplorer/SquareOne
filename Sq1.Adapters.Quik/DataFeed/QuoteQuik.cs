using System;

using Sq1.Core.DataTypes;
using Newtonsoft.Json;

namespace Sq1.Adapters.Quik {
	public class QuoteQuik : Quote {
		[JsonProperty]	public double	FortsDepositBuy;
		[JsonProperty]	public double	FortsDepositSell;

		[JsonProperty]	public double	FortsPriceMax;
		[JsonProperty]	public double	FortsPriceMin;

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
