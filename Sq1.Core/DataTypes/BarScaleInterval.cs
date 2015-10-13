using System;
using System.Text;

using Newtonsoft.Json;

namespace Sq1.Core.DataTypes {
	public class BarScaleInterval {
		[JsonProperty]	public	string		AsString_cached;
		[JsonProperty]	public	BarScale	Scale;
		[JsonProperty]	public	int			Interval;
		[JsonProperty]	public	bool		IsIntraday { get {
				if (this.Scale == BarScale.Hour) return true;
				if (this.Scale == BarScale.Minute) return true;
				if (this.Scale == BarScale.Second) return true;
				if (this.Scale == BarScale.Tick) return true;
				return false;
			} }
		[JsonProperty]	public int AsTimeSpanInSeconds { get {
				int ret = -1;
				int yearly = (365 + 1/4) * 24 * 60 * 60;
				switch(this.Scale) {
					case BarScale.Yearly:		ret = yearly;			break;
					case BarScale.Quarterly:	ret = yearly / 4;		break;
					case BarScale.Monthly:		ret = yearly / 12;		break;
					case BarScale.Weekly:		ret = 60 * 60 * 24 * 7;	break;
					case BarScale.Daily:		ret = 60 * 60 * 24;		break;
					case BarScale.Hour:			ret = 60 * 60;			break;
					case BarScale.Minute:		ret = 60;				break;
					case BarScale.Second:		ret = 1;				break;

					case BarScale.Unknown:
					case BarScale.Tick:
					default:
						break;
				}
				ret *= this.Interval;
				return ret;
			} }
		[JsonProperty]	public TimeSpan AsTimeSpan { get { return new TimeSpan(0, 0, this.AsTimeSpanInSeconds); } }

		// keep public ctor for JSON_DESERIALIZER :((
		public BarScaleInterval() {
			this.Scale = BarScale.Unknown;
			this.Interval = 0;
		}
		public BarScaleInterval(BarScale scale, int barInterval) : this() {
			if (scale != BarScale.Unknown && barInterval <= 0) throw new Exception("please mention barInterval for scale[" + scale + "]");
			Scale = scale;
			Interval = barInterval;
		}
		public static BarScaleInterval Parse(string str) {
			if (str == null) {
				throw new ArgumentNullException();
			}
			string[] array = str.Split(' ');
			if (array.Length == 0) {
				throw new ArgumentException();
			}
			int barInterval = 0;
			BarScale scale;
			if (array.Length == 1) {
				scale = (BarScale)Enum.Parse(typeof(BarScale), array[0]);
			} else {
				barInterval = int.Parse(array[0]);
				scale = (BarScale)Enum.Parse(typeof(BarScale), array[1]);
			}
			return new BarScaleInterval(scale, barInterval);
		}
		#region Equals and GetHashCode implementation // SharpDevelop5.0beta
		public override bool Equals(object obj) {
			BarScaleInterval other = obj as BarScaleInterval;
			if (other == null) return false;
			return this.Scale == other.Scale && this.Interval == other.Interval;
		}
		public static bool operator ==(BarScaleInterval lhs, BarScaleInterval rhs) {
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(BarScaleInterval lhs, BarScaleInterval rhs) {
			return !(lhs == rhs);
		}
		public override int GetHashCode() {
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * this.Scale.GetHashCode();
				hashCode += 1000000009 * this.Interval.GetHashCode();
			}
			return hashCode;
		}
		#endregion

		public bool LessGranularThan(BarScaleInterval another) {
			return this.AsTimeSpanInSeconds < another.AsTimeSpanInSeconds;		// SHOW_ME_THE_NUMBERS this.AsTimeSpan < another.AsTimeSpan
		}
		public bool MoreGranularThan(BarScaleInterval another) {
			return this.AsTimeSpanInSeconds > another.AsTimeSpanInSeconds;		// SHOW_ME_THE_NUMBERS this.AsTimeSpan > another.AsTimeSpan
		}

		public bool CanConvertTo(BarScaleInterval scaleIntervalTo) {
			// for proper comparison, make sure Sq1.Core.DataTypes.BarScale enum has scales growing from Tick to Yearly
			if (this.Scale > scaleIntervalTo.Scale) return false;	//can't convert from 1hr to 5min
			if (this.Scale < scaleIntervalTo.Scale) return true;
			// here we are if (this.ScaleInterval.Scale == scaleIntervalTo.Scale)
			if (this.Interval <= scaleIntervalTo.Interval) return true;
			return false;
		}

		public static BarScaleInterval MaxValue = new BarScaleInterval(BarScale.Yearly, 1);
		public static BarScaleInterval MinValue = new BarScaleInterval(BarScale.Tick, 1);
		public static BarScaleInterval FromTimeSpan(TimeSpan ts) {
			BarScaleInterval ret = new BarScaleInterval(BarScale.Unknown, 0);
			if (ts.Days >= 365) {
				ret.Scale = BarScale.Yearly;
				ret.Interval = 1 + ((int) ts.Days / 365);
				return ret;
			}
			if (ts.Days >= 28) {
				ret.Scale = BarScale.Monthly;
				ret.Interval = 1 + ((int) ts.Days / 28);
				return ret;
			}
			ret.Scale = BarScale.Weekly;
			if (ts.Days >= 7) {
				ret.Interval = 1 + ((int) ts.Days / 7);
				return ret;
			}
			ret.Scale = BarScale.Daily;
			if (ts.Days >= 1) {
				ret.Interval = ts.Days;
				return ret;
			}
			ret.Scale = BarScale.Hour;
			if (ts.Hours >= 1) {
				ret.Interval = ts.Hours;
				return ret;
			}
			ret.Scale = BarScale.Minute;
			if (ts.Minutes >= 30) {
				ret.Interval = 30 * ((int) ts.Minutes / 30);
				return ret;
			}
			if (ts.Minutes >= 20) {
				ret.Interval = 20 * ((int) ts.Minutes / 20);
				return ret;
			}
			if (ts.Minutes >= 15) {
				ret.Interval = 15 * ((int) ts.Minutes / 15);
				return ret;
			}
			if (ts.Minutes >= 10) {
				ret.Interval = 10 * ((int) ts.Minutes / 10);
				return ret;
			}
			if (ts.Minutes >= 5) {
				ret.Interval = 5 * ((int) ts.Minutes / 5);
				return ret;
			}
			if (ts.Minutes >= 1) {
				ret.Interval = 1 * ((int) ts.Minutes / 1);
				return ret;
			}
			ret.Scale = BarScale.Tick;
			return ret;
		}
		
		public override string ToString() {
			if (this.AsString_cached == null) {
				StringBuilder sb = new StringBuilder();
				if (this.IsIntraday) {
					sb.Append(this.Interval);
					sb.Append("-");
				}
				sb.Append(this.Scale.ToString());
				this.AsString_cached = sb.ToString();
			}
			return this.AsString_cached; 
		}
		public BarScaleInterval Clone() {
			return (BarScaleInterval)this.MemberwiseClone();
		}
	}
}
