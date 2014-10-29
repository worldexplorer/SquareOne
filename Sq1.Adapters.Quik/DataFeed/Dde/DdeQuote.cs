// QScalp source code was downloaded on Apr 2012 for free from http://www.qscalp.ru/download
// SquareOne uses QScalp's modified classes and keeps original author Name and URL
// Nikolay can tell me to remove your code completely => I'll rewrite borrowed pieces //worldexplorer 

namespace Sq1.Adapters.Quik.Dde {
	public struct DdeQuote {
		public int Price;
		public int Volume;
		public DdeQuoteType Type;
		public DdeQuote(int price, int volume, DdeQuoteType type) {
			this.Price = price;
			this.Volume = volume;
			this.Type = type;
		}
	}
}