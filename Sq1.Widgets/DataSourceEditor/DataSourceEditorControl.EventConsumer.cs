using System;
using System.Windows.Forms;
using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.DataTypes;
using Sq1.Core.Static;
using Sq1.Core.Streaming;
using WeifenLuo.WinFormsUI.Docking;

namespace Sq1.Widgets.DataSourceEditor {
	public partial class DataSourceEditorControl {
		void lvStaticProviders_SelectedIndexChanged(object sender, EventArgs e) {
			if (this.lvStaticProviders.SelectedItems.Count == 0) {
				this.btnNext.Enabled = false;
				//this.btnSave.Enabled = false;
				return;
			}
			ListViewItem lvi = this.lvStaticProviders.SelectedItems[0];
			StaticProvider staticPrevious = ds.StaticProvider;
			ds.StaticProvider = (StaticProvider)lvi.Tag;
			this.btnNext.Enabled = true;
			this.panel_0 = this.pnlIntro;
			this.pnlIntro.BringToFront();
			//UserControl userControl = ds.StaticProvider.WizardFirstPage();
			UserControl userControl = null;
			if (userControl == null) {
				this.btnSave.Enabled = true;
				this.btnNext.Enabled = false;
				this.btnPrevious.Enabled = false;
			}
			if (staticPrevious != null) {
				if (this.txtDataSourceName.Text == staticPrevious.PreferredDataSourceName) {
					this.txtDataSourceName.Text = "";
				}
			}
			if (String.IsNullOrEmpty(this.txtDataSourceName.Text)) {
				this.txtDataSourceName.Text = ds.StaticProvider.PreferredDataSourceName;
				this.txtDataSourceName.SelectAll();
				this.txtDataSourceName.Focus();
			}
			HighlightStreamingByName(ds.StaticProvider.PreferredStreamingProviderTypeName);
			HighlightBrokerByName(ds.StaticProvider.PreferredBrokerProviderTypeName);
			if (ds.StaticProvider.UserAllowedToModifySymbols == false) {
				if (txtSymbols.Text == symbolsDefault) txtSymbols.Clear();
				txtSymbols.Enabled = false;
			}
		}
		void lvStreamingProviders_SelectedIndexChanged(object sender, EventArgs e) {
			if (this.lvStreamingProviders.SelectedItems.Count == 0) {
				//this.btnNext.Enabled = false;
				//this.btnFinished.Enabled = false;
				return;
			}
			ListViewItem lvi = this.lvStreamingProviders.SelectedItems[0];
			if (lvi.Tag == null) {
				this.pnlStreamingEditor.Controls.Clear();
				ds.StreamingProvider = null;
				return;
			}
			ds.StreamingProvider = (StreamingProvider)lvi.Tag;
			ds.StreamingProvider.EditorInstance.PushStreamingProviderSettingsToEditor();
			ds.StreamingProvider.EditorInstance.Dock = DockStyle.Fill;
			//this.btnNext.Enabled = true;
			//this.btnFinished.Enabled = false;
			this.pnlStreamingEditor.Controls.Clear();
			this.pnlStreamingEditor.Controls.Add(ds.StreamingProvider.EditorInstance);
			this.grpStreaming.Text = ds.StreamingProvider.Name + " Settings";
		}
		void lvBrokerProviders_SelectedIndexChanged(object sender, EventArgs e) {
			if (this.lvBrokerProviders.SelectedItems.Count == 0) {
				//this.btnNext.Enabled = false;
				//this.btnFinished.Enabled = false;
				return;
			}
			ListViewItem lvi = this.lvBrokerProviders.SelectedItems[0];
			if (lvi.Tag == null) {
				this.pnlBrokerEditor.Controls.Clear();
				ds.BrokerProvider = null;
				return;
			}
			ds.BrokerProvider = (BrokerProvider)lvi.Tag;
			ds.BrokerProvider.EditorInstance.PushBrokerProviderSettingsToEditor();
			ds.BrokerProvider.EditorInstance.Dock = DockStyle.Fill;
			//this.btnNext.Enabled = true;
			//this.btnFinished.Enabled = false;
			this.pnlBrokerEditor.Controls.Clear();
			this.pnlBrokerEditor.Controls.Add(ds.BrokerProvider.EditorInstance);
			this.grpExecution.Text = ds.BrokerProvider.Name + " Settings";
		}
		void lnkStaticDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			if (ds.StaticProvider == null) return;
			//new StaticProviderDetailsForm(ds.StaticProvider).Show();
		}
		void btnCancel_Click(object sender, EventArgs e) {
			if (this.Parent != null) {
				DockContent parentAsDock = this.Parent as DockContent;  
				if (parentAsDock != null) {
					// Really closes the Form, so that next time invoked .Instance should invoke Initialize() to add Rows into this.dgMarketName parentAsDock.Close();
					// RESOLVED: DockContent.Hide() does its job
					parentAsDock.Hide();
				} else {
					base.Parent.Hide();
				}
			} else {
				base.Hide();
			}
		}
		void btnNext_Click(object sender, EventArgs e) { }
		void btnPrevious_Click(object sender, EventArgs e) { }
		void btnSave_Click(object sender, EventArgs e) {
			try {
				this.ApplyEditorsToDataSourceAndClose();
			} catch (Exception exc) {
				Assembler.PopupException("btnSave_Click()", exc);
			}
		}
		void cmbScale_SelectedIndexChanged(object sender, EventArgs e) {
			int i = 0;
			foreach (BarScale barScale in Enum.GetValues(typeof(BarScale))) {
				if (i == this.cmbScale.SelectedIndex) {
					ds.ScaleInterval.Scale = barScale;
					break;
				}
				i++;
			}
			if (this.cmbScale.SelectedIndex > 3) {
				this.nmrInterval.Value = 0;
				this.lblInterval.Enabled = false;
				this.nmrInterval.Enabled = false;
				return;
			}
			this.lblInterval.Enabled = true;
			this.nmrInterval.Enabled = true;
		}
		void nmrInterval_ValueChanged(object sender, EventArgs e) {
			ds.ScaleInterval.Interval = (int)this.nmrInterval.Value;
		}
	}
}