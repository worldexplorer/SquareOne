using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Widgets.Exceptions {
	public partial class ExceptionsControl {
		void form_OnLoad(object sender, System.EventArgs e) {
			if (base.DesignMode) return;
			this.FlushListToTreeIfDockContentDeserialized_inGuiThread();
		}
		void tree_SelectedIndexChanged(object sender, EventArgs e) {
			if (this.treeExceptions.SelectedObject == null) {
				this.txtExceptionMessage.Text = "";
				return;
			}
			this.txtExceptionMessage.Text = this.exceptionSelectedInTree.Message;
			this.displayStackTrace();
		}
		void mniClear_Click(object sender, EventArgs e) {
			this.Exceptions.Clear();
			this.FlushListToTreeIfDockContentDeserialized_inGuiThread();
		}
		void mniCopy_Click(object sender, EventArgs e) {
			this.CopyExceptionDataToClipboard();
		}
		void splitContainerVertical_SplitterMoved(object sender, SplitterEventArgs e) {
			if (this.DataSnapshot == null) return;	// there is no DataSnapshot deserialized in InitializeComponents()
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;
			//v1 BECAUSE_MESSAGE_DELIVERY_IS_ASYNC_IM_FIRED_AFTER_IT'S_ALREADY_TRUE
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			//v2 HACK http://stackoverflow.com/questions/10161088/get-elapsed-time-since-application-start-in-c-sharp
			//try {
			//	TimeSpan sinceApplicationStart = DateTime.Now - Process.GetCurrentProcess().StartTime;
			//	if (sinceApplicationStart.Seconds <= 10) return;
			//} catch (Exception ex) {
			//	Assembler.PopupException("SEEMS_TO_BE_UNSUPPORTED_Process.GetCurrentProcess()", ex);
			//}
			//v3
			if (Assembler.InstanceInitialized.SplitterEventsAreAllowedAssumingInitialInnerDockResizingFinished == false) return;
			if (this.DataSnapshot.SplitDistanceVertical == this.splitContainerVertical.SplitterDistance) return;
			this.DataSnapshot.SplitDistanceVertical = this.splitContainerVertical.SplitterDistance;
			this.DataSnapshotSerializer.Serialize();
		}
		
		void SplitContainerHorizontal_SplitterMoved(object sender, SplitterEventArgs e) {
			if (this.DataSnapshot == null) return;	// there is no DataSnapshot deserialized in InitializeComponents()
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;
			//v1 BECAUSE_MESSAGE_DELIVERY_IS_ASYNC_IM_FIRED_AFTER_IT'S_ALREADY_TRUE
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			//v2 HACK http://stackoverflow.com/questions/10161088/get-elapsed-time-since-application-start-in-c-sharp
			//try {
			//	TimeSpan sinceApplicationStart = DateTime.Now - Process.GetCurrentProcess().StartTime;
			//	if (sinceApplicationStart.Seconds <= 10) return;
			//} catch (Exception ex) {
			//	Assembler.PopupException("SEEMS_TO_BE_UNSUPPORTED_Process.GetCurrentProcess()", ex);
			//}
			//v3
			if (Assembler.InstanceInitialized.SplitterEventsAreAllowedAssumingInitialInnerDockResizingFinished == false) return;
			if (this.DataSnapshot.SplitDistanceHorizontal == this.splitContainerHorizontal.SplitterDistance) return;
			this.DataSnapshot.SplitDistanceHorizontal = this.splitContainerHorizontal.SplitterDistance;
			this.DataSnapshotSerializer.Serialize();
		}
		void mniCopyStackPosition_Click(object sender, EventArgs e) {
			//this.P
		}
		void mniltbDelay_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			MenuItemLabeledTextBox mnilbDelay = sender as MenuItemLabeledTextBox;
			string typed = e.StringUserTyped;
			int typedMsec = this.DataSnapshot.TreeRefreshDelayMsec;
			bool parsed = Int32.TryParse(typed, out typedMsec);
			if (parsed == false) {
				mnilbDelay.InputFieldValue = this.DataSnapshot.TreeRefreshDelayMsec.ToString();
				mnilbDelay.TextRed = true;
				return;
			}
			this.DataSnapshot.TreeRefreshDelayMsec = typedMsec;
			mnilbDelay.TextRed = false;
			this.DataSnapshotSerializer.Serialize();
			e.RootHandlerShouldCloseParentContextMenuStrip = true;
		}
		void mniTreeShowsTimesInsteadOfMessages_Click(object sender, EventArgs e) {
			this.DataSnapshot.TreeShowExceptionTime = mniTreeShowExceptionTime.Checked;
			this.DataSnapshotSerializer.Serialize();
			this.olvTime.Text = this.DataSnapshot.TreeShowExceptionTime ? "Time" : "Message";
			this.treeExceptions.RebuildAll(true);
		}
	}
}