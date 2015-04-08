using System;
using System.Threading;
using System.Collections.Generic;

using Sq1.Core.Indicators;
using Sq1.Core.Optimization;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Optimization {
	public class DisposableExecutorsPool : IDisposable {
		Optimizer						optimizer;
		OptimizerParametersSequencer	parametersSequencer;
		DisposableExecutor				executorEthalon;
		List<DisposableExecutor>		executorsSpawned;
		List<DisposableExecutor>		executorsRunningNow;
		List<DisposableExecutor>		executorsIdlingNow;
		object							oneLockForAllLists;
		public	bool					IsDisposed;

		public int ExecutorsSpawnedNow { get { lock(this.oneLockForAllLists) {
			return this.executorsSpawned.Count;
		} } }

		public int ExecutorsRunningNow { get { lock(this.oneLockForAllLists) {
			return this.executorsRunningNow.Count;
		} } }

		public int ExecutorsIdlingNow { get { lock(this.oneLockForAllLists) {
			return this.executorsIdlingNow.Count;
		} } }

		DisposableExecutorsPool() {
			oneLockForAllLists	= new object();
			executorsRunningNow		= new List<DisposableExecutor>();
			executorsIdlingNow		= new List<DisposableExecutor>();
			executorsSpawned		= new List<DisposableExecutor>();
		}
		public DisposableExecutorsPool(Optimizer optimizer) : this() {
			this.optimizer = optimizer;
			executorEthalon = new DisposableExecutor(this.optimizer.Executor);
			parametersSequencer = new OptimizerParametersSequencer(executorEthalon.Strategy.ScriptContextCurrent);
			IsDisposed = false;
		}

		public void Dispose() {
			foreach (DisposableExecutor executorSpawned in this.executorsSpawned) {
				executorSpawned.Dispose();
				//HEHE executorSpawned = null;
			}
			this.executorEthalon.Dispose();
			this.IsDisposed = true;
		}

		internal void AbortAllTasksAndDispose() { lock (this.oneLockForAllLists) {
			foreach (DisposableExecutor disposable in this.executorsRunningNow) {
				string msg = "DISPOSABLE_ABORTED " + this.executorsRunningNow.IndexOf(disposable) + "/" + this.executorsRunningNow.Count;
				disposable.Backtester.AbortRunningBacktestWaitAborted(msg, 0);
			}
			this.Dispose();
		} }

		internal void SpawnAndLaunch(int threadsToUse) { lock (this.oneLockForAllLists) {
			if (threadsToUse <= 0) {
				string msg = "I_REFUSE_TO_SpawnAndLaunch()_threadsToUse[" + threadsToUse + "]_MUST_BE>=0";
				Assembler.PopupException(msg);
				//this.optimizer.PoolFinishedBacktestsAll();
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
				DisposableExecutor executorSpawned = executorEthalon.SpawnEthalonForEachThread_forEachDisposableExecutorInOptimizerPool();
				executorSpawned.Backtester.TO_STRING_PREFIX = "DISPOSABLE_" + (i+1) + "/" +  threadsToUse;
				this.executorsSpawned.Add(executorSpawned);
				this.executorsIdlingNow.Add(executorSpawned);
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
				string ctxName = Optimizer.OPTIMIZATION_CONTEXT_PREFIX + " " + (this.optimizer.BacktestsFinished + 1) + "/" + this.optimizer.BacktestsTotal;
				ContextScript ctxNext = (this.optimizer.BacktestsFinished == 0)
					? this.parametersSequencer.GetFirstScriptContext(ctxName)
					: this.parametersSequencer.GetNextScriptContextSequenced(ctxName);

				DisposableExecutor launchingIdle = this.executorsIdlingNow[0];
				this.executorsIdlingNow.Remove(launchingIdle);
				this.executorsRunningNow.Add(launchingIdle);
				launchingIdle.InitializeAndPushParametersToCurrentContext(ctxNext);
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
			this.executorsIdlingNow.Add(disposableFinished);

			if (this.optimizer.AbortedDontScheduleNewBacktests) {
				string msg = "CHECK_ANOTHER_THREAD_IT_MUST_BE_RUNNING_AbortAllTasksAndDispose()";
				Assembler.PopupException(msg);
				return;
			}

			this.optimizer.PoolFinishedBacktestOne(disposableFinished.Performance);
		} }
	}
}
