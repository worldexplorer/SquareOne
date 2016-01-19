using System;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core.Support;

using Sq1.Adapters.Quik.Streaming.Dde;

namespace Sq1.Adapters.Quik.Streaming.Monitor {
	public partial class QuikStreamingMonitorDomUserControl : UserControlResizeable {
		DdeTableDepth tableLevel2;

		public QuikStreamingMonitorDomUserControl() {
			InitializeComponent();
		}
		void layoutUserControlResizeable() {
			base.UserControlInner.Controls.Add(this.olvcDom);
			this.olvcDom.Dock = DockStyle.Fill;
		}
		internal void Initialize(DdeTableDepth tableLevel2Passed) {
			this.tableLevel2 = tableLevel2Passed;
			this.tableLevel2.WhereIamMonitored = this;
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
	}
}
