using System;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;

using Sq1.Core.DataTypes;
using Sq1.Core.Livesim;

namespace Sq1.Core.Streaming {
	public class QueuePerSymbol<QUOTE, STREAMING_CONSUMER_CHILD>
								 where STREAMING_CONSUMER_CHILD : StreamingConsumer {

		static		string THREAD_PREFIX_QUEUE	= "QUEUE";	//SINGLE_THREADED_FOR_
		protected	string ScaleInterval_DSN	= "UNKNOWN_ScaleInterval_DSN";
		protected	string ReasonChannelExists	= "UNKNOWN_ReasonChannelExists";

		public		string OfWhat		{ get; private set; }
		public		string ConsumerType	{ get; private set; }

		protected	SymbolChannel<STREAMING_CONSUMER_CHILD>				SymbolChannel;
		protected	ConcurrentQueue<QUOTE>		ConQ;
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

		public QueuePerSymbol(SymbolChannel<STREAMING_CONSUMER_CHILD> channel) {
			ConQ						= new ConcurrentQueue<QUOTE>();
			waitedForBacktestToFinish	= new Stopwatch();

			OfWhat						= typeof(QUOTE).Name;
			ConsumerType				= typeof(STREAMING_CONSUMER_CHILD).Name.Replace("StreamingConsumer", "");
			if (typeof(QUOTE) == typeof(LevelTwoFrozen) && typeof(STREAMING_CONSUMER_CHILD) == typeof(StreamingConsumerSolidifier)) {
				string msg = "DONT_SUBSCRIBE_SOLIDIFIERS_TO_LEVEL_TWO";
				Assembler.PopupException(msg);
			}

			SymbolChannel				= channel;
			if (	this.SymbolChannel != null &&
					this.SymbolChannel.Distributor != null &&
					this.SymbolChannel.Distributor.StreamingAdapter != null &&
					this.SymbolChannel.Distributor.StreamingAdapter.DataSource != null) {
				this.ScaleInterval_DSN = this.SymbolChannel.Distributor.StreamingAdapter.DataSource.ScaleInterval_DSN;
				this.ScaleInterval_DSN = "(" + this.SymbolChannel.Symbol + ":" + this.ScaleInterval_DSN + ")";
				this.ReasonChannelExists = this.SymbolChannel.ReasonIwasCreated_propagatedFromDistributor;
			}
		}
		public virtual int Push_straightOrBuffered_QuotesOrLevels2(QUOTE quote_singleInstance_tillStreamBindsAll__orLevelTwoFrozen) {
			string msig = " //Push_straightOrBuffered_QuotesOrLevels2" + this.ToString();

			if (this.ConQ.Count != 0) {
				string msg = "MUST_BE_EMPTY__NOW_HAS_[" + this.ConQ.Count + "] QUEUE_IS_NOT_FULLY_USED_IN_SINGLE_THREADED";
				Assembler.PopupException(msg);
			}

			// any better solution? (enough of abstract/overridden)
			if (this.OfWhat == typeof(Quote).Name) {
				this.SymbolChannel.PushQuote_toStreams(quote_singleInstance_tillStreamBindsAll__orLevelTwoFrozen as Quote);
			} else if (this.OfWhat == typeof(LevelTwoFrozen).Name) {
				this.SymbolChannel.PushLevelTwoFrozen_toStreams(quote_singleInstance_tillStreamBindsAll__orLevelTwoFrozen as LevelTwoFrozen);
			} else {
				string msg = "I_REFUSE_TO_PUSH_TO_STREAMS__NO_HANDLER_FOR this.OfWhat[" + this.OfWhat + "]";
				Assembler.PopupException(msg + msig);
			}
			return 1;
		}
		protected int FlushQueued_QuotesOrLevels2() {
			string msig = " //FlushQueued_QuotesOrLevels2" + this.ToString();

			int itemsCollected = this.ConQ.Count;
			if (itemsCollected == 0) return -1;

			if (itemsCollected > 1) {
				this.waitedForBacktestToFinish.Restart();
			}
			int itemsProcessed = 0;
			int customerCalls = 0;
			QUOTE quoteDequeued_singleInstance_tillStreamBindsAll__orLevel2;
			while (this.ConQ.TryDequeue(out quoteDequeued_singleInstance_tillStreamBindsAll__orLevel2)) {
				try {

					// any better solution? (enough of abstract/overridden)
					if (this.OfWhat == typeof(Quote).Name) {
						this.SymbolChannel.PushQuote_toStreams(quoteDequeued_singleInstance_tillStreamBindsAll__orLevel2 as Quote);
					} else if (this.OfWhat == typeof(LevelTwoFrozen).Name) {
						this.SymbolChannel.PushLevelTwoFrozen_toStreams(quoteDequeued_singleInstance_tillStreamBindsAll__orLevel2 as LevelTwoFrozen);
					} else {
						string msg = "I_REFUSE_TO_PUSH_TO_STREAMS__NO_HANDLER_FOR this.OfWhat[" + this.OfWhat + "]";
						Assembler.PopupException(msg + msig);
					}

					itemsProcessed++;
					customerCalls += this.SymbolChannel.ConsumersBarCount + this.SymbolChannel.ConsumersQuoteCount;
				} catch (Exception ex) {
					string msg2 = "CONSUMER_FAILED_TO_DIGEST_QUOTE SymbolChannel[" + this.SymbolChannel.ToString()
						+ "] quoteDequeued_singleInstance_tillStreamBindsAll__orLevel2[" + quoteDequeued_singleInstance_tillStreamBindsAll__orLevel2.ToString() + "]";
					Assembler.PopupException(msg2 + msig, ex, true);
					continue;
				}
			}
			if (itemsCollected > 1) {
				this.waitedForBacktestToFinish.Stop();
				string msg = "BACKLOG_DRAINED [" + this.waitedForBacktestToFinish.ElapsedMilliseconds + "]ms"
					+ " customerCalls[" + customerCalls + "]"
					+ " itemsCollected[" + itemsCollected + "] itemsCollected[" + itemsCollected + "]";
				Assembler.PopupException(msg + msig, null, false);
			}
			return itemsProcessed;
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

			// Pump doesn't contain name of the consumers anymore
			//if (this.SymbolChannel.ConsumersBarCount == 0) {
			//    string msg = "INVOKE_ME_LATER_SO_THAT_THREAD_NAME_WILL_CONTAIN_CONSUMER_NAMES_AS_WELL ";
			//    //Assembler.PopupException(msg + msig, null, false);
			//    return;
			//} else {
			//    string msg = "YEAH_NOW_IS_BETTER_TIME_TO_SET_THREAD_NAME__WILL_CONTAIN_CONSUMER_NAMES_AS_WELL ";
			//    //Assembler.PopupException(msg + msig, null, false);
			//}

			Assembler.SetThreadName(msig, "SUBSCRIBERS_ADDED_BUT_Thread.CurrentThread.Name_WAS_ALREADY_SET__REMOVE_THE_FIRST_INVOCATION", false);

			return;
		}

		protected string AsString_cached;
		public override string ToString() {
			if (string.IsNullOrEmpty(this.AsString_cached) == false) return this.AsString_cached;

			string ret = QueuePerSymbol<QUOTE, STREAMING_CONSUMER_CHILD>.THREAD_PREFIX_QUEUE;
			ret += "<" + this.OfWhat + "," + this.ConsumerType  + ">";
			ret += " " + this.ScaleInterval_DSN;
			ret += " //" + this.ReasonChannelExists;
			this.AsString_cached = ret;
			return this.AsString_cached;
		}
	}
}
