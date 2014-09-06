using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;

using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Indicators {
	public abstract class Indicator {
		public string Name;
		public ChartPanelType ChartPanelType;

		public DataSeriesProxyableFromBars DataSeriesProxyFor;
		public ScriptExecutor Executor;
		public DataSeriesTimeBased OwnValuesCalculated;

		// FirstValidBarIndex::virtual>abstract koz inside base class (Indicator) it's still zero!!
		public abstract int FirstValidBarIndex { get; set; }
		
		public string NotOnChartDataSourceName;				//empty means the same as on the chart
		public string NotOnChartSymbol;						//empty means the same as on the chart
		public BarScaleInterval NotOnChartBarScaleInterval;	//null  means the same as on the chart
		public NotOnChartBarsKey NotOnChartBarsKey { get {
				if (string.IsNullOrEmpty(this.NotOnChartSymbol)
				   	//HACK SUBSTITUTED_DURING_BACKTEST_FROM_BACKTESTDATASOURCE_TO_MOCK && string.IsNullOrEmpty(this.NotOnChartDataSourceName)
				  	&& this.NotOnChartBarScaleInterval == null) {
					return null;
				}
//				if (this.Executor == null) {
//					// BactestStarting 
//					return null;
//				}
				try {
					string symbol = string.IsNullOrEmpty(this.NotOnChartSymbol)
									? this.Executor.Bars.Symbol : this.NotOnChartSymbol;
					string dataSourceName = string.IsNullOrEmpty(this.NotOnChartDataSourceName)
									? this.Executor.Bars.DataSource.Name : this.NotOnChartDataSourceName;
					BarScaleInterval scaleInterval = this.NotOnChartBarScaleInterval == null
									? this.Executor.Bars.ScaleInterval : this.NotOnChartBarScaleInterval;
					// by saying {this.OwnValuesCalculated.ScaleInterval == scaleInterval } you invoke BarScaleInterval.{static bool operator ==} - UNDEBUGGABLE
					if (scaleInterval != null && scaleInterval.Equals(this.OwnValuesCalculated.ScaleInterval) == false) {
						this.OwnValuesCalculated.ScaleInterval = scaleInterval;
					}
					return new NotOnChartBarsKey(scaleInterval, symbol, dataSourceName);
				} catch (Exception ex) {
					Assembler.PopupException("Indicator.NotOnChartBarsKey_get()", ex);
					return null;
				}
			} }
		
		private Bars barsEffective_cached;
		public Bars BarsEffective { get {
				if (this.Executor == null) {
					Assembler.PopupException("YOU_FORGOT_TO_INVOKE_INDICATOR.INITIALIZE()_OR_WAIT_UNTIL_ITLLBE_INVOKED_LATER Executor=null in Indicator.BarsEffective_get()");
					Debugger.Break();
					return this.barsEffective_cached;
				}
				this.barsEffective_cached = null;
				//if (this.barsEffective_cached == null) {
					NotOnChartBarsKey key = this.NotOnChartBarsKey;
					if (key != null) {
						this.barsEffective_cached = this.Executor.NotOnChartBarsHelper.RescaledBarsGetRegisteredFor(key);
						bool barsEffective_cachedNull = ReferenceEquals(this.barsEffective_cached, null);
						if (barsEffective_cachedNull) {
						    this.barsEffective_cached = this.Executor.NotOnChartBarsHelper.RescaleBarsAndRegister(key);
						}
					}
					bool barsEffective_cachedNull2 = ReferenceEquals(this.barsEffective_cached, null);
					if (barsEffective_cachedNull2) {
						this.barsEffective_cached = this.Executor.Bars;
					}
				//}
				return this.barsEffective_cached;
			} }
		private DataSeriesProxyBars closesProxyEffective_cached;
		public DataSeriesProxyBars ClosesProxyEffective { get {
				if (this.closesProxyEffective_cached == null) {
					this.closesProxyEffective_cached = new DataSeriesProxyBars(this.BarsEffective, this.DataSeriesProxyFor);
				}
				return this.closesProxyEffective_cached;
			} }
		
		public HostPanelForIndicator HostPanelForIndicator;

		private Color lineColor;
		public Color LineColor {
			get { return this.lineColor; }
			set { this.lineColor = value; this.brushForeground = null; this.penForeground = null; }
		}
		private int lineWidth;
		public int LineWidth {
			get { return this.lineWidth; }
			set { this.lineWidth = value; this.brushForeground = null; this.penForeground = null; }
		}

		private Brush brushForeground;
		public Brush BrushForeground { get {
				if (this.brushForeground == null) {
					this.brushForeground = new SolidBrush(this.LineColor);
				}
				return this.brushForeground;
			} }
		private Pen penForeground;
		public Pen PenForeground { get {
				if (this.penForeground == null) {
					this.penForeground = new Pen(this.LineColor, this.LineWidth);
				}
				return this.penForeground;
			} }
			
		
		public Dictionary<string, IndicatorParameter> ParametersByName;
		
		public string parametersAsStringShort;
		public string ParametersAsStringShort { get {
				if (parametersAsStringShort == null) {
					StringBuilder sb = new StringBuilder();
					foreach (string paramName in this.ParametersByName.Keys) {
						IndicatorParameter param = this.ParametersByName[paramName];
						if (sb.Length > 0) sb.Append(",");
						sb.Append(param);
					}
					this.parametersAsStringShort = sb.ToString();
				}
				return this.parametersAsStringShort;
			} }
//		public string ParametersAsStringLong { get ; }
		
		public string NameWithParameters { get {
				string parameters = this.ParametersAsStringShort;
				if (string.IsNullOrEmpty(parameters)) parameters = "NOT_BUILT_YET_ParametersByName_DIDNT_INVOKE_BuildParametersFromAttributes()";
				string ret = this.Name + " (" + parameters + ") ";
				string otherSymbolTimeframe = "";
				if (string.IsNullOrEmpty(this.NotOnChartSymbol)) {
					//if (this.NotOnChartBarScaleInterval != null) otherSymbolTimeframe += "~";			// "timeframe converted"
					if (Object.ReferenceEquals(this.NotOnChartBarScaleInterval, null) == false) otherSymbolTimeframe += "~";			// "timeframe converted"
					//otherSymbolTimeframe += (this.barsEffective_cached != null) ? this.barsEffective_cached.SymbolIntervalScale : "NULL";
					//otherSymbolTimeframe += (this.BarsEffective != null) ? this.BarsEffective.SymbolIntervalScale : "NULL";
					if (Object.ReferenceEquals(this.NotOnChartBarScaleInterval, null) == false) otherSymbolTimeframe += this.BarsEffective.SymbolIntervalScale;
				} else {
					otherSymbolTimeframe += "(" + this.NotOnChartSymbol;
					//if (this.NotOnChartBarScaleInterval != null) otherSymbolTimeframe += ":" + this.NotOnChartBarScaleInterval;
					if (Object.ReferenceEquals(this.NotOnChartBarScaleInterval, null) == false) otherSymbolTimeframe += ":" + this.NotOnChartBarScaleInterval;
					otherSymbolTimeframe += ")!";								// "Symbol is not from the chart"
					//otherSymbolTimeframe += (this.NotOnChartBarScaleInterval == null) ? "=" : "~";		// "timeframe converted"
					otherSymbolTimeframe += Object.ReferenceEquals(this.NotOnChartBarScaleInterval, null) ? "=" : "~";		// "timeframe converted"
					otherSymbolTimeframe += (this.barsEffective_cached != null) ? this.barsEffective_cached.SymbolIntervalScale : "NULL";
				}
				ret += otherSymbolTimeframe;
				return ret;
			} }

		public string IndicatorErrorsOnBacktestStarting;
		
		public int DotsDrawnForCurrentSlidingWindow;		// nope I won't use a separate "responsibility" (I told you "SOLID principles are always misused" :)
		public int DotsExistsForCurrentSlidingWindow;		// just because the object itself is the most convenient place to incapsulate it
		
		public Indicator() {
			this.Name = "INDICATOR_NAME_NOT_SET_IN_DERIVED_CONSTRUCTOR";
			this.DataSeriesProxyFor = DataSeriesProxyableFromBars.Close;
			this.ParametersByName = new Dictionary<string, IndicatorParameter>();
			this.ChartPanelType = ChartPanelType.PanelIndicatorSingle;
			this.OwnValuesCalculated = new DataSeriesTimeBased(this.Name);
			this.LineColor = Color.Indigo;
			this.LineWidth = 1;
		}
		protected void BuildParametersFromAttributes() {
			this.ParametersByName.Clear();
			Type myChild = this.GetType();
			PropertyInfo[] lookingForIndicatorParameters = myChild.GetProperties();
			foreach (PropertyInfo property in lookingForIndicatorParameters) {
				IndicatorParameterAttribute attrFoundUnique = null;
				object[] attributes = property.GetCustomAttributes(typeof(IndicatorParameterAttribute), true);
				foreach (object attrObj in attributes) {
					IndicatorParameterAttribute attr = attrObj as IndicatorParameterAttribute;
					if (attrFoundUnique != null) {
						string msg = "ATTRIBUTE_INDICATOR_PARAMETER_SET_MULTIPLE_TIMES_MUST_BE_SINGLE attrFoundUnique[" + attrFoundUnique + "]";
						Assembler.PopupException(msg);
					}
					attrFoundUnique = attr;
				}
				if (attrFoundUnique == null) continue;
				IndicatorParameter param = new IndicatorParameter(attrFoundUnique);
				
				object valueCurrentCasted = param.ValueCurrent;
				if (property.PropertyType.Name == "Int32") {
					valueCurrentCasted = (int)Math.Round(param.ValueCurrent);
				}
				property.SetValue(this, valueCurrentCasted, null);
				if (this.ParametersByName.ContainsKey(param.Name)) {
					string msg = "INDICATOR_PARAMETER_ALREADY_ADDED_MUST_BE_UNIQUE param[" + param + "]";
					Assembler.PopupException(msg);
				}
				// MOVED_TO_LATER_STAGE
				//string validationError = param.ValidateSelf();
				//if (string.IsNullOrEmpty(validationError) == false) {
				//    string msg = "INDICATOR_SELF_VALIDATION_FAILED [" + validationError + "] for param[" + param + "]";
				//    Assembler.PopupException(msg);
				//}
				this.ParametersByName.Add(param.Name, param);
			}
			// resetting it for fair recalculation to include parameters into this.NameWithParameters; it isn't redundant!
			this.parametersAsStringShort = null;
		}
		public void Initialize(HostPanelForIndicator panelNamedFolding) {
			this.HostPanelForIndicator = panelNamedFolding;
			this.closesProxyEffective_cached = null;
			this.barsEffective_cached = null;
			this.BuildParametersFromAttributes();
			this.OwnValuesCalculated.Clear();
			//this.OwnValuesCalculated.Description = this.Name;	//appears after .BuildParametersFromAttributes();
			this.OwnValuesCalculated.Description = this.NameWithParameters;
		}
		public void IndicatorErrorsOnBacktestStartingAppend(string msg, string separator = "; ") {
			if (string.IsNullOrEmpty(msg)) return;
			if (string.IsNullOrEmpty(this.IndicatorErrorsOnBacktestStarting) == false) msg += separator;
			this.IndicatorErrorsOnBacktestStarting += msg;
		}
		public bool BacktestStarting(ScriptExecutor executor) {
			this.Executor = executor;
			string msig = " Indicator[" + this.NameWithParameters + "].BacktestStarting()";

			// HACK new Indicator will be Initialize()d with an Executor having Bars generated by BacktestDataSource;
			// HACK so that this.NotOnChartBarsKey won't help to pick up the Registered from NotOnChartBarsHelper;
			// HACK I want the this.NotOnChartDataSourceName be the same as before backtest, from a real datasource ("MOCK")
			this.NotOnChartDataSourceName = executor.Bars.DataSource.Name;

			NotOnChartBarsKey key = this.NotOnChartBarsKey;
			if (string.IsNullOrEmpty(this.NotOnChartSymbol) == false) {
				this.Executor.NotOnChartBarsHelper.FindNonChartBarsSubscribeRegisterForIndicator(key);
				if (this.BarsEffective == null) {
					string msg = "NOT_ON_CHART_BARS_HELPER_DOESNT_HAVE_BARS_REGISTERED[" + this.NotOnChartSymbol + "]";
					this.IndicatorErrorsOnBacktestStartingAppend(msg);
					Assembler.PopupException(msg + msig);
				}
			} else {
				if (this.NotOnChartBarScaleInterval != null) {
					if (this.NotOnChartBarScaleInterval.Scale == BarScale.Unknown
						&& this.NotOnChartBarScaleInterval == this.Executor.Bars.ScaleInterval) {
						string msg = "SET_TO_NULL_IF_SAME_AS_EXECUTORS_BARS";
						this.IndicatorErrorsOnBacktestStartingAppend(msg);
						Assembler.PopupException(msg + msig);
					}
					// v2 self-registered earlier
					Bars rescaled = this.Executor.NotOnChartBarsHelper.RescaledBarsGetRegisteredFor(key);
					if (rescaled == null) {
						// v1
						rescaled = this.Executor.NotOnChartBarsHelper.RescaleBarsAndRegister(key);
					}
					if (rescaled == null) {
						string msg = "BARS_NOT_RESCALABLE[" + this.Executor.Bars.SymbolIntervalScale + "]>[" + key + "]";
						this.IndicatorErrorsOnBacktestStartingAppend(msg);
						Assembler.PopupException(msg + msig);
					}
				}
			}
			
			string paramerersAllValidatedErrors = this.ParametersAllValidate();
			this.IndicatorErrorsOnBacktestStartingAppend(paramerersAllValidatedErrors);

			string indicatorSelfValidationErrors = this.BacktestStartingPreCheckErrors();
			this.IndicatorErrorsOnBacktestStartingAppend(indicatorSelfValidationErrors);
			
			if (string.IsNullOrEmpty(indicatorSelfValidationErrors) == false) {
				string msig2 = " Indicator[" + this.NameWithParameters + "].BacktestStartingPreCheck()";
				Assembler.PopupException(indicatorSelfValidationErrors + msig2);
			}
			bool backtestCanStart = string.IsNullOrEmpty(paramerersAllValidatedErrors)
								 && string.IsNullOrEmpty(indicatorSelfValidationErrors);
			return backtestCanStart;
		}
		
		public virtual string ParametersAllValidate() {
			string ret = "";
			foreach (IndicatorParameter param in this.ParametersByName.Values) {
				string paramErrors = param.ValidateSelf();
				if (string.IsNullOrEmpty(paramErrors)) continue; 
				if (string.IsNullOrEmpty(ret) == false) ret += "; ";
				ret += paramErrors;
			}

			if (string.IsNullOrEmpty(ret) == false) {
				string msig = " Indicator[" + this.NameWithParameters + "].ParametersAllValidate()";
				Assembler.PopupException(ret + msig);
			}

			return ret;
		}
		public virtual string BacktestStartingPreCheckErrors() {
			return null;
		}
		public virtual double CalculateOwnValueOnNewStaticBarFormed(Bar newStaticBar) {
			return double.NaN;
		}
		public virtual double CalculateOwnValueOnNewStreamingQuote(Quote newStreamingQuote) {
			//return double.NaN;
			return this.CalculateOwnValueOnNewStaticBarFormed(newStreamingQuote.ParentStreamingBar);
		}
		public void OnNewStaticBarFormed(Bar newStaticBar) {
			if (string.IsNullOrEmpty(this.IndicatorErrorsOnBacktestStarting) == false) return;
				
			int newStaticBarIndex = newStaticBar.ParentBarsIndex;
			 
			if (newStaticBarIndex <= this.OwnValuesCalculated.Count - 1) {
				double alreadyCalculated = this.OwnValuesCalculated[newStaticBarIndex];
				string msg = "THERE_MUST_BE_NO_OWN_VALUE_PRIOR_TO_INVOCATION alreadyCalculated[" + alreadyCalculated + "]";
				string msig = " OnNewStaticBarFormed(" + newStaticBar + ")";
				throw new Exception(msg + msig);
			}
			double derivedCalculated = this.CalculateOwnValueOnNewStaticBarFormed(newStaticBar);
			this.OwnValuesCalculated.Append(newStaticBar.DateTimeOpen, derivedCalculated);
		}
		public void OnNewStreamingQuote(Quote newStreamingQuote) {
			int streamingBarIndex = newStreamingQuote.ParentStreamingBar.ParentBarsIndex;
			double derivedCalculated = this.CalculateOwnValueOnNewStreamingQuote(newStreamingQuote);
			//this.OwnValuesCalculated.StreamingValue = derivedCalculated;
		}
		
		string format;
		public string Format {
			get {
				if (this.format == null) this.format = this.BarsEffective.Format; 
				return this.format;
			}
			set { this.format = value; }
		}
		public string FormatValue(double value) {
			return value.ToString(this.Format);
		}
		public string FormatValueForBar(Bar bar) {
			string ret = "";
			DateTime barDateTime = bar.DateTimeOpen;
			if (this.OwnValuesCalculated.ContainsKey(barDateTime) == false) {
				ret = "!ex[" + barDateTime.ToString(Assembler.DateTimeFormatIndicatorHasNoValuesFor) + "]";
				return ret;
			}
			double calculated = this.OwnValuesCalculated[barDateTime];
			if (double.IsNaN(calculated)) {
				ret = "NaN";
			}
			ret = this.FormatValue(calculated);
			return ret;
		}
		
		public virtual bool DrawValue(Graphics g, Bar bar, Rectangle barPlaceholder) {
			bool indicatorLegDrawn = false;
			if (Object.ReferenceEquals(this.OwnValuesCalculated, null)) return indicatorLegDrawn;
			string msig = " Indicator[" + this.NameWithParameters + "].DrawValue(" + bar + ")";
			//v1
			//if (this.OwnValuesCalculated.Count < bar.ParentBarsIndex) {
			//    string msg = "CAN_NOT_DRAW_INDICATOR_HAS_NO_VALUE_CALCULATED_FOR_BAR bar[" + bar + "]";
			//    //Assembler.PopupException(msg + msig);
			//    return indicatorLegDrawn;
			//}
			//double calculated = this.OwnValuesCalculated[bar.ParentBarsIndex];
			// v2-BEGIN
			if (null == this.NotOnChartBarScaleInterval && bar.ParentBarsIndex < this.FirstValidBarIndex) {
				return indicatorLegDrawn;	// INVALID FOR INDICATOR BASED ON NON_CHART_BARS_SCALE_INTERVAL
			}
			if (this.OwnValuesCalculated.ContainsKey(bar.DateTimeOpen) == false) {
				string msg = "CAN_NOT_DRAW_INDICATOR_HAS_NO_VALUE_CALCULATED_FOR_BAR bar[" + bar + "]";
				if (this.OwnValuesCalculated.ScaleInterval != bar.ParentBars.ScaleInterval) {
					msg += " OwnValuesCalculated.ScaleInterval[" + this.OwnValuesCalculated.ScaleInterval + "] != bar.ParentBars.ScaleInterval[" + bar.ParentBars.ScaleInterval + "]";
				}
				//Assembler.PopupException(msg + msig);
				return indicatorLegDrawn;
			}
			double calculated = this.OwnValuesCalculated[bar.DateTimeOpen];
			// v2-END
			if (double.IsNaN(calculated)) {
				string msg = "CAN_NOT_DRAW_INDICATOR_HAS_NAN_FOR_BAR bar[" + bar + "]";
				//Assembler.PopupException(msg + msig);
				return indicatorLegDrawn;
			}
			this.DotsExistsForCurrentSlidingWindow++;
			int x = this.HostPanelForIndicator.BarToX(bar.ParentBarsIndex) + this.HostPanelForIndicator.BarShadowOffset;
			int y = this.HostPanelForIndicator.ValueToYinverted(calculated);
			if (y == 0) return indicatorLegDrawn;
			int max = this.HostPanelForIndicator.ValueToYinverted(0);
			if (y == max) return indicatorLegDrawn;

			Point myDot = new Point(x, y);
			
			Point myDotPrev = myDot;
			int barIndexPrev = bar.ParentBarsIndex - 1;
			Bar barPrev = bar.ParentBars[barIndexPrev];
			if (barIndexPrev < 0 || barIndexPrev > this.OwnValuesCalculated.Count - 1) {
				string msg = "CANT_TAKE_PREV_INDICATOR_VALUE_PREVIOUS_BAR_BEOYND_AVAILABLE barIndexPrev[" + barIndexPrev + "] OwnValuesCalculated.Count[" + this.OwnValuesCalculated.Count + "]";
				Assembler.PopupException(msg);
			} else {
				double calculatedPrev = this.OwnValuesCalculated[barIndexPrev];
				if (double.IsNaN(calculatedPrev)) {
					string msg = "CAN_NOT_DRAW_INDICATOR_HAS_NAN_FOR_PREVBAR barIndexPrev[" + barIndexPrev + "]";
					//Assembler.PopupException(msg + msig);
				} else {
					int yPrev = this.HostPanelForIndicator.ValueToYinverted(calculatedPrev);
					int xPrev = this.HostPanelForIndicator.BarToX(barIndexPrev) + this.HostPanelForIndicator.BarShadowOffset;
					myDotPrev = new Point(xPrev, yPrev);
				}
			}
				
			g.DrawLine(this.PenForeground, myDot, myDotPrev);
			this.DotsDrawnForCurrentSlidingWindow++;
			
			indicatorLegDrawn = true;
			return indicatorLegDrawn;
		}
		public override string ToString() {
			return this.NameWithParameters;
		}
//		public void SetExternalSymbolScaleInterval(string notOnChartDataSourceName, string notOnChartSymbol, BarScaleInterval notOnChartBarScaleInterval) {
//			this.Executor.NotOnChartBarsHelper.RescaledBarsUnregisterFor(this.NotOnChartBarsKey);
//			
//			this.barsEffective_cached = null;
//			this.closesProxyEffective_cached = null;
//			
//			this.NotOnChartDataSourceName = notOnChartDataSourceName;
//			this.NotOnChartSymbol = notOnChartSymbol;
//			this.NotOnChartBarScaleInterval = notOnChartBarScaleInterval;
//		}
		
	}
}
