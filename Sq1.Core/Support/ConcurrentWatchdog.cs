using System;
using System.Collections.Generic;
using System.Threading;

using Sq1.Core.Execution;

namespace Sq1.Core.Support {
	public class ConcurrentWatchdog {
		protected	object				LockObject;
					ManualResetEvent	isFree;
		protected	object				LockOwner					{ get; private set; }
		protected	int					SameOwnerRequestedLocks		{ get; private set; }
		protected	string				LockPurpose					{ get; private set; }
		public		string				ReasonToExist				{ get; protected set; }
		
		public ConcurrentWatchdog(string reasonToExist) {
			ReasonToExist	= reasonToExist;
			LockObject		= new object();
			isFree			= new ManualResetEvent(true);
		}
		public bool LockFor(object owner, string lockPurpose, int waitMillis = -1, bool engageWaitingForEva = false) {
			if (this.LockOwner == owner) {
				this.SameOwnerRequestedLocks++;
				return true;
			}
			bool unlocked = this.isFree.WaitOne(waitMillis);
			if (unlocked == false) {
				if (waitMillis == -1 && engageWaitingForEva) {
					string msg = this.ReasonToExist + ": THREAD_MAY_FREEZE_FOREVER!!! ENGAGING_WAITING_INDEFINITELY_FOR_UNLOCK LOCK_HELD_BY[" + this.LockOwner + "]/[" + this.LockPurpose + "]";
					this.isFree.WaitOne(waitMillis);
				} else {
					string msg = this.ReasonToExist + ": LOCK_NOT_AQUIRED_WITIN_MILLIS: [" + waitMillis + "] LOCK_HELD_BY[" + this.LockOwner + "]/[" + this.LockPurpose + "]";
					Assembler.PopupException(msg);
					return false;
				}
			}
			lock (this.LockObject) {
				this.LockOwner = owner;
				this.LockPurpose = lockPurpose;
				this.isFree.Reset();
				this.SameOwnerRequestedLocks++;
			}
			return true;
		}
		public bool UnLockFor(object owner, string releasingAfter, bool reportViolation = false, int waitMillis = -1) {
			if (this.LockOwner != owner) {
				string msg = this.ReasonToExist + ": WILL_UNLOCK_BUT_YOU_MUST_BE_THE_SAME_OBJECT_WHO_LOCKED YOU_ARE[" + owner + "]/[" + releasingAfter + "] LOCK_HELD_BY[" + this.LockOwner + "]/[" + this.LockPurpose + "]";
				Assembler.PopupException(msg);
			} else {
				if (this.LockOwner == owner) {
					this.SameOwnerRequestedLocks--;
					if (this.SameOwnerRequestedLocks > 0) {
						return true;
					}
				}
				//bool mustBeUnlocked = this.isLocked.WaitOne(waitMillis);
				//if (mustBeUnlocked == false) {
				//    string msg = this.ReasonToExist + ": LOCK_NOT_RELEASED_WITIN_MILLIS: [" + waitMillis + "] LOCK_HELD_BY[" + this.LockOwner + "]/[" + this.LockPurpose + "]";
				//    Assembler.PopupException(msg);
				//    return false;
				//}
			}
			lock (this.LockObject) {
				this.LockOwner = null;
				this.LockPurpose = "UNLOCKED_AFTER_" + this.LockPurpose;
				this.isFree.Set();
			}
			return true;
		}
		
		public override string ToString() {
			return string.Format("ExclusiveLocker[" + this.ReasonToExist + "]");
		}
	}
}
