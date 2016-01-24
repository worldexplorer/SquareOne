using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;

using Sq1.Core.Support;
using Sq1.Core.Streaming;

using Sq1.Adapters.Quik.Streaming.Dde;

namespace Sq1.Adapters.Quik.Streaming.Monitor {
	public partial class QuikStreamingMonitorDomUserControl : UserControlResizeable {
		QuikStreaming	quikStreaming;
		DdeTableDepth	tableLevel2;
		Stopwatch		stopwatchRarifyingUIupdates;

		public QuikStreamingMonitorDomUserControl() {
			InitializeComponent();
			this.olvDomCustomize();
			this.stopwatchRarifyingUIupdates = new Stopwatch();
			this.stopwatchRarifyingUIupdates.Start();
		}
		void layoutUserControlResizeable() {
			base.UserControlInner.Controls.Add(this.olvcLevelTwo);
			this.olvcLevelTwo.Dock = DockStyle.Fill;
		}
		internal void Initialize(QuikStreaming quikStreamingPassed, DdeTableDepth tableLevel2Passed) {
			this.quikStreaming = quikStreamingPassed;
			this.tableLevel2 = tableLevel2Passed;
			this.tableLevel2.UserControlMonitoringMe = this;

			if (this.tableLevel2.SymbolInfo.Level2ShowCumulativesInsteadOfLots) {
				this.olvAskCumulative	.IsVisible = true;
				this.olvAsk				.IsVisible = false;
				this.olvBid				.IsVisible = false;
				this.olvBidCumulative	.IsVisible = true;
			} else {
				this.olvAskCumulative	.IsVisible = false;
				this.olvAsk				.IsVisible = true;
				this.olvBid				.IsVisible = true;
				this.olvBidCumulative	.IsVisible = false;
			}
			this.olvcLevelTwo.RebuildColumns();

			this.layoutUserControlResizeable();
			this.PopulateLevel2ToTitle(true);
		}
		public void PopulateLevel2ToTitle(bool ignoreTimer = false) {
			if (this.olvcLevelTwo.IsDisposed) return;
			if (base.IsDisposed) return;

			if (ignoreTimer == false) {
				// WHAT_IF_BEFORE_SWITCHING_TO_GUI_THREAD?
				if (this.stopwatchRarifyingUIupdates.ElapsedMilliseconds < this.quikStreaming.DdeMonitorRefreshRate) return;
			}

			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.PopulateLevel2ToTitle(ignoreTimer); });
				return;
			}
			// I paid the price of switching to GuiThread, but I don' have to worry if I already stopwatch.Restart()ed
			//if (ignoreTimer == false) {
			//	if (this.stopwatchRarifyingUIupdates.ElapsedMilliseconds < this.quikStreaming.DdeMonitorRefreshRate) return;
			//RESTART_ONLY_IN_ONE_PLACE_OTHERWIZE_THE_OTHER_NEVER_DOES_ANYTHING this.stopwatchRarifyingUIupdates.Restart();
			//}

			this.lblDomTitle.Text = this.tableLevel2.ToString();
		}

		internal void PopulateLevel2ToDomControl(LevelTwoOlv levelTwoOLV_gotFromDde_pushTo_domResizeableUserControl) {
			if (this.olvcLevelTwo.IsDisposed) return;
			if (base.IsDisposed) return;

			// WHAT_IF_BEFORE_SWITCHING_TO_GUI_THREAD?
			if (this.stopwatchRarifyingUIupdates.ElapsedMilliseconds < this.quikStreaming.DdeMonitorRefreshRate) return;
			//this.stopwatchRarifyingUIupdates.Restart();

			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.PopulateLevel2ToDomControl(levelTwoOLV_gotFromDde_pushTo_domResizeableUserControl); });
				return;
			}
			// I paid the price of switching to GuiThread, but I don' have to worry if I already stopwatch.Restart()ed
			//if (this.stopwatchRarifyingUIupdates.ElapsedMilliseconds < this.quikStreaming.DdeMonitorRefreshRate) return;
			this.stopwatchRarifyingUIupdates.Restart();

			List<LevelTwoOlvEachLine> level2rows = levelTwoOLV_gotFromDde_pushTo_domResizeableUserControl.FrozenSortedFlattened_priceLevelsInserted;
			this.olvcLevelTwo.SetObjects(level2rows);
		}
	}
}
