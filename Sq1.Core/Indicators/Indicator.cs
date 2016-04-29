using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Indicators {
	public abstract partial class Indicator {
		public	static	int AbsnoCurrent = 0;
		public			int AbsnoInstance;
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
					string instances = this.barsEffective_cached.MyInstanceAsString + " =>" + this.Executor.Bars.MyInstanceAsString;
					string msg = "INDICATOR_SWITCHED_BARS_BarsEffective " + instances
						//+ " (Bars were replaced to Backtester's growing copy and new Proxy)"
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
					string msg = "INDICATOR_SWITCHED_BARS_ClosesProxyEffective (Bars were replaced to Backtesting growing copy and create new Proxy)"
						+ this.closesProxyEffective_cached.ToString() + " => " + this.BarsEffective.ToString();
					Assembler.PopupException(msg, null, false);
					this.closesProxyEffective_cached = new DataSeriesProxyBars(this.BarsEffective, this.DataSeriesProxyFor);
				}
				return this.closesProxyEffective_cached;
			} }

		public		string	IndicatorErrorsOnBacktestStarting		{ get; protected set; }
		
		public		int		DotsDrawnForCurrentSlidingWindow;									// nope I won't use a separate "responsibility" (I told you "SOLID principles are always misused" :)
		public		int		DotsExistsForCurrentSlidingWindow		{ get; protected set; }		// just because the object itself is the most convenient place to incapsulate it

		protected	bool	AllowsOnNewQuote;
		
		protected Indicator() {
			AbsnoInstance = ++AbsnoCurrent;
			Name = "INDICATOR_NAME_NOT_SET_IN_DERIVED_CONSTRUCTOR //will be replaced by Script.IndicatorsByName_ReflectedCached()";
			DataSeriesProxyFor = DataSeriesProxyableFromBars.Close;
			ChartPanelType = ChartPanelType.PanelPrice;
			OwnValuesCalculated = new DataSeriesTimeBased(new BarScaleInterval(BarScale.Unknown, 0), this.Name);
			LineColor = Color.Indigo;
			LineWidth = 1;
			Decimals = 2;
			parametersByName = new Dictionary<string, IndicatorParameter>();
			parametersByName_ReflectionForced = true;
			AllowsOnNewQuote = false;
		}

		public void SetHostPanel(HostPanelForIndicator panelNamedFolding) {
			if (this.HostPanelForIndicator == panelNamedFolding) {
				string msg = "INDICATOR_ALREADY_INITIALIZED_WITH_SAME_HOST_PANEL [" + this + "] this.HostPanelForIndicator[" + this.HostPanelForIndicator + "]";
				//Assembler.PopupException(msg, null, false);
				return;
			}
			this.HostPanelForIndicator = panelNamedFolding;
		}
		public void IndicatorErrorsAppend_OnBacktestStarting(string msg, string separator = "; ") {
			if (string.IsNullOrEmpty(msg)) return;
			if (string.IsNullOrEmpty(this.IndicatorErrorsOnBacktestStarting) == false) msg += separator;
			this.IndicatorErrorsOnBacktestStarting += msg;
		}
		public void Initialize(ScriptExecutor executor) {
			// will need this.Executor already set => next line
			//if (this.OwnValuesCalculated.ScaleInterval.Scale == BarScale.Unknown) {
			//    this.OwnValuesCalculated = new DataSeriesTimeBased(this.BarsEffective.ScaleInterval, this.NameWithParameters);
			//}

			if (this.Executor != executor || this.closesProxyEffective_cached == null) {
				this.Executor  = executor;	//this.ClosesProxyEffective will be automatically re-created to match
			}

			this.Decimals = Math.Max(this.BarsEffective.SymbolInfo.PriceDecimals, 1);	// for SBER, constant ATR shows truncated (imprecise) mouseOver value on gutter

			if (this.closesProxyEffective_cached == null) {
				this.BacktestStarting_substituteBarsEffectiveProxy_clearOwnValues_propagatePeriodsToHelperSeries();
			}

			if (this.OwnValuesCalculated.Count == 0) {
				string msg = "NO_NEED_TO_CLEAR";
				//Assembler.PopupException(msg);
			} else {
				if (this.BarsEffective.Count == 0) {
					string msg = "SO_WHY_ClearAllBeforeBacktest()_DIDNT_CLEAR_INDICATORS??? SORRY_FOR_THE_MESS__VALID_ONLY_FOR_MANUAL_REBACKTEST_DURING_LIVE";
					Assembler.PopupException(msg, null, false);
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
		public bool BacktestStarting_validateParameters() {
			//string msg = "MADE_SURE_WE_WILL_INVOKE_BacktestStartingConstructOwnValuesValidateParameters()";
			//Assembler.PopupException(msg, null, false);

			string msig = " Indicator[" + this.NameWithParameters + "].BacktestStarting()";
			this.BacktestStarting_substituteBarsEffectiveProxy_clearOwnValues_propagatePeriodsToHelperSeries();
			
			string parametersAllValidatedErrors = this.ParametersAll_validate();
			this.IndicatorErrorsAppend_OnBacktestStarting(parametersAllValidatedErrors);

			string indicatorSelfValidationErrors = this.InitializeBacktest_beforeStarted_checkErrors();
			this.IndicatorErrorsAppend_OnBacktestStarting(indicatorSelfValidationErrors);
			
			if (string.IsNullOrEmpty(this.IndicatorErrorsOnBacktestStarting) == false) {
				string msig2 = " Indicator[" + this.NameWithParameters + "].BacktestStartingPreCheck()";
				Assembler.PopupException(this.IndicatorErrorsOnBacktestStarting + msig2);
			}
			
			bool backtestCanStart = string.IsNullOrEmpty(parametersAllValidatedErrors)
								 && string.IsNullOrEmpty(indicatorSelfValidationErrors);
			return backtestCanStart;
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
		public	string	FormatForced_DecimalsIndependent		{ get; protected set; }
		public	int		Decimals								{ get; protected set; }
		//v2 end
		public	string	Format									{ get {
				if (string.IsNullOrEmpty(this.FormatForced_DecimalsIndependent) == false) return this.FormatForced_DecimalsIndependent;
				return "N" + this.Decimals;
			} }
		public string FormatValue(double value) {
			return value.ToString(this.Format);
		}
		public string OwnValueForBar_formatted(Bar bar, DataSeriesTimeBased ownValues_orOverridenBandSeries = null) {
			string ret = "";
			if (ownValues_orOverridenBandSeries == null) {
				ownValues_orOverridenBandSeries = this.OwnValuesCalculated;
			}
				
			DateTime barDateTime = bar.DateTimeOpen;
			int barIndex = bar.ParentBarsIndex;
			if (ownValues_orOverridenBandSeries.ContainsDate(barDateTime) == false) {
				ret = "!ex[" + barDateTime.ToString(Assembler.DateTimeFormatIndicatorHasNoValuesFor) + "]";
				return ret;
			}
			double calculated = ownValues_orOverridenBandSeries[barIndex];
			if (double.IsNaN(calculated)) {
				ret = "NaN";
			}
			ret = this.FormatValue(calculated);
			return ret;
		}
		public virtual SortedDictionary<string, string> OwnValuesForTooltipPrice(Bar bar) {
			SortedDictionary<string, string> ret = new SortedDictionary<string, string>();
			ret.Add(this.Name, this.OwnValueForBar_formatted(bar));
			return ret;
		}
		
		string	lastStats_asString							{ get {
			return 	" LastValue[" + this.OwnValuesCalculated.LastValue.ToString(this.Format) + "]"
					+ " LastIndex[" + this.OwnValuesCalculated.LastIndex + "]"
					+ " LastValueAppended[" + this.OwnValuesCalculated.LastValueAppended.ToString(this.Format) + "]"
					+ " LastDateAppended[" + this.OwnValuesCalculated.LastDateAppended + "]"
					+ " Count[" + this.OwnValuesCalculated.Count + "]";
		} }
		public override string ToString() {
			return "#" + this.AbsnoInstance + " " + this.NameWithParameters + " " + this.OwnValuesCalculated.ScaleIntervalCount_asString;
		}
	}
}
