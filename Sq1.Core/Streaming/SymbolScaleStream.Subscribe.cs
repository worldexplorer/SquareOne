using System;
using System.Collections.Generic;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStream<STREAMING_CONSUMER_CHILD> {
		public		List<STREAMING_CONSUMER_CHILD>		ConsumersQuote					{ get; protected set; }
		public		List<STREAMING_CONSUMER_CHILD>		ConsumersBar					{ get; protected set; }
		public		List<STREAMING_CONSUMER_CHILD>		ConsumersLevelTwoFrozen			{ get; protected set; }
		

		protected	object						LockConsumersQuote;
		protected	object						LockConsumersBar;
		protected	object						LockConsumersLevelTwoFrozen;

		public	List<STREAMING_CONSUMER_CHILD>		Consumers_QuoteAndBar_GroupedInvocation		{ get { lock (this.LockConsumersQuote) {
			List<STREAMING_CONSUMER_CHILD> ret = new List<STREAMING_CONSUMER_CHILD>(this.ConsumersQuote);
			foreach (STREAMING_CONSUMER_CHILD consumer in this.ConsumersBar) {
				if (ret.Contains(consumer)) continue;
				ret.Add(consumer);
			}
			return ret;
		} } }
		public	List<STREAMING_CONSUMER_CHILD>		ConsumersAll_avoidTriplication				{ get { lock (this.LockConsumersQuote) {
			List<STREAMING_CONSUMER_CHILD> ret = new List<STREAMING_CONSUMER_CHILD>(this.ConsumersQuote);
			foreach (STREAMING_CONSUMER_CHILD consumer in this.ConsumersBar) {
				if (ret.Contains(consumer)) continue;
				ret.Add(consumer);
			}
			foreach (STREAMING_CONSUMER_CHILD consumer in this.ConsumersLevelTwoFrozen) {
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
		public string ConsumersLevelTwoFrozenAsString { get { lock (this.LockConsumersLevelTwoFrozen) {
					string ret = "";
					foreach (STREAMING_CONSUMER_CHILD consumer in this.ConsumersLevelTwoFrozen) {
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
		public string ConsumersLevelTwoFrozenNames { get { lock (this.LockConsumersLevelTwoFrozen) {
					string ret = "";
					foreach (STREAMING_CONSUMER_CHILD consumer in this.ConsumersLevelTwoFrozen) {
						if (ret != "") ret += ",";
						ret += consumer.ReasonToExist;
					}
					return ret;
				} } }


		public string ConsumerNames_QuoteBarLevel2	{ get {
			List<STREAMING_CONSUMER_CHILD>	merged = new List<STREAMING_CONSUMER_CHILD>();
			merged.AddRange(this.ConsumersQuote);
			merged.AddRange(this.ConsumersBar);
			merged.AddRange(this.ConsumersLevelTwoFrozen);

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

		public virtual bool	ConsumersQuoteContains(STREAMING_CONSUMER_CHILD consumer) { lock (this.LockConsumersQuote) { return this.ConsumersQuote.Contains(consumer); } }
		public virtual bool	ConsumerQuoteAdd(STREAMING_CONSUMER_CHILD quoteConsumer) { lock (this.LockConsumersQuote) {
			if (this.ConsumersQuoteContains(quoteConsumer)) {
				Assembler.PopupException("I_REFUSE_TO_SUBSCRIBE_TWICE quoteConsumer[" + quoteConsumer + "] to [" + this + "]");
				return false;
			}
			this.ConsumersQuote.Add(quoteConsumer);
			return true;
		} }
		public virtual bool	ConsumerQuoteRemove(STREAMING_CONSUMER_CHILD quoteConsumer) { lock (this.LockConsumersQuote) {
			if (this.ConsumersQuoteContains(quoteConsumer) == false) {
				Assembler.PopupException("I_REFUSE_TO_REMOVE_UNSUBSCRIBED_CONSUMER quoteConsumer[" + quoteConsumer + "] from [" + this + "]");
				return false;
			}
			this.ConsumersQuote.Remove(quoteConsumer);
			return true;
		} }
		public virtual int	ConsumersQuoteCount { get { lock (this.LockConsumersQuote) { return this.ConsumersQuote.Count; } } }

		public virtual bool	ConsumersBarContains(STREAMING_CONSUMER_CHILD consumer) { lock (this.LockConsumersBar) { return this.ConsumersBar.Contains(consumer); } }
		public virtual bool	ConsumerBarAdd(STREAMING_CONSUMER_CHILD barConsumer) { lock (this.LockConsumersBar) {
			if (this.ConsumersBar.Contains(barConsumer)) {
				Assembler.PopupException("I_REFUSE_TO_SUBSCRIBE_TWICE barConsumer[" + barConsumer + "] to [" + this + "]");
				return false;
			}
			this.ConsumersBar.Add(barConsumer);
			return true;
		} }
		public virtual bool	ConsumerBarRemove(STREAMING_CONSUMER_CHILD barConsumer) { lock (this.LockConsumersBar) {
			if (this.ConsumersBar.Contains(barConsumer) == false) {
				Assembler.PopupException("I_REFUSE_TO_REMOVE_UNSUBSCRIBED_CONSUMER barConsumer[" + barConsumer + "] from [" + this + "]");
				return false;
			}
			this.ConsumersBar.Remove(barConsumer);
			return true;
		} }
		public virtual int	ConsumersBarCount { get { lock (this.LockConsumersBar) { return this.ConsumersBar.Count; } } }

		public virtual bool	ConsumersLevelTwoFrozenContains(STREAMING_CONSUMER_CHILD consumer) { lock (this.LockConsumersLevelTwoFrozen) { return this.ConsumersLevelTwoFrozen.Contains(consumer); } }
		public virtual bool	ConsumerLevelTwoFrozenAdd(STREAMING_CONSUMER_CHILD levelTwoFrozenConsumer) { lock (this.LockConsumersLevelTwoFrozen) {
			if (this.ConsumersLevelTwoFrozen.Contains(levelTwoFrozenConsumer)) {
				Assembler.PopupException("I_REFUSE_TO_SUBSCRIBE_TWICE levelTwoFrozenConsumer[" + levelTwoFrozenConsumer + "] to [" + this + "]");
				return false;
			}
			this.ConsumersLevelTwoFrozen.Add(levelTwoFrozenConsumer);
			return true;
		} }
		public virtual bool	ConsumerLevelTwoFrozenRemove(STREAMING_CONSUMER_CHILD levelTwoFrozenConsumer) { lock (this.LockConsumersLevelTwoFrozen) {
			if (this.ConsumersLevelTwoFrozen.Contains(levelTwoFrozenConsumer) == false) {
				Assembler.PopupException("I_REFUSE_TO_REMOVE_UNSUBSCRIBED_CONSUMER levelTwoFrozenConsumer[" + levelTwoFrozenConsumer + "] from [" + this + "]");
				return false;
			}
			this.ConsumersLevelTwoFrozen.Remove(levelTwoFrozenConsumer);
			return true;
		} }
		public virtual int	ConsumersLevelTwoFrozenCount { get { lock (this.LockConsumersLevelTwoFrozen) { return this.ConsumersLevelTwoFrozen.Count; } } }

	}
}
