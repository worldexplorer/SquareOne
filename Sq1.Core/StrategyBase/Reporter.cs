using System;
using System.Windows.Forms;

using Sq1.Core.Execution;
using Sq1.Core.Charting;

namespace Sq1.Core.StrategyBase {
	public class Reporter : UserControl {
		public string TabText;
		protected ChartShadow Chart;

		protected SystemPerformance SystemPerformance;
		protected string FormatPrice { get { 
				string ret = "C";
				if (	this.SystemPerformance == null
					 || this.SystemPerformance.Bars == null
					 || this.SystemPerformance.Bars.SymbolInfo == null) {
					string msig = " " + this.GetType().FullName;
					string msg = "DEVELOPER_OF_[" + msig + "], don't forget to do base.SystemPerformance=performance as first line of overriden BuildOnceAfterFullBlindBacktestFinished() so that Reporter.Format picks up DecimalsPrice";
					Assembler.PopupException(msg);
					return ret;
				}
				return this.SystemPerformance.Bars.SymbolInfo.FormatPrice;
			} }


		public Reporter() {
			this.TabText = "UNKNOWN_REPORTER";
		}
		public Reporter(ChartShadow chart) : this() {
			this.Initialize(chart, null);
		}
		public virtual void Initialize(ChartShadow chart, object reportersOwnDataSnapshotInOut) {
			this.Chart = chart;
		}

		public virtual void BuildOnceAfterFullBlindBacktestFinished(SystemPerformance performance) {
			string msg = "DERIVED_REPORTERS_MUST_IMPLEMENT BuildOnceAfterFullBlindBacktestFinished(SystemPerformance)" + this.TabText + "/" + this.GetType();
			msg = "; don't forget to do base.SystemPerformance=performance so that Reporter.Format picks up DecimalsPrice";
			throw new NotImplementedException(msg);
		}
		public virtual void BuildIncrementalAfterPositionsChangedInRealTime(ReporterPokeUnit pokeUnit) {
			string msg = "DERIVED_REPORTERS_MUST_IMPLEMENT BuildIncrementalAfterPositionsChangedInRealTime(ReporterPokeUnit)" + this.TabText + "/" + this.GetType();
			throw new NotImplementedException(msg);
		}
		public virtual object CreateSnapshotToStoreInScriptContext() {
			return null;
		}
		public void RaiseContextScriptChangedContainerShouldSerialize() {
			this.Chart.RaiseContextScriptChangedContainerShouldSerialize();
		}
	}
}
