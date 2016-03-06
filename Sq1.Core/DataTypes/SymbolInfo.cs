using System;
using System.Diagnostics;
using System.ComponentModel;

using Newtonsoft.Json;
using Sq1.Core.Execution;

namespace Sq1.Core.DataTypes {
	public class SymbolInfo {
		[JsonIgnore]	const double PriceStepFromDde_NOT_RECEIVED = -1;


		[Category("1. Essential"), Description("[Stock] is linear, [Future] expires and has to be glued up (2D), [Option] has strikes (3D), [Forex] allows non-integer lots and trades Mon6am-Fri5pm EST, [CryptoCurrencies] are slow due to their JSON-over-web nature, [USBond] has 1/16 PriceStep"), DefaultValue(SecurityType.Stock)]
		[JsonProperty]	public	SecurityType	SecurityType				{ get; set; }

		[Category("1. Essential"), Description(""), ReadOnly(true)]
		[JsonProperty]	public	string			Symbol						{ get; set; }

		[Category("1. Essential"), Description("")]
		[JsonProperty]	public	string			SymbolClass					{ get; set; }

		[Category("1. Essential"), Description("For {RTSIndex = MICEX * USD/RUR}: {Position.Size = Position.Size * SymbolInfo.Point2Dollar}"), DefaultValue(1)]
		[JsonProperty]	public	double			Point2Dollar				{ get; set; }

		[Category("1. Essential"), Description("when received same Bid and Ask as for the previous Quote, don't push it to consumers and to DdeMonitor; reduces size=-1"), DefaultValue(true)]
		[JsonProperty]	public	bool			FilterOutSameBestBidAskDupes		{ get; set; }

		//[Category("EmergencyProcessor"), Description("NOT_USED Connected somehow with Point2Dollar")]
		//[JsonProperty]	public	double			LeverageForFutures			{ get; set; }


		[Browsable(false)]
		[JsonProperty]	public	int				priceDecimals;

		[Category("2. Price and Volume Units"), Description("digits after decimal dot for min Price Step; 2 for Cents, 0 for Dollars, -2 for $100"), DefaultValue(2)]
		[JsonIgnore]	public	int				PriceDecimals				{
			get { return this.priceDecimals; }
			set { this.priceDecimals = value; this.raisePriceDecimalsChanged(); }
		}


		//BEFORE Pow/Log was invented: for (int i = this.Decimals; i > 0; i--) this.PriceLevelSize /= 10.0;
		[Category("2. Price and Volume Units"), Description("TODO")]
		[JsonIgnore]	public	double			PriceStepFromDecimal		{ get { return Math.Pow(10, -this.PriceDecimals); } }		// 10^(-2) = 0.01

		[Category("2. Price and Volume Units"), Description("if not -1 then goes to PriceStep; set to -1 to use PriceStepFromDecimal for PriceStep")]
		[JsonProperty]	public	double			PriceStepFromDde			{ get; set; }
		
		[Category("2. Price and Volume Units"), Description("TODO")]
		[JsonIgnore]	public	double			PriceStep					{ get {
			return this.PriceStepFromDde != SymbolInfo.PriceStepFromDde_NOT_RECEIVED ? this.PriceStepFromDde : this.PriceStepFromDecimal; } }

		[Category("2. Price and Volume Units"), Description("Chart and Execution will display PriceDecimals+1 to visually assure Streaming and Broker adapters haven't got prices out of your expectations")]
		[JsonIgnore]	public	string			PriceFormat					{ get { return "N" + (this.PriceDecimals); } }



		[Category("2. Price and Volume Units"), Description("digits after decimal dot for min lot Volume; valid for partial Forex lots (-5 for 0.00001) and Bitcoins (-6 for 0.0000001); for stocks/options/futures I'd set 0 here"), DefaultValue(0)]
		[JsonProperty]	public	int				VolumeDecimals				{ get; set; }

		//BEFORE Pow/Log was invented: for (int i = this.Decimals; i > 0; i--) this.PriceLevelSize /= 10.0;
		[Category("2. Price and Volume Units"), Description("digits after decimal dot for min lot Volume; valid for partial Forex lots (-5 for 0.00001) and Bitcoins (-6 for 0.0000001); for stocks/options/futures I'd set 0 here")]
		[JsonIgnore]	public	double			VolumeStepFromDecimal		{ get { return Math.Pow(10, -this.VolumeDecimals); } }		// 10^(-2) = 0.01

		[Category("2. Price and Volume Units"), Description("Chart and Execution will display VolumeDecimals+1 to visually assure Streaming and Broker adapters haven't got volumes out of your expectations")]
		[JsonIgnore]	public	string			VolumeFormat				{ get { return "N" + (this.VolumeDecimals); } }



		[Category("3. OrderProcessor"), Description("for same-bar open+close (MA crossover), SameBarPolarCloseThenOpen=[True] will submit close first, wait for Close=>Filled/KilledPending + SequencedOpeningAfterClosedDelayMillis"), DefaultValue(true)]
		[JsonProperty]	public	bool			SameBarPolarCloseThenOpen	{ get; set; }

		[Category("3. OrderProcessor"), Description("for same-bar open+close (MA crossover), SameBarPolarCloseThenOpen=[True] will submit close first, wait for Close=>Filled/KilledPending + SequencedOpeningAfterClosedDelayMillis"), DefaultValue(100)]
		[JsonProperty]	public	int				SequencedOpeningAfterClosedDelayMillis		{ get; set; }



		[Category("4. Post-OrderProcessor"), Description("EmergencyClose is PostProcessor's thread that kicks in when triggers when Position's Close was Rejected (Ctrl+Shift+F: InStateErrorComplementaryEmergencyState)"), DefaultValue(5)]
		[JsonProperty]	public	int				EmergencyCloseAttemptsMax	{ get; set; }

		[Category("4. Post-OrderProcessor"), Description("EmergencyClose will sleep EmergencyCloseInterAttemptDelayMillis in its thread and repeat Closing of a Rejected ExitOrder, until ExitOrder.Clone will be returned by the BrokerAdapter as Filled, EmergencyCloseAttemptsMax times max"), DefaultValue(100)]
		[JsonProperty]	public	int				EmergencyCloseInterAttemptDelayMillis	{ get; set; }

		[Category("4. Post-OrderProcessor"), Description("OrderPostProcessorRejected is somehow different than OrderPostProcessorEmergency... sorry"), DefaultValue(true)]
		[JsonProperty]	public	bool			ReSubmitRejected			{ get; set; }

		[Category("4. Post-OrderProcessor"), Description("OrderPostProcessorRejected and OrderPostProcessorEmergency will increase the distance (=> decrease the profit) by using next available from SlippagesBuy/SlippagesSell"), DefaultValue(true)]
		[JsonProperty]	public	bool			ReSubmittedUsesNextSlippage	{ get; set; }



		[Category("5. Pre-OrderProcessor"), Description("prior to Emitting, auto-convert Market orders by setting a Broker-acceptable Price: [MarketZeroSentToBroker] will set Alert[Market].Price=0 and send it;[MarketMinMaxSentToBroker] sets Alert.Price to Streaming's two special values received per instrument (see QUIK to Excel import); [LimitCrossMarket] puts counterparty's current observed price from the other side of the spread; [LimitTidal] is good for frequent fluctuations, saves you spread but has less chance to get fill; auto-conversion is useful for: 1) Forex doesn't support market orders (MT4/MT5?); 2) Market Buy for RTS-Index must mention MaxPrice instead of 0/omitted"), DefaultValue(MarketOrderAs.MarketZeroSentToBroker)]
		[JsonProperty]	public	MarketOrderAs	MarketOrderAs				{ get; set; }

		[Browsable(false)]
		[JsonIgnore]	public	bool			MarketZeroOrMinMax			{ get {
				return this.MarketOrderAs == MarketOrderAs.MarketZeroSentToBroker
					|| this.MarketOrderAs == MarketOrderAs.MarketMinMaxSentToBroker;
			} }

		[Category("5. Pre-OrderProcessor"), Description(""), DefaultValue("10,20,30,40")]
		[JsonProperty]	public	string			SlippagesBuy				{ get; set; }

		[Category("5. Pre-OrderProcessor"), Description(""), DefaultValue("10,20,30,40")]
		[JsonProperty]	public	string			SlippagesSell				{ get; set; }

		[Category("5. Pre-OrderProcessor"), Description(""), DefaultValue(true)]
		[JsonProperty]	public	bool			UseFirstSlippageForBacktest	{ get; set; }

		[Category("5. Pre-OrderProcessor"), Description("For StopSell + "), DefaultValue(true)]
		[JsonProperty]	public	bool			ReplaceTidalWithCrossMarket	{ get; set; }

		[Category("5. Pre-OrderProcessor"), Description(""), DefaultValue(100)]
		[JsonProperty]	public	int				ReplaceTidalMillis			{ get; set; }


		[Category("6. Other"), Description(""), DefaultValue(true)]
		[JsonProperty]	public	bool			SimBugOutOfBarStopsFill		{ get; set; }

		[Category("6. Other"), Description(""), DefaultValue(true)]
		[JsonProperty]	public	bool			SimBugOutOfBarLimitsFill	{ get; set; }


		[Category("7. DdeMonitor"), Description(""), DefaultValue(true)]
		[JsonProperty]	public	bool			Level2AskShowHoles			{ get; set; }

		[Category("7. DdeMonitor"), Description(""), DefaultValue(true)]
		[JsonProperty]	public	bool			Level2ShowSpread			{ get; set; }

		[Category("7. DdeMonitor"), Description(""), DefaultValue(true)]
		[JsonProperty]	public	bool			Level2BidShowHoles			{ get; set; }

		[Category("7. DdeMonitor"), Description(""), DefaultValue(true)]
		[JsonProperty]	public bool				Level2ShowCumulativesInsteadOfLots	{ get; set; }

		[Category("7. DdeMonitor"), Description(""), DefaultValue(true)]
		[JsonProperty]	public int				Level2PriceLevels			{ get; set; }


		public SymbolInfo() { 		// used by JSONdeserialize() /  XMLdeserialize()
			//this.MarketName = "US Equities";
			this.SecurityType					= SecurityType.Stock;
			this.Symbol							= "UNKNOWN_SYMBOL";
			this.SymbolClass					= "";
			this.Point2Dollar					= 1.0;
			this.FilterOutSameBestBidAskDupes	= true;

			this.PriceDecimals					= 2;
			this.PriceStepFromDde				= SymbolInfo.PriceStepFromDde_NOT_RECEIVED;
			this.VolumeDecimals					= 0;	// if your Forex Symbol uses lotMin=0.001, DecimalsVolume = 3 
			this.SameBarPolarCloseThenOpen		= true;
			this.SequencedOpeningAfterClosedDelayMillis = 1000;
			this.MarketOrderAs					= MarketOrderAs.Unknown;
			this.ReplaceTidalWithCrossMarket	= false;
			this.ReplaceTidalMillis				= 0;
			this.SlippagesBuy					= "";
			this.SlippagesSell					= "";
			this.ReSubmitRejected				= false;
			this.ReSubmittedUsesNextSlippage	= false;
			this.UseFirstSlippageForBacktest	= true;
			this.EmergencyCloseInterAttemptDelayMillis		= 8000;
			this.EmergencyCloseAttemptsMax		= 5;

			this.Level2AskShowHoles				= true;
			this.Level2ShowSpread				= true;
			this.Level2BidShowHoles				= true;
			this.Level2PriceLevels				= 10;
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

		[Obsolete("REMOVE_ONCE_NEW_ALIGNMENT_MATURES_DECEMBER_15TH_2014 used only in tidal calculations")]
		public double Order_alignToPriceLevel(double orderPrice, Direction direction, MarketLimitStop marketLimitStop) {
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
			return this.Alert_alignToPriceLevel(orderPrice, entryNotExit, positionLongShortV1, marketLimitStop);
		}
		public double Alert_alignToPriceLevel(double alertPrice, bool buyOrShort, PositionLongShort positionLongShort0, MarketLimitStop marketLimitStop0) {
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
		public double Alert_alignToPriceLevel_simplified(double alertPrice, Direction direction, MarketLimitStop marketLimitStop) {
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
				#if DEBUG
				Debugger.Break();
				#endif
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
			if (this.PriceDecimals < 0) {
				#if DEBUG
				Debugger.Break();
				#endif
				throw new NotImplementedException();
			}
			int integefier = (int)Math.Pow(10, this.PriceDecimals);		// 10 ^ 2 = 100;
			decimal ret = (decimal) price * integefier; 				// 90.145 => 9014.5
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
				throw new NotImplementedException("AlignToPriceLevel() for PriceLevelRoundingMode." + upOrDown);
			}
			ret /= integefier;	// 9015.0 => 90.15
			ret = Math.Round(ret, this.PriceDecimals);
			return (double)ret;
		}
		#endif
		public double AlignToVolumeStep(double volume, PriceLevelRoundingMode upOrDown = PriceLevelRoundingMode.RoundToClosest) {
			if (this.VolumeDecimals < 0) {
				#if DEBUG
				Debugger.Break();
				#endif
				throw new NotImplementedException();
			}
			int integefier = (int)Math.Pow(10, this.VolumeDecimals);		// 10 ^ 2 = 100;
			decimal ret = (decimal) volume * integefier; 					// 90.145 => 9014.5
			switch (upOrDown) {
				case PriceLevelRoundingMode.RoundDown:		ret = Math.Floor(ret);		break;		// 9014.5 => 9014.0
				case PriceLevelRoundingMode.RoundUp:		ret = Math.Ceiling(ret);	break;		// 9014.5 => 9015.0
				case PriceLevelRoundingMode.RoundToClosest:	ret = Math.Round(ret);		break;		// 9014.5 => 9015.0
				case PriceLevelRoundingMode.DontRoundPrintLowerUpper:
				double lowerLevel = this.AlignToVolumeStep(volume, PriceLevelRoundingMode.RoundDown);
				double upperLevel = this.AlignToVolumeStep(volume, PriceLevelRoundingMode.RoundUp);
					string msg = "DontRound=>returning[" + volume + "] RoundDown[" + lowerLevel + "] RoundUp[" + upperLevel + "] ";
					Assembler.PopupException(msg);
					return volume;
				default:
				throw new NotImplementedException("AlignToVolumeStep() for PriceLevelRoundingMode." + upOrDown);
			}
			ret /= integefier;	// 9015.0 => 90.15
			ret = Math.Round(ret, this.VolumeDecimals);
			return (double)ret;
		}

		public double PriceRound_fractionsBeyondDecimals(double orderPrice) {
			double decimalPointShifterBeforeRounding = Math.Pow(10, this.PriceDecimals);		// 2 => 100
			// assuming this.DecimalsPrice=2: orderPrice=156.633,27272 => 15.663.327,272 => 15.663.327 => 156.633,27[tailTruncated] 
			double ret = Math.Round(orderPrice * decimalPointShifterBeforeRounding, 0) / decimalPointShifterBeforeRounding;
			return ret;
		}
		public override string ToString() {
			string ret = this.Symbol + ":" + this.PriceStep;
			ret += "(" + Enum.GetName(typeof(SecurityType), this.SecurityType) + ")";
			return ret;
		}
		// I_HATE_SUCH_INTRANSPARENCY__ALMOST_INTRODUCED_FOR_List<Symbol>__BUT_IMPLEMENTED__MY_WAY_TO_AVOID_OVERRIDING_EQUALS
		//public override bool Equals(object obj) {
		//	return this.Symbol == (((SymbolInfo))obj));
		//}


		public event EventHandler<EventArgs> PriceDecimalsChanged;
		void raisePriceDecimalsChanged() {
			if (this.PriceDecimalsChanged == null) return;
			try {
				this.PriceDecimalsChanged(this, null);
			} catch (Exception ex) {
				string msg = "ONE_OF_PriceDecimalsChanged_SUBSCRIBERS_THREW_DEPRIVING_OTHERS SymbolInfo[" + this.Symbol + "].PriceDecimals=>[" + this.PriceDecimals + "]";
				Assembler.PopupException(msg, ex);
			}
		}
	}
}
