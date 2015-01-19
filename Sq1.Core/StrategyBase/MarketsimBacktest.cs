using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.StrategyBase {
	public class MarketsimBacktest {
		ScriptExecutor executor;
		List<Alert> stopLossesActivatedOnPreviousQuotes;
		public bool FillOutsideQuoteSpreadParanoidCheckThrow { get; private set; }

		public MarketsimBacktest(ScriptExecutor executor) {
			this.executor = executor;
			this.stopLossesActivatedOnPreviousQuotes = new List<Alert>();
		}
		public void Initialize(bool fillOutsideQuoteSpreadParanoidCheckThrow = false) {
			this.stopLossesActivatedOnPreviousQuotes.Clear();
			this.FillOutsideQuoteSpreadParanoidCheckThrow = fillOutsideQuoteSpreadParanoidCheckThrow;
		}
		public bool CheckEntryAlertWillBeFilledByQuote(Alert entryAlert, Quote quote, out double entryPriceOut, out double entrySlippageOutNotUsed) {
			// if entry is triggered, call position.EnterFinalize(entryPrice, entrySlippage, entryCommission);
			//v2
			entryPriceOut = entryAlert.PriceScriptAligned;

			#if DEBUG	// REMOVE_ONCE_NEW_ALIGNMENT_MATURES_NOVEMBER_15TH_2014
			//v1
			double entryPriceOut1 = this.executor.AlignAlertPriceToPriceLevel(entryAlert.Bars, entryAlert.PriceScript, true,
				entryAlert.PositionLongShortFromDirection, entryAlert.MarketLimitStop);
			if (entryPriceOut1 != entryAlert.PriceScriptAligned) {
				string msg = "FIX_Alert.PriceScriptAligned";
				Debugger.Break();
			} else {
				string msg = "GET_RID_OF_COMPLEX_ALIGNMENT executor.AlignAlertPriceToPriceLevel()";
			}
			#endif

			entrySlippageOutNotUsed = 0;
			//Direction directionPositionClose = MarketConverter.ExitDirectionFromLongShort(alert.PositionLongShortFromDirection);
			//double slippageLimit = this.executor.getSlippage(
			//	alert.PriceScript, alert.Direction, 0, this.executor.IsStreaming, true);
			//double slippageNonLimit = this.executor.getSlippage(
			//	alert.PriceScript, alert.Direction, 0, this.executor.IsStreaming, false);
			//double slippageMarketForClosingPosition = this.executor.getSlippage(
			//	alert.PriceScript, directionPositionClose, 0, this.executor.IsStreaming, false);

			int barIndexTested = entryAlert.Bars.Count - 1;
			switch (entryAlert.MarketLimitStop) {
				case MarketLimitStop.Limit:
					switch (entryAlert.Direction) {
						case Direction.Buy:
							//dont check if (slippage >= 0) throw new Exception("BuyAtLimit: slippage[" + slippage + "] should be negative -");
							//if (priceScriptAligned + slippageLimit < quoteToReach.Bid) return false;
							if (entryPriceOut < quote.Bid) return false;
							break;
						case Direction.Short:
							//dont check if (slippage <= 0) throw new Exception("ShortAtLimit: slippage[" + slippage + "] should be positive +");
							//if (priceScriptAligned + slippageLimit > quoteToReach.Ask) return false;
							if (entryPriceOut > quote.Ask) return false;
							break;
						default:
							#if DEBUG
							Debugger.Break();
							#endif
							throw new Exception("NYI: direction[" + entryAlert.Direction + "] is not Long or Short");
					}
					break;
				case MarketLimitStop.Stop:
					switch (entryAlert.Direction) {
						case Direction.Buy:
							if (entryPriceOut > quote.Ask) return false;
							//entrySlippageOutNotUsed = slippageNonLimit;
							//priceScriptAligned += entrySlippageOutNotUsed;
							break;
						case Direction.Short:
							if (entryPriceOut < quote.Bid) return false;
							//entrySlippageOutNotUsed = -slippageNonLimit;
							//priceScriptAligned += entrySlippageOutNotUsed;
							break;
						default:
							#if DEBUG
							Debugger.Break();
							#endif
							throw new Exception("NYI: direction[" + entryAlert.Direction + "] is not Long or Short");
					}
					//entryPriceOut = this.executor.AlignAlertPriceToPriceLevel(entryAlert.Bars, entryPriceOut, true,
					//	entryAlert.PositionLongShortFromDirection, entryAlert.MarketLimitStop);
					break;
				case MarketLimitStop.Market:
				case MarketLimitStop.AtClose:
					switch (entryAlert.Direction) {
						case Direction.Buy:
							if (entryPriceOut > quote.Ask) {
								entryPriceOut = quote.Ask;
								string msg = "BuyAtMarket/BuyAtClose(bar[" + barIndexTested
									+ "], signalName[" + entryAlert.PositionAffected.EntrySignal + "]) "
									+ "entryPrice[" + entryPriceOut + "] > := quoteToReach.Ask["
									+ barIndexTested + "]=[" + quote.Ask + "]"
									+ " while basisPrice=[" + entryAlert.PositionAffected.LastQuoteForMarketOrStopLimitImplicitPrice + "]";
							}
							//entrySlippageOutNotUsed = slippageNonLimit;
							//priceScriptAligned += entrySlippageOutNotUsed;
							break;
						case Direction.Short:
							if (entryAlert.PriceScript < quote.Bid) {
								entryPriceOut = quote.Bid;
								string msg = "ShortAtMarket/ShortAtClose(bar[" + barIndexTested
									+ "], signalName[" + entryAlert.PositionAffected.EntrySignal + "]) "
									+ "entryPrice[" + entryPriceOut + "] < := quoteToReach.Bid["
									+ barIndexTested + "]=[" + quote.Bid + "]"
									+ " while basisPrice=[" + entryAlert.PositionAffected.LastQuoteForMarketOrStopLimitImplicitPrice + "]";
							}
							//entrySlippageOutNotUsed = -slippageNonLimit;
							//priceScriptAligned += entrySlippageOutNotUsed;
							break;
						default:
							#if DEBUG
							Debugger.Break();
							#endif
							throw new Exception("CheckEntry() NYI direction[" + entryAlert.Direction + "] for [" + entryAlert + "]");
					}
					break;
				default:
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception("NYI: marketLimitStop[" + entryAlert.MarketLimitStop + "] is not Limit or Stop");
			}
			//entryPriceOut = this.executor.AlignAlertPriceToPriceLevel(entryAlert.Bars, entryPriceOut, true,
			//	entryAlert.PositionLongShortFromDirection, entryAlert.MarketLimitStop);
			if (entryPriceOut <= 0) {
				string msg = "entryPrice[" + entryPriceOut + "]<=0 what do you mean??? get Bars.LastBar.Close for Market...";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			// v1 BarStreaming with 1 quote (=> 0px height) won't contain any price you may check here;
			// v1 REMOVED because MarketLimitStop.Market haven't been filled immediately and behaved like stop/limit entries/exits;
			// v1 for Stop/Limit you already hit "return false" above; market should get whatever crazy bid/ask and get the fill at first attempt now
			// v1 INAPPROPRIATE_BECAUSE_MARKET_SIM_SHOULNT_KNOW_ANYTHING_ABOUT_ORIGINAL_SIMULATED_BAR_AND_STREAMING_IS_IN_PROGRESS
			// v1 CHECK_IF_QUOTE_ISNT_BEYOND_BAR_EARLIER_UPSTACK alternative implementation: GENERATED_QUOTE_OUT_OF_BOUNDARY_CHECK
			//if (entryAlert.Bars.BarStreaming.ContainsPrice(entryPriceOut) == false) {		// earlier version of same checkup as Position.FillEntryWith() 
			//	string msg = "QUOTE_UNFILLABLE_ON_BAR_STREAMING quote[" + quote + "] => entryPriceOut["
			//		+ entryPriceOut + "] at entryAlert.Bars.BarStreaming[" + entryAlert.Bars.BarStreaming + "]";
			//	//throw new Exception(msg);
			//	#if DEBUG
			//	//Debugger.Break();
			//	#endif
			//	return false;
			//}
			//v1
			
			if (quote.PriceBetweenBidAsk(entryPriceOut) == false) {
				string msig = " MUST_BE_BETWEEN: [" + quote.Bid + "] < [" + entryPriceOut + "] < [" + quote.Ask + "]";
				if (this.executor.Backtester.IsBacktestingNow) {
					string msg = "OBSOLETE I_DONT_UNDERSTAND_HOW_I_DIDNT_DROP_THIS_QUOTE_BEFORE_BUT_I_HAVE_TO_DROP_IT_NOW";
					Assembler.PopupException(msg + msig, null);
				} else {
					string msg = "QuikTerminalMock uses MarketSim.SimulateFill() for live-simulated mode => no surprise here";
					//Assembler.PopupException(msg + msig, null);
				}
				return false;
			}
			return true;
		}
		public bool CheckExitAlertWillBeFilledByQuote(Alert exitAlert, Quote quote
				, out double exitPriceOut, out double exitSlippageOut) {

			//v2
			exitPriceOut = exitAlert.PriceScriptAligned;

			//v1
			#if DEBUG	// REMOVE_ONCE_NEW_ALIGNMENT_MATURES_NOVEMBER_15TH_2014
			double exitPriceOut1 = this.executor.AlignAlertPriceToPriceLevel(
				exitAlert.Bars, exitAlert.PriceScript, false,
				exitAlert.PositionLongShortFromDirection, exitAlert.MarketLimitStop);
			if (exitPriceOut1 != exitAlert.PriceScriptAligned) {
				string msg = "FIX_Alert.PriceScriptAligned";
				Debugger.Break();
			} else {
				string msg = "GET_RID_OF_COMPLEX_ALIGNMENT executor.AlignAlertPriceToPriceLevel()";
			}
			#endif
			
			exitSlippageOut = 0;
			//double slippageLimit = this.executor.getSlippage(exitAlert.PriceScript
			//	, exitAlert.Direction, 0, this.executor.IsStreaming, true);
			//double slippageNonLimit = this.executor.getSlippage(exitAlert.PriceScript
			//	, exitAlert.Direction, 0, this.executor.IsStreaming, false);

			int exitBarToTestOn = exitAlert.Bars.Count;
			Bar bar = exitAlert.Bars[exitBarToTestOn];
			switch (exitAlert.MarketLimitStop) {
				case MarketLimitStop.Limit:
					switch (exitAlert.Direction) {
						case Direction.Sell:
							//dont check if (slippage < 0) throw new Exception("SellAtLimit: slippage[" + slippage + "] should be positive >=0 +");
							//if (exitPriceOut + slippageLimit > quoteToReach.Ask) return false;
							if (exitPriceOut > quote.Ask) return false;
							break;
						case Direction.Cover:
							//dont check if (slippage > 0) throw new Exception("CoverAtLimit: slippage[" + slippage + "] should be negative <=0 -");
							//if (exitPriceOut - slippageLimit < quoteToReach.Bid) return false;
							if (exitPriceOut < quote.Bid) return false;
							break;
						default:
							#if DEBUG
							Debugger.Break();
							#endif
							throw new Exception("CheckExit() NYI direction[" + exitAlert.Direction + "] for [" + exitAlert + "]");
					}
					break;
				case MarketLimitStop.Stop:
					switch (exitAlert.Direction) {
						case Direction.Sell:
							if (exitPriceOut < quote.Bid) return false;
							//exitSlippageOut = -slippageNonLimit;
							//exitPriceOut += exitSlippageOut;
							break;
						case Direction.Cover:
							if (exitPriceOut > quote.Ask) return false;
							//exitSlippageOut = slippageNonLimit;
							//exitPriceOut += exitSlippageOut;
							break;
						default:
							#if DEBUG
							Debugger.Break();
							#endif
							throw new Exception("CheckExit() NYI direction[" + exitAlert.Direction + "] for [" + exitAlert + "]");
					}
					break;
				case MarketLimitStop.StopLimit:
					if (quote.ParentBarStreaming == null) {
						string msg = "quoteToReach must be bound here!!!";
						//Debugger.Break();
					} else {
						if (quote.ParentBarStreaming.ParentBarsIndex == 133) Debugger.Break();
					}

					//v2
					double priceStopActivationAligned = exitAlert.PriceStopLimitActivationAligned;

					//v1
					#if DEBUG	// REMOVE_ONCE_NEW_ALIGNMENT_MATURES_NOVEMBER_15TH_2014
					// we aligned priceStopActivation before we stored it in exitAlert  
					double priceStopActivationAligned1 = this.executor.AlignAlertPriceToPriceLevel(
						exitAlert.Bars, exitAlert.PriceStopLimitActivation, false,
						exitAlert.PositionLongShortFromDirection, exitAlert.MarketLimitStop);
					if (priceStopActivationAligned1 != exitAlert.PriceStopLimitActivationAligned) {
						string msg = "FIX_Alert.PriceStopLimitActivation";
						Debugger.Break();
					} else {
						string msg = "GET_RID_OF_COMPLEX_ALIGNMENT executor.AlignAlertPriceToPriceLevel()";
					}
					#endif

					switch (exitAlert.Direction) {
						case Direction.Sell:
							if (exitPriceOut < priceStopActivationAligned) {
								string msg = "SellLimit is below StopActivation, alert look consistent";
							} else {
								string msg = "SellLimit is below SellStop, expecting (TODO: multibar) bounce back to SellLimit"
									+ " (like on MT5 picture, {Sell stop-limit} case)";
								//http://www.metatrader5.com/en/terminal/help/trading/general_concept/order_types
							}
							// NB! we're getting here 4 times per bar for BacktestingMode.FourStroke
							// so Open stays the same, Volume increases, Close will be zig-zagging each "stroke", High=Max(Close), Low=Min(Close)
							bool SLSellActivated = this.stopLossesActivatedOnPreviousQuotes.Contains(exitAlert);
							if (SLSellActivated == false) {
								if (priceStopActivationAligned < quote.Bid) {
									string msg = "StopActivation is VIRTUALLY not activated by TOUCHED_FROM_ABOVE";
									return false;
								}
								this.stopLossesActivatedOnPreviousQuotes.Add(exitAlert); // yes SellStop is VIRTUALLY activated by TOUCHED_FROM_ABOVE
							}
							if (exitPriceOut < quote.Bid) {		// + slippageLimit
								string msg = "SellLimit is not filled because whole quoteToReach is below SellLimit"
									+ "; rare case when PositionPrototype.ctor() checks didn't catch it?";
								return false;
							}
							if (exitPriceOut < quote.Ask) {
								string msg = "SellLimit wasn't TOUCHED_FROM_ABOVE; staying WaitingFillBroker but not filled at this bar" 
									+ " (TODO: if on next bar SellLimit is fillable, avoid first StopActivation condition above)";
								return false;
							}
							if (this.stopLossesActivatedOnPreviousQuotes.Contains(exitAlert) == false) {
								string msg = "DUPE simulateFillExit has previously completely filled Sell StopLimit (StopActivated+Limit) " + exitAlert;
								#if DEBUG
								Debugger.Break();
								#endif
								throw new Exception(msg);
							}
							this.stopLossesActivatedOnPreviousQuotes.Remove(exitAlert);
							break;
						case Direction.Cover:
							if (exitPriceOut > priceStopActivationAligned) {
								string msg = "BuyLimit is above StopActivation, alert look consistent"
									+ ", expecting ~immediate fill between [StopActivation...BuyLimit]"
									+ " (if slippage is too tight and price rockets we'll get no fill)"
									+ " ( QUIK model)";
							} else {
								string msg = "BuyLimit is below SellStop, expecting (TODO: multibar) bounce back to BuyLimit"
									+ " (like on MT5 picture, {Sell stop-limit} case)";
								//http://www.metatrader5.com/en/terminal/help/trading/general_concept/order_types
							}
							// NB! we're getting here 4 times per bar for BacktestingMode.FourStroke so HLCV will be zig-zagging each "stroke"
							bool SLCoverActivated = this.stopLossesActivatedOnPreviousQuotes.Contains(exitAlert);
							if (SLCoverActivated == false) {
								if (priceStopActivationAligned > quote.Ask) {
									string msg = "StopActivation is VIRTUALLY not activated by TOUCHED_FROM_BELOW";
									return false;
								}
								this.stopLossesActivatedOnPreviousQuotes.Add(exitAlert);
							}
							// yes SellStop is "virtually" activated by "touched from above" (UnitTest: eyeball the current bar)
							if (exitPriceOut > quote.Bid) {		// - slippageLimit
								string msg = "BuyLimit is not filled because whole the bar was above BuyLimit"
									+ "; rare case when PositionPrototype.ctor() checks didn't catch it?";
								return false;
							}
							if (exitPriceOut > quote.Ask) {
								string msg = "BuyLimit wasn't TOUCHED_FROM_BELOW; staying WaitingFillBroker but not filled at this bar"
									+ " (TODO: if on next bar BuyLimit is fillable, avoid first StopActivation condition above)";
								return false;
							}
							if (this.stopLossesActivatedOnPreviousQuotes.Contains(exitAlert) == false) {
								string msg = "DUPE simulateFillExit has previously completely filled Cover StopLimit (StopActivated+Limit) " + exitAlert;
								throw new Exception(msg);
							}
							this.stopLossesActivatedOnPreviousQuotes.Remove(exitAlert);
							break;
						default:
							#if DEBUG
							Debugger.Break();
							#endif
							throw new Exception("CheckExit() NYI direction[" + exitAlert.Direction + "] for [" + exitAlert + "]");
					}
					break;
				case MarketLimitStop.Market:
				case MarketLimitStop.AtClose:
					// WHY (IsBacktestingNow == true): market orders during LIVE could be filled at virtually ANY price
					if (quote.PriceBetweenBidAsk(exitPriceOut) == false && this.executor.Backtester.IsBacktestingNow == true) {
						string msg = "this isn't enough: market orders are still executed outside the bar; we'll need to generate one more quote onTheWayTo exitPriceOut";
						Assembler.PopupException(msg, null, false);
						return false;
					}
					switch (exitAlert.Direction) {
						case Direction.Sell:
							//exitPriceOut -= slippageNonLimit;
							break;
						case Direction.Cover:
							//exitPriceOut += slippageNonLimit;
							break;
						default:
							#if DEBUG
							Debugger.Break();
							#endif
							throw new Exception("CheckExit() NYI direction[" + exitAlert.Direction + "] for [" + exitAlert + "]");
					}
					break;
				default:
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception("NYI marketLimitStop[" + exitAlert.MarketLimitStop + "]");
			}

			//exitPriceOut = this.executor.AlignAlertPriceToPriceLevel(exitAlert.Bars, exitPriceOut, false,
			//	exitAlert.PositionLongShortFromDirection, exitAlert.MarketLimitStop);
			if (exitPriceOut <= 0) {
				string msg = "exitPriceOut[" + exitPriceOut + "]<=0 what do you mean??? get Bars.LastBar.Close for Market...";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			// v1 BarStreaming with 1 quote (=> 0px height) won't contain any price you may check here;
			// v1 REMOVED because MarketLimitStop.Market haven't been filled immediately and behaved like stop/limit entries/exits;
			// v1 for Stop/Limit you already hit "return false" above; market should get whatever crazy bid/ask and get the fill at first attempt now
			// v1 INAPPROPRIATE_BECAUSE_MARKET_SIM_SHOULNT_KNOW_ANYTHING_ABOUT_ORIGINAL_SIMULATED_BAR_AND_STREAMING_IS_IN_PROGRESS
			// v1 CHECK_IF_QUOTE_ISNT_BEYOND_BAR_EARLIER_UPSTACK alternative implementation: GENERATED_QUOTE_OUT_OF_BOUNDARY_CHECK
			//if (exitAlert.Bars.BarStreaming.ContainsPrice(exitPriceOut) == false) {		// earlier version of same checkup as Position.FillEntryWith() 
			//	string msg = "QUOTE_GENERATED_UNFILLABLE_ON_BAR_STREAMING quote[" + quote + "] => exitPriceOut["
			//		+ exitPriceOut + "] at exitAlert.Bars.BarStreaming[" + exitAlert.Bars.BarStreaming + "]";
			//	//throw new Exception(msg);
			//	#if DEBUG
			//	Debugger.Break();
			//	#endif
			//	return false;
			//}
			// /v1
			
			return true;
		}

		public int SimulateFillAllPendingAlerts(Quote quote) {
			this.checkThrowRealtimePendingQuote(quote);
			// there is no userlevel API to kill orders; in your Script, you operate SellAt/BuyAt; protoActivator.StopMove
			// killing orders is the privilege of Realtime: OrderManager kills orders by
			// 1) protoActivator.StopLossNewNegativeOffsetUpdateActivate or 2) user clicks in GUI
			//int alertsKilled = this.simulateBacktestPendingAlertsKill(currentBarToTestOn);

			int exitsFilled = 0;
			int entriesFilled = 0;

			List<Alert> alertsPendingSafeCopy = this.executor.ExecutionDataSnapshot.AlertsPending.InnerListSafeCopy;
			foreach (Alert alert in alertsPendingSafeCopy) {
				if (alert.IsFilled && alert.IsExitAlert && alert.PositionAffected.Prototype != null) {
					bool thisAlertWasAnnihilated = false;
					if (alert.PositionAffected.ExitAlert == alert.PositionAffected.Prototype.StopLossAlertForAnnihilation
							&& alert == alert.PositionAffected.Prototype.TakeProfitAlertForAnnihilation) {
						thisAlertWasAnnihilated = true;
					}
					if (alert.PositionAffected.ExitAlert == alert.PositionAffected.Prototype.TakeProfitAlertForAnnihilation
							&& alert == alert.PositionAffected.Prototype.StopLossAlertForAnnihilation) {
						thisAlertWasAnnihilated = true;
					}
					if (thisAlertWasAnnihilated) continue;
				}
				string reasonRefuseTryFill = this.checkAlertFillable(alert);
				if (String.IsNullOrEmpty(reasonRefuseTryFill) == false) {
					this.executor.PopupException(reasonRefuseTryFill);
					continue;
				}

				bool filled = this.SimulateFillPendingAlert(alert, quote);
				if (filled) {
					if (alert.IsEntryAlert)	entriesFilled++;
					else					exitsFilled++;
				}
			}
			int filledTotal = entriesFilled + exitsFilled;// + entriesStuckFilled + exitsStuckFilled;
			if (filledTotal > alertsPendingSafeCopy.Count) {
				string msg = "WHY_THERE_WERE_MORE_ALERTS_FILLED_THAN_THERE_WERE_ALERTS??? filledTotal["
					+ filledTotal + "] > AlertsPending.SafeCopy.Count[" + alertsPendingSafeCopy.Count + "]";
				Assembler.PopupException(msg);
			}
			return filledTotal;
		}

		public bool SimulateFillPendingAlert(Alert alert, Quote quote) {
			bool filled = false;
			if (alert.IsEntryAlert) {
				filled = this.simulatePendingFillEntry(alert, quote);
			} else {
				filled = this.simulatePendingFillExit(alert, quote);
			}
			if (filled == false) return filled;
			
			if (this.executor.Backtester.IsBacktestingNow == false) {
				string msg = "OrderProcessor.PostProcessOrderState() will invoke CallbackAlertFilledMoveAroundInvokeScript() for filled orders";
				return filled;
			}
			string msg2 = "below is a shortcut for Backtest+MarketSim to shorten realtime mutithreaded logic: Order.ctor()=>OrderSubmit()=>PostProcessOrderState=>CallbackAlertFilledMoveAroundInvokeScript()";
			
			if (this.executor.ExecutionDataSnapshot.AlertsPending.ContainsInInnerList(alert) == true) {
				string msg = "ALERT_MUST_HAVE_BEEN_REMOVED_FROM_PENDINGS_AFTER_FILL"
					+ "; normally, the filled alert should be already removed here by CallbackAlertFilledMoveAroundInvokeScript()"
					+ "; AlertsPending.Contains(" + alert + ")=true";
				bool removed = this.executor.ExecutionDataSnapshot.AlertsPending.Remove(alert);
				Assembler.PopupException(msg + " SimulatePendingFill(" + quote + ")");
			} else {
				//Debugger.Break();
			}
			if (this.FillOutsideQuoteSpreadParanoidCheckThrow == true) {
				bool isFilledOutsideQuote = alert.IsFilledOutsideQuote_DEBUG_CHECK;
				bool isFilledOutsideBar = alert.IsFilledOutsideBarSnapshotFrozen_DEBUG_CHECK;
				Assembler.PopupException("ALERT_FILLED_OUSIDE_QUOTE");
			}
			return filled;
		}
		string checkAlertFillable(Alert alert) {
			string msg = null;
			if (alert.IsFilled == true) {
				msg = "alert.IsFilled=true shouldn't stay in AlertsPending; shall I remove it now? [" + alert + "]";
				return msg;
			}
			if (alert.PositionAffected == null) {
				msg = "alert.PositionAffected=null should never be null; [" + alert + "]";
				return msg;
			}
			//if (alertToBeKilled.BarPlaced.ParentBarsIndex > alertToBeKilled.Bars.Count) {
			//	msg = "YOU_SHOULDNT_PEND_ALERT_FOR_FUTURE_BARS: alertToBeKilled.BarPlaced.ParentBarsIndex["
			//		+ alertToBeKilled.BarPlaced.ParentBarsIndex + "] > alertToBeKilled.Bars.Count[" + alertToBeKilled.Bars.Count + "]";
			//	return msg;
			//};
			//if (alertToBeKilled.BarPlaced.ParentBarsIndex < alertToBeKilled.Bars.Count) {
			//	msg = "YOU_SHOULDNT_PEND_ALERT_FOR_PAST_BARS: alertToBeKilled.BarPlaced.ParentBarsIndex["
			//		+ alertToBeKilled.BarPlaced.ParentBarsIndex + "] < alertToBeKilled.Bars.Count[" + alertToBeKilled.Bars.Count + "]";
			//	return msg;
			//};
			// (i don't have any simultaneous EntryAlerts for a bar, but)
			// once EntryAlerts is filled it's removed from Snap; however we have it in the copy so we need to skip it
			if (alert.IsEntryAlert) {
				if (alert.PositionAffected.IsEntryFilled) {
					msg = "I refuse to SimulatePendingFill for alertToBeKilled.PositionAffected.IsEntryFilled=true";
					return msg;
				}
			}
			if (alert.IsExitAlert) {
				if (alert.PositionAffected.IsExitFilled == true) {
					msg = "I refuse to simulatePendingFillExit() since PositionAffected.ExitFilled=true";
					if (alert.PositionAffected.IsExitFilledWithPrototypedAlert) {
						msg += "; possibly simulation glitch and AlertsPending should not contain Annihilated counterparty"
							+ "; Both StopLoss and TakeProfit can be executed on this bar"
							+ " (who's first will depend on MarketRealTime price fluctuations);"
							+ " position=" + alert.PositionAffected
							+ " SecondAlertButMightBeFirst=" + alert;
					} else {
						msg += "PositionAffected.ExitFilled and not by Prototype!"
							+ " should skip&continue but I'm reporting an Exception instead...";
					}
					return msg;
				}
			}
			return msg;
		}
		void checkThrowRealtimePendingQuote(Quote quote) {
			if (this.executor.Backtester.IsBacktestingNow == false) {
				string msg = "SimulateFill*() should not be used for RealTime BrokerProviders and RealTime Mocks!"
					+ " make sure you invoked executor.CallbackAlertFilledInvokeScript() from where you are now";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			if (this.executor.ExecutionDataSnapshot.AlertsPending.Count == 0) {
				string msg = "Before you call me, Please check executor.ExecutionDataSnapshot.AlertsPending.Count!=0";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
				//return 0;
			}
			if (quote.ParentBarStreaming == null) {
				string msg = "I refuse to serve this quoteToReach.ParentStreamingBar=null";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			if (quote.ParentBarStreaming.ParentBarsIndex != this.executor.Bars.Count - 1) {
				string msg = "I refuse to serve this quoteToReach.ParentStreamingBar.ParentBarsIndex["
					+ quote.ParentBarStreaming.ParentBarsIndex + "] != this.executor.Bars.Count-1[" + (this.executor.Bars.Count - 1) + "]";
				throw new Exception(msg);
			}
		}
		bool simulatePendingFillEntry(Alert alert, Quote quote) {
			double priceFill = -1;
			double slippageFill = -1;
			bool filled = this.CheckEntryAlertWillBeFilledByQuote(alert, quote, out priceFill, out slippageFill);
			if (filled == false) return filled;

			int entryBarToTestOn = quote.ParentBarStreaming.ParentBarsIndex;
			if (entryBarToTestOn == -1) {
				string msg = "AVOIDING_ALL_SORT_OF_MOVE_AROUND_ERRORS MUST_BE_POSITIVE_GOT_-1_quote.ParentStreamingBar.ParentBarsIndex";
				Assembler.PopupException(msg);
				return filled;
			}
			// making a derived quoteToReach look like "dedicated" specifically to the filled alertToBeKilled
			if (quote.IntraBarSerno >= Quote.IntraBarSernoShiftForGeneratedTowardsPendingFill) {
				quote.Size = alert.Qty;
			}
			double entryCommission = this.executor.OrderCommissionCalculate(alert.Direction,
				alert.MarketLimitStop, priceFill, alert.Qty, alert.Bars);
			
			if (this.executor.Backtester.IsBacktestingNow == false) {
				string msg = "OrderProcessor.PostProcessOrderState() will invoke CallbackAlertFilledMoveAroundInvokeScript() for filled orders";
				return filled;
			}
			string msg2 = "below is a shortcut for Backtest+MarketSim to shorten realtime mutithreaded logic: Order.ctor()=>OrderSubmit()=>PostProcessOrderState=>CallbackAlertFilledMoveAroundInvokeScript()";
			this.executor.CallbackAlertFilledMoveAroundInvokeScript(alert, quote,
				priceFill, alert.Qty, slippageFill, entryCommission);
			return filled;
		}
		bool simulatePendingFillExit(Alert alert, Quote quote) {
			double priceFill = -1;
			double slippageFill = 1;
			bool filled = this.CheckExitAlertWillBeFilledByQuote(alert, quote, out priceFill, out slippageFill);
			if (filled == false) return filled;

			double exitCommission = this.executor.OrderCommissionCalculate(alert.Direction,
				alert.MarketLimitStop, priceFill, alert.Qty, alert.Bars);
			if (quote.ParentBarStreaming.ParentBarsIndex == -1) {
				string msg = "AVOIDING_ALL_SORT_OF_MOVE_AROUND_ERRORS MUST_BE_POSITIVE_GOT_-1_quote.ParentStreamingBar.ParentBarsIndex";
				Assembler.PopupException(msg);
				return filled;
			}
			
			if (this.executor.Backtester.IsBacktestingNow == false) {
				string msg = "OrderProcessor.PostProcessOrderState() will invoke CallbackAlertFilledMoveAroundInvokeScript() for filled orders";
				return filled;
			}
			string msg2 = "below is a shortcut for Backtest+MarketSim to shorten realtime mutithreaded logic: Order.ctor()=>OrderSubmit()=>PostProcessOrderState=>CallbackAlertFilledMoveAroundInvokeScript()";
			this.executor.CallbackAlertFilledMoveAroundInvokeScript(alert, quote,
				priceFill, alert.Qty, slippageFill, exitCommission);
			return filled;
		}

		public void SimulateStopLossMoved(Alert alertToBeKilled) {
			Alert replacement = executor.PositionPrototypeActivator.CreateStopLossFromPositionPrototype(alertToBeKilled.PositionAffected);
			bool removed = executor.ExecutionDataSnapshot.AlertsPending.Remove(alertToBeKilled);
			//ALREADY_ADDED_BY AlertEnrichedRegister
			// executor.ExecutionDataSnapshot.AlertsPending.ByBarExpectedFillAddNoDupe(alertToBeKilled);
			// executor.ExecutionDataSnapshot.AlertsPending.AddNoDupe(replacement);
		}
		public void SimulateTakeProfitMoved(Alert alertToBeKilled) {
			Alert replacement = executor.PositionPrototypeActivator.CreateTakeProfitFromPositionPrototype(alertToBeKilled.PositionAffected);
			bool removed = executor.ExecutionDataSnapshot.AlertsPending.Remove(alertToBeKilled);
			//ALREADY_ADDED_BY AlertEnrichedRegister
			// executor.ExecutionDataSnapshot.AlertsPending.ByBarExpectedFillAddNoDupe(alertToBeKilled);
			// executor.ExecutionDataSnapshot.AlertsPending.AddNoDupe(replacement);
		}
		public bool AnnihilateCounterpartyAlert(Alert alert) {
			if (this.executor.ExecutionDataSnapshot.AlertsPending.ContainsInInnerList(alert) == false) {
				string msg = "ANNIHILATE_COUNTERPARTY_ALREADY_REMOVED " + alert;	//ExecSnap.AlertsPending not synchronized: already removed
				throw new Exception(msg);
				//return false;
			}
			bool removed = this.executor.ExecutionDataSnapshot.AlertsPending.Remove(alert);
			// no alert.OrderFollowed here!
			//this.executor.RemovePendingAlertClosePosition(alert, "MarketSim:AnnihilateCounterparty(): ");
			return true;
		}
		
		public void SimulateAlertKillPending(Alert alert) {
			alert.IsKilled = true;
			this.executor.CallbackAlertKilledInvokeScript(alert);
		}
	}
}