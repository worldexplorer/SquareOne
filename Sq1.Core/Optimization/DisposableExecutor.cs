using System;

using Sq1.Core.Charting;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Optimization {
	public class DisposableExecutor : ScriptExecutor, IDisposable  {
		// invoked once per DisposableExecutorsPool.ctor() { executorToBeSpawned = new DisposableExecutor(this.optimizer.Executor)}
		public DisposableExecutor(string reasonToExist, ScriptExecutor scriptExecutor) : base(reasonToExist) {
			if (base.Livesimulator != null) {
				string msg = "I_WANTED_TO_AVOID_BASE_NORMAL_ScripExecutor.ctor()_AND_REPLACED_IT_WITH_DisposableExecutor()";
				//Assembler.PopupException(msg);
				//base.MarketLive		= null;
				base.Optimizer		= null;
				base.Livesimulator	= null;
				base.OrderProcessor	= null;
			}

			base.Optimizer = scriptExecutor.Optimizer;		// this one has event handlers attached
			//NOPE_I_NEED_SEQUENCER_FOR_EXECUTOR_ETHALON WILL_BE_CLONED_FOR_EACH_TO_HAVE_FRESH_SCRIPT_AND_EMPTY_CURRENT_CONTEXT
			base.Strategy	= scriptExecutor.Strategy.CloneMinimalForEachThread_forEachDisposableExecutorInOptimizerPool();
			base.SetBars(scriptExecutor.Bars.SafeCopy_oneCopyForEachDisposableExecutors(reasonToExist));	// frozen, streamingBar detached

			// DO_I_NEED_IT??_MOVE_TO_BASE? dummy empty one instance for all spawned
			this.ChartShadow = new ChartShadow();
			this.ChartShadow.SetExecutor(this);
		}

// using default instead, for ScriptExecutor, not this one for DisposableExecutor
//		DisposableExecutor(string reasonToExist, DisposableExecutor disposableExecutor) : base(reasonToExist) {
//			if (base.Livesimulator != null) {
//				string msg = "I_WANTED_TO_AVOID_BASE_NORMAL_ScripExecutor.ctor()_AND_REPLACED_IT_WITH_DisposableExecutor()";
//				//Assembler.PopupException(msg);
//				//base.MarketLive		= null;
//				base.Optimizer		= null;
//				base.Livesimulator	= null;
//				base.OrderProcessor	= null;
//			}
//
//			base.Optimizer = disposableExecutor.Optimizer;
//			//NOPE_I_NEED_SEQUENCER_FOR_EXECUTOR_ETHALON WILL_BE_CLONED_FOR_EACH_TO_HAVE_FRESH_SCRIPT_AND_EMPTY_CURRENT_CONTEXT
//			base.Strategy	= disposableExecutor.Strategy.CloneMinimalForEachThread_forEachDisposableExecutorInOptimizerPool();
//			base.SetBars(disposableExecutor.Bars.SafeCopy_oneCopyForEachDisposableExecutors(reasonToExist));	// frozen, streamingBar detached
//
//			// dummy empty one instance for all spawned
//			this.ChartShadow = new ChartShadow();
//			this.ChartShadow.SetExecutor(this);
//		}

		// invoked many times in DisposableExecutorsPool.spawnToReachTotalNr(int threadsToUse) to spawn executorToBeSpawned and add to this.executorsIdlingNow
		internal DisposableExecutor SpawnEthalonForEachThread_forEachDisposableExecutorInOptimizerPool(string reasonToExist) {
			//v1 I_DONT_WANT_TO_RESET_AGAIN_base.Livesimulator=null
			DisposableExecutor clone = new DisposableExecutor(reasonToExist, this);
			//v2 HOW_DO_YOU_CREATE_INDIVIDUAL_BACKTESTERS_THIS_WAY??? DisposableExecutor clone = (DisposableExecutor) base.MemberwiseClone();
			//ALREADY clone.Strategy = this.Strategy.CloneMinimalForEachThread_forEachDisposableExecutorInOptimizerPool();
			return clone;
		}

		internal void InitializeAndPushParameters_toDefaultContextCurrent_fromNextSequenced(ContextScript ctxNext) {
			base.ExecutionDataSnapshot.Initialize();
			base.PerformanceAfterBacktest.Initialize();
			base.Strategy.Script.Initialize(this, false);
			base.MarketsimBacktest.Initialize(base.Strategy.ScriptContextCurrent.FillOutsideQuoteSpreadParanoidCheckThrow);
			base.Strategy.ScriptContextCurrent.AbsorbOnlyScriptAndIndicatorParameterCurrentValues_toDisposableFromSequencer(ctxNext);
		}

		public void Dispose() {
			base.ExecutionDataSnapshot.Initialize();
		}
	}
}
