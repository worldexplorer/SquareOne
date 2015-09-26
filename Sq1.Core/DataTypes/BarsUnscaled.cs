using System;

namespace Sq1.Core.DataTypes {
	// #DEVELOP_PROFILER_SAYS_DATETIME>.COMPARE_IS_VERY_SLOW_DURING_BACKTEST  public class BarsUnscaled : BarsUnscaledSortedList_DEPRECATED {
	public class BarsUnscaled : BarsUnscaledListTwins {
		public event EventHandler<BarsUnscaledEventArgs> BarsRenamed_SEEMS_EXCESSIVE;
	
		public	string Symbol { get; protected set; }
		public	string ReasonToExist;

				SymbolInfo symbolInfo;
		public	SymbolInfo SymbolInfo {
			get {
				if (this.symbolInfo == null) {
					this.symbolInfo = Assembler.InstanceInitialized.RepositorySymbolInfo.FindSymbolInfoOrNew(this.Symbol);
				}
				return this.symbolInfo;
			}
			set { this.symbolInfo = value; }	//it's initialized in ctor(), SymbolInfo=null will initiate repository scan on next get{}
		}

		public	Bar BarFirst	{ get { return (base.Count >= 1) ? this[0] : null; } }
		public	Bar BarLast		{ get { return (base.Count >= 1) ? this[this.Count - 1] : null; } }
		public	Bar BarPreLast	{ get { return (base.Count >= 2) ? this[this.Count - 2] : null; } }


		public BarsUnscaled(string symbol, string reasonToExist = "NOREASON") : base() {
			this.ReasonToExist = reasonToExist;
			this.Symbol = symbol;
			this.symbolInfo = new SymbolInfo();
			// not initialized when Designer shows you Sq1.Charting.ChartControl with sample BarsUnscaled-derived 10 bars
			// (Designer doesn't like reflecting/invoking static methods used to instantiate Assembler-singleton)
			if (Assembler.IsInitialized) this.symbolInfo = Assembler.InstanceInitialized.RepositorySymbolInfo.FindSymbolInfoOrNew(this.Symbol);
		}

		public void Add(DateTime key, double value) {
			throw new Exception("UNSUPPORTED_AVOID_USING_BarsUnscaled.Add(DateTime key, double value): use BarAppend(Bar barAdding) instead");
		}

		protected virtual void BarAppend(Bar barAdding) {
			lock (this.BarsLock) {
				this.CheckThrowDateNotNullNotMinValue(barAdding);
				this.CheckThrowDateAlreadyAdded(barAdding.DateTimeOpen);
				this.CheckThrowDatePriorToLast(barAdding.DateTimeOpen);
				this.CheckThrowDateIsNotLessThanScaleDictates(barAdding.DateTimeOpen);
				this.CheckThrowDOHLCVasLast(barAdding);
				try {
					base.Add(barAdding);
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
			if (base.ContainsDate(appending) == false) return;
			Bar alreadyAdded = base[appending];
			string msg = "Can not add time[" + appending + "]: already added as ["
				+ alreadyAdded + "]: " + this.ToString();
			throw new Exception(msg);
		}
		public void CheckThrowDOHLCVasLast(Bar barAdding) {
			Bar lastBar = this.BarLast;
			if (lastBar == null) return;
			string msg = "BARS_IDENTICAL";
			bool sameDOHLCV = lastBar.HasSameDOHLCVas(barAdding, "barAdding", "LastBar", ref msg);
			if (sameDOHLCV == false) return;
			if (string.IsNullOrEmpty(msg)) msg = "PROJECT_SETTINGS_COMPILE_[VERBOSE_STRINGS_SLOW]_OPTION_ENABLE_TO_SEE_DETAILS";
			string wonder = "WHO_ADDED? LastBar.DOHLCV[" + lastBar + "] = barAdding.DOHLCV[" + barAdding + "]; " + msg;
			throw new Exception(msg);
		}
		public void CheckThrowDatePriorToLast(DateTime appending) {
			if (this.Count == 0) return;
			DateTime lastDateTime = this.BarLast.DateTimeOpen;
			if (lastDateTime.Ticks < appending.Ticks) return;

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
		
		public virtual void RenameSymbol(string symbolNew) {
			// TODO test rename during streaming OR disable renaming feature in GUI while streaming
			this.Symbol = symbolNew;
			foreach (Bar barRegardlessScaledOrNot in this.BarsList) {
				barRegardlessScaledOrNot.Symbol = symbolNew;
			}
			this.RaiseBarsRenamed_SEEMS_EXCESSIVE();
		}
		public void RaiseBarsRenamed_SEEMS_EXCESSIVE() {
			if (this.BarsRenamed_SEEMS_EXCESSIVE == null) return;
			this.BarsRenamed_SEEMS_EXCESSIVE(this, new BarsUnscaledEventArgs(this));
		}
//		public Bar FindBarWithDateEqualOrLaterThan(DateTime dateTime) {
//			Bar barFound = null;
//			int indexFound = this.FindBarIndexWithDateEqualOrLaterThan(dateTime);
//			if (indexFound >= 0)  barFound = this[indexFound];
//			return barFound;
//		}
//		public int FindBarIndexWithDateEqualOrLaterThan(DateTime target) {
//			int ret = -1;
//			if (this.Count < 1) return -1;  
//			if (target < this.BarFirst.DateTimeOpen) return -2;  
//			if (target > this.BarLast.DateTimeOpen) return -3;
//			if (target == this.BarFirst.DateTimeOpen) return 0;  
//			if (target == this.BarLast.DateTimeOpen) return this.Count - 1;
//		
//			//v1
//			DateTime dateTimeFound = this.DateTimes.Find(delegate(DateTime eachBarDateTime) { return eachBarDateTime >= target; });
//			if (dateTimeFound != DateTime.MinValue) ret = this.DateTimes.IndexOf(dateTimeFound);
//			//v2
////			int left = 0;
////			int right = this.Count - 1;
////			int middle = (right - left) / 2;
////			DateTime middleDate = this[middle].DateTimeOpen;
////			bool directionAscending = false;
////			for (int nodesChecked = 0; nodesChecked < this.Count; nodesChecked++) {
////				if (target > middleDate) {
////					ret = middle;
////					left = middle;
////					directionAscending = true;
////				} else {
////					right = middle;
////					directionAscending = false;
////				}
////				ret = middle;
////				if (++nodesChecked >= this.Count) return -1;
////			}
////			Array.Fi
//			
//			return ret;
//		}
		public string FormatValue(double value) {
			return value.ToString(this.SymbolInfo.PriceFormat);
		}
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
