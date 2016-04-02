using System;
using System.Diagnostics;
using System.ComponentModel;

using Newtonsoft.Json;

using Sq1.Core.Execution;

namespace Sq1.Core.DataTypes {
	public partial class SymbolInfo {
		[JsonIgnore]	const double PriceStepFromDde_NOT_RECEIVED = -1;


		[Category("1. Essential"), Description("[Stock] is linear, [Future] expires and has to be glued up (2D), [Option] has strikes (3D), [Forex] allows non-integer lots and trades Mon6am-Fri5pm EST, [CryptoCurrencies] are slow due to their JSON-over-web nature, [USBond] has 1/16 PriceStep"), DefaultValue(SecurityType.Stock)]
		[JsonProperty]	public	SecurityType	SecurityType				{ get; set; }

		[Category("1. Essential"), Description(""), ReadOnly(true)]
		[JsonProperty]	public	string			Symbol						{ get; set; }

		[Category("1. Essential"), Description("")]
		[JsonProperty]	public	string			SymbolClass					{ get; set; }

		[Category("1. Essential"), Description("For {RTSIndex = MICEX * USD/RUR}: {Position.Size = Position.Size * SymbolInfo.Point2Dollar}"), DefaultValue(1)]
		[JsonProperty]	public	double			Point2Dollar				{ get; set; }

		[Category("1. Essential"), Description("0=DISABLED Forces killUnfilledPendings, closeOpenPositions N seconds prior to MarketInfo.GetReason_ifMarket_closedOrSuspended_secondsFromNow(int secondsFromNow) Backtest,Livesim,Live: no timer/thread, just inside StrategyExecutor for each newQuote")]
		[JsonProperty]	public	int				GoFlat_priorTo_marketClose_clearing_sec	{ get; set; }

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



		[Category("3. Pre-OrderProcessor"), Description("prior to Emitting, auto-convert Market orders by setting a Broker-acceptable Price: [MarketZeroSentToBroker] will set Alert[Market].Price=0 and send it;[MarketMinMaxSentToBroker] sets Alert.Price to Streaming's two special values received per instrument (see QUIK to Excel import); [LimitCrossMarket] puts counterparty's current observed price from the other side of the spread; [LimitTidal] is good for frequent fluctuations, saves you spread but has less chance to get fill; auto-conversion is useful for: 1) Forex doesn't support market orders (MT4/MT5?); 2) Market Buy for RTS-Index must mention MaxPrice instead of 0/omitted"), DefaultValue(MarketOrderAs.MarketZeroSentToBroker)]
		[JsonProperty]	public	MarketOrderAs	MarketOrderAs				{ get; set; }

		[Browsable(false)]
		[JsonIgnore]	public	bool			MarketZeroOrMinMax			{ get {
				return this.MarketOrderAs == MarketOrderAs.MarketZeroSentToBroker
					|| this.MarketOrderAs == MarketOrderAs.MarketMinMaxSentToBroker;
			} }

		[Category("3. Pre-OrderProcessor"), Description(""), DefaultValue("10,20,30,40")]
		[JsonProperty]	public	string			SlippagesCrossMarketCsv				{ get; set; }

		[Category("3. Pre-OrderProcessor"), Description(""), DefaultValue("10,20,30,40")]
		[JsonProperty]	public	string			SlippagesTidalCsv				{ get; set; }

		[Category("3. Pre-OrderProcessor"), Description(""), DefaultValue(true)]
		[JsonProperty]	public	bool			UseFirstSlippageForBacktest	{ get; set; }

		[Category("3. Pre-OrderProcessor"), Description("For StopSell + "), DefaultValue(true)]
		[JsonProperty]	public	bool			ReplaceTidalWithCrossMarket	{ get; set; }

		[Category("3. Pre-OrderProcessor"), Description(""), DefaultValue(100)]
		[JsonProperty]	public	int				ReplaceTidalMillis			{ get; set; }

		[Category("3. Pre-OrderProcessor"), Description(""), DefaultValue(false)]
		[JsonProperty]	public	bool			MarketOrders_priceFill_bringBackFromOutrageous			{ get; set; }



		[Category("4. OrderProcessor"), Description("for same-bar open+close (MA crossover), SameBarPolarCloseThenOpen=[True] will submit close first, wait for Close=>Filled/KilledPending + SequencedOpeningAfterClosedDelayMillis"), DefaultValue(true)]
		[JsonProperty]	public	bool			SameBarPolarCloseThenOpen	{ get; set; }

		[Category("4. OrderProcessor"), Description("for same-bar open+close (MA crossover), SameBarPolarCloseThenOpen=[True] will submit close first, wait for Close=>Filled/KilledPending + SequencedOpeningAfterClosedDelayMillis"), DefaultValue(100)]
		[JsonProperty]	public	int				SequencedOpeningAfterClosedDelayMillis		{ get; set; }

		[Category("4. OrderProcessor"), Description("For each Broker.OrderSubmit(), check if a similar [TBD] order is already in the Pendings; useful when you are debugging your strategy that shoots the same order multiple times by mistake"), DefaultValue(false)]
		[JsonProperty]	public	bool			CheckForSimilarAlreadyPending { get; set; }



		[Category("5. Post-OrderProcessor"), Description("EmergencyClose is PostProcessor's thread that kicks in when triggers when Position's Close was Rejected (Ctrl+Shift+F: InStateErrorComplementaryEmergencyState)"), DefaultValue(5)]
		[JsonProperty]	public	int				EmergencyCloseAttemptsMax	{ get; set; }

		[Category("5. Post-OrderProcessor"), Description("EmergencyClose will sleep EmergencyCloseInterAttemptDelayMillis in its thread and repeat Closing of a Rejected ExitOrder, until ExitOrder.Clone will be returned by the BrokerAdapter as Filled, EmergencyCloseAttemptsMax times max"), DefaultValue(100)]
		[JsonProperty]	public	int				EmergencyCloseInterAttemptDelayMillis	{ get; set; }

		[Category("5. Post-OrderProcessor"), Description("OrderPostProcessorRejected is somehow different than OrderPostProcessorEmergency... sorry"), DefaultValue(true)]
		[JsonProperty]	public	bool			ReSubmitRejected			{ get; set; }

		[Category("5. Post-OrderProcessor"), Description("OrderPostProcessorRejected and OrderPostProcessorEmergency will increase the distance (=> decrease the profit) by using next available from SlippagesBuy/SlippagesSell"), DefaultValue(true)]
		[JsonProperty]	public	bool			ReSubmittedUsesNextSlippage	{ get; set; }



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
			this.GoFlat_priorTo_marketClose_clearing_sec = 0;

			this.PriceDecimals					= 2;
			this.PriceStepFromDde				= SymbolInfo.PriceStepFromDde_NOT_RECEIVED;
			this.VolumeDecimals					= 0;	// if your Forex Symbol uses lotMin=0.001, DecimalsVolume = 3 
			this.SameBarPolarCloseThenOpen		= true;
			this.SequencedOpeningAfterClosedDelayMillis = 1000;
			this.MarketOrderAs					= MarketOrderAs.Unknown;
			this.ReplaceTidalWithCrossMarket	= false;
			this.ReplaceTidalMillis				= 0;
			this.SlippagesCrossMarketCsv		= "";
			this.SlippagesTidalCsv				= "";
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

		public string getSlippagesCsv(MarketOrderAs crossOrTidal) {
			string ret = null;
			switch (crossOrTidal) {
				case MarketOrderAs.LimitCrossMarket:	ret = this.SlippagesCrossMarketCsv;	break;
				case MarketOrderAs.LimitTidal:			ret = this.SlippagesTidalCsv;			break;

				case MarketOrderAs.MarketUnchanged_DANGEROUS:
				case MarketOrderAs.MarketZeroSentToBroker:
				case MarketOrderAs.MarketMinMaxSentToBroker:
					string msg = "SLIPAGE_NOT_APPLICABLE_FOR[" + crossOrTidal + "] returning slippage=0";
					Assembler.PopupException(msg);
					break;

				default:
					string msg1 = "ADD_HANDLER_FOR_YOUR_NEW_MarketOrderAs[" + crossOrTidal + "]";
					Assembler.PopupException(msg1);
					break;
			}
			return ret;
		}
		public int GetSlippage_maxIndex_forLimitOrdersOnly(Alert alert) {
			return this.getSlippage_maxIndex_forLimitOrdersOnly(alert.Direction, alert.MarketOrderAs);
		}
		int getSlippage_maxIndex_forLimitOrdersOnly(Direction direction, MarketOrderAs crossOrTidal) {
			int ret = -1;
			string slippagesCsv = this.getSlippagesCsv(crossOrTidal);
			if (string.IsNullOrEmpty(slippagesCsv)) {
				return ret;
			}
			string[] slippages = slippagesCsv.Split(',');
			if (slippages != null) ret = slippages.Length - 1;
			return ret;
		}
		//public double GetSlippage_signAware_forLimitAlertsOnly(Alert alert, int slippageIndex=0, bool isStreaming=true) {
		//    return this.GetSlippage_signAware_forLimitOrdersOnly(alert.PriceScriptAligned, alert.Direction, alert.MarketOrderAs, slippageIndex, isStreaming);
		//}

		public double GetSlippage_signAware_forLimitOrdersOnly(double priceAligned, Direction direction, MarketOrderAs crossOrTidal, int slippageIndex=0, bool isStreaming=true) {
			double ret = 0;
			
			if (isStreaming == false && this.UseFirstSlippageForBacktest == false) return ret;		// HACKY

			string slippagesCsv = this.getSlippagesCsv(crossOrTidal);

			if (string.IsNullOrEmpty(slippagesCsv)) return ret;

			string[] slippages = slippagesCsv.Split(',');
			if (slippages.Length == 0)
				throw new Exception("check getSlippagesAvailable(" + direction + ") != 0) before calling me");

			if (slippageIndex < 0) slippageIndex = 0;
			if (slippageIndex >= slippages.Length) slippageIndex = slippages.Length - 1;

			string slippage_asString = slippages[slippageIndex];
			try {
				ret = Convert.ToDouble(slippage_asString);
			} catch (Exception ex) {
				string msg = "slippages[" + slippageIndex + "]=[" + slippage_asString + "] should be Double"
					+ " slippagesCsv[" + slippagesCsv + "]";
				//throw new Exception(msg, e);
				Assembler.PopupException(msg, ex, false);
			}

			if (direction == Direction.Short || direction == Direction.Sell) ret = -ret;
			return ret;
		}
		public SymbolInfo Clone() {
			return (SymbolInfo)this.MemberwiseClone();
		}

		public double Alert_alignToPriceLevel(double alertPrice, Direction direction, MarketLimitStop marketLimitStop) {
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
			decimal integefier = Convert.ToDecimal(this.PriceStepFromDde);
			if (integefier == -1) {
				integefier = Convert.ToDecimal(Math.Pow(10, this.PriceDecimals));	// 10 ^ 2 = 100;
				//integefier = this.PriceStepFromDecimal;
			}
			if (integefier < 0) {
				throw new ArithmeticException();
			}
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
				throw new NotImplementedException("AlignToPriceLevel() for PriceLevelRoundingMode." + upOrDown);
			}
			ret /= integefier;														// 9015.0 => 90.15
			//ret = Math.Round(ret, integefier);
			if (ret < integefier) {
				ret = integefier;
			} else {
				ret = Math.Round(ret, (int)integefier);	// DIRTY
			}
			return Convert.ToDouble(ret);
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
	}
}
