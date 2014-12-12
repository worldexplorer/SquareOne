using System;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Execution {
	public partial class Position {
		public double NetProfit { get {
				double netProfit = this.GrossProfit - this.EntryFilledCommission;
				if (this.ExitFilledBarIndex != -1) netProfit -= this.ExitFilledCommission;
				return netProfit;
			} }
		public double GrossProfit { get { return this.Shares * this.DistanceInPoints * this.Bars.SymbolInfo.Point2Dollar; } }
		public double DistanceInPoints { get {
				return (this.PositionLongShort == PositionLongShort.Long)
					? this.ExitOrStreamingPrice - this.EntryFilledPrice
					: this.EntryFilledPrice - this.ExitOrStreamingPrice;;
			} }
		public double DistanceInPointsNoSlippage { get {
				return (this.PositionLongShort == PositionLongShort.Long)
					? this.ExitOrCurrentPriceNoSlippage - this.EntryPriceNoSlippage
					: this.EntryPriceNoSlippage - this.ExitOrCurrentPriceNoSlippage;
			} }
		public double NetProfitNoSlippage { get {
				if (this.Bars == null) return -999999;
				double netProfitNoSlippage = this.Shares * this.DistanceInPointsNoSlippage * this.Bars.SymbolInfo.Point2Dollar;
				netProfitNoSlippage -= this.EntryFilledCommission;
				if (this.ExitFilledBarIndex != -1) netProfitNoSlippage -= this.ExitFilledCommission;
				return netProfitNoSlippage;
			} }
		public double PositionCost { get { return this.Shares * this.EntryFilledPrice * this.Bars.SymbolInfo.Point2Dollar; } }
		public double NetProfitPercent { get {
				if (this.Bars == null) return -999999;
				return 100.0 * this.NetProfit / this.PositionCost;
			} }
		public int BarsHeld { get {
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
		public double ProfitPerBar { get { return this.NetProfit / (double)this.BarsHeld;} }
		
		int mfeUptoBarIndex;
		double mfeUptoValue;
		public double MFE {	// will be always positive
			get {
				if (this.Bars == null) return -999999;
				if (this.EntryFilledBarIndex == -1) return -999998;
				if (this.Bars.Count <= 0) return -999997;
				int barIndexExitOrStreaming = (this.ExitFilledBarIndex != -1) ? this.ExitFilledBarIndex : this.Bars.Count - 1;
				//v1 if (barIndexExitOrStreaming == this.mfeUptoBarIndex) return this.mfeUptoValue;
				if (this.ExitFilledBarIndex == -1) return this.mfeUptoValue;
				
				double mfe = 0;
				int mfeBarIndex = -1;
				for (int i = this.EntryFilledBarIndex; i <= barIndexExitOrStreaming; i++) {
					Bar bar = this.Bars[i];
					if (bar == null) throw new Exception("POSITION.MFE_BAR_BETWEEN_ENTRY_AND_EXIT_IS_NULL");
					double mfeAtBar = (this.PositionLongShort == PositionLongShort.Long)
						? bar.High - this.EntryFilledPrice : this.EntryFilledPrice - bar.Low;
					if (mfeAtBar < 0) continue;
					if (mfe < mfeAtBar) {
						mfe = mfeAtBar;
						mfeBarIndex = i;
					}
				}
				mfe *= this.Shares;
				mfe *= this.Bars.SymbolInfo.Point2Dollar;
				mfe -= this.EntryFilledCommission;
				this.mfeUptoBarIndex = barIndexExitOrStreaming; 
				this.mfeUptoValue = mfe;
				return mfe;
			}
		}
		public double MFEPercent { get {
				if (this.Bars == null) return -999999;
				return 100.0 * this.MFE / this.PositionCost;
			} }
		
		int maeUptoBarIndex;
		double maeUptoValue;
		public double MAE {	// will be always negative
			get {
				if (this.Bars == null) return -999999;
				if (this.EntryFilledBarIndex == -1) return -999999;
				if (this.Bars.Count <= 0) return -9999998;
				int barExitOrStreaming = (this.ExitFilledBarIndex != -1) ? this.ExitFilledBarIndex : this.Bars.Count - 1;
				//v1 if (barExitOrStreaming == this.maeUptoBarIndex) return this.maeUptoValue;
				if (this.ExitFilledBarIndex == -1) return this.maeUptoValue;
				
				double mae = 0;
				int maeBarIndex = -1;
				for (int i = this.EntryFilledBarIndex; i <= barExitOrStreaming; i++) {
					Bar bar = this.Bars[i];
					if (bar == null) throw new Exception("POSITION.MAE_BAR_BETWEEN_ENTRY_AND_EXIT_IS_NULL");
					double maeAtBar = (this.PositionLongShort == PositionLongShort.Long)
						? bar.Low - this.EntryFilledPrice : this.EntryFilledPrice - bar.High;
					if (maeAtBar > 0) continue;
					if (mae > maeAtBar) {
						mae = maeAtBar;
						maeBarIndex = i;
					}
				}
				mae *= this.Shares;
				mae *= this.Bars.SymbolInfo.Point2Dollar;
				mae += this.EntryFilledCommission;
				this.maeUptoBarIndex = barExitOrStreaming; 
				this.maeUptoValue = mae;
				return mae;
			}
		}
		public double MAEPercent { get {
				if (this.Bars == null) return -999999;
				return 100.0 * this.MAE / this.PositionCost;
			} }
	}
}