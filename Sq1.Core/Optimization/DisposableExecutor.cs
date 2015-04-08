using System;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Execution;
using Sq1.Core.Backtesting;
using Sq1.Core.Optimization;
using Sq1.Core.Broker;
using Sq1.Core.Charting;
using Sq1.Core.StrategyBase;
using Sq1.Core.Indicators;
using Sq1.Core.Livesim;
using Sq1.Core.Support;

namespace Sq1.Core.Optimization {
	public class DisposableExecutor : ScriptExecutor, IDisposable  {
		// invoked once per DisposableExecutorsPool.ctor() { executorToBeSpawned = new DisposableExecutor(this.optimizer.Executor)}
		public DisposableExecutor(ScriptExecutor scriptExecutor) : base() {
			if (base.Livesimulator != null) {
				string msg = "I_WANTED_TO_AVOID_BASE_NORMAL_ScripExecutor.ctor()_AND_REPLACED_IT_WITH_DisposableExecutor()";
				//Assembler.PopupException(msg);
				//base.MarketLive		= null;
				base.Optimizer		= null;
				base.Livesimulator	= null;
				base.OrderProcessor	= null;
			}

			base.Optimizer = scriptExecutor.Optimizer;
			//NOPE_I_NEED_SEQUENCER_FOR_EXECUTOR_ETHALON WILL_BE_CLONED_FOR_EACH_TO_HAVE_FRESH_SCRIPT_AND_EMPTY_CURRENT_CONTEXT
			base.Strategy	= scriptExecutor.Strategy.CloneMinimalForEachThread_forEachDisposableExecutorInOptimizerPool();
			base.SetBars(scriptExecutor.Bars.SafeCopy_safeToUseOneCopyForAllDisposableExecutors);	// frozen, streamingBar detached

			// dummy empty one instance for all spawned
			this.ChartShadow = new ChartShadow();
			this.ChartShadow.SetExecutor(this);
		}

		DisposableExecutor(DisposableExecutor disposableExecutor) : base() {
			if (base.Livesimulator != null) {
				string msg = "I_WANTED_TO_AVOID_BASE_NORMAL_ScripExecutor.ctor()_AND_REPLACED_IT_WITH_DisposableExecutor()";
				//Assembler.PopupException(msg);
				//base.MarketLive		= null;
				base.Optimizer		= null;
				base.Livesimulator	= null;
				base.OrderProcessor	= null;
			}

			base.Optimizer = disposableExecutor.Optimizer;
			//NOPE_I_NEED_SEQUENCER_FOR_EXECUTOR_ETHALON WILL_BE_CLONED_FOR_EACH_TO_HAVE_FRESH_SCRIPT_AND_EMPTY_CURRENT_CONTEXT
			base.Strategy	= disposableExecutor.Strategy.CloneMinimalForEachThread_forEachDisposableExecutorInOptimizerPool();
			base.SetBars(disposableExecutor.Bars.SafeCopy_safeToUseOneCopyForAllDisposableExecutors);	// frozen, streamingBar detached

			// dummy empty one instance for all spawned
			this.ChartShadow = new ChartShadow();
			this.ChartShadow.SetExecutor(this);
		}

		// invoked many times in DisposableExecutorsPool.spawnToReachTotalNr(int threadsToUse) to spawn executorToBeSpawned and add to this.executorsIdlingNow
		internal DisposableExecutor SpawnEthalonForEachThread_forEachDisposableExecutorInOptimizerPool() {
			//v1 I_DONT_WANT_TO_RESET_AGAIN_base.Livesimulator=null
			DisposableExecutor clone = new DisposableExecutor(this);
			//v2 HOW_DO_YOU_CREATE_INDIVIDUAL_BACKTESTERS_THIS_WAY??? DisposableExecutor clone = (DisposableExecutor) base.MemberwiseClone();
			//ALREADY clone.Strategy = this.Strategy.CloneMinimalForEachThread_forEachDisposableExecutorInOptimizerPool();
			return clone;
		}

		internal void InitializeAndPushParametersToCurrentContext(ContextScript ctxNext) {
			base.ExecutionDataSnapshot.Initialize();
			base.Performance.Initialize();
			base.Strategy.Script.Initialize(this, false);
			base.MarketsimBacktest.Initialize(base.Strategy.ScriptContextCurrent.FillOutsideQuoteSpreadParanoidCheckThrow);
			base.Strategy.ScriptContextCurrent.AbsorbOnlyScriptAndIndicatorParameterCurrentValues_backToStrategyFromOptimizer_noChecksDirtyImplementation(ctxNext);
		}

		public void Dispose() {
			base.ExecutionDataSnapshot.Initialize();
		}

		//DisposableExecutor() {	// BY_COMMENTING_YOU_CANT_AVOID_IT__IT_WILL_BE_INVOKED_ANYWAY : base() {
		//    base.ExecutionDataSnapshot		= new ExecutionDataSnapshot(this);
		//    base.Backtester					= new Backtester(this);
		//    base.PositionPrototypeActivator	= new PositionPrototypeActivator(this);
		//    //NULL base.MarketLive					= new MarketLive(this);
		//    base.MarketsimBacktest			= new MarketsimBacktest(this);
		//    base.EventGenerator				= new ScriptExecutorEventGenerator(this);
		//    base.CommissionCalculator		= new CommissionCalculatorZero(this);
		//    //NULL base.Optimizer					= new Optimizer(this);
		//    //NULL base.Livesimulator				= new Livesimulator(this);
		//    //NULL_WILL_ROUTE_ORDERS_TO_MARKETSIMBACKTEST base.OrderProcessor				= Assembler.InstanceInitialized.OrderProcessor;
		//    base.Performance				= new SystemPerformance(this);
		//    base.ScriptIsRunningCantAlterInternalLists = new ConcurrentWatchdog("WAITING_FOR_SCRIPT_OVERRIDDEN_METHOD_TO_RETURN");
		//}
	}
}
