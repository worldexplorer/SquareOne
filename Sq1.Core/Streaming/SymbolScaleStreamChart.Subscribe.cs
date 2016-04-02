using System;
using System.Collections.Generic;

using Sq1.Core.Charting;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStreamChart {

		public override bool ConsumerQuoteAdd(StreamingConsumerChart quoteConsumer) { lock (base.LockConsumersQuote) {
			bool baseAdded = base.ConsumerQuoteAdd(quoteConsumer);
			if (baseAdded == false) return baseAdded;
			if (this.binderPerConsumer.ContainsKey(quoteConsumer) == false) {
				this.binderPerConsumer.Add(quoteConsumer, new BinderAttacher_perStreamingChart(this, quoteConsumer));
			}
			return true;
		} }
		public override bool ConsumerQuoteRemove(StreamingConsumerChart quoteConsumer) { lock (base.LockConsumersQuote) {
			bool baseRemoved = base.ConsumerQuoteRemove(quoteConsumer);
			if (baseRemoved == false) return baseRemoved;
			if (base.ConsumersBar.Contains(quoteConsumer)) return true;		// will remove binder only if there is no other consumers for it to serve & protect
			if (this.binderPerConsumer.ContainsKey(quoteConsumer) == false) return true;
			this.binderPerConsumer.Remove(quoteConsumer);
			return true;
		} }
		public override bool ConsumerBarAdd(StreamingConsumerChart barConsumer) { lock (base.LockConsumersBar) {
			bool baseAdded = base.ConsumerBarAdd(barConsumer);
			if (baseAdded == false) return baseAdded;
			if (this.binderPerConsumer.ContainsKey(barConsumer) == false) {
				this.binderPerConsumer.Add(barConsumer, new BinderAttacher_perStreamingChart(this, barConsumer));
			}
			return true;
		} }
		public override bool ConsumerBarRemove(StreamingConsumerChart barConsumer) { lock (base.LockConsumersBar) {
			bool baseRemoved = base.ConsumerBarRemove(barConsumer);
			if (baseRemoved == false) return baseRemoved;
			if (this.ConsumersQuote.Contains(barConsumer)) return true;		// will remove binder only if there is no other consumers for it to serve & protect
			if (this.binderPerConsumer.ContainsKey(barConsumer) == false) return true;
			this.binderPerConsumer.Remove(barConsumer);
			return true;
		} }
	}
}
