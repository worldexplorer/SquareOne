using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Indicators {
	public partial class Indicator {

		public virtual void BacktestContextRestore_backToOriginalBarsEffectiveProxy_continueToLive_noRecalculate() {
		}
		public virtual void BacktestStarting_substituteBarsEffectiveProxy_clearOwnValues_propagatePeriodsToHelperSeries() {
			this.ParametersAsStringShort_forceRecalculate = true;
			// muting INDICATOR_SWITCHED_BARS
			this.barsEffective_cached = this.Executor.Bars;
			this.closesProxyEffective_cached = new DataSeriesProxyBars(this.BarsEffective, this.DataSeriesProxyFor);
			this.OwnValuesCalculated.Clear();
		}
		
		public virtual string ParametersAll_validate() {
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
		public virtual string InitializeBacktest_beforeStarted_checkErrors() {
			return null;
		}
		public virtual double CalculateOwnValue_onNewStaticBarFormed_invokedAtEachBarNoExceptions_NoPeriodWaiting(Bar newStaticBar) {
			if (this.OwnValuesCalculated.ContainsDate(newStaticBar.DateTimeOpen)) {
				string msg = "ALREADY_HAVE_VALUE[" + this.OwnValuesCalculated[newStaticBar.DateTimeOpen] + "] on[" + newStaticBar.DateTimeOpen + "]"
					+ " CLEAR_INDICATOR DONT_INVOKE_ME_TWICE";
				Assembler.PopupException(msg);
			}
			return double.NaN;
		}
		public virtual double CalculateOwnValue_onNewStreamingQuote_invokedAtEachQuoteNoExceptions_NoPeriodWaiting(Quote newStreamingQuote) {
			return double.NaN;
			//PROHIBITED_TO_CALCULATE_EACH_QUOTE_SLOW return this.CalculateOwnValueOnNewStaticBarFormed(newStreamingQuote.ParentStreamingBar);
		}
		//public abstract double CalculateOwnValueOnNewStaticBarFormed_invokedAtEachBarNoExceptions_NoPeriodWaiting(Bar newStaticBar);
		//public abstract double CalculateOwnValueOnNewStreamingQuote_invokedAtEachQuoteNoExceptions_NoPeriodWaiting(Quote newStreamingQuote);


		bool checkThrow_canRunCalculation_onBarOrQuote(Bar newStaticBar = null, Quote quote = null, bool popup = true) {
			bool ret = false;
			if (this.OwnValuesCalculated == null) {
				if (popup) {
					string msg = "HAPPENS_DURING_REBACKTEST_AFTER_ABORTION this.OwnValuesCalculated=null " + this.ToString();
					Assembler.PopupException(msg);
				}
				return ret;
			}
			if (string.IsNullOrEmpty(this.IndicatorErrorsOnBacktestStarting) == false) {
				if (popup) {
					string msg = "I_REFUSE_TO_CALCULATE_DUE_TO_PREVIOUS_ERRORS Indicator.OnNewStaticBarFormed(" + newStaticBar + ")" + this.ToString();
					Assembler.PopupException(msg);
				}
				return ret;
			}
			if (this.Executor == null) {
				if (popup) {
					string msg = "NO_EXECUTOR_FOR_INDICATOR this.Executor=null" + this.ToString();
					Assembler.PopupException(msg, null, false);
				}
				return ret;
			}
			if (this.ClosesProxyEffective == null) {
				if (popup) {
					string msg = "NO_BARS_FOR_INDICATOR this.ClosesProxyEffective=null" + this.ToString();
					Assembler.PopupException(msg);
				}
				return ret;
			}
			//if (this.ClosesProxyEffective.Count - 1 < this.FirstValidBarIndex) {
			//	//if (popupException) {
			//	//	string msg = "DONT_INVOKE_ME__MAKE_THIS_CHECK_UPSTACK"
			//	//		+ " base.ClosesProxyEffective.Count-1[" + (this.ClosesProxyEffective.Count - 1)
			//	//		+ "] < this.FirstValidBarIndex[" + this.FirstValidBarIndex + "]"
			//	//		+ this.ToString();
			//	//	Assembler.PopupException(msg);
			//	//}
			//	//if (newStaticBar.ParentBarsIndex - 1 < this.FirstValidBarIndex) {
			//	//	int barsWaitToFirstIndicatorValidIndex = this.FirstValidBarIndex - newStaticBar.ParentBarsIndex;
			//	//	string msg = "barsWaitToFirstIndicatorValidIndex[" + barsWaitToFirstIndicatorValidIndex + "] newStaticBar.ParentBarsIndex - 1 < this.FirstValidBarIndex";
			//	//	//Assembler.PopupException(msg, null, false);
			//	//	return;
			//	//}
			//	return ret;
			//}
			ret = true;
			return ret;
		}
		public virtual void OnBarStaticLastFormed_whileStreamingBar_withOneQuote_alreadyAppended(Bar newStaticBar) {
			string msig = " //OnNewStaticBarFormed(" + newStaticBar.ToString() + ")";
			bool canRunCalculation = this.checkThrow_canRunCalculation_onBarOrQuote(newStaticBar, null);
			if (canRunCalculation == false) {
				//this.OwnValuesCalculated.Append(newStaticBar.DateTimeOpen, double.NegativeInfinity);
				this.OwnValuesCalculated.Append(newStaticBar.DateTimeOpen, double.NaN);
				return;
			}
			double derivedCalculated = this.CalculateOwnValue_onNewStaticBarFormed_invokedAtEachBarNoExceptions_NoPeriodWaiting(newStaticBar);
			if (newStaticBar.ParentBarsIndex >= this.FirstValidBarIndex) {
				string msg3 = "MUST_NOT_BE_NaN_ANYMORE";
			}
			if (double.IsNaN(derivedCalculated) == false) {
				string msg2 = "newStaticBar.ParentBarsIndex[" + newStaticBar.ParentBarsIndex + "] FirstValidBarIndex[" + this.FirstValidBarIndex + "]";
			}

			int barsAheadOfIndicator = newStaticBar.ParentBarsIndex - this.OwnValuesCalculated.LastIndex;
			msig = " barsAheadOfIndicator[" + barsAheadOfIndicator + "]" + this.lastStats_asString + msig;

			string msg;
			if (barsAheadOfIndicator == 0) {
				msg = "PREV_QUOTE_ADDED_LAST_VALUE_PREVENTIVELY__I_WILL_REPLACE_STREAMING_VALUE";
				if (this.OwnValuesCalculated.LastDateAppended != newStaticBar.DateTimeOpen) {
					msg = "UNSYNC#1_FIX_ME " + msg;
					Assembler.PopupException(msg + msig, null, false);
					return;
				}
				this.OwnValuesCalculated.LastValue = derivedCalculated;
			} else if (barsAheadOfIndicator == 1) {
				msg = "PREV_QUOTE_DIDNT_ADD_STREAMING_OWN_VALUE_PREVENTIVELY_ADDING_NOW";
				if (this.OwnValuesCalculated.LastDateAppended == newStaticBar.DateTimeOpen) {
					msg = "UNSYNC#2_FIX_ME " + msg;
					Assembler.PopupException(msg + msig, null, false);
					return;
				}
				this.OwnValuesCalculated.Append(newStaticBar.DateTimeOpen, derivedCalculated);
			} else {
				msg = "INDICATOR_CALCULATE_OWN_VALUE_WASNT_CALLED_WITHIN_LAST_BARS barsAheadOfIndicator[" + barsAheadOfIndicator + "]";
				Assembler.PopupException(msg + msig, null, false);
				return;
			}
		}
		public virtual void OnNewQuote(Quote newStreamingQuote) {
			if (this.AllowsOnNewQuote == false) return;

			#if VERBOSE_STRINGS_SLOW
			string msig = " //OnNewQuote(" + newStreamingQuote.ToString() + ")";
			#else
			string msig = " //OnNewQuote()";
			#endif
			
			bool canRunCalculation = this.checkThrow_canRunCalculation_onBarOrQuote(null, newStreamingQuote);
			if (canRunCalculation == false) return;
			double derivedCalculated = this.CalculateOwnValue_onNewStreamingQuote_invokedAtEachQuoteNoExceptions_NoPeriodWaiting(newStreamingQuote);

			if (newStreamingQuote.ParentBarStreaming == null) {
				if (newStreamingQuote.AbsnoPerSymbol == 0) {
					string msg4 = "FIRST_QUOTE_OF_LIVESIM_HAS_NO_PARENT_STREAMING";
				} else {
					string msg2 = "DONT_FEED_ME_WITH_QUOTE_UNATTACHED";
					Assembler.PopupException(msg2 + msig, null, false);
				}
				return;
			}

			if (newStreamingQuote.ParentBarStreaming.ParentBars == null) {
				string msg2 = "DONT_FEED_ME_WITH_BAR_UNATTACHED";
				Assembler.PopupException(msg2 + msig, null, false);
				return;
			}

			int barsAheadOfIndicator = newStreamingQuote.ParentBarStreaming.ParentBarsIndex - this.OwnValuesCalculated.LastIndex;
			msig = " barsAheadOfIndicator[" + barsAheadOfIndicator + "]" + this.lastStats_asString + msig;
			if (barsAheadOfIndicator == 0) {
				string msg = "UPDATING_STREAMING_VALUE";
				if (this.OwnValuesCalculated.LastDateAppended != newStreamingQuote.ParentBarStreaming.DateTimeOpen) {
					msg = "UNSYNC#1 " + msg;
					Assembler.PopupException(msg + msig, null, false);
					return;
				}
				if (double.IsNaN(this.OwnValuesCalculated.LastValue) && double.IsNaN(derivedCalculated)) {
					msg = "WONT_UPDATE_NAN " + msg;
					Assembler.PopupException(msg + msig, null, false);
					return;
				}
				this.OwnValuesCalculated.LastValue = derivedCalculated;
			} else if (barsAheadOfIndicator == 1) {
				string msg = "PREVENTIVE_ADD";
				if (this.OwnValuesCalculated.LastDateAppended >= newStreamingQuote.ParentBarStreaming.DateTimeOpen) {
					msg = "UNSYNC#2_FIX_ME " + msg;
					Assembler.PopupException(msg + msig, null, false);
					return;
				}
				this.OwnValuesCalculated.Append(newStreamingQuote.ParentBarStreaming.DateTimeOpen, derivedCalculated);
			} else {
				string msg = "INDICATOR_CALCULATE_OWN_VALUE_WASNT_CALLED_WITHIN_LAST_BARS barsAheadOfIndicator[" + barsAheadOfIndicator + "]";
				Assembler.PopupException(msg + msig, null, false);
				return;
			}
		}
	
	}
}
