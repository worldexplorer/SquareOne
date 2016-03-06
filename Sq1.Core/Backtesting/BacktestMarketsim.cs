using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Backtesting {
	public class BacktestMarketsim {
				List<Alert> stopLossesActivatedOnPreviousQuotes;
				BacktestBroker backtestBroker;

		BacktestMarketsim() {
			this.stopLossesActivatedOnPreviousQuotes = new List<Alert>();
		}
		public BacktestMarketsim(BacktestBroker backtestBroker) : this() {
			this.backtestBroker = backtestBroker;
		}

		public void Initialize(bool fillOutsideQuoteSpreadParanoidCheckThrow = false) {
			this.stopLossesActivatedOnPreviousQuotes.Clear();
		}
		public bool CheckAlertWillBeFilledByQuote(Alert alert, Quote quote, out double entryPriceOut, out double entrySlippageOutNotUsed) {
			return alert.IsEntryAlert
				? this.CheckEntryAlertWillBeFilledByQuote(alert, quote, out entryPriceOut, out entrySlippageOutNotUsed)
				: this.CheckExitAlertWillBeFilledByQuote(alert, quote, out entryPriceOut, out entrySlippageOutNotUsed);
		}
		public bool CheckEntryAlertWillBeFilledByQuote(Alert entryAlert, Quote quote, out double entryPriceOut, out double entrySlippageOutNotUsed) {
			// if entry is triggered, call position.EnterFinalize(entryPrice, entrySlippage, entryCommission);
			//v2
			entryPriceOut = entryAlert.PriceScriptAligned;

			#if DEBUG	// REMOVE_ONCE_NEW_ALIGNMENT_MATURES_NOVEMBER_15TH_2014
			//v1
			double entryPriceOut1 = this.backtestBroker.ScriptExecutor.AlertPrice_alignToPriceLevel(entryAlert.Bars, entryAlert.PriceScript, true,
				entryAlert.PositionLongShortFromDirection, entryAlert.MarketLimitStop);
			if (entryPriceOut1 != entryAlert.PriceScriptAligned) {
				string msg = "FIX_Alert.PriceScriptAligned";
				Debugger.Break();
			} else {
				string msg = "GET_RID_OF_COMPLEX_ALIGNMENT this.backtestBroker.ScriptExecutor.AlignAlertPriceToPriceLevel()";
			}
			#endif

			entrySlippageOutNotUsed = 0;
			//Direction directionPositionClose = MarketConverter.ExitDirectionFromLongShort(alert.PositionLongShortFromDirection);
			//double slippageLimit = this.backtestBroker.ScriptExecutor.getSlippage(
			//	alert.PriceScript, alert.Direction, 0, this.backtestBroker.ScriptExecutor.IsStreaming, true);
			//double slippageNonLimit = this.backtestBroker.ScriptExecutor.getSlippage(
			//	alert.PriceScript, alert.Direction, 0, this.backtestBroker.ScriptExecutor.IsStreaming, false);
			//double slippageMarketForClosingPosition = this.backtestBroker.ScriptExecutor.getSlippage(
			//	alert.PriceScript, directionPositionClose, 0, this.backtestBroker.ScriptExecutor.IsStreaming, false);

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
					//entryPriceOut = this.backtestBroker.ScriptExecutor.AlignAlertPriceToPriceLevel(entryAlert.Bars, entryPriceOut, true,
					//	entryAlert.PositionLongShortFromDirection, entryAlert.MarketLimitStop);
					break;
				case MarketLimitStop.Market:
				//MUST_DIE case MarketLimitStop.AtClose:
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
			//entryPriceOut = this.backtestBroker.ScriptExecutor.AlignAlertPriceToPriceLevel(entryAlert.Bars, entryPriceOut, true,
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
				if (this.backtestBroker.ScriptExecutor.BacktesterOrLivesimulator.ImRunningChartlessBacktesting) {
					string msg = "OBSOLETE I_DONT_UNDERSTAND_HOW_I_DIDNT_DROP_THIS_QUOTE_BEFORE_BUT_I_HAVE_TO_DROP_IT_NOW";
					Assembler.PopupException(msg + msig);
				} else {
					string msg = "QuikTerminalMock uses MarketSim.SimulateFill() for live-simulated mode => no surprise here";
					//Assembler.PopupException(msg + msig);
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
			double exitPriceOut1 = this.backtestBroker.ScriptExecutor.AlertPrice_alignToPriceLevel(
				exitAlert.Bars, exitAlert.PriceScript, false,
				exitAlert.PositionLongShortFromDirection, exitAlert.MarketLimitStop);
			if (exitPriceOut1 != exitAlert.PriceScriptAligned) {
				string msg = "FIX_Alert.PriceScriptAligned";
				Debugger.Break();
			} else {
				string msg = "GET_RID_OF_COMPLEX_ALIGNMENT this.backtestBroker.ScriptExecutor.AlignAlertPriceToPriceLevel()";
			}
			#endif
			
			exitSlippageOut = 0;
			//double slippageLimit = this.backtestBroker.ScriptExecutor.getSlippage(exitAlert.PriceScript
			//	, exitAlert.Direction, 0, this.backtestBroker.ScriptExecutor.IsStreaming, true);
			//double slippageNonLimit = this.backtestBroker.ScriptExecutor.getSlippage(exitAlert.PriceScript
			//	, exitAlert.Direction, 0, this.backtestBroker.ScriptExecutor.IsStreaming, false);

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
						Assembler.PopupException(msg);
					}

					//v2
					double priceStopActivationAligned = exitAlert.PriceStopLimitActivationAligned;

					//v1
					#if DEBUG	// REMOVE_ONCE_NEW_ALIGNMENT_MATURES_NOVEMBER_15TH_2014
					// we aligned priceStopActivation before we stored it in exitAlert  
					double priceStopActivationAligned1 = this.backtestBroker.ScriptExecutor.AlertPrice_alignToPriceLevel(
						exitAlert.Bars, exitAlert.PriceStopLimitActivation, false,
						exitAlert.PositionLongShortFromDirection, exitAlert.MarketLimitStop);
					if (priceStopActivationAligned1 != exitAlert.PriceStopLimitActivationAligned) {
						string msg = "FIX_Alert.PriceStopLimitActivation";
						Debugger.Break();
					} else {
						string msg = "GET_RID_OF_COMPLEX_ALIGNMENT this.backtestBroker.ScriptExecutor.AlignAlertPriceToPriceLevel()";
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
				//MUST_DIE case MarketLimitStop.AtClose:
				case MarketLimitStop.Market:
					// WHY (IsBacktestingNow == true): market orders during LIVE could be filled at virtually ANY price
					if (quote.PriceBetweenBidAsk(exitPriceOut) == false && this.backtestBroker.ScriptExecutor.BacktesterOrLivesimulator.ImRunningChartlessBacktesting == true) {
						string msg = "exitPriceOut[" + exitPriceOut + "] must be inside the bar; we'll need to generate one more quote onTheWayTo exitPriceOut";
						Assembler.PopupException(msg);
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
							throw new Exception("CheckExitAlertWillBeFilledByQuote() NYI direction[" + exitAlert.Direction + "] for [" + exitAlert + "]");
					}
					break;
				default:
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception("NYI marketLimitStop[" + exitAlert.MarketLimitStop + "]");
			}

			//exitPriceOut = this.backtestBroker.ScriptExecutor.AlignAlertPriceToPriceLevel(exitAlert.Bars, exitPriceOut, false,
			//	exitAlert.PositionLongShortFromDirection, exitAlert.MarketLimitStop);
			if (exitPriceOut <= 0) {
				string msg = "exitPriceOut[" + exitPriceOut + "]<=0 what do you mean??? get Bars.LastBar.Close for Market...";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}

			return true;
		}

		public int SimulateFillAllPendingAlerts(Quote quote, Action<Alert, double, double> action_afterAlertFilled_beforeMovedAround = null) {
			this.checkThrowRealtimePendingQuote(quote);
			// there is no userlevel API to kill orders; in your Script, you operate SellAt/BuyAt; protoActivator.StopMove
			// killing orders is the privilege of Realtime: OrderManager kills orders by
			// 1) protoActivator.StopLossNewNegativeOffsetUpdateActivate or 2) user clicks in GUI
			//int alertsKilled = this.simulateBacktestPendingAlertsKill(currentBarToTestOn);

			int exitsFilled = 0;
			int entriesFilled = 0;

			List<Alert> alertsPendingSafeCopy = this.backtestBroker.ScriptExecutor.ExecutionDataSnapshot.AlertsPending.SafeCopy(this, "SimulateFillAllPendingAlerts(WAIT)");
			foreach (Alert alert in alertsPendingSafeCopy) {
				if (alert.IsFilled && alert.IsExitAlert && alert.PositionAffected.Prototype != null) {
					bool thisAlertWasAnnihilated = false;
					if (alert.PositionAffected.ExitAlert == alert.PositionAffected.Prototype.StopLossAlert_forMoveAndAnnihilation
							&& alert == alert.PositionAffected.Prototype.TakeProfitAlert_forMoveAndAnnihilation) {
						thisAlertWasAnnihilated = true;
					}
					if (alert.PositionAffected.ExitAlert == alert.PositionAffected.Prototype.TakeProfitAlert_forMoveAndAnnihilation
							&& alert == alert.PositionAffected.Prototype.StopLossAlert_forMoveAndAnnihilation) {
						thisAlertWasAnnihilated = true;
					}
					if (thisAlertWasAnnihilated) continue;
				}
				string reasonRefuseTryFill = this.checkAlertFillable(alert);
				if (string.IsNullOrEmpty(reasonRefuseTryFill) == false) {
					this.backtestBroker.ScriptExecutor.PopupException(reasonRefuseTryFill);
					continue;
				}

				bool filled = this.SimulateFillPendingAlert(alert, quote, action_afterAlertFilled_beforeMovedAround);
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

		public bool SimulateFillPendingAlert(Alert alert, Quote quote, Action<Alert, double, double> action_afterAlertFilled_beforeMovedAround = null) {
			string msig = " //SimulateFillPendingAlert(alert[" + alert + "], quote[" + quote + "])";

			bool filled = false;
			if (alert.IsEntryAlert) {
				filled = this.simulatePendingFillEntry(alert, quote, action_afterAlertFilled_beforeMovedAround);
			} else {
				filled = this.simulatePendingFillExit(alert, quote, action_afterAlertFilled_beforeMovedAround);
			}
			if (filled == false) return filled;
			
			//if (this.backtestBroker.ScriptExecutor.Backtester.IsBacktestingNow == false) {
			if (this.backtestBroker.ScriptExecutor.BacktesterOrLivesimulator.ImBacktestingOrLivesimming == false) {
				string msg = "OrderProcessor.PostProcessOrderState() will invoke CallbackAlertFilledMoveAroundInvokeScript() for filled orders";
				return filled;
			}
			string msg2 = "below is a shortcut for Backtest+MarketSim to shorten realtime mutithreaded"
				+ " logic: Order.ctor()=>OrderSubmit()=>PostProcessOrderState=>CallbackAlertFilledMoveAroundInvokeScript()";
			
			if (this.backtestBroker.ScriptExecutor.ExecutionDataSnapshot.AlertsPending.Contains(alert, this, "SimulateFillPendingAlert(WAIT)") == true) {
				bool removedForcibly = this.backtestBroker.ScriptExecutor.ExecutionDataSnapshot.AlertsPending.Remove(alert, this, "SimulateFillPendingAlert(WAIT)");
				string msg = "ALERT_MUST_HAVE_BEEN_REMOVED_FROM_PENDINGS_AFTER_FILL"
					//+ "; normally, the filled alert should be already removed here by CallbackAlertFilledMoveAroundInvokeScript()"
					//+ "; AlertsPending.Contains(" + alert + ")=true"
					+ " removedForcibly[" + removedForcibly + "]"
					;
				Assembler.PopupException(msg + msig, null, false);
			} else {
				//Debugger.Break();
			}
			if (	this.backtestBroker.ScriptExecutor.Strategy.ScriptContextCurrent.FillOutsideQuoteSpreadParanoidCheckThrow == true
				 && this.backtestBroker.ScriptExecutor.BacktesterOrLivesimulator.ImLivesimulator == false) {
				string msg = "";
				if (alert.IsFilledOutsideQuote_DEBUG_CHECK)				msg += "ALERT_FILLED_OUSIDE_QUOTE ";
				if (alert.IsFilledOutsideBarSnapshotFrozen_DEBUG_CHECK) msg += "ALERT_FILLED_OUSIDE_BAR " + quote.ParentBarStreaming;
				if (string.IsNullOrEmpty(msg) == false) Assembler.PopupException(msg + msig);
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
			if (this.backtestBroker.ScriptExecutor == null) {
				string msg = "YOU_DIDNT_INVOKE_this.BacktesterOrLivesimulator.BacktestDataSource.BrokerAsBacktest_nullUnsafe.InitializeBacktestBroker(this)_FROM_ScriptExecutor.Initialize()";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			if (this.backtestBroker.ScriptExecutor.BacktesterOrLivesimulator.ImBacktestingOrLivesimming == false && this.backtestBroker.ScriptExecutor.BacktesterOrLivesimulator.WasBacktestAborted == false) {
				string msg = "SimulateFill*() should not be used for RealTime BrokerAdapters and RealTime Mocks!"
					+ " make sure you invoked this.backtestBroker.ScriptExecutor.CallbackAlertFilledInvokeScript() from where you are now";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			if (this.backtestBroker.ScriptExecutor.ExecutionDataSnapshot.AlertsPending.Count == 0) {
				string msg = "Before you call me, Please check this.backtestBroker.ScriptExecutor.ExecutionDataSnapshot.AlertsPending.Count!=0";
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
			if (this.backtestBroker.ScriptExecutor.BacktesterOrLivesimulator.ImLivesimulator == false && quote.ParentBarStreaming.ParentBarsIndex != this.backtestBroker.ScriptExecutor.Bars.Count - 1) {
				string msg = "I refuse to serve this quoteToReach.ParentStreamingBar.ParentBarsIndex["
					+ quote.ParentBarStreaming.ParentBarsIndex + "] != this.backtestBroker.ScriptExecutor.Bars.Count-1[" + (this.backtestBroker.ScriptExecutor.Bars.Count - 1) + "]";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
		}
		bool simulatePendingFillEntry(Alert alert, Quote quote, Action<Alert, double, double> action_afterAlertFilled_beforeMovedAround = null) {
			string msig = " //simulatePendingFillEntry(alert[" + alert + "], quote[" + quote + "]) " + this.backtestBroker.ScriptExecutor;
			double priceFill = -1;
			double slippageFill = -1;
			bool filled = this.CheckEntryAlertWillBeFilledByQuote(alert, quote, out priceFill, out slippageFill);
			if (filled == false) return filled;

			int entryBarToTestOn = quote.ParentBarStreaming.ParentBarsIndex;
			if (entryBarToTestOn == -1) {
				string msg = "AVOIDING_ALL_SORT_OF_MOVE_AROUND_ERRORS MUST_BE_POSITIVE_GOT_-1_quote.ParentStreamingBar.ParentBarsIndex";
				Assembler.PopupException(msg + msig);
				return filled;
			}
			// making a derived quoteToReach look like "dedicated" specifically to the filled alertToBeKilled
			if (quote.IamInjectedToFillPendingAlerts) {
				quote.Size = alert.Qty;
			}
			double entryCommission = this.backtestBroker.ScriptExecutor.OrderCommissionCalculate(alert.Direction,
				alert.MarketLimitStop, priceFill, alert.Qty, alert.Bars);
			
			//if (this.backtestBroker.ScriptExecutor.Backtester.IsBacktestingNow == false) {
			if (this.backtestBroker.ScriptExecutor.BacktesterOrLivesimulator.ImBacktestingOrLivesimming == false) {
				string msg = "OrderProcessor.PostProcessOrderState() will invoke CallbackAlertFilledMoveAroundInvokeScript() for filled orders";
				return filled;
			}
			string msg2 = "below is a shortcut for Backtest+MarketSim to shorten realtime multithreaded"
				+ " logic: Order.ctor()=>OrderSubmit()=>PostProcessOrderState=>CallbackAlertFilledMoveAroundInvokeScript()";
			//if (this.backtestBroker.ScriptExecutor.BacktesterOrLivesimulator.ImLivesimulator) {
			if (this.backtestBroker.ImLivesimBroker) {		// LivesimBrokerDefault and any other BrokerOwnLivesimImplementation
				if (alert.OrderFollowed == null) {
					string msg = "ON_ALERT_FILLED__LIVESIM_BROKER__NOT_INVOKING_EMIT_BUTTON_OFF";
					//Assembler.PopupException(msg + msig, null, false);
					return filled;
				}
				if (action_afterAlertFilled_beforeMovedAround == null) {
					string msg = "ON_ALERT_FILLED__LIVESIM_BROKER__NOT_INVOKING_ACTION_NULL";
					Assembler.PopupException(msg + msig, null, false);
					return filled;
				}
				action_afterAlertFilled_beforeMovedAround(alert, priceFill, alert.Qty);
				return filled;
			}
			this.backtestBroker.ScriptExecutor.CallbackAlertFilled_moveAround_invokeScriptNonReenterably(alert, quote,
				priceFill, alert.Qty, slippageFill, entryCommission);
			return filled;
		}
		bool simulatePendingFillExit(Alert alert, Quote quote, Action<Alert, double, double> executeAfterAlertFilled = null) {
			string msig = " //simulatePendingFillExit(alert[" + alert + "], quote[" + quote + "]) " + this.backtestBroker.ScriptExecutor;

			double priceFill = -1;
			double slippageFill = 1;
			bool filled = this.CheckExitAlertWillBeFilledByQuote(alert, quote, out priceFill, out slippageFill);
			if (filled == false) return filled;

			double exitCommission = this.backtestBroker.ScriptExecutor.OrderCommissionCalculate(alert.Direction,
				alert.MarketLimitStop, priceFill, alert.Qty, alert.Bars);
			if (quote.ParentBarStreaming.ParentBarsIndex == -1) {
				string msg = "AVOIDING_ALL_SORT_OF_MOVE_AROUND_ERRORS MUST_BE_POSITIVE_GOT_-1_quote.ParentStreamingBar.ParentBarsIndex";
				Assembler.PopupException(msg + msig);
				return filled;
			}
			
			//if (this.backtestBroker.ScriptExecutor.Backtester.IsBacktestingNow == false) {
			if (this.backtestBroker.ScriptExecutor.BacktesterOrLivesimulator.ImBacktestingOrLivesimming == false) {
				string msg = "OrderProcessor.PostProcessOrderState() will invoke CallbackAlertFilledMoveAroundInvokeScript() for filled orders";
				return filled;
			}
			string msg2 = "below is a shortcut for Backtest+MarketSim to shorten realtime mutithreaded logic: Order.ctor()=>OrderSubmit()=>PostProcessOrderState=>CallbackAlertFilledMoveAroundInvokeScript()";
			//if (this.backtestBroker.ScriptExecutor.BacktesterOrLivesimulator.ImLivesimulator) {
			if (this.backtestBroker.ImLivesimBroker) {		// LivesimBrokerDefault and any other BrokerOwnLivesimImplementation
				if (executeAfterAlertFilled != null) {
					executeAfterAlertFilled(alert, priceFill, alert.Qty);
				} else {
					string msg = "ON_ALERT_FILLED__LIVESIM_BROKER__NOT_INVOKING_ACTION_NULL";
					Assembler.PopupException(msg + msig);
				}
				return filled;
			}
			this.backtestBroker.ScriptExecutor.CallbackAlertFilled_moveAround_invokeScriptNonReenterably(alert, quote,
				priceFill, alert.Qty, slippageFill, exitCommission);
			return filled;
		}

		public void StopLoss_simulateMoved(Alert alertToBeKilled) {
		    Alert replacement = this.backtestBroker.ScriptExecutor.PositionPrototypeActivator.CreateStopLoss_fromPositionPrototype(alertToBeKilled.PositionAffected);
		    bool removed = this.backtestBroker.ScriptExecutor.ExecutionDataSnapshot.AlertsPending.Remove(alertToBeKilled, this, "SimulateStopLossMoved(WAIT)");
		    //ALREADY_ADDED_BY AlertEnrichedRegister
		    // this.backtestBroker.ScriptExecutor.ExecutionDataSnapshot.AlertsPending.ByBarExpectedFillAddNoDupe(alertToBeKilled);
		    // this.backtestBroker.ScriptExecutor.ExecutionDataSnapshot.AlertsPending.AddNoDupe(replacement);
		}
		public void TakeProfit_simulateMoved(Alert alertToBeKilled) {
		    Alert replacement = this.backtestBroker.ScriptExecutor.PositionPrototypeActivator.CreateTakeProfit_fromPositionPrototype(alertToBeKilled.PositionAffected);
		    bool removed = this.backtestBroker.ScriptExecutor.ExecutionDataSnapshot.AlertsPending.Remove(alertToBeKilled, this, "SimulateTakeProfitMoved(WAIT)");
		    //ALREADY_ADDED_BY AlertEnrichedRegister
		    // this.backtestBroker.ScriptExecutor.ExecutionDataSnapshot.AlertsPending.ByBarExpectedFillAddNoDupe(alertToBeKilled);
		    // this.backtestBroker.ScriptExecutor.ExecutionDataSnapshot.AlertsPending.AddNoDupe(replacement);
		}
		public bool AlertCounterparty_annihilate(Alert alert) {
			if (this.backtestBroker.ScriptExecutor.ExecutionDataSnapshot.AlertsPending.Contains(alert, this, "AnnihilateCounterpartyAlert(WAIT)") == false) {
				string msg = "ANNIHILATE_COUNTERPARTY_ALREADY_REMOVED " + alert;	//ExecSnap.AlertsPending not synchronized: already removed
				throw new Exception(msg);
				//return false;
			}
			bool removed = this.backtestBroker.ScriptExecutor.ExecutionDataSnapshot.AlertsPending.Remove(alert, this, "AnnihilateCounterpartyAlert(WAIT)");
			// no alert.OrderFollowed here!
			//this.backtestBroker.ScriptExecutor.RemovePendingAlertClosePosition(alert, "MarketSim:AnnihilateCounterparty(): ");
			return true;
		}
		
		public void AlertPending_simulateKill(Alert alert) {
			alert.IsKilled = true;
			this.backtestBroker.ScriptExecutor.CallbackAlertKilled_invokeScript_nonReenterably(alert);
		}
	}
}