using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.Execution;
using Sq1.Core.DataTypes;

namespace Sq1.Core.StrategyBase {
	public class PositionPrototypeActivator {
		ScriptExecutor executor;
		public PositionPrototypeActivator(ScriptExecutor strategyExecutor) {
			this.executor = strategyExecutor;
		}
		public void PlaceOnce(PositionPrototype proto) {
			if (proto.PriceEntry == 0) {
				//v1 string msg = "market orders can't be found in OrdersPending";
				string msg = "EntryMarketLimitStopFromDirection(XX, priceExecutionDesired=0, XX) will return MarketLimitStop.Market";
			}
			if (this.checkPrototypeAlreadyPlaced(proto)) return;
			//this.checkThrowPlacingProtoMakesSense(proto);
			proto.checkTPOffsetThrowBeforeAbsorbing(proto.TakeProfitPositiveOffset);
			proto.checkSLOffsetsThrowBeforeAbsorbing(proto.StopLossNegativeOffset, proto.StopLossActivationNegativeOffset);
			//bool a = this.executor.Backtester.IsBacktestingNow;

			Position posWithAlert = executor.BuyOrShortAlertCreateRegister (
				executor.Bars.BarStreamingNullUnsafe, proto.PriceEntry,
				proto.SignalEntry + "protoEntry@" + proto.PriceEntry,
				MarketConverter.EntryDirectionFromLongShort(proto.LongShort),
				MarketConverter.EntryMarketLimitStopFromDirection(
					executor.Bars.BarStreamingNullUnsafeCloneReadonly.Close, proto.PriceEntry, proto.LongShort)
				);
			if (posWithAlert == null) {
				string msg = "man I don't understand this null; out-of-bar limit should still leave a pending Alert.PositionAffected";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}
			if (posWithAlert.Prototype != null) {
				string msg = "CLEANUP: I was trying to catch MoveStopLoss::if(proto.StopLossAlertForAnnihilation==null)"
					+ " so I thought there is a new prototype assigned to a position,"
					+ " since we never put null directly proto.StopLossAlertForAnnihilation";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}
			posWithAlert.Prototype = proto;
		}

		bool checkPrototypeAlreadyPlaced(PositionPrototype proto) {
			List<Alert> pendingSafe = this.executor.ExecutionDataSnapshot.AlertsPending.SafeCopy(this, " //checkPrototypeAlreadyPlaced(WAIT)");
			foreach (Alert alert in pendingSafe) {
				Position pos = alert.PositionAffected;
				if (pos == null) continue;
				if (pos.Prototype == null) continue;
				if (pos.Prototype.IsIdenticalTo(proto)) return true;
			}
			return false;
		}
		bool checkPendingEntryPositionAlreadyPlaced(Position entryPosition) {
			if (entryPosition.EntryAlert == null) return false;
			List<Alert> pendingSafe = this.executor.ExecutionDataSnapshot.AlertsPending.SafeCopy(this, " //checkPendingEntryPositionAlreadyPlaced(WAIT)");
			foreach (Alert alert in pendingSafe) {
				Position pos = alert.PositionAffected;
				if (pos == null) continue;
				if (pos.EntryAlert == null) continue;
				if (pos.EntryAlert == entryPosition.EntryAlert) return true;
				if (pos.EntryAlert.IsIdenticalForOrdersPending(entryPosition.EntryAlert)) return true;
			}
			return false;
		}

		public List<Alert> CreateStopLossAndTakeProfitAlertsFromPositionPrototype(Position position) {
			if (position.IsEntryFilled == false) {
				string msg = "I can not place SL and TP for an unopened position; alert[" + position.EntryAlert + "]";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}
			List<Alert> ret = new List<Alert>(); 
			Alert SlPlaced = this.CreateStopLossFromPositionPrototype(position);
			Alert TpPlaced = this.CreateTakeProfitFromPositionPrototype(position);
			if (SlPlaced != null) ret.Add(SlPlaced);
			if (TpPlaced != null) ret.Add(TpPlaced);
			return ret; 
		}
		public Alert CreateStopLossFromPositionPrototype(Position position) {
			//if (this.executor.checkPrototypeAlreadyPlaced(position)) return;
			PositionPrototype proto = position.Prototype;
			if (proto.StopLossNegativeOffset > 0) {
				string msg = "PROTOTYPE_STOPLOSS_NOT_CREATED STOPLOSS_OFFSET_MUST_BE_NEGATIVE(FOR_STOP_LOSS))_OR_ZERO(FOR_STOP)";
				// DONT_SCREAM_SO_MUCH	throw new Exception(msg);
				// ALLOW_POSITIVE_OFFSET_FROM_PRICE_ENTRY_TO_TIGHTEN_SL_ACROSS_PRICE_ENTRY_LEVEL return null;
			}

			MarketLimitStop simpleStopIfActivationZero = (proto.StopLossActivationNegativeOffset == 0) ? MarketLimitStop.Stop : MarketLimitStop.StopLimit;

			if (proto.StopLossNegativeOffset == 0) {
				string msg = this.ReasonWhyNewStopLossOffsetDoesntMakeSense(position, proto.StopLossNegativeOffset, simpleStopIfActivationZero);
				if (String.IsNullOrEmpty(msg) == false) {
					string msg2 = "What should Activator do with sense-less proto.StopLossNegativeOffset[" + proto.StopLossNegativeOffset + "], ";
					#if DEBUG
					Debugger.Launch();
					#endif
					throw new Exception(msg2, new Exception(msg));
				}
			}

			if (proto.PriceEntry == 0) {
				string msg = "POSITION_PROTOTYPE_SUPPORTS_MARKET_ENTRY";
				proto.PriceEntryAbsorb(position.EntryFilledPrice);
			}
			Alert alertStopLoss = executor.SellOrCoverAlertCreateDontRegisterInNew (
				executor.Bars.BarStreamingNullUnsafe,
				position, proto.PriceStopLoss,
				proto.SignalStopLoss + "protoStopLossExit:" + proto.StopLossActivationNegativeOffset
					+ "@" + proto.StopLossNegativeOffset + " for " + position.EntrySignal,
				MarketConverter.ExitDirectionFromLongShort(proto.LongShort),
				simpleStopIfActivationZero);
			if (alertStopLoss == null) {
				string msg = "alertStopLoss should NOT be null";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}
			alertStopLoss.PriceStopLimitActivation = 0;
			if (proto.StopLossActivationNegativeOffset < 0) alertStopLoss.PriceStopLimitActivation = proto.PriceStopLossActivation;
			if (proto.StopLossAlertForAnnihilation != null && this.executor.Backtester.IsBacktestingNoLivesimNow == false) {
				string msg = "CLEANUP: I was trying to catch MoveStopLoss::if(proto.StopLossAlertForAnnihilation==null)"
					+ " so I thought there is a new prototype assigned to a position,"
					+ " since we never put null directly proto.StopLossAlertForAnnihilation";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}

			proto.StopLossAlertForAnnihilation = alertStopLoss;
			return alertStopLoss;
		}
		public Alert CreateTakeProfitFromPositionPrototype(Position position) {
			PositionPrototype proto = position.Prototype;
			if (proto.TakeProfitPositiveOffset <= 0) {
				string msg = "PROTOTYPE_TAKEPROFIT_NOT_CREATED PROTOTYPE_TAKEPROFIT_OFFSET_MUST_BE_POSITIVE_NON_ZERO";
				// DONT_SCREAM_SO_MUCH	throw new Exception(msg);
				return null;
			}

			Alert alertTakeProfit = executor.SellOrCoverAlertCreateDontRegisterInNew (
				executor.Bars.BarStreamingNullUnsafe,
				position, proto.PriceTakeProfit,
				proto.SignalTakeProfit + "protoTakeProfitExit:" + proto.TakeProfitPositiveOffset
					+ "@" + proto.PriceTakeProfit + " for " + position.EntrySignal,
				MarketConverter.ExitDirectionFromLongShort(position.Prototype.LongShort),
				MarketLimitStop.Limit);
			if (alertTakeProfit == null) {
				string msg = "alertTakeProfit should NOT be null";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}
			proto.TakeProfitAlertForAnnihilation = alertTakeProfit;
			return alertTakeProfit;
		}
		public bool AnnihilateCounterpartyForClosedPosition(Position position) {
			bool killed = false;
			if (position.IsExitFilled == false) {
				string msg = "I refuse to AnnihilateCounterpartyForClosedPosition: position.IsExitFilled=false for position=[" + position + "]";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}
			if (position.Prototype == null) {
				string msg = "I refuse to AnnihilateCounterpartyForClosedPosition: position.Prototype=null";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}
			if (position.IsExitFilledByPrototypedTakeProfit && position.Prototype.StopLossNegativeOffset < 0) {
				if (position.Prototype.StopLossAlertForAnnihilation == null) {
					string msg = "FAILED_ANNIHILATE_STOPLOSS Prototype.StopLossAlertForAnnihilation=null for position[" + position + "]";
					#if DEBUG
					Debugger.Launch();
					#endif
					throw new Exception(msg);
				}
				killed = executor.AnnihilateCounterpartyAlertDispatched(position.Prototype.StopLossAlertForAnnihilation);
				if (executor.Backtester.IsBacktestingNoLivesimNow == false) {
					string msg = "killed[" + killed + "] counterParty [" + position.Prototype.StopLossAlertForAnnihilation + "]";
					this.appendMessageToTakeProfitOrder(position, msg);
				}
				if (killed == false) {
					string msg = "position.ClosedByTakeProfit but StopLoss wasn't annihilated"
						+ " or DUPE annihilation of a previously annihilated StopLoss";
					#if DEBUG
					Debugger.Launch();
					#endif
					throw new Exception(msg);
				}
			}
			if (position.IsExitFilledByPrototypedStopLoss && position.Prototype.TakeProfitPositiveOffset > 0) {
				if (position.Prototype.TakeProfitAlertForAnnihilation == null) {
					string msg = "FAILED_ANNIHILATE_TAKEPROFIT Prototype.TakeProfitAlertForAnnihilation=null for position[" + position + "]";
					#if DEBUG
					Debugger.Launch();
					#endif
					throw new Exception(msg);
				}
				killed = executor.AnnihilateCounterpartyAlertDispatched(position.Prototype.TakeProfitAlertForAnnihilation);
				if (executor.Backtester.IsBacktestingNoLivesimNow == false) {
					string msg = "killed[" + killed + "] counterParty [" + position.Prototype.TakeProfitAlertForAnnihilation + "]";
					this.appendMessageToStopLossOrder(position, msg);
				}
				if (killed == false) {
					string msg = "position.ClosedByStopLoss but TakeProfit wasn't annihilated"
						+ " or DUPE annihilation of a previously annihilated TakeProfit";
					#if DEBUG
					Debugger.Launch();
					#endif
					throw new Exception(msg);
				}
			}
			return killed;
		}
		void appendMessageToStopLossOrder(Position position, string msgOrder) {
			if (position.Prototype.StopLossAlertForAnnihilation == null) {
				string msg = "Can't AppendMessageToTakeProfitOrder: position.Prototype.StopLossAlertForAnnihilation=null";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}
			if (position.Prototype.StopLossAlertForAnnihilation.OrderFollowed == null) {
				string msg = "Can't AppendMessageToTakeProfitOrder: position.Prototype.StopLossAlertForAnnihilation.OrderFollowed=null";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}
			position.Prototype.StopLossAlertForAnnihilation.OrderFollowed.AppendMessage(msgOrder);
		}
		void appendMessageToTakeProfitOrder(Position position, string msgOrder) {
			if (position.Prototype.TakeProfitAlertForAnnihilation == null) {
				string msg = "Can't AppendMessageToTakeProfitOrder: position.Prototype.TakeProfitAlertForAnnihilation=null";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}
			if (position.Prototype.TakeProfitAlertForAnnihilation.OrderFollowed == null) {
				string msg = "Can't AppendMessageToTakeProfitOrder: position.Prototype.TakeProfitAlertForAnnihilation.OrderFollowed=null";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}
			position.Prototype.TakeProfitAlertForAnnihilation.OrderFollowed.AppendMessage(msgOrder);
		}
		public List<Alert> AlertFilledCreateSlTpOrAnnihilateCounterparty(Alert alert) {
			List<Alert> ret = new List<Alert>();
			if (alert.PositionAffected == null) return ret;
			if (alert.PositionAffected.Prototype == null) return ret;
			if (alert.IsEntryAlert) {
				return this.CreateStopLossAndTakeProfitAlertsFromPositionPrototype(alert.PositionAffected);
			} else {
				bool killed = this.AnnihilateCounterpartyForClosedPosition(alert.PositionAffected);
				return ret;
			}
		}
		public void StopLossNewNegativeOffsetUpdateActivate(Position position, double newStopLossNegativeOffset) {
			string msig = " StopLossNewNegativeOffsetUpdateActivate(position[" + position + "], newStopLossNegativeOffset[" + newStopLossNegativeOffset + "])";
			PositionPrototype proto = position.Prototype;
			if (proto == null) {
				string msg = "StopLossNewNegativeOffsetUpdateActivate() can't update StopLoss for a position.Prototype=null: position=[" + position + "]";
				Assembler.PopupException(msg, null, false);
			}
			try {
				this.checkThrowNewStopLossOffsetMakesSense(position, newStopLossNegativeOffset);
			} catch (Exception ex) {
				Assembler.PopupException("REFUSED_StopLossNewNegativeOffsetUpdateActivate()", ex, false);
				return;
			}
			double newActivationOffset = proto.CalcActivationOffsetForNewClosing(newStopLossNegativeOffset);
			switch (proto.StopLossAlertForAnnihilation.MarketLimitStop) {
				case MarketLimitStop.StopLimit:
					#region StopLimit are considered NYI in Backtester.QuoteGenerator; switched to Stops since they do SL job perfectly;
					//proto.checkOffsetsThrowBeforeAbsorbing(proto.TakeProfitPositiveOffset, newStopLossNegativeOffset, newActivationOffset);
					if (executor.Backtester.IsBacktestingNoLivesimNow) {
						// NOPE!!! I WANT ALL ALERTS TO STAY IN HISTORY!!! just move SL in Alert, change Prices immediately; no orderKill/newOrder for StopLoss (Alerts don't have OrdersFollowed)
						// PositionPrototypeActivator.checkThrowNewPriceMakesSense() made sure Alert!=null
						// SL_WILL_BE_KILLED proto.StopLossAlertForAnnihilation.PriceScript = proto.OffsetToPrice(newStopLossNegativeOffset);
						// SL_WILL_BE_KILLED proto.StopLossAlertForAnnihilation.PriceStopLimitActivation = proto.OffsetToPrice(newActivationOffset);
						proto.SetNewStopLossOffsets(newStopLossNegativeOffset, newActivationOffset);
						// TODO: don't forget about backtesting and MarketSim (implement OppMover for offline)
						executor.MarketsimBacktest.SimulateStopLossMoved(proto.StopLossAlertForAnnihilation);
					} else {
						executor.DataSource.BrokerAdapter.MoveStopLossOverrideable(proto, newActivationOffset, newStopLossNegativeOffset);
					}
					break;
					#endregion
				case MarketLimitStop.Stop:
					if (executor.Backtester.IsBacktestingNoLivesimNow) {
						proto.SetNewStopLossOffsets(newStopLossNegativeOffset, 0);
						// TODO: don't forget about backtesting and MarketSim (implement OppMover for offline)
						executor.MarketsimBacktest.SimulateStopLossMoved(proto.StopLossAlertForAnnihilation);
					} else {
						executor.DataSource.BrokerAdapter.MoveStopLossOverrideable(proto, 0, newStopLossNegativeOffset);
					}
					break;
				default:
					string msg = "UNSUPPORTED_STOP_LOSS_CANT_MOVE [" + proto.StopLossAlertForAnnihilation.MarketLimitStop
						+ "] must be Stop or StopLimit(weak support now, almost NYI)";
					#if DEBUG
					Debugger.Launch();
					#endif
					throw new Exception(msg + msig);
			}
		}
		public void TakeProfitNewPositiveOffsetUpdateActivate(Position position, double newTakeProfitPositiveOffset) {
			PositionPrototype proto = position.Prototype;
			if (proto == null) {
				string msg = "TakeProfitNewPositiveOffsetUpdateActivate() can't update TakeProfit for a position.Prototype=null: position=[" + position + "]";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}
			this.checkThrowNewTakeProfitOffsetMakesSense(position, newTakeProfitPositiveOffset);
			//proto.checkOffsetsThrowBeforeAbsorbing(newTakeProfitPositiveOffset, proto.StopLossNegativeOffset, proto.StopLossActivationNegativeOffset);
			if (executor.Backtester.IsBacktestingNoLivesimNow) {
				// NOPE!!! I WANT ALL ALERTS TO STAY IN HISTORY!!! just move TP in Alert, change Prices immediately; no orderKill/newOrder for TakeProfit (Alerts don't have OrdersFollowed)
				// PositionPrototypeActivator.checkThrowNewPriceMakesSense() made sure Alert!=null
				// TP_WILL_BE_KILLED proto.TakeProfitAlertForAnnihilation.PriceScript = proto.OffsetToPrice(newTakeProfitPositiveOffset);
				proto.SetNewTakeProfitOffset(newTakeProfitPositiveOffset);
				// TODO: don't forget about backtesting and MarketSim (implement OppMover for offline)
				executor.MarketsimBacktest.SimulateTakeProfitMoved(proto.TakeProfitAlertForAnnihilation);
			} else {
				executor.DataSource.BrokerAdapter.MoveTakeProfitOverrideable(proto, newTakeProfitPositiveOffset);
			}
		}
		void checkThrowPlacingProtoMakesSense(PositionPrototype proto) {
			string msg = this.ReasonWhyPlacingProtoDoesntMakeSense(proto);
			if (String.IsNullOrEmpty(msg) == false) {
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}
		}
		void checkThrowNewStopLossOffsetMakesSense(Position position, double newStopLossNegativeOffset) {
			string msg = this.ReasonWhyNewStopLossOffsetDoesntMakeSense(position, newStopLossNegativeOffset, position.ExitAlert.MarketLimitStop);
			if (String.IsNullOrEmpty(msg) == false) {
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}
		}
		void checkThrowNewTakeProfitOffsetMakesSense(Position position, double newTakeProfitPositiveOffset) {
			string msg = this.ReasonWhyNewTakeProfitOffsetDoesntMakeSense(position, newTakeProfitPositiveOffset);
			if (String.IsNullOrEmpty(msg) == false) {
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg);
			}
		}

		public string ReasonWhyPlacingProtoDoesntMakeSense(PositionPrototype proto, bool internalCallee = false) {
			double lastPrice = executor.DataSource.StreamingAdapter.StreamingDataSnapshot.LastQuoteGetPriceForMarketOrder(proto.Symbol);
			Quote quote = executor.DataSource.StreamingAdapter.StreamingDataSnapshot.LastQuoteCloneGetForSymbol(proto.Symbol);
			double priceBestBidAsk = executor.DataSource.StreamingAdapter.StreamingDataSnapshot.BidOrAskFor(proto.Symbol, proto.LongShort);
			bool willBeExecutedImmediately = false;
			MarketLimitStop planningEntryUsing = MarketConverter.EntryMarketLimitStopFromDirection(
				executor.Bars.BarStreamingNullUnsafeCloneReadonly.Close, proto.PriceEntry, proto.LongShort);

			string msg = "";
			Direction dir = MarketConverter.EntryDirectionFromLongShort(proto.LongShort);
			switch (dir) {
				case Direction.Buy:
					if (proto.PriceEntry > priceBestBidAsk) {
						willBeExecutedImmediately = true;
						msg = "proto.PriceEntry[" + proto.PriceEntry + "] > Bid[" + priceBestBidAsk + "]"
							+ " your Alert.EntryPrice goes above current price"
							+ " so the planningEntryUsing[" + planningEntryUsing + "] will be activated/filled immediately..."
							+ " //proto[" + proto + "]";
					}
					break;
				case Direction.Short:
					if (proto.PriceEntry < priceBestBidAsk) {
						willBeExecutedImmediately = true;
						msg = "proto.PriceEntry[" + proto.PriceEntry + "] < Ask[" + priceBestBidAsk + "]"
							+ " your Alert.EntryPrice goes below current price"
							+ " so the planningEntryUsing[" + planningEntryUsing + "] will be activated/filled immediately..."
							+ " //proto[" + proto + "]";
					}
					break;
				default:
					msg = "I refuse to PlaceOnce(proto) for.Direction=[" + dir + "] - must be Buy or Sell only!!!";
					break;
			}
			if (internalCallee == true) {
				msg += " (Script is strongly recommented to check proto.EntryPrice first so we don't pass unexpected position entries to the Market)";
			}
			return msg;
		}

		public string ReasonWhyNewStopLossOffsetDoesntMakeSense(Position position, double newStopLossNegativeOffset,
				MarketLimitStop marketLimitStopPlanned = MarketLimitStop.Unknown, bool internalCallee = false) {
			PositionPrototype proto = position.Prototype;
			if (position.Symbol != proto.Symbol) {
				string msg1 = "NotYetImplemented: your script changes StopLossOffset for a proto.Symbol[" + proto.Symbol + "]!=position.Symbol[" + position.Symbol + "]";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg1);
			}
			Direction dir = MarketConverter.EntryDirectionFromLongShort(proto.LongShort);
			if (position.EntryAlert.Direction != dir) {
				string msg1 = "NotYetImplemented: Crazy check here";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg1);
			}

			string msg = "";
			double priceBestBidAsk = executor.DataSource.StreamingAdapter.StreamingDataSnapshot.BidOrAskFor(proto.Symbol, proto.LongShort);
			double newStopLossPrice = proto.OffsetToPrice(newStopLossNegativeOffset);
			//switch (proto.StopLossAlertForAnnihilation.MarketLimitStop) {
			switch (marketLimitStopPlanned) {
				#region StopLimits are considered NYI; mess v1 implementation
				case MarketLimitStop.StopLimit:
					double newActivationOffset = proto.CalcActivationOffsetForNewClosing(newStopLossNegativeOffset);
					//double lastPrice = executor.DataSource.StreamingAdapter.StreamingDataSnapshot.LastQuoteGetPriceForMarketOrder(proto.Symbol);
					//Quote quote = executor.DataSource.StreamingAdapter.StreamingDataSnapshot.LastQuoteGetForSymbol(proto.Symbol);
					double newActivationPrice = proto.OffsetToPrice(newActivationOffset);
					bool willBeExecutedImmediately = false;
					string ident = "StopLoss{old[" + proto.PriceStopLoss + "]:new[" + newStopLossPrice + "]}"
								 + " Activation{old[" + proto.PriceStopLossActivation + "]:new[" + newActivationPrice + "]}";
					switch (dir) {
						case Direction.Buy:
							if (newActivationPrice > priceBestBidAsk) {
								willBeExecutedImmediately = true;
								msg = "newActivationPrice[" + newActivationPrice + "] > Bid[" + priceBestBidAsk + "] " + ident
									+ " your SLactivation goes above current price so the StopLoss will be activated/filled immediately..."
									+ " //position[" + position + "]";
							}
							break;
						case Direction.Short:
							if (newActivationPrice < priceBestBidAsk) {
								willBeExecutedImmediately = true;
								msg = "newActivationPrice[" + newActivationPrice + "] < Ask[" + priceBestBidAsk + "] " + ident
									+ " your SLactivation goes below current price so the StopLoss will be activated/filled immediately..."
									+ " //position[" + position + "]";
							}
							break;
						default:
							msg = "I refuse to change StopLoss when position.EntryAlert.Direction=["
								+ position.EntryAlert.Direction + "] - must be Buy or Short only!!!";
							break;
					}
					break;
					#endregion
				case MarketLimitStop.Stop:
					string ident2 = "PureStopLoss{old[" + proto.PriceStopLoss + "]:new[" + newStopLossPrice + "]}";
					Direction slDirectionAssumedOrActualIfFilled = proto.LongShort == PositionLongShort.Long ? Direction.Sell : Direction.Cover;
					if (proto.StopLossAlertForAnnihilation != null) slDirectionAssumedOrActualIfFilled = proto.StopLossAlertForAnnihilation.Direction;
					switch (slDirectionAssumedOrActualIfFilled) {
						case Direction.Sell:
							double ask = executor.DataSource.StreamingAdapter.StreamingDataSnapshot.BestAskGetForMarketOrder(proto.Symbol);
							if (newStopLossPrice > ask) {
								msg = "NEW_STOP_PRICE_BELOW_ASK_WILL_BE_REJECTED_BY_MARKET"
									+ " newStopLossPrice[" + newStopLossPrice + "] < Ask[" + ask + "] " + ident2
									+ " //position[" + position + "]";
							}
							break;
						case Direction.Cover:
							double bid = executor.DataSource.StreamingAdapter.StreamingDataSnapshot.BestBidGetForMarketOrder(proto.Symbol);
							if (newStopLossPrice < bid) {
								msg = "NEW_STOP_PRICE_ABOVE_BID_WILL_BE_REJECTED_BY_MARKET"
									+ " newStopLossPrice[" + newStopLossPrice + "] > Bid[" + bid + "] " + ident2
									+ " //position[" + position + "]";
							}
							break;
						default:
							msg = "PROTOTYPE_BASED_STOP_LOSS_MODIFICATION_REFUSED_MUSTBE_SELL_OR_COVER"
								+ ";  position.ExitA.Direction=[" + position.ExitAlert.Direction + "]";
							break;
					}
					break;
				default:
					msg = "STOP_LOSS_ALERT_TYPE_NOT_SUPPORTED [" + proto.StopLossAlertForAnnihilation.MarketLimitStop + "]";
					#if DEBUG
					Debugger.Launch();
					#endif
					throw new Exception(msg);
					//break;
			}
			if (internalCallee == true) {
				msg += " (Script is strongly recommented to check SL price first so we don't pass unexpected position closures to the Market)";
			}
			return msg;
		}
		public string ReasonWhyNewTakeProfitOffsetDoesntMakeSense(Position position, double newTakeProfitPositiveOffset, bool internalCallee = false) {
			PositionPrototype proto = position.Prototype;
			if (position.Symbol != proto.Symbol) {
				string msg1 = "NotYetImplemented: your script changes TakeProfitOffset for a proto.Symbol[" + proto.Symbol + "]!=position.Symbol[" + position.Symbol + "]";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg1);
			}
			Direction dir = MarketConverter.EntryDirectionFromLongShort(proto.LongShort);
			if (position.EntryAlert.Direction != dir) {
				string msg1 = "NotYetImplemented: Crazy check here";
				#if DEBUG
				Debugger.Launch();
				#endif
				throw new Exception(msg1);
			}
			Quote quote = executor.DataSource.StreamingAdapter.StreamingDataSnapshot.LastQuoteCloneGetForSymbol(proto.Symbol);
			double priceBestBidAsk = executor.DataSource.StreamingAdapter.StreamingDataSnapshot.BidOrAskFor(proto.Symbol, proto.LongShort);
			double newTakeProfitPrice = proto.OffsetToPrice(newTakeProfitPositiveOffset);
			bool willBeExecutedImmediately = false;
			string ident = "TakeProfit{old[" + proto.PriceTakeProfit + "]:new[" + newTakeProfitPrice + "]}";
			string msg = "";
			switch (dir) {
				case Direction.Buy:
					if (newTakeProfitPrice < priceBestBidAsk) {
						willBeExecutedImmediately = true;
						msg = "will be filled immediately: newLongTakeProfitPrice[" + newTakeProfitPrice + "] < Ask[" + priceBestBidAsk + "] "
							+ ident + " //position[" + position + "]";
					}
					break;
				case Direction.Short:
					if (newTakeProfitPrice > priceBestBidAsk) {
						willBeExecutedImmediately = true;
						msg = "will be filled immediately: newShortTakeProfitPrice[" + newTakeProfitPrice + "] > Bid[" + priceBestBidAsk + "] "
							+ ident + " //position[" + position + "]";
					}
					break;
				default:
					msg = "I refuse to change TakeProfit when position.EntryAlert.Direction=["
						+ position.EntryAlert.Direction + "] - must be Buy or Sell only!!!";
					break;
			}
			if (internalCallee == true) {
				msg += " (Script is strongly recommented to check TP price first so we don't pass unexpected position closures to the Market)";
			}
			return msg;
		}
		public double StopLossCurrentGetNaNunsafe(PositionPrototype proto) {
			if (proto.StopLossNegativeOffset == 0) return double.NaN;
			Alert SLalert = proto.StopLossAlertForAnnihilation;
			if (SLalert == null) {
				string msg = "CHECK_UPSTACK_WHAT_LED_TO_proto.StopLossAlertForAnnihilation=NULL";
				Assembler.PopupException(msg);
				return double.NaN;
			}
			if (SLalert.MarketLimitStop != MarketLimitStop.Stop && SLalert.MarketLimitStop != MarketLimitStop.StopLimit) {
				string msg = "CHECK_UPSTACK_WHAT_LED_TO_SLalert.MarketLimitStop=" + SLalert.MarketLimitStop;
				Assembler.PopupException(msg);
				return double.NaN;
			}
			return SLalert.PriceScriptAligned;
		}
		//[Obsolete("USELESS & UNTESTED")]
		//public double StopLossCurrentNegativeOffsetGetNaNunsafe(PositionPrototype proto) {
		//	double SL = this.StopLossCurrentGetNaNunsafe(proto);
		//	if (double.IsNaN(SL)) return SL;

		//	Bar bar = this.executor.Bars.BarStreamingCloneReadonly;
		//	if (bar == null) {
		//		string msg = "WHEN?";
		//		#if DEBUG
		//		Debugger.Launch();
		//		#endif
		//		return double.NaN;
		//	}
		//	// long has SLalert.PriceScriptAligned < bar.Close; we need NEGATIVE
		//	double ret = SL - bar.Close;
		//	Alert SLalert = proto.StopLossAlertForAnnihilation;
		//	if (SLalert.Direction == Direction.Cover) ret = -ret;
		//	return ret;
		//}
	}
}