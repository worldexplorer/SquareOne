using System;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Support {
	public interface IStatusReporter {
		void DisplayStatus(string Message);
		void DisplayStatusConnected(bool reconnect);
		void DisplayStatusConnected();
		void DisplayStatusDisconnected();
		void UpdateConnectionStatus(ConnectionState status, int StatusCode, string Message);
		void PopupException(string msg, Exception exc = null, bool debuggingBreak = true);
	}
}
