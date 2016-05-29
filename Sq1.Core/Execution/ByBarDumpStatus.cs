using System;

namespace Sq1.Core.Execution {
	public enum ByBarDumpStatus {
		BarAlreadyContained_alertYouAdd,
		OneNewAlertAdded_forNewBar,
		SequentialAlertAdded_forExistingBar
	}
}
