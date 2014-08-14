using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Sq1.Core.DoubleBuffered;

namespace Sq1.Widgets.SteppingSlider {
	public class PanelFillSlider : PanelDoubleBuffered {
	    private SolidBrush brushBgValueCurrentEnabled = new SolidBrush(System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255))))));
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
		public decimal ValueStep {
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
			set { valueCurrent = value; this.Invalidate(); }
		}
		
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public decimal ValueMouseOver {get; protected set;}
		
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public float FilledPercentageCurrentValue {get; protected set;}
		
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public float FilledPercentageMouseOver { get; protected set; }
		
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public string LabelText {get;set;}

		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		private string valueFormat;
		public string ValueFormat {
			get { if (valueFormat == null) return "0.#";  return valueFormat; }
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
			get {
				return this.colorFgParameterLabel;
			}
			set {
				this.colorFgParameterLabel = value;
				this.brushFgParameterLabel = new SolidBrush(value);
			}
		}
		
		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
	            cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT
				return cp;
			}
		}

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
				int valueMinMeasured = (int)Math.Round(g.MeasureString(this.ValueMin.ToString(this.ValueFormat), this.Font).Width);
				int valueCurrentMeasured = (int)Math.Round(g.MeasureString(this.ValueCurrent.ToString(this.ValueFormat), this.Font).Width);
				int valueMouseOverMeasured = (int)Math.Round(g.MeasureString(this.ValueMouseOver.ToString(this.ValueFormat), this.Font).Width);
				int valueMaxMeasured = (int)Math.Round(g.MeasureString(this.ValueMax.ToString(this.ValueFormat), this.Font).Width);
				int labelTextMeasured = (int)Math.Round(g.MeasureString(this.LabelText + "", this.Font).Width);
				int labelXposition = base.Padding.Left + valueMinMeasured + paddingLeft;
				int labelYposition = base.Padding.Top + paddingTop;

				decimal range = Math.Abs(this.ValueMax - this.ValueMin);
				if (range == 0) {
					string msg = "ValueMax[" + this.ValueMax + "] - ValueMin[" + this.ValueMin + "] = 0";
					g.DrawString(msg, this.Font, this.brushFgText, base.Padding.Left, base.Padding.Top);
					return;
				}
				float partOfRange = (float)(this.ValueCurrent - this.ValueMin) / (float)range;
				this.FilledPercentageCurrentValue = 100 * partOfRange;
				int widthMinusBorder = base.Width + base.Padding.Left + base.Padding.Right;
				int widthToFillCurrentValue = (int)Math.Round(widthMinusBorder * partOfRange);
				g.FillRectangle(brushBgValueCurrent, 0, 0, widthToFillCurrentValue, base.Height);

				int widthToFillMouseOver = 0;
				if (this.mouseOver) {
					widthToFillMouseOver = (int)Math.Round((widthMinusBorder * this.FilledPercentageMouseOver / 100));
					g.FillRectangle(brushBgMouseOver, 0, 0, widthToFillMouseOver, base.Height);
				}

				int valueCurrentXpos = widthToFillCurrentValue - valueCurrentMeasured - base.Padding.Right - paddingRight;
				int valueMinEndsAt = valueMinMeasured + base.Padding.Left;
				bool currentOverlapsMin = valueCurrentXpos < valueMinEndsAt;
				if (currentOverlapsMin == false) {
					g.DrawString(this.ValueMin.ToString(this.ValueFormat), this.Font, this.brushFgText, base.Padding.Left, labelYposition);
				}

				int valueMaxXpos = widthMinusBorder - valueMaxMeasured - base.Padding.Right - paddingRight;
				bool currentOverlapsMax = widthToFillCurrentValue > valueMaxXpos;
				if (currentOverlapsMax == false || this.mouseOver == true) {
					g.DrawString(this.ValueMax.ToString(this.ValueFormat), this.Font, this.brushFgText, valueMaxXpos, labelYposition);
					if (labelXposition + labelTextMeasured > valueMaxXpos) {
						// paint bg under MAX label if it overlaps long labelText
						if (brushBgControl == null) brushBgControl = new SolidBrush(base.BackColor);
						g.FillRectangle(brushBgControl, valueMaxXpos - base.Padding.Left, 0, valueMaxXpos + base.Padding.Right, base.Height);
					}
				}

				if (this.mouseOver == true && this.ValueMouseOver > this.ValueMin) {	// && this.ValueMouseOver < this.ValueMax) {
					g.DrawString(this.LabelText, this.Font, this.brushFgParameterLabel, labelXposition, labelYposition);

					int valueMouseOverXpos = widthToFillMouseOver - valueMouseOverMeasured - base.Padding.Right - paddingRight;
					if (valueMouseOverXpos < 0) valueMouseOverXpos = 0;

					int widthToOverlalNumbers = 5;
					int rectXpos = valueMouseOverXpos;	// -base.Padding.Left - paddingLeft;
					int rectWidth = valueMouseOverMeasured;
					rectXpos -= widthToOverlalNumbers;
					if (rectXpos < 0) rectXpos = 0;
					rectWidth += widthToOverlalNumbers*2;
					if (rectXpos + rectWidth > widthMinusBorder) rectWidth = widthMinusBorder - rectXpos;
					g.FillRectangle(brushBgMouseOver, rectXpos, 0, rectWidth, base.Height);

					g.DrawString(this.ValueMouseOver.ToString(this.ValueFormat), this.Font, this.brushFgText, valueMouseOverXpos, labelYposition);
				} else {
					g.DrawString(this.LabelText, this.Font, this.brushFgParameterLabel, labelXposition, labelYposition);
					g.DrawString(this.ValueCurrent.ToString(this.ValueFormat), this.Font, this.brushFgText, valueCurrentXpos, labelYposition);
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
			float partFilled = e.X / (float) base.Width;	//without (float) division of two ints is an int !!! (zero)
			//this.ValueMouseOver = this.ValueMin + new decimal(partFilled * this.PixelsForOneValueUnit);
			float range = (float) Math.Abs(this.ValueMax - this.ValueMin);
			this.ValueMouseOver = this.ValueMin + new decimal(partFilled * range);
			this.ValueMouseOver = this.RoundToClosestStep(this.ValueMouseOver);
			this.FilledPercentageMouseOver = 100 * ((float)Math.Round(this.ValueMouseOver - this.ValueMin) / range);
			base.Invalidate();
			base.OnMouseMove(e);
		}
		
		public decimal RoundToClosestStep(decimal rawValue) {
			decimal valueStepSafe = (this.ValueStep != 0) ? this.ValueStep : 1;
			int fullSteps = (int)Math.Floor(rawValue / valueStepSafe);
			decimal fullStepsReminder = rawValue % valueStepSafe;
			decimal halfStep = valueStepSafe / 2;
			if (fullStepsReminder < halfStep) {
				return valueStepSafe * fullSteps;
			} else {
				return valueStepSafe * (fullSteps + 1);
			}
		}
		
		private bool mouseOver = false;
		protected override void OnMouseEnter(EventArgs e) {
			this.mouseOver = true;
			base.Invalidate();
			base.OnMouseEnter(e);
		}
		
		protected override void OnMouseLeave(EventArgs e) {
			if (rightClickShouldKeepMouseOver == true) {
				rightClickShouldKeepMouseOver = false;
				base.OnMouseLeave(e);
				return;
			}
			this.mouseOver = false;
			base.Invalidate();
			base.OnMouseLeave(e);
		}

		bool rightClickShouldKeepMouseOver = false;
		protected override void OnMouseUp(MouseEventArgs e) {
			if (e.Button != MouseButtons.Left) {
				rightClickShouldKeepMouseOver = true;
				return;
			}
			//this.FilledPercentageCurrentValue = this.FilledPercentageMouseOver;
			this.ValueCurrent = this.ValueMouseOver;
			base.Invalidate();
			base.OnMouseUp(e);
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