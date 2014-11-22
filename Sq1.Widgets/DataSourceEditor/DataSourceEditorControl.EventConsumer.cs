using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;

namespace Sq1.Widgets.DataSourceEditor {
	public partial class DataSourceEditorControl {
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