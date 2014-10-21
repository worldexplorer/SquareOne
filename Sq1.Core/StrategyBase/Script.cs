using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;

using Sq1.Core.Backtesting;
using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;

namespace Sq1.Core.StrategyBase {
	public abstract class Script {
		public ScriptExecutor Executor { get; private set; }
		protected Bars Bars { get { return (Executor == null) ? null : Executor.Bars; } }
		//Bars barsInitialContext;
		
		// TODO: move to Strategy; how does it survive restart now?...
		public SortedDictionary<int, ScriptParameter> ParametersById;
		public Dictionary<string, ScriptParameter> ParametersByNameInlineCopy { get {
				Dictionary<string, ScriptParameter> ret = new Dictionary<string, ScriptParameter>();
				foreach (ScriptParameter param in ParametersById.Values) {
					if (ret.ContainsKey(param.Name)) {
						string msg = "PARAMETER_NAME_NOT_UNIQUE[" + param.Name + "], prev[" + ret[param.Name].ToString() + "] this[" + param.ToString() + "]";
						Assembler.PopupException(msg + " //Script.ParametersByName");
						continue;
					}
					ret.Add(param.Name, param);
				}
				return ret;
			} }

		public BacktestMode BacktestMode;
		public Strategy Strategy { get { return this.Executor.Strategy; } }
		public string StrategyName { get { return this.Executor.StrategyName; } }

		public List<Position> Positions { get { return this.Executor.ExecutionDataSnapshot.PositionsMaster; } }
		public string ParametersAsString { get {
				if (this.ParametersById.Count == 0) return "(None)";
				string ret = "";
				foreach (int id in this.ParametersById.Keys) {
					ret += this.ParametersById[id].Name + "=" + this.ParametersById[id].ValueCurrent + ",";
				}
				ret = ret.TrimEnd(",".ToCharArray());
				return "(" + ret + ")";
			} }
		
		public bool IsLastPositionNotClosedYet { get {
				//v1 return LastPosition.Active;
				Position pos = this.LastPosition;
				if (null == pos) return false;
				return (pos.ExitMarketLimitStop == MarketLimitStop.Unknown);
			} }
		public Position LastPositionOpenNow { get {
				List<Position> positionsOpenNow = this.Executor.ExecutionDataSnapshot.PositionsOpenNow;
				if (positionsOpenNow.Count == 0) return null;
				return positionsOpenNow[positionsOpenNow.Count - 1];
			} }
		public Position LastPosition { get {
				List<Position> positionsMaster = this.Executor.ExecutionDataSnapshot.PositionsMaster;
				if (positionsMaster.Count == 0) return null;
				return positionsMaster[positionsMaster.Count - 1];
			} }
		public bool HasAlertsPendingOrPositionsOpenNow { get { return this.HasAlertsPending || this.HasPositionsOpenNow; } }
		public bool HasAlertsPending { get { return (this.Executor.ExecutionDataSnapshot.AlertsPending.Count > 0); } }
		public bool HasPositionsOpenNow { get { return (this.Executor.ExecutionDataSnapshot.PositionsOpenNow.Count > 0); } }

		public Script() {
			this.ParametersById = new SortedDictionary<int, ScriptParameter>();
			Type myChild = this.GetType();
			
			object[] scriptParameterAttrs = myChild.GetCustomAttributes(typeof(ScriptParameterAttribute), true);
			foreach (object attrObj in scriptParameterAttrs) {
				ScriptParameterAttribute attr = attrObj as ScriptParameterAttribute;
				if (attr == null) continue;
				this.ScriptParameterCreateRegister(attr.Id, attr.Name, attr.ValueCurrent, attr.ValueMin, attr.ValueMax, attr.ValueIncrement);
			}

			BacktestMode = BacktestMode.FourStrokeOHLC;
		}

		#region script parameters and indicator parameter userland-invokable helpers
		public ScriptParameter ScriptParameterCreateRegister(int id, string name, double value, double min, double max, double increment) {
			this.checkThrowScriptParameterAlreadyRegistered(id, name);
			ScriptParameter strategyParameter = new ScriptParameter(id, name, value, min, max, increment);
			this.ParametersById.Add(id, strategyParameter);
			return strategyParameter;
		}
		protected void checkThrowScriptParameterAlreadyRegistered(int id, string name) {
			if (this.ParametersById.ContainsKey(id) == false) return;
			ScriptParameter param = this.ParametersById[id];
			string msg = "Script[" + this.StrategyName + "] already had parameter {id[" + param.Id + "] name[" + param.Name + "]}"
				+ " while adding {id[" + id + "] name[" + name + "]}; edit source code and make IDs unique for every parameter";
			#if DEBUG
			Debugger.Break();
			#endif
			throw new Exception(msg);
		}

		#endregion

		#region Initializers
		public void Initialize(ScriptExecutor scriptExecutor) {
			this.Executor = scriptExecutor;
		}
		public void InitializeBacktestWithExecutorsBarsInstantiateIndicators() {
			if (this.Bars == null) Debugger.Break();
			//this.barsInitialContext = this.Bars;
			//this.CreateOHLCVproxies();
			// ALLOWED_TO_REMOVE_IMPLICIT_INDICATOR_CTORS_FROM_SCRIPT this.ma = new IndicatorAverageMovingSimple();
			// CLEANER_INDICATORS MOVED_TO_PushScriptIndicatorsParametersIntoCurrentContext() this.IndicatorsInstantiateStoreInSnapshot();
 
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
		public void PullParametersFromCurrentContextSaveStrategy() {
			bool storeStrategySinceParametersGottenFromScript = false;
			foreach (ScriptParameter scriptParam in this.ParametersById.Values) {
				if (this.Strategy.ScriptContextCurrent.ScriptParametersById.ContainsKey(scriptParam.Id)) {
					double valueContext = this.Strategy.ScriptContextCurrent.ScriptParametersById[scriptParam.Id].ValueCurrent;
					scriptParam.ValueCurrent = valueContext;
				} else {
					this.Strategy.ScriptContextCurrent.ScriptParametersById.Add(scriptParam.Id, scriptParam);
					string msg = "added scriptParam[Id=" + scriptParam.Id + " value=" + scriptParam.ValueCurrent + "]"
						+ " into Script[" + this.GetType().Name + "].Strategy.ScriptContextCurrent[" + this.Strategy.ScriptContextCurrent.Name + "]"
						+ " /ScriptParametersMergedWithCurrentContext";
					Assembler.PopupException(msg, null, false);
					storeStrategySinceParametersGottenFromScript = true;
				}
			}
			if (storeStrategySinceParametersGottenFromScript) {
				Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.Strategy);
			}
		}

		public Dictionary<string, IndicatorParameter> IndicatorsParametersInitializedInDerivedConstructorByNameForSliders { get {
				Dictionary<string, IndicatorParameter> ret = new Dictionary<string, IndicatorParameter>();
				//v1 [Obsolete("THIS_IS_UNMERGED_WITH_SCRIPT_CONTEXT_USE_Strategy.ScriptContextCurrent.IndicatorParametersByNameConstructedMergedWithJson")]
				//foreach (Indicator indicator in this.IndicatorsInitializedInDerivedConstructor) {
				//	Dictionary<string, IndicatorParameter> parametersByName = indicator.ParametersByName;	// dont make me calculate it twice 
				//	foreach (string paramName in parametersByName.Keys) {									// #1
				//		IndicatorParameter param = parametersByName[paramName];								// #2
				//		ret.Add(indicator.Name + "." + param.Name, param);
				//	}
				//}
				//v2
				Dictionary<string, List<IndicatorParameter>> parametersByIndicatorName = this.Strategy.ScriptContextCurrent.IndicatorParametersByName;

				bool mustBeMergedIfAny = this.Strategy.Script.IndicatorsInitializedInDerivedConstructor.Count > 0 && parametersByIndicatorName.Count == 0;
				if (mustBeMergedIfAny) {
					#if DEBUG
					Debugger.Break();
					this.Strategy.Script.IndicatorsInitializeAbsorbParamsFromJsonStoreInSnapshot();
					parametersByIndicatorName = this.Strategy.ScriptContextCurrent.IndicatorParametersByName;
					#endif
					if (parametersByIndicatorName.Count == 0) {
						#if DEBUG
						Debugger.Break();
						#endif
						return ret;
					}
				}
				foreach (string indicatorName in parametersByIndicatorName.Keys) {
					List<IndicatorParameter> indicatorParameters = parametersByIndicatorName[indicatorName];
					foreach (IndicatorParameter indicatorParameter in indicatorParameters) {
						string indicatorDotParameterName = indicatorName + "." + indicatorParameter.Name;
						ret.Add(indicatorDotParameterName, indicatorParameter);
					}
				}
				return ret;
			} }
		
		public List<Indicator> IndicatorsInitializedInDerivedConstructor { get {
				List<Indicator> ret = new List<Indicator>();
				
				Type myChild = this.GetType();
				//PropertyInfo[] lookingForIndicators = myChild.GetProperties();
				FieldInfo[] lookingForIndicators = myChild.GetFields();
				foreach (FieldInfo indicatorCandidate in lookingForIndicators) {
					Type indicatorConcreteType = indicatorCandidate.FieldType;
					bool isIndicatorChild = typeof(Indicator).IsAssignableFrom(indicatorConcreteType);
					if (isIndicatorChild == false) continue;
					Indicator indicatorInstance = null;
					object expectingConstructedNonNull = indicatorCandidate.GetValue(this);
					if (expectingConstructedNonNull == null) {
						string msg = "INDICATOR_DECLARED_BUT_NOT_CREATED+ASSIGNED_IN_CONSTRUCTOR Script[" + this.ToString();// + "].[" + variableIndicator.Name + "]";
						#if DEBUG
						Debugger.Break();
						#endif
						Assembler.PopupException(msg);
						continue;
					}
					Indicator variableIndicator = expectingConstructedNonNull as Indicator;
					variableIndicator.Name = indicatorCandidate.Name;
					ret.Add(variableIndicator);
				}
				return ret;
			} }
		
		public void IndicatorsInitializeAbsorbParamsFromJsonStoreInSnapshot() {
			this.Executor.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances.Clear();
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
					Assembler.PopupException(msg);
					this.Strategy.ScriptContextCurrent.IndicatorParametersByName.Add(indicatorInstance.Name, new List<IndicatorParameter>(indicatorInstance.ParametersByName.Values));
				}

				this.Executor.ExecutionDataSnapshot.IndicatorsReflectedScriptInstances.Add(indicatorInstance.Name, indicatorInstance);
				
				// moved from upstairs coz: after absorbing all deserialized indicator parameters from ScriptContext, GetHostPanelForIndicator will return an pre-instantiated PanelIndicator
				// otherwize GetHostPanelForIndicator created a new one for an indicator with default Indicator parameters;
				// example: MultiSplitterPropertiesByPanelName["ATR (Period:9[1..11/2])"] exists, while default Period for ATR is 5 => new PanelIndicator will be created
				// final goal is to avoid (splitterPropertiesByPanelName.Count != this.panels.Count) in SplitterPropertiesByPanelNameSet() and (splitterFound == null)  
				HostPanelForIndicator priceOrItsOwnPanel = this.Executor.ChartShadow.HostPanelForIndicatorGet(indicatorInstance);
				indicatorInstance.Initialize(priceOrItsOwnPanel);
			}
		}
		#endregion

		#region methods to override in derived strategy
		public virtual void InitializeBacktest() {
		}
		public virtual void OnNewQuoteOfStreamingBarCallback(Quote quoteNewArrived) {
		}
		public virtual void OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(Bar barNewStaticArrived) {
			string msg = "SCRIPT_DERIVED_MUST_IMPLEMENT Script[" + this.GetType().FullName + "]: public override void OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(Bar)";
			throw new Exception(msg);
		}
		public virtual void OnAlertFilledCallback(Alert alertFilled) {
		}
		public virtual void OnAlertKilledCallback(Alert alertKilled) {
		}
		public virtual void OnAlertNotSubmittedCallback(Alert alertNotSubmitted, int barNotSubmittedRelno) {
		}
		public virtual void OnPositionOpenedCallback(Position positionOpened) {
		}
		public virtual void OnPositionOpenedPrototypeSlTpPlacedCallback(Position positionOpenedByPrototype) {
		}
		public virtual void OnPositionClosedCallback(Position positionClosed) {
		}
		//public virtual void ExecuteOnStopLossNegativeOffsetUpdateActivationSucceeded(Position position, PositionPrototype proto) {
		//}
		//public virtual void ExecuteOnStopLossNegativeOffsetUpdateActivationFailed(Position position, PositionPrototype proto) {
		//}
		#endregion

		#region Buy/Sell Short/Cover
//		public Position BuyAtClose(Bar bar, string signalName = "BOUGHT_AT_CLOSE") {
//			return this.Executor.BuyOrShortAlertCreateRegister(bar, 0, signalName, Direction.Buy, MarketLimitStop.AtClose);
//		}
		public Position BuyAtLimit(Bar bar, double limitPrice, string signalName = "BOUGHT_AT_LIMIT") {
			return this.Executor.BuyOrShortAlertCreateRegister(bar, limitPrice, signalName, Direction.Buy, MarketLimitStop.Limit);
		}
		public Position BuyAtMarket(Bar bar, string signalName = "BOUGHT_AT_MARKET") {
			return this.Executor.BuyOrShortAlertCreateRegister(bar, 0, signalName, Direction.Buy, MarketLimitStop.Market);
		}
		public Position BuyAtStop(Bar bar, double stopPrice, string signalName = "BOUGHT_AT_STOP") {
			return this.Executor.BuyOrShortAlertCreateRegister(bar, stopPrice, signalName, Direction.Buy, MarketLimitStop.Stop);
		}
		
//		public Alert CoverAtClose(Bar bar, Position position, string signalName = "COVERED_AT_CLOSE") {
//			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, 0, signalName, Direction.Cover, MarketLimitStop.AtClose);
//		}
		public Alert CoverAtLimit(Bar bar, Position position, double limitPrice, string signalName = "COVERED_AT_LIMIT") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, limitPrice, signalName, Direction.Cover, MarketLimitStop.Limit);
		}
		public Alert CoverAtMarket(Bar bar, Position position, string signalName = "COVERED_AT_MARKET") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, 0, signalName, Direction.Cover, MarketLimitStop.Market);
		}
		public Alert CoverAtStop(Bar bar, Position position, double stopPrice, string signalName = "COVERED_AT_STOP") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, stopPrice, signalName, Direction.Cover, MarketLimitStop.Stop);
		}

//		public Alert SellAtClose(Bar bar, Position position, string signalName = "SOLD_AT_CLOSE") {
//			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, 0, signalName, Direction.Sell, MarketLimitStop.AtClose);
//		}
		public Alert SellAtLimit(Bar bar, Position position, double limitPrice, string signalName = "SOLD_AT_LIMIT") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, limitPrice, signalName, Direction.Sell, MarketLimitStop.Limit);
		}
		public Alert SellAtMarket(Bar bar, Position position, string signalName = "SOLD_AT_MARKET") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, 0, signalName, Direction.Sell, MarketLimitStop.Market);
		}
		public Alert SellAtStop(Bar bar, Position position, double stopPrice, string signalName = "SOLD_AT_STOP") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, stopPrice, signalName, Direction.Sell, MarketLimitStop.Stop);
		}
		
//		public Position ShortAtClose(Bar bar, string signalName = "SHORTED_AT_CLOSE") {
//			return this.Executor.BuyOrShortAlertCreateRegister(bar, 0, signalName, Direction.Short, MarketLimitStop.AtClose);
//		}
		public Position ShortAtLimit(Bar bar, double limitPrice, string signalName = "SHORTED_AT_LIMIT") {
			return this.Executor.BuyOrShortAlertCreateRegister(bar, limitPrice, signalName, Direction.Short, MarketLimitStop.Limit);
		}
		public Position ShortAtMarket(Bar bar, string signalName = "SHORTED_AT_MARKET") {
			return this.Executor.BuyOrShortAlertCreateRegister(bar, 0, signalName, Direction.Short, MarketLimitStop.Market);
		}
		public Position ShortAtStop(Bar bar, double stopPrice, string signalName = "SHORTED_AT_STOP") {
			return this.Executor.BuyOrShortAlertCreateRegister(bar, stopPrice, signalName, Direction.Short, MarketLimitStop.Stop);
		}
		public Alert ExitAtMarket(Bar bar, Position position, string signalName = "EXITED_AT_MARKET") {
			if (position.PositionLongShort == PositionLongShort.Long) {
				return this.SellAtMarket(bar, position, signalName);
			} else {
				return this.CoverAtMarket(bar, position, signalName);
			}
		}
//		public Alert ExitAtClose(Bar bar, Position position, string signalName = "EXITED_AT_CLOSE") {
//			if (position.PositionLongShort == PositionLongShort.Long) {
//				return this.SellAtClose(bar, position, signalName);
//			} else {
//				return this.CoverAtClose(bar, position, signalName);
//			}
//		}
		public Alert ExitAtLimit(Bar bar, Position position, double price, string signalName = "EXITED_AT_LIMIT") {
			if (position.PositionLongShort == PositionLongShort.Long) {
				return this.SellAtLimit(bar, position, price, signalName);
			} else {
				return this.CoverAtLimit(bar, position, price, signalName);
			}
		}
		public Alert ExitAtStop(Bar bar, Position position, double price, string signalName = "EXITED_AT_STOP") {
			if (position.PositionLongShort == PositionLongShort.Long) {
				return this.SellAtStop(bar, position, price, signalName);
			} else {
				return this.CoverAtStop(bar, position, price, signalName);
			}
		}
		#endregion

		#region 
		public void SetBarColorBackground(Bar bar, Color color) {
			this.SetBarColorBackground(bar, color, ChartPanelType.PanelPrice);
		}
		public void SetBarColorBackground(Bar bar, Color color, ChartPanelType chartPanelType) {
			throw new NotImplementedException();
			//this.Executor.ChartShadow.
		}
		#endregion


		#region Kill pending alert
		public void AlertKillPending(Alert alert) {
			this.Executor.AlertKillPending(alert);
		}

		[Obsolete("looks unreliable until refactored; must kill previous alertExit AFTER killing market completes => userland callback or more intelligent management in CORE level")]
		public List<Alert> PositionCloseImmediately(Position position, string signalName) {
			this.ExitAtMarket(this.Bars.BarStreaming, position, signalName);
			// BETTER WOULD BE KILL PREVIOUS PENDING ALERT FROM A CALBACK AFTER MARKET EXIT ORDER GETS FILLED, IT'S UNRELIABLE EXIT IF WE KILL IT HERE
			// LOOK AT EMERGENCY CLASSES, SOLUTION MIGHT BE THERE ALREADY
			return this.PositionKillExitAlert(position, signalName);
		}
		public List<Alert> PositionKillExitAlert(Position position, string signalName) {
			List<Alert> alertsSubmittedToKill = new List<Alert>();
			if (position.IsEntryFilled == false) {
				Debugger.Break();
				return alertsSubmittedToKill;
			}
			if (null == position.ExitAlert) {	// for prototyped position, position.ExitAlert contains TakeProfit 
				Debugger.Break();
				return alertsSubmittedToKill;
			}
			if (string.IsNullOrEmpty(signalName)) signalName = "PositionCloseImmediately()";
			if (position.Prototype != null) {
				alertsSubmittedToKill = this.PositionPrototypeKillWhateverIsPending(position.Prototype, signalName);
				return alertsSubmittedToKill;
			}
			this.AlertKillPending(position.ExitAlert);
			alertsSubmittedToKill.Add(position.ExitAlert);
			return alertsSubmittedToKill;
		}
		
		public List<Alert> PositionPrototypeKillWhateverIsPending(PositionPrototype proto, string signalName) {
			List<Alert> alertsSubmittedToKill = new List<Alert>();
			if (proto.StopLossAlertForAnnihilation != null) {
				this.AlertKillPending(proto.StopLossAlertForAnnihilation);
				alertsSubmittedToKill.Add(proto.StopLossAlertForAnnihilation);
			}
			if (proto.TakeProfitAlertForAnnihilation != null) {
				this.AlertKillPending(proto.TakeProfitAlertForAnnihilation);
				alertsSubmittedToKill.Add(proto.TakeProfitAlertForAnnihilation);
			}
			return alertsSubmittedToKill;
		}
		#endregion

	}
}