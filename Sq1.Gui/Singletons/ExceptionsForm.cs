using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

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
		public void PopupException(Exception exception) {
			if (base.IsDisposed) return;
			if (base.InvokeRequired == true) {
				base.BeginInvoke((MethodInvoker)delegate { this.PopupException(exception); });
				return;
			}
			this.ExceptionControl.InsertException(exception);
			// doesn't help to show up an Auto-Hidden-after-creation THIS
			//base.Show();
			//base.BringToFront();

			//base.Pane=null after RestoreXmlLayout()
			if (base.Pane == null) return;
			if (base.Pane.IsAutoHide) {
				// why DockHelper.ToggleAutoHideState() and DockHandler.SetDockState() are internal???...
				DockState newState = DockHelper.ToggleAutoHideState(base.Pane.DockState);
				base.Pane.SetDockState(newState);
			}
			// added if(base.IsHidden) to stop Pane.Activate() steal focus during keyUp/keyDown in ExecutionTree when this generates Exceptions
			if (base.IsHidden) base.Pane.Activate();
			// removes focus from other forms; makes ExecutionForm.SelectedRow blue=>gray
			// base.Activate();
		}
//		public override void VisibleChanged(object sender, EventArgs e) {
//			int a = 1;
//		}
	}
}