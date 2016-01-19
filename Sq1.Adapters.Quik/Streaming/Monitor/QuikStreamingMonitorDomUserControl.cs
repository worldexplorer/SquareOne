using System;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core.Support;

using Sq1.Adapters.Quik.Streaming.Dde;
using Sq1.Core.Streaming;

namespace Sq1.Adapters.Quik.Streaming.Monitor {
	public partial class QuikStreamingMonitorDomUserControl : UserControlResizeable {
		DdeTableDepth tableLevel2;

		public QuikStreamingMonitorDomUserControl() {
			InitializeComponent();
			this.olvDomCustomize();
		}
		void layoutUserControlResizeable() {
			base.UserControlInner.Controls.Add(this.olvcLevelTwo);
			this.olvcLevelTwo.Dock = DockStyle.Fill;
		}
		internal void Initialize(DdeTableDepth tableLevel2Passed) {
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
			this.lblDomTitle.Text = this.tableLevel2.ToString();
		}

		internal void PopulateLevel2ToDomControl(LevelTwoOlv levelTwoOLV_gotFromDde_pushTo_domResizeableUserControl) {
			if (this.olvcLevelTwo.IsDisposed) return;
			if (base.IsDisposed) return;

			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.PopulateLevel2ToDomControl(levelTwoOLV_gotFromDde_pushTo_domResizeableUserControl); });
				return;
			}

			this.olvcLevelTwo.SetObjects(levelTwoOLV_gotFromDde_pushTo_domResizeableUserControl.FreezeSortAndFlatten());
		}
	}
}
