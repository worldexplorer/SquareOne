using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Sq1.Widgets.Exceptions {
	public partial class ExceptionsControl {
		void InitializeSearch() {
			//this.pnlSearch.Visible = this.dataSnapshot.ShowSearchbar;
			this.statusStrip_search.Visible = this.dataSnapshot.ShowSearchbar;

			if (this.dataSnapshot.ShowSearchbar) {
				if(this.dataSnapshot.ShowSearchbar_ExcludeKeywordApplied) {
					this.keywordsExclude_apply();
				}
				if (this.dataSnapshot.ShowSearchbar_SearchKeywordApplied) {
					this.keywordsSearch_apply();
				}
			}

			//this.tsiLtb_SearchKeyword.LabeledTextBoxControl.InternalTextBox.KeyUp += new KeyEventHandler(InternalTextBox_KeyUp);
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
		void keywordsSearch_apply() {
			string keywordsCsv_nullUnsafe =
					this.dataSnapshot.ShowSearchbar_SearchKeywordApplied
				?	this.dataSnapshot.ShowSearchbar_SearchKeywordsCsv
				:	null;

			if (string.IsNullOrEmpty(keywordsCsv_nullUnsafe)) {
				this.olvTreeExceptions.SetObjects(this.Exceptions.SafeCopy(this, "keywordsSearch_apply()"));

				this.tsiLtb_SearchKeywords.LabeledTextBoxControl.InternalTextBox.BackColor = Color.White;
				// yeps color from the opposite checkbox
				Color newBgColor = this.dataSnapshot.ShowSearchbar_ExcludeKeywordApplied ? Color.FloralWhite : Color.White;
				this.olvTreeExceptions.BackColor = newBgColor;
				//this.txtSearch.BackColor = newBgColor;
				return;
			}

			List<Exception> filtered_fromAllOrNoExcluded = this.Exceptions.SearchForKeywords_StaticSnapshotSubset(keywordsCsv_nullUnsafe);
			this.olvTreeExceptions.SetObjects(filtered_fromAllOrNoExcluded);
			this.olvTreeExceptions.BackColor = Color.LightSalmon;
			//this.txtSearch.BackColor = Color.LightSalmon;
			this.tsiLtb_SearchKeywords.LabeledTextBoxControl.InternalTextBox.BackColor = Color.LightSalmon;
		}

		void keywordsExclude_apply() {
			string keywordsCsv_nullUnsafe =
					this.dataSnapshot.ShowSearchbar_ExcludeKeywordApplied
				?	this.dataSnapshot.ShowSearchbar_ExcludeKeywordsCsv
				:	null;

			if (string.IsNullOrEmpty(keywordsCsv_nullUnsafe)) {
				this.Exceptions.InitKeywordsToExclude_AndSetPointer(null);
				this.olvTreeExceptions.SetObjects(this.Exceptions.SafeCopy(this, "keywordsExclude_apply()"));

				// yeps color from the opposite checkbox
				Color newBgColor = this.dataSnapshot.ShowSearchbar_SearchKeywordApplied ? Color.LightSalmon : Color.White;
				this.olvTreeExceptions.BackColor = newBgColor;
				//this.txtSearch.BackColor = newBgColor;
				this.tsiLtb_ExcludeKeywords.LabeledTextBoxControl.InternalTextBox.BackColor = Color.White;
				return;
			}

			List<Exception> innerList_excludeKeywordsApplied = this.Exceptions.InitKeywordsToExclude_AndSetPointer(keywordsCsv_nullUnsafe);
			this.olvTreeExceptions.SetObjects(innerList_excludeKeywordsApplied);

			// yeps color from the opposite checkbox
			Color oppositeBgColor = this.dataSnapshot.ShowSearchbar_SearchKeywordApplied ? Color.LightSalmon : Color.FloralWhite;
			this.olvTreeExceptions.BackColor = oppositeBgColor;
			//this.txtSearch.BackColor = Color.FloralWhite;
			this.tsiLtb_ExcludeKeywords.LabeledTextBoxControl.InternalTextBox.BackColor = Color.FloralWhite;
		}

		void mniShowSearchbar_Click(object sender, EventArgs e) {
			this.dataSnapshot.ShowSearchbar = this.mniShowSearchbar.Checked;
			this.dataSnapshotSerializer.Serialize();
			//this.pnlSearch.Visible = this.dataSnapshot.ShowSearchbar;
			//this.txtSearch.Focus();
			//if (this.pnlSearch.Visible == false) {
			//    this.olvTreeExceptions.SetObjects(this.Exceptions.SafeCopy(this, "mniShowSearchbar_Click"));
			//}
			this.statusStrip_search.Visible = this.dataSnapshot.ShowSearchbar;
			if (this.statusStrip_search.Visible == false) {
				this.olvTreeExceptions.SetObjects(this.Exceptions.SafeCopy(this, "mniShowSearchbar_Click"));
			} else {
				this.tsiLtb_SearchKeywords.LabeledTextBoxControl.InternalTextBox.Focus();
			}

			this.keywordsExclude_apply();
			this.keywordsSearch_apply();

			this.ctxTree.Show();
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
			//switch(e.KeyCode) {
			//    //case Keys.Enter:
			//    //    List<Exception> filtered = this.Exceptions.SubsetContainingKeyword(this.txtSearch.Text);
			//    //    this.olvTreeExceptions.SetObjects(filtered);
			//    //    this.olvTreeExceptions.BackColor = Color.Gainsboro;
			//    //    return;
			//    //case Keys.Escape:
			//    //    this.btnSearchClose_Click(this, null);
			//    //    return;
			//}
			////string keywords = this.txtSearch.Text;
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
			this.mniShowSearchbar.Checked = false;
			this.olvTreeExceptions.SetObjects(this.Exceptions.SafeCopy(this, "btnSearchClose_Click()"));
			this.olvTreeExceptions.BackColor = Color.White;
		}
		void btnSearchClear_Click(object sender, EventArgs e) {
			this.mniClear_Click(sender, e);
		}


		void mniShowCounterAndGroup_Click(object sender, EventArgs e) {
			this.dataSnapshot.ShowTimesOccured = this.mniShowCounterAndGroup.Checked;
			this.dataSnapshotSerializer.Serialize();
			//if (this.dataSnapshot.ShowCounterToGroup == false) return;
			this.olvcTimesOccured.IsVisible = this.dataSnapshot.ShowTimesOccured;
			this.ctxTree.Show();
		}


		void mniExcludeKeywordsContaining_Click(object sender, EventArgs e) {
			this.dataSnapshot.ShowSearchbar_ExcludeKeywordApplied = this.mniExcludeKeywords.Checked;
			this.dataSnapshotSerializer.Serialize();
		}

		void tsiBtnClear_Click(object sender, EventArgs e) {
			this.mniClear_Click(sender, e);
		}


		string exceptionMessageFragmentSelected;


		void ctxExceptionMessage_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
			this.exceptionMessageFragmentSelected = this.txtExceptionMessage.SelectedText;
			this.mniAddToExcludeKeywordsCsv.Text = "Append [" + this.exceptionMessageFragmentSelected + "] to KeywordsExcluded list";
		}

		void mniAddToExcludeKeywordsCsv_Click(object sender, EventArgs e) {
			this.Exceptions.AppendKeywordToIgnore(this.exceptionMessageFragmentSelected);
			this.dataSnapshot.ShowSearchbar_ExcludeKeywordsCsv = this.Exceptions.KeywordsToExclude_asCsv;
			this.tsiLtb_ExcludeKeywords.InputFieldValue = this.dataSnapshot.ShowSearchbar_ExcludeKeywordsCsv;
			this.dataSnapshotSerializer.Serialize();
			this.keywordsExclude_apply();
		}

	}
}