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

		public int LastEntryFilledBarIndex;
		public int LastExitFilledBarIndex;
		
		public PositionList(string reasonToExist) : base(reasonToExist) {
			ByEntryBarFilled	= new Dictionary<int, List<Position>>();
			ByExitBarFilled		= new Dictionary<int, List<Position>>();
			LastEntryFilledBarIndex	= -1;
			LastExitFilledBarIndex	= -1;
		}
//		public PositionList(string reasonToExist, List<Position> positions) : this(reasonToExist) {
//			this.AddRange(positions);
//		}
		public void AddRange(List<Position> positions) {
			foreach (Position position in positions) this.AddOpening_step1of2(position);
		}
		
		public override void Clear() { lock(base.LockObject) {
			base					.Clear();
			this.ByEntryBarFilled	.Clear();
			this.ByExitBarFilled	.Clear();
			this.LastEntryFilledBarIndex	= -1;
			this.LastExitFilledBarIndex		= -1;
		}
		}
		public void AddClosed(Position position) { lock(base.LockObject) {
			this.AddOpening_step1of2(position);
			this.AddToClosedDictionary_step2of2(position);
		} }
		public bool AddOpening_step1of2(Position position, bool duplicateIsAnError = true) { lock(base.LockObject) {
			bool added = false;
			if (position == null) {
				string msg = "ADD_ONLY_FILLED_POSITION_NOT_NULL position[" + position + "]";
				Assembler.PopupException(msg);
				return added;
			}
			if (position.Shares == 0.0) {
				string msg = "POSITION_MUST_HAVE_POSITIVE_SIZE position[" + position + "]";
				Assembler.PopupException(msg);
				return added;
			}
			if (base.InnerList.Contains(position) && duplicateIsAnError) {
				string msg = "POSITION_MUST_BE_ADDED_ONLY_ONCE__ALREADY_ADDED_BEFORE " + position.ToString();
				Assembler.PopupException(msg);
				return added;
			}
			if (position.EntryFilledBarIndex == -1) {
				string msg = "POSITION_MUST_HAVE_ENTRY_FILLED_PRIOR_TO_ADDING_TO_POSITIONLIST " + position.ToString();
				Assembler.PopupException(msg);
				return added;
			}
			
			if (this.ByEntryBarFilled.ContainsKey(position.EntryFilledBarIndex) == false) {
				this.ByEntryBarFilled.Add(position.EntryFilledBarIndex, new List<Position>());
			}
			List<Position> byEntrySlot = this.ByEntryBarFilled[position.EntryFilledBarIndex];
			byEntrySlot.Add(position);
			
			added = base.Add(position);
			if (this.LastEntryFilledBarIndex < position.EntryFilledBarIndex) this.LastEntryFilledBarIndex = position.EntryFilledBarIndex;

			//if (position.ExitFilledBarIndex == -1) return;
			//this.AddToClosedDictionary_step2of2(position);
			return added;
		} }
		public bool AddToClosedDictionary_step2of2(Position position, bool absenseIsAnError = true) { lock(base.LockObject) {
			bool added = false;
			if (base.InnerList.Contains(position) == false && absenseIsAnError) {
				string msg = "POSITION_MUST_BE_ADDED_WHILE_JUST_OPENED_AND_SYNCED_TO_CLOSED_DICTIONARY_ON_CLOSE " + position.ToString();
				Assembler.PopupException(msg);
				return added;
			}
			if (position.ExitFilledBarIndex == -1) {
				string msg = "POSITION_MUST_HAVE_EXIT_FILLED_PRIOR_TO_ADDING_TO_CLOSED_DICTIONARY " + position.ToString();
				Assembler.PopupException(msg);
				return added;
			}
			
			if (this.ByExitBarFilled.ContainsKey(position.ExitFilledBarIndex) == false) {
				this.ByExitBarFilled.Add(position.ExitFilledBarIndex, new List<Position>());
			}
			List<Position> byExitSlot = this.ByExitBarFilled[position.ExitFilledBarIndex];
			byExitSlot.Add(position);
			added = true;
			this.LastExitFilledBarIndex = position.ExitFilledBarIndex;
			return added;
		} }
		public override bool Remove(Position position, bool absenseIsAnError = true) { lock(base.LockObject) {
			bool removed = base.Remove(position, absenseIsAnError);
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
		public new PositionList Clone() {
			PositionList ret		= new PositionList(this.ReasonToExist + "_CLONE");
			ret.InnerList			= this.SafeCopy;
			ret.ByEntryBarFilled	= this.ByEntryBarFilledSafeCopy;
			ret.ByExitBarFilled		= this.ByExitBarFilledSafeCopy;
			return ret;
		}
		public override string ToString() { lock(base.LockObject) {
			return base.ToString() + string.Format(" ByEntryFilled.Bars[{2}] ByExitFilled.Bars[{3}]",
				ReasonToExist, InnerList.Count, ByEntryBarFilled.Keys.Count, ByExitBarFilled.Keys.Count);
		} }
	}
}