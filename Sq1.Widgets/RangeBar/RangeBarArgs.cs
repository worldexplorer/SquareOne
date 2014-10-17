using System;

namespace Sq1.Widgets.RangeBar {
	public class RangeArgs<T> : EventArgs {
		
		public T RangeMin;
		public T RangeMax;
		public T ValueMin;
		public T ValueMax;
		public T ValueMouseOver;

		public string RangeMinFormatted;
		public string RangeMaxFormatted;
		public string ValueMinFormatted;
		public string ValueMaxFormatted;
		public string ValueMouseOverFormatted;
		
		public RangeArgs(T rangeMin, string rangeMinFormatted,
						 T rangeMax, string rangeMaxFormatted,
						 T valueMin, string valueMinFormatted,
						 T valueMax, string valueMaxFormatted,
						 T valueMouseOver, string valueMouseOverFormatted) {
			
			this.RangeMin = rangeMin;
			this.RangeMinFormatted = rangeMinFormatted;

			this.RangeMax = rangeMax;
			this.RangeMaxFormatted = rangeMaxFormatted;

			this.ValueMin = valueMin;
			this.ValueMinFormatted = valueMinFormatted;

			this.ValueMax = valueMax;
			this.ValueMaxFormatted = valueMaxFormatted;

			this.ValueMouseOver = valueMouseOver;
			this.ValueMouseOverFormatted = valueMouseOverFormatted;
		}
	}
}
