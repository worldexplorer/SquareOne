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
			base.Name = "BacktestBrokerAdapter";
			this.BacktestMarketsim			= new BacktestMarketsim(this);
			base.AccountAutoPropagate		= new Account("BACKTEST_ACCOUNT", -1000);
			base.AccountAutoPropagate.Initialize(this);
		}

		internal void InitializeBacktestBroker(ScriptExecutor scriptExecutor) {
			this.ScriptExecutor = scriptExecutor;
			this.BacktestMarketsim.Initialize();
		}

		public override void MoveStopLossOverrideable(PositionPrototype proto, double newActivationOffset, double newStopLossNegativeOffset) {
			this.BacktestMarketsim.SimulateStopLossMoved(proto.StopLossAlertForAnnihilation);
		}
		public override bool AnnihilateCounterpartyAlert(Alert alert) {
			return this.BacktestMarketsim.AnnihilateCounterpartyAlert(alert);
		}
		public override void AlertKillPending(Alert alert) {
			this.BacktestMarketsim.SimulateAlertKillPending(alert);
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