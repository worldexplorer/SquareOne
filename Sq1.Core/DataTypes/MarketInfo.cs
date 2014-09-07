using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sq1.Core.DataTypes {
	[DataContract]
	public class MarketInfo {
		[DataMember] public string Name;
		[DataMember] public string Description;
		[DataMember] public DateTime MarketOpenServerTime;
					 public string MarketOpenServerTimeAsString { get { return MarketCloseServerTime.ToString("HH:mm"); } }
		[DataMember] public DateTime MarketCloseServerTime;
					 public string MarketCloseServerTimeAsString { get { return MarketOpenServerTime.ToString("HH:mm"); } }
		[DataMember] public List<DayOfWeek> DaysOfWeekOpen;
		[DataMember] public string TimeZoneName;
		public TimeZoneInfo TimeZoneInfo { get {
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
		[DataMember] public List<MarketClearingTimespan> ClearingTimespans;
		[DataMember] public List<DateTime> HolidaysYMD000;
		[DataMember] public List<MarketShortDay> ShortDays;

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
		public DateTime ServerTimeNow { get { return this.ConvertLocalTimeToServer(DateTime.Now); } }
		public bool TodayIsTradingDay { get { return this.IsTradeableDayServerTime(this.ServerTimeNow); } }
		public bool IsMarketOpenNow { get { return this.IsMarketOpenAtServerTime(this.ServerTimeNow); } }
		public bool MarketIsAfterCloseNow { get { return this.isMarketAfterCloseServerTime(this.ServerTimeNow); } }
		public DateTime MarketCloseLocalTime { get { return this.ConvertServerTimeToLocal(this.MarketCloseServerTime); } }
		public DateTime LastTradingSessionEndedServerTime { get {
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
			bool isMarketSuspendedForClearing = this.isMarketSuspendedForClearing(dateTimeServer);
			return isMarketAfterOpening && isMarketBeforeClosing && (isMarketSuspendedForClearing == false);
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

					clearingTimespan = getClearingTimespanIfMarketSuspended(ret);
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

					clearingTimespan = getClearingTimespanIfMarketSuspended(ret);
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

					clearingTimespan = getClearingTimespanIfMarketSuspended(ret);
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
		bool isMarketSuspendedForClearing(DateTime dateTimeServer) {
			MarketClearingTimespan suspendedNow = getClearingTimespanIfMarketSuspended(dateTimeServer);
			return (suspendedNow != null) ? true : false;
		}
		MarketClearingTimespan getClearingTimespanIfMarketSuspended(DateTime dateTimeServer) {
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

		public DateTime getThisDayClose(Quote quote) {
			return new DateTime(quote.ServerTime.Date.Year, quote.ServerTime.Date.Month, quote.ServerTime.Date.Day,
				this.MarketCloseServerTime.Hour, this.MarketCloseServerTime.Minute, this.MarketCloseServerTime.Second);
		}
	}
}
