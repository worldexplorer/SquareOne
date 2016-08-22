using System;

using Newtonsoft.Json;

namespace Sq1.Core.DataTypes {
	public partial class Bar {
		#region long story short
		[JsonIgnore]			SymbolInfo	symbolInfo_nullUnsafe					{ get {
			SymbolInfo ret = this.symbolInfo;
			if (ret == null) ret = this.symbolInfo_fromParentBars_nullUnsafe;
			if (ret == null) {
				ret = Assembler.InstanceInitialized.RepositorySymbolInfos.FindSymbolInfo_nullUnsafe(this.Symbol);
			}
			return ret;
		} }
		[JsonIgnore]			SymbolInfo	symbolInfo_fromParentBars_nullUnsafe	{ get {
			SymbolInfo ret = null;
			if (this.ParentBars				== null) return ret;
			if (this.ParentBars.SymbolInfo	== null) return ret;
			ret = this.ParentBars.SymbolInfo;
			return ret;
		} }

		[JsonIgnore]	protected	string		PriceFormat	{ get {
			SymbolInfo symbolInfo = this.symbolInfo_nullUnsafe;
			return symbolInfo != null ? symbolInfo.PriceFormat : "N2";
		} }
		[JsonIgnore]	protected	string		VolumeFormat	{ get {
			SymbolInfo symbolInfo = this.symbolInfo_nullUnsafe;
			return symbolInfo != null ? symbolInfo.PriceFormat : "N0";
		} }
		[JsonIgnore]	protected	string		DateTimeFormat	{ get {
			string ret = Assembler.DateTimeFormat_toSeconds_noYear;	// less than a minute
			switch (this.ScaleInterval.Scale) {
				case BarScale.Unknown:
				case BarScale.Tick:
				case BarScale.Second:		ret = Assembler.DateTimeFormat_toSeconds_noYear; break;

				case BarScale.Minute:
				case BarScale.Hour:			ret = Assembler.DateTimeFormat_toSeconds_noYear; break;
				
				case BarScale.Daily:
				case BarScale.Weekly:
				case BarScale.Monthly:
				case BarScale.Quarterly:
				case BarScale.Yearly:		ret = Assembler.DateTimeFormat_toDays; break;

				default: 
					string msg = "YOU_ADDED_NEW_TIMEFRAME_BUT_DIDNT_DEFINE_DATETIME_FORMAT_FOR_IT";
					Assembler.PopupException(msg);
					ret = Assembler.DateTimeFormat_toSeconds_noYear;
					break;
			}
			return ret;
		} }
		#endregion

		[JsonProperty]	public	string	DateTimeOpen_formatted	{ get { return string.Format("{0:" + this.DateTimeFormat + "}", this.DateTimeOpen); } }
		[JsonProperty]	public	string	Open_formatted			{ get { return string.Format("{0:" + this.PriceFormat + "}", this.Open); } }
		[JsonProperty]	public	string	High_formatted			{ get { return string.Format("{0:" + this.PriceFormat + "}", this.High); } }
		[JsonProperty]	public	string	Low_formatted			{ get { return string.Format("{0:" + this.PriceFormat + "}", this.Low); } }
		[JsonProperty]	public	string	Close_formatted			{ get { return string.Format("{0:" + this.PriceFormat + "}", this.Close); } }
		[JsonProperty]	public	string	Volume_formatted		{ get { return string.Format("{0:" + this.VolumeFormat + "}", this.Volume); } }

	
		[JsonIgnore]	public bool IsBarStreaming { get {
				if (this.HasParentBars == false) {
					throw new Exception("PROPERTY_VALID_ONLY_WHEN_THIS_BAR_IS_ADDED_INTO_BARS: IsStreamingBar: Bar[" + this + "].HasParentBars=false");
				}
				return this == this.ParentBars.BarStreaming_nullUnsafe;
			} }
		[JsonIgnore]	public bool IsBarStaticLast { get {
				if (this.HasParentBars == false) {
					throw new Exception("PROPERTY_VALID_ONLY_WHEN_THIS_BAR_IS_ADDED_INTO_BARS: IsLastStaticBar: Bar[" + this + "].HasParentBars=false");
				}
				return this == this.ParentBars.BarStaticLast_nullUnsafe;
			} }
		[JsonIgnore]	public bool IsBarStaticFirst { get {
				if (this.HasParentBars == false) {
					throw new Exception("PROPERTY_VALID_ONLY_WHEN_THIS_BAR_IS_ADDED_INTO_BARS: IsFirstStaticBar: Bar[" + this + "].HasParentBars=false");
				}
				return this == this.ParentBars.BarStaticFirst_nullUnsafe;
			} }
		[JsonIgnore]	public Bar BarPrevious_nullUnsafe { get {
				if (this.HasParentBars == false) {
					throw new Exception("PROPERTY_VALID_ONLY_WHEN_THIS_BAR_IS_ADDED_INTO_BARS: BarPrevious: Bar[" + this + "].HasParentBars=false");
				}
				if (this.ParentBarsIndex == 0) {
					return null;
				}
				return this.ParentBars[this.ParentBarsIndex - 1];
			} }
		[JsonIgnore]	public Bar BarNext_nullUnsafe { get {
				if (this.HasParentBars == false) {
					throw new Exception("PROPERTY_VALID_ONLY_WHEN_THIS_BAR_IS_ADDED_INTO_BARS: BarNext: Bar[" + this + "].HasParentBars=false");
				}
				if (this.ParentBarsIndex == this.ParentBars.Count - 1) {
					return null;
				}
				return this.ParentBars[this.ParentBarsIndex + 1];
			} }
//		[JsonIgnore]	public DateTime DateTimeMarketOpenedTodayExpectedBasedOnMarketInfo { get {
//				if (this.HasParentBars == false) {
//					throw new Exception("PROPERTY_VALID_ONLY_WHEN_THIS_BAR_IS_ADDED_INTO_BARS: BarPrevious: Bar[" + this + "].HasParentBars=false");
//				}
//				DateTime marketOpenServerTime = this.ParentBars.MarketInfo.MarketOpenServerTime;
//				if (marketOpenServerTime == DateTime.MinValue) return DateTime.MinValue;
//				DateTime todayMarketOpenServerTime = this.ParentBars.CombineBarDateWithMarketOpenTime(this.DateTimeOpen, marketOpenServerTime);
//				return todayMarketOpenServerTime;
//			} }
//		[JsonIgnore]	public DateTime DateTimeMarketClosesTodayExpectedBasedOnMarketInfo { get {
//				if (this.HasParentBars == false) {
//					throw new Exception("PROPERTY_VALID_ONLY_WHEN_THIS_BAR_IS_ADDED_INTO_BARS: BarPrevious: Bar[" + this + "].HasParentBars=false");
//				}
//				DateTime marketCloseServerTime = this.ParentBars.MarketInfo.MarketCloseServerTime;
//				if (marketCloseServerTime == DateTime.MinValue) return DateTime.MinValue;
//				DateTime todayMarketCloseServerTime = this.ParentBars.CombineBarDateWithMarketOpenTime(this.DateTimeOpen, marketCloseServerTime);
//				return todayMarketCloseServerTime;
//			} }
//		[JsonIgnore]	public int BarIndexMarketOpenedTodayExpectedBasedOnMarketInfoDoesntCountClearingIntervals { get {
//				if (this.HasParentBars == false) {
//					throw new Exception("PROPERTY_VALID_ONLY_WHEN_THIS_BAR_IS_ADDED_INTO_BARS: BarPrevious: Bar[" + this + "].HasParentBars=false");
//				}
//				int ret = -1;
//				DateTime todayMarketOpenServerTime = this.DateTimeMarketOpenedTodayExpectedBasedOnMarketInfo;
//				Bar barFirstReceivedToday = this.BarMarketOpenedTodayScanBackwardIgnoringMarketInfo;
//				ret = barFirstReceivedToday.ParentBarsIndex;
//				DateTime dateBarFirstReceivedToday = barFirstReceivedToday.DateTimeOpen;
//				if (dateBarFirstReceivedToday == todayMarketOpenServerTime) {
//					return ret; 
//				}
//				TimeSpan betweenMarketOpenAndFirstBarOfDay = dateBarFirstReceivedToday.Subtract(todayMarketOpenServerTime);
//				int firstBarWasLateSeconds = (int) betweenMarketOpenAndFirstBarOfDay.TotalSeconds;
//				int secondsInOneBar = this.ScaleInterval.AsTimeSpanInSeconds;
//				int barsLate = (int)firstBarWasLateSeconds / secondsInOneBar;
//				ret += barsLate;
//				return ret;
//			} }
//		[JsonIgnore]	public int BarIndexMarketClosesTodayExpectedBasedOnMarketInfoIncludingClearing { get {
//				if (this.HasParentBars == false) {
//					throw new Exception("PROPERTY_VALID_ONLY_WHEN_THIS_BAR_IS_ADDED_INTO_BARS: BarPrevious: Bar[" + this + "].HasParentBars=false");
//				}
//				int ret = -1;
//				DateTime todayMarketClosesServerTime = this.DateTimeMarketClosesTodayExpectedBasedOnMarketInfo;
//				Bar barFirstReceivedToday = this.BarMarketOpenedTodayScanBackwardIgnoringMarketInfo;
//				ret = barFirstReceivedToday.ParentBarsIndex;
//				DateTime dateBarFirstReceivedToday = barFirstReceivedToday.DateTimeOpen;
//				if (dateBarFirstReceivedToday == todayMarketClosesServerTime) {
//					return ret;
//				}
//				if (dateBarFirstReceivedToday < todayMarketClosesServerTime) {
//					// WALKING_FORWARD_SKIPPING_CLEARING_INTERVALS
//					int offset = this.ParentBars.DistanceInBarsWalkBackwardBasedOnMarketInfoExcludingClearing(dateBarFirstReceivedToday, todayMarketClosesServerTime);
//					ret -= offset; 
//					return ret;
//				}
//				TimeSpan betweenMarketOpenAndMarketClose = dateBarFirstReceivedToday.Subtract(todayMarketClosesServerTime);
//				// TODO: CAN_BE_WRONG_APPROACH_BUT_FILTERING_CLEARING_INTERVALS_STRETCHING_BEYOND_MARKETOPEN_MARKETCLOSE_ACTUALLY_WORKS_HERE
//				TimeSpan clearingWholeDayWholeBars = this.ParentBars.ClearingIntervalsStretchingWholeBarsTotalled;
//				betweenMarketOpenAndMarketClose = betweenMarketOpenAndMarketClose.Subtract(clearingWholeDayWholeBars);
//				
//				int distanceSeconds = (int) betweenMarketOpenAndMarketClose.TotalSeconds;
//				int secondsInOneBar = this.ScaleInterval.AsTimeSpanInSeconds;
//				int distanceBars = (int)distanceSeconds / secondsInOneBar;
//				ret += distanceBars;
//				return ret;
//			} }
		[JsonIgnore]	public Bar BarMarketOpenedTodayScanBackwardIgnoringMarketInfo { get {
				// I_CAN_NOT_JUST_ADD_DATES_BECAUSE_MARKETINFO_HAS_MARKET_OPEN_TIME_BUT_BAR_MAY_NOT_EXIST
				return this.ParentBars.BarMarketOpenedTodayScanBackward(this);
			} }
//		[JsonIgnore]	public Bar BarMarketClosedTodayScanForwardIgnoringMarketInfo { get {
//				return this.ParentBars.BarMarketClosedTodayScanForwardIgnoringMarketInfo(this);
//			} }
//		[JsonIgnore]	public Bar BarMarketClosedTodayScanBackwardIgnoringMarketInfo { get {
//				return this.ParentBars.BarMarketClosedTodayScanBackward(this);
//			} }
//		[Obsolete("WARNING_NOT_YET_IMPLEMENTED")]
//		[JsonIgnore]	public int BarIndexExpectedMarketClosesToday { get {
//				return this.ParentBars.SuggestBarIndexExpectedMarketClosesToday(this);
//			} }
		[JsonIgnore]	public int BarIndexAfterMidnightReceived { get {
				return this.ParentBarsIndex - this.BarMarketOpenedTodayScanBackwardIgnoringMarketInfo.ParentBarsIndex;
			} }
		[JsonIgnore]	public int BarIndexExpectedSinceTodayMarketOpen { get {
				int ret = -1;
				if (this.HasParentBars == false) return ret;
				ret = this.ParentBars.BarIndexSinceTodayMarketOpenSuggestForwardFor(this.DateTimeOpen);
				return ret; 
			} }
//		[JsonIgnore]	public int BarIndexExpectedMarketClosesToday { get {
//				int ret = -1;
//				if (this.HasParentBars == false) return ret;
//				ret = this.BarIndexExpectedMarketClosesTodaySinceMarketOpen + this.ParentBarsIndex;
//				return ret; 
//			}}
		[JsonIgnore]	public int BarIndexExpectedMarketClosesTodaySinceMarketOpen { get {
				int ret = -1;
				if (this.HasParentBars == false) return ret;
				ret = this.ParentBars.BarIndexExpectedMarketClosesTodaySinceMarketOpenSuggestBackwardForDateLaterOrEqual(this.DateTimeOpen);
				return ret; 
			}}

		[JsonIgnore]	public bool IsMarketSuspended_forClearing_duringThisBar { get {
			bool ret = false;
			string msig = " //Bar.IsMarketSuspended_forClearing_duringThisBar; returning[" + ret + "]";
			if (this.ParentBars == null) {
				string msg = "ASK_MarketInfo_KOZ_PseudoBar_DOESNT_HAVE_ParentBars";
				Assembler.PopupException(msg + msig);
				return ret;
			}
			if (this.ParentBars.MarketInfo == null) {
				string msg = "Bars_DOESNT_HAVE_MarketInfo";
				Assembler.PopupException(msg + msig);
				return ret;
			}
			ret = this.ParentBars.MarketInfo.IsMarketSuspended_forClearing_duringBar(this.DateTimeOpen, this.DateTime_nextBarOpen_unconditional);
			return ret;
		} }

//		[JsonIgnore]	public int BarsDuringMarketOpenExpectedIncludingClearingIntervals { get {
//				int ret = -1;
//				if (this.HasParentBars == false) return ret;
//				ret = this.ParentBars.BarsDuringMarketOpenExpectedIncludingClearingIntervals;
//				return ret; 
//			} }
		
//		public DateTime ReplaceTimeOpenWith(DateTime marketOpenCloseIntradayTime) {
//			DateTime ret = new DateTime(this.DateTimeOpen.Year, this.DateTimeOpen.Month, this.DateTimeOpen.Day,
//				marketOpenCloseIntradayTime.Hour, marketOpenCloseIntradayTime.Minute, marketOpenCloseIntradayTime.Second);
//			return ret;
//		}
//		public DateTime AddBarIntervalsToDate(int howManyBarsToAdd) {
//			if (this.HasParentBars == false) {
//				throw new Exception("PROPERTY_VALID_ONLY_WHEN_THIS_BAR_IS_ADDED_INTO_BARS: BarPrevious: Bar[" + this + "].HasParentBars=false");
//			}
//			return this.ParentBars.AddBarIntervalsToDate(this.DateTimeOpen, howManyBarsToAdd);
//		}
//		[Obsolete("Redundant for a Bar added into Bars, but helps your maniacal disorder to slow down")]
//		[JsonIgnore]	public bool IsAlignedToOwnScaleInterval { get {
//				return (this.DateTimeOpen == this.roundDateDownToMyInterval(this.DateTimeOpen)); } }
	}
}
