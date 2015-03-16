using System;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Support {
	public interface IStatusReporter {
		void PopupException(string msg, Exception exc = null, bool debuggingBreak = true);
		void DisplayStatus(string message);
		void DisplayConnectionStatus(ConnectionState status, string message);
	}
}
