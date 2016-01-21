using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

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
			this.layoutUserControlResizeable();
			this.PopulateLevel2ToTitle();
		}
		public void PopulateLevel2ToTitle() {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.PopulateLevel2ToTitle(); });
				return;
			}
			// I paid the price of switching to GuiThread, but I don' have to worry if I already stopwatch.Restart()ed
			if (this.stopwatchRarifyingUIupdates.ElapsedMilliseconds < this.quikStreaming.DdeMonitorRefreshRate) return;
			this.stopwatchRarifyingUIupdates.Restart();

			this.lblDomTitle.Text = this.tableLevel2.ToString();
		}

		internal void PopulateLevel2ToDomControl(LevelTwoOlv levelTwoOLV_gotFromDde_pushTo_domResizeableUserControl) {
			if (this.olvcLevelTwo.IsDisposed) return;
			if (base.IsDisposed) return;

			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.PopulateLevel2ToDomControl(levelTwoOLV_gotFromDde_pushTo_domResizeableUserControl); });
				return;
			}
			// I paid the price of switching to GuiThread, but I don' have to worry if I already stopwatch.Restart()ed
			if (this.stopwatchRarifyingUIupdates.ElapsedMilliseconds < this.quikStreaming.DdeMonitorRefreshRate) return;
			this.stopwatchRarifyingUIupdates.Restart();

			this.olvcLevelTwo.SetObjects(levelTwoOLV_gotFromDde_pushTo_domResizeableUserControl.FreezeSortAndFlatten());
		}
	}
}
