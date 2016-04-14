using System;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Charting {
	public partial class ChartShadow {
		public event EventHandler<EventArgs>	OnChartSettingsChanged_containerShouldSerialize_ChartFormDataSnapshot;
		public event EventHandler<EventArgs>	OnContextScriptChanged_containerShouldSerialize;

		public event EventHandler<BarEventArgs>	OnBarStreamingUpdatedMerged_chartFormPrintsQuoteTimestamp;

		public event EventHandler<EventArgs>	OnPumpPaused;
		public event EventHandler<EventArgs>	OnPumpUnPaused;

		protected void RaiseOnBarStreamingUpdatedMerged_chartFormPrintsQuoteTimestamp(BarEventArgs e) {
			if (this.OnBarStreamingUpdatedMerged_chartFormPrintsQuoteTimestamp == null) return;
			try {
				this.OnBarStreamingUpdatedMerged_chartFormPrintsQuoteTimestamp(this, e);
			} catch (Exception ex) {
				string msg = "RaiseBarStreamingUpdatedMerged(bar[" + e.Bar + "])";
				Assembler.PopupException(msg, ex, false);
			}
		}
		
		public void RaiseOnChartSettingsChanged_containerShouldSerialize_ChartFormDataSnapshot_copyMultiSplitterDictionaries() {
			if (this.OnChartSettingsChanged_containerShouldSerialize_ChartFormDataSnapshot == null) return;
			try {
				this.OnChartSettingsChanged_containerShouldSerialize_ChartFormDataSnapshot(this, null);
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnChartSettingsChanged_containerShouldSerialize_ChartFormDataSnapshot_copyMultiSplitterDictionaries()", ex);
			}
		}
		public void RaiseOnContextScriptChanged_containerShouldSerialize() {
			if (this.OnContextScriptChanged_containerShouldSerialize == null) return;
			try {
				this.OnContextScriptChanged_containerShouldSerialize(this, null);
			} catch (Exception ex) {
				Assembler.PopupException("RaiseContextScriptChangedContainerShouldSerialize()", ex);
			}
		}

		void raiseOnPumpPaused() {
			if (this.OnPumpPaused == null) return;
			try {
				this.OnPumpPaused(this, null);
			} catch (Exception ex) {
				string msg = "RaiseOnPumpPaused()";
				Assembler.PopupException(msg, ex, false);
			}
		}

		void raiseOnPumpUnPaused() {
			if (this.OnPumpUnPaused == null) return;
			try {
				this.OnPumpUnPaused(this, null);
			} catch (Exception ex) {
				string msg = "RaiseOnPumpUnPaused()";
				Assembler.PopupException(msg, ex, false);
			}
		}

	}
}
