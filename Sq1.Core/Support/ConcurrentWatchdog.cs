using System;
using System.Collections.Generic;
using System.Threading;

using Sq1.Core.Execution;

namespace Sq1.Core.Support {
	public class ConcurrentWatchdog {
		public		string				reasonToExist				{ get; protected set; }
					object				atomiticity;
					ManualResetEvent	isFree;
					object				lockOwner;
					string				lockPurpose;
					Thread				lockThread;
					int					sameThreadRequestedLocks;

		public ConcurrentWatchdog(string reasonToExist) {
			this.reasonToExist	= reasonToExist;
			this.atomiticity	= new object();
			this.isFree			= new ManualResetEvent(true);
		}
		public bool WaitAndLockFor(object owner, string lockPurpose,
		                           int waitMillis = 2000, bool engageWaitingForEva = true) {
			if (this.lockThread == Thread.CurrentThread) {
				this.sameThreadRequestedLocks++;
				return true;
			}
			bool hadToWaitWasLockedAtFirst = false;
			bool unlocked = this.isFree.WaitOne(waitMillis);
			if (unlocked == false && engageWaitingForEva) {
				hadToWaitWasLockedAtFirst = true;
				if (waitMillis == -1) {
					string msg = "ENGAGING_WAITING_INDEFINITELY_FOR_UNLOCK " + this.ToString()
						+ " IF_THREAD_FROZE_FOREVER_USE_WAIT_MILLIS_TO_FIGURE_OUT_WHO_IS_STILL_KEEPING_THE_LOCK_IF_YOU_ARE_SURE_ITS_NOT_THE_GUY_JUST_REPORTED";
					this.isFree.WaitOne(waitMillis);
				} else {
					while (unlocked == false) {
						unlocked = this.isFree.WaitOne(waitMillis);
						if (unlocked) break;
						string msg = "LOCK_NOT_AQUIRED_WITHIN_MILLIS: [" + waitMillis + "] ";
						Assembler.PopupException(msg + this.ToString(), null, false);
					}
				}
			}
			lock (this.atomiticity) {
				this.lockThread = Thread.CurrentThread;
				this.lockOwner = owner;
				this.lockPurpose = lockPurpose;
				this.isFree.Reset();
				this.sameThreadRequestedLocks++;
				if (hadToWaitWasLockedAtFirst) {
					string msg = "LOCKED_AFTER_WAITING_FOR ";
					Assembler.PopupException (msg + this.ToString());
				}
			}
			return true;
		}
		public bool UnLockFor(object owner, string releasingAfter = null, bool reportViolation = false,
									int waitMillis = 2000, bool engageWaitingForEva = true) {
			if (string.IsNullOrEmpty(releasingAfter)) {
				releasingAfter = this.lockPurpose;
			} else {
				if (releasingAfter != this.lockPurpose) {
					string msg = "releasingAfter[" + releasingAfter + "] != this.LockPurpose[" + this.lockPurpose + "]";
					Assembler.PopupException(msg + this.ToString());
				}
			}
			if (this.lockThread == Thread.CurrentThread) {
				this.sameThreadRequestedLocks--;
				if (this.sameThreadRequestedLocks > 0) {
					return true;
				} // if no stacked locks from the same owner - unlock it! 6 last lines
			} else {
				// SURE YOULL FIND YOUR DEADLOCK HERE
				//bool hadToWaitWasLockedAtFirst = false;
				//bool unlocked = this.isFree.WaitOne(waitMillis);
				//if (unlocked == false && engageWaitingForEva) {
				//	hadToWaitWasLockedAtFirst = true;
				//	if (waitMillis == -1) {
				//		string msg = this.ReasonToExist + ": ENGAGING_WAITING_INDEFINITELY_FOR_UNLOCK LOCK_HELD_BY[" + this.LockOwner
				//			+ "]/[" + this.LockPurpose + "]"
				//			+ " IF_THREAD_FROZE_FOREVER_USE_WAIT_MILLIS_TO_FIGURE_OUT_WHO_IS_STILL_KEEPING_THE_LOCK_IF_YOU_ARE_SURE_ITS_NOT_THE_GUY_JUST_REPORTED";
				//		this.isFree.WaitOne(waitMillis);
				//	} else {
				//		while (unlocked == false) {
				//			unlocked = this.isFree.WaitOne(waitMillis);
				//			if (unlocked) break;
				//			string msg = this.ReasonToExist + ": LOCK_NOT_AQUIRED_WITHIN_MILLIS: [" + waitMillis + "] LOCK_HELD_BY[" + this.LockOwner + "]/[" + this.LockPurpose + "]";
				//			Assembler.PopupException(msg, null, false);
				//		}
				//	}
				//}
				bool mustBeLocked = this.isFree.WaitOne(0);
				if (mustBeLocked) {
					string msg = "WILL_UNLOCK_BUT_YOU_MUST_BE_THE_SAME_OBJECT_WHO_LOCKED YOU_ARE[" + owner + "]/[" + releasingAfter + "]";
					Assembler.PopupException(msg + this.ToString());
				} else {
					string msg = "MUST_BE_UNLOCKED_UNPROOF_OF_CONCEPT YOU_ARE[" + owner + "]/[" + releasingAfter + "]";
					Assembler.PopupException(msg + this.ToString());
				}
			}
			lock (this.atomiticity) {
				this.lockThread = null;
				this.lockOwner = null;
				this.lockPurpose = "UNLOCKED_AFTER_" + releasingAfter;
				this.isFree.Set();
			}
			return true;
		}
		
		public override string ToString() {
			string ret = "ConcurrentWatchdog[" + this.reasonToExist + "]";
			ret  += "LOCK_HELD_BY[" + this.lockOwner + "]/[" + this.lockPurpose + "]";
			if (this.lockThread != null) {
				ret += "managed[" + this.lockThread.ManagedThreadId + "]";
				if (string.IsNullOrEmpty(this.lockThread.Name) == false) ret += ":[" + this.lockThread.Name + "]";
			}
			return ret;
		}
	}
}
