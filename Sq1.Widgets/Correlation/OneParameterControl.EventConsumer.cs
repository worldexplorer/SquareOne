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
				this.Parameter.MaximizationCriterion = MaximizationCriterion.UNKNOWN;
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

			this.dontRaiseContainerShouldSerializedForEachColumnVisibilityChanged_alreadyRaised = true;
			this.populateKPIsToParamColumnHeader();

			try {
				foreach (OLVColumn column in columns) {
					column.IsVisible = newCheckedState;
				}
				this.olv.RebuildColumns();
				this.AlignBaseSizeToDisplayedCells();
				this.ctxOneParameterControl.Show();	//I like when it stays open, but AutoClose=false results in not opening at all

				CorrelatorOneParameterSnapshot snap = this.indicatorParameterNullUnsafe.CorrelatorSnap;
				if (mni == this.mniShowAllBacktestedParams) {
					snap.MniShowAllBacktestsChecked = mni.Checked;
				} else if (mni == this.mniShowChosenParams) {
					snap.MniShowChosenChecked = mni.Checked;
				} else if (mni == this.mniShowDeltasBtwAllAndChosenParams) {
					snap.MniShowDeltaChecked = mni.Checked;

				} else if (mni == this.mniShowMomentumsAverage) {
					snap.MniShowMomentumsAverageChecked = mni.Checked;
				} else if (mni == this.mniShowMomentumsDispersion) {
					snap.MniShowMomentumsDispersionChecked = mni.Checked;
				} else if (mni == this.mniShowMomentumsVariance) {
					snap.MniShowMomentumsVarianceChecked = mni.Checked;
				} else {
					string msg = "I_DONT_HAVE_A_PLACE_IN_CorrelatorOneParameterSnapshot_TO_SAVE_STATE_OF_YOUR_MNI[" + mni.Text + "]";
					Assembler.PopupException(msg);
				}

				// shortest path to serialize CorrelationSnapshot
				this.allParametersControl.Correlator.Executor.ChartShadow.RaiseContextScriptChangedContainerShouldSerialize();
			} catch (Exception ex) {
				string msg = "STRATEGY_SERIALIZATION_FAILED";
				Assembler.PopupException(msg + msig, ex);
			} finally {
				this.dontRaiseContainerShouldSerializedForEachColumnVisibilityChanged_alreadyRaised = false;
			}
		}

		void populateKPIsToParamColumnHeader() {
			string mnisCheckedToHeader = "";
			foreach (ToolStripMenuItem each in this.columnsByFilter.Keys) {
				if (each.Checked == false) continue;
				if (mnisCheckedToHeader != "") mnisCheckedToHeader += ",";
				if (each == this.mniShowAllBacktestedParams) {
					mnisCheckedToHeader += "All";
				} else if (each == this.mniShowChosenParams) {
					mnisCheckedToHeader += "Chosen";
				} else if (each == this.mniShowDeltasBtwAllAndChosenParams) {
					mnisCheckedToHeader += "Deltas";

				} else if (each == this.mniShowMomentumsAverage) {
					mnisCheckedToHeader += "Mean";
				} else if (each == this.mniShowMomentumsDispersion) {
					mnisCheckedToHeader += "StDev";
				} else if (each == this.mniShowMomentumsVariance) {
					mnisCheckedToHeader += "Var";
				} else {
					string msg = "CHECKED_MNIS_TO_HEADER_DOESNT_KNOW_ABOUT_YOUR_NEW_MNI[" + each.Text + "]";
					Assembler.PopupException(msg);
				}
			}
			this.olvcParamValues.Text = this.Parameter.ParameterName + " [" + mnisCheckedToHeader + "]";
		}
		void parameter_ParameterRecalculatedLocalsAndDeltas(object sender, OneParameterAllValuesAveragedEventArgs e) {
			this.Initialize();
		}
		void mniShowAllVisibleCells_Click(object sender, EventArgs e) {
			this.AlignBaseSizeToDisplayedCells();
		}
	}
}
