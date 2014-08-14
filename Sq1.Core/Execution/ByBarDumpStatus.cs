using System;
using System.Collections.Generic;
using System.Text;

namespace Sq1.Core.Execution {
	public enum ByBarDumpStatus {
		BarAlreadyContainedTheAlertToAdd,
		OneNewAlertAddedForNewBarInHistory,
		SequentialAlertAddedForExistingBarInHistory
	}
}
