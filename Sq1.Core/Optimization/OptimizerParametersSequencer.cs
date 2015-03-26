using System;
using System.Collections.Generic;

using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Optimization {
	public class OptimizerParametersSequencer {
		public	ContextScript				ContextScriptCloneIterateable;
				object						getNextLock;
				List<IndicatorParameter>	paramsMerged;
				int							slowIndex;
				int							fastIndex;
				string						log;
		
		public OptimizerParametersSequencer(ContextScript contextScript) {
			getNextLock = new object();
			ContextScriptCloneIterateable = contextScript.CloneResetAllToMinForOptimizer();
			//v1
			//paramsMerged = new List<IndicatorParameter>();
			//paramsMerged.AddRange(this.ContextScriptIterated.ScriptParametersById.Values);
			//foreach (List<IndicatorParameter> iParams in this.ContextScriptIterated.IndicatorParametersByName.Values) {
			//	paramsMerged.AddRange(iParams);
			//}
			//v2 moved to ContextScript.ParametersMerged; Sq1.Widgets.SlidersAutoGrowControl.MenuProvider.EventConsumer.DumpScriptIndicatorParametersToMenuItems() uses the same mechanism
			paramsMerged = ContextScriptCloneIterateable.ScriptAndIndicatorParametersMergedClonedForSequencer;
			slowIndex = 0; 
			fastIndex = 0;
			log = "";
		}
		public ContextScript GetFirstScriptContext(string ctxName) {
			lock (this.getNextLock) {
				ContextScript ret = new ContextScript(ctxName);
				this.logDump(ctxName);
				ret.AbsorbFrom(this.ContextScriptCloneIterateable);
				return ret;
			}
		}
		public ContextScript GetNextScriptContextSequenced(string ctxName) {
			lock (this.getNextLock) {
				ContextScript ret = new ContextScript(ctxName);
				this.nextMerged();
				this.logDump(ctxName);
				ret.AbsorbFrom(this.ContextScriptCloneIterateable);
				return ret;
			}
		}
		// a[0...2/1] b[0...10/5] c[0...30]/10
		// step1   0  0  0
		// step2   1  0  0
		// step3   2  0  0
		// step4   0  5  0
		// step5   1  5  0
		// step6   2  5  0
		// step7   0 10  0
		// step8   1 10  0
		// step9   2 10  0
		// step10  0  0 10
		// step11  1  0 10
		// step12  2  0 10
		// ......
		// stepXX  2 10 30
		// stepXY  0  0  0
		
		void nextMerged() {
			IndicatorParameter paramFast = this.paramsMerged[this.fastIndex];
			if (paramFast.WillBeSequencedDuringOptimization == false) return;

			paramFast.ValueCurrent += paramFast.ValueIncrement;
			if (paramFast.ValueCurrent <= paramFast.ValueMax) {
				string msg = "increased fastIndex[0] when slowIndex[1] (step1...step3)";
				return;
			}
			if (this.fastIndex < this.slowIndex) {
				string msg = "overflow in fastIndex[0] when slowIndex[1], moving fastIndex[0]=>[1] (steps 4,7)";
				paramFast.ValueCurrent = paramFast.ValueMin;
				this.fastIndex++;
				this.nextMerged();
				this.fastIndex = 0;
				return;
			}

			if (this.fastIndex == this.slowIndex) {
				string msg = "overflow in fastIndex[1] when slowIndex[1], moving slowIndex[1]=>[2], resetting [0]...slowIndex[1]=>[Min] (step 10)";
				this.slowIndex++;
				if (this.slowIndex == this.paramsMerged.Count) {
					string over = "slowest parameter's ValueMax reached; outer loop shouldn't go that far (stepXY)";
					this.logDump(over);
					Assembler.PopupException(over);
					return;
				}
				for (int i=0; i<=slowIndex; i++) {
					IndicatorParameter paramToMin = this.paramsMerged[i];
					if (paramToMin.WillBeSequencedDuringOptimization == false) continue;
					paramToMin.ValueCurrent = paramToMin.ValueMin;
				}
				this.fastIndex++;
				this.nextMerged();
				this.fastIndex = 0;
				return;
			}
			if (this.fastIndex > this.slowIndex) {
				string msg = "we should never get here";
				throw new Exception(msg);
			}
		}
		void logDump(string ctxName) {
			string iteration = ctxName; 
			foreach(IndicatorParameter param in this.paramsMerged) {
				iteration += "\t" + param.ToString();
			}
			this.log += iteration + "\r\n"; 
		}
	}
}
