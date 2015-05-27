using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

using Sq1.Core.Execution;

namespace Sq1.Core.Support {
	public class ConcurrentWatchdog : IDisposable {
		public		const int		TIMEOUT_DEFAULT				= 3000;

		public		string					ReasonToExist { get; protected set; }
		protected	ExecutionDataSnapshot	Snap;

					object				customerLockingQueue;
					ManualResetEvent	isFree;
					object				lockedClass;
					string				lockedPurposeFirstInTheStack;
					Thread				lockedThread;
					int					sameThreadLocksRequestedStackDepth;
					Stopwatch			stopwatchLock;
					Stopwatch			stopwatchUnlock;

					object				customerUnLockingQueue;
					object				unlockedClass;
					string				unlockedAfter;
					Thread				unlockedThread;

		public ConcurrentWatchdog(string reasonToExist, ExecutionDataSnapshot snap = null) {
			ReasonToExist			= reasonToExist;
			Snap					= snap;
			customerLockingQueue	= new object();
			isFree					= new ManualResetEvent(true);
			stopwatchLock			= new Stopwatch();
			stopwatchUnlock			= new Stopwatch();
			customerUnLockingQueue	= new object();
		}
		public bool WaitAndLockFor(object owner, string lockPurpose,
					int waitMillis = TIMEOUT_DEFAULT, bool engageWaitingForEva = true) {
			// lock(){} above WAS DEADLY when, between two stack frames, another thread locked me
			// 1. Thread1,stack1shallow: locked, unlock to come on top level;
			// 2. Thread2 came in, engagedWaitingForEva
			// 3. Thread1,stack2deeper came in and it was held at the lock(){} above
			// 4. Thread1: stack unable to unwind and unlock Thread2 => checkmate in two!
			if (this.lockedThread == Thread.CurrentThread) {
				this.sameThreadLocksRequestedStackDepth++;
				return false;
			}
			lock (this.customerLockingQueue) {		// keep same-stack-return above first ever lock() {}
				bool unlocked = this.isFree.WaitOne(waitMillis);
				if (unlocked == false && this.Snap != null) {
					this.Snap.BarkIfAnyScriptOverrideIsRunning("TRYING_TO_LOCK_FOR[" + lockPurpose + "]"
						+ " while ALREADY_LOCKED_FOR[" + this.lockedPurposeFirstInTheStack + "]");
				}

				bool hadToWaitWasLockedAtFirst = false;
				this.stopwatchLock.Restart();
				if (unlocked == false && engageWaitingForEva) {
					hadToWaitWasLockedAtFirst = true;
					if (waitMillis == -1) {
						string msg = "ENGAGING_WAITING_INDEFINITELY_FOR_UNLOCK "
							//+ " IF_THREAD_FROZE_FOREVER_USE_WAIT_MILLIS_TO_FIGURE_OUT_WHO_IS_STILL_KEEPING_THE_LOCK_IF_YOU_ARE_SURE_ITS_NOT_THE_GUY_JUST_REPORTED"
							;
						this.isFree.WaitOne(waitMillis);
					} else {
						string msig = "LOCK_REQUESTED_BY[" + owner.ToString() + "]_FOR[" + lockPurpose + "] ";
						while (unlocked == false) {
							unlocked = this.isFree.WaitOne(waitMillis);
							if (unlocked) break;
							string msg = "LOCK_NOT_ACQUIRED_WITHIN_MILLIS: [" + this.stopwatchLock.ElapsedMilliseconds + "]/[" + waitMillis + "] ";
							Assembler.PopupException(msig + msg + this.Ident, null, false);
						}
					}
				}
				this.stopwatchLock.Stop();
				if (hadToWaitWasLockedAtFirst) {
					string msig = "LOCK_REQUESTED_BY[" + owner.ToString() + "]_FOR[" + lockPurpose + "] ";
					string msg = "AQUIRED_AFTER[" + this.stopwatchLock.ElapsedMilliseconds + "]ms ";
					Assembler.PopupException(msig + msg + this.Ident, null, false);
				}

				this.lockedThread = Thread.CurrentThread;
				this.lockedClass = owner;
				this.lockedPurposeFirstInTheStack = lockPurpose;

				this.sameThreadLocksRequestedStackDepth++;
				this.isFree.Reset();
				return true;
			}
		}
		public bool UnLockFor(object owner, string releasingAfter = null, bool reportViolation = false,
						int waitMillis = TIMEOUT_DEFAULT, bool engageWaitingForEva = true) {
			// lock(){} above WAS DEADLY when, between two stack frames, another thread locked me
			// 1. Thread1,stack1shallow: locked, unlock to come on top level;
			// 2. Thread2 came in, engagedWaitingForEva
			// 3. Thread1,stack2deeper came in and it was held at the lock(){} above
			// 4. Thread1: stack unable to unwind and unlock Thread2 => checkmate in two!
			if (this.lockedThread == Thread.CurrentThread) {
				this.sameThreadLocksRequestedStackDepth--;
				if (this.sameThreadLocksRequestedStackDepth > 0) {
					return false;
				} // if no stacked locks from the same owner - unlock it! 6 last lines
			}
			lock (this.customerUnLockingQueue) {		// keep same-stack-return above first ever lock() {}
				if (string.IsNullOrEmpty(releasingAfter)) {
					releasingAfter = this.lockedPurposeFirstInTheStack;
				} else {
					if (releasingAfter != this.lockedPurposeFirstInTheStack) {
						string msg2 = "releasingAfter[" + releasingAfter + "] != this.LockPurpose[" + this.lockedPurposeFirstInTheStack + "]";
						Assembler.PopupException(msg2 + this.Ident, null, false);
					}
				}

				string msg = null;
				string youAre = " YOU_ARE_managed[" + Thread.CurrentThread.ManagedThreadId + "]owner[" + owner + "]releasingAfter[" + releasingAfter + "] ";
				bool unlocked = this.isFree.WaitOne(0);
				if (unlocked) {
					msg = "MUST_BE_LOCKED_UNPROOF_OF_CONCEPT";
					throw new Exception(msg);
				} else {
					if (this.lockedClass != owner) {
						msg = "YOU_MUST_BE_THE_SAME_OBJECT_WHO_LOCKED this.lockOwner[" + this.lockedClass + "] != owner[" + owner + "]";
						throw new Exception(msg);
					}
					if (this.lockedPurposeFirstInTheStack != releasingAfter) {
						msg = "YOUR_UNLOCK_REASON_MUST_BE_THE_SAME_AS_LOCKED_REASON this.lockPurposeFirstInTheStack["
							+ this.lockedPurposeFirstInTheStack + "] != releasingAfter[" + releasingAfter + "]";
						throw new Exception(msg);
					}
				}

				this.lockedThread = null;
				this.lockedClass = null;
				this.lockedPurposeFirstInTheStack = null;

				this.unlockedClass = owner;
				this.unlockedAfter = releasingAfter;
				this.unlockedThread = Thread.CurrentThread;

				this.isFree.Set();	// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through
				return true;
			}
		}

		public string Ident { get {
			StringBuilder sb = new StringBuilder();
			if (this.lockedThread != null) {
				sb.Append("LOCK_HELD_BY _managed[");
				sb.Append(this.lockedThread.ManagedThreadId);
				sb.Append("]");
				if (string.IsNullOrEmpty(this.lockedThread.Name) == false) {
					sb.Append(":[");
					sb.Append(this.lockedThread.Name);
					sb.Append("]");
				}
				sb.Append("lockedClass[");
				sb.Append(this.lockedClass);
				sb.Append("]lockedFor[");
				sb.Append(this.lockedPurposeFirstInTheStack);
				sb.Append("]");
				//ret += "ConcurrentWatchdog[" + this.ReasonToExist + "]";
				//if (this is typeof(ConcurrentListWD) == false) {
				//sb.Append("NOW:");
				//sb.Append(this.ToString());
				//}
			}
			if (this.unlockedThread != null) {
				sb.Append("LOCK_WAS_RELEASED_BY_managed[");
				sb.Append(this.unlockedThread.ManagedThreadId);
				sb.Append("]");
				if (string.IsNullOrEmpty(this.unlockedThread.Name) == false) {
					sb.Append(":[");
					sb.Append(this.unlockedThread.Name);
					sb.Append("]");
				}
				sb.Append("unlockedClass[");
				sb.Append(this.unlockedClass);
				sb.Append("]unlockedAfter[");
				sb.Append(this.unlockedAfter);
				sb.Append("]");
				//ret += "ConcurrentWatchdog[" + this.ReasonToExist + "]";
				//if (this is typeof(ConcurrentListWD) == false) {
				//sb.Append("WAS:");
				//sb.Append(this.Ident());
				//}
			}
			return sb.ToString();
		} }

		public void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE__" + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
			//I_ABORT_GUI_THREAD_HERE?!?!???
			//if (this.unlockedThread != null) {
			//    try {
			//        this.unlockedThread.Abort();
			//    } catch (Exception ex) {
			//        string msg = "DONT_RE_THROW";
			//    }
			//}
			try {
				this.stopwatchLock.Stop();
			} catch (Exception ex) {
			}
			try {
				this.stopwatchUnlock.Stop();
			} catch (Exception ex) {
			}
			try {
				this.isFree.Dispose();
			} catch (Exception ex) {
			}
			this.IsDisposed = true;
		}
		public bool IsDisposed { get; private set; }
	}
}
