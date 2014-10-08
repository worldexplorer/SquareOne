using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Sq1.Core.DataTypes {
	public class MarketClearingTimespan {
		[JsonProperty]	public DateTime SuspendServerTimeOfDay;
		[JsonIgnore]	public string SuspendServerTimeOfDayAsString { get { return SuspendServerTimeOfDay.ToString("HH:mm"); } }
		[JsonProperty]	public DateTime ResumeServerTimeOfDay;
		[JsonIgnore]	public string ResumeServerTimeOfDayAsString { get { return ResumeServerTimeOfDay.ToString("HH:mm"); } }
		[JsonProperty]	public List<DayOfWeek> DaysOfWeekWhenClearingHappens;
		[JsonIgnore]	public string DaysOfWeekWhenClosingNotHappensAsString { get {
				if (DaysOfWeekWhenClearingHappens == null) return "";
				return MarketInfo.DaysOfWeekAsString(DaysOfWeekWhenClearingHappens);
			} }
		public override string ToString() {
			string ret = "[" + SuspendServerTimeOfDayAsString + ".." + ResumeServerTimeOfDayAsString + "]";
			if (DaysOfWeekWhenClearingHappens != null && DaysOfWeekWhenClearingHappens.Count > 0) {
				ret += "{" + DaysOfWeekWhenClosingNotHappensAsString + "}";
			}
			return ret;
		}
		public bool ClearingHappensOnDayOfWeek(DayOfWeek dayOfWeek) {
			if (this.DaysOfWeekWhenClearingHappens == null) return true;
			if (this.DaysOfWeekWhenClearingHappens.Count == 0) return true;
			if (this.DaysOfWeekWhenClearingHappens.Contains(dayOfWeek)) return true;
			return false;
		}
	}
}
