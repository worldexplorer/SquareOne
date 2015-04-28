using System;

using Newtonsoft.Json;

namespace Sq1.Core.Correlation {
	public class CorrelatorOneParameterSnapshot {
		[JsonProperty]	public bool		MniShowAllBacktestsChecked;
		[JsonProperty]	public bool		MniShowChosenChecked;
		[JsonProperty]	public bool		MniShowDeltaChecked;

		[JsonProperty]	public bool		MniShowMomentumsAverageChecked;
		[JsonProperty]	public bool		MniShowMomentumsDispersionChecked;
		[JsonProperty]	public bool		MniShowMomentumsVarianceChecked;

		[JsonProperty]	public string	OlvStateBase64;

		public CorrelatorOneParameterSnapshot() {
			MniShowAllBacktestsChecked			= false;
			MniShowChosenChecked				= false;
			MniShowDeltaChecked					= false;

			MniShowMomentumsAverageChecked		= false;
			MniShowMomentumsDispersionChecked	= true;
			MniShowMomentumsVarianceChecked		= false;

			OlvStateBase64 = "";	//NPE otherwize
		}
	}
}
