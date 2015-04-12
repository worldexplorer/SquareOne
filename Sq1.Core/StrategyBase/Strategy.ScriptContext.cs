using System;
using Newtonsoft.Json;

using Sq1.Core.Sequencing;

namespace Sq1.Core.StrategyBase {
	public partial class Strategy {
		[JsonIgnore]	object scriptContextCurrentNameLock;

		void checkThrowContextNameShouldExist(string scriptContextNameShouldExist) {
			if (string.IsNullOrEmpty(scriptContextNameShouldExist)) {
				string msg = "you didn't pass ScriptContext's strategyNameTo into ContextSwitchCurrentToNamed(" + scriptContextNameShouldExist + ")/ContextMarkCurrentInListByName/ContextDelete()";
				throw new Exception(msg);
			}
			if (this.ScriptContextsByName.ContainsKey(scriptContextNameShouldExist) == false) {
				string msg = "Strategy[" + this.Name + "].ScriptContexts.ContainsKey(" + scriptContextNameShouldExist + ")=false; ScriptContext wasn't previously saved; Use ScriptParametersForm->SaveAs or ChartForm->SaveStrategyAs; ";
				throw new Exception(msg);
			}
		}
		void checkThrowContextNameShouldNotExist(string scriptContextNameShouldNotExist) {
			if (string.IsNullOrEmpty(scriptContextNameShouldNotExist)) {
				string msg = "you didn't pass ScriptContext's strategyNameTo into ContextSwitchCurrentToNamed(" + scriptContextNameShouldNotExist + ")/ContextMarkCurrentInListByName/ContextDelete()";
				throw new Exception(msg);
			}
			if (this.ScriptContextsByName.ContainsKey(scriptContextNameShouldNotExist) == true) {
				string msg = "Strategy[" + this.Name + "].ScriptContexts.ContainsKey(" + scriptContextNameShouldNotExist + ")=true; ScriptContext was already saved";
				throw new Exception(msg);
			}
		}
		//public ContextScript ContextAppendHardcopyFromCurrentToNamed(string scriptContextNameNew) {
		//	this.checkThrowContextNameShouldNotExist(scriptContextNameNew);
		//	ContextScript clone = this.ScriptContextCurrent.MemberwiseCloneMadePublic();
		//	clone.Name = scriptContextNameNew;
		//	this.ScriptContextsByName.Add(clone.Name, clone);
		//	this.ContextMarkCurrentInListByName(scriptContextNameNew);
		//	return clone;
		//}
		[Obsolete("REDUNDANT_MEANINGLESS_UNCLEAR_COPYING_HERE")]
		public void ContextSwitchCurrentToNamedAndSerialize(string scriptContextName, bool shouldSave = true) {
			lock (this.ScriptContextCurrentName) {	// Monitor shouldn't care whether I change the variable that I use for exclusive access...
			//v2 lock (this.scriptContextCurrentNameLock) {
				this.checkThrowContextNameShouldExist(scriptContextName);
				ContextScript found = this.ScriptContextsByName[scriptContextName];
				this.ScriptContextCurrentName = found.Name;
			}
			if (shouldSave) {
				this.Serialize();
			}
			this.ContextMarkCurrentInListByName(scriptContextName);
			if (this.Script != null) {
				this.ScriptParametersReflectedAbsorbMergeFromCurrentContext_SaveStrategy();
				this.Script.IndicatorParamsAbsorbMergeFromReflected_InitializeIndicatorsWithHostPanel();
			}
		}
		public void ContextMarkCurrentInListByName(string scriptContextName) {
			this.checkThrowContextNameShouldExist(scriptContextName);
			foreach (ContextScript ctx in this.ScriptContextsByName.Values) {
				ctx.IsCurrent = (ctx.Name == scriptContextName) ? true : false;
			}
		}
		public void ScriptContextAdd_duplicatedInSliders(string newScriptContextName,
						ContextScript absorbParamsFrom = null, bool setAddedAsCurrent = false) {
			if (this.ScriptContextsByName.ContainsKey(newScriptContextName)) {
				string msg = "CANT_ADD_EXISTING scriptContextName[" + newScriptContextName + "] already exists for strategy[" + this + "]";
				//Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg);
				Assembler.PopupException(msg, null, false);
				return;
			}
			ContextScript newScriptContext = new ContextScript(newScriptContextName);
			if (absorbParamsFrom != null) {
				newScriptContext.AbsorbFrom_duplicatedInSliders_or_importedFromSequencer(absorbParamsFrom, true);
			} else {
				newScriptContext.DataSourceName = this.ScriptContextCurrent.DataSourceName;
				newScriptContext.Symbol = this.ScriptContextCurrent.Symbol;
				//HOPEFULLY_GETS_AUTORESET_TO_DATASOURCES_SCALEINTERVAL_WHEN_FIRST_DISPLAYED
				//newScriptContext.ScaleInterval = this.ScriptContextCurrent.Da;
			}
			//ABSORBS_TO_CURRENT_INSTEAD_OF_NEW var forceParametersFillScriptContext = this.ScriptParametersMergedWithCurrentContext;
			this.ScriptContextsByName.Add(newScriptContextName, newScriptContext);

			bool dontSaveWeOptimize = newScriptContextName.Contains(Sequencer.OPTIMIZATION_CONTEXT_PREFIX);
			bool shouldSave = !dontSaveWeOptimize; 
			if (setAddedAsCurrent) {
				this.ContextSwitchCurrentToNamedAndSerialize(newScriptContextName, shouldSave);
			}
			if (dontSaveWeOptimize) {
				return;
			}
			this.Serialize();
			string msg2 = "scriptContextName[" + newScriptContextName + "] added for strategy[" + this + "]";
			Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg2);
		}
		public void ScriptContextAdd_cloneAndAbsorbCurrentValuesFromSequencer(string newScriptContextName,
						SystemPerformanceRestoreAble absorbParamsFrom, bool setAddedAsCurrent = false) {
			if (this.ScriptContextsByName.ContainsKey(newScriptContextName)) {
				string msg = "CANT_ADD_EXISTING scriptContextName[" + newScriptContextName + "] already exists for strategy[" + this + "]";
				//Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg);
				Assembler.PopupException(msg, null, false);
				return;
			}
			ContextScript newScriptContext = this.ScriptContextCurrent.CloneAndAbsorbFromSystemPerformanceRestoreAble(absorbParamsFrom, newScriptContextName);
			this.ScriptContextsByName.Add(newScriptContextName, newScriptContext);

			bool dontSaveWeOptimize = newScriptContextName.Contains(Sequencer.OPTIMIZATION_CONTEXT_PREFIX);
			bool shouldSave = !dontSaveWeOptimize; 
			if (setAddedAsCurrent) {
				this.ContextSwitchCurrentToNamedAndSerialize(newScriptContextName, shouldSave);
			}
			if (dontSaveWeOptimize) {
				return;
			}
			this.Serialize();
			string msg2 = "scriptContextName[" + newScriptContextName + "] added for strategy[" + this + "]";
			Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg2);
		}
		public void ScriptContextDelete(string scriptContextName) {
			if (this.ScriptContextsByName.ContainsKey(scriptContextName) == false) {
				string msg = "CANT_DELETE_NON_EXISITNG scriptContextName[" + scriptContextName + "] doesn't exist for strategy[" + this + "]";
				//Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg);
				Assembler.PopupException(msg);
				return;
				//e.Cancel = true;
			}
			if (this.ScriptContextCurrent.Name == scriptContextName) {
				string msg = "CANT_DELETE_CURRENT_LOAD_NEXT_NYI scriptContextName[" + scriptContextName + "] is the current one; load another one first and then delete [" + scriptContextName + "]";
				//Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg);
				Assembler.PopupException(msg);
				return;
				//e.Cancel = true;
			}
			this.ScriptContextsByName.Remove(scriptContextName);
			this.Serialize();
			string msg2 = "scriptContextName[" + scriptContextName + "] deleted for strategy[" + this + "]";
			Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg2);
		}
		public void ScriptContextRename(ContextScript scriptContextToRename, string scriptContextNewName) {
			string msig = "ERROR_RENAMING_CONTEXT";
			lock (this.ScriptContextCurrentName) {	// Monitor shouldn't care whether I change the variable that I use for exclusive access...
			//v2 lock (this.scriptContextCurrentNameLock) {
				if (scriptContextToRename.Name == scriptContextNewName) {
					string msg = "WONT_RENAME_TO_SAME_NAME scriptContextNewName[" + scriptContextNewName + "]=scriptContextToRename.Name[" + scriptContextToRename.Name + "], type another name";
					//Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg);
					Assembler.PopupException(msg);
					return;
					//e.Cancel = true;
				}
				if (this.ScriptContextsByName.ContainsKey(scriptContextNewName)) {
					string msg = "CANT_RENAME_NAME_ALREADY_EXISTS scriptContextNewName[" + scriptContextNewName + "] already exists for strategy[" + this + "]";
					//Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg);
					Assembler.PopupException(msg);
					return;
					//e.Cancel = true;
				}
				string oldName = scriptContextToRename.Name;
				this.ScriptContextsByName.Remove(oldName);
				scriptContextToRename.Name = scriptContextNewName;
				this.ScriptContextsByName.Add(scriptContextNewName, scriptContextToRename);
				if (this.ScriptContextCurrentName == oldName) this.ScriptContextCurrentName = scriptContextNewName;
				msig = "Successfully renamed scriptContextName[" + oldName + "]=>[" + scriptContextNewName + "] for strategy[" + this + "]";
			}
			
			this.Serialize();
			Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msig);
		}
	}
}
