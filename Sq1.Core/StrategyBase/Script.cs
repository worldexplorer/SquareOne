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
		public Dictionary<int, ScriptParameter> Parameters;

		public BacktestMode BacktestMode;
		public Strategy Strategy { get { return this.Executor.Strategy; } }
		public string StrategyName { get { return this.Executor.StrategyName; } }

		public List<Position> Positions { get { return this.Executor.ExecutionDataSnapshot.PositionsMaster; } }
		public string ParametersAsString {
			get {
				if (this.Parameters.Count == 0) return "(None)";
				string ret = "";
				foreach (int id in this.Parameters.Keys) {
					ret += this.Parameters[id].Name + "=" + this.Parameters[id].ValueCurrent + ",";
				}
				ret = ret.TrimEnd(",".ToCharArray());
				return "(" + ret + ")";
			}
		}
		
		public List<Position> PositionsOpenNow { get { return this.Executor.ExecutionDataSnapshot.PositionsOpenNow; } }
		public bool IsLastPositionNotClosedYet { get {
				//v1 return LastPosition.Active;
				Position pos = this.LastPosition;
				if (null == pos) return false;
				return (pos.ExitMarketLimitStop == MarketLimitStop.Unknown);
			} }
		public Position LastPositionOpenNow { get {
				List<Position> positionsOpenNow = this.PositionsOpenNow;
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
			this.Parameters = new Dictionary<int, ScriptParameter>();
			Type myChild = this.GetType();
			object[] attributes = myChild.GetCustomAttributes(typeof(ScriptParameterAttribute), true);
			foreach (object attrObj in attributes) {
				ScriptParameterAttribute attr = attrObj as ScriptParameterAttribute;
				if (attr == null) continue;
				this.ParameterCreateRegister(attr.Id, attr.Name, attr.ValueCurrent, attr.ValueMin, attr.ValueMax, attr.ValueIncrement);
			}
			BacktestMode = BacktestMode.FourStrokeOHLC;
		}

		#region script parameters
		public ScriptParameter ParameterCreateRegister(int id, string name, double value, double start, double stop, double step) {
			this.checkThrowParameterAlreadyRegistered(id, name);
			ScriptParameter strategyParameter = new ScriptParameter(id, name, value, start, stop, step);
			this.Parameters.Add(id, strategyParameter);
			return strategyParameter;
		}
		protected void checkThrowParameterAlreadyRegistered(int id, string name) {
			if (this.Parameters.ContainsKey(id) == false) return;
			ScriptParameter param = this.Parameters[id];
			string msg = "Script[" + this.StrategyName + "] already had parameter {id[" + param.Id + "] name[" + param.Name + "]}"
				+ " while adding {id[" + id + "] name[" + name + "]}; edit source code and make IDs unique for every parameter";
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
			this.IndicatorsInstantiateStoreInSnapshot();
 
			//this.executor.Backtester.Initialize(this.BacktestMode);
			try {
				this.InitializeBacktest();
			} catch (Exception ex) {
				Assembler.PopupException("Script.Initialize()", ex);
			}
		}
		public void IndicatorsInstantiateStoreInSnapshot() {
			this.Executor.ExecutionDataSnapshot.Indicators.Clear();
			Type myChild = this.GetType();
			PropertyInfo[] lookingForIndicators = myChild.GetProperties();
			foreach (PropertyInfo propertyIndicator in lookingForIndicators) {
				Type hopefullyIndicatorChildType = propertyIndicator.PropertyType;
				bool isIndicatorChild = typeof(Indicator).IsAssignableFrom(hopefullyIndicatorChildType);
				if (isIndicatorChild == false) continue;

				Indicator indicatorInstance = null;
				var expectingNull = propertyIndicator.GetValue(this, null);
				if (expectingNull != null) {
					indicatorInstance = expectingNull as Indicator;
					//Debugger.Break();
					//continue;
				} else {
					object indicatorInstanceUntyped = Activator.CreateInstance(hopefullyIndicatorChildType);
					indicatorInstance = indicatorInstanceUntyped as Indicator;
					if (indicatorInstance == null) {
						Debugger.Break();
						continue;
					}
				}

				indicatorInstance.Name = propertyIndicator.Name;
				//MORE_EFFECTIVE_IN_Indicator.NotOnChartBarsKey_get()  indicatorInstance.OwnValuesCalculated.ScaleInterval = this.Bars.ScaleInterval;

				//indicatorInstance.BuildParametersFromAttributes();	//indicatorInstance.Initialize() invokes BuildParametersFromAttributes();
				HostPanelForIndicator priceOrItsOwnPanel = this.Executor.ChartShadow.GetHostPanelForIndicator(indicatorInstance);
				indicatorInstance.Initialize(priceOrItsOwnPanel);

				#region overwriting IndicatorParameters from Strategy attributes; similar to Indicator.BuildParametersFromAttributes()
				object[] attributes = propertyIndicator.GetCustomAttributes(typeof(IndicatorParameterAttribute), true);
				string attributesAllAvailable = "";
				foreach (object attrObj in attributes) {
					IndicatorParameterAttribute attr = attrObj as IndicatorParameterAttribute;
					if (attributesAllAvailable.Length > 0) attributesAllAvailable += ",";
					attributesAllAvailable += attr;
				}
				foreach (object attrObj in attributes) {
					IndicatorParameterAttribute attr = attrObj as IndicatorParameterAttribute;
					IndicatorParameter param = new IndicatorParameter(attr);

					if (indicatorInstance.ParametersByName.ContainsKey(param.Name) == false) {
						string msg = "SCRIPT_SETS_ATTRIBUTE_FOR_INDICATOR_WHICH_IT_DOESNT_HAVE#1 param[" + param + "] attributesAllAvailable[" + attributesAllAvailable + "]";
						Assembler.PopupException(msg);
						continue;
					}
					// strategy has an Indicator and an attribute for that indicator; find matching property in the indicator & overwrite
					PropertyInfo propertyIndicatorFoundUnique = null;
					PropertyInfo[] lookingForIndicatorParameters = indicatorInstance.GetType().GetProperties();
					foreach (PropertyInfo propertyIndicatorInstance in lookingForIndicatorParameters) {
						if (propertyIndicatorInstance.Name != param.Name) continue;
						propertyIndicatorFoundUnique = propertyIndicatorInstance;
						break;
					}
					if (propertyIndicatorFoundUnique == null) {
						string msg = "SCRIPT_SETS_ATTRIBUTE_FOR_INDICATOR_WHICH_IT_DOESNT_HAVE#2 param[" + param + "] attributesAllAvailable[" + attributesAllAvailable + "]";
						Assembler.PopupException(msg);
						continue;
					}
					object valueCurrentCasted = param.ValueCurrent;
					if (propertyIndicatorFoundUnique.PropertyType.Name == "Int32") {
						valueCurrentCasted = (int)Math.Round(param.ValueCurrent);
					}
					propertyIndicatorFoundUnique.SetValue(indicatorInstance, valueCurrentCasted, null);
					indicatorInstance.ParametersByName[param.Name] = param;
				}
				// resetting it for fair recalculation to include parameters into this.NameWithParameters; it isn't redundant!
				indicatorInstance.parametersAsStringShort = null;
				#endregion
				propertyIndicator.SetValue(this, indicatorInstance, null);
				this.Executor.ExecutionDataSnapshot.Indicators.Add(propertyIndicator.Name, indicatorInstance);
			}
		}
		#endregion

		#region methods to override in derived strategy
		public virtual void InitializeBacktest() {
		}
		public virtual void OnNewQuoteOfStreamingBarCallback(Quote quoteNewArrived) {
		}
		public virtual void OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(Bar barNewStaticArrived) {
			throw new Exception("please overwrite OnNewBarCallback(Bar); Execute() is obsolete now");
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
		public Position BuyAtClose(Bar bar, string signalName = "BOUGHT_AT_CLOSE") {
			return this.Executor.BuyOrShortAlertCreateRegister(bar, 0, signalName, Direction.Buy, MarketLimitStop.AtClose);
		}
		public Position BuyAtLimit(Bar bar, double limitPrice, string signalName = "BOUGHT_AT_LIMIT") {
			return this.Executor.BuyOrShortAlertCreateRegister(bar, limitPrice, signalName, Direction.Buy, MarketLimitStop.Limit);
		}
		public Position BuyAtMarket(Bar bar, string signalName = "BOUGHT_AT_MARKET") {
			return this.Executor.BuyOrShortAlertCreateRegister(bar, 0, signalName, Direction.Buy, MarketLimitStop.Market);
		}
		public Position BuyAtStop(Bar bar, double stopPrice, string signalName = "BOUGHT_AT_STOP") {
			return this.Executor.BuyOrShortAlertCreateRegister(bar, stopPrice, signalName, Direction.Buy, MarketLimitStop.Stop);
		}
		
		public Alert CoverAtClose(Bar bar, Position position, string signalName = "COVERED_AT_CLOSE") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, 0, signalName, Direction.Cover, MarketLimitStop.AtClose);
		}
		public Alert CoverAtLimit(Bar bar, Position position, double limitPrice, string signalName = "COVERED_AT_LIMIT") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, limitPrice, signalName, Direction.Cover, MarketLimitStop.Limit);
		}
		public Alert CoverAtMarket(Bar bar, Position position, string signalName = "COVERED_AT_MARKET") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, 0, signalName, Direction.Cover, MarketLimitStop.Market);
		}
		public Alert CoverAtStop(Bar bar, Position position, double stopPrice, string signalName = "COVERED_AT_STOP") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, stopPrice, signalName, Direction.Cover, MarketLimitStop.Stop);
		}

		public Alert SellAtClose(Bar bar, Position position, string signalName = "SOLD_AT_CLOSE") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, 0, signalName, Direction.Sell, MarketLimitStop.AtClose);
		}
		public Alert SellAtLimit(Bar bar, Position position, double limitPrice, string signalName = "SOLD_AT_LIMIT") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, limitPrice, signalName, Direction.Sell, MarketLimitStop.Limit);
		}
		public Alert SellAtMarket(Bar bar, Position position, string signalName = "SOLD_AT_MARKET") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, 0, signalName, Direction.Sell, MarketLimitStop.Market);
		}
		public Alert SellAtStop(Bar bar, Position position, double stopPrice, string signalName = "SOLD_AT_STOP") {
			return this.Executor.SellOrCoverAlertCreateRegister(bar, position, stopPrice, signalName, Direction.Sell, MarketLimitStop.Stop);
		}
		
		public Position ShortAtClose(Bar bar, string signalName = "SHORTED_AT_CLOSE") {
			return this.Executor.BuyOrShortAlertCreateRegister(bar, 0, signalName, Direction.Short, MarketLimitStop.AtClose);
		}
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
		public Alert ExitAtClose(Bar bar, Position position, string signalName = "EXITED_AT_CLOSE") {
			if (position.PositionLongShort == PositionLongShort.Long) {
				return this.SellAtClose(bar, position, signalName);
			} else {
				return this.CoverAtClose(bar, position, signalName);
			}
		}
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