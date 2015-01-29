using System;
using System.Diagnostics;

namespace Sq1.Core.DataTypes {
	public class DataSeriesProxyBars : DataSeriesTimeBased {
		// private => public allows Indicators to "feel" that Bars were replaced to Backtesting growing copy and create new Proxy
		public	readonly Bars							BarsBeingProxied;
				readonly DataSeriesProxyableFromBars	dataSeriesBeingExposed;

		public DataSeriesProxyBars(Bars bars, DataSeriesProxyableFromBars dataSeriesBeingExposed) : base(bars.ScaleInterval, "WILL_INITIALIZE_LATER") {
			//base.ScaleInterval = bars.ScaleInterval;
			this.BarsBeingProxied = bars;
			this.dataSeriesBeingExposed = dataSeriesBeingExposed;
			base.Description = this.dataSeriesBeingExposed + " for " + this.ToString();
		}
//		public override double StreamingValue {
//			get { return this[this.Count-1]; }
//			set {
//				if (double.IsNaN(value)) return;
//				string msg = "Cannot set StreamingValue[" + value + "] for " + this.ToString();
//				throw new InvalidOperationException();
//			}
//		}
		//public override IList<DateTime> DateTimes { get { return this.barsBeingProxied.DateTimes; } }
		public override int Count { get { return this.BarsBeingProxied.Count; } }
		public override double this[int barIndex] {
			get {
				if (this.BarsBeingProxied == null) {
					Debugger.Break();
					return double.NaN;
				}
				if (barIndex >= this.BarsBeingProxied.Count) {
					string msg = "DEPRECATED_NOTATION this[this.Count]_get; STREAMING_VALUE_IS_NOW_AT_this[this.Count-1]";
					Debugger.Break();
					throw new Exception(msg);
					//return double.NaN;
				}
				switch (this.dataSeriesBeingExposed) {
					//case "DateTimeOpen": return this.barsBeingProxied[barIndex].DateTimeOpen;
					case DataSeriesProxyableFromBars.Open:		return this.BarsBeingProxied[barIndex].Open;
					case DataSeriesProxyableFromBars.High:		return this.BarsBeingProxied[barIndex].High;
					case DataSeriesProxyableFromBars.Low:		return this.BarsBeingProxied[barIndex].Low;
					case DataSeriesProxyableFromBars.Close:		return this.BarsBeingProxied[barIndex].Close;
					case DataSeriesProxyableFromBars.Volume:	return this.BarsBeingProxied[barIndex].Volume;
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
			throw new InvalidOperationException("DataSeriesProxyBars_IS_A_PROXY__DID_YOU_WANT_TO_APPEND_TO_BARS???_CAN_NOT_ADD_ONLY_ONE_VALUE_INTO " + this.ToString());
		}
		public override void Clear() {
			throw new InvalidOperationException("DataSeriesProxyBars_IS_A_PROXY__DID_YOU_WANT_TO_CLEAR_BARS???_CAN_NOT_CLEAR " + this.ToString());
		}
		public override string ToString() {
			//return "DataSeriesProxyBars[" + this.dataSeriesBeingExposed.ToString() + "] based on " + this.BarsBeingProxied.ToString();
			return "[" + this.dataSeriesBeingExposed.ToString() + "] for " + this.BarsBeingProxied.ToString();
		}
		public bool IsProxyFor(Bars bars, DataSeriesProxyableFromBars dataSeriesProxyableFromBars) {
			return BarsBeingProxied == bars && dataSeriesBeingExposed == dataSeriesProxyableFromBars;
		}
	}
}