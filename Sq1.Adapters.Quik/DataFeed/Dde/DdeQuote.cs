// QScalp source code was downloaded on O2-Jun-2012 for free from http://www.qscalp.ru/download/qscalp_src.zip
// SquareOne uses QScalp's modified classes and keeps original author Name and URL
// Nikolay Moroshkin can tell me to remove his code completely => I'll rewrite the pieces borrowed //Pavel Chuchkalov 

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