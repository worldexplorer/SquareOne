using System;
using System.Collections.Generic;

using Sq1.Core.Support;

namespace Sq1.Widgets.Exceptions {
	public class ExceptionList : ConcurrentListFiltered<Exception> {
		public ExceptionList(string reasonToExist) : base(reasonToExist) {
		}

		public new bool AppendUnique(Exception exception, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			return base.AppendUnique(exception, owner, lockPurpose, waitMillis, duplicateThrowsAnError);
		}

		public new bool InsertUnique(Exception exception, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			return base.InsertUnique(exception, owner, lockPurpose, waitMillis, duplicateThrowsAnError);
		}

		public new int RemoveRange(List<Exception> exceptions, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenceThrowsAnError = true) {
			return base.RemoveRange(exceptions, owner, lockPurpose, waitMillis, absenceThrowsAnError);
		}
	}
}
