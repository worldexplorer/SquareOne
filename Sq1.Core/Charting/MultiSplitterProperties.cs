using System;

using Newtonsoft.Json;

namespace Sq1.Core.Charting {
	public class MultiSplitterProperties {
		[JsonProperty]	public int ManualOrder;
		[JsonProperty]	public int Distance;
		[JsonProperty]	public int DistanceMinimal;
		
		public MultiSplitterProperties(int manualOrder, int distance, int distanceMinimal = 15) {
			ManualOrder = manualOrder;
			Distance = distance;
			DistanceMinimal = distanceMinimal;
		}

		// I will not delete ManualOrder because:
		// 1) Dictionary doesn't guarantee ordering so I can't fetch "prevSplitterLocationY" just by enumerating ChartSettings.MultiSplitterPropertiesByPanelName
		// 2) JsonDeserializer seems to be unable to serialize/deserialize SortedDictionary<T>
	}
}
