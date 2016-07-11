using System;
using System.Windows.Forms;

namespace Sq1.Widgets.DataSourceEditor {
	public partial class MarketInfoEditor {
		void dgHolidays_SelectionChanged(object sender, EventArgs e) {
			if (this.ignoreSelectionEventDuringPopulate) return;
			this.lnkHolidaysDelete.Enabled = (this.dgHolidays.SelectedRows.Count > 0) ? true : false;
		}
		void lnkHolidaysDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

		}
		void dgHolidays_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

		}
	}
}
