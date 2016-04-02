using System;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;

using Sq1.Core.DataTypes;
using Sq1.Core.Livesim;

namespace Sq1.Core.Streaming {
	public class QueuePerSymbol<QUOTE, STREAMING_CONSUMER_CHILD>
								 where STREAMING_CONSUMER_CHILD : StreamingConsumer {
		const string THREAD_PREFIX = "QUEUE_";	//SINGLE_THREADED_FOR_

		//v1 BEFORE_STREAM_WENT_GENERIC
		protected	SymbolChannel<STREAMING_CONSUMER_CHILD>				SymbolChannel;
		protected	ConcurrentQueue<QUOTE>		QQ;
					Stopwatch					waitedForBacktestToFinish;

		public			bool					UpdateThreadNameAfterMaxConsumersSubscribed;
		public			bool					HasSeparatePushingThread						{ get { return this is PumpPerSymbol<QUOTE, STREAMING_CONSUMER_CHILD>; } }
		public virtual	bool					Paused											{ get {
				string msg = "QuoteQueue.Paused: OVERRIDE_ME_KOZ_PAUSING_MAKES_SENSE_FOR_REAL_STREAMING_QUOTE_PUMP_NOT_QUEUE"
					+ " WHILE_ACTIVATING_ONE_OPRIMIZATION_RESULT_YOU_PAUSE_SINGLE_THREADED_BACKTESTER_INSTEAD_OF_STREAMING_PROVIDER?";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg); } }

		//v1 BEFORE_STREAM_WENT_GENERIC
		public QueuePerSymbol(SymbolChannel<STREAMING_CONSUMER_CHILD> channel) {
		//public QueuePerSymbol() {
			QQ							= new ConcurrentQueue<QUOTE>();
			waitedForBacktestToFinish	= new Stopwatch();
			//UpdateThreadNameAfterMaxConsumersSubscribed = true;
			//v1 BEFORE_STREAM_WENT_GENERIC 
			SymbolChannel				= channel;
		}
		public virtual int Push_straightOrBuffered(QUOTE quote_singleInstance_tillStreamBindsAll) {
			if (this.QQ.Count != 0) {
				string msg = "MUST_BE_EMPTY__NOW_HAS_[" + this.QQ.Count + "] QUEUE_IS_NOT_FULLY_USED_IN_SINGLE_THREADED";
				Assembler.PopupException(msg);
			}
			this.SymbolChannel.PushQuote_toStreams(quote_singleInstance_tillStreamBindsAll as Quote);
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
			QUOTE quoteDequeued_singleInstance_tillStreamBindsAll;
			while (this.QQ.TryDequeue(out quoteDequeued_singleInstance_tillStreamBindsAll)) {
				try {
					this.SymbolChannel.PushQuote_toStreams(quoteDequeued_singleInstance_tillStreamBindsAll as Quote);
					quotesProcessed++;
					customerCalls += this.SymbolChannel.ConsumersBarCount + this.SymbolChannel.ConsumersQuoteCount;
				} catch (Exception ex) {
					string msg2 = "CONSUMER_FAILED_TO_DIGEST_QUOTE recipient[" + this.SymbolChannel.ToString()
						+ "] quoteDequeued_clonedUboundUnattached[" + quoteDequeued_singleInstance_tillStreamBindsAll.ToString() + "]";
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

		public virtual void PusherPause_waitUntilPaused(int waitUntilPaused_millis = -1) {
			string msg = "I_REFUSE_TO_BE_PAUSED_BEACUSE_IM_SINGLE_THREADED__USE_MULTI_THREADED_QUOTE_PUMP_TO_PAUSE";
			Assembler.PopupException(msg);
		}

		public virtual void PusherUnpause_waitUntilUnpaused(int waitUntilUnpaused_millis = -1) {
			string msg = "I_REFUSE_TO_BE_UNPAUSED_BEACUSE_IM_SINGLE_THREADED__USE_MULTI_THREADED_QUOTE_PUMP_TO_UNPAUSE";
			Assembler.PopupException(msg);
		}

		public virtual bool WaitUntilUnpaused(int maxWaitingMillis = 1000) {
			bool servingLivesimChannels = this.SymbolChannel.ConsumersQuoteAsString.Contains(Livesimulator.TO_STRING_PREFIX);
			if (this.HasSeparatePushingThread == false && servingLivesimChannels == false) {
				string msg = "PAUSING_MAKES_SENSE_FOR_REAL_STREAMING_QUOTE_PUMP_NOT_QUEUE"
					+ " ONLY_GOOD_FOR_NON-BACKTEST_AND_NON-LIVESIM_CONTROLLED_CHANNELS //WaitUntilUnpaused()";
				Assembler.PopupException(msg);
			}
			return false;
		}
		public virtual bool WaitUntilPaused(int maxWaitingMillis = 1000) {
			bool servingLivesimChannels = this.SymbolChannel.ConsumersQuoteAsString.Contains(Livesimulator.TO_STRING_PREFIX);
			if (this.HasSeparatePushingThread == false && servingLivesimChannels == false) {
				string msg = "PAUSING_MAKES_SENSE_FOR_REAL_STREAMING_QUOTE_PUMP_NOT_QUEUE"
					+ " ONLY_GOOD_FOR_NON-BACKTEST_AND_NON-LIVESIM_CONTROLLED_CHANNELS //WaitUntilPaused()";
				Assembler.PopupException(msg);
			}
			return false;
		}
		public void SetThreadName() {
			if (Thread.CurrentThread.ManagedThreadId == 1) {
				string msg = "I_REFUSE_TO_CHANGE_GUI_THREAD_NAME DONT_INVOKE_ME_ON_DATA_SOURCE_INITIALIZATION [" + Thread.CurrentThread.Name + "]";
				Assembler.PopupException(msg, null, false);
				return;	
			}

			if (string.IsNullOrEmpty(Thread.CurrentThread.Name) == false) return;
			string msig = this.ToString();
			//if (Thread.CurrentThread.Name == msig) return;
			if (msig.Contains("UNKNOWN")) return;

			if (this.SymbolChannel.ConsumersBarCount == 0) {
				string msg = "INVOKE_ME_LATER_SO_THAT_THREAD_NAME_WILL_CONTAIN_CONSUMER_NAMES_AS_WELL ";
				//Assembler.PopupException(msg + msig, null, false);
				return;
			} else {
				string msg = "YEAH_NOW_IS_BETTER_TIME_TO_SET_THREAD_NAME__WILL_CONTAIN_CONSUMER_NAMES_AS_WELL ";
				//Assembler.PopupException(msg + msig, null, false);
			}

			Assembler.SetThreadName(msig, "SUBSCRIBERS_ADDED_BUT_Thread.CurrentThread.Name_WAS_ALREADY_SET__REMOVE_THE_FIRST_INVOCATION", false);

			return;
		}
		public override string ToString() { return THREAD_PREFIX + this.SymbolChannel.ConsumerNames; }
	}
}
