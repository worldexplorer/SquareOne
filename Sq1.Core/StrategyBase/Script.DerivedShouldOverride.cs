using System;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.StrategyBase {
	public abstract partial class Script {
		// REASON_TO_EXIST: methods to override in derived strategy
		public virtual void InitializeBacktest() {
		}
		public virtual void OnNewQuoteOfStreamingBarCallback(Quote quoteNewArrived) {
		}
		public virtual void OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(Bar barNewStaticArrived) {
			string msg = "SCRIPT_DERIVED_MUST_IMPLEMENT Script[" + this.GetType().FullName + "]: public override void OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(Bar)";
			throw new Exception(msg);
		}
		public virtual void OnAlertFilledCallback(Alert alertFilled) {
		}
		public virtual void OnAlertKilledCallback(Alert alertKilled) {
		}
		public virtual void OnAlertNotSubmittedCallback(Alert alertNotSubmitted, int barNotSubmittedRelno) {
		}
		public virtual void OnPositionOpenedCallback(Position positionOpened) {
		}
		public virtual void OnPositionOpenedPrototypeSlTpPlacedCallback(Position positionOpenedByPrototype) {
		}
		public virtual void OnPositionClosedCallback(Position positionClosed) {
		}
		//public virtual void ExecuteOnStopLossNegativeOffsetUpdateActivationSucceeded(Position position, PositionPrototype proto) {
		//}
		//public virtual void ExecuteOnStopLossNegativeOffsetUpdateActivationFailed(Position position, PositionPrototype proto) {
		//}
	}
}
