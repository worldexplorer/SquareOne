using System;

namespace Sq1.Adapters.Quik.Dde {
	public class QuikTrade {
		public	string		Source;

		public	string		Symbol;
		public	string		SymbolClass;
		public	DateTime	ServerTime;
		public	DateTime	LocalTimeCreated	{ get; protected set; }

		public	double		Price;
		public	double		Quantity;
		public	bool		BuyTrue_SellFalse;

		public QuikTrade(string reasonToExist) {
			Source = reasonToExist;
			LocalTimeCreated = DateTime.Now;
		}
	}
}