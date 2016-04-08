using System;
using System.Windows.Forms;

using Sq1.Core.Execution;
using Sq1.Core.Charting;

namespace Sq1.Core.StrategyBase {
	public /*abstract*/ class Reporter : UserControl {
		public		string				TabText;
		protected	ChartShadow			Chart;
		public		SystemPerformance	SystemPerformance { get; private set; }

		protected	string				FormatPrice { get { 
				string ret = "C";
				if (	this.SystemPerformance == null
					 || this.SystemPerformance.Bars == null
					 || this.SystemPerformance.Bars.SymbolInfo == null) {
					string msig = " " + this.GetType().FullName;
					string msg = "DEVELOPER_OF_[" + msig + "], don't forget to do base.SystemPerformance=performance as first line of overriden BuildOnceAfterFullBlindBacktestFinished() so that Reporter.FormatPrice picks up DecimalsPrice";
					//Assembler.PopupException(msg);
					return ret;
				}
				return this.SystemPerformance.Bars.SymbolInfo.PriceFormat;
			} }
		protected	string				FormatVolume { get {
				string ret = "C";
				if (	this.SystemPerformance == null
					 || this.SystemPerformance.Bars == null
					 || this.SystemPerformance.Bars.SymbolInfo == null) {
					string msig = " " + this.GetType().FullName;
					string msg = "DEVELOPER_OF_[" + msig + "], don't forget to do base.SystemPerformance=performance as first line of overriden BuildOnceAfterFullBlindBacktestFinished() so that Reporter.VolumeFormat picks up DecimalsVolume";
					//Assembler.PopupException(msg);
					return ret;
				}
				return this.SystemPerformance.Bars.SymbolInfo.VolumeFormat;
		} }

		public Reporter() {
			this.TabText = "UNKNOWN_REPORTER";
		}
		public Reporter(ChartShadow chart) : this() {
			this.Initialize(chart, null, null);
		}
		public virtual void Initialize(ChartShadow chart, object reportersOwnDataSnapshotInOut, SystemPerformance performance = null) {
			this.Chart = chart;

			if (this.SystemPerformance != null && this.SystemPerformance == performance) {
				string msg = "FYI_MOVED_TO__ChartFormManager.ReportersFormsManager.ReporterActivateShowRegisterMniTick()";
				Assembler.PopupException(msg);
			}

			if (this.SystemPerformance != null && this.SystemPerformance.Bars != null) {
				string msg = "REPORTER_WILL_CHANGE_BARS_AFTER_CHART_LOADS_BACKTESTS_ANOTHER_STRATEGY?";
				Assembler.PopupException(msg);
				this.SystemPerformance.Bars.SymbolInfo.PriceDecimalsChanged -= new EventHandler<EventArgs>(SymbolInfo_PriceDecimalsChanged);
			}
			this.SystemPerformance = performance;
			this.SystemPerformance.Bars.SymbolInfo.PriceDecimalsChanged += new EventHandler<EventArgs>(SymbolInfo_PriceDecimalsChanged);
		}

		protected virtual void SymbolInfo_PriceDecimalsChanged(object sender, EventArgs e) {
		}

		// dont make it runtime error, bring the error to the earliest stage!
		//The designer must create an instance of type 'Sq1.Core.StrategyBase.Reporter' but it cannot because the type is declared as abstract. 
		//public abstract void BuildFullOnBacktestFinished();
		public virtual void BuildFull_onBacktestFinished() {
			string msg = "DERIVED_REPORTERS_MUST_IMPLEMENT BuildFullOnBacktestFinished()" + this.TabText + "/" + this.GetType();
			msg = "; don't forget to do base.SystemPerformance=performance so that Reporter.Format picks up DecimalsPrice";
			throw new NotImplementedException(msg);
		}
		//public abstract void BuildIncrementalOnPositionsOpenedClosed_step3of3(ReporterPokeUnit pokeUnit);
		public virtual void BuildIncremental_onPositionsOpenedClosed_step3of3(ReporterPokeUnit pokeUnit) {
			string msg = "DERIVED_REPORTERS_MUST_IMPLEMENT BuildIncremental_onPositionsOpenedClosed_step3of3(ReporterPokeUnit)" + this.TabText + "/" + this.GetType();
			throw new NotImplementedException(msg);
		}
		//public abstract void BuildIncrementalUpdateOpenPositionsDueToStreamingNewQuote_step2of3(ReporterPokeUnit pokeUnit);
		public virtual void BuildIncremental_updateOpenPositions_dueToStreamingNewQuote_step2of3(ReporterPokeUnit pokeUnit) {
			string msg = "DERIVED_REPORTERS_MUST_IMPLEMENT BuildIncremental_updateOpenPositions_dueToStreamingNewQuote_step2of3(ReporterPokeUnit)" + this.TabText + "/" + this.GetType();
			throw new NotImplementedException(msg);
		}
		//public abstract void BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3(ReporterPokeUnit pokeUnit);
		public virtual void BuildIncremental_onBrokerFilled_alertsOpening_forPositions_step1of3(ReporterPokeUnit pokeUnit) {
			string msg = "DERIVED_REPORTERS_MUST_IMPLEMENT BuildIncremental_onBrokerFilled_alertsOpening_forPositions_step1of3(ReporterPokeUnit)" + this.TabText + "/" + this.GetType();
			throw new NotImplementedException(msg);
		}

		public virtual object CreateSnapshot_toStore_inScriptContext() {
			return null;
		}

		public virtual void Stash_windowTextSuffix_inBaseTabText_usefulToUpdateAutohiddenStats_withoutRebuildingFullReport_OLVisSlow() { }
		public virtual void RebuildingFullReport_forced_onLivesimPaused() { }

		public void RaiseContextScriptChangedContainerShouldSerialize() {
			this.Chart.RaiseOnContextScriptChanged_containerShouldSerialize();
		}
	}
}
