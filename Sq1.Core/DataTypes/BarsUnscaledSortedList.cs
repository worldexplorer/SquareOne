using System;
using System.Collections.Generic;

namespace Sq1.Core.DataTypes {
	// REASON_TO_EXIST: first implementation for Bars, appears to be slow and replaced now by BarsUnscaledList
	[Obsolete("#DEVELOP_PROFILER_SAYS_DATETIME>.COMPARE_IS_VERY_SLOW_DURING_BACKTEST")]
	public class BarsUnscaledSortedList_DEPRECATED {
		protected object BarsLock;
		
		protected SortedList<DateTime, Bar> BarsByDateTime;

		DateTime DateTimeFirst;
		DateTime DateTimeLast;
		
		//public ICollection<DateTime> DateTimesCollection { get { return base.Keys; } }
		public List<DateTime> DateTimes { get { return new List<DateTime>(this.BarsByDateTime.Keys); } }

		public BarsUnscaledSortedList_DEPRECATED() {
			BarsLock = new object();
			BarsByDateTime = new SortedList<DateTime, Bar>(new DateTimeAscending());
		}

		#region THATS_ALL_I_NEED_FROM_BARS_UNSCALED COULD_BE_AN_INTERFACE_BUT_IM_LAZY
		public void Add(Bar barAdding) {
			lock (this.BarsLock) {
				if (this.BarsByDateTime.Count == 0) this.DateTimeFirst = barAdding.DateTimeOpen; 
				this.BarsByDateTime.Add(barAdding.DateTimeOpen, barAdding);
				this.DateTimeLast = barAdding.DateTimeOpen;
			}
		}
		public Bar this[int indexRequested] { get {
				lock (this.BarsLock) {
					if (indexRequested < 0) return null;
					if (indexRequested >= this.BarsByDateTime.Count) return null;
					Bar bar = this.BarsByDateTime[this.BarsByDateTime.Keys[indexRequested]];
					
					string msg = "";
					if (bar.ParentBarsIndex != indexRequested) {
						msg = "bar.ParentBarsIndex[" + bar.ParentBarsIndex + "] != indexRequested[" + indexRequested + "]"
							+ "; Please modify bar.ParentBarsIndex only inside NOT_YET_IMPLEMENTED Bars.Add()/Bars.Remove()"
							+ "; you will be upset if I return here the bar with the bar's own index you didn't request";
						throw new Exception(msg);
					}
					if (double.IsNaN(bar.Open)) msg += "bar.Open[NaN] ";
					if (double.IsNaN(bar.Low)) msg += "bar.Low[NaN] ";
					if (double.IsNaN(bar.High)) msg += "bar.High[NaN] ";
					if (double.IsNaN(bar.Close)) msg += "bar.Close[NaN] ";
					if (double.IsNaN(bar.Volume)) msg += "bar.Volume[NaN] ";
					if (string.IsNullOrEmpty(msg) == false) throw new Exception("BARS_UNSCALED[]_MUST_ALWAYS_RETURN_BAR_WITHOUT_NANS: " + msg);
					return bar;
				}
			} }
		public Bar this[DateTime dateTimeRequested] { get {
				lock (this.BarsLock) {
					if (this.DateTimeFirst == DateTime.MinValue) return null;
					if (dateTimeRequested < this.DateTimeFirst) return null;
					if (dateTimeRequested >= this.DateTimeLast) return null;
					Bar bar = this.BarsByDateTime[dateTimeRequested];
					
					string msg = "";
					if (bar.DateTimeOpen != dateTimeRequested) {
						msg = "bar.DateTimeOpen[" + bar.DateTimeOpen + "] != dateTimeRequested[" + dateTimeRequested + "]";
						throw new Exception(msg);
					}
					if (double.IsNaN(bar.Open)) msg += "bar.Open[NaN] ";
					if (double.IsNaN(bar.Low)) msg += "bar.Low[NaN] ";
					if (double.IsNaN(bar.High)) msg += "bar.High[NaN] ";
					if (double.IsNaN(bar.Close)) msg += "bar.Close[NaN] ";
					if (double.IsNaN(bar.Volume)) msg += "bar.Volume[NaN] ";
					if (string.IsNullOrEmpty(msg) == false) throw new Exception("BARS_UNSCALED[]_MUST_ALWAYS_RETURN_BAR_WITHOUT_NANS: " + msg);
					return bar;
				}
			} }
		public int Count { get { 
				lock (this.BarsLock) {
					return this.BarsByDateTime.Count;
				}
			} }
		public IList<Bar> Values { get {
				lock (this.BarsLock) {
					return this.BarsByDateTime.Values;
				}
			} }
		public bool ContainsDate(DateTime startScanFrom) {
			lock (this.BarsLock) {
				return this.BarsByDateTime.ContainsKey(startScanFrom);
			}
		}
		public int IndexOfDate(DateTime startScanFrom) {
			lock (this.BarsLock) {
				return this.BarsByDateTime.IndexOfKey(startScanFrom);
			}
		}
		#endregion
	}
	public class DateTimeAscending : IComparer<DateTime> {
		int IComparer<DateTime>.Compare(DateTime x, DateTime y) {
			//Comparer<DateTime>.Default;
			//DESCENDING return DateTime.Compare(x, y) * -1;
			return DateTime.Compare(x, y);
			// ASCENDING_IMPLEMENTATION_DEMO
//			long internalTicks = x.Ticks;
//			long internalTicks2 = y.Ticks;
//			if (internalTicks > internalTicks2) return 1;
//			if (internalTicks < internalTicks2) return -1;
//			return 0;
		}
	}
}