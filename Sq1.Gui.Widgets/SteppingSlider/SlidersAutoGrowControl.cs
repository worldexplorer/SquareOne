using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DoubleBuffered;
using Sq1.Core.StrategyBase;

namespace Sq1.Widgets.SteppingSlider {
	public partial class SlidersAutoGrowControl : UserControlDoubleBuffered {
		public Strategy Strategy { get; private set; }

		[Browsable(true)]
		public new bool Enabled {
			get { return base.Enabled; }
			set {
				base.Enabled = value;
				foreach (Control child in base.Controls) child.Enabled = base.Enabled;
			}
		}
		public int PreferredHeight { get {
				int ret = 0;
				foreach (UserControl control in base.Controls) ret += control.Height + this.VerticalSpaceBetweenSliders;
				return ret;
			} }
		public List<SliderComboControl> SlidersScriptParameters { get {
				List<SliderComboControl> ret = new List<SliderComboControl>();
				foreach (Control mustBeSliderCombo in base.Controls) {
					SliderComboControl slider = mustBeSliderCombo as SliderComboControl;
					if (slider == null) continue;
					ret.Add(slider);
				}
				return ret;
			} }
		public Dictionary<string, double> CurrentParametersFromChildSliders {
			get {
				if (base.DesignMode == true) return null;
				Dictionary<string, double> ret = new Dictionary<string, double>();
				foreach (SliderComboControl slider in this.SlidersScriptParameters) {
					ScriptParameter parameter = slider.Tag as ScriptParameter;
					ret.Add(parameter.Name, (double)slider.ValueCurrent);
				}
				return ret;
			}
			set { }
		}
		
		[Browsable(true)]
		public int VerticalSpaceBetweenSliders { get; set; }

		[Browsable(true)]
		public bool AllSlidersBorderShownByDefault { get; set; }

		[Browsable(true)]
		public bool AllSlidersNumericShownByDefault { get; set; }

		public SlidersAutoGrowControl() {
			InitializeComponent();
			// JUST_IN_CASE_IF_DESIGNER_REMOVED_COMMENTED_LINES, v1 contained this.mniParameterBagLoad as well:
			// IF_UNCOMMENT_DONT_FORGET_TO_CLEAN_AFTER_INITALIZE_COMPONENTS: this.ctxOperations.Items.Clear();
			//this.ctxOperations.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			//						//FIRST_LEVEL_MNI_HAS_VISUAL_TICK this.mniParameterBagLoad,
			//						this.mniltbParameterBagRenameTo,
			//						this.mniltbParameterBagDuplicateTo,
			//						this.mniParameterBagDelete});

			//WindowsFormsUtils.SetDoubleBuffered(this);
		}

		public void Initialize(Strategy strategy) {
			this.Strategy = strategy;

			base.SuspendLayout();
			foreach (UserControl control in base.Controls) control.Dispose();
			base.Controls.Clear();
			try {
				if (this.Strategy == null) return;
				if (this.Strategy.Script == null) return;
				//v1 TOO_SMART_INCOMPATIBLE_WITH_LIFE_SPENT_4_HOURS_DEBUGGING DESERIALIZED_STRATEGY_HAD_PARAMETERS_NOT_INITIALIZED INITIALIZED_BY_SLIDERS_AUTO_GROW_CONTROL
				// foreach (ScriptParameter parameter in this.Strategy.ScriptParametersMergedWithCurrentContext.Values) {
				foreach (ScriptParameter parameter in this.Strategy.Script.ParametersById.Values) {
					SliderComboControl slider = this.SliderComboFactory(parameter);
					base.Controls.Add(slider);		// later accessible by this.SlidersScriptParameters
				}
			} finally {
				base.Height = this.PreferredHeight;
				base.ResumeLayout(true);
			}
		}
		
		private void syncMniAllParamsShowBorderAndNumeric() {
			bool atLeastOneBorderShown = false;
			bool atLeastOneNumericShown = false;

			foreach (SliderComboControl slider in this.SlidersScriptParameters) {
				if (slider.EnableBorder) atLeastOneBorderShown = true;
				if (slider.EnableNumeric) atLeastOneNumericShown = true;
			}

			this.mniAllParamsShowBorder.Checked = atLeastOneBorderShown;
			this.mniAllParamsShowBorder.Text = atLeastOneBorderShown ? "All Params -> HideBorder" : "All Params -> ShowBorder";

			this.mniAllParamsShowNumeric.Checked = atLeastOneNumericShown;
			this.mniAllParamsShowNumeric.Text = atLeastOneNumericShown ? "All Params -> HideNumeric" : "All Params -> ShowNumeric";
		}

		private SliderComboControl SliderComboFactory(ScriptParameter parameter) {
			//v1 WOULD_BE_TOO_EASY ret = this.templateSliderControl.Clone();
			//BEGIN merged with SlidersAutoGrow.Designer.cs:InitializeComponent()
			SliderComboControl ret = new SliderComboControl();
			//SCHEMA1
			//ret.ColorBgMouseOver = System.Drawing.Color.Gold;
			//ret.ColorBgValueCurrent = System.Drawing.SystemColors.ActiveCaption;
			//ret.ColorFgParameterLabel = System.Drawing.Color.RoyalBlue;
			//ret.ColorFgValues = System.Drawing.Color.Lime;
			//SCHEMA2
			//ret.ColorBgMouseOver = System.Drawing.Color.Gold;
			//ret.ColorBgValueCurrent = System.Drawing.Color.LightSteelBlue;
			//ret.ColorFgParameterLabel = System.Drawing.Color.AliceBlue;
			//ret.ColorFgValues = System.Drawing.Color.Magenta;
			//SCHEMA3
			//ret.ColorBgMouseOver = System.Drawing.Color.Thistle;
			//ret.ColorBgValueCurrent = System.Drawing.Color.LightSteelBlue;
			//ret.ColorFgParameterLabel = System.Drawing.Color.White;
			//ret.ColorFgValues = System.Drawing.Color.DeepPink;
			//Designer!
			ret.ColorBgMouseOverEnabled = this.templateSliderControl.ColorBgMouseOverEnabled;
//			ret.ColorBgMouseOverDisabled = this.templateSliderControl.ColorBgMouseOverDisabled;
			ret.ColorBgValueCurrent = this.templateSliderControl.ColorBgValueCurrent;
			ret.ColorFgParameterLabel = this.templateSliderControl.ColorFgParameterLabel;
			ret.ColorFgValues = this.templateSliderControl.ColorFgValues;

			//ret.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
			//ret.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			ret.Anchor = this.templateSliderControl.Anchor;
			ret.Padding = this.templateSliderControl.Padding;
			ret.PaddingPanelSlider = this.templateSliderControl.PaddingPanelSlider;
			//END merged

			ret.LabelText = parameter.Name;
			ret.Name = "parameter_" + parameter.Name;
			ret.ValueCurrent = new decimal(parameter.ValueCurrent);
			ret.ValueMax = new decimal(parameter.ValueMax);
			ret.ValueMin = new decimal(parameter.ValueMin);
			ret.ValueIncrement = new decimal(parameter.ValueIncrement);
			//DOESNT_WORK?... ret.PanelFillSlider.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
			//ret.PaddingPanelSlider = new System.Windows.Forms.Padding(0, 1, 0, 0);
			ret.Location = new System.Drawing.Point(0, this.PreferredHeight + this.VerticalSpaceBetweenSliders);
			ret.Size = new System.Drawing.Size(this.Width, ret.Size.Height);
			ret.Tag = parameter;
			ret.ValueCurrentChanged += slider_ValueCurrentChanged;
			// WILL_ADD_PARENT_MENU_ITEMS_IN_Opening
			
			if (this.Strategy.SliderBordersShownByParameterId.ContainsKey(parameter.Id)) {
				ret.EnableBorder = this.Strategy.SliderBordersShownByParameterId[parameter.Id];
			} else {
				ret.EnableBorder = this.AllSlidersBorderShownByDefault;
			}
			if (this.Strategy.SliderNumericUpdownsShownByParameterId.ContainsKey(parameter.Id)) {
				ret.EnableNumeric = this.Strategy.SliderNumericUpdownsShownByParameterId[parameter.Id];
			} else {
				ret.EnableNumeric = this.AllSlidersNumericShownByDefault;
			}
			
			ret.ShowBorderChanged += slider_ShowBorderChanged;
			ret.ShowNumericUpdownChanged += slider_ShowNumericUpdownChanged;
			
			return ret;
		}
	}
}
