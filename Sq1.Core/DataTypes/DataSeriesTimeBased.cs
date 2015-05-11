using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sq1.Core.DataTypes {
	public class DataSeriesTimeBased : DataSeriesBasic {
				SortedList<DateTime, double> doublesByDate;
		public	DateTime	LastDateAppended	{ get; protected set; }
		public	double		LastValueAppended	{ get; protected set; }

		public BarScaleInterval ScaleInterval;
		public virtual double this[int barIndex] {
			get { return base[barIndex]; }
			set { base[barIndex] = value; }
		}
		//public virtual double this[int barIndex] {
		//	get { return this.doublesByDate[this.doublesByDate.Keys[barIndex]]; }
		//	set { this.doublesByDate[this.doublesByDate.Keys[barIndex]] = value; }
		//}

		public virtual double this[DateTime date] { get { return this.doublesByDate[date]; } }

		protected DataSeriesTimeBased(BarScaleInterval scaleInterval, int decimals = 2) : base() {	// NOT_USING_PARENTS_List<double>doubleValues_BUT_I_NEED_IT_NON_NULL_FOR_CLEAR()
			doublesByDate = new SortedList<DateTime, double>();
			ScaleInterval = scaleInterval;
			LastDateAppended = DateTime.MinValue;
			LastValueAppended = double.NaN;
			base.Decimals = decimals;
		}
		public DataSeriesTimeBased(BarScaleInterval scaleInterval, string description, int decimals = 2) : this(scaleInterval, decimals) {
			this.Description = description;
		}
		public bool ContainsDate(DateTime dateTime) {
			return this.doublesByDate.ContainsKey(dateTime);
		}
		public override void Append(double value) {
			throw new Exception("USE_BASE_CLASS_DataSeriesBasic_INSTEAD_OF_DataSeriesTimeBased");
		}
		public virtual void Append(DateTime dateTimeAdding, double value) {
			try {
				this.checkThrow(dateTimeAdding);
				base.Append(value);
				this.doublesByDate.Add(dateTimeAdding, value);
				this.LastDateAppended = dateTimeAdding;
				this.LastValueAppended = value;
			} catch (Exception e) {
				#if DEBUG
				Debugger.Launch();
				#endif
				throw e;
			}
		}
		public override void Clear() {
			// base.doubleValues_NOT_CONSTRUCTED_base.Clear()_WILL_THROW
			base.Clear();
			this.doublesByDate.Clear();
			this.LastDateAppended = DateTime.MinValue;
			this.LastValueAppended = double.NaN;
		}
		void checkThrow(DateTime appending) {
			if (appending == DateTime.MinValue) {
				string msg = "#1 APPENDING_MIN_DATE_NOT_ALLOWED appending[" + appending + "]";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}
			if (this.doublesByDate.ContainsKey(appending)) {
				string msg = "#2 APPENDING_SAME_DATE_TWICE_NOT_ALLOWED doublesByDate[" + appending + "]=[" + this.doublesByDate[appending] + "]";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}

			if (this.doublesByDate.Count == 0) return;
			int lastIndex = this.doublesByDate.Count - 1;
			DateTime lastDateKey = this.doublesByDate.Keys[lastIndex];
			if (lastDateKey != this.LastDateAppended) {
				string msg = "#3 LAST_APPEND_DIDNT_UPDATE_INTERNAL_LastDateAppended this.LastStaticDate[" + this.LastDateAppended
					+ "] should be = lastDateKey[" + lastDateKey + "]; appending[" + appending + "] is probably ok";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}

			if (appending < this.LastDateAppended) {
				string msg = "#4 APPENDING_EARLIER_DATE_NOT_ALLOWED appending[" + appending + "] should be > this.LastStaticDate["
					+ lastIndex + "/" + this.LastDateAppended + "]=[" + this.LastDateAppended + "]";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}
		}
		public void SumupOrAppend(DateTime dateTimeAdding, double value) {
			int indexFound = this.doublesByDate.IndexOfKey(dateTimeAdding);
			if (indexFound == -1) {
				this.Append(dateTimeAdding, value);
				return;
			}
			double valueExisting = this[indexFound];
			valueExisting += value;
			this[indexFound] = valueExisting;
		}
		// decided to use CumulativeCash,CumulativeEquity in SystemPerformanceSlice
//		public void Replace(DateTime dateTimeAdding, double value, bool throwIfDoesntExist = true) {
//			int indexFound = this.doublesByDate.IndexOfKey(dateTimeAdding);
//			if (indexFound == -1) {
//				this.Append(dateTimeAdding, value);
//				return;
//			}
//			double valueExisting = this[indexFound];
//			valueExisting += value;
//			this[indexFound] = valueExisting;
//		}
		public override string ToString() {
			string ret = "[" + this.ScaleInterval + "]" + this.Count + "doublesByDate";
			ret += " Last[" + this.LastValueAppended.ToString(base.Format) + "]@[" + this.LastDateAppended + "]";
			ret += base.ToString();
			return ret;
		}
	}
}