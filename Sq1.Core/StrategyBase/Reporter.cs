using System;
using System.Windows.Forms;

using Sq1.Core.Execution;
using Sq1.Core.Charting;

namespace Sq1.Core.StrategyBase {
	public abstract class Reporter : UserControl {
		public		string				TabText;
		protected	ChartShadow			Chart;
		public		SystemPerformance	SystemPerformance { get; private set; }
		protected	string				FormatPrice { get { 
				string ret = "C";
				if (	this.SystemPerformance == null
					 || this.SystemPerformance.Bars == null
					 || this.SystemPerformance.Bars.SymbolInfo == null) {
					string msig = " " + this.GetType().FullName;
					string msg = "DEVELOPER_OF_[" + msig + "], don't forget to do base.SystemPerformance=performance as first line of overriden BuildOnceAfterFullBlindBacktestFinished() so that Reporter.Format picks up DecimalsPrice";
					//Assembler.PopupException(msg);
					return ret;
				}
				return this.SystemPerformance.Bars.SymbolInfo.PriceFormat;
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
			this.SystemPerformance = performance;
		}

		// dont make it runtime error, bring the error to the earliest stage!
		public abstract void BuildFullOnBacktestFinished();
		//public virtual void BuildOnceAfterFullBlindBacktestFinished(SystemPerformance performance) {
		//	string msg = "DERIVED_REPORTERS_MUST_IMPLEMENT BuildOnceAfterFullBlindBacktestFinished(SystemPerformance)" + this.TabText + "/" + this.GetType();
		//	msg = "; don't forget to do base.SystemPerformance=performance so that Reporter.Format picks up DecimalsPrice";
		//	throw new NotImplementedException(msg);
		//}
		public abstract void BuildIncrementalOnPositionsOpenedClosed_step3of3(ReporterPokeUnit pokeUnit);
		//public virtual void BuildIncrementalAfterPositionsChangedInRealTime(ReporterPokeUnit pokeUnit) {
		//	string msg = "DERIVED_REPORTERS_MUST_IMPLEMENT BuildIncrementalAfterPositionsChangedInRealTime(ReporterPokeUnit)" + this.TabText + "/" + this.GetType();
		//	throw new NotImplementedException(msg);
		//}
		public abstract void BuildIncrementalUpdateOpenPositionsDueToStreamingNewQuote_step2of3(ReporterPokeUnit pokeUnit);
		//public virtual void OpenPositionsUpdatedDueToStreamingNewQuote(ReporterPokeUnit pokeUnit) {
		//	string msg = "DERIVED_REPORTERS_MUST_IMPLEMENT BuildIncrementalAfterPositionsChangedInRealTime(ReporterPokeUnit)" + this.TabText + "/" + this.GetType();
		//	throw new NotImplementedException(msg);
		//}
		public abstract void BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3(ReporterPokeUnit pokeUnit);
		public virtual object CreateSnapshotToStoreInScriptContext() {
			return null;
		}
		public void RaiseContextScriptChangedContainerShouldSerialize() {
			this.Chart.RaiseContextScriptChangedContainerShouldSerialize();
		}
	}
}
