using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Newtonsoft.Json;
using Sq1.Core.Backtesting;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;

namespace Sq1.Core.StrategyBase {
	public class Strategy {
		[JsonProperty]	public Guid Guid;

		[JsonProperty]	public string Name;
		[JsonProperty]	public string ScriptSourceCode;
		[JsonProperty]	public string DotNetReferences;
		[JsonProperty]	public string DllPathIfNoSourceCode;
		[JsonProperty]	public int ExceptionsLimitToAbortBacktest;

		[JsonProperty]	public string StoredInJsonAbspath;
		[JsonIgnore]	public string StoredInFolderRelName	{ get { return Path.GetFileName(Path.GetDirectoryName(this.StoredInJsonAbspath)); } }
		[JsonIgnore]	public string StoredInJsonRelName	{ get { return Path.GetFileName(this.StoredInJsonAbspath); } }
		[JsonIgnore]	public bool ActivatedFromDll { get {
				if (string.IsNullOrEmpty(this.DllPathIfNoSourceCode)) return false;
				if (this.DllPathIfNoSourceCode.Length <= 4) return false;
				string substr = this.DllPathIfNoSourceCode.Substring(DllPathIfNoSourceCode.Length - 4);
				return substr.ToUpper() == ".DLL";
			} }
		[JsonProperty]	public bool HasChartOnly { get { return string.IsNullOrEmpty(this.StoredInFolderRelName); } }
		[JsonIgnore]	public Script Script;
		//CANT_DESERIALIZE_JsonException public Dictionary<int, ScriptParameter> ScriptParametersByIdJSONcheck { get {	// not for in-program use; for a human reading Strategy's JSON
		//					return this.Script.ParametersById;
		//				} }
		[JsonIgnore]	public string ScriptParametersAsStringByIdJSONcheck { get {	// not for in-program use; for a human reading Strategy's JSON
							if (this.Script == null) return null;
							return this.Script.ScriptParametersAsString;
						} }
		[JsonIgnore]	public string IndicatorParametersAsStringByIdJSONcheck { get {	// not for in-program use; for a human reading Strategy's JSON
							if (this.Script == null) return null;
							return this.Script.IndicatorParametersAsString;
						} }
		[JsonProperty]	public string ScriptContextCurrentName;	// if you restrict SET, serializer won't be able to restore from JSON { get; private set; }
		[JsonProperty]	public Dictionary<string, ContextScript> ScriptContextsByName;
		[JsonIgnore]	public ContextScript ScriptContextCurrent { get {
				lock (this.ScriptContextCurrentName) {	// Monitor shouldn't care whether I change the variable that I use for exclusive access...
				//v2 lock (this.scriptContextCurrentNameLock) {
					if (this.ScriptContextsByName.ContainsKey(ScriptContextCurrentName) == false)  {
					string msg = "ScriptContextCurrentName[" + ScriptContextCurrentName + "] doesn't exist in Strategy[" + this.ToString() + "]";
						#if DEBUG
						Debugger.Break();
						#endif
						throw new Exception(msg);
					}
					return this.ScriptContextsByName[this.ScriptContextCurrentName];
				}
			} }
		[JsonIgnore]	public ScriptCompiler ScriptCompiler;
		// I_DONT_WANT_TO_BRING_CHART_SETTINGS_TO_CORE public ChartSettings ChartSettings;
		
		// programmer's constructor
		public Strategy(string name) : this() {
			this.Name = name;
		}
		// deserializer's constructor
		public Strategy() {
			this.Guid = Guid.NewGuid();
			this.ScriptContextCurrentName = "Default";
			this.ScriptContextsByName = new Dictionary<string, ContextScript>();
			this.ScriptContextsByName.Add(this.ScriptContextCurrentName, new ContextScript(this.ScriptContextCurrentName));
			this.ScriptCompiler = new ScriptCompiler();
			this.ExceptionsLimitToAbortBacktest = 10;
		}
		public override string ToString() {
			string ret = this.Name;
			//v1, infinite recursion if (this.ScriptContextCurrent != null) {
			if (this.ScriptContextsByName.ContainsKey(this.ScriptContextCurrentName) == false) {
				ret += "NOT_FOUND_ScriptContextCurrentName[" + this.ScriptContextCurrentName + "]";
				return ret;
			}
			ret += "[" + this.ScriptContextCurrent.ToString() + "]";
			//ret += this.ScriptParametersAsStringByIdJSONcheck + this.IndicatorParametersAsStringByIdJSONcheck;
			return ret;
		}
		public void CompileInstantiate() {
			if (this.ActivatedFromDll) return;
			this.Script = this.ScriptCompiler.CompileSourceReturnInstance(this.ScriptSourceCode, this.DotNetReferences);
		}
		private void checkThrowContextNameShouldExist(string scriptContextNameShouldExist) {
			if (string.IsNullOrEmpty(scriptContextNameShouldExist)) {
				string msg = "you didn't pass ScriptContext's strategyNameTo into ContextSwitchCurrentToNamed(" + scriptContextNameShouldExist + ")/ContextMarkCurrentInListByName/ContextDelete()";
				throw new Exception(msg);
			}
			if (this.ScriptContextsByName.ContainsKey(scriptContextNameShouldExist) == false) {
				string msg = "Strategy[" + this.Name + "].ScriptContexts.ContainsKey(" + scriptContextNameShouldExist + ")=false; ScriptContext wasn't previously saved; Use ScriptParametersForm->SaveAs or ChartForm->SaveStrategyAs; ";
				throw new Exception(msg);
			}
		}
		private void checkThrowContextNameShouldNotExist(string scriptContextNameShouldNotExist) {
			if (string.IsNullOrEmpty(scriptContextNameShouldNotExist)) {
				string msg = "you didn't pass ScriptContext's strategyNameTo into ContextSwitchCurrentToNamed(" + scriptContextNameShouldNotExist + ")/ContextMarkCurrentInListByName/ContextDelete()";
				throw new Exception(msg);
			}
			if (this.ScriptContextsByName.ContainsKey(scriptContextNameShouldNotExist) == true) {
				string msg = "Strategy[" + this.Name + "].ScriptContexts.ContainsKey(" + scriptContextNameShouldNotExist + ")=true; ScriptContext was already saved";
				throw new Exception(msg);
			}
		}
		public ContextScript ContextAppendHardcopyFromCurrentToNamed(string scriptContextNameNew) {
			this.checkThrowContextNameShouldNotExist(scriptContextNameNew);
			ContextScript clone = this.ScriptContextCurrent.MemberwiseCloneMadePublic();
			clone.Name = scriptContextNameNew;
			this.ScriptContextsByName.Add(clone.Name, clone);
			this.ContextMarkCurrentInListByName(scriptContextNameNew);
			return clone;
		}
		public void ContextSwitchCurrentToNamedAndSerialize(string scriptContextName, bool shouldSave = true) {
			lock (this.ScriptContextCurrentName) {	// Monitor shouldn't care whether I change the variable that I use for exclusive access...
			//v2 lock (this.scriptContextCurrentNameLock) {
				this.checkThrowContextNameShouldExist(scriptContextName);
				ContextScript found = this.ScriptContextsByName[scriptContextName];
				this.ScriptContextCurrentName = found.Name;
			}
			if (shouldSave) {
				Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this);
			}
			this.ContextMarkCurrentInListByName(scriptContextName);
			if (this.Script != null) {
				this.Script.PullParametersFromCurrentContextSaveStrategyIfAbsorbedFromScript();
			}
		}
		public void ContextMarkCurrentInListByName(string scriptContextName) {
			this.checkThrowContextNameShouldExist(scriptContextName);
			foreach (ContextScript ctx in this.ScriptContextsByName.Values) {
				ctx.IsCurrent = (ctx.Name == scriptContextName) ? true : false;
			}
		}
		public Strategy CloneWithNewGuid() {
			var ret = (Strategy)base.MemberwiseClone();
			ret.Guid = Guid.NewGuid();
			return ret;
		}
		public Strategy CloneWithNewScriptInstanceResetContextsToSingle(ContextScript ctxNext, ScriptExecutor executorCloneForOptimizer) {
			var ret = (Strategy)base.MemberwiseClone();
			if (ret.Script != null) {
				//WILL_THROW: public Strategy Strategy { get { return this.Executor.Strategy } }
				ret.Script = (Script) Activator.CreateInstance(ret.Script.GetType());
				//ret.Script = ret.Script.Clo);
				ret.Script.Initialize(executorCloneForOptimizer);
			}
			//Debugger.Break();
			//ret.ScriptContextsByName = new Dictionary<string, ContextScript>();
			//if (ret.ScriptContextsByName.Count == this.ScriptContextsByName.Count) {
			//    Debugger.Break();
			//}
			//ret.ScriptContextAdd(ctxNext.Name, ctxNext, true);
			//MOVED_UPSTACK ret.ContextSwitchCurrentToNamedAndSerialize(ctxNext.Name, false);
			return ret;
		}
		public void PushChangedScriptParameterValueToScriptAndSerialize(ScriptParameter scriptParameter) {
			//ScriptParameters are only identical objects between script context and sliders.tags, while every click-change is pushed into Script.ParametersByID)
			int paramId = scriptParameter.Id;
			double valueNew = scriptParameter.ValueCurrent;
			if (this.Script.ParametersById.ContainsKey(paramId) == false) {
				string msg = "YOU_CHANGED_SCRIPT_PARAMETER_WHICH_NO_LONGER_EXISTS_IN_SCRIPT";
				Assembler.PopupException(msg);
				return;
			}

			double valueOld = this.Script.ParametersById[paramId].ValueCurrent;
			if (valueOld == valueNew) {
				string msg = "SLIDER_CHANGED_TO_VALUE_SCRIPT_PARAMETER_ALREADY_HAD [" + valueOld + "]=[" + valueNew + "]";
				Assembler.PopupException(msg);
				return;
			}
			this.Script.ParametersById[paramId].ValueCurrent = valueNew;
		}
		public void PushChangedIndicatorParameterValueToScriptAndSerialize(IndicatorParameter iParamChangedCtx) {
			//new concept that IndicatorParameters are only identical objects between script context and sliders.tags, while every click-change is absorbed by snapshot.IndicatorsInstancesReflected
			string indicatorName = iParamChangedCtx.IndicatorName;
			string indicatorParameterName = iParamChangedCtx.Name;
			Dictionary<string, Indicator> indicatorsByName = this.Script.Executor.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances;
			if (indicatorsByName.ContainsKey(indicatorName) == false) {
				string msg = "WILL_PICK_UP_ON_BACKTEST__INDICATOR_NOT_FOUND_IN_INDICATORS_REFLECTED: " + indicatorName;
				Assembler.PopupException(msg);
				return;
			}
			Indicator indicatorInstantiated = indicatorsByName[indicatorName];
			if (indicatorInstantiated.ParametersByName.ContainsKey(indicatorParameterName) == false) {
				string msg = "INDICATOR_PARAMETER_NOT_FOUND_FOR_INDICATOR_REFLECTED_FOUND: " + indicatorParameterName;
				Assembler.PopupException(msg);
				return;
			}
			IndicatorParameter iParamInstantiated = indicatorInstantiated.ParametersByName[indicatorParameterName];
			double valueNew = iParamChangedCtx.ValueCurrent;
			double valueOld = iParamInstantiated.ValueCurrent;
			if (valueOld == valueNew) {
				string msg = "SLIDER_CHANGED_TO_VALUE_INDICATOR_PARAMETER_ALREADY_HAD [" + valueOld + "]=[" + valueNew + "]";
				Assembler.PopupException(msg);
				return;
			}
			iParamInstantiated.AbsorbCurrentFixBoundariesIfChanged(iParamChangedCtx);
		}
		public void ScriptContextAdd(string newScriptContextName, ContextScript absorbParamsFrom = null, bool setAddedAsCurrent = false) {
			if (this.ScriptContextsByName.ContainsKey(newScriptContextName)) {
				string msg = "CANT_ADD_EXISTING scriptContextName[" + newScriptContextName + "] already exists for strategy[" + this + "]";
				//Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg);
				Assembler.PopupException(msg, null, false);
				return;
				//e.Cancel = true;
			}
			ContextScript newScriptContext = new ContextScript(newScriptContextName);
			if (absorbParamsFrom != null) {
				newScriptContext.AbsorbFrom(absorbParamsFrom, true);
			} else {
				newScriptContext.DataSourceName = this.ScriptContextCurrent.DataSourceName;
				newScriptContext.Symbol = this.ScriptContextCurrent.Symbol;
				//HOPEFULLY_GETS_AUTORESET_TO_DATASOURCES_SCALEINTERVAL_WHEN_FIRST_DISPLAYED
				//newScriptContext.ScaleInterval = this.ScriptContextCurrent.Da;
			}
			//ABSORBS_TO_CURRENT_INSTEAD_OF_NEW var forceParametersFillScriptContext = this.ScriptParametersMergedWithCurrentContext;
			this.ScriptContextsByName.Add(newScriptContextName, newScriptContext);

			bool dontSaveWeOptimize = newScriptContextName.Contains(Optimizer.OPTIMIZATION_CONTEXT_PREFIX);
			bool shouldSave = !dontSaveWeOptimize; 
			if (setAddedAsCurrent) {
				this.ContextSwitchCurrentToNamedAndSerialize(newScriptContextName, shouldSave);
			}
			if (dontSaveWeOptimize) {
				return;
			}
			Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this);
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
			Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this);
			string msg2 = "scriptContextName[" + scriptContextName + "] deleted for strategy[" + this + "]";
			Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg2);
		}
		object scriptContextCurrentNameLock = new object();
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
			
			Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this);
			Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msig);
		}
		public void ResetScriptAndIndicatorParametersInCurrentContextToScriptDefaultsAndSave() {
			if (this.Script == null) {
				string msg = "SCRIPT_IS_NULL_CAN_NOT_RESET_PARAMETERS_TO_CLONE_CONSTRUCTED";
				Assembler.PopupException(msg);
				return;
			}
			try {
				this.Script.AbsorbScriptAndIndicatorParametersFromSelfCloneConstructed();
				Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this);
				string msg = "Successfully reset ScriptContextCurrentName[" + this.ScriptContextCurrentName + "] for strategy[" + this + "]";
				Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg);
			} catch (Exception ex) {
				Assembler.PopupException("ResetScriptAndIndicatorParametersToScriptDefaults()", ex);
			}
		}
	}
}
