using System;
using System.Diagnostics;

using Newtonsoft.Json;
using Sq1.Core.Execution;

namespace Sq1.Core.DataTypes {
	public class SymbolInfo {
		[JsonProperty]	public SecurityType		SecurityType;
		[JsonProperty]	public string			Symbol;
		[JsonProperty]	public string			SymbolClass;
		[JsonProperty]		   double			_Point2Dollar;
		[JsonProperty]	public double			Point2Dollar {
			get { return this._Point2Dollar; }
			set {
				if (value <= 0.0) {
					this._Point2Dollar = 1.0;
					return;
				}
				this._Point2Dollar = value;
			}
		}
		[JsonProperty]	public double			PriceLevelSizeForBonds;
		[JsonProperty]	public int				DecimalsPrice;
		[JsonProperty]	public int				DecimalsVolume;					// valid for partial Forex lots and Bitcoins; for stocks/options/futures its always (int)1

		//BEFORE Pow/Log was invented: for (int i = this.Decimals; i > 0; i--) this.PriceLevelSize /= 10.0;
		[JsonProperty]	public double			PriceMinimalStepFromDecimal		{ get { return Math.Pow(10, -this.DecimalsPrice); } }			// 10^(-2) = 0.01
		[JsonProperty]	public double			VolumeMinimalStepFromDecimal	{ get { return Math.Pow(10, -this.DecimalsVolume); } }		// 10^(-2) = 0.01
		
		[JsonProperty]	public bool				SameBarPolarCloseThenOpen;
		[JsonProperty]	public int				SequencedOpeningAfterClosedDelayMillis;
		[JsonProperty]	public int				EmergencyCloseDelayMillis;
		[JsonProperty]	public int				EmergencyCloseAttemptsMax;
		[JsonProperty]	public bool				ReSubmitRejected;
		[JsonProperty]	public bool				ReSubmittedUsesNextSlippage;
		[JsonProperty]	public bool				UseFirstSlippageForBacktest;
		[JsonProperty]	public string			SlippagesBuy;
		[JsonProperty]	public string			SlippagesSell;
		[JsonProperty]	public MarketOrderAs	MarketOrderAs;
		[JsonProperty]	public bool				MarketZeroOrMinMax				{ get {
				return this.MarketOrderAs == MarketOrderAs.MarketZeroSentToBroker
					|| this.MarketOrderAs == MarketOrderAs.MarketMinMaxSentToBroker;
			} }
		[JsonProperty]	public bool				ReplaceTidalWithCrossMarket;
		[JsonProperty]	public int				ReplaceTidalMillis;
		[JsonProperty]	public bool				SimBugOutOfBarStopsFill;
		[JsonProperty]	public bool				SimBugOutOfBarLimitsFill;
		[JsonProperty]		   double			_Margin;
		[JsonIgnore]	public double			LeverageForFutures {
			get { return this._Margin; }
			set {
				if (value <= 0.0) {
					this._Margin = 1000.0;
					return;
				}
				this._Margin = value;
			}
		}

		public SymbolInfo() { 		// used by JSONdeserialize() /  XMLdeserialize()
			//this.MarketName = "US Equities";
			this.SecurityType = SecurityType.Stock;
			this.SymbolClass = "";
			this.Point2Dollar = 1.0;
			this.DecimalsPrice = 2;
			this.DecimalsVolume = 0;	// if your Forex Symbol uses lotMin=0.001, DecimalsVolume = 3 
			this.PriceLevelSizeForBonds = 1.0;
			this.SameBarPolarCloseThenOpen = true;
			this.SequencedOpeningAfterClosedDelayMillis = 1000;
			this.MarketOrderAs = MarketOrderAs.Unknown;
			this.ReplaceTidalWithCrossMarket = false;
			this.ReplaceTidalMillis = 0;
			this.SlippagesBuy = "";
			this.SlippagesSell = "";
			this.ReSubmitRejected = false;
			this.ReSubmittedUsesNextSlippage = false;
			this.UseFirstSlippageForBacktest = true;
			this.EmergencyCloseDelayMillis = 8000;
			this.EmergencyCloseAttemptsMax = 5;
			this.LeverageForFutures = 1;
		}
		public SymbolInfo(string symbol, SecurityType securityType, int decimals) : this() {
			this.Symbol = symbol;
			this.SecurityType = securityType;
			this.DecimalsPrice = decimals;
		}
		public SymbolInfo(string symbol, SecurityType securityType, int decimals, string ClassCode) : this(symbol, securityType, decimals) {
				//,, double priceLevelSizeForBonds, double margin, double point2Dollar
				//bool overrideMarketPriceToZero, //bool reSubmitRejected, bool reSubmittedUsesNextSlippage) {
			//this.LeverageForFutures = margin;
			//this.Point2Dollar = point2Dollar;
			//this.PriceLevelSizeForBonds = priceLevelSizeForBonds;
			this.SymbolClass = ClassCode;
			//this.OverrideMarketPriceToZero = overrideMarketPriceToZero;
			//this.ReSubmitRejected = reSubmitRejected;
			//this.ReSubmittedUsesNextSlippage = reSubmittedUsesNextSlippage;
		}
		public int getSlippageIndexMax(Direction direction) {
			int ret = -1;
			string slippagesCommaSeparated = (direction == Direction.Buy || direction == Direction.Cover)
				? this.SlippagesBuy : this.SlippagesSell;
			if (string.IsNullOrEmpty(slippagesCommaSeparated)) {
				return ret;
			}
			string[] slippages = slippagesCommaSeparated.Split(',');
			if (slippages != null) ret = slippages.Length - 1;
			return ret;
		}
		public double getSlippage(double priceAligned, Direction direction, int slippageIndex, bool isStreaming, bool isLimitOrder) {
			if (isStreaming == false && this.UseFirstSlippageForBacktest == false) {
				return 0;
			}
			string slippagesCommaSeparated = (direction == Direction.Buy || direction == Direction.Cover)
				? this.SlippagesBuy : this.SlippagesSell;
			if (string.IsNullOrEmpty(slippagesCommaSeparated)) return 0;
			string[] slippages = slippagesCommaSeparated.Split(',');
			if (slippages.Length == 0)
				throw new Exception("check getSlippagesAvailable(" + direction + ") != 0) before calling me");

			if (slippageIndex < 0) slippageIndex = 0;
			if (slippageIndex >= slippages.Length) slippageIndex = slippages.Length - 1;
			double slippageValue = 0;
			try {
				slippageValue = Convert.ToDouble(slippages[slippageIndex]);
			} catch (Exception e) {
				string msg = "slippages[" + slippageIndex + "]=[" + slippages[slippageIndex] + "] should be Double"
					+ " slippagesCommaSeparated[" + slippagesCommaSeparated + "]";
				//throw new Exception(msg, e);
			}
			//if (direction == Direction.Short || direction == Direction.Sell) slippageValue = -slippageValue;
			return slippageValue;
		}
		public SymbolInfo Clone() {
			return (SymbolInfo)this.MemberwiseClone();
		}
		public string FormatPrice { get { return "N" + (this.DecimalsPrice + 1); } }
		public string FormatVolume { get { return "N" + (this.DecimalsVolume + 1); } }

		[Obsolete("REMOVE_ONCE_NEW_ALIGNMENT_MATURES_DECEMBER_15TH_2014 used only in tidal calculations")]
		public double AlignOrderToPriceLevel(double orderPrice, Direction direction, MarketLimitStop marketLimitStop) {
			bool entryNotExit = true;
			PositionLongShort positionLongShortV1 = PositionLongShort.Long;
			switch (direction) {
				case Direction.Buy:
					entryNotExit = true;
					positionLongShortV1 = PositionLongShort.Long;
					break;
				case Direction.Sell:
					entryNotExit = false;
					positionLongShortV1 = PositionLongShort.Long;
					break;
				case Direction.Short:
					entryNotExit = true;
					positionLongShortV1 = PositionLongShort.Short;
					break;
				case Direction.Cover:
					entryNotExit = false;
					positionLongShortV1 = PositionLongShort.Short;
					break;
				default:
					throw new Exception("add new handler for new Direction[" + direction + "] besides {Buy,Sell,Cover,Short}");
			}
			PositionLongShort positionLongShort = MarketConverter.LongShortFromDirection(direction);
			if (positionLongShortV1 != positionLongShort) {
				string msg = "DEFINITELY_DIFFERENT_POSTPONE_TILL_ORDER_EXECUTOR_BACK_FOR_QUIK_BROKER";
				//Debugger.Break();
			}
			return this.AlignAlertToPriceLevel(orderPrice, entryNotExit, positionLongShortV1, marketLimitStop);
		}
		public double AlignAlertToPriceLevel(double alertPrice, bool buyOrShort, PositionLongShort positionLongShort0, MarketLimitStop marketLimitStop0) {
			PriceLevelRoundingMode roundingMode;
			switch (marketLimitStop0) {
				case MarketLimitStop.Limit:
					if (positionLongShort0 == PositionLongShort.Long) {
						roundingMode = (buyOrShort ? PriceLevelRoundingMode.RoundDown : PriceLevelRoundingMode.RoundUp);
					} else {
						roundingMode = (buyOrShort ? PriceLevelRoundingMode.RoundUp : PriceLevelRoundingMode.RoundDown);
					}
					break;
				case MarketLimitStop.Stop:
				case MarketLimitStop.StopLimit:
					if (positionLongShort0 == PositionLongShort.Long) {
						roundingMode = (buyOrShort ? PriceLevelRoundingMode.RoundUp : PriceLevelRoundingMode.RoundDown);
					} else {
						roundingMode = (buyOrShort ? PriceLevelRoundingMode.RoundDown : PriceLevelRoundingMode.RoundUp);
					}
					break;
				case MarketLimitStop.Market:
					roundingMode = PriceLevelRoundingMode.RoundToClosest;
					break;
				default:
					roundingMode = PriceLevelRoundingMode.DontRoundPrintLowerUpper;
					break;
			}
			return this.AlignToPriceLevel(alertPrice, roundingMode);
		}
		public double AlignAlertToPriceLevelSimplified(double alertPrice, Direction direction, MarketLimitStop marketLimitStop) {
			PriceLevelRoundingMode roundingMode = PriceLevelRoundingMode.DontRoundPrintLowerUpper;
			switch (marketLimitStop) {
				case MarketLimitStop.Limit:
					switch (direction) {
						case Direction.Buy:		roundingMode = PriceLevelRoundingMode.RoundDown;	break;
						case Direction.Sell:	roundingMode = PriceLevelRoundingMode.RoundUp;		break;
						case Direction.Short:	roundingMode = PriceLevelRoundingMode.RoundUp;		break;
						case Direction.Cover:	roundingMode = PriceLevelRoundingMode.RoundDown;	break;
						default: throw new Exception("add new handler for new Direction[" + direction + "] besides {Buy,Sell,Cover,Short}");
					}
					break;
				case MarketLimitStop.Stop:
				case MarketLimitStop.StopLimit:
					switch (direction) {
						case Direction.Buy:		roundingMode = PriceLevelRoundingMode.RoundUp;		break;
						case Direction.Sell:	roundingMode = PriceLevelRoundingMode.RoundDown;	break;
						case Direction.Short:	roundingMode = PriceLevelRoundingMode.RoundDown;	break;
						case Direction.Cover:	roundingMode = PriceLevelRoundingMode.RoundUp;		break;
						default: throw new Exception("add new handler for new Direction[" + direction + "] besides {Buy,Sell,Cover,Short}");
					}
					break;
				case MarketLimitStop.Market:
					roundingMode = PriceLevelRoundingMode.RoundToClosest;
					break;
				default:
					roundingMode = PriceLevelRoundingMode.DontRoundPrintLowerUpper;
					break;
			}
			return this.AlignToPriceLevel(alertPrice, roundingMode);
		}
		
		#if USE_CUSTOM_ROUNDING
		public double AlignToPriceLevel(double price, PriceLevelRoundingMode upOrDown = PriceLevelRoundingMode.RoundToClosest, double priceReference = double.NaN) {
			double ret = -1;
			double lowerLevel = Math.Truncate(price / this.PriceMinimalStepFromDecimal);
			lowerLevel *= this.PriceMinimalStepFromDecimal;
			//double upperLevel = (price == lowerLevel) ? lowerLevel : lowerLevel + this.PriceMinimalStepFromDecimal;
			if (price == lowerLevel) {
				string msg = "ALREADY_ROUNDED_NO_REMAINDER_AFTER_DIVISION_TO_STEPS";
				Debugger.Break();
				return price;
			}
			double upperLevel = lowerLevel + this.PriceMinimalStepFromDecimal;
			lowerLevel = Math.Round(lowerLevel, this.DecimalsPrice);	// getting rid of Double's compound rounding error
			upperLevel = Math.Round(upperLevel, this.DecimalsPrice);	// getting rid of Double's compound rounding error

			switch (upOrDown) {
				case PriceLevelRoundingMode.RoundDown:		ret = lowerLevel;		break;
				case PriceLevelRoundingMode.RoundUp:		ret = upperLevel;		break;
				case PriceLevelRoundingMode.RoundToClosest:
					double priceOrRef	= double.IsNaN(priceReference) ? price : priceReference;
					double distanceUp	= Math.Abs(priceOrRef - upperLevel);
					double distanceDown	= Math.Abs(priceOrRef - lowerLevel);
					ret = (distanceUp <= distanceDown) ? upperLevel : lowerLevel;
					ret = Math.Round(ret, this.DecimalsPrice);			// getting rid of Double's compound rounding error

					#if DEBUG
					double mathRound = Math.Round(priceOrRef, this.DecimalsPrice);
					string msg1 = "price[" + price.ToString("R") + "]=>aligned[" + ret.ToString("R") + "] mathRound[" + mathRound + "]";
					if (this.DecimalsPrice > 0 && ret != mathRound) {
						Debugger.Break();
					}
					#endif

					break;
				case PriceLevelRoundingMode.DontRoundPrintLowerUpper:
					string msg = "DontRound=>returning[" + price + "] lowerLevel[" + lowerLevel + "] upperLevel[" + upperLevel + "] ";
					Assembler.PopupException(msg);
					return price;
				default:
					throw new NotImplementedException("RoundAlertPriceToPriceLevel() for PriceLevelRoundingMode." + upOrDown);
			}
			#if DEBUG
			string msg2 = "price[" + price.ToString("R") + "]=>aligned[" + ret.ToString("R") + "]";
			#endif
			return ret;
		}
		#else
		public double AlignToPriceLevel(double price, PriceLevelRoundingMode upOrDown = PriceLevelRoundingMode.RoundToClosest) {
			if (this.DecimalsPrice < 0) {
				#if DEBUG
				Debugger.Break();
				#endif
				throw new NotImplementedException();
			}
			int integefier = (int)Math.Pow(10, this.DecimalsPrice);		// 10 ^ 2 = 100;
			decimal ret = (decimal) price * integefier; 							// 90.145 => 9014.5
			switch (upOrDown) {
				case PriceLevelRoundingMode.RoundDown:		ret = Math.Floor(ret);		break;		// 9014.5 => 9014.0
				case PriceLevelRoundingMode.RoundUp:		ret = Math.Ceiling(ret);	break;		// 9014.5 => 9015.0
				case PriceLevelRoundingMode.RoundToClosest:	ret = Math.Round(ret);		break;		// 9014.5 => 9015.0
				case PriceLevelRoundingMode.DontRoundPrintLowerUpper:
					double lowerLevel = this.AlignToPriceLevel(price, PriceLevelRoundingMode.RoundDown);
					double upperLevel = this.AlignToPriceLevel(price, PriceLevelRoundingMode.RoundUp);
					string msg = "DontRound=>returning[" + price + "] RoundDown[" + lowerLevel + "] RoundUp[" + upperLevel + "] ";
					Assembler.PopupException(msg);
					return price;
				default:
					throw new NotImplementedException("RoundAlertPriceToPriceLevel() for PriceLevelRoundingMode." + upOrDown);
			}
			ret /= integefier;	// 9015.0 => 90.15
			ret = Math.Round(ret, this.DecimalsPrice);
			return (double)ret;
		}
		#endif
		public double PriceRoundFractionsBeyondDecimals(double orderPrice) {
			double decimalPointShifterBeforeRounding = Math.Pow(10, this.DecimalsPrice);		// 2 => 100
			// assuming this.DecimalsPrice=2: orderPrice=156.633,27272 => 15.663.327,272 => 15.663.327 => 156.633,27[tailTruncated] 
			double ret = Math.Round(orderPrice * decimalPointShifterBeforeRounding, 0) / decimalPointShifterBeforeRounding;
			return ret;
		}
		public override string ToString() {
			string ret = this.Symbol + ":" + this.PriceMinimalStepFromDecimal;
			ret += "(" + Enum.GetName(typeof(SecurityType), this.SecurityType) + ")";
			return ret;
		}
	}
}
