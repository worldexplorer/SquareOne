using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	// REASON_TO_EXIST: allows to store temporarily incoming streaming quotes to backtest while streaming is on;
	// steps to reproduce 1) I have quotes being generated, 2) Executor.IsStreaming+IsStreamingTriggeringScript,
	// 3) I run Backtest => I need to postpone the reception of the incoming quotes for the duration of the backtest and then continue live orders emission
	public class QuotePump : IDisposable {
		ConcurrentQueue<Quote> qq;
		ManualResetEvent	hasQuoteToPush;			// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through
		ManualResetEvent	confirmThreadStarted;	// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through
		ManualResetEvent	confirmThreadExited;	// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through

		bool				pauseRequested;
		bool				unPauseRequested;
		ManualResetEvent	confirmPaused;		// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through
		ManualResetEvent	confirmUnpaused;	// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through

		SymbolScaleDistributionChannel channel;
		
		const		string	THREAD_PREFIX = "QUOTE_PUMP_FOR_"; 
					Task	bufferPusher;
		readonly	int		heartbeatTimeout;
		const		int		HEARTBEAT_TIMEOUT_DEFAULT = 1000;
					bool	exitPushingThreadRequested;
		const		int		WARN_AFTER_QUOTES_BUFFERED = 10;
					int		timesThreadWasStarted;

		public		bool	UpdateThreadNameAfterMaxConsumersSubscribed;
				Stopwatch	watchForPpausedQuotesProcessing;
		
		bool separatePushingThreadEnabled;
		public bool SeparatePushingThreadEnabled {
			get { return this.separatePushingThreadEnabled; }
			set {
				bool currentlyPushing = this.SeparatePushingThreadEnabled;
				string msig = " //SeparatePushingThreadEnabled[" + currentlyPushing + "]=>[" + value + "] " + this.ToString();
				try {
					if (this.separatePushingThreadEnabled == value) {
						// this.simulationPreBarsSubstitute() waits for 10 seconds
						// you'll be waiting for confirmThreadExited.WaitOne(1000) because there was no running thread to confirm its own exit
						return;
					}
					if (value) {
						this.exitPushingThreadRequested = false;
						if (this.timesThreadWasStarted >= 1) {
							Assembler.PopupException("TESTME_AND_DELETE_IF_OK TASK_MAY_NOT_BE_LAUNCHEABLE_MORE_THAN_ONCE");
						}

						this.confirmThreadStarted.Reset();
						this.bufferPusher.Start();
						bool startConfirmed = this.confirmThreadStarted.WaitOne(this.heartbeatTimeout * 2);
						string msg = startConfirmed ? "PUMPING_THREAD_STARTED" : "PUMPING_THREAD_STARTED_NOT_CONFIRMED";
						Assembler.PopupException(msg + msig, null, false);
					} else {
						this.confirmThreadExited.Reset();
						this.exitPushingThreadRequested = true;
						bool exitConfirmed = this.confirmThreadExited.WaitOne(this.heartbeatTimeout * 2);
						string msg = exitConfirmed ? "THREAD_EXITED__" : "EXITING_THREAD_DIDNT_CONFIRM_ITS_OWN_EXIT__";
						Assembler.PopupException(msg + msig, null, false);
					}
					this.separatePushingThreadEnabled = value;
				} catch (Exception ex) {
					string msg = "IMPOSSIBLE_HAPPENED_WHILE_PUSHING_THREAD_STARTING/STOPPING";
					Assembler.PopupException(msg + msig, ex);
				}
			}
		}
		bool pushConsumersPaused;
		public bool IshouldWaitConfirmationFromAnotherThread { get { return
					Thread.CurrentThread.Name != null
				&&	Thread.CurrentThread.Name.StartsWith(THREAD_PREFIX) == false;
			} }

		public bool PushConsumersPaused {
			get { return this.pushConsumersPaused; }
			set {
				bool currentlyPaused = this.PushConsumersPaused;
				string msig = " //PushConsumersPaused_set[" + currentlyPaused + "]=>[" + value + "] " + this.ToString();
				try {
					if (currentlyPaused == value) {
						// this.simulationPreBarsSubstitute() waits for 10 seconds
						// you'll be waiting for confirmThreadExited.WaitOne(1000) because there was no running thread to confirm its own exit
						return;
					}
					// Thread.CurrentThread.Name=null for Backtester.RunSimulation
					if (value == true) {
						this.confirmPaused.Reset();
						if (this.SeparatePushingThreadEnabled == false) {
							//v1
							//string msg2 = "SKIPPING_PAUSE_PUSHING_THREAD_THAT_HAVENT_STARTED_YET (review how you use QuotePump)";
							//Assembler.PopupException(msg2 + msig, null, true);
							//v2: SOFTENED_PAUSING_REQUIREMENT__SINGLE_THREADING_PAUSED_WILL_DROP_INCOMING_QUOTES__AFTER_DEBUGGING_DONE_COMMENT_UPSTACK_IN_SCRIPT_EXECITOR
							string msg = "PUMPING_PAUSED_SINGLE_THREADED";
							Assembler.PopupException(msg + msig, null, false);
						} else {
							if (this.IshouldWaitConfirmationFromAnotherThread) {
								this.pauseRequested = true;
								this.HasQuoteToPush = true;		// fake gateway open, just to let the thread process pauseRequested=true
								bool pausedConfirmed = this.confirmPaused.WaitOne(this.heartbeatTimeout * 2);
								string msg2 = pausedConfirmed ? "PUMPING_PAUSED" : "PUMPING_PAUSED_NOT_CONFIRMED";
								Assembler.PopupException(msg2 + msig, null, false);
							} else {
								bool unPausedNow = this.confirmUnpaused.WaitOne(0);
								if (unPausedNow == true) {
									string msg = "added complimentary to PushConsumersPaused=false below => I didn't confirm unpausing because I just paused => for whoever might check Wait() un-timely";
									this.confirmUnpaused.Reset();
								}
								string msg2 = "PAUSED_FROM_WITHIN_PUMPING_THREAD__NO_NEED_TO_WAIT_CONFIRMATION";
								//Assembler.PopupException(msg2 + msig, null, false);
							}
						}
					} else {
						this.confirmUnpaused.Reset();
						if (this.SeparatePushingThreadEnabled == false) {
							//v1
							//string msg2 = "SKIPPING_UNPAUSE_PUSHING_THREAD_THAT_HAVENT_STARTED_YET (review how you use QuotePump)";
							//Assembler.PopupException(msg2 + msig, null, true);
							//v2: SOFTENED_UNPAUSING_REQUIREMENT__SINGLE_THREADING_PAUSED_WILL_DROP_INCOMING_QUOTES__AFTER_DEBUGGING_DONE_COMMENT_UPSTACK_IN_SCRIPT_EXECITOR
							string msg2 = "PUMPING_UNPAUSED_SINGLE_THREADED";
							Assembler.PopupException(msg2 + msig, null, false);
						} else {
							if (this.IshouldWaitConfirmationFromAnotherThread) {
								this.unPauseRequested = true;
								this.HasQuoteToPush = true;		// fake gateway open, just to let the thread process unPauseRequested=true
								bool unPausedConfirmed = this.confirmUnpaused.WaitOne(this.heartbeatTimeout * 2);
								string msg = unPausedConfirmed ? "PUMPING_RESUMED" : "PUMPING_RESUMED_NOT_CONFIRMED";
								Assembler.PopupException(msg + msig, null, false);
							} else {
								bool unPausedNow = this.confirmUnpaused.WaitOne(0);
								if (unPausedNow == false) {
									string msg = "added since BrokerMock.SubmitOrder was waiting for 2 minutes after someone has already unpaused"
										+ " ; I have to notify waiters can proceed via WaitUntilUnpaused, even if noone is WaitingOne()";
									this.confirmUnpaused.Set();
								}
								string msg2 = "UNPAUSED_FROM_WITHIN_PUMPING_THREAD__NO_NEED_TO_WAIT_CONFIRMATION";
								//Assembler.PopupException(msg2 + msig, null, false);
							}
						}
					}
					this.pushConsumersPaused = value;
				} catch (Exception ex) {
					string msg = "IMPOSSIBLE_HAPPENED_WHILE_(UN)PAUSING";
					Assembler.PopupException(msg + msig, ex);
				}
			}
		}
		public bool HasQuoteToPush {
			get { return this.hasQuoteToPush.WaitOne(0); }
			protected set {
				if (value == true) this.hasQuoteToPush.Set();
				else this.hasQuoteToPush.Reset();
			}
		}
		
		public QuotePump(SymbolScaleDistributionChannel channel, bool separatePushingThreadEnabled = false, int heartbeatTimeout = HEARTBEAT_TIMEOUT_DEFAULT) {
			this.channel = channel;
			this.heartbeatTimeout = heartbeatTimeout;
			qq = new ConcurrentQueue<Quote>();
			hasQuoteToPush = new ManualResetEvent(false);
			confirmThreadStarted = new ManualResetEvent(false);
			confirmThreadExited = new ManualResetEvent(false);
			bufferPusher = new Task(this.pusherEntryPoint);
			confirmPaused = new ManualResetEvent(false);
			confirmUnpaused = new ManualResetEvent(false);

			//v1
			if (this.SeparatePushingThreadEnabled != separatePushingThreadEnabled) {
				this.SeparatePushingThreadEnabled  = separatePushingThreadEnabled;
			}
			//// else you'll be waiting for confirmThreadExited.WaitOne(1000) because there was no running thread to confirm its own exit
			//v2 it'll exit if both were false => no waiting for confirmation from non-started thread
			// I_STILL_WANT_LESS_NOISE_FOR_THAT_SAFETY_RETURN_BREAKPOINT this.SeparatePushingThreadEnabled = separatePushingThreadEnabled;
			watchForPpausedQuotesProcessing = new Stopwatch();
		}
		public void PushStraightOrBuffered(Quote quoteSernoEnrichedWithUnboundStreamingBar) {
			if (this.separatePushingThreadEnabled == false) {
				if (this.PushConsumersPaused == false) {
					this.channel.PushQuoteToConsumers(quoteSernoEnrichedWithUnboundStreamingBar);
				} else {
					string msg = "IM_PAUSED_AND_SINGLE_THREADED__JUST_DROPPED_QUOTE [" + quoteSernoEnrichedWithUnboundStreamingBar + "]";
					Assembler.PopupException(msg, null, false);
				}
				return;
			}
			if (this.qq.Count == 1) { 
				string msg = "QUOTES_BACKLOG_STARTED_TO_GROW this.qq.Count[" + this.qq.Count + "]";
				//Assembler.PopupException(msg, null, false);
			}
			if (this.qq.Count > 0 && this.qq.Count % WARN_AFTER_QUOTES_BUFFERED == 0) { 
				string msg = "QUOTES_BACKLOG_GREW [" + WARN_AFTER_QUOTES_BUFFERED + "] qq.Count[" + this.qq.Count + "]";
				Assembler.PopupException(msg, null, false);
			}
			qq.Enqueue(quoteSernoEnrichedWithUnboundStreamingBar);
			this.HasQuoteToPush = true;
		}
		void pusherEntryPoint() {
			string msig = "MSIG_NOT_INITIALIZED_YET_pusherEntryPoint()";
			try {
				this.timesThreadWasStarted++;
				this.confirmThreadStarted.Set();
				while (this.exitPushingThreadRequested == false) {
					msig = this.ToString();
					if (this.UpdateThreadNameAfterMaxConsumersSubscribed) {
						if (Thread.CurrentThread.Name != msig) {
							try {
								Thread.CurrentThread.Name = msig;
							} catch (Exception ex) {
								string msg = "SUBSCRIBERS_ADDED_BUT_Thread.CurrentThread.Name_IS_NOT_IN_SYNC";
								Assembler.PopupException(msg, ex, false);
							}
						}
						this.UpdateThreadNameAfterMaxConsumersSubscribed = false;
					}

					bool signalled = this.hasQuoteToPush.WaitOne(this.heartbeatTimeout);
					if (this.exitPushingThreadRequested) {
						string msg = "ABORTING_PUMP_AFTER_SeparatePushingThreadEnabled=false_OR_ IDisposable.Dispose()";
						Assembler.PopupException(msg, null, true);
						break;
					}
					if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms == true) {
						string msg = "MainFormClosingIgnoreReLayoutDockedForms == true";
						Assembler.PopupException(msg, null, false);
						break;	// breaks WHILE and exits the thread
					}

					if (this.pauseRequested) {
						this.pauseRequested = false;
						this.confirmPaused.Set();	// whoever was waiting for PushConsumersPaused=true rest assured that pauseRequested is satisfied (no more quotes pumping anymore until unPauseRequested)
						//continue;							// here might be quotes regardless my fake HasQuoteToPush = true
					}
					if (this.unPauseRequested) {
						this.unPauseRequested = false;
						this.confirmUnpaused.Set();	// whoever was waiting for PushConsumersPaused=false rest assured that unPauseRequested is satisfied (quotes pumping from now on until pauseRequested)
						if (this.qq.Count > 0) {
							string msg = "PUSHER_COLLECTED_QUOTES_DURING_PAUSE: qq.Count[" + this.qq.Count + "]";
							Assembler.PopupException(msg, null, false);
						}
						//continue;							// here might be quotes regardless my fake HasQuoteToPush = true
					}

					if (this.PushConsumersPaused) {
						//if (this.qq.Count == 0) continue;
					    //string msg = "QUOTE_QUEUE_HAS_GROWN_WHILE_PAUSED qq.Count[" + this.qq.Count + "]";
					    //Assembler.PopupException(msg, null, false);
					    continue;
					}
					if (this.qq.Count == 0) {
						continue;
					}

					int quotesCollected = 0;
					int quotesProcessed = 0;
					int customerCalls = 0;
					Quote quoteDequeued;
					while (qq.TryDequeue(out quoteDequeued)) {
						if (quotesCollected == 0) quotesCollected = qq.Count;
						if (quotesCollected  > 0) watchForPpausedQuotesProcessing.Restart();
						try {
							this.channel.PushQuoteToConsumers(quoteDequeued);
							quotesProcessed++;
							customerCalls += this.channel.ConsumersBarCount + this.channel.ConsumersQuoteCount;
						} catch (Exception ex) {
							string msg = "CONSUMER_FAILED_TO_DIGEST_QUOTE recipient[" + this.channel.ToString()
								+ "] quoteDequeued[" + quoteDequeued.ToString() + "]";
							Assembler.PopupException(msg, ex, true);
							continue;
						}
					}
					if (quotesCollected > 0) {
						watchForPpausedQuotesProcessing.Stop();
						string msg = "QUOTES_BACKLOG_DRAINED [" + watchForPpausedQuotesProcessing.ElapsedMilliseconds + "]ms"
							+ " customerCalls[" + customerCalls + "]"
							+ " qCollected[" + quotesCollected + "] qProcessed[" + quotesProcessed + "]";
						Assembler.PopupException(msg, null, false);
					}

					this.HasQuoteToPush = false;
				}
				this.confirmThreadExited.Set();
			} catch (Exception ex) {
				string msg = "PUMPING_THREAD_EXITED_NON_RESUMABLY " + this.bufferPusher.ToString() + " for ChannelManaged[" + this.ToString() + "]";
				Assembler.PopupException(msg, ex);
			}
		}
		#region NOT_USED_YET if I'll need to stop the Pump+AllConsumers when ChartFormsManager gets disposed
		void IDisposable.Dispose() {
			this.exitPushingThreadRequested = true;
			this.HasQuoteToPush = true;		// fake gateway open, just to let the thread process disposed=true; 
		}
		#endregion
		public string ToString() { return THREAD_PREFIX + this.channel.ToString(); }

		public bool WaitUntilUnpaused(int maxWaitingMillis = 1000) {
			bool unpaused = this.confirmUnpaused.WaitOne(maxWaitingMillis);
			return unpaused;
		}
		public bool WaitUntilPaused(int maxWaitingMillis = 1000) {
			bool paused = this.confirmPaused.WaitOne(maxWaitingMillis);
			return paused;
		}
	}
}
