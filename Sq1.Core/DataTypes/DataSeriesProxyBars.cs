using System;
using System.Diagnostics;

namespace Sq1.Core.DataTypes {
	public class DataSeriesProxyBars : DataSeriesTimeBased {
		readonly Bars barsBeingProxied;
		readonly DataSeriesProxyableFromBars dataSeriesBeingExposed;

		public DataSeriesProxyBars(Bars bars, DataSeriesProxyableFromBars dataSeriesBeingExposed) : base(bars.ScaleInterval, "WILL_INITIALIZE_LATER") {
			//base.ScaleInterval = bars.ScaleInterval;
			this.barsBeingProxied = bars;
			this.dataSeriesBeingExposed = dataSeriesBeingExposed;
			base.Description = this.dataSeriesBeingExposed + " for " + this.ToString();
		}
//		public override double StreamingValue {
//			get {
//				return this[this.Count-1];
//			}
//			set {
//				if (double.IsNaN(value)) return;
//				string msg = "Cannot set StreamingValue[" + value + "] for " + this.ToString();
//				throw new InvalidOperationException();
//			}
//		}
		//public override IList<DateTime> DateTimes { get { return this.barsBeingProxied.DateTimes; } }
		public override int Count { get { return this.barsBeingProxied.Count; } }
		public override double this[int barIndex] {
			get {
				if (this.barsBeingProxied == null) {
					Debugger.Break();
					return double.NaN;
				}
				if (barIndex >= this.barsBeingProxied.Count) {
					string msg = "DEPRECATED_NOTATION this[this.Count]_get; STREAMING_VALUE_IS_NOW_AT_this[this.Count-1]";
					Debugger.Break();
					throw new Exception(msg);
					//return double.NaN;
				}
				switch (this.dataSeriesBeingExposed) {
						//case "DateTimeOpen": return this.barsBeingProxied[barIndex].DateTimeOpen;
					case DataSeriesProxyableFromBars.Open: return this.barsBeingProxied[barIndex].Open;
					case DataSeriesProxyableFromBars.High: return this.barsBeingProxied[barIndex].High;
					case DataSeriesProxyableFromBars.Low: return this.barsBeingProxied[barIndex].Low;
					case DataSeriesProxyableFromBars.Close: return this.barsBeingProxied[barIndex].Close;
					case DataSeriesProxyableFromBars.Volume: return this.barsBeingProxied[barIndex].Volume;
				}
				throw new InvalidOperationException("PLEASE DON'T ADD MORE VALUES INTO ENUM DataSeriesProxyableFromBars:"
					+ " barFieldBeingProxied[" + this.dataSeriesBeingExposed.ToString()
					+ "] should've been constructed as Open/High/Low/Close/Volume in " + this.ToString());
			}
			set {
				throw new InvalidOperationException("Cannot assign [" + this.dataSeriesBeingExposed.ToString() + "]=[" + value + "] in " + this.ToString());
			}
		}
		public override void Append(DateTime dateTimeAdding, double value) {
			throw new InvalidOperationException("Cannot Add into " + this.ToString());
		}
		public override void Clear() {
			throw new InvalidOperationException("Cannot Clear " + this.ToString());
		}
		public override string ToString() {
			return "DataSeriesProxyBars[" + this.dataSeriesBeingExposed.ToString() + "] based on " + this.barsBeingProxied.ToString();
		}
		public bool IsProxyFor(Bars bars, DataSeriesProxyableFromBars dataSeriesProxyableFromBars) {
			return barsBeingProxied == bars && dataSeriesBeingExposed == dataSeriesProxyableFromBars;
		}
	}
}