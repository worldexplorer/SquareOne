using System;

namespace Sq1.Core.Execution {
	public class AlertEventArgs : EventArgs {
		public Alert Alert { get; protected set; }
		public AlertEventArgs(Alert alert) {
			this.Alert = alert;
		}
	}
}
