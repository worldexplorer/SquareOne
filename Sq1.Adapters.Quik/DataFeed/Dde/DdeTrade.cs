// QScalp source code was downloaded on Apr 2012 for free from http://www.qscalp.ru/download
// SquareOne uses QScalp's modified classes and keeps original author Name and URL
// Nikolay Moroshkin can tell me to remove his code completely => I'll rewrite the pieces borrowed //Pavel Chuchkalov 
using System;

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