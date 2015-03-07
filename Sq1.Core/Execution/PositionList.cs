using System;
using System.Collections.Generic;

using Sq1.Core.Support;

namespace Sq1.Core.Execution {
	public class PositionList : ConcurrentList<Position> {
		public Dictionary<int, List<Position>>	ByEntryBarFilled	{ get; protected set; }
		public Dictionary<int, List<Position>>	ByExitBarFilled		{ get; protected set; }
		
		public Dictionary<int, List<Position>>	ByExitBarFilledSafeCopy { get { lock (base.LockObject) {
			Dictionary<int, List<Position>> ret = new Dictionary<int, List<Position>>();
			foreach (int bar in this.ByExitBarFilled.Keys) ret.Add(bar, new List<Position>(this.ByExitBarFilled[bar]));
			return ret;
		} } }
		public Dictionary<int, List<Position>>	ByEntryBarFilledSafeCopy { get { lock (base.LockObject) {
			Dictionary<int, List<Position>> ret = new Dictionary<int, List<Position>>();
			foreach (int bar in this.ByEntryBarFilled.Keys) ret.Add(bar, new List<Position>(this.ByEntryBarFilled[bar]));
			return ret;
		} } }
		public AlertList AlertsEntry { get {
			AlertList ret = new AlertList("AlertsEntry");
			foreach (List<Position> positionsOpened in this.ByEntryBarFilled.Values) {
				foreach (Position position in positionsOpened) {
					if (position.EntryAlert == null) continue;
					ret.AddNoDupe(position.EntryAlert);
				}
			}
			return ret;
		} }
		public AlertList AlertsExit { get {
			AlertList ret = new AlertList("AlertsExit");
			foreach (List<Position> positionsClosed in this.ByExitBarFilled.Values) {
				foreach (Position position in positionsClosed) {
					if (position.ExitAlert == null) continue;
					ret.AddNoDupe(position.ExitAlert);
				}
			}
			return ret;
		} }
		public AlertList AlertsOpenNow { get {
			AlertList ret = new AlertList("AlertsOpenNow");
			foreach (Position position in this.InnerList) {
				if (position.ExitAlert == null) continue;
				ret.AddNoDupe(position.ExitAlert);
			}
			return ret;
		} }

		public int LastBarIndexEntry;
		public int LastBarIndexExit;
		
		public PositionList(string reasonToExist) : base(reasonToExist) {
			ByEntryBarFilled	= new Dictionary<int, List<Position>>();
			ByExitBarFilled		= new Dictionary<int, List<Position>>();
			LastBarIndexEntry	= -1;
			LastBarIndexExit	= -1;
		}
		public void Clear() { lock(base.LockObject) {
			base					.ClearInnerList();
			this.ByEntryBarFilled	.Clear();
			this.ByExitBarFilled	.Clear();
			this.LastBarIndexEntry	= -1;
			this.LastBarIndexExit	= -1;
		} }
		// NEVER_USED__UNCOMMENT_WHEN_YOU_NEED_IT public void AddRangeOpened(List<Position> positions) {
		//	foreach (Position position in positions) this.AddOpened_step1of2(position);
		//}
		public void AddClosed(Position position) { lock(base.LockObject) {
			this.AddOpened_step1of2(position);
			this.AddToClosedDictionary_step2of2(position);
		} }
		public bool AddOpened_step1of2(Position positionOpened, bool duplicateThrowsAnError = true) { lock(base.LockObject) {
			bool added = false;
			if (positionOpened == null) {
				string msg = "ADD_ONLY_FILLED_POSITION_NOT_NULL position[" + positionOpened + "]";
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
			if (positionOpened.EntryDate == DateTime.MinValue) {
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
			added = base.AddToInnerList(positionOpened, duplicateThrowsAnError);
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
		} }
		public bool AddToClosedDictionary_step2of2(Position positionClosed, bool absenseThrowsAnError = true) { lock(base.LockObject) {
			bool added = false;
			if (positionClosed.ExitAlert == null) {
				string msg = "POSITION_ATBAR_HAS_NO_EXIT_ALERT";
				Assembler.PopupException(msg);
				return added;
			}
			if (positionClosed.ExitDate == DateTime.MinValue) {
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
		} }
		public bool Remove(Position position, bool absenseThrowsAnError = true) { lock(base.LockObject) {
			bool removed = base.RemoveFromInnerList(position, absenseThrowsAnError);
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
		} }
		public PositionList Clone() { lock(base.LockObject) {
			PositionList ret		= new PositionList(this.ReasonToExist + "_CLONE");
			ret.InnerList			= this.InnerListSafeCopy;
			ret.ByEntryBarFilled	= this.ByEntryBarFilledSafeCopy;
			ret.ByExitBarFilled		= this.ByExitBarFilledSafeCopy;
			return ret;
		} }
		public override string ToString() { lock(base.LockObject) {
			string ret = base.ToString()
				+ " ByEntryFilled.Bars[" + ByEntryBarFilled.Keys.Count + "]"
				+ "  ByExitFilled.Bars[" +  ByExitBarFilled.Keys.Count+ "]";
			return ret;
		} }
	}
}