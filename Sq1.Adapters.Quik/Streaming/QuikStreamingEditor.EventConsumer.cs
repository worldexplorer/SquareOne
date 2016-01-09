using System;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

using Sq1.Core;
using Sq1.Widgets.DataSourceEditor;

namespace Sq1.Adapters.Quik.Streaming {
	public partial class QuikStreamingEditor  {
		void txtDdeServerPrefix_TextChanged(object sender, EventArgs e) {
			this.lblMinus4.Text = this.txtDdeServerPrefix.Text + "-SYMBOL-";
			this.lblMinus2.Text = this.txtDdeServerPrefix.Text + "-";
		}
		void lnkDdeMonitor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			this.quikStreamingAdapter.MonitorForm.PopulateWindowTitle_dataSourceName_market_quotesTopic();
			//COUNTRY_STARTING_WITH_U DockContent parentDocked = base.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent as DockContent;
			DataSourceEditorControl myParent = base.DataSourceEditor as DataSourceEditorControl;
			if (myParent != null && myParent.MainFormDockPanel_forDdeMonitor != null) {
				this.quikStreamingAdapter.MonitorForm.Show(myParent.MainFormDockPanel_forDdeMonitor);
			} else {
				string msg = "WILL_POPUP_AS_UNDOCKABLE_WINDOWS_FORM Assembler.InstanceInitialized.MainFormDockPanel_forDdeMonitor=NULL";
				Assembler.PopupException(msg);
				this.quikStreamingAdapter.MonitorForm.Show();
			}
		}
		void cbxStartDde_CheckedChanged(object sender, EventArgs e) {
			if (this.cbxStartDde.Checked) {
				this.quikStreamingAdapter.DdeServerStart();
				this.cbxStartDde.Text = "Stop DDE Server (now started)";
			} else {
				this.quikStreamingAdapter.DdeServerStop();
				this.cbxStartDde.Text = "Start DDE Server (now stopped)";
			}
		}
   }
}