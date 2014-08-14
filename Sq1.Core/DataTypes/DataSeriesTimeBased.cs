using System;
using System.Collections.Generic;

namespace Sq1.Core.DataTypes {
	public class DataSeriesTimeBased : DataSeriesBasic {
		SortedList<DateTime, double> doublesByDate;
		public virtual IList<DateTime> DateTimes { get { return this.doublesByDate.Keys; } }
		public override int Count { get { return this.doublesByDate.Count; } }
		public override int Capacity {
			get { return this.doublesByDate.Capacity; }
			set { this.doublesByDate.Capacity = value; }
		}
		public BarScaleInterval ScaleInterval;
		public bool IsIntraday { get { return this.ScaleInterval.IsIntraday; } }
		public DataSeriesTimeBased() {	// : base()
			this.doublesByDate = new SortedList<DateTime, double>();
			this.firstValidValueIndex = 0;
			this.StreamingValue = double.NaN;
		}
		public DataSeriesTimeBased(string description) : this() {
			this.Description = description;
		}
		public double this[DateTime dateTime] { get {
				return this.doublesByDate[dateTime];
			} }
		public bool ContainsKey(DateTime dateTime) {
			return this.doublesByDate.ContainsKey(dateTime);
		}
		public override double this[int barIndex] {
			get {
				if (barIndex < 0 || barIndex > this.Count) {
					string msg = "[" + barIndex + "] is out of bounds: " + this;
					throw new ArgumentOutOfRangeException(msg);
				}
				if (barIndex == this.doublesByDate.Count) {
					return base.StreamingValue;
				}
				return this.doublesByDate.Values[barIndex];
			}
			set {
				if (barIndex < 0 || barIndex > this.Count) {
					string msg = "[" + barIndex + "] is out of bounds: " + this;
					throw new ArgumentOutOfRangeException(msg);
				}
				if (barIndex == this.doublesByDate.Count) {
					base.StreamingValue = value;
					return;
				}
				DateTime dateKey = this.doublesByDate.Keys[barIndex];
				this.doublesByDate[dateKey] = value;
			}
		}
		public virtual void Append(DateTime dateTimeAdding, double value) {
			try {
				this.checkThrowDateAlreadyAdded(dateTimeAdding);
				//this.CheckThrowDatePriorToLast(dateTimeAdding);
				this.doublesByDate.Add(dateTimeAdding, value);
			} catch (Exception e) {
				int a = 1;
				throw e;
			}
		}
		public override void Insert(int index, double value) {
			throw new Exception("Can't insert by index, use Add(DateTime, double) instead");
		}
		public override void Clear() {
			this.doublesByDate.Clear();
		}
		public override void RemoveAt(int index) {
			this.doublesByDate.RemoveAt(index);
		}
		private void checkThrowDateAlreadyAdded(DateTime adding) {
			if (doublesByDate.ContainsKey(adding) == false) return;
			string msg = "#2 Can not append time[" + adding + "] which already was added as [" + this.doublesByDate[adding] + "]";
			throw new Exception(msg);
		}
		protected void CheckThrowDatePriorToLast(DateTime appending) {
			if (this.DateTimes.Count == 0) return;
			DateTime lastDateTime = this.LastStaticDate;
			if (lastDateTime.ToBinary() < appending.ToBinary()) return;

			string msg = "#3 Can not append time[" + appending + "] should be > this.Date["
				+ (this.DateTimes.Count - 1) + "]=[" + lastDateTime + "]";
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
		public DateTime LastStaticDate {
			get {
				if (this.DateTimes.Count <= 0) return DateTime.MinValue;
				return this.DateTimes[this.DateTimes.Count - 1];
			}
		}
		public override double TotalSumTillIndex(int indexTill) {
			if (this.Count == 0) return 0.0;
			double ret = 0;
			for (int i = 0; i < this.Count; i++) ret += this.doublesByDate.Values[i];
			return ret;
		}
		public override string ToString() {
			string ret = "[" + this.ScaleInterval + "]" + this.Count + "doublesByDate";
			if (this.Count > 0) {
				int lastBar = this.Count - 1;
				ret += " last=[" + this[lastBar] + "] @[" + this.DateTimes[lastBar] + "]";
			}
			if (string.IsNullOrEmpty(base.Description) == false) ret += "/" + base.Description;
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