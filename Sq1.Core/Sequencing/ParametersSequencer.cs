using System;
using System.Collections.Generic;

using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Sequencing {
	public class ParametersSequencer {
				ContextScript				contextScriptCloneIterateable;
		public	int							IncrementsDone					{ get; private set; }
				object						getNextLock;
				List<IndicatorParameter>	paramsMerged;
				int							slowIndex;
				int							fastIndex;
				string						log;
		
		public ParametersSequencer(ContextScript contextScript) {
			getNextLock = new object();
			contextScriptCloneIterateable = contextScript.CloneResetAllToMin_ForSequencer("FOR_SequencerParametersSequencer");
			paramsMerged = contextScriptCloneIterateable.ScriptAndIndicatorParametersMergedUnclonedForSequencerAndSliders;
			slowIndex = 0; 
			fastIndex = 0;
			log = "";
			IncrementsDone = 0;
		}
		public ContextScript GetFirstOrNextScriptContext(string ctxName) {
			return (this.IncrementsDone == 0)
					? this.getFirstScriptContext(ctxName)
					: this.getNextScriptContextSequenced(ctxName);
		}

		ContextScript getFirstScriptContext(string ctxName) { lock (this.getNextLock) {
			this.fastIndex = this.confirmOrFindNextParameterMarkedForSequencing(this.fastIndex);
			this.slowIndex = this.confirmOrFindNextParameterMarkedForSequencing(this.slowIndex);

			ContextScript ret = new ContextScript(ctxName);
			ret.AbsorbOnlyScriptAndIndicatorParamsFrom_usedBySequencerSequencerOnly("FOR_userClickedDuplicateCtx", this.contextScriptCloneIterateable);
			this.IncrementsDone++;
			ret.OptimizationIterationSerno = this.IncrementsDone;
			this.logDump(ctxName);
			return ret;
		} }
		ContextScript getNextScriptContextSequenced(string ctxName) { lock (this.getNextLock) {
			ContextScript ret = new ContextScript(ctxName);
			this.nextMerged();
			ret.AbsorbOnlyScriptAndIndicatorParamsFrom_usedBySequencerSequencerOnly("FOR_userClickedDuplicateCtx", this.contextScriptCloneIterateable);
			this.IncrementsDone++;
			ret.OptimizationIterationSerno = this.IncrementsDone;
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
