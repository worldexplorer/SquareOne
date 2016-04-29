using System;

using Sq1.Core.Charting;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Sequencing {
	public class ReusableExecutor : ScriptExecutor/*, IDisposable*/  {
		// invoked once per DisposableExecutorsPool.ctor() { executorToBeSpawned = new DisposableExecutor(this.sequencer.Executor)}
		public ReusableExecutor(string reasonToExist, ScriptExecutor scriptExecutor) : base(reasonToExist) {
			if (base.Livesimulator != null) {
				string msg = "I_WANTED_TO_AVOID_BASE_NORMAL_ScripExecutor.ctor()_AND_REPLACED_IT_WITH_DisposableExecutor()";
				//Assembler.PopupException(msg);
				//base.MarketLive		= null;
				base.Sequencer		= null;
				base.Livesimulator	= null;
				base.OrderProcessor	= null;
			}

			base.Sequencer = scriptExecutor.Sequencer;		// this one has event handlers attached
			//NOPE_I_NEED_SEQUENCER_FOR_EXECUTOR_ETHALON WILL_BE_CLONED_FOR_EACH_TO_HAVE_FRESH_SCRIPT_AND_EMPTY_CURRENT_CONTEXT
			base.Strategy	= scriptExecutor.Strategy.CloneMinimalForEachThread_forEachDisposableExecutorInSequencerPool();
			base.SetBars(scriptExecutor.Bars.SafeCopy_oneCopyForEachDisposableExecutors(reasonToExist), true);	// frozen, streamingBar detached

			// DO_I_NEED_IT??_MOVE_TO_BASE? dummy empty one instance for all spawned
			this.ChartShadow = new ChartShadow();
			this.ChartShadow.SetExecutor(this);
		}

		// invoked many times in DisposableExecutorsPool.spawnToReachTotalNr(int threadsToUse) to spawn executorToBeSpawned and add to this.executorsIdlingNow
		internal ReusableExecutor SpawnEthalonForEachThread_forEachDisposableExecutorInSequencerPool(string reasonToExist) {
			//v1 I_DONT_WANT_TO_RESET_AGAIN_base.Livesimulator=null
			ReusableExecutor clone = new ReusableExecutor(reasonToExist, this);
			//v2 HOW_DO_YOU_CREATE_INDIVIDUAL_BACKTESTERS_THIS_WAY??? DisposableExecutor clone = (DisposableExecutor) base.MemberwiseClone();
			//ALREADY clone.Strategy = this.Strategy.CloneMinimalForEachThread_forEachDisposableExecutorInSequencerPool();
			return clone;
		}

		internal void InitializeAndPushParameters_toDefaultContextCurrent_fromNextSequenced(ContextScript ctxNext) {
			base.ExecutionDataSnapshot.Initialize();

			//MULTITHREADING_ISSUE__YOU_MUST_PASS_CLONE_AND_THEN_LET_OTHER_DISPOSABLE_EXECUTOR_TO_RUN_ANOTHER_BACKTEST
			base.PerformanceAfterBacktest = new SystemPerformance(this);
			base.PerformanceAfterBacktest.Initialize();

			//v1 dynamically taken now in BacktestMarketsim.cs:476 base.MarketsimBacktest.Initialize(base.Strategy.ScriptContextCurrent.FillOutsideQuoteSpreadParanoidCheckThrow);
			//v2
			this.BacktesterOrLivesimulator.BacktestDataSource.BrokerAsBacktest_nullUnsafe.InitializeMarketsim(this);

			base.Strategy.ScriptContextCurrent.AbsorbOnlyScriptAndIndicatorParameterCurrentValues_toDisposableFromSequencer(ctxNext);

			// MOVING_1_LINE_UP_WILL_INDUCE_SEQUENCER_INITPARAMSxCORE_BUG pushes Strategy.ScriptContextCurrent => Strategy.Script
			base.Strategy.Script.Initialize(this, false);

			#if DEBUG
			string copyFromCtx =							ctxNext.ScriptAndIndicatorParametersMergedUnclonedForSequencerByName_AsString;
			string copyToCtx   = base.Strategy.ScriptContextCurrent.ScriptAndIndicatorParametersMergedUnclonedForSequencerByName_AsString;
			if (copyFromCtx != copyToCtx) {
				string msg = "NOT_ABSORBED_PROPERLY__SEQUENCER_INITPARAMSxCORE_BUG copyFrom[" + copyFromCtx + "] != copyTo[" + copyToCtx + "]";
				Assembler.PopupException(msg, null, false);
			}
			string copyToScript = base.Strategy.Script.ScriptAndIndicatorParameters_mergedUncloned_forReusableExecutorToCheckByName_AsString;
			if (copyFromCtx != copyToScript) {
				string msg = "NOT_ABSORBED_PROPERLY__SEQUENCER_INITPARAMSxCORE_BUG copyFrom[" + copyFromCtx + "] != copyScript[" + copyToScript + "]";
				Assembler.PopupException(msg, null, false);
			}
			#endif
		}

		public void Reinitialize() {
			base.ExecutionDataSnapshot.Initialize();
		}
	}
}
