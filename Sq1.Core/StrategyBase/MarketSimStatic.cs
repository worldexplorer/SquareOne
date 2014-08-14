using System;
using System.Collections.Generic;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using log4net;
using System.Reflection;

namespace Sq1.Core.StrategyBase {
	public class MarketSimStatic {
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private ScriptExecutor executor;
		private List<Alert> stopLossesActivatedOnPreviousBars;

		public MarketSimStatic(ScriptExecutor executor) {
			this.executor = executor;
			this.stopLossesActivatedOnPreviousBars = new List<Alert>();
		}
		public void Initialize() {
			this.stopLossesActivatedOnPreviousBars.Clear();
		}

		public Alert EntryAlertCreate(Bar entryBar, double stopOrLimitPrice, string entrySignalName,
				Direction direction, MarketLimitStop entryMarketLimitStop) {
			
			//if (direction == Direction.Buy
			//	 && this.executor._BarsLimitedUpBy(entryBar) == false) return null;
			//if (direction == Direction.Short
			//	&& this.executor._BarsLimitedDownBy(entryBar) == false) return null;

			double entryPriceScript = stopOrLimitPrice;

			int barSmallestAllowed = 0;
			OrderSpreadSide orderSpreadSide = OrderSpreadSide.Unknown;
			switch (entryMarketLimitStop) {
				case MarketLimitStop.Market:
					//if (entryBar >= this.executor.Bars.Count) {
					if (entryBar.IsBarStreaming) {
						if (this.executor.IsStreaming) {
							//basisPrice = 0;
							this.executor.DataSource.StreamingProvider.StreamingDataSnapshot.GetAlignedBidOrAskForTidalOrCrossMarketFromStreaming(
								this.executor.Bars.Symbol, direction, out entryPriceScript, out orderSpreadSide, false);
						} else {
							//basisPrice = this.executor.Bars.LastBar.Open;
							//if (bar > this.executor.Bars.Count) {
							//	throw new Exception("got LastBar.Open while MarketOrder is placed few bars further from LastBar");
							//}
//							entryPriceScript = this.executor.Bars[entryBar - 1].Close;
//							string msg = "[" + direction + "]At[" + entryMarketLimitStop + "](bar=[" + entryBar + "])"
//								+ " when LastBar[" + (this.executor.Bars.Count - 1) + "]; No way I can bring you price from the future, even by executing your order right now"
//								+ "; can't do inequivalent repacement to LastBar.Open";
//							log.Fatal(msg, new Exception(msg));
							string msg = "[" + direction + "]At[" + entryMarketLimitStop + "](bar=[" + entryBar + "])";
							throw new Exception("UNABLE_TO_REFACTOR#1 " + msg);
						}
					} else {
						//entryPriceScript = this.executor.Bars[entryBar].Open;
						entryPriceScript = entryBar.Open;
					}
					barSmallestAllowed = 1;
					break;
				case MarketLimitStop.AtClose:
					//basisPrice = this.executor.Bars[bar - 1].Close;
					//if (entryBar >= this.executor.Bars.Count) { //bar+1
					if (entryBar.IsBarStreaming) {
						entryPriceScript = 0;
						string msg = "[" + direction + "]At[" + entryMarketLimitStop + "](bar=[" + entryBar + "])"
							+ " when LastBar[" + (this.executor.Bars.Count - 1) + "]; No way I can bring you a future price,"
							+ " even by executing your order right now"
							+ "; can't do inequivalent repacement to LastBar.Close";
						log.Fatal(msg, new Exception(msg));
					} else {
						//entryPriceScript = this.executor.Bars[entryBar].Close;
						entryPriceScript = entryBar.Close;
					}
					barSmallestAllowed = 0;
					break;
				case MarketLimitStop.Stop:
					break;
				case MarketLimitStop.Limit:
					break;
				default:
					throw new Exception("no handler for Direction.[" + direction + "]");
			}
			//BAR_NOT_NULL_CHECKED_UPSTACK_IN_ScriptExecutor.BuyOrShortAlertCreateRegister() this.executor._throwWhenBarOutsideInterval(entryBar, barSmallestAllowed, this.executor.Bars.Count, this.executor.Bars);
			//if (entryBar < this.executor.Bars.Count) {
			this.basisPriceZeroCheckThrow(entryPriceScript);
			//}

			PositionLongShort longShortFromDirection = MarketConverter.LongShortFromDirection(direction);
			entryPriceScript = this.executor.AlignAlertPriceToPriceLevel(this.executor.Bars, entryPriceScript, true, longShortFromDirection, entryMarketLimitStop);
			if (entryPriceScript == 0) {
				int a = 1;
			}
			double shares = this.executor.PositionSizeCalculator.CalcPositionSize(entryBar,
				entryPriceScript, longShortFromDirection, this.executor.ScriptExecutorConfig.RiskStopLevel, true);

			Alert alert = new Alert(entryBar, shares, entryPriceScript, entrySignalName,
				direction, entryMarketLimitStop, orderSpreadSide,
				//this.executor.Script,
				this.executor.Strategy);
			alert.AbsorbFromExecutor(executor);

			return alert;
		}
		private bool simulateFillAlertBuyShort(Alert entryAlert, out double entryPriceOut, out double entrySlippageOut, int entryBarToTestOn = -1) {
			// if entry is triggered, call position.EnterFinalize(entryPrice, entrySlippage, entryCommission);
			entryPriceOut = entryAlert.PriceScript;
			entrySlippageOut = 0;

			Direction directionPositionClose = MarketConverter.ExitDirectionFromLongShort(entryAlert.PositionLongShortFromDirection);

			double slippageLimit = this.executor.getSlippage(
				entryAlert.PriceScript, entryAlert.Direction, 0, this.executor.IsStreaming, true);
			double slippageNonLimit = this.executor.getSlippage(
				entryAlert.PriceScript, entryAlert.Direction, 0, this.executor.IsStreaming, false);
			double slippageMarketForClosingPosition = this.executor.getSlippage(
				entryAlert.PriceScript, directionPositionClose, 0, this.executor.IsStreaming, false);

			if (entryBarToTestOn == -1) entryBarToTestOn = entryAlert.Bars.Count;
			if (entryBarToTestOn > entryAlert.Bars.Count) {
				int a = 1;
			}

			double entryPriceDotAligned = this.executor.AlignAlertPriceToPriceLevel(
				entryAlert.Bars, entryAlert.PriceScript, false,
				entryAlert.PositionLongShortFromDirection, entryAlert.MarketLimitStop);

			if (entryBarToTestOn > entryAlert.Bars.Count) return false;
			Bar bar = entryAlert.Bars[entryBarToTestOn];

			switch (entryAlert.MarketLimitStop) {
				case MarketLimitStop.Limit:
					switch (entryAlert.Direction) {
						case Direction.Buy:
							//dont check if (slippage >= 0) throw new Exception("BuyAtLimit: slippage[" + slippage + "] should be negative -");
							if (entryPriceOut + slippageLimit < bar.Low) return false;
							if (entryPriceOut > bar.Open) {
								if (entryAlert.Bars.SymbolInfo.SimBugOutOfBarLimitsFill) {
									entryPriceOut = bar.Open;
									this.executor.AddStringToDebugMessages("BuyAtLimit(bar[" + entryBarToTestOn
										+ "], LimitPrice[" + entryAlert.PriceScript + "], signalName["
										+ entryAlert.PositionAffected.EntrySignal + "]) "
										+ "entryPrice[" + entryPriceOut + "] > := " 
										+ "Open[" + entryBarToTestOn + "]=[" + bar.Open + "]"
										+ " while basisPrice=["
										+ entryAlert.PositionAffected.LastQuoteForMarketOrStopLimitImplicitPrice + "]");
								} else {
									return false;
								}
							}
							break;
						case Direction.Short:
							//dont check if (slippage <= 0) throw new Exception("ShortAtLimit: slippage[" + slippage + "] should be positive +");
							if (entryPriceOut + slippageLimit > bar.High) return false;
							if (bar.Open > entryPriceOut) {
								if (entryAlert.Bars.SymbolInfo.SimBugOutOfBarLimitsFill) {
									entryPriceOut = bar.Open;
									this.executor.AddStringToDebugMessages("ShortAtLimit/ShortAtStop(bar[" + entryBarToTestOn 
										+ "], LimitPrice[" + entryAlert.PriceScript
										+ "], signalName[" + entryAlert.PositionAffected.EntrySignal + "]) "
										+ "entryPrice[" + entryPriceOut + "] < := "
										+ "Open[" + entryBarToTestOn + "]=[" + bar.Open + "]"
										+ " while basisPrice=[" + entryAlert.PositionAffected.LastQuoteForMarketOrStopLimitImplicitPrice + "]");
								} else {
									return false;
								}
							}
							break;
						default:
							throw new Exception("NYI: direction[" + entryAlert.Direction + "] is not Long or Short");
					}
					break;
				case MarketLimitStop.Stop:
					switch (entryAlert.Direction) {
						case Direction.Buy:
							if (entryAlert.PriceScript > bar.High) return false;
							if (entryAlert.Bars.SymbolInfo.SimBugOutOfBarStopsFill) {
								if (entryAlert.PriceScript < bar.Open) {
									entryPriceOut = bar.Open;
									//entryPriceScript = entryPrice;
									this.executor.AddStringToDebugMessages("BuyAtStop(bar[" + entryBarToTestOn
										+ "], stopPrice[" + entryAlert.PriceScript + "], signalName["
										+ entryAlert.PositionAffected.EntrySignal + "])"
										+ " entryPrice[" + entryPriceOut + "] < := Open["
										+ entryBarToTestOn + "]=[" + bar.Open + "]"
										+ " while position.BasisPrice=["
										+ entryAlert.PositionAffected.LastQuoteForMarketOrStopLimitImplicitPrice + "]");
								}
							} else {
								if (entryBarToTestOn < entryAlert.Bars.Count) {
									if (entryAlert.PriceScript < bar.Low) {
										return false;
									}
								}
							}
							entrySlippageOut = slippageNonLimit;
							entryPriceOut += entrySlippageOut;
							entryPriceOut = this.executor.AlignAlertPriceToPriceLevel(entryAlert.Bars, entryPriceOut, false,
								entryAlert.PositionLongShortFromDirection, entryAlert.MarketLimitStop);
							if (entryPriceOut > bar.High) {
								if (entryAlert.Bars.SymbolInfo.SimBugOutOfBarStopsFill) {
									entryPriceOut = bar.High;
									//entryPriceOut += entrySlippageOut;
									entryPriceOut = this.executor.AlignAlertPriceToPriceLevel(entryAlert.Bars, entryPriceOut, false,
										entryAlert.PositionLongShortFromDirection, entryAlert.MarketLimitStop);
									this.executor.AddStringToDebugMessages("BuyAtStop(bar[" + entryBarToTestOn
										+ "], stopPrice[" + entryAlert.PriceScript + "], signalName["
										+ entryAlert.PositionAffected.EntrySignal + "])"
										+ " stopPrice[" + entryPriceOut + "] > := High["
										+ entryBarToTestOn + "]=[" + bar.High + "]"
										+ " while position.BasisPrice=["
										+ entryAlert.PositionAffected.LastQuoteForMarketOrStopLimitImplicitPrice + "]");
								} else {
									return false;
								}
							}
							break;
						case Direction.Short:
							if (entryAlert.PriceScript < bar.Low) return false;
							if (entryAlert.Bars.SymbolInfo.SimBugOutOfBarStopsFill) {
								if (entryAlert.PriceScript > bar.Open) {
									entryPriceOut = bar.Open;
									//entryPriceScript = entryPrice;
									this.executor.AddStringToDebugMessages("ShortAtStop(bar[" + entryBarToTestOn
										+ "], stopPrice[" + entryAlert.PriceScript + "], signalName["
										+ entryAlert.PositionAffected.EntrySignal + "])"
										+ " entryPrice[" + entryPriceOut + "] < := Open["
										+ entryBarToTestOn + "]=[" + bar.Open + "]"
										+ " while position.BasisPrice=["
										+ entryAlert.PositionAffected.LastQuoteForMarketOrStopLimitImplicitPrice + "]");
								}
							} else {
								if (entryBarToTestOn < entryAlert.Bars.Count) {
									if (entryPriceOut > bar.High) {
										return false;
									}
								}
							}
							entrySlippageOut = -slippageNonLimit;
							entryPriceOut += entrySlippageOut;
							entryPriceOut = this.executor.AlignAlertPriceToPriceLevel(entryAlert.Bars, entryPriceOut, false,
								entryAlert.PositionLongShortFromDirection, entryAlert.MarketLimitStop);
							if (entryPriceOut < bar.Low) {
								if (entryAlert.Bars.SymbolInfo.SimBugOutOfBarStopsFill) {
									entryPriceOut = bar.Low;
									//entryPriceOut += entrySlippageOut;
									entryPriceOut = this.executor.AlignAlertPriceToPriceLevel(entryAlert.Bars, entryPriceOut, false,
										entryAlert.PositionLongShortFromDirection, entryAlert.MarketLimitStop);
									this.executor.AddStringToDebugMessages("ShortAtStop(bar[" + entryBarToTestOn
										+ "], stopPrice[" + entryAlert.PriceScript + "], signalName["
										+ entryAlert.PositionAffected.EntrySignal + "])"
										+ " stopPrice[" + entryPriceOut + "] < := Low[" + entryBarToTestOn
										+ "]=[" + bar.Low + "]"
										+ " while position.BasisPrice=["
										+ entryAlert.PositionAffected.LastQuoteForMarketOrStopLimitImplicitPrice + "]");
								} else {
									return false;
								}
							}
							break;
						default:
							throw new Exception("NYI: direction[" + entryAlert.Direction + "] is not Long or Short");
					}
					break;
				case MarketLimitStop.Market:
				case MarketLimitStop.AtClose:
					switch (entryAlert.Direction) {
						case Direction.Buy:
							if (entryAlert.PriceScript > bar.High) {
								entryPriceOut = bar.High;
								this.executor.AddStringToDebugMessages("BuyAtMarket/BuyAtClose(bar[" + entryBarToTestOn
									+ "], signalName[" + entryAlert.PositionAffected.EntrySignal + "]) "
									+ "entryPrice[" + entryPriceOut + "] > := High["
									+ entryBarToTestOn + "]=[" + bar.High + "]"
									+ " while basisPrice=[" + entryAlert.PositionAffected.LastQuoteForMarketOrStopLimitImplicitPrice + "]");
							}
							entrySlippageOut = slippageNonLimit;
							entryPriceOut += entrySlippageOut;
							entryPriceOut = this.executor.AlignAlertPriceToPriceLevel(entryAlert.Bars, entryPriceOut, true,
								entryAlert.PositionLongShortFromDirection, entryAlert.MarketLimitStop);
							break;
						case Direction.Short:
							if (entryAlert.PriceScript < bar.Low) {
								entryPriceOut = bar.Low;
								this.executor.AddStringToDebugMessages("ShortAtMarket/ShortAtClose(bar[" + entryBarToTestOn
									+ "], signalName[" + entryAlert.PositionAffected.EntrySignal + "]) "
									+ "entryPrice[" + entryPriceOut + "] < := Low["
									+ entryBarToTestOn + "]=[" + bar.Low + "]"
									+ " while basisPrice=[" + entryAlert.PositionAffected.LastQuoteForMarketOrStopLimitImplicitPrice + "]");
							}
							entrySlippageOut = -slippageNonLimit;
							entryPriceOut += entrySlippageOut;
							entryPriceOut = this.executor.AlignAlertPriceToPriceLevel(entryAlert.Bars, entryPriceOut, true,
								entryAlert.PositionLongShortFromDirection, entryAlert.MarketLimitStop);
							break;
						default:
							throw new Exception("NYI direction[" + entryAlert.Direction + "]");
					}
					break;
				default:
					throw new Exception("NYI: marketLimitStop[" + entryAlert.MarketLimitStop + "] is not Limit or Stop");
			}
			if (entryPriceOut <= 0) {
				log.Fatal("entryPrice[" + entryPriceOut + "]<=0 what do you mean??? get Bars.LastBar.Close for Market...");
			}

			return true;
		}

		public Alert ExitAlertCreate(Bar exitBar, Position position, double stopOrLimitPrice, string signalName,
				Direction direction, MarketLimitStop exitMarketLimitStop) {
			if (position == null) {
				string msg = "do not SellOrCover(position = null) please";
				throw new Exception(msg);
				return null;
			}
			if (position.NoExitBarOrStreaming == false) return null;

			//Bars bars = position.Bars;
			// backtest substitutes StrategyExecutor.Bars, so exit(bar+1) should point on StreamingBar, not backtested!
			//Bars backtestingOrStreamingBars = this.executor.Bars;

			Direction posDirection = MarketConverter.ExitDirectionFromLongShort(position.PositionLongShort);
			if (posDirection != direction) {
				string msg = "here is the case when you Sell a Short once again or Cover a Long, what's the matter with you?";
				throw new Exception(msg);
			}

			PositionLongShort positionDirection = PositionLongShort.Long;
			switch (direction) {
				case Direction.Sell:
					positionDirection = PositionLongShort.Long;
					break;
				case Direction.Cover:
					positionDirection = PositionLongShort.Short;
					break;
				default:
					throw new Exception("NYI direction[" + direction + "]");
			}
			if (positionDirection != position.PositionLongShort) {
				string msg = "pass positionDirection to AlignAlertPriceToPriceLevel!!! you should close with an opposite alignment";
				throw new Exception(msg);
			}

			double exitPriceScript = stopOrLimitPrice;

			int barSmallestAllowed = 0;
			OrderSpreadSide orderSpreadSide = OrderSpreadSide.Unknown;
			switch (exitMarketLimitStop) {
				case MarketLimitStop.Market:
					//if (exitBar >= this.executor.Bars.Count) {	//bar+1
					if (exitBar.IsBarStreaming) {	//bar+1
						if (this.executor.IsStreaming) {
							//basisPrice = 0;
							this.executor.DataSource.StreamingProvider.StreamingDataSnapshot.GetAlignedBidOrAskForTidalOrCrossMarketFromStreaming(
								this.executor.Bars.Symbol, posDirection, out exitPriceScript, out orderSpreadSide, false);
						} else {
							//basisPrice = this.executor.Bars[bar].Open;
							//basisPrice = this.executor.Bars.LastBar.Close;
							//if (bar > this.executor.Bars.Count) { //bar+2
//							exitPriceScript = this.executor.Bars[exitBar - 1].Close;
//							string msg = "[" + direction + "]At[" + exitMarketLimitStop + "](bar=[" + exitBar + "])"
//								+ " when LastBar[" + (this.executor.Bars.Count - 1) + "]; No way I can bring you"
//								+ " price from the future, even by executing your order right now"
//								+ "; can't do inequivalent repacement to LastBar.Open";
//							log.Fatal(msg, new Exception(msg));
							string msg = "[" + direction + "]At[" + exitMarketLimitStop + "](bar=[" + exitBar + "])";
							throw new Exception("UNABLE_TO_REFACTOR#2 " + msg);
							//}
						}
					} else {
						//exitPriceScript = this.executor.Bars[exitBar].Open;
						exitPriceScript = exitBar.Open;
					}
					barSmallestAllowed = 1;
					break;
				case MarketLimitStop.AtClose:
					//if (exitBar >= this.executor.Bars.Count) { //bar+1
					if (exitBar.IsBarStreaming) { //bar+1
						exitPriceScript = 0;
						string msg = "[" + direction + "]At[" + exitMarketLimitStop + "](bar=[" + exitBar + "])"
							+ " when LastBar[" + this.executor.Bars.BarStaticLast + "]; No way I can bring you price from the future,"
							+ " even by executing your order right now"
							+ "; can't do inequivalent repacement to LastBar.Close";
						log.Fatal(msg, new Exception(msg));
					} else {
						//exitPriceScript = this.executor.Bars[exitBar].Close;
						exitPriceScript = exitBar.Close;
					}
					barSmallestAllowed = 0;
					break;
				case MarketLimitStop.Stop:
					break;
				case MarketLimitStop.Limit:
					break;
				case MarketLimitStop.StopLimit:
					break;
				default:
					throw new Exception("no handler for exitMarketLimitStop.[" + exitMarketLimitStop + "]");
			}
			//BAR_NOT_NULL_CHECKED_UPSTACK_IN_ScriptExecutor.SellOrCoverAlertCreateRegister() this.executor._throwWhenBarOutsideInterval(exitBar, barSmallestAllowed, this.executor.Bars.Count, this.executor.Bars);
			//if (exitBar < this.executor.Bars.Count) {
			if (exitBar.IsBarStreaming == false) {
				this.basisPriceZeroCheckThrow(exitPriceScript);
			}

			//if (position == Position.AllPositions) {
			//	bool result = false;
			//	int num = this.executor.ExecutionDataSnapshot.PositionsOpenNow.Count;
			//	for (int i = this.executor.ExecutionDataSnapshot.PositionsMaster.Count - 1; i >= 0; i--) {
			//		if (num == 0) return result;
			//		bool soldOrCovered = SellOrCover(exitBar, this.executor.ExecutionDataSnapshot.PositionsMaster[i],
			//			stopOrLimitPrice, signalName, direction, exitMarketLimitStop);
			//		if (this.executor.ExecutionDataSnapshot.PositionsMaster[i].NoExitBarOrStreaming && soldOrCovered) { result = true; num--; }
			//	}
			//	return result;
			//}
			//if (exitBar < this.executor.Bars.Count) {
			if (exitBar.IsBarStreaming == false) {
				//BAR_NOT_NULL_CHECKED_UPSTACK_IN_ScriptExecutor.SellOrCoverAlertCreateRegister() this.executor._throwWhenBarOutsideInterval(exitBar, position.EntryBarIndex, position.Bars.Count, position.Bars);
				//switch (direction) {
				//	case Direction.Sell:
				//		if (this.executor._IsDayLimitedDownWithBarsBy(position.Bars, exitBar) == false) return null;
				//		if (position.PositionLongShort != PositionLongShort.Long) return null;
				//		break;
				//	case Direction.Cover:
				//		if (this.executor._IsDayLimitedUpWithBarsBy(position.Bars, exitBar) == false) return null;
				//		if (position.PositionLongShort != PositionLongShort.Short) return null;
				//		break;
				//	default:
				//		throw new Exception("NYI direction[" + direction + "]");
				//}
			}

			PositionLongShort longShortFromDirection = MarketConverter.LongShortFromDirection(direction);
			exitPriceScript = this.executor.AlignAlertPriceToPriceLevel(this.executor.Bars, exitPriceScript, true, longShortFromDirection, exitMarketLimitStop);

			Alert alert = new Alert(exitBar, position.Shares, exitPriceScript, signalName,
				direction, exitMarketLimitStop, orderSpreadSide,
				//this.executor.Script,
				this.executor.Strategy);
			alert.AbsorbFromExecutor(executor);
			alert.PositionAffected = position;
			// moved to CallbackAlertFilled - we can exit by TP or SL - and position has no clue which Alert was filled!!!
			//position.ExitCopyFromAlert(alert);

			return alert;
		}
		private bool simulateFillAlertSellCover(Alert exitAlert, out double exitPriceOut, out double exitSlippageOut, int exitBarToTestOn = -1) {
			double exitPriceDotAligned = this.executor.AlignAlertPriceToPriceLevel(
				exitAlert.Bars, exitAlert.PriceScript, false,
				exitAlert.PositionLongShortFromDirection, exitAlert.MarketLimitStop);
			exitPriceOut = exitPriceDotAligned;
			exitSlippageOut = 0;

			double slippageLimit = this.executor.getSlippage(
				exitAlert.PriceScript, exitAlert.Direction, 0, this.executor.IsStreaming, true);
			double slippageNonLimit = this.executor.getSlippage(
				exitAlert.PriceScript, exitAlert.Direction, 0, this.executor.IsStreaming, false);

			if (exitAlert.PositionAffected.SernoAbs == 12) {
				int a = 1;
			}
			if (exitBarToTestOn == 2) {
				int a = 1;
			}
			if (exitBarToTestOn == -1) exitBarToTestOn = exitAlert.Bars.Count;
			Bar bar = exitAlert.Bars[exitBarToTestOn];
			switch (exitAlert.MarketLimitStop) {
				case MarketLimitStop.Limit:
					if (exitBarToTestOn > exitAlert.Bars.Count) return false;
					switch (exitAlert.Direction) {
						case Direction.Sell:
							//dont check if (slippage < 0) throw new Exception("SellAtLimit: slippage[" + slippage + "] should be positive >=0 +");
							if (exitPriceOut + slippageLimit > bar.High) return false;
							if (exitBarToTestOn < exitAlert.Bars.Count) {
								if (exitPriceOut < bar.Open) {
									if (exitAlert.Bars.SymbolInfo.SimBugOutOfBarLimitsFill) {
										exitPriceOut = bar.Open;
									} else {
										return false;
									}
								}
							}
							break;
						case Direction.Cover:
							//dont check if (slippage > 0) throw new Exception("CoverAtLimit: slippage[" + slippage + "] should be negative <=0 -");
							if (exitPriceOut - slippageLimit < bar.Low) return false;
							if (exitBarToTestOn < exitAlert.Bars.Count) {
								if (bar.Open < exitPriceOut) {
									if (exitAlert.Bars.SymbolInfo.SimBugOutOfBarLimitsFill) {
										exitPriceOut = bar.Open;
									} else {
										return false;
									}
								}
							}
							break;
						default:
							throw new Exception("NYI direction[" + exitAlert.Direction + "]");
					}
					break;
				case MarketLimitStop.Stop:
					if (exitBarToTestOn > exitAlert.Bars.Count) return false;
					switch (exitAlert.Direction) {
						case Direction.Sell:
							if (exitPriceOut < bar.Low) return false;
							if (exitBarToTestOn < exitAlert.Bars.Count) {
								if (bar.Open < exitPriceOut) {
									//exitPrice = bar.Open;
									if (exitAlert.Bars.SymbolInfo.SimBugOutOfBarStopsFill) {
										exitPriceOut = bar.Open;
									} else {
										return false;
									}
								}
							}
							exitSlippageOut = -slippageNonLimit;
							exitPriceOut += exitSlippageOut;
							exitPriceOut = this.executor.AlignAlertPriceToPriceLevel(exitAlert.Bars, exitPriceOut, false,
								exitAlert.PositionLongShortFromDirection, exitAlert.MarketLimitStop);
							break;
						case Direction.Cover:
							if (exitPriceOut > bar.High) return false;
							if (exitBarToTestOn < exitAlert.Bars.Count) {
								if (bar.Open > exitPriceOut) {
									//exitPrice = bar.Open;
									if (exitAlert.Bars.SymbolInfo.SimBugOutOfBarStopsFill) {
										exitPriceOut = bar.Open;
									} else {
										return false;
									}
								}
							}
							exitSlippageOut = slippageNonLimit;
							exitPriceOut += exitSlippageOut;
							exitPriceOut = this.executor.AlignAlertPriceToPriceLevel(exitAlert.Bars, exitPriceOut, false,
								exitAlert.PositionLongShortFromDirection, exitAlert.MarketLimitStop);
							break;
						default:
							throw new Exception("NYI direction[" + exitAlert.Direction + "]");
					}
					break;
				case MarketLimitStop.StopLimit:
					if (exitBarToTestOn > exitAlert.Bars.Count) return false;
					if (exitBarToTestOn < exitAlert.Bars.Count) {
						string msg = "MarketSim is not accurate for Backtester.BacktestMode=SingleStroke";
					}
					double priceStopActivationAligned = this.executor.AlignAlertPriceToPriceLevel(
						exitAlert.Bars, exitAlert.PriceStopLimitActivation, false,
						exitAlert.PositionLongShortFromDirection, exitAlert.MarketLimitStop);
					switch (exitAlert.Direction) {
						case Direction.Sell:
							if (exitPriceOut < priceStopActivationAligned) {
								string msg = "SellLimit is below StopActivation"
									+ ", expecting ~immediate fill between [SellLimit...StopActivation]"
									+ " (if slippage is too tight and price plummets we'll get no fill)"
									+ " ( QUIK model)";
							} else {
								string msg = "SellLimit is below SellStop, expecting (TODO: multibar) bounce back to SellLimit"
									+ " (like on MT5 picture, {Sell stop-limit} case)";
								//http://www.metatrader5.com/en/terminal/help/trading/general_concept/order_types
							}
							// NB! we're getting here 4 times per bar for BacktestingMode.FourStroke
							// so Open stays the same, Volume increases, Close will be zig-zagging each "stroke", High=Max(Close), Low=Min(Close)
							bool SLactivated = this.stopLossesActivatedOnPreviousBars.Contains(exitAlert);
							if (priceStopActivationAligned < bar.Low && SLactivated == false) {
								string msg = "StopActivation is VIRTUALLY not activated by TOUCHED_FROM_ABOVE";
								return false;
							}
							this.stopLossesActivatedOnPreviousBars.Add(exitAlert);
							// yes SellStop is "virtually" activated by "touched from above"
							// (UnitTest: eyeball the current bar)
							if (exitPriceOut > bar.High) {		// + slippageLimit
								string msg = "SellLimit is not filled because whole the bar was below SellLimit"
									+ "; rare case when PositionPrototype.ctor() checks didn't catch it?";
								return false;
							}
							if (exitPriceOut < bar.Low) {
								string msg = "SellLimit wasn't TOUCHED_FROM_ABOVE; staying Active but not filled at this bar"
									+ " (TODO: if on next bar SellLimit is fillable, avoid first StopActivation condition above)";
								return false;
							}

							if (exitPriceOut < priceStopActivationAligned) {
								double slippage = priceStopActivationAligned - exitPriceOut;
								double exitPriceHalfSlippage = exitPriceOut - slippage/2;
								exitPriceHalfSlippage = this.executor.AlignAlertPriceToPriceLevel(
									exitAlert.Bars, exitPriceHalfSlippage, false,
									exitAlert.PositionLongShortFromDirection, exitAlert.MarketLimitStop);
								string msg = "SellLimit[" + exitPriceOut + "] is filled at exitPriceHalfSlippage["
									+ exitPriceHalfSlippage + "]"
									//+ "; not checking for bar boundaries since (priceStopActivationAligned < bar.Low)=true"
									;
								exitPriceOut = exitPriceHalfSlippage;
							} else {
								string msg = "SellLimit is filled exactly at SellLimitPrice[" + exitPriceOut + "]";
								//exitPrice = exitPrice
							}
							//if (exitBarToTestOn == 5) int a = 1;
							if (this.stopLossesActivatedOnPreviousBars.Contains(exitAlert) == false) {
								string msg = "DUPE simulateFillAlertSellCover has previously completely filled Sell StopLimit (StopActivated+Limit) " + exitAlert;
								throw new Exception(msg);
							}
							this.stopLossesActivatedOnPreviousBars.Remove(exitAlert);
							break;

						case Direction.Cover:
							//v1
							//if (priceStopActivationAligned > bar.High) return false;
							//if (exitPrice - slippageLimit < bar.Low) return false;
							//v2
							if (exitPriceOut > priceStopActivationAligned) {
								string msg = "BuyLimit is above StopActivation"
									+ ", expecting ~immediate fill between [StopActivation...BuyLimit]"
									+ " (if slippage is too tight and price rockets we'll get no fill)"
									+ " ( QUIK model)";
							} else {
								string msg = "BuyLimit is below SellStop, expecting (TODO: multibar) bounce back to BuyLimit"
									+ " (like on MT5 picture, {Sell stop-limit} case)";
								//http://www.metatrader5.com/en/terminal/help/trading/general_concept/order_types
							}
							// NB! we're getting here 4 times per bar for BacktestingMode.FourStroke so HLCV will be zig-zagging each "stroke"
							if (priceStopActivationAligned > bar.High && this.stopLossesActivatedOnPreviousBars.Contains(exitAlert) == false) {
								string msg = "StopActivation is VIRTUALLY not activated by TOUCHED_FROM_BELOW";
								return false;
							}
							this.stopLossesActivatedOnPreviousBars.Add(exitAlert);
							// yes SellStop is "virtually" activated by "touched from above" (UnitTest: eyeball the current bar)
							if (exitPriceOut < bar.Low) {		// - slippageLimit
								string msg = "BuyLimit is not filled because whole the bar was above BuyLimit"
									+ "; rare case when PositionPrototype.ctor() checks didn't catch it?";
								return false;
							}
							if (exitPriceOut > bar.High) {
								string msg = "BuyLimit wasn't TOUCHED_FROM_BELOW; staying Active but not filled at this bar"
									+ " (TODO: if on next bar BuyLimit is fillable, avoid first StopActivation condition above)";
								return false;
							}

							if (exitPriceOut > priceStopActivationAligned) {
								double slippage = exitPriceOut - priceStopActivationAligned;
								double exitPriceHalfSlippage = exitPriceOut - slippage/2;
								exitPriceHalfSlippage = this.executor.AlignAlertPriceToPriceLevel(
									exitAlert.Bars, exitPriceHalfSlippage, false,
									exitAlert.PositionLongShortFromDirection, exitAlert.MarketLimitStop);
								string msg = "BuyLimit[" + exitPriceOut + "] is filled at exitPriceHalfSlippage["
									+ exitPriceHalfSlippage + "]"
									//+ "; not checking for bar boundaries since (priceStopActivationAligned > bar.High)=true"
									;
								exitPriceOut = exitPriceHalfSlippage;
							} else {
								string msg = "BuyLimit is filled exactly at BuyLimitPrice[" + exitPriceOut + "]";
								//exitPrice = exitPrice
							}
							break;
							if (this.stopLossesActivatedOnPreviousBars.Contains(exitAlert) == false) {
								string msg = "DUPE simulateFillAlertSellCover has previously completely filled Cover StopLimit (StopActivated+Limit) " + exitAlert;
								throw new Exception(msg);
							}
							this.stopLossesActivatedOnPreviousBars.Remove(exitAlert);
						default:
							throw new Exception("NYI direction[" + exitAlert.Direction + "]");
					}
					break;
				case MarketLimitStop.Market:
				case MarketLimitStop.AtClose:
					switch (exitAlert.Direction) {
						case Direction.Sell:
							exitSlippageOut = -slippageNonLimit;
							exitPriceOut += exitSlippageOut;
							exitPriceOut = this.executor.AlignAlertPriceToPriceLevel(exitAlert.Bars, exitPriceOut, false,
								exitAlert.PositionLongShortFromDirection, exitAlert.MarketLimitStop);
							break;
						case Direction.Cover:
							exitSlippageOut = slippageNonLimit;
							exitPriceOut += exitSlippageOut;
							exitPriceOut = this.executor.AlignAlertPriceToPriceLevel(exitAlert.Bars, exitPriceOut, false,
								exitAlert.PositionLongShortFromDirection, exitAlert.MarketLimitStop);
							break;
						default:
							throw new Exception("NYI direction[" + exitAlert.Direction + "]");
					}
					break;
				default:
					throw new Exception("NYI marketLimitStop[" + exitAlert.MarketLimitStop + "]");
			}

			return true;
		}

		public bool SimulateRealtimeOrderFill(Order order, out bool abortTryFill, out string abortTryFillReason) {
			abortTryFill = false;
			abortTryFillReason = "ABORT_NO_REASON_SO_SHOULD_CONTINUE_TRY_FILL";
			Alert alert = order.Alert;
			if (alert.DataSource.BrokerProviderName.Contains("Mock") == false) {
				string msg = "SimulateRealtimeOrderFill() should be called only from BrokerProvidersName.Contains(Mock)"
					+ "; here you have MOCK Realtime Streaming and Broker,"
					+ " it's not a time-insensitive QuotesFromBar-generated Streaming Backtest"
					+ " (both are routed to here, MarketSim, hypothetical order execution)";
				throw new Exception(msg);
			}

			if (alert.PositionAffected == null) {
				string msg = "alert always has a PositionAffected, even for OnChartManual Buy/Short Market/Stop/Limit";
				throw new Exception(msg);
			}

			bool filled = false;
			double priceFill = -1;
			double slippageFill = -1;
			if (alert.IsEntryAlert) {
				if (alert.PositionAffected.IsEntryFilled) {
					string msg = "PositionAffected.EntryFilled => did you create many threads in your QuikTerminalMock?";
					throw new Exception(msg);
				}
				filled = this.simulateFillAlertBuyShort(alert, out priceFill, out slippageFill);
			} else {
				if (alert.PositionAffected.IsEntryFilled == false) {
					string msg = "I refuse to tryFill an ExitOrder because ExitOrder.Alert.PositionAffected.EntryFilled=false";
					throw new Exception(msg);
				}
				if (alert.PositionAffected.IsExitFilled) {
					string msg = null;
					if (alert.PositionAffected.IsExitFilledWithPrototypedAlert) {
						msg = "ExitAlert already filled and Counterparty.Status=["
							+ alert.PositionAffected.PrototypedExitCounterpartyAlert.OrderFollowed.State + "] (does it look OK for you?)";
					} else {
						msg = "I refuse to tryFill non-Prototype based ExitOrder having PositionAffected.IsExitFilled=true";
					}
					abortTryFill = true;
					abortTryFillReason = msg;
					// abortTryFillReason goes to the order.Message inside the caller
					//this.executor.ThrowPopup(new Exception(msg));
					return false;
				}
				filled = this.simulateFillAlertSellCover(alert, out priceFill, out slippageFill);
			}
			return filled;
		}

//		[Obsolete]
//		public int DEPRECATEDSimulateBacktestPendingAlertsFill() {
//			if (this.executor.BacktesterFacade.IsBacktestingNow == false) {
//				string msg = "SimulateBacktest*() should not be used for RealTime BrokerProviders and RealTime Mocks!"
//					+ " make sure you invoked executor.CallbackAlertFilledInvokeScript() from where you are now";
//				throw new Exception(msg);
//			}
//			if (this.executor.ExecutionDataSnapshot.AlertsPending.Count == 0) return 0;
//			int currentBarToTestOn = this.executor.Bars.Count;
//			// there is no userlevel API to kill orders; in your Script, you operate SellAt/BuyAt; protoActivator.StopMove
//			// killing orders is the privilege of Realtime: OrderManager kills orders by
//			// 1) protoActivator.StopLossNewNegativeOffsetUpdateActivate or 2) user clicks in GUI
//			//int alertsKilled = this.simulateBacktestPendingAlertsKill(currentBarToTestOn);
//			int exitsFilled = this.DEPRECATEDsimulateBacktestPendingAlertsFillExit(currentBarToTestOn);
//			int entriesFilled = this.DEPRECATEDsimulateBacktestPendingAlertsFillEntry(currentBarToTestOn);
//			return entriesFilled + exitsFilled;
//		}
//		[Obsolete]
//		private int DEPRECATEDsimulateBacktestPendingAlertsFillEntry(int entryBarToTestOn) {
//			if (this.executor.ExecutionDataSnapshot.AlertsPending.Count == 0) return 0;
//			int filledAlerts = 0;
//			double priceFill = -1;
//			double slippageFill = -1;
//			//List<Alert> filled2beRemoved = new List<Alert>();
//			if (this.executor.ExecutionDataSnapshot.AlertsPending == null) {
//				int a = 1;
//			}
//			List<Alert> alertsPendingSafeCopy = new List<Alert>(this.executor.ExecutionDataSnapshot.AlertsPending);
//			foreach (Alert alert in alertsPendingSafeCopy) {
//				if (alert.IsEntryAlert == false) continue;
//				// (i don't have any simultaneous EntryAlerts for a bar, but)
//				// once EntryAlerts is filled it's removed from Snap; however we have it in the copy so we need to skip it
//				if (alert.IsFilled == true) continue;
//				if (alert.PositionAffected == null) continue;
//				if (alert.PositionAffected.IsEntryFilled) continue;
//				bool filled = this.simulateFillAlertBuyShort(alert, out priceFill, out slippageFill, entryBarToTestOn);
//				if (filled) {
//					//filled2beRemoved.Add(alert);
//					filledAlerts++;
//
//					double entryCommission = this.executor.getCommissionForPosition(alert.Direction,
//						alert.MarketLimitStop, priceFill, alert.Qty, alert.Bars);
//					this.executor.CallbackAlertFilledMoveAroundInvokeScript(alert, null, entryBarToTestOn,
//						priceFill, alert.Qty, slippageFill, entryCommission);
//				}
//			}
//			//foreach (Alert alert in filled2beRemoved) {
//			//	this.executor.ExecutionDataSnapshot.AlertsPendingRemove(alert);
//			//}
//			return filledAlerts;
//		}
//		[Obsolete]
//		private int DEPRECATEDsimulateBacktestPendingAlertsFillExit(int exitBarToTestOn) {
//			if (this.executor.ExecutionDataSnapshot.AlertsPending.Count == 0) return 0;
//			int filledPositions = 0;
//			//List<Alert> filled2beRemoved = new List<Alert>();
//			if (this.executor.ExecutionDataSnapshot.AlertsPending == null) {
//				int a = 1;
//			}
//			List<Alert> alertsPendingSafeCopy = new List<Alert>(this.executor.ExecutionDataSnapshot.AlertsPending);
//			foreach (Alert alert in alertsPendingSafeCopy) {
//				if (alert.IsExitAlert == false) continue;
//				// (i don't have any simultaneous EntryAlerts for a bar, but)
//				// once EntryAlerts is filled it's removed from Snap; however we have it in the copy so we need to skip it
//				if (alert.IsFilled == true) continue;
//				if (alert.PositionAffected == null) continue;
//				// moved ExitAlert initialization in CallbackAlertFilled
//				//if (alert.PositionAffected.ExitAlert == null) continue;
//				if (alert.PositionAffected.IsEntryFilled == false) continue;
//				if (alert.PositionAffected.IsExitFilled) {
//					// alert.PositionAffected.ExitPrice was set by DEPRECATEDsimulateBacktestPendingAlertsFillExit() already
//					//string msg = "ExecutionDataSnapshot.AlertsPending should not contain alert(s.PositionAffected.ExitFilled=true; position=" + alert.PositionAffected;
//					//throw new Exception(msg);
//					//TODO: cleanup this mess below; trying SimulateFill will throw coz ExitBar!=-1 already
//					if (alert.PositionAffected.IsExitFilledWithPrototypedAlert) {
//						string msg = "Won't execute ExitFill since PositionAffected.ExitFilled"
//							+ "; possibly simulation glitch and AlertsPending should not contain Annihilated counterparty"
//							+ "; Both StopLoss and TakeProfit can be executed on this bar"
//							+ " (who's first will depend on MarketRealTime price fluctuations);"
//							+ " position=" + alert.PositionAffected
//							+ " SecondAlertButMightBeFirst=" + alert;
//						this.executor.PopupException(msg);
//						continue;
//					} else {
//						string msg = "PositionAffected.ExitFilled and not by Prototype!"
//							+ " should skip&continue but I'm reporting an Exception instead...";
//						this.executor.PopupException(msg);
//					}
//				}
//
//				double exitPrice = -1;
//				double exitSlippage = 1;
//				bool filled = this.simulateFillAlertSellCover(alert, out exitPrice, out exitSlippage, exitBarToTestOn);
//				if (filled) {
//					//filled2beRemoved.Add(alert);
//					filledPositions++;
//
//					// we are skipping creating an Order and filling position line filled order data gets transmitted into a position
//					double exitCommission = this.executor.getCommissionForPosition(
//						alert.Direction, alert.MarketLimitStop, exitPrice, alert.Qty, alert.Bars);
//
//					this.executor.CallbackAlertFilledMoveAroundInvokeScript(alert, null, exitBarToTestOn,
//						exitPrice, alert.Qty, exitSlippage, exitCommission);
//
//					// quick check
//					if (alert.PositionAffected.ExitPrice != alert.PositionAffected.ExitPriceScript + alert.PositionAffected.ExitSlippage
//							|| alert.PositionAffected.ExitSlippage == exitSlippage) {
//						int a = 1;
//					}
//					if (alert.PriceScript != exitPrice) {
//						int a = 1;
//					}
//					if (alert.PositionAffected.ExitSlippage == 0) {
//						int a = 1;
//					}
//				}
//			}
//			//foreach (Alert alert in filled2beRemoved) {
//			//	this.executor.ExecutionDataSnapshot.AlertsPendingRemove(alert);
//			//}
//			return filledPositions;
//		}

		private void basisPriceZeroCheckThrow(double _basisPrice) {
			if (_basisPrice == 0.0) {
				throw new ArgumentException("Can't calculate PositionSize: Basis price for Position entry cannot be zero");
			}
		}
//		[Obsolete]
//		public bool DEPRECATEDCoverAtTrailingStop(int exitBar, Position position, double stopPrice, string signalName) {
//			if (position == null || !position.NoExitBarOrStreaming) return false;
//			if (position == null) return false;
//			if (position == Position.AllPositions) {
//				bool result = false;
//				int num = this.executor.ExecutionDataSnapshot.PositionsOpenNow.Count;
//				int num2 = this.executor.ExecutionDataSnapshot.PositionsMaster.Count - 1;
//				while (num2 >= 0 && num != 0) {
//					if (this.executor.ExecutionDataSnapshot.PositionsMaster[num2].NoExitBarOrStreaming
//						&& this.DEPRECATEDCoverAtTrailingStop(exitBar, this.executor.ExecutionDataSnapshot.PositionsMaster[num2], stopPrice, signalName)) {
//						result = true;
//						num--;
//					}
//					num2--;
//				}
//				return result;
//			}
//			//if (!this.executor._IsDayLimitedUpWithBarsBy(position.Bars, exitBar)) return false;
//			double stopPriceAligned = this.executor.AlignAlertPriceToPriceLevel(position.Bars, stopPrice, false, PositionLongShort.Short, MarketLimitStop.Stop);
//			if (stopPriceAligned < position.TrailingStop || position.TrailingStop == 0.0) {
//				position.TrailingStop = stopPriceAligned;
//			}
//			stopPriceAligned = position.TrailingStop;
//			this.executor._throwWhenBarOutsideInterval(exitBar, position.EntryBarIndex, position.Bars.Count, position.Bars);
//			if (position.PositionLongShort != PositionLongShort.Short) {
//				return false;
//			}
//
//			Alert alert = new Alert(position.Bars, exitBar, position.Shares, stopPriceAligned, signalName,
//				Direction.Cover, MarketLimitStop.Stop, OrderSpreadSide.Unknown,
//				//this.executor.Script, 
//				this.executor.Strategy);
//			alert.AbsorbFromExecutor(executor);
//
//			alert.PositionAffected = position;
//			position.ExitCopyFromAlert(alert);
//
//			if (exitBar == this.executor.Bars.Count) {
//				this.executor.ExecutionDataSnapshot.AlertEnrichedRegister(alert);
//				return false;
//			}
//			//if (this.executor.Renderer != null && this.executor.Renderer.PlotStops) {	// && position.Bars == this.executor.barsInitialContext
//			//	this.executor.Renderer.AddPendingAlertDot(exitBar, stopPriceAligned, Direction.Cover);
//			//}
//			if (stopPriceAligned > position.Bars[exitBar].High) {
//				return false;
//			}
//			if (position.Bars[exitBar].Open > stopPriceAligned) {
//				stopPriceAligned = position.Bars[exitBar].Open;
//			}
//			double exitPrice = stopPriceAligned;
//			if (this.executor.ScriptExecutorConfig.EnableSlippage) {
//				exitPrice += this.executor.getSlippage(stopPriceAligned, Direction.Cover, 0, this.executor.IsStreaming, false);
//				exitPrice = this.executor.AlignAlertPriceToPriceLevel(position.Bars, exitPrice, false, PositionLongShort.Short, MarketLimitStop.Stop);
//			}
//
//			double exitCommission = this.executor.getCommissionForPosition(
//				Direction.Cover, position.ExitMarketLimitStop, position.ExitPrice, position.Shares, position.Bars);
//
//			this.executor.CallbackAlertFilledMoveAroundInvokeScript(alert, null, exitBar,
//				exitPrice, position.Shares, 0, exitCommission);
//
//			return true;
//		}
//		[Obsolete]
//		public bool DEPRECATEDSellAtTrailingStop(int exitBar, Position position, double stopPrice, string signalName) {
//			if (position == null || !position.NoExitBarOrStreaming) return false;
//			if (position == null) return false;
//			if (position == Position.AllPositions) {
//				bool result = false;
//				int num = this.executor.ExecutionDataSnapshot.PositionsOpenNow.Count;
//				int num2 = this.executor.ExecutionDataSnapshot.PositionsMaster.Count - 1;
//				while (num2 >= 0 && num != 0) {
//					if (this.executor.ExecutionDataSnapshot.PositionsMaster[num2].NoExitBarOrStreaming && this.DEPRECATEDSellAtTrailingStop(exitBar, this.executor.ExecutionDataSnapshot.PositionsMaster[num2], stopPrice, signalName)) {
//						result = true;
//						num--;
//					}
//					num2--;
//				}
//				return result;
//			}
//			//if (!this.executor._IsDayLimitedDownWithBarsBy(position.Bars, exitBar)) return false;
//			double stopPriceAligned = this.executor.AlignAlertPriceToPriceLevel(position.Bars, stopPrice, false, PositionLongShort.Long, MarketLimitStop.Stop);
//			if (stopPriceAligned > position.TrailingStop) {
//				position.TrailingStop = stopPriceAligned;
//			}
//			stopPriceAligned = position.TrailingStop;
//			this.executor._throwWhenBarOutsideInterval(exitBar, position.EntryBarIndex, position.Bars.Count, position.Bars);
//			if (position.PositionLongShort != PositionLongShort.Long) {
//				return false;
//			}
//
//			Alert alert = new Alert(position.Bars, exitBar, position.Shares, stopPriceAligned, signalName,
//				Direction.Sell, MarketLimitStop.Stop, OrderSpreadSide.Unknown,
//				//this.executor.Script,
//				this.executor.Strategy);
//			alert.AbsorbFromExecutor(executor);
//
//			alert.PositionAffected = position;
//			position.ExitCopyFromAlert(alert);
//
//			if (exitBar == this.executor.Bars.Count) {
//				this.executor.ExecutionDataSnapshot.AlertEnrichedRegister(alert);
//				return false;
//			}
//			//if (this.executor.Renderer != null && this.executor.Renderer.PlotStops) {	//  && position.Bars == this.executor.barsInitialContext
//			//	this.executor.Renderer.AddPendingAlertDot(exitBar, stopPriceAligned, Direction.Sell);
//			//}
//			if (stopPriceAligned < position.Bars[exitBar].Low) {
//				return false;
//			}
//			if (position.Bars[exitBar].Open < stopPriceAligned) {
//				stopPriceAligned = position.Bars[exitBar].Open;
//			}
//			double exitPrice = stopPriceAligned;
//			if (this.executor.ScriptExecutorConfig.EnableSlippage) {
//				exitPrice -= this.executor.getSlippage(stopPriceAligned, Direction.Sell, 0, this.executor.IsStreaming, false);
//				exitPrice = this.executor.AlignAlertPriceToPriceLevel(position.Bars, exitPrice, false, PositionLongShort.Long, MarketLimitStop.Stop);
//			}
//
//			double exitCommission = this.executor.getCommissionForPosition(
//				Direction.Sell, position.ExitMarketLimitStop, position.ExitPrice, position.Shares, position.Bars);
//
//			this.executor.CallbackAlertFilledMoveAroundInvokeScript(alert, null, exitBar,
//				exitPrice, position.Shares, 0, exitCommission);
//
//			return true;
//		}
	}
}