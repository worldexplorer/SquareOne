using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Backtesting;
using Sq1.Core.Livesim;
using Sq1.Core.Charting;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStream<STREAMING_CONSUMER_CHILD> {
		public		List<STREAMING_CONSUMER_CHILD>		ConsumersQuote					{ get; protected set; }
		public		List<STREAMING_CONSUMER_CHILD>		ConsumersBar					{ get; protected set; }

		protected	object						LockConsumersQuote;
		protected	object						LockConsumersBar;

		public	List<STREAMING_CONSUMER_CHILD>		ConsumersAll						{ get { lock (this.LockConsumersQuote) {
			List<STREAMING_CONSUMER_CHILD> ret = new List<STREAMING_CONSUMER_CHILD>(this.ConsumersQuote);
			foreach (STREAMING_CONSUMER_CHILD consumer in this.ConsumersBar) {
				if (ret.Contains(consumer)) continue;
				ret.Add(consumer);
			}
			return ret;
		} } }

		public string ConsumersQuoteAsString { get { lock (this.LockConsumersQuote) {
					string ret = "";
					foreach (STREAMING_CONSUMER_CHILD consumer in this.ConsumersQuote) {
						if (ret != "") ret += ", ";
						ret += consumer.ToString();
					}
					return ret;
				} } }
		public string ConsumersBarAsString { get { lock (this.LockConsumersBar) {
					string ret = "";
					foreach (STREAMING_CONSUMER_CHILD consumer in this.ConsumersBar) {
						if (ret != "") ret += ", ";
						ret += consumer.ToString();
					}
					return ret;
				} } }
		public string ConsumersQuoteNames { get { lock (this.LockConsumersQuote) {
					string ret = "";
					foreach (STREAMING_CONSUMER_CHILD consumer in this.ConsumersQuote) {
						if (ret != "") ret += ",";
						ret += consumer.ReasonToExist;
					}
					return ret;
				} } }
		public string ConsumersBarNames { get { lock (this.LockConsumersBar) {
					string ret = "";
					foreach (STREAMING_CONSUMER_CHILD consumer in this.ConsumersBar) {
						if (ret != "") ret += ",";
						ret += consumer.ReasonToExist;
					}
					return ret;
				} } }


		public string ConsumerNames	{ get {
			List<STREAMING_CONSUMER_CHILD>	merged = new List<STREAMING_CONSUMER_CHILD>();
			merged.AddRange(this.ConsumersQuote);
			merged.AddRange(this.ConsumersBar);

			string ret = "";
			foreach (STREAMING_CONSUMER_CHILD consumerEach in merged) {
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
		//			foreach (ISTREAMING_CONSUMER_CHILD consumer in this.ConsumersQuote) {
		//				if (ret != "") ret += ", ";
		//				ret += consumer.Na();
		//			}
		//			return ret;
		//		} } }
		//public string ConsumersBarAsShortString { get { lock (this.lockConsumersBar) {
		//			string ret = "";
		//			foreach (ISTREAMING_CONSUMER_CHILD consumer in this.ConsumersBar) {
		//				if (ret != "") ret += ", ";
		//				ret += consumer.ToString();
		//			}
		//			return ret;
		//		} } }
		//public override string ToShortString() { return this.SymbolScaleInterval + ":Quotes[" + this.ConsumersQuoteAsShortString + "],Bars[" + this.ConsumersBarAsShortString + "]"; }

		public virtual bool ConsumersQuoteContains(STREAMING_CONSUMER_CHILD consumer) { lock (this.LockConsumersQuote) { return this.ConsumersQuote.Contains(consumer); } }
		public virtual bool ConsumerQuoteAdd(STREAMING_CONSUMER_CHILD quoteConsumer) { lock (this.LockConsumersQuote) {
			if (this.ConsumersQuoteContains(quoteConsumer)) {
				Assembler.PopupException("I_REFUSE_TO_SUBSCRIBE_TWICE quoteConsumer[" + quoteConsumer + "] to [" + this + "]");
				return false;
			}
			this.ConsumersQuote.Add(quoteConsumer);
			return true;
		} }
		public virtual bool ConsumerQuoteRemove(STREAMING_CONSUMER_CHILD quoteConsumer) { lock (this.LockConsumersQuote) {
			if (this.ConsumersQuoteContains(quoteConsumer) == false) {
				Assembler.PopupException("I_REFUSE_TO_REMOVE_UNSUBSCRIBED_CONSUMER quoteConsumer[" + quoteConsumer + "] from [" + this + "]");
				return false;
			}
			this.ConsumersQuote.Remove(quoteConsumer);
			return true;
		} }
		public virtual int ConsumersQuoteCount { get { lock (this.LockConsumersQuote) { return this.ConsumersQuote.Count; } } }

		public virtual bool ConsumersBarContains(STREAMING_CONSUMER_CHILD consumer) { lock (this.LockConsumersBar) { return this.ConsumersBar.Contains(consumer); } }
		public virtual bool ConsumerBarAdd(STREAMING_CONSUMER_CHILD barConsumer) { lock (this.LockConsumersBar) {
			if (this.ConsumersBar.Contains(barConsumer)) {
				Assembler.PopupException("I_REFUSE_TO_SUBSCRIBE_TWICE barConsumer[" + barConsumer + "] to [" + this + "]");
				return false;
			}
			this.ConsumersBar.Add(barConsumer);
			return true;
		} }
		public virtual bool ConsumerBarRemove(STREAMING_CONSUMER_CHILD barConsumer) { lock (this.LockConsumersBar) {
			if (this.ConsumersBar.Contains(barConsumer) == false) {
				Assembler.PopupException("I_REFUSE_TO_REMOVE_UNSUBSCRIBED_CONSUMER barConsumer[" + barConsumer + "] from [" + this + "]");
				return false;
			}
			this.ConsumersBar.Remove(barConsumer);
			return true;
		} }
		public int ConsumersBarCount { get { lock (this.LockConsumersBar) { return this.ConsumersBar.Count; } } }

	}
}
