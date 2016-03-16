using System;

using Newtonsoft.Json;

using Sq1.Core.Accounting;
using Sq1.Core.Broker;
using Sq1.Core.Support;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;
using Sq1.Core.Livesim;

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

		internal void InitializeBacktestBroker(ScriptExecutor scriptExecutor) {
			this.ScriptExecutor = scriptExecutor;
			this.BacktestMarketsim.Initialize();
		}

		public override void OrderMoveExisting_stopLoss_overrideable(PositionPrototype proto, double newActivationOffset, double newStopLossNegativeOffset) {
			this.BacktestMarketsim.StopLoss_simulateMoved(proto.StopLossAlert_forMoveAndAnnihilation);
		}
		public override void OrderMoveExisting_takeProfit_overrideable(PositionPrototype proto, double newTakeProfit_positiveOffset) {
			this.BacktestMarketsim.TakeProfit_simulateMoved(proto.TakeProfitAlert_forMoveAndAnnihilation);
		}


		public override void Order_kill_dispatcher(Order victimOrder) {
		    throw new Exception("please override BrokerAdapter::Order_kill() for BrokerAdapter.Name=[" + Name + "]");
		}
		//public override void Order_killPending_withoutKiller(Order victimOrder) {
		//    throw new Exception("please override BrokerAdapter::Order_killPending_withoutKiller() for BrokerAdapter.Name=[" + Name + "]");
		//}
		public override void Order_killPending_usingKiller(Order killerOrder) {
		    throw new Exception("please override BrokerAdapter::Order_killPending_usingKiller() for BrokerAdapter.Name=[" + Name + "]");
		}


		public override bool AlertCounterparty_annihilate(Alert alert) {
			return this.BacktestMarketsim.AlertCounterparty_annihilate(alert);
		}
		public override void AlertPending_kill(Alert alert) {
			this.BacktestMarketsim.AlertPending_simulateKill(alert);
		}

		public bool ImLivesimBroker { get {
			bool ret = this is LivesimBroker;
			if (this.ScriptExecutor != null) {	// while creating a new DataSource, we take Dummy who doesn't have anything
				if (ret != this.ScriptExecutor.BacktesterOrLivesimulator.ImLivesimulator) {
					string msg = "FOR_LIVESIM__MUST_BE_DERIVED_FROM_LivesimBroker [" + this + "]";	// PARANOID_BUT_JUSTFIFIED
					Assembler.PopupException(msg);
				}
			}
			return ret;
		} }
	}
}