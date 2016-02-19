using System;

using Newtonsoft.Json;

namespace Sq1.Core.DataTypes {
	public partial class Bars {
		[JsonIgnore]	public Bar BarStaticFirst_nullUnsafe { get {
			Bar last = base.BarFirst;
			if (last == null) return null; 
			if (last != this.BarStreaming_nullUnsafe) return last;
			return null;
			//throw new Exception("Bars.BarLast point to Bars.StreamingBar???");
		} }
		[JsonIgnore]	public Bar BarStaticLast_nullUnsafe { get {
			Bar last = base.BarLast;
			if (last == null) return null; 
			if (last != this.BarStreaming_nullUnsafe) return last;
			Bar preLast = base.BarPreLast;
			if (preLast == null) return null;
			if (preLast != this.BarStreaming_nullUnsafe) return preLast;
			//return null;
			throw new Exception("both Bars.BarLast and Bars.BarPreLast point to Bars.StreamingBar???");
		} }
		public Quote LastQuoteClone_nullUnsafe { get {
			Quote ret = null;
			if (this.DataSource == null) return ret;
			if (this.DataSource.StreamingAdapter == null) return ret;
			if (this.DataSource.StreamingAdapter.StreamingDataSnapshot == null) return ret;
			ret = this.DataSource.StreamingAdapter.StreamingDataSnapshot.LastQuoteClone_getForSymbol(this.Symbol);
			return ret;
		} }

//		public Bar BarMarketClosedTodayScanBackward(Bar barLastToday) {
//			Bar ret = null;
//			if (barLastToday == null) return ret;
//			if (this.MarketInfo == null) return ret;
//			#if DEBUG // TEST_EMBEDDED
//			if (barLastToday.IsAlignedToOwnScaleInterval == false) {
//				Debugger.Break();
//				return null;
//			}
//			#endif
//			DateTime dateExisting = this.DateTimeMarketWasClosedTodayScanBackwardMinValueUnsafe(barLastToday.DateTimeOpen);
//			if (dateExisting == DateTime.MinValue) {
//				string msg = "YOU_SHOULD_SCAN_FORWARD_MARKET_CLOSES_LATER OR BAR_NOT_APPENDED OR MARKETINFO_NULL OR MARKETCLOSESERVERTIME_NULL barLastToday["
//					+ barLastToday + "].ParentBars.MarketInfo[" + this.MarketInfo + "]";
//				Assembler.PopupException(msg);
//				#if DEBUG
//				Debugger.Break();
//				#endif
//				return ret;
//			}
//			ret = this[dateExisting];
//			return ret;
//		}
//		public DateTime DateTimeMarketWasClosedTodayScanBackwardMinValueUnsafe(DateTime dateStartScanBackwards) {
//			DateTime ret = DateTime.MinValue;
//			if (this.ContainsKey(dateStartScanBackwards) == false) {
//				string msg = "CANT_START_SCANNING_MUST_EXIST_IN_THIS_BARS dateStartScanBackwards[" + dateStartScanBackwards + "]"
//					+ " 1) applicable to bars APPENDED to THIS_BARS;"
//					+ " 2) use bar.IsAlignedToOwnScaleInterval before passing it to me;"
//					+ " 3) use Bar/DataSeriesTimeBased.roundDateDownToMyInterval() before passing it to me;"; 
//				Assembler.PopupException(msg);
//				#if DEBUG
//				Debugger.Break();
//				#endif
//				return ret;
//			}
//			if (this.MarketInfo == null) {
//				return ret;
//			}
//			// TODO: use Weekends, year-dependent irregular Holidays
//			// TODO: this calculation is irrelevant for FOREX since FOREX doesn't interrupt overnight 
//			DateTime marketCloseServerTime = this.MarketInfo.MarketCloseServerTime;
//			if (marketCloseServerTime == DateTime.MinValue) {
//				return ret;
//			}
//			DateTime todayMarketCloseServerTime = this.CombineBarDateWithMarketOpenTime(dateStartScanBackwards, marketCloseServerTime);
//			if (dateStartScanBackwards < todayMarketCloseServerTime) {
//				string msg = "BAR_MISSING_MARKET_IS_ALREADY_CLOSED_RETURNING_ startScanFrom[" + dateStartScanBackwards 
//					+ "] MarketInfo.MarketCloseServerTime[" + this.MarketInfo.MarketCloseServerTime
//					+ "] todayMarketCloseServerTime[" + todayMarketCloseServerTime + "]";
//				Assembler.PopupException(msg);
//				#if DEBUG
//				Debugger.Break();
//				#endif
//				return todayMarketCloseServerTime;
//			}
//			//FIRST_BAR_WILL_BECOME_ZERO ret = 0;
//			TimeSpan distanceBetweenBars = this.ScaleInterval.AsTimeSpan;
//			int barsMaxDayCanFit = this.BarsMaxOneDayCanFit;
//			int barsScanned = 0;
//			for (ret = dateStartScanBackwards; ret > todayMarketCloseServerTime; ret = ret.Subtract(distanceBetweenBars)) {
//				if (barsScanned > barsMaxDayCanFit) {
//					#if DEBUG	// TEST_EMBEDDED
//					Debugger.Break();
//					#endif
//					break;
//				}
//				barsScanned++;
//				
//				// I won't cry if we haven't received a bar between lastBarToday and an earlier barMarketClose
//				if (this.ContainsKey(ret) == false) continue;
//				#if DEBUG	// TEST_EMBEDDED
//				if (this.IsMarketOpenDuringWholeBar(this[ret]) == false) Debugger.Break();
//				#endif
//			}
//			// FOR ended when ret <= todayMarketCloseServerTime, ret=dateOfExistingBarPriorOrEqualToMarketClose
//			return ret;
//		}
//		public int DistanceInBarsWalkBackwardBasedOnMarketInfoExcludingClearing(DateTime dateStartScanBackwards, DateTime todayMarketCloseServerTime) {
//			TimeSpan distanceBetweenBars = this.ScaleInterval.AsTimeSpan;
//			int barsMaxDayCanFit = this.BarsMaxOneDayCanFit;
//			int barsScanned = 0;
//			for (DateTime eachBar = dateStartScanBackwards; eachBar > todayMarketCloseServerTime; eachBar = eachBar.Subtract(distanceBetweenBars)) {
//				if (barsScanned > barsMaxDayCanFit) {
//					#if DEBUG	// TEST_EMBEDDED
//					Debugger.Break();
//					#endif
//					break;
//				}
//				barsScanned++;
//				
//				// I won't cry if we haven't received a bar between lastBarToday and an earlier barMarketClose
//				//if (this.ContainsKey(eachBar) == false) continue;
//				//if (this.IsMarketOpenDuringWholeBar(this[eachBar]) == false) Debugger.Break();
//				if (this.MarketInfo.IsMarketOpenDuringDateIntervalServerTime(eachBar, eachBar.Add(distanceBetweenBars)) == false) continue;
//			}
//			// FOR ended when ret <= todayMarketCloseServerTime, ret=dateOfExistingBarPriorOrEqualToMarketClose
//			return barsScanned;
//		}
//		public bool IsMarketOpenDuringWholeBar(Bar bar) {
//			return this.MarketInfo.IsMarketOpenDuringDateIntervalServerTime(bar.DateTimeOpen, bar.DateTimeNextBarOpenUnconditional);
//		}
		public Bar BarMarketOpenedTodayScanBackward(Bar startScanFrom) {
			Bar ret = null;
			DateTime dateFound = this.DateTimeBeforeDayDecreasedScanBackward(startScanFrom.DateTimeOpen);
			ret = base[dateFound];
			return ret;
		}
		public DateTime DateTimeBeforeDayDecreasedScanBackward(DateTime startScanFrom, bool scanForEarlier = false) {
			DateTime ret = startScanFrom;
			if (base.ContainsDate(startScanFrom) == false) {
				//THIS_ISNT_EMBEDDED_TEST_BUT_YOU_EXPLAIN_TO_USER_HOW_TO_USE_THIS_METHOD
				string msg = "BARS_DOEST_CONTAIN_DATEOPEN[" + startScanFrom + "]_RETURNING_UNMODIFIED Bars[" + this.ToString() + "]"
					+ "; EXPECTING_BARS_WERE_APPENDED_WITHOUT_SCALEINTERVAL_CHANGE"
					+ "; WILL_NOT_RETURN_EARLIER_DATE: implement scanForEarlier if you need to pass ScaleInterval-unaligned or non-existing Date to find an existing one";
				Assembler.PopupException(msg);
				return ret;
			}
			int indexToStartScanningBackwards = base.IndexOfDate(startScanFrom);
			for (int i = indexToStartScanningBackwards; i >= 0; i--) {
				Bar eachBarBackwards = this[i];
				// stop scanning when we hit yesterday; then in RET we'll get lastKnownSameDayBar
				if (eachBarBackwards.DateTimeOpen.Day	< startScanFrom.Day) break;
				if (eachBarBackwards.DateTimeOpen.Month	< startScanFrom.Month) break;
				if (eachBarBackwards.DateTimeOpen.Year	< startScanFrom.Year) break;
				ret = eachBarBackwards.DateTimeOpen;
			}
			return ret;
		}
//		public Bar BarMarketClosedTodayScanForwardIgnoringMarketInfo(Bar startScanFrom) {
//			Bar ret = startScanFrom;
//			if (this.ContainsKey(startScanFrom.DateTimeOpen) == false) {
//				#if DEBUG
//				string msg = "BARS_DOEST_CONTAIN_THE_DATEOPEN_OF_ITSOWN_BAR_ADDING_SHOULDVE_DONE_A_BETTER_JOB_AND_THROW_OR_ROUND";
//				Assembler.PopupException(msg);
//				#endif
//			}
//			int indexToStartScanningForward = this.IndexOfKey(startScanFrom.DateTimeOpen);
//			for (int i = indexToStartScanningForward; i >= 0; i--) {
//				Bar eachBarBackwards = this[i];
//				// stop scanning when we hit yesterday; then in RET we'll get lastKnownSameDayBar
//				if (eachBarBackwards.DateTimeOpen.Day	< startScanFrom.DateTimeOpen.Day) break;
//				if (eachBarBackwards.DateTimeOpen.Month	< startScanFrom.DateTimeOpen.Month) break;
//				if (eachBarBackwards.DateTimeOpen.Year	< startScanFrom.DateTimeOpen.Year) break;
//				ret = eachBarBackwards;
//			}
//			return ret;
//		}
//		[Obsolete("WARNING_NOT_YET_IMPLEMENTED")]
//		public int SuggestBarIndexExpectedMarketClosesToday(Bar startScanFrom) {
//			return -1;
//		}
		[JsonIgnore]	public int BarsMaxOneDayCanFit { get {
				int ret = 0;
				TimeSpan wholeDay = new TimeSpan(24, 0, 0);
				ret = (int) (wholeDay.TotalSeconds / this.ScaleInterval.AsTimeSpan.TotalSeconds);
				return ret;
			} }
		public int BarIndexSinceTodayMarketOpenSuggestForwardFor(DateTime dateTimeToFind) {
			string msig = " //BarIndexSinceTodayMarketOpenSuggestForwardFor(" + dateTimeToFind + ") USEFUL_ONLY_IN_SCRIPT";
			int ret = -1;
			if (this.MarketInfo == null) return ret;
			// v1: MarketInfo.MarketOpenServerTime may contain only Hour:Minute:Second and all the rest is 01-Jan-01 => use than ReplaceTimeOpenWith() 
			//DateTime marketOpenServerTime = this.ParentBars.MarketInfo.MarketOpenServerTime;
			//if (marketOpenServerTime == DateTime.MinValue) return ret;
			//DateTime todayMarketOpenServerTime = marketOpenServerTime;
			//todayMarketOpenServerTime.AddYears(this.DateTimeOpen.Year);
			//todayMarketOpenServerTime.AddMonths(this.DateTimeOpen.Month);
			//todayMarketOpenServerTime.AddDays(this.DateTimeOpen.Day);
			//v2
			// TODO: use Weekends, year-dependent irregular Holidays
			// TODO: this calculation is irrelevant for FOREX since FOREX doesn't interrupt overnight
			DateTime marketOpenServerTime = this.MarketInfo.MarketOpenServerTime;
			if (marketOpenServerTime == DateTime.MinValue) return ret;
			DateTime todayMarketOpenServerTime = Bars.CombineBarDateWithMarketOpenTime(dateTimeToFind, marketOpenServerTime);
			if (dateTimeToFind < todayMarketOpenServerTime) {
				string msg = "BAR_INVALID_MARKET_IS_NOT_OPEN_YET bar.DateTimeOpen[" + dateTimeToFind + 
					"] while MarketInfo.MarketOpenServerTime[" + marketOpenServerTime + "]";
				Assembler.PopupException(msg + msig);
				return ret;
			}
			//FIRST_BAR_WILL_BECOME_ZERO ret = 0;
			TimeSpan distanceBetweenBars = this.ScaleInterval.AsTimeSpan;
			int barsMaxDayCanFit = this.BarsMaxOneDayCanFit;
			for (DateTime forwardFromMarketOpenToCurrentBar = todayMarketOpenServerTime;
				 		  forwardFromMarketOpenToCurrentBar <= dateTimeToFind;
				 		  forwardFromMarketOpenToCurrentBar = forwardFromMarketOpenToCurrentBar.Add(distanceBetweenBars)) {
				if (ret > barsMaxDayCanFit) {
					string msg = "BAR_INDEX_BEYOND_DAY_CAN_FIT ret[" + ret + "] > barsMaxDayCanFit[" + barsMaxDayCanFit + "]";
					Assembler.PopupException(msg + msig);
					return ret;
				}
				DateTime nextBarWillOpen = forwardFromMarketOpenToCurrentBar.Add(distanceBetweenBars);
				if (this.MarketInfo.IsMarketOpenDuringDateIntervalServerTime(forwardFromMarketOpenToCurrentBar, nextBarWillOpen) == false) continue;
				ret++;	//FIRST_BAR_WILL_BECOME_ZERO
			}
			return ret;
		}
		public int BarIndexExpectedMarketClosesTodaySinceMarketOpenSuggestBackwardForDateLaterOrEqual(DateTime dateTimeToFind) {
			string msig = " //BarIndexExpectedMarketClosesTodaySinceMarketOpenSuggestBackwardForDateLaterOrEqual(" + dateTimeToFind + ") USEFUL_ONLY_IN_SCRIPT";
			int ret = -1;
			if (this.MarketInfo == null) return ret;
			// TODO: use Weekends, year-dependent irregular Holidays
			// TODO: this calculation is irrelevant for FOREX since FOREX doesn't interrupt overnight 
			DateTime marketCloseServerTime = this.MarketInfo.MarketCloseServerTime;
			if (marketCloseServerTime == DateTime.MinValue) return ret;
			DateTime todayMarketCloseServerTime = Bars.CombineBarDateWithMarketOpenTime(dateTimeToFind, marketCloseServerTime);
			if (dateTimeToFind > todayMarketCloseServerTime) {
				string msg = "BAR_INVALID_MARKET_IS_ALREADY_CLOSED bar.DateTimeOpen[" + dateTimeToFind + 
					"] while MarketInfo.MarketCloseServerTime[" + this.MarketInfo.MarketCloseServerTime + "]";
				Assembler.PopupException(msg + msig);
				return ret;
			}
			//FIRST_BAR_WILL_BECOME_ZERO ret = 0;
			TimeSpan distanceBetweenBars = this.ScaleInterval.AsTimeSpan;
			int barsMaxDayCanFit = this.BarsMaxOneDayCanFit;
			for (DateTime backFromDayCloseToCurrentBar = todayMarketCloseServerTime;
				 		  backFromDayCloseToCurrentBar > dateTimeToFind;
				 		  backFromDayCloseToCurrentBar = backFromDayCloseToCurrentBar.Subtract(distanceBetweenBars)) {
				if (ret > barsMaxDayCanFit) {
					string msg = "BAR_INDEX_BEYOND_DAY_CAN_FIT ret[" + ret + "] > barsMaxDayCanFit[" + barsMaxDayCanFit + "]";
					Assembler.PopupException(msg + msig);
					return ret;
				}
				DateTime thisBarWillOpen = backFromDayCloseToCurrentBar.Subtract(distanceBetweenBars);
				if (this.MarketInfo.IsMarketOpenDuringDateIntervalServerTime(thisBarWillOpen, backFromDayCloseToCurrentBar) == false) continue;
				ret++;	//FIRST_BAR_WILL_BECOME_ZERO
			}
			return ret;
		}
		public static DateTime CombineBarDateWithMarketOpenTime(DateTime barDateTimeOpen, DateTime marketOpenCloseIntradayTime) {
			DateTime ret = new DateTime(barDateTimeOpen.Year, barDateTimeOpen.Month, barDateTimeOpen.Day,
				marketOpenCloseIntradayTime.Hour, marketOpenCloseIntradayTime.Minute, marketOpenCloseIntradayTime.Second);
			return ret;
		}
		
//		public DateTime AddBarIntervalsToDate(DateTime dateTimeToAddIntervalsTo, int howManyBarsToAdd) {
//			if (dateTimeToAddIntervalsTo == DateTime.MinValue) return DateTime.MinValue;
//			switch (this.ScaleInterval.Scale) {
//				case BarScale.Tick:
//					throw new ArgumentException("Tick scale is not supported");
//				case BarScale.Second:
//				case BarScale.Minute:
//				case BarScale.Hour:
//				case BarScale.Daily:
//				case BarScale.Weekly:
//				case BarScale.Monthly:
//				case BarScale.Quarterly:
//				case BarScale.Yearly:
//					break;
//				default:
//					throw new Exception("this.ScaleInterval.Scale[" + this.ScaleInterval.Scale
//						+ "] is not supported");
//			}
//			//if (howManyBarsToAdd < 0) {
//			//TESTED:1yes2yes 	string msg = "CHECKING_1)_TimeSpan_CAN_BE_NEGATIVE_2)_DATE.ADD(NEGATIVE)_SHOULD_BE_REWRITTEN";
//			//	Debugger.Break();
//			//}
//			TimeSpan totalTimeSpan = new TimeSpan(this.ScaleInterval.AsTimeSpan.Ticks * howManyBarsToAdd);
//			DateTime ret = dateTimeToAddIntervalsTo.Add(totalTimeSpan);
//			return ret;
//		}
//		[JsonIgnore]	public int BarsDuringMarketOpenExpectedIncludingClearingIntervals { get {
//				int ret = -1;
//				if (this.MarketInfo == null) return ret;
//				if (this.ScaleInterval.Scale == BarScale.Unknown) {
//					#if DEBUG
//					Debugger.Break();
//					#endif
//					return ret;
//				}
//				int seconds = this.ScaleInterval.AsTimeSpanInSeconds;
//				if (seconds <= 0) {
//					#if DEBUG
//					Debugger.Break();
//					#endif
//					return ret;
//				}
//				TimeSpan duration = this.MarketInfo.MarketCloseServerTime.TimeOfDay.Subtract(this.MarketInfo.MarketOpenServerTime.TimeOfDay);
//				int marketOpenDurationSeconds = (int) duration.TotalSeconds;
//				#if DEBUG		//quickCheck
//				if (this.ScaleInterval == new BarScaleInterval(BarScale.Minute, 5)) {
//					string timeOpen = this.MarketInfo.MarketOpenServerTime.ToString("HH:mm");
//					string timeClose = this.MarketInfo.MarketCloseServerTime.ToString("HH:mm"); 
//					if (timeOpen == "10:00" && timeClose == "23:50") {
//						int secondsInFortsSession = (24-10) * 60 * 60;	// 10...24 in seconds
//						secondsInFortsSession -= 10 * 60;				// subtract 10 minutes from 23:50
//						if (marketOpenDurationSeconds != secondsInFortsSession) {
//							Debugger.Break();
//						}
//					}
//				}
//				#endif
//				ret = marketOpenDurationSeconds / seconds;
//				return ret;
//			} }
//		[JsonIgnore]	public TimeSpan ClearingIntervalsStretchingWholeBarsTotalled { get {
//				TimeSpan ret = new TimeSpan();
//				if (this.MarketInfo == null) return ret;
//				if (this.ScaleInterval == null) {
//					string msg = "CAN_NOT_CALCULATE_ClearingIntervalsStretchingWholeBarsTotalled_FOR_BARS_WITH_EMPTY_BarScaleInterval this.ScaleInterval[" + this.ScaleInterval + "]";
//					Assembler.PopupException(msg);
//					#if DEBUG
//					Debugger.Break();
//					#endif
//					return ret;
//				}
//				if (this.ScaleInterval.Scale == BarScale.Unknown) {
//					string msg = "CAN_NOT_CALCULATE_ClearingIntervalsStretchingWholeBarsTotalled_FOR_BARS_WITH_UNKNOWN_BarScaleInterval this.ScaleInterval[" + this.ScaleInterval + "]";
//					Assembler.PopupException(msg);
//					#if DEBUG
//					Debugger.Break();
//					#endif
//					return ret;
//				}
//				ret = this.MarketInfo.ClearingIntervalsStretchingWholeBarsTotalled(this.ScaleInterval);
//				return ret;
//			} }
	}
}
