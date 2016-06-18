using System;
using System.Windows.Forms;

using Sq1.Core;

using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Widgets.Exceptions {
	public partial class ExceptionsControl {
		void form_OnLoad(object sender, System.EventArgs e) {
			if (base.DesignMode) return;
			// IF_ONLY_YOU_WANT_TO_FLUSH_DESERIALIZED_FROM_LAST_RESTART_NIY this.FlushExceptionsToOLVIfDockContentDeserialized_inGuiThread();

			this.mniShowHeaders.Checked = this.dataSnapshot.ShowHeaders;
			this.olvTreeExceptions	.HeaderStyle = this.dataSnapshot.ShowHeaders ? ColumnHeaderStyle.Clickable : ColumnHeaderStyle.None;
			this.olvStackTrace		.HeaderStyle = this.dataSnapshot.ShowHeaders ? ColumnHeaderStyle.Clickable : ColumnHeaderStyle.None;

			this.mniShowTimestamps.Checked = this.dataSnapshot.TreeShowTimestamps;
			this.olvcTime.IsVisible = this.dataSnapshot.TreeShowTimestamps;
			this.olvTreeExceptions.RebuildColumns();

			this.mniPopupOnIncomingException.Checked = this.dataSnapshot.PopupOnIncomingException;
		}
		void tree_SelectedIndexChanged(object sender, EventArgs e) {
			if (this.olvTreeExceptions.SelectedObject == null) {
				this.txtExceptionMessage.Text = "";
				return;
			}
			this.txtExceptionMessage.Text = this.exceptionSingleSelectedInTree_nullUnsafe.Message;
			this.displayStackTrace_forSingleSelected();
		}
		void mniClear_Click(object sender, EventArgs e) {
			this.flushExceptionsToOLV_switchToGuiThread();
			this.Exceptions.Clear(this, "mniClear_Click");
			this.flushExceptionsToOLV_switchToGuiThread();
		}
		void mniCopy_Click(object sender, EventArgs e) {
			this.copyExceptionsSelected_toClipboard();
		}
		void splitContainerVertical_SplitterMoved(object sender, SplitterEventArgs e) {
			if (this.dataSnapshot == null) return;	// there is no DataSnapshot deserialized in InitializeComponents()
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;
			//v1 WHATT?? BECAUSE_MESSAGE_DELIVERY_IS_ASYNC_IM_FIRED_AFTER_IT'S_ALREADY_TRUE
			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) return;
			//v2 HACK http://stackoverflow.com/questions/10161088/get-elapsed-time-since-application-start-in-c-sharp
			//try {
			//	TimeSpan sinceApplicationStart = DateTime.Now - Process.GetCurrentProcess().StartTime;
			//	if (sinceApplicationStart.Seconds <= 10) return;
			//} catch (Exception ex) {
			//	Assembler.PopupException("SEEMS_TO_BE_UNSUPPORTED_Process.GetCurrentProcess()", ex);
			//}
			//v3
			//NOT_UNDER_WINDOWS
			if (Assembler.InstanceInitialized.SplitterEventsAllowed_tenSecondsPassed_afterAppLaunch_hopingInitialInnerDockResizingIsFinished == false) return;
			if (this.dataSnapshot.SplitDistanceVertical == this.splitContainerVertical.SplitterDistance) return;
				this.dataSnapshot.SplitDistanceVertical  = this.splitContainerVertical.SplitterDistance;
			this.dataSnapshotSerializer.Serialize();
		}
		
		void splitContainerHorizontal_SplitterMoved(object sender, SplitterEventArgs e) {
			if (this.dataSnapshot == null) return;	// there is no DataSnapshot deserialized in InitializeComponents()
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;
			//v1 WHATT??? BECAUSE_MESSAGE_DELIVERY_IS_ASYNC_IM_FIRED_AFTER_IT'S_ALREADY_TRUE
			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) return;
			//v2 HACK http://stackoverflow.com/questions/10161088/get-elapsed-time-since-application-start-in-c-sharp
			//try {
			//	TimeSpan sinceApplicationStart = DateTime.Now - Process.GetCurrentProcess().StartTime;
			//	if (sinceApplicationStart.Seconds <= 10) return;
			//} catch (Exception ex) {
			//	Assembler.PopupException("SEEMS_TO_BE_UNSUPPORTED_Process.GetCurrentProcess()", ex);
			//}
			//v3 NOT_UNDER_WINDOWS
			if (Assembler.InstanceInitialized.SplitterEventsAllowed_tenSecondsPassed_afterAppLaunch_hopingInitialInnerDockResizingIsFinished == false) return;
			if (this.dataSnapshot.SplitDistanceHorizontal == this.splitContainerHorizontal.SplitterDistance) return;
				this.dataSnapshot.SplitDistanceHorizontal =  this.splitContainerHorizontal.SplitterDistance;
			this.dataSnapshotSerializer.Serialize();
		}
		void mniCopyStackPosition_Click(object sender, EventArgs e) {
			//this.P
		}
		void mniltbFlushToGuiDelayMsec_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			MenuItemLabeledTextBox mnilbDelay = sender as MenuItemLabeledTextBox;
			string typed = e.StringUserTyped;
			int typedMsec = this.dataSnapshot.FlushToGuiDelayMsec;
			bool parsed = Int32.TryParse(typed, out typedMsec);
			if (parsed == false) {
				mnilbDelay.InputFieldValue = this.dataSnapshot.FlushToGuiDelayMsec.ToString();
				mnilbDelay.TextRed = true;
				return;
			}
			this.dataSnapshot.FlushToGuiDelayMsec = typedMsec;
			this.dataSnapshotSerializer.Serialize();
			this.Timed_flushingToGui.DelayMillis = this.dataSnapshot.FlushToGuiDelayMsec;
			mnilbDelay.TextRed = false;
			e.RootHandlerShouldCloseParentContextMenuStrip = true;
			this.populateWindowTitle();
			this.ctxTree.Visible = true;	// keep it open
		}
		void mniShowTimestamps_Click(object sender, EventArgs e) {
			this.dataSnapshot.TreeShowTimestamps = mniShowTimestamps.Checked;
			this.dataSnapshotSerializer.Serialize();
			//this.olvcTime.Text = this.DataSnapshot.TreeShowExceptionTime ? "Time" : "Message";
			//this.olvTreeExceptions.RebuildAll(true);
			this.olvcTime.IsVisible = this.dataSnapshot.TreeShowTimestamps;
			this.olvTreeExceptions.RebuildColumns();
			this.ctxTree.Visible = true;	// keep it open
		}
		void mniRecentAlwaysSelected_Click(object sender, EventArgs e) {
			this.dataSnapshot.RecentAlwaysSelected = this.mniRecentAlwaysSelected.Checked;
			this.dataSnapshotSerializer.Serialize();
			if (this.dataSnapshot.RecentAlwaysSelected == false) return;
			this.selectMostRecentException();
			this.ctxTree.Visible = true;	// keep it open
		}
		void mniRefresh_Click(object sender, EventArgs e) {
			this.flushExceptionsToOLV_switchToGuiThread();
		}
		void mniShowHeaders_Click(object sender, EventArgs e) {
			try {
				this.dataSnapshot.ShowHeaders = this.mniShowHeaders.Checked;
				this.dataSnapshotSerializer.Serialize();
				this.olvTreeExceptions	.HeaderStyle = this.dataSnapshot.ShowHeaders ? ColumnHeaderStyle.Clickable : ColumnHeaderStyle.None;
				this.olvStackTrace		.HeaderStyle = this.dataSnapshot.ShowHeaders ? ColumnHeaderStyle.Clickable : ColumnHeaderStyle.None;
				this.ctxTree.Visible = true;	// keep it open
			} catch (Exception ex) {
				Assembler.PopupException("mniShowHeader_Click", ex);
			}
		}
		void mniPopupOnEveryIncomingException_Click(object sender, EventArgs e) {
			try {
				this.dataSnapshot.PopupOnIncomingException = this.mniPopupOnIncomingException.Checked;
				this.dataSnapshotSerializer.Serialize();
				this.ctxTree.Visible = true;	// keep it open
			} catch (Exception ex) {
				Assembler.PopupException("mniPopupOnEveryIncomingException_Click", ex);
			}
		}
		void exceptionsControl_ResizeStopped(object sender, EventArgs e) {
			// I ignore 10 seconds after appRestart for setting datasnap => splitter.distance
			// I postpone all resizes to 2 seconds after the last one
			// on apprestart I'm populating it here
			// 10 seconds after appRestart, all splitters will serialize themselves if moved inside, without Resize
			// any myDockedPane / application resize will invoke both splittersMove and Resize
			// splittersMove will serialize immediately; Resize will come 2 seconds after last and serialize again
			this.PopulateDataSnapshot_initializeSplitters_afterDockContentDeserialized();
			this.dataSnapshotSerializer.Serialize();
		}
	}
}