using System;
using System.Collections.Generic;

using Sq1.Core.Execution;

namespace Sq1.Core.Support {
	public class ConcurrentListFiltered<T> : ConcurrentWatchdog {
		public		string			Separator							{ get; private set; }
		public		bool			CaseSensitive						{ get; private set; }
		public		List<string>	KeywordsToExclude					{ get; private set; }
		public		string			KeywordsToExclude_asCsv				{ get {
			string ret = "";
			foreach (string kw in this.KeywordsToExclude) {
				if (ret != "") ret += this.Separator;
				ret += kw;
			}
			return ret;
		} }

		protected	List<T>			InnerList							{ get; private set; }
		protected	List<T>			InnerList_excludeKeywordsApplied	{ get; private set; }
		protected	List<T>			InnerList_swingingPointer			{ get; private set; }
		public		int				Count								{ get; private set; }

		public		bool			ExcludeKeywordsApplied				{ get {
			return this.InnerList_swingingPointer == this.InnerList_excludeKeywordsApplied;
		} }

		public		bool			SearchApplied						{ get {
			return this.InnerList_swingingPointer == this.InnerList_excludeKeywordsApplied;
		} }


		// used in SafeCopy() only
		ConcurrentListFiltered(string reasonToExist, ExecutorDataSnapshot snap, List<T> copyFrom, string separator = ",", bool caseSensitive = false)
								: this(reasonToExist, snap, separator, caseSensitive) {
		    InnerList.AddRange(copyFrom);
		    Count = InnerList.Count;
		}
			
		public ConcurrentListFiltered(string reasonToExist, ExecutorDataSnapshot snap = null, string separator = ",", bool caseSensitive = false) : base(reasonToExist, snap) {
			InnerList							= new List<T>();
			InnerList_excludeKeywordsApplied	= new List<T>();
			InnerList_swingingPointer			= InnerList;
			KeywordsToExclude					= new List<string>();
			Separator							= separator;
			CaseSensitive						= caseSensitive;
			Snap								= snap;
		}

		public T First_nullUnsafe(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			T ret = default(T);
			lockPurpose += " //" + this.ToString() + ".First_nullUnsafe()";
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				if (ret != null) {
					string msg = "PARANOID I_WANT_NULL_HERE!!! NOT_TRUSTING_default(T)_AND_GENERIC_TYPE_CAN_NOT_BE_ASSIGNED_TO_NULL";
					Assembler.PopupException(msg);
				}
				if (this.InnerList_swingingPointer.Count > 0) ret = this.InnerList_swingingPointer[0];
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return ret;
		}
		public T Last_nullUnsafe(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			T ret = default(T);
			lockPurpose += " //" + this.ToString() + ".Last_nullUnsafe()";
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				if (ret != null) {
					string msg = "PARANOID I_WANT_NULL_HERE!!! NOT_TRUSTING_default(T)_AND_GENERIC_TYPE_CAN_NOT_BE_ASSIGNED_TO_NULL";
					Assembler.PopupException(msg);
				}
				if (this.InnerList_swingingPointer.Count > 0) ret = this.InnerList_swingingPointer[this.InnerList_swingingPointer.Count - 1];
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return ret;
		}
		public T PreLast_nullUnsafe(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			T ret = default(T);
			lockPurpose += " //" + this.ToString() + ".PreLast_nullUnsafe()";
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				if (ret != null) {
					string msg = "PARANOID I_WANT_NULL_HERE!!! NOT_TRUSTING_default(T)_AND_GENERIC_TYPE_CAN_NOT_BE_ASSIGNED_TO_NULL";
					Assembler.PopupException(msg);
				}
				if (this.InnerList_swingingPointer.Count > 1) ret = this.InnerList_swingingPointer[this.InnerList_swingingPointer.Count - 2];
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return ret;
		}
		public List<T> SafeCopy(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			List<T> ret = new List<T>();
			lockPurpose += " //" + this.ToString() + ".SafeCopy()";
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				ret = new List<T>(this.InnerList_swingingPointer);
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
			return ret;
		}
		public bool Contains(T position, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			//lockPurpose += " //" + this.ToString() + ".Contains(" + position.ToString() + ")";
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				return this.InnerList_swingingPointer.Contains(position);
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}
		public int Clear(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			//lockPurpose += " //" + this.ToString() + ".Clear()";
			try {
				int countBeforeCleared = this.Count;
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				this.InnerList.Clear();
				this.InnerList_excludeKeywordsApplied.Clear();
				this.Count = this.InnerList_swingingPointer.Count;
				return countBeforeCleared;
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}

		// "protected" forces derived classes to use the wrapper (for narrower debugging)
		protected virtual bool RemoveUnique(T position, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenceThrowsAnError = true) {
			//lockPurpose += " //" + this.ToString() + ".Remove(" + position.ToString() + ")";
			bool removed = false;
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				if (this.InnerList_excludeKeywordsApplied.Contains(position)) {
					bool removed1 = this.InnerList_excludeKeywordsApplied.Remove(position);
					if (this.InnerList_swingingPointer == this.InnerList_excludeKeywordsApplied) removed = removed1;
				}
				if (this.InnerList.Contains(position) == false) {
					if (absenceThrowsAnError == true) {
						string msg = "WAS_REMOVED_EARLIER__OR_NEVER_ADDED position[" + position + "] LIVESIM_SHOULD_NOT_FILL_ORDER_THAT_WAS_ALREADY_KILLED";
						Assembler.PopupException(msg + this.ToString());
					}
				} else {
					bool removed2 = this.InnerList.Remove(position);
					if (this.InnerList_swingingPointer == this.InnerList) removed = removed2;
				}
			} finally {
				this.Count = this.InnerList_swingingPointer.Count;
				base.UnLockFor(owner, lockPurpose);
			}
			return removed;
		}

		// "protected" forces derived classes to use the wrapper (for narrower debugging)
		protected virtual bool AppendUnique(T alertOrPosition, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			//lockPurpose += " //" + this.ToString() + ".Add(" + alertOrPosition.ToString() + ")";
			bool appended = false;
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				if (this.matchesAtLeastOne_Keyword(alertOrPosition, this.KeywordsToExclude) == false) {
					if (this.InnerList_excludeKeywordsApplied.Contains(alertOrPosition) == false) {
						this.InnerList_excludeKeywordsApplied.Add(alertOrPosition);
						if (this.InnerList_swingingPointer == this.InnerList_excludeKeywordsApplied) appended = true;
					}
				}

				if (this.InnerList.Contains(alertOrPosition) && duplicateThrowsAnError) {
					string msg = base.ReasonToExist + ": CLWD_MUST_BE_ADDED_ONLY_ONCE__ALREADY_ADDED_BEFORE " + alertOrPosition.ToString();
					Assembler.PopupException(msg, null, true);
				} else {
					this.InnerList.Add(alertOrPosition);
					if (this.InnerList_swingingPointer == this.InnerList) appended = true;
				}
			} finally {
				this.Count = this.InnerList_swingingPointer.Count;
				base.UnLockFor(owner, lockPurpose);
			}
			return appended;
		}

		// "protected" forces derived classes to use the wrapper (for narrower debugging)
		protected virtual bool InsertUnique(T alertOrPosition, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			//lockPurpose += " //" + this.ToString() + ".Add(" + alertOrPosition.ToString() + ")";
			int indexToInsertAt = 0;
			bool inserted = false;
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				if (this.matchesAtLeastOne_Keyword(alertOrPosition, this.KeywordsToExclude) == false) {
					if (this.InnerList_excludeKeywordsApplied.Contains(alertOrPosition) == false) {
						this.InnerList_excludeKeywordsApplied.Insert(indexToInsertAt, alertOrPosition);
						if (this.InnerList_swingingPointer == this.InnerList_excludeKeywordsApplied) inserted = true;
					}
				}
				if (this.InnerList.Contains(alertOrPosition) && duplicateThrowsAnError) {
					string msg = base.ReasonToExist + ": CLWD_MUST_BE_INSERTED_ONLY_ONCE__ALREADY_INSERTED_BEFORE " + alertOrPosition.ToString();
					Assembler.PopupException(msg, null, true);
				} else {
					this.InnerList.Insert(indexToInsertAt, alertOrPosition);
					if (this.InnerList_swingingPointer == this.InnerList) inserted = true;
				}
			} finally {
				this.Count = this.InnerList_swingingPointer.Count;
				base.UnLockFor(owner, lockPurpose);
			}
			return inserted;
		}

		
		protected int AddRange(List<T> ordersInit, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			int ret = 0;
			foreach (T eachOrder in ordersInit) {
				bool inserted = this.InsertUnique(eachOrder, owner, lockPurpose, waitMillis, duplicateThrowsAnError);
				if (inserted) ret ++;
			}
			return ret;
		}

		protected int RemoveRange(List<T> ordersToRemove, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenceThrowsAnError = true) {
			string	msig = " //ConcurrentListFiltered.RemoveRange(" + ordersToRemove.Count + ")";
			int removed_counter = 0;

			foreach (T eachOrder in ordersToRemove) {
				bool removed = this.RemoveUnique(eachOrder, owner, lockPurpose, waitMillis, absenceThrowsAnError);
				removed_counter++;
			}
			return removed_counter;
		}



		public ConcurrentListFiltered<T> Clone(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			//lockPurpose += " //" + this.ToString() + ".Clone()";
			ConcurrentListFiltered<T> ret = null;
			try {
				base.WaitAndLockFor(owner, lockPurpose, waitMillis);
				ret = new ConcurrentListFiltered<T>("CLONE_" + this.ReasonToExist, this.Snap, this.InnerList_swingingPointer);
				return ret;
			} finally {
				base.UnLockFor(owner, lockPurpose);
			}
		}

		bool matchesAtLeastOne_Keyword(T exception, List<string> keywords) {
			bool ret = false;
			if (exception == null) return ret;

			//v1 string eachException_asStringForMatch = exception.ToString();
			string eachException_asStringForMatch = this.ToString_forMatch(exception);
			string exception_asString = this.CaseSensitive
				? eachException_asStringForMatch
				: eachException_asStringForMatch.ToUpper();

			foreach (string keyword in keywords) {
				string keyword_withCase = this.CaseSensitive ? keyword : keyword.ToUpper();
				if (exception_asString.Contains(keyword_withCase) == false) continue;
				ret = true;
				break;
			}
			return ret;
		}

		public virtual string ToString_forMatch(T exception) {
			return exception.ToString();
		}

		public List<T> SearchForKeywords_StaticSnapshotSubset(string keywordsToSearch_csv) {
		    string msig = "FOUND[" + keywordsToSearch_csv + "] ";
			List<string> keywordsToSearch = new List<string>();
			if (string.IsNullOrEmpty(keywordsToSearch_csv) == false) {
				string[] keywords = keywordsToSearch_csv.Split(this.Separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				keywordsToSearch.AddRange(keywords);
			}
		    List<T> fullClone_orAlreadyFiltered = this.SafeCopy(this, msig);

			List<T> ret = new List<T>();
			foreach (T exception in fullClone_orAlreadyFiltered) {
				bool matchesProvidedKeyword = this.matchesAtLeastOne_Keyword(exception, keywordsToSearch);
				if (matchesProvidedKeyword == false) continue;
				ret.Add(exception);
			}
			return ret;
		}
		public List<T> InitKeywordsToExclude_AndSetPointer(string keywordsCsv = "") {
			this.KeywordsToExclude = new List<string>();
			if (string.IsNullOrEmpty(keywordsCsv) == false) {
				string[] keywords = keywordsCsv.Split(this.Separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				this.KeywordsToExclude.AddRange(keywords);
			}

			if (this.KeywordsToExclude.Count == 0) {
				this.InnerList_excludeKeywordsApplied.Clear();
				this.InnerList_swingingPointer = this.InnerList;
			} else {
				this.InnerList_excludeKeywordsApplied = this.excludeEntitiesContainingKeywords_from(this.InnerList, "//KeywordsToExclude_InitAndSetPointer()");
				this.InnerList_swingingPointer = this.InnerList_excludeKeywordsApplied;
			}
			return this.InnerList_swingingPointer;
		}
		
		public void AppendKeywordToIgnore(string keyword) {
			this.KeywordsToExclude.Add(keyword);
			this.InnerList_excludeKeywordsApplied = this.excludeEntitiesContainingKeywords_from(this.InnerList, "//AppendKeywordToIgnore()");
		}

		List<T> excludeEntitiesContainingKeywords_from(List<T> fullClone_orAlreadyFiltered, string invoker) {
			List<T> ret = new List<T>();
			foreach (T exception in fullClone_orAlreadyFiltered) {
				bool matchesDeniedKeyword = this.matchesAtLeastOne_Keyword(exception, this.KeywordsToExclude);
				if (matchesDeniedKeyword) continue;
				ret.Add(exception);
			}
			return ret;
		}

	}
}
