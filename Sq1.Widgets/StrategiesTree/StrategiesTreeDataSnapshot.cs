using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Sq1.Widgets.StrategiesTree {
	public class StrategiesTreeDataSnapshot {
		[JsonProperty]	public List<string> StrategyFoldersExpanded = new List<string>();
		[JsonProperty]	public bool ShowHeader = false;
		[JsonProperty]	public bool ShowSearchBar = false;
	}
}
