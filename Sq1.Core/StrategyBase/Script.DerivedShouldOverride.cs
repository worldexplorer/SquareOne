using System;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Streaming;

namespace Sq1.Core.StrategyBase {
	public abstract partial class Script {
		// REASON_TO_EXIST: methods to override in derived strategy
		public virtual void InitializeBacktest() {
		}
		public virtual void OnNewQuoteOfStreamingBar_callback(Quote quoteNewArrived) {
		}
		public virtual void OnBarStaticLastFormed_whileStreamingBarWithOneQuoteAlreadyAppended_callback(Bar barNewStaticArrived) {
			string msg = "SCRIPT_DERIVED_MUST_IMPLEMENT Script[" + this.GetType().FullName + "]: public override void OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(Bar)";
			throw new Exception(msg);
		}
		public virtual void OnLevelTwoChanged_noNewQuote_callback(LevelTwoFrozen levelTwoFrozen) {
		}

		public virtual void OnAlertFilled_callback(Alert alertFilled) {
		}
		public virtual void OnAlertKilled_callback(Alert alertKilled) {
		}
		public virtual void OnAlertNotSubmitted_callback(Alert alertNotSubmitted, int barNotSubmittedRelno) {
		}
		public virtual void OnPositionOpened_callback(Position positionOpened) {
		}
		public virtual void OnPositionOpened_prototypeSlTpPlaced_callback(Position positionOpenedByPrototype) {
		}
		public virtual void OnPositionClosed_callback(Position positionClosed) {
		}
		//public virtual void ExecuteOnStopLossNegativeOffsetUpdateActivationSucceeded(Position position, PositionPrototype proto) {
		//}
		//public virtual void ExecuteOnStopLossNegativeOffsetUpdateActivationFailed(Position position, PositionPrototype proto) {
		//}

		public virtual void OnStreamingTriggeringScript_turnedOn_callback() {
		}
		public virtual void OnStreamingTriggeringScript_turnedOff_callback() {
		}
		public virtual void OnStrategyEmittingOrders_turnedOn_callback() {
		}
		public virtual void OnStrategyEmittingOrders_turnedOff_callback() {
		}
	}
}
