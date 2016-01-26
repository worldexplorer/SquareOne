using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	// REASON_TO_EXIST: allows to store temporarily incoming streaming quotes to backtest while streaming is on;
	// steps to reproduce 1) I have quotes being generated, 2) Executor.IsStreaming+IsStreamingTriggeringScript,
	// 3) I run Backtest => I need to postpone the reception of the incoming quotes for the duration of the backtest and then continue live orders emission
	public class QuotePumpPerChannel : QuoteQueuePerChannel, IDisposable {
		const string THREAD_PREFIX = "PUMP_";	//SEPARATE_THREAD_QUOTE_FOR_

					ManualResetEvent	hasQuoteToPush;			// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through
					ManualResetEvent	confirmThreadStarted;	// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through
					ManualResetEvent	confirmThreadExited;	// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through

		protected	bool				pauseRequested;
		protected	bool				unPauseRequested;
					ManualResetEvent	confirmPaused;		// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through
					ManualResetEvent	confirmUnpaused;	// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through
		public override bool			Paused { get { return this.confirmPaused.WaitOne(0); } }

					Task	bufferPusher;
					int		bufferPusherThreadId;
		readonly	int		heartbeatTimeout;
		const		int		HEARTBEAT_TIMEOUT_DEFAULT = 1000;
					bool	exitPushingThreadRequested;
		const		int		WARN_AFTER_QUOTES_BUFFERED = 10;
					int		timesThreadWasStarted;

					bool	isPushingThreadStarted;
					bool	signalledTo_pauseUnpauseAbort;

		bool iShouldWaitConfirmationFromAnotherThread { get {
				//v1
				//return
				//		Thread.CurrentThread.Name != null
				//	&&	Thread.CurrentThread.Name.StartsWith(THREAD_PREFIX) == false;
				//v2 NOPE_ITS_TOO_THICK_FOR_POST_CONSTRUCTOR_TIMES if (this.bufferPusherThreadId == 0) return false;	// ALL_PUMPS_AT_BIRTH_ARE_PAUSED__AVOIDING_INDICATORS_NOT_HAVING_EXECUTOR_AT_APP_RESTART_BACKTEST
				return this.bufferPusherThreadId != Thread.CurrentThread.ManagedThreadId;
			} }
		bool hasQuoteToPush_nonBlocking {
			get { return this.hasQuoteToPush.WaitOne(0); }
			set {
				if (value == hasQuoteToPush_nonBlocking) {
					string msg = "DONT_SET_ME_TWICE";
					return;
				}
				if (value == true) this.hasQuoteToPush.Set();
				else this.hasQuoteToPush.Reset();
			} }
		bool hasQuoteToPush_blockingAtHeartBeatRate {
			get { return this.hasQuoteToPush.WaitOne(this.heartbeatTimeout); }
		}

		public QuotePumpPerChannel(SymbolScaleDistributionChannel channel, int heartbeatTimeout = HEARTBEAT_TIMEOUT_DEFAULT) : base(channel) {
			this.heartbeatTimeout	= heartbeatTimeout;
			hasQuoteToPush			= new ManualResetEvent(false);
			bufferPusher			= new Task(this.pusherEntryPoint);
			confirmThreadStarted	= new ManualResetEvent(false);
			confirmThreadExited		= new ManualResetEvent(false);
			confirmPaused			= new ManualResetEvent(false);
			confirmUnpaused			= new ManualResetEvent(false);
			base.UpdateThreadNameAfterMaxConsumersSubscribed = false;
			//v1 NOPE_ITS_TOO_THICK_FOR_POST_CONSTRUCTOR_TIMES_this.PusherPause();	// ALL_PUMPS_AT_BIRTH_ARE_PAUSED__AVOIDING_INDICATORS_NOT_HAVING_EXECUTOR_AT_APP_RESTART_BACKTEST
			this.confirmPaused.Set();		// IF_ON_APP_RESTART_WE_HAVE_BACKTESTS_SCHEDULED_SymbolCScaleDistributionChannel.PumpResumeBacktesterFinishedRemove()_WILL_UNPAUSE_AFTER_THEY_FINISH
			this.pushingThreadStart();
			if (this.Paused) {
				string msg = "DONT_FORGET_UNPAUSE__STARTED_THE_PUSHER_THREAD";
				Assembler.PopupException(msg, null, false);
			}
		}

		void pushingThreadStart() {
			string msig = " //PushingThreadStart[" + this.isPushingThreadStarted + "]=>[" + true + "] " + this.ToString();
			if (this.isPushingThreadStarted == true) {
				// this.simulationPreBarsSubstitute() waits for 10 seconds
				// you'll be waiting for confirmThreadExited.WaitOne(1000) because there was no running thread to confirm its own exit
				return;
			}

			this.exitPushingThreadRequested = false;
			if (this.timesThreadWasStarted >= 1) {
				Assembler.PopupException("TESTME_AND_DELETE_IF_OK QUOTE_PUMP_MUST_START_PUSHING_THREAD_JUST_ONCE_PER_LIFETIME");
			}

			try {
				this.confirmThreadStarted.Reset();
				this.bufferPusher.Start();
				//v1 SET_INSIDE_THE_LAUNCHED_TREAD__in_pusherEntryPoint()
				//int delay = 10;
				//for (int i = 0; i <= 10000; i++) {
				//    if (this.bufferPusherThreadId != 0) {
				//        msig = " this.bufferPusherThreadId[" + this.bufferPusherThreadId + "] " + msig;
				//        string msg2 = "THREAD_ID_SET_AFTER[" + (i * delay) + "]ms[" + i + "]iterations";
				//        Assembler.PopupException(msg2 + msig, null, false);
				//        break;
				//    }
				//    Thread.Sleep(delay);	// I_WANT_THIS.BUFFERPUSHERTHREADID_TO_GET_SET!
				//}
				bool startConfirmed = this.confirmThreadStarted.WaitOne(this.heartbeatTimeout * 2);
				string msg = startConfirmed ? "PUMPING_THREAD_STARTED_CONFIRMED" : "PUMPING_THREAD_STARTED_NOT_CONFIRMED";
				//Assembler.PopupException(msg + msig, null, false);
			} catch (Exception ex) {
				string msg = "IMPOSSIBLE_HAPPENED_WHILE_PUSHING_THREAD_STARTING";
				Assembler.PopupException(msg + msig, ex);
			}
			this.isPushingThreadStarted = true;
		}
		public void PushingThreadStop() {
			string msig = " //PushingThreadStop[" + this.isPushingThreadStarted + "]=>[" + false + "] " + this.ToString();
			if (this.isPushingThreadStarted == false) {
				// this.simulationPreBarsSubstitute() waits for 10 seconds
				// you'll be waiting for confirmThreadExited.WaitOne(1000) because there was no running thread to confirm its own exit
				return;
			}
			try {
				this.confirmThreadExited.Reset();
				this.exitPushingThreadRequested = true;
				bool exitConfirmed = this.confirmThreadExited.WaitOne(this.heartbeatTimeout * 2);
				string msg = exitConfirmed ? "THREAD_EXITED__" : "EXITING_THREAD_DIDNT_CONFIRM_ITS_OWN_EXIT__";
				Assembler.PopupException(msg + msig, null, false);
			} catch (Exception ex) {
				string msg = "IMPOSSIBLE_HAPPENED_WHILE_PUSHING_THREAD_STARTING/STOPPING";
				Assembler.PopupException(msg + msig, ex);
			}
			this.isPushingThreadStarted = false;
		}
		public override int PushStraightOrBuffered(Quote quoteSernoEnrichedWithUnboundStreamingBar) {
			if (this.isPushingThreadStarted == false) {
				string msg = "PUSHING_THREAD_MUST_START_IN_CTOR_OTHERWISE_USE_SINGLE_THREADED_QUEUE";
				Assembler.PopupException(msg);
				return 0;
			}
			if (this.QQ.Count == 1) { 
				string msg = "QUOTES_BACKLOG_STARTED_TO_GROW this.qq.Count[" + this.QQ.Count + "]";
				//Assembler.PopupException(msg, null, false);
			}
			if (this.QQ.Count > 0 && this.QQ.Count % WARN_AFTER_QUOTES_BUFFERED == 0) { 
				string msg = "QUOTES_BACKLOG_GREW [" + WARN_AFTER_QUOTES_BUFFERED + "] qq.Count[" + this.QQ.Count + "]";
				Assembler.PopupException(msg, null, false);
			}
			this.QQ.Enqueue(quoteSernoEnrichedWithUnboundStreamingBar);
			this.hasQuoteToPush_nonBlocking = true;
			return 1;
		}
		void pusherEntryPoint() {
			string msig = "THREW_PRIOR_TO_INITIALIZATION_pusherEntryPoint()";
			if (this.bufferPusherThreadId == 0) {
				base.SetThreadName();
				this.bufferPusherThreadId = Thread.CurrentThread.ManagedThreadId;
			} else {
				string msg = "DID_YOU_REACTIVATE I_WANT_THIS.BUFFERPUSHERTHREADID_TO_GET_SET?";
				Assembler.PopupException(msg);
			}
			try {
				this.timesThreadWasStarted++;
				this.confirmThreadStarted.Set();
				while (this.exitPushingThreadRequested == false) {
					bool chartDiThread = this.Channel.ReasonIwasCreated_propagatedFromDistributor.Contains("DataDistributorCharts");
					bool solidifThread = this.Channel.ReasonIwasCreated_propagatedFromDistributor.Contains("DataDistributorSolidifier");
					bool livesimThread = this.Channel.ReasonIwasCreated_propagatedFromDistributor.Contains("LIVESIM_STARTED");
					msig = this.ToString();

					if (this.UpdateThreadNameAfterMaxConsumersSubscribed) {
						base.SetThreadName();
						this.UpdateThreadNameAfterMaxConsumersSubscribed = false;
					} else {
						if (chartDiThread) {
							base.SetThreadName();
						}
					}

					//DEBUGGING_100%CPU#1
					//Stopwatch mustBeHeartBeatInterval = new Stopwatch();
					//mustBeHeartBeatInterval.Start();

					bool signalledToConsumeEnqueuedQuote = this.hasQuoteToPush_blockingAtHeartBeatRate;

					if (this.signalledTo_pauseUnpauseAbort) {
						this.signalledTo_pauseUnpauseAbort = false;
						// avoiding 100%CPU after livesim paused   original Solidifier/Charts pumps; was done by "fake gateway open" to process pauseRequested=true
						// avoiding 100%CPU after livesim unpaused original Solidifier/Charts pumps; was done by "fake gateway open" to process unPauseRequested=true
						this.hasQuoteToPush_nonBlocking  = false;
					}

					//DEBUGGING_100%CPU#2
					//mustBeHeartBeatInterval.Stop();
					//bool waitedLessThanHalfInterval = mustBeHeartBeatInterval.ElapsedMilliseconds < this.heartbeatTimeout / 2;
					//if (waitedLessThanHalfInterval
					//        && livesimThread
					//    ) {
					//    string msg = "I_MUST_BE_IN_LIVESIM_OR_REAL"
					//        + " mustBeHeartBeatInterval.ElapsedMilliseconds[" + mustBeHeartBeatInterval.ElapsedMilliseconds + "]"
					//        + " this.heartbeatTimeout[" + this.heartbeatTimeout + "]";
					//    //Assembler.PopupException(msg, null, false);
					//}

					if (this.exitPushingThreadRequested) {
						string msg = "ABORTING_PUMP_AFTER_SeparatePushingThreadEnabled=false_OR_ IDisposable.Dispose()";
						Assembler.PopupException(msg, null, false);
						break;	//while loop
					}
					if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms == true) {
						string msg = "AM_I_CLOSING_THE_APPLICATION? MainFormClosingIgnoreReLayoutDockedForms == true";
						//Assembler.PopupException(msg);
						break;	// breaks WHILE and exits the thread
					}

					if (this.pauseRequested) {
						this.pauseRequested = false;
						this.confirmPaused.Set();	// whoever was waiting for PushConsumersPaused=true rest assured that pauseRequested is satisfied (no more quotes pumping anymore until unPauseRequested)
						if (this.QQ.Count > 0) {
							string msg = "PUSHER_COLLECTED_QUOTES_THAT_WILL_STUCK: qq.Count[" + this.QQ.Count + "] ";
							Assembler.PopupException(msg + msig, null, false);
						}
						continue;	// I_TOLD_TO_PAUSE_THEM!!! here might be quotes regardless my fake HasQuoteToPush = true
					}
					if (this.unPauseRequested) {
						this.unPauseRequested = false;
						this.confirmPaused.Reset();	// this.HasQuoteToPushWrite = true;	//fake gate open
						this.confirmUnpaused.Set();	// whoever was waiting for PushConsumersPaused=false rest assured that unPauseRequested is satisfied (quotes pumping from now on until pauseRequested)
						if (this.QQ.Count > 0) {
							string msg = "PUSHER_COLLECTED_QUOTES_DURING_PAUSE: qq.Count[" + this.QQ.Count + "] ";
							Assembler.PopupException(msg + msig, null, false);
						}
						continue;	// I_TOLD_TO_UNPAUSE_THEM!!! here might be quotes regardless my fake HasQuoteToPush = true
					}

					if (this.Paused) {
						//if (this.qq.Count == 0) continue;
						string msg = "PAUSED";
						if (this.QQ.Count > 0) {
							msg += "_BUT_QUEUE_HAS_GROWN_ALREADY qq.Count[" + this.QQ.Count + "]";
						} else {
							msg += "_AND_QUEUE_IS_EMPTY_SO_FAR";
						}
						//Assembler.PopupException(msg + msig, null, false);
						bool unPausedNow = this.confirmUnpaused.WaitOne(this.heartbeatTimeout);
						//bool unPausedNow = this.confirmUnpaused.WaitOne(-1);
						if (unPausedNow == false) {
							if (livesimThread) {
								msg = "STILL_PAUSED_AFTER=" + this.heartbeatTimeout + "sec";
								Assembler.PopupException(msg + msig, null, false);
							} else {
								msg = "WILL_RECHECK_IF_UNPAUSED_ON_NEXT_HEARTBEAT_IN_MILLISEC=" + this.heartbeatTimeout + " " + msg;
								//Assembler.PopupException(msg + msig, null, false);
							}
						}
						continue;
					}
					if (solidifThread == false) {
						string msg1 = "IM_UNPAUSED_AFTER_LIVESIM_OR_BACKTEST_FINISHED_OR_STARTED???=" + msig;
						//Assembler.PopupException(msg1, null, false);
					}

					int quotesCollected = base.QQ.Count;
					try {
						if (quotesCollected > 0) {
							if (chartDiThread) {
								string msg1 = "I_CAN_BE_TOO_EARLY_INDICATORS_MAY_HAVE_NOT_GOTTEN_EXECUTOR_YET";
							}
							int quotesProcessed = base.FlushQuotesQueued();
						}
					} catch (Exception exFlush) {
						Assembler.PopupException(msig, exFlush);
					} finally {
						if (this.hasQuoteToPush_nonBlocking == true) {
							this.hasQuoteToPush_nonBlocking  = false;
						}
					}
				}
				this.confirmThreadExited.Set();
			} catch (Exception ex) {
				string msg = "PUMPING_THREAD_EXITED_NON_RESUMABLY " + this.bufferPusher.ToString() + " for ChannelManaged[" + this.ToString() + "]";
				Assembler.PopupException(msg + msig, ex);
			}
		}
		void signalTo_pauseUnpauseAbort() {
			// this must go first!!
			this.signalledTo_pauseUnpauseAbort	= true;
			// fake gateway open (forgetting to close it results in 100%CPU/pump), just to let the thread process disposed=true; 
			// fake gateway open (forgetting to close it results in 100%CPU/pump), just to let the thread process pauseRequested=true
			// fake gateway open (forgetting to close it results in 100%CPU/pump), just to let the thread process unPauseRequested=true
			this.hasQuoteToPush_nonBlocking		= true;	
		}

		#region NOT_USED_YET if I'll need to stop the Pump+AllConsumers when ChartFormsManager gets disposed
		void IDisposable.Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE__" + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
			this.hasQuoteToPush			.Dispose();
			this.bufferPusher			.Dispose();
			this.confirmThreadStarted	.Dispose();
			this.confirmThreadExited	.Dispose();
			this.confirmPaused			.Dispose();
			this.confirmUnpaused		.Dispose();

			this.exitPushingThreadRequested = true;
			//v1
			//this.hasQuoteToPush_nonBlocking = true;		// fake gateway open (forgetting to close it results in 100%CPU/pump), just to let the thread process disposed=true; 
			//this.signalledTo_pauseUnpauseAbort = true;
			this.signalTo_pauseUnpauseAbort();

			this.IsDisposed = true;
		}
		public bool IsDisposed { get; private set; }
		#endregion

		public override void PusherPause() {
			bool currentlyPaused = this.Paused;
			if (currentlyPaused == true) {
				// this.simulationPreBarsSubstitute() waits for 10 seconds
				// you'll be waiting for confirmThreadExited.WaitOne(1000) because there was no running thread to confirm its own exit
				string msg = "DONT_PAUSE_ME__IM_ALREADY_PAUSED";
				Assembler.PopupException(msg, null, false);
				return;
			}
			string msig = " //pusherPause[" + currentlyPaused + "]=>[" + true + "] " + this.ToString();
			this.confirmPaused.Reset();
			try {
				if (this.iShouldWaitConfirmationFromAnotherThread) {
					this.confirmPaused.Reset();
					this.pauseRequested = true;
					//v1
					//this.hasQuoteToPush_nonBlocking = true;		// fake gateway open (forgetting to close it results in 100%CPU/pump), just to let the thread process pauseRequested=true
					//this.signalledTo_pauseUnpauseAbort = true;
					signalTo_pauseUnpauseAbort();

					//bool pausedConfirmed = this.confirmPaused.WaitOne(this.heartbeatTimeout * 2);
					bool pausedConfirmed = this.confirmPaused.WaitOne(-1);
					string msg2 = pausedConfirmed ? "PUMPING_PAUSED" : "PUMPING_PAUSED_NOT_CONFIRMED";
					Assembler.PopupException(msg2 + msig, null, false);
					return;
				}
				bool pausedNow = this.confirmPaused.WaitOne(0);
				if (pausedNow == false) {
					string msg3 = "PAUSED_FROM_WITHIN_PUMPING_THREAD__NO_NEED_TO_WAIT_CONFIRMATION";
					this.confirmPaused.Set();
					this.confirmUnpaused.Reset();
				} else {
					string msg2 = "PAUSED_FROM_WITHIN_PUMPING_THREAD__NO_NEED_TO_WAIT_CONFIRMATION";
					Assembler.PopupException(msg2 + msig);
				}
			} catch (Exception ex) {
				string msg = "IMPOSSIBLE_HAPPENED_WHILE_UNPAUSING";
				Assembler.PopupException(msg + msig, ex);
			}
		}
		public override void PusherUnpause() {
			bool currentlyPaused = this.Paused;
			if (currentlyPaused == false) {
				// this.simulationPreBarsSubstitute() waits for 10 seconds
				// you'll be waiting for confirmThreadExited.WaitOne(1000) because there was no running thread to confirm its own exit
				string msg = "DONT_UNPAUSE_ME__IM_ALREADY_UNPAUSED";
				Assembler.PopupException(msg);
				return;
			}
			string msig = " //pusherUnpause[" + currentlyPaused + "]=>[" + true + "] " + this.ToString();
			try {
				if (this.iShouldWaitConfirmationFromAnotherThread) {
					this.confirmUnpaused.Reset();
					this.unPauseRequested = true;
					//v1
					//this.hasQuoteToPush_nonBlocking = true;		// fake gateway open (forgetting to close it results in 100%CPU/pump), just to let the thread process unPauseRequested=true
					//this.signalledTo_pauseUnpauseAbort = true;
					this.signalTo_pauseUnpauseAbort();

					//bool unPausedConfirmed = this.confirmUnpaused.WaitOne(this.heartbeatTimeout * 2);
					bool unPausedConfirmed = this.confirmUnpaused.WaitOne(-1);
					string msg = unPausedConfirmed ? "PUMPING_RESUMED" : "PUMPING_RESUMED_NOT_CONFIRMED";
					//Assembler.PopupException(msg + msig, null, false);
					return;
				}
				this.confirmUnpaused.Reset();
				bool unPausedNow = this.confirmUnpaused.WaitOne(0);
				if (unPausedNow == false) {
					string msg = "UNPAUSED_FROM_WITHIN_PUMPING_THREAD__NO_NEED_TO_WAIT_CONFIRMATION"
						//+ " added since BrokerMock.SubmitOrder was waiting for 2 minutes after someone has already unpaused"
						//+ " ; I have to notify waiters can proceed via WaitUntilUnpaused, even if noone is WaitingOne()"
						;
					this.confirmUnpaused.Set();
					this.confirmPaused.Reset();
				} else {
					string msg2 = "UNPAUSED_EARLIER__WRONG_USAGE";
					Assembler.PopupException(msg2 + msig);
				}
			} catch (Exception ex) {
				string msg = "IMPOSSIBLE_HAPPENED_WHILE_UNPAUSING";
				Assembler.PopupException(msg + msig, ex);
			}
		}
		public override bool WaitUntilUnpaused(int maxWaitingMillis = 1000) {
			bool unpaused = this.confirmUnpaused.WaitOne(maxWaitingMillis);
			return unpaused;
		}
		public override bool WaitUntilPaused(int maxWaitingMillis = 1000) {
			bool paused = this.confirmPaused.WaitOne(maxWaitingMillis);
			return paused;
		}
		public override string ToString() { return THREAD_PREFIX + this.Channel.ConsumerNames; }
	}
}
