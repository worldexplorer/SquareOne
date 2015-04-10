using System;
using System.Collections.Generic;

namespace Sq1.Core.Optimization {
	public class SystemPerformanceRestoreAbleListEventArgs : EventArgs {
		public List<SystemPerformanceRestoreAble>	SystemPerformanceRestoreAbleList	{get; private set;}
		public string								FileName							{get; private set;}

		public SystemPerformanceRestoreAbleListEventArgs(
				List<SystemPerformanceRestoreAble> systemPerformanceRestoreAbleList
				, string fileName) {
			SystemPerformanceRestoreAbleList = systemPerformanceRestoreAbleList;
			FileName = fileName;
		}

	}
}
