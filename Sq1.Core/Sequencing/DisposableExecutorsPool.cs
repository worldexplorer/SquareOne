using System;
using System.Collections.Generic;

using Sq1.Core.StrategyBase;

namespace Sq1.Core.Sequencing {
	public class DisposableExecutorsPool : IDisposable {
		public	bool							IsDisposed 			{ get; private set; }
		public	int								ExecutorsSpawnedNow { get; private set; }
		public	int								ExecutorsRunningNow { get; private set; }
		public	int								ExecutorsIdlingNow	{ get; private set; }

				Sequencer						sequencer;
				ParametersSequencer	parametersSequencer;
				DisposableExecutor				executorEthalonWithDetachedBars;
				List<DisposableExecutor>		executorsSpawned;
				List<DisposableExecutor>		executorsRunningNow;
				List<DisposableExecutor>		executorsIdlingNow;
				object							oneLockForAllLists;

		DisposableExecutorsPool() {
			oneLockForAllLists	= new object();
			executorsRunningNow	= new List<DisposableExecutor>();
			executorsIdlingNow	= new List<DisposableExecutor>();
			executorsSpawned	= new List<DisposableExecutor>();
		}
		public DisposableExecutorsPool(Sequencer sequencer) : this() {
			this.sequencer = sequencer;
			executorEthalonWithDetachedBars = new DisposableExecutor("DISPOSABLE_EHTALON", this.sequencer.Executor);
			parametersSequencer = new ParametersSequencer(executorEthalonWithDetachedBars.Strategy.ScriptContextCurrent);
			IsDisposed = false;
		}

		public void Dispose() { lock (this.oneLockForAllLists) {
			var parametersMustNotBeZero = executorEthalonWithDetachedBars.Strategy.ScriptContextCurrent;
			if (parametersMustNotBeZero.ScriptAndIndicatorParametersMergedClonedForSequencerByName.Count == 0) {
				string msg = "I_FIXED_IT_BY_REPLACING_THIS_TO_RET_IN_Strategy.CloneMinimalForEachThread_forEachDisposableExecutorInSequencerPool()";
				Assembler.PopupException(msg);
			}

			if (this.IsDisposed == true) {
				string msg = "POOL_WAS_AREADY_FULLY_DISPOSED__RECREATE_POOL_AGAIN_UPSTACK__USING_ME_AGAIN_WILL_THROW_NPE";
				Assembler.PopupException(msg);
			}
			foreach (DisposableExecutor executorSpawned in this.executorsSpawned) {
				executorSpawned.Dispose();
				//HEHE executorSpawned = null;
			}
			this.executorsSpawned.Clear();
			this.executorsSpawned = null;

			this.executorEthalonWithDetachedBars.Dispose();
			this.executorEthalonWithDetachedBars = null;
			this.IsDisposed = true;
		} }

		internal void AbortAllTasksAndDispose() { lock (this.oneLockForAllLists) {
			foreach (DisposableExecutor disposable in this.executorsRunningNow) {
				string msg = "DISPOSABLE_ABORTED " + this.executorsRunningNow.IndexOf(disposable) + "/" + this.executorsRunningNow.Count;
				disposable.Backtester.AbortRunningBacktestWaitAborted(msg);
			}
			this.Dispose();
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
				string reasonToExist = "DISPOSABLE_" + (i+1) + "/" +  threadsToUse;
				DisposableExecutor executorSpawned = executorEthalonWithDetachedBars.SpawnEthalonForEachThread_forEachDisposableExecutorInSequencerPool(reasonToExist);

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
				string ctxName = Sequencer.OPTIMIZATION_CONTEXT_PREFIX + " " + (this.sequencer.BacktestsFinished + 1) + "/" + this.sequencer.BacktestsTotal;
				ContextScript ctxNext = this.parametersSequencer.GetFirstOrNextScriptContext(ctxName);
				DisposableExecutor launchingIdle = this.executorsIdlingNow[0];

				this.executorsIdlingNow.Remove(launchingIdle);
				this.ExecutorsIdlingNow = this.executorsIdlingNow.Count;
				this.executorsRunningNow.Add(launchingIdle);
				this.ExecutorsRunningNow = this.executorsRunningNow.Count;

				launchingIdle.InitializeAndPushParameters_toDefaultContextCurrent_fromNextSequenced(ctxNext);
				//WHOLE REASON OF THIS POOL IS TO DISPOSE ALERTS launchingIdle.ExecutionDataSnapshot.Initialize();	// LEAKED_HANDLES_HUNTER for v2
				//string ctxParamsAsString = launchingIdle.ToStringWithCurrentParameters();

				launchingIdle.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), true);
			}
		} }
		void afterBacktesterComplete(ScriptExecutor executorFinished) { lock (this.oneLockForAllLists) {
			string msig = " //DisposableExecutorsPool.afterBacktesterComplete()";

			if (executorFinished == null) {
				string msg = "CAN_NOT_BE_NULL_executorCompletePooled";
				Assembler.PopupException(msg + msig);
				return;
			}
			//msig = executorFinished.ToStringWithCurrentParameters() + msig;
			//string msg2 = " ANOTHER_IN_SEQUENCE_executorCompletePooled";
			//Assembler.PopupException(msg2 + msig, null, false);

			DisposableExecutor disposableFinished = executorFinished as DisposableExecutor;
			if (disposableFinished == null) {
				string msg = "CAN_NOT_BE_NULL_disposableFinished";
				Assembler.PopupException(msg + msig);
				return;
			}

			this.executorsRunningNow.Remove(disposableFinished);
			this.ExecutorsRunningNow = this.executorsRunningNow.Count;
			this.executorsIdlingNow.Add(disposableFinished);
			this.ExecutorsIdlingNow = this.executorsIdlingNow.Count;

			if (this.sequencer.AbortedDontScheduleNewBacktests) {
				string msg = "CHECK_ANOTHER_THREAD_IT_MUST_BE_RUNNING_AbortAllTasksAndDispose()";
				//Assembler.PopupException(msg);
				return;
			}

			this.sequencer.PoolFinishedBacktestOne(disposableFinished.PerformanceAfterBacktest);
		} }
	}
}
