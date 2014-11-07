using System;
using Newtonsoft.Json;

namespace Sq1.Reporters {
	public class PositionsDataSnapshot {
		// renaming Json-serialized variables is ok but values will be lost next time you deserialize
		// changing types of Json-serialized variables will throw next time you deserialize
		[JsonProperty]	public bool ShowEntriesExits = true;
		[JsonProperty]	public bool ShowPercentage = true;
		[JsonProperty]	public bool ShowBarsHeld = true;
		[JsonProperty]	public bool ShowMaeMfe = true;
		[JsonProperty]	public bool ShowSignals = true;
		[JsonProperty]	public bool ShowCommission = true;
		[JsonProperty]	public bool Colorify = true;
	}
}
