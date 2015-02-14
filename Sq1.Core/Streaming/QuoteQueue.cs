using System;
using System.Threading;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public class QuoteQueue {
		protected SymbolScaleDistributionChannel channel;
		public const string THREAD_PREFIX = "QUOTE_PUMP_FOR_"; 

		public QuoteQueue(SymbolScaleDistributionChannel channel) {
			this.channel = channel;
		}
		public virtual void PushStraightOrBuffered(Quote quoteSernoEnrichedWithUnboundStreamingBar) {
			this.channel.PushQuoteToConsumers(quoteSernoEnrichedWithUnboundStreamingBar);
		}
		public void SetThreadName() {
			string msig = this.ToString();
			if (Thread.CurrentThread.Name == msig) return;

			if (this.channel.ConsumersBarCount > 0) {
				string msg = "INVOKE_ME_LATER_SO_THAT_THREAD_NAME_WILL_CONTAIN_CONSUMER_NAMES_AS_WELL ";
				Assembler.PopupException(msg + msig, null, false);
			} else {
				string msg = "YEAH_NOW_IS_BETTER_TIME_TO_SET_THREAD_NAME__WILL_CONTAIN_CONSUMER_NAMES_AS_WELL ";
				Assembler.PopupException(msg + msig, null, false);
			}

			try {
				Thread.CurrentThread.Name = msig;
			} catch (Exception ex) {
				string msg = "SUBSCRIBERS_ADDED_BUT_Thread.CurrentThread.Name_WAS_ALREADY_SET__REMOVE_THE_FIRST_INVOCATION";
				Assembler.PopupException(msg, ex, false);
			}
			return;
		}
		public string ToString() { return THREAD_PREFIX + this.channel.ToString(); }
	}
}
