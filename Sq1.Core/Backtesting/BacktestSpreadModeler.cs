using System;
using System.Diagnostics;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Backtesting {
	public abstract class BacktestSpreadModeler {
		public abstract double GenerateFillBidAskSymmetrically(QuoteGenerated quote, double medianPrice, Bar barSimulated);
		public abstract void GenerateFillAskBasedOnBid(QuoteGenerated quote);
		public abstract void GenerateFillBidBasedOnAsk(QuoteGenerated quote);

		public bool AlignToPriceLevel;
		
		public BacktestSpreadModeler(bool alignToPriceLevel = true) {
			AlignToPriceLevel = alignToPriceLevel;
		}
		
		public void GeneratedQuoteFillBidAsk(QuoteGenerated quote, Bar barSimulated, double priceFromAlignedBarForSymmetricFillAtOpenOrClose = -1) {
			if (quote == null) return;

			if (quote.ItriggeredFillAtBidOrAsk != BidOrAsk.UNKNOWN) {
				string msg = "I_REFUSE_TO_GENERATE_BIDASK THIS_QUOTE_ALREADY_TRIGGERED_SOME_FILL";
				Assembler.PopupException(msg);
				return;
			}

			if (double.IsNaN(quote.Bid) == false && double.IsNaN(quote.Ask) == false) {
				string msg = "I_REFUSE_TO_GENERATE_BIDASK THIS_QUOTE_ALREADY_HAS_BID_AND_ASK";
				Assembler.PopupException(msg);
				return;
			}

			if (double.IsNaN(quote.Bid) && double.IsNaN(quote.Ask)) {
				string msg = "WARNING_IMPRECISE_QUOTE_MODELING: at Open or Close stroke when I don't have to keep"
					+ " bar boundaries very precise; check generateNewQuoteChildrenHelper() for BidOrAsk=UNKNOWN";
				//Assembler.PopupException(msg);
				
				double spreadAligned = this.GenerateFillBidAskSymmetrically(quote, priceFromAlignedBarForSymmetricFillAtOpenOrClose, barSimulated);
				
				// QUOTEGEN_PROBLEM#2 : at Open/Close, when they are == to Low/High, the Symmetrical quote will go beoynd bar boundaries => MarketSim will freak out
				if (quote.Ask > barSimulated.High) {
					double pushDown = quote.Ask - barSimulated.High;
					double pushDownAligned = barSimulated.ParentBars.SymbolInfo.AlignToPriceLevel(pushDown, PriceLevelRoundingMode.RoundToClosest);
					quote.Bid -= pushDown;
					quote.Ask -= pushDown;
					//quote.Bid = Math.Round(quote.Bid, barSimulated.ParentBars.SymbolInfo.DecimalsPrice);
					//quote.Ask = Math.Round(quote.Ask, barSimulated.ParentBars.SymbolInfo.DecimalsPrice);
					quote.Bid = barSimulated.ParentBars.SymbolInfo.AlignToPriceLevel(quote.Bid, PriceLevelRoundingMode.RoundToClosest);
					quote.Ask = barSimulated.ParentBars.SymbolInfo.AlignToPriceLevel(quote.Ask, PriceLevelRoundingMode.RoundToClosest);
				}
				if (quote.Bid < barSimulated.Low) {
					double pushUp = barSimulated.Low - quote.Bid;
					quote.Bid += pushUp;
					quote.Ask += pushUp;
					//quote.Bid = Math.Round(quote.Bid, barSimulated.ParentBars.SymbolInfo.DecimalsPrice);
					//quote.Ask = Math.Round(quote.Ask, barSimulated.ParentBars.SymbolInfo.DecimalsPrice);
					quote.Bid = barSimulated.ParentBars.SymbolInfo.AlignToPriceLevel(quote.Bid, PriceLevelRoundingMode.RoundToClosest);
					quote.Ask = barSimulated.ParentBars.SymbolInfo.AlignToPriceLevel(quote.Ask, PriceLevelRoundingMode.RoundToClosest);
				}
				return;
			}

			if (double.IsNaN(quote.Bid)) {
				this.GenerateFillAskBasedOnBid(quote);
				return;
			}
			if (double.IsNaN(quote.Ask)) {
				this.GenerateFillBidBasedOnAsk(quote);
				return;
			}			
		}
		public void AlignBidAskToPriceLevel(QuoteGenerated quote, PriceLevelRoundingMode upOrDown = PriceLevelRoundingMode.DontRoundPrintLowerUpper,
											double spreadAlignedToMaintain = -1, double dontGoBeyond = -1) {
			string msig = " " + this.GetType().Name + ".AlignBidAskToPriceLevel(" + quote.ToString() + ")";
			try {
				CheckThrowCanReachSymbolInfo(quote);
			} catch (Exception ex) {
				Assembler.PopupException(ex.Message + msig);
				return;
			}
			SymbolInfo symbolInfo = quote.ParentBarSimulated.ParentBars.SymbolInfo;
			
			double bidAligned = symbolInfo.AlignToPriceLevel(quote.Bid, upOrDown); 
			double askAligned = symbolInfo.AlignToPriceLevel(quote.Ask, upOrDown);

			if (dontGoBeyond != -1) {
				switch(upOrDown) {
					case PriceLevelRoundingMode.RoundDown:
						if (bidAligned < dontGoBeyond) bidAligned = dontGoBeyond; 
						break;
					case PriceLevelRoundingMode.RoundUp:
						if (askAligned > dontGoBeyond) askAligned = dontGoBeyond; 
						break;
					default:
						// upOrDown=PriceLevelRoundingMode.DontRound will PopupException(rounding results) in AlignToPriceLevel() above; KISS
						break;
				}
			}
			
			if (spreadAlignedToMaintain != -1) {
				double spreadAligned = spreadAlignedToMaintain;
				// DONT_ROUND_UP_ALREADY_ALIGNED_UPSTACK
				spreadAligned = symbolInfo.AlignToPriceLevel(spreadAlignedToMaintain, PriceLevelRoundingMode.RoundToClosest);	//changed to RoundToClosest and checking below; RoundUp so I wont' get spread = 0
				if (spreadAligned == 0) {
					//string msg = "you can't use RoundDown here";
					//Debugger.Break();
					spreadAligned = symbolInfo.PriceMinimalStepFromDecimal;
				}
				if (quote.SpreadAligned < spreadAligned) {
					switch(upOrDown) {
						case PriceLevelRoundingMode.RoundDown:
							askAligned = bidAligned + spreadAligned; 
							break;
						case PriceLevelRoundingMode.RoundUp:
							bidAligned = askAligned - spreadAligned; 
							break;
						case PriceLevelRoundingMode.RoundToClosest:
							askAligned = bidAligned + spreadAligned; 
							break;
						default:
							// upOrDown=PriceLevelRoundingMode.DontRound will PopupException(rounding results) in AlignToPriceLevel() above; KISS
							break;
					}
				}

				double quoteSreadAligned = symbolInfo.AlignToPriceLevel(askAligned - bidAligned, PriceLevelRoundingMode.RoundUp);
				if (quoteSreadAligned <= 0) {
					//Debugger.Break();
					bidAligned = symbolInfo.AlignToPriceLevel(quote.Bid, PriceLevelRoundingMode.RoundDown); 
					askAligned = symbolInfo.AlignToPriceLevel(quote.Ask, PriceLevelRoundingMode.RoundUp);
					double quoteSreadAligned2 = symbolInfo.AlignToPriceLevel(askAligned - bidAligned, PriceLevelRoundingMode.RoundUp);
					if (quoteSreadAligned2 > spreadAligned) {
						string msg = "place to cheat it once again to avoid ContainsBidAskForQuoteGenerated() to fail us";
						Assembler.PopupException(msg);
					}
				}
			}

			quote.Bid = bidAligned; 
			quote.Ask = askAligned;
		}
		protected SymbolInfo CheckThrowCanReachSymbolInfo(QuoteGenerated quote) {
			if (quote == null) {
				string msg = "REFUSE_TO_ALIGN: DONT_PASS_QUOTE_NULL";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			if (quote.ParentBarSimulated == null) {
				string msg = "REFUSE_TO_ALIGN: quote.ParentBarSimulated=null";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			if (quote.ParentBarSimulated.ParentBars == null) {
				string msg = "REFUSE_TO_ALIGN: quote.ParentBarSimulated.ParentBars=null";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			if (quote.ParentBarSimulated.ParentBars.SymbolInfo == null) {
				string msg = "REFUSE_TO_ALIGN: quote.ParentBarSimulated.ParentBars.SymbolInfo=null";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			SymbolInfo ret = quote.ParentBarSimulated.ParentBars.SymbolInfo;
			return ret;
		}
	}
}
