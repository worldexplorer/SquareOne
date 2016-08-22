using System;

using Newtonsoft.Json;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Execution {
	public partial class Position {
		[JsonIgnore]	public	double	NetProfit { get {
				double netProfit = this.GrossProfit - this.EntryFilled_commission;
				if (this.ExitFilledBarIndex != -1) netProfit -= this.ExitFilled_commission;
				return netProfit;
			} }
		[JsonIgnore]	public	double	GrossProfit { get { return this.Shares * this.DistanceInPoints * this.symbolInfo.Point2Dollar; } }
		[JsonIgnore]	public	double	DistanceInPoints { get {
				return (this.PositionLongShort == PositionLongShort.Long)
					? this.ExitFilled_orBarsClose_forOpenPositions - this.EntryFilled_price
					: this.EntryFilled_price - this.ExitFilled_orBarsClose_forOpenPositions;
			} }
//		public double DistanceInPointsNoSlippage { get {
//				return (this.PositionLongShort == PositionLongShort.Long)
//					? this.ExitOrCurrentPriceNoSlippage - this.EntryPriceNoSlippage
//					: this.EntryPriceNoSlippage - this.ExitOrCurrentPriceNoSlippage;
//			} }
//		public double NetProfitNoSlippage { get {
//				if (this.Bars == null) return -999999;
//				double netProfitNoSlippage = this.NetProfit;
//				if (this.ExitFilledBarIndex != -1) netProfitNoSlippage -= this.ExitFilledCommission;
//				return netProfitNoSlippage;
//			} }
		[JsonIgnore]	public	double	PositionCost { get { return this.Shares * this.EntryFilled_price * this.symbolInfo.Point2Dollar; } }
		[JsonIgnore]	public	double	NetProfitPercent { get {
				if (this.Bars == null) return -999999;
				return 100.0 * this.NetProfit / this.PositionCost;
			} }
		[JsonIgnore]	public	int		BarsHeld { get {
				if (this.Bars == null) return -999999;
				int barsHeld;
				if (this.ExitFilledBarIndex == -1) {
					barsHeld = this.Bars.Count - 1 - this.EntryFilledBarIndex;
				} else {
					barsHeld = this.ExitFilledBarIndex - this.EntryFilledBarIndex + 1;
				}
				if (barsHeld < 1) barsHeld = 1;
				return barsHeld;
			} }
		[JsonIgnore]	public	double	ProfitPerBar { get { return this.NetProfit / (double)this.BarsHeld;} }
		
		[JsonIgnore]			int		mfeUptoBarIndex;
		[JsonIgnore]			double	mfeUptoValue;
		[JsonIgnore]	public	double	MFE { get {	// will be always positive
				if (this.Bars == null) return -999999;
				if (this.EntryFilledBarIndex == -1) return -999998;
				if (this.Bars.Count <= 0) return -999997;
				int barIndexExitOrStreaming = (this.ExitFilledBarIndex != -1) ? this.ExitFilledBarIndex : this.Bars.Count - 1;
				//v1 if (barIndexExitOrStreaming == this.mfeUptoBarIndex) return this.mfeUptoValue;
				//V2 if (this.ExitFilledBarIndex == -1) return this.mfeUptoValue;
				
				double mfe = 0;
				int mfeBarIndex = -1;
				for (int i = this.EntryFilledBarIndex; i <= barIndexExitOrStreaming; i++) {
					Bar bar = this.Bars[i];
					if (bar == null) throw new Exception("POSITION.MFE_BAR_BETWEEN_ENTRY_AND_EXIT_IS_NULL " + this.ToString());
					double mfeAtBar = (this.PositionLongShort == PositionLongShort.Long)
						? bar.High - this.EntryFilled_price : this.EntryFilled_price - bar.Low;
					if (mfeAtBar < 0) continue;
					if (mfe < mfeAtBar) {
						mfe = mfeAtBar;
						mfeBarIndex = i;
					}
				}
				mfe *= this.Shares;
				mfe *= this.symbolInfo.Point2Dollar;
				//mfe -= this.EntryFilledCommission;
				this.mfeUptoBarIndex = barIndexExitOrStreaming; 
				this.mfeUptoValue = mfe;
				return mfe;
			} }
		[JsonIgnore]	public	double	MFEPercent { get {
				if (this.Bars == null) return -999999;
				return 100.0 * this.MFE / this.PositionCost;
			} }
		
		[JsonIgnore]			int		maeUptoBarIndex;
		[JsonIgnore]			double	maeUptoValue;
		[JsonIgnore]	public	double	MAE { get {	// will be always negative
				if (this.Bars == null) return -999999;
				if (this.EntryFilledBarIndex == -1) return -999999;
				if (this.Bars.Count <= 0) return -9999998;
				int barExitOrStreaming = (this.ExitFilledBarIndex != -1) ? this.ExitFilledBarIndex : this.Bars.Count - 1;
				//v1 if (barExitOrStreaming == this.maeUptoBarIndex) return this.maeUptoValue;
				//V2 if (this.ExitFilledBarIndex == -1) return this.maeUptoValue;
				
				double mae = 0;
				int maeBarIndex = -1;
				for (int i = this.EntryFilledBarIndex; i <= barExitOrStreaming; i++) {
					Bar bar = this.Bars[i];
					if (bar == null) throw new Exception("POSITION.MAE_BAR_BETWEEN_ENTRY_AND_EXIT_IS_NULL " + this.ToString());
					double maeAtBar = (this.PositionLongShort == PositionLongShort.Long)
						? bar.Low - this.EntryFilled_price : this.EntryFilled_price - bar.High;
					if (maeAtBar > 0) continue;
					if (mae > maeAtBar) {
						mae = maeAtBar;
						maeBarIndex = i;
					}
				}
				mae *= this.Shares;
				mae *= this.symbolInfo.Point2Dollar;
				//mae += this.EntryFilledCommission;
				this.maeUptoBarIndex = barExitOrStreaming; 
				this.maeUptoValue = mae;
				return mae;
			} }
		[JsonIgnore]	public	double	MAEPercent { get {
				if (this.Bars == null) return -999999;
				return 100.0 * this.MAE / this.PositionCost;
			} }
	}
}