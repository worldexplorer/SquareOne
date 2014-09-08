using System;
namespace Sq1.Core.DataTypes {
	public class BarScaleInterval {
		public BarScale Scale;
		public int Interval;
		public bool IsIntraday {
			get {
				if (this.Scale == BarScale.Hour) return true;
				if (this.Scale == BarScale.Minute) return true;
				if (this.Scale == BarScale.Second) return true;
				if (this.Scale == BarScale.Tick) return true;
				return false;
			}
		}
		public BarScaleInterval() {
			this.Scale = BarScale.Unknown;
			this.Interval = 0;
		}
		public BarScaleInterval(BarScale scale, int barInterval) : this() {
			if (scale != BarScale.Unknown && barInterval <= 0) throw new Exception("please mention barInterval for scale[" + scale + "]");
			this.Scale = scale;
			this.Interval = barInterval;
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
			
		public static bool operator <(BarScaleInterval first, BarScaleInterval second) {
			return first != second && !(first > second);
		}
		public static bool operator >(BarScaleInterval first, BarScaleInterval second) {
			switch (first.Scale) {
				case BarScale.Daily:
					return second.IsIntraday;
				case BarScale.Weekly:
					return second.IsIntraday
						|| second.Scale == BarScale.Daily;
				case BarScale.Monthly:
					return second.IsIntraday 
						|| second.Scale == BarScale.Daily 
						|| second.Scale == BarScale.Weekly;
				case BarScale.Minute:
					if (second.Scale != BarScale.Second) {
						if (second.Scale != BarScale.Tick) {
							return second.Scale == BarScale.Minute && first.Interval
								> second.Interval;
						}
					}
					return true;
				case BarScale.Second:
					return second.Scale == BarScale.Tick 
						|| (second.Scale == BarScale.Second && first.Interval > second.Interval);
				case BarScale.Tick:
					return second.Scale == BarScale.Tick && first.Interval > second.Interval;
				case BarScale.Quarterly:
					return second.IsIntraday 
						|| second.Scale == BarScale.Daily 
						|| second.Scale == BarScale.Weekly 
						|| second.Scale == BarScale.Monthly;
				case BarScale.Yearly:
					return second.Scale != BarScale.Yearly;
				default:
					return false;
			}
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
		public int AsTimeSpanInSeconds { get {
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
		public TimeSpan AsTimeSpan { get { return new TimeSpan(0, 0, this.AsTimeSpanInSeconds); } }
		public override string ToString() {
			string str = "";
			if (this.IsIntraday) {
				str = this.Interval + "-";
			}
			return str + this.Scale.ToString();
		}
		public BarScaleInterval Clone() {
			return (BarScaleInterval)this.MemberwiseClone();
		}
	}
}
