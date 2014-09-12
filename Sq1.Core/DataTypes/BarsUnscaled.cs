using System;
using System.Collections.Generic;

namespace Sq1.Core.DataTypes {
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
	public class BarsUnscaled : SortedList<DateTime, Bar> {		//SortedDictionary<DateTime, Bar> 
		public event EventHandler<BarsUnscaledEventArgs> BarsRenamed_SEEMS_EXCESSIVE;
		
		public string Symbol { get; protected set; }
		public string ReasonToExist;
		public ICollection<DateTime> DateTimesCollection { get { return base.Keys; } }		// that's exactly why I inherited from SortedList<DateTime, Bar> 
		public List<DateTime> DateTimes { get { return new List<DateTime>(base.Keys); } }
		private SymbolInfo symbolInfo;
		public SymbolInfo SymbolInfo {
			get {
				if (this.symbolInfo == null) {
					this.symbolInfo = Assembler.InstanceInitialized.RepositoryCustomSymbolInfo.FindSymbolInfoOrNew(this.Symbol);
				}
				return this.symbolInfo;
			}
			set { this.symbolInfo = value; }	//it's initialized in ctor(), SymbolInfo=null will initiate repository scan on next get{}
		}
		public Bar BarFirst { get { return (base.Count >= 1) ? this[0] : null; } }
		public Bar BarLast { get { return (base.Count >= 1) ? this[this.Count - 1] : null; } }
		public Bar BarPreLast { get { return (base.Count >= 2) ? this[this.Count - 2] : null; } }
		public Bar this[int indexRequested] { get {
				lock (this.LockBars) {
					if (indexRequested < 0) return null;
					if (indexRequested >= base.Count) return null;
					Bar bar = base[base.Keys[indexRequested]];
					
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
		protected object LockBars;
		public BarsUnscaled(string symbol, string reasonToExist = "NOREASON") : base(new DateTimeAscending()) {
			this.LockBars = new object();
			this.ReasonToExist = reasonToExist;
			this.Symbol = symbol;
			this.symbolInfo = new SymbolInfo();
			// not initialized when Designer shows you Sq1.Charting.ChartControl with sample BarsUnscaled-derived 10 bars
			// (Designer doesn't like reflecting/invoking static methods used to instantiate Assembler-singleton)
			if (Assembler.IsInitialized) this.symbolInfo = Assembler.InstanceInitialized.RepositoryCustomSymbolInfo.FindSymbolInfoOrNew(this.Symbol);
		}
		public void Add(DateTime key, double value) {
			throw new Exception("UNSUPPORTED_AVOID_USING_BarsUnscaled.Add(DateTime key, double value): use BarAppend(Bar barAdding) instead");
		}
		public new void Remove(DateTime key) {
			throw new Exception("UNSUPPORTED_AVOID_USING_BarsUnscaled.Remove(int bar): users rely on BarsUnscaled[index].ParentBarsIndex");
		}
		public new void RemoveAt(int bar) {
			throw new Exception("UNSUPPORTED_AVOID_USING_BarsUnscaled.RemoveAt(int bar): users rely on BarsUnscaled[index].ParentBarsIndex");
		}
		public new void InsertAt(int bar) {
			throw new Exception("UNSUPPORTED_AVOID_USING_BarsUnscaled.InsertAt(int bar): users rely on BarsUnscaled[index].ParentBarsIndex");
		}
		public virtual void RenameSymbol(string symbolNew) {
			// TODO test rename during streaming OR disable renaming feature in GUI while streaming
			this.Symbol = symbolNew;
			foreach (Bar barRegardlessScaledOrNot in this.Values) {
				barRegardlessScaledOrNot.Symbol = symbolNew;
			}
			this.RaiseBarsRenamed_SEEMS_EXCESSIVE();
		}
		public void RaiseBarsRenamed_SEEMS_EXCESSIVE() {
			if (this.BarsRenamed_SEEMS_EXCESSIVE == null) return;
			this.BarsRenamed_SEEMS_EXCESSIVE(this, new BarsUnscaledEventArgs(this));
		}
		protected virtual void BarAppend(Bar barAdding) {
			lock (this.LockBars) {
				this.CheckThrowDateNotNullNotMinValue(barAdding);
				this.CheckThrowDateAlreadyAdded(barAdding.DateTimeOpen);
				this.CheckThrowDatePriorToLast(barAdding.DateTimeOpen);
				this.CheckThrowDateIsNotLessThanScaleDictates(barAdding.DateTimeOpen);
				this.CheckThrowDOHLCVasLast(barAdding);
				try {
					base.Add(barAdding.DateTimeOpen, barAdding);
					//barAdding.SetParentForBackwardUpdate(this, base.Count - 1);
				} catch (Exception e) {
					string msg = "SORTEDLIST_INTERNAL_EXCEPTION base.Add(bar[" + barAdding + "]) to " + this;
					throw (new Exception(msg, e));
				}
			}
		}
		
		protected virtual void CheckThrowDateIsNotLessThanScaleDictates(DateTime dateTimeOpen) {
			return;
		}
		public void CheckThrowDateAlreadyAdded(DateTime appending) {
			if (base.Keys.Contains(appending) == false) return;
			Bar alreadyAdded = base[appending];
			string msg = "Can not add time[" + appending + "]: already added as ["
				+ alreadyAdded + "]: " + this.ToString();
			throw new Exception(msg);
		}
		public void CheckThrowDOHLCVasLast(Bar barAdding) {
			Bar lastBar = this.BarLast;
			if (lastBar == null) return;
			string msg;
			bool sameDOHLCV = lastBar.HasSameDOHLCVas(barAdding, "barAdding", "LastBar", out msg);
			if (sameDOHLCV == false) return;
			string wonder = "WHO_ADDED? LastBar.DOHLCV[" + lastBar + "] = barAdding.DOHLCV[" + barAdding + "]; " + msg;
			throw new Exception(msg);
		}
		public void CheckThrowDatePriorToLast(DateTime appending) {
			if (this.Count == 0) return;
			DateTime lastDateTime = this.BarLast.DateTimeOpen;
			if (lastDateTime.ToBinary() < appending.ToBinary()) return;

			string msg = "#1 Can not append time[" + appending + "] should be > LastBar["
				+ (this.Count - 1) + "].DateTimeOpen=[" + lastDateTime + "]: " + this;
			throw new Exception(msg);
		}
		public void CheckThrowDateNotNullNotMinValue(Bar barAdding) {
			if (barAdding == null) {
				throw new Exception("Can't Bars.Add() barAdding=null");
			}
			if (barAdding.DateTimeOpen == DateTime.MinValue) {
				throw new Exception("Can't Bars.Add() barAdding.DateTimeOpen=DateTime.MinValue");
			}
		}
		public Bar FindBarWithDateEqualOrLaterThan(DateTime dateTime) {
			Bar barFound = null;
			int indexFound = this.FindBarIndexWithDateEqualOrLaterThan(dateTime);
			if (indexFound >= 0)  barFound = this[indexFound];
			return barFound;
		}
		public int FindBarIndexWithDateEqualOrLaterThan(DateTime target) {
			int ret = -1;
			if (this.Count < 1) return -1;  
			if (target < this.BarFirst.DateTimeOpen) return -2;  
			if (target > this.BarLast.DateTimeOpen) return -3;
			if (target == this.BarFirst.DateTimeOpen) return 0;  
			if (target == this.BarLast.DateTimeOpen) return this.Count - 1;
		
			//v1
			DateTime dateTimeFound = this.DateTimes.Find(delegate(DateTime eachBarDateTime) { return eachBarDateTime >= target; });
			if (dateTimeFound != DateTime.MinValue) ret = this.DateTimes.IndexOf(dateTimeFound);
			//v2
//			int left = 0;
//			int right = this.Count - 1;
//			int middle = (right - left) / 2;
//			DateTime middleDate = this[middle].DateTimeOpen;
//			bool directionAscending = false;
//			for (int nodesChecked = 0; nodesChecked < this.Count; nodesChecked++) {
//				if (target > middleDate) {
//					ret = middle;
//					left = middle;
//					directionAscending = true;
//				} else {
//					right = middle;
//					directionAscending = false;
//				}
//				ret = middle;
//				if (++nodesChecked >= this.Count) return -1;
//			}
//			Array.Fi
			
			return ret;
		}
		public string FormatValue(double value) {
			return value.ToString(this.Format);
		}
		public string Format { get {
				return "N" + this.SymbolInfo.DecimalsPrice;
			} }
		public override string ToString() {
			string ret = "[" + this.Symbol + "]" + this.Count + "bars";
			Bar barLast = this.BarLast;
			ret += (barLast != null)
				? " LastClose=[" + barLast.Close + "] @[" + barLast.DateTimeOpen + "]"
				: " this.BarLast=[null]";
			if (string.IsNullOrEmpty(ReasonToExist) == false) ret += "/" + ReasonToExist;
			return ret;
		}
	}
}