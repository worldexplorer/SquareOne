using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;

namespace Sq1.Widgets.DataSourceEditor {
	public partial class DataSourceEditorControl {
		void lvStreamingAdapters_SelectedIndexChanged(object sender, EventArgs e) {
			if (this.lvStreamingAdapters.SelectedItems.Count == 0) {
				//this.btnNext.Enabled = false;
				//this.btnFinished.Enabled = false;
				return;
			}
			ListViewItem lvi = this.lvStreamingAdapters.SelectedItems[0];
			if (lvi.Tag == null) {
				this.pnlStreamingEditor.Controls.Clear();
				ds.StreamingAdapter = null;
				return;
			}
			ds.StreamingAdapter = (StreamingAdapter)lvi.Tag;
			ds.StreamingAdapter.EditorInstance.PushStreamingAdapterSettingsToEditor();
			ds.StreamingAdapter.EditorInstance.Dock = DockStyle.Fill;
			//this.btnNext.Enabled = true;
			//this.btnFinished.Enabled = false;
			this.pnlStreamingEditor.Controls.Clear();
			this.pnlStreamingEditor.Controls.Add(ds.StreamingAdapter.EditorInstance);
			this.grpStreaming.Text = ds.StreamingAdapter.Name + " Settings";
		}
		void lvBrokerAdapters_SelectedIndexChanged(object sender, EventArgs e) {
			if (this.lvBrokerAdapters.SelectedItems.Count == 0) {
				//this.btnNext.Enabled = false;
				//this.btnFinished.Enabled = false;
				return;
			}
			ListViewItem lvi = this.lvBrokerAdapters.SelectedItems[0];
			if (lvi.Tag == null) {
				this.pnlBrokerEditor.Controls.Clear();
				ds.BrokerAdapter = null;
				return;
			}
			ds.BrokerAdapter = (BrokerAdapter)lvi.Tag;
			ds.BrokerAdapter.EditorInstance.PushBrokerAdapterSettingsToEditor();
			ds.BrokerAdapter.EditorInstance.Dock = DockStyle.Fill;
			//this.btnNext.Enabled = true;
			//this.btnFinished.Enabled = false;
			this.pnlBrokerEditor.Controls.Clear();
			this.pnlBrokerEditor.Controls.Add(ds.BrokerAdapter.EditorInstance);
			this.grpExecution.Text = ds.BrokerAdapter.Name + " Settings";
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