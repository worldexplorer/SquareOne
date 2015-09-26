using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core.DoubleBuffered;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Widgets.SteppingSlider {
	//public partial class SlidersAutoGrowControl : UserControlDoubleBuffered {
	public partial class SteppingSlidersAutoGrowControl : UserControl {
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
		public List<SteppingSliderComboControl> SlidersScriptAndIndicatorParameters { get {
				List<SteppingSliderComboControl> ret = new List<SteppingSliderComboControl>();
				foreach (Control mustBeSliderCombo in base.Controls) {
					SteppingSliderComboControl slider = mustBeSliderCombo as SteppingSliderComboControl;
					if (slider == null) continue;
					ret.Add(slider);
				}
				return ret;
			} }

		[Browsable(true)]
		public int VerticalSpaceBetweenSliders { get; set; }

		public SteppingSlidersAutoGrowControl() {
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
			if (this.skipSlidersFactory_changingValueDoesntChangeNumberOfSliders) return;

			this.Strategy = strategy;

			base.SuspendLayout();
			foreach (UserControl control in base.Controls) {
				SteppingSliderComboControl  slider = control as SteppingSliderComboControl;
				if (slider == null) {
					string msg = "let context menus with items live; dispose only the sliders since you're going to rebuild them"
						+ "; remember you blew up on this.mniAllParamsResetToScriptDefaults.IsDisposed in TsiDynamic {get{}} in commit fb4c86e31ab6d67fa8b9aad2756e0f4b9c14d4db ?...";
					continue;
				}
				//base.Controls.Remove(slider);
				slider.Dispose();
			}
			// MOVED_UP_2_LINES NOT_ENOUGH__EVERY_SLIDER_CLICK_WHOLE_BAG_MOVES_DOWN (!!!)
			base.Controls.Clear();
			try {
				if (this.Strategy == null) return;
				if (this.Strategy.Script == null) return;

				//v1
				//Dictionary<string, IndicatorParameter> parametersByName = this.Strategy.Script.IndicatorsParametersInitializedInDerivedConstructorByNameForSliders;	// dont make me calculate it twice 
				//foreach (string indicatorNameDotParameterName in parametersByName.Keys) {																// #1
				//    IndicatorParameter parameter = parametersByName[indicatorNameDotParameterName];														// #2
				//    parameter.IndicatorName = indicatorNameDotParameterName.Substring(0, indicatorNameDotParameterName.IndexOf('.'));
				//    SliderComboControl slider = this.SliderComboFactory(parameter, indicatorNameDotParameterName);
				//    base.Controls.Add(slider);		// later accessible by this.SlidersScriptParameters
				//}
				//foreach (ScriptParameter parameter in this.Strategy.ScriptContextCurrent.ScriptParametersById.Values) {
				//    SliderComboControl slider = this.SliderComboFactory(parameter);
				//    base.Controls.Add(slider);		// later accessible by this.SlidersScriptParameters
				//    string mustBeMe = slider.Parent.ToString();
				//    if (mustBeMe != this.ToString()) {
				//        System.Diagnostics.Debugger.Break();
				//        string msg = "WHO_THEN???";
				//    }
				//}

				//v2
				IndicatorParameter parameterPrevToFeelChangeAndAddSpacing = null;
				List<IndicatorParameter> parameters = this.Strategy.ScriptContextCurrent.ScriptAndIndicatorParametersMergedUnclonedForSequencerAndSliders;	// dont make me calculate it twice 
				foreach (IndicatorParameter param in parameters) {
					if (parameterPrevToFeelChangeAndAddSpacing == null) {
						parameterPrevToFeelChangeAndAddSpacing = param;
					}
					if (parameterPrevToFeelChangeAndAddSpacing.GetType() != param.GetType()) {
						this.addSpacingBeforeIndicatorParameters();
					}
					parameterPrevToFeelChangeAndAddSpacing = param;
					SteppingSliderComboControl slider = this.SliderComboFactory(param);
					base.Controls.Add(slider);		// later accessible by this.SlidersScriptParameters
				}

				// while switching ActiveDocument (ChartForm), tsiScriptContextsDynamic.Clear() and don't complain about missing ScriptContexts
				this.tsiScriptContextsDynamic.Clear();
			} finally {
				base.Height = this.PreferredHeight;
				base.ResumeLayout(true);
			}
		}

		void addSpacingBeforeIndicatorParameters() {
			Panel ret = new Panel();
			ret.Size = new System.Drawing.Size(this.Width, 8);
			//base.Controls.Add(ret);
		}
		
		void syncMniAllParamsShowBorderAndNumeric() {
			bool atLeastOneBorderShown = false;
			bool atLeastOneNumericShown = false;

			foreach (SteppingSliderComboControl slider in this.SlidersScriptAndIndicatorParameters) {
				if (slider.EnableBorder) atLeastOneBorderShown = true;
				if (slider.EnableNumeric) atLeastOneNumericShown = true;
			}

			this.mniAllParamsShowBorder.Checked = atLeastOneBorderShown;
			this.mniAllParamsShowBorder.Text = atLeastOneBorderShown ? "All Params -> HideBorder" : "All Params -> ShowBorder";

			this.mniAllParamsShowNumeric.Checked = atLeastOneNumericShown;
			this.mniAllParamsShowNumeric.Text = atLeastOneNumericShown ? "All Params -> HideNumeric" : "All Params -> ShowNumeric";
		}

		SteppingSliderComboControl SliderComboFactory(IndicatorParameter indicatorOrScriptParameter, string indicatorNameDotParameterName = null) {
			//v1 WOULD_BE_TOO_EASY ret = this.templateSliderControl.Clone();
			//BEGIN merged with SlidersAutoGrow.Designer.cs:InitializeComponent()
			SteppingSliderComboControl ret = new SteppingSliderComboControl();
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
//			ret.ColorBgMouseOverEnabled = this.templateSliderControl.ColorBgMouseOverEnabled;
////			ret.ColorBgMouseOverDisabled = this.templateSliderControl.ColorBgMouseOverDisabled;
//			ret.ColorBgValueCurrent = this.templateSliderControl.ColorBgValueCurrent;
//			ret.ColorFgParameterLabel = this.templateSliderControl.ColorFgParameterLabel;
//			ret.ColorFgValues = this.templateSliderControl.ColorFgValues;

			//ret.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
			//ret.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			ret.Anchor = this.templateSliderControl.Anchor;
			ret.Padding = this.templateSliderControl.Padding;
			ret.PaddingPanelSlider = this.templateSliderControl.PaddingPanelSlider;
			//END merged

			string nameForScriptDotSeparatedForIndicator = indicatorNameDotParameterName;
			if (string.IsNullOrEmpty(nameForScriptDotSeparatedForIndicator)) nameForScriptDotSeparatedForIndicator = indicatorOrScriptParameter.FullName; 
			ret.LabelText = nameForScriptDotSeparatedForIndicator;
			ret.Name = "parameter_" + nameForScriptDotSeparatedForIndicator;
			
			//v1 ValueCurrent="200" set initially, impedes setting ValueMax=10
			//sequence matters! ret.ValueCurrent checks that you didn't set it outside the boundaries AND within the Increment; fix manually designer-generated SliderComboControl.InitializeComponents() as well 
			ret.ValueIncrement	= new decimal(indicatorOrScriptParameter.ValueIncrement);
			ret.ValueMin		= new decimal(indicatorOrScriptParameter.ValueMin);
			ret.ValueMax		= new decimal(indicatorOrScriptParameter.ValueMax);
			ret.ValueCurrent	= new decimal(indicatorOrScriptParameter.ValueCurrent);
			//v2
			//ret.ValidateValuesAndAbsorbFrom(indicatorOrScriptparameter);
			
			//DOESNT_WORK?... ret.PanelFillSlider.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
			//ret.PaddingPanelSlider = new System.Windows.Forms.Padding(0, 1, 0, 0);
			ret.Location = new System.Drawing.Point(0, this.PreferredHeight + this.VerticalSpaceBetweenSliders);
			ret.Size = new System.Drawing.Size(this.Width, ret.Size.Height);
			ret.Tag = indicatorOrScriptParameter;
			ret.ParentAutoGrowControl = this;
			ret.ValueCurrentChanged += slider_ValueCurrentChanged;
			// WILL_ADD_PARENT_MENU_ITEMS_IN_Opening
			
			ret.EnableBorder	= indicatorOrScriptParameter.BorderShown;
			ret.EnableNumeric	= indicatorOrScriptParameter.NumericUpdownShown;
			
			ret.ShowBorderChanged			+= slider_ShowBorderChanged;
			ret.ShowNumericUpdownChanged	+= slider_ShowNumericUpdownChanged;
			
			return ret;
		}
		public void PopupScriptContextsToConfirmAddedOptimized(string scriptContextNameToExpand = null) {
			this.ctxScriptContexts_Opening(this, null);
			//this.ctxParameterBags.Visible = true;
			this.ctxScriptContexts.Show(this, new System.Drawing.Point(20, 20));
			if (string.IsNullOrEmpty(scriptContextNameToExpand)) return;
			foreach (var item in this.ctxScriptContexts.Items) {
				ToolStripMenuItem mni = item as ToolStripMenuItem;
				if (mni == null) continue;
				if (mni.Text != scriptContextNameToExpand) continue;
				mni.Select();
				mni.ShowDropDown();
				break;
			}
		}
	}
}
