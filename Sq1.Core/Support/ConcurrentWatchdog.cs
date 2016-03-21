using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using Sq1.Core.Execution;

namespace Sq1.Core.Support {
	public class ConcurrentWatchdog : IDisposable {
		public		const int		TIMEOUT_DEFAULT				= 3000;		// 3000ms = 3s

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
					Stack<string>		recursionDetector;

		public ConcurrentWatchdog(string reasonToExist, ExecutionDataSnapshot snap = null) {
			ReasonToExist			= reasonToExist;
			Snap					= snap;
			customerLockingQueue	= new object();
			isFree					= new ManualResetEvent(true);
			stopwatchLock			= new Stopwatch();
			stopwatchUnlock			= new Stopwatch();
			customerUnLockingQueue	= new object();
			recursionDetector		= new Stack<string>();
		}
		public bool IsUnlocked { get {
			bool unlocked = this.WaitUnlocked(0);
			return unlocked;
		} }

		public string LockStack_asString { get {
			string ret = "";
			List<string> stackAsList = new List<string>(this.recursionDetector.ToArray());
			//LOOKS_LIKE_THEY_INSERT_RECENT_INTO_LIST__NO_NEED_TO_REVERSE stackAsList.Reverse();
			foreach (string msig in stackAsList) {
				if (ret != "") ret += "," + Environment.NewLine;
				ret += msig;
			}
			return ret;
		} }

		public bool WaitUnlocked(int waitMillis = TIMEOUT_DEFAULT) {
			bool unlocked = this.isFree.WaitOne(waitMillis);
			return unlocked;
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

			string msig = " //WaitAndLockFor(owner[" + owner + "] lockPurpose[" + lockPurpose + "]) [" + Thread.CurrentThread.ManagedThreadId + "]";
			//string msig = "LOCK_REQUESTED_BY[" + owner.ToString() + "]_FOR[" + lockPurpose + "] ";
			if (this.IsDisposed) {
				string msg = "DISPOSED_WAS_AREADY_INVOKED#1 ";
				Assembler.PopupException(msg + msig);
			}
			if (this.isFree == null) {
				string msg = "DISPOSED_WAS_AREADY_INVOKED#2 isFree = null";
				Assembler.PopupException(msg + msig);
			}
			lock (this.customerLockingQueue) {		// keep same-stack-return above first ever lock() {}
				bool unlocked = this.isFree.WaitOne(waitMillis);
				if (unlocked == false && this.Snap != null) {
					this.Snap.BarkIfAnyScriptOverrideIsRunning("TRYING_TO_LOCK_FOR[" + lockPurpose + "]"
						+ " while ALREADY_LOCKED_FOR[" + this.lockedPurposeFirstInTheStack + "]");
				}

				if (this.recursionDetector.Contains(lockPurpose)) {
					string msg = "YOU_ALREADY_LOCKED_ME_WITH_SAME_REASON[" + lockPurpose + "] lockStack[" + this.LockStack_asString + "] AVOIDING_STACK_OVERFLOW RECURSIVE_CALL";
					Assembler.PopupException(msg + msig, null, false);
				}
				this.recursionDetector.Push(lockPurpose);

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
							Assembler.PopupException(msig + msg + this.Ident, null, false);
						}
					}
				}
				this.stopwatchLock.Stop();
				if (hadToWaitWasLockedAtFirst) {
					string msg = "ACQUIRED_AFTER[" + this.stopwatchLock.ElapsedMilliseconds + "]ms ";
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
		public bool UnLockFor(object owner, string releasingAfter, bool reportViolation = false,
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
			string msig = " //UnLockFor(owner[" + owner + "] releasingAfter[" + releasingAfter + "]) [" + Thread.CurrentThread.ManagedThreadId + "]";
			if (this.IsDisposed) {
				string msg = "DISPOSED_WAS_AREADY_INVOKED#1 ";
				Assembler.PopupException(msg + msig);
			}
			if (this.isFree == null) {
				string msg = "DISPOSED_WAS_AREADY_INVOKED#2 isFree = null";
				Assembler.PopupException(msg + msig);
			}
			// NEVER_USE_THIS__FIRST_CONCURRENT_ACCESS_LEADS_TO_DEADLOCK lock (this.customerLockingQueue) {		// keep same-stack-return above first ever lock() {}
			lock (this.customerUnLockingQueue) {		// keep same-stack-return above first ever lock() {}
				//if (string.IsNullOrEmpty(releasingAfter)) {
				//    releasingAfter = this.lockedPurposeFirstInTheStack;
				//} else {
				//    if (releasingAfter != this.lockedPurposeFirstInTheStack) {
				//        string msg2 = "releasingAfter[" + releasingAfter + "] != this.LockPurpose[" + this.lockedPurposeFirstInTheStack + "]";
				//        Assembler.PopupException(msg2 + this.Ident, null, false);
				//    }
				//}
				string msg = null;
				bool unlocked = this.isFree.WaitOne(0);
				if (unlocked) {
					msg = "MUST_BE_LOCKED__UNPROOF_OF_CONCEPT";
					// DONT_WORRY__BE_HAPPY throw new Exception(msg + msig);
				} else {
					if (this.lockedClass != owner) {
						msg = "YOU_MUST_BE_THE_SAME_OBJECT_WHO_LOCKED this.lockOwner[" + this.lockedClass + "] != owner[" + owner + "]";
						throw new Exception(msg + msig);
					}
					if (this.lockedPurposeFirstInTheStack != releasingAfter) {
						msg = "YOUR_UNLOCK_REASON_MUST_BE_THE_SAME_AS_LOCKED_REASON this.lockPurposeFirstInTheStack["
							+ this.lockedPurposeFirstInTheStack + "] != releasingAfter[" + releasingAfter + "]";
						//NO_IF_I_DONT_USE_TWO_SEPARATE_LOCKS_ILL_ALWAYS_BE_IN_DEADLOCK throw new Exception(msg + msig);
                        //Assembler.PopupException(msg + msig, null, false);
					}
				}

				if (this.recursionDetector.Contains(releasingAfter) == false) {
					string msg1 = "YOU_NEVER_LOCKED_ME_WITH_SAME_REASON_YOU_ARE_RELEASING[" + releasingAfter + "] lockStack[" + this.LockStack_asString + "]";
					Assembler.PopupException(msg1 + msig, null, false);
				} else {
					string lastLockReason = this.recursionDetector.Peek();
					if (lastLockReason != releasingAfter) {
						string msg1 = "MUST_BE_LAST_IN_INVOCATION_STACK[" + Environment.NewLine + Environment.NewLine
							+ releasingAfter
							+ Environment.NewLine + Environment.NewLine + "] lastLockReason[" + Environment.NewLine + Environment.NewLine
							+ lastLockReason
							+ Environment.NewLine + Environment.NewLine + "] lockStack[" + Environment.NewLine + Environment.NewLine
							+ this.LockStack_asString + "]";
						Assembler.PopupException(msg1 + msig, null, false);
					} else {
						this.recursionDetector.Pop();
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
			//	try {
			//		this.unlockedThread.Abort();
			//	} catch (Exception ex) {
			//		string msg = "DONT_RE_THROW";
			//	}
			//}
			this.IsDisposed = true;
			try {
				this.stopwatchLock.Stop();
			} catch (Exception ex) {
				string msg = "THREW_AT_this.stopwatchLock.Stop()";
				Assembler.PopupException(msg, ex);
			}
			try {
				this.stopwatchUnlock.Stop();
			} catch (Exception ex) {
				string msg = "THREW_AT_this.stopwatchUnlock.Stop()";
				Assembler.PopupException(msg);
			}
			try {
				this.isFree.Dispose();
				this.isFree = null;	// I wanted to check this.isFree.IsDisposed() but a WaitHandle doesn't have it; so I nullify and check for null
			} catch (Exception ex) {
				string msg = "THREW_AT_this.isFree.Dispose()";
				Assembler.PopupException(msg);
			}
		}
		public bool IsDisposed { get; private set; }
	}
}
