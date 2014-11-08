using System;
using System.Drawing;

using Sq1.Core.Charting.OnChart;

namespace Sq1.Charting.OnChart {
	public class OnChartLine {
		public OnChartObjectOperationStatus Status;
		
		public string LineId;
		public int BarLeft;
		public double PriceLeft;
		public int BarRight;
		public double PriceRight;
		public Color Color;
		public int Width;
		
		public OnChartLine(string lineId, int barLeft, double priceLeft, int barRigth, double priceRight,
				Color color, int width) {
			LineId = lineId;
			BarLeft = barLeft;
			PriceLeft = priceLeft;
			BarRight = barRigth;
			PriceRight = priceRight;
			Color = color;
			Width = width;
			Status = OnChartObjectOperationStatus.OnChartObjectJustCreated;
		}

		public override string ToString() {
			string ret = "Status[" + Enum.GetName(typeof(OnChartObjectOperationStatus), this.Status) + "]"
				+ " BarLeft[" + BarLeft + "]"
				+ " PriceLeft[" + PriceLeft + "]"
				+ " BarRight[" + BarRight + "]"
				+ " PriceRight[" + PriceRight + "]"
				+ " Color[" + Color + "]"
				+ " Width[" + Width + "]";
			return ret;
		}

	}
}
