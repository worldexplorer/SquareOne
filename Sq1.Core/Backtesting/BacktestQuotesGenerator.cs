using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.Backtesting {
	public class BacktestQuotesGenerator {
		Backtester backtester;
		public readonly BacktestMode BacktestModeSuitsFor;
		public readonly int QuotePerBarGenerates;
		List<QuoteGenerated> QuotesGeneratedForOneBar;

		// IMPLEMENTED_BELOW_UNABSTRACTED_THIS_CLASS protected public abstract SortedList<int, QuoteGenerated> GenerateQuotesFromBarAvoidClearing(Bar bar);
		public int QuoteAbsno;

		protected string GeneratorName;

		protected BacktestQuotesGenerator(Backtester backtester, int quotesPerBar, BacktestMode mode) {
			this.backtester = backtester;
			this.QuotePerBarGenerates = quotesPerBar;
			this.BacktestModeSuitsFor = mode;
			this.QuotesGeneratedForOneBar = new List<QuoteGenerated>();
			this.QuoteAbsno = 0;
			this.GeneratorName = this.GetType().Name;
		}

		protected QuoteGenerated generateNewQuoteChildrenHelper(int intraBarSerno, string whoGenerated, string symbol, DateTime serverTime,
				BidOrAsk bidOrAsk, double priceFromAlignedBar, double volume, Bar barSimulated) {
			QuoteGenerated ret = new QuoteGenerated(serverTime);
			ret.Absno = ++this.QuoteAbsno;
			ret.ServerTime = serverTime;
			ret.IntraBarSerno = intraBarSerno;
			ret.Source = whoGenerated;
			ret.Symbol = symbol;
			ret.SymbolClass = this.backtester.BarsOriginal.SymbolInfo.SymbolClass;
			ret.Size = volume;
			ret.ParentBarSimulated = barSimulated;
			//v1 ret.PriceLastDeal = price;
			switch (bidOrAsk) {
				case BidOrAsk.Bid:
					ret.Bid = priceFromAlignedBar;
					ret.LastDealBidOrAsk = BidOrAsk.Bid;
					this.backtester.BacktestDataSource.BacktestStreamingProvider.SpreadModeler.GenerateFillAskBasedOnBid(ret);
					if (ret.Spread == 0) {
						Debugger.Break();
					}
					break;
				case BidOrAsk.Ask:
					ret.Ask = priceFromAlignedBar;
					ret.LastDealBidOrAsk = BidOrAsk.Ask;
					this.backtester.BacktestDataSource.BacktestStreamingProvider.SpreadModeler.GenerateFillBidBasedOnAsk(ret);
					if (ret.Spread == 0) {
						Debugger.Break();
					}
					break;
				case BidOrAsk.UNKNOWN:
					this.backtester.BacktestDataSource.BacktestStreamingProvider.SpreadModeler.GeneratedQuoteFillBidAsk(ret, barSimulated, priceFromAlignedBar);
					// I_DONT_KNOW_WHAT_TO_PUT_HERE
					ret.LastDealBidOrAsk = BidOrAsk.Bid;
					if (ret.Spread == 0) {
						Debugger.Break();
					}
					break;
			}

			// see QUOTEGEN_PROBLEM#2 : at Open/Close, when they are == to Low/High, the Symmetrical quote will go beoynd bar boundaries => MarketSim will freak out
			// 1) moved to GeneratedQuoteFillBidAsk() //replaced by BacktestQuoteGenerator.AlignBidAskToPriceLevel
			// 2) irrelevant after barSimulated.OHLC <= barOriginalOHLC, which got aligned during BAR file is read 
			// 3) should be done only for strokes 2 and 3
			//if (ret.Ask > barSimulated.High) {
			//	double pushDown = ret.Ask - barSimulated.High;
			//	ret.Ask -= pushDown;
			//	ret.Bid -= pushDown;
			//}
			//if (ret.Bid < barSimulated.Low) {
			//	double pushUp = barSimulated.Low - ret.Bid;
			//	ret.Ask += pushUp;
			//	ret.Bid += pushUp;
			//}

			return ret;
		}

		public virtual List<QuoteGenerated> InjectQuotesToFillPendingAlerts(QuoteGenerated quoteToReach, Bar bar2simulate, int iterationsLimit = 5) {
			List<QuoteGenerated> ret = new List<QuoteGenerated>();
			int pendingsToFillInitially = this.backtester.Executor.ExecutionDataSnapshot.AlertsPending.Count;
			if (pendingsToFillInitially == 0) return ret;

			// hard to debug but I hate while(){} loops
			//for (QuoteGenerated closestOnOurWay  = this.generateClosestQuoteForEachPendingAlertOnOurWayTo(quoteToReach);
			//           closestOnOurWay != null;
			//           closestOnOurWay  = this.generateClosestQuoteForEachPendingAlertOnOurWayTo(quoteToReach)) {

			if (bar2simulate.HighLowDistance == 0) {
				string msg = "WEIRD_BAR_SHOULDNT_FAIL_FOLLOWING_QUOTE_CHECK ContainsBidAskForQuoteGenerated(quoteToReach)";
				return ret;
			}

			if (bar2simulate.ContainsBidAskForQuoteGenerated(quoteToReach) == false) {
				string msg = "KEEP_ME_HERE_DONT_MAKE_LOOP_UPSTACK_COMPLICATED";
				return ret;
			}

			QuoteGenerated closestOnOurWay  = this.GenerateClosestQuoteForEachPendingAlertOnOurWayTo(quoteToReach, bar2simulate);
			while (closestOnOurWay != null) {
				// GENERATED_QUOTE_OUT_OF_BOUNDARY_CHECK #2/2
				//v1 if (bar2simulate.ContainsBidAskForQuoteGenerated(closestOnOurWay) == false) {
				if (bar2simulate.HighLowDistance > 0 && bar2simulate.HighLowDistance > closestOnOurWay.Spread
						&& bar2simulate.ContainsBidAskForQuoteGenerated(closestOnOurWay) == false) {
					Debugger.Break();
					continue;
				}

				closestOnOurWay.IntraBarSerno += ret.Count;
				closestOnOurWay.Absno = ++this.QuoteAbsno;		//DONT_FORGET_TO_ASSIGN_LATEST_ABSNO_TO_QUOTE_TO_REACH

				this.backtester.BacktestDataSource.BacktestStreamingProvider.PushQuoteReceived(closestOnOurWay);
				ret.Add(closestOnOurWay);

				int pendingAfterInjected = this.backtester.Executor.ExecutionDataSnapshot.AlertsPending.Count;
				if (pendingsToFillInitially != pendingAfterInjected) {
					string msg = "it looks like the quoteInjected triggered something";
					//Debugger.Break();
				}
				if (ret.Count > iterationsLimit) {
					string msg = "InjectQuotesToFillPendingAlerts(): quotesInjected["
						+ ret + "] > iterationsLimit[" + iterationsLimit + "]"
						+ " pendingNow[" + this.backtester.Executor.ExecutionDataSnapshot.AlertsPending.Count + "]"
						+ " quoteToReach[" + quoteToReach + "]";
					//throw new Exception(msg);
					break;
				}
				bool abortRequested = this.backtester.RequestingBacktestAbort.WaitOne(0);
				if (abortRequested) {
					break;
				}
				bool backtestAborted = this.backtester.BacktestAborted.WaitOne(0);
				if (backtestAborted) {
					break;
				}
				closestOnOurWay = this.GenerateClosestQuoteForEachPendingAlertOnOurWayTo(quoteToReach, bar2simulate);
			}
			return ret;
		}
		public QuoteGenerated GenerateClosestQuoteForEachPendingAlertOnOurWayTo(QuoteGenerated quoteToReach, Bar bar2simulate) {
			if (this.backtester.Executor.ExecutionDataSnapshot.AlertsPending.Count == 0) {
				string msg = "it looks like no Pending alerts are left anymore";
				return null;
			}

			// PARANOINDAL_CHECK_IF_PREV_QUOTE_IS_QUOTE_TO_REACH copypaste
			Quote quotePrevDowncasted = this.backtester.BacktestDataSource.BacktestStreamingProvider.StreamingDataSnapshot
				.LastQuoteGetForSymbol(quoteToReach.Symbol);
			QuoteGenerated quotePrev = quotePrevDowncasted as QuoteGenerated;
			if (quotePrev == null) {
				#if DEBUG
				Debugger.Break();
				#endif
			}

			if (bar2simulate.HighLowDistance > 0) {
				// IRRELEVANT TO COMPARE PREV_QUOTE BID AGAINST THIS_BAR_ASK
				//if (bar2simulate.HighLowDistance > quotePrev.Spread && bar2simulate.ContainsBidAskForQuoteGenerated(quotePrev) == false) {
				//	Debugger.Break();
				//}
				if (bar2simulate.HighLowDistance > quoteToReach.Spread && bar2simulate.ContainsBidAskForQuoteGenerated(quoteToReach) == false) {
					Debugger.Break();
				}
			}
			// PARANOINDAL_CHECK_IF_PREV_QUOTE_IS_QUOTE_TO_REACH

			// assuming both quotes generated using same SpreadModeler and their spreads are equal
			//v1 if (quoteToReach.Bid == quotePrev.Bid || quoteToReach.Ask == quotePrev.Ask) {
			if (quoteToReach.SameBidAsk(quotePrev)) {
				string msg = "WE_REACHED_QUOTE_TO_REACH_SAME_BIDASK_NOTHING_ELSE_TO_GENERATE";
				return null;
			}
			if (quotePrev.ServerTime >= quoteToReach.ServerTime) {
				string msg = "WE_REACHED_QUOTE_TO_REACH_SAME_SERVERTIME_NOTHING_ELSE_TO_GENERATE";
				return null;
			}

			bool scanningDown = quoteToReach.Bid < quotePrev.Bid;
			QuoteGenerated quoteClosest = null;

			List<Alert> alertsPendingSafeCopy = new List<Alert>(this.backtester.Executor.ExecutionDataSnapshot.AlertsPending);
			foreach (Alert alert in alertsPendingSafeCopy) {
				// DONT_EXPECT_THEM_TO_BE_FILLED_YOU_SHOULD_FILL_ALL_RELEVANT_NOW
				//if (scanningDown) {
				//    // while GEN'ing down, all BUY/COVER STOPs pending were already triggered & executed
				//    bool executedAtAsk = alert.Direction == Direction.Buy || alert.Direction == Direction.Cover;
				//    if (executedAtAsk && alert.MarketLimitStop == MarketLimitStop.Stop) continue;
				//} else {
				//    // while GEN'ing up, all SHORT/SELL STOPs pending were already triggered & executed
				//    bool executedAtBid = alert.Direction == Direction.Short || alert.Direction == Direction.Sell;
				//    if (executedAtBid && alert.MarketLimitStop == MarketLimitStop.Stop) continue;
				//}
				QuoteGenerated quoteThatWillFillAlert = this.modelQuoteThatCouldFillAlert(alert, new DateTime(quoteToReach.LocalTimeCreatedMillis.Ticks - 911), bar2simulate);
				if (quoteThatWillFillAlert == null) continue;

				// trying to keep QuoteGenerated within the original simulated Bar (lazy to attach StreamingBar to QuoteGenerated now)
				if (scanningDown) {
					if (quoteThatWillFillAlert.Bid > quotePrev.Bid) {
						string msg = "IGNORING_QUOTE_HIGHER_THAN_PREVIOUS_WHILE_SCANNING_DOWN";
						continue;
					}
					if (bar2simulate.HighLowDistance > 0 && bar2simulate.HighLowDistance > quoteToReach.Spread
							&& bar2simulate.ContainsBidAskForQuoteGenerated(quoteThatWillFillAlert, true) == false) {
						string msg = "IGNORING_QUOTE_BEYOND_BAR_DISTANCE_WHILE_SCANNING_DOWN";
						continue;
					}
					if (quoteThatWillFillAlert.Bid < quoteToReach.Bid) {
						string msg = "IGNORING_QUOTE_LOWER_THAN_TARGET_WHILE_SCANNING_DOWN";
						continue;
					}
				} else {
					if (quoteThatWillFillAlert.Ask < quotePrev.Ask) {
						string msg = "IGNORING_QUOTE_LOWER_THAN_PREVIOUS_WHILE_SCANNING_UP";
						continue;
					}
					if (bar2simulate.HighLowDistance > 0 && bar2simulate.HighLowDistance > quoteToReach.Spread
							&& bar2simulate.ContainsBidAskForQuoteGenerated(quoteThatWillFillAlert, true) == false) {
						string msg = "IGNORING_QUOTE_BEYOND_BAR_DISTANCE_WHILE_SCANNING_UP";
						continue;
					}
					if (quoteThatWillFillAlert.Ask > quoteToReach.Ask) {
						string msg = "IGNORING_QUOTE_HIGHER_THAN_TARGET_WHILE_SCANNING_UP";
						continue;
					}
				}

				if (quoteClosest == null) {
					quoteClosest = quoteThatWillFillAlert;
				} else {
					if (scanningDown) {
						// while GEN'ing closest to the top, pick the highest possible
						// while scanning down, I'm looking for the QuoteGenerated with highest Ask that will trigger the closest pending
						if (quoteThatWillFillAlert.Bid < quoteClosest.Bid) continue;
						quoteClosest = quoteThatWillFillAlert;
					} else {
						// while GEN'ing closest to the bottom, pick the lowest possible
						if (quoteThatWillFillAlert.Ask > quoteClosest.Ask) continue;
						quoteClosest = quoteThatWillFillAlert;
					}
				}
			}

			if (quoteClosest == null) return quoteClosest;

			if (scanningDown) {
				if (quoteClosest.Bid > quotePrev.Bid) {
					string msg = "WHILE_SCANNING_DOWN_RETURNING_QUOTE_HIGHER_THAN_PREVIOUS_IS_WRONG";
					Debugger.Break();
				}
				if (quoteClosest.Bid < quoteToReach.Bid) {
					string msg = "WHILE_SCANNING_DOWN_RETURNING_QUOTE_LOWER_THAN_TARGET_IS_WRONG";
					Debugger.Break();
				}
			} else {
				if (quoteClosest.Ask < quotePrev.Ask) {
					string msg = "WHILE_SCANNING_UP_RETURNING_QUOTE_LOWER_THAN_PREVIOUS_IS_WRONG";
					Debugger.Break();
				}
				if (quoteClosest.Ask > quoteToReach.Ask) {
					string msg = "WHILE_SCANNING_UP_RETURNING_QUOTE_HIGHER_THAN_TARGET_IS_WRONG";
					Debugger.Break();
				}
			}

			//I_WILL_SPOIL_STREAMING_BAR_IF_I_ATTACH_LIKE_THIS QuoteGenerated quoteNextAttached = this.backtester.BacktestDataSource.BacktestStreamingProvider.(quoteToReach.Clone());
			QuoteGenerated ret = quotePrev.DeriveIdenticalButFresh();
			ret.Bid = quoteClosest.Bid;
			ret.Ask = quoteClosest.Ask;
			//FILLED_AT_ALERT_FILL ret.PriceLastDeal = (ret.Ask + ret.Bid) / 2;
			ret.Size = quoteClosest.Size;
			ret.Source = quoteClosest.Source;
			return ret;
		}
		QuoteGenerated modelQuoteThatCouldFillAlert(Alert alert, DateTime localDateTimeBasedOnServerForBacktest, Bar bar2simulate) {
			string err;

			QuoteGenerated ret = new QuoteGenerated(localDateTimeBasedOnServerForBacktest);
			//ret.Source = "GENERATED_TO_FILL_" + alert.ToString();			// PROFILER_SAID_TOO_SLOW + alert.ToString();
			ret.Source = "GENERATED_TO_FILL_alert@bar#" + alert.PlacedBarIndex;
			ret.Size = alert.Qty;
			ret.ParentBarSimulated = bar2simulate;

			double priceScriptAligned = this.backtester.Executor.AlignAlertPriceToPriceLevel(alert.Bars, alert.PriceScript, true,
				alert.PositionLongShortFromDirection, alert.MarketLimitStop);
			
			//quick check
			if (priceScriptAligned != alert.PriceScript) {
				if (alert.MarketLimitStop == MarketLimitStop.Market) {
					string msg = "WHY_YOU_DID_CHANGE_THE_PRICE__PRICE_SCRIPT_MUST_BE_ALREADY_GOOD";
					#if DEBUG
					Debugger.Break();
					#endif
				}
			}

			//long check, switch marketSim calculations to alert.PriceScriptAligned 
			if (priceScriptAligned != alert.PriceScriptAligned) {
				string msg = "FIX_Alert.PriceScriptAligned";
				Debugger.Break();
			} else {
				string msg = "GET_RID_OF_COMPLEX_ALIGNMENT executor.AlignAlertPriceToPriceLevel()";
			}


			Quote quotePrevDowncasted = this.backtester.BacktestDataSource.BacktestStreamingProvider.StreamingDataSnapshot
				.LastQuoteGetForSymbol(alert.Symbol);

			QuoteGenerated quotePrev = quotePrevDowncasted as QuoteGenerated;
			if (quotePrev == null) {
				#if DEBUG
				Debugger.Break();
				#endif
			}

			switch (alert.MarketLimitStop) {
				case MarketLimitStop.Limit:
					switch (alert.Direction) {
						case Direction.Buy:
						case Direction.Cover:
							if (priceScriptAligned > quotePrev.Ask) {
								err = "INVALID_PRICESCRIPT_FOR_LIMIT_BUY_MUST_NOT_BE_ABOVE_CURRENT_ASK";
								return null;
							}
							ret.Ask = priceScriptAligned;
							ret.LastDealBidOrAsk = BidOrAsk.Ask;
							this.backtester.BacktestDataSource.BacktestStreamingProvider.SpreadModeler
								.GenerateFillBidBasedOnAsk(ret);
							break;
						case Direction.Short:
						case Direction.Sell:
							if (priceScriptAligned < quotePrev.Bid) {
								err = "INVALID_PRICESCRIPT_FOR_LIMIT_SELL_MUST_NOT_BE_BELOW_CURRENT_BID";
								return null;
							}
							ret.Bid = priceScriptAligned;
							ret.LastDealBidOrAsk = BidOrAsk.Bid;
							this.backtester.BacktestDataSource.BacktestStreamingProvider.SpreadModeler
								.GenerateFillAskBasedOnBid(ret);
							break;
						default:
							throw new Exception("ALERT_LIMIT_DIRECTION_UNKNOWN direction[" + alert.Direction + "] is not Buy/Cover/Short/Sell modelQuoteThatCouldFillAlert()");
					}
					break;
				case MarketLimitStop.Stop:
					switch (alert.Direction) {
						case Direction.Buy:
						case Direction.Cover:
							if (priceScriptAligned < quotePrev.Ask) {
								err = "INVALID_PRICESCRIPT_FOR_STOP_BUY_MUST_NOT_BE_BELOW_CURRENT_ASK";
								return null;
							}
							ret.Ask = priceScriptAligned;
							ret.LastDealBidOrAsk = BidOrAsk.Ask;
							this.backtester.BacktestDataSource.BacktestStreamingProvider.SpreadModeler
								.GenerateFillBidBasedOnAsk(ret);
							break;
						case Direction.Short:
						case Direction.Sell:
							if (priceScriptAligned > quotePrev.Bid) {
								err = "INVALID_PRICESCRIPT_FOR_STOP_SELL_MUST_NOT_BE_ABOVE_CURRENT_BID";
								return null;
							}
							ret.Bid = priceScriptAligned;
							ret.LastDealBidOrAsk = BidOrAsk.Bid;
							this.backtester.BacktestDataSource.BacktestStreamingProvider.SpreadModeler
								.GenerateFillAskBasedOnBid(ret);
							break;
						default:
							throw new Exception("ALERT_STOP_DIRECTION_UNKNOWN direction[" + alert.Direction + "] is not Buy/Cover/Short/Sell modelQuoteThatCouldFillAlert()");
					}
					break;
				case MarketLimitStop.Market:
					switch (alert.Direction) {
						case Direction.Buy:
						case Direction.Cover:
							ret.Ask = quotePrev.Ask;
							ret.LastDealBidOrAsk = BidOrAsk.Ask;
							this.backtester.BacktestDataSource.BacktestStreamingProvider.SpreadModeler
								.GenerateFillBidBasedOnAsk(ret);
							break;
						case Direction.Short:
						case Direction.Sell:
							ret.Bid = quotePrev.Bid;
							ret.LastDealBidOrAsk = BidOrAsk.Bid;
							this.backtester.BacktestDataSource.BacktestStreamingProvider.SpreadModeler
								.GenerateFillAskBasedOnBid(ret);
							break;
						default:
							throw new Exception("ALERT_MARKET_DIRECTION_UNKNOWN direction[" + alert.Direction + "] is not Buy/Cover/Short/Sell modelQuoteThatCouldFillAlert()");
					}
					break;
				case MarketLimitStop.StopLimit: // HACK one QuoteGenerated might satisfy SLactivation, the other one might fill it; time to introduce state of SL into Alert???
					string msg = "STOP_LIMIT_QUOTE_FILLING_GENERATION_REQUIRES_TWO_STEPS_NYI"
						+ "; pass SLActivation=0 to PositionPrototypeActivator so that it generates STOP instead of STOPLOSS which I can't generate yet";
					//Debugger.Break();
					break;
				case MarketLimitStop.AtClose:
				default:
					throw new Exception("ALERT_TYPE_UNKNOWN MarketLimitStop[" + alert.MarketLimitStop + "] is not Market/Limit/Stop modelQuoteThatCouldFillAlert()");
			}

			return ret;
		}
		public virtual List<QuoteGenerated> GenerateQuotesFromBarAvoidClearing(Bar barSimulated) {
			this.QuotesGeneratedForOneBar.Clear();

			double volumeOneQuarterOfBar = barSimulated.Volume / this.QuotePerBarGenerates;
			if (barSimulated.ParentBars != null && barSimulated.ParentBars.SymbolInfo != null) {
				volumeOneQuarterOfBar = Math.Round(volumeOneQuarterOfBar, barSimulated.ParentBars.SymbolInfo.DecimalsVolume);
				if (volumeOneQuarterOfBar == 0) {
					#if DEBUG	// TEST_EMBEDDED
					//TESTED Debugger.Break();
					//double minimalValue = Math.Pow(1, -decimalsVolume);		// 1^(-2) = 0.01
				    #endif
					volumeOneQuarterOfBar = barSimulated.ParentBars.SymbolInfo.VolumeMinimalStepFromDecimal;
				}
			}
			if (volumeOneQuarterOfBar == 0) {
				Debugger.Break();
				volumeOneQuarterOfBar = 1;
			}

			DateTime barOpenNext = barSimulated.DateTimeNextBarOpenUnconditional;
			
			DateTime barOpenOrResume = barSimulated.DateTimeOpen;
			MarketInfo marketInfo = barSimulated.ParentBars.MarketInfo;
			MarketClearingTimespan clearing = marketInfo.GetSingleClearingTimespanIfMarketSuspendedDuringBar(barOpenOrResume, barOpenNext);
			if (clearing != null) {
				DateTime resumes = clearing.ResumeServerTimeOfDay;
				barOpenOrResume = new DateTime(barOpenOrResume.Year, barOpenOrResume.Month, barOpenOrResume.Day,
					resumes.Hour, resumes.Minute, resumes.Second);
			}

			TimeSpan leftTillNextBar = barOpenNext - barOpenOrResume;
			int incrementSeconds = ((int)(leftTillNextBar.TotalSeconds / this.QuotePerBarGenerates));	// WRONG -1 one second less to not have exactly next bar opening at last stroke but 4 seconds before
			TimeSpan increment = new TimeSpan(0, 0, incrementSeconds);
			TimeSpan cumulativeOffset = new TimeSpan(0);
			
			for (int stroke = 0; stroke < this.QuotePerBarGenerates; stroke++) {
				bool whiteCandle = barSimulated.Close > barSimulated.Open;
				double price = 0;
				BidOrAsk bidOrAsk = BidOrAsk.UNKNOWN;
				switch (stroke) {
					case 0:
						price		= barSimulated.Open;
						bidOrAsk	= BidOrAsk.UNKNOWN;
						// QUOTEGEN_PROBLEM#1 : here we don't know the derived.Spread, trying to avoid SymmetricFill 
						// don't fail Bar.ContainsBidAskForQuoteGenerated() check CANT_BE_LESS_OR_GREATER_ONLY_EQUAL_BUT_DOUBLE_VALUES_HAVE_TINY_TAILS
						if (barSimulated.Open <= barSimulated.Low)		bidOrAsk = BidOrAsk.Bid; 
						if (barSimulated.Open >= barSimulated.High)		bidOrAsk = BidOrAsk.Ask; 
						break;
					case 1:
						price		= whiteCandle ? barSimulated.Low : barSimulated.High;
						bidOrAsk	= whiteCandle ? BidOrAsk.Bid : BidOrAsk.Ask;
						break;
					case 2:
						price		= whiteCandle ? barSimulated.High : barSimulated.Low;
						bidOrAsk	= whiteCandle ? BidOrAsk.Ask : BidOrAsk.Bid;
						break;
					case 3:
						price		= barSimulated.Close;
						bidOrAsk	= BidOrAsk.UNKNOWN;
						// QUOTEGEN_PROBLEM#1 : here we don't know the derived.Spread, trying to avoid SymmetricFill 
						// don't fail Bar.ContainsBidAskForQuoteGenerated() check CANT_BE_LESS_OR_GREATER_ONLY_EQUAL_BUT_DOUBLE_VALUES_HAVE_TINY_TAILS
						if (barSimulated.Close <= barSimulated.Low)		bidOrAsk = BidOrAsk.Bid;
						if (barSimulated.Close >= barSimulated.High)	bidOrAsk = BidOrAsk.Ask; 
						break;
					default:
						throw new Exception("Stroke[" + stroke + "] isn't supported in 4-stroke QuotesGenerator");
				}

				DateTime serverTime = barOpenOrResume + cumulativeOffset;
				DateTime clearingResumes = marketInfo.GetClearingResumes(serverTime);
				if (clearingResumes >= barOpenNext) {	// NO_NEED_TO_BIND_CLEARING_TO_BARS
					#if DEBUG	// TEST_EMBEDDED
					string msg = "CLEARING_EXTENDS_BEOYND_SIMULATED_BAR I_STOP_GENERATING_HERE_AND_EXPECT_BAR_INCREASE_UPSTACK";
				    //TESTED Debugger.Break();
				    #endif
				    break;
				}
				
				bool recalcShrunkenIncrement = false;
				if (serverTime != clearingResumes) {
					serverTime = clearingResumes;
					recalcShrunkenIncrement = true;
				}
				
				string generatorName = Enum.GetName(typeof(BacktestMode), this.BacktestModeSuitsFor);		//"RunSimulationFourStrokeOHLC";
				QuoteGenerated quote = this.generateNewQuoteChildrenHelper(stroke, generatorName,
					barSimulated.Symbol, serverTime, bidOrAsk, price, volumeOneQuarterOfBar, barSimulated);
				this.QuotesGeneratedForOneBar.Add(quote);
				
				if (stroke == this.QuotePerBarGenerates - 1) break;		// avoiding expensive {cumulativeOffset += increment} at last stroke 

				// below goes "stroke++" brother
				if (recalcShrunkenIncrement == false) {
					cumulativeOffset += increment;
					continue;
				}
			    
				#if DEBUG	// TEST_EMBEDDED
				//string whereAmI = "GOT_INTRABAR_CLEARING RECALC_SHRINKEN_INCREMENT_FOR_NEXT_LOOPS";
				//TESTED Debugger.Break();
			    #endif
				
			    leftTillNextBar = barOpenNext - clearingResumes;
			    int quotesLeft = this.QuotePerBarGenerates - stroke;
			    increment = new TimeSpan(0, 0, ((int)(leftTillNextBar.TotalSeconds / quotesLeft)));
			    
			    TimeSpan clearingResumesOffset = clearingResumes - barOpenOrResume;
			    cumulativeOffset = clearingResumesOffset + increment;
			    
			    // WRONG -1 one second less to not have exactly next bar opening at last stroke but 4 seconds before
				// BAR_WITHOUT_INTRABAR_CLEARING	11:00
			  	// stroke0 increment (5:00 / 4 = 1:15): 11:00:00
			  	// stroke1 increment (5:00 / 4 = 1:15): 11:01:15
			  	// stroke2 increment (5:00 / 4 = 1:15): 11:02:30
			  	// stroke3 increment (5:00 / 4 = 1:15): 11:03:45 (next stroke4 would be 11:05:00 which next bar/invocation)
				// BAR_WITH_INTRABAR_CLEARING    	[14:00...14:03]
			  	// stroke0 increment (5:00 / 4 = 1:15): 14:00:00 => 14:03:00
				// stroke1 increment (2:00 / 4 = 0:30): 14:03:30
				// stroke2 increment (2:00 / 4 = 0:30): 14:04:00
				// stroke3 increment (2:00 / 4 = 0:30): 14:04:30 (next stroke4 would be 14:05:00 which next bar/invocation)
			}
			return this.QuotesGeneratedForOneBar;
		}
	}
}