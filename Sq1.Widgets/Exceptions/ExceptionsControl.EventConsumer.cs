using System;
using System.Windows.Forms;
using System.ComponentModel;

using Sq1.Core;
using Sq1.Core.Serializers;

using Sq1.Widgets.LabeledTextBox;
using System.Collections.Generic;

namespace Sq1.Widgets.Exceptions {
	public partial class ExceptionsControl {
		void form_OnLoad(object sender, System.EventArgs e) {
			if (base.DesignMode) return;
			// IF_ONLY_YOU_WANT_TO_FLUSH_DESERIALIZED_FROM_LAST_RESTART_NIY this.FlushExceptionsToOLVIfDockContentDeserialized_inGuiThread();

			this.mniShowHeader.Checked = this.dataSnapshot.ShowHeaders;
			this.olvTreeExceptions	.HeaderStyle = this.dataSnapshot.ShowHeaders ? ColumnHeaderStyle.Clickable : ColumnHeaderStyle.None;
			this.olvStackTrace		.HeaderStyle = this.dataSnapshot.ShowHeaders ? ColumnHeaderStyle.Clickable : ColumnHeaderStyle.None;

			this.mniShowTimestamps.Checked = this.dataSnapshot.ShowTimestamps;
			this.olvcTimestamp.IsVisible = this.dataSnapshot.ShowTimestamps;
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
			this.Clear();
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
		void mniltbLogrotateLargerThan_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string msig = " //mniltbLogrotateLargerThan_UserTyped";
			try {
				float userTyped = e.UserTyped_asFloat;		// makes it red if failed to parse; "an event is a passive POCO" concept is broken here
				this.dataSnapshot.LogRotateSizeLimit_Mb = userTyped;
				this.dataSnapshotSerializer.Serialize();

				SerializerLogrotatePeriodic<Exception> logrotator = this.Exceptions.Logrotator;
				logrotator.LogRotateSizeLimit_Mb = this.dataSnapshot.LogRotateSizeLimit_Mb;
				string msg = "NEW_INTERVAL_ACTIVATED__SAVED SerializerLogrotatePeriodic<Order>.LogRotateSizeLimit_Mb=[" + logrotator.LogRotateSizeLimit_Mb + "]";
				Assembler.PopupException(msg, null, false);

				this.mniltbLogrotateLargerThan.InputFieldValue = logrotator.LogRotateSizeLimit_Mb.ToString();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				this.ctxTree.Show();
			}
		}
		void mniSerializeNow_Click(object sender, EventArgs e) {
			SerializerLogrotatePeriodic<Exception> logrotator = this.Exceptions.Logrotator;
			logrotator.HasChangesToSave = true;
			logrotator.Serialize();
		}
		void mniDeleteAllLogrotatedExceptionJsons_Click(object sender, EventArgs e) {
			try {
				SerializerLogrotatePeriodic<Exception> logrotator = this.Exceptions.Logrotator;
				logrotator.FindAndDelete_allLogrotatedFiles_butNotMainJson();
			} catch (Exception ex) {
				Assembler.PopupException(" //mniDeleteAllLogrotatedJsons_Click", ex);
			} finally {
				this.ctxTree.Show();
			}
		}
		void ctxTree_Opening(object sender, CancelEventArgs e) {
			SerializerLogrotatePeriodic<Exception> logrotator	= this.Exceptions.Logrotator;
			int		logrotatedFiles_count						= logrotator.AllLogrotatedAbsFnames_butNotMainJson_scanned.Count;
			string	logrotatedFiles_size						= logrotator.AllLogrotatedSize_butNotMainJson_scanned;
			this.mniDeleteAllLogrotatedExceptionJsons.Text		= "Delete All[" + logrotatedFiles_count + "] logrotated Exception*.json " + logrotatedFiles_size;
			this.mniDeleteAllLogrotatedExceptionJsons.Enabled	= logrotatedFiles_count > 0;
		}

		void mniShowTimestamps_Click(object sender, EventArgs e) {
			this.dataSnapshot.ShowTimestamps = mniShowTimestamps.Checked;
			this.dataSnapshotSerializer.Serialize();
			//this.olvcTime.Text = this.DataSnapshot.TreeShowExceptionTime ? "Time" : "Message";
			//this.olvTreeExceptions.RebuildAll(true);
			this.olvcTimestamp.IsVisible = this.dataSnapshot.ShowTimestamps;
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

		void mniTreeRebuildAll_Click(object sender, EventArgs e) {
			string msig = " //mniTreeRebuildAll_Click";
			try {
				this.flushExceptionsToOLV_switchToGuiThread();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				this.ctxTree.Show();
			}
		}
		void mniTreeCollapseAll_Click(object sender, EventArgs e) {
			string msig = " //mniTreeCollapseAll_Click";
			try {
				this.olvTreeExceptions.CollapseAll();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				this.ctxTree.Show();
			}
		}
		void mniTreeExpandAll_Click(object sender, EventArgs e) {
			string msig = " //mniTreeExpandAll_Click";
			try {
				this.olvTreeExceptions.ExpandAll();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			} finally {
				this.ctxTree.Show();
			}
		}

		void mniShowHeaders_Click(object sender, EventArgs e) {
			try {
				this.dataSnapshot.ShowHeaders = this.mniShowHeader.Checked;
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
		void mniDeleteExceptionsSelected_Click(object sender, EventArgs e) {
			string msig = " //mniDeleteExceptionsSelected_Click()";
			try {
				//List<Exception> exceptionsSelected = new List<Exception>(this.olvTreeExceptions.SelectedObjects);
				List<Exception> exceptionsSelected = new List<Exception>();
				foreach (int index in this.olvTreeExceptions.SelectedIndices) {
					object objectSelected = this.olvTreeExceptions.GetModelObject(index);
					Exception exceptionSelected = objectSelected as Exception;
					if (exceptionSelected == null) {
						string msg = "MUST_BE_AN_Exception_GOT [" + objectSelected.GetType().Name + "]";
						Assembler.PopupException(msg);
						continue;
					}
					bool thisIs_innerException = this.Exceptions.Contains(exceptionSelected, this, msig) == false;
					if (thisIs_innerException) continue;
					exceptionsSelected.Add(exceptionSelected);
				}
				int removedCount = this.Exceptions.RemoveRange(exceptionsSelected, this, msig);
				this.olvTreeExceptions.SetObjects(this.Exceptions.SafeCopy(this, msig));		// it's slow but thread-safe and you set a delay 200ms EXACTLY because it's all expensive
				this.olvTreeExceptions.DeselectAll();	// otherwize selected rows "slide" down to the ones I never selected 
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			}
		}
	}
}