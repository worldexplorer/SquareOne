using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Sq1.Core.Accounting;
using Sq1.Core.Broker;
using Sq1.Core.Support;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Backtesting {
	[SkipInstantiationAt(Startup = true)]
	public class BacktestBroker : BrokerAdapter {
		[JsonIgnore]	public	BacktestMarketsim		BacktestMarketsim	{ get; protected set; }
		[JsonIgnore]	public	ScriptExecutor			ScriptExecutor		{ get; private set; }

		public BacktestBroker(string reasonToExist) : base(reasonToExist) {
			base.Name						= "BacktestBrokerAdapter";
			this.BacktestMarketsim			= new BacktestMarketsim(this);
			base.AccountAutoPropagate		= new Account("BACKTEST_ACCOUNT", -1000);
			base.AccountAutoPropagate.Initialize(this);
		}

		internal void InitializeMarketsim(ScriptExecutor scriptExecutor) {
			this.ScriptExecutor = scriptExecutor;
			this.BacktestMarketsim.Initialize(this.ScriptExecutor);
		}

		public override void OrderMoveExisting_stopLoss_overrideable(PositionPrototype proto, double newActivationOffset, double newStopLossNegativeOffset) {
			this.BacktestMarketsim.StopLoss_simulateMoved(proto.StopLossAlert_forMoveAndAnnihilation);
		}
		public override void OrderMoveExisting_takeProfit_overrideable(PositionPrototype proto, double newTakeProfit_positiveOffset) {
			this.BacktestMarketsim.TakeProfit_simulateMoved(proto.TakeProfitAlert_forMoveAndAnnihilation);
		}


		//public override void Order_kill_dispatcher(Order victimOrder) {
		//    throw new Exception("please override BrokerAdapter::Order_kill() for BrokerAdapter.Name=[" + Name + "]");
		//}
		//public override void Order_killPending_withoutKiller(Order victimOrder) {
		//    throw new Exception("please override BrokerAdapter::Order_killPending_withoutKiller() for BrokerAdapter.Name=[" + Name + "]");
		//}
		public override void Order_killPending_usingKiller(Order killerOrder) {
		    throw new Exception("please override BrokerAdapter::Order_killPending_usingKiller() for BrokerAdapter.Name=[" + Name + "]");
		}


		public override bool AlertCounterparty_annihilate(Alert alertCounterparty_toAnnihilate) {
			return this.BacktestMarketsim.AlertCounterparty_annihilate(alertCounterparty_toAnnihilate);
		}
		public override int AlertPendings_kill(List<Alert> alerts2kill_afterScript_onQuote_onBar) {
			int emitted = alerts2kill_afterScript_onQuote_onBar.Count;
			foreach (Alert alert2kill in alerts2kill_afterScript_onQuote_onBar) {
				this.BacktestMarketsim.AlertPending_simulateKill(alert2kill);
			}
			return emitted;
		}

	}
}