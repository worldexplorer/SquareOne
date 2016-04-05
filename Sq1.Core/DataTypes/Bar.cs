using System;
using System.Diagnostics;
using System.Text;

using Newtonsoft.Json;

namespace Sq1.Core.DataTypes {
	public partial class Bar {
		[JsonProperty]	public	string		Symbol				{ get; internal set; }		// protected=>internal so that BarsUnscaled can rename each bar
		[JsonProperty]	public	BarScaleInterval ScaleInterval	{ get; protected set; }
		[JsonProperty]	public	DateTime	DateTimeOpen		{ get; protected set; }
		[JsonProperty]	public	DateTime	DateTimeNextBarOpenUnconditional		{ get; protected set; }
		[JsonProperty]	public	DateTime	DateTimePreviousBarOpenUnconditional	{ get; protected set; }
		
		[JsonProperty]	public	double	Open;
		[JsonProperty]	public	double	High;
		[JsonProperty]	public	double	Low;
		[JsonProperty]	public	double	Close;
		[JsonProperty]	public	double	Volume;

		[JsonIgnore]	public	Bars	ParentBars			{ get; protected set; }
		[JsonIgnore]	public	int		ParentBarsIndex		{ get; protected set; }
		[JsonIgnore]	public	bool	HasParentBars		{ get { return this.ParentBars != null; } }
		[JsonProperty]	public	string	ParentBarsIdent		{ get {
				if (this.HasParentBars == false) return "NO_PARENT_BARS";
				StringBuilder sb = new StringBuilder("");
				//if (this.ParentBarsIndex <  this.ParentBars.Count - 1) ret = this.ParentBarsIndex.ToString();
				//if (this.ParentBarsIndex == this.ParentBars.Count - 1) ret = "LastStaticBar";// +this.ParentBarsIndex;
				//if (this.ParentBarsIndex == this.ParentBars.Count) ret = "StreamingBar";// +this.ParentBarsIndex;
				//if (this.ParentBarsIndex >  this.ParentBars.Count) ret = "ScaryGhostBar:" + this.ParentBarsIndex;
				if (this.IsBarStreaming)	sb.Append("BarStreaming");// +this.ParentBarsIndex;
				if (this.IsBarStaticLast)	sb.Append("StaticBarLast");// +this.ParentBarsIndex;
				if (this.IsBarStaticFirst)	sb.Append("StaticBarFist");// +this.ParentBarsIndex;
				sb.Append("#");
				sb.Append(this.ParentBarsIndex);
				sb.Append("/");
				sb.Append(this.ParentBars.Count-1);
				return sb.ToString();
			} }
		// Perst deserializer invokes default ctor()
		[JsonProperty]	public int DaySerial;

		[JsonIgnore]	public double	HighLowDistance		{ get { return this.High - this.Low; } }
		[JsonIgnore]	public bool		IsWhiteCandle		{ get { return this.Close > this.Open; } }


		public Bar() {
			// ChartRenderer would update its max/min if NaN
			this.ParentBarsIndex = -1;
			this.Open = Double.NaN;
			this.High = Double.NaN;
			this.Low = Double.NaN;
			this.Close = Double.NaN;
			this.Volume = Double.NaN;
		}
		public Bar(string symbol, BarScaleInterval scaleInterval, DateTime dateTimeOpen) : this() {
			this.Symbol = symbol;
			this.ScaleInterval = scaleInterval;
			if (dateTimeOpen == DateTime.MinValue) {
				this.DateTimeOpen = DateTime.MinValue;
				this.DateTimeNextBarOpenUnconditional = DateTime.MinValue;
				this.DateTimePreviousBarOpenUnconditional = DateTime.MinValue;
			} else {
				this.RoundDateDownInitTwoAuxDates(dateTimeOpen);
			}
		}
		public Bar(string symbol, BarScaleInterval scaleInterval, DateTime dateTimeOpen, double firstPriceOfBar, double firstVolumeOfBar, SymbolInfo symbolInfo)
						: this(symbol, scaleInterval, dateTimeOpen) {
			this.SetSame_OHLCValigned(firstPriceOfBar, firstVolumeOfBar, symbolInfo);
		}
		public void SetSame_OHLCValigned(double firstPriceOfBar, double firstVolumeOfBar, SymbolInfo symbolInfo) {
			this.SetOHLCValigned(firstPriceOfBar, firstPriceOfBar, firstPriceOfBar, firstPriceOfBar, firstVolumeOfBar, symbolInfo);
		}
		public void SetOHLCValigned(double open, double high, double low, double close, double volume, SymbolInfo symbolInfo = null) {
			this.Open = open;
			this.High = high;
			this.Low = low;
			this.Close = close;
			this.Volume = volume;
			if (symbolInfo != null) {
				this.Open	= Math.Round(this.Open,		symbolInfo.PriceDecimals);
				this.High	= Math.Round(this.High,		symbolInfo.PriceDecimals);
				this.Low	= Math.Round(this.Low,		symbolInfo.PriceDecimals);
				this.Close	= Math.Round(this.Close,	symbolInfo.PriceDecimals);
				this.Volume	= Math.Round(this.Volume,	symbolInfo.VolumeDecimals);

				//this.Open = symbolInfo.AlignToPriceLevel(this.Open, PriceLevelRoundingMode.RoundToClosest);
				//this.High = symbolInfo.AlignToPriceLevel(this.High, PriceLevelRoundingMode.RoundToClosest);
				//this.Low  = symbolInfo.AlignToPriceLevel(this.Low, PriceLevelRoundingMode.RoundToClosest);
				//this.Close  = symbolInfo.AlignToPriceLevel(this.Close, PriceLevelRoundingMode.RoundToClosest);
				//this.Volume = Math.Round(this.Volume, symbolInfo.DecimalsVolume);
			} else {
				string msg = "PARENT_BARS_NOT_ACCESSIBLE OHLC_IS_NOT_ALIGNED_TRY_TO_AVOID_IT";
				//Assembler.PopupException(msg, null);
			}
		}
		public void AbsorbOHLCVfrom(Bar bar) {
			SymbolInfo symbolInfo = null;
			if (bar.ParentBars != null) {
				symbolInfo = bar.ParentBars.SymbolInfo;
			} else {
				if (this.ParentBars != null) {
					symbolInfo = this.ParentBars.SymbolInfo;
				}
			}
			if (symbolInfo == null) {
				string msg = "PARENT_BARS_NOT_ACCESSIBLE WONT_ROUND_ABSORBED_OHLC"
					//+ "; it's null for first ever bar of freshly added symbol for Sq1.Adapters.QuikMock.StreamingMock"
					;
				Assembler.PopupException(msg, null);
			}
			this.SetOHLCValigned(bar.Open, bar.High, bar.Low, bar.Close, bar.Volume, symbolInfo);
		}
		public void SetParentForBackwardUpdate(Bars parentBars, int parentBarsIndex) {
			if (this.ParentBars == parentBars) {
				string msg = "TYRING_AVOID_BUGS: same ParentBars as I have already;"
					+ "  this.ParentBars==parentBars[" + parentBars + "]";
				throw new Exception(msg);
			}
			if (this.ParentBarsIndex == parentBarsIndex) {
				string msg = "TYRING_AVOID_BUGS: same ParentBarsIndex[" + this.ParentBarsIndex + "] as I have already;"
					+ "  this.ParentBars==parentBars[" + parentBars + "]";
				throw new Exception(msg);
			}
			if (this.ParentBars != null) {
				if (this.ParentBars.Symbol != parentBars.Symbol) {
					string msg1 = "here is the problem for a streaming bar to carry another symbol!";
					throw new Exception(msg1);
				}
				string msg = "this.ParentBars!=null => this Bar is already assigned to Bars;"
					+ " use Bar.CloneDetached() if you add this Bar to another BarSeries"
					+ " otherwise reciprocity will be uneven"
					+ " and strategies relying on quote.ParentBar.ParentBarsIndex will be messed up";
				throw new Exception(msg);
			}
			if (this.ParentBarsIndex != -1) {
				string msg = "this.ParentBarsIndex!=-1 => this Bar is already assigned to Bars;"
					+ " use Bar.CloneDetached() if you add this Bar to another BarSeries"
					+ " otherwise reciprocity will be uneven"
					+ " and strategies relying on quote.ParentBar.ParentBarsIndex will be messed up";
				throw new Exception(msg);
			}
			this.ParentBars = parentBars;
			this.ParentBarsIndex = parentBarsIndex;
			
			Bar barPrevious = this.BarPrevious_nullUnsafe;
			if (barPrevious != null) {
				this.DaySerial = barPrevious.DaySerial;
				if (this.DateTimeOpen.Date > barPrevious.DateTimeOpen.Date) {
					this.DaySerial++;
				}
			}
		}
		public Bar Clone() {
			return (Bar)this.MemberwiseClone();
		}
		public Bar CloneDetached() {
			Bar detached = this.Clone();
			detached.ParentBars = null;
			detached.ParentBarsIndex = -1;
			return detached;
		}
		public override bool Equals(object other) {
			if (other is Bar == false) {
				return base.Equals(other);
			}
			Bar bar = (Bar)other;
			string barAsString = bar.ToString();
			string thisAsString = this.ToString();
			return (barAsString == thisAsString);
		}
		public string CheckOHLCVthrow(bool throwNewException = true) {
			string msg = "";
			
			if (this.Open <= 0)			msg += "Open[" + this.Open + "]<=0 ";
			if (this.High <= 0)			msg += "High[" + this.High + "]<=0 ";
			if (this.Low <= 0)			msg += "Low[" + this.Low + "]<=0 ";
			if (this.Close <= 0)		msg += "Close[" + this.Close + "]<=0 ";
			//if (this.Volume <= 0)		msg += "Volume[" + this.Volume + "]<=0 ";
			
			if (this.High < this.Low)	msg += "High[" + this.High + "]<Low[" + this.High + "] ";
			if (this.Low <= 0)			msg += "Low[" + this.Low + "]<=0 ";
			
			if (this.Close > this.High)	msg += "Close[" + this.Close + "]>High[" + this.High + "] ";
			if (this.Close < this.Low)	msg += "Close[" + this.Close + "]<Low[" + this.High + "] ";
			
			if (this.Open > this.High)	msg += "Open[" + this.Open + "]>High[" + this.High + "] ";
			if (this.Open < this.Low)	msg += "Open[" + this.Open + "]<Low[" + this.High + "] ";

			if (string.IsNullOrEmpty(msg)) return msg;

			//Debugger.Break();
			if (throwNewException) {
				throw new Exception(msg);
			}
			return msg;
		}
		public bool HasSameDOHLCVas(Bar bar, string barIdent, string thisIdent, ref string errRef) {
			if (this.Symbol != bar.Symbol) {
				#if VERBOSE_STRINGS_SLOW
				errRef = thisIdent + ".Symbol[" + this.Symbol + "] != " + barIdent + ".Symbol[" + bar.Symbol + "]";
				#endif
				return false;
			}

			if (this.ScaleInterval != bar.ScaleInterval) {
				#if VERBOSE_STRINGS_SLOW
				errRef = thisIdent + ".ScaleInterval[" + this.ScaleInterval + "] != "
					+ barIdent + ".ScaleInterval[" + bar.ScaleInterval + "]";
				#endif
				return false;
			}

			if (this.DateTimeOpen != bar.DateTimeOpen) {
				#if VERBOSE_STRINGS_SLOW
				errRef = thisIdent + ".DateTimeOpen[" + this.DateTimeOpen + "] != " 
					+ barIdent + ".DateTimeOpen[" + bar.DateTimeOpen + "]";
				#endif
				return false;
			}

			//v1
			//bool sameOHLCV = (this.Open == bar.Open && this.High == bar.High
			//	&& this.Low == bar.Low && this.Close == bar.Close && this.Volume == bar.Volume);
			//if (sameOHLCV == false) {
			//	#if VERBOSE_STRINGS_SLOW
			//	msg = "OHLCV are different while DateTimeOpen is the same: "
			//		+ thisIdent + "[" + this + "] != " + barIdent + "[" + bar + "]";
			//	#endif
			//	return false;
			//}
			//v2
			bool OHLCV_different = false;
			string OHLCV_msg = "";
			if (this.Open != bar.Open) {
				OHLCV_different = true;
				#if VERBOSE_STRINGS_SLOW
				OHLCV_msg += thisIdent + ".Open[" + this.Open + "] != " + barIdent + ".Open[" + bar.Open + "] ";
				#endif
			}
			if (this.High != bar.High) {
				OHLCV_different = true;
				#if VERBOSE_STRINGS_SLOW
				OHLCV_msg += thisIdent + ".High[" + this.High + "] != " + barIdent + ".High[" + bar.High + "] ";
				#endif
			}
			if (this.Low != bar.Low) {
				OHLCV_different = true;
				#if VERBOSE_STRINGS_SLOW
				OHLCV_msg += thisIdent + ".Low[" + this.Low + "] != " + barIdent + ".Low[" + bar.Low + "] ";
				#endif
			}
			if (this.Close != bar.Close) {
				OHLCV_different = true;
				#if VERBOSE_STRINGS_SLOW
				OHLCV_msg += thisIdent + ".Close[" + this.Close + "] != " + barIdent + ".Close[" + bar.Close + "] ";
				#endif
			}
			if (this.Volume != bar.Volume) {
				OHLCV_different = true;
				#if VERBOSE_STRINGS_SLOW
				OHLCV_msg += thisIdent + ".Volume[" + this.Volume + "] != " + barIdent + ".Volume[" + bar.Volume + "] ";
				#endif
			}
			if (OHLCV_different == true) {
				#if VERBOSE_STRINGS_SLOW
				errRef = OHLCV_msg;
				#else
				errRef = "ENABLE_CONDITIONAL_COMPILATION_SYMBOL_TO_SEE_ERROR: VERBOSE_STRINGS_SLOW";
				#endif
				return false;
			}


			bool sameParent = (this.ParentBars == bar.ParentBars && this.ParentBarsIndex == bar.ParentBarsIndex);
			if (sameParent == false) {
				#if VERBOSE_STRINGS_SLOW
				errRef = "CAN_SKIP_PARENT_DIFFERENT:"
					+ " " + thisIdent + ".ParentBars[" + this.ParentBarsIdent + "]"
					+ " != " + bar + ".ParentBarsIndex[" + bar.ParentBarsIdent + "]"
					+ " while lastStaticBar.DOHLCV=barAdding.DOHLCV";
				#endif
				return true;
			}

			#if VERBOSE_STRINGS_SLOW
			errRef = "lastStaticBar.DOHLCV=barAdding.DOHLCV";
			#endif
			return true;
		}
		#region THIS_ISNT_IN_BARS_BECAUSE_UNATTACHED_STREAMING_BAR_SHOULD_HAVE_ITS_OWN_BRAIN_BUT_OTHERWIZE_BARS_SHOULD_BE_BAR_FACTORY
		// DataSeriesTimeBased contains this method, too
		public void RoundDateDownInitTwoAuxDates(DateTime dateTimeOpen) {
			this.DateTimeOpen = roundDateDownToMyInterval(dateTimeOpen);
			///if (this.DateTimeOpen.CompareTo(dateTimeOpen) == 0) {
			//	int a = 1;
			//}
			this.DateTimeNextBarOpenUnconditional = this.addIntervalsToDate(this.DateTimeOpen, 1);
			this.DateTimePreviousBarOpenUnconditional = this.addIntervalsToDate(this.DateTimeOpen, -1);
		}
		// DataSeriesTimeBased contains this method, too
		DateTime addIntervalsToDate(DateTime dateTimeToAddIntervalsTo, int intervalMultiplier) {
			if (this.DateTimeOpen == DateTime.MinValue) return DateTime.MinValue;
			DateTime dateTime = roundDateDownToMyInterval(dateTimeToAddIntervalsTo);
			int addTimeIntervals = this.ScaleInterval.Interval * intervalMultiplier;
			switch (this.ScaleInterval.Scale) {
				case BarScale.Tick:
					throw new ArgumentException("Tick scale is not supported");
				case BarScale.Second:
					dateTime = dateTime.AddSeconds((double)addTimeIntervals);
					break;
				case BarScale.Minute:
					dateTime = dateTime.AddMinutes((double)addTimeIntervals);
					break;
				case BarScale.Hour:
					dateTime = dateTime.AddHours((double)addTimeIntervals);
					break;
				case BarScale.Daily:
					dateTime = dateTime.Date.AddDays((double)addTimeIntervals);
					break;
				case BarScale.Weekly:
					dateTime = dateTime.Date.AddDays(addTimeIntervals * 7);
					break;
				case BarScale.Monthly:
					dateTime = dateTime.Date.AddMonths(addTimeIntervals);
					break;
				case BarScale.Quarterly:
					dateTime = dateTime.Date.AddMonths(addTimeIntervals * 3);
					break;
				case BarScale.Yearly:
					dateTime = dateTime.Date.AddYears(addTimeIntervals);
					break;
				default:
					throw new Exception("this.ScaleInterval.Scale[" + this.ScaleInterval.Scale
						+ "] is not supported");
			}
			return dateTime;
		}
		// DataSeriesTimeBased contains this method, too
		DateTime roundDateDownToMyInterval(DateTime dateTimeToRoundDown) {
			if (this.ScaleInterval == null) throw new Exception("ScaleInterval=null in roundDateDownToInterval(" + dateTimeToRoundDown + ")");
			DateTime dateTime = new DateTime(dateTimeToRoundDown.Ticks);
			switch (this.ScaleInterval.Scale) {
				case BarScale.Tick:
					throw new ArgumentException("Tick scale is not supported");
				case BarScale.Second:
					int secondsRoundedDown = ((int)Math.Floor((double)dateTime.Second / this.ScaleInterval.Interval)) * this.ScaleInterval.Interval;
					dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, secondsRoundedDown);
					break;
				case BarScale.Minute:
					int minutesRoundedDown = ((int)Math.Floor((double)dateTime.Minute / this.ScaleInterval.Interval)) * this.ScaleInterval.Interval;
					dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, minutesRoundedDown, 0);
					break;
				case BarScale.Hour:
					int hoursRoundedDown = ((int)Math.Floor((double)dateTime.Hour / this.ScaleInterval.Interval)) * this.ScaleInterval.Interval;
					dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hoursRoundedDown, 0, 0);
					break;
				case BarScale.Daily:
					dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
					break;
				case BarScale.Weekly:
					while (dateTime.DayOfWeek != DayOfWeek.Monday) dateTime.AddDays(-1);
					dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
					break;
				case BarScale.Monthly:
					dateTime = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
					break;
				case BarScale.Quarterly:
					int monthBeginningOfQuarter = ((int)Math.Floor((double)dateTime.Month / 3)) * 3;
					dateTime = new DateTime(dateTime.Year, monthBeginningOfQuarter, 1, 0, 0, 0);
					break;
				case BarScale.Yearly:
					dateTime = new DateTime(dateTime.Year, 1, 1, 0, 0, 0);
					break;
				default:
					throw new Exception("this.ScaleInterval.Scale[" + this.ScaleInterval.Scale + "] is not supported");
			}
			return dateTime;
		}
		#endregion
		public void MergeExpandHLCV_whileCompressingManyBarsToOne(Bar bar, bool addVolumeWeAreCompressingStaticBarsToLargerScaleInterval = true) {
			if (bar.High > this.High) this.High = bar.High;
			if (bar.Low < this.Low) this.Low = bar.Low;
			this.Close = bar.Close;
			if (addVolumeWeAreCompressingStaticBarsToLargerScaleInterval) {
				this.Volume += bar.Volume;
			} else {
				this.Volume = bar.Volume;
			}
		}
		public bool MergeExpandHLCV_forStreamingBarUnattached(Quote quoteClone) {
			bool barExpanded = false;
			//if (quoteClone.PriceLastDeal > this.High) this.High = quoteClone.PriceLastDeal;
			//if (quoteClone.PriceLastDeal < this.Low) this.Low = quoteClone.PriceLastDeal;
			if (quoteClone.Ask > this.High) {
				this.High = quoteClone.Ask;
				barExpanded = true;
			}
			if (quoteClone.Bid < this.Low) {
				this.Low = quoteClone.Bid;
				barExpanded = true;
			}
			if (double.IsNaN(quoteClone.TradedPrice)) {
				string msg = "FYI LAST_DEAL_PRICE_IS_NAN WHILE_MERGING_QUOTE_INTO_STREAMING_BAR quoteClone[" +
					quoteClone + "] => " + this.ToString();
			}
			this.Close = quoteClone.TradedPrice;
			if (quoteClone.Size < 0) {
				string msg = "WHATEVER_QUOTE_YOU_CREATE_IN_STREAMING__MUST_HAVE_NON_DEFAULT_SIZE quoteClone[" + quoteClone + "]";
				Assembler.PopupException(msg);
			}
			this.Volume += quoteClone.Size;
			return barExpanded;
		}
		public override string ToString() {
			bool formatValues = false;
			
			string priceFormat = "N";
			string volumeFormat = "N";
			int priceDecimals = 3;
			int volumeDecimals = 3;
			
			if (this.ParentBars != null && this.ParentBars.SymbolInfo != null) {
				priceDecimals	= this.ParentBars.SymbolInfo.PriceDecimals;
				volumeDecimals	= this.ParentBars.SymbolInfo.VolumeDecimals;
				priceFormat		= this.ParentBars.SymbolInfo.PriceFormat;
				volumeFormat	= this.ParentBars.SymbolInfo.VolumeFormat;
			}
			
//			return this.ParentBarsIdent + ":"
//				+ Symbol + "(" + ScaleInterval + ") "
//				+ "T[" + DateTimeOpen + "]"
//				+ "O[" + Math.Round(this.Open,	priceDecimals).ToString(priceFormat) + "]"
//				+ "H[" + Math.Round(this.High,	priceDecimals).ToString(priceFormat) + "]"
//				+ "L[" + Math.Round(this.Low,	priceDecimals).ToString(priceFormat) + "]"
//				+ "C[" + Math.Round(this.Close,	priceDecimals).ToString(priceFormat) + "]"
//				+ "V[" + Math.Round(this.Volume,volumeDecimals).ToString(volumeFormat) + "]"
//				;

			#if DEBUG
			if (this.Open.ToString(priceFormat) != Math.Round(this.Open,	priceDecimals).ToString(priceFormat)) {
				string msg = "Double.ToString() doesn't invoke Math.Round!";
				//Debugger.Break();
				Assembler.PopupException(msg, null, false);
			}
			#endif
			
			StringBuilder sb = new StringBuilder();
			sb.Append(this.ParentBarsIdent);
			sb.Append(":");
			sb.Append(Symbol);
			sb.Append("(");
			sb.Append(ScaleInterval.ToString());
			sb.Append(") ");
			
			sb.Append("T[");
			sb.Append(DateTimeOpen.ToString());
			sb.Append("]");

			sb.Append("O[");
			sb.Append(formatValues ? this.Open.ToString(priceFormat) : this.Open.ToString());
			sb.Append("]");

			sb.Append("H[");
			sb.Append(formatValues ? this.High.ToString(priceFormat) : this.High.ToString());
			sb.Append("]");

			sb.Append("L[");
			sb.Append(formatValues ? this.Low.ToString(priceFormat) : this.Low.ToString());
			sb.Append("]");

			sb.Append("C[");
			sb.Append(formatValues ? this.Close.ToString(priceFormat) : this.Close.ToString());
			sb.Append("]");

			sb.Append("V[");
			sb.Append(formatValues ?  this.Volume.ToString(volumeFormat) : this.Volume.ToString());
			sb.Append("]");

			return sb.ToString();
		}
		public bool ContainsPrice(double entryFillPrice) {
			if (entryFillPrice < this.Low) return false; 
			if (entryFillPrice > this.High) return false;
			return true;
		}
		public bool ContainsBidAskForQuoteGenerated(Quote quote, bool feedingGarbageAndIknowItDontBreak = false) {
			if (this.HighLowDistance == 0) {
				if (quote.Bid > this.Open || quote.Ask < this.Open) {
					string msg = "FIRST_QUOTE_OF_THE_BAR MUST_HAVE_BID_OR_ASK_EQUALS_TO_OPEN(WHY_NOT_MORE_ELABORATE?)"
						+ " FOR_QUOTE_GENERATED_I_HOPE_OPEN_IS_IN_THE_MIDDLE_OF_QUOTE'S_SPREAD";
					Assembler.PopupException(msg, null, false);
					return false;
				}
				return true;
			}

			if (quote.Spread > this.HighLowDistance) {
				string msg = "CONSIDER_REDUCING_SPREAD_SIZE_IN_Chart>Strategy>SpreadModeler"
					+ " quote.Spread[" + quote.Spread + "] > this.HighLowDistance[" + this.HighLowDistance + "]"
					+ " for quote[" + quote.ToString() + "] bar[" + this.ToString() + "]";
				Assembler.PopupException(msg, null, false);
				return false;
			}
			
			if (quote.Ask == quote.Bid) {
				#if DEBUG
				Debugger.Break();
				#endif
				return false;
			}
			if (quote.Ask < quote.Bid) {
				#if DEBUG
				Debugger.Break();
				#endif
				return false;
			}
			
			// 81.41 > 81.41 ???? introducing Rounding
			double lowRounded = Math.Round(this.Low, this.ParentBars.SymbolInfo.PriceDecimals);
			double highRounded = Math.Round(this.High, this.ParentBars.SymbolInfo.PriceDecimals);
			double bidRounded = Math.Round(quote.Bid, this.ParentBars.SymbolInfo.PriceDecimals);
			double askRounded = Math.Round(quote.Ask, this.ParentBars.SymbolInfo.PriceDecimals);
			
			//if (quote.Bid < this.Low) {
			if (bidRounded < lowRounded) {
				// MOSTLY_FROM_SCANNING_UP_GenerateClosestQuoteForEachPendingAlertOnOurWayTo() 
				if (feedingGarbageAndIknowItDontBreak == false) {
					#if DEBUG
					Debugger.Break();
					#endif
				}
				return false;
			}
			// 81.41 > 81.41 ???? introducing Rounding
			//if (quote.Ask > this.High) {
			if (askRounded > highRounded) {
				// MOSTLY_FROM_SCANNING_UP_GenerateClosestQuoteForEachPendingAlertOnOurWayTo()
				if (feedingGarbageAndIknowItDontBreak == false) {
					#if DEBUG
					Debugger.Break();
					#endif
				}
				return false;
			}
			
			if (quote.Size > this.Volume) {
				#if DEBUG
				Debugger.Break();
				#endif
				return false;
			}
			return true;
		}
		public bool FillAtSlimBarIsWithinSpread(double priceFilledThroughPosition, double spreadMaxDeviated) {
			// not testing here if priceFilledThroughPosition is within Low..High => use ContainsPrice() before calling me
			//bool insideBar = this.ContainsPrice(priceFilledThroughPosition);

			// this bar isn't slim, its height is at least Spread => use ContainsPrice() before calling me
			double zeroAtFirstSimulatedBarTick = this.High - this.Low;
			if (zeroAtFirstSimulatedBarTick >= spreadMaxDeviated) return true;

			// not testing if the quote with spreadMaxDeviated is within Low..High => use ContainsBidAskForQuoteGenerated() before calling me
			double fillPriceAwayFromLow = Math.Abs(this.Low - priceFilledThroughPosition);
			double fillPriceAwayFromHigh = Math.Abs(this.High - priceFilledThroughPosition);
			bool fillBelowLowAndSpread = fillPriceAwayFromLow > spreadMaxDeviated;
			bool fillAboveHightAndSpread = fillPriceAwayFromHigh > spreadMaxDeviated;

			bool outsideSpread = fillBelowLowAndSpread || fillBelowLowAndSpread;
			if (outsideSpread) {
				#if DEBUG
				Debugger.Break();
				#endif
			}
			return !outsideSpread;
		}
	}
}