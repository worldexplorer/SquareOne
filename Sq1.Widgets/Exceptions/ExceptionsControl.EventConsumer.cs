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
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			//v2 HACK http://stackoverflow.com/questions/10161088/get-elapsed-time-since-application-start-in-c-sharp
			//try {
			//	TimeSpan sinceApplicationStart = DateTime.Now - Process.GetCurrentProcess().StartTime;
			//	if (sinceApplicationStart.Seconds <= 10) return;
			//} catch (Exception ex) {
			//	Assembler.PopupException("SEEMS_TO_BE_UNSUPPORTED_Process.GetCurrentProcess()", ex);
			//}
			//v3
			//NOT_UNDER_WINDOWS if (Assembler.InstanceInitialized.SplitterEventsAreAllowedNsecAfterLaunchHopingInitialInnerDockResizingIsFinished == false) return;
			if (this.dataSnapshot.SplitDistanceVertical == this.splitContainerVertical.SplitterDistance) return;
			this.dataSnapshot.SplitDistanceVertical = this.splitContainerVertical.SplitterDistance;
			this.dataSnapshotSerializer.Serialize();
		}
		
		void splitContainerHorizontal_SplitterMoved(object sender, SplitterEventArgs e) {
			if (this.dataSnapshot == null) return;	// there is no DataSnapshot deserialized in InitializeComponents()
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;
			//v1 WHATT??? BECAUSE_MESSAGE_DELIVERY_IS_ASYNC_IM_FIRED_AFTER_IT'S_ALREADY_TRUE
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			//v2 HACK http://stackoverflow.com/questions/10161088/get-elapsed-time-since-application-start-in-c-sharp
			//try {
			//	TimeSpan sinceApplicationStart = DateTime.Now - Process.GetCurrentProcess().StartTime;
			//	if (sinceApplicationStart.Seconds <= 10) return;
			//} catch (Exception ex) {
			//	Assembler.PopupException("SEEMS_TO_BE_UNSUPPORTED_Process.GetCurrentProcess()", ex);
			//}
			//v3 NOT_UNDER_WINDOWS if (Assembler.InstanceInitialized.SplitterEventsAreAllowedNsecAfterLaunchHopingInitialInnerDockResizingIsFinished == false) return;
			if (this.dataSnapshot.SplitDistanceHorizontal == this.splitContainerHorizontal.SplitterDistance) return;
			this.dataSnapshot.SplitDistanceHorizontal = this.splitContainerHorizontal.SplitterDistance;
			this.dataSnapshotSerializer.Serialize();
		}
		void mniCopyStackPosition_Click(object sender, EventArgs e) {
			//this.P
		}
		void mniltbDelay_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			MenuItemLabeledTextBox mnilbDelay = sender as MenuItemLabeledTextBox;
			string typed = e.StringUserTyped;
			int typedMsec = this.dataSnapshot.TreeRefreshDelayMsec;
			bool parsed = Int32.TryParse(typed, out typedMsec);
			if (parsed == false) {
				mnilbDelay.InputFieldValue = this.dataSnapshot.TreeRefreshDelayMsec.ToString();
				mnilbDelay.TextRed = true;
				return;
			}
			this.dataSnapshot.TreeRefreshDelayMsec = typedMsec;
			this.dataSnapshotSerializer.Serialize();
			this.timedTask_flushingToGui.Delay = this.dataSnapshot.TreeRefreshDelayMsec;
			mnilbDelay.TextRed = false;
			e.RootHandlerShouldCloseParentContextMenuStrip = true;
			this.populateWindowsTitle();
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
		void exceptionsControl_VisibleChanged(object sender, EventArgs e) {
			//I_WANTED_TO_CATCH_DockContent.AutoHiddenShownByMouseOver(),AutoHiddenNowDocked_BUT_GOT_WINFORMS_GARBAGE__WILL_USE_IT_THOUGH
			//DockContentImproved parentAsDockContentImproved = this.parentAsDockContentImproved_nullUnsafe;
			//if (parentAsDockContentImproved != null) {
			//	if (parentAsDockContentImproved.IsCoveredOrAutoHidden) return;
			//}
			////if (base.Visible == false) return; 
			string msg = "I_WAS_NOT_FLUSHING_TO_GUI_UNLESS_VISIBLE";
			////STACK_OVERFLOW_CONGRATS Assembler.PopupException(msg, null, false);
			//this.insertTo_exceptionsNotFlushedYet_willReportIfBlocking(new Exception(msg));
			this.flushExceptionsToOLV_switchToGuiThread();
		}
	}
}