using System;
using System.Collections.Generic;
using System.Drawing;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Indicators.HelperSeries {
	public class DataSeriesTimeBasedColorified : DataSeriesTimeBased {
		public Color Color;
		public SortedList<DateTime, Bar> ParentBarsByDate;

		public DataSeriesTimeBasedColorified(BarScaleInterval scaleInterval, string description, Color color) : base(scaleInterval, description) {
			Color = color;
			ParentBarsByDate = new SortedList<DateTime, Bar>();
		}
		
		public void AppendWithParentBar(DateTime dateTimeOpen, double value, Bar parentBar) {
			base.Append(dateTimeOpen, value);
			this.ParentBarsByDate.Add(dateTimeOpen, parentBar);
			//Debugger.Break();
		}
		
		public override void Clear() {
			base.Clear();
			this.ParentBarsByDate.Clear();
		}
	}
}
