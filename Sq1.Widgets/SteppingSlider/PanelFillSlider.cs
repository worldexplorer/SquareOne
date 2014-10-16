using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DoubleBuffered;

namespace Sq1.Widgets.SteppingSlider {
	public partial class PanelFillSlider : PanelDoubleBuffered {
		private SolidBrush brushBgValueCurrentEnabled = new SolidBrush(System.Drawing.Color.FromArgb(192, 192, 255));
	    private SolidBrush brushBgValueCurrentDisabled = new SolidBrush(Color.DarkGray);
		private SolidBrush brushBgValueCurrent { get { return base.Enabled ? brushBgValueCurrentEnabled : brushBgValueCurrentDisabled; } }
	    
	    private SolidBrush brushBgMouseOverEnabled = new SolidBrush(Color.LightGreen);
	    private SolidBrush brushBgMouseOverDisabled = new SolidBrush(Color.DarkGreen);
		private SolidBrush brushBgMouseOver { get { return base.Enabled ? brushBgMouseOverEnabled : brushBgMouseOverDisabled; } }

	    private SolidBrush brushFgParameterLabel = new SolidBrush(Color.White);
		private SolidBrush brushFgText;
		private SolidBrush brushBgControl;
	    
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		private decimal valueMin;
		public decimal ValueMin {
			get { return valueMin; }
			set { valueMin = value; this.Invalidate(); }
		}
	
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		private decimal valueStep;
		public decimal ValueIncrement {
			get { return valueStep; }
			set { valueStep = value; this.Invalidate(); }
		}
		
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		private decimal valueMax;
		public decimal ValueMax {
			get { return valueMax; }
			set { valueMax = value; this.Invalidate(); }
		}

		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		private decimal valueCurrent;
		public decimal ValueCurrent {
			get { return valueCurrent; }
			set {
				if (value > this.ValueMax) {
					Debugger.Break();
					return;
				}
				if (value < this.ValueMin) {
					Debugger.Break();
					return;
				}
				if (value != this.RoundToClosestStep(value)) {
					Debugger.Break();
					return;
				}
				valueCurrent = value;
				this.RaiseValueCurrentChanged();
				this.Invalidate();
			}
		}
		
		// COPYPASTE from IndicatorParameter.cs BEGIN
		public string ValidateSelf() {
			if (this.ValueMin > this.ValueMax)		return "ValueMin[" + this.ValueMin + "] > ValueMax[" + this.ValueMax + "]";
			if (this.ValueCurrent > this.ValueMax)	return "ValueCurrent[" + this.ValueCurrent + "] > ValueMax[" + this.ValueMax + "]";
			if (this.ValueCurrent < this.ValueMin)	return "ValueCurrent[" + this.ValueCurrent + "] < ValueMin[" + this.ValueMin + "]";
			return null;
		}
		public override string ToString() {
			return this.Name + ":" + this.ValueCurrent + "[" + this.ValueMin + ".." + this.ValueMax + "/" + this.ValueIncrement + "]";
		}
		// COPYPASTE from IndicatorParameter.cs END
		
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public decimal ValueMouseOver {get; protected set;}
		
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public float FilledPercentageCurrentValue {get; protected set;}
		
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public float FilledPercentageMouseOver { get; protected set; }
		
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public string LabelText {get; set;}

		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		private string valueFormat;
		public string ValueFormat {
			get {
				if (valueFormat == null) {
					valueFormat = "0.#"; 
					if (this.ValueIncrement < 1) {
						// FIXME log10(0.008) = -2.09691
						valueFormat = "N" + Math.Round(Math.Abs(Math.Log10((double)this.ValueIncrement)));
					}
				}
				return valueFormat;
			}
			set { this.valueFormat = value; }
		}

		private bool borderOn;
		[Browsable(true)]
		public bool BorderOn {
			get { return (this.BorderStyle == BorderStyle.FixedSingle) ? true : false; }
			set { this.BorderStyle = (value == true) ? BorderStyle.FixedSingle : BorderStyle.None; }
		}
		

		[Browsable(true)]
		public Color ColorBgValueCurrentEnabled {
			get { return this.brushBgValueCurrentEnabled.Color; }
			set { this.brushBgValueCurrentEnabled = new SolidBrush(value); }
		}
		
		[Browsable(true)]
		public Color ColorBgValueCurrentDisabled {
			get { return this.brushBgValueCurrentDisabled.Color; }
			set { this.brushBgValueCurrentDisabled = new SolidBrush(value); }
		}
		
		[Browsable(true)]
		public Color ColorBgMouseOverEnabled {
			get { return this.brushBgMouseOverEnabled.Color; }
			set { this.brushBgMouseOverEnabled = new SolidBrush(value); }
		}

//		[Browsable(true)]
//		public Color ColorBgMouseOverDisabled {
//			get { return this.brushBgMouseOverDisabled.Color; }
//			set { this.brushBgMouseOverDisabled = new SolidBrush(value); }
//		}

		private Color colorFgParameterLabel;
		[Browsable(true)]
		public Color ColorFgParameterLabel {
			get { return this.colorFgParameterLabel; }
			set {
				this.colorFgParameterLabel = value;
				this.brushFgParameterLabel = new SolidBrush(value);
			}
		}
		
		protected override CreateParams CreateParams { get {
				CreateParams cp = base.CreateParams;
	            cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT
				return cp;
			} }
		
		public bool LeftToRight { get { return (this.ValueMin < this.ValueMax); } }

		public PanelFillSlider() : base() {
        	//SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
	        //base.BackColor = Color.FromArgb(0, 0, 0, 0);//Added this because image wasnt redrawn when resizing form
			this.SetStyle(
				ControlStyles.UserPaint |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.OptimizedDoubleBuffer, true);
		}

		//WHEN_INHERITED_FROM_REGULAR_USERCONTROL protected override void OnPaint(PaintEventArgs e) {
		protected override void OnPaintDoubleBuffered(PaintEventArgs pe) {
			int paddingLeft = 3;
			int paddingTop = 0;
			int paddingRight = 3;
			// base.ForeColor initialized after ctor(), in parent's InitializeComponents
			if (this.brushFgText == null) this.brushFgText = new SolidBrush(base.ForeColor);
			try {
				Graphics g = pe.Graphics;
				int valueMinWidthMeasured = (int)Math.Round(g.MeasureString(this.ValueMin.ToString(this.ValueFormat), this.Font).Width);
				int valueCurrentWidthMeasured = (int)Math.Round(g.MeasureString(this.ValueCurrent.ToString(this.ValueFormat), this.Font).Width);
				int valueMouseOverWidthMeasured = (int)Math.Round(g.MeasureString(this.ValueMouseOver.ToString(this.ValueFormat), this.Font).Width);
				int valueMaxWidthMeasured = (int)Math.Round(g.MeasureString(this.ValueMax.ToString(this.ValueFormat), this.Font).Width);
				int labelTextWidthMeasured = (int)Math.Round(g.MeasureString(this.LabelText + "", this.Font).Width);
				int labelYposition = base.Padding.Top + paddingTop;

				decimal range = Math.Abs(this.ValueMax - this.ValueMin);
				if (range == 0) {
					string msg = "ValueMax[" + this.ValueMax + "] - ValueMin[" + this.ValueMin + "] = 0";
					g.DrawString(msg, this.Font, this.brushFgText, base.Padding.Left, base.Padding.Top);
					return;
				}
				int widthMinusBorder = base.Width + base.Padding.Left + base.Padding.Right;
				
				if (this.LeftToRight) {		// INVERTED_SLIDER_ORIGINAL TODO: DO_PIXEL_PERFECT_REFACTORING
					float partOfRange = (float)(this.ValueCurrent - this.ValueMin) / (float)range;
					this.FilledPercentageCurrentValue = 100 * partOfRange;
					int widthToFillCurrentValue = (int)Math.Round(widthMinusBorder * partOfRange);
					
					g.FillRectangle(brushBgValueCurrent, 0, 0, widthToFillCurrentValue, base.Height);
					using (Pen curPurple = new Pen(base.ForeColor)) {
						g.DrawLine(curPurple, widthToFillCurrentValue, 0, widthToFillCurrentValue, base.Height);
					}

					int widthToFillMouseOver = 0;
					if (this.mouseOver) {
						widthToFillMouseOver = (int)Math.Round((widthMinusBorder * this.FilledPercentageMouseOver / 100));
						g.FillRectangle(brushBgMouseOver, 0, 0, widthToFillMouseOver, base.Height);
					}
	
					int valueCurrentXpos = widthToFillCurrentValue - valueCurrentWidthMeasured - base.Padding.Right - paddingRight;
					int valueMinEndsAt = valueMinWidthMeasured + base.Padding.Left;
					bool currentOverlapsMin = valueCurrentXpos < valueMinEndsAt;
					if (currentOverlapsMin == false) {
						g.DrawString(this.ValueMin.ToString(this.ValueFormat), this.Font, this.brushFgText, base.Padding.Left, labelYposition);
					}
	
					int labelTextXposition = base.Padding.Left + valueMinWidthMeasured + paddingLeft;
					int valueMaxXpos = widthMinusBorder - valueMaxWidthMeasured - base.Padding.Right - paddingRight;
					bool currentOverlapsMax = widthToFillCurrentValue > valueMaxXpos;
					if (currentOverlapsMax == false || this.mouseOver == true) {
						g.DrawString(this.ValueMax.ToString(this.ValueFormat), this.Font, this.brushFgText, valueMaxXpos, labelYposition);
// LOOKS LIKE NONSENSE here in ORIGINAL LeftToRight version (TODO: comment out and observe the "missing")
						if (labelTextXposition + labelTextWidthMeasured > valueMaxXpos) {
							// paint bg under MAX label if it overlaps long labelText
							if (brushBgControl == null) brushBgControl = new SolidBrush(base.BackColor);
							g.FillRectangle(brushBgControl, valueMaxXpos - base.Padding.Left, 0, valueMaxXpos + base.Padding.Right, base.Height);
						}
					}
	
					if (this.mouseOver == true && this.ValueMouseOver > this.ValueMin) {	// && this.ValueMouseOver < this.ValueMax) {
						g.DrawString(this.LabelText, this.Font, this.brushFgParameterLabel, labelTextXposition, labelYposition);
	
						int valueMouseOverXpos = widthToFillMouseOver - valueMouseOverWidthMeasured - base.Padding.Right - paddingRight;
						if (valueMouseOverXpos < base.Padding.Right + paddingRight) valueMouseOverXpos = base.Padding.Right + paddingRight;
						g.FillRectangle(brushBgMouseOver, valueMouseOverXpos, 0, valueMouseOverWidthMeasured, base.Height);
	
						g.DrawString(this.ValueMouseOver.ToString(this.ValueFormat), this.Font, this.brushFgText, valueMouseOverXpos, labelYposition);
					} else {
						g.DrawString(this.LabelText, this.Font, this.brushFgParameterLabel, labelTextXposition, labelYposition);
						g.DrawString(this.ValueCurrent.ToString(this.ValueFormat), this.Font, this.brushFgText, valueCurrentXpos, labelYposition);
					}
				} else {		// INVERTED_SLIDER_ORIGINAL_PORTED_BY_MIRRORRING; MAX and MIN INVERTED BELOW
					float partOfRange = (float)(this.ValueCurrent - this.ValueMax) / (float)range;
					this.FilledPercentageCurrentValue = 100 * partOfRange;
					int widthToFillCurrentValue = (int)Math.Round(widthMinusBorder * partOfRange);

					g.FillRectangle(brushBgValueCurrent, base.Width - widthToFillCurrentValue, 0, widthToFillCurrentValue, base.Height);
					using (Pen curPurple = new Pen(base.ForeColor)) {
						g.DrawLine(curPurple, widthToFillCurrentValue, 0, widthToFillCurrentValue, base.Height);
					}
	
					int widthToFillMouseOver = 0;
					if (this.mouseOver) {
						widthToFillMouseOver = (int)Math.Round((widthMinusBorder * this.FilledPercentageMouseOver / 100));
						g.FillRectangle(brushBgMouseOver, base.Width - this.Padding.Right - widthToFillMouseOver, 0, widthToFillMouseOver, base.Height);
					}
	
					int valueLeftXpos = widthMinusBorder - valueMinWidthMeasured - base.Padding.Right - paddingRight;
					bool currentOverlapsRight = widthToFillCurrentValue > valueLeftXpos;
					if (currentOverlapsRight == false) {
						g.DrawString(this.ValueMax.ToString(this.ValueFormat), this.Font, this.brushFgText, valueLeftXpos, labelYposition);
					}

					int labelTextXposition = base.Padding.Left + valueMaxWidthMeasured + paddingLeft;
					int valueCurrentXpos = base.Width - widthToFillCurrentValue + base.Padding.Left + paddingLeft;
					if (this.mouseOver == true) {		// && this.ValueMouseOver > this.ValueMax
						g.DrawString(this.LabelText, this.Font, this.brushFgParameterLabel, labelTextXposition, labelYposition);
	
						int valueMouseOverXpos = base.Width - widthToFillMouseOver + paddingLeft;
						int valueMouseOverXposStillVisible = base.Width - valueMouseOverWidthMeasured + base.Padding.Left - paddingLeft;
						if (valueMouseOverXpos > valueMouseOverXposStillVisible) valueMouseOverXpos = valueMouseOverXposStillVisible;
						g.FillRectangle(brushBgMouseOver, valueMouseOverXpos, 0, valueMouseOverWidthMeasured, base.Height);
	
						g.DrawString(this.ValueMouseOver.ToString(this.ValueFormat), this.Font, this.brushFgText, valueMouseOverXpos, labelYposition);
					} else {
						g.DrawString(this.LabelText, this.Font, this.brushFgParameterLabel, labelTextXposition, labelYposition);
						g.DrawString(this.ValueCurrent.ToString(this.ValueFormat), this.Font, this.brushFgText, valueCurrentXpos, labelYposition);
					}

					int valueLeftEndsAt = valueMaxWidthMeasured + base.Padding.Left + base.Padding.Right;
					bool currentOverlapsLeft = valueCurrentXpos < valueLeftEndsAt;
					if (currentOverlapsLeft == false || this.mouseOver == true) {
						g.DrawString(this.ValueMin.ToString(this.ValueFormat), this.Font, this.brushFgText, base.Padding.Left, labelYposition);
					}
				}
				//DOUBLEBUFFERED_PARENT_DID_THAT base.OnPaint(pe);
			} catch (Exception e) {
				string msg = "WindProc won't catch your exceptions; keep a breakpoint here";
			}
		}
		
//	param:	[0...P..........500]units 
//	width:	[0...X..........250]px
//	ratio:	500/250 = 2 px/unit								//PixelsForOnePercentage
//	mouse:	X = 60px						
//	%%:		60px / 250px = 0.24; 0.24 * 100 = 24%			//OnMouseMove
//	%%:		60px * 2px/unit = 120units; 120units / 500units = 0.24 * 100 = 24%
//	filled:	24% / 100 = 0.24; 250px * 0.24 = 60px
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if (e.X > base.Width) return;

			float range = (float) Math.Abs(this.ValueMax - this.ValueMin);
			decimal mouseRange;
			if (this.LeftToRight) {
				float partFilled = e.X / (float)base.Width;	//without (float) division of two ints is an int !!! (zero)
				if (partFilled > 1) {
					string msg = "PART_FILLED_MUST_BE_LESS_THAN_1=e.X[" + e.X + "]/base.Width[" + base.Width + "] MOUSE_MOVE_FROM_ANOTHER_CONTROL?";
					#if DEBUG
					Debugger.Break();
					#endif
					Assembler.PopupException(msg, null, false);
					return;
				}
				//this.ValueMouseOver = this.ValueMin + new decimal(partFilled * this.PixelsForOneValueUnit);
				this.ValueMouseOver = this.ValueMin + new decimal(partFilled * range);
				this.ValueMouseOver = this.RoundToClosestStep(this.ValueMouseOver);
				mouseRange = this.ValueMouseOver - this.ValueMin;
			} else {
				//float partFilled = (base.Width - e.X) / (float) base.Width;	//without (float) division of two ints is an int !!! (zero)
				float partFilled = (base.Width - e.X) / (float) base.Width;	//without (float) division of two ints is an int !!! (zero)
				//this.ValueMouseOver = this.ValueMax + new decimal(partFilled * this.PixelsForOneValueUnit);
				this.ValueMouseOver = this.ValueMax + new decimal(partFilled * range);
				this.ValueMouseOver = this.RoundToClosestStep(this.ValueMouseOver);
				mouseRange = this.ValueMouseOver - this.ValueMax;
			}
			if (leftMouseButtonHeldDown) {	// I_HATE_HACKING_F_WINDOWS_FORMS
				string msg = "DRAG_SIMULATION_AND_ON_DRAG_OVER_BOTH_DONT_WORK IF_YOU_SEE_THIS_SEND_A_SCREENSHOT_TO_DEVELOPER";
				//Debugger.Break();
				Assembler.PopupException(msg);
				if (this.ValueCurrent != this.ValueMouseOver) {
					//Debugger.Break();
					this.ValueCurrent = this.ValueMouseOver;
				}
			}
			this.FilledPercentageMouseOver = 100 * ((float)mouseRange / range);
			base.Invalidate();
		}
		// I_HATE_HACKING_F_WINDOWS_FORMS
		protected override void OnDragOver(System.Windows.Forms.DragEventArgs e) {
			base.OnDragOver(e);
			this.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, e.X, e.Y, 0));
			if (this.ValueCurrent != this.ValueMouseOver) {
				Debugger.Break();
				this.ValueCurrent = this.ValueMouseOver;
			}
		}
		public decimal RoundToClosestStep(decimal rawValue) {
			// rawValue = 61, ValueMin=30, ValueIncrement=20
			//rawValue = (decimal) 6.1;
			decimal distFromMin = Math.Abs(rawValue - this.ValueMin);							// distFromMin = 61 - 30 = 31
			decimal valueStepSafe = (this.ValueIncrement != 0) ? this.ValueIncrement : 1;		// valueStepSafe = 20
			//int fullSteps = (int)Math.Floor(distFromMin / valueStepSafe);						// fullSteps = floor(31 / 20) = floor(1.55) = 1
			//decimal ret = this.ValueMin + valueStepSafe * fullSteps;							// ret = 3 + 2 * 1 = 3 + 2 = 5

			decimal stepsRounded = Math.Round(distFromMin / valueStepSafe);						// stepsRounded = round(31 / 20) = floor(1.55) = 2
			decimal ret = this.ValueMin + valueStepSafe * (decimal)Math.Round(stepsRounded);	// ret = 3 + 2 * 1 = 3 + 2 = 5

			//Rounding of 5.6 down to 5 in terms of levels 3-5-7 with step=2; rounding between 0...1 could've been done with Math.Round() which isn't the case
			//decimal fullStepsRemainder = distFromMin % valueStepSafe;
			//decimal halfStep = (decimal) ((double) valueStepSafe / (double) 2);	// THE_ONLY_THING_I_HATE_IN_DOT_NET

			//if (fullStepsRemainder > halfStep) {
			//	ret += this.ValueIncrement;
			//}

			if (ret > this.ValueMax) {
				Debugger.Break();
				return rawValue;
			}
			if (ret < this.ValueMin) {
				Debugger.Break();
				return rawValue;
			}
			return ret;
		}
		
		private bool mouseOver = false;
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			this.mouseOver = true;
			base.Invalidate();
		}		
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			if (rightClickShouldKeepMouseOver == true) {
				rightClickShouldKeepMouseOver = false;
				return;
			}
			this.mouseOver = false;
			this.leftMouseButtonHeldDown = false; 
			base.Invalidate();
		}
		bool leftMouseButtonHeldDown = false;
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if (e.Button == MouseButtons.Left) {
				leftMouseButtonHeldDown = true;	// I_HATE_HACKING_F_WINDOWS_FORMS
			}
			if (this.ValueCurrent != this.ValueMouseOver && leftMouseButtonHeldDown) {
				this.ValueCurrent = this.ValueMouseOver;
			}
		}
		bool rightClickShouldKeepMouseOver = false;
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if (e.Button == MouseButtons.Left) {
				leftMouseButtonHeldDown = false; 
			}
			if (e.Button != MouseButtons.Left) {
				rightClickShouldKeepMouseOver = true;
				return;
			}
			//this.FilledPercentageCurrentValue = this.FilledPercentageMouseOver;
			// we Raise on MouseDown and MouseDrag now
//			if (this.ValueCurrent != this.ValueMouseOver && this.mouseOver) {
//				this.ValueCurrent = this.ValueMouseOver;	// we Raise on MouseDown and MouseDrag now
//			}
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (this.brushBgValueCurrent != null) this.brushBgValueCurrent.Dispose();
				if (this.brushBgMouseOver != null) this.brushBgMouseOver.Dispose();
				if (this.brushBgValueCurrentDisabled != null) this.brushBgValueCurrentDisabled.Dispose();
				//if (this.brushBgMouseOverDisabled != null) this.brushBgMouseOverDisabled.Dispose();
				if (this.brushFgParameterLabel != null) this.brushFgParameterLabel.Dispose();
				if (this.brushFgText != null) this.brushFgText.Dispose();
				if (this.brushBgControl != null) this.brushBgControl.Dispose();
			}
			base.Dispose(disposing);
		}

	}
}