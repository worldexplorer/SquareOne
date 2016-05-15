using System;
using System.Collections.Generic;

using Sq1.Core.StrategyBase;

namespace Sq1.Core.Sequencing {
	public class ReusableExecutorsPool : IDisposable {
		public	bool						IsReinitialized 	{ get; private set; }
		public	int							ExecutorsSpawnedNow { get; private set; }
		public	int							ExecutorsRunningNow { get; private set; }
		public	int							ExecutorsIdlingNow	{ get; private set; }

				Sequencer					sequencer;
				ParametersSequencer			parametersSequencer;
				ReusableExecutor			executorEthalonWithDetachedBars;
				List<ReusableExecutor>		executorsSpawned;
				List<ReusableExecutor>		executorsRunningNow;
				List<ReusableExecutor>		executorsIdlingNow;
				object						oneLockForAllLists;

		ReusableExecutorsPool() {
			oneLockForAllLists	= new object();
			executorsRunningNow	= new List<ReusableExecutor>();
			executorsIdlingNow	= new List<ReusableExecutor>();
			executorsSpawned	= new List<ReusableExecutor>();
		}
		public ReusableExecutorsPool(Sequencer sequencerPassed) : this() {
			this.sequencer = sequencerPassed;
			executorEthalonWithDetachedBars = new ReusableExecutor("REUSABLE_EHTALON", this.sequencer.Executor);
			parametersSequencer = new ParametersSequencer(executorEthalonWithDetachedBars.Strategy.ScriptContextCurrent);
			IsReinitialized = false;
		}

		public void Reinitialize() { lock (this.oneLockForAllLists) {
			var parametersMustNotBeZero = executorEthalonWithDetachedBars.Strategy.ScriptContextCurrent;
			if (parametersMustNotBeZero.ScriptAndIndicatorParameters_mergedUncloned_forSequencerByName.Count == 0) {
				string msg = "I_FIXED_IT_BY_REPLACING_THIS_TO_RET_IN_Strategy.CloneMinimalForEachThread_forEachReusableExecutorInSequencerPool()";
				Assembler.PopupException(msg);
			}

			if (this.IsReinitialized == true) {
				string msg = "POOL_WAS_AREADY_FULLY_REUSED__RECREATE_POOL_AGAIN_UPSTACK__USING_ME_AGAIN_WILL_THROW_NPE";
				Assembler.PopupException(msg);
			}
			foreach (ReusableExecutor executorSpawned in this.executorsSpawned) {
				executorSpawned.Reinitialize();
				//HEHE executorSpawned = null;
			}
			this.executorsSpawned.Clear();
			this.executorsSpawned = null;

			this.executorEthalonWithDetachedBars.Reinitialize();
			this.executorEthalonWithDetachedBars = null;
			this.IsReinitialized = true;
		} }

		internal void AbortAllTasksAndDispose() { lock (this.oneLockForAllLists) {
			foreach (ReusableExecutor reusable in this.executorsRunningNow) {
				string msg = "REUSABLE_ABORTED " + this.executorsRunningNow.IndexOf(reusable) + "/" + this.executorsRunningNow.Count;
				reusable.BacktesterOrLivesimulator.AbortRunningBacktest_waitAborted(msg);
			}
			this.Reinitialize();
		} }

		internal void SpawnAndLaunch(int threadsToUse) { lock (this.oneLockForAllLists) {
			if (threadsToUse <= 0) {
				string msg = "I_REFUSE_TO_SpawnAndLaunch()_threadsToUse[" + threadsToUse + "]_MUST_BE>=0";
				Assembler.PopupException(msg);
				//this.sequencer.PoolFinishedBacktestsAll();
				return;
			}
			if (executorsSpawned.Count > 0) {
				string msg = "I_REFUSE_TO_SpawnAndLaunch()_DUE_TO_executorsSpawned.Count>0_NOW_DISPOSE_AND_RECREATE_POOL_AGAIN";
				Assembler.PopupException(msg);
				return;
			}
			this.LaunchToReachTotalNr(threadsToUse);
		} }

		void spawnToReachTotalNr(int threadsToUse) { lock (this.oneLockForAllLists) {
			if (this.ExecutorsSpawnedNow >= threadsToUse) {
				string msg = "I_REFUSE_TO_SPAWN_MORE_THREADS_this.ExecutorsRunningNow[" + this.ExecutorsSpawnedNow + "] >= threadsToUse[" + threadsToUse + "]";
				//Assembler.PopupException(msg);
				return;
			}
			for (int i = this.ExecutorsSpawnedNow; i < threadsToUse; i++) {
				string reasonToExist = "REUSABLE_" + (i+1) + "/" +  threadsToUse;
				ReusableExecutor executorSpawned = executorEthalonWithDetachedBars.SpawnEthalonForEachThread_forEachReusableExecutor_inSequencerPool(reasonToExist);

				this.executorsSpawned.Add(executorSpawned);
				this.ExecutorsSpawnedNow = this.executorsSpawned.Count;
				this.executorsIdlingNow.Add(executorSpawned);
				this.ExecutorsIdlingNow = this.executorsIdlingNow.Count;

			}
		} }

		internal void LaunchToReachTotalNr(int threadsToUse) { lock (this.oneLockForAllLists) {
			if (this.ExecutorsRunningNow >= threadsToUse) {
				string msg = "I_REFUSE_TO_LAUNCH_MORE_THREADS_this.ExecutorsRunningNow[" + this.ExecutorsRunningNow + "] >= threadsToUse[" + threadsToUse + "]";
				//Assembler.PopupException(msg);
				return;
			}
			if (threadsToUse > this.ExecutorsSpawnedNow) {
				this.spawnToReachTotalNr(threadsToUse);
			}
			for (int i = this.ExecutorsRunningNow; i < threadsToUse; i++) {
				//v1 string ctxName = Sequencer.ITERATION_PREFIX + (this.sequencer.BacktestsFinished + 1) + "/" + this.sequencer.BacktestsTotal;
				string ctxName = Sequencer.ITERATION_PREFIX + (this.parametersSequencer.IncrementsDone + 1) + "/" + this.sequencer.BacktestsTotal;
				ContextScript ctxNext = this.parametersSequencer.GetFirstOrNextScriptContext(ctxName);
				if (ctxNext.SequenceIterationSerno != this.parametersSequencer.IncrementsDone) {
					string msg = "WRONG_GUESS_ON_ctxName__GetFirstOrNextScriptContext()_SHOULD_HAVE_INCREASED_+1_WHILE_YOU_ADDED_"
						+ (this.parametersSequencer.IncrementsDone - ctxNext.SequenceIterationSerno - 1);
					Assembler.PopupException(msg);
				}
				ReusableExecutor launchingIdle = this.executorsIdlingNow[0];

				this.executorsIdlingNow.Remove(launchingIdle);
				this.ExecutorsIdlingNow = this.executorsIdlingNow.Count;
				this.executorsRunningNow.Add(launchingIdle);
				this.ExecutorsRunningNow = this.executorsRunningNow.Count;

				launchingIdle.InitializeAndPushParameters_toDefaultContextCurrent_fromNextSequenced(ctxNext);
				//WHOLE REASON OF THIS POOL IS TO DISPOSE ALERTS launchingIdle.ExecutionDataSnapshot.Initialize();	// LEAKED_HANDLES_HUNTER for v2
				//string ctxParamsAsString = launchingIdle.ToStringWithCurrentParameters();

				launchingIdle.BacktesterRun_trampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), true);
			}
		} }
		void afterBacktesterComplete(ScriptExecutor executorFinished) { lock (this.oneLockForAllLists) {
			string msig = " //ReusableExecutorsPool.afterBacktesterComplete()";

			if (executorFinished == null) {
				string msg = "CAN_NOT_BE_NULL_executorCompletePooled";
				Assembler.PopupException(msg + msig);
				return;
			}
			//msig = executorFinished.ToStringWithCurrentParameters() + msig;
			//string msg2 = " ANOTHER_IN_SEQUENCE_executorCompletePooled";
			//Assembler.PopupException(msg2 + msig, null, false);

			ReusableExecutor reusableFinished = executorFinished as ReusableExecutor;
			if (reusableFinished == null) {
				string msg = "CAN_NOT_BE_NULL_reusableFinished";
				Assembler.PopupException(msg + msig);
				return;
			}

			this.executorsRunningNow.Remove(reusableFinished);
			this.ExecutorsRunningNow = this.executorsRunningNow.Count;
			this.executorsIdlingNow.Add(reusableFinished);
			this.ExecutorsIdlingNow = this.executorsIdlingNow.Count;

			if (this.sequencer.AbortedDontScheduleNewBacktests) {
				string msg = "CHECK_ANOTHER_THREAD_IT_MUST_BE_RUNNING_AbortAllTasksAndDispose()";
				//Assembler.PopupException(msg);
				return;
			}

			this.sequencer.PoolFinishedBacktestOne(reusableFinished.PerformanceAfterBacktest);
		} }

		public void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE  " + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
			foreach (ScriptExecutor each in this.executorsSpawned) {
				each.Dispose();
				//DOESNT_NULLIFY each = null;
			}
			this.executorsSpawned.Clear();
			this.executorsSpawned = null;
			this.IsDisposed = true;
		}
		public bool IsDisposed { get; private set; }
	}
}
