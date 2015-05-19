using System;

using Sq1.Core.DataTypes;

namespace Sq1.Widgets.SymbolEditor {
	public partial class SymbolEditorControl {
		void olvSymbolsCustomize() {
			this.olvcSymbol.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				return symbolInfo.Symbol;
			};
			this.olvcSymbolClass.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				return symbolInfo.SymbolClass;
			};
			this.olvcSecurityType.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				if (symbolInfo.MarketOrderAs == null) return "";
				return symbolInfo.SecurityType.ToString();
			};
			this.olvcPoint2Dollar.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				return symbolInfo.Point2Dollar.ToString();
			};
			this.olvcPriceStep.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				return symbolInfo.PriceStep.ToString();
			};
			this.olvcPriceDecimals.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				return symbolInfo.PriceDecimals.ToString();
			};
			this.olvcVolumeDecimals.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				return symbolInfo.VolumeDecimals.ToString();
			};
			this.olvcSameBarPolarCloseThenOpen.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				return symbolInfo.SameBarPolarCloseThenOpen.ToString();
			};
			this.olvcSequencedOpeningAfterClosedDelayMillis.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				return symbolInfo.SequencedOpeningAfterClosedDelayMillis.ToString();
			};
			this.olvcEmergencyCloseDelayMillis.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				return symbolInfo.EmergencyCloseDelayMillis.ToString();
			};
			this.olvcEmergencyCloseAttemptsMax.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				return symbolInfo.EmergencyCloseAttemptsMax.ToString();
			};
			this.olvcReSubmitRejected.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				return symbolInfo.ReSubmitRejected.ToString();
			};
			this.olvcReSubmittedUsesNextSlippage.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				return symbolInfo.ReSubmittedUsesNextSlippage.ToString();
			};
			this.olvcUseFirstSlippageForBacktest.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				return symbolInfo.UseFirstSlippageForBacktest.ToString();
			};
			this.olvcSlippagesBuy.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				if (string.IsNullOrEmpty(symbolInfo.SlippagesBuy)) return "";
				return symbolInfo.SlippagesBuy.ToString();
			};
			this.olvcSlippagesSell.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				if (string.IsNullOrEmpty(symbolInfo.SlippagesBuy)) return "";
				return symbolInfo.SlippagesSell;
			};
			this.olvcMarketOrderAs.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				if (symbolInfo.MarketOrderAs == null) return "";
				return symbolInfo.MarketOrderAs.ToString();
			};
			this.olvcReplaceTidalWithCrossMarket.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				if (string.IsNullOrEmpty(symbolInfo.SlippagesBuy)) return "";
				return symbolInfo.ReplaceTidalWithCrossMarket.ToString();
			};
			this.olvcReplaceTidalMillis.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				if (string.IsNullOrEmpty(symbolInfo.SlippagesBuy)) return "";
				return symbolInfo.ReplaceTidalMillis.ToString();
			};
			this.olvcSimBugOutOfBarStopsFill.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				return symbolInfo.SimBugOutOfBarStopsFill.ToString();
			};
			this.olvcSimBugOutOfBarLimitsFill.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				return symbolInfo.SimBugOutOfBarLimitsFill.ToString();
			};
			this.olvcLeverageForFutures.AspectGetter = delegate(object o) {
				SymbolInfo symbolInfo = o as SymbolInfo;
				if (symbolInfo == null) return "olvcSymbol.AspectGetter: symbolInfo=null";
				return symbolInfo.LeverageForFutures.ToString();
			};
		}
	}
}
