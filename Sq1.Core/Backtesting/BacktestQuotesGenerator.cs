using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.Backtesting {
	public abstract class BacktestQuotesGenerator {
		public readonly BacktestStrokesPerBar	BacktestStrokesPerBar;
		public readonly int						BacktestStrokesPerBarAsInt;

		public		string					REASON_TO_EXIST										{ get; protected set; }
		public		int						LastGeneratedAbsnoPerSymbol;
		public		string					WhoGeneratedQuote									{ get; protected set; }
		protected	List<QuoteGenerated>	Quotes_generatedForOneBar_amountDependsOnEngineType;
					Backtester				backtester;
		protected	int						AbsnoPerSymbol;		// reset each InitializeQuoteGenerator() <= time we run a 

		protected BacktestQuotesGenerator(BacktestStrokesPerBar engineType) {
			this.REASON_TO_EXIST				= "DUMMY_GENERATOR_TO_BE_CLONED_AND_INITIALIZED__YOU_SELECT_ME_FROM_CHART_FORM_CONTEXT_MENU";
			this.BacktestStrokesPerBar			= engineType;
			this.BacktestStrokesPerBarAsInt		= (int) this.BacktestStrokesPerBar;
			this.Quotes_generatedForOneBar_amountDependsOnEngineType		= new List<QuoteGenerated>();
			this.LastGeneratedAbsnoPerSymbol	= -1;

			string subclassName = this.GetType().Name;
			//string BacktestQuotesGeneratorString = base.GetType().Name;	// assuming no grand-children; inheritance tree is ONE level deep
			//if (subclassName.StartsWith(BacktestQuotesGeneratorString)) {
			//	subclassName = this.WhoGeneratedThisQuote.Remove(0, BacktestQuotesGeneratorString.Length);
			//}
			string strokesEnum = Enum.GetName(typeof(BacktestStrokesPerBar), this.BacktestStrokesPerBar);
			this.WhoGeneratedQuote = strokesEnum + ":" + subclassName + ".cs";
		}
		void initialize(string reasonToExist, Backtester backtester) {
			this.REASON_TO_EXIST = reasonToExist;
			this.backtester = backtester;
		}
		//public BacktestQuotesGenerator CloneAndInitialize(Backtester backtester) {
		//	BacktestQuotesGenerator initializedFromMni = (BacktestQuotesGenerator)Activator.CreateInstance(this.GetType());
		//	initializedFromMni.initialize(backtester);
		//	return initializedFromMni;
		//}

		protected QuoteGenerated GenerateNewQuote_childrenHelper(int intraBarSerno, string symbol, DateTime serverTime,
				BidOrAsk bidOrAsk, double priceAligned, double volume, Bar barSimulated) {

			QuoteGenerated ret = new QuoteGenerated(serverTime, symbol, ++this.AbsnoPerSymbol,
													double.NaN, double.NaN, volume);
			//ret.ServerTime = serverTime;
			//FILLED_LATER_DONT_CONFUSE_STREAMING_ADAPDER ret.AbsnoPerSymbol = ++this.LastGeneratedAbsnoPerSymbol;
			//FILLED_LATER_DONT_CONFUSE_STREAMING_ADAPDER ret.IntraBarSerno = intraBarSerno;
			ret.Source = this.WhoGeneratedQuote;
			//ret.Symbol = symbol;
			ret.SymbolClass = this.backtester.BarsOriginal.SymbolInfo.SymbolClass;
			//ret.Size = volume;
			ret.ParentBarSimulated = barSimulated;
			//v1 ret.PriceLastDeal = price;

			BacktestSpreadModeler modeler = this.backtester.BacktestDataSource.StreamingAsBacktest_nullUnsafe.SpreadModeler;
			switch (bidOrAsk) {
				case BidOrAsk.Bid:
					ret.Bid = priceAligned;
					ret.TradedAt = BidOrAsk.Bid;
					modeler.FillAskBasedOnBid_aligned(ret);
					if (ret.Spread == 0) {
						string msig = " returned by modeler[" + modeler + "] for quote[" + ret + "]";
						string msg = "SPREAD_MUST_NOT_BE_ZERO_AFTER GenerateFillAskBasedOnBid()";
						Assembler.PopupException(msg + msig);
						#if DEBUG
						Debugger.Break();
						#endif
					}
					break;
				case BidOrAsk.Ask:
					ret.Ask = priceAligned;
					ret.TradedAt = BidOrAsk.Ask;
					modeler.FillBidBasedOnAsk_aligned(ret);
					if (ret.Spread == 0) {
						string msig = " returned by modeler[" + modeler + "] for quote[" + ret + "]";
						string msg = "SPREAD_MUST_NOT_BE_ZERO_AFTER GenerateFillAskBasedOnBid()";
						Assembler.PopupException(msg + msig);
						#if DEBUG
						Debugger.Break();
						#endif
					}
					break;
				case BidOrAsk.UNKNOWN:
					modeler.GeneratedQuoteFillBidAsk(ret, barSimulated, priceAligned);
					// I_DONT_KNOW_WHAT_TO_PUT_HERE
					ret.TradedAt = BidOrAsk.Bid;
					if (ret.Spread == 0) {
						string msig = " returned by modeler[" + modeler + "] for quote[" + ret + "]";
						string msg = "SPREAD_MUST_NOT_BE_ZERO_AFTER GeneratedQuoteFillBidAsk()";
						Assembler.PopupException(msg + msig);
						#if DEBUG
						Debugger.Break();
						#endif
					}
					break;
			}

			//if (barSimulated.HighLowDistance > 0 && barSimulated.HighLowDistance > ret.Spread) {
				if (barSimulated.ContainsBidAskForQuoteGenerated(ret, true) == false) {
					Assembler.PopupException("TEST_EMBEDDED GENERATED_QUOTE_OUT_OF_BOUNDARY_CHECK #0/2", null, false);
				}
			//}

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

		public virtual List<QuoteGenerated> InjectQuotes_toFillPendingAlerts_push(
				QuoteGenerated quoteToReach, Bar bar2simulate, int iterationsLimit = 1) {
			List<QuoteGenerated> injectedPushed = new List<QuoteGenerated>();
			int pendingsToFillInitially = this.backtester.Executor.ExecutionDataSnapshot.AlertsPending.Count;
			if (pendingsToFillInitially == 0) return injectedPushed;

			// hard to debug but I hate while(){} loops
			//for (QuoteGenerated closestOnOurWay  = this.generateClosestQuoteForEachPendingAlertOnOurWayTo(quoteToReach);
			//		   closestOnOurWay != null;
			//		   closestOnOurWay  = this.generateClosestQuoteForEachPendingAlertOnOurWayTo(quoteToReach)) {

			if (bar2simulate.HighLowDistance == 0) {
				string msg = "WEIRD_BAR_SHOULDNT_FAIL_FOLLOWING_QUOTE_CHECK ContainsBidAskForQuoteGenerated(quoteToReach)";
				return injectedPushed;
			}

			if (bar2simulate.ContainsBidAskForQuoteGenerated(quoteToReach) == false) {
				string msg = "KEEP_ME_HERE_DONT_MAKE_LOOP_UPSTACK_COMPLICATED";
				return injectedPushed;
			}

			QuoteGenerated closestOnOurWay  = this.generateClosestQuote_forEachPendingAlert_onOurWayTo(quoteToReach, bar2simulate);
			while (closestOnOurWay != null) {
				//v1 if (bar2simulate.ContainsBidAskForQuoteGenerated(closestOnOurWay) == false) {
				if (bar2simulate.HighLowDistance > 0 && bar2simulate.HighLowDistance > closestOnOurWay.Spread
						&& bar2simulate.ContainsBidAskForQuoteGenerated(closestOnOurWay) == false) {
					string msg = "GENERATED_QUOTE_OUT_OF_BOUNDARY_CHECK #2/2";
					Assembler.PopupException(msg);
					continue;
				}

				#if DEBUG // INLINE TEST
				if (closestOnOurWay.AbsnoPerSymbol == -1) {
					string msg = "QUOTE_ABSNO_MUST_BE_SEQUENTIAL_PER_SYMBOL INITIALIZED_IN_STREAMING_ADAPDER";
					Assembler.PopupException(msg);
				}
				#endif

				closestOnOurWay.AbsnoPerSymbol = ++this.LastGeneratedAbsnoPerSymbol;		// DONT_FORGET_TO_ASSIGN_LATEST_ABSNO_TO_QUOTE_TO_REACH
				closestOnOurWay.IntraBarSerno += injectedPushed.Count;						// first quote has IntraBarSerno=-1, rimemba?

				//v1 "Received" is for Quik/InteractiveBrokers; "Generated" is for Backtest/Livesim  this.backtester.BacktestDataSource.StreamingAsBacktest_nullUnsafe.PushQuoteReceived(closestOnOurWay);
				//v2 1) LivesimBroker will push it delayed; 2) register closestOnOurWay as Streaming.LastQuoteReceived[Symbol]
				this.backtester.BacktestDataSource.StreamingAsBacktest_nullUnsafe.PushQuoteGenerated(closestOnOurWay);

				injectedPushed.Add(closestOnOurWay);

				int pendingAfterInjected = this.backtester.Executor.ExecutionDataSnapshot.AlertsPending.Count;
				if (pendingsToFillInitially != pendingAfterInjected) {
					string msg = " it looks like the quoteInjected triggered something";
					//Assembler.PopupException(msg, null, false);
					if (this.backtester.ImLivesimulator && this.backtester.Executor.Strategy.LivesimBrokerSettings.DelayBeforeFillEnabled) {
						msg = "SEPARATE_MARKET_MODEL_WOULD_HELP_LAZY NO_ORDER_MUST_HAVE_BEEN_FILLED_WHILE_INJECTING__KOZ_LIVESIM_BROKER_EXECUTION_IS_DELAYED" + msg;
						// NOTHING_WRONG_WITH_ALERT_FILLED_DURING_LIVESIM Assembler.PopupException(msg, null, false);
					}
				}
				if (injectedPushed.Count >= iterationsLimit) {
					string msg = "InjectQuotesToFillPendingAlerts(): quotesInjected["
						+ injectedPushed + "] > iterationsLimit[" + iterationsLimit + "]"
						+ " pendingAfterInjected[" + pendingAfterInjected + "]"
						+ " quoteToReach[" + quoteToReach + "]";
					//Assembler.PopupException(msg);
					break;
				}
				bool abortRequested = this.backtester.RequestingBacktestAbortMre.WaitOne(0);
				if (abortRequested) break;
				bool backtestAborted = this.backtester.BacktestAbortedMre.WaitOne(0);
				if (backtestAborted) break;
				closestOnOurWay = this.generateClosestQuote_forEachPendingAlert_onOurWayTo(quoteToReach, bar2simulate);
			}
			return injectedPushed;
		}
		QuoteGenerated  generateClosestQuote_forEachPendingAlert_onOurWayTo(QuoteGenerated quoteToReach, Bar bar2simulate) {
			string msig = " //generateClosestQuote_forEachPendingAlert_onOurWayTo(" + quoteToReach + "," + bar2simulate + ")";

			if (this.backtester.Executor.ExecutionDataSnapshot.AlertsPending.Count == 0) {
				string msg = "it looks like no Pending alerts are left anymore";
				return null;
			}

			Quote quotePrev_QuoteGenerated_orQuoteQuikIrretraceableAfterDde =
				this.backtester.BacktestDataSource.StreamingAsBacktest_nullUnsafe.StreamingDataSnapshot
					.GetQuoteCurrent_forSymbol_nullUnsafe(quoteToReach.Symbol);

			if (quotePrev_QuoteGenerated_orQuoteQuikIrretraceableAfterDde == null) {
				string msg = "I_CANNOT_CONTINUE_LIVESIM_FIXME#1";
				Assembler.PopupException(msg + msig, null, false);
				return null;
			}

			QuoteGenerated quotePrev = quotePrev_QuoteGenerated_orQuoteQuikIrretraceableAfterDde as QuoteGenerated;
			if (quotePrev == null) {
				string msg = "YES_WE_LOST_PARENT_BAR_BECAUSE_QUOTE_WENT_THROUGH_QuikLivesimStreaming"
					+ " Source[" + quotePrev_QuoteGenerated_orQuoteQuikIrretraceableAfterDde.Source + "]";
				quotePrev = new QuoteGenerated(quotePrev_QuoteGenerated_orQuoteQuikIrretraceableAfterDde, bar2simulate);
				if (quotePrev == null) {
					string msg1 = "I_CANNOT_CONTINUE_LIVESIM_FIXME#1";
					Assembler.PopupException(msg1 + msig);
					return null;
				}
			}

			#region PARANOID_CHECKS_HERE THANK_YOU_LED_TO_10_LINES_ABOVE
			QuoteGenerated quotePrevAsDde = quotePrev_QuoteGenerated_orQuoteQuikIrretraceableAfterDde as QuoteGenerated;
			if (quotePrev == null) {
				string msg = "PARANOINDAL_CHECK_IF_PREV_QUOTE_IS_QUOTE_TO_REACH copypaste";
				Assembler.PopupException(msg);
			}

			if (bar2simulate.HighLowDistance > 0) {
				// IRRELEVANT TO COMPARE PREV_QUOTE BID AGAINST THIS_BAR_ASK
				//if (bar2simulate.HighLowDistance > quotePrev.Spread && bar2simulate.ContainsBidAskForQuoteGenerated(quotePrev) == false) {
				//	Debugger.Break();
				//}
				if (bar2simulate.HighLowDistance > quoteToReach.Spread
					&& bar2simulate.ContainsBidAskForQuoteGenerated(quoteToReach) == false) {
				string msg = "SPREAD_IS_SO_HUGE_THAT_QUOTE_WENT_OUT_OF_BAR_HILO";
				Assembler.PopupException(msg);
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
			#endregion

			bool scanningDown = quoteToReach.Bid < quotePrev.Bid;
			QuoteGenerated quoteClosest = null;
			List<Alert> alertsSafe = this.backtester.Executor.ExecutionDataSnapshot.AlertsPending.SafeCopy(this, "generateClosestQuote_forEachPendingAlert_onOurWayTo(WAIT)");
			foreach (Alert alert in alertsSafe) {
				// DONT_EXPECT_THEM_TO_BE_FILLED_YOU_SHOULD_FILL_ALL_RELEVANT_NOW
				//if (scanningDown) {
				//	// while GEN'ing down, all BUY/COVER STOPs pending were already triggered & executed
				//	bool executedAtAsk = alert.Direction == Direction.Buy || alert.Direction == Direction.Cover;
				//	if (executedAtAsk && alert.MarketLimitStop == MarketLimitStop.Stop) continue;
				//} else {
				//	// while GEN'ing up, all SHORT/SELL STOPs pending were already triggered & executed
				//	bool executedAtBid = alert.Direction == Direction.Short || alert.Direction == Direction.Sell;
				//	if (executedAtBid && alert.MarketLimitStop == MarketLimitStop.Stop) continue;
				//}
				string errModelingQuoteThatCouldFill;
				QuoteGenerated quoteThatWillFillAlert = this.modelQuote_thatCouldFillAlert(alert,
					new DateTime(quoteToReach.ServerTime.Ticks - 911), bar2simulate, out errModelingQuoteThatCouldFill);
				if (quoteThatWillFillAlert == null) {
					Assembler.PopupException(errModelingQuoteThatCouldFill);
					continue;
				}

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
					continue;
				}
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

			if (quoteClosest == null) return quoteClosest;

			if (scanningDown) {
				if (quoteClosest.Bid > quotePrev.Bid) {
					string msg = "WHILE_SCANNING_DOWN_RETURNING_QUOTE_HIGHER_THAN_PREVIOUS_IS_WRONG";
					Assembler.PopupException(msg);
				}
				if (quoteClosest.Bid < quoteToReach.Bid) {
					string msg = "WHILE_SCANNING_DOWN_RETURNING_QUOTE_LOWER_THAN_TARGET_IS_WRONG";
					Assembler.PopupException(msg);
				}
			} else {
				if (quoteClosest.Ask < quotePrev.Ask) {
					string msg = "WHILE_SCANNING_UP_RETURNING_QUOTE_LOWER_THAN_PREVIOUS_IS_WRONG";
					Assembler.PopupException(msg);
				}
				if (quoteClosest.Ask > quoteToReach.Ask) {
					string msg = "WHILE_SCANNING_UP_RETURNING_QUOTE_HIGHER_THAN_TARGET_IS_WRONG";
					Assembler.PopupException(msg);
				}
			}

			//I_WILL_SPOIL_STREAMING_BAR_IF_I_ATTACH_LIKE_THIS QuoteGenerated quoteNextAttached = this.backtester.BacktestDataSource.BacktestStreamingAdapter.(quoteToReach.Clone());
			QuoteGenerated ret = quotePrev.DeriveIdenticalButFresh();
			ret.Bid = quoteClosest.Bid;
			ret.Ask = quoteClosest.Ask;
			//FILLED_AT_ALERT_FILL ret.PriceLastDeal = (ret.Ask + ret.Bid) / 2;
			ret.Size = quoteClosest.Size;
			ret.Source = quoteClosest.Source;

			//LIVESIM_HACK
			//if (ret.ParentBarStreaming.ParentBars != null) {
			//	ret.SetParentBarStreaming(null);
			//}
			return ret;
		}
		QuoteGenerated modelQuote_thatCouldFillAlert(Alert alert, DateTime quoteServerTime, Bar bar2simulate
				, out string errOut, bool checkAgainstPrevReceivedQuote = false) {
			string msig = " //modelQuote_thatCouldFillAlert(alert[" + alert+ "] bar2simulate[" +bar2simulate+ "])";
			errOut = "NO_ERROR__QUOTE_MODELLED_MUST_BE_NON_NULL";

			//QuoteGenerated ret = new QuoteGenerated(localDateTimeBasedOnServerForBacktest);

			QuoteGenerated ret = new QuoteGenerated(quoteServerTime,
													alert.Symbol, -1,
													double.NaN, double.NaN, alert.Qty);

			//ret.Source = "GENERATED_TO_FILL_" + alert.ToString();			// PROFILER_SAID_TOO_SLOW + alert.ToString();
			ret.Source = "GENERATED_TO_FILL_alert@bar#" + alert.PlacedBarIndex;
			ret.Size = alert.Qty;
			ret.ParentBarSimulated = bar2simulate;

			//v2
			double priceScriptAligned = alert.PriceEmitted;

			//#if DEBUG
			////v1
			//double priceScriptAligned1 = this.backtester.Executor.AlertPrice_alignToPriceLevel(alert.Bars, alert.PriceScript, true,
			//	alert.PositionLongShortFromDirection, alert.MarketLimitStop);
			////quick check
			//if (priceScriptAligned1 != alert.PriceScript) {
			//	if (alert.MarketLimitStop == MarketLimitStop.Market) {
			//		string msg = "WHY_YOU_DID_CHANGE_THE_PRICE__PRICE_SCRIPT_MUST_BE_ALREADY_GOOD";
			//		Assembler.PopupException(msg);
			//	}
			//}
			////long check, switch marketSim calculations to alert.PriceScriptAligned 
			////v1 if (priceScriptAligned != alert.PriceScriptAligned) {
			////v2 
			//bool alignedDifferently_iHateDoublesComparison = Math.Round(priceScriptAligned, 1) != Math.Round(alert.PriceScriptAligned, 1);
			//if (alignedDifferently_iHateDoublesComparison) {
			//	string msg = "FIX_Alert.PriceScriptAligned";
			//	Assembler.PopupException(msg);
			//} else {
			//	string msg = "GET_RID_OF_COMPLEX_ALIGNMENT executor.AlignAlertPriceToPriceLevel()";
			//	//Assembler.PopupException(msg, null, false);
			//}

			//if (alert.MarketLimitStop.ToString() != alert.MarketLimitStopAsString) {
			//	string msg = "IT_WAS_TIDAL_OR_OTHER_EXOTIC_MARKET_ORDER";
			//	//Assembler.PopupException(msg);
			//}
			//#endif

			//v1
			//Quote quotePrevDowncasted = this.backtester.BacktestDataSource.StreamingAsBacktest_nullUnsafe.StreamingDataSnapshot
			//	.LastQuoteCloneGetForSymbol(alert.Symbol);
			//QuoteGenerated quotePrev = quotePrevDowncasted as QuoteGenerated;
			//if (quotePrev == null) {
			//	string msg = "PREV_QUOTE_NULL";
			//	Assembler.PopupException(msg + msig);
			//}

			//v2
			Quote quotePrev_QuoteGenerated_orQuoteQuik_irretraceableAfterDde =
				this.backtester.BacktestDataSource.StreamingAsBacktest_nullUnsafe.StreamingDataSnapshot
					.GetQuoteCurrent_forSymbol_nullUnsafe(alert.Symbol);
			QuoteGenerated quotePrev = quotePrev_QuoteGenerated_orQuoteQuik_irretraceableAfterDde as QuoteGenerated;
			if (quotePrev == null) {
				string msg = "YES_WE_LOST_PARENT_BAR_BECAUSE_QUOTE_WENT_THROUGH_QuikLivesimStreaming"
					+ " Source[" + quotePrev_QuoteGenerated_orQuoteQuik_irretraceableAfterDde.Source + "]";
				quotePrev = new QuoteGenerated(quotePrev_QuoteGenerated_orQuoteQuik_irretraceableAfterDde, bar2simulate);
			}

			BacktestSpreadModeler modeler = this.backtester.BacktestDataSource.StreamingAsBacktest_nullUnsafe.SpreadModeler;
			switch (alert.MarketLimitStop) {
				case MarketLimitStop.Limit:
					switch (alert.Direction) {
						case Direction.Buy:
						case Direction.Cover:
							if (priceScriptAligned > quotePrev.Ask && checkAgainstPrevReceivedQuote) {
								errOut = "INVALID_PRICESCRIPT_FOR_LIMIT_BUY_MUST_NOT_BE_ABOVE_CURRENT_ASK";
								return null;
							}
							ret.Ask = priceScriptAligned;
							ret.TradedAt = BidOrAsk.Ask;
							modeler.FillBidBasedOnAsk_aligned(ret);
							break;
						case Direction.Short:
						case Direction.Sell:
							if (priceScriptAligned < quotePrev.Bid && checkAgainstPrevReceivedQuote) {
								errOut = "INVALID_PRICESCRIPT_FOR_LIMIT_SELL_MUST_NOT_BE_BELOW_CURRENT_BID";
								return null;
							}
							ret.Bid = priceScriptAligned;
							ret.TradedAt = BidOrAsk.Bid;
							modeler.FillAskBasedOnBid_aligned(ret);
							break;
						default:
							string msg = "ALERT_LIMIT_DIRECTION_UNKNOWN direction[" + alert.Direction
								+ "] is not Buy/Cover/Short/Sell modelQuoteThatCouldFillAlert()";
							throw new Exception(msg + msig);
					}
					break;
				case MarketLimitStop.Stop:
					switch (alert.Direction) {
						case Direction.Buy:
						case Direction.Cover:
							if (priceScriptAligned < quotePrev.Ask && checkAgainstPrevReceivedQuote) {
								errOut = "INVALID_PRICESCRIPT_FOR_STOP_BUY_MUST_NOT_BE_BELOW_CURRENT_ASK";
								return null;
							}
							ret.Ask = priceScriptAligned;
							ret.TradedAt = BidOrAsk.Ask;
							modeler.FillBidBasedOnAsk_aligned(ret);
							break;
						case Direction.Short:
						case Direction.Sell:
							if (priceScriptAligned > quotePrev.Bid && checkAgainstPrevReceivedQuote) {
								errOut = "INVALID_PRICESCRIPT_FOR_STOP_SELL_MUST_NOT_BE_ABOVE_CURRENT_BID";
								return null;
							}
							ret.Bid = priceScriptAligned;
							ret.TradedAt = BidOrAsk.Bid;
							modeler.FillAskBasedOnBid_aligned(ret);
							break;
						default:
							string msg = "ALERT_STOP_DIRECTION_UNKNOWN direction[" + alert.Direction
								+ "] is not Buy/Cover/Short/Sell modelQuote_thatCouldFillAlert()";
							throw new Exception(msg + msig);
					}
					break;
				case MarketLimitStop.Market:
					switch (alert.Direction) {
						case Direction.Buy:
						case Direction.Cover:
							ret.Ask = quotePrev.Ask;
							ret.TradedAt = BidOrAsk.Ask;
							modeler.FillBidBasedOnAsk_aligned(ret);
							break;
						case Direction.Short:
						case Direction.Sell:
							ret.Bid = quotePrev.Bid;
							ret.TradedAt = BidOrAsk.Bid;
							modeler.FillAskBasedOnBid_aligned(ret);
							break;
						default:
							string msg = "ALERT_MARKET_DIRECTION_UNKNOWN direction[" + alert.Direction
								+ "] is not Buy/Cover/Short/Sell modelQuote_thatCouldFillAlert()";
							throw new Exception(msg + msig);
					}
					break;
				case MarketLimitStop.StopLimit: // HACK one QuoteGenerated might satisfy SLactivation, the other one might fill it; time to introduce state of SL into Alert???
					string msg2 = "STOP_LIMIT_QUOTE_FILLING_GENERATION_REQUIRES_TWO_STEPS_NYI"
						+ "; pass SLActivation=0 to PositionPrototypeActivator so that it generates STOP instead of STOPLOSS which I can't generate yet";
					Assembler.PopupException(msg2 + msig, null, false);
					break;
				default:
					throw new Exception("ALERT_TYPE_UNKNOWN MarketLimitStop[" + alert.MarketLimitStop + "] is not Market/Limit/Stop modelQuote_thatCouldFillAlert()");
			}

			return ret;
		}
		public virtual List<QuoteGenerated> Generate_quotesFromBar_avoidClearing(Bar barSimulated) {
			this.Quotes_generatedForOneBar_amountDependsOnEngineType.Clear();

			double volumeOneQuarterOfBar = barSimulated.Volume / this.BacktestStrokesPerBarAsInt;
			if (barSimulated.ParentBars != null && barSimulated.ParentBars.SymbolInfo != null) {
				volumeOneQuarterOfBar = Math.Round(volumeOneQuarterOfBar, barSimulated.ParentBars.SymbolInfo.VolumeDecimals);
				if (volumeOneQuarterOfBar == 0) {
					#if DEBUG	// TEST_EMBEDDED
					//TESTED Debugger.Break();
					//double minimalValue = Math.Pow(1, -decimalsVolume);		// 1^(-2) = 0.01
					#endif
					volumeOneQuarterOfBar = barSimulated.ParentBars.SymbolInfo.VolumeStepFromDecimal;
				}
			}
			if (volumeOneQuarterOfBar == 0) {
				volumeOneQuarterOfBar = 1;
			}

			DateTime barOpenNext = barSimulated.DateTimeNextBarOpenUnconditional;
			
			DateTime barOpenOrResume = barSimulated.DateTimeOpen;
			MarketInfo marketInfo = barSimulated.ParentBars.MarketInfo;
			MarketClearingTimespan clearing = marketInfo.GetSingleClearingTimespan_ifMarketSuspended_duringBar(barOpenOrResume, barOpenNext);
			if (clearing != null) {
				DateTime resumes = clearing.ResumeServerTimeOfDay;
				barOpenOrResume = new DateTime(barOpenOrResume.Year, barOpenOrResume.Month, barOpenOrResume.Day,
					resumes.Hour, resumes.Minute, resumes.Second);
			}

			TimeSpan leftTillNextBar = barOpenNext - barOpenOrResume;
			int incrementSeconds = ((int)(leftTillNextBar.TotalSeconds / this.BacktestStrokesPerBarAsInt));	// WRONG -1 one second less to not have exactly next bar opening at last stroke but 4 seconds before
			TimeSpan timespanIncrementInterquote = new TimeSpan(0, 0, incrementSeconds);
			TimeSpan timespanBarBeginningOffsetCumulative = new TimeSpan(0);
			
			SymbolInfo symbolInfo = barSimulated.ParentBars.SymbolInfo;

			for (int stroke = 0; stroke < this.BacktestStrokesPerBarAsInt; stroke++) {
				double price_withGranularStrokes_requiresAlignement = 0;
				BidOrAsk bidOrAsk = BidOrAsk.UNKNOWN;
				this.Assign_priceAndBidOrAsk_dependingOnQuotesPerBar_forStroke(barSimulated, stroke, out price_withGranularStrokes_requiresAlignement, out bidOrAsk);
				double priceAligned = symbolInfo.AlignToPriceLevel(price_withGranularStrokes_requiresAlignement);

				if (priceAligned != price_withGranularStrokes_requiresAlignement) {
					string msg = "OUT_OF_BAR_CHECK_IS_WAITING_FOR_US_IN_this.GenerateNewQuote_childrenHelper()";
				}

				DateTime timeServerForStroke = barOpenOrResume + timespanBarBeginningOffsetCumulative;
				// for clearing[14:00...14:03].GetClearingResumes(14:00)=14:03, will be shifted now
				DateTime timeClearingResumes = marketInfo.GetClearingResumes(timeServerForStroke);
				if (timeClearingResumes != DateTime.MinValue && timeClearingResumes >= barOpenNext) {
					string msg = "CLEARING_EXTENDS_BEOYND_BAR_SIMULATED SKIPPING_QUOTE_GENERATION_TILL_END_OF_BAR EXPECTING_BAR_INCREASE_UPSTACK";
					Assembler.PopupException(msg, null, false);
					break;
				}
				
				bool recalcShrunkenIncrementDueToIntrabarClearing = false;
				if (timeServerForStroke < timeClearingResumes) {
					// 4quotes 1bar 5min @14:00: clearing[14:00...14:03] should produce 14:03.00 14:03.30 14:04.00 14:04.30
					timeServerForStroke = timeClearingResumes;
					recalcShrunkenIncrementDueToIntrabarClearing = true;
				}
				
				QuoteGenerated quote = this.GenerateNewQuote_childrenHelper(stroke, barSimulated.Symbol,
					timeServerForStroke, BidOrAsk.UNKNOWN, priceAligned, volumeOneQuarterOfBar, barSimulated);
				this.Quotes_generatedForOneBar_amountDependsOnEngineType.Add(quote);

				if (stroke == this.BacktestStrokesPerBarAsInt - 1) break;		// avoiding expensive {cumulativeOffset += increment} at last stroke 

				if (recalcShrunkenIncrementDueToIntrabarClearing == false) {
					timespanBarBeginningOffsetCumulative += timespanIncrementInterquote;
					continue;
				}

				TimeSpan timespanIncrementInterquoteOld = timespanIncrementInterquote;
				leftTillNextBar = barOpenNext - timeClearingResumes;
				int quotesLeft = this.BacktestStrokesPerBarAsInt - stroke;
				timespanIncrementInterquote = new TimeSpan(0, 0, ((int)(leftTillNextBar.TotalSeconds / quotesLeft)));
				
				TimeSpan clearingResumesOffset = timeClearingResumes - barOpenOrResume;
				timespanBarBeginningOffsetCumulative = clearingResumesOffset + timespanIncrementInterquote;

				#if DEBUG
				MarketClearingTimespan clearing1 = marketInfo.GetSingleClearingTimespan_ifMarketSuspended_duringBar(barSimulated.DateTimeOpen, barSimulated.DateTimeOpen);
				string whereAmI = "INTRABAR_CLEARING @" + barSimulated.DateTimeOpen.TimeOfDay
					+ " clearing" + clearing1
					+ " barOpenOrResume[" + barOpenOrResume.TimeOfDay + "]=>[" + timeClearingResumes.TimeOfDay + "]"
					+ " timespanIncrementInterquote[" + timespanIncrementInterquoteOld + "]=>[" + timespanIncrementInterquote + "]";
				//Assembler.PopupException(whereAmI, null, false);
				#endif

				// WRONG -1 one second less to not have exactly next bar opening at last stroke but 4 seconds before
				// BAR_WITHOUT_INTRABAR_CLEARING	4quotes 1bar 5min 11:00 => 11:05
			  	// stroke0 increment (5:00 / 4 = 1:15): 11:00:00
			  	// stroke1 increment (5:00 / 4 = 1:15): 11:01:15
			  	// stroke2 increment (5:00 / 4 = 1:15): 11:02:30
			  	// stroke3 increment (5:00 / 4 = 1:15): 11:03:45 (next stroke4 would be 11:05:00 which next bar/invocation)
				// BAR_WITH_INTRABAR_CLEARING		[14:00...14:03]
			  	// stroke0 increment (5:00 / 4 = 1:15): 14:00:00 => 14:03:00
				// stroke1 increment (2:00 / 4 = 0:30): 14:03:30
				// stroke2 increment (2:00 / 4 = 0:30): 14:04:00
				// stroke3 increment (2:00 / 4 = 0:30): 14:04:30 (next stroke4 would be 14:05:00 which next bar/invocation)
			}
			return this.Quotes_generatedForOneBar_amountDependsOnEngineType;
		}

		protected abstract void Assign_priceAndBidOrAsk_dependingOnQuotesPerBar_forStroke(
			Bar barSimulated, int stroke, out double priceFromBar_granularStrokes_willBeAligned_upstack, out BidOrAsk bidOrAsk);

		public static BacktestQuotesGenerator CreateForQuotesPerBar_initialize(BacktestStrokesPerBar backtestStrokesPerBar, Backtester backtester) {
			BacktestQuotesGenerator ret = new BacktestQuotesGeneratorFourStroke();
			switch (backtestStrokesPerBar) {
				case BacktestStrokesPerBar.FourStrokeOHLC:
					//DEFAULT ret = new BacktestQuotesGeneratorFourStroke(reasonToExist);
					break;
				case BacktestStrokesPerBar.TenStroke:
					ret = new BacktestQuotesGeneratorTenStroke();
					break;
				case BacktestStrokesPerBar.SixteenStroke:
					ret = new BacktestQuotesGeneratorSixteenStroke();
					break;
				default:
					string msg = "NYI: [" + backtestStrokesPerBar + "] //CreateForQuotesPerBar_initialize() " + ret.ToString();
					Assembler.PopupException(msg, null, false);
					break;
			}
			ret.initialize("STRATEGY_INDUCED_OR_USER_CLICKED", backtester);
			return ret;
		}
	}
}
