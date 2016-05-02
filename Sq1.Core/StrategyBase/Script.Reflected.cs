using System;
using System.Collections.Generic;
using System.Reflection;

using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;

namespace Sq1.Core.StrategyBase {
	public partial class Script {
		public	string IndicatorParametersAsString { get {
			if (this.Executor == null) return "(NoExecutorAssignedYetJustInstantiatedFromDll)";
			Dictionary<string, IndicatorParameter> merged = this.IndicatorsParameters_reflectedCached;
			if (merged.Count == 0) return "(NoIndicatorParameters)";
			string ret = "";
			foreach (string indicatorDotParameter in merged.Keys) {
				ret += indicatorDotParameter + "=" + merged[indicatorDotParameter].ValueCurrent + ",";
			}
			ret = ret.TrimEnd(",".ToCharArray());
			return "(" + ret + ")";
		} }

		public void  IndicatorParametersByIndicator_reflectionForced_byClearingCache() {
				this.indicatorParametersByIndicator_reflectedCached = null;
		}
				Dictionary<string, List<IndicatorParameter>>	indicatorParametersByIndicator_reflectedCached;
		public	Dictionary<string, List<IndicatorParameter>>	IndicatorParametersByIndicatorName_reflectedCached { get {
			if (this.indicatorParametersByIndicator_reflectedCached != null) {
				return this.indicatorParametersByIndicator_reflectedCached;
			}

			this.indicatorParametersByIndicator_reflectedCached = new Dictionary<string,List<IndicatorParameter>>();

			foreach (string indicatorName in this.IndicatorsByName_reflectedCached_primary.Keys) {
				Indicator indicator = this.IndicatorsByName_reflectedCached_primary[indicatorName];
				List<IndicatorParameter> indicatorParameters = new List<IndicatorParameter>(indicator.ParametersByName.Values);
				this.indicatorParametersByIndicator_reflectedCached.Add(indicator.Name, indicatorParameters);
			}
			return this.IndicatorParametersByIndicatorName_reflectedCached;
		} }

		public void  IndicatorParameters_reflectionForced_byClearingCache() {
				this.indicatorsParameters_reflectedCached = null;
		}
				Dictionary<string, IndicatorParameter> indicatorsParameters_reflectedCached;
		public	Dictionary<string, IndicatorParameter> IndicatorsParameters_reflectedCached { get {
			if (this.indicatorsParameters_reflectedCached != null) {
				return this.indicatorsParameters_reflectedCached;
			}
		
			this.indicatorsParameters_reflectedCached = new Dictionary<string,IndicatorParameter>();

			foreach (Indicator indicator in this.IndicatorsByName_reflectedCached_primary.Values) {
				foreach (string parameterName in indicator.ParametersByName.Keys) {
					IndicatorParameter indicatorParameter = indicator.ParametersByName[parameterName];
					if (indicatorParameter.FullName.StartsWith(indicator.Name) == false) {
						string msg = "FIXME_NOW!!! YOU_MUST_TRIGGER_Script.IndicatorsByName_ReflectedCached_AND_THEN_Script.IndicatorsParameters_ReflectedCached"
							+ " Indicator.ParametersByName_SHOULD_HAVE_SET_indicatorParameterInstance.IndicatorName = this.Name;"
							+ " Script.IndicatorsByName_ReflectedCached Indicator.Name=<Script's variable name>";
						Assembler.PopupException(msg);
						indicatorParameter.IndicatorName = indicator.Name;
					}
					indicatorParameter.Owner_asString = "REFLECTED_FOR_INDICATOR[" + indicator.ToString() + "] script.Strategy[" + this.StrategyName + "]";
					this.indicatorsParameters_reflectedCached.Add(indicatorParameter.FullName, indicatorParameter);
				}
			}
			return this.indicatorsParameters_reflectedCached;
		} }

		public void  IndicatorsByName_reflectionForced_byClearingCache() {
				this.indicatorsByName_reflectedCached = null;
		}
				Dictionary<string, Indicator>	indicatorsByName_reflectedCached;
		public	Dictionary<string, Indicator>	IndicatorsByName_reflectedCached_primary { get {
			if (this.indicatorsByName_reflectedCached != null) {
				if (this.indicatorsByName_reflectedCached.Count == 0) {
					int a = 1;
				}
				return this.indicatorsByName_reflectedCached;
			}

			this.indicatorsByName_reflectedCached = new Dictionary<string,Indicator>();
				
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
				Indicator indicatorReflected = expectingConstructedNonNull as Indicator;
				indicatorReflected.Name = indicatorCandidate.Name;		// after Script.ctor() { this.MAfast = new IndicatorMovingAverageSimple(); } I set this.Mafast.Name="Mafast" instead of "MA";
				this.indicatorsByName_reflectedCached.Add(indicatorReflected.Name, indicatorReflected);
			}
			if (this.indicatorsByName_reflectedCached.Count == 0) {
				int a = 1;
			}
			return this.indicatorsByName_reflectedCached;
		} }

		public void  ScriptParametersById_reflectionForced_byClearingCache() {
				this.scriptParametersById_reflectedCached = null;
		}
				SortedDictionary<int, ScriptParameter> scriptParametersById_reflectedCached;
		public	SortedDictionary<int, ScriptParameter> ScriptParametersById_reflectedCached_primary { get {
			if (this.scriptParametersById_reflectedCached != null) {
				return this.scriptParametersById_reflectedCached;
			}

			this.scriptParametersById_reflectedCached = new SortedDictionary<int,ScriptParameter>();
				
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
				ScriptParameter scriptParameterReflected = expectingConstructedNonNull as ScriptParameter;
				// if Script constructed a ScriptParameter with "null" as it second ctor() parameter, take introspected variable name as declared in Script
				if (string.IsNullOrEmpty(scriptParameterReflected.Name)) scriptParameterReflected.Name = scriptParameterCandidate.Name;
				scriptParameterReflected.Owner_asString = "REFLECTED_FOR_SCRIPT[" + this.StrategyName + "]";
				try {
					this.scriptParametersById_reflectedCached.Add(scriptParameterReflected.Id, scriptParameterReflected);
				} catch (Exception ex) {
					string msg = "SCRIPT_PARAMETER_ID_SHOULD_BE_UNIQUE_FOR_EACH id[" + scriptParameterReflected.Id
						+ "] is declared by two parameters"
						+ " 1) [" + this.scriptParametersById_reflectedCached[scriptParameterReflected.Id] + "]"
						+ " 2) [" + scriptParameterReflected + "]";
					Assembler.PopupException(msg, ex, false);
				}
			}
			return this.scriptParametersById_reflectedCached;
		} }

		public void InitializeIndicatorsReflected_withHostPanel() {
			foreach (Indicator indicatorInstance in this.IndicatorsByName_reflectedCached_primary.Values) {
				// moved from upstairs coz: after absorbing all deserialized indicator parameters from ScriptContext, GetHostPanelForIndicator will return an pre-instantiated PanelIndicator
				// otherwize GetHostPanelForIndicator created a new one for an indicator with default Indicator parameters;
				// example: MultiSplitterPropertiesByPanelName["ATR (Period:9[1..11/2])"] exists, while default Period for ATR is 5 => new PanelIndicator will be created
				// final goal is to avoid (splitterPropertiesByPanelName.Count != this.panels.Count) in SplitterPropertiesByPanelNameSet() and (splitterFound == null)  
				HostPanelForIndicator priceOrItsOwnPanel = this.Executor.ChartConditional_hostPanelForIndicatorGet(indicatorInstance);
				indicatorInstance.SetHostPanel(priceOrItsOwnPanel);
			}
		}

		internal int ScriptParametersReflected_absorbFromCurrentContext_pushBackToCurrentContext(SortedDictionary<int, ScriptParameter> scriptParametersById_fromCtx) {
			string msig = " //ScriptParametersReflected_absorbFromCurrentContext_pushBackToCurrentContext()";
			int ret = 0;
			if (scriptParametersById_fromCtx.Count == 0) return ret;
			foreach (ScriptParameter spReflected in this.ScriptParametersById_reflectedCached_primary.Values) {
				if (scriptParametersById_fromCtx.ContainsKey(spReflected.Id) == false) continue;
				ScriptParameter spContext = scriptParametersById_fromCtx[spReflected.Id];
				bool valueCurrentAbsorbed = spReflected.AbsorbCurrent_fixBoundaries_from(spContext);
				if (valueCurrentAbsorbed) ret++;

				// pushing back, so that the next step is to set the same pointer to Slider.Tag for change-by-click
				if (scriptParametersById_fromCtx[spReflected.Id] != spReflected) {
					scriptParametersById_fromCtx[spReflected.Id]  = spReflected;
				}
			}
			//YOU_JUST_SYNCHED_IN_TWO_INNER_LOOPS__WHY_DO_YOU_OVERWRITE_BRO???? this.ScriptParametersById = scriptParametersById_reflectedCached;
			//bool dontSaveWeSequence = this.Name.Contains(Sequencer.ITERATION_PREFIX);
			//if (dontSaveWeSequence) {
			//    string msg = "SCRIPT_RECOMPILED_ADDING_MORE_PARAMETERS_THAN_SEQUENCER_PROVIDED_IN_SCRIPTCONTEXT #1";
			//    Assembler.PopupException(msg + msig, null, true);
			//    //strategySerializeRequired = false;
			//}
			if (ret == 0) {
				string msg = "NO_SCRIPT_PARAMETER_VALUES_ABSORBED_SAME_FOR_Count[" + scriptParametersById_fromCtx.Count + "]";
				//Assembler.PopupException(msg, null, false);
			}
			return ret;
		}
		internal int IndicatorParametersReflected_absorbFromCurrentContext_pushBackToCurrentContext(Dictionary<string, List<IndicatorParameter>> indicatorParametersByIndicatorName_fromCtx) {
			string msig = " //IndicatorParametersReflected_absorbFromCurrentContext_pushBackToCurrentContext()";
			int ret = 0;
			if (indicatorParametersByIndicatorName_fromCtx.Count == 0) return ret;
			foreach (string indicatorName in this.IndicatorParametersByIndicatorName_reflectedCached.Keys) {
				if (indicatorParametersByIndicatorName_fromCtx.ContainsKey(indicatorName) == false) continue;
				List<IndicatorParameter> iParamsReflected	= this.IndicatorParametersByIndicatorName_reflectedCached	[indicatorName];
				List<IndicatorParameter> iParamsCtx			=	   indicatorParametersByIndicatorName_fromCtx			[indicatorName];

				int parametersForIndicator_absorbed = 0;
				foreach (IndicatorParameter iParamCtx in iParamsCtx) {
					//DIFFERENT_POINTERS_100%_COMPARING_BY_NAME_IN_LOOP if (iParamsReflected.Contains(iParamInstantiated) == false) continue;
					foreach (IndicatorParameter iParamReflected in iParamsReflected) {
						//v1 WILL_ALWAYS_CONTINUE_KOZ_iParamCtx.IndicatorName="NOT_ATTACHED_TO_ANY_INDICATOR"__LAZY_TO_SET_AFTER_DESERIALIZATION_KOZ_WILL_THROW_IT_10_LINES_BELOW_AND_PARAM_NAMES_ARE_UNIQUE_WITHIN_INDICATOR if (iParamReflected.FullName != iParamCtx.FullName) {
						if (iParamReflected.Name != iParamCtx.Name) {
							string msg = "iParamReflected[" + iParamReflected	.ToString() + "].Name[" + iParamReflected	.Name + "]"
								   + " != iParamCtx["		+ iParamCtx			.ToString() + "].Name[" + iParamCtx			.Name + "]";
							Assembler.PopupException(msg);
							continue;
						}
						bool valueCurrentAbsorbed = iParamReflected.AbsorbCurrent_fixBoundaries_from(iParamCtx);
						if (valueCurrentAbsorbed) parametersForIndicator_absorbed++;
					}
				}
				//if (parametersForIndicator_absorbed > 0) {
					this.IndicatorsByName_reflectedCached_primary[indicatorName].ParametersAsStringShort_forceRecalculate = true;
				//}
				ret += parametersForIndicator_absorbed;

				// pushing back, so that the next step is to set the same pointer to Slider.Tag for change-by-click
				if (indicatorParametersByIndicatorName_fromCtx[indicatorName] != iParamsReflected) {
					indicatorParametersByIndicatorName_fromCtx[indicatorName]  = iParamsReflected;
				}
			}
			//YOU_JUST_SYNCHED_IN_TWO_INNER_LOOPS__WHY_DO_YOU_OVERWRITE_BRO???? this.IndicatorParametersByName = indicatorParametersByIndicator_ReflectedCached;

			//bool dontSaveWeSequence = this.Name.Contains(Sequencer.ITERATION_PREFIX);
			//if (dontSaveWeSequence) {
			//    string msg = "SCRIPT_RECOMPILED_ADDING_MORE_PARAMETERS_THAN_SEQUENCER_PROVIDED_IN_SCRIPTCONTEXT #2";
			//    Assembler.PopupException(msg + msig, null, true);
			//    //strategySerializeRequired = false;
			//}
			if (ret == 0) {
				string msg = "NO_INDICATOR_PARAMETER_VALUES_ABSORBED_SAME_FOR_Count[" + indicatorParametersByIndicatorName_fromCtx.Count + "]";
				//Assembler.PopupException(msg, null, false);
			}
			return ret;
		}
	}
}
