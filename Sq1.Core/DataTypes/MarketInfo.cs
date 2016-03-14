using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Sq1.Core.DataTypes {
	public class MarketInfo {
		[JsonProperty]	public	string				Name;
		[JsonProperty]	public	string				Description;
		[JsonProperty]	public	DateTime			MarketOpenServerTime;
		[JsonIgnore]	public	string				MarketOpenServerTimeAsString { get { return MarketCloseServerTime.ToString("HH:mm"); } }
		[JsonProperty]	public	DateTime			MarketCloseServerTime;
		[JsonIgnore]	public	string				MarketCloseServerTimeAsString { get { return MarketOpenServerTime.ToString("HH:mm"); } }
		[JsonProperty]	public	List<DayOfWeek>		DaysOfWeekOpen;
		[JsonProperty]	public	string				TimeZoneName;
		[JsonProperty]	public	TimeZoneInfo		TimeZoneInfo { get {
				TimeZoneInfo ret = TimeZoneInfo.Local;
				if (String.IsNullOrEmpty(this.TimeZoneName)) return ret;
				try {
					ret = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneName);
				} catch (Exception e) {
					string msg = "we are here when?...";
					ret = TimeZoneInfo.Local;
					//throw;
				}
				return ret;
			} }
		[JsonProperty]	public	List<MarketClearingTimespan>	ClearingTimespans;
		[JsonProperty]	public	List<DateTime>					HolidaysYMD000;
		[JsonProperty]	public	List<MarketShortDay>			ShortDays;

		public MarketInfo() {
			Name = "ERROR_DESERIALISING_JSON";
			Description = "ERROR_DESERIALISING_JSON";
			MarketOpenServerTime = DateTime.MinValue;
			MarketCloseServerTime = DateTime.MinValue;
			DaysOfWeekOpen = new List<DayOfWeek>();
			//DaysOfWeekOpen.AddRange(new List<DayOfWeek>() {
			//	DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
			TimeZoneName = "ERROR_DESERIALISING_JSON";
			ClearingTimespans = new List<MarketClearingTimespan>();
			HolidaysYMD000 = new List<DateTime>();
			ShortDays = new List<MarketShortDay>();
		}
		public string DaysOfWeekOpenAsString { get {
				if (DaysOfWeekOpen == null) return "Mon,Tue,Wed,Thu,Fri";
				return DaysOfWeekAsString(DaysOfWeekOpen);
			} }
		string clearingTimespansAsString { get {
				string closedHours = "";
				foreach (MarketClearingTimespan closedHour in this.ClearingTimespans) {
					closedHours += closedHour + ",";
				}
				closedHours = closedHours.TrimEnd(',');
				return closedHours;
			} }
		public static string DaysOfWeekAsString(List<DayOfWeek> dows) {
			string ret = "";
			foreach (DayOfWeek dow in dows) {
				ret += dow.ToString().Substring(0, 3) + ",";
			}
			ret = ret.TrimEnd(',');
			return ret;
		}
		public override string ToString() {
			string ret = this.Name + ": " + this.MarketOpenServerTime.ToString("HH:mm")
				+ ".." + this.MarketCloseServerTime.ToString("HH:mm");
			if (ClearingTimespans != null && ClearingTimespans.Count > 0) {
				ret += " clearing:" + this.clearingTimespansAsString;
			}
			return ret;
		}
		
		// wanna put it in separate file?... partial?..
		[JsonIgnore]	public DateTime ServerTimeNow { get { return this.ConvertLocalTimeToServer(DateTime.Now); } }
		[JsonIgnore]	public bool TodayIsTradingDay { get { return this.IsTradeableDayServerTime(this.ServerTimeNow); } }
		[JsonIgnore]	public bool IsMarketOpenNow { get { return this.IsMarketOpenAtServerTime(this.ServerTimeNow); } }
		[JsonIgnore]	public bool MarketIsAfterCloseNow { get { return this.isMarketAfterCloseServerTime(this.ServerTimeNow); } }
		[JsonIgnore]	public DateTime MarketCloseLocalTime { get { return this.ConvertServerTimeToLocal(this.MarketCloseServerTime); } }
		[JsonIgnore]	public DateTime LastTradingSessionEndedServerTime { get {
				DateTime dateTimeServer = ServerTimeNow;
				if (this.IsTradeableDayServerTime(dateTimeServer)
						&& this.isMarketAfterCloseServerTime(dateTimeServer) == false) {
					dateTimeServer = dateTimeServer.AddDays(-1.0);
				}
				while (!this.IsTradeableDayServerTime(dateTimeServer)) {
					dateTimeServer = dateTimeServer.AddDays(-1.0);
				}
				dateTimeServer = new DateTime(dateTimeServer.Year, dateTimeServer.Month, dateTimeServer.Day,
					this.MarketOpenServerTime.Hour, this.MarketOpenServerTime.Minute, 0);
				return dateTimeServer;
			} }
		public bool IsTradeableDayServerTime(DateTime dateServer) {
			bool isHoliday = this.isHoliday(dateServer);
			bool isDayOfWeekOpen = this.isTradeableDayOfWeek(dateServer);
			return (isDayOfWeekOpen == true && isHoliday == false);
		}
		public bool IsMarketOpenAtServerTime(DateTime dateTimeServer) {
			if (this.IsTradeableDayServerTime(dateTimeServer.Date) == false) return false;
			DateTime openTimeServer;
			DateTime closeTimeServer;
			this.GetRegularOrShortDayOpenCloseMarketTimeForServerDate(dateTimeServer, out openTimeServer, out closeTimeServer);
			bool isMarketAfterOpening = dateTimeServer.TimeOfDay >= openTimeServer.TimeOfDay;
			bool isMarketBeforeClosing = dateTimeServer.TimeOfDay < closeTimeServer.TimeOfDay;
			bool isMarketSuspendedForClearing = this.IsMarketSuspendedForClearing(dateTimeServer);
			bool marketIsOpen = isMarketAfterOpening && isMarketBeforeClosing && (isMarketSuspendedForClearing == false);
			if (marketIsOpen == false) {
				string msg = "breakpoint";
			}
			return marketIsOpen;
		}
		bool isMarketAfterCloseServerTime(DateTime dateTimeServer) {
			DateTime GenericOrShortDayCloseTimeServer = this.MarketCloseServerTime;
			MarketShortDay shortDay = this.getShortMarketDayForServerTime(dateTimeServer);
			if (shortDay != null) {
				GenericOrShortDayCloseTimeServer = shortDay.ServerTimeClosing;
			}
			if (this.IsTradeableDayServerTime(dateTimeServer) == false) return false;
			if (dateTimeServer.Hour == GenericOrShortDayCloseTimeServer.Hour) {
				return dateTimeServer.Minute > GenericOrShortDayCloseTimeServer.Minute;
			}
			return dateTimeServer.Hour > GenericOrShortDayCloseTimeServer.Hour;
		}
		private bool isMarketBeforeOpenServerTime(DateTime dateTimeServer) {
			DateTime GenericOrShortDayOpenTimeServer = this.MarketOpenServerTime;
			MarketShortDay shortDay = this.getShortMarketDayForServerTime(dateTimeServer);
			if (shortDay != null) {
				GenericOrShortDayOpenTimeServer = shortDay.ServerTimeClosing;
			}
			if (this.IsTradeableDayServerTime(dateTimeServer) == false) return false;
			if (dateTimeServer.Hour == GenericOrShortDayOpenTimeServer.Hour) {
				return dateTimeServer.Minute < GenericOrShortDayOpenTimeServer.Minute;
			}
			return dateTimeServer.Hour < GenericOrShortDayOpenTimeServer.Hour;
		}
		public void GetRegularOrShortDayOpenCloseMarketTimeForServerDate(DateTime dateTimeServer
				, out DateTime openTime, out DateTime closeTime) {
			openTime = this.MarketOpenServerTime;
			closeTime = this.MarketCloseServerTime;
			MarketShortDay shortDay = getShortMarketDayForServerTime(dateTimeServer);
			if (shortDay != null) {
				openTime = shortDay.ServerTimeOpening;
				closeTime = shortDay.ServerTimeClosing;
			}
			return;
		}
		public DateTime GetNextMarketServerTimeStamp(DateTime serverDateTime, BarScaleInterval scale,
				out MarketClearingTimespan clearingTimespan) {
			if (serverDateTime == DateTime.MinValue) {
				throw new Exception("IRefuse: NextTimeStamp for DateTime.MinValue");
				//serverDateTime = this.ConvertLocalTimeToServer(DateTime.Now);
			}
			DateTime ret = serverDateTime;
			bool addNextRank = false;
			int year = ret.Year;
			int nextMonth = ret.Month + 1;
			int intervalsCeiled = 0;
			int valueCeiled = 0;
			//MarketClearingTimespan
			clearingTimespan = null;
			switch (scale.Scale) {
				case BarScale.Tick:
					throw new ArgumentException("Tick scale is not supported");
				case BarScale.Second:
					intervalsCeiled = (int)Math.Floor((double)(serverDateTime.Second / scale.Interval));
					valueCeiled = (intervalsCeiled + 1) * scale.Interval;
					if (valueCeiled >= 60) {
						addNextRank = true;
						valueCeiled -= 60;
					}
					ret = new DateTime(serverDateTime.Year, serverDateTime.Month, serverDateTime.Day,
						serverDateTime.Hour, serverDateTime.Minute, valueCeiled);
					if (addNextRank) ret = ret.AddMinutes(1);

					clearingTimespan = GetClearingTimespanIfMarketSuspended(ret);
					if (clearingTimespan != null) {
						ret = this.AdvanceToWhenClearingResumes(ret, clearingTimespan);
						//ret = ret.AddSeconds((double)scale.Interval);
					}
					if (this.isMarketAfterCloseServerTime(ret)) {
						ret = this.AdvanceToNextDayOpen(ret);
						//ret = ret.AddSeconds((double)scale.Interval);
					}
					break;
				case BarScale.Minute:
					intervalsCeiled = (int)Math.Floor((double)(serverDateTime.Minute / scale.Interval));
					valueCeiled = (intervalsCeiled + 1) * scale.Interval;
					if (valueCeiled >= 60) {
						addNextRank = true;
						valueCeiled -= 60;
					}
					ret = new DateTime(serverDateTime.Year, serverDateTime.Month, serverDateTime.Day,
						serverDateTime.Hour, valueCeiled, 0);
					if (addNextRank) ret = ret.AddHours(1);

					clearingTimespan = GetClearingTimespanIfMarketSuspended(ret);
					if (clearingTimespan != null) {
						ret = this.AdvanceToWhenClearingResumes(ret, clearingTimespan);
						//ret = ret.AddMinutes((double)scale.Interval);
					}
					bool afterClose = this.isMarketAfterCloseServerTime(ret);
					bool beforeOpen = this.isMarketBeforeOpenServerTime(ret);
					if (beforeOpen) {
						ret = this.AdvanceToThisDayOpen(ret);
					} else {
						if (afterClose) {
							ret = this.AdvanceToNextDayOpen(ret);
						}
					}

					break;
				case BarScale.Hour:
					intervalsCeiled = (int)Math.Floor((double)(serverDateTime.Minute / scale.Interval));
					valueCeiled = (intervalsCeiled + 1) * scale.Interval;
					if (valueCeiled >= 24) {
						addNextRank = true;
						valueCeiled -= 24;
					}
					ret = new DateTime(serverDateTime.Year, serverDateTime.Month, serverDateTime.Day,
						serverDateTime.Hour, valueCeiled, 0);
					if (addNextRank) ret = ret.AddDays(1);

					clearingTimespan = GetClearingTimespanIfMarketSuspended(ret);
					if (clearingTimespan != null) {
						ret = this.AdvanceToWhenClearingResumes(ret, clearingTimespan);
						//ret = ret.AddHours((double)scale.Interval);
					}
					if (this.isMarketAfterCloseServerTime(ret)) {
						ret = this.AdvanceToNextDayOpen(ret);
						//ret = ret.AddHours((double)scale.Interval);
					}
					break;
				case BarScale.Daily:
					ret = this.AdvanceToNextDayOpen(ret);
					break;
				case BarScale.Weekly:
					ret = this.AdvanceToNextDayOpen(ret);
					while (ret.DayOfWeek != DayOfWeek.Friday) {
						ret = ret.AddDays(1.0);
					}
					if (this.IsTradeableDayServerTime(ret) == false) {
						if (ret.Day == serverDateTime.Day - 1) {
							ret = ret.AddDays(7.0);
							if (!this.IsTradeableDayServerTime(ret)) {
								ret = ret.AddDays(-1.0);
							}
						} else {
							ret = ret.AddDays(-1.0);
						}
					}
					break;
				case BarScale.Monthly:
					if (nextMonth == 13) {
						nextMonth = 1;
						year++;
					}
					ret = new DateTime(year, nextMonth, 1, 0, 0, 0);
					ret = this.AdvanceToNextDayOpen(ret);
					ret = ret.AddDays(-1.0);
					while (!this.IsTradeableDayServerTime(ret)) {
						ret = ret.AddDays(-1.0);
					}
					break;
				case BarScale.Quarterly:
					if (nextMonth == 13) {
						year++;
						nextMonth = 1;
					} else {
						nextMonth = ((int)Math.Floor((double)ret.Month / 3)) * 3;
					}
					ret = new DateTime(year, nextMonth, 28);
					do {
						ret = ret.AddDays(1.0);
					} while (ret.Month == nextMonth);
					ret = ret.AddDays(-1.0);
					ret = this.AdvanceToNextDayOpen(ret);
					while (!this.IsTradeableDayServerTime(ret)) {
						ret = ret.AddDays(-1.0);
					}
					break;
				case BarScale.Yearly:
					ret = new DateTime(ret.Year + 1, 12, 31);
					ret = this.AdvanceToNextDayOpen(ret);
					while (!this.IsTradeableDayServerTime(ret)) {
						ret = ret.AddDays(-1.0);
					}
					break;
			}
			return ret;
		}
		public DateTime AdvanceToNextDayOpen(DateTime dateTimeServer) {
			DateTime ret = new DateTime(dateTimeServer.Year, dateTimeServer.Month, dateTimeServer.Day,
				 this.MarketOpenServerTime.Hour, this.MarketOpenServerTime.Minute, this.MarketOpenServerTime.Second);
			do {
				ret = ret.AddDays(1.0);
			} while (!this.IsTradeableDayServerTime(ret));
			MarketShortDay shortDay = this.getShortMarketDayForServerTime(ret);
			if (shortDay != null) {
				ret = new DateTime(ret.Year, ret.Month, ret.Day, shortDay.ServerTimeOpening.Hour,
					shortDay.ServerTimeOpening.Minute, shortDay.ServerTimeOpening.Second);
			}
			return ret;
		}
		public DateTime AdvanceToThisDayOpen(DateTime dateTimeServer) {
			DateTime ret = new DateTime(dateTimeServer.Year, dateTimeServer.Month, dateTimeServer.Day,
				 this.MarketOpenServerTime.Hour, this.MarketOpenServerTime.Minute, this.MarketOpenServerTime.Second);
			MarketShortDay shortDay = this.getShortMarketDayForServerTime(ret);
			if (shortDay != null) {
				ret = new DateTime(ret.Year, ret.Month, ret.Day, shortDay.ServerTimeOpening.Hour,
					shortDay.ServerTimeOpening.Minute, shortDay.ServerTimeOpening.Second);
			}
			return ret;
		}
		public DateTime AdvanceToWhenClearingResumes(DateTime dateTimeServer, MarketClearingTimespan clearingTimespan) {
			DateTime ret = new DateTime(dateTimeServer.Year, dateTimeServer.Month, dateTimeServer.Day,
				 clearingTimespan.ResumeServerTimeOfDay.Hour, clearingTimespan.ResumeServerTimeOfDay.Minute, clearingTimespan.ResumeServerTimeOfDay.Second);
			if (ret.TimeOfDay < dateTimeServer.TimeOfDay) {
				ret = ret.AddDays(1);
			}
			return ret;
		}
		public DateTime ConvertLocalTimeToServer(DateTime localTime) {
			//return TimeZoneInfo.ConvertTime(localTime.ToUniversalTime(), this.TimeZoneInfo);
			return TimeZoneInfo.ConvertTime(localTime, TimeZoneInfo.Local, this.TimeZoneInfo);
		}
		public DateTime ConvertServerTimeToLocal(DateTime serverTime) {
			//return TimeZoneInfo.ConvertTime(nativeTime.ToUniversalTime(), TimeZoneInfo.Local);
			return TimeZoneInfo.ConvertTime(serverTime, this.TimeZoneInfo, TimeZoneInfo.Local);
		}
		MarketShortDay getShortMarketDayForServerTime(DateTime dateTimeServer) {
			foreach (MarketShortDay shortDay in this.ShortDays) {
				if (shortDay.Date == dateTimeServer.Date) return shortDay;
			}
			return null;
		}
		bool isTradeableDayOfWeek(DateTime dateServer) {
			if (this.DaysOfWeekOpen == null) return true;
			if (this.DaysOfWeekOpen.Contains(dateServer.DayOfWeek)) return true;
			return false;
		}
		bool isHoliday(DateTime dateServer) {
			bool ret = false;
			if (this.HolidaysYMD000 == null) return ret;
			//if (this.HolidaysYMD000.Contains(dateServer.Date)) return true;
			foreach (DateTime holiday in this.HolidaysYMD000) {
				if (holiday.Date == dateServer.Date) {
					ret = true;
					break;
				}
			}
			return ret;
		}
		public bool IsMarketSuspendedForClearing(DateTime dateTimeServer, bool considerSuspendedIfFullBarIsWithinClearingTimespan_NYI = false) {
			MarketClearingTimespan suspendedNow = this.GetClearingTimespanIfMarketSuspended(dateTimeServer, considerSuspendedIfFullBarIsWithinClearingTimespan_NYI);
			return (suspendedNow != null) ? true : false;
		}
		public MarketClearingTimespan GetClearingTimespanIfMarketSuspended(DateTime dateTimeServer, bool considerSuspendedIfFullBarIsWithinClearingTimespan_NYI = false) {
			MarketClearingTimespan ret = null;
			if (this.ClearingTimespans == null) return ret;
			foreach (MarketClearingTimespan clearingTimespan in this.ClearingTimespans) {
				if (clearingTimespan.SuspendServerTimeOfDay == DateTime.MinValue) continue;
				if (clearingTimespan.ResumeServerTimeOfDay == DateTime.MinValue) continue;
				if (clearingTimespan.ClearingHappensOnDayOfWeek(dateTimeServer.DayOfWeek) == false) {
					ret = null;
					break;
				}
				bool afterSuspend = dateTimeServer.TimeOfDay >= clearingTimespan.SuspendServerTimeOfDay.TimeOfDay;
				bool beforeResume = dateTimeServer.TimeOfDay <  clearingTimespan.ResumeServerTimeOfDay.TimeOfDay;
				if (afterSuspend && beforeResume) {
					ret = clearingTimespan;
					break;
				}
			}
			return ret;
		}
		#region CANT_SET_DEFAULT_DATETIME_PARAMETER_TO_NULL_OR_MINIMALVALUE_SORRY_REDUNDANCY
		public bool IsMarketOpenDuringDateIntervalServerTime(DateTime dateTimeServerBarOpen, DateTime dateTimeServerBarClose) {
			if (this.IsTradeableDayServerTime(dateTimeServerBarOpen.Date) == false) return false;
			DateTime openTimeServer;
			DateTime closeTimeServer;
			this.GetRegularOrShortDayOpenCloseMarketTimeForServerDate(dateTimeServerBarOpen, out openTimeServer, out closeTimeServer);
			bool isMarketAfterOpening = dateTimeServerBarOpen.TimeOfDay >= openTimeServer.TimeOfDay;
			bool isMarketBeforeClosing = dateTimeServerBarOpen.TimeOfDay < closeTimeServer.TimeOfDay;
			bool isMarketSuspendedForClearing = this.IsMarketSuspendedForClearingDuringBar(dateTimeServerBarOpen, dateTimeServerBarClose);
			bool marketIsOpen = isMarketAfterOpening && isMarketBeforeClosing && (isMarketSuspendedForClearing == false);
			if (marketIsOpen == false) {
				string msg = "breakpoint";
			}
			return marketIsOpen;
		}
		public bool IsMarketSuspendedForClearingDuringBar(DateTime dateTimeServerBarOpen, DateTime dateTimeServerBarClose) {
			MarketClearingTimespan suspendedNow = this.GetSingleClearingTimespan_ifMarketSuspended_duringBar(dateTimeServerBarOpen, dateTimeServerBarClose);
			return (suspendedNow != null) ? true : false;
		}
		[Obsolete("ERRONEOUS_FOR_MULTIPLE_SHORT_CLEARINGS_DURING_LONG_BARS (when bar[4:00..4:30] and ClearingTimespans{[4:05..4:10],[4:15..4:20]}) combining multiple MarketTimeSpan to one is wrong solution, use two pinpointing methods from this class")]
		public MarketClearingTimespan GetSingleClearingTimespan_ifMarketSuspended_duringBar(DateTime dateTimeServerBarOpen, DateTime dateTimeServerBarClose) {
			MarketClearingTimespan ret = null;
			if (this.ClearingTimespans == null) return ret;
			foreach (MarketClearingTimespan clearingTimespan in this.ClearingTimespans) {
				if (clearingTimespan.SuspendServerTimeOfDay == DateTime.MinValue) continue;
				if (clearingTimespan.ResumeServerTimeOfDay == DateTime.MinValue) continue;
				if (clearingTimespan.ClearingHappensOnDayOfWeek(dateTimeServerBarOpen.DayOfWeek) == false) {
					ret = null;
					break;
				}
				bool afterSuspend = dateTimeServerBarOpen.TimeOfDay >= clearingTimespan.SuspendServerTimeOfDay.TimeOfDay;
				bool beforeResume = dateTimeServerBarClose.TimeOfDay <=  clearingTimespan.ResumeServerTimeOfDay.TimeOfDay;
				if (afterSuspend && beforeResume) {
					ret = clearingTimespan;
					break;
				}
			}
			return ret;
		}
		#endregion

		public DateTime getThisDayClose(Quote quote) {
			return new DateTime(quote.ServerTime.Date.Year, quote.ServerTime.Date.Month, quote.ServerTime.Date.Day,
				this.MarketCloseServerTime.Hour, this.MarketCloseServerTime.Minute, this.MarketCloseServerTime.Second);
		}
		public TimeSpan ClearingIntervalsStretchingWholeBarsTotalled(BarScaleInterval oneFullBarLasts) {
			TimeSpan ret = new TimeSpan();
			if (oneFullBarLasts == null) {
				string msg = "DONT_PASS_EMPTY_BarScaleInterval oneFullBarLasts[" + oneFullBarLasts + "]";
				Assembler.PopupException(msg);
				return ret;
			}
			if (oneFullBarLasts.Scale == BarScale.Unknown) {
				string msg = "DONT_PASS_UNKNOWN_BarScaleInterval oneFullBarLasts[" + oneFullBarLasts + "]";
				Assembler.PopupException(msg);
				return ret;
			}
			foreach (MarketClearingTimespan clearingTimespan in this.ClearingTimespans) {
				if (clearingTimespan.SuspendServerTimeOfDay == DateTime.MinValue) {
					string msg = "CLEARING_INTERVAL_SUSPEND_TIME_MUST_BE_INITIALIZED clearingTimespan["
						+ clearingTimespan.ToString() + "].SuspendServerTimeOfDay[" + clearingTimespan.SuspendServerTimeOfDay + "]";
					Assembler.PopupException(msg);
					continue;
				}
				if (clearingTimespan.ResumeServerTimeOfDay == DateTime.MinValue) {
					string msg = "CLEARING_INTERVAL_RESUME_TIME_MUST_BE_INITIALIZED clearingTimespan["
						+ clearingTimespan.ToString() + "].ResumeServerTimeOfDay[" + clearingTimespan.ResumeServerTimeOfDay + "]";
					Assembler.PopupException(msg);
					continue;
				}
				if (clearingTimespan.SuspendServerTimeOfDay.TimeOfDay <= clearingTimespan.ResumeServerTimeOfDay.TimeOfDay) {
					string msg = "CLEARING_INTERVAL_MUST_BE_POSITIVE clearingTimespan[" + clearingTimespan.ToString()
						+ "].ResumeServerTimeOfDay[" + clearingTimespan.ResumeServerTimeOfDay 
						+ "] MUST_BE_GREATER_THAN .SuspendServerTimeOfDay[" + clearingTimespan.SuspendServerTimeOfDay + "]";
					Assembler.PopupException(msg);
					continue;
				}

				// TODO: CAN_BE_WRONG_APPROACH_BUT_FILTERING_CLEARING_INTERVALS_STRETCHING_BEYOND_MARKETOPEN_MARKETCLOSE_ACTUALLY_WORKS_HERE
				DateTime suspendTime = clearingTimespan.SuspendServerTimeOfDay;
				if (suspendTime.TimeOfDay < this.MarketOpenServerTime.TimeOfDay) suspendTime = this.MarketOpenServerTime; 
				DateTime resumeTime = clearingTimespan.ResumeServerTimeOfDay;
				if (resumeTime.TimeOfDay < this.MarketCloseServerTime.TimeOfDay) resumeTime = this.MarketCloseServerTime;
				// TODO: CAN_BE_WRONG_APPROACH_BUT_FILTERING_CLEARING_INTERVALS_STRETCHING_BEYOND_MARKETOPEN_MARKETCLOSE_ACTUALLY_WORKS_HERE

				TimeSpan clearingInterval = resumeTime.TimeOfDay.Subtract(suspendTime.TimeOfDay);
				
				int fullBars = (int)(clearingInterval.Seconds / oneFullBarLasts.AsTimeSpanInSeconds);
				TimeSpan fullBarsAsTimeSpan = new TimeSpan(0, 0, fullBars * oneFullBarLasts.AsTimeSpanInSeconds);
				ret.Add(fullBarsAsTimeSpan);
			}
			return ret;
		}
		public DateTime GetClearingResumes(DateTime quoteTimeGuess) {
			DateTime ret = quoteTimeGuess;
			MarketClearingTimespan clearingNow = this.GetClearingTimespanIfMarketSuspended(quoteTimeGuess);
			if (clearingNow == null) return ret;
			ret = Bars.CombineBarDateWithMarketOpenTime(quoteTimeGuess, clearingNow.ResumeServerTimeOfDay);
			//string msg = "[" + this.Name + "]Market is CLEARING, resumes["
			//	+ ret.ToString("HH:mm") + "], +[" + (ret-quoteTimeGuess).TotalSeconds + "]sec for quoteTimeGuess[" + quoteTimeGuess + "]";
			//TESTED Debugger.Break();
			return ret;
		}
		[Obsolete("you pay too much for OFFSET; get DateTime adjusted instead! use GetClearingResumes()")]
		public TimeSpan GetClearingResumesOffsetOrZeroSeconds(DateTime assumed) {
			TimeSpan ret = new TimeSpan(0);
			MarketClearingTimespan clearingNow = this.GetClearingTimespanIfMarketSuspended(assumed);
			if (clearingNow == null) return ret;
			DateTime clearingEndsDateTime = Bars.CombineBarDateWithMarketOpenTime(assumed, clearingNow.ResumeServerTimeOfDay);
			ret = clearingEndsDateTime.Subtract(assumed);
			//if (ret.TotalSeconds > 0) {
			//	string msg = "[" + this.Name + "]Market is CLEARING, resumes["
			//		+ clearingEndsDateTime.ToString("HH:mm") + "], +[" + ret.TotalSeconds + "]sec for [" + assumed + "]";
			//TESTED	Debugger.Break();
			//}
			return ret;
		}
	}
}
