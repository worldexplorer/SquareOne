using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.StrategyBase {
	public class Strategy {
		public Guid Guid;

		public string Name;
		public string ScriptSourceCode;
		public string DotNetReferences;
		public string DllPathIfNoSourceCode;
		public int ExceptionsLimitToAbortBacktest;

		public string StoredInJsonAbspath;
		[JsonIgnore] public string StoredInFolderRelName	{ get { return Path.GetFileName(Path.GetDirectoryName(this.StoredInJsonAbspath)); } }
		[JsonIgnore] public string StoredInJsonRelName	{ get { return Path.GetFileName(this.StoredInJsonAbspath); } }
		[JsonIgnore] public bool ActivatedFromDll { get {
				if (string.IsNullOrEmpty(this.DllPathIfNoSourceCode)) return false;
				if (this.DllPathIfNoSourceCode.Length <= 4) return false;
				string substr = this.DllPathIfNoSourceCode.Substring(DllPathIfNoSourceCode.Length - 4);
				return substr.ToUpper() == ".DLL";
			} }
		public bool HasChartOnly { get { return string.IsNullOrEmpty(this.StoredInFolderRelName); } }
		[JsonIgnore] public Script Script;
		public string ScriptParametersByIdJSONcheck { get {	// not for in-program use; for a human reading Strategy's JSON
				if (this.Script == null) return null;
				return this.Script.ParametersAsString;
			} }
		public string ScriptContextCurrentName;	// if you restrict SET, serializer won't be able to restore from JSON { get; private set; }
		public Dictionary<string, ContextScript> ScriptContextsByName;
		[JsonIgnore] public ContextScript ScriptContextCurrent { get {
				if (this.ScriptContextsByName.ContainsKey(ScriptContextCurrentName) == false)  {
					string msg = "ScriptContextCurrentName[" + ScriptContextCurrentName + "] doesn't exist in Strategy[" + this + "]";
					throw new Exception(msg);
				}
				return this.ScriptContextsByName[this.ScriptContextCurrentName];
			} }
		[JsonIgnore] public Dictionary<int, ScriptParameter> ScriptParametersMergedWithCurrentContext { get {
				Dictionary<int, ScriptParameter> ret = new Dictionary<int, ScriptParameter>();
				if (this.Script == null) return ret;
				bool storeStrategySinceParametersGottenFromScript = false;
				foreach (ScriptParameter paramScript in this.Script.ParametersById.Values) {
					//ScriptParameter paramMerged = paramScript.Clone();
					ScriptParameter paramMerged = paramScript;
					if (this.ScriptContextCurrent.ParameterValuesById.ContainsKey(paramScript.Id)) {
						double valueContext = this.ScriptContextCurrent.ParameterValuesById[paramScript.Id];
						paramMerged.ValueCurrent = valueContext;
					} else {
						this.ScriptContextCurrent.ParameterValuesById.Add(paramScript.Id, paramScript.ValueCurrent);
						string msg = "added paramScript[Id=" + paramScript.Id + " value=" + paramScript.ValueCurrent + "]"
							+ " into Script[" + this.Script.GetType().Name + "].ScriptContextCurrent[" + this.ScriptContextCurrent.Name + "]";
						Assembler.PopupException(msg);
						storeStrategySinceParametersGottenFromScript = true;
					}
					ret.Add(paramMerged.Id, paramMerged);
				}
				if (storeStrategySinceParametersGottenFromScript) {
					Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this);
				}
				return ret;
			} }
		[JsonIgnore] public ScriptCompiler ScriptCompiler;
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
			return this.Name;
		}

//		public void SaveSourceCompileActivate(string sourceCode) {
//			this.ScriptSourceCode = sourceCode;
//			this.CompileInstantiate();
//		}

		public void CompileInstantiate() {
			if (this.ActivatedFromDll) {
				// TODO: for now, now multiple charts running the same strategy are allowed
				//if (type.BaseType.Name != typeof(Script).Name) continue;
				//result = (Activator.CreateInstance(type) as Script);
				//if (result == null) {
				//	string msg = "Activator.CreateInstance(" + type + ") as Script == null";
				//	throw new Exception();
				//}
				return;
			}
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
		public void ContextSwitchCurrentToNamedAndSerialize(string scriptContextName) {
			this.checkThrowContextNameShouldExist(scriptContextName);
			ContextScript found = this.ScriptContextsByName[scriptContextName];
			this.ScriptContextCurrentName = found.Name;
			Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this);
			this.ContextMarkCurrentInListByName(scriptContextName);
		}
		public void ContextMarkCurrentInListByName(string scriptContextName) {
			this.checkThrowContextNameShouldExist(scriptContextName);
			foreach (ContextScript ctx in this.ScriptContextsByName.Values) {
				ctx.IsCurrent = (ctx.Name == scriptContextName) ? true : false;
			}
		}

		public Strategy Clone() {
			var ret = (Strategy)base.MemberwiseClone();
			ret.Guid = Guid.NewGuid();
			return ret;
		}

		public void DropChangedValueToScriptAndCurrentContextAndSerialize(ScriptParameter scriptParameter) {
			int paramId = scriptParameter.Id;
			double valueNew = scriptParameter.ValueCurrent;
			// merge them to ONE !! I hate proxies and facades...
			this.Script.ParametersById[paramId].ValueCurrent = valueNew;
			//double valueOld = this.ScriptContextCurrent.ParameterValuesById[paramId];
			this.ScriptContextCurrent.ParameterValuesById[paramId] = valueNew;
			Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this);
		}
		
		public void ScriptContextAdd(string newScriptContextName, ContextScript absorbParamsFrom = null) {
			if (this.ScriptContextsByName.ContainsKey(newScriptContextName)) {
				string msg = "CANT_ADD_EXISTING scriptContextName[" + newScriptContextName + "] already exists for strategy[" + this + "]";
				Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg);
				return;
				//e.Cancel = true;
			}
			ContextScript newScriptContext = new ContextScript(newScriptContextName);
			if (absorbParamsFrom != null) {
				newScriptContext.AbsorbFrom(absorbParamsFrom, true);
			}
			//ABSORBS_TO_CURRENT_INSTEAD_OF_NEW var forceParametersFillScriptContext = this.ScriptParametersMergedWithCurrentContext;
			this.ScriptContextsByName.Add(newScriptContextName, newScriptContext);
			Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this);
			string msg2 = "scriptContextName[" + newScriptContextName + "] added for strategy[" + this + "]";
			Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg2);
		}
		
		public void ScriptContextDelete(string scriptContextName) {
			if (this.ScriptContextsByName.ContainsKey(scriptContextName) == false) {
				string msg = "CANT_DELETE_NON_EXISITNG scriptContextName[" + scriptContextName + "] doesn't exist for strategy[" + this + "]";
				Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg);
				return;
				//e.Cancel = true;
			}
			if (this.ScriptContextCurrent.Name == scriptContextName) {
				string msg = "CANT_DELETE_CURRENT_LOAD_NEXT_NYI scriptContextName[" + scriptContextName + "] is the current one; load another one first and then delete [" + scriptContextName + "]";
				Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg);
				return;
				//e.Cancel = true;
			}
			this.ScriptContextsByName.Remove(scriptContextName);
			Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this);
			string msg2 = "scriptContextName[" + scriptContextName + "] deleted for strategy[" + this + "]";
			Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg2);
		}
		
		public void ScriptContextRename(ContextScript scriptContextToRename, string scriptContextNewName) {
			if (scriptContextToRename.Name == scriptContextNewName) {
				string msg = "WONT_RENAME_TO_SAME_NAME scriptContextNewName[" + scriptContextNewName + "]=scriptContextToRename.Name[" + scriptContextToRename.Name + "], type another name";
				Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg);
				return;
				//e.Cancel = true;
			}
			if (this.ScriptContextsByName.ContainsKey(scriptContextNewName)) {
				string msg = "CANT_RENAME_NAME_ALREADY_EXISTS scriptContextNewName[" + scriptContextNewName + "] already exists for strategy[" + this + "]";
				Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg);
				return;
				//e.Cancel = true;
			}
			string oldName = scriptContextToRename.Name;
			this.ScriptContextsByName.Remove(oldName);
			scriptContextToRename.Name = scriptContextNewName;
			this.ScriptContextsByName.Add(scriptContextNewName, scriptContextToRename);
			if (this.ScriptContextCurrentName == oldName) this.ScriptContextCurrentName = scriptContextNewName;
			
			Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this);
			string msg2 = "scriptContextName[" + oldName + "]=>[" + scriptContextNewName + "] for strategy[" + this + "]";
			Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg2);
		}
	}
}