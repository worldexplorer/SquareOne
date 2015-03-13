using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core.DoubleBuffered;
using Sq1.Core;

namespace Sq1.Widgets.RangeBar {
	public abstract class RangeBar<T> : UserControlDoubleBuffered {
		public event EventHandler<RangeArgs<T>> ValueMinChanged;
		public event EventHandler<RangeArgs<T>> ValueMaxChanged;
		public event EventHandler<RangeArgs<T>> ValuesMinAndMaxChanged;
		public event EventHandler<RangeArgs<T>> ValueMouseOverChanged;
		
		[Browsable(true), DefaultValueAttribute(typeof(TextBox), null)]
		public T RangeMin {get; set;}
		[Browsable(true), DefaultValueAttribute(typeof(TextBox), null)]
		public T RangeMax {get; set;}
		[Browsable(true), DefaultValueAttribute(typeof(TextBox), null)]
		public T ValueMin {get; set;}
		[Browsable(true), DefaultValueAttribute(typeof(TextBox), null)]
		public T ValueMax {get; set;}

		[Browsable(true), DefaultValueAttribute(typeof(TextBox), null)]
		public int RangeScaleLabelDistancePx {get; set;}
	
		[Browsable(true)]
		public Padding PaddingInner;

		[Browsable(true), DefaultValueAttribute(typeof(TextBox), null)]
		protected string valueFormat;
		public string ValueFormat {
			get { if (valueFormat == null) return "0.#";  return valueFormat; }
			set { this.valueFormat = value; }
		}

		[Browsable(true), DefaultValueAttribute(typeof(TextBox), null)]
		public Color ColorBgOutsideRange { get; set; }
		
		private SolidBrush brushOutsideRange;
		protected SolidBrush BrushOutsideRange {
			get {
				//if (this.ColorBgOutsideRange == null) this.ColorBgOutsideRange = Color.LightSteelBlue;
				if (this.brushOutsideRange == null) this.brushOutsideRange = new SolidBrush(this.ColorBgOutsideRange);
				return this.brushOutsideRange;
			}
		}

		[Browsable(true), DefaultValueAttribute(typeof(TextBox), null)]
		public Color ColorBgOutsideMouseOver { get; set; }
		
		private SolidBrush brushMouseOverOutsideRange;
		protected SolidBrush BrushMouseOverOutsideRange {
			get {
				//if (this.ColorBgOutsideMouseOver == null) this.ColorBgOutsideMouseOver = Color.LightBlue;
				if (this.brushMouseOverOutsideRange == null) this.brushMouseOverOutsideRange = new SolidBrush(this.ColorBgOutsideMouseOver);
				return this.brushMouseOverOutsideRange;
			}
		}

		private SolidBrush brushFgText;
		protected SolidBrush BrushFgText {
			get {
				//if (this.ForeColor == null) this.ForeColor = Color.Black;
				if (this.brushFgText == null) this.brushFgText = new SolidBrush(base.ForeColor);
				return this.brushFgText; 
			}
		}
		

		[Browsable(true), DefaultValueAttribute(typeof(TextBox), null)]
		public float ScalePenWidth {get; set;}
		private Pen penFgText;
		protected Pen PenFgText {
			get {
				//if (this.ForeColor == null) this.ForeColor = Color.Black;
				if (this.penFgText == null) this.penFgText = new Pen(base.ForeColor, this.ScalePenWidth);
				return this.penFgText; 
			}
		}

		public virtual string RangeMinFormatted { get { return this.Format(this.RangeMin); } }
		public virtual string RangeMaxFormatted { get { return this.Format(this.RangeMax); } }
		public virtual string ValueMinFormatted { get { return this.Format(this.ValueMin); } }
		public virtual string ValueMaxFormatted { get { return this.Format(this.ValueMax); } }
		public virtual string ValueMouseOverFormatted { get { return this.Format(this.ValueMouseOverFromRangePercentage); } }
		public virtual string Format(T value) { return String.Format("{0:" + this.ValueFormat + "}", value); }

		public float ValueMinRangePercentage { get { return this.PercentageFromValue(this.ValueMin); } }
		public float ValueMaxRangePercentage { get { return this.PercentageFromValue(this.ValueMax); } }
		public virtual float ValueMinMaxMedianPercentage { get {
			return (this.ValueMaxRangePercentage + this.ValueMinRangePercentage) / 2;
		} }

		public float ValueMouseOverRangePercentage;
		public T ValueMouseOverFromRangePercentage { get {
				if (this.ValueMouseOverRangePercentage == -1) return default(T);
				return this.ValueFromPercentage(this.ValueMouseOverRangePercentage);
		} }

		public int ValueMinXOnGraphics { get { return this.RoundInt(base.Width * this.ValueMinRangePercentage); } }
		public int ValueMaxXOnGraphics { get { return this.RoundInt(base.Width * this.ValueMaxRangePercentage); } }
		public int ValueMouseOverXOnGraphics { get { return this.RoundInt(base.Width * this.ValueMouseOverRangePercentage); } }
		
		public virtual int ValueYposition { get { return base.Height - this.LabelHeight - this.LineSpacing - this.PaddingInner.Bottom; } }
		public int LabelHeight;	// { get { return this.RoundInt(g.MeasureString("ABC123", this.Font).Height); } }
		public int LineSpacing { get { return this.RoundInt(this.LabelHeight / 6); } }
		public virtual int RangeYposition { get { return this.PaddingInner.Top; /*+ base.Padding.Top;*/ } }
		public virtual int ValueMouseOverYPosition { get { return this.ValueYposition - this.LabelHeight - this.LineSpacing; }}

		protected bool mouseOver;
		protected float dragStartedPercentage;
		protected bool dragging;
		protected bool dragButtonPressed;
		int dragStartSensitivity;
		int dragStartedX;
		
		public RangeBar() {
			//DOUBLEBUFFERED_ALREADY_DID_THIS base.SetStyle( ControlStyles.AllPaintingInWmPaint
			//			 | ControlStyles.OptimizedDoubleBuffer
			//		//	 | ControlStyles.UserPaint
			//		//	 | ControlStyles.ResizeRedraw
			//		, true);
			this.PaddingInner = new Padding(3, 3, 3, 3); // left, top, right, bottom 
			//this.BorderStyle = BorderStyle.FixedSingle;
			this.ColorBgOutsideRange = Color.LightSteelBlue;
			this.ColorBgOutsideMouseOver = Color.LightBlue;
			base.BackColor = Color.AliceBlue;
			base.ForeColor = Color.Black;
			this.ValueFormat = "0.#"; 
			this.ValueMouseOverRangePercentage = -1;
			this.ScalePenWidth = 1;

			this.mouseOver = false;
			this.dragging = false;
			this.dragButtonPressed = false;
			this.dragStartSensitivity = 3;
		}

		public abstract void checkThrowOnPaint();

		public abstract T ValueFromPercentage(float percentage0to1);
		public abstract float PercentageFromValue(T value);

		public float XonGraphicsToPercentage(int currentXposition) {
			return currentXposition / (float) base.Width;	//without (float) division of two ints is an int !!! (zero)
		}

		protected virtual void DrawGraph(Graphics graphics) {
		}

		protected virtual void DrawScale(Graphics g) {
			//draw scale at the top of G, excluding RangeMin and RangeMax - already drawn by onPaint():
			if (this.RangeScaleLabelDistancePx == 0) return;

			int rangeMinMeasured = this.RoundInt(g.MeasureString(this.RangeMinFormatted, this.Font).Width);
			int rangeMaxMeasured = this.RoundInt(g.MeasureString(this.RangeMaxFormatted, this.Font).Width);
			int valueMinMeasured = this.RoundInt(g.MeasureString(this.ValueMinFormatted, this.Font).Width);
			int valueMaxMeasured = this.RoundInt(g.MeasureString(this.ValueMaxFormatted, this.Font).Width);
			int averageLabelWidth = this.RoundInt((rangeMinMeasured + rangeMaxMeasured + valueMinMeasured + valueMaxMeasured) / 4);
			
			int averageLabelSpaceRequired = averageLabelWidth + this.RangeScaleLabelDistancePx;
			if (averageLabelSpaceRequired == 0) return;		//avoiding OVERFLOW_EXCEPTION (division by zero, my "favorite" one) 

			int leftPosition = this.PaddingInner.Left + rangeMinMeasured + this.RangeScaleLabelDistancePx;
			int rightPosition = base.Width - valueMaxMeasured - this.PaddingInner.Right - this.RangeScaleLabelDistancePx;
			int spaceAvailable = rightPosition - leftPosition;
			int howManyCanFit = (int) (spaceAvailable / averageLabelSpaceRequired);
			if (howManyCanFit == 0) return;		//avoiding OVERFLOW_EXCEPTION (division by zero, my "favorite" one) 

			int rangeScaleLabelDispancePxAdjusted = (int) ((spaceAvailable - (howManyCanFit * averageLabelSpaceRequired)) / (float) howManyCanFit);
			int step = averageLabelSpaceRequired + rangeScaleLabelDispancePxAdjusted;

			int currentXposition = leftPosition + rangeScaleLabelDispancePxAdjusted;
			int midlabelXposition = currentXposition + (int) (averageLabelSpaceRequired / 2);
			for (int i = 0; i<howManyCanFit; i++) {
				float percentage0to1 = this.XonGraphicsToPercentage(midlabelXposition);
				T currentValue = this.ValueFromPercentage(percentage0to1);
				string currentValueFormatted = this.Format(currentValue);
				g.DrawString(currentValueFormatted, this.Font, this.BrushFgText, currentXposition, this.RangeYposition);
				g.DrawLine(this.PenFgText, midlabelXposition, 0, midlabelXposition, this.PaddingInner.Top);
				currentXposition += step;
				midlabelXposition += step;
			}
		}
		//WHEN_INHERITED_FROM_REGULAR_USERCONTROL protected override void OnPaint(PaintEventArgs e) {
		protected override void OnPaintDoubleBuffered(PaintEventArgs e) {
			string msig = " //OnPaintDoubleBuffered() " + this.ToString();
			this.checkThrowOnPaint();
			Graphics g = e.Graphics;
			if (this.LabelHeight == 0) this.LabelHeight = this.RoundInt(g.MeasureString("ABC~`gj123", this.Font).Height);
		
			try {
				if (this.dragging) {
					this.OnPaintMouseDrag(g);
				} else {
					this.OnPaintMouseFree(g);
				}

				this.DrawGraph(g);
				this.DrawScale(g);

				// taken from OnPaintMouseFree() to DrawStrings() above the Graph
				if (this.mouseOver) {
					this.OnPaintMouseFreeMouseOverLabels(g);
				}
				
				this.OnPaintMouseFreeRangeLabels(g);
				this.OnPaintMouseFreeValueLabels(g);

				//DOUBLEBUFFERED_PARENT_DID_THAT base.OnPaint(e);
			} catch (Exception ex) {
				string msg = "WindProc won't catch your exceptions; keep a breakpoint here";
				Assembler.PopupException(msg + msig, ex);
				throw ex;
			}
		}
		protected virtual void OnPaintMouseDrag(Graphics g) {
			float dragSmallerPercentage = this.dragStartedPercentage;
			float dragGreaterPercentage = this.ValueMouseOverRangePercentage;
			if (dragSmallerPercentage > dragGreaterPercentage) {
				dragSmallerPercentage = this.ValueMouseOverRangePercentage;
				dragGreaterPercentage = this.dragStartedPercentage;
			}
			T dragSmaller = this.ValueFromPercentage(dragSmallerPercentage);
			T dragGreater = this.ValueFromPercentage(dragGreaterPercentage);

			string dragSmallerFormatted = this.Format(dragSmaller);
			string dragGreaterFormatted = this.Format(dragGreater);
		
			int dragSmallerMeasured = this.RoundInt(g.MeasureString(dragSmallerFormatted, this.Font).Width);
			int dragGreaterMeasured = this.RoundInt(g.MeasureString(dragGreaterFormatted, this.Font).Width);

			int dragSmallerXOnGraphics = this.RoundInt(base.Width * dragSmallerPercentage);
			int dragGreaterXOnGraphics = this.RoundInt(base.Width * dragGreaterPercentage);

			int paddingDueToBorder = (base.BorderStyle == BorderStyle.None) ? 0 : 1;

			Rectangle outOfRangeLeft = new Rectangle(0, 0, dragSmallerXOnGraphics, base.Height - paddingDueToBorder);
			g.FillRectangle(this.BrushOutsideRange, outOfRangeLeft);

			int oorRightMost = base.Width - dragGreaterXOnGraphics;
			Rectangle outOfRangeRight = new Rectangle(dragGreaterXOnGraphics, 0, oorRightMost - paddingDueToBorder, base.Height - paddingDueToBorder);
			g.FillRectangle(this.BrushOutsideRange, outOfRangeRight);

			/*int dragSmallerX = dragSmallerXOnGraphics - dragSmallerMeasured - this.PaddingInner.Right;
			if (dragSmallerX < 0) dragSmallerX = 0;
			g.DrawString(dragSmallerFormatted, this.Font, this.BrushFgText, dragSmallerX, this.ValueYposition);

			int dragGreaterX = dragGreaterXOnGraphics + this.PaddingInner.Left;
			int valueMaxXrightMost = base.Width - dragGreaterMeasured - this.PaddingInner.Right;
			if (dragGreaterX > valueMaxXrightMost) dragGreaterX = valueMaxXrightMost;
			g.DrawString(dragGreaterFormatted, this.Font, this.BrushFgText, dragGreaterX, this.ValueYposition);*/
		}
		protected virtual void OnPaintMouseFree(Graphics g) {
			int paddingDueToBorder = (base.BorderStyle == BorderStyle.None) ? 0 : 1;
			Rectangle outOfRangeLeft = new Rectangle(0, 0, this.ValueMinXOnGraphics, base.Height - paddingDueToBorder);
			g.FillRectangle(this.BrushOutsideRange, outOfRangeLeft);

			int oorRightMost = base.Width - this.ValueMaxXOnGraphics;
			Rectangle outOfRangeRight = new Rectangle(this.ValueMaxXOnGraphics, 0, oorRightMost - paddingDueToBorder, base.Height - paddingDueToBorder);
			g.FillRectangle(this.BrushOutsideRange, outOfRangeRight);
			
			if (this.mouseOver) {
				int valueMouseOverMeasured = (int)Math.Round(g.MeasureString(this.ValueMouseOverFormatted, this.Font).Width);
				if (this.ValueMouseOverRangePercentage < this.ValueMinMaxMedianPercentage) {
					Rectangle outOfRangeMouseOverLeft = new Rectangle(0, 0, this.ValueMouseOverXOnGraphics, base.Height - 1);
					g.FillRectangle(this.BrushMouseOverOutsideRange, outOfRangeMouseOverLeft);
				} else {
					int rightWidth = base.Width - this.ValueMouseOverXOnGraphics;
					Rectangle outOfRangeMouseOverRight = new Rectangle(this.ValueMouseOverXOnGraphics, 0, rightWidth - paddingDueToBorder, base.Height - paddingDueToBorder);
					g.FillRectangle(this.BrushMouseOverOutsideRange, outOfRangeMouseOverRight);
				}
			}
		}
		protected virtual void OnPaintMouseFreeValueLabels(Graphics g) {
			int valueMinMeasured = this.RoundInt(g.MeasureString(this.ValueMinFormatted, this.Font).Width);
			int valueMaxMeasured = this.RoundInt(g.MeasureString(this.ValueMaxFormatted, this.Font).Width);

			// no mouseover - draw both; onMouseOver - draw the mouseOver one and the opposite (hide the one replaced by mouseover)
			//if (this.mouseOver == false || this.ValueMouseOverRangePercentage >= this.ValueMinMaxMedianPercentage) {
				int valueMinX = this.ValueMinXOnGraphics - valueMinMeasured - this.PaddingInner.Right;
				if (valueMinX < 0) valueMinX = 0;
				g.DrawString(this.ValueMinFormatted, this.Font, this.BrushFgText, valueMinX, this.ValueYposition);
				int notGoingBeyondLeftEdge = (this.ValueMinXOnGraphics <= 0) ? 0 : this.ValueMinXOnGraphics;
				g.DrawLine(this.PenFgText, notGoingBeyondLeftEdge, base.Height - 1, notGoingBeyondLeftEdge, base.Height - this.PaddingInner.Bottom - 3);
			//}
			//if (this.mouseOver == false || this.ValueMouseOverRangePercentage < this.ValueMinMaxMedianPercentage) {
				int valueMaxX = this.ValueMaxXOnGraphics + this.PaddingInner.Left;
				int valueMaxXrightMost = base.Width - valueMaxMeasured - this.PaddingInner.Right;
				if (valueMaxX > valueMaxXrightMost) valueMaxX = valueMaxXrightMost;
				g.DrawString(this.ValueMaxFormatted, this.Font, this.BrushFgText, valueMaxX, this.ValueYposition);
				int notGoingBeyondRightEdge = (this.ValueMaxXOnGraphics > base.Width-3) ? base.Width-3 : this.ValueMaxXOnGraphics;
				g.DrawLine(this.PenFgText, notGoingBeyondRightEdge, base.Height - 1, notGoingBeyondRightEdge, base.Height - this.PaddingInner.Bottom - 3);
			//}
		}
		protected virtual void OnPaintMouseFreeRangeLabels(Graphics g) {
				int rangeMinMeasured = this.RoundInt(g.MeasureString(this.RangeMinFormatted, this.Font).Width);
				int rangeMaxMeasured = this.RoundInt(g.MeasureString(this.RangeMaxFormatted, this.Font).Width);

				g.DrawString(this.RangeMinFormatted, this.Font, this.BrushFgText, this.PaddingInner.Left, this.RangeYposition);
				int rangeMaxXrightMost = base.Width - rangeMaxMeasured - this.PaddingInner.Right;
				g.DrawString(this.RangeMaxFormatted, this.Font, this.BrushFgText, rangeMaxXrightMost, this.RangeYposition);
		}
		protected virtual void OnPaintMouseFreeMouseOverLabels(Graphics g) {
			int valueMouseOverMeasured = (int)Math.Round(g.MeasureString(this.ValueMouseOverFormatted, this.Font).Width);
			
			float medianOrOppositePercentage = this.ValueMinMaxMedianPercentage;
			if (this.dragging) {
				bool mouseDraggingToTheLeft = this.ValueMouseOverRangePercentage < this.dragStartedPercentage;
				// 0: string at right of MouseOver, 1: string at left of MouseOver 
				medianOrOppositePercentage = (mouseDraggingToTheLeft) ? 1 : 0;
				
				// place dragStarted to the Left/Right of current mouseDrag X position
				T dragStarted = this.ValueFromPercentage(this.dragStartedPercentage);
				string dragStartedFormatted = this.Format(dragStarted);
				int dragStartedMeasured = this.RoundInt(g.MeasureString(dragStartedFormatted, this.Font).Width);
				int dragStartedXOnGraphics = this.RoundInt(base.Width * this.dragStartedPercentage);
				
				if (mouseDraggingToTheLeft) {
					// dragging to the left => place to the right of dragStartedX
					int dragStartLabelX = dragStartedXOnGraphics + this.PaddingInner.Left;
					int dragStartLabelXrightMost = base.Width - valueMouseOverMeasured - this.PaddingInner.Right;
					if (dragStartLabelX > dragStartLabelXrightMost) dragStartLabelX = dragStartLabelXrightMost;
					g.DrawString(dragStartedFormatted, this.Font, this.BrushFgText, dragStartLabelX, this.ValueMouseOverYPosition);
				} else {
					// dragging to the right => place to the left of dragStartedX
					int dragStartLabelX = dragStartedXOnGraphics - dragStartedMeasured - this.PaddingInner.Right;
					if (dragStartLabelX < 0) dragStartLabelX = this.PaddingInner.Left;
					g.DrawString(dragStartedFormatted, this.Font, this.BrushFgText, dragStartLabelX, this.ValueMouseOverYPosition);
				}
				
				g.DrawLine(this.PenFgText, dragStartedXOnGraphics, base.Height - 1, dragStartedXOnGraphics, base.Height - this.PaddingInner.Bottom - 3);
				g.DrawLine(this.PenFgText, this.ValueMouseOverXOnGraphics, base.Height - 1, this.ValueMouseOverXOnGraphics, base.Height - this.PaddingInner.Bottom - 3);
			}

			if (this.ValueMouseOverRangePercentage < medianOrOppositePercentage) {
				int movLabelX = this.ValueMouseOverXOnGraphics - valueMouseOverMeasured - this.PaddingInner.Right;
				if (movLabelX < 0) movLabelX = this.PaddingInner.Left;
				g.DrawString(this.ValueMouseOverFormatted, this.Font, this.BrushFgText, movLabelX, this.ValueMouseOverYPosition);
			} else {
				int movX = this.ValueMouseOverXOnGraphics + this.PaddingInner.Left;
				int movXrightMost = base.Width - valueMouseOverMeasured - this.PaddingInner.Right;
				if (movX > movXrightMost) movX = movXrightMost;
				g.DrawString(this.ValueMouseOverFormatted, this.Font, this.BrushFgText, movX, this.ValueMouseOverYPosition);
			}
		}
		//WHEN_INHERITED_FROM_REGULAR_USERCONTROL protected override void OnPaintBackground(PaintEventArgs e) {
		//DOUBLEBUFFERED_ALREADY_PAINTS_BACKCOLOR protected override void OnPaintBackgroundDoubleBuffered (PaintEventArgs e) {
		//	e.Graphics.Clear(base.BackColor);
		//}
		protected override void OnMouseEnter(EventArgs e) {
			this.mouseOver = true;
			this.dragging = false;
			base.Invalidate();
			base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			this.mouseOver = false;
			this.dragging = false;
			base.Invalidate();	// visuals first!!!
			this.ValueMouseOverRangePercentage = -1;
			this.RaiseValueMouseOverChanged();
			base.OnMouseLeave(e);
		}		
		protected override void OnMouseDown(MouseEventArgs e) {
			if (e.Button != MouseButtons.Left) return;
			this.dragButtonPressed = true;
			this.dragging = false;
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			string msig = " //OnMouseUp() " + this.ToString();
			if (e.Button != MouseButtons.Left) return;
			try {
				if (this.ValueMouseOverRangePercentage > 1) {
					string msg = "NEVER_OBSERVED_BEFORE Houston we have a problem";
					Assembler.PopupException(msg + msig);
				}
				if (this.dragging == false) {
					if (this.ValueMouseOverRangePercentage < this.ValueMinMaxMedianPercentage) {
						this.ValueMin = this.ValueMouseOverFromRangePercentage;
						base.Invalidate();	// visuals first!!!
						this.RaiseValueMinChanged();
					} else {
						this.ValueMax = this.ValueMouseOverFromRangePercentage;
						base.Invalidate();	// visuals first!!!
						this.RaiseValueMaxChanged();
					}
				} else {
					// handling dragEnd here
					float dragSmaller = this.dragStartedPercentage;
					float dragGreater = this.ValueMouseOverRangePercentage;
					if (dragSmaller > dragGreater) {
						dragSmaller = this.ValueMouseOverRangePercentage;
						dragGreater = this.dragStartedPercentage;
					}
					// fixed using XbecomesNegativeWhileDragging
					//if (dragSmaller < 0) dragSmaller = 0;		// when we released Left mouse button beyond  left edge of the RangeBar control
					//if (dragGreater > 1) dragGreater = 1; 		// when we released Left mouse button beyond right edge of the RangeBar control
					this.ValueMin = this.ValueFromPercentage(dragSmaller);
					this.ValueMax = this.ValueFromPercentage(dragGreater);
					this.dragStartedPercentage = -1;
					this.dragging = false;
					base.Invalidate();	// visuals first!!!
					this.RaiseValuesMinAndMaxChanged();
				}
				this.dragButtonPressed = false;
			} catch (Exception ex) {
				string msg = "WindProc won't catch your exceptions; keep a breakpoint here";
				Assembler.PopupException(msg + msig, ex);
				throw ex;
			}
			base.OnMouseUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			try {
				if (this.dragging) {
					// should drag a little for me to consider the user is really dragging anything
					if (Math.Abs(dragStartedX - e.X) < this.dragStartSensitivity) return;
					int XnotBeyond0width = e.X;
					if (e.X < 0) XnotBeyond0width = 0;
					if (e.X > base.Width) XnotBeyond0width = base.Width;
					this.ValueMouseOverRangePercentage = this.XonGraphicsToPercentage(XnotBeyond0width);
				} else {
					this.ValueMouseOverRangePercentage = this.XonGraphicsToPercentage(e.X);
				}
				
				if (this.dragButtonPressed == false) {
					base.Invalidate();	// visuals first!!!
					this.RaiseValueMouseOverChanged();
					base.OnMouseMove(e);
					return;
				}
				if (this.dragging == false) {
					// first move after this.mousePressed means DRAG!! wasn't in UserControl due to ambiguity between OnMouseMove and OnMouseDrag, probably 
					this.dragStartedX = e.X;
					this.dragStartedPercentage = this.XonGraphicsToPercentage(e.X);
					this.dragging = true;
				}
				base.Invalidate();	// visuals first!!!
				base.OnMouseMove(e);
			} catch (Exception ex) {
				string msg = "WindProc won't catch your exceptions; keep a breakpoint here";
				throw ex;
			}
		}
		public int RoundInt(double d) {
			return (int)Math.Round(d);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				this.BrushOutsideRange.Dispose();
				if (this.brushFgText != null) this.brushFgText.Dispose();
				this.BrushMouseOverOutsideRange.Dispose();
				//if (components != null) components.Dispose();
			}
			base.Dispose(disposing);
		}
		protected virtual void RaiseValueMinChanged() {
			if (this.ValueMinChanged == null) return;
			RangeArgs<T> args = this.CreateEventArgsSnapshot();
			this.ValueMinChanged(this, args);
		}
		protected virtual void RaiseValueMaxChanged() {
			if (this.ValueMaxChanged == null) return;
			RangeArgs<T> args = this.CreateEventArgsSnapshot();
			this.ValueMaxChanged(this, args);
		}
		protected virtual void RaiseValuesMinAndMaxChanged() {
			if (this.ValuesMinAndMaxChanged == null) return;
			RangeArgs<T> args = this.CreateEventArgsSnapshot();
			this.ValuesMinAndMaxChanged(this, args);
		}
		protected virtual void RaiseValueMouseOverChanged() {
			if (this.ValueMouseOverChanged == null) return;
			RangeArgs<T> args = this.CreateEventArgsSnapshot();
			this.ValueMouseOverChanged(this, args);
		}
		private RangeArgs<T> CreateEventArgsSnapshot() {
			return new RangeArgs<T>(this.RangeMin, this.RangeMinFormatted,
							 		this.RangeMax, this.RangeMaxFormatted,
							 		this.ValueMin, this.ValueMinFormatted,
							 		this.ValueMax, this.ValueMaxFormatted,
									this.ValueMouseOverFromRangePercentage, this.ValueMouseOverFormatted);
		}

		//public abstract T XonGraphicsToValue(int currentXposition);
		//public T RangeWidth { get { return base.RangeMax - base.RangeMin; } }
		//public virtual T XonGraphicsToValue(int currentXposition) {
		//	return this.RoundInt(this.RangeMin + currentXposition*this.RangeWidth);
		//}

		/*ABSTRACT_COZ_T_DOESNT_ALLOW_ARITHMETICS public void checkThrow() {
			if (this.RangeMin > this.RangeMax) {
				string msg = "RangeBar.RangeMin[" + this.RangeMin + "] > RangeBar.RangeMax[" + this.RangeMax + "]";
				throw new Exception(msg);
			}
			if (this.ValueMin > this.ValueMax) {
				string msg = "RangeBar.ValueMin[" + this.ValueMin + "] > RangeBar.ValueMax[" + this.ValueMax + "]";
				throw new Exception(msg);
			}
			if (this.RangeMin > this.ValueMin) {
				string msg = "RangeBar.RangeMin[" + this.RangeMin + "] > RangeBar.ValueMin[" + this.ValueMin + "]";
				throw new Exception(msg);
			}
			if (this.ValueMax > this.RangeMax) {
				string msg = "RangeBar.ValueMax[" + this.ValueMax + "] > RangeBar.RangeMax[" + this.RangeMax + "]";
				throw new Exception(msg);
			}
		}*/

	}
}