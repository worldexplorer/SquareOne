using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;
using Sq1.Core.DataFeed;

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
				dataSourceIamEditing.StreamingAdapter = null;
				return;
			}
			dataSourceIamEditing.StreamingAdapter = (StreamingAdapter)lvi.Tag;
			dataSourceIamEditing.StreamingAdapter.EditorInstance.PushStreamingAdapterSettingsToEditor();
			dataSourceIamEditing.StreamingAdapter.EditorInstance.Dock = DockStyle.Fill;
			//this.btnNext.Enabled = true;
			//this.btnFinished.Enabled = false;
			this.pnlStreamingEditor.Controls.Clear();
			this.pnlStreamingEditor.Controls.Add(dataSourceIamEditing.StreamingAdapter.EditorInstance);
			this.grpStreaming.Text = dataSourceIamEditing.StreamingAdapter.Name + " Settings";
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
				dataSourceIamEditing.BrokerAdapter = null;
				return;
			}
			dataSourceIamEditing.BrokerAdapter = (BrokerAdapter)lvi.Tag;
			dataSourceIamEditing.BrokerAdapter.EditorInstance.PushBrokerAdapterSettingsToEditor();
			dataSourceIamEditing.BrokerAdapter.EditorInstance.Dock = DockStyle.Fill;
			//this.btnNext.Enabled = true;
			//this.btnFinished.Enabled = false;
			this.pnlBrokerEditor.Controls.Clear();
			this.pnlBrokerEditor.Controls.Add(dataSourceIamEditing.BrokerAdapter.EditorInstance);
			this.grpExecution.Text = dataSourceIamEditing.BrokerAdapter.Name + " Settings";
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
					dataSourceIamEditing.ScaleInterval.Scale = barScale;
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
			dataSourceIamEditing.ScaleInterval.Interval = (int)this.nmrInterval.Value;
		}

		void repositoryJsonDataSource_OnDataSourceRenamed_refreshTitle(object sender, NamedObjectJsonEventArgs<DataSource> e) {
			this.txtDataSourceName.Text = this.dataSourceIamEditing.Name;
		}
		void repositoryJsonDataSource_OnDataSourceDeleted_closeDataSourceEditor(object sender, NamedObjectJsonEventArgs<DataSource> e) {
			this.ParentForm.Close();
		}
		void repositoryJsonDataSource_OnSymbolAddedRenamedRemoved_refreshSymbolsTextarea(object sender, DataSourceSymbolEventArgs e) {
			if (this.dataSourceIamEditing != e.DataSource) {
				string msg = "NOT_THE_DATASOURCE_IM_EDITING_IGNORING"
					//+ " WHERE_SHOULD_I_GET_SymbolsCSV ? this.ds[" + this.ds.Name + "] != e.DataSource[" + e.DataSource.Name + "]"
					//+ this.ds.Name + ".SymbolsCSV[" + this.ds.Name + "] or [" + e.DataSource.Name + "].SymbolsCSV" + e.DataSource.SymbolsCSV + "] ?"
					;
				Assembler.PopupException(msg, null, false);
				return;
			}
			//this.txtSymbols.Text = this.ds.SymbolsCSV;
			this.txtSymbols.Text = e.DataSource.SymbolsCSV;
		}
	}
}