using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Sq1.Core.Execution {
	public class PositionPrototype {
		public readonly string Symbol;
		public readonly PositionLongShort LongShort;
		public double PriceEntry { get; protected set; }
		public double StopLossNegativeOffset;	// { get; protected set; }
		public double StopLossActivationNegativeOffset { get; protected set; }
		public double TakeProfitPositiveOffset { get; protected set; }
		public double PriceStopLossActivation { get { return this.OffsetToPrice(this.StopLossActivationNegativeOffset); } }
		public double PriceStopLoss { get { return this.OffsetToPrice(this.StopLossNegativeOffset); } }
		public double PriceTakeProfit { get { return this.OffsetToPrice(this.TakeProfitPositiveOffset); } }

		public Alert StopLossAlertForAnnihilation;
		public Alert TakeProfitAlertForAnnihilation;
		
		public string SignalEntry = "";
		public string SignalStopLoss = "";
		public string SignalTakeProfit = "";

		public PositionPrototype(string symbol, PositionLongShort positionLongShort, double priceEntry,
				double takeProfitPositiveOffset,
				double stopLossNegativeOffset, double stopLossActivationNegativeOffset = 0,
				string signalEntry = "", string signalStopLoss = "", string signalTakeProfit = "") {

			this.Symbol = symbol;
			this.LongShort = positionLongShort;
			this.PriceEntry = priceEntry;
			this.SetNewTakeProfitOffset(takeProfitPositiveOffset);
			this.SetNewStopLossOffsets(stopLossNegativeOffset, stopLossActivationNegativeOffset);
			
			if (string.IsNullOrEmpty(signalEntry) == false)			this.SignalEntry = signalEntry;
			if (string.IsNullOrEmpty(signalStopLoss) == false)		this.SignalStopLoss = signalStopLoss;
			if (string.IsNullOrEmpty(signalTakeProfit) == false)	this.SignalTakeProfit = signalTakeProfit;
		}

		public void SetNewTakeProfitOffset(double newTakeProfitPositiveOffset) {
			this.checkTPOffsetThrowBeforeAbsorbing(newTakeProfitPositiveOffset);
			this.TakeProfitPositiveOffset = newTakeProfitPositiveOffset;
		}
		public void SetNewStopLossOffsets(double newStopLossNegativeOffset, double stopLossActivationNegativeOffset) {
			this.checkSLOffsetsThrowBeforeAbsorbing(newStopLossNegativeOffset, stopLossActivationNegativeOffset);
			this.StopLossActivationNegativeOffset = stopLossActivationNegativeOffset;
			this.StopLossNegativeOffset = newStopLossNegativeOffset;
		}
		public void checkTPOffsetThrowBeforeAbsorbing(double takeProfitPositiveOffset) {
			if (takeProfitPositiveOffset < 0) {
				string msg = "WRONG USAGE OF PositionPrototype.ctor()!"
					+ " PositionPrototype should contain positive offset for TakeProfit";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
		}
		public void checkSLOffsetsThrowBeforeAbsorbing(double stopLossNegativeOffset, double stopLossActivationNegativeOffset) {
			if (stopLossNegativeOffset > 0) {
				string msg = "WRONG USAGE OF PositionPrototype.ctor()!"
					+ " PositionPrototype should contain negative offset for StopLoss";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			if (stopLossActivationNegativeOffset > 0) {
				string msg = "WRONG USAGE OF PositionPrototype.ctor()!"
					+ " PositionPrototype should contain negative offset for StopLossActivation";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
			if (stopLossActivationNegativeOffset == 0) return;
			if (stopLossActivationNegativeOffset <= stopLossNegativeOffset) {
				string msg = "USAGE: PositionPrototype(Long, Entry=100, TP=150, SL=-50, SLa=-40)"
					+ "; StopLossActivation[" + stopLossActivationNegativeOffset + "]"
					+ " should be >= StopLoss[" + stopLossNegativeOffset + "]";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
		}
		//public void checkThrowAbsorbed() {
		//	this.checkSlOffsetsThrowBeforeAbsorbing(this.TakeProfitPositiveOffset, this.StopLossNegativeOffset, this.StopLossActivationNegativeOffset);
		//}
		//internal void StopLossNegativeSameActivationDistanceOffsetSafeUpdate(double newStopLossNegativeOffset) {
		//	double newActivationOffset = this.CalcActivationOffsetForNewClosing(newStopLossNegativeOffset);
		//	this.checkSlOffsetsThrowBeforeAbsorbing(this.TakeProfitPositiveOffset, newStopLossNegativeOffset, newActivationOffset);
		//	this.StopLossNegativeOffset = newStopLossNegativeOffset;
		//}

		public double CalcActivationOffsetForNewClosing(double newStopLossNegativeOffset) {
			// for a long position, activation price is above closing price
			double currentDistance = this.StopLossActivationNegativeOffset - this.StopLossNegativeOffset;
			return newStopLossNegativeOffset + currentDistance;
		}

		public double CalcStopLossDifference(double newStopLossNegativeOffset) {
			return newStopLossNegativeOffset - this.StopLossNegativeOffset;
		}

		public double OffsetToPrice(double newActivationOffset) {
			double priceFromOffset = 0;
			switch (this.LongShort) {
				case PositionLongShort.Long:
					priceFromOffset = this.PriceEntry + newActivationOffset;
					break;
				case PositionLongShort.Short:
					priceFromOffset = this.PriceEntry - newActivationOffset;
					break;
				default:
					string msg = "OffsetToPrice(): No PositionLongShort[" + this.LongShort + "] handler "
						+ "; must be one of those: Long/Short";
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception(msg);
			}
			return priceFromOffset;
		}

		public double PriceToOffset(double newPrice) {
			return this.PriceEntry - newPrice;
		}
		public bool IsIdenticalTo(PositionPrototype proto) {
			return this.LongShort == proto.LongShort
				&& this.PriceEntry == proto.PriceEntry
				&& this.TakeProfitPositiveOffset == proto.TakeProfitPositiveOffset
				&& this.StopLossNegativeOffset == proto.StopLossNegativeOffset
				&& this.StopLossActivationNegativeOffset == proto.StopLossActivationNegativeOffset;
		}
		public override string ToString() {
//			return this.LongShort + " Entry[" + this.PriceEntry + "]TP[" + this.TakeProfitPositiveOffset + "]SL["
//				+ this.StopLossNegativeOffset + "]SLA[" + this.StopLossActivationNegativeOffset + "]";
			StringBuilder msg = new StringBuilder();
			msg.Append(this.LongShort);
			msg.Append(" Entry[");
			msg.Append(this.PriceEntry);
			msg.Append("]TP[");
			msg.Append(this.TakeProfitPositiveOffset);
			msg.Append("]SL[");
			msg.Append(this.StopLossNegativeOffset);
			msg.Append("]SLA[");
			msg.Append(this.StopLossActivationNegativeOffset + "]");
			return msg.ToString();
		}

		public void PriceEntryAbsorb(double priceEntryAlertFilled) {
			this.PriceEntry = priceEntryAlertFilled;
		}
	}
}