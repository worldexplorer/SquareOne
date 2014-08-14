using System;
using System.Collections.Generic;
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
				string msg = "market orders can't be found in OrdersPending";
			}
			if (this.checkPrototypeAlreadyPlaced(proto)) return;
			//this.checkThrowPlacingProtoMakesSense(proto);
			proto.checkTPOffsetThrowBeforeAbsorbing(proto.TakeProfitPositiveOffset);
			proto.checkSLOffsetsThrowBeforeAbsorbing(proto.StopLossNegativeOffset, proto.StopLossActivationNegativeOffset);
			//bool a = this.executor.BacktesterFacade.IsBacktestingNow;

			Position posWithAlert = executor.BuyOrShortAlertCreateRegister (
				executor.Bars.BarStreaming,
				proto.PriceEntry, "protoEntry@" + proto.PriceEntry,
				MarketConverter.EntryDirectionFromLongShort(proto.LongShort),
				MarketConverter.EntryMarketLimitStopFromDirection(
					executor.Bars.BarStreamingCloneReadonly.Close, proto.PriceEntry, proto.LongShort)
				);
			if (posWithAlert == null) {
				string msg = "man I don't understand this null; out-of-bar limit should still leave a pending Alert.PositionAffected";
				throw new Exception(msg);
			}
			if (posWithAlert.Prototype != null) {
				string msg = "CLEANUP: I was trying to catch MoveStopLoss::if(proto.StopLossAlertForAnnihilation==null)"
					+ " so I thought there is a new prototype assigned to a position,"
					+ " since we never put null directly proto.StopLossAlertForAnnihilation";
				throw new Exception(msg);
			}
			posWithAlert.Prototype = proto;
		}

		private bool checkPrototypeAlreadyPlaced(PositionPrototype proto) {
			foreach (Alert alert in executor.ExecutionDataSnapshot.AlertsPending) {
				Position pos = alert.PositionAffected;
				if (pos == null) continue;
				if (pos.Prototype == null) continue;
				if (pos.Prototype.IsIdenticalTo(proto)) return true;
			}
			return false;
		}
		private bool checkPendingEntryPositionAlreadyPlaced(Position entryPosition) {
			if (entryPosition.EntryAlert == null) return false;
			foreach (Alert alert in executor.ExecutionDataSnapshot.AlertsPending) {
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
				throw new Exception(msg);
			}
			Alert SlPlaced = this.CreateStopLossFromPositionPrototype(position);
			Alert TpPlaced = this.CreateTakeProfitFromPositionPrototype(position);
			return new List<Alert>() { SlPlaced, TpPlaced };
		}
		public Alert CreateStopLossFromPositionPrototype(Position position) {
			//if (this.executor.checkPrototypeAlreadyPlaced(position)) return;
			PositionPrototype proto = position.Prototype;
			if (proto.StopLossNegativeOffset == 0) {
				string msg = "What should Activator do with proto.StopLossNegativeOffset=0?";
				throw new Exception(msg);
			}

			if (proto.StopLossNegativeOffset == 0) {
				string msg = this.ReasonWhyNewStopLossOffsetDoesntMakeSense(position, proto.StopLossNegativeOffset);
				if (String.IsNullOrEmpty(msg) == false) {
					string msg2 = "What should Activator do with sense-less proto.StopLossNegativeOffset[" + proto.StopLossNegativeOffset + "], ";
					throw new Exception(msg2, new Exception(msg));
				}
			}

			MarketLimitStop simpleStopIfActivationZero = (proto.StopLossActivationNegativeOffset == 0) ? MarketLimitStop.Stop : MarketLimitStop.StopLimit;

			Alert alertStopLoss = executor.SellOrCoverAlertCreateDontRegister (
				executor.Bars.BarStreaming,
				position, proto.PriceStopLoss,
				"protoStopLossExit:" + proto.StopLossActivationNegativeOffset
					+ "@" + proto.StopLossNegativeOffset + " for " + position.EntrySignal,
				MarketConverter.ExitDirectionFromLongShort(proto.LongShort),
				simpleStopIfActivationZero);
			if (alertStopLoss == null) {
				string msg = "alertStopLoss should NOT be null";
				throw new Exception(msg);
			}
			alertStopLoss.PriceStopLimitActivation = 0;
			if (proto.StopLossActivationNegativeOffset < 0) alertStopLoss.PriceStopLimitActivation = proto.PriceStopLossActivation;
			if (proto.StopLossAlertForAnnihilation != null && this.executor.Backtester.IsBacktestingNow == false) {
				string msg = "CLEANUP: I was trying to catch MoveStopLoss::if(proto.StopLossAlertForAnnihilation==null)"
					+ " so I thought there is a new prototype assigned to a position,"
					+ " since we never put null directly proto.StopLossAlertForAnnihilation";
				throw new Exception(msg);
			}

			proto.StopLossAlertForAnnihilation = alertStopLoss;
			return alertStopLoss;
		}
		public Alert CreateTakeProfitFromPositionPrototype(Position position) {
			PositionPrototype proto = position.Prototype;
			if (proto.TakeProfitPositiveOffset == 0) {
				string msg = "What should Activator do with proto.StopLossNegativeOffset=0?";
				throw new Exception(msg);
			}

			Alert alertTakeProfit = executor.SellOrCoverAlertCreateDontRegister (
				executor.Bars.BarStreaming,
				position, proto.PriceTakeProfit,
				"protoTakeProfitExit:" + proto.TakeProfitPositiveOffset
					+ "@" + proto.PriceTakeProfit + " for " + position.EntrySignal,
				MarketConverter.ExitDirectionFromLongShort(position.Prototype.LongShort),
				MarketLimitStop.Limit);
			if (alertTakeProfit == null) {
				string msg = "alertTakeProfit should NOT be null";
				throw new Exception(msg);
			}
			proto.TakeProfitAlertForAnnihilation = alertTakeProfit;
			return alertTakeProfit;
		}
		public bool AnnihilateCounterpartyForClosedPosition(Position position) {
			bool killed = false;
			if (position.IsExitFilled == false) {
				string msg = "I refuse to AnnihilateCounterpartyForClosedPosition: position.IsExitFilled=false for position=[" + position + "]";
				throw new Exception(msg);
			}
			if (position.Prototype == null) {
				string msg = "I refuse to AnnihilateCounterpartyForClosedPosition: position.Prototype=null";
				throw new Exception(msg);
			}
			if (position.IsExitFilledByPrototypedTakeProfit) {
				if (position.Prototype.StopLossAlertForAnnihilation == null) {
					string msg = "FAILED_ANNIHILATE_STOPLOSS Prototype.StopLossAlertForAnnihilation=null for position[" + position + "]";
					throw new Exception(msg);
				}
				killed = executor.AnnihilateCounterpartyAlertDispatched(position.Prototype.StopLossAlertForAnnihilation);
				if (executor.Backtester.IsBacktestingNow == false) {
					string msg = "killed[" + killed + "] counterParty [" + position.Prototype.StopLossAlertForAnnihilation + "]";
					this.appendMessageToTakeProfitOrder(position, msg);
				}
				if (killed == false) {
					string msg = "position.ClosedByTakeProfit but StopLoss wasn't annihilated"
						+ " or DUPE annihilation of a previously annihilated StopLoss";
					throw new Exception(msg);
				}
			}
			if (position.IsExitFilledByPrototypedStopLoss) {
				if (position.Prototype.TakeProfitAlertForAnnihilation == null) {
					string msg = "FAILED_ANNIHILATE_TAKEPROFIT Prototype.TakeProfitAlertForAnnihilation=null for position[" + position + "]";
					throw new Exception(msg);
				}
				killed = executor.AnnihilateCounterpartyAlertDispatched(position.Prototype.TakeProfitAlertForAnnihilation);
				if (executor.Backtester.IsBacktestingNow == false) {
					string msg = "killed[" + killed + "] counterParty [" + position.Prototype.TakeProfitAlertForAnnihilation + "]";
					this.appendMessageToStopLossOrder(position, msg);
				}
				if (killed == false) {
					string msg = "position.ClosedByStopLoss but TakeProfit wasn't annihilated"
						+ " or DUPE annihilation of a previously annihilated TakeProfit";
					throw new Exception(msg);
				}
			}
			return killed;
		}
		void appendMessageToStopLossOrder(Position position, string msgOrder) {
			if (position.Prototype.StopLossAlertForAnnihilation == null) {
				string msg = "Can't AppendMessageToTakeProfitOrder: position.Prototype.StopLossAlertForAnnihilation=null";
				throw new Exception(msg);
			}
			if (position.Prototype.StopLossAlertForAnnihilation.OrderFollowed == null) {
				string msg = "Can't AppendMessageToTakeProfitOrder: position.Prototype.StopLossAlertForAnnihilation.OrderFollowed=null";
				throw new Exception(msg);
			}
			position.Prototype.StopLossAlertForAnnihilation.OrderFollowed.AppendMessage(msgOrder);
		}
		void appendMessageToTakeProfitOrder(Position position, string msgOrder) {
			if (position.Prototype.TakeProfitAlertForAnnihilation == null) {
				string msg = "Can't AppendMessageToTakeProfitOrder: position.Prototype.TakeProfitAlertForAnnihilation=null";
				throw new Exception(msg);
			}
			if (position.Prototype.TakeProfitAlertForAnnihilation.OrderFollowed == null) {
				string msg = "Can't AppendMessageToTakeProfitOrder: position.Prototype.TakeProfitAlertForAnnihilation.OrderFollowed=null";
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
				throw new Exception(msg);
			}
			this.checkThrowNewStopLossOffsetMakesSense(position, newStopLossNegativeOffset);
			double newActivationOffset = proto.CalcActivationOffsetForNewClosing(newStopLossNegativeOffset);
			switch (proto.StopLossAlertForAnnihilation.MarketLimitStop) {
				case MarketLimitStop.StopLimit:
					#region StopLimit are considered NYI in Backtester.QuoteGenerator; switched to Stops since they do SL job perfectly;
					//proto.checkOffsetsThrowBeforeAbsorbing(proto.TakeProfitPositiveOffset, newStopLossNegativeOffset, newActivationOffset);
					if (executor.Backtester.IsBacktestingNow) {
						// NOPE!!! I WANT ALL ALERTS TO STAY IN HISTORY!!! just move SL in Alert, change Prices immediately; no orderKill/newOrder for StopLoss (Alerts don't have OrdersFollowed)
						// PositionPrototypeActivator.checkThrowNewPriceMakesSense() made sure Alert!=null
						// SL_WILL_BE_KILLED proto.StopLossAlertForAnnihilation.PriceScript = proto.OffsetToPrice(newStopLossNegativeOffset);
						// SL_WILL_BE_KILLED proto.StopLossAlertForAnnihilation.PriceStopLimitActivation = proto.OffsetToPrice(newActivationOffset);
						proto.SetNewStopLossOffsets(newStopLossNegativeOffset, newActivationOffset);
						// TODO: don't forget about backtesting and MarketSim (implement OppMover for offline)
						executor.MarketSimStreaming.SimulateStopLossMoved(proto.StopLossAlertForAnnihilation);
					} else {
						executor.DataSource.BrokerProvider.MoveStopLossOrderProcessorInvoker(proto, newActivationOffset, newStopLossNegativeOffset);
					}
					break;
					#endregion
				case MarketLimitStop.Stop:
					if (executor.Backtester.IsBacktestingNow) {
						proto.SetNewStopLossOffsets(newStopLossNegativeOffset, 0);
						// TODO: don't forget about backtesting and MarketSim (implement OppMover for offline)
						executor.MarketSimStreaming.SimulateStopLossMoved(proto.StopLossAlertForAnnihilation);
					} else {
						executor.DataSource.BrokerProvider.MoveStopLossOrderProcessorInvoker(proto, 0, newStopLossNegativeOffset);
					}
					break;
				default:
					string msg = "UNSUPPORTED_STOP_LOSS_CANT_MOVE [" + proto.StopLossAlertForAnnihilation.MarketLimitStop + "] must be Stop or StopLimit(weak support now, almost NYI)";
					throw new Exception(msg + msig);
			}
		}
		public void TakeProfitNewPositiveOffsetUpdateActivate(Position position, double newTakeProfitPositiveOffset) {
			PositionPrototype proto = position.Prototype;
			if (proto == null) {
				string msg = "TakeProfitNewPositiveOffsetUpdateActivate() can't update TakeProfit for a position.Prototype=null: position=[" + position + "]";
				throw new Exception(msg);
			}
			this.checkThrowNewTakeProfitOffsetMakesSense(position, newTakeProfitPositiveOffset);
			//proto.checkOffsetsThrowBeforeAbsorbing(newTakeProfitPositiveOffset, proto.StopLossNegativeOffset, proto.StopLossActivationNegativeOffset);
			if (executor.Backtester.IsBacktestingNow) {
				// NOPE!!! I WANT ALL ALERTS TO STAY IN HISTORY!!! just move TP in Alert, change Prices immediately; no orderKill/newOrder for TakeProfit (Alerts don't have OrdersFollowed)
				// PositionPrototypeActivator.checkThrowNewPriceMakesSense() made sure Alert!=null
				// TP_WILL_BE_KILLED proto.TakeProfitAlertForAnnihilation.PriceScript = proto.OffsetToPrice(newTakeProfitPositiveOffset);
				proto.SetNewTakeProfitOffset(newTakeProfitPositiveOffset);
				// TODO: don't forget about backtesting and MarketSim (implement OppMover for offline)
				executor.MarketSimStreaming.SimulateTakeProfitMoved(proto.TakeProfitAlertForAnnihilation);
			} else {
				executor.DataSource.BrokerProvider.MoveTakeProfitOrderProcessorInvoker(proto, newTakeProfitPositiveOffset);
			}
		}
		void checkThrowPlacingProtoMakesSense(PositionPrototype proto) {
			string msg = this.ReasonWhyPlacingProtoDoesntMakeSense(proto);
			if (String.IsNullOrEmpty(msg) == false) throw new Exception(msg);
		}
		void checkThrowNewStopLossOffsetMakesSense(Position position, double newStopLossNegativeOffset) {
			string msg = this.ReasonWhyNewStopLossOffsetDoesntMakeSense(position, newStopLossNegativeOffset);
			if (String.IsNullOrEmpty(msg) == false) throw new Exception(msg);
		}
		void checkThrowNewTakeProfitOffsetMakesSense(Position position, double newTakeProfitPositiveOffset) {
			string msg = this.ReasonWhyNewTakeProfitOffsetDoesntMakeSense(position, newTakeProfitPositiveOffset);
			if (String.IsNullOrEmpty(msg) == false) throw new Exception(msg);
		}

		public string ReasonWhyPlacingProtoDoesntMakeSense(PositionPrototype proto, bool internalCallee = false) {
			double lastPrice = executor.DataSource.StreamingProvider.StreamingDataSnapshot.LastQuoteGetPriceForMarketOrder(proto.Symbol);
			Quote quote = executor.DataSource.StreamingProvider.StreamingDataSnapshot.LastQuoteGetForSymbol(proto.Symbol);
			double priceBestBidAsk = executor.DataSource.StreamingProvider.StreamingDataSnapshot.BidOrAskFor(proto.Symbol, proto.LongShort);
			bool willBeExecutedImmediately = false;
			MarketLimitStop planningEntryUsing = MarketConverter.EntryMarketLimitStopFromDirection(
				executor.Bars.BarStreamingCloneReadonly.Close, proto.PriceEntry, proto.LongShort);

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

		public string ReasonWhyNewStopLossOffsetDoesntMakeSense(Position position, double newStopLossNegativeOffset, bool internalCallee = false) {
			PositionPrototype proto = position.Prototype;
			if (position.Symbol != proto.Symbol) {
				string msg1 = "NotYetImplemented: your script changes StopLossOffset for a proto.Symbol[" + proto.Symbol + "]!=position.Symbol[" + position.Symbol + "]";
				throw new Exception(msg1);
			}
			Direction dir = MarketConverter.EntryDirectionFromLongShort(proto.LongShort);
			if (position.EntryAlert.Direction != dir) {
				string msg1 = "NotYetImplemented: Crazy check here";
				throw new Exception(msg1);
			}

			string msg = "";
			double priceBestBidAsk = executor.DataSource.StreamingProvider.StreamingDataSnapshot.BidOrAskFor(proto.Symbol, proto.LongShort);
			double newStopLossPrice = proto.OffsetToPrice(newStopLossNegativeOffset);
			switch (proto.StopLossAlertForAnnihilation.MarketLimitStop) {
				#region StopLimits are considered NYI; mess v1 implementation
				case MarketLimitStop.StopLimit:
					double newActivationOffset = proto.CalcActivationOffsetForNewClosing(newStopLossNegativeOffset);
					//double lastPrice = executor.DataSource.StreamingProvider.StreamingDataSnapshot.LastQuoteGetPriceForMarketOrder(proto.Symbol);
					//Quote quote = executor.DataSource.StreamingProvider.StreamingDataSnapshot.LastQuoteGetForSymbol(proto.Symbol);
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
					switch (proto.StopLossAlertForAnnihilation.Direction) {
						case Direction.Sell:
							double ask = executor.DataSource.StreamingProvider.StreamingDataSnapshot.BestAskGetForMarketOrder(proto.Symbol);
							if (newStopLossPrice > ask) {
								msg = "NEW_STOP_PRICE_BELOW_ASK_WILL_BE_REJECTED_BY_MARKET"
									+ " newStopLossPrice[" + newStopLossPrice + "] < Ask[" + ask + "] " + ident2
									+ " //position[" + position + "]";
							}
							break;
						case Direction.Cover:
							double bid = executor.DataSource.StreamingProvider.StreamingDataSnapshot.BestBidGetForMarketOrder(proto.Symbol);
							if (newStopLossPrice > bid) {
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
					throw new Exception(msg);
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
				throw new Exception(msg1);
			}
			Direction dir = MarketConverter.EntryDirectionFromLongShort(proto.LongShort);
			if (position.EntryAlert.Direction != dir) {
				string msg1 = "NotYetImplemented: Crazy check here";
				throw new Exception(msg1);
			}
			Quote quote = executor.DataSource.StreamingProvider.StreamingDataSnapshot.LastQuoteGetForSymbol(proto.Symbol);
			double priceBestBidAsk = executor.DataSource.StreamingProvider.StreamingDataSnapshot.BidOrAskFor(proto.Symbol, proto.LongShort);
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
	}
}