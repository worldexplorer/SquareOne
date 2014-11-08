using System;

using Newtonsoft.Json;

namespace Sq1.Core.Charting {
	public class MultiSplitterProperties {
		[JsonProperty]	public int ManualOrder;
		[JsonProperty]	public int Distance;
		
		public MultiSplitterProperties(int manualOrder, int distance) {
			ManualOrder = manualOrder;
			Distance = distance;
		}

		// I will not delete ManualOrder because:
		// 1) Dictionary doesn't guarantee ordering so I can't fetch "prevSplitterLocationY" just by enumerating ChartSettings.MultiSplitterPropertiesByPanelName
		// 2) JsonDeserializer seems to be unable to serialize/deserialize SortedDictionary<T>
	}
}
