using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Sq1.Core.Execution;

namespace Sq1.Widgets.Execution {
	public partial class ExecutionTreeControl {
		void InitializeSearch() {
			//this.pnlSearch.Visible = this.dataSnapshot.ShowSearchbar;
			this.statusStrip_search.Visible = this.dataSnapshot.ShowSearchbar;

			if (this.dataSnapshot.ShowSearchbar) {
				if (this.dataSnapshot.ShowSearchbar_ExcludeKeywordApplied) {
					this.keywordsExclude_apply();
				}
				if (this.dataSnapshot.ShowSearchbar_SearchKeywordApplied) {
					this.keywordsSearch_apply();
				}
			}

			this.tsiCbx_SearchApply.Click += new EventHandler(this.tsiCbx_SearchApply_Click);
			this.tsiCbx_SearchApply.CheckBoxChecked = this.dataSnapshot.ShowSearchbar_SearchKeywordApplied;
			this.tsiLtb_SearchKeywords.LabeledTextBoxControl.InternalTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyUp);
			this.tsiLtb_SearchKeywords.LabeledTextBoxControl.InternalTextBox.Text = this.dataSnapshot.ShowSearchbar_SearchKeywordsCsv;

			this.tsiCbx_ExcludeApply.Click += new EventHandler(this.tsiCbx_ExcludeApply_Click);
			this.tsiCbx_ExcludeApply.CheckBoxChecked = this.dataSnapshot.ShowSearchbar_ExcludeKeywordApplied;
			this.tsiLtb_ExcludeKeywords.LabeledTextBoxControl.InternalTextBox.Text = this.dataSnapshot.ShowSearchbar_ExcludeKeywordsCsv;
			this.tsiLtb_ExcludeKeywords.LabeledTextBoxControl.InternalTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtExclude_KeyUp);

			// who makes them blue???
			base.BackColor = SystemColors.Control;
			//this.statusStrip_search.BackColor = SystemColors.Control;
			//this.tsilbl_separator.BackColor = SystemColors.Control;
			//this.tsiCbx_SearchApply.BackColor = SystemColors.Control;
			//this.tsiCbx_ExcludeApply.BackColor = SystemColors.Control;
		}

		void tsiCbx_SearchApply_Click(object sender, EventArgs e) {
			this.dataSnapshot.ShowSearchbar_SearchKeywordApplied = this.tsiCbx_SearchApply.CheckBoxChecked;
			this.dataSnapshotSerializer.Serialize();
			this.keywordsSearch_apply();
		}
		void txtSearch_KeyUp(object sender, KeyEventArgs e) {
			this.dataSnapshot.ShowSearchbar_SearchKeywordsCsv = this.tsiLtb_SearchKeywords.LabeledTextBoxControl.InternalTextBox.Text;
			this.dataSnapshotSerializer.Serialize();
			if (this.dataSnapshot.ShowSearchbar_SearchKeywordApplied == false) return;
			this.keywordsSearch_apply();
		}

		void tsiCbx_ExcludeApply_Click(object sender, EventArgs e) {
			this.dataSnapshot.ShowSearchbar_ExcludeKeywordApplied = this.tsiCbx_ExcludeApply.CheckBoxChecked;
			this.dataSnapshotSerializer.Serialize();

			this.keywordsExclude_apply();
		}

		void txtExclude_KeyUp(object sender, KeyEventArgs e) {
			this.dataSnapshot.ShowSearchbar_ExcludeKeywordsCsv = this.tsiLtb_ExcludeKeywords.LabeledTextBoxControl.InternalTextBox.Text;
			this.dataSnapshotSerializer.Serialize();

			if (this.dataSnapshot.ShowSearchbar_ExcludeKeywordApplied == false) return;
			this.keywordsExclude_apply();
		}

		void btnSearchClose_Click(object sender, EventArgs e) {
			//this.pnlSearch.Visible = false;
			this.statusStrip_search.Visible = false;
			//this.mniShowSearchbar.Checked = false;
			//this.olvOrdersTree.SetObjects(this.ordersRoot.SafeCopy(this, "btnSearchClose_Click()"));
			this.olvOrdersTree.BackColor = Color.White;
		}
		void tsiBtnClear_Click(object sender, EventArgs e) {
			this.Clear();
		}


		void keywordsSearch_apply() {
			string keywordsCsv_nullUnsafe =
					this.dataSnapshot.ShowSearchbar_SearchKeywordApplied
				?	this.dataSnapshot.ShowSearchbar_SearchKeywordsCsv
				:	null;

			bool willBeFlat =
				this.dataSnapshot.ShowSearchbar_SearchKeywordApplied	&& string.IsNullOrEmpty(this.dataSnapshot.ShowSearchbar_SearchKeywordsCsv) == false ||
				this.dataSnapshot.ShowSearchbar_ExcludeKeywordApplied	&& string.IsNullOrEmpty(this.dataSnapshot.ShowSearchbar_ExcludeKeywordsCsv) == false;
			this.ordersViewProxy.ShowingTreeTrue_FlatFalse = willBeFlat == false;

			this.ordersViewProxy.SearchForKeywords(keywordsCsv_nullUnsafe);
			this.RebuildAllTree_focusOnRecent();

			if (string.IsNullOrEmpty(keywordsCsv_nullUnsafe)) {
			    this.olvOrdersTree.SetObjects(this.ordersViewProxy.OrdersList_switchable);

			    this.tsiLtb_SearchKeywords.LabeledTextBoxControl.InternalTextBox.BackColor = Color.White;
			    // yeps color from the opposite checkbox
			    Color newBgColor = this.dataSnapshot.ShowSearchbar_ExcludeKeywordApplied ? Color.FloralWhite : Color.White;
			    this.olvOrdersTree.BackColor = newBgColor;
			    return;
			}

			this.olvOrdersTree.BackColor = Color.LightGreen;
			this.tsiLtb_SearchKeywords.LabeledTextBoxControl.InternalTextBox.BackColor = Color.LightGreen;
		}

		void keywordsExclude_apply() {
			string keywordsCsv_nullUnsafe =
					this.dataSnapshot.ShowSearchbar_ExcludeKeywordApplied
				?	this.dataSnapshot.ShowSearchbar_ExcludeKeywordsCsv
				:	null;

			bool willBeFlat =
				this.dataSnapshot.ShowSearchbar_SearchKeywordApplied	&& string.IsNullOrEmpty(this.dataSnapshot.ShowSearchbar_SearchKeywordsCsv) == false ||
				this.dataSnapshot.ShowSearchbar_ExcludeKeywordApplied	&& string.IsNullOrEmpty(this.dataSnapshot.ShowSearchbar_ExcludeKeywordsCsv) == false;
			this.ordersViewProxy.ShowingTreeTrue_FlatFalse = willBeFlat == false;

			if (string.IsNullOrEmpty(keywordsCsv_nullUnsafe)) {
			    this.ordersViewProxy.OrdersSearchable_forGui.InitKeywordsToExclude_AndSetPointer(null);
			    this.olvOrdersTree.SetObjects(this.ordersViewProxy.OrdersList_switchable);

			    // yeps color from the opposite checkbox
			    Color newBgColor = this.dataSnapshot.ShowSearchbar_SearchKeywordApplied ? Color.LightGreen : Color.White;
			    this.olvOrdersTree.BackColor = newBgColor;
			    this.tsiLtb_ExcludeKeywords.LabeledTextBoxControl.InternalTextBox.BackColor = Color.White;
			    return;
			}

			List<Order> innerList_excludeKeywordsApplied = this.ordersViewProxy.OrdersSearchable_forGui.InitKeywordsToExclude_AndSetPointer(keywordsCsv_nullUnsafe);
			this.olvOrdersTree.SetObjects(innerList_excludeKeywordsApplied);
			this.RebuildAllTree_focusOnRecent();

			// yeps color from the opposite checkbox
			Color oppositeBgColor = this.dataSnapshot.ShowSearchbar_SearchKeywordApplied ? Color.LightGreen : Color.FloralWhite;
			this.olvOrdersTree.BackColor = oppositeBgColor;
			this.tsiLtb_ExcludeKeywords.LabeledTextBoxControl.InternalTextBox.BackColor = Color.FloralWhite;
		}

		/*
		string exceptionMessageFragmentSelected;
		void ctxExceptionMessage_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
			this.exceptionMessageFragmentSelected = this.txtExceptionMessage.SelectedText;
			this.mniAddToExcludeKeywordsCsv.Text = "Append [" + this.exceptionMessageFragmentSelected + "] to KeywordsExcluded list";
		}

		void mniAddToExcludeKeywordsCsv_Click(object sender, EventArgs e) {
			this.ordersRoot.AppendKeywordToIgnore(this.exceptionMessageFragmentSelected);
			this.dataSnapshot.ShowSearchbar_ExcludeKeywordsCsv = this.ordersRoot.KeywordsToExclude_asCsv;
			this.tsiLtb_ExcludeKeywords.InputFieldValue = this.dataSnapshot.ShowSearchbar_ExcludeKeywordsCsv;
			this.dataSnapshotSerializer.Serialize();
			this.keywordsExclude_apply();
		}
		*/
		
		//void mniShowSearchbar_Click(object sender, EventArgs e) {
		//    this.dataSnapshot.ShowSearchbar = this.mniShowSearchbar.Checked;
		//    this.dataSnapshotSerializer.Serialize();
		//    //this.pnlSearch.Visible = this.dataSnapshot.ShowSearchbar;
		//    //this.txtSearch.Focus();
		//    //if (this.pnlSearch.Visible == false) {
		//    //    this.olvOrdersTree.SetObjects(this.ordersRoot.SafeCopy(this, "mniShowSearchbar_Click"));
		//    //}
		//    this.statusStrip_search.Visible = this.dataSnapshot.ShowSearchbar;
		//    if (this.statusStrip_search.Visible == false) {
		//        this.olvOrdersTree.SetObjects(this.ordersRoot.SafeCopy(this, "mniShowSearchbar_Click"));
		//    } else {
		//        this.tsiLtb_SearchKeywords.LabeledTextBoxControl.InternalTextBox.Focus();
		//    }

		//    this.keywordsExclude_apply();
		//    this.keywordsSearch_apply();

		//    this.ctxTree.Show();
		//}

	}
}