using System;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;

using Sq1.Widgets.RangeBar;

namespace Sq1.Widgets.FuturesMerger {
	public partial class BarsEditorUserControl {
		void btnSave_Click(object sender, System.EventArgs e) {
			Assembler.PopupException("SHOULD_DATASOURCE_REPO_PROVIDE_BARS_SAVING_METHODS???", null, false);
			this.barsCloned.Save();
		}

		void btnReload_Click(object sender, EventArgs e) {
			this.reload();
		}

		void cbxRevert_Click(object sender, EventArgs e) {
			this.barsDescending = this.cbxRevert.Checked;
			this.reload();
		}
		void cbxShowDatasources_Click(object sender, System.EventArgs e) {
			 this.dataSourcesTreeCollapsed = !this.cbxShowDatasources.Checked;
		}
		void cbxShowRange_Click(object sender, System.EventArgs e) {
			 this.barsRangeCollapsed = !this.cbxShowRange.Checked;
		}
		void dataSourcesTreeControl_OnSymbolSelected(object sender, DataSourceSymbolEventArgs e) {
			this.LoadBars(e.DataSource.Name, e.Symbol);
		}

		void rangeBar_OnAnyValueChanged(object sender, RangeArgs<DateTime> e) {
			BarDataRange userDragged_bBarDataRange = new BarDataRange(e.ValueMin, e.ValueMax);
			this.bars_selectRange_flushToGui(userDragged_bBarDataRange);
		}
	}
}
