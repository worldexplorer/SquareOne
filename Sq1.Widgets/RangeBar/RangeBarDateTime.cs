using System;

using Sq1.Core.DataTypes;

namespace Sq1.Widgets.RangeBar {
	public class RangeBarDateTime : RangeBarWithGraph<DateTime> {
		public TimeSpan RangeWidth { get {return base.RangeMax - base.RangeMin; } }

		public override DateTime ValueFromPercentage(float percentage0to1) {
			//without (long)() you round the [0...1] RangePercentage up to zero => wrong output
			DateTime ret = new DateTime(base.RangeMin.Ticks + (long)(this.RangeWidth.Ticks * percentage0to1));
			if (ret < this.RangeMin) ret = this.RangeMin; 
			if (ret > this.RangeMax) ret = this.RangeMax; 
			return ret;
		}

		public override float PercentageFromValue(DateTime value) {
			//without (float) division of two ints is an int !!! (zero)
			return (value - base.RangeMin).Ticks / (float)this.RangeWidth.Ticks;
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

		public RangeBarDateTime() : base() {
			base.RangeMin = DateTime.Parse("12-May-2010");
			base.RangeMax = DateTime.Parse("12-May-2013");
			base.ValueMin = DateTime.Parse("12-May-2011");
			base.ValueMax = DateTime.Parse("12-May-2012");
			base.valueFormat = "dd-MMM-yy";
			// MANDATORY IN THE RangeBarWithGraph<T> CHILD
			base.GraphBuilder = new AsyncGraphBuilderBars(this, 0);
		}
		
		public void Reset() {
			//base.GraphBuilder.Initialize(new BarsUnscaled(null, "RESETTING_RangeBarDateTime"));
			base.GraphBuilder.Initialize(null);
			//DOESNT_HELP_TO_DRAW_FIRST_TIME_AFTER_INITIALIZED this.Invalidate();
		}

		public void Initialize(BarsUnscaled barsAllAvailable, BarsUnscaled barsActivated = null) {
			if (barsActivated == null) barsActivated = barsAllAvailable; 
			if (barsAllAvailable.BarFirst == null) {
				string msg = "barsAllAvailable.FirstStaticBar=null; barsAllAvailable[" + barsAllAvailable + "]";
				throw new Exception(msg);
			}
			if (barsActivated.BarFirst == null) {
				string msg = "barsActivated.FirstStaticBar=null; barsActivated[" + barsActivated + "]";
				throw new Exception(msg);
			}
			//DateTime lastBarAvailable = (barsAllAvailable.StreamingBarSafeClone.DateTimeOpen);
			//DateTime lastBarActivated = (barsActivated.StreamingBarSafeClone.DateTimeOpen);
			DateTime lastBarAvailable = (barsAllAvailable.BarLast.DateTimeOpen);
			DateTime lastBarActivated = (barsActivated.BarLast.DateTimeOpen);
			if (lastBarAvailable < lastBarActivated) {
				string msg = "lastBarAvailable[" + lastBarAvailable + "] < lastBarActivated[" + lastBarActivated + "];"
					+ " available[" + barsAllAvailable + "], activated[" + barsActivated + "]";
				throw new Exception(msg);
			}

			base.RangeMin = barsAllAvailable.BarFirst.DateTimeOpen;
			base.RangeMax = lastBarAvailable;
			base.ValueMin = barsActivated.BarFirst.DateTimeOpen;
			base.ValueMax = lastBarActivated;

			base.GraphBuilder.Initialize(barsAllAvailable);
			//DOESNT_HELP_TO_DRAW_FIRST_TIME_AFTER_INITIALIZED this.Invalidate();
		}

	}
}