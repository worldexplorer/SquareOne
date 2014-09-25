using System;
using System.Drawing;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Indicators.HelperSeries {
	public class DataSeriesTimeBasedColorified : DataSeriesTimeBased {
		public Color Color;
		
		public DataSeriesTimeBasedColorified(BarScaleInterval scaleInterval, Color color) : base(scaleInterval) {
			Color = color;
		}
	}
}
