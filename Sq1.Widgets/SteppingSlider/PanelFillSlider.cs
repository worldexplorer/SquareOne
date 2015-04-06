using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DoubleBuffered;
using Sq1.Core.Indicators;

namespace Sq1.Widgets.SteppingSlider {
	public partial class PanelFillSlider : PanelDoubleBuffered {
		SolidBrush brushBgValueCurrentEnabled = new SolidBrush(System.Drawing.Color.FromArgb(192, 192, 255));
		SolidBrush brushBgValueCurrentDisabled = new SolidBrush(Color.DarkGray);
		SolidBrush brushBgValueCurrent { get { return base.Enabled ? brushBgValueCurrentEnabled : brushBgValueCurrentDisabled; } }
		
		SolidBrush brushBgMouseOverEnabled = new SolidBrush(Color.LightGreen);
		SolidBrush brushBgMouseOverDisabled = new SolidBrush(Color.DarkGreen);
		SolidBrush brushBgMouseOver { get { return base.Enabled ? brushBgMouseOverEnabled : brushBgMouseOverDisabled; } }

		SolidBrush brushFgParameterLabel = new SolidBrush(Color.White);
		SolidBrush brushFgText;
		SolidBrush brushBgControl;
		
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		decimal valueMin;
		public decimal ValueMin {
			get { return valueMin; }
			set { valueMin = value; this.Invalidate(); }
		}
	
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		decimal valueStep;
		public decimal ValueIncrement {
			get { return valueStep; }
			set { valueStep = value; this.Invalidate(); }
		}
		
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		decimal valueMax;
		public decimal ValueMax {
			get { return valueMax; }
			set { valueMax = value; this.Invalidate(); }
		}

		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		decimal valueCurrent;
		public decimal ValueCurrent {
			get { return valueCurrent; }
			set {
				if (value > this.ValueMax) {
					string msg = "I_REFUSE_OUT_OF_BOUNDARY_ASSIGNMENT value[" + value + "] > this.ValueMax[" + this.ValueMax + "]";
					Assembler.PopupException(msg);
					return;
				}
				if (value < this.ValueMin) {
					string msg = "I_REFUSE_OUT_OF_BOUNDARY_ASSIGNMENT value[" + value + "] < this.ValueMin[" + this.ValueMin + "]";
					Assembler.PopupException(msg);
					return;
				}
				decimal roundedChangesSliders = this.RoundToClosestStep(value);
				if (value != roundedChangesSliders) {
					//Debugger.Break();
				}
				valueCurrent = roundedChangesSliders;
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
			return this.LabelText + ":" + this.ValueCurrent + "[" + this.ValueMin + ".." + this.ValueMax + "/" + this.ValueIncrement + "]";
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

		//[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		string valueFormat;
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
			//set { this.valueFormat = value; }
		}

		bool borderOn;
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

		Color colorFgParameterLabel;
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

		IndicatorParameter parameterJustForDebugging;
		public void SetParameterForDebuggingOnly(IndicatorParameter parameterJustForDebugging) {
			this.parameterJustForDebugging = parameterJustForDebugging;
		}
		public PanelFillSlider() : base() {
			//SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
			//base.BackColor = Color.FromArgb(0, 0, 0, 0);//Added this because image wasnt redrawn when resizing form
			//this.SetStyle(
			//    ControlStyles.UserPaint |
			//    ControlStyles.AllPaintingInWmPaint |
			//    ControlStyles.OptimizedDoubleBuffer, true);
		}
		public decimal RoundToClosestStep(decimal rawValue) {
			string msig = " //RoundToClosestStep(" + rawValue + ")";

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
				string msg = "ret(" + ret + ") > this.ValueMax(" + this.ValueMax + ")";
				Assembler.PopupException(msg + msig, null, false);
				return rawValue;
			}
			if (ret < this.ValueMin) {
				string msg = "ret(" + ret + ") < this.ValueMin(" + this.ValueMin + ")";
				Assembler.PopupException(msg + msig, null, false);
				return rawValue;
			}
			return ret;
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
