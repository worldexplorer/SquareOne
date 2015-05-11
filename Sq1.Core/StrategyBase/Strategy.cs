using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Newtonsoft.Json;
using Sq1.Core.Livesim;

namespace Sq1.Core.StrategyBase {
	public partial class Strategy {
		[JsonProperty]	public Guid									Guid;
		[JsonProperty]	public string								Name;
		[JsonProperty]	public string								ScriptSourceCode;
		[JsonProperty]	public string								DotNetReferences;
		[JsonProperty]	public string								DllPathIfNoSourceCode;
		[JsonProperty]	public int									ExceptionsLimitToAbortBacktest;

		[JsonProperty]	public string								StoredInJsonAbspath;
		[JsonIgnore]	public string								StoredInFolderRelName				{ get { return Path.GetFileName(Path.GetDirectoryName(this.StoredInJsonAbspath)); } }
		[JsonIgnore]	public string								StoredInJsonRelName					{ get { return Path.GetFileName(this.StoredInJsonAbspath); } }
		[JsonIgnore]	public string								RelPathAndNameForSequencerResults	{ get { return Path.Combine(this.StoredInFolderRelName, this.Name); } }
		
		[JsonIgnore]	public bool									ActivatedFromDll { get {
				if (string.IsNullOrEmpty(this.DllPathIfNoSourceCode)) return false;
				if (this.DllPathIfNoSourceCode.Length <= 4) return false;
				string substr = this.DllPathIfNoSourceCode.Substring(DllPathIfNoSourceCode.Length - 4);
				return substr.ToUpper() == ".DLL";
			} }
		[JsonProperty]	public bool									HasChartOnly { get { return string.IsNullOrEmpty(this.StoredInFolderRelName); } }
		[JsonIgnore]	public Script								Script;
		//CANT_DESERIALIZE_JsonException public Dictionary<int, ScriptParameter> ScriptParametersByIdJSONcheck { get {	// not for in-program use; for a human reading Strategy's JSON
		//					return this.Script.ParametersById;
		//				} }
		[JsonIgnore]	public string								ScriptParametersAsStringByIdJSONcheck { get {	// not for in-program use; for a human reading Strategy's JSON
				if (this.Script == null) return null;
				return this.Script.ScriptParametersAsString;
			} }
		[JsonIgnore]	public string								IndicatorParametersAsStringByIdJSONcheck { get {	// not for in-program use; for a human reading Strategy's JSON
				if (this.Script == null) return null;
				return this.Script.IndicatorParametersAsString;
			} }
		[JsonProperty]	public string								ScriptContextCurrentName;	// if you restrict SET, serializer won't be able to restore from JSON { get; private set; }
		[JsonProperty]	public Dictionary<string, ContextScript>	ScriptContextsByName;
		[JsonIgnore]	public ContextScript						ScriptContextCurrent { get {
				lock (this.ScriptContextCurrentName) {	// Monitor shouldn't care whether I change the variable that I use for exclusive access...
				//v2 lock (this.scriptContextCurrentNameLock) {
					if (this.ScriptContextsByName.ContainsKey(ScriptContextCurrentName) == false)  {
					string msg = "ScriptContextCurrentName[" + ScriptContextCurrentName + "] doesn't exist in Strategy[" + this.ToString() + "]";
						#if DEBUG
						Debugger.Launch();
						#endif
						throw new Exception(msg);
					}
					return this.ScriptContextsByName[this.ScriptContextCurrentName];
				}
			} }
		[JsonIgnore]	public ScriptCompiler						ScriptCompiler;
		// I_DONT_WANT_TO_BRING_CHART_SETTINGS_TO_CORE public ChartSettings ChartSettings;
		[JsonProperty]	public LivesimBrokerSettings				LivesimBrokerSettings;
		[JsonProperty]	public LivesimStreamingSettings				LivesimStreamingSettings;
		//v1 [JsonProperty]	public Dictionary<string, List<SystemPerformanceRestoreAble>>	OptimizationResultsByContextIdent;

	
		// programmer's constructor
		public Strategy(string name) : this() {
			this.Name = name;
		}
		// deserializer's constructor
		public Strategy() {
			string msig = "THIS_CTOR_IS_INVOKED_BY_JSON_DESERIALIZER__KEEP_ME_PUBLIC__CREATE_[JsonIgnore]d_VARIABLES_HERE";
			
			this.Guid = Guid.NewGuid();
			this.ScriptContextCurrentName			= ContextScript.DEFAULT_NAME;
			this.ScriptContextsByName				= new Dictionary<string, ContextScript>();
			this.ScriptContextsByName.Add(this.ScriptContextCurrentName, new ContextScript(this.ScriptContextCurrentName));
			this.ScriptCompiler						= new ScriptCompiler();
			this.ExceptionsLimitToAbortBacktest 	= 10;
			this.scriptContextCurrentNameLock		= new object();
			this.LivesimBrokerSettings				= new LivesimBrokerSettings(this);
			this.LivesimStreamingSettings			= new LivesimStreamingSettings(this);
			//v1this.OptimizationResultsByContextIdent	= new Dictionary<string, List<SystemPerformanceRestoreAble>>();
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

		public Strategy CloneWithNewGuid() {
			Strategy ret = (Strategy)base.MemberwiseClone();
			ret.Guid = Guid.NewGuid();
			return ret;
		}

		//v2 I_NEED_SCRIPT_ONLY__WILL_FULLY_RECONSTRUCT_CONTEXT_FROM_PARAMETERS_SEQUENCER
		internal Strategy CloneMinimalForEachThread_forEachDisposableExecutorInSequencerPool() {
			Strategy ret = (Strategy)base.MemberwiseClone();

			// I don't want ScriptDerived's own List and Dictionaries to refer to the Strategy.Script running live now;
			ret.Script = (Script) Activator.CreateInstance(this.Script.GetType());

			// each ParameterSequencer.Next() (having its own NAME), will be absorbed to DEFAULT; messing with Dictionary to keep synchronized is too costly
			ret.ScriptContextsByName = new Dictionary<string, ContextScript>();
			ret.ScriptContextsByName.Add(this.ScriptContextCurrentName, new ContextScript(this.ScriptContextCurrentName));
			ret.ScriptContextCurrentName = ContextScript.DEFAULT_NAME;
			//v1 ret.ScriptContextCurrent..CloneResetAllToMin_ForSequencer("FOR_EACH_DISPOSABLE_EXECUTOR");
			ret.ScriptContextCurrent.AbsorbOnlyScriptAndIndicatorParamsFrom_usedBySequencerSequencerOnly("FRESH_DEFAULT_CTX_FOR_EACH_DISPOSABLE", this.ScriptContextCurrent);

			ret.ScriptCompiler				= null;
			ret.LivesimBrokerSettings		= null;
			ret.LivesimStreamingSettings	= null;
			return ret;
		}

		public void ResetScriptAndIndicatorParametersInCurrentContextToScriptDefaultsAndSave() {
			if (this.Script == null) {
				string msg = "SCRIPT_IS_NULL_CAN_NOT_RESET_PARAMETERS_TO_CLONE_CONSTRUCTED";
				Assembler.PopupException(msg);
				return;
			}
			try {
				this.Script.SwitchToDefaultContextByAbsorbingScriptAndIndicatorParametersFromSelfCloneConstructed();
				//ALREADY_SAVED_BY_LINE_ABOVE this.Serialize();
				string msg = "Successfully reset ScriptContextCurrentName[" + this.ScriptContextCurrentName + "] for strategy[" + this + "]";
				Assembler.DisplayStatus(msg);
			} catch (Exception ex) {
				Assembler.PopupException("ResetScriptAndIndicatorParametersToScriptDefaults()", ex);
			}
		}
		public void Serialize() {
			Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this);
		}

		public int ScriptParametersReflectedAbsorbMergeFromCurrentContext_SaveStrategy(bool saveStrategy = false) {
			int currentValuesAbsorbed = this.ScriptContextCurrent.ScriptParametersReflectedAbsorbFromCurrentContextReplace(
					this.Script.ScriptParametersById_ReflectedCached);
			if (currentValuesAbsorbed > 0 && saveStrategy == true) this.Serialize();
			return currentValuesAbsorbed;
		}

		internal int IndicatorParametersReflectedAbsorbMergeFromCurrenctContext_SaveStrategy(bool saveStrategy = false) {
			int currentValuesAbsorbed = this.ScriptContextCurrent.IndicatorParamsReflectedAbsorbFromCurrentContextReplace(
					this.Script.IndicatorParametersByIndicator_ReflectedCached);
			if (currentValuesAbsorbed > 0 && saveStrategy == true) this.Serialize();
			return currentValuesAbsorbed;
		}
	}
}
