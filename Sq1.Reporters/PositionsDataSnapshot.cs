using System;
using System.Runtime.Serialization;

namespace Sq1.Reporters {
	[DataContract] 
	public class PositionsDataSnapshot {
		// renaming Json-serialized variables is ok but values will be lost next time you deserialize
		// changing types of Json-serialized variables will throw next time you deserialize
		[DataMember] public bool ShowEntriesExits = true;
		[DataMember] public bool ShowPercentage = true;
		[DataMember] public bool ShowBarsHeld = true;
		[DataMember] public bool ShowMaeMfe = true;
		[DataMember] public bool ShowSignals = true;
		[DataMember] public bool ShowCommission = true;
		[DataMember] public bool Colorify = true;
	}
}
