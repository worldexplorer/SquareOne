using System;
using System.Text;

using Newtonsoft.Json;

namespace Sq1.Core.DataTypes {
	public class BarScaleInterval {
		[JsonProperty]	public	int			Interval;
		[JsonProperty]	public	BarScale	Scale;

		[JsonProperty]	public	bool		IsIntraday { get {
				switch (this.Scale) {
					case BarScale.Unknown:
					case BarScale.Tick:
					case BarScale.Second:
					case BarScale.Minute:
					case BarScale.Hour:
						return true;

					case BarScale.Daily:
					case BarScale.Weekly:
					case BarScale.Monthly:
					case BarScale.Quarterly:
					case BarScale.Yearly:
						return true;

					default:
						Assembler.PopupException("IsIntraday__ADD_HANDLER_FOR_NEW_BarScale_ENUM[" + this.Scale + "]");
						return true;
				}
			} }

		[JsonProperty]	public	string		AsString_cached;
		[JsonProperty]	public	int			AsTimeSpanInSeconds { get {
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
		[JsonProperty]	public	TimeSpan	AsTimeSpan { get { return new TimeSpan(0, 0, this.AsTimeSpanInSeconds); } }


		[JsonIgnore]			string		asStringShortest_cached;
		[JsonProperty]	public	string		AsStringShortest_cached { get {
			if (this.asStringShortest_cached == null) {
				this.asStringShortest_cached = this.Interval.ToString();
				switch (this.Scale) {
					case BarScale.Unknown:		this.asStringShortest_cached += "UKNOWN"; break;
					case BarScale.Tick:			this.asStringShortest_cached += "t"; break;
					case BarScale.Second:		this.asStringShortest_cached += "s"; break;
					case BarScale.Minute:		this.asStringShortest_cached += "m"; break;
					case BarScale.Hour:			this.asStringShortest_cached += "h"; break;
					case BarScale.Daily:		this.asStringShortest_cached += "d"; break;
					case BarScale.Weekly:		this.asStringShortest_cached += "w"; break;
					case BarScale.Monthly:		this.asStringShortest_cached += "mo"; break;
					case BarScale.Quarterly:	this.asStringShortest_cached += "q"; break;
					case BarScale.Yearly:		this.asStringShortest_cached += "y"; break;
					default: 					this.asStringShortest_cached += "AsStringShortest_cached__ADD_HANDLER_FOR_NEW_BarScale_ENUM[" + this.Scale + "]"; break;
				}
			}
			return asStringShortest_cached;
		} }
		[JsonIgnore]			string		asStringShort_cached;
		[JsonProperty]	public	string		AsStringShort_cached { get {
			if (this.asStringShort_cached == null) {
				this.asStringShort_cached = this.Interval.ToString();
				switch (this.Scale) {
					case BarScale.Unknown:		this.asStringShort_cached += "UKNOWN"; break;
					case BarScale.Tick:			this.asStringShort_cached += "tks"; break;
					case BarScale.Second:		this.asStringShort_cached += "sec"; break;
					case BarScale.Minute:		this.asStringShort_cached += "min"; break;
					case BarScale.Hour:			this.asStringShort_cached += "hrs"; break;
					case BarScale.Daily:		this.asStringShort_cached += "days"; break;
					case BarScale.Weekly:		this.asStringShort_cached += "wks"; break;
					case BarScale.Monthly:		this.asStringShort_cached += "months"; break;
					case BarScale.Quarterly:	this.asStringShort_cached += "quart"; break;
					case BarScale.Yearly:		this.asStringShort_cached += "yr"; break;
					default: 					this.asStringShort_cached += "AsStringShort_cached__ADD_HANDLER_FOR_NEW_BarScale_ENUM[" + this.Scale + "]"; break;
				}
			}
			return asStringShort_cached;
		} }

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
				this.AsString_cached = this.Interval +  "-" + this.Scale.ToString();
			}
			return this.AsString_cached; 
		}
		public BarScaleInterval Clone() {
			return (BarScaleInterval)this.MemberwiseClone();
		}

		public void StringsCachedInvalidate() {
			this.asStringShort_cached = null;
			this.asStringShortest_cached = null;
			this.AsString_cached = null;
		}
	}
}
