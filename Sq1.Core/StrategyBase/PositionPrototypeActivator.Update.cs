using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.Execution;
using Sq1.Core.DataTypes;

namespace Sq1.Core.StrategyBase {
	public partial class PositionPrototypeActivator {

		public void StopLoss_newNegativeOffset_updateActivate(Position position, double newStopLoss_negativeOffset) {
			string msig = " StopLossNewNegativeOffsetUpdateActivate(position[" + position + "], newStopLossNegativeOffset[" + newStopLoss_negativeOffset + "])";
			PositionPrototype proto = position.Prototype;
			if (proto == null) {
				string msg = "StopLossNewNegativeOffsetUpdateActivate() can't update StopLoss for a position.Prototype=null: position=[" + position + "]";
				Assembler.PopupException(msg, null, false);
			}
			try {
				this.checkThrow_newStopLossOffset_makesSense(position, newStopLoss_negativeOffset);
			} catch (Exception ex) {
				Assembler.PopupException("REFUSED_StopLossNewNegativeOffsetUpdateActivate()", ex, false);
				return;
			}
			double newActivationOffset = proto.CalcActivationOffset_forNewClosing(newStopLoss_negativeOffset);
			switch (proto.StopLossAlert_forMoveAndAnnihilation.MarketLimitStop) {
				case MarketLimitStop.StopLimit:
					#region StopLimit are considered NYI in Backtester.QuoteGenerator; switched to Stops since they do SL job perfectly;
					//v1 ScriptExecutor trying be too smart
					//proto.checkOffsetsThrowBeforeAbsorbing(proto.TakeProfitPositiveOffset, newStopLossNegativeOffset, newActivationOffset);
					//if (executor.BacktesterOrLivesimulator.IsBacktestingNoLivesimNow) {
					//	// NOPE!!! I WANT ALL ALERTS TO STAY IN HISTORY!!! just move SL in Alert, change Prices immediately; no orderKill/newOrder for StopLoss (Alerts don't have OrdersFollowed)
					//	// PositionPrototypeActivator.checkThrowNewPriceMakesSense() made sure Alert!=null
					//	// SL_WILL_BE_KILLED proto.StopLossAlertForAnnihilation.PriceScript = proto.OffsetToPrice(newStopLossNegativeOffset);
					//	// SL_WILL_BE_KILLED proto.StopLossAlertForAnnihilation.PriceStopLimitActivation = proto.OffsetToPrice(newActivationOffset);
					//	proto.SetNewStopLossOffsets(newStopLossNegativeOffset, newActivationOffset);
					//	// TODO: don't forget about backtesting and MarketSim (implement OppMover for offline)
					//	executor.BacktesterOrLivesimulator.BacktestDataSource.BrokerAsBacktest_nullUnsafe.SimulateStopLossMoved(proto.StopLossAlertForAnnihilation);
					//} else {
					//	this.executor.DataSource_fromBars.BrokerAdapter.MoveStopLossOverrideable(proto, newActivationOffset, newStopLossNegativeOffset);
					//}
					//v2 BrokerAdapter is now responsible for the implementation (Backtest/Livesim/Live)
					this.executor.DataSource_fromBars.BrokerAdapter.OrderMoveExisting_stopLoss_overrideable(proto, newActivationOffset, newStopLoss_negativeOffset);
					break;
					#endregion
				case MarketLimitStop.Stop:
					//v1 ScriptExecutor trying be too smart
					//if (executor.BacktesterOrLivesimulator.IsBacktestingNoLivesimNow) {
					//	proto.SetNewStopLossOffsets(newStopLossNegativeOffset, 0);
					//	// TODO: don't forget about backtesting and MarketSim (implement OppMover for offline)
					//	executor.MarketsimBacktest.SimulateStopLossMoved(proto.StopLossAlertForAnnihilation);
					//} else {
					//	executor.DataSource_fromBars.BrokerAdapter.MoveStopLossOverrideable(proto, 0, newStopLossNegativeOffset);
					//}
					//v2 BrokerAdapter is now responsible for the implementation (Backtest/Livesim/Live)
					this.executor.DataSource_fromBars.BrokerAdapter.OrderMoveExisting_stopLoss_overrideable(proto, 0, newStopLoss_negativeOffset);
					break;
				default:
					string msg = "UNSUPPORTED_STOP_LOSS_CANT_MOVE [" + proto.StopLossAlert_forMoveAndAnnihilation.MarketLimitStop
						+ "] must be Stop or StopLimit(weak support now, almost NYI)";
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception(msg + msig);
			}
		}
		public void TakeProfit_newPositiveOffset_updateActivate(Position position, double newTakeProfit_positiveOffset) {
			string msig = " //TakeProfit_newPositiveOffset_updateActivate(" + position + ", newTakeProfit_positiveOffset[" + newTakeProfit_positiveOffset + "])";
			PositionPrototype proto = position.Prototype;
			if (proto == null) {
				string msg = "TakeProfitNewPositiveOffsetUpdateActivate() can't update TakeProfit for a position.Prototype=null: position=[" + position + "]";
				Assembler.PopupException(msg + msig);
				throw new Exception(msg + msig);
			}
			this.checkThrow_newTakeProfitOffset_makesSense(position, newTakeProfit_positiveOffset);
			//proto.checkOffsetsThrowBeforeAbsorbing(newTakeProfitPositiveOffset, proto.StopLossNegativeOffset, proto.StopLossActivationNegativeOffset);

			//v1 ScriptExecutor trying be too smart
			//if (executor.BacktesterOrLivesimulator.IsBacktestingNoLivesimNow) {
			//	// NOPE!!! I WANT ALL ALERTS TO STAY IN HISTORY!!! just move TP in Alert, change Prices immediately; no orderKill/newOrder for TakeProfit (Alerts don't have OrdersFollowed)
			//	// PositionPrototypeActivator.checkThrowNewPriceMakesSense() made sure Alert!=null
			//	// TP_WILL_BE_KILLED proto.TakeProfitAlertForAnnihilation.PriceScript = proto.OffsetToPrice(newTakeProfitPositiveOffset);
			//	proto.SetNewTakeProfitOffset(newTakeProfitPositiveOffset);
			//	// TODO: don't forget about backtesting and MarketSim (implement OppMover for offline)
			//	this.executor.MarketsimBacktest.SimulateTakeProfitMoved(proto.TakeProfitAlertForAnnihilation);
			//} else {
			//	this.executor.DataSource_fromBars.BrokerAdapter.MoveTakeProfitOverrideable(proto, newTakeProfitPositiveOffset);
			//}
			//v2 BrokerAdapter is now responsible for the implementation (Backtest/Livesim/Live)
			this.executor.DataSource_fromBars.BrokerAdapter.OrderMoveExisting_takeProfit_overrideable(proto, newTakeProfit_positiveOffset);
		}

		void checkThrow_newStopLossOffset_makesSense(Position position, double newStopLoss_negativeOffset) {
			string msig = " //checkThrow_newStopLossOffset_makesSense(" + position + ", newStopLoss_negativeOffset[" + newStopLoss_negativeOffset + "])";
			string msg = this.reasonWhy_newStopLossOffset_doesntMakeSense(position, newStopLoss_negativeOffset, position.ExitAlert.MarketLimitStop);
			if (String.IsNullOrEmpty(msg) == false) {
				Assembler.PopupException(msg + msig);
				throw new Exception(msg + msig);
			}
		}
		void checkThrow_newTakeProfitOffset_makesSense(Position position, double newTakeProfit_positiveOffset) {
			string msig = " //checkThrow_newTakeProfitOffset_makesSense(" + position + ", newStopLoss_negativeOffset[" + newTakeProfit_positiveOffset + "])";
			string msg = this.reasonWhy_newTakeProfitOffset_doesntMakeSense(position, newTakeProfit_positiveOffset);
			if (String.IsNullOrEmpty(msg) == false) {
				Assembler.PopupException(msg + msig);
				throw new Exception(msg + msig);
			}
		}

		string reasonWhy_newStopLossOffset_doesntMakeSense(Position position, double newStopLoss_negativeOffset,
				MarketLimitStop marketLimitStopPlanned = MarketLimitStop.Unknown, bool internalCallee = false) {
			string msig = " //reasonWhy_newStopLossOffset_doesntMakeSense(" + position + ", newStopLoss_negativeOffset[" + newStopLoss_negativeOffset + "])";

			PositionPrototype proto = position.Prototype;
			if (position.Symbol != proto.Symbol) {
				string msg1 = "NotYetImplemented: your script changes StopLossOffset for a proto.Symbol[" + proto.Symbol + "]!=position.Symbol[" + position.Symbol + "]";
				Assembler.PopupException(msg1 + msig);
				throw new Exception(msg1 + msig);
			}
			Direction dir = MarketConverter.EntryDirectionFromLongShort(proto.LongShort);
			if (position.EntryAlert.Direction != dir) {
				string msg1 = "NotYetImplemented: Crazy check here";
				Assembler.PopupException(msg1 + msig);
				throw new Exception(msg1 + msig);
			}

			string msg = "";
			double priceBestBidAsk = executor.DataSource_fromBars.StreamingAdapter.StreamingDataSnapshot.BidOrAsk_forDirection(proto.Symbol, proto.LongShort);
			double newStopLossPrice = proto.OffsetToPrice(newStopLoss_negativeOffset);
			//switch (proto.StopLossAlertForAnnihilation.MarketLimitStop) {
			switch (marketLimitStopPlanned) {
				#region StopLimits are considered NYI; mess v1 implementation
				case MarketLimitStop.StopLimit:
					double newActivationOffset = proto.CalcActivationOffset_forNewClosing(newStopLoss_negativeOffset);
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
					if (proto.StopLossAlert_forMoveAndAnnihilation != null) slDirectionAssumedOrActualIfFilled = proto.StopLossAlert_forMoveAndAnnihilation.Direction;
					switch (slDirectionAssumedOrActualIfFilled) {
						case Direction.Sell:
							double ask = executor.DataSource_fromBars.StreamingAdapter.StreamingDataSnapshot.BestAsk_getForMarketOrder(proto.Symbol);
							if (newStopLossPrice > ask) {
								msg = "NEW_STOP_PRICE_BELOW_ASK_WILL_BE_REJECTED_BY_MARKET"
									+ " newStopLossPrice[" + newStopLossPrice + "] < Ask[" + ask + "] " + ident2
									+ " //position[" + position + "]";
							}
							break;
						case Direction.Cover:
							double bid = executor.DataSource_fromBars.StreamingAdapter.StreamingDataSnapshot.BestBid_getForMarketOrder(proto.Symbol);
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
					msg = "STOP_LOSS_ALERT_TYPE_NOT_SUPPORTED [" + proto.StopLossAlert_forMoveAndAnnihilation.MarketLimitStop + "]";
					Assembler.PopupException(msg + msig);
					throw new Exception(msg + msig);
					//break;
			}
			if (internalCallee == true) {
				msg += " (Script is strongly recommented to check SL price first so we don't pass unexpected position closures to the Market)";
			}
			return msg;
		}
		string reasonWhy_newTakeProfitOffset_doesntMakeSense(Position position, double newTakeProfit_positiveOffset, bool internalCallee = false) {
			string msig = " //reasonWhy_newTakeProfitOffset_doesntMakeSense(" + position + ", newTakeProfit_positiveOffset[" + newTakeProfit_positiveOffset + "])";
			PositionPrototype proto = position.Prototype;
			if (position.Symbol != proto.Symbol) {
				string msg1 = "NotYetImplemented: your script changes TakeProfitOffset for a proto.Symbol[" + proto.Symbol + "]!=position.Symbol[" + position.Symbol + "]";
				Assembler.PopupException(msg1 + msig);
				throw new Exception(msg1 + msig);
			}
			Direction dir = MarketConverter.EntryDirectionFromLongShort(proto.LongShort);
			if (position.EntryAlert.Direction != dir) {
				string msg1 = "NotYetImplemented: Crazy check here";
				Assembler.PopupException(msg1 + msig);
				throw new Exception(msg1 + msig);
			}
			Quote quote = executor.DataSource_fromBars.StreamingAdapter.StreamingDataSnapshot.LastQuote_getForSymbol(proto.Symbol);
			double priceBestBidAsk = executor.DataSource_fromBars.StreamingAdapter.StreamingDataSnapshot.BidOrAsk_forDirection(proto.Symbol, proto.LongShort);
			double newTakeProfitPrice = proto.OffsetToPrice(newTakeProfit_positiveOffset);
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