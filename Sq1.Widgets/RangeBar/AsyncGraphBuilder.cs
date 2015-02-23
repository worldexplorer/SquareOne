using System;
using System.Drawing;
using System.Threading;
using System.Timers;
using Sq1.Core;

namespace Sq1.Widgets.RangeBar {
	public abstract class AsyncGraphBuilder<T> {
		protected RangeBarWithGraph<T> RangeBarWithGraph;
		
		protected int millisAfterLastResizeToBuildAsync;
		//protected long ticksWhenLastResized;
		
		public bool IsCalculating;
		protected bool timerStarted;
		protected bool AbortCalculation;
		protected AutoResetEvent areCalculationsAborted;

		//private System.Windows.Forms.Timer	timerWinForms;
		private System.Timers.Timer 		timerSystem;

		protected abstract double ValueRange { get; }
		public abstract bool HasDataToDraw { get; }
		protected abstract float PercentageYOnGraphForRangePercentage(float pixelToPercentage0to1);
		public int[] ValueYinvertedForGraphicsWidth;
		private object valueYInvertedLock;
		public int PrevCalculationHeight;
		
		
		public AsyncGraphBuilder(RangeBarWithGraph<T> rangeBarWithGraph, int defaultMillisToWait = 500)
				: this(defaultMillisToWait) {
			this.RangeBarWithGraph = rangeBarWithGraph;
		}
		private AsyncGraphBuilder(int defaultMillisToWait = 500) {
			this.millisAfterLastResizeToBuildAsync = defaultMillisToWait;
			//this.ticksWhenLastResized = 0;
			this.valueYInvertedLock = new object();
			this.IsCalculating = false;
			this.timerStarted = false;
			this.AbortCalculation = false;
			this.areCalculationsAborted = new AutoResetEvent(false);
			//this.timerWinForms = new System.Windows.Forms.Timer();
			this.timerSystem = new System.Timers.Timer();
			if (this.millisAfterLastResizeToBuildAsync > 0) {
				this.timerSystem.Elapsed += new ElapsedEventHandler(OnTimerElapsed);
				this.timerSystem.Interval = this.millisAfterLastResizeToBuildAsync;
			}
			this.PrevCalculationHeight = 0;
		}
		public void Dispose() {
			this.timerSystem.Elapsed -= new ElapsedEventHandler(OnTimerElapsed);
			this.timerSystem.Close();
			this.timerSystem.Dispose();
		}
		public void BuildGraphInNewThreadAndInvalidateDelayed() {
			//ticksWhenLastResized = DateTime.Now.Ticks;
			if (this.millisAfterLastResizeToBuildAsync == 0) {
				this.BuildGraphInNewThreadAndInvalidate();
				return;
			}
			if (this.IsCalculating) return;
			
			// interrupt the Timer and start counting timerSystem.Interval again
			this.timerSystem.Stop();
			this.timerSystem.Start();
			this.timerStarted = true;
		}
		public void OnTimerElapsed( Object source, ElapsedEventArgs e ) {
			this.timerSystem.Stop();
			this.timerStarted = false;
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.BuildGraphInNewThreadAndInvalidate));
		}
		public void BuildGraphInNewThreadAndInvalidate(object state=null) {
			if (this.IsCalculating) {
				this.AbortCalculation = true;
				this.areCalculationsAborted.WaitOne();
				this.AbortCalculation = false;
			}
			this.BuildGraphTimeConsuming();
			//REASON_FOR_INFINITE_LOOP_DONT_UNCOMMENT_NOR_DELETE
			this.RangeBarWithGraph.Invalidate();
		}
		protected void BuildGraphTimeConsuming() {
			string msig = " //AsyncGraphBuilder<T>.BuildGraphTimeConsuming()";
			if (this.RangeBarWithGraph.BufferedGraphics == null) return;
			if (this.HasDataToDraw == false) {
				if (this.ValueYinvertedForGraphicsWidth != null) {
					this.ValueYinvertedForGraphicsWidth = null;
				}
				return;
			}
			Graphics g = this.RangeBarWithGraph.BufferedGraphics.Graphics;
			//float anyGivenValueToGraphicsHeight = (float)(this.ValueRange) / (float) g.VisibleClipBounds.Height;
			lock (this.valueYInvertedLock) {
				try {
					this.IsCalculating = true;
					int graphHeight = this.RangeBarWithGraph.GraphHeight;
					int graphYposition = this.RangeBarWithGraph.GraphYposition;
					this.PrevCalculationHeight = this.RangeBarWithGraph.Height;
					//OBJECT_USED_ELSEWHERE??? this.ValueYinvertedForGraphicsWidth = new int[(int)g.VisibleClipBounds.Width];
					//OBJECT_USED_ELSEWHERE??? for (int i = 0; i < g.VisibleClipBounds.Width; i++) {
					this.ValueYinvertedForGraphicsWidth = new int[this.RangeBarWithGraph.Width];
					for (int i = 0; i < this.ValueYinvertedForGraphicsWidth.Length; i++) {
						if (this.AbortCalculation) {
							this.areCalculationsAborted.Set();
							return;
						}
						this.ValueYinvertedForGraphicsWidth[i] = -1;
						try {
							float percentageX = this.RangeBarWithGraph.XonGraphicsToPercentage(i);
							float percentageY = this.PercentageYOnGraphForRangePercentage(percentageX);
							if (percentageY == -1) {
								object value = this.RangeBarWithGraph.ValueFromPercentage(percentageX);
								string msg = "empty value at [" + value + "]";
								continue;
							}
	
							//int y = this.RangeBar.RoundInt(percentageY * anyGivenValueToGraphicsHeight);

							int y = this.RangeBarWithGraph.RoundInt(percentageY * graphHeight);
							if (y > graphHeight) {
								continue;
							}
							//int yInverted = ((int)g.VisibleClipBounds.Height) - y;
							int yInverted = graphHeight - y;
							yInverted += graphYposition;
							this.ValueYinvertedForGraphicsWidth[i] = yInverted;
						} catch (Exception ex) {
							Assembler.PopupException("INNER_LOOP_AT" + msig, ex);
							continue;
						}
					}
				} catch (Exception ex) {
					Assembler.PopupException("OUTER_CAUGHT_AT" + msig, ex);
				} finally {
					this.IsCalculating = false;
				}
			}
		}
		public void SpillGraphOnGraphics(Graphics g) {
			if (this.ValueYinvertedForGraphicsWidth == null) return;
			if (this.ValueYinvertedForGraphicsWidth.Length < 2 ) return;
			if (this.millisAfterLastResizeToBuildAsync == 0) {
				int newCalculationRequest = (int)g.VisibleClipBounds.Width;
				int alreadyCalculated = this.ValueYinvertedForGraphicsWidth.Length;
				bool resizedWith = newCalculationRequest != alreadyCalculated; 
				bool resizedHeight = (int)g.VisibleClipBounds.Height != this.PrevCalculationHeight; 
				if (resizedWith || resizedHeight) {
					this.BuildGraphTimeConsuming();
					if (this.ValueYinvertedForGraphicsWidth == null) return;	//happens when spilling empty chart	HasDataToDraw=false
				}
			} else {
				if (this.timerStarted) return;
				if (this.IsCalculating) return;
			}
			//MOVED_UP if (g.VisibleClipBounds.Width < 2) return;
			lock (this.valueYInvertedLock) {
				//for (int x = 1; x < g.VisibleClipBounds.Width; x++) {
				for (int x = 1; x < this.ValueYinvertedForGraphicsWidth.Length; x++) {
					try {
						int yInverted = this.ValueYinvertedForGraphicsWidth[x];
						if (yInverted == -1) continue;
						int prevYinverted = this.ValueYinvertedForGraphicsWidth[x-1]; 
						if (prevYinverted == -1) continue;
						g.DrawLine(this.RangeBarWithGraph.PenFgGraph, x-1, prevYinverted, x, yInverted);
					} catch (Exception e) {
						continue;
					}
				}
			}
		}
	}
}
