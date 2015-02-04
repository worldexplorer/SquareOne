using System;
//using Newtonsoft.Json;
//	[DataContract]
//		[JsonIgnore]
//		[DataMember]
//		[IgnoreDataMember]

namespace Sq1.Core {
	public class AssemblerDataSnapshot {
		public string	CurrentWorkspaceName;
		public int		SplitterEventsShouldBeIgnoredSecondsAfterAppLaunch;
		
		public AssemblerDataSnapshot() {
			CurrentWorkspaceName = "Default";
			SplitterEventsShouldBeIgnoredSecondsAfterAppLaunch = 10;	// = 0 on 3.1GHz restores exactly as it was saved; ZERO_HELPS_BUT_MEANS_NO_DELAY: for Charts but not for Exceptions and Execution 
		}
	}
}
