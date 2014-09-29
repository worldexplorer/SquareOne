using System;
using Newtonsoft.Json.Serialization;

namespace Sq1.Charting.MultiSplit {
	public class MultiSplitterProperties {
		//[JsonProperty]
		public int ManualOrder;
		//[JsonProperty]
		public int Distance;
		
		public MultiSplitterProperties(int manualOrder, int distance) {
			ManualOrder = manualOrder;
			Distance = distance;
		}
	}
}
