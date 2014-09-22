using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.StrategyBase {
	public class MarketSimStreaming {
		ScriptExecutor executor;
		List<Alert> stopLossesActivatedOnPreviousQuotes;

		public MarketSimStreaming(ScriptExecutor executor) {
			this.executor = executor;
			this.stopLossesActivatedOnPreviousQuotes = new List<Alert>();
		}
		public void Initialize() {
			this.stopLossesActivatedOnPreviousQuotes.Clear();
		}

		public bool SimulateFill(Alert alert, out bool abortTryFill, out string abortTryFillReason) {
			abortTryFill = false;
			abortTryFillReason = "ABORT_NO_REASON_SO_SHOULD_CONTINUE_TRY_FILL";
			if (alert.DataSource.BrokerProviderName.Contains("Mock") == false) {
				string msg = "SimulateRealtimeOrderFill() should be called only from BrokerProvidersName.Contains(Mock)"
					+ "; here you have MOCK Realtime Streaming and Broker,"
					+ " it's not a time-insensitive QuotesFromBar-generated Streaming Backtest"
					+ " (both are routed to here, MarketSim, hypothetical order execution)";
				throw new Exception(msg);
			}

			if (alert.PositionAffected == null) {
				string msg = "alertToBeKilled always has a PositionAffected, even for OnChartManual Buy/Short Market/Stop/Limit";
				throw new Exception(msg);
			}

			bool filled = false;
			double priceFill = -1;
			double slippageFill = -1;
			Quote quote = this.executor.DataSource.StreamingProvider.StreamingDataSnapshot.LastQuoteGetForSymbol(alert.Symbol);
			if (quote == null) {
				string msg = "how come LastQuoteGetForSymbol(" + alert.Symbol + ")=null??? StreamingProvider[" + this.executor.DataSource.StreamingProvider + "]";
				//this.executor.PopupException(new Exception(msg));
				return false;
			}

			if (alert.IsEntryAlert) {
				if (alert.PositionAffected.IsEntryFilled) {
					string msg = "PositionAffected.EntryFilled => did you create many threads in your QuikTerminalMock?";
					throw new Exception(msg);
				}
				filled = this.CheckEntryAlertWillBeFilledByQuote(alert, quote, out priceFill, out slippageFill);
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
				try {
					filled = this.CheckExitAlertWillBeFilledByQuote(alert, quote, out priceFill, out slippageFill);
				} catch (Exception e) {
					int a = 1;
				}
			}
			return filled;
		}
		public bool CheckEntryAlertWillBeFilledByQuote(Alert entryAlert, Quote quote, out double entryPriceOut, out double entrySlippageOutNotUsed) {
			// if entry is triggered, call position.EnterFinalize(entryPrice, entrySlippage, entryCommission);
			entryPriceOut = this.executor.AlignAlertPriceToPriceLevel(entryAlert.Bars, entryAlert.PriceScript, true,
				entryAlert.PositionLongShortFromDirection, entryAlert.MarketLimitStop);
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
							throw new Exception("CheckEntry() NYI direction[" + entryAlert.Direction + "] for [" + entryAlert + "]");
					}
					break;
				default:
					throw new Exception("NYI: marketLimitStop[" + entryAlert.MarketLimitStop + "] is not Limit or Stop");
			}
			//entryPriceOut = this.executor.AlignAlertPriceToPriceLevel(entryAlert.Bars, entryPriceOut, true,
			//	entryAlert.PositionLongShortFromDirection, entryAlert.MarketLimitStop);
			if (entryPriceOut <= 0) {
				string msg = "entryPrice[" + entryPriceOut + "]<=0 what do you mean??? get Bars.LastBar.Close for Market...";
				throw new Exception(msg);
			}
			// v1 BarStreaming with 1 quote (=> 0px height) won't contain any price you may check here;
			// v1 REMOVED because MarketLimitStop.Market haven't been filled immediately and behaved like stop/limit entries/exits;
			// v1 for Stop/Limit you already hit "return false" above; market should get whatever crazy bid/ask and get the fill at first attempt now
			// v1 INAPPROPRIATE_BECAUSE_MARKET_SIM_SHOULNT_KNOW_ANYTHING_ABOUT_ORIGINAL_SIMULATED_BAR_AND_STREAMING_IS_IN_PROGRESS
			// v1 CHECK_IF_QUOTE_ISNT_BEYOND_BAR_EARLIER_UPSTACK alternative implementation: GENERATED_QUOTE_OUT_OF_BOUNDARY_CHECK
			//if (entryAlert.Bars.BarStreaming.ContainsPrice(entryPriceOut) == false) {		// earlier version of same checkup as Position.FillEntryWith() 
			//    string msg = "QUOTE_UNFILLABLE_ON_BAR_STREAMING quote[" + quote + "] => entryPriceOut["
			//        + entryPriceOut + "] at entryAlert.Bars.BarStreaming[" + entryAlert.Bars.BarStreaming + "]";
			//    //throw new Exception(msg);
			//    #if DEBUG
			//    //Debugger.Break();
			//    #endif
			//    return false;
			//}
			//v1
			
			#if DEBUG
			if (quote.PriceBetweenBidAsk(entryPriceOut) == false) {
				string msg = "I_DONT_UNDERSTAND_HOW_I_DIDNT_DROP_THIS_QUOTE_BEFORE_BUT_I_HAVE_TO_DROP_IT_NOW";
				//Debugger.Break();
				return false;
			}
			#endif
			return true;
		}
		public bool CheckExitAlertWillBeFilledByQuote(Alert exitAlert, Quote quote
				, out double exitPriceOut, out double exitSlippageOut) {

			exitPriceOut = this.executor.AlignAlertPriceToPriceLevel(
				exitAlert.Bars, exitAlert.PriceScript, false,
				exitAlert.PositionLongShortFromDirection, exitAlert.MarketLimitStop);
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
							throw new Exception("CheckExit() NYI direction[" + exitAlert.Direction + "] for [" + exitAlert + "]");
					}
					break;
				case MarketLimitStop.StopLimit:
					if (quote.ParentStreamingBar == null) {
						string msg = "quoteToReach must be bound here!!!";
						//Debugger.Break();
					} else {
						if (quote.ParentStreamingBar.ParentBarsIndex == 133) Debugger.Break();
					}

					double priceStopActivationAligned = this.executor.AlignAlertPriceToPriceLevel(
						exitAlert.Bars, exitAlert.PriceStopLimitActivation, false,
						exitAlert.PositionLongShortFromDirection, exitAlert.MarketLimitStop);
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
								string msg = "SellLimit wasn't TOUCHED_FROM_ABOVE; staying Active but not filled at this bar" 
									+ " (TODO: if on next bar SellLimit is fillable, avoid first StopActivation condition above)";
								return false;
							}
							if (this.stopLossesActivatedOnPreviousQuotes.Contains(exitAlert) == false) {
								string msg = "DUPE simulateFillExit has previously completely filled Sell StopLimit (StopActivated+Limit) " + exitAlert;
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
								string msg = "BuyLimit wasn't TOUCHED_FROM_BELOW; staying Active but not filled at this bar"
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
							throw new Exception("CheckExit() NYI direction[" + exitAlert.Direction + "] for [" + exitAlert + "]");
					}
					break;
				case MarketLimitStop.Market:
				case MarketLimitStop.AtClose:
					switch (exitAlert.Direction) {
						case Direction.Sell:
							//exitPriceOut -= slippageNonLimit;
							break;
						case Direction.Cover:
							//exitPriceOut += slippageNonLimit;
							break;
						default:
							throw new Exception("CheckExit() NYI direction[" + exitAlert.Direction + "] for [" + exitAlert + "]");
					}
					break;
				default:
					throw new Exception("NYI marketLimitStop[" + exitAlert.MarketLimitStop + "]");
			}

			//exitPriceOut = this.executor.AlignAlertPriceToPriceLevel(exitAlert.Bars, exitPriceOut, false,
			//	exitAlert.PositionLongShortFromDirection, exitAlert.MarketLimitStop);
			if (exitPriceOut <= 0) {
				string msg = "exitPriceOut[" + exitPriceOut + "]<=0 what do you mean??? get Bars.LastBar.Close for Market...";
				throw new Exception(msg);
			}
			// v1 BarStreaming with 1 quote (=> 0px height) won't contain any price you may check here;
			// v1 REMOVED because MarketLimitStop.Market haven't been filled immediately and behaved like stop/limit entries/exits;
			// v1 for Stop/Limit you already hit "return false" above; market should get whatever crazy bid/ask and get the fill at first attempt now
			// v1 INAPPROPRIATE_BECAUSE_MARKET_SIM_SHOULNT_KNOW_ANYTHING_ABOUT_ORIGINAL_SIMULATED_BAR_AND_STREAMING_IS_IN_PROGRESS
			// v1 CHECK_IF_QUOTE_ISNT_BEYOND_BAR_EARLIER_UPSTACK alternative implementation: GENERATED_QUOTE_OUT_OF_BOUNDARY_CHECK
			//if (exitAlert.Bars.BarStreaming.ContainsPrice(exitPriceOut) == false) {		// earlier version of same checkup as Position.FillEntryWith() 
			//    string msg = "QUOTE_GENERATED_UNFILLABLE_ON_BAR_STREAMING quote[" + quote + "] => exitPriceOut["
			//        + exitPriceOut + "] at exitAlert.Bars.BarStreaming[" + exitAlert.Bars.BarStreaming + "]";
			//    //throw new Exception(msg);
			//    #if DEBUG
			//    Debugger.Break();
			//    #endif
			//    return false;
			//}
			// /v1
			
			#if DEBUG
			if (quote.PriceBetweenBidAsk(exitPriceOut) == false) {
				Debugger.Break();
			}
			#endif
			return true;
		}

		public int SimulatePendingFill(Quote quote) {
			this.checkThrowRealtimePendingQuote(quote);
			// there is no userlevel API to kill orders; in your Script, you operate SellAt/BuyAt; protoActivator.StopMove
			// killing orders is the privilege of Realtime: OrderManager kills orders by
			// 1) protoActivator.StopLossNewNegativeOffsetUpdateActivate or 2) user clicks in GUI
			//int alertsKilled = this.simulateBacktestPendingAlertsKill(currentBarToTestOn);

			int exitsFilled = 0;
			int entriesFilled = 0;

			List<Alert> alertsPendingSafeCopy = new List<Alert>(this.executor.ExecutionDataSnapshot.AlertsPending);
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

				//if (quoteToReach.ParentStreamingBar.ParentBarsIndex == 133) Debugger.Break();

				int filled = 0;
				if (alert.IsEntryAlert) {
					filled = this.simulatePendingFillEntry(alert, quote);
					entriesFilled += filled;
				} else {
					filled = this.simulatePendingFillExit(alert, quote);
					exitsFilled += filled;
				}
				if (filled == 0) continue;
				if (filled > 1) Debugger.Break();

				if (this.executor.ExecutionDataSnapshot.AlertsPendingContains(alert)) {
					string msg = "normally, the filled alert already removed by CallbackAlertFilledMoveAroundInvokeScript()";
					bool removed = this.executor.ExecutionDataSnapshot.AlertsPendingRemove(alert);
					Assembler.PopupException(msg + " SimulatePendingFill(" + quote + ")");
					Debugger.Break();
				} else {
					//Debugger.Break();
				}
			}
			return entriesFilled + exitsFilled;// + entriesStuckFilled + exitsStuckFilled;
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
				string msg = "SimulateBacktest*() should not be used for RealTime BrokerProviders and RealTime Mocks!"
					+ " make sure you invoked executor.CallbackAlertFilledInvokeScript() from where you are now";
				throw new Exception(msg);
			}
			if (this.executor.ExecutionDataSnapshot.AlertsPending.Count == 0) {
				string msg = "Before you call me, Please check executor.ExecutionDataSnapshot.AlertsPending.Count!=0";
				throw new Exception(msg);
				//return 0;
			}
			if (quote.ParentStreamingBar == null) {
				string msg = "I refuse to serve this quoteToReach.ParentStreamingBar=null";
				throw new Exception(msg);
			}
			if (quote.ParentStreamingBar.ParentBarsIndex != this.executor.Bars.Count - 1) {
				string msg = "I refuse to serve this quoteToReach.ParentStreamingBar.ParentBarsIndex["
					+ quote.ParentStreamingBar.ParentBarsIndex + "] != this.executor.Bars.Count-1[" + (this.executor.Bars.Count - 1) + "]";
				throw new Exception(msg);
			}
		}
		int simulatePendingFillEntry(Alert alert, Quote quote) {
			int filledEntries = 0;
			double priceFill = -1;
			double slippageFill = -1;
			if (quote.Absno == 523 && quote.IntraBarSerno == 100001) {
				//Debugger.Break();
			}
			bool filled = this.CheckEntryAlertWillBeFilledByQuote(alert, quote, out priceFill, out slippageFill);
			if (filled) {
				filledEntries++;
				double entryCommission = this.executor.OrderCommissionCalculate(alert.Direction,
					alert.MarketLimitStop, priceFill, alert.Qty, alert.Bars);
				int entryBarToTestOn = quote.ParentStreamingBar.ParentBarsIndex;
				// making a derived quoteToReach look like "dedicated" specifically to the filled alertToBeKilled
				if (quote.IntraBarSerno >= Quote.IntraBarSernoShiftForGeneratedTowardsPendingFill) {
					quote.Size = alert.Qty;
				}
				this.executor.CallbackAlertFilledMoveAroundInvokeScript(alert, quote, entryBarToTestOn,
					priceFill, alert.Qty, slippageFill, entryCommission);
			}
			return filledEntries;
		}
		int simulatePendingFillExit(Alert alert, Quote quote) {
			int filledExits = 0;
			double priceFill = -1;
			double slippageFill = 1;
			bool filled = this.CheckExitAlertWillBeFilledByQuote(alert, quote, out priceFill, out slippageFill);
			if (filled) {
				filledExits++;
				double exitCommission = this.executor.OrderCommissionCalculate(alert.Direction,
					alert.MarketLimitStop, priceFill, alert.Qty, alert.Bars);
				int exitBarToTestOn = quote.ParentStreamingBar.ParentBarsIndex;
				this.executor.CallbackAlertFilledMoveAroundInvokeScript(alert, quote, exitBarToTestOn,
					priceFill, alert.Qty, slippageFill, exitCommission);
			}
			return filledExits;
		}

		public void SimulateStopLossMoved(Alert alertToBeKilled) {
			Alert replacement = executor.PositionPrototypeActivator.CreateStopLossFromPositionPrototype(alertToBeKilled.PositionAffected);
			bool removed = executor.ExecutionDataSnapshot.AlertsPendingRemove(alertToBeKilled);
			//ALREADY_ADDED_BY AlertEnrichedRegister
			// executor.ExecutionDataSnapshot.AlertsPendingHistoryByBarAddNoDupe(alertToBeKilled);
			// executor.ExecutionDataSnapshot.AlertsPendingAdd(replacement);
		}
		public void SimulateTakeProfitMoved(Alert alertToBeKilled) {
			Alert replacement = executor.PositionPrototypeActivator.CreateTakeProfitFromPositionPrototype(alertToBeKilled.PositionAffected);
			bool removed = executor.ExecutionDataSnapshot.AlertsPendingRemove(alertToBeKilled);
			//ALREADY_ADDED_BY AlertEnrichedRegister
			// executor.ExecutionDataSnapshot.AlertsPendingHistoryByBarAddNoDupe(alertToBeKilled);
			// executor.ExecutionDataSnapshot.AlertsPendingAdd(replacement);
		}
		public bool AnnihilateCounterpartyAlert(Alert alert) {
			if (this.executor.ExecutionDataSnapshot.AlertsPendingContains(alert) == false) {
				string msg = "ANNIHILATE_COUNTERPARTY_ALREADY_REMOVED " + alert;	//ExecSnap.AlertsPending not synchronized: already removed
				throw new Exception(msg);
				//return false;
			}
			bool removed = this.executor.ExecutionDataSnapshot.AlertsPendingRemove(alert);
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