using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataTypes;

namespace Sq1.Widgets.DataSourceEditor {
	public partial class MarketInfoEditor {
		void dgMarketName_SelectionChanged(object sender, EventArgs e) {
			if (this.dgMarketName.SelectedRows.Count == 0) return;
			var dgSelectedRow = this.dgMarketName.SelectedRows[0];
			if (dgSelectedRow == null) return;
			object tagContent = dgSelectedRow.Tag;
			if (tagContent == null) return;
			MarketInfo marketInfo_onlyEditing_withoutChangingInDataSource = tagContent as MarketInfo;
			if (marketInfo_onlyEditing_withoutChangingInDataSource == null) return;
			try {
				this.populateAllDataGrids_withMarketInfo(marketInfo_onlyEditing_withoutChangingInDataSource);
			} catch (Exception ex) {
				Assembler.PopupException("dgMarketName_SelectionChanged", ex);
			}
		}

		void dgMarketName_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e) {
			if (this.ignoreSelectionEventDuringPopulate) return;
			this.lnkMarketNameDelete.Enabled = false;
			if (this.dgMarketName.SelectedRows.Count == 0) return;
			if (e.ColumnIndex != 1) return;		// doubleClick on UserCount only => will switch Market for the DataSource

			MarketInfo marketInfo = (MarketInfo)this.dgMarketName.SelectedRows[0].Tag;
			if (marketInfo == null) return;
			try {
				this.dataSource.MarketInfo = marketInfo;
				this.populateMarket_fromDataSource();
				this.lnkMarketNameDelete.Enabled = (this.dataSourceRepository.SameMarketInfoInHowManyDataSources(marketInfo) > 0) ? true : false;
				this.dataSourceRepository.SerializeSingle(this.dataSource);
			} catch (Exception ex) {
				Assembler.PopupException("dgMarketName_SelectionChanged", ex);
			}
		}
		void lnkMarketNameDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

		}
		void dgMarketName_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
			if (this.ignoreSelectionEventDuringPopulate) return;
			if (e.RowIndex < 0) return;
			if (e.ColumnIndex < 0) return;
			string errormsg = "";
			string headerText = this.dgMarketName.Columns[e.ColumnIndex].HeaderText;
			MarketInfo marketInfo = this.dgMarketName.Rows[e.RowIndex].Tag as MarketInfo;
			if (marketInfo == null) {
				string msg = "Rows[" + e.RowIndex + "].Tag=null (expecting MarketInfo) " + headerText;
				Assembler.PopupException(msg + " /dgMarketName_SelectionChanged");
				return;
			}
			DataGridViewCell cell = this.dgMarketName[e.ColumnIndex, e.RowIndex];
			string cellValueAsString = cell.Value.ToString();
			string columnNameChanged = this.dgMarketName.Columns[e.ColumnIndex].Name;
			this.ignoreSelectionEventDuringPopulate = true;
			switch (columnNameChanged) {
				case "colMarketName":
					if (string.IsNullOrEmpty(cellValueAsString) == false) {
						marketInfo.Name = cellValueAsString;
						this.marketInfoRepository.RenameMarketInfoRearrangeDictionary(marketInfo);
					} else {
						errormsg = columnNameChanged + ": cellValueAsString[" + cellValueAsString + "] IsNullOrEmpty"
							+ ", back to old value [" + marketInfo.Name + "]";
					}
					cell.Value = marketInfo.Name;
					break;
				default:
					errormsg = "no handler for field:[" + columnNameChanged + "]";
					break;
			}
			this.ignoreSelectionEventDuringPopulate = false;
			if (string.IsNullOrEmpty(errormsg) == false) {
				string msg = errormsg + headerText;
				Assembler.PopupException(msg + " /dgMarketName_CellValueChanged", null, false);
				return;
			}
			this.marketInfoRepository.Serialize();
		}
	}
}
