using System;
using System.Diagnostics;
using System.Windows.Forms;

using Sq1.Core;

namespace Sq1.Gui.Singletons {
	public partial class ExceptionsForm : DockContentSingleton<ExceptionsForm> {
		public ExceptionsForm() : base() {
			this.InitializeComponent();
			
			// below I want to avoid {Assembler.instance=null}-related exception in DesignMode
			if (base.DesignMode) {
				throw new Exception("I doubt that a Form.ctor() could ever have base.DesignMode=true" +
					"; base() has not been informed yet that the form IS in DesignMode, right?...");
				//return; 
			}
//			if (ExceptionsForm.instanceBeingConstructedUseForDesignMode == false) {
//				string msg = "while in DesignMode, trying to avoid"
//					+ "	1) The variable 'ExceptionControl' is either undeclared or was never assigned."
//					+ "	2) System.Exception: Assembler.instance=null; use Assembler.InstanceUninitialized.Initialize(MainForm); this singleton requires IStatusReporter to get fully initialized"
//					;
//				throw new Exception(msg);
//			}
			
			this.ExceptionControl.Initialize();
		}
		public void PopupException(string msg, Exception ex = null, bool debuggingBreak = true) {
			#if DEBUG
			if (debuggingBreak) {
				Debugger.Break();
			}
			#endif

			if (msg != null) ex = new Exception(msg, ex);
			this.popupException(ex);
		}
		void popupException(Exception exception) {
			if (base.IsDisposed) return;
			if (base.InvokeRequired == true) {
				base.BeginInvoke((MethodInvoker)delegate { this.popupException(exception); });
				return;
			}
			this.ExceptionControl.InsertException(exception);
			base.ShowPopup();
		}
		protected override void OnLoad(EventArgs e) {
			foreach (Exception beforeFormInstantiated in Assembler.InstanceInitialized.ExceptionsWhileInstantiating) {
				this.PopupException(null, beforeFormInstantiated, false);
			}
		}
	}
}