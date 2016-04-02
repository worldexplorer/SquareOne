using System;
using System.Collections.Generic;
using System.Reflection;

using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;

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
				return this.Executor.ExecutionDataSnapshot.PositionsOpenNow.Last_nullUnsafe(this, "//LastPositionOpenNowWAIT");
			} }
		public		Position		LastPosition						{ get {
				return this.Executor.ExecutionDataSnapshot.PositionsMaster.Last_nullUnsafe(this, "//LastPositionWAIT");
			} }
		public		bool			HasAlertsPendingAndPositionsOpenNow	{ get { return this.HasAlertsPending && this.HasPositionsOpenNow; } }
		public		bool			HasAlertsPendingOrPositionsOpenNow	{ get { return this.HasAlertsPending || this.HasPositionsOpenNow; } }
		public		bool			HasAlertsPending					{ get { return (this.Executor.ExecutionDataSnapshot.AlertsPending.Count > 0); } }
		public		bool			HasPositionsOpenNow					{ get { return (this.Executor.ExecutionDataSnapshot.PositionsOpenNow.Count > 0); } }
		#endregion
		
		public	string				ScriptParametersAsString		{ get {
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
			scriptParametersById_ReflectedCached			= new SortedDictionary<int, ScriptParameter>();
			indicatorsByName_ReflectedCached				= new Dictionary<string, Indicator>();
			indicatorsParameters_ReflectedCached			= new Dictionary<string, IndicatorParameter>();
			indicatorParametersByIndicator_ReflectedCached	= new Dictionary<string, List<IndicatorParameter>>();
		}

		#region Initializers
		public void Initialize(ScriptExecutor scriptExecutor, bool saveStrategy_falseForSequencer = true) {
			this.Executor = scriptExecutor;

			this.scriptParametersById_ReflectionForced = true;
			this.indicatorsByName_ReflectionForced = true;
			this.indicatorParameters_ReflectionForced = true;
			this.indicatorParametersByIndicator_ReflectionForced = true;

			//v1
			this.Executor.Strategy.ScriptAndIndicatorParametersReflected_absorbMergeFromCurrentContext_saveStrategy(saveStrategy_falseForSequencer);
			//v2
			//this.AbsorbValuesFromCurrentContextAndReplacePointers();
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
				this.Executor.BacktesterOrLivesimulator.RequestingBacktestAbortMre.Set();
			}
		}		
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

		public void IndicatorParamsAbsorbMergeFromReflected_InitializeIndicatorsWithHostPanel(bool saveStrategy_falseForSequencer = true) {
			int parametersAbsorbed = this.Strategy.IndicatorParametersReflectedAbsorbMergeFromCurrentContext_SaveStrategy(saveStrategy_falseForSequencer);
			foreach (Indicator indicatorInstance in this.IndicatorsByName_ReflectedCached.Values) {
				// moved from upstairs coz: after absorbing all deserialized indicator parameters from ScriptContext, GetHostPanelForIndicator will return an pre-instantiated PanelIndicator
				// otherwize GetHostPanelForIndicator created a new one for an indicator with default Indicator parameters;
				// example: MultiSplitterPropertiesByPanelName["ATR (Period:9[1..11/2])"] exists, while default Period for ATR is 5 => new PanelIndicator will be created
				// final goal is to avoid (splitterPropertiesByPanelName.Count != this.panels.Count) in SplitterPropertiesByPanelNameSet() and (splitterFound == null)  
				HostPanelForIndicator priceOrItsOwnPanel = this.Executor.ChartConditional_hostPanelForIndicatorGet(indicatorInstance);
				indicatorInstance.Initialize(priceOrItsOwnPanel);
			}
		}
		#endregion

		public void SwitchToDefaultContext_byAbsorbingScriptAndIndicatorParameters_fromSelfCloneConstructed() {
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
					string msg = "myctxScriptParametersTo.ContainsKey(" + cloneSPindex + ") == false; Script.ScriptParametersAsString=" + this.ScriptParametersAsString;
					Assembler.PopupException(msg);
					//continue;
					myctxScriptParametersTo.Add(cloneSPindex, cloneSparam.CloneAsScriptParameter("REFACTOR_SwitchedToDefaultContextByAbsorbingScriptAndIndicatorParametersFromSelfCloneConstructed"));
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
						myctxIparams.Add(cloneIparam.CloneAsIndicatorParameter("REFACTOR_SwitchedToDefaultContextByAbsorbingScriptAndIndicatorParametersFromSelfCloneConstructed"));
					} else {
						IndicatorParameter myctxIparam = myctxIparamsLookup[cloneIparam.Name];
						myctxIparam.AbsorbCurrentFixBoundariesIfChanged(cloneIparam);
					}
				}
			}
			string msg2 = "YOU_JUST_SELF_CLONED_AND_ABSORBED__YOU_JUST_NEED_TO_INIT_SCRIPT_INDICATORS_WITH_NEW_INDICATOR_PARAMS_AND_HOST_PANELS";
			Assembler.PopupException(msg2);
			this.IndicatorParamsAbsorbMergeFromReflected_InitializeIndicatorsWithHostPanel();
		}
		internal void PushChangedScriptParameterValueToScript(IndicatorParameter indicatorParameterChangedDueToUserClickInSliders) {
			string msig = " //Script.PushChangedScriptParameterValueToScript(" + indicatorParameterChangedDueToUserClickInSliders + ")";
			if (indicatorParameterChangedDueToUserClickInSliders == null) {
				string msg = "I_REFUSE_TO_PUSH_PARAMETER_CLICKED_TO_SCRIPT SLIDERS_AUTOGROW_GENERATED_AN_EVENT_WITH_EMPTY_INDICATOR_PARAMETER_INSIDE";
				Assembler.PopupException(msg + msig);
				return;
			}
			SortedDictionary<string, IndicatorParameter> reflectedAll = this.scriptAndIndicatorParametersReflectedMergedUnclonedForReusableExecutorToCheckByName;
			if (reflectedAll.ContainsKey(indicatorParameterChangedDueToUserClickInSliders.FullName) == false) {
				string msg = "I_REFUSE_TO_PUSH_PARAMETER_CLICKED_TO_SCRIPT STALE_PARAMETER_CLICKED_WHICH_DOESNT_EXIST_IN_RECOMPILED_SCRIPT";
				Assembler.PopupException(msg + msig);
				return;
			}
			IndicatorParameter reflected = reflectedAll[indicatorParameterChangedDueToUserClickInSliders.FullName];
			if (reflected == indicatorParameterChangedDueToUserClickInSliders) {
				string msg = "SCRIPT_PARAMETERS_SEEMS_TO_BE_THE_SAME_OBJECTS_WHILE_INDICATOR_PARAMETERS_ARE_DIFFERENT??? CATCH_DECIDE_COPY_OR_SAME";
				Assembler.PopupException(msg);
				return;
			}
			if (reflected.ValueCurrent == indicatorParameterChangedDueToUserClickInSliders.ValueCurrent) {
				string msg = "SCRIPT_PARAMETER_VALUE_ALREADY_SAME_AS_PROPAGATING NAIL_ANOTHER_SYNC/PUSH_MECHANISM";
				Assembler.PopupException(msg);
				return;
			}
			reflected.AbsorbCurrentFixBoundariesIfChanged(indicatorParameterChangedDueToUserClickInSliders);
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

		public List<Alert> CloseAllOpenPositions_killAllPendingAlerts() {
			return this.Executor.CloseAllOpenPositions_killAllPendingAlerts();
		}
	}
}
