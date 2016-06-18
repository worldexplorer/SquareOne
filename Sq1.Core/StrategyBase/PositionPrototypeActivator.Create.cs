using System;
using System.Collections.Generic;

using Sq1.Core.Execution;
using Sq1.Core.DataTypes;

namespace Sq1.Core.StrategyBase {
	public partial class PositionPrototypeActivator {

		public void PlaceOnce(PositionPrototype proto) {
			string msig = " //PlaceOnce(" + proto + ")";
			if (proto.PriceEntry == 0) {
				//v1 string msg = "market orders can't be found in OrdersPending";
				string msg = "EntryMarketLimitStopFromDirection(XX, priceExecutionDesired=0, XX) will return MarketLimitStop.Market";
			}
			if (this.checkPrototype_alreadyPlaced(proto)) return;
			//this.checkThrowPlacingProtoMakesSense(proto);
			proto.checkTPOffset_throwBeforeAbsorbing(proto.TakeProfit_positiveOffset);
			proto.checkSLOffsets_throwBeforeAbsorbing(proto.StopLoss_negativeOffset, proto.StopLossActivation_negativeOffset);
			//bool a = this.executor.Backtester.IsBacktestingNow;

			Alert alert = executor.BuyOrShort_alertCreateRegister (
				this.executor.Bars.BarStreaming_nullUnsafe, proto.PriceEntry,
				proto.SignalEntry + "protoEntry@" + proto.PriceEntry,
				MarketConverter.EntryDirectionFromLongShort(proto.LongShort),
				MarketConverter.EntryMarketLimitStopFromDirection(
					this.executor.Bars.BarStreaming_nullUnsafeCloneReadonly.Close, proto.PriceEntry, proto.LongShort)
				);
			if (alert == null) {
				string msg = "man I don't understand this null; out-of-bar limit should still leave a pending Alert.PositionAffected";
				Assembler.PopupException(msg + msig);
				throw new Exception(msg + msig);
			}
			if (alert.PositionPrototype != null) {
				string msg = "CLEANUP: I was trying to catch MoveStopLoss::if(proto.StopLossAlertForAnnihilation==null)"
					+ " so I thought there is a new prototype assigned to a position,"
					+ " since we never put null directly proto.StopLossAlertForAnnihilation";
				Assembler.PopupException(msg + msig);
				throw new Exception(msg + msig);
			}
			alert.PositionPrototype = proto;
		}

		bool checkPrototype_alreadyPlaced(PositionPrototype proto) {
			string msig = " //checkPrototype_alreadyPlaced(WAIT)";
			List<Alert> pendingSafe = this.executor.ExecutionDataSnapshot.AlertsUnfilled.SafeCopy(this, msig);
			foreach (Alert alert in pendingSafe) {
				//Position pos = alert.PositionAffected;
				//if (pos == null) continue;
				if (alert.PositionPrototype == null) continue;
				if (alert.PositionPrototype.IsIdenticalTo(proto)) return true;
			}
			return false;
		}
		bool checkPendingEntry_positionAlreadyPlaced(Position entryPosition) {
			string msig = " //checkPendingEntry_positionAlreadyPlaced(WAIT)";
			if (entryPosition.EntryAlert == null) return false;
			List<Alert> pendingSafe = this.executor.ExecutionDataSnapshot.AlertsUnfilled.SafeCopy(this, msig);
			foreach (Alert alert in pendingSafe) {
				Position pos = alert.PositionAffected;
				if (pos == null) continue;
				if (pos.EntryAlert == null) continue;
				if (pos.EntryAlert == entryPosition.EntryAlert) return true;
				if (pos.EntryAlert.IsIdentical_forOrdersPending(entryPosition.EntryAlert)) return true;
			}
			return false;
		}

		public List<Alert> CreateStopLossAndTakeProfitAlerts_fromPositionPrototype(Position position) {
			string msig = " //CreateStopLossAndTakeProfitAlerts_fromPositionPrototype(" + position + ")";
			if (position.IsEntryFilled == false) {
				string msg = "I can not place SL and TP for an unopened position; alert[" + position.EntryAlert + "]";
				Assembler.PopupException(msg + msig);
				throw new Exception(msg + msig);
			}
			List<Alert> ret = new List<Alert>(); 
			Alert SlPlaced = this.CreateStopLoss_fromPositionPrototype(position);
			Alert TpPlaced = this.CreateTakeProfit_fromPositionPrototype(position);
			if (SlPlaced != null) ret.Add(SlPlaced);
			if (TpPlaced != null) ret.Add(TpPlaced);
			return ret; 
		}
		public Alert CreateStopLoss_fromPositionPrototype(Position position) {
			string msig = " //CreateStopLoss_fromPositionPrototype(" + position + ")";
			//if (this.executor.checkPrototypeAlreadyPlaced(position)) return;
			PositionPrototype proto = position.Prototype;
			if (proto.StopLoss_negativeOffset > 0) {
				string msg = "PROTOTYPE_STOPLOSS_NOT_CREATED STOPLOSS_OFFSET_MUST_BE_NEGATIVE(FOR_STOP_LOSS))_OR_ZERO(FOR_STOP)";
				// DONT_SCREAM_SO_MUCH	throw new Exception(msg);
				// ALLOW_POSITIVE_OFFSET_FROM_PRICE_ENTRY_TO_TIGHTEN_SL_ACROSS_PRICE_ENTRY_LEVEL return null;
			}

			MarketLimitStop simpleStopIfActivationZero = (proto.StopLossActivation_negativeOffset == 0) ? MarketLimitStop.Stop : MarketLimitStop.StopLimit;

			if (proto.StopLoss_negativeOffset == 0) {
				string msg = this.reasonWhy_newStopLossOffset_doesntMakeSense(position, proto.StopLoss_negativeOffset, simpleStopIfActivationZero);
				if (String.IsNullOrEmpty(msg) == false) {
					string msg2 = "What should Activator do with sense-less proto.StopLossNegativeOffset[" + proto.StopLoss_negativeOffset + "], ";
					Assembler.PopupException(msg2 + msg + msig);
					throw new Exception(msg2 + msg + msig);
				}
			}

			if (proto.PriceEntry == 0) {
				string msg = "POSITION_PROTOTYPE_SUPPORTS_MARKET_ENTRY";
				proto.PriceEntryAbsorb(position.EntryFilled_price);
			}
			Alert alertStopLoss = executor.SellOrCover_alertCreate_dontRegisterInNew_prototypeActivator (
				this.executor.Bars.BarStreaming_nullUnsafe,
				position, proto.PriceStopLoss,
				proto.SignalStopLoss + "protoStopLossExit:" + proto.StopLossActivation_negativeOffset
					+ "@" + proto.StopLoss_negativeOffset + " for " + position.EntrySignal,
				MarketConverter.ExitDirectionFromLongShort(proto.LongShort),
				simpleStopIfActivationZero);
			if (alertStopLoss == null) {
				string msg = "alertStopLoss should NOT be null";
				Assembler.PopupException(msg + msig);
				throw new Exception(msg + msig);
			}
			alertStopLoss.PriceStopLimitActivation = 0;
			if (proto.StopLossActivation_negativeOffset < 0) alertStopLoss.PriceStopLimitActivation = proto.PriceStopLossActivation;
			if (proto.StopLossAlert_forMoveAndAnnihilation != null && this.executor.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing == false) {
				string msg = "CLEANUP: I was trying to catch MoveStopLoss::if(proto.StopLossAlertForAnnihilation==null)"
					+ " so I thought there is a new prototype assigned to a position,"
					+ " since we never put null directly proto.StopLossAlertForAnnihilation";
				Assembler.PopupException(msg + msig);
				throw new Exception(msg + msig);
			}

			proto.StopLossAlert_forMoveAndAnnihilation = alertStopLoss;
			return alertStopLoss;
		}
		public Alert CreateTakeProfit_fromPositionPrototype(Position position) {
			string msig = " //CreateTakeProfit_fromPositionPrototype(" + position + ")";
			PositionPrototype proto = position.Prototype;
			if (proto.TakeProfit_positiveOffset <= 0) {
				string msg = "PROTOTYPE_TAKEPROFIT_NOT_CREATED PROTOTYPE_TAKEPROFIT_OFFSET_MUST_BE_POSITIVE_NON_ZERO";
				// DONT_SCREAM_SO_MUCH	throw new Exception(msg);
				return null;
			}

			Alert alertTakeProfit = executor.SellOrCover_alertCreate_dontRegisterInNew_prototypeActivator (
				this.executor.Bars.BarStreaming_nullUnsafe,
				position, proto.PriceTakeProfit,
				proto.SignalTakeProfit + "protoTakeProfitExit:" + proto.TakeProfit_positiveOffset
					+ "@" + proto.PriceTakeProfit + " for " + position.EntrySignal,
				MarketConverter.ExitDirectionFromLongShort(position.Prototype.LongShort),
				MarketLimitStop.Limit);
			if (alertTakeProfit == null) {
				string msg = "alertTakeProfit should NOT be null";
				Assembler.PopupException(msg + msig);
				throw new Exception(msg + msig);
			}
			proto.TakeProfitAlert_forMoveAndAnnihilation = alertTakeProfit;
			return alertTakeProfit;
		}

		//void checkThrow_placingProto_makesSense(PositionPrototype proto, string msig_invoker) {
		//    string msg = this.reasonWhy_placingProto_doesntMakeSense(proto);
		//    if (String.IsNullOrEmpty(msg) == false) {
		//        Assembler.PopupException(msg + msig_invoker);
		//        throw new Exception(msg + msig_invoker);
		//    }
		//}
		string reasonWhy_placingProto_doesntMakeSense(PositionPrototype proto, bool internalCallee = false) {
			double lastPrice = executor.DataSource_fromBars.StreamingAdapter.StreamingDataSnapshot.GetPriceForMarketOrder_notAligned_fromQuoteLast_NOT_RELIABLE(proto.Symbol);
			Quote quote = executor.DataSource_fromBars.StreamingAdapter.StreamingDataSnapshot.GetQuoteLast_forSymbol_nullUnsafe(proto.Symbol);
			double priceBestBidAsk = executor.DataSource_fromBars.StreamingAdapter.StreamingDataSnapshot.GetBidOrAsk_forDirection_fromQuoteLast(proto.Symbol, proto.LongShort);
			bool willBeExecutedImmediately = false;
			MarketLimitStop planningEntryUsing = MarketConverter.EntryMarketLimitStopFromDirection(
				this.executor.Bars.BarStreaming_nullUnsafeCloneReadonly.Close, proto.PriceEntry, proto.LongShort);

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
	}
}
