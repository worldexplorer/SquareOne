using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.Backtesting {
	public abstract class BacktestQuotesGenerator {
		Backtester backtester;
		public readonly BacktestMode BacktestModeSuitsFor;
		public readonly int QuotePerBarGenerates;
		protected SortedList<int, QuoteGenerated> QuotesGeneratedForOneBar;

		public abstract SortedList<int, QuoteGenerated> GenerateQuotesFromBar(Bar bar);
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
			if (barSimulated.ParentBarsIndex == 11) {
				Debugger.Break();
			}
			ret.Absno = ++this.QuoteAbsno;
			if (ret.Absno == 46) {
				Debugger.Break();
			}
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

		public virtual int InjectQuotesToFillPendingAlerts(QuoteGenerated quoteToReach, Bar bar2simulate) {
			int quotesInjected = 0;
			int pendingsToFillInitially = this.backtester.Executor.ExecutionDataSnapshot.AlertsPending.Count;
			if (pendingsToFillInitially == 0) return quotesInjected;

			int iterationsLimit = 5;
			// hard to debug but I hate while(){} loops
			//for (QuoteGenerated closestOnOurWay  = this.generateClosestQuoteForEachPendingAlertOnOurWayTo(quoteToReach);
			//           closestOnOurWay != null;
			//           closestOnOurWay  = this.generateClosestQuoteForEachPendingAlertOnOurWayTo(quoteToReach)) {
			QuoteGenerated closestOnOurWay  = this.generateClosestQuoteForEachPendingAlertOnOurWayTo(quoteToReach);
			while (closestOnOurWay != null) {
				// GENERATED_QUOTE_OUT_OF_BOUNDARY_CHECK #2/2
				if (bar2simulate.ContainsQuoteGenerated(closestOnOurWay) == false) {
					Debugger.Break();
					continue;
				}

				quotesInjected++;
				closestOnOurWay.IntraBarSerno += quotesInjected;
				this.backtester.BacktestDataSource.BacktestStreamingProvider.PushQuoteReceived(closestOnOurWay);
				int pendingAfterInjected = this.backtester.Executor.ExecutionDataSnapshot.AlertsPending.Count;
				if (pendingsToFillInitially != pendingAfterInjected) {
					string msg = "it looks like the quoteInjected triggered something";
					//Debugger.Break();
				}
				if (quotesInjected > iterationsLimit) {
					string msg = "InjectQuotesToFillPendingAlerts(): quotesInjected["
						+ quotesInjected + "] > iterationsLimit[" + iterationsLimit + "]"
						+ " pendingNow[" + this.backtester.Executor.ExecutionDataSnapshot.AlertsPending.Count + "]"
						+ " quoteToReach[" + quoteToReach + "]";
					//throw new Exception(msg);
					break;
				}
				if (this.backtester.BacktestAborted.WaitOne(0)) break;
				if (this.backtester.RequestingBacktestAbort.WaitOne(0)) break;
				closestOnOurWay = this.generateClosestQuoteForEachPendingAlertOnOurWayTo(quoteToReach);
			}
			return quotesInjected;
		}
		public QuoteGenerated generateClosestQuoteForEachPendingAlertOnOurWayTo(QuoteGenerated quoteToReach) {
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
	}
}