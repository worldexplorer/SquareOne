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
			paramsMerged = ContextScriptCloneIterateable.ScriptAndIndicatorParametersMergedClonedForSequencerAndSliders;
			slowIndex = 0; 
			fastIndex = 0;
			log = "";
		}
		public ContextScript GetFirstScriptContext(string ctxName) { lock (this.getNextLock) {
			this.fastIndex = this.confirmOrFindNextParameterMarkedForSequencing(this.fastIndex);
			this.slowIndex = this.confirmOrFindNextParameterMarkedForSequencing(this.slowIndex);

			ContextScript ret = new ContextScript(ctxName);
			this.logDump(ctxName);
			//ret.AbsorbFrom(this.ContextScriptCloneIterateable);
			ret.AbsorbScriptAndIndicatorParamsOnlyFrom("FOR_userClickedDuplicateCtx"
			                                           , this.ContextScriptCloneIterateable.ScriptParametersById
			                                           , this.ContextScriptCloneIterateable.IndicatorParametersByName);
			return ret;
		} }
		public ContextScript GetNextScriptContextSequenced(string ctxName) { lock (this.getNextLock) {
			ContextScript ret = new ContextScript(ctxName);
			this.nextMerged();
			//ret.AbsorbFrom(this.ContextScriptCloneIterateable);
			ret.AbsorbScriptAndIndicatorParamsOnlyFrom("FOR_userClickedDuplicateCtx"
			                                           , this.ContextScriptCloneIterateable.ScriptParametersById
			                                           , this.ContextScriptCloneIterateable.IndicatorParametersByName);
			this.logDump(ctxName);
			return ret;
		} }
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
			this.fastIndex = this.confirmOrFindNextParameterMarkedForSequencing(this.fastIndex);
			IndicatorParameter paramFast = this.paramsMerged[this.fastIndex];

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
				this.slowIndex = this.confirmOrFindNextParameterMarkedForSequencing(this.slowIndex);
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
				string msg = "ADJUSTING_SLOW this.fastIndex[" + this.fastIndex + "] > this.slowIndex[" + this.slowIndex + "]";
				this.slowIndex = this.confirmOrFindNextParameterMarkedForSequencing(this.slowIndex);
				if (this.fastIndex > this.slowIndex) {
					string msg2 = "...DIDNT_HELP this.fastIndex[" + this.fastIndex + "] > this.slowIndex[" + this.slowIndex + "]";
					throw new Exception(msg2);
				}
			}
		}
		int confirmOrFindNextParameterMarkedForSequencing(int fastindex) {
			int ret = fastIndex;
			IndicatorParameter paramFast = this.paramsMerged[fastIndex];
			bool paramToSequenceFound = paramFast.WillBeSequencedDuringOptimization;
			if (paramToSequenceFound == true) return ret; 
			for (int i = fastIndex; i < this.paramsMerged.Count; i++) {
				paramFast = this.paramsMerged[i];
				paramToSequenceFound = paramFast.WillBeSequencedDuringOptimization;
				if (paramToSequenceFound == true) {
					ret = i;
					break;
				}
			}
			//if (i == this.paramsMerged.Count - 1) {
			if (paramToSequenceFound == false) {
				string msg = "DONT_RUN_NEXT_MERGED_WHEN_NO_PARAMETER_TO_SEQUENCE_WAS_CHECKED";
				Assembler.PopupException(msg);
			}
			return ret;
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
