using System;
//using Newtonsoft.Json;
//	[DataContract]
//		[JsonIgnore]
//		[DataMember]
//		[IgnoreDataMember]

namespace Sq1.Core {
	public class AssemblerDataSnapshot {
		public string	WorkspaceCurrentlyLoaded;
		public int		SplitterEventsShouldBeIgnoredSecondsAfterAppLaunch;
		
		public AssemblerDataSnapshot() {
			WorkspaceCurrentlyLoaded = "Default";
			SplitterEventsShouldBeIgnoredSecondsAfterAppLaunch = 10;	// = 0 on 3.1GHz restores exactly as it was saved; ZERO_HELPS_BUT_MEANS_NO_DELAY: for Charts but not for Exceptions and Execution 
		}
	}
}
