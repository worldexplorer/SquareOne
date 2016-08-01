using System;
using System.Diagnostics;
using System.ComponentModel;

using Newtonsoft.Json;

using Sq1.Core.Execution;
using System.Collections.Generic;
using System.Reflection;

namespace Sq1.Core.DataTypes {
	public partial class SymbolInfo {
		[JsonIgnore]	const double PriceStepFromDde_NOT_RECEIVED = -1;


		[Category("1. Essential"), DefaultValue(SecurityType.Stock), Description("[Stock] is linear, [Future] expires and has to be glued up (2D), [Option] has strikes (3D), [Forex] allows non-integer lots and trades Mon6am-Fri5pm EST, [CryptoCurrencies] are slow due to their JSON-over-web nature, [USBond] has 1/16 PriceStep")]
		[JsonProperty]	public	SecurityType	SecurityType				{ get; set; }

		[Category("1. Essential"), Description(""), ReadOnly(true)]
		[JsonProperty]	public	string			Symbol						{ get; set; }

		[Category("1. Essential"), Description("")]
		[JsonProperty]	public	string			SymbolClass					{ get; set; }

		[Category("1. Essential"), DefaultValue(1), Description("For {RTSIndex = MICEX * USD/RUR}: {Position.Size = Position.Size * SymbolInfo.Point2Dollar}")]
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

		[Category("2. Price and Volume Units"), DefaultValue(0), Description("digits after decimal dot for min lot Volume; valid for partial Forex lots (-5 for 0.00001) and Bitcoins (-6 for 0.0000001); for stocks/options/futures I'd set 0 here")]
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

		[Category("3. Pre-OrderProcessor"), DefaultValue("10,20,30,40"), Description("")]
		[JsonProperty]	public	string			SlippagesCrossMarketCsv				{ get; set; }

		[Category("3. Pre-OrderProcessor"), DefaultValue("10,20,30,40"), Description("")]
		[JsonProperty]	public	string			SlippagesTidalCsv				{ get; set; }

		[Category("3. Pre-OrderProcessor"), DefaultValue(true), Description("")]
		[JsonProperty]	public	bool			UseFirstSlippageForMarketAlertsAsLimit	{ get; set; }

		[Category("3. Pre-OrderProcessor"), DefaultValue(true), Description("For StopSell + ")]
		[JsonProperty]	public	bool			NOT_USED_ReplaceTidalWithCrossMarket	{ get; set; }

		[Category("3. Pre-OrderProcessor"), DefaultValue(100), Description("")]
		[JsonProperty]	public	int				NOT_USED_ReplaceTidalMillis			{ get; set; }

		[Category("3. Pre-OrderProcessor"), DefaultValue(false), Description("")]
		[JsonProperty]	public	bool			NOT_USED_MarketOrders_priceFill_bringBackFromOutrageous			{ get; set; }



		[Category("4. OrderProcessor"), DefaultValue(true),		Description("for same-bar open+close (MA crossover), SameBarPolarCloseThenOpen=[True] will submit close first, wait for Close=>Filled/KilledPending + SequencedOpeningAfterClosedDelayMillis")]
		[JsonProperty]	public	bool			SameBarPolarCloseThenOpen	{ get; set; }

		[Category("4. OrderProcessor"), DefaultValue(100),		Description("for same-bar open+close (MA crossover), SameBarPolarCloseThenOpen=[True] will submit close first, wait for Close=>Filled/KilledPending + SequencedOpeningAfterClosedDelayMillis")]
		[JsonProperty]	public	int				SequencedOpeningAfterClosedDelayMillis		{ get; set; }

		[Category("4. OrderProcessor"), DefaultValue(false),	Description("For each Broker.OrderSubmit(), check if a similar [TBD] order is already in the Pendings; useful when you are debugging your strategy that shoots the same order multiple times by mistake")]
		[JsonProperty]	public	bool			CheckForSimilarAlreadyPending { get; set; }



		[Category("5.1 OrderPostProcessor StuckInSubmitted: Reconnect, Kill, Resubmit"), DefaultValue(400),	Description("")]
		[JsonProperty]	public	int				IfNoTransactionCallback_MillisAllowed					{ get; set; }

		[Category("5.1 OrderPostProcessor StuckInSubmitted: Reconnect, Kill, Resubmit"), DefaultValue(true),	Description("")]
		[JsonProperty]	public	bool			IfNoTransactionCallback_ReconnectBrokerDll				{ get; set; }

		[Category("5.1 OrderPostProcessor StuckInSubmitted: Reconnect, Kill, Resubmit"), DefaultValue(true),	Description("")]
		[JsonProperty]	public	bool			IfNoTransactionCallback_Kill_StuckInSubmitted			{ get; set; }

		[Category("5.1 OrderPostProcessor StuckInSubmitted: Reconnect, Kill, Resubmit"), DefaultValue(true),	Description("")]
		[JsonProperty]	public	bool			IfNoTransactionCallback_Resubmit						{ get; set; }

		[Category("5.1 OrderPostProcessor StuckInSubmitted: Reconnect, Kill, Resubmit"), DefaultValue(3),		Description("")]
		[JsonProperty]	public	int				IfNoTransactionCallback_ResubmitLimit					{ get; set; }



		[Category("5.2 OrderPostProcessor:Replace Limit Orders after Expiration @WaitingBrokerFill"), DefaultValue(-1),		Description("if!=-1: 1) Kill Pending Limit + wait it's killed, 2) use SlippagesCrossMarketCsv for CrossMarket and SlippagesTidalCsv for Tidal, 3) send replacement order with more cutting-through slippage")]
		[JsonProperty]	public	int				LimitExpiresAfterMillis		{ get; set; }

		[Category("5.2 OrderPostProcessor:Replace Limit Orders after Expiration @WaitingBrokerFill"), DefaultValue(true), Description("Kill->Wait->SubmitReplacement using {SlippagesCrossMarketCsv/SlippagesTidalCsv} after ReSubmitLimitNotFilledWithinMillis")]
		[JsonProperty]	public	bool			LimitExpiredResubmit		{ get; set; }

		[Category("5.2 OrderPostProcessor:Replace Limit Orders after Expiration @WaitingBrokerFill"), DefaultValue(true), Description("if the price went out of grasp and your strategy will issue a new identical alert OnNextStaticBar")]
		[JsonProperty]	public	bool			LimitExpired_KillUnfilledWithLastSlippage		{ get; set; }



		[Category("5.3 OrderPostProcessor:ReplaceRejected"), DefaultValue(true), Description("OrderPostProcessorRejected is somehow different than OrderPostProcessorEmergency... sorry")]
		[JsonProperty]	public	bool			RejectedResubmit					{ get; set; }

		[Category("5.3 OrderPostProcessor:ReplaceRejected"), DefaultValue(true), Description("OrderPostProcessorRejected is somehow different than OrderPostProcessorEmergency... sorry")]
		[JsonProperty]	public	bool			RejectedKill						{ get; set; }

		[Category("5.3 OrderPostProcessor:ReplaceRejected"), DefaultValue(false), Description("OrderPostProcessorEmergency and OrderPostProcessorEmergency will increase the distance (=> decrease the profit) by using next available from SlippagesBuy/SlippagesSell")]
		[JsonProperty]	public	bool			RejectedResubmitWithNextSlippage	{ get; set; }


		[Category("5.9 OrderPostProcessor:Emergency"), DefaultValue(5), Description("EmergencyClose is PostProcessor's thread that kicks in when triggers when Position's Close was Rejected (Ctrl+Shift+F: InStateErrorComplementaryEmergencyState)")]
		[JsonProperty]	public	int				EmergencyCloseAttemptsMax				{ get; set; }

		[Category("5.9 OrderPostProcessor:Emergency"), DefaultValue(100), Description("EmergencyClose will sleep EmergencyCloseInterAttemptDelayMillis in its thread and repeat Closing of a Rejected ExitOrder, until ExitOrder.Clone will be returned by the BrokerAdapter as Filled, EmergencyCloseAttemptsMax times max")]
		[JsonProperty]	public	int				EmergencyCloseInterAttemptDelayMillis	{ get; set; }


		[Category("6. Other"), DefaultValue(true), Description("")]
		[JsonProperty]	public	bool			SimBugOutOfBarStopsFill		{ get; set; }

		[Category("6. Other"), DefaultValue(true), Description("")]
		[JsonProperty]	public	bool			SimBugOutOfBarLimitsFill	{ get; set; }



		[Category("7. DdeMonitor"), DefaultValue(true), Description("")]
		[JsonProperty]	public	bool			Level2AskShowHoles			{ get; set; }

		[Category("7. DdeMonitor"), DefaultValue(true), Description("")]
		[JsonProperty]	public	bool			Level2ShowSpread			{ get; set; }

		[Category("7. DdeMonitor"), DefaultValue(true), Description("")]
		[JsonProperty]	public	bool			Level2BidShowHoles			{ get; set; }

		[Category("7. DdeMonitor"), DefaultValue(true), Description("")]
		[JsonProperty]	public bool				Level2ShowCumulativesInsteadOfLots	{ get; set; }

		[Category("7. DdeMonitor"), DefaultValue(true), Description("")]
		[JsonProperty]	public int				Level2PriceLevels			{ get; set; }


		public SymbolInfo() { 		// used by JSONdeserialize() /  XMLdeserialize()
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
			this.MarketOrderAs							= MarketOrderAs.Unknown;
			this.NOT_USED_ReplaceTidalWithCrossMarket	= false;
			this.NOT_USED_ReplaceTidalMillis			= 0;
			this.SlippagesCrossMarketCsv		= "";
			this.SlippagesTidalCsv				= "";

			this.IfNoTransactionCallback_MillisAllowed			= 400;
			this.IfNoTransactionCallback_ReconnectBrokerDll		= true;
			this.IfNoTransactionCallback_Kill_StuckInSubmitted	= true;
			this.IfNoTransactionCallback_Resubmit				= true;
			this.IfNoTransactionCallback_ResubmitLimit			= 3;
		
			this.RejectedKill							= true;
			this.RejectedResubmit						= true;
			this.RejectedResubmitWithNextSlippage		= false;

			this.UseFirstSlippageForMarketAlertsAsLimit	= false;
			this.EmergencyCloseInterAttemptDelayMillis	= 8000;

			this.LimitExpiresAfterMillis		= 2000;
			this.LimitExpiredResubmit			= false;
			this.LimitExpired_KillUnfilledWithLastSlippage = true;

			this.EmergencyCloseAttemptsMax		= 5;

			this.Level2AskShowHoles				= true;
			this.Level2ShowSpread				= true;
			this.Level2BidShowHoles				= true;
			this.Level2PriceLevels				= 10;
		}

		string getSlippagesCsv(MarketOrderAs crossOrTidal) {
			string ret = null;
			switch (crossOrTidal) {
				case MarketOrderAs.LimitCrossMarket:	ret = this.SlippagesCrossMarketCsv;		break;
				case MarketOrderAs.LimitTidal:			ret = this.SlippagesTidalCsv;			break;

				//case MarketOrderAs.MarketUnchanged_DANGEROUS:
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
		public List<double> GetSlippages_forLimitOrdersOnly(MarketOrderAs crossOrTidal) {
			List<double> ret = new List<double>();
			string slippagesCsv = this.getSlippagesCsv(crossOrTidal);
			if (string.IsNullOrEmpty(slippagesCsv)) return ret;
			string[] slippages = slippagesCsv.Split(',');
			for (int i=0; i<slippages.Length; i++) {
				string slippage_asString = slippages[i];
				try {
					double parsed = Convert.ToDouble(slippage_asString);
					ret.Add(parsed);
				} catch (Exception ex) {
					string msg = "slippages[" + i + "]=[" + slippage_asString + "] should be Double"
						+ " slippagesCsv[" + slippagesCsv + "]";
					//throw new Exception(msg, e);
					Assembler.PopupException(msg, ex, false);
				}

			}
			return ret;
		}
		public int GetSlippage_maxIndex_forLimitOrdersOnly(MarketOrderAs crossOrTidal) {
			List<double> slippages = this.GetSlippages_forLimitOrdersOnly(crossOrTidal);
			int ret = slippages.Count - 1;
			return ret;
		}
		//public double GetSlippage_signAware_forLimitAlertsOnly(Alert alert, int slippageIndex=0, bool isStreaming=true) {
		//    return this.GetSlippage_signAware_forLimitOrdersOnly(alert.PriceScriptAligned, alert.Direction, alert.MarketOrderAs, slippageIndex, isStreaming);
		//}

		public double GetSlippage_signAware_forLimitAlertsOnly_NanWhenNoMore(Direction direction, MarketOrderAs crossOrTidal,
					int slippageIndex=0, bool NaN_whenNoMoreSlippagesAvailable = true) {
			double ret = 0;
			List<double> slippagesAvailable = this.GetSlippages_forLimitOrdersOnly(crossOrTidal);
			if (slippagesAvailable.Count == 0) return ret;
			if (slippageIndex < 0) slippageIndex = 0;
			if (slippageIndex >= slippagesAvailable.Count) {
				if (NaN_whenNoMoreSlippagesAvailable) {
					string msg = "NO_MORE_SLIPPAGES_AVAILABLE slippageIndex[" + slippageIndex + "] >= slippagesAvailable.Count[" + slippagesAvailable.Count + "]";
					//throw new Exception(msg);
					return double.NaN;
				} else {
					slippageIndex = slippagesAvailable.Count - 1;
				}
			}
			ret = slippagesAvailable[slippageIndex];

			if (direction == Direction.Short || direction == Direction.Sell) ret = -ret;
			return ret;
		}
		public SymbolInfo Clone() {
			return (SymbolInfo)this.MemberwiseClone();
		}

		public double Alert_alignToPriceStep(double alertPrice, Direction direction, MarketLimitStop marketLimitStop) {
			PriceLevelRoundingMode roundingMode = PriceLevelRoundingMode.RoundToClosest;
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
					string msg = "NO_HANDLER_TO_ROUND_ALERT_PRICE_FOR marketLimitStop[" + marketLimitStop+ "]";
					Assembler.PopupException(msg);
					//roundingMode = PriceLevelRoundingMode.DontRoundPrintLowerUpper;
					break;
			}
			return this.AlignToPriceStep(alertPrice, roundingMode);
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
			public double AlignToPriceLevel(double price, PriceLevelRoundingMode upOrDown = PriceLevelRoundingMode.RoundToClosest) {
				double decimals = this.PriceDecimals;	//this.PriceStep);
				//if (integefier == -1) {
				//    integefier = Convert.ToDecimal(this.PriceDecimals);		//integefier = this.PriceStepFromDecimal;
				//}
				if (decimals < 0) {
					throw new ArithmeticException();
				}
				decimal integefier = Convert.ToDecimal(Math.Pow(10, decimals));	// 10 ^ 2 = 100; 10 ^ 0.01
				decimal ret = (decimal) price * integefier; 					// 90.145 => 9014.5
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
				//if (ret < integefier) {
				//    ret = integefier;
				//} else {
				//    ret = Math.Round(ret, (int)integefier);	// DIRTY
				//}
				return Convert.ToDouble(ret);
			}
		#else
		public double AlignToPriceStep(double price, PriceLevelRoundingMode upOrDown = PriceLevelRoundingMode.RoundToClosest) {
			if (this.PriceStep == 0) {
				string msg = "CAN_NOT_ALIGN_TO_ZERO_PRICE_STEP Symbol[" + this.SymbolClass + "] price[" + price + "]";
				throw new Exception(msg);
			}
			double reminder = price % this.PriceStep;				// 18,562.17 % 10 = 2.17 (reminder);		10.2 (step 0.5) => 0.2
			double level_lower = price - reminder;					// 18,562.17 - 2.17 (reminder) = 18,560;	10.2 - 0.2 (reminder) = 10.0
			double level_upper = level_lower + this.PriceStep;		// 18,560 + 10 = 18,570;					10.0 + 0.5 = 10.5
			double level_closest = reminder >= this.PriceStep/2d ? level_upper : level_lower;
			switch (upOrDown) {
				case PriceLevelRoundingMode.RoundDown:		return level_lower;		// 18,562.17 (step 10) => 18,560; 10.2 (step 0.5) => 10.0
				case PriceLevelRoundingMode.RoundUp:		return level_upper;		// 18,568.17 (step 10) => 18,570; 10.2 (step 0.5) => 10.5
				case PriceLevelRoundingMode.RoundToClosest:	return level_closest;	// 18,568.17 (step 10) => 18,570; 10.6 (step 0.5) => 10.5
				//case PriceLevelRoundingMode.DontRoundPrintLowerUpper:
				//    string msg = "DontRound=>returning[" + price + "] RoundDown[" + level_lower + "] RoundUp[" + level_upper + "] ";
				//    Assembler.PopupException(msg);
				//    return price;
				default:
					throw new NotImplementedException("AlignToPriceLevel() for PriceLevelRoundingMode." + upOrDown);
			}
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
				//case PriceLevelRoundingMode.DontRoundPrintLowerUpper:
				//    double lowerLevel = this.AlignToVolumeStep(volume, PriceLevelRoundingMode.RoundDown);
				//    double upperLevel = this.AlignToVolumeStep(volume, PriceLevelRoundingMode.RoundUp);
				//        string msg = "DontRound=>returning[" + volume + "] RoundDown[" + lowerLevel + "] RoundUp[" + upperLevel + "] ";
				//        Assembler.PopupException(msg);
				//        return volume;
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

		//v1 I_HATE_SUCH_INTRANSPARENCY__ALMOST_INTRODUCED_FOR_List<Symbol>__BUT_IMPLEMENTED__MY_WAY_TO_AVOID_OVERRIDING_EQUALS
		//public override bool Equals(object obj) {
		//	return this.Symbol == (((SymbolInfo))obj));
		//}
		//v2 OK_LETS_TRY
		public override bool Equals(object obj) {
			if (obj == null) return false;
			SymbolInfo obj_asSymbolInfo = obj as SymbolInfo;
			if (obj_asSymbolInfo == null) {
				string msg = "DONT_COMPARE_APPLES_TO_ORANGES_UPSTACK";
				//Assembler.PopupException(msg);
				return false;
			}
			return obj_asSymbolInfo.Symbol == this.Symbol;
		}

		public int AbsorbEvery_JsonProperty_exceptSymbol_from(SymbolInfo symbolInfo_imPullingFrom) {
			int ret = 0;
			if (symbolInfo_imPullingFrom == null) {
				string msg = "CAN_NOT_ABSORB_FROM_SymbolInfo=null //AbsorbEvery_JsonProperty_exceptSymbol_from(NULL)";
				Assembler.PopupException(msg);
				return ret;
			}
			PropertyInfo[] props = this.GetType().GetProperties();
			foreach (PropertyInfo prop in props) {
				//bool propHasJsonAttribute = false;
				//foreach (prop.Attributes) {
				//}
				if (prop.CanWrite == false) continue;
				if (prop.Name.ToUpper() == "SYMBOL") continue;

				try {
					object value_imPullingFrom = prop.GetValue(symbolInfo_imPullingFrom, null);
					prop.SetValue(this, value_imPullingFrom, null);
					ret++;
				} catch (Exception ex) {
					string msg = "GET/SET_REFLECTED_VALUE_ERROR_FOR_PROPERTY[" + prop.Name + "]";
					Assembler.PopupException(msg, ex);
				}
			}
			return ret;
		}
	}
}
