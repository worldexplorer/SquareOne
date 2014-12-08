using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

using Sq1.Core;

namespace Sq1.Gui.Singletons {
	public partial class ExceptionsForm : DockContentSingleton<ExceptionsForm> {
		public ExceptionsForm() : base() {
			this.InitializeComponent();
			
			// below I want to avoid {Assembler.instance=null}-related exception in DesignMode
			if (base.DesignMode) {
				Debugger.Break();
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
			this.ExceptionControl.InsertException(ex);

			#region EXPERIMENTAL
			Task t = new Task(delegate {
				base.ShowPopupSwitchToGuiThreadRunDelegateInIt(new Action(delegate {
					this.ExceptionControl.FlushListToTreeIfDockContentDeserialized();
				}));
			});
			t.ContinueWith(delegate {
				string msg2 = "TASK_THREW_ExceptionsForm.popupException()";
				//Debugger.Break();
				//Assembler.PopupException(msg2, t.Exception);
			}, TaskContinuationOptions.OnlyOnFaulted);
			t.Start();
			#endregion
		}
		protected override void OnLoad(EventArgs e) {
			foreach (Exception beforeFormInstantiated in Assembler.InstanceInitialized.ExceptionsWhileInstantiating) {
				this.PopupException(null, beforeFormInstantiated, false);
			}
		}
	}
}