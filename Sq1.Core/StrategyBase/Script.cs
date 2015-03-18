using System;
using System.Collections.Generic;
using System.Reflection;

using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;
using Sq1.Core.Optimization;

namespace Sq1.Core.StrategyBase {
	public abstract partial class Script {
		public		ScriptExecutor	Executor							{ get; private set; }
		protected	Bars			Bars								{ get { return (Executor == null) ? null : Executor.Bars; } }
		public		Strategy		Strategy							{ get { return this.Executor.Strategy; } }
		public		string			StrategyName						{ get { return this.Executor.StrategyName; } }
		public		PositionList	Positions							{ get { return this.Executor.ExecutionDataSnapshot.PositionsMaster; } }
		
		#region Position-related userland-invokeable parts
		public		bool			IsLastPositionNotClosedYet			{ get {
				//v1 return LastPosition.Active;
				Position pos = this.LastPosition;
				if (null == pos) return false;
				return (pos.ExitMarketLimitStop == MarketLimitStop.Unknown);
			} }
		public		Position		LastPositionOpenNow					{ get {
				List<Position> positionsOpenNow = this.Executor.ExecutionDataSnapshot.PositionsOpenNow.InnerList;
				if (positionsOpenNow.Count == 0) return null;
				return positionsOpenNow[positionsOpenNow.Count - 1];
			} }
		public		Position		LastPosition						{ get {
				List<Position> positionsMaster = this.Executor.ExecutionDataSnapshot.PositionsMaster.InnerList;
				if (positionsMaster.Count == 0) return null;
				return positionsMaster[positionsMaster.Count - 1];
			} }
		public		bool			HasAlertsPendingAndPositionsOpenNow	{ get { return this.HasAlertsPending && this.HasPositionsOpenNow; } }
		public		bool			HasAlertsPendingOrPositionsOpenNow	{ get { return this.HasAlertsPending || this.HasPositionsOpenNow; } }
		public		bool			HasAlertsPending					{ get { return (this.Executor.ExecutionDataSnapshot.AlertsPending.Count > 0); } }
		public		bool			HasPositionsOpenNow					{ get { return (this.Executor.ExecutionDataSnapshot.PositionsOpenNow.Count > 0); } }
		#endregion
		
		public	SortedDictionary<int, ScriptParameter>	ScriptParametersById_ReflectedCached;
		public	Dictionary<string, ScriptParameter>		ScriptParametersByNameInlineCopy { get {
				Dictionary<string, ScriptParameter> ret = new Dictionary<string, ScriptParameter>();
				foreach (ScriptParameter param in ScriptParametersById_ReflectedCached.Values) {
					if (ret.ContainsKey(param.Name)) {
						string msg = "PARAMETER_NAME_NOT_UNIQUE[" + param.Name + "], prev[" + ret[param.Name].ToString() + "] this[" + param.ToString() + "]";
						Assembler.PopupException(msg + " //Script.ParametersByName");
						continue;
					}
					ret.Add(param.Name, param);
				}
				return ret;
			} }
		public	string				ScriptParametersByIdAsString		{ get {
				if (this.ScriptParametersById_ReflectedCached.Count == 0) return "(NoScriptParameters)";
				string ret = "";
				foreach (int id in this.ScriptParametersById_ReflectedCached.Keys) {
					ret += this.ScriptParametersById_ReflectedCached[id].Name + "=" + this.ScriptParametersById_ReflectedCached[id].ValueCurrent + ",";
				}
				ret = ret.TrimEnd(",".ToCharArray());
				return "(" + ret + ")";
			} }

		// that's a smart suggestion SharpDevelop pops up: "Constructors in abstract classes should not be public"
		protected Script() {
			ScriptParametersById_ReflectedCached = new SortedDictionary<int, ScriptParameter>();
		}

		#region Initializers
		public void Initialize(ScriptExecutor scriptExecutor) {
			this.Executor = scriptExecutor;
			// TODO: simplify or justify/explain why ScriptParametersById is so complicated; it's a cached reflection, but so many identical structures / copying / sync...
			if (this.ScriptParametersById_ReflectedCached.Count != 0) {
				string msg = "DUPLICATE_SCRIPT_INITIALIZATION__INVOKE_ME_ONLY_ONCE_PER_MY_LIFETIME__CLEARING_AND_REFILLING_MY_SCRIPT_PARAMETERS";
				Assembler.PopupException(msg);
				this.ScriptParametersById_ReflectedCached.Clear();
			}
			foreach(KeyValuePair<int, ScriptParameter> keyValue in this.ScriptParametersInitializedInDerivedConstructor) {
				this.ScriptParametersById_ReflectedCached.Add(keyValue.Key, keyValue.Value);
			}
		}
		public void InitializeBacktestWrapper() {
			if (this.Bars == null) {
				string msg = "I_REFUSE_TO_INVOKE_CHILDS_InitializeBacktest() Bars=null " + this.ToString();
				Assembler.PopupException(msg);
			}
			//this.executor.Backtester.Initialize(this.BacktestMode);

			//MOVED_TO_ChartFormManager.InitializeStrategyAfterDeserialization() FIX_FOR: TOO_SMART_INCOMPATIBLE_WITH_LIFE_SPENT_4_HOURS_DEBUGGING DESERIALIZED_STRATEGY_HAD_PARAMETERS_NOT_INITIALIZED INITIALIZED_BY_SLIDERS_AUTO_GROW_CONTROL
			//string msg = "DONT_UNCOMMENT_ITS_LIKE_METHOD_BUT_USED_IN_SLIDERS_AUTO_GROW_CONTROL_4_HOURS_DEBUGGING";
			//this.PullCurrentContextParametersFromStrategyTwoWayMergeSaveStrategy();
			
			try {
				this.InitializeBacktest();
			} catch (Exception ex) {
				Assembler.PopupException("FIX_YOUR_OVERRIDEN_METHOD Strategy[" + this.StrategyName + "].InitializeBacktest()", ex);
				this.Executor.Backtester.RequestingBacktestAbort.Set();
			}
		}		
		//FIX_FOR: TOO_SMART_INCOMPATIBLE_WITH_LIFE_SPENT_4_HOURS_DEBUGGING DESERIALIZED_STRATEGY_HAD_PARAMETERS_NOT_INITIALIZED INITIALIZED_BY_SLIDERS_AUTO_GROW_CONTROL
//		public void ScriptParametersPushCurrentContextIntoReflectedSaveStrategy() {
//			string msig = " //ScriptParametersPushCurrentContextIntoReflectedSaveStrategy()";
//			bool storeStrategySinceParametersGottenFromScript = false;
//			foreach (ScriptParameter scriptParam in this.ScriptParametersById_ReflectedCached.Values) {
//				if (this.Strategy.ScriptContextCurrent.ScriptParametersById.ContainsKey(scriptParam.Id)) {
//					double valueCurrentContext = this.Strategy.ScriptContextCurrent.ScriptParametersById[scriptParam.Id].ValueCurrent;
//					if (scriptParam.ValueCurrent != valueCurrentContext) {
//					string msg = "REPLACED_ScriptParameter[Id=" + scriptParam.Id + " ValueCurrent=" + scriptParam.ValueCurrent + "] => valueCurrentContext[" + valueCurrentContext + "] " + this.ToString();
//						#if DEBUG
//						Assembler.PopupException(msg + msig, null, false);
//						#endif
//						scriptParam.ValueCurrent = valueCurrentContext;
//						storeStrategySinceParametersGottenFromScript = true;
//					}
//					double valueMaxContext = this.Strategy.ScriptContextCurrent.ScriptParametersById[scriptParam.Id].ValueMax;
//					if (scriptParam.ValueMax != valueMaxContext) {
//						string msg = "REPLACED_ScriptParameter[Id=" + scriptParam.Id + " ValueMax=" + scriptParam.ValueMax + "] => valueMaxContext[" + valueMaxContext + "] " + this.ToString();
//						#if DEBUG
//						Assembler.PopupException(msg + msig, null, false);
//						#endif
//						scriptParam.ValueMax = valueMaxContext;
//						storeStrategySinceParametersGottenFromScript = true;
//					}
//					double valueMinContext = this.Strategy.ScriptContextCurrent.ScriptParametersById[scriptParam.Id].ValueMin;
//					if (scriptParam.ValueMin != valueMinContext) {
//						string msg = "REPLACED_ScriptParameter[Id=" + scriptParam.Id + " ValueMin=" + scriptParam.ValueMin + "] => valueMinContext[" + valueMinContext + "] " + this.ToString();
//						#if DEBUG
//						Assembler.PopupException(msg + msig, null, false);
//						#endif
//						scriptParam.ValueMin = valueMinContext;
//						storeStrategySinceParametersGottenFromScript = true;
//					}
//					string nameContext = this.Strategy.ScriptContextCurrent.ScriptParametersById[scriptParam.Id].Name;
//					if (scriptParam.Name != nameContext) {
//						string msg = "REPLACED_ScriptParameter[Id=" + scriptParam.Id + " Name=" + scriptParam.Name + "] => nameContext[" + nameContext + "] " + this.ToString();
//						#if DEBUG
//						Assembler.PopupException(msg + msig, null, false);
//						#endif
//						scriptParam.Name = nameContext;
//						storeStrategySinceParametersGottenFromScript = true;
//					}
//				} else {
//					this.Strategy.ScriptContextCurrent.ScriptParametersById.Add(scriptParam.Id, scriptParam);
//					string msg = "ADDED_ScriptParameter[Id=" + scriptParam.Id + " value=" + scriptParam.ValueCurrent + "] " + this.ToString();
//					#if DEBUG
//					Assembler.PopupException(msg + msig, null, false);
//					#endif
//					storeStrategySinceParametersGottenFromScript = true;
//				}
//			}
//			if (storeStrategySinceParametersGottenFromScript) {
//				bool dontSaveWeOptimize = this.Strategy.ScriptContextCurrent.Name.Contains(Optimizer.OPTIMIZATION_CONTEXT_PREFIX);
//				if (dontSaveWeOptimize) {
//					string msg = "SCRIPT_RECOMPILED_ADDING_MORE_PARAMETERS_THAN_OPTIMIZER_PROVIDED_IN_SCRIPTCONTEXT";
//					Assembler.PopupException(msg + msig, null, false);
//					return;
//				}
//				this.Strategy.Serialize();
//			}
//		}
	public void ScriptParametersPushReflectedIntoCurrentContextSaveStrategy() {
		string msig = " //PushRegisteredScriptParametersIntoCurrentContextSaveStrategy()";
		bool storeStrategySinceParametersGottenFromScript = false;
		foreach (ScriptParameter spReflected in this.ScriptParametersById_ReflectedCached.Values) {
			if (this.Strategy.ScriptContextCurrent.ScriptParametersById.ContainsKey(spReflected.Id)) {
				ScriptParameter spContext = this.Strategy.ScriptContextCurrent.ScriptParametersById[spReflected.Id];
				if (spContext.ValueCurrent != spReflected.ValueCurrent) {
					string msg = "REPLACED_ScriptParameter[Id=" + spReflected.Id + " spContext.ValueCurrent=[" + spContext.ValueCurrent + "] => spReflected.ValueCurrent[" + spReflected.ValueCurrent + "] " + this.ToString();
					#if DEBUG
					Assembler.PopupException(msg + msig, null, false);
					#endif
					spContext.ValueCurrent = spReflected.ValueCurrent;
					storeStrategySinceParametersGottenFromScript = true;
				}
				if (spContext.ValueMax != spReflected.ValueMax) {
					string msg = "REPLACED_ScriptParameter[Id=" + spReflected.Id + " spContext.ValueMax=[" + spContext.ValueMax + "] => spReflected.ValueMax[" + spReflected.ValueMax + "] " + this.ToString();
					#if DEBUG
					Assembler.PopupException(msg + msig, null, false);
					#endif
					spContext.ValueCurrent = spReflected.ValueMax;
					storeStrategySinceParametersGottenFromScript = true;
				}
				if (spContext.ValueMin != spReflected.ValueMin) {
					string msg = "REPLACED_ScriptParameter[Id=" + spReflected.Id + " spContext.ValueMin=" + spContext.ValueMin + "] => spReflected.ValueMin[" + spReflected.ValueMin + "] " + this.ToString();
					#if DEBUG
					Assembler.PopupException(msg + msig, null, false);
					#endif
					spContext.ValueMin = spReflected.ValueMin;
					storeStrategySinceParametersGottenFromScript = true;
				}
				if (spContext.Name != spReflected.Name) {
					string msg = "REPLACED_ScriptParameter[Id=" + spReflected.Id + " spContext.Name=[" + spContext.Name + "] => spReflected.Name[" + spReflected.Name + "] " + this.ToString();
					#if DEBUG
					Assembler.PopupException(msg + msig, null, false);
					#endif
					spContext.Name = spReflected.Name;
					storeStrategySinceParametersGottenFromScript = true;
				}
			} else {
				this.Strategy.ScriptContextCurrent.ScriptParametersById.Add(spReflected.Id, spReflected);
				string msg = "ADDED_ScriptParameter[Id=" + spReflected.Id + " value=" + spReflected.ValueCurrent + "] " + this.ToString();
				#if DEBUG
				Assembler.PopupException(msg + msig, null, false);
				#endif
				storeStrategySinceParametersGottenFromScript = true;
			}
		}
		if (storeStrategySinceParametersGottenFromScript) {
			bool dontSaveWeOptimize = this.Strategy.ScriptContextCurrent.Name.Contains(Optimizer.OPTIMIZATION_CONTEXT_PREFIX);
			if (dontSaveWeOptimize) {
				string msg = "SCRIPT_RECOMPILED_ADDING_MORE_PARAMETERS_THAN_OPTIMIZER_PROVIDED_IN_SCRIPTCONTEXT";
				Assembler.PopupException(msg + msig, null, false);
				return;
			}
			this.Strategy.Serialize();
		}
	}
		#endregion

		#region script parameters and indicator parameter userland-invokable helper
//		[Obsolete("JUST_DECLARE_YOUR_PARAMETER_AS_CLASS_VARIABLE__INTROSPECTION_WILL_PICK_IT_UP")]
//		public ScriptParameter ScriptParameterCreateRegister(int id, string name, double value, double min, double max, double increment, string reasonToExist="NO_REASON_TO_EXIST") {
//			ScriptParameter strategyParameter = new ScriptParameter(id, name, value, min, max, increment, reasonToExist);
//			this.checkThrowScriptParameterAlreadyRegistered(id, name);
//			this.ScriptParametersById_ReflectedCached.Add(id, strategyParameter);
//			return strategyParameter;
//		}
//		protected void checkThrowScriptParameterAlreadyRegistered(int id, string name) {
//			if (this.ScriptParametersById.ContainsKey(id) == false) return;
//			ScriptParameter param = this.ScriptParametersById[id];
//			string msg = "Script[" + this.StrategyName + "] already had parameter {id[" + param.Id + "] name[" + param.Name + "]}"
//				+ " while adding {id[" + id + "] name[" + name + "]}; edit source code and make IDs unique for every parameter";
//			#if DEBUG
//			Debugger.Break();
//			#endif
//			throw new Exception(msg);
//		}
		#endregion

		#region all Indicator-related is grouped here
		public Dictionary<string, IndicatorParameter> IndicatorsParametersInitializedInDerivedConstructorByNameForSliders { get {
				Dictionary<string, IndicatorParameter> ret = new Dictionary<string, IndicatorParameter>();
				Dictionary<string, List<IndicatorParameter>> parametersByIndicatorName = this.Strategy.ScriptContextCurrent.IndicatorParametersByName;

				//List<Indicator> indicatorsInitializedInDerivedConstructor = this.IndicatorsInitializedInDerivedConstructor; 
				//bool mustBeMergedIfAny = indicatorsInitializedInDerivedConstructor.Count > 0
				//	//&& parametersByIndicatorName.Count != indicatorsInitializedInDerivedConstructor.Count
				//	;
				//if (mustBeMergedIfAny) {
				//	#if DEBUG
				//	Debugger.Break();
				//	this.Strategy.Script.IndicatorsInitializeAbsorbParamsFromJsonStoreInSnapshot();
				//	parametersByIndicatorName = this.Strategy.ScriptContextCurrent.IndicatorParametersByName;
				//	#endif
				//	if (parametersByIndicatorName.Count == 0) {
				//		#if DEBUG
				//		Debugger.Break();
				//		#endif
				//		return ret;
				//	}
				//}
				foreach (string indicatorName in parametersByIndicatorName.Keys) {
					List<IndicatorParameter> indicatorParameters = parametersByIndicatorName[indicatorName];
					foreach (IndicatorParameter indicatorParameter in indicatorParameters) {
						string indicatorDotParameterName = indicatorName + "." + indicatorParameter.Name;
						if (indicatorParameter.FullName.StartsWith(indicatorName) == false) {
							// HACK! Indicator Instantiator must have had set Indicator.Name=<Script's variable name>
							//Debugger.Break();
							indicatorParameter.IndicatorName = indicatorName;
						}
						ret.Add(indicatorDotParameterName, indicatorParameter);
					}
				}
				return ret;
			} }
		public string IndicatorParametersAsString { get {
				if (this.Executor == null) return "(NoExecutorAssignedYetJustInstantiatedFromDll)";
				Dictionary<string, IndicatorParameter> merged = this.IndicatorsParametersInitializedInDerivedConstructorByNameForSliders;
				if (merged.Count == 0) return "(NoIndicatorParameters)";
				string ret = "";
				foreach (string indicatorDotParameter in merged.Keys) {
					ret += indicatorDotParameter + "=" + merged[indicatorDotParameter].ValueCurrent + ",";
				}
				ret = ret.TrimEnd(",".ToCharArray());
				return "(" + ret + ")";
			} }
		public List<Indicator> IndicatorsInitializedInDerivedConstructor { get {
				List<Indicator> ret = new List<Indicator>();
				
				Type myChild = this.GetType();
				//PropertyInfo[] lookingForIndicators = myChild.GetProperties();
				FieldInfo[] lookingForIndicators = myChild.GetFields(
															  BindingFlags.Public
															| BindingFlags.NonPublic
															| BindingFlags.DeclaredOnly
															| BindingFlags.Instance
														);
				foreach (FieldInfo indicatorCandidate in lookingForIndicators) {
					Type indicatorConcreteType = indicatorCandidate.FieldType;
					bool isIndicatorChild = typeof(Indicator).IsAssignableFrom(indicatorConcreteType);
					if (isIndicatorChild == false) continue;
					Indicator indicatorInstance = null;
					object expectingConstructedNonNull = indicatorCandidate.GetValue(this);
					if (expectingConstructedNonNull == null) {
						string msg = "INDICATOR_DECLARED_BUT_NOT_CREATED+ASSIGNED_IN_CONSTRUCTOR Script[" + this.ToString();// + "].[" + variableIndicator.Name + "]";
						Assembler.PopupException(msg);
						continue;
					}
					Indicator variableIndicator = expectingConstructedNonNull as Indicator;
					// if Script constructed a ScriptParameter with "null" as it second ctor() parameter, take introspected variable name as declared in Script
					if (string.IsNullOrEmpty(variableIndicator.Name)) {
//						string msg = "IT_WILL_WORK_RELAX WHERE_DID_INDICATORS_NAME_SET_TO_A_RANDOM_NAME_HAS_GONE??? IVE_EXPLICITLY_SET_IT_FOR_SECOND_LEVEL_INTROSPECTION_NAMING_NOT_TO_FAIL MaFast.Period; ";
//						Assembler.PopupException(msg);
						variableIndicator.Name = indicatorCandidate.Name;
					}
					ret.Add(variableIndicator);
				}
				return ret;
			} }
		public SortedDictionary<int, ScriptParameter> ScriptParametersInitializedInDerivedConstructor { get {
				SortedDictionary<int, ScriptParameter> ret = new SortedDictionary<int, ScriptParameter>();
				
				Type myChild = this.GetType();
				//PropertyInfo[] lookingForScriptParameters = myChild.GetProperties();
				FieldInfo[] lookingForScriptParameters = myChild.GetFields(
															  BindingFlags.Public
															| BindingFlags.NonPublic
															| BindingFlags.DeclaredOnly
															| BindingFlags.Instance
														);
				foreach (FieldInfo scriptParameterCandidate in lookingForScriptParameters) {
					Type scriptParameterConcreteType = scriptParameterCandidate.FieldType;
					bool isIndicatorChild = typeof(ScriptParameter).IsAssignableFrom(scriptParameterConcreteType);
					if (isIndicatorChild == false) continue;
					ScriptParameter scriptParameterInstance = null;
					object expectingConstructedNonNull = scriptParameterCandidate.GetValue(this);
					if (expectingConstructedNonNull == null) {
						string msg = "SCRIPT_PARAMETER_DECLARED_BUT_NOT_CREATED+ASSIGNED_IN_CONSTRUCTOR Script[" + this.ToString();// + "].[" + variableIndicator.Name + "]";
						Assembler.PopupException(msg);
						continue;
					}
					ScriptParameter variableScriptParameter = expectingConstructedNonNull as ScriptParameter;
					// if Script constructed a ScriptParameter with "null" as it second ctor() parameter, take introspected variable name as declared in Script
					if (string.IsNullOrEmpty(variableScriptParameter.Name)) variableScriptParameter.Name = scriptParameterCandidate.Name;
					ret.Add(variableScriptParameter.Id, variableScriptParameter);
				}
				return ret;
			} }
		public void IndicatorsInitializeAbsorbParamsFromJsonStoreInSnapshot() {
			this.Executor.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances.Clear();
			bool strategySaveRequired = false;
			foreach (Indicator indicatorInstance in this.IndicatorsInitializedInDerivedConstructor) {
				if (this.Strategy.ScriptContextCurrent.IndicatorParametersByName.ContainsKey(indicatorInstance.Name)) {
					string msg = "IndicatorsInitializedInDerivedConstructor are getting initialized from ContextCurrent and will be kept in sync with user clicks"
						+ "; ScriptContextCurrent.IndicatorParametersByName are assigned to PanelSlider.Tag and click will save to JSON by StrategyRepo.Save(Strategy)";
					List<IndicatorParameter> iParamsCtx = this.Strategy.ScriptContextCurrent.IndicatorParametersByName[indicatorInstance.Name];
					Dictionary<string, IndicatorParameter> iParamsCtxLookup = new Dictionary<string, IndicatorParameter>();
					foreach (IndicatorParameter iParamCtx in iParamsCtx) iParamsCtxLookup.Add(iParamCtx.Name, iParamCtx);

					foreach (IndicatorParameter iParamInstantiated in indicatorInstance.ParametersByName.Values) {
						if (iParamsCtxLookup.ContainsKey(iParamInstantiated.Name) == false) {
							msg = "JSONStrategy_UNCHANGED_BUT_INDICATOR_EVOLVED_AND_INRODUCED_NEW_PARAMETER__APPARENTLY_STORING_DEFAULT_VALUE_IN_CURRENT_CONTEXT"
								+ "; CLONE_OF_INSTANTIATED_GOES_TO_CONTEXT_AND_TO_SLIDER__THIS_CLONE_HAS_SHORTER_LIFECYCLE_WILL_REMAIN_IN_SYNC_FROM_WITHIN_CLICK_HANLDER";
							iParamsCtx.Add(iParamInstantiated.Clone());
							continue;
						}
						msg = "ABSORBING_CONTEXT_INDICATOR_VALUE_INTO_INSTANTIATED_INDICATOR_PARAMETER";
						IndicatorParameter iParamFoundCtx = iParamsCtxLookup[iParamInstantiated.Name];
						iParamInstantiated.AbsorbCurrentFixBoundariesIfChanged(iParamFoundCtx);

						//WRONG_CONTEXT_AND_SLIDER_ARE_SAME__KEEPING_INSTANTIATED_CHANGING_SEPARATELY 
						/*if (iParamInstantiated != iParamFoundCtx) {
							#if DEBUG
							Debugger.Break();			//NOPE_ITS_A_CLONE
							#endif
							iParamsCtx.Remove(iParamFoundCtx);	// instead of JsonDeserialized,
							iParamsCtx.Add(iParamInstantiated);	// ...put Instantiated into the Context
						} */
					}
				} else {
					string msg = "JSONStrategy_JUST_ADDED_NEW_INDICATOR_WITH_ITS_NEW_PARAMETERS[" + indicatorInstance.Name + "]";
					Assembler.PopupException(msg, null, false);
					this.Strategy.ScriptContextCurrent.IndicatorParametersByName.Add(indicatorInstance.Name, new List<IndicatorParameter>(indicatorInstance.ParametersByName.Values));
					strategySaveRequired = true;
				}

				this.Executor.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances.Add(indicatorInstance.Name, indicatorInstance);
				
				// moved from upstairs coz: after absorbing all deserialized indicator parameters from ScriptContext, GetHostPanelForIndicator will return an pre-instantiated PanelIndicator
				// otherwize GetHostPanelForIndicator created a new one for an indicator with default Indicator parameters;
				// example: MultiSplitterPropertiesByPanelName["ATR (Period:9[1..11/2])"] exists, while default Period for ATR is 5 => new PanelIndicator will be created
				// final goal is to avoid (splitterPropertiesByPanelName.Count != this.panels.Count) in SplitterPropertiesByPanelNameSet() and (splitterFound == null)  
				HostPanelForIndicator priceOrItsOwnPanel = this.Executor.ChartConditionalHostPanelForIndicatorGet(indicatorInstance);
				indicatorInstance.Initialize(priceOrItsOwnPanel);
			}
			if (strategySaveRequired) this.Strategy.Serialize();
		}
		#endregion

		public void AbsorbScriptAndIndicatorParametersFromSelfCloneConstructed() {
			object selfCloneConstructed = Activator.CreateInstance(this.GetType());	//default ctor invoked where developer is supposed to add ScriptAndIndicatorParameters into new This
			Script clone = selfCloneConstructed as Script;
			if (clone == null) {
				string msg = "Activator.CreateInstance(" + this.GetType().ToString() + " as Script == null";
				Assembler.PopupException(msg);
				return;
			}

			// first half of the job
			SortedDictionary<int, ScriptParameter> cloneScriptParametersFrom = clone.ScriptParametersById_ReflectedCached;
			Dictionary<int, ScriptParameter>	   myctxScriptParametersTo	 = this.Strategy.ScriptContextCurrent.ScriptParametersById;
			foreach (int cloneSPindex in cloneScriptParametersFrom.Keys) {
				ScriptParameter cloneSparam = cloneScriptParametersFrom[cloneSPindex];
				if (myctxScriptParametersTo.ContainsKey(cloneSPindex) == false) {
					string msg = "myctxScriptParametersTo.ContainsKey(" + cloneSPindex + ") == false; Script.ScriptParametersAsString=" + this.ScriptParametersByIdAsString;
					Assembler.PopupException(msg);
					//continue;
					myctxScriptParametersTo.Add(cloneSPindex, cloneSparam.Clone());
				} else {
					myctxScriptParametersTo[cloneSPindex].AbsorbCurrentFixBoundariesIfChanged(cloneSparam);
				}
			}

			// second half of the job
			List<Indicator> cloneIndicators = clone.IndicatorsInitializedInDerivedConstructor;
			Dictionary<string, List<IndicatorParameter>> cloneIndicatorsFrom = new Dictionary<string, List<IndicatorParameter>>();
			foreach (Indicator cloneI in cloneIndicators) cloneIndicatorsFrom.Add(cloneI.Name, new List<IndicatorParameter>(cloneI.ParametersByName.Values));

			Dictionary<string, List<IndicatorParameter>> myctxIndicatorsTo	= this.Strategy.ScriptContextCurrent.IndicatorParametersByName;
			foreach (string cloneIname in cloneIndicatorsFrom.Keys) {
				List<IndicatorParameter> cloneIparams = cloneIndicatorsFrom[cloneIname];
				if (myctxIndicatorsTo.ContainsKey(cloneIname) == false) {
					string msg = "myctxIndicatorsTo.ContainsKey(" + cloneIname + ") == false; Script.IndicatorParametersAsString=" + this.IndicatorParametersAsString;
					Assembler.PopupException(msg);
					//continue;
					myctxIndicatorsTo.Add(cloneIname, new List<IndicatorParameter>());
				}
				List<IndicatorParameter> myctxIparams = myctxIndicatorsTo[cloneIname];
				Dictionary<string, IndicatorParameter> myctxIparamsLookup = new Dictionary<string, IndicatorParameter>();
				foreach (IndicatorParameter myctxIparam in myctxIparams) myctxIparamsLookup.Add(myctxIparam.Name, myctxIparam);
				foreach (IndicatorParameter cloneIparam in cloneIparams) {
					if (myctxIparamsLookup.ContainsKey(cloneIparam.Name) == false) {
						string msg = "myctxIparamsLookup.ContainsKey(" + cloneIparam.Name + ") == false";
						Assembler.PopupException(msg);
						//continue;
						myctxIparams.Add(cloneIparam.Clone());
					} else {
						IndicatorParameter myctxIparam = myctxIparamsLookup[cloneIparam.Name];
						myctxIparam.AbsorbCurrentFixBoundariesIfChanged(cloneIparam);
					}
				}
			}
			this.IndicatorsInitializeAbsorbParamsFromJsonStoreInSnapshot();
		}
		public override string ToString() {
			string ret = "Script[" + this.GetType().Name + "].Strategy";
			if (this.Strategy == null) {
				ret += "[NULL_NONSENSE!!!]";
			} else {
				ret += ".ScriptContextCurrent[" + this.Strategy.ScriptContextCurrent.Name + "]";
			}
			return ret;
		}
	}
}
