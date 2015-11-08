using System;

using Newtonsoft.Json;

namespace Sq1.Core.Sequencing {
	public class SequencerDataSnapshot {
		[JsonProperty]	public	bool	ShowOnlyCorrelatorChosenBacktests;		//CANDIDATE_FOR_SMALLER_JSON_FILE { get; private set; }
		[JsonProperty]	public	bool	StatsAndHistoryCollapsed;				//CANDIDATE_FOR_SMALLER_JSON_FILE { get; private set; }

		public SequencerDataSnapshot() {
			ShowOnlyCorrelatorChosenBacktests = false;
			StatsAndHistoryCollapsed = false;
		}
	}
}
