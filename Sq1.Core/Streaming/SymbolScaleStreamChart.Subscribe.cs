using System;
using System.Collections.Generic;

using Sq1.Core.Charting;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStreamChart {

		//public override bool ConsumerQuoteAdd(StreamingConsumerChart quoteConsumer) { lock (base.LockConsumersQuote) {
		//    quoteConsumer.CreateBinderAttacher(this);
		//    bool baseAdded = base.ConsumerQuoteAdd(quoteConsumer);
		//    return baseAdded;
		//} }
		//public override bool ConsumerQuoteRemove(StreamingConsumerChart quoteConsumer) { lock (base.LockConsumersQuote) {
		//    bool baseRemoved = base.ConsumerQuoteRemove(quoteConsumer);
		//    return baseRemoved;
		//} }
		//public override bool ConsumerBarAdd(StreamingConsumerChart barConsumer) { lock (base.LockConsumersBar) {
		//    barConsumer.CreateBinderAttacher(this);
		//    bool baseAdded = base.ConsumerBarAdd(barConsumer);
		//    return baseAdded;
		//} }
		//public override bool ConsumerBarRemove(StreamingConsumerChart barConsumer) { lock (base.LockConsumersBar) {
		//    bool baseRemoved = base.ConsumerBarRemove(barConsumer);
		//    return baseRemoved;
		//} }
	}
}
