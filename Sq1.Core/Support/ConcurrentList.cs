using System;
using System.Collections.Generic;

using Sq1.Core.Execution;

namespace Sq1.Core.Support {
	public class ConcurrentList<T> {
		protected	object					LockObject;
		protected	ExecutionDataSnapshot	Snap;
		public		string					ReasonToExist		{ get; protected set; }
		public		List<T>					InnerList			{ get; protected set; }
		
		public int Count { get { lock(this.LockObject) {
			return this.InnerList.Count;
		} } }
		public T LastNullUnsafe { get { lock(this.LockObject) {
			T ret = default(T);
			if (ret != null) {
				string msg = "PARANOID I_WANT_NULL_HERE!!! NOT_TRUSTING_default(T)_AND_GENERIC_TYPE_CAN_NOT_BE_ASSIGNED_TO_NULL";
				Assembler.PopupException(msg);
			}
			if (this.InnerList.Count > 0) ret = this.InnerList[this.InnerList.Count - 1];
			return ret;
		} } }
		public T PreLastNullUnsafe { get { lock(this.LockObject) {
			T ret = default(T);
			if (ret != null) {
				string msg = "PARANOID I_WANT_NULL_HERE!!! NOT_TRUSTING_default(T)_AND_GENERIC_TYPE_CAN_NOT_BE_ASSIGNED_TO_NULL";
				Assembler.PopupException(msg);
			}
			if (this.InnerList.Count > 1) ret = this.InnerList[this.InnerList.Count - 2];
			return ret;
		} } }
		public List<T> InnerListSafeCopy { get {
			//this.Snap.PopupIfRunning(" //" + this.ToString() + ".InnerListSafeCopy()");
			lock (this.LockObject) {
				return new List<T>(this.InnerList);
			}
		} }
		public ConcurrentList(string reasonToExist, ExecutionDataSnapshot snap) {
			this.Snap			= snap;
			ReasonToExist		= reasonToExist;
			LockObject			= new object();
			InnerList			= new List<T>();
		}
		public bool ContainsInInnerList(T position) {
			if (this.Snap != null) this.Snap.PopupIfAnyScriptOverrideIsRunning(" //" + this.ToString() + ".ContainsInInnerList(" + position.ToString() + ")");
			lock (this.LockObject) {
				return this.InnerList.Contains(position);
			}
		}
		protected virtual void ClearInnerList() {
			if (this.Snap != null) this.Snap.PopupIfAnyScriptOverrideIsRunning(" //" + this.ToString() + ".ClearInnerList()");
			lock (this.LockObject) {
				this.InnerList.Clear();
			}
		}
		protected virtual bool RemoveFromInnerList(T position, bool absenseThrowsAnError = true) {
			if (this.Snap != null) this.Snap.PopupIfAnyScriptOverrideIsRunning(" //" + this.ToString() + "RemoveFromInnerList(" + position.ToString() + ")");
			lock(this.LockObject) {
				bool removed = false;
				if (this.InnerList.Contains(position) == false) {
					if (absenseThrowsAnError == true) {
						string msg = "CANT_REMOVE_REMOVED_EARLIER_OR_WASNT_ADDED " + position.ToString();
						Assembler.PopupException(msg);
						return removed;
					}
				} else {
					removed = this.InnerList.Remove(position);
				}
				return removed;
			}
		}
		protected virtual bool AddToInnerList(T alertOrPosition, bool duplicateThrowsAnError = true) {
			if (this.Snap != null) this.Snap.PopupIfAnyScriptOverrideIsRunning(" //" + this.ToString() + "AddToInnerList(" + alertOrPosition.ToString() + ")");
			lock (this.LockObject) {
				bool added = false;
				if (this.InnerList.Contains(alertOrPosition) && duplicateThrowsAnError) {
					string msg = this.ReasonToExist + ": MUST_BE_ADDED_ONLY_ONCE__ALREADY_ADDED_BEFORE " + alertOrPosition.ToString();
					Assembler.PopupException(msg, null, false);
					return added;
				}
				this.InnerList.Add(alertOrPosition);
				added = true;
				return added;
			}
		}
		// 1) won't use without subclassing; 2) in the subclass, {(AlertList) ConcurrentList<T>} doesn't work properly => disabled completely 
		//public virtual ConcurrentList<T> Clone() { lock (this.LockObject) {
		//	ConcurrentList<T> ret	= new ConcurrentList<T>(this.ReasonToExist + "_CLONE");
		//	ret.InnerList			= this.SafeCopy;
		//	return ret;
		//} }
		public override string ToString() { lock(this.LockObject) {
			return string.Format("{0}:InnerList[{1}]", ReasonToExist, InnerList.Count);
		} }
	}
}
