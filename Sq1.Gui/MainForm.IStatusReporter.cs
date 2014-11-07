using System;
using System.Collections.Generic;

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
//			if (ExceptionsForm.Instance.Visible == false) {
//				ExceptionsForm.Instance.ExceptionControl.InsertException(exc);
//				//throw (exc);
//				return;
//			}
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
				ExceptionsForm.Instance.PopupException(msg, exc, debuggingBreak);
			} catch (Exception ex) {
				//Assembler.PopupException(null, exc);
				throw (ex);
			}
		}
	}
}