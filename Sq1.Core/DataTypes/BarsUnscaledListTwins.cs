using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sq1.Core.DataTypes {
	// REASON_TO_EXIST: get a faster this[] comparing to BarsUnscaledSortedList
	public class BarsUnscaledListTwins {
		protected object BarsLock;

		// two parallel structures with same Bars inside:
		protected	List<Bar>					BarsList;	// List<Bar> for fast this[int]
		//public		IList<Bar>					Values		{ get { lock (this.BarsLock) { return this.BarsList; } } }
		public		int							Count			{ get { lock (this.BarsLock) { return this.BarsList.Count; } } }
		public		bool						ImSortedReverse	{ get; private set; }
		
		//protected List<DateTime> DateTimeList;				// List<DateTime> for DataSeriesProxyBars.DateTimes
		protected	SortedList<DateTime, Bar>	BarsByDateTime;	// SortedList for slow this[DateTime] and checks
		
		DateTime DateTimeFirst;
		DateTime DateTimeLast;
		
		//public ICollection<DateTime> DateTimesCollection { get { return this.DateTimesSortedList.Keys; } }
		// DateTimes_ARE_VERY_SLOW_USED_ONLY_FOR_DataSeriesProxyBars
		//v1 public List<DateTime> DateTimes { get { return new List<DateTime>(this.BarsByDateTime.Keys); } }
		//public List<DateTime> DateTimes { get { return this.DateTimeList; } }

		public BarsUnscaledListTwins() : base() {
			BarsLock = new object();
			DateTimeFirst = DateTime.MinValue;
			BarsList = new List<Bar>();
			BarsByDateTime = new SortedList<DateTime, Bar>();
		}
		
		#region THATS_ALL_I_NEED_FROM_BARS_UNSCALED COULD_BE_AN_INTERFACE_BUT_IM_LAZY
		public void Add(Bar barAdding) { lock (this.BarsLock) {
				if (this.ImSortedReverse) {
					string msg = "NYI__ADDING_BARS_IN_BarsEditorControl_HAS_UNKNOWN_IMPLICATIONS";
					Assembler.PopupException(msg);
					return;
				}
				if (this.BarsList.Count == 0) this.DateTimeFirst = barAdding.DateTimeOpen; 
				this.BarsList.Add(barAdding);
				//this.DateTimeList.Add(barAdding.DateTimeOpen);
				this.BarsByDateTime.Add(barAdding.DateTimeOpen, barAdding);
				this.DateTimeLast = barAdding.DateTimeOpen;
			} }
		public Bar this[int indexRequested] { get { lock (this.BarsLock) {
				if (indexRequested < 0) return null;
				if (indexRequested >= this.BarsList.Count) return null;
				//Bar bar = this.BarsList[base.Keys[indexRequested]];
				Bar bar = this.BarsList[indexRequested];
				
				string msg = "";
				if (this.ImSortedReverse == false && bar.ParentBarsIndex != indexRequested) {
					msg = "bar.ParentBarsIndex[" + bar.ParentBarsIndex + "] != indexRequested[" + indexRequested + "]"
						+ "; Please modify bar.ParentBarsIndex only inside NOT_YET_IMPLEMENTED Bars.Add()/Bars.Remove()"
						+ "; you will be upset if I return here the bar with the bar's own index you didn't request";
					throw new Exception(msg);
				}

				#if DEBUG
				//v1
				//if (double.IsNaN(bar.Open)) msg += "bar.Open[NaN] ";
				//if (double.IsNaN(bar.Low)) msg += "bar.Low[NaN] ";
				//if (double.IsNaN(bar.High)) msg += "bar.High[NaN] ";
				//if (double.IsNaN(bar.Close)) msg += "bar.Close[NaN] ";
				//if (double.IsNaN(bar.Volume)) msg += "bar.Volume[NaN] ";
				//v2 APP_STARTUP_TOO_SLOW__EXCESSIVE_FOR_READING__WRITING_ALREADY_VALID
				//msg = bar.CheckThrow_DateOHLCV_validForSaving(false);

				//if (string.IsNullOrEmpty(msg) == false) {
				//    //Debugger.Break();
				//    throw new Exception("BARS_UNSCALED[]_MUST_ALWAYS_RETURN_BAR_WITHOUT_NANS: " + msg);
				//}
				#endif

				return bar;
			} } }
		public Bar this[DateTime dateTimeRequested] { get { lock (this.BarsLock) {
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
				if (string.IsNullOrEmpty(msg) == false) {
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception("BARS_UNSCALED[]_MUST_ALWAYS_RETURN_BAR_WITHOUT_NANS: " + msg);
				}
				return bar;
			} } }
		public bool ContainsDate(DateTime startScanFrom) { lock (this.BarsLock) {
				return this.BarsByDateTime.ContainsKey(startScanFrom);
			} }
		public int IndexOfDate(DateTime startScanFrom) { lock (this.BarsLock) {
				return this.BarsByDateTime.IndexOfKey(startScanFrom);
			} }
		#endregion


		public void Reverse() {
			this.ImSortedReverse = !this.ImSortedReverse;
			this.BarsList.Reverse();
		}
	}
}