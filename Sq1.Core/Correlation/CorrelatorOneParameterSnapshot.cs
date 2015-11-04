using System;

using Newtonsoft.Json;

namespace Sq1.Core.Correlation {
	public class CorrelatorOneParameterSnapshot {
		[JsonProperty]	public string	ParameterName;
		[JsonProperty]	public int		ParameterId;

		[JsonProperty]	public bool		MniShowAllBacktestsChecked;
		[JsonProperty]	public bool		MniShowChosenChecked;
		[JsonProperty]	public bool		MniShowDeltaChecked;

		[JsonProperty]	public bool		MniShowMomentumsAverageChecked;
		[JsonProperty]	public bool		MniShowMomentumsDispersionGlobalChecked;
		[JsonProperty]	public bool		MniShowMomentumsDispersionLocalChecked;
		[JsonProperty]	public bool		MniShowMomentumsDispersionDeltaChecked;
		[JsonProperty]	public bool		MniShowMomentumsVarianceChecked;

		[JsonProperty]	public string	OlvStateBase64;

		public CorrelatorOneParameterSnapshot(string parameterName, int parameterId = 0) {	// 0=>IndicatorParameter, 1..n=>ScriptParameter
			ParameterName						= parameterName;
			ParameterId							= parameterId;

			MniShowAllBacktestsChecked			= false;
			MniShowChosenChecked				= false;
			MniShowDeltaChecked					= false;

			MniShowMomentumsAverageChecked		= false;
			MniShowMomentumsDispersionGlobalChecked	= false;
			MniShowMomentumsDispersionLocalChecked	= false;
			MniShowMomentumsDispersionDeltaChecked	= true;
			MniShowMomentumsVarianceChecked		= false;

			OlvStateBase64 = "";	//NPE otherwize
		}
	}
}
