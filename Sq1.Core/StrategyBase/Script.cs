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
		protected	Bars			Bars								{ get { return (this.Executor == null) ? null : this.Executor.Bars; } }
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
				return this.Executor.ExecutionDataSnapshot.PositionsOpenNow.LastNullUnsafe(this, "//LastPositionOpenNowWAIT");
			} }
		public		Position		LastPosition						{ get {
				return this.Executor.ExecutionDataSnapshot.PositionsMaster.LastNullUnsafe(this, "//LastPositionWAIT");
			} }
		public		bool			HasAlertsPendingAndPositionsOpenNow	{ get { return this.HasAlertsPending && this.HasPositionsOpenNow; } }
		public		bool			HasAlertsPendingOrPositionsOpenNow	{ get { return this.HasAlertsPending || this.HasPositionsOpenNow; } }
		public		bool			HasAlertsPending					{ get { return (this.Executor.ExecutionDataSnapshot.AlertsPending.Count > 0); } }
		public		bool			HasPositionsOpenNow					{ get { return (this.Executor.ExecutionDataSnapshot.PositionsOpenNow.Count > 0); } }
		#endregion
		
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
			scriptParametersById_ReflectedCached = new SortedDictionary<int, ScriptParameter>();
			indicatorsByName_ReflectedCached = new Dictionary<string, Indicator>();
			indicatorsParameters_ReflectedCached = new Dictionary<string, IndicatorParameter>();
			indicatorParametersByIndicator_ReflectedCached = new Dictionary<string, List<IndicatorParameter>>();
		}

		#region Initializers
		public void Initialize(ScriptExecutor scriptExecutor) {
			this.Executor = scriptExecutor;

			this.scriptParametersById_ReflectionForced = true;
			this.indicatorsByName_ReflectionForced = true;
			this.indicatorParameters_ReflectionForced = true;
			this.indicatorParametersByIndicator_ReflectionForced = true;

			//v1
			this.IndicatorParamsAbsorbMergeFromReflected_InitializeIndicatorsWithHostPanel();
			this.Executor.Strategy.ScriptParametersAbsorbMergeFromReflected_StoreInCurrentContext_SaveStrategy_notSameObjects_usedForResettingToDefault();
			//v2
			//this.AbsorbValuesFromCurrentContextAndReplacePointers();
		}

		//private void AbsorbValuesFromCurrentContextAndReplacePointers() {
		//	ContextScript jsonDeserialized = this.Executor.Strategy.ScriptContextCurrent;
		//	foreach (ScriptParameter param in this.ScriptParametersById_ReflectedCached.Values) {
		//		if (jsonDeserialized.
		//	}
		//}
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
		public	string IndicatorParametersAsString { get {
				if (this.Executor == null) return "(NoExecutorAssignedYetJustInstantiatedFromDll)";
				Dictionary<string, IndicatorParameter> merged = this.IndicatorsParameters_ReflectedCached;
				if (merged.Count == 0) return "(NoIndicatorParameters)";
				string ret = "";
				foreach (string indicatorDotParameter in merged.Keys) {
					ret += indicatorDotParameter + "=" + merged[indicatorDotParameter].ValueCurrent + ",";
				}
				ret = ret.TrimEnd(",".ToCharArray());
				return "(" + ret + ")";
			} }


				bool indicatorParametersByIndicator_ReflectionForced;
				Dictionary<string, List<IndicatorParameter>>	indicatorParametersByIndicator_ReflectedCached;
		public	Dictionary<string, List<IndicatorParameter>>	IndicatorParametersByIndicator_ReflectedCached { get {
				if (indicatorParametersByIndicator_ReflectionForced == false) return this.indicatorParametersByIndicator_ReflectedCached;
				indicatorParametersByIndicator_ReflectionForced = false;
				indicatorParametersByIndicator_ReflectedCached.Clear();

				foreach (string indicatorName in this.IndicatorsByName_ReflectedCached.Keys) {
					Indicator indicator = this.IndicatorsByName_ReflectedCached[indicatorName];
					List<IndicatorParameter> indicatorParameters = new List<IndicatorParameter>(indicator.ParametersByName.Values);
					indicatorParametersByIndicator_ReflectedCached.Add(indicator.Name, indicatorParameters);
				}
				return this.IndicatorParametersByIndicator_ReflectedCached;
			} }


				bool indicatorParameters_ReflectionForced;
				Dictionary<string, IndicatorParameter> indicatorsParameters_ReflectedCached;
		public	Dictionary<string, IndicatorParameter> IndicatorsParameters_ReflectedCached { get {
				if (indicatorParameters_ReflectionForced == false) return this.indicatorsParameters_ReflectedCached;
				indicatorParameters_ReflectionForced = false;
				indicatorsParameters_ReflectedCached.Clear();

				foreach (Indicator indicator in this.IndicatorsByName_ReflectedCached.Values) {
					foreach (string parameterName in indicator.ParametersByName.Keys) {
						IndicatorParameter indicatorParameter = indicator.ParametersByName[parameterName];
						if (indicatorParameter.FullName.StartsWith(indicator.Name) == false) {
							string msg = "FIXME_NOW!!! YOU_MUST_TRIGGER_Script.IndicatorsByName_ReflectedCached_AND_THEN_Script.IndicatorsParameters_ReflectedCached"
								+ " Indicator.ParametersByName_SHOULD_HAVE_SET_indicatorParameterInstance.IndicatorName = this.Name;"
								+ " Script.IndicatorsByName_ReflectedCached Indicator.Name=<Script's variable name>";
							Assembler.PopupException(msg);
							indicatorParameter.IndicatorName = indicator.Name;
						}
						this.indicatorsParameters_ReflectedCached.Add(indicatorParameter.FullName, indicatorParameter);
					}
				}
				return this.indicatorsParameters_ReflectedCached;
			} }

				bool indicatorsByName_ReflectionForced;
				Dictionary<string, Indicator>	indicatorsByName_ReflectedCached;
		public	Dictionary<string, Indicator>	IndicatorsByName_ReflectedCached { get {
				if (indicatorsByName_ReflectionForced == false) return this.indicatorsByName_ReflectedCached;
				indicatorsByName_ReflectionForced = false;
				this.indicatorsByName_ReflectedCached.Clear();
				
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
					object expectingConstructedNonNull = indicatorCandidate.GetValue(this);
					if (expectingConstructedNonNull == null) {
						string msg = "YOU_MUST_ASSIGN_INDICATOR_IN_CTOR " + this.GetType().Name + "() { this." + indicatorCandidate.Name + " = new " + indicatorCandidate.GetType() + "(...); }"
							//"INDICATOR_DECLARED_BUT_NOT_CREATED+ASSIGNED_IN_CONSTRUCTOR Script[" + this.ToString();// + "].[" + variableIndicator.Name + "]";
							;
						Assembler.PopupException(msg);
						continue;
					}
					Indicator variableIndicator = expectingConstructedNonNull as Indicator;
					variableIndicator.Name = indicatorCandidate.Name;		// after Script.ctor() { this.MAfast = new IndicatorMovingAverageSimple(); } I set this.Mafast.Name="Mafast" instead of "MA";
					this.indicatorsByName_ReflectedCached.Add(variableIndicator.Name, variableIndicator);
				}
				return this.indicatorsByName_ReflectedCached;
			} }

				bool scriptParametersById_ReflectionForced;
				SortedDictionary<int, ScriptParameter> scriptParametersById_ReflectedCached;
		public	SortedDictionary<int, ScriptParameter> ScriptParametersById_ReflectedCached { get {
				if (scriptParametersById_ReflectionForced == false) return scriptParametersById_ReflectedCached;
				scriptParametersById_ReflectionForced = false;
				scriptParametersById_ReflectedCached.Clear();
				
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
					object expectingConstructedNonNull = scriptParameterCandidate.GetValue(this);
					if (expectingConstructedNonNull == null) {
						string msg = "YOU_MUST_ASSIGN_INDICATOR_IN_CTOR " + this.GetType().Name + "() { this." + scriptParameterCandidate.Name + " = new " + scriptParameterCandidate.GetType() + "(...); }";
							//+ "SCRIPT_PARAMETER_DECLARED_BUT_NOT_CREATED+ASSIGNED_IN_CONSTRUCTOR Script[" + this.ToString();// + "].[" + variableIndicator.Name + "]";
							;
						Assembler.PopupException(msg);
						continue;
					}
					ScriptParameter variableScriptParameter = expectingConstructedNonNull as ScriptParameter;
					// if Script constructed a ScriptParameter with "null" as it second ctor() parameter, take introspected variable name as declared in Script
					if (string.IsNullOrEmpty(variableScriptParameter.Name)) variableScriptParameter.Name = scriptParameterCandidate.Name;
					try {
						scriptParametersById_ReflectedCached.Add(variableScriptParameter.Id, variableScriptParameter);
					} catch (Exception ex) {
						string msg = "SCRIPT_PARAMETER_ID_SHOULD_BE_UNIQUE_FOR_EACH id[" + variableScriptParameter.Id
							+ "] is declared by two parameters 1) [" + scriptParametersById_ReflectedCached[variableScriptParameter.Id] + "] 2) [" + variableScriptParameter + "]";
						Assembler.PopupException(msg, null, false);
					}
				}
				return scriptParametersById_ReflectedCached;
			} }

		public void IndicatorParamsAbsorbMergeFromReflected_InitializeIndicatorsWithHostPanel() {
			this.Strategy.IndicatorParamsAbsorbMergeFromReflected_StoreInCurrenctContext_SaveStrategy();
			foreach (Indicator indicatorInstance in this.IndicatorsByName_ReflectedCached.Values) {
				// moved from upstairs coz: after absorbing all deserialized indicator parameters from ScriptContext, GetHostPanelForIndicator will return an pre-instantiated PanelIndicator
				// otherwize GetHostPanelForIndicator created a new one for an indicator with default Indicator parameters;
				// example: MultiSplitterPropertiesByPanelName["ATR (Period:9[1..11/2])"] exists, while default Period for ATR is 5 => new PanelIndicator will be created
				// final goal is to avoid (splitterPropertiesByPanelName.Count != this.panels.Count) in SplitterPropertiesByPanelNameSet() and (splitterFound == null)  
				HostPanelForIndicator priceOrItsOwnPanel = this.Executor.ChartConditionalHostPanelForIndicatorGet(indicatorInstance);
				indicatorInstance.Initialize(priceOrItsOwnPanel);
			}
		}
		#endregion

		public void SwitchToDefaultContextByAbsorbingScriptAndIndicatorParametersFromSelfCloneConstructed() {
			object selfCloneConstructed = Activator.CreateInstance(this.GetType());	//default ctor invoked where developer is supposed to add ScriptAndIndicatorParameters into new This
			Script clone = selfCloneConstructed as Script;
			if (clone == null) {
				string msg = "Activator.CreateInstance(" + this.GetType().ToString() + " as Script == null";
				Assembler.PopupException(msg);
				return;
			}

			// first half of the job
			SortedDictionary<int, ScriptParameter>	cloneScriptParametersFrom	= clone.ScriptParametersById_ReflectedCached;
			SortedDictionary<int, ScriptParameter>	myctxScriptParametersTo		= this.Strategy.ScriptContextCurrent.ScriptParametersById;
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
			List<Indicator> cloneIndicators = new List<Indicator>(clone.IndicatorsByName_ReflectedCached.Values);
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
			this.IndicatorParamsAbsorbMergeFromReflected_InitializeIndicatorsWithHostPanel();
		}
		public override string ToString() {
			string ret = "Script[" + this.GetType().Name + "].Strategy";
			if (this.Strategy == null) {
				ret += "[STRATEGY_NULL_NO_CTX_NONSENSE!!!]";
			} else {
				ret += ".ScriptContextCurrent[" + this.Strategy.ScriptContextCurrent.Name + "]";
			}
			return ret;
		}
	}
}
