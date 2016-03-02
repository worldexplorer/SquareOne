using System;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

using Sq1.Core;

using Sq1.Widgets;
using Sq1.Widgets.DataSourceEditor;

namespace Sq1.Adapters.Quik.Streaming {
	public partial class QuikStreamingEditorControl  {
		void txtDdeServerPrefix_TextChanged(object sender, EventArgs e) {
			this.lblMinus4.Text = this.txtDdeServerPrefix.Text + "-SYMBOL-";
			this.lblMinus2.Text = this.txtDdeServerPrefix.Text + "-";
		}
		void lnkDdeMonitor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			if (this.quikStreamingAdapter.MonitorForm.IsShown) {
				this.quikStreamingAdapter.MonitorForm.Activate();
				//if Activate() won't be enough: this.quikStreamingAdapter.MonitorForm_localSingleton.BringToFront();
			}
			this.quikStreamingAdapter.MonitorForm.PopulateWindowTitle_dataSourceName_market_quotesTopic();
			//COUNTRY_STARTING_WITH_U DockContent parentDocked = base.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent as DockContent;
			DataSourceEditorControl imEditorControl = base.DataSourceEditor as DataSourceEditorControl;
			DockContentImproved imEditorForm = null;
			if (imEditorControl != null) imEditorForm = imEditorControl.Parent as DockContentImproved;
			if (imEditorForm != null && imEditorForm.DockPanel != null) {
				//dataSourceEditorForm.ShowAsSiblingTabOnTopOfMe(this.quikStreamingAdapter.MonitorForm);
				//this.quikStreamingAdapter.MonitorForm.ShowOnTopOf(imEditorForm);
				this.quikStreamingAdapter.MonitorForm.ShowStackedHinted(imEditorForm.DockPanel);
			//}
			//if (myParent != null && myParent.MainFormDockPanel_forDdeMonitor != null) {
			//	this.quikStreamingAdapter.MonitorForm.Show(myParent.MainFormDockPanel_forDdeMonitor, dataSourceEditorForm.DockState);
			} else {
				//string msg = "WILL_POPUP_AS_UNDOCKABLE_WINDOWS_FORM Assembler.InstanceInitialized.MainFormDockPanel_forDdeMonitor=NULL";
				string msg = "SHOULD_NEVER_HAPPEN_THAT_CONTROL_CAN_NOT_CAST_ITS_PARENT";
				Assembler.PopupException(msg);
				this.quikStreamingAdapter.MonitorForm.Show();
			}
		}
		void cbxStartDde_CheckedChanged(object sender, EventArgs e) {
			if (this.dontStartStopDdeServer_imSyncingDdeStarted_intoTheBtnText_only) {
				this.propagateStreamingConnected_intoBtnStateText();
				return;
			}
			if (this.cbxStartDde.Checked) {
				//v1 this.quikStreamingAdapter.DdeServerRegister();
				this.quikStreamingAdapter.UpstreamConnect();
			} else {
				this.quikStreamingAdapter.DdeServerUnregister();
			}
		}
		protected override void StreamingAdapter_OnConnectionStateChanged(object sender, EventArgs e) {
			this.propagateStreamingConnected_intoBtnStateText();
		}
   }
}