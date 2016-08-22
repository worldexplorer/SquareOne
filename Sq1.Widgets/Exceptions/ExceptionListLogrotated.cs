using System;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.Support;
using Sq1.Core.Serializers;

namespace Sq1.Widgets.Exceptions {
	public class ExceptionListLogrotated : ExceptionList {
		public SerializerLogrotatePeriodic<Exception> Logrotator { get; private set; }

		public ExceptionListLogrotated(string reasonToExist) : base(reasonToExist) {
			this.Logrotator = new SerializerLogrotatePeriodic<Exception>();
		}

		public void Initialize() {
			string rootPath = Assembler.InstanceInitialized.AppDataPath;
			this.Logrotator.Initialize(rootPath, "Exceptions.json", "Exceptions", null);
			List<Exception> deserialized = this.Logrotator.Deserialize();
			//base.InnerList.AddRange(this.Logrotator.EntityDeserialized);
			base.InnerList.AddRange(deserialized);
			this.Logrotator.StartSerializerThread();
		}

		public new void Clear(object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT) {
			base.Clear(owner, lockPurpose, waitMillis);
			this.Logrotator.Clear();
		}

		public new bool InsertUnique(Exception exception, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool duplicateThrowsAnError = true) {
			bool inserted = base.InsertUnique(exception, owner, lockPurpose, waitMillis, duplicateThrowsAnError);
			this.Logrotator.Insert(0, exception);
			return inserted;
		}

		public new int RemoveRange(List<Exception> exceptions, object owner, string lockPurpose, int waitMillis = ConcurrentWatchdog.TIMEOUT_DEFAULT, bool absenceThrowsAnError = true) {
			int removed_fromList = base.RemoveRange(exceptions, owner, lockPurpose, waitMillis, absenceThrowsAnError);
			int removed_fromLogrotator = this.Logrotator.RemoveRange(exceptions);
			return removed_fromLogrotator;
		}
	}
}
