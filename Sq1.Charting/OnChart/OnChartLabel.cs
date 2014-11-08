using System;
using System.Drawing;

using Sq1.Core.Charting.OnChart;

namespace Sq1.Charting.OnChart {
	public class OnChartLabel {
		public OnChartObjectOperationStatus Status;
		
		public string LabelId;
		public string LabelText;
		public Font Font;
		public Color ColorForeground;
		public Color ColorBackground;
		
		public OnChartLabel(string labelId, string labelText,
							Font font, Color colorForeground, Color colorBackground) {
			LabelId = labelId;
			LabelText = labelText;
			Font = font;
			ColorForeground = colorForeground;
			ColorBackground = colorBackground;
			Status = OnChartObjectOperationStatus.OnChartObjectJustCreated;
		}

		public override string ToString() {
			string ret = "Status[" + Enum.GetName(typeof(OnChartObjectOperationStatus), this.Status) + "]"
				+ " LabelText[" + LabelText + "]"
				+ " Font[" + Font + "]"
				+ " ColorFore[" + ColorForeground + "]"
				+ " ColorBack[" + ColorBackground + "]";
			return ret;
		}

	}
}
