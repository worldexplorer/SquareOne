using System;
//using Newtonsoft.Json;
//	[DataContract]
//		[JsonIgnore]
//		[DataMember]
//		[IgnoreDataMember]

namespace Sq1.Core {
	public class AssemblerDataSnapshot {
		public string CurrentWorkspaceName;
		
		public AssemblerDataSnapshot() {
			this.CurrentWorkspaceName = "Default";
		}
	}
}
