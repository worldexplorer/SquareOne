using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
