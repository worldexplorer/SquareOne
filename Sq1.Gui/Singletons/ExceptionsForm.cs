using System;
using System.Diagnostics;
using System.Threading.Tasks;

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
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;

			#region EXPERIMENTAL
			Task t = new Task(delegate {
				base.ShowPopupSwitchToGuiThreadRunDelegateInIt(new Action(delegate {
					this.ExceptionControl.FlushListToTreeIfDockContentDeserialized();
				}));
			});
			t.ContinueWith(delegate {
				string msg2 = "TASK_THREW_ExceptionsForm.popupException()";
				Debugger.Break();
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

		internal void UpdateConnectionStatus(Core.DataTypes.ConnectionState status, int statusCode, string message) {
			throw new NotImplementedException();
		}
	}
}