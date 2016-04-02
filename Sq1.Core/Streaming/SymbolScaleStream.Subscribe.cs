using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Backtesting;
using Sq1.Core.Livesim;
using Sq1.Core.Charting;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStream {
		public	List<StreamingConsumer>		ConsumersQuote					{ get; protected set; }
		public	List<StreamingConsumer>		ConsumersBar					{ get; protected set; }

				object						lockConsumersQuote;
				object						lockConsumersBar;

		public	List<StreamingConsumer>		ConsumersAll						{ get { lock (this.lockConsumersQuote) {
			List<StreamingConsumer> ret = new List<StreamingConsumer>(this.ConsumersQuote);
			foreach (StreamingConsumer consumer in this.ConsumersBar) {
				if (ret.Contains(consumer)) continue;
				ret.Add(consumer);
			}
			return ret;
		} } }

		public string ConsumersQuoteAsString { get { lock (this.lockConsumersQuote) {
					string ret = "";
					foreach (StreamingConsumer consumer in this.ConsumersQuote) {
						if (ret != "") ret += ", ";
						ret += consumer.ToString();
					}
					return ret;
				} } }
		public string ConsumersBarAsString { get { lock (this.lockConsumersBar) {
					string ret = "";
					foreach (StreamingConsumer consumer in this.ConsumersBar) {
						if (ret != "") ret += ", ";
						ret += consumer.ToString();
					}
					return ret;
				} } }
		public string ConsumersQuoteNames { get { lock (this.lockConsumersQuote) {
					string ret = "";
					foreach (StreamingConsumer consumer in this.ConsumersQuote) {
						if (ret != "") ret += ",";
						ret += consumer.ReasonToExist;
					}
					return ret;
				} } }
		public string ConsumersBarNames { get { lock (this.lockConsumersBar) {
					string ret = "";
					foreach (StreamingConsumer consumer in this.ConsumersBar) {
						if (ret != "") ret += ",";
						ret += consumer.ReasonToExist;
					}
					return ret;
				} } }


		public string ConsumerNames	{ get {
			List<StreamingConsumer>	merged = new List<StreamingConsumer>();
			merged.AddRange(this.ConsumersQuote);
			merged.AddRange(this.ConsumersBar);

			string ret = "";
			foreach (StreamingConsumer consumerEach in merged) {
				string reasonOrFullDump = string.IsNullOrEmpty(consumerEach.ReasonToExist) ? consumerEach.ToString() : consumerEach.ReasonToExist;
				if (string.IsNullOrEmpty(reasonOrFullDump)) continue;
				if (ret.Contains(reasonOrFullDump)) continue;
				if (ret != "") ret += ",";
				ret += reasonOrFullDump;
			}
			if (ret == "") ret = "NO_CONSUMERS";
			ret = this.ReasonIwasCreated_propagatedFromDistributor + " :: " + ret;
			ret += " " + this.SymbolScaleInterval;
			return ret;
		} }

		//public string ConsumersQuoteAsShortString { get { lock (this.lockConsumersQuote) {
		//			string ret = "";
		//			foreach (IStreamingConsumer consumer in this.ConsumersQuote) {
		//				if (ret != "") ret += ", ";
		//				ret += consumer.Na();
		//			}
		//			return ret;
		//		} } }
		//public string ConsumersBarAsShortString { get { lock (this.lockConsumersBar) {
		//			string ret = "";
		//			foreach (IStreamingConsumer consumer in this.ConsumersBar) {
		//				if (ret != "") ret += ", ";
		//				ret += consumer.ToString();
		//			}
		//			return ret;
		//		} } }
		//public override string ToShortString() { return this.SymbolScaleInterval + ":Quotes[" + this.ConsumersQuoteAsShortString + "],Bars[" + this.ConsumersBarAsShortString + "]"; }

		public bool ConsumersQuoteContains(StreamingConsumer consumer) { lock (this.lockConsumersQuote) { return this.ConsumersQuote.Contains(consumer); } }
		public bool ConsumerQuoteAdd(StreamingConsumer quoteConsumer) { lock (this.lockConsumersQuote) {
				if (this.ConsumersQuoteContains(quoteConsumer)) {
					Assembler.PopupException("I_REFUSE_TO_SUBSCRIBE_TWICE quoteConsumer[" + quoteConsumer + "] to [" + this + "]");
					return false;
				}
				this.ConsumersQuote.Add(quoteConsumer);
				if (this.binderPerConsumer.ContainsKey(quoteConsumer) == false) {
					this.binderPerConsumer.Add(quoteConsumer, new BinderAttacherPerConsumer(this, quoteConsumer));
				}
				return true;
			} }
		public bool ConsumerQuoteRemove(StreamingConsumer quoteConsumer) { lock (this.lockConsumersQuote) {
				if (this.ConsumersQuoteContains(quoteConsumer) == false) {
					Assembler.PopupException("I_REFUSE_TO_REMOVE_UNSUBSCRIBED_CONSUMER quoteConsumer[" + quoteConsumer + "] from [" + this + "]");
					return false;
				}
				this.ConsumersQuote.Remove(quoteConsumer);
				if (this.ConsumersBar.Contains(quoteConsumer)) return true;		// will remove binder only if there is no other consumers for it to serve & protect
				if (this.binderPerConsumer.ContainsKey(quoteConsumer) == false) return true;
				this.binderPerConsumer.Remove(quoteConsumer);
				return true;
			} }
		public int ConsumersQuoteCount { get { lock (this.lockConsumersQuote) { return this.ConsumersQuote.Count; } } }

		public bool ConsumersBarContains(StreamingConsumer consumer) { lock (this.lockConsumersBar) { return this.ConsumersBar.Contains(consumer); } }
		public bool ConsumerBarAdd(StreamingConsumer barConsumer) { lock (this.lockConsumersBar) {
				if (this.ConsumersBar.Contains(barConsumer)) {
					Assembler.PopupException("I_REFUSE_TO_SUBSCRIBE_TWICE barConsumer[" + barConsumer + "] to [" + this + "]");
					return false;
				}
				this.ConsumersBar.Add(barConsumer);
				if (this.binderPerConsumer.ContainsKey(barConsumer) == false) {
					this.binderPerConsumer.Add(barConsumer, new BinderAttacherPerConsumer(this, barConsumer));
				}
				return true;
			} }
		public bool ConsumerBarRemove(StreamingConsumer barConsumer) { lock (this.lockConsumersBar) {
				if (this.ConsumersBar.Contains(barConsumer) == false) {
					Assembler.PopupException("I_REFUSE_TO_REMOVE_UNSUBSCRIBED_CONSUMER barConsumer[" + barConsumer + "] from [" + this + "]");
					return false;
				}
				this.ConsumersBar.Remove(barConsumer);
				//if (earlyBinders.ContainsKey(consumer) && this.consumersQuote.Contains(consumer) == false) {
				if (this.ConsumersQuote.Contains(barConsumer)) return true;		// will remove binder only if there is no other consumers for it to serve & protect
				if (this.binderPerConsumer.ContainsKey(barConsumer) == false) return true;
				this.binderPerConsumer.Remove(barConsumer);
				return true;
			} }
		public int ConsumersBarCount { get { lock (this.lockConsumersBar) { return this.ConsumersBar.Count; } } }

	}
}
