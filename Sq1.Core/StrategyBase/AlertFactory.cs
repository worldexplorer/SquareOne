using System;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.StrategyBase {
	public class AlertFactory {
		ScriptExecutor executor;

		public AlertFactory(ScriptExecutor executor) {
			this.executor = executor;
		}

		public Alert AlertEntry_create(Bar entryBar,
			double priceLimitOrStop_zeroForMarket, double priceStopLimitActivation,
			string entrySignalName, Direction direction, MarketLimitStop entryMarketLimitStop) {
			this.checkThrow_entryBar_isValid(entryBar);

			double priceScript = priceLimitOrStop_zeroForMarket;
			double entryPriceScript = entryMarketLimitStop == MarketLimitStop.Market && priceLimitOrStop_zeroForMarket == 0
				? 0
				: entryBar.ParentBars.SymbolInfo.Alert_alignToPriceStep(priceScript, direction, entryMarketLimitStop);
			double shares = this.executor.PositionSizeCalculate(entryBar, entryPriceScript);

			Alert alertEntry = new Alert(entryBar, shares, entryPriceScript, priceStopLimitActivation,
				entrySignalName, direction, entryMarketLimitStop, this.executor.Strategy);

			return alertEntry;
		}

		public Alert AlertExit_create(Bar exitBar, Position position,
			double priceLimitOrStop_zeroForMarket, double priceStopLimitActivation,
			string signalName, Direction direction, MarketLimitStop exitMarketLimitStop) {

			this.checkThrow_entryBar_isValid(exitBar);
			this.checkThrow_positionToClose_isValid(position);

			double priceScript = priceLimitOrStop_zeroForMarket;
			double exitPriceScript = exitMarketLimitStop == MarketLimitStop.Market && priceLimitOrStop_zeroForMarket == 0
				? 0
				: exitBar.ParentBars.SymbolInfo.Alert_alignToPriceStep(priceScript, direction, exitMarketLimitStop);

			if (exitMarketLimitStop == MarketLimitStop.StopLimit && priceStopLimitActivation > 0) {
				// LAZY PriceLevelRoundingMode tighterToPriceEntry = position.IsLong && 
				priceStopLimitActivation = exitBar.ParentBars.SymbolInfo.AlignToPriceStep(priceStopLimitActivation);
			}
	
			Alert alertExit = new Alert(exitBar, position.Shares, exitPriceScript, priceStopLimitActivation,
									signalName, direction, exitMarketLimitStop, this.executor.Strategy);
			alertExit.PositionAffected = position;
			alertExit.PositionAffected.ExitAlertAttach(alertExit);
			if (position.Prototype != null) alertExit.PositionPrototype = position.Prototype;

			return alertExit;
		}

		void checkThrow_positionToClose_isValid(Position position) {
			if (position == null) {
				string msg = "position=null, can't close it!";
				throw new Exception(msg);
			}
			if (position.ExitNotFilledOrStreaming == false) {
				string msg = "position.Active=false, can't close it!";
				throw new Exception(msg);
			}
		}
		void checkThrow_entryBar_isValid(Bar entryBar) {
			//if (entryBar < this.executor.Bars.Count) {
			//	string msg = "use MarketSim for Backtest! MarketRealTime is for entryBars >= this.executor.Bars.Count";
			//	throw new Exception(msg);
			//	//this.executor.ThrowPopup(new Exception(msg));

			//}

			//if (entryBar > this.executor.Bars.Count) {
			//	string msg = "entryBar[" + entryBar + "] > Bars.Count[" + this.executor.Bars.Count + "]"
			//		+ " for [" + this.executor.Bars + "]"
			//		+ " Bars.StreamingBarSafeClone=[" + this.executor.Bars.StreamingBarCloneReadonly + "]"
			//		+ "; can't open any other position but on StreamingBar; positions postponed for tomorrow NYI";
			//	throw new Exception(msg);
			//	//this.executor.ThrowPopup(new Exception(msg));
			//}
			if (entryBar == null) {
				string msg = "entryBar == null";
				throw new Exception(msg);
			}
			if (entryBar.ParentBars == null) {
				string msg = "entryBar.ParentBars == null";
				throw new Exception(msg);
			}
			if (entryBar.ParentBars.SymbolInfo == null) {
				string msg = "entryBar.ParentBars.SymbolInfo == null";
				throw new Exception(msg);
			}
		}
	}
}
