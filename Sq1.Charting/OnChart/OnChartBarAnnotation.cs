using System;
using System.Drawing;
using Sq1.Core.Charting.OnChart;

namespace Sq1.Charting.OnChart {
	public class OnChartBarAnnotation {
		public OnChartObjectOperationStatus Status;
		
		public string BarAnnotationId;
		public string BarAnnotationText;
		public Font Font;
		public Color ColorForeground;
		public Color ColorBackground;
		
		public OnChartBarAnnotation(string barAnnotationId, string barAnnotationText,
		                            Font font, Color colorForeground, Color colorBackground) {
			BarAnnotationId = barAnnotationId;
			BarAnnotationText = barAnnotationText;
			Font = font;
			ColorForeground = colorForeground;
			ColorBackground = colorBackground;
			Status = OnChartObjectOperationStatus.OnChartObjectJustCreated;
		}

		public override string ToString() {
			string ret = "Status[" + Enum.GetName(typeof(OnChartObjectOperationStatus), this.Status) + "]"
				+ " BarAnnotationText[" + BarAnnotationText + "]"
				+ " Font[" + Font + "]"
				+ " ColorFore[" + ColorForeground + "]"
				+ " ColorBack[" + ColorBackground + "]";
			return ret;
		}

	}
}
