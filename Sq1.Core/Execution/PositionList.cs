using System;
using System.Collections.Generic;

using Sq1.Core.Support;

namespace Sq1.Core.Execution {
	public class PositionList : ConcurrentList<Position>, IDisposable {
		public Dictionary<int, List<Position>>	ByEntryBarFilled	{ get; protected set; }
		public Dictionary<int, List<Position>>	ByExitBarFilled		{ get; protected set; }
		
		public Dictionary<int, List<Position>>	ByExitBarFilled_safeCopy(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			lockPurpose += " //" + base.ReasonToExist + "..ByExitBarFilledSafeCopy()";
			Dictionary<int, List<Position>> ret = new Dictionary<int, List<Position>>();
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				foreach (int bar in this.ByExitBarFilled.Keys) {
					if (ret.ContainsKey(bar) == false) {
						ret.Add(bar, new List<Position>(this.ByExitBarFilled[bar]));
					} else {
						string msg = "INVESTIGATE_THIS";
						Assembler.PopupException(msg + lockPurpose);
						List<Position> alreadyThere = ret[bar];
						alreadyThere.AddRange(this.ByExitBarFilled[bar]);
					}
				}
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return ret;
		}
		public Dictionary<int, List<Position>>	ByEntryBarFilled_safeCopy(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			lockPurpose += " //" + base.ReasonToExist + ".ByExitBarFilledSafeCopy()";
			Dictionary<int, List<Position>> ret = new Dictionary<int, List<Position>>();
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				foreach (int bar in this.ByEntryBarFilled.Keys) {
					if (ret.ContainsKey(bar) == false) {
						ret.Add(bar, new List<Position>(this.ByEntryBarFilled[bar]));
					} else {
						string msg = "INVESTIGATE_THIS";
						Assembler.PopupException(msg + lockPurpose);
						List<Position> alreadyThere = ret[bar];
						alreadyThere.AddRange(this.ByEntryBarFilled[bar]);
					}
				}
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
			return ret;
		}
		#region AVOIDING_HEADACHE_DISPOSING_THOSE
		//public AlertList AlertsEntry { get {
		//	AlertList ret = new AlertList("AlertsEntry", base.Snap);
		//	foreach (List<Position> positionsOpened in this.ByEntryBarFilled.Values) {
		//		foreach (Position position in positionsOpened) {
		//			if (position.EntryAlert == null) continue;
		//			ret.AddNoDupe(position.EntryAlert, this, "AlertsEntry(WAIT)");
		//		}
		//	}
		//	return ret;
		//} }
		//public AlertList AlertsExit { get {
		//	AlertList ret = new AlertList("AlertsExit", base.Snap);
		//	foreach (List<Position> positionsClosed in this.ByExitBarFilled.Values) {
		//		foreach (Position position in positionsClosed) {
		//			if (position.ExitAlert == null) continue;
		//			ret.AddNoDupe(position.ExitAlert, this, "AlertsExit(WAIT)");
		//		}
		//	}
		//	return ret;
		//} }
		//public AlertList AlertsOpenNow { get {
		//	AlertList ret = new AlertList("AlertsOpenNow", base.Snap);
		//	foreach (Position position in base.InnerList) {
		//		if (position.ExitAlert == null) continue;
		//		ret.AddNoDupe(position.ExitAlert, this, "AlertsOpenNow(WAIT)");
		//	}
		//	return ret;
		//} }
		#endregion

		public int LastBarIndexEntry;
		public int LastBarIndexExit;

		public PositionList(string reasonToExist, ExecutorDataSnapshot snap = null, List<Position> copyFrom = null) : this(reasonToExist, snap) {
			if (copyFrom == null) return;
			base.InnerList.AddRange(copyFrom);
			Count = base.InnerList.Count;
		}
		public PositionList(string reasonToExist, ExecutorDataSnapshot snap = null) : base(reasonToExist, snap) {
			ByEntryBarFilled	= new Dictionary<int, List<Position>>();
			ByExitBarFilled		= new Dictionary<int, List<Position>>();
			LastBarIndexEntry	= -1;
			LastBarIndexExit	= -1;
		}
		public void Clear(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			base					.Clear(owner, lockPurpose, waitMillis);
			this.ByEntryBarFilled	.Clear();
			this.ByExitBarFilled	.Clear();
			this.LastBarIndexEntry	= -1;
			this.LastBarIndexExit	= -1;
		}
		public void DisposeTwoRelatedAlerts_waitHandles_andClearInnerList(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			lockPurpose += " //" + base.ReasonToExist + ".DisposeTwoRelatedAlertsWaitHandlesAndClear()";
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				foreach (Position pos in base.InnerList) pos.Dispose();
				this.Clear(lockOwner, lockPurpose, waitMillis);
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}

		// NEVER_USED__UNCOMMENT_WHEN_YOU_NEED_IT public void AddRangeOpened(List<Position> positions) {
		//	foreach (Position position in positions) this.AddOpened_step1of2(position);
		//}
		public void AddClosed(Position position, object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			this.AddOpened_step1of2(position, lockOwner, lockPurpose, waitMillis, duplicateThrowsAnError);
			this.AddToClosedDictionary_step2of2(position, lockOwner, lockPurpose, waitMillis, duplicateThrowsAnError);
		}
		//public bool Add_PositionOpened(Position positionWithFilledEntryAlert, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
		//    lockPurpose += " //" + base.ReasonToExist + ".AddPending(" + positionWithFilledEntryAlert.ToString() + ")";
		//    try {
		//        base.WaitAndLockFor(owner, lockPurpose, waitMillis);
		//        bool added = false;
		//        if (positionWithFilledEntryAlert == null) {
		//            string msg = "ADD_ONLY_FILLED_POSITION_NOT_NULL position[" + positionWithFilledEntryAlert + "]";
		//            Assembler.PopupException(msg);
		//            return added;
		//        }
		//        if (positionWithFilledEntryAlert.Shares == 0.0) {
		//            string msg = "POSITION_MUST_HAVE_POSITIVE_SIZE position[" + positionWithFilledEntryAlert + "]";
		//            Assembler.PopupException(msg);
		//            return added;
		//        }
		//        if (positionWithFilledEntryAlert.EntryAlert == null) {
		//            string msg = "POSITION_ATBAR_HAS_NO_ENTRY_ALERT position[" + positionWithFilledEntryAlert + "]";
		//            Assembler.PopupException(msg);
		//            return added;
		//        }
		//        added = base.AppendUnique(positionWithFilledEntryAlert, owner, lockPurpose, waitMillis, duplicateThrowsAnError);
		//        if (added == false) {
		//            string msg = "IS_THIS_WHY_I_GET_EMPTY_INNER_LIST_FOR_SLICE_BOTH?";
		//            Assembler.PopupException(msg);
		//            return added;
		//        }
		//        return added;
		//    } finally {
		//        base.UnLockFor(owner, lockPurpose);
		//    }
		//}
		public bool AddOpened_step1of2(Position positionOpened, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			lockPurpose += " //" + base.ReasonToExist + ".AddOpened_step1of2(" + positionOpened.ToString() + ")";
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				bool added = false;
				if (positionOpened == null) {
					string msg = "ADD_ONLY_POSITION_NOT_NULL position[" + positionOpened + "]";
					Assembler.PopupException(msg);
					return added;
				}
				if (positionOpened.Shares == 0.0) {
					string msg = "POSITION_MUST_HAVE_POSITIVE_SIZE position[" + positionOpened + "]";
					Assembler.PopupException(msg);
					return added;
				}
				if (positionOpened.EntryAlert == null) {
					string msg = "POSITION_ATBAR_HAS_NO_ENTRY_ALERT position[" + positionOpened + "]";
					Assembler.PopupException(msg);
					return added;
				}
				if (positionOpened.EntryDateBarTimeOpen == DateTime.MinValue) {
					string msg = "POSITION_ATBAR_HAS_NO_ENTRY_DATE"
						+ " while EntryAlert.FilledBar.DateTimeOpen[" + positionOpened.EntryAlert.FilledBar.DateTimeOpen + "]";
					Assembler.PopupException(msg);
					return added;
				}
				if (positionOpened.EntryFilledBarIndex == -1) {
					string msg = "POSITION_MUST_HAVE_ENTRY_FILLED_PRIOR_TO_ADDING_TO_POSITIONLIST " + positionOpened.ToString();
					Assembler.PopupException(msg);
					return added;
				}
				//v1
				//if (base.InnerList.Contains(position) && duplicateThrowsAnError) {
				//	string msg = this.ReasonToExist + " MUST_BE_ADDED_ONLY_ONCE__ALREADY_ADDED_BEFORE " + position.ToString();
				//	Assembler.PopupException(msg);
				//	return added;
				//}
				//v2
				added = base.AppendUnique(positionOpened, owner, lockPurpose, waitMillis, duplicateThrowsAnError);
				if (added == false) {
					string msg = "IS_THIS_WHY_I_GET_EMPTY_INNER_LIST_FOR_SLICE_BOTH?";
					Assembler.PopupException(msg);
					return added;
				}
			
				if (this.LastBarIndexEntry < positionOpened.EntryFilledBarIndex) this.LastBarIndexEntry = positionOpened.EntryFilledBarIndex;
			
				if (this.ByEntryBarFilled.ContainsKey(positionOpened.EntryFilledBarIndex) == false) {
					this.ByEntryBarFilled.Add(positionOpened.EntryFilledBarIndex, new List<Position>());
				}
				List<Position> byEntrySlot = this.ByEntryBarFilled[positionOpened.EntryFilledBarIndex];
				byEntrySlot.Add(positionOpened);
				return added;
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}
		public bool AddToClosedDictionary_step2of2(Position positionClosed, object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenseThrowsAnError = true) {
			lockPurpose += " //" + base.ReasonToExist + ".AddToClosedDictionary_step2of2(" + positionClosed.ToString() + ")";
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				bool added = false;
				if (positionClosed.ExitAlert == null) {
					string msg = "POSITION_ATBAR_HAS_NO_EXIT_ALERT";
					Assembler.PopupException(msg);
					return added;
				}
				if (positionClosed.ExitDateBarTimeOpen == DateTime.MinValue) {
					string msg = "POSITION_ATBAR_HAS_NO_EXIT_DATE"
						+ " while ExitAlert.FilledBar.DateTimeOpen[" + positionClosed.ExitAlert.FilledBar.DateTimeOpen + "]";
					Assembler.PopupException(msg);
					return added;
				}
				if (base.InnerList.Contains(positionClosed) == false && absenseThrowsAnError) {
					string msg = "POSITION_MUST_BE_ADDED_WHILE_JUST_OPENED_AND_SYNCED_TO_CLOSED_DICTIONARY_ON_CLOSE " + positionClosed.ToString();
					Assembler.PopupException(msg);
					return added;
				}
				if (positionClosed.ExitFilledBarIndex == -1) {
					string msg = "POSITION_MUST_HAVE_EXIT_FILLED_PRIOR_TO_ADDING_TO_CLOSED_DICTIONARY " + positionClosed.ToString();
					Assembler.PopupException(msg);
					return added;
				}
			
				if (this.ByExitBarFilled.ContainsKey(positionClosed.ExitFilledBarIndex) == false) {
					this.ByExitBarFilled.Add(positionClosed.ExitFilledBarIndex, new List<Position>());
				}
				List<Position> byExitSlot = this.ByExitBarFilled[positionClosed.ExitFilledBarIndex];
				byExitSlot.Add(positionClosed);
			
				added = true;
				if (this.LastBarIndexExit < positionClosed.ExitFilledBarIndex) this.LastBarIndexExit = positionClosed.ExitFilledBarIndex;
			
				return added;
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
		public bool Remove(Position position, object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenseThrowsAnError = true) {
			lockPurpose += " //" + base.ReasonToExist + ".Remove(" + position.ToString() + ")";
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				bool removed = base.RemoveUnique(position, lockOwner, lockPurpose, waitMillis, absenseThrowsAnError);
				if (this.ByEntryBarFilled.ContainsKey(position.EntryFilledBarIndex)) {
					List<Position> byEntrySlot = this.ByEntryBarFilled[position.EntryFilledBarIndex];
					if (byEntrySlot.Contains(position)) byEntrySlot.Remove(position);
					if (byEntrySlot.Count == 0) this.ByEntryBarFilled.Remove(position.EntryFilledBarIndex);
				}
				if (this.ByExitBarFilled.ContainsKey(position.ExitFilledBarIndex)) {
					List<Position> byExitSlot = this.ByExitBarFilled[position.ExitFilledBarIndex];
					if (byExitSlot.Contains(position)) byExitSlot.Remove(position);
					if (byExitSlot.Count == 0) this.ByEntryBarFilled.Remove(position.ExitFilledBarIndex);
				}
				return removed;
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
		public new PositionList Clone(object lockOwner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			lockPurpose += " //" + base.ReasonToExist + ".Clone()";
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose, waitMillis);
				PositionList ret		= new PositionList("CLONE_" + base.ReasonToExist, base.Snap, base.InnerList);		// WHO_DISPOSES_MANUAL_RESET_EVENT_OF_THE_CHILD__WHEN_RELOADING_WORKSPACE???
				ret.ByEntryBarFilled	= this.ByEntryBarFilled_safeCopy(this, "Clone(WAIT)");
				ret.ByExitBarFilled		= this.ByExitBarFilled_safeCopy(this, "Clone(WAIT)");
				ret.Count				= this.Count;
				return ret;
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}
		public override string ToString() {
			return base.ToString()
				+ " ByEntryFilled.Bars[" + ByEntryBarFilled.Keys.Count + "]"
				+ "  ByExitFilled.Bars[" +  ByExitBarFilled.Keys.Count+ "]";
		}
	}
}
