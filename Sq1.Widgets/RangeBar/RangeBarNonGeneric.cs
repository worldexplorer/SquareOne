using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

using Sq1.Core.DoubleBuffered;

namespace Sq1.Widgets.RangeBar {
	public class RangeBarNonGeneric : UserControlDoubleBuffered {
		[Browsable(true), DefaultValueAttribute(typeof(TextBox), null)]
		public int RangeMin {get; set;}
		[Browsable(true), DefaultValueAttribute(typeof(TextBox), null)]
		public int RangeMax {get; set;}
		[Browsable(true), DefaultValueAttribute(typeof(TextBox), null)]
		public int ValueMin {get; set;}
		[Browsable(true), DefaultValueAttribute(typeof(TextBox), null)]
		public int ValueMax {get; set;}
		
		public int ValueMouseOver;
		public bool ValueMouseOverIsCloserToValueMin { get {
				return (this.ValueMouseOver < this.ValueMedian) ? true: false;
			} }
		
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		private string valueFormat;
		public string ValueFormat {
			get { if (valueFormat == null) return "0.#";  return valueFormat; }
			set { this.valueFormat = value; }
		}

		[Browsable(true), DefaultValueAttribute(typeof(TextBox), null)]
		public Color ColorBgOutsideRange {get; set;}
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public Color ColorBgOutsideMouseOver {get; set;}
		
		[Browsable(true), DefaultValueAttribute(typeof(TextBox), null)]
		public Padding PaddingInner;

		public int RangeWidth { get {return this.RangeMax - this.RangeMin; } } 
		public int ValueMinDistance { get {return this.ValueMin - this.RangeMin; } } 
		public int ValueMaxDistance { get {return this.ValueMax - this.RangeMin; } } 
		public int ValueMouseOverDistance { get {return this.ValueMouseOver - this.RangeMin; } } 
		public int ValueMedian { get { return this.ValueMin + this.RoundInt((this.ValueMax - this.ValueMin)/2); } }

		public string RangeMinFormatted { get { return this.RangeMin.ToString(this.ValueFormat); } }
		public string RangeMaxFormatted { get { return this.RangeMax.ToString(this.ValueFormat); } }
		public string ValueMinFormatted { get { return this.ValueMin.ToString(this.ValueFormat); } }
		public string ValueMaxFormatted { get { return this.ValueMax.ToString(this.ValueFormat); } }
		public string ValueMouseOverFormatted { get { return this.ValueMouseOver.ToString(this.ValueFormat); } }
		public string ValueMedianFormatted { get { return this.ValueMedian.ToString(this.ValueFormat); } }

		public int ValueMinXOnGraphics { get {
				float valueMin0to1 = this.ValueMinDistance / (float) this.RangeWidth;	//without (float) division of two ints is an int !!! (zero)
				return this.RoundInt(base.Width * valueMin0to1);
			} }
		public int ValueMaxXOnGraphics { get {
				float valueMax0to1 = this.ValueMaxDistance / (float) this.RangeWidth;
				return this.RoundInt(base.Width * valueMax0to1);
			} }
		public int ValueMouseOverXOnGraphics { get {
				float valueMouseOver0to1 = this.ValueMouseOverDistance / (float) this.RangeWidth;
				return this.RoundInt(base.Width * valueMouseOver0to1);
			} }
		
		public int ValueYposition { get { return PaddingInner.Top; /*+ base.Padding.Top;*/ } }
		public int LabelHeight;	// { get { return this.RoundInt(g.MeasureString("ABC123", this.Font).Height); } }
		public int LineSpacing { get { return this.RoundInt(this.LabelHeight / 6); } }
		public int RangeYposition { get { return base.Height - this.LabelHeight - this.LineSpacing - this.PaddingInner.Bottom; } } 
		
		public RangeBarNonGeneric() {
			//this.SetStyle(
			//	ControlStyles.UserPaint |
			//	ControlStyles.AllPaintingInWmPaint |
			//	ControlStyles.OptimizedDoubleBuffer |
			//	ControlStyles.ResizeRedraw, true);
			this.RangeMin = 100;
			this.RangeMax = 500;
			this.ValueMin = 220;
			this.ValueMax = 380;
			this.PaddingInner = new Padding(3);
			this.BorderStyle = BorderStyle.FixedSingle;
			this.BackColor = Color.AliceBlue;
			this.ColorBgOutsideMouseOver = Color.LightBlue;
			this.ColorBgOutsideRange = Color.LightSteelBlue;
		}

		void checkThrow() {
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
		}
		protected override void OnPaintDoubleBuffered(PaintEventArgs e) {
			//if (base.DesignMode) return;
			
			this.checkThrow();

			Graphics g = e.Graphics;
			if (this.LabelHeight == 0) {
				this.LabelHeight = this.RoundInt(g.MeasureString("ABC123", this.Font).Height);
			}
			SolidBrush brushOutsideRange = new SolidBrush(ColorBgOutsideRange);
			SolidBrush brushFgText = new SolidBrush(base.ForeColor);
			SolidBrush brushMouseOverOutsideRange = new SolidBrush(ColorBgOutsideMouseOver);
		
			try {
				int rangeMinMeasured = (int)Math.Round(g.MeasureString(this.RangeMinFormatted, this.Font).Width);
				int rangeMaxMeasured = (int)Math.Round(g.MeasureString(this.RangeMaxFormatted, this.Font).Width);
				int valueMinMeasured = (int)Math.Round(g.MeasureString(this.ValueMinFormatted, this.Font).Width);
				int valueMaxMeasured = (int)Math.Round(g.MeasureString(this.ValueMaxFormatted, this.Font).Width);
				int valueMedianMeasured = (int)Math.Round(g.MeasureString(this.ValueMedianFormatted, this.Font).Width);

				Rectangle outOfRangeLeft = new Rectangle(0, 0, this.ValueMinXOnGraphics, base.Height-1);
				g.FillRectangle(brushOutsideRange, outOfRangeLeft);
				
				int oorRightMost = base.Width - this.ValueMaxXOnGraphics;
				Rectangle outOfRangeRight = new Rectangle(this.ValueMaxXOnGraphics, 0, oorRightMost-1, base.Height-1);
				g.FillRectangle(brushOutsideRange, outOfRangeRight);
				
				if (this.mouseOver) {
					int valueMouseOverMeasured = (int)Math.Round(g.MeasureString(this.ValueMouseOverFormatted, this.Font).Width);
					if (this.ValueMouseOverIsCloserToValueMin) {
						Rectangle outOfRangeMouseOverLeft = new Rectangle(0, 0, this.ValueMouseOverXOnGraphics, base.Height-1);
						g.FillRectangle(brushMouseOverOutsideRange, outOfRangeMouseOverLeft);
					
						int movLabelX = this.ValueMouseOverXOnGraphics - valueMouseOverMeasured - PaddingInner.Right;
						if (movLabelX < 0) movLabelX = 0;
						g.DrawString(this.ValueMouseOverFormatted, this.Font, brushFgText, movLabelX, ValueYposition);
					} else {
						int rightWidth = base.Width - this.ValueMouseOverXOnGraphics;
						Rectangle outOfRangeMouseOverRight = new Rectangle(this.ValueMouseOverXOnGraphics, 0, rightWidth -1, base.Height-1);
						g.FillRectangle(brushMouseOverOutsideRange, outOfRangeMouseOverRight);

						int movX = this.ValueMouseOverXOnGraphics + PaddingInner.Left;
						int movXrightMost = base.Width - valueMouseOverMeasured - PaddingInner.Right;
						if (movX > movXrightMost) movX = movXrightMost;
						g.DrawString(this.ValueMouseOverFormatted, this.Font, brushFgText, movX, ValueYposition);
					}
					//g.DrawString(this.ValueMedianFormatted, this.Font, brushFgText, this.ValueMouseOverXOnGraphics, labelYposition + 10);
				}
				
				// no mouseover - draw both; onMouseOver - draw the mouseOver one and the opposite (hide the one replaced by mouseover)
				if (this.mouseOver == false || this.ValueMouseOverIsCloserToValueMin == false) {
					int valueMinX = this.ValueMinXOnGraphics - valueMinMeasured - PaddingInner.Right;
					if (valueMinX < 0) valueMinX = 0;
					g.DrawString(this.ValueMinFormatted, this.Font, brushFgText, valueMinX, ValueYposition);
				}
				if (this.mouseOver == false || this.ValueMouseOverIsCloserToValueMin == true) {
					int valueMaxX = this.ValueMaxXOnGraphics + PaddingInner.Left;
					int valueMaxXrightMost = base.Width - valueMaxMeasured - PaddingInner.Right;
					if (valueMaxX > valueMaxXrightMost) valueMaxX = valueMaxXrightMost;
					g.DrawString(this.ValueMaxFormatted, this.Font, brushFgText, valueMaxX, ValueYposition);
				}
		
				g.DrawString(this.RangeMinFormatted, this.Font, brushFgText, PaddingInner.Left, this.RangeYposition);
				int rangeMaxXrightMost = base.Width - rangeMaxMeasured - PaddingInner.Right;
				g.DrawString(this.RangeMaxFormatted, this.Font, brushFgText, rangeMaxXrightMost, this.RangeYposition);
				
				base.OnPaint(e);
			} catch (Exception ex) {
				string msg = "WindProc won't catch your exceptions; keep a breakpoint here";
			} finally {
				brushOutsideRange.Dispose();
				brushFgText.Dispose();
				brushMouseOverOutsideRange.Dispose();
			}
		}
		//void RenderValueMouseOver(Graphics g) { }
		protected override void OnPaintBackground (PaintEventArgs e) {
			e.Graphics.Clear(base.BackColor);
		}
		
		private bool mouseOver = false;
		protected override void OnMouseEnter(EventArgs e) {
			this.mouseOver = true;
			base.Invalidate();
			base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			this.mouseOver = false;
			base.Invalidate();
			base.OnMouseLeave(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			if (e.Button != MouseButtons.Left) {
				return;
			}
			if (this.ValueMouseOverIsCloserToValueMin) {
				this.ValueMin = this.ValueMouseOver;
			} else {
				this.ValueMax = this.ValueMouseOver;
			}
			base.Invalidate();
			base.OnMouseUp(e);
		}
		
		protected override void OnMouseMove(MouseEventArgs e) {
			try {
				float value0to1MouseOver = e.X / (float) base.Width;	//without (float) division of two ints is an int !!! (zero)
				this.ValueMouseOver = this.RangeMin + this.RoundInt(this.RangeWidth * value0to1MouseOver);
				base.Invalidate();
				base.OnMouseMove(e);
			} catch (Exception ex) {
				string msg = "WindProc won't catch your exceptions; keep a breakpoint here";
			}
		}
		/*
		protected override void OnMouseClick(MouseEventArgs e) {
			//if (base.DesignMode) return;
			try {
				float value0to1MouseOver = e.X / (float) base.Width;	//without (float) division of two ints is an int !!! (zero)
				int valueMouseOver = this.RangeMin + this.RoundInt(this.RangeWidth * value0to1MouseOver);
				if (value0to1MouseOver >= 0.5) {
					this.ValueMax = valueMouseOver;
				} else {
					this.ValueMin = valueMouseOver;
				}
				this.Invalidate();
				base.OnMouseClick(e);
			} catch (Exception ex) {
				string msg = "WindProc won't catch your exceptions; keep a breakpoint here";
			}
		}
		*/
		private int RoundInt(double d) {
			return (int)Math.Round(d);
		}
	}
}