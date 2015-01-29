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
		public	string						Name;
		public	ChartPanelType				ChartPanelType;
		public	DataSeriesProxyableFromBars	DataSeriesProxyFor;
		public	ScriptExecutor				Executor			{ get; protected set; }
		public	DataSeriesTimeBased			OwnValuesCalculated	{ get; protected set; }
		public	abstract int				FirstValidBarIndex	{ get; protected set; }	// FirstValidBarIndex::virtual>abstract koz inside base class (Indicator) it's still zero!!

				Bars barsEffective_cached;
		public	Bars BarsEffective								{ get {
				if (this.Executor == null) {
					string msg = "YOU_FORGOT_TO_INVOKE_INDICATOR.INITIALIZE()_OR_WAIT_UNTIL_ITLLBE_INVOKED_LATER Executor=null in Indicator.BarsEffective_get()";
					Assembler.PopupException(msg);
					return this.barsEffective_cached;
				}
				if (this.barsEffective_cached == null) {
					this.barsEffective_cached = this.Executor.Bars;
				}
				if (this.barsEffective_cached != this.Executor.Bars) {
					string msg = "INDICATOR_SWITCHED_BARS (Bars were replaced to Backtesting growing copy and create new Proxy)"
						+ this.barsEffective_cached.ToString() + " => " + this.Executor.Bars.ToString();
					Assembler.PopupException(msg, null, false);
					this.barsEffective_cached = this.Executor.Bars;
				}
				return this.barsEffective_cached;
			} }

				DataSeriesProxyBars closesProxyEffective_cached;
		public	DataSeriesProxyBars ClosesProxyEffective		{ get {
				if (this.closesProxyEffective_cached == null) {
					this.closesProxyEffective_cached = new DataSeriesProxyBars(this.BarsEffective, this.DataSeriesProxyFor);
				}
				if (this.closesProxyEffective_cached.BarsBeingProxied != this.BarsEffective) {
					string msg = "INDICATOR_SWITCHED_BARS (Bars were replaced to Backtesting growing copy and create new Proxy)"
						+ this.closesProxyEffective_cached.ToString() + " => " + this.BarsEffective.ToString();
					Assembler.PopupException(msg, null, false);
					this.closesProxyEffective_cached = new DataSeriesProxyBars(this.BarsEffective, this.DataSeriesProxyFor);
				}
				return this.closesProxyEffective_cached;
			} }
		
		public	HostPanelForIndicator HostPanelForIndicator		{ get; protected set; }

				Color lineColor;
		public	Color LineColor {
			get { return this.lineColor; }
			set { this.lineColor = value; this.brushForeground = null; this.penForeground = null; }
		}
				int lineWidth;
		public	int LineWidth {
			get { return this.lineWidth; }
			set { this.lineWidth = value; this.brushForeground = null; this.penForeground = null; }
		}

				Brush brushForeground;
		public	Brush BrushForeground							{ get {
				if (this.brushForeground == null) {
					this.brushForeground = new SolidBrush(this.LineColor);
				}
				return this.brushForeground;
			} }
				Pen penForeground;
		public	Pen PenForeground { get {
				if (this.penForeground == null) {
					this.penForeground = new Pen(this.LineColor, this.LineWidth);
				}
				return this.penForeground;
			} }
			
		
		public	Dictionary<string, IndicatorParameter> ParametersByName { get {
				Dictionary<string, IndicatorParameter> ret = new Dictionary<string, IndicatorParameter>();
				Type myChild = this.GetType();
				//v1
				//PropertyInfo[] lookingForIndicatorParameterProperties = myChild.GetProperties();
				//foreach (PropertyInfo indicatorParameterPropertyInfo in lookingForIndicatorParameterProperties) {
				//	Type expectingIndicatorParameterType = indicatorParameter.PropertyType;
				//v2
				FieldInfo[] lookingForIndicatorParameterFields = myChild.GetFields();
				foreach (FieldInfo indicatorParameter in lookingForIndicatorParameterFields) {
					Type expectingIndicatorParameterType = indicatorParameter.FieldType;
					bool isIndicatorParameterChild = typeof(IndicatorParameter).IsAssignableFrom(expectingIndicatorParameterType);
					if (isIndicatorParameterChild == false) continue;
					//object expectingConstructedNonNull = indicatorParameter.GetValue(this, null);
					object expectingConstructedNonNull = indicatorParameter.GetValue(this);
					if (expectingConstructedNonNull == null) {
						string msg = "INDICATOR_DEVELOPER,INITIALIZE_INDICATOR_PARAMETER_IN_INDICATOR_CONSTRUCTOR Indicator[" + this.Name + "].ctor()"
							+ " { iParamFound = new new IndicatorParameter([" + indicatorParameter.Name + "], cur, min, max, increment); }";
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
		
		public	string	parametersAsStringShort_cached;
		public	string	ParametersAsStringShort					{ get {
				if (parametersAsStringShort_cached == null) {
					StringBuilder sb = new StringBuilder();
					foreach (string paramName in this.ParametersByName.Keys) {
						IndicatorParameter param = this.ParametersByName[paramName];
						if (sb.Length > 0) sb.Append(",");
						sb.Append(paramName);
						sb.Append(":");
						string shortName = param.ValuesAsString;
						sb.Append(shortName);
					}
					this.parametersAsStringShort_cached = sb.ToString();
				}
				return this.parametersAsStringShort_cached;
			} }
		
		public	string	NameWithParameters { get {
				string parameters = this.ParametersAsStringShort;
				if (string.IsNullOrEmpty(parameters)) parameters = "NOT_BUILT_YET_ParametersByName_DIDNT_INVOKE_BuildParametersFromAttributes()";
				string ret = this.Name + " (" + parameters + ")";
				return ret;
			} }

		public	string	IndicatorErrorsOnBacktestStarting		{ get; protected set; }
		
		public	int		DotsDrawnForCurrentSlidingWindow;									// nope I won't use a separate "responsibility" (I told you "SOLID principles are always misused" :)
		public	int		DotsExistsForCurrentSlidingWindow		{ get; protected set; }		// just because the object itself is the most convenient place to incapsulate it
		
		public Indicator() {
			Name = "INDICATOR_NAME_NOT_SET_IN_DERIVED_CONSTRUCTOR";
			DataSeriesProxyFor = DataSeriesProxyableFromBars.Close;
			ChartPanelType = ChartPanelType.PanelPrice;
			OwnValuesCalculated = new DataSeriesTimeBased(new BarScaleInterval(BarScale.Unknown, 0), this.Name);
			LineColor = Color.Indigo;
			LineWidth = 1;
			Decimals = 2;
		}

		public virtual void ResetBarsEffectiveProxyForBacktestStartingOrSwitchToOriginalBarsContinueToLiveNorecalculateStopped() {
			this.parametersAsStringShort_cached = null;
			// muting INDICATOR_SWITCHED_BARS
			this.barsEffective_cached = this.Executor.Bars;
			this.closesProxyEffective_cached = new DataSeriesProxyBars(this.BarsEffective, this.DataSeriesProxyFor);

			if (this.OwnValuesCalculated.Count == 0) {
				string msg = "NO_NEED_TO_CLEAR";
				//Assembler.PopupException(msg);
			} else {
				if (this.BarsEffective.Count == 0) {
					string msg = "SORRY_FOR_THE_MESS__VALID_ONLY_FOR_MANUAL_REBACKTEST_DURING_LIVE";
					this.OwnValuesCalculated.Clear();
				}
			}
			if (this.OwnValuesCalculated.Description == this.NameWithParameters) {
				string msg = "NO_NEED_TO_SET_SAME_DESCRIPTION";
				//Assembler.PopupException(msg);
			} else {
				//this.OwnValuesCalculated.Description = this.Name;	//appears after .BuildParametersFromAttributes();
				this.OwnValuesCalculated.Description = this.NameWithParameters;
			}
		}
		public void Initialize(HostPanelForIndicator panelNamedFolding) {
			this.HostPanelForIndicator = panelNamedFolding;
		}
		public void IndicatorErrorsOnBacktestStartingAppend(string msg, string separator = "; ") {
			if (string.IsNullOrEmpty(msg)) return;
			if (string.IsNullOrEmpty(this.IndicatorErrorsOnBacktestStarting) == false) msg += separator;
			this.IndicatorErrorsOnBacktestStarting += msg;
		}
		public bool BacktestStartingConstructOwnValuesValidateParameters(ScriptExecutor executor) {
			this.Executor = executor;
			string msig = " Indicator[" + this.NameWithParameters + "].BacktestStarting()";

			this.ResetBarsEffectiveProxyForBacktestStartingOrSwitchToOriginalBarsContinueToLiveNorecalculateStopped();
			this.OwnValuesCalculated = new DataSeriesTimeBased(this.BarsEffective.ScaleInterval, this.NameWithParameters);
			
			string paramerersAllValidatedErrors = this.ParametersAllValidate();
			this.IndicatorErrorsOnBacktestStartingAppend(paramerersAllValidatedErrors);

			this.Decimals = Math.Max(this.BarsEffective.SymbolInfo.DecimalsPrice, 1);	// for SBER, constant ATR shows truncated (imprecise) mouseOver value on gutter

			string indicatorSelfValidationErrors = this.InitializeBacktestStartingPreCheckErrors();
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
		public virtual string InitializeBacktestStartingPreCheckErrors() {
			return null;
		}
		public virtual double CalculateOwnValueOnNewStaticBarFormed(Bar newStaticBar) {
			if (this.OwnValuesCalculated.ContainsDate(newStaticBar.DateTimeOpen)) {
				string msg = "PROHIBITED_TO_CALCULATE_EACH_QUOTE_SLOW DONT_INVOKE_ME_TWICE on[" + newStaticBar.DateTimeOpen + "]";
				Assembler.PopupException(msg);
			}
			return double.NaN;
		}
		public virtual double CalculateOwnValueOnNewStreamingQuote(Quote newStreamingQuote) {
			return double.NaN;
			//PROHIBITED_TO_CALCULATE_EACH_QUOTE_SLOW return this.CalculateOwnValueOnNewStaticBarFormed(newStreamingQuote.ParentStreamingBar);
		}
		bool canRunCalculation(bool popupException = false, Bar newStaticBar = null, Quote quote = null) {
			bool ret = false;
			if (this.OwnValuesCalculated == null) {
				string msg = "HAPPENS_DURING_REBACKTEST_AFTER_ABORTION this.OwnValuesCalculated=null " + this.ToString();
				Assembler.PopupException(msg);
				return ret;
			}
			if (string.IsNullOrEmpty(this.IndicatorErrorsOnBacktestStarting) == false) {
				if (popupException) {
					string msg = "I_REFUSE_TO_CALCULATE_DUE_TO_PREVIOUS_ERRORS Indicator.OnNewStaticBarFormed(" + newStaticBar + ")" + this.ToString();
					Assembler.PopupException(msg);
				}
				return ret;
			}

			if (this.ClosesProxyEffective == null) {
				if (popupException) {
					string msg = "NO_BARS_FOR_INDICATOR this.ClosesProxyEffective=null" + this.ToString();
					Assembler.PopupException(msg);
				}
				return ret;
			}
			if (this.ClosesProxyEffective.Count - 1 < this.FirstValidBarIndex) {
				//if (popupException) {
				//    string msg = "DONT_INVOKE_ME__MAKE_THIS_CHECK_UPSTACK"
				//        + " base.ClosesProxyEffective.Count-1[" + (this.ClosesProxyEffective.Count - 1)
				//        + "] < this.FirstValidBarIndex[" + this.FirstValidBarIndex + "]"
				//        + this.ToString();
				//    Assembler.PopupException(msg);
				//}
				//if (newStaticBar.ParentBarsIndex - 1 < this.FirstValidBarIndex) {
				//    int barsWaitToFirstIndicatorValidIndex = this.FirstValidBarIndex - newStaticBar.ParentBarsIndex;
				//    string msg = "barsWaitToFirstIndicatorValidIndex[" + barsWaitToFirstIndicatorValidIndex + "] newStaticBar.ParentBarsIndex - 1 < this.FirstValidBarIndex";
				//    //Assembler.PopupException(msg, null, false);
				//    return;
				//}
				return ret;
			}
			ret = true;
			return ret;
		}
		public virtual void OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppended(Bar newStaticBar) {
			bool canRunCalculation = this.canRunCalculation();
			if (canRunCalculation == false) {
				this.OwnValuesCalculated.Append(newStaticBar.DateTimeOpen, double.NegativeInfinity);
				return;
			}

			double derivedCalculated = this.CalculateOwnValueOnNewStaticBarFormed(newStaticBar);

			int barsAheadOfIndicator = newStaticBar.ParentBarsIndex - this.OwnValuesCalculated.LastIndex;
			if (barsAheadOfIndicator != 1) {
				string msig = " barsAheadOfIndicator[" + barsAheadOfIndicator + "]" + this.indexesAsString + " //OnNewStaticBarFormed(" + newStaticBar.ToString() + ")";
				string msg = (barsAheadOfIndicator == 0)
					? "I_REFUSE_TO_REPLACE_STREAMING_VALUE__YOU_MUST_CALCULATE_OWN_VALUE_ONCE_PER_BAR_FOR_PREV_BAR"
					: "INDICATOR_CALCULATE_OWN_VALUE_WASNT_CALLED_WITHIN_LAST_BARS";
				Assembler.PopupException(msg + msig);
				return;
			}
			this.OwnValuesCalculated.Append(newStaticBar.DateTimeOpen, derivedCalculated);
		}
		public virtual void OnNewStreamingQuote(Quote newStreamingQuote) {
			bool canRunCalculation = this.canRunCalculation();
			if (canRunCalculation == false) return;
			
			double derivedCalculated = this.CalculateOwnValueOnNewStreamingQuote(newStreamingQuote);
			
			int barsAheadOfIndicator = newStreamingQuote.ParentBarStreaming.ParentBarsIndex - this.OwnValuesCalculated.LastIndex;
			string msig = " barsAheadOfIndicator[" + barsAheadOfIndicator + "]" + this.indexesAsString + " //OnNewStreamingQuote(" + newStreamingQuote.ToString() + ")";
			if (barsAheadOfIndicator != 0) {
				if (newStreamingQuote.IntraBarSerno == 0) {
					string msg2 = "MUST_HAPPEN_UNFINISHED___??IM_GONNA_BE_INVOKED_ONCE_AGAIN_AFTER_OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppended()";
					return;
				}
				string msg = "MUST_HAPPEN_UNFINISHED___I_REFUSE_TO_ADD_NEW_VALUE__INVOKE_FIRST_OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppended()";
				//Assembler.PopupException(msg + msig);
				return;
			}
			string msg3 = "I_DIDNT_FINISH_CLEANING_UP_LIFECYLE";
			Assembler.PopupException(msg3 + msig);
			return;
			this.OwnValuesCalculated.LastValue = derivedCalculated;
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
		public	string	FormatForcedDecimalsIndependent			{ get; protected set; }
		public	int		Decimals								{ get; protected set; }
		//v2 end
		public	string	Format									{ get {
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
		
				string	indexesAsString							{ get {
			return 	" LastValue[" + this.OwnValuesCalculated.LastValue.ToString(this.Format) + "]"
					+ " LastIndex[" + this.OwnValuesCalculated.LastIndex + "]"
					+ " LastValueAppended[" + this.OwnValuesCalculated.LastValueAppended.ToString(this.Format) + "]"
					+ " LastDateAppended[" + this.OwnValuesCalculated.LastDateAppended + "]"
					+ " Count[" + this.OwnValuesCalculated.Count + "]";
		} }
		public override string ToString() {
			return this.NameWithParameters;
		}
	}
}
