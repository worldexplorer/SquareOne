using System;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;

using Sq1.Core.DataTypes;
using Sq1.Core.Livesim;

namespace Sq1.Core.Streaming {
	public class QuoteQueuePerChannel {
		const string THREAD_PREFIX = "QUEUE_";	//SINGLE_THREADED_FOR_

		protected	SymbolScaleDistributionChannel	Channel;
		protected	ConcurrentQueue<Quote>			QQ;
					Stopwatch						waitedForBacktestToFinish;

		public			bool	UpdateThreadNameAfterMaxConsumersSubscribed;
		public			bool	HasSeparatePushingThread	{ get { return this is QuotePumpPerChannel; } }
		public virtual	bool	Paused			{ get { throw new Exception("OVERRIDE_ME_KOZ_PAUSING_MAKES_SENSE_FOR_REAL_STREAMING_QUOTE_PUMP_NOT_QUEUE"); } }

		public QuoteQueuePerChannel(SymbolScaleDistributionChannel channel) {
			QQ							= new ConcurrentQueue<Quote>();
			waitedForBacktestToFinish	= new Stopwatch();
			//UpdateThreadNameAfterMaxConsumersSubscribed = true;
			this.Channel				= channel;
		}
		public virtual int PushStraightOrBuffered(Quote quoteSernoEnrichedWithUnboundStreamingBar) {
			if (this.QQ.Count != 0) {
				string msg = "MUST_BE_EMPTY__NOW_HAS_[" + this.QQ.Count + "] QUEUE_IS_NOT_FULLY_USED_IN_SINGLE_THREADED";
				Assembler.PopupException(msg);
			}
			this.Channel.PushQuoteToConsumers(quoteSernoEnrichedWithUnboundStreamingBar);
			return 1;
		}
		protected int FlushQuotesQueued() {
			string msig = this.ToString();

			int quotesCollected = this.QQ.Count;
			if (quotesCollected == 0) return -1;

			if (quotesCollected > 1) {
				this.waitedForBacktestToFinish.Restart();
			}
			int quotesProcessed = 0;
			int customerCalls = 0;
			Quote quoteDequeued;
			while (this.QQ.TryDequeue(out quoteDequeued)) {
				try {
					this.Channel.PushQuoteToConsumers(quoteDequeued);
					quotesProcessed++;
					customerCalls += this.Channel.ConsumersBarCount + this.Channel.ConsumersQuoteCount;
				} catch (Exception ex) {
					string msg2 = "CONSUMER_FAILED_TO_DIGEST_QUOTE recipient[" + this.Channel.ToString()
						+ "] quoteDequeued[" + quoteDequeued.ToString() + "]";
					Assembler.PopupException(msg2 + msig, ex, true);
					continue;
				}
			}
			if (quotesCollected > 1) {
				this.waitedForBacktestToFinish.Stop();
				string msg = "QUOTES_BACKLOG_DRAINED [" + this.waitedForBacktestToFinish.ElapsedMilliseconds + "]ms"
					+ " customerCalls[" + customerCalls + "]"
					+ " qCollected[" + quotesCollected + "] qProcessed[" + quotesProcessed + "]";
				Assembler.PopupException(msg + msig, null, false);
			}
			return quotesProcessed;
		}

		public virtual void PusherPause() {
			string msg = "I_REFUSE_TO_BE_PAUSED_BEACUSE_IM_SINGLE_THREADED__USE_MULTI_THREADED_QUOTE_PUMP_TO_PAUSE";
			Assembler.PopupException(msg);
		}

		public virtual void PusherUnpause() {
			string msg = "I_REFUSE_TO_BE_UNPAUSED_BEACUSE_IM_SINGLE_THREADED__USE_MULTI_THREADED_QUOTE_PUMP_TO_UNPAUSE";
			Assembler.PopupException(msg);
		}

		public virtual bool WaitUntilUnpaused(int maxWaitingMillis = 1000) {
			bool servingLivesimChannels = this.Channel.ConsumersQuoteAsString.Contains(Livesimulator.TO_STRING_PREFIX);
			if (this.HasSeparatePushingThread == false && servingLivesimChannels == false) {
				string msg = "PAUSING_MAKES_SENSE_FOR_REAL_STREAMING_QUOTE_PUMP_NOT_QUEUE"
					+ " ONLY_GOOD_FOR_NON-BACKTEST_AND_NON-LIVESIM_CONTROLLED_CHANNELS //WaitUntilUnpaused()";
				Assembler.PopupException(msg);
			}
			return false;
		}
		public virtual bool WaitUntilPaused(int maxWaitingMillis = 1000) {
			bool servingLivesimChannels = this.Channel.ConsumersQuoteAsString.Contains(Livesimulator.TO_STRING_PREFIX);
			if (this.HasSeparatePushingThread == false && servingLivesimChannels == false) {
				string msg = "PAUSING_MAKES_SENSE_FOR_REAL_STREAMING_QUOTE_PUMP_NOT_QUEUE"
					+ " ONLY_GOOD_FOR_NON-BACKTEST_AND_NON-LIVESIM_CONTROLLED_CHANNELS //WaitUntilPaused()";
				Assembler.PopupException(msg);
			}
			return false;
		}
		public void SetThreadName() {
			string msig = this.ToString();
			if (Thread.CurrentThread.Name == msig) return;

			if (this.Channel.ConsumersBarCount == 0) {
				string msg = "INVOKE_ME_LATER_SO_THAT_THREAD_NAME_WILL_CONTAIN_CONSUMER_NAMES_AS_WELL ";
				//Assembler.PopupException(msg + msig, null, false);
			} else {
				string msg = "YEAH_NOW_IS_BETTER_TIME_TO_SET_THREAD_NAME__WILL_CONTAIN_CONSUMER_NAMES_AS_WELL ";
				//Assembler.PopupException(msg + msig, null, false);
			}

			try {
				Thread.CurrentThread.Name = msig;
			} catch (Exception ex) {
				string msg = "SUBSCRIBERS_ADDED_BUT_Thread.CurrentThread.Name_WAS_ALREADY_SET__REMOVE_THE_FIRST_INVOCATION";
				Assembler.PopupException(msg, ex, false);
			}
			return;
		}
		public override string ToString() { return THREAD_PREFIX + this.Channel.ToString(); }
	}
}
