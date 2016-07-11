using System;
using System.Windows.Forms;

namespace Sq1.Widgets.DataSourceEditor {
	public partial class MarketInfoEditor {
		void dgShortDays_SelectionChanged(object sender, EventArgs e) {
			if (this.ignoreSelectionEventDuringPopulate) return;
			this.lnkShortDaysDelete.Enabled = (this.dgShortDays.SelectedRows.Count > 0) ? true : false;
		}

		void dgShortDays_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

		}
		void lnkShortDaysDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
 
		}
	}
}
