using System;
using System.Collections.Generic;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.Sequencing;
using Sq1.Core.Correlation;

namespace Sq1.Widgets.Correlation {
	public partial class OneParameterControl {

		void mniMaximizationKPISortDescendingBy_Click(object sender, EventArgs e) {
			string msig = " //mniMaximizationKPISortDescendingBy_CheckStateChanged()";
			ToolStripMenuItem mniClicked = sender as ToolStripMenuItem;
			if (mniClicked == null) {
				string msg = "You clicked on something not being a ToolStripMenuItem";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (mniClicked.Checked) {
				this.parameter.MaximizationCriterion = MaximizationCriterion.UNKNOWN;
				mniClicked.Checked = false;
				this.olv.Unsort();
				return;
			}
			this.checkOneMniForMaximizationCriterionUncheckOthers(mniClicked);
			// MOVED_TO_WHERE_THE_OLV_ISNT_EMPTY__DOESNT_SORT_ON_EMPTY_STOMACH
			OLVColumn sortBy = this.columnToSortDescendingByMaximizationMni[mniClicked];
			this.olv.Sort(sortBy, SortOrder.Descending);
		}

		void mniShowColumnByFilter_Click(object sender, EventArgs e) {
			string msig = " //mniShowColumnByFilter_CheckedChanged()";
			ToolStripMenuItem mni = sender as ToolStripMenuItem;
			if (mni == null) {
				string msg = "You clicked on something not being a ToolStripMenuItem";
				Assembler.PopupException(msg + msig);
				return;
			}
			// F4.CheckOnClick=False because mni stays unchecked after I just checked
			//mni.Checked = !mni.Checked;
			if (this.columnsByFilter.ContainsKey(mni) == false) {
				string msg = "Add ToolStripMenuItem[" + mni.Name + "] into columnByFilters";
				Assembler.PopupException(msg + msig);
				return;
			}
			bool newCheckedState = mni.Checked;
			// F4.CheckOnClick=true mni.Checked = newState;
			//	this.settingsManager.Set("ExecutionForm." + mni.Name + ".Checked", mni.Checked);
			//	this.settingsManager.SaveSettings();

			List<OLVColumn> columns = this.columnsByFilter[mni];
			if (columns.Count == 0) return;

			foreach (OLVColumn column in columns) {
				column.IsVisible = newCheckedState;
			}
			this.olv.RebuildColumns();
			this.ctxOneParameterControl.Show();	//I like when it stays open, but AutoClose=false results in not opening at all
		}

		void parameter_ParameterRecalculatedLocalsAndDeltas(object sender, OneParameterAllValuesAveragedEventArgs e) {
			Initialize();
		}

	}
}
