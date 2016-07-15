using System;
using System.Collections.Generic;

using Sq1.Core.Support;

namespace Sq1.Widgets.Exceptions {
	public class ExceptionList : ConcurrentList<Exception> {
		public ExceptionList(string reasonToExist) : base(reasonToExist) {
		}

		public new bool AppendUnique(Exception exception, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			return base.AppendUnique(exception, owner, lockPurpose, waitMillis, duplicateThrowsAnError);
		}

		public new bool InsertUnique(int indexToInsertAt, Exception exception, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			return base.InsertUnique(indexToInsertAt, exception, owner, lockPurpose, waitMillis, duplicateThrowsAnError);
		}

	}
}
