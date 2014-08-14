using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sq1.Core.DataTypes {
	[DataContract]
	public class MarketClearingTimespan {
		[DataMember]
		public DateTime SuspendServerTimeOfDay;
		public string SuspendServerTimeOfDayAsString { get { return SuspendServerTimeOfDay.ToString("HH:mm"); } }
		[DataMember]
		public DateTime ResumeServerTimeOfDay;
		public string ResumeServerTimeOfDayAsString { get { return ResumeServerTimeOfDay.ToString("HH:mm"); } }
		[DataMember]
		public List<DayOfWeek> DaysOfWeekWhenClearingHappens;
		public string DaysOfWeekWhenClosingNotHappensAsString {
			get {
				if (DaysOfWeekWhenClearingHappens == null) return "";
				return MarketInfo.DaysOfWeekAsString(DaysOfWeekWhenClearingHappens);
			}
		}
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
