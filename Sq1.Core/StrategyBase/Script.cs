using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;

namespace Sq1.Core.StrategyBase {
	public partial class Script {
		public		ScriptExecutor	Executor							{ get; private set; }
		protected	Bars			Bars								{ get { return this.Executor == null ? null : this.Executor.Bars; } }
		public		Strategy		Strategy							{ get { return this.Executor.Strategy; } }
		public		string			StrategyName						{ get { return this.Executor.StrategyName; } }
		//public	PositionList	Positions_AllBacktested				{ get { return this.Executor.ExecutionDataSnapshot.Positions_AllBacktested; } }
		
		#region Position-related userland-invokeable parts
		public		bool			HasAlertsUnfilled					{ get { return this.Executor.ExecutionDataSnapshot.AlertsUnfilled		.Count > 0; } }
		public		bool			HasPositions_OpenNow				{ get { return this.Executor.ExecutionDataSnapshot.Positions_OpenNow	.Count > 0; } }
		public		Position		LastPosition_OpenNow_nullUnsafe		{ get { return this.Executor.ExecutionDataSnapshot.Positions_OpenNow	.Last_nullUnsafe(this, "LastPosition_OpenNow_nullUnsafe"); } }
		#endregion
		
		public	string				ScriptParametersAsString		{ get {
				if (this.ScriptParametersById_reflectedCached_primary.Count == 0) return "(NoScriptParameters)";
				string ret = "";
				foreach (int id in this.ScriptParametersById_reflectedCached_primary.Keys) {
					ret += this.ScriptParametersById_reflectedCached_primary[id].Name + "=" + this.ScriptParametersById_reflectedCached_primary[id].ValueCurrent + ",";
				}
				ret = ret.TrimEnd(",".ToCharArray());
				return "(" + ret + ")";
			} }

		// that's a smart suggestion SharpDevelop pops up: "Constructors in abstract classes should not be public"
		protected Script() {
		}

		public void Initialize(ScriptExecutor scriptExecutor, bool saveStrategy_falseForSequencer = true) {
			if (this.Executor == scriptExecutor) {
				string msg = "SRIPT_ALREADY_INITIALIZED_WITH_EXECUTOR_AND_NEVER_GETS_ANOTHER_ONE"
					+ " INDICATORS_WILL_COMPLAIN__HOST_PANEL_ALREADY_ASSIGNED";
				Assembler.PopupException(msg);
				
				var reflected	= this.Strategy.Script.ScriptParametersById_reflectedCached_primary;
				var ctx			= this.Strategy.ScriptContextCurrent.ScriptParametersById;
				if (reflected.Count != ctx.Count) {
					string msg2 = "RUDE_SYNC_STALE_CTX_WITH_REFLECTED";
					Assembler.PopupException(msg2, null, false);
					this.Strategy.ScriptContextCurrent.ScriptParametersById = this.Strategy.Script.ScriptParametersById_reflectedCached_primary;
				}

				return;
			}

			this.Executor = scriptExecutor;

			this.		   ScriptParametersById_reflectionForced_byClearingCache();
			this.			   IndicatorsByName_reflectionForced_byClearingCache();
			this.			IndicatorParameters_reflectionForced_byClearingCache();
			this.IndicatorParametersByIndicator_reflectionForced_byClearingCache();

			this.Strategy.ScriptAndIndicatorParametersReflected_absorbFromCurrentContext_saveStrategy(saveStrategy_falseForSequencer);
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

		public void SwitchToDefaultContext_byAbsorbingScriptAndIndicatorParameters_fromSelfCloneConstructed() {
			string msig = " //SwitchToDefaultContext_byAbsorbingScriptAndIndicatorParameters_fromSelfCloneConstructed()";
			object selfCloneConstructed = Activator.CreateInstance(this.GetType());	//default ctor invoked where developer is supposed to add ScriptAndIndicatorParameters into new This
			Script clone = selfCloneConstructed as Script;
			if (clone == null) {
				string msg = "Activator.CreateInstance(" + this.GetType().ToString() + " as Script == null";
				Assembler.PopupException(msg);
				return;
			}

			// first half of the job
			SortedDictionary<int, ScriptParameter>	cloneScriptParametersFrom	= clone.ScriptParametersById_reflectedCached_primary;
			SortedDictionary<int, ScriptParameter>	myctxScriptParametersTo		= this.Strategy.ScriptContextCurrent.ScriptParametersById;
			foreach (int cloneSPindex in cloneScriptParametersFrom.Keys) {
				ScriptParameter cloneSparam = cloneScriptParametersFrom[cloneSPindex];
				if (myctxScriptParametersTo.ContainsKey(cloneSPindex) == false) {
					string msg = "myctxScriptParametersTo.ContainsKey(" + cloneSPindex + ") == false; Script.ScriptParametersAsString=" + this.ScriptParametersAsString;
					Assembler.PopupException(msg);
					//continue;
					ScriptParameter clonedCloneSparam = cloneSparam.Clone_asScriptParameter(
						"REFACTOR_SwitchedToDefaultContextByAbsorbingScriptAndIndicatorParametersFromSelfCloneConstructed",
						"script[" + this.StrategyName + "]" + msig);
					myctxScriptParametersTo.Add(cloneSPindex, clonedCloneSparam);
				} else {
					myctxScriptParametersTo[cloneSPindex].AbsorbCurrent_fixBoundaries_from(cloneSparam);
				}
			}

			// second half of the job
			List<Indicator> cloneIndicators = new List<Indicator>(clone.IndicatorsByName_reflectedCached_primary.Values);
			Dictionary<string, List<IndicatorParameter>> cloneIndicatorsFrom = new Dictionary<string, List<IndicatorParameter>>();
			foreach (Indicator cloneI in cloneIndicators) cloneIndicatorsFrom.Add(cloneI.Name, new List<IndicatorParameter>(cloneI.ParametersByName.Values));

			Dictionary<string, List<IndicatorParameter>> myctxIndicatorsTo	= this.Strategy.ScriptContextCurrent.IndicatorParametersByIndicatorName;
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
						myctxIparams.Add(cloneIparam.Clone_asIndicatorParameter(
							"REFACTOR_SwitchedToDefaultContextByAbsorbingScriptAndIndicatorParametersFromSelfCloneConstructed",
							"script[" + this.StrategyName + "]" + msig));
					} else {
						IndicatorParameter myctxIparam = myctxIparamsLookup[cloneIparam.Name];
						myctxIparam.AbsorbCurrent_fixBoundaries_from(cloneIparam);
					}
				}
			}
			string msg2 = "YOU_JUST_SELF_CLONED_AND_ABSORBED__YOU_JUST_NEED_TO_INIT_SCRIPT_INDICATORS_WITH_NEW_INDICATOR_PARAMS_AND_HOST_PANELS";
			Assembler.PopupException(msg2);
			this.Strategy.IndicatorParametersReflected_absorbFromCurrentContext_saveStrategy(true);
			this.InitializeIndicatorsReflected_withHostPanel();
		}
		internal void RecalculateIndicator(IndicatorParameter indicatorParameterChanged_userClickedInSliders) {
			string msig = " //Script.RecalculateIndicator(" + indicatorParameterChanged_userClickedInSliders + ")";
			if (this.IndicatorsByName_reflectedCached_primary.ContainsKey(indicatorParameterChanged_userClickedInSliders.IndicatorName) == false) {
				string msg = "YOU_MUST_FORCE_INDICATORS_REFLECTION__AFTER_YOU_RECOMPILE_YOUR_SCRIPT";
				Assembler.PopupException(msg);
				return;
			}
			Indicator indicatorForChangedParameter = this.IndicatorsByName_reflectedCached_primary[indicatorParameterChanged_userClickedInSliders.IndicatorName];
			this.Executor.PreCalculateIndicators_forLoadedBars_backtestWontFollow(indicatorForChangedParameter);		// WILL_CLEAR eachIndicator.OwnValuesCalculated.Clear()
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
