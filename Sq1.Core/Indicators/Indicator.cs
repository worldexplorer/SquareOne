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
	public abstract partial class Indicator {
		public string Name;
		public ChartPanelType ChartPanelType;

		public DataSeriesProxyableFromBars DataSeriesProxyFor;
		public ScriptExecutor Executor;
		public DataSeriesTimeBased OwnValuesCalculated;

		// FirstValidBarIndex::virtual>abstract koz inside base class (Indicator) it's still zero!!
		public abstract int FirstValidBarIndex { get; set; }

		// USE_NOT_ON_CHART_CONCEPT_WHEN_YOU_HIT_THE_NEED_IN_IT
		//public string NotOnChartDataSourceName;				//empty means the same as on the chart
		//public string NotOnChartSymbol;						//empty means the same as on the chart
		//public BarScaleInterval NotOnChartBarScaleInterval;	//null  means the same as on the chart
		//public NotOnChartBarsKey NotOnChartBarsKey { get {
		//		if (string.IsNullOrEmpty(this.NotOnChartSymbol)
		//			//HACK SUBSTITUTED_DURING_BACKTEST_FROM_BACKTESTDATASOURCE_TO_MOCK && string.IsNullOrEmpty(this.NotOnChartDataSourceName)
		//			&& this.NotOnChartBarScaleInterval == null) {
		//			return null;
		//		}
		//		//if (this.Executor == null) {
		//		//	// BactestStarting 
		//		//	return null;
		//		//}
		//		try {
		//			string symbol = string.IsNullOrEmpty(this.NotOnChartSymbol)
		//							? this.Executor.Bars.Symbol : this.NotOnChartSymbol;
		//			string dataSourceName = string.IsNullOrEmpty(this.NotOnChartDataSourceName)
		//							? this.Executor.Bars.DataSource.Name : this.NotOnChartDataSourceName;
		//			BarScaleInterval scaleInterval = this.NotOnChartBarScaleInterval == null
		//							? this.Executor.Bars.ScaleInterval : this.NotOnChartBarScaleInterval;
		//			// by saying {this.OwnValuesCalculated.ScaleInterval == scaleInterval } you invoke BarScaleInterval.{static bool operator ==} - UNDEBUGGABLE
		//			if (scaleInterval != null && scaleInterval.Equals(this.OwnValuesCalculated.ScaleInterval) == false) {
		//				this.OwnValuesCalculated.ScaleInterval = scaleInterval;
		//			}
		//			return new NotOnChartBarsKey(scaleInterval, symbol, dataSourceName);
		//		} catch (Exception ex) {
		//			Assembler.PopupException("Indicator.NotOnChartBarsKey_get()", ex);
		//			return null;
		//		}
		//	} }
		
		private Bars barsEffective_cached;
		public Bars BarsEffective { get {
				if (this.Executor == null) {
					Assembler.PopupException("YOU_FORGOT_TO_INVOKE_INDICATOR.INITIALIZE()_OR_WAIT_UNTIL_ITLLBE_INVOKED_LATER Executor=null in Indicator.BarsEffective_get()");
					Debugger.Break();
					return this.barsEffective_cached;
				}
				// USE_NOT_ON_CHART_CONCEPT_WHEN_YOU_HIT_THE_NEED_IN_IT
				//this.barsEffective_cached = null;
				////if (this.barsEffective_cached == null) {
				//	NotOnChartBarsKey key = this.NotOnChartBarsKey;
				//	if (key != null) {
				//		this.barsEffective_cached = this.Executor.NotOnChartBarsHelper.RescaledBarsGetRegisteredFor(key);
				//		bool barsEffective_cachedNull = ReferenceEquals(this.barsEffective_cached, null);
				//		if (barsEffective_cachedNull) {
				//			this.barsEffective_cached = this.Executor.NotOnChartBarsHelper.RescaleBarsAndRegister(key);
				//		}
				//	}
				//	bool barsEffective_cachedNull2 = ReferenceEquals(this.barsEffective_cached, null);
				//	if (barsEffective_cachedNull2) {
				//		this.barsEffective_cached = this.Executor.Bars;
				//	}
				////}
				//return this.barsEffective_cached;
				return this.Executor.Bars;
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
			
		
		public Dictionary<string, IndicatorParameter> ParametersByName { get {
				Dictionary<string, IndicatorParameter> ret = new Dictionary<string, IndicatorParameter>();
				Type myChild = this.GetType();
				PropertyInfo[] lookingForIndicatorParameterProperties = myChild.GetProperties();
				foreach (PropertyInfo indicatorParameterPropertyInfo in lookingForIndicatorParameterProperties) {
					Type expectingIndicatorParameterType = indicatorParameterPropertyInfo.PropertyType;
					bool isIndicatorParameterChild = typeof(IndicatorParameter).IsAssignableFrom(expectingIndicatorParameterType);
					if (isIndicatorParameterChild == false) continue;
					object expectingConstructedNonNull = indicatorParameterPropertyInfo.GetValue(this, null);
					if (expectingConstructedNonNull == null) {
						string msg = "INDICATOR_DEVELOPER,INITIALIZE_INDICATOR_PARAMETER_IN_INDICATOR_CONSTRUCTOR Indicator[" + this.Name + "].ctor()"
							+ " { iParamFound = new new IndicatorParameter([" + indicatorParameterPropertyInfo.Name + "], cur, min, max, increment); }";
						Assembler.PopupException(msg);
						#if DEBUG
						Debugger.Break();
						#endif
						Assembler.PopupException(msg);
						continue;
					}
					IndicatorParameter indicatorParameterInstance = expectingConstructedNonNull as IndicatorParameter; 
					// NOPE_COZ_ATR.ParamPeriod=new IndicatorParameter("Period",..) indicatorParameterInstance.Name = indicatorParameterPropertyInfo.Name;
					indicatorParameterInstance.ValidateSelf();
					ret.Add(indicatorParameterInstance.Name, indicatorParameterInstance);
				}
				return ret;
			} }
		
		public string parametersAsStringShort_cached;
		public string ParametersAsStringShort { get {
				if (parametersAsStringShort_cached == null) {
					StringBuilder sb = new StringBuilder();
					foreach (string paramName in this.ParametersByName.Keys) {
						IndicatorParameter param = this.ParametersByName[paramName];
						if (sb.Length > 0) sb.Append(",");
						sb.Append(param.ToString());
					}
					this.parametersAsStringShort_cached = sb.ToString();
				}
				return this.parametersAsStringShort_cached;
			} }
//		public string ParametersAsStringLong { get ; }
		
		public string NameWithParameters { get {
				string parameters = this.ParametersAsStringShort;
				if (string.IsNullOrEmpty(parameters)) parameters = "NOT_BUILT_YET_ParametersByName_DIDNT_INVOKE_BuildParametersFromAttributes()";
				string ret = this.Name + " (" + parameters + ")";
				// USE_NOT_ON_CHART_CONCEPT_WHEN_YOU_HIT_THE_NEED_IN_IT
				//string otherSymbolTimeframe = "";
				//if (string.IsNullOrEmpty(this.NotOnChartSymbol)) {
				//	//if (this.NotOnChartBarScaleInterval != null) otherSymbolTimeframe += "~";			// "timeframe converted"
				//	if (Object.ReferenceEquals(this.NotOnChartBarScaleInterval, null) == false) otherSymbolTimeframe += "~";			// "timeframe converted"
				//	//otherSymbolTimeframe += (this.barsEffective_cached != null) ? this.barsEffective_cached.SymbolIntervalScale : "NULL";
				//	//otherSymbolTimeframe += (this.BarsEffective != null) ? this.BarsEffective.SymbolIntervalScale : "NULL";
				//	if (Object.ReferenceEquals(this.NotOnChartBarScaleInterval, null) == false) otherSymbolTimeframe += this.BarsEffective.SymbolIntervalScale;
				//} else {
				//	otherSymbolTimeframe += "(" + this.NotOnChartSymbol;
				//	//if (this.NotOnChartBarScaleInterval != null) otherSymbolTimeframe += ":" + this.NotOnChartBarScaleInterval;
				//	if (Object.ReferenceEquals(this.NotOnChartBarScaleInterval, null) == false) otherSymbolTimeframe += ":" + this.NotOnChartBarScaleInterval;
				//	otherSymbolTimeframe += ")!";								// "Symbol is not from the chart"
				//	//otherSymbolTimeframe += (this.NotOnChartBarScaleInterval == null) ? "=" : "~";		// "timeframe converted"
				//	otherSymbolTimeframe += Object.ReferenceEquals(this.NotOnChartBarScaleInterval, null) ? "=" : "~";		// "timeframe converted"
				//	otherSymbolTimeframe += (this.barsEffective_cached != null) ? this.barsEffective_cached.SymbolIntervalScale : "NULL";
				//}
				//ret += otherSymbolTimeframe;
				return ret;
			} }

		public string IndicatorErrorsOnBacktestStarting;
		
		public int DotsDrawnForCurrentSlidingWindow;		// nope I won't use a separate "responsibility" (I told you "SOLID principles are always misused" :)
		public int DotsExistsForCurrentSlidingWindow;		// just because the object itself is the most convenient place to incapsulate it
		
		public Indicator() {
			Name = "INDICATOR_NAME_NOT_SET_IN_DERIVED_CONSTRUCTOR";
			DataSeriesProxyFor = DataSeriesProxyableFromBars.Close;
			ChartPanelType = ChartPanelType.PanelPrice;
			// MOVED_TO_BacktestStarting();
			//this.OwnValuesCalculated = new DataSeriesTimeBased(new BarScaleInterval(BarScale.Unknown, 0), this.Name);
			LineColor = Color.Indigo;
			LineWidth = 1;
			Decimals = 2;
		}

		public void Initialize(HostPanelForIndicator panelNamedFolding) {
			this.HostPanelForIndicator = panelNamedFolding;
			this.closesProxyEffective_cached = null;
			this.barsEffective_cached = null;
			//this.OwnValuesCalculated.Clear();
			////this.OwnValuesCalculated.Description = this.Name;	//appears after .BuildParametersFromAttributes();
			//this.OwnValuesCalculated.Description = this.NameWithParameters;
		}
		public void IndicatorErrorsOnBacktestStartingAppend(string msg, string separator = "; ") {
			if (string.IsNullOrEmpty(msg)) return;
			if (string.IsNullOrEmpty(this.IndicatorErrorsOnBacktestStarting) == false) msg += separator;
			this.IndicatorErrorsOnBacktestStarting += msg;
		}
		public bool BacktestStartingConstructOwnValuesValidateParameters(ScriptExecutor executor) {
			this.Executor = executor;
			string msig = " Indicator[" + this.NameWithParameters + "].BacktestStarting()";

			// HACK new Indicator will be Initialize()d with an Executor having Bars generated by BacktestDataSource;
			// HACK so that this.NotOnChartBarsKey won't help to pick up the Registered from NotOnChartBarsHelper;
			// HACK I want the this.NotOnChartDataSourceName be the same as before backtest, from a real datasource ("MOCK")
			// USE_NOT_ON_CHART_CONCEPT_WHEN_YOU_HIT_THE_NEED_IN_IT
			//this.NotOnChartDataSourceName = executor.Bars.DataSource.Name;
			//NotOnChartBarsKey key = this.NotOnChartBarsKey;
			//if (string.IsNullOrEmpty(this.NotOnChartSymbol) == false) {
			//	this.Executor.NotOnChartBarsHelper.FindNonChartBarsSubscribeRegisterForIndicator(key);
			//	if (this.BarsEffective == null) {
			//		string msg = "NOT_ON_CHART_BARS_HELPER_DOESNT_HAVE_BARS_REGISTERED[" + this.NotOnChartSymbol + "]";
			//		this.IndicatorErrorsOnBacktestStartingAppend(msg);
			//		Assembler.PopupException(msg + msig);
			//	}
			//} else {
			//	if (this.NotOnChartBarScaleInterval != null) {
			//		if (this.NotOnChartBarScaleInterval.Scale == BarScale.Unknown
			//			&& this.NotOnChartBarScaleInterval == this.Executor.Bars.ScaleInterval) {
			//			string msg = "SET_TO_NULL_IF_SAME_AS_EXECUTORS_BARS";
			//			this.IndicatorErrorsOnBacktestStartingAppend(msg);
			//			Assembler.PopupException(msg + msig);
			//		}
			//		// v2 self-registered earlier
			//		Bars rescaled = this.Executor.NotOnChartBarsHelper.RescaledBarsGetRegisteredFor(key);
			//		if (rescaled == null) {
			//			// v1
			//			rescaled = this.Executor.NotOnChartBarsHelper.RescaleBarsAndRegister(key);
			//		}
			//		if (rescaled == null) {
			//			string msg = "BARS_NOT_RESCALABLE[" + this.Executor.Bars.SymbolIntervalScale + "]>[" + key + "]";
			//			this.IndicatorErrorsOnBacktestStartingAppend(msg);
			//			Assembler.PopupException(msg + msig);
			//		}
			//	}
			//}

			this.parametersAsStringShort_cached = null;
			this.OwnValuesCalculated = new DataSeriesTimeBased(this.Executor.Bars.ScaleInterval, this.NameWithParameters);
			
			string paramerersAllValidatedErrors = this.ParametersAllValidate();
			this.IndicatorErrorsOnBacktestStartingAppend(paramerersAllValidatedErrors);

			this.Decimals = Math.Max(this.Executor.Bars.SymbolInfo.DecimalsPrice, 1);	// for SBER, constant ATR shows truncated (imprecise) mouseOver value on gutter
			
			string indicatorSelfValidationErrors = this.BacktestStartingPreCheckErrors();
			this.IndicatorErrorsOnBacktestStartingAppend(indicatorSelfValidationErrors);
			
			if (string.IsNullOrEmpty(this.IndicatorErrorsOnBacktestStarting) == false) {
				string msig2 = " Indicator[" + this.NameWithParameters + "].BacktestStartingPreCheck()";
				Assembler.PopupException(this.IndicatorErrorsOnBacktestStarting + msig2);
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
			if (this.OwnValuesCalculated.ContainsDate(newStaticBar.DateTimeOpen)) {
				string msg = "PROHIBITED_TO_CALCULATE_EACH_QUOTE_SLOW DONT_INVOKE_ME_TWICE on[" + newStaticBar.DateTimeOpen + "]";
				#if DEBUG
				Debugger.Break();
				#endif
				Assembler.PopupException(msg);
			}
			return double.NaN;
		}
		public virtual double CalculateOwnValueOnNewStreamingQuote(Quote newStreamingQuote) {
			return double.NaN;
			//PROHIBITED_TO_CALCULATE_EACH_QUOTE_SLOW return this.CalculateOwnValueOnNewStaticBarFormed(newStreamingQuote.ParentStreamingBar);
		}
		public void OnNewStaticBarFormed(Bar newStaticBar) {
			if (string.IsNullOrEmpty(this.IndicatorErrorsOnBacktestStarting) == false) {
				return;
			}
				
			double derivedCalculated = this.CalculateOwnValueOnNewStaticBarFormed(newStaticBar);

			int newStaticBarIndex = newStaticBar.ParentBarsIndex;
			int differenceMustNotBeMoreThanOne = newStaticBarIndex - this.OwnValuesCalculated.StreamingIndex;
			if (differenceMustNotBeMoreThanOne > 1) {
				string msig = " OnNewStaticBarFormed(" + newStaticBar.ToString() + ")";
				string msg = "INDICATOR_CALCULATE_OWN_VALUE_WASNT_CALLED_WITHIN_LAST_BARS[" + differenceMustNotBeMoreThanOne + "]";
				#if DEBUG
				//Debugger.Break();
				#endif
				throw new Exception(msg + msig);
			}
			if (differenceMustNotBeMoreThanOne == 1) {
				DateTime streamingBarDateTime = newStaticBar.DateTimeOpen;
				this.OwnValuesCalculated.Append(streamingBarDateTime, derivedCalculated);
			} else {
				this.OwnValuesCalculated.StreamingValue = derivedCalculated;
			}
		}
		public void OnNewStreamingQuote(Quote newStreamingQuote) {
			if (this.OwnValuesCalculated == null) {
				string msg = "HAPPENS_DURING_REBACKTEST_AFTER_ABORTION";
				return;
			}
			
			double derivedCalculated = this.CalculateOwnValueOnNewStreamingQuote(newStreamingQuote);
			
			int streamingBarIndex = newStreamingQuote.ParentBarStreaming.ParentBarsIndex;
			int differenceMustNotBeMoreThanOne = streamingBarIndex - this.OwnValuesCalculated.StreamingIndex;
			if (differenceMustNotBeMoreThanOne > 1) {
				string msig = " OnNewStreamingQuote(" + newStreamingQuote.ToString() + ")";
				string msg = "INDICATOR_CALCULATE_OWN_VALUE_WASNT_CALLED_WITHIN_LAST_BARS[" + differenceMustNotBeMoreThanOne + "]";
				#if DEBUG
				//Debugger.Break();
				#endif
				throw new Exception(msg + msig);
			}
			if (differenceMustNotBeMoreThanOne == 1) {
				DateTime streamingBarDateTime = newStreamingQuote.ParentBarStreaming.DateTimeOpen;
				this.OwnValuesCalculated.Append(streamingBarDateTime, derivedCalculated);
			} else {
				this.OwnValuesCalculated.StreamingValue = derivedCalculated;
			}
		}
		
		//v1
		//string format;
		//public string Format {
		//	get {
		//		if (this.format == null) this.format = this.BarsEffective.Format; 
		//		return this.format;
		//	}
		//	set { this.format = value; }
		//}
		//v2 begin
		public string FormatForcedDecimalsIndependent;
		public int Decimals;
		//v2 end
		public string Format { get {
				if (string.IsNullOrEmpty(this.FormatForcedDecimalsIndependent) == false) return this.FormatForcedDecimalsIndependent;
				return "N" + this.Decimals;
			} }
		public string FormatValue(double value) {
			return value.ToString(this.Format);
		}
		public string FormatValueForBar(Bar bar, DataSeriesTimeBased ownValuesOrOverridenBandSeries = null) {
			string ret = "";
			if (ownValuesOrOverridenBandSeries == null) {
				ownValuesOrOverridenBandSeries = this.OwnValuesCalculated;
			}
				
			DateTime barDateTime = bar.DateTimeOpen;
			int barIndex = bar.ParentBarsIndex;
			if (ownValuesOrOverridenBandSeries.ContainsDate(barDateTime) == false) {
				ret = "!ex[" + barDateTime.ToString(Assembler.DateTimeFormatIndicatorHasNoValuesFor) + "]";
				return ret;
			}
			double calculated = ownValuesOrOverridenBandSeries[barIndex];
			if (double.IsNaN(calculated)) {
				ret = "NaN";
			}
			ret = this.FormatValue(calculated);
			return ret;
		}
		public virtual SortedDictionary<string, string> ValuesForTooltipPrice(Bar bar) {
			SortedDictionary<string, string> ret = new SortedDictionary<string, string>();
			ret.Add(this.Name, this.FormatValueForBar(bar));
			return ret;
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
