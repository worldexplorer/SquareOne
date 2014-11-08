using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core.DoubleBuffered;

namespace Sq1.Widgets.SteppingSlider {
	public partial class SliderComboControl : UserControlDoubleBuffered {
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public decimal ValueMin {
			get { return this.PanelFillSlider.ValueMin; }
			set { this.PanelFillSlider.ValueMin = value; }
		}
		
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public decimal ValueIncrement {
			get { return this.PanelFillSlider.ValueIncrement; }
			set { this.PanelFillSlider.ValueIncrement = value; }
		}
		
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public decimal ValueCurrent {
			get { return this.PanelFillSlider.ValueCurrent; }
			set {
				this.PanelFillSlider.ValueCurrent = value;
				this.mniltbValueCurrent.InputFieldValue = this.format(this.PanelFillSlider.ValueCurrent);
				this.DomainUpDown.Text = this.format(this.PanelFillSlider.ValueCurrent);
				// numericUpDown clicked causes double backtest and Disposes TSI menu items this.RaiseValueChanged();
			}
		}
		string format(decimal value) {
			if (this.ValueFormat == null) {
				if (this.PanelFillSlider.ValueFormat != null) value.ToString(this.PanelFillSlider.ValueFormat); 
				return value.ToString();
			}
			return value.ToString(this.ValueFormat);
		}
		
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public decimal ValueMax {
			get { return this.PanelFillSlider.ValueMax; }
			set { this.PanelFillSlider.ValueMax = value; }
		}
		
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public int FilledPercentageCurrentValue {get;set;}
		
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public int FilledPercentageMouseOver {get;set;}
		
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public String LabelText {
			get { return this.PanelFillSlider.LabelText;}
			set { this.PanelFillSlider.LabelText = value; }
		}

		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public float LabelFilledPercentage {
			get { return this.PanelFillSlider.FilledPercentageCurrentValue; }
			//set { this.panelWithBackground1.FilledPercentageCurrentValue = value; }
		}
		
		[Browsable(true)]
		public Color ColorBgValueCurrent {
			get { return this.PanelFillSlider.ColorBgValueCurrentEnabled; }
			set { this.PanelFillSlider.ColorBgValueCurrentEnabled = value; }
		}
		
		[Browsable(true)]
		public Color ColorBgMouseOverEnabled {
			get { return this.PanelFillSlider.ColorBgMouseOverEnabled; }
			set { this.PanelFillSlider.ColorBgMouseOverEnabled = value; }
		}

//		[Browsable(true)]
//		public Color ColorBgMouseOverDisabled {
//			get { return this.PanelFillSlider.ColorBgMouseOverDisabled; }
//			set { this.PanelFillSlider.ColorBgMouseOverDisabled = value; }
//		}

		[Browsable(true)]
		public Color ColorFgParameterLabel {
			get { return this.PanelFillSlider.ColorFgParameterLabel; }
			set { this.PanelFillSlider.ColorFgParameterLabel = value; }
		}

		[Browsable(true)]
		public Color ColorFgValues {
			get { return this.PanelFillSlider.ForeColor; }
			set { this.PanelFillSlider.ForeColor = value; }
		}

		[Browsable(true)]
		public Padding PaddingPanelSlider {
			get { return this.PanelFillSlider.Padding; }
			set { this.PanelFillSlider.Padding = value; }
		}

		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public String ValueFormat;
			
		public SliderComboControl() {
			InitializeComponent();
		}

		[Browsable(true)]
		public bool EnableBorder {
			get { return this.PanelFillSlider.BorderOn; }
			set { 
				this.PanelFillSlider.BorderOn = value;
				int offset = this.PanelFillSlider.BorderOn ? -1 : 1;
				int offsetLeft = this.PanelFillSlider.Padding.Left + offset;
				if (offsetLeft < 0) offsetLeft = 0;
				int offsetTop = this.PanelFillSlider.Padding.Top + offset;
				if (offsetTop < 0) offsetTop = 0;
				int offsetRight = this.PanelFillSlider.Padding.Right + offset;
				if (offsetRight < 0) offsetRight = 0;
				int offsetBottom = this.PanelFillSlider.Padding.Bottom + offset;
				if (offsetBottom < 0) offsetBottom = 0;
				Padding newPadding = new Padding(offsetLeft, offsetTop, offsetRight, offsetBottom);
				this.PanelFillSlider.Padding = newPadding;
				//this.mniDisableBorder.Text = this.panelFillSlider.BorderOn ? "Hide Slider Border" : "Show Slider Border";
				this.mniShowBorder.Checked = this.PanelFillSlider.BorderOn;
			}
		}

		[Browsable(true)]
		public bool EnableNumeric {
			get { return !this.splitContainer1.Panel1Collapsed; }
			set {
				this.splitContainer1.Panel1Collapsed = !value;
				// disables cursor on hidden field and makes keybord ArrowUp & ArrowDown not affect the value
				this.DomainUpDown.Enabled = value;
				//this.mniDisableNumeric.Text = this.splitContainer1.Panel1Collapsed ? "Show Numeric Field" : "Hide Numeric Field";
				this.mniShowNumeric.Checked = this.EnableNumeric;
			}
		}

		[Browsable(true)]
		public PanelFillSlider InnerPanelFillSlider {
			get { return this.PanelFillSlider; }
			set { this.PanelFillSlider = value; }
		}

		[Browsable(true)]
		public DomainUpDownWithMouseEvents InnerDomainUpDownWithMouseEvents {
			get { return this.DomainUpDown; }
			set { this.DomainUpDown = value; }
		}
		
		[Browsable(true)]
		public new bool Enabled {
			get { return base.Enabled; }
			set { base.Enabled = value; base.Invalidate(); }
		}
		
		//v2  SteppingSlider_RTL branch: SteppingSlider if Max < Min then draw filled part from right to left
		public decimal ValueMinRtlSafe { get { return Math.Min(this.ValueMin, this.ValueMax); } }
		public decimal ValueMaxRtlSafe { get { return Math.Max(this.ValueMin, this.ValueMax); } }

		public override string ToString() {
			string ret = "UNDERLYING_PANEL_FILL_SLIDER_NOT_YET_INITIALIZED";
			if (this.PanelFillSlider == null) return ret;
			return this.PanelFillSlider.ToString();
		}
	}
}