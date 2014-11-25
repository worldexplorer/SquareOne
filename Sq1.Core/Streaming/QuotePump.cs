using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	// REASON_TO_EXIST: allows to store temporarily incoming streaming quotes to backtest while streaming is on;
	// steps to reproduce 1) I have quotes being generated, 2) Executor.IsStreaming+IsStreamingTriggeringScript,
	// 3) I run Backtest => I need to postpone the reception of the incoming quotes for the duration of the backtest and then continue live orders emission
	public class QuotePump : IDisposable {
		ConcurrentQueue<Quote> qq;
		ManualResetEvent hasQuoteToPush;		// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through
		ManualResetEvent confirmThreadStarted;	// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through
		ManualResetEvent confirmThreadExited;	// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through

		bool pauseRequested;
		bool unPauseRequested;
		ManualResetEvent pumpingPaused;			// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through
		ManualResetEvent confirmPauseSwitched;	// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through

		SymbolScaleDistributionChannel channel;
		Task bufferPusher;
		readonly int heartbeatTimeout;
		const int HEARTBEAT_TIMEOUT_DEFAULT = 1000;
		bool exitPushingThreadRequested;
		int quotesPrevWarning;

		public bool UpdateThreadNameSinceMaxConsumersSubscribed;
		
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
					bool launchingSecondTime = (this.separatePushingThreadEnabled = false && value == true);
					this.separatePushingThreadEnabled = value;
					if (this.separatePushingThreadEnabled) {
						this.exitPushingThreadRequested = false;
						if (launchingSecondTime) {
							Assembler.PopupException("TASK_MAY_NOT_BE_LAUNCHEABLE_MORE_THAN_ONCE FIRST_LAUNCH_WAS_IN_CTOR");
						}

						this.confirmThreadStarted.Reset();
						this.bufferPusher.Start();
						bool startConfirmed = this.confirmThreadStarted.WaitOne(10000);
						string msg = startConfirmed ? "PUMPING_THREAD_STARTED" : "PUMPING_THREAD_STARTED_NOT_CONFIRMED";
						Assembler.PopupException(msg + msig, null, false);
					} else {
						this.confirmThreadExited.Reset();
						this.exitPushingThreadRequested = true;
						bool exitConfirmed = this.confirmThreadExited.WaitOne(10000);
						string msg = exitConfirmed ? "THREAD_EXITED__" : "EXITING_THREAD_DIDNT_CONFIRM_ITS_OWN_EXIT__";
						Assembler.PopupException(msg + msig, null, false);
					}
				} catch (Exception ex) {
					string msg = "IMPOSSIBLE_HAPPENED_WHILE_PUSHING_THREAD_STARTING/STOPPING";
					Assembler.PopupException(msg + msig, ex);
				}
			}
		}
		public bool PushConsumersPaused {
			get { return this.pumpingPaused.WaitOne(0); }
			set {
				bool currentlyPaused = this.PushConsumersPaused;
				string msig = " //PushConsumersPaused_set[" + currentlyPaused + "]=>[" + value + "] " + this.ToString();
				try {
					if (currentlyPaused == value) {
						// this.simulationPreBarsSubstitute() waits for 10 seconds
						// you'll be waiting for confirmThreadExited.WaitOne(1000) because there was no running thread to confirm its own exit
						return;
					}
					if (value == true) {
						this.confirmPauseSwitched.Reset();
						this.pauseRequested = true;
						this.HasQuoteToPush = true;		// fake gateway open, just to let the thread process pauseRequested=true
						bool pausedConfirmed = this.confirmPauseSwitched.WaitOne(10000);
						string msg = pausedConfirmed ? "PUMPING_PAUSED" : "PUMPING_PAUSED_NOT_CONFIRMED";
						Assembler.PopupException(msg + msig, null, false);
					} else {
						this.confirmPauseSwitched.Reset();
						this.unPauseRequested = true;
						this.HasQuoteToPush = true;		// fake gateway open, just to let the thread process unPauseRequested=true
						bool unPausedConfirmed = this.confirmPauseSwitched.WaitOne(10000);
						string msg = unPausedConfirmed ? "PUMPING_RESUMED" : "PUMPING_RESUMED_NOT_CONFIRMED";
						Assembler.PopupException(msg + msig, null, false);
					}
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
			pumpingPaused = new ManualResetEvent(false);
			hasQuoteToPush = new ManualResetEvent(false);
			confirmThreadStarted = new ManualResetEvent(false);
			confirmThreadExited = new ManualResetEvent(false);
			bufferPusher = new Task(this.pusherEntryPoint);
			confirmPauseSwitched = new ManualResetEvent(false);

			//v1
			if (this.SeparatePushingThreadEnabled != separatePushingThreadEnabled) this.SeparatePushingThreadEnabled = separatePushingThreadEnabled;
			//// else you'll be waiting for confirmThreadExited.WaitOne(1000) because there was no running thread to confirm its own exit
			//v2 it'll exit if both were false => no waiting for confirmation from non-started thread
			// I_STILL_WANT_LESS_NOISE_FOR_THAT_SAFETY_RETURN_BREAKPOINT this.SeparatePushingThreadEnabled = separatePushingThreadEnabled;
		}
		public void PushStraightOrBuffered(Quote quoteSernoEnrichedWithUnboundStreamingBar) {
			if (this.separatePushingThreadEnabled == false) {
				this.channel.PushQuoteToConsumers(quoteSernoEnrichedWithUnboundStreamingBar);
				return;
			}
			if (this.quotesPrevWarning == 0 || this.qq.Count % 1000 > 0) {
				int nowInQueue = (int)(this.qq.Count / 1000);
				if (this.quotesPrevWarning < nowInQueue) {
					this.quotesPrevWarning = nowInQueue;
					Assembler.PopupException("QUOTES_BACKLOG_GREWUP_FOR_ONE_MORE_THOUSAND this.qq.Count[" + this.qq.Count + "]");
				}
			}
			qq.Enqueue(quoteSernoEnrichedWithUnboundStreamingBar);
			this.HasQuoteToPush = true;
		}
		void pusherEntryPoint() {
			string msig = "MSIG_NOT_INITIALIZED_YET_pusherEntryPoint()";
			try {
				this.confirmThreadStarted.Set();
				while (this.exitPushingThreadRequested == false) {
					msig = this.ToString();
					if (this.UpdateThreadNameSinceMaxConsumersSubscribed) {
						if (Thread.CurrentThread.Name != msig) {
							try {
								Thread.CurrentThread.Name = msig;
							} catch (Exception ex) {
								string msg = "SUBSCRIBERS_ADDED_BUT_Thread.CurrentThread.Name_IS_NOT_IN_SYNC";
								Assembler.PopupException(msg, ex, false);
							}
						}
						this.UpdateThreadNameSinceMaxConsumersSubscribed = false;
					}

					bool signalled = this.hasQuoteToPush.WaitOne(this.heartbeatTimeout);
					if (this.exitPushingThreadRequested) {
						string msg = "ABORTING_PUMP_AFTER_SeparatePushingThreadEnabled=false_OR_ IDisposable.Dispose()";
						//Assembler.PopupException(msg, null, true);
						break;
					}
					if (this.pauseRequested) {
						this.pauseRequested = false;
						pumpingPaused.Set();
						this.confirmPauseSwitched.Set();	// whoever was waiting for PushConsumersPaused=true rest assured that pauseRequested is satisfied (no more quotes pumping anymore until unPauseRequested)
						//continue;							// here might be quotes regardless my fake HasQuoteToPush = true
					}
					if (this.unPauseRequested) {
						this.unPauseRequested = false;
						pumpingPaused.Reset();
						this.confirmPauseSwitched.Set();	// whoever was waiting for PushConsumersPaused=false rest assured that unPauseRequested is satisfied (quotes pumping from now on until pauseRequested)
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
					if (this.qq.Count == 0) continue;

					Quote quoteDequeued;
					while (qq.TryDequeue(out quoteDequeued)) {
						try {
							this.channel.PushQuoteToConsumers(quoteDequeued);
						} catch (Exception ex) {
							string msg = "CONSUMER_FAILED_TO_DIGEST_QUOTE recipient[" + this.channel.ToString()
								+ "] quoteDequeued[" + quoteDequeued.ToString() + "]";
							Assembler.PopupException(msg, ex, true);
							continue;
						}
					}
					this.HasQuoteToPush = false;
				}
				this.confirmThreadExited.Set();
			} catch (Exception ex) {
				string msg = "PUMPING_THREAD_EXITED_NON_RESUMABLY " + this.bufferPusher.ToString() + " for ChannelManaged[" + this.ToString() + "]";
				Assembler.PopupException(msg, ex);
			}
		}
		#region IDisposable implementation, just in case I'll need to abort the Pump when ChartFormsManager gets disposed
		void IDisposable.Dispose() {
			this.exitPushingThreadRequested = true;
			this.HasQuoteToPush = true;		// fake gateway open, just to let the thread process disposed=true; 
		}
		#endregion
		public string ToString() { return "QUOTE_PUMP_FOR_" + this.channel.ToString(); }
	}
}
