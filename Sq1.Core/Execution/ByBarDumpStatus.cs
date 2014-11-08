using System;

namespace Sq1.Core.Execution {
	public enum ByBarDumpStatus {
		BarAlreadyContainedTheAlertToAdd,
		OneNewAlertAddedForNewBarInHistory,
		SequentialAlertAddedForExistingBarInHistory
	}
}
