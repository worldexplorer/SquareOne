using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Sq1.Core.DataTypes {
	public partial class Bar {
		[JsonIgnore] public bool IsBarStreaming { get {
				if (this.HasParentBars == false) {
					throw new Exception("PROPERTY_VALID_ONLY_WHEN_THIS_BAR_IS_ADDED_INTO_BARS: IsStreamingBar: Bar[" + this + "].HasParentBars=false");
				}
				return this == this.ParentBars.BarStreaming;
			} }
		[JsonIgnore] public bool IsBarStaticLast { get {
				if (this.HasParentBars == false) {
					throw new Exception("PROPERTY_VALID_ONLY_WHEN_THIS_BAR_IS_ADDED_INTO_BARS: IsLastStaticBar: Bar[" + this + "].HasParentBars=false");
				}
				return this == this.ParentBars.BarStaticLast;
			} }
		[JsonIgnore] public bool IsBarStaticFirst { get {
				if (this.HasParentBars == false) {
					throw new Exception("PROPERTY_VALID_ONLY_WHEN_THIS_BAR_IS_ADDED_INTO_BARS: IsFirstStaticBar: Bar[" + this + "].HasParentBars=false");
				}
				return this == this.ParentBars.BarStaticFirst;
			} }
		[JsonIgnore] public Bar BarPrevious { get {
				if (this.HasParentBars == false) {
					throw new Exception("PROPERTY_VALID_ONLY_WHEN_THIS_BAR_IS_ADDED_INTO_BARS: BarPrevious: Bar[" + this + "].HasParentBars=false");
				}
				if (this.ParentBarsIndex == 0) {
					return null;
				}
				return this.ParentBars[this.ParentBarsIndex - 1];
			} }
		[JsonIgnore] public Bar BarNext { get {
				if (this.HasParentBars == false) {
					throw new Exception("PROPERTY_VALID_ONLY_WHEN_THIS_BAR_IS_ADDED_INTO_BARS: BarNext: Bar[" + this + "].HasParentBars=false");
				}
				if (this.ParentBarsIndex == this.ParentBars.Count - 1) {
					return null;
				}
				return this.ParentBars[this.ParentBarsIndex + 1];
			} }
		[JsonIgnore] public Bar BarFirstForCurrentTradingDay { get {
				return this.ParentBars.ScanBackwardsFindBarFirstForCurrentTradingDay(this);
			} }
		[JsonIgnore] public int BarIndexAfterMidnightReceived { get {
				return this.ParentBarsIndex - this.BarFirstForCurrentTradingDay.ParentBarsIndex;
			} }
		[JsonIgnore] public int BarIndexAfterMarketOpenExpected { get {
				int ret = -1;
				if (this.HasParentBars == false) return ret;
				if (this.ParentBars.MarketInfo == null) return ret;
				// v1: MarketInfo.MarketOpenServerTime may contain only Hour:Minute:Second and all the rest is 01-Jan-01 => use than ReplaceTimeOpenWith() 
				//DateTime marketOpenServerTime = this.ParentBars.MarketInfo.MarketOpenServerTime;
				//if (marketOpenServerTime == DateTime.MinValue) return ret;
				//DateTime todayMarketOpenServerTime = marketOpenServerTime;
				//todayMarketOpenServerTime.AddYears(this.DateTimeOpen.Year);
				//todayMarketOpenServerTime.AddMonths(this.DateTimeOpen.Month);
				//todayMarketOpenServerTime.AddDays(this.DateTimeOpen.Day);
				//v2
				// TODO: use Weekends, year-dependent irregular Holidays and MarketClosedForClearing hours
				// TODO: this calculation is irrelevant for FOREX since it doesn't interrupt overnight
				MarketInfo marketInfo = this.ParentBars.MarketInfo;
				DateTime marketOpenServerTime = marketInfo.MarketOpenServerTime;
				if (marketOpenServerTime == DateTime.MinValue) return ret;
				DateTime todayMarketOpenServerTime = this.ReplaceTimeOpenWith(marketOpenServerTime);
				if (this.DateTimeOpen < todayMarketOpenServerTime) {
					string msg = "BAR_INVALID_MARKET_IS_NOT_OPEN_YET bar.DateTimeOpen[" + this.DateTimeOpen + 
						"] while MarketInfo.MarketOpenServerTime[" + this.ParentBars.MarketInfo.MarketOpenServerTime + "]";
					Assembler.PopupException(msg);
					#if DEBUG
					Debugger.Break();
					#endif
					return ret;
				}
				//FIRST_BAR_WILL_BECOME_ZERO ret = 0;
				TimeSpan distanceBetweenBars = this.ParentBars.ScaleInterval.AsTimeSpan;
				int barsMaxDayCanFit = this.ParentBars.BarsMaxDayCanFit;
				for (DateTime forwardFromMarketOpenToCurrentBar = todayMarketOpenServerTime;
				     		  forwardFromMarketOpenToCurrentBar <= this.DateTimeOpen;
				     		  forwardFromMarketOpenToCurrentBar = forwardFromMarketOpenToCurrentBar.Add(distanceBetweenBars)) {
					if (ret > barsMaxDayCanFit) {
						#if DEBUG
						Debugger.Break();
						#endif
						return ret;
					}
					DateTime nextBarWillOpen = forwardFromMarketOpenToCurrentBar.Add(distanceBetweenBars);
					if (marketInfo.IsMarketOpenDuringDateIntervalServerTime(forwardFromMarketOpenToCurrentBar, nextBarWillOpen) == false) continue;
					ret++;	//FIRST_BAR_WILL_BECOME_ZERO
				}
				return ret;
			} }
		[JsonIgnore] public int BarIndexBeforeMarketCloseExpected { get {
				int ret = -1;
				if (this.HasParentBars == false) return ret;
				if (this.ParentBars.MarketInfo == null) return ret;
				// TODO: use Weekends, year-dependent irregular Holidays and MarketClosedForClearing hours
				// TODO: this calculation is irrelevant for FOREX since it doesn't interrupt overnight 
				MarketInfo marketInfo = this.ParentBars.MarketInfo;
				DateTime marketCloseServerTime = marketInfo.MarketCloseServerTime;
				if (marketCloseServerTime == DateTime.MinValue) return ret;
				DateTime todayMarketCloseServerTime = this.ReplaceTimeOpenWith(marketCloseServerTime);
				if (this.DateTimeOpen > todayMarketCloseServerTime) {
					string msg = "BAR_INVALID_MARKET_IS_ALREADY_CLOSED bar.DateTimeOpen[" + this.DateTimeOpen + 
						"] while MarketInfo.MarketCloseServerTime[" + this.ParentBars.MarketInfo.MarketCloseServerTime + "]";
					Assembler.PopupException(msg);
					#if DEBUG
					Debugger.Break();
					#endif
					return ret;
				}
				//FIRST_BAR_WILL_BECOME_ZERO ret = 0;
				TimeSpan distanceBetweenBars = this.ParentBars.ScaleInterval.AsTimeSpan;
				int barsMaxDayCanFit = this.ParentBars.BarsMaxDayCanFit;
				for (DateTime backFromDayCloseToCurrentBar = todayMarketCloseServerTime;
				     		  backFromDayCloseToCurrentBar > this.DateTimeOpen;
				     		  backFromDayCloseToCurrentBar = backFromDayCloseToCurrentBar.Subtract(distanceBetweenBars)) {
					if (ret > barsMaxDayCanFit) {
						#if DEBUG
						Debugger.Break();
						#endif
						return ret;
					}
					DateTime thisBarWillOpen = backFromDayCloseToCurrentBar.Subtract(distanceBetweenBars);
					if (marketInfo.IsMarketOpenDuringDateIntervalServerTime(thisBarWillOpen, backFromDayCloseToCurrentBar) == false) continue;
					ret++;	//FIRST_BAR_WILL_BECOME_ZERO
				}
				return ret;
			} }
		public DateTime ReplaceTimeOpenWith(DateTime marketOpenCloseIntradayTime) {
			DateTime ret = new DateTime(this.DateTimeOpen.Year, this.DateTimeOpen.Month, this.DateTimeOpen.Day,
				marketOpenCloseIntradayTime.Hour, marketOpenCloseIntradayTime.Minute, marketOpenCloseIntradayTime.Second);
			return ret;
		}
	}
}
