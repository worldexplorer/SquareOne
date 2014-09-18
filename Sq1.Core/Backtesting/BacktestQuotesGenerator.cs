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
		SortedList<int, QuoteGenerated> QuotesGeneratedForOneBar;

		// IMPLEMENTED_BELOW_UNABSTRACTED_THIS_CLASS protected public abstract SortedList<int, QuoteGenerated> GenerateQuotesFromBarAvoidClearing(Bar bar);
		public int QuoteAbsno;

		protected BacktestQuotesGenerator(Backtester backtester, int quotesPerBar, BacktestMode mode) {
			this.backtester = backtester;
			this.QuotePerBarGenerates = quotesPerBar;
			this.BacktestModeSuitsFor = mode;
			this.QuotesGeneratedForOneBar = new SortedList<int, QuoteGenerated>();
			this.QuoteAbsno = 0;
		}

		protected QuoteGenerated generateNewQuoteChildrenHelper(int intraBarSerno, string source, string symbol, DateTime serverTime, double price, double volume, Bar barSimulated) {
			QuoteGenerated ret = new QuoteGenerated();
			ret.Absno = ++this.QuoteAbsno;
			ret.ServerTime = serverTime;
			ret.IntraBarSerno = intraBarSerno;
			ret.Source = source;
			ret.Symbol = symbol;
			ret.SymbolClass = this.backtester.BarsOriginal.SymbolInfo.SymbolClass;
			ret.Size = volume;
			ret.PriceLastDeal = price;
			// moved to BacktestQuoteModeler
			//quote.Bid = quote.PriceLastDeal - 10;
			//quote.Ask = quote.PriceLastDeal + 10;
			ret.ParentBarSimulated = barSimulated;
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
			QuoteGenerated closestOnOurWay  = this.GenerateClosestQuoteForEachPendingAlertOnOurWayTo(quoteToReach);
			while (closestOnOurWay != null) {
				// GENERATED_QUOTE_OUT_OF_BOUNDARY_CHECK #2/2
				if (bar2simulate.ContainsBidAskForQuoteGenerated(closestOnOurWay) == false) {
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
				if (this.backtester.BacktestAborted.WaitOne(0)) break;
				if (this.backtester.RequestingBacktestAbort.WaitOne(0)) break;
				closestOnOurWay = this.GenerateClosestQuoteForEachPendingAlertOnOurWayTo(quoteToReach);
			}
			return ret;
		}
		public QuoteGenerated GenerateClosestQuoteForEachPendingAlertOnOurWayTo(QuoteGenerated quoteToReach) {
			if (this.backtester.Executor.ExecutionDataSnapshot.AlertsPending.Count == 0) {
				string msg = "it looks like no Pending alerts are left anymore";
				return null;
			}

			Quote quotePrevDowncasted = this.backtester.BacktestDataSource.BacktestStreamingProvider.StreamingDataSnapshot
				.LastQuoteGetForSymbol(quoteToReach.Symbol);

			QuoteGenerated quotePrev = quotePrevDowncasted as QuoteGenerated;
			if (quotePrev == null) {
				#if DEBUG
				Debugger.Break();
				#endif
			}

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
				QuoteGenerated quoteThatWillFillAlert = this.modelQuoteThatCouldFillAlert(alert);
				if (quoteThatWillFillAlert == null) continue;


				// trying to keep QuoteGenerated within the original simulated Bar (lazy to attach StreamingBar to QuoteGenerated now)
				if (scanningDown) {
					if (quoteThatWillFillAlert.Bid > quotePrev.Bid) {
						string msg = "IGNORING_QUOTE_HIGHER_THAN_PREVIOUS_WHILE_SCANNING_DOWN";
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
				}
				if (quoteClosest.Bid < quoteToReach.Bid) {
					string msg = "WHILE_SCANNING_DOWN_RETURNING_QUOTE_LOWER_THAN_TARGET_IS_WRONG";
				}
			} else {
				if (quoteClosest.Ask < quotePrev.Ask) {
					string msg = "WHILE_SCANNING_UP_RETURNING_QUOTE_LOWER_THAN_PREVIOUS_IS_WRONG";
				}
				if (quoteClosest.Ask > quoteToReach.Ask) {
					string msg = "WHILE_SCANNING_UP_RETURNING_QUOTE_HIGHER_THAN_TARGET_IS_WRONG";
				}
			}

			//I_WILL_SPOIL_STREAMING_BAR_IF_I_ATTACH_LIKE_THIS QuoteGenerated quoteNextAttached = this.backtester.BacktestDataSource.BacktestStreamingProvider.(quoteToReach.Clone());
			QuoteGenerated ret = quotePrev.DeriveIdenticalButFresh();
			ret.Bid = quoteClosest.Bid;
			ret.Ask = quoteClosest.Ask;
			ret.Size = quoteClosest.Size;
			ret.Source = quoteClosest.Source;
			return ret;
		}
		QuoteGenerated modelQuoteThatCouldFillAlert(Alert alert) {
			string err;

			QuoteGenerated quoteModel = new QuoteGenerated();
			//quoteModel.Source = "GENERATED_TO_FILL_" + alert.ToString();			// PROFILER_SAID_TOO_SLOW + alert.ToString();
			quoteModel.Source = "GENERATED_TO_FILL_alert@bar#" + alert.PlacedBarIndex;
			quoteModel.Size = alert.Qty;

			double priceScriptAligned = this.backtester.Executor.AlignAlertPriceToPriceLevel(alert.Bars, alert.PriceScript, true,
				alert.PositionLongShortFromDirection, alert.MarketLimitStop);

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
							this.backtester.BacktestDataSource.BacktestStreamingProvider.SpreadModeler
								.GenerateFillBidBasedOnAsk(quoteModel, priceScriptAligned);
							break;
						case Direction.Short:
						case Direction.Sell:
							if (priceScriptAligned < quotePrev.Bid) {
								err = "INVALID_PRICESCRIPT_FOR_LIMIT_SELL_MUST_NOT_BE_BELOW_CURRENT_BID";
								return null;
							}
							this.backtester.BacktestDataSource.BacktestStreamingProvider.SpreadModeler
								.GenerateFillAskBasedOnBid(quoteModel, priceScriptAligned);
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
							this.backtester.BacktestDataSource.BacktestStreamingProvider.SpreadModeler
								.GenerateFillBidBasedOnAsk(quoteModel, priceScriptAligned);
							break;
						case Direction.Short:
						case Direction.Sell:
							if (priceScriptAligned > quotePrev.Bid) {
								err = "INVALID_PRICESCRIPT_FOR_STOP_SELL_MUST_NOT_BE_ABOVE_CURRENT_BID";
								return null;
							}
							this.backtester.BacktestDataSource.BacktestStreamingProvider.SpreadModeler
								.GenerateFillAskBasedOnBid(quoteModel, priceScriptAligned);
							break;
						default:
							throw new Exception("ALERT_STOP_DIRECTION_UNKNOWN direction[" + alert.Direction + "] is not Buy/Cover/Short/Sell modelQuoteThatCouldFillAlert()");
					}
					break;
				case MarketLimitStop.Market:
					switch (alert.Direction) {
						case Direction.Buy:
						case Direction.Cover:
							quoteModel.Ask = quotePrev.Ask;
							this.backtester.BacktestDataSource.BacktestStreamingProvider.SpreadModeler
								.GenerateFillBidBasedOnAsk(quoteModel, quotePrev.Ask);
							break;
						case Direction.Short:
						case Direction.Sell:
							quoteModel.Bid = quotePrev.Bid;
							this.backtester.BacktestDataSource.BacktestStreamingProvider.SpreadModeler
								.GenerateFillAskBasedOnBid(quoteModel, quotePrev.Bid);
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

			return quoteModel;
		}
		public virtual SortedList<int, QuoteGenerated> GenerateQuotesFromBarAvoidClearing(Bar barSimulated) {
			this.QuotesGeneratedForOneBar.Clear();

			double volumeOneQuarterOfBar = barSimulated.Volume / 4;
			if (barSimulated.ParentBars != null && barSimulated.ParentBars.SymbolInfo != null) {
				volumeOneQuarterOfBar = Math.Round(volumeOneQuarterOfBar, barSimulated.ParentBars.SymbolInfo.DecimalsVolume);
				if (volumeOneQuarterOfBar == 0) {
					#if DEBUG	// TEST_EMBEDDED
					//TESTED Debugger.Break();
					//double minimalValue = Math.Pow(1, -decimalsVolume);		// 1^(-2) = 0.01
				    #endif
					volumeOneQuarterOfBar = barSimulated.ParentBars.SymbolInfo.VolumeMinimalFromDecimal;
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
				double price = 0;
				switch (stroke) {
					case 0: price =  barSimulated.Open; break;
					case 1: price = (barSimulated.Close > barSimulated.Open) ? barSimulated.Low : barSimulated.High; break;
					case 2: price = (barSimulated.Close > barSimulated.Open) ? barSimulated.High : barSimulated.Low; break;
					case 3: price =  barSimulated.Close; break;
					default: throw new Exception("Stroke[" + stroke + "] isn't supported in 4-stroke QuotesGenerator");
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
				
				QuoteGenerated quote = this.generateNewQuoteChildrenHelper(stroke, "RunSimulationFourStrokeOHLC",
					barSimulated.Symbol, serverTime, price, volumeOneQuarterOfBar, barSimulated);
				this.QuotesGeneratedForOneBar.Add(stroke, quote);
				
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