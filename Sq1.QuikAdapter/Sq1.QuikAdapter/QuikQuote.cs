using System;
using Sq1.Core.DataTypes;

namespace Sq1.QuikAdapter {
	public class QuikQuote : Quote {
		public double FortsDepositBuy;
		public double FortsDepositSell;

		public double FortsPriceMax;
		public double FortsPriceMin;

		public QuikQuote() : base() {
		}

		public void EnrichFromQuikStreamingDataSnapshot(QuikStreamingDataSnapshot quikStreamingDataSnapshot) {
			this.FortsDepositBuy = quikStreamingDataSnapshot.FortsGetDepositBuyForSymbol(base.Symbol);
			this.FortsDepositSell = quikStreamingDataSnapshot.FortsGetDepositSellForSymbol(base.Symbol);
			this.FortsPriceMin = quikStreamingDataSnapshot.FortsGetPriceMinForSymbol(base.Symbol);
			this.FortsPriceMax = quikStreamingDataSnapshot.FortsGetPriceMaxForSymbol(base.Symbol);
		}
		public static QuikQuote SafeUpcast(Quote quote) {
			if (quote is QuikQuote == false) {
				string msg = "Should be of a type Sq1.QuikAdapter.QuikQuote instead of Sq1.Core.DataTypes.Quote: "
					+ quote;
				throw new Exception(msg);
			}
			return quote as QuikQuote;
		}
	}
}
