using System;

namespace Sq1.Widgets.RangeBar {
	public class RangeBarInteger : RangeBar<int> {
		
		public float RangeWidth { get {return base.RangeMax - base.RangeMin; } } 	//without (float) division of two ints is an int !!! (zero)

		public override float PercentageFromValue(int value) {
			//without (float) division of two ints is an int !!! (zero)
			return (value - base.RangeMin) / (float)this.RangeWidth;
		}

		public override int ValueFromPercentage(float percentage0to1) {
			return this.RoundInt(this.RangeMin + (int) (this.RangeWidth * percentage0to1));
		}
		
		public override void checkThrowOnPaint() {
			if (base.RangeMin > base.RangeMax) {
				string msg = "RangeBar.RangeMin[" + base.RangeMin + "] > RangeBar.RangeMax[" + base.RangeMax + "]";
				throw new Exception(msg);
			}
			if (base.ValueMin > base.ValueMax) {
				string msg = "RangeBar.ValueMin[" + base.ValueMin + "] > RangeBar.ValueMax[" + base.ValueMax + "]";
				throw new Exception(msg);
			}
			if (base.RangeMin > base.ValueMin) {
				string msg = "RangeBar.RangeMin[" + base.RangeMin + "] > RangeBar.ValueMin[" + base.ValueMin + "]";
				throw new Exception(msg);
			}
			if (base.ValueMax > base.RangeMax) {
				string msg = "RangeBar.ValueMax[" + base.ValueMax + "] > RangeBar.RangeMax[" + base.RangeMax + "]";
				throw new Exception(msg);
			}
		}
		
		public RangeBarInteger() {
			base.RangeMin = 100;
			base.RangeMax = 500;
			base.ValueMin = 220;
			base.ValueMax = 380;
		}
	}
}
