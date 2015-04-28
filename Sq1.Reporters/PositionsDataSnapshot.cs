using System;

using Newtonsoft.Json;

namespace Sq1.Reporters {
	public class PositionsDataSnapshot {
		// renaming Json-serialized variables is ok but values will be lost next time you deserialize
		// changing types of Json-serialized variables will throw next time you deserialize
		[JsonProperty]	public bool		Colorify					= true;
		[JsonProperty]	public string	PositionsOlvStateBase64		= "";	// NPE otherwize
	}
}
