using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sq1.Core.Streaming {
	// REASON_TO_EXIST: allows to store temporarily incoming streaming quotes to backtest while streaming is on;
	// steps to reproduce 1) I have quotes being generated, 2) Executor.IsStreaming+IsStreamingTriggeringScript,
	// 3) I run Backtest => I need to postpone the reception of the incoming quotes for the duration of the backtest and then continue live orders emission
	public class PumpPerSymbol<QUOTE, STREAMING_CONSUMER_CHILD> : QueuePerSymbol<QUOTE, STREAMING_CONSUMER_CHILD>, IDisposable
								 where STREAMING_CONSUMER_CHILD : StreamingConsumer {

		static		string THREAD_PREFIX_PUMP = "PUMP";	//SEPARATE_THREAD_QUOTE_FOR_

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

		public		bool	IsPushingThreadStarted		{ get; private set; }
					bool	signalledTo_pauseUnpauseAbort;

		bool iShouldWaitConfirmationFromPusherThread_notYetInLoop { get {
				//v1
				//return
				//		Thread.CurrentThread.Name != null
				//	&&	Thread.CurrentThread.Name.StartsWith(THREAD_PREFIX) == false;
				//v2 NOPE_ITS_TOO_THICK_FOR_POST_CONSTRUCTOR_TIMES if (this.bufferPusherThreadId == 0) return false;	// ALL_PUMPS_AT_BIRTH_ARE_PAUSED__AVOIDING_INDICATORS_NOT_HAVING_EXECUTOR_AT_APP_RESTART_BACKTEST
				return this.bufferPusherThreadId != Thread.CurrentThread.ManagedThreadId;
			} }
		bool hasQuoteToPush_nonBlocking {
			get { return this.IsDisposed ? false : this.hasQuoteToPush.WaitOne(0); }
			set {
				if (this.IsDisposed) return;
				if (value == hasQuoteToPush_nonBlocking) {
					string msg = "DONT_INVOKE_ME_TWICE__I_DONT_WANNA_SIGNAL_AGAIN_THOSE_WHO_ARE_WAITING__YOU_HAVE_TO_FIX_IT";
					Assembler.PopupException(msg);
					return;
				}
				if (value == true) this.hasQuoteToPush.Set();
				else this.hasQuoteToPush.Reset();
			} }
		bool hasQuoteToPush_blockingAtHeartBeatRate { get {
			bool signalledTrue_expiredFalse = this.hasQuoteToPush.WaitOne(this.heartbeatTimeout);
			if (this.signalledTo_pauseUnpauseAbort) {
				if (signalledTrue_expiredFalse == false) {
					string msg = "IMPOSSIBLE__MUST_BE_signalTo_pauseUnpauseAbort()_WAS_INVOKED_SECOND_TIME_BEFORE_I_EXECUTED_TWO_RESETS_BELOW__ADD_LOCK_STATEMENT?";
					Assembler.PopupException(msg);
				}
				this.signalledTo_pauseUnpauseAbort = false;
				Thread.Sleep(10);	// that helped somewhere else
				// avoiding 100%CPU after livesim paused   original Solidifier/Charts pumps; was done by "fake gateway open" to process pauseRequested=true		in signalTo_pauseUnpauseAbort()
				// avoiding 100%CPU after livesim unpaused original Solidifier/Charts pumps; was done by "fake gateway open" to process unPauseRequested=true	in signalTo_pauseUnpauseAbort()
				this.hasQuoteToPush_nonBlocking  = false;
			}
			return signalledTrue_expiredFalse;
		} }

		~PumpPerSymbol() { this.Dispose(); }
		//v1 BEFORE_STREAM_WENT_GENERIC
		public PumpPerSymbol(SymbolChannel<STREAMING_CONSUMER_CHILD> channel, int heartbeatTimeout = HEARTBEAT_TIMEOUT_DEFAULT) : base(channel) {
		//v2public PumpPerSymbol(int heartbeatTimeout = HEARTBEAT_TIMEOUT_DEFAULT) : base() {
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
			this.pushingThreadStart_waitConfirmed();
			if (this.Paused) {
				string msg = "PUSHER_THREAD_STARTED__DONT_FORGET_UNPAUSE " + this.ToString();
#if DEBUG_STREAMING
				Assembler.PopupException(msg, null, false);
#endif
			}
		}

		public void PushingThreadStop_waitConfirmed() {
			string msig = " //PushingThreadStop isPushingThreadStarted[" + this.IsPushingThreadStarted + "]=>[" + false + "] " + this.ToString();
			if (this.IsPushingThreadStarted == false) {
				// this.simulationPreBarsSubstitute() waits for 10 seconds
				// you'll be waiting for confirmThreadExited.WaitOne(1000) because there was no running thread to confirm its own exit
				return;
			}
			try {
				this.confirmThreadExited.Reset();
				this.exitPushingThreadRequested = true;
				this.signalTo_pauseUnpauseAbort();
				bool exitConfirmed = this.confirmThreadExited.WaitOne(this.heartbeatTimeout * 5);
				string msg = exitConfirmed ? "THREAD_EXITED__" : "EXITING_THREAD_DIDNT_CONFIRM_ITS_OWN_EXIT__";
				Assembler.PopupException(msg + msig, null, false);
			} catch (Exception ex) {
				string msg = "IMPOSSIBLE_HAPPENED_WHILE_PUSHING_THREAD_STARTING/STOPPING";
				Assembler.PopupException(msg + msig, ex);
			}
			this.IsPushingThreadStarted = false;
		}
		void pushingThreadStart_waitConfirmed() {
			string msig = " //pushingThreadStart isPushingThreadStarted[" + this.IsPushingThreadStarted + "]=>[" + true + "] " + this.ToString();
			if (this.IsPushingThreadStarted == true) {
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
				bool startConfirmed = this.confirmThreadStarted.WaitOne(this.heartbeatTimeout * 4);
				string msg = startConfirmed ? "PUMPING_THREAD_STARTED_CONFIRMED" : "PUMPING_THREAD_STARTED_NOT_CONFIRMED";
				if (startConfirmed == false) {
					msg = "ACCELERATE_APPSTARTUP_HERE " + msg;
					Assembler.PopupException(msg + msig, null, false);
				}
			} catch (Exception ex) {
				string msg = "IMPOSSIBLE_HAPPENED_WHILE_PUSHING_THREAD_STARTING";
				Assembler.PopupException(msg + msig, ex);
			}
			this.IsPushingThreadStarted = true;
		}
		void signalTo_pauseUnpauseAbort() {
			// this must go first!!
			this.signalledTo_pauseUnpauseAbort	= true;
			// fake gateway open (forgetting to close it results in 100%CPU/pump), just to let the thread process disposed=true; 
			// fake gateway open (forgetting to close it results in 100%CPU/pump), just to let the thread process pauseRequested=true
			// fake gateway open (forgetting to close it results in 100%CPU/pump), just to let the thread process unPauseRequested=true
			this.hasQuoteToPush_nonBlocking		= true;	
		}
		void pusherEntryPoint() {
			string msig = "THREW_PRIOR_TO_INITIALIZATION_pusherEntryPoint()";
			if (this.bufferPusherThreadId == 0) {
				//INVOKE_ME_LATER_TO_CONTAIN_CONSUMER_NAMES_AS_WELL base.SetThreadName();
				this.bufferPusherThreadId = Thread.CurrentThread.ManagedThreadId;
			} else {
				string msg = "DID_YOU_REACTIVATE I_WANT_THIS.BUFFERPUSHERTHREADID_TO_GET_SET?";
				Assembler.PopupException(msg);
			}
			try {
				this.timesThreadWasStarted++;
				this.confirmThreadStarted.Set();
				while (this.exitPushingThreadRequested == false) {
					bool chartDiThread = this.SymbolChannel.ReasonIwasCreated_propagatedFromDistributor.Contains(DistributorCharts.LIVE_CHARTS_FOR);
					bool solidifThread = this.SymbolChannel.ReasonIwasCreated_propagatedFromDistributor.Contains(DistributorCharts.SOLIDIFIERS_FOR);
					bool livesimThread = this.SymbolChannel.ReasonIwasCreated_propagatedFromDistributor.Contains(DistributorCharts.SUBSTITUTED_LIVESIM_STARTED);
					msig = this.ToString();

					#region OPTIMIZE_ME
					if (this.UpdateThreadNameAfterMaxConsumersSubscribed) {
						string msg = "looks like only for Solidifier Thread (adding more symbols will be reflected in ThreadName after appRestart)";
						base.SetThreadName();
						this.UpdateThreadNameAfterMaxConsumersSubscribed = false;
					} else {
						if (chartDiThread || livesimThread) {
							string msg = "excessive calls here, poor SetThreadName() has to check if ThreadName != null";
							base.SetThreadName();
						} else if (solidifThread) {
							string msg = "ThreadName already set with this.UpdateThreadNameAfterMaxConsumersSubscribed";
						} else {
							string msg = "do you have an effective filtering criteria";
						}
					}
					#endregion

					//DEBUGGING_100%CPU#1
					Stopwatch mustBeHeartBeatInterval = new Stopwatch();
					mustBeHeartBeatInterval.Start();

					bool gotQuoteTrue_pausingUnpausingAbortingTrue_heartBeatExpiredFalse = this.hasQuoteToPush_blockingAtHeartBeatRate;

					//DEBUGGING_100%CPU#2
					mustBeHeartBeatInterval.Stop();
					bool waitedLessThanHalfInterval = mustBeHeartBeatInterval.ElapsedMilliseconds < this.heartbeatTimeout / 2;
					if (waitedLessThanHalfInterval
							//&& livesimThread
						) {
						string msg = "I_MUST_BE_IN_LIVESIM_OR_REAL"
							+ " mustBeHeartBeatInterval.ElapsedMilliseconds[" + mustBeHeartBeatInterval.ElapsedMilliseconds + "]"
							+ " this.heartbeatTimeout[" + this.heartbeatTimeout + "]";
						//Assembler.PopupException(msg, null, false);
					}

					if (this.exitPushingThreadRequested) {
						string msg = "ABORTING_PUMP_AFTER_SeparatePushingThreadEnabled=false_OR_ IDisposable.Dispose()";
						Assembler.PopupException(msg, null, false);
						break;	// breaks WHILE and exits the thread
					}
					if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms == true) {
						string msg = "AM_I_CLOSING_THE_APPLICATION? MainFormClosingIgnoreReLayoutDockedForms == true";
						//Assembler.PopupException(msg);
						break;	// breaks WHILE and exits the thread
					}

					if (this.pauseRequested) {
						this.pauseRequested = false;
						this.confirmPaused.Set();	// whoever was waiting for PushConsumersPaused=true rest assured that pauseRequested is satisfied (no more quotes pumping anymore until unPauseRequested)
						if (this.ConQ.Count > 0) {
							string msg = "PUSHER_COLLECTED_QUOTES_THAT_WILL_STUCK: qq.Count[" + this.ConQ.Count + "] ";
							Assembler.PopupException(msg + msig, null, false);
						}
						continue;	// I_TOLD_TO_PAUSE_THEM!!! here might be quotes regardless my fake HasQuoteToPush = true
					}
					if (this.unPauseRequested) {
						this.unPauseRequested = false;
						this.confirmPaused.Reset();	// this.HasQuoteToPushWrite = true;	//fake gate open
						this.confirmUnpaused.Set();	// whoever was waiting for PushConsumersPaused=false rest assured that unPauseRequested is satisfied (quotes pumping from now on until pauseRequested)
						if (this.ConQ.Count > 0) {
							string msg = "PUSHER_COLLECTED_QUOTES_DURING_PAUSE: qq.Count[" + this.ConQ.Count + "] ";
							Assembler.PopupException(msg + msig, null, false);
						}
						continue;	// I_TOLD_TO_UNPAUSE_THEM!!! here might be quotes regardless my fake HasQuoteToPush = true
					}

					if (this.Paused) {
						//if (this.qq.Count == 0) continue;
						string msg = "PAUSED";
						if (this.ConQ.Count > 0) {
							msg += "_BUT_QUEUE_HAS_GROWN_ALREADY qq.Count[" + this.ConQ.Count + "]";
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

					int quotesCollected = base.ConQ.Count;
					try {
						if (quotesCollected > 0) {
							if (chartDiThread) {
								string msg1 = "I_CAN_BE_TOO_EARLY_INDICATORS_MAY_HAVE_NOT_GOTTEN_EXECUTOR_YET";
							}
							int quotesProcessed = base.FlushQueued_QuotesOrLevels2();
						}
					} catch (Exception exFlush) {
						Assembler.PopupException(msig, exFlush);
					} finally {
						if (this.hasQuoteToPush_nonBlocking == true) {
							this.hasQuoteToPush_nonBlocking  = false;
						}
					}
				}
				this.confirmThreadExited.Set();		// unblocks whoever was Wait()ing
			} catch (Exception ex) {
				string msg = "PUMPING_THREAD_EXITED_NON_RESUMABLY " + this.bufferPusher.ToString() + " for ChannelManaged[" + this.ToString() + "]";
				Assembler.PopupException(msg + msig, ex);
			}
		}
		public override int Push_straightOrBuffered_QuotesOrLevels2(QUOTE quote_singleInstance_tillStreamBindsAll__orLevel2) {
			if (this.IsPushingThreadStarted == false) {
				string msg = "PUSHING_THREAD_MUST_START_IN_CTOR_OTHERWISE_USE_SINGLE_THREADED_QUEUE";
				Assembler.PopupException(msg);
				return 0;
			}
			//I don't need confirmation from the PusherThread because I want to let the StreamingAdapter go ASAP by Enqueueing and returning
			if (this.ConQ.Count == 1) { 
				string msg = "<" + this.OfWhat + ">_BACKLOG_STARTED_TO_GROW this.qq.Count[" + this.ConQ.Count + "]";
				//Assembler.PopupException(msg, null, false);
			}
			if (this.ConQ.Count > 0 && this.ConQ.Count % WARN_AFTER_QUOTES_BUFFERED == 0) { 
				string msg = "<" + this.OfWhat + ">_BACKLOG_GREW [" + WARN_AFTER_QUOTES_BUFFERED + "] qq.Count[" + this.ConQ.Count + "]";
				Assembler.PopupException(msg, null, false);		// just adding to buffered List<Exception> => very quick (sync'ing with GUI is a separate 200ms-timered Task)
			}
			this.ConQ.Enqueue(quote_singleInstance_tillStreamBindsAll__orLevel2);
			if (this.hasQuoteToPush_nonBlocking == false) {
				this.hasQuoteToPush_nonBlocking  = true;
			}
			return 1;
		}

		public	bool IsDisposed { get; private set; }
		public	void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE__" + this.ToString();
				Assembler.PopupException(msg);
				return;
			}

			this.IsDisposed = true;		// do it first so that all threads accessing handles return get FALSE

			if (this.IsPushingThreadStarted) {
				try {
					this.PushingThreadStop_waitConfirmed();
				} catch(Exception ex) {
					Assembler.PopupException(this.ToString() + ".PushingThreadStop()", ex, false);
				}
			}
			try {
				this.bufferPusher.Dispose();
			} catch(Exception ex) {
				Assembler.PopupException(this.ToString() + ".bufferPusher.Dispose()", ex, false);
			}
			
			this.hasQuoteToPush			.Dispose();
			//"CAN_BE_DISPOSED_ONLY_IF_RAN_TILL_COMPLETION...." this.bufferPusher			.Dispose();
			this.confirmThreadStarted	.Dispose();
			this.confirmThreadExited	.Dispose();
			this.confirmPaused			.Dispose();
			this.confirmUnpaused		.Dispose();

			this.exitPushingThreadRequested = true;
			//v1
			//this.hasQuoteToPush_nonBlocking = true;		// fake gateway open (forgetting to close it results in 100%CPU/pump), just to let the thread process disposed=true; 
			//this.signalledTo_pauseUnpauseAbort = true;
			this.signalTo_pauseUnpauseAbort();
		}

		public override void PusherPause_waitUntilPaused(int waitUntilPaused_millis = -1) {
			bool currentlyPaused = this.Paused;
			if (currentlyPaused == true) {
				// this.simulationPreBarsSubstitute() waits for 10 seconds
				// you'll be waiting for confirmThreadExited.WaitOne(1000) because there was no running thread to confirm its own exit
				string msg = "DONT_PAUSE_ME__IM_ALREADY_PAUSED";
				Assembler.PopupException(msg, null, false);
				return;
			}
			string msig = " //PusherPause[" + currentlyPaused + "]=>[" + true + "] " + this.ToString();
			this.confirmPaused.Reset();
			try {
				if (this.iShouldWaitConfirmationFromPusherThread_notYetInLoop) {
					this.confirmPaused.Reset();
					this.pauseRequested = true;
					//v1
					//this.hasQuoteToPush_nonBlocking = true;		// fake gateway open (forgetting to close it results in 100%CPU/pump), just to let the thread process pauseRequested=true
					//this.signalledTo_pauseUnpauseAbort = true;
					this.signalTo_pauseUnpauseAbort();

					//v1 bool pausedConfirmed = this.confirmPaused.WaitOne(this.heartbeatTimeout * 2);
					//v2 bool pausedConfirmed = this.confirmPaused.WaitOne(-1);
					bool pausedConfirmed = this.WaitUntilPaused(waitUntilPaused_millis);
					string msg2 = pausedConfirmed ? "PUSHER_THREAD_PAUSED_PUMPING" : "PUSHER_THREAD_PAUSED_PUMPING_BUT_NOT_CONFIRMED";
#if DEBUG_STREAMING
					Assembler.PopupException(msg2 + msig, null, false);
#endif

					//even for a LivesimStreamingDefault-based no-strategy Chart I wanna see "PAUSED" added to ChartForm.Text
					this.notifyConsumers_pumpWasPaused();
					return;
				}
				bool pausedNow = this.confirmPaused.WaitOne(0);
				if (pausedNow == false) {
					this.confirmPaused.Set();
					this.notifyConsumers_pumpWasPaused();
					this.confirmUnpaused.Reset();
					string msg = "PUSHER_THREAD_PAUSED_FROM_WITHIN_PUMPING_THREAD__NO_NEED_TO_WAIT_CONFIRMATION";
#if DEBUG_STREAMING
					//Assembler.PopupException(msg + msig);
#endif
				} else {
					string msg2 = "PUSHER_THREAD_PAUSED_FROM_WITHIN_PUMPING_THREAD__NO_NEED_TO_WAIT_CONFIRMATION";
#if DEBUG_STREAMING
					Assembler.PopupException(msg2 + msig);
#endif
				}
			} catch (Exception ex) {
				string msg = "IMPOSSIBLE_HAPPENED_WHILE_UNPAUSING";
				Assembler.PopupException(msg + msig, ex);
			}
		}
		public override void PusherUnpause_waitUntilUnpaused(int waitUntilUnpaused_millis = -1) {
			bool currentlyPaused = this.Paused;
			if (currentlyPaused == false) {
				// this.simulationPreBarsSubstitute() waits for 10 seconds
				// you'll be waiting for confirmThreadExited.WaitOne(1000) because there was no running thread to confirm its own exit
				string msg = "DONT_UNPAUSE_ME__IM_ALREADY_UNPAUSED";
				Assembler.PopupException(msg);
				return;
			}
			string msig = " //PusherUnpause[" + currentlyPaused + "]=>[" + true + "] " + this.ToString();
			try {
				if (this.iShouldWaitConfirmationFromPusherThread_notYetInLoop) {
					this.confirmUnpaused.Reset();
					this.unPauseRequested = true;
					//v1
					//this.hasQuoteToPush_nonBlocking = true;		// fake gateway open (forgetting to close it results in 100%CPU/pump), just to let the thread process unPauseRequested=true
					//this.signalledTo_pauseUnpauseAbort = true;
					this.signalTo_pauseUnpauseAbort();

					//v1 bool unPausedConfirmed = this.confirmUnpaused.WaitOne(this.heartbeatTimeout * 2);
					//v2 bool unPausedConfirmed = this.confirmUnpaused.WaitOne(-1);
					bool unPausedConfirmed = this.WaitUntilUnpaused(waitUntilUnpaused_millis);

					string msg = unPausedConfirmed ? "PUSHER_THREAD_UNPAUSED_PUMPING" : "PUSHER_THREAD_UNPAUSED_PUMPING_BUT_NOT_CONFIRMED";
#if DEBUG_STREAMING
					Assembler.PopupException(msg + msig, null, false);
#endif

					//even for a LivesimStreamingDefault-based no-strategy Chart I wanna see "PAUSED" removed to ChartForm.Text
					this.notifyConsumers_pumpWasUnPaused();
					return;
				}
				this.confirmUnpaused.Reset();
				bool unPausedNow = this.confirmUnpaused.WaitOne(0);
				if (unPausedNow == false) {
					this.confirmUnpaused.Set();
					this.notifyConsumers_pumpWasUnPaused();
					this.confirmPaused.Reset();
					string msg = "PUSHER_THREAD_UNPAUSED_FROM_WITHIN_PUMPING_THREAD__NO_NEED_TO_WAIT_CONFIRMATION"
						//+ " added since BrokerMock.SubmitOrder was waiting for 2 minutes after someone has already unpaused"
						//+ " ; I have to notify waiters can proceed via WaitUntilUnpaused, even if noone is WaitingOne()"
						;
#if DEBUG_STREAMING
					//Assembler.PopupException(msg, null, false);
#endif
				} else {
					string msg2 = "UNPAUSED_EARLIER__WRONG_USAGE";
					Assembler.PopupException(msg2 + msig);
				}
			} catch (Exception ex) {
				string msg = "IMPOSSIBLE_HAPPENED_WHILE_UNPAUSING";
				Assembler.PopupException(msg + msig, ex);
			}
		}
		public override bool WaitUntilPaused(int maxWaitingMillis = 1000) {
			bool paused = this.confirmPaused.WaitOne(maxWaitingMillis);
			return paused;
		}
		public override bool WaitUntilUnpaused(int maxWaitingMillis = 1000) {
			bool unpaused = this.confirmUnpaused.WaitOne(maxWaitingMillis);
			return unpaused;
		}

		void notifyConsumers_pumpWasPaused() {
			foreach (StreamingConsumer consumer in this.SymbolChannel.Consumers_QuoteBarLevel2_unique) {
				consumer.PumpPaused_notification_overrideMe_switchLivesimmingThreadToGui();
			}
		}
		void notifyConsumers_pumpWasUnPaused() {
			foreach (StreamingConsumer consumer in this.SymbolChannel.Consumers_QuoteBarLevel2_unique) {
				consumer.PumpUnPaused_notification_overrideMe_switchLivesimmingThreadToGui();
			}
		}

		public override string ToString() {
			if (string.IsNullOrEmpty(base.AsString_cached) == false) return base.AsString_cached;

			string ret = PumpPerSymbol<QUOTE, STREAMING_CONSUMER_CHILD>.THREAD_PREFIX_PUMP;
			ret += "<" + base.OfWhat + "," + base.ConsumerType  + ">";
			ret += " " + base.ScaleInterval_DSN;
			ret += " //" + base.ReasonChannelExists;
			base.AsString_cached = ret;
			return base.AsString_cached;
		}
	}
}
