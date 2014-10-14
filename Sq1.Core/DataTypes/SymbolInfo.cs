using System;

using Newtonsoft.Json;
using Sq1.Core.Execution;

namespace Sq1.Core.DataTypes {
	public class SymbolInfo {
		[JsonProperty]	public SecurityType SecurityType;
		[JsonProperty]	public string Symbol;
		[JsonProperty]	public string SymbolClass;
		[JsonProperty]		   double _Point2Dollar;
		[JsonProperty]	public double Point2Dollar {
			get { return this._Point2Dollar; }
			set {
				if (value <= 0.0) {
					this._Point2Dollar = 1.0;
					return;
				}
				this._Point2Dollar = value;
			}
		}
		[JsonProperty]	public double PriceLevelSizeForBonds;
		[JsonProperty]	public int DecimalsPrice;
		[JsonProperty]	public int DecimalsVolume;		// valid for partial Forex lots and Bitcoins; for stocks/options/futures its always (int)1

		//BEFORE Pow/Log was invented: for (int i = this.Decimals; i > 0; i--) this.PriceLevelSize /= 10.0;
		[JsonProperty]	public double PriceMinimalStepFromDecimal { get { return Math.Pow(10, -this.DecimalsPrice); } }			// 10^(-2) = 0.01
		[JsonProperty]	public double VolumeMinimalStepFromDecimal { get { return Math.Pow(10, -this.DecimalsVolume); } }		// 10^(-2) = 0.01
		
		[JsonProperty]	public bool SameBarPolarCloseThenOpen;
		[JsonProperty]	public int SequencedOpeningAfterClosedDelayMillis;
		[JsonProperty]	public int EmergencyCloseDelayMillis;
		[JsonProperty]	public int EmergencyCloseAttemptsMax;
		[JsonProperty]	public bool ReSubmitRejected;
		[JsonProperty]	public bool ReSubmittedUsesNextSlippage;
		[JsonProperty]	public bool UseFirstSlippageForBacktest;
		[JsonProperty]	public string SlippagesBuy;
		[JsonProperty]	public string SlippagesSell;
		[JsonProperty]	public MarketOrderAs MarketOrderAs;
		[JsonProperty]	public bool MarketZeroOrMinMax { get {
				return this.MarketOrderAs == MarketOrderAs.MarketZeroSentToBroker
					|| this.MarketOrderAs == MarketOrderAs.MarketMinMaxSentToBroker;
			} }
		[JsonProperty]	public bool ReplaceTidalWithCrossMarket;
		[JsonProperty]	public int ReplaceTidalMillis;
		[JsonProperty]	public bool SimBugOutOfBarStopsFill;
		[JsonProperty]	public bool SimBugOutOfBarLimitsFill;
		[JsonProperty]		   double _Margin;
		[JsonProperty]	public double LeverageForFutures {
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
			if (string.IsNullOrEmpty(slippagesCommaSeparated)) return ret;
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
		public double AlignOrderToPriceLevel(double orderPrice, Direction direction, MarketLimitStop marketLimitStop) {
			bool buyOrShort = true;
			PositionLongShort positionLongShort = PositionLongShort.Long;
			switch (direction) {
				case Direction.Buy:
					buyOrShort = true;
					positionLongShort = PositionLongShort.Long;
					break;
				case Direction.Sell:
					buyOrShort = false;
					positionLongShort = PositionLongShort.Long;
					break;
				case Direction.Short:
					buyOrShort = true;
					positionLongShort = PositionLongShort.Short;
					break;
				case Direction.Cover:
					buyOrShort = false;
					positionLongShort = PositionLongShort.Short;
					break;
				default:
					throw new Exception("add new handler for new Direction[" + direction + "] besides {Buy,Sell,Cover,Short}");
			}
			return this.AlignAlertToPriceLevel(orderPrice, buyOrShort, positionLongShort, marketLimitStop);
		}
		public double AlignAlertToPriceLevel(double alertPrice, bool buyOrShort, PositionLongShort positionLongShort0, MarketLimitStop marketLimitStop0) {
			//WHAT_IS_THIS_FOR?
			//double d = orderPrice / this.PriceMinimalFromDecimal;
			//string a = d.ToString("N6");
			//string b = Math.Truncate(d).ToString("N6");
			//if (a == b) return orderPrice;

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
					roundingMode = PriceLevelRoundingMode.SimulateMathRound;
					break;
				default:
					roundingMode = PriceLevelRoundingMode.DontRound;
					break;
			}
			return this.AlignToPriceLevel(alertPrice, roundingMode);
		}
		public double AlignToPriceLevel(double price, PriceLevelRoundingMode upOrDown = PriceLevelRoundingMode.DontRound, double priceReference = double.NaN) {
			double ret = -1;
			double lowerLevel = Math.Truncate(price / this.PriceMinimalStepFromDecimal);
			lowerLevel *= this.PriceMinimalStepFromDecimal;
			double upperLevel = lowerLevel + this.PriceMinimalStepFromDecimal;
			switch (upOrDown) {
				case PriceLevelRoundingMode.DontRound:
					string msg = "DontRound=>returning[" + price + "] lowerLevel[" + lowerLevel + "] upperLevel[" + upperLevel + "] ";
					Assembler.PopupException(msg);
					return price;
				case PriceLevelRoundingMode.RoundDown:
					ret = (lowerLevel >= upperLevel) ? upperLevel : lowerLevel;
					break;
				case PriceLevelRoundingMode.RoundUp:
					ret = (lowerLevel <= upperLevel) ? upperLevel : lowerLevel;
					break;
				case PriceLevelRoundingMode.SimulateMathRound:
					if (double.IsNaN(priceReference) == false) {
					    double distanceUp = Math.Abs(priceReference - upperLevel);
					    double distanceDown = Math.Abs(priceReference - lowerLevel);
					    ret = (distanceUp <= distanceDown) ? upperLevel : lowerLevel;
					} else {
					    double distanceUp = Math.Abs(price - upperLevel);
					    double distanceDown = Math.Abs(price - lowerLevel);
					    ret = (distanceUp <= distanceDown) ? upperLevel : lowerLevel;
					}
					break;
				default:
					throw new NotImplementedException("RoundAlertPriceToPriceLevel() for PriceLevelRoundingMode." + upOrDown);
			}
			ret = Math.Round(ret, this.DecimalsPrice);
			return ret;
		}
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
