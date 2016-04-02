using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStreamSolidifier {
		public override bool ConsumerQuoteAdd(StreamingConsumerSolidifier quoteConsumer) { lock (base.LockConsumersQuote) {
			bool baseAdded = base.ConsumerQuoteAdd(quoteConsumer);
			if (baseAdded == false) {
				throw new Exception("YOU_CAN_ADD__ONLY_ONE_QUOTE_SOLIDIFIER__PER_EACH_SYMBOL_SYMBOL");
			}
			if (baseAdded == false) return baseAdded;
			return true;
		} }
		public override bool ConsumerBarAdd(StreamingConsumerSolidifier barConsumer) { lock (base.LockConsumersBar) {
			bool baseAdded = base.ConsumerBarAdd(barConsumer);
			if (baseAdded == false) {
				throw new Exception("YOU_CAN_ADD__ONLY_ONE_BAR_SOLIDIFIER__PER_EACH_SYMBOL_SYMBOL");
			}
			return true;
		} }
	}
}
