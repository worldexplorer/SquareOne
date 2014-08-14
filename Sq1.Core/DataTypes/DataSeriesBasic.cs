using System;
using System.Collections.Generic;

namespace Sq1.Core.DataTypes {
	public class DataSeriesBasic {
		public string Description;
		List<double> doubleValues;
		public virtual double StreamingValue { get; set; }
		public virtual int Count { get { return this.doubleValues.Count; } }
		public virtual double LastStaticValue {
			get {
				if (this.Count == 0) return Double.NaN; 
				return this[this.Count - 1];
			}
		}
		protected int firstValidValueIndex;
		public int FirstValidValueIndex {
			get { return this.firstValidValueIndex; }
			set {
				if (value < 0) {
					this.firstValidValueIndex = 0;
				} else {
					this.firstValidValueIndex = value;
				}
				if (this.firstValidValueIndex >= this.Count) {
					this.firstValidValueIndex = this.Count - 1;
				}
			}
		}
		public double TotalSum { get { return this.TotalSumTillIndex(this.Count - 1); } }
		public virtual int Capacity {
			get { return this.doubleValues.Capacity; }
			set { this.doubleValues.Capacity = value; }
		}
		public DataSeriesBasic() {
			this.doubleValues = new List<double>();
			this.firstValidValueIndex = 0;
			this.StreamingValue = double.NaN;
		}
		public DataSeriesBasic(string description) : this() {
			this.Description = description;
		}
		public virtual double this[int barIndex] {
			get {
				if (barIndex < 0 || barIndex > this.doubleValues.Count) {
					string msg = this.Description + "[" + barIndex + "] is out of bounds: " 
						+ this.Description + ".Count=" + this.doubleValues.Count;
					throw new ArgumentOutOfRangeException(msg);
				}
				if (barIndex == this.doubleValues.Count) {
					// get rid of this call, it's inaccurate
					return this.doubleValues[barIndex - 1];
				}
				return this.doubleValues[barIndex];
			}
			set {
				if (barIndex < 0 || barIndex > this.Count) {
					string msg = "[" + barIndex + "] is out of bounds: " + this;
					throw new ArgumentOutOfRangeException(msg);
				}
				if (barIndex == this.doubleValues.Count) {
					this.StreamingValue = value;
					return;
				}
				this.doubleValues[barIndex] = value;
			}
		}
		public virtual void Add(double value) {
			this.doubleValues.Add(value);
		}
		public virtual void Insert(int index, double value) {
			this.doubleValues.Insert(index, value);
		}
		public virtual void Clear() {
			this.doubleValues.Clear();
		}
		public virtual void RemoveAt(int index) {
			this.doubleValues.RemoveAt(index);
		}
		public virtual double TotalSumTillIndex(int indexTill) {
			if (this.Count == 0) {
				return 0.0;
			}
			double ret = 0;
			for (int i = this.FirstValidValueIndex; i < this.Count; i++) {
				ret += this.doubleValues[i];
			}
			return ret;
		}
		// bool handlingVolume = indicator.Series.TypeSafeIsProxyFor(this.Executor.Bars, DataSeriesProxyableFromBars.Volume)
		public virtual bool TypeSafeIsProxyFor(Bars bars, DataSeriesProxyableFromBars barField) {
			if (this is DataSeriesProxyBars == false) return false;
			DataSeriesProxyBars proxy = this as DataSeriesProxyBars;
			if (proxy.IsProxyFor(bars, barField)) return true;
			return false;
		}
		
		public bool TurnsDownAtBarIndex(int bar) {
			if (bar <= 0) return false;
			if (this.Count < 2) return false;
			if (this[bar] >= this[bar - 1]) return false;
			for (int i = bar - 1 ; i >= 1 ; i--) {
				if (this[i] > this[i - 1]) return true;
				if (this[i] < this[i - 1]) return false;
			}
			return false;
		}
		public bool TurnsUpAtBarIndex(int bar) {
			if (bar <= 0) return false;
			if (this.Count < 2) return false;
			if (this[bar] <= this[bar - 1]) return false;
			for (int i = bar - 1 ; i >= 1 ; i--) {
				if (this[i] < this[i - 1]) return true;
				if (this[i] > this[i - 1]) return false;
			}
			return false;
		}
	}
}
