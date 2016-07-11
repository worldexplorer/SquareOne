using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Sq1.Core.DataTypes {
	public class MarketClearingTimespan {
		[JsonProperty]	public DateTime			SuspendServerTimeOfDay;
		[JsonIgnore]	public string			SuspendServerTimeOfDay_asString { get { return this.SuspendServerTimeOfDay.ToString("HH:mm"); } }

		[JsonProperty]	public DateTime			ResumeServerTimeOfDay;
		[JsonIgnore]	public string			ResumeServerTimeOfDay_asString { get { return this.ResumeServerTimeOfDay.ToString("HH:mm"); } }

		[JsonProperty]	public List<DayOfWeek>	DaysOfWeekWhenClearingHappens;
		[JsonIgnore]	public string			DaysOfWeekWhenClearingHappens_asString { get {
				if (DaysOfWeekWhenClearingHappens == null) return "";
				return MarketInfo.DaysOfWeekAsString(DaysOfWeekWhenClearingHappens);
			} }

		public bool ClearingHappensOnDayOfWeek(DayOfWeek dayOfWeek) {
			if (this.DaysOfWeekWhenClearingHappens == null) return true;
			if (this.DaysOfWeekWhenClearingHappens.Count == 0) return true;
			if (this.DaysOfWeekWhenClearingHappens.Contains(dayOfWeek)) return true;
			return false;
		}
		public void AbsorbFrom(MarketClearingTimespan another) {
			this.SuspendServerTimeOfDay = another.SuspendServerTimeOfDay;
			this.ResumeServerTimeOfDay  = another.ResumeServerTimeOfDay;
			if (another.DaysOfWeekWhenClearingHappens == null) return;
			this.DaysOfWeekWhenClearingHappens = new List<DayOfWeek>(another.DaysOfWeekWhenClearingHappens);
		}
		public override string ToString() {
			string ret = "[" + this.SuspendServerTimeOfDay_asString + ".." + this.ResumeServerTimeOfDay_asString + "]";
			if (this.DaysOfWeekWhenClearingHappens != null && this.DaysOfWeekWhenClearingHappens.Count > 0) {
				ret += "{" + this.DaysOfWeekWhenClearingHappens_asString + "}";
			}
			return ret;
		}
		//public override bool Equals(object obj) {
		//    MarketClearingTimespan another = obj as MarketClearingTimespan;
		//    if (another == null) return false;
		//    bool ret =
		//            this. ResumeServerTimeOfDay_asString == another. ResumeServerTimeOfDay_asString
		//         && this.SuspendServerTimeOfDay_asString == another.SuspendServerTimeOfDay_asString
		//         && this.DaysOfWeekWhenClearingHappens_asString == another.DaysOfWeekWhenClearingHappens_asString;
		//    return ret;
		//}
	}
}
