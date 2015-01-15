using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sq1.Core.Support {
	public class ConcurrentList<T> {
		protected	object	LockObject;
		public		string	ReasonToExist		{ get; protected set; }
		public		List<T>	InnerList			{ get; protected set; }
		
		public int Count { get { lock(this.LockObject) {
			return this.InnerList.Count;
		} } }
		public T LastNullUnsafe { get { lock(this.LockObject) {
			T ret = default(T);
			if (ret != null) {
				Debugger.Break();
			}
			if (this.InnerList.Count > 0) ret = this.InnerList[this.InnerList.Count - 1];
			return ret;
		} } }
		public T PreLastNullUnsafe { get { lock(this.LockObject) {
			T ret = default(T);
			if (ret != null) {
				Debugger.Break();
			}
			if (this.InnerList.Count > 1) ret = this.InnerList[this.InnerList.Count - 2];
			return ret;
		} } }
		public virtual List<T>	SafeCopy { get { lock (this.LockObject) {
			return new List<T>(this.InnerList);
		} } }
		public ConcurrentList(string reasonToExist) {
			ReasonToExist		= reasonToExist;
			LockObject			= new object();
			InnerList			= new List<T>();
		}
		public bool Contains(T position) { lock(this.LockObject) {
			return this.InnerList.Contains(position);
		} }
		public virtual void Clear() { lock(this.LockObject) {
			this.InnerList.Clear();
		} }
		public virtual bool Remove(T position, bool absenseIsAnError = true) { lock(this.LockObject) {
			bool removed = false;
			if (this.InnerList.Contains(position) == false) {
				if (absenseIsAnError == true) {
					string msg = "CANT_REMOVE_REMOVED_EARLIER_OR_WASNT_ADDED " + position.ToString();
					Assembler.PopupException(msg);
					return removed;
				}
			} else {
				removed = this.InnerList.Remove(position);
			}
			return removed;
		} }
		public virtual bool Add(T alertOrPosition) { lock(this.LockObject) {
			bool added = false;
			if (this.InnerList.Contains(alertOrPosition)) {
				string msg = "ALREADY_ADDED " + alertOrPosition.ToString();
				Assembler.PopupException(msg);
				return added;
			}
			this.InnerList.Add(alertOrPosition);
			added = true;
			return added;
		} }
		public virtual ConcurrentList<T> Clone() {
			ConcurrentList<T> ret	= new ConcurrentList<T>(this.ReasonToExist + "_CLONE");
			ret.InnerList			= this.SafeCopy;
			return ret;
		}
		public override string ToString() { lock(this.LockObject) {
			return string.Format("{0} InnerList[{1}]", ReasonToExist, InnerList.Count);
		} }
	}
}
