using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sq1.Core.DataTypes {
	public class DataSeriesTimeBased : DataSeriesBasic {
		SortedList<DateTime, double> doublesByDate;
		public DateTime LastDateAppended;

//			{ get {
//				if (this.DateTimes.Count <= 0) return DateTime.MinValue;
//				return this.DateTimes[this.DateTimes.Count - 1];
//			} }
//		public override double TotalSumTillIndex(int indexTill) {
//			if (this.Count == 0) return 0.0;
//			double ret = 0;
//			for (int i = 0; i < this.Count; i++) ret += this.doublesByDate.Values[i];
//			return ret;
//		}

//		public virtual IList<DateTime> DateTimes { get { return this.doublesByDate.Keys; } }
//		public override IList<double> Values { get { return this.doublesByDate.Values; } }
//		public override int Count { get { return this.doublesByDate.Count; } }
//		public override int Capacity {
//			get { return this.doublesByDate.Capacity; }
//			set { this.doublesByDate.Capacity = value; }
//		}
		public BarScaleInterval ScaleInterval;
//		public bool IsIntraday { get { return this.ScaleInterval.IsIntraday; } }

		public DataSeriesTimeBased(BarScaleInterval scaleInterval) {	// : base()
			doublesByDate = new SortedList<DateTime, double>();
//			firstValidValueIndex = 0;
			ScaleInterval = scaleInterval;
			LastDateAppended = DateTime.MinValue;
		}
		public DataSeriesTimeBased(BarScaleInterval scaleInterval, string description) : this(scaleInterval) {
			this.Description = description;
		}
//		public double this[DateTime dateTime] { get {
//				if (this.doublesByDate.ContainsKey(dateTime) == false) {
//					DateTime existingKeyFromBelow = FindExistingKeyFromBelow(dateTime);
//					DateTime existingKeyFromAbove = FindExistingKeyFromAbove(dateTime);
//					string msg = "KEY_NOT_FOUND[" + dateTime + "]_CLOSEST_[" + existingKeyFromBelow + "]_[" + existingKeyFromAbove + "]: " + this;
//					throw new ArgumentException(msg);
//				}
//				return this.doublesByDate[dateTime];
//			} }
//		public DateTime FindExistingKeyFromBelow(DateTime dateNonExistingRequested) {
//			DateTime ret = DateTime.MinValue;
//			foreach (DateTime each in this.doublesByDate.Keys) {
//				if (dateNonExistingRequested > each) continue;
//				if (ret <= each) ret = each;
//			}
//			return ret;
//		}
//		public DateTime FindExistingKeyFromAbove(DateTime dateNonExistingRequested) {
//			DateTime ret = DateTime.MaxValue;
//			foreach (DateTime each in this.doublesByDate.Keys) {
//				if (dateNonExistingRequested < each) continue;
//				if (ret >= each) ret = each;
//			}
//			return ret;
//		}
		public bool ContainsDate(DateTime dateTime) {
			return this.doublesByDate.ContainsKey(dateTime);
		}
//		[Obsolete("USE_BASE_THIS[INT]_DONT_USE_DATETIME_KEYS_ITS_SLOW!!! after profiling you'll see lot of SortedList.BinarySearch()")]
//		public override double this[int barIndex] {
//			get {
//				if (barIndex < 0 || barIndex > this.Count) {
//					string msg = "[" + barIndex + "] is out of bounds: " + this;
//					throw new ArgumentOutOfRangeException(msg);
//				}
//				if (barIndex == this.doublesByDate.Count) {
//					return base.StreamingValue;
//				}
//				return this.doublesByDate.Values[barIndex];
//			}
//			set {
//				if (barIndex < 0 || barIndex > this.Count) {
//					string msg = "[" + barIndex + "] is out of bounds: " + this;
//					throw new ArgumentOutOfRangeException(msg);
//				}
//				if (barIndex == this.doublesByDate.Count) {
//					base.StreamingValue = value;
//					return;
//				}
//				DateTime dateKey = this.doublesByDate.Keys[barIndex];
//				this.doublesByDate[dateKey] = value;
//			}
//		}
		public virtual void Append(DateTime dateTimeAdding, double value) {
			try {
				this.checkThrowDateAlreadyAdded(dateTimeAdding);
				this.checkThrowDatePriorToLast(dateTimeAdding);
				base.Append(value);
				this.doublesByDate.Add(dateTimeAdding, value);
			} catch (Exception e) {
				#if DEBUG
				Debugger.Break();
				#endif
				throw e;
			}
		}
		public override void Clear() {
			base.Clear();
			this.doublesByDate.Clear();
			this.LastDateAppended = DateTime.MinValue;
		}
//		public override void Insert(int index, double value) {
//			throw new Exception("Can't insert by index, use Add(DateTime, double) instead");
//		}
//		public override void RemoveAt(int index) {
//			this.doublesByDate.RemoveAt(index);
//		}
		private void checkThrowDateAlreadyAdded(DateTime appending) {
			if (this.doublesByDate.ContainsKey(appending) == false) return;
			string msg = "#2 Can not append time[" + appending + "] which already was added as [" + this.doublesByDate[appending] + "]";
			#if DEBUG
			Debugger.Break();
			#endif
			throw new Exception(msg);
		}
		private  void checkThrowDatePriorToLast(DateTime appending) {
			if (this.LastDateAppended == DateTime.MinValue) return;
			if (this.LastDateAppended < appending) return;

			string msg = "#3 Can not append time[" + appending + "] should be > this.LastStaticDate["
				+ this.LastDateAppended + "]=[" + (this.doublesByDate.Count - 1) + "]=[" + this.LastDateAppended + "]";
			#if DEBUG
			Debugger.Break();
			#endif
			throw new Exception(msg);
		}
		private void checkThrowLastStaticDateEqualsToAppended(DateTime appending) {
			if (this.doublesByDate.Count == 0) return; 
			if (this.doublesByDate.Keys[this.doublesByDate.Count - 1] == this.LastDateAppended) return;
			string msg = "#4 Can not append time[" + appending + "] this.LastStaticDate[" + this.LastDateAppended
				+ "] should be = LastAppended[" + this.doublesByDate.Keys[this.doublesByDate.Count - 1] + "]";
			#if DEBUG
			Debugger.Break();
			#endif
			throw new Exception(msg);
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
		public override string ToString() {
			string ret = "[" + this.ScaleInterval + "]" + this.Count + "doublesByDate ";
			ret += base.ToString();
			return ret;
		}


		#region copypaste from Bar
		public DateTime DateTimeOpen { get; protected set; }
		public DateTime DateTimeNextBarOpenUnconditional { get; protected set; }
		public DateTime DateTimePreviousBarOpenUnconditional { get; protected set; }

		public void RoundDateDownInitTwoAuxDates(DateTime dateTimeOpen) {
			this.DateTimeOpen = roundDateDownToMyInterval(dateTimeOpen);
			///if (this.DateTimeOpen.CompareTo(dateTimeOpen) == 0) {
			//	int a = 1;
			//}
			this.DateTimeNextBarOpenUnconditional = this.addIntervalsToDate(this.DateTimeOpen, 1);
			this.DateTimePreviousBarOpenUnconditional = this.addIntervalsToDate(this.DateTimeOpen, -1);
		}
		private DateTime addIntervalsToDate(DateTime dateTime1, int intervalMultiplier) {
			if (this.DateTimeOpen == DateTime.MinValue) return DateTime.MinValue;
			DateTime dateTime = roundDateDownToMyInterval(dateTime1);
			int addTimeIntervals = this.ScaleInterval.Interval * intervalMultiplier;
			switch (this.ScaleInterval.Scale) {
				case BarScale.Tick:
					throw new ArgumentException("Tick scale is not supported");
				case BarScale.Second:
					dateTime = dateTime.AddSeconds((double)addTimeIntervals);
					break;
				case BarScale.Minute:
					dateTime = dateTime.AddMinutes((double)addTimeIntervals);
					break;
				case BarScale.Hour:
					dateTime = dateTime.AddHours((double)addTimeIntervals);
					break;
				case BarScale.Daily:
					dateTime = dateTime.Date.AddDays((double)addTimeIntervals);
					break;
				case BarScale.Weekly:
					dateTime = dateTime.Date.AddDays(addTimeIntervals * 7);
					break;
				case BarScale.Monthly:
					dateTime = dateTime.Date.AddMonths(addTimeIntervals);
					break;
				case BarScale.Quarterly:
					dateTime = dateTime.Date.AddMonths(addTimeIntervals * 3);
					break;
				case BarScale.Yearly:
					dateTime = dateTime.Date.AddYears(addTimeIntervals);
					break;
				default:
					throw new Exception("this.ScaleInterval.Scale[" + this.ScaleInterval.Scale
						+ "] is not supported");
			}
			return dateTime;
		}
		private DateTime roundDateDownToMyInterval(DateTime dateTime1) {
			if (this.ScaleInterval == null) throw new Exception("ScaleInterval=null in roundDateDownToInterval(" + dateTime1 + ")");
			DateTime dateTime = new DateTime(dateTime1.Ticks);
			switch (this.ScaleInterval.Scale) {
				case BarScale.Tick:
					throw new ArgumentException("Tick scale is not supported");
				case BarScale.Second:
					int secondsRoundedDown = ((int)Math.Floor((double)dateTime.Second / this.ScaleInterval.Interval)) * this.ScaleInterval.Interval;
					dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, secondsRoundedDown);
					break;
				case BarScale.Minute:
					int minutesRoundedDown = ((int)Math.Floor((double)dateTime.Minute / this.ScaleInterval.Interval)) * this.ScaleInterval.Interval;
					dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, minutesRoundedDown, 0);
					break;
				case BarScale.Hour:
					int hoursRoundedDown = ((int)Math.Floor((double)dateTime.Hour / this.ScaleInterval.Interval)) * this.ScaleInterval.Interval;
					dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hoursRoundedDown, 0, 0);
					break;
				case BarScale.Daily:
					dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
					break;
				case BarScale.Weekly:
					while (dateTime.DayOfWeek != DayOfWeek.Monday) dateTime.AddDays(-1);
					dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
					break;
				case BarScale.Monthly:
					dateTime = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
					break;
				case BarScale.Quarterly:
					int monthBeginningOfQuarter = ((int)Math.Floor((double)dateTime.Month / 3)) * 3;
					dateTime = new DateTime(dateTime.Year, monthBeginningOfQuarter, 1, 0, 0, 0);
					break;
				case BarScale.Yearly:
					dateTime = new DateTime(dateTime.Year, 1, 1, 0, 0, 0);
					break;
				default:
					throw new Exception("this.ScaleInterval.Scale[" + this.ScaleInterval.Scale + "] is not supported");
			}
			return dateTime;
		}
		#endregion
	}
}