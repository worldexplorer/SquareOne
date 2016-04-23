using System;
using System.Windows.Forms;

using System.Diagnostics;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Support;

using Sq1.Gui.Singletons;
using System.Threading;

namespace Sq1.Gui {
	public partial class MainForm : IStatusReporter {
	
		public void DisplayStatus(string Message) {
			//Debugger.Break();	// ACTIVATE_MY_ATTEMPTS_BELOW_TO MAKE THE LABEL DISPLAY THE DAMN MESSAGE
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.DisplayStatus(Message); });
				return;
			}
			this.lblStatus.Text = Message;
			//this.lblStatus.Invalidate();
		}

		public void DisplayConnectionStatus(ConnectionState status, string message) {
			this.DisplayStatus(status + " :: " + message);
		}

		public void PopupException(string msg, Exception exc = null, bool debuggingBreak = true) {
			//if (this.DockPanel == null) return;

			if (this.MainFormClosing_skipChartFormsRemoval_serializeExceptionsToPopupInNotepad) {
				if (msg != null) exc = new Exception(msg, exc);
				Assembler.Exceptions_duringApplicationShutdown_InsertAndSerialize(exc);
				return;
			}

			bool mainThreadGotHandle = Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete
				&& Thread.CurrentThread.ManagedThreadId == 1
				//&& base.InvokeRequired
				;

			if (mainThreadGotHandle == false && Assembler.InstanceInitialized.ExceptionsFormInstance_safelyCreated == false) {
				if (msg != null) exc = new Exception(msg, exc);
				Assembler.InstanceInitialized.Exceptions_duringApplicationStartup_Insert(exc);
				return;
			}
			
			// I_SHOULDNT_CARE_IF_ASSEMBLER_INSTANTIATED_EXCEPTIONS_FORM__koZ_ITS_IN_THIS_DLL ExceptionsForm exceptionsForm = Assembler.InstanceInitialized.StatusReporter as ExceptionsForm;
			ExceptionsForm exceptionsForm = ExceptionsForm.Instance;
			if (exceptionsForm == null) {
				string msg2 = "Assembler.InstanceInitialized.StatusReporter is not a Form";
				//MessageBox.Show(ex.Message, msg);
				throw new Exception(msg2, exc);
			}

			Assembler.InstanceInitialized.ExceptionsFormInstance_safelyCreated = true;

			if (Assembler.InstanceInitialized.Exceptions_duringApplicationStartup_beforeMainForm_gotWindowHandle.Count > 0) {
				foreach (Exception excStartup in Assembler.InstanceInitialized.Exceptions_duringApplicationStartup_beforeMainForm_gotWindowHandle) {
					exceptionsForm.ExceptionControl.InsertAsyncAutoFlush(excStartup);
				}
				Assembler.InstanceInitialized.Exceptions_duringApplicationStartup_beforeMainForm_gotWindowHandle.Clear();
			}

			try {
				exceptionsForm.PopupException(msg, exc, debuggingBreak);
			} catch (Exception ex) {
				//Assembler.PopupException(null, exc);
				#if DEBUG
				Debugger.Break();
				#endif
				throw (ex);
			}
		}
	}
}