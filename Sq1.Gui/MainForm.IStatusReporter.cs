using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Serializers;
using Sq1.Core.Support;
using Sq1.Gui.Singletons;

namespace Sq1.Gui {
	public partial class MainForm : IStatusReporter {
	
		public void DisplayStatus(string Message) {
			this.lblStatus.Text = Message;
		}

		public void DisplayStatusConnected(bool reconnect) {
			ExceptionsForm.Instance.PopupException(null, new NotImplementedException("DisplayStatusConnected"));
		}

		public void DisplayStatusConnected() {
			ExceptionsForm.Instance.PopupException(null, new NotImplementedException("DisplayStatusConnected"));
		}

		public void DisplayStatusDisconnected() {
			ExceptionsForm.Instance.PopupException(null, new NotImplementedException("DisplayStatusDisconnected"));
		}

		public void UpdateConnectionStatus(ConnectionState status, int StatusCode, string Message) {
			//ExceptionsForm.Instance.PopupException(new NotImplementedException("UpdateConnectionStatus"));
		}

		List<Exception> ExceptionsDuringApplicationShutdown;
		Serializer<List<Exception>> ExceptionsDuringApplicationShutdownSerializer;
		public void PopupException(string msg, Exception exc = null, bool debuggingBreak = true) {
			//if (this.DockPanel == null) return;
			
//			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) {
//				#if DEBUG
//				if (debuggingBreak) {
//					Debugger.Break();
//				}
//				#endif
//				
//				string msg2 = "TRYING_TO_AVOID [IM_NOT_FIXING_WINDOW_HANDLE_NOT_YET_CREATED]";
//				ExceptionsForm.Instance.ExceptionControl.InsertException(new Exception(msg2, exc));
//
//				//ExceptionsForm.Instance.ExceptionControl.InsertException(exc);
//				return;
//			}

			
			try {
				//v2-SHARP_DEVELOP_THROWS_WHEN_TRYING_TO_POPUP_EXCEPTION_FROM_QUIK_TERMINAL_MOCK_THREAD
				// this is gonna throw from a non-GUI thread, right?!... (moved to MainForm.PopupException() with base.BeginInvoke() as first step)
				// USED_WHEN_base.InvokeRequired_THROWS_#D
				if (base.InvokeRequired == true) {
				//string currentThreadName = Thread.CurrentThread.Name;
				//if (currentThreadName != GUI_THREAD_NAME) {
					#if DEBUG
					if (debuggingBreak) {
						string note = "BREAKING_EARLIER__BECAUSE_YOU_WILL_LOOSE_CALLSTACK_SOON";
						Debugger.Break();
					}
					#endif
					base.BeginInvoke((MethodInvoker)delegate { this.PopupException(msg, exc, false); });
					return;
				}
			} catch (Exception ex) {
				#if DEBUG
				if (debuggingBreak) {
					Debugger.Break();
				}
				#endif
				
				string msg2 = "IM_NOT_FIXING_WINDOW_HANDLE_NOT_YET_CREATED WHAT_MIGHT_POSSIBLY_BE_WRONG_WITH_base.InvokeRequired()";
				ExceptionsForm.Instance.ExceptionControl.InsertException(new Exception(msg2, ex));
				ExceptionsForm.Instance.ExceptionControl.InsertException(exc);
				return;
			}

			
			// I_SHOULDNT_CARE_IF_ASSEMBLER_INSTANTIATED_EXCEPTIONS_FORM__koZ_ITS_IN_THIS_DLL ExceptionsForm exceptionsForm = Assembler.InstanceInitialized.StatusReporter as ExceptionsForm;
			ExceptionsForm exceptionsForm = ExceptionsForm.Instance;
			if (exceptionsForm == null) {
				string msg2 = "Assembler.InstanceInitialized.StatusReporter is not a Form";
				//MessageBox.Show(ex.Message, msg);
				throw new Exception(msg2, exc);
			}
			//SHARP_DEVELOP_THROWS_WHEN_TRYING_TO_POPUP_EXCEPTION_FROM_QUIK_TERMINAL_MOCK_THREAD if I PopupException from a BrokerProvider thread, exceptionsForm.Visible and others should throw
//			// "you are trying to access WinForms.Component's properties within the thread", right?...
//			if (exceptionsForm.Visible == false) {
////				//v1 throw (exc);
////				//string msg2 = "Assembler.InstanceInitialized.StatusReporter is a Form but Visible=false";
////				//v2 MessageBox.Show(ex.Message, msg);
////				//v3 throw new Exception(msg2, ex);
//				string msg2 = "ExceptionForm.Visible=false"
//					+ "; but .PopupException() will insert your exception into the tree and display OnLoad()";
//			}

			
//			if (ExceptionsForm.Instance.Visible == false) {
//				ExceptionsForm.Instance.ExceptionControl.InsertException(exc);
//				//throw (exc);
//				return;
//			}

			if (exceptionsForm.Visible == false && Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == true) {
				exceptionsForm.Visible = true;
			}

			if (this.MainFormClosingSkipChartFormsRemoval) {
				if (this.ExceptionsDuringApplicationShutdown == null) {
					this.ExceptionsDuringApplicationShutdown = new List<Exception>();
					this.ExceptionsDuringApplicationShutdownSerializer = new Serializer<List<Exception>>();
					string now = Assembler.FormattedLongFilename(DateTime.Now);
					bool createdNewFile = this.ExceptionsDuringApplicationShutdownSerializer.Initialize(Assembler.InstanceInitialized.AppDataPath,
						"ExceptionsDuringApplicationShutdown-" + now + ".json", "Exceptions", null, true, true);
					this.ExceptionsDuringApplicationShutdown = this.ExceptionsDuringApplicationShutdownSerializer.Deserialize(); 
				}
				this.ExceptionsDuringApplicationShutdown.Insert(0, exc);
				this.ExceptionsDuringApplicationShutdownSerializer.Serialize();
			}
			try {
				exceptionsForm.PopupException(msg, exc, debuggingBreak);
			} catch (Exception ex) {
				//Assembler.PopupException(null, exc);
				throw (ex);
			}
		}
	}
}