using System;
using System.Diagnostics;
using System.Text;

using Newtonsoft.Json;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Execution {
	public class PositionPrototype {
		[JsonIgnore]	public	readonly	string Symbol;
		[JsonIgnore]	public	readonly	PositionLongShort LongShort;
		[JsonProperty]	public	double		PriceEntry									{ get; protected set; }

		[JsonProperty]	public	double		TakeProfit_priceEntryPositiveOffset			{ get; protected set; }
		[JsonProperty]	public	double		PriceTakeProfit								{ get; protected set; }

		[JsonProperty]	public	double		StopLoss_priceEntryNegativeOffset			{ get; protected set; }
		[JsonProperty]	public	double		StopLossActivation_priceEntryNegativeOffset	{ get; protected set; }
		[JsonProperty]	public	double		PriceStopLoss								{ get; protected set; }
		[JsonProperty]	public	double		PriceStopLossActivation						{ get; protected set; }

		[JsonProperty]	public	double		StopLossActivation_distanceFromStopLoss		{ get {
			return
				this.LongShort == PositionLongShort.Long
					? this.PriceStopLoss - this.PriceStopLossActivation
					: this.PriceStopLossActivation - this.PriceStopLoss;
		} }

		[JsonIgnore]	public	Alert		StopLossAlert_forMoveAndAnnihilation;
		[JsonIgnore]	public	Alert		TakeProfitAlert_forMoveAndAnnihilation;
		
		[JsonProperty]	public	string		SignalEntry = "";
		[JsonProperty]	public	string		SignalStopLoss = "";
		[JsonProperty]	public	string		SignalTakeProfit = "";
		[JsonIgnore]			SymbolInfo	symbolInfo;

		public PositionPrototype(string symbol, PositionLongShort positionLongShort, double priceEntry_mostLikelyZero,
				double takeProfit_priceEntryPositiveOffset,
				double stopLoss_priceEntryNegativeOffset, double stopLossActivation_stopLossNegativeOffset = 0,
				string signalEntry = "", string signalStopLoss = "", string signalTakeProfit = "") {

			this.Symbol = symbol;
			this.LongShort = positionLongShort;

			this.symbolInfo = Assembler.InstanceInitialized.RepositorySymbolInfos.FindSymbolInfo_nullUnsafe(this.Symbol);
			if (priceEntry_mostLikelyZero > 0) this.PriceEntryAbsorb_calculateTpSl(priceEntry_mostLikelyZero);

			this.TakeProfit_priceEntryPositiveOffset			= takeProfit_priceEntryPositiveOffset;
			this.StopLoss_priceEntryNegativeOffset				= stopLoss_priceEntryNegativeOffset;
			this.StopLossActivation_priceEntryNegativeOffset	= stopLossActivation_stopLossNegativeOffset;

			if (string.IsNullOrEmpty(signalEntry) == false)			this.SignalEntry = signalEntry;
			if (string.IsNullOrEmpty(signalStopLoss) == false)		this.SignalStopLoss = signalStopLoss;
			if (string.IsNullOrEmpty(signalTakeProfit) == false)	this.SignalTakeProfit = signalTakeProfit;
		}

		public void PriceEntryAbsorb_calculateTpSl(double priceEntry_afterAlertFilled) {
			if (priceEntry_afterAlertFilled == 0) {
				string msg = "STRATEGY_CREATED_PROTOTYPE__HOPING_TO_FILL_ENTRY_ALERT_FIRST__ABSORB_AND_RECALCULATE_AFTER_FILL";
				return;
			}

			this.PriceEntry = priceEntry_afterAlertFilled;
			this.Calculate_TakeProfitOffset();
			this.Calculate_StopLossOffsets();
		}

		public void Calculate_TakeProfitOffset(double newTakeProfit_positiveOffset = 0) {
			if (newTakeProfit_positiveOffset > 0) {
				this.checkTPOffset_throwBeforeAbsorbing(newTakeProfit_positiveOffset);
				this.TakeProfit_priceEntryPositiveOffset = newTakeProfit_positiveOffset;
			}
			this.PriceTakeProfit = this.OffsetToPriceEntry(this.TakeProfit_priceEntryPositiveOffset);

			if (this.symbolInfo == null) return;

			PriceLevelRoundingMode roundingMode_TakeProfit_tighterToEntry =
				this.LongShort == PositionLongShort.Long
					? PriceLevelRoundingMode.RoundDown
					: PriceLevelRoundingMode.RoundUp;
			this.PriceTakeProfit = this.symbolInfo.AlignToPriceStep(this.PriceTakeProfit, roundingMode_TakeProfit_tighterToEntry);
		}

		public void Calculate_StopLossOffsets(double newStopLoss_negativeOffset = 0, double newStopLossActivation_negativeOffset = 0) {
			if (newStopLoss_negativeOffset < 0 && newStopLossActivation_negativeOffset < 0) {
				this.checkSLOffsets_throwBeforeAbsorbing(newStopLoss_negativeOffset, newStopLossActivation_negativeOffset);
				this.StopLoss_priceEntryNegativeOffset				= newStopLoss_negativeOffset;
				this.StopLossActivation_priceEntryNegativeOffset	= newStopLossActivation_negativeOffset;
			}
			this.PriceStopLoss				= this.OffsetToPriceEntry(this.StopLoss_priceEntryNegativeOffset);
			this.PriceStopLossActivation	= this.OffsetToPriceEntry(-this.StopLossActivation_priceEntryNegativeOffset, this.PriceStopLoss);

			if (this.symbolInfo == null) return;

			PriceLevelRoundingMode roundingMode_StopLoss_tighterToEntry =
				this.LongShort == PositionLongShort.Long
					? PriceLevelRoundingMode.RoundUp
					: PriceLevelRoundingMode.RoundDown;
			this.PriceStopLoss				= this.symbolInfo.AlignToPriceStep(this.PriceStopLoss,				roundingMode_StopLoss_tighterToEntry);
			this.PriceStopLossActivation	= this.symbolInfo.AlignToPriceStep(this.PriceStopLossActivation,	roundingMode_StopLoss_tighterToEntry);
		}

		public void checkTPOffset_throwBeforeAbsorbing(double takeProfitPositiveOffset) {
			if (takeProfitPositiveOffset < 0) {
				string msg = "WRONG USAGE OF PositionPrototype.ctor()!"
					+ " PositionPrototype should contain positive offset for TakeProfit";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg);
			}
		}
		public void checkSLOffsets_throwBeforeAbsorbing(double stopLossNegativeOffset, double stopLossActivationNegativeOffset) {
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

		public double CalcActivationOffset_forNewClosing(double newStopLossNegativeOffset) {
			// for a long position, activation price is above closing price
			double currentDistance = this.StopLossActivation_priceEntryNegativeOffset - this.StopLoss_priceEntryNegativeOffset;
			return newStopLossNegativeOffset + currentDistance;
		}

		public double OffsetToPriceEntry(double offset, double priceEntry_orStopLoss = 0) {
			if (priceEntry_orStopLoss == 0) priceEntry_orStopLoss = this.PriceEntry;
			double price_withOffset = 0;

			switch (this.LongShort) {
				case PositionLongShort.Long:
					price_withOffset = priceEntry_orStopLoss + offset;
					break;

				case PositionLongShort.Short:
					price_withOffset = priceEntry_orStopLoss - offset;
					break;

				default:
					string msg = "OffsetToPrice(): No PositionLongShort[" + this.LongShort + "] handler "
						+ "; must be one of those: Long/Short";
					#if DEBUG
					Debugger.Break();
					#endif
					throw new Exception(msg);
			}

			return price_withOffset;
		}

		public bool IsIdenticalTo(PositionPrototype proto) {
			return this.LongShort == proto.LongShort
				&& this.PriceEntry == proto.PriceEntry
				&& this.TakeProfit_priceEntryPositiveOffset == proto.TakeProfit_priceEntryPositiveOffset
				&& this.StopLoss_priceEntryNegativeOffset == proto.StopLoss_priceEntryNegativeOffset
				&& this.StopLossActivation_priceEntryNegativeOffset == proto.StopLossActivation_priceEntryNegativeOffset;
		}

		public override string ToString() {
//			return this.LongShort + " Entry[" + this.PriceEntry + "]TP[" + this.TakeProfitPositiveOffset + "]SL["
//				+ this.StopLossNegativeOffset + "]SLA[" + this.StopLossActivationNegativeOffset + "]";
			StringBuilder msg = new StringBuilder();
			msg.Append(this.LongShort);
			msg.Append(" Entry[");
			msg.Append(this.PriceEntry);
			msg.Append("]TP[");
			msg.Append(this.TakeProfit_priceEntryPositiveOffset);
			msg.Append("]SL[");
			msg.Append(this.StopLoss_priceEntryNegativeOffset);
			msg.Append("]SLA[");
			msg.Append(this.StopLossActivation_priceEntryNegativeOffset + "]");
			return msg.ToString();
		}
	}
}