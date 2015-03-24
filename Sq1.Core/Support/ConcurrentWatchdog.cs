using System.Diagnostics;
using System.Threading;

using Sq1.Core.Execution;
using System;

namespace Sq1.Core.Support {
	public class ConcurrentWatchdog {
		public		const int TIMEOUT_DEFAULT = 3000;

		public		string					ReasonToExist { get; protected set; }
		protected	ExecutionDataSnapshot	Snap;

					object				customerLockingQueue;
					ManualResetEvent	isFree;
					object				lockOwner;
					string				lockPurposeFirstInTheStack;
					Thread				lockThread;
					int					sameThreadLocksRequestedStackDepth;
					Stopwatch			stopwatchLock;
					Stopwatch			stopwatchUnlock;

					//ManualResetEvent	isLocked;
					object				customerUnLockingQueue;
					object				unlockedClass;
					string				unlockedAfter;
					Thread				unlockedThread;

		public string Ident { get { lock(this.customerLockingQueue) {
				string ret = "";
				ret += "LOCK_HELD_BY";
				if (this.lockThread != null) {
					ret += "_managed[" + this.lockThread.ManagedThreadId + "]";
					if (string.IsNullOrEmpty(this.lockThread.Name) == false) ret += ":[" + this.lockThread.Name + "]";
				}
				ret += "owner[" + this.lockOwner + "]lockedFor[" + this.lockPurposeFirstInTheStack + "]";
				//ret += "ConcurrentWatchdog[" + this.ReasonToExist + "]";
				//if (this is typeof(ConcurrentListWD) == false) {
					ret += "NOW:" + this.ToString();
				//}
				return ret;
			} } }
		public string IdentUnlocked { get { lock(this.customerUnLockingQueue) {
				string ret = " ";
				ret += "LOCK_WAS_RELEASED_BY";
				if (this.unlockedThread != null) {
					ret += "_managed[" + this.unlockedThread.ManagedThreadId + "]";
					if (string.IsNullOrEmpty(this.unlockedThread.Name) == false) ret += ":[" + this.unlockedThread.Name + "]";
				}
				ret += "unlockedClass[" + this.unlockedClass + "]unlockedAfter[" + this.unlockedAfter + "]";
				//ret += "ConcurrentWatchdog[" + this.ReasonToExist + "]";
				//if (this is typeof(ConcurrentListWD) == false) {
					ret += ":" + this.ToString();
				//}
				return ret;
			} } }
		public ConcurrentWatchdog(string reasonToExist, ExecutionDataSnapshot snap = null) {
			this.ReasonToExist			= reasonToExist;
			this.Snap					= snap;
			this.customerLockingQueue	= new object();
			this.isFree					= new ManualResetEvent(true);
			//this.isLocked				= new ManualResetEvent(false);
			this.stopwatchLock			= new Stopwatch();
			this.stopwatchUnlock		= new Stopwatch();
			this.customerUnLockingQueue = new object();
		}
		public bool WaitAndLockFor(object owner, string lockPurpose,
					int waitMillis = TIMEOUT_DEFAULT, bool engageWaitingForEva = true) {
			// lock(){} above WAS DEADLY when, between two stack frames, another thread locked me
			// 1. Thread1,stack1shallow: locked, unlock to come on top level;
			// 2. Thread2 came in, engagedWaitingForEva
			// 3. Thread1,stack2deeper came in and it was held at the lock(){} above
			// 4. Thread1: stack unable to unwind and unlock Thread2 => checkmate in two!
			if (this.lockThread == Thread.CurrentThread) {
				this.sameThreadLocksRequestedStackDepth++;
				return false;
			}
			lock (this.customerLockingQueue) {		// keep same-stack-return above first ever lock() {}
				bool unlocked = this.isFree.WaitOne(waitMillis);
				if (unlocked == false && this.Snap != null) {
					this.Snap.BarkIfAnyScriptOverrideIsRunning("TRYING_TO_LOCK_FOR[" + lockPurpose + "]"
						+ " while ALREADY_LOCKED_FOR[" + this.lockPurposeFirstInTheStack + "]");
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
						while (unlocked == false) {
							unlocked = this.isFree.WaitOne(waitMillis);
							if (unlocked) break;
							string msg = "LOCK_NOT_ACQUIRED_WITHIN_MILLIS: [" + this.stopwatchLock.ElapsedMilliseconds + "]/[" + waitMillis + "] ";
							Assembler.PopupException(msg + this.Ident, null, false);
						}
					}
				}
				this.stopwatchLock.Stop();
				if (hadToWaitWasLockedAtFirst) {
					string msg = "LOCKED_AFTER_WAITING_FOR[" + this.stopwatchLock.ElapsedMilliseconds + "]ms FOR ";
					Assembler.PopupException(msg + this.Ident);
				}

				this.lockThread = Thread.CurrentThread;
				this.lockOwner = owner;
				this.lockPurposeFirstInTheStack = lockPurpose;
				this.sameThreadLocksRequestedStackDepth++;

				this.isFree.Reset();
				//this.isLocked.Set();

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
			if (this.lockThread == Thread.CurrentThread) {
				this.sameThreadLocksRequestedStackDepth--;
				if (this.sameThreadLocksRequestedStackDepth > 0) {
					return false;
				} // if no stacked locks from the same owner - unlock it! 6 last lines
			}
			lock (this.customerUnLockingQueue) {		// keep same-stack-return above first ever lock() {}
				if (string.IsNullOrEmpty(releasingAfter)) {
					releasingAfter = this.lockPurposeFirstInTheStack;
				} else {
					if (releasingAfter != this.lockPurposeFirstInTheStack) {
						string msg2 = "releasingAfter[" + releasingAfter + "] != this.LockPurpose[" + this.lockPurposeFirstInTheStack + "]";
						Assembler.PopupException(msg2 + this.Ident, null, false);
					}
				}
				//bool locked = false;

				//bool iAmUnlockedOrLockedToMyself = false;
				string msg = null;
				//bool hadToWaitWasUnLockedAtFirst = false;
				string youAre = " YOU_ARE_managed[" + Thread.CurrentThread.ManagedThreadId + "]owner[" + owner + "]releasingAfter[" + releasingAfter + "] ";
				//this.stopwatchUnlock.Restart();
				//while (iAmUnlockedOrLockedToMyself == false) {
					//locked = this.isLocked.WaitOne(0);
					bool unlocked = this.isFree.WaitOne(0);
					if (unlocked) {
						msg = "MUST_BE_LOCKED_UNPROOF_OF_CONCEPT";
						throw new Exception(msg);
					} else {
						if (this.lockOwner != owner) {
							msg = "YOU_MUST_BE_THE_SAME_OBJECT_WHO_LOCKED this.lockOwner[" + this.lockOwner + "] != owner[" + owner + "]";
							throw new Exception(msg);
						}
						if (this.lockPurposeFirstInTheStack != releasingAfter) {
							msg = "YOUR_UNLOCK_REASON_MUST_BE_THE_SAME_AS_LOCKED_REASON this.lockPurposeFirstInTheStack["
								+ this.lockPurposeFirstInTheStack + "] != releasingAfter[" + releasingAfter + "]";
							throw new Exception(msg);
						}
					}
					//if (string.IsNullOrEmpty(msg)) {
					//    iAmUnlockedOrLockedToMyself = true;
					//    break;
					//}
					//if (engageWaitingForEva) {
					//    if (waitMillis == -1) {
					//        msg = " ENGAGING_WAITING_INDEFINITELY_FOR_UNLOCK "
					//            //+ " IF_THREAD_FROZE_FOREVER_USE_WAIT_MILLIS_TO_FIGURE_OUT_WHO_IS_STILL_KEEPING_THE_LOCK_IF_YOU_ARE_SURE_ITS_NOT_THE_GUY_JUST_REPORTED"
					//            + msg;
					//        Assembler.PopupException(msg + youAre + this.Ident + this.IdentUnlocked, null, false);
					//        hadToWaitWasUnLockedAtFirst = true;
					//        this.isLocked.WaitOne(waitMillis);
					//        iAmUnlockedOrLockedToMyself = true;
					//        break;
					//    }
					//    if (hadToWaitWasUnLockedAtFirst) {
					//        msg = "UNLOCK_WAITED_FOR_LOCK_WITHIN_MILLIS: [" + this.stopwatchUnlock.ElapsedMilliseconds + "]/[" + waitMillis + "] " + msg;
					//        Assembler.PopupException(msg + youAre + this.Ident + this.IdentUnlocked, null, false);
					//    } else {
					//        hadToWaitWasUnLockedAtFirst = true;
					//    }
					//    //this.isLocked.WaitOne(waitMillis);
					//    Thread.Sleep(waitMillis);
					//}
				//}
				//this.stopwatchUnlock.Stop();
				//if (hadToWaitWasUnLockedAtFirst) {
				//    msg = "UNLOCKED_AFTER_WAITING_FOR[" + this.stopwatchUnlock.ElapsedMilliseconds + "]ms FOR ";
				//    Assembler.PopupException(msg + youAre + this.Ident + this.IdentUnlocked, null, false);
				//}

				this.unlockedClass = owner;
				this.unlockedAfter = releasingAfter;
				this.unlockedThread = Thread.CurrentThread;

				this.lockThread = null;
				this.lockOwner = null;
				//this.lockPurposeFirstInTheStack = "UNLOCKED_AFTER_" + releasingAfter;
				this.lockPurposeFirstInTheStack = null;

				//this.isLocked.Reset();
				this.isFree.Set();	// Calling ManualResetEvent.Set opens the gate, allowing any number of threads calling WaitOne to be let through
				return true;
			}
		}
		public override string ToString() { return "USE_IDENT"; }
	}
}
