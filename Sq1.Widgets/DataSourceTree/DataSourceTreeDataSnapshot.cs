using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Sq1.Widgets.DataSourcesTree {
	public class DataSourceTreeDataSnapshot {
		[JsonProperty]	public List<string> DataSourceFoldersExpanded = new List<string>();
		[JsonProperty]	public bool ShowHeader = false;
		[JsonProperty]	public bool ShowSearchBar = false;
	}
}
