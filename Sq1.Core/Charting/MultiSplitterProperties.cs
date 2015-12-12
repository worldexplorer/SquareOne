using System;

using Newtonsoft.Json;

namespace Sq1.Core.Charting {
	public class MultiSplitterProperties {
		[JsonProperty]	public int ManualOrder;
		[JsonProperty]	public int Distance;
		[JsonProperty]	public int SplitterHeight;
		[JsonProperty]	public int PanelHeight;
		
		public MultiSplitterProperties(int manualOrder, int distance, int splitterHeight, int panelHeight) {
			ManualOrder = manualOrder;
			Distance = distance;
			SplitterHeight = splitterHeight;
			PanelHeight = panelHeight;

			if (distance < 0) {
				string msg = "DONT_PASS_NEGATIVE_Distance";
				Assembler.PopupException(msg);
			}
			if (splitterHeight <= 0) {
				string msg = "DONT_PASS_NEGATIVE_SplitterHeight";
				Assembler.PopupException(msg);
			}
			if (panelHeight <= 0) {
				string msg = "DONT_PASS_NEGATIVE_PanelHeight";
				Assembler.PopupException(msg);
			}
		}

		public override string ToString() {
			int splitterStart = this.Distance;
			int splitterEnd = splitterStart + this.SplitterHeight;
			int panelStart = splitterEnd + 1;
			int panelEnd = panelStart + this.PanelHeight;
			string ret = "[" + this.ManualOrder + "]:[" + splitterStart + ".." + splitterEnd + "],[" + panelStart + ".." + panelEnd + "]";
			return ret;
		}

		// I will not delete ManualOrder because:
		// 1) Dictionary doesn't guarantee ordering so I can't fetch "prevSplitterLocationY" just by enumerating ChartSettings.MultiSplitterPropertiesByPanelName
		// 2) JsonDeserializer seems to be unable to serialize/deserialize SortedDictionary<T>
	}
}
