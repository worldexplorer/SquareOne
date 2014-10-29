using System;
// QScalp source code was downloaded on Apr 2012 for free from http://www.qscalp.ru/download
// SquareOne uses QScalp's modified classes and keeps original author Name and URL
// Nikolay can tell me to remove your code completely => I'll rewrite borrowed pieces //worldexplorer 

namespace Sq1.Adapters.Quik.Dde {
	public struct DdeTrade {
		public int IntPrice;
		public double RawPrice;
		public int Quantity;
		public DdeTradeOp Op;
		public DateTime DateTime;
		//public DateTime Received;
	}
}