using System;
using System.Drawing;

using Sq1.Core.StrategyBase;
using Sq1.Gui.Forms;
using Sq1.Support;

namespace Sq1.Gui.Singletons {
	public partial class SlidersForm : DockContentSingleton<SlidersForm> {
		public SlidersForm() {
			InitializeComponent();
			WindowsFormsUtils.SetDoubleBuffered(this);
		}
		public void Initialize(Strategy strategy) {
			try {
				base.SuspendLayout();
				this.Size = new Size(this.Size.Width, this.SteppingSlidersAutoGrowControl.Height);
				bool showEmptyStubWhenStrategyNullOrNoParameters = true;
				if (strategy != null && strategy.Script != null) {
					int parametersToShow = strategy.ScriptContextCurrent.IndicatorParametersByName.Values.Count + strategy.Script.ScriptParametersById_ReflectedCached.Count; 
					if (parametersToShow > 0) showEmptyStubWhenStrategyNullOrNoParameters = false;
				}
				if (showEmptyStubWhenStrategyNullOrNoParameters == false) {
					this.SteppingSlidersAutoGrowControl.Show();
					this.pnlNoParametersInScript.Hide();
					this.SteppingSlidersAutoGrowControl.Initialize(strategy);
					this.PopulateFormTitle(strategy);
					return;
				}
				
				this.SteppingSlidersAutoGrowControl.Hide();
				this.pnlNoParametersInScript.Show();
				if (strategy == null) {
					string scriptName = "CHART_NO_STRATEGY";
					ChartForm chartActive = base.DockPanel.ActiveDocument as ChartForm;
					if (chartActive != null) {
						scriptName = chartActive.Text;
					}
					this.lblNoParametersDefined.Text = "No Strategy in DockPanel.ActiveDocument; it contains only Chart";
					this.lblScriptName.Text = scriptName;
					base.Text = "CHART_NO_STRATEGY";
					return;
				}
				this.lblNoParametersDefined.Text = "No parameters/indicators defined in the Script < DockPanel.ActiveDocument";
				this.lblScriptName.Text = strategy.Name;
				base.Text = "[" + strategy.ScriptContextCurrent.Name + "] [" + strategy.Name + "] ScriptContext";
				if (strategy.Script == null) {
					this.lblScriptName.Text += " NO_SCRIPT";
				}
			} finally {
				base.ResumeLayout(true);
			}
		}
		public void PopulateFormTitle(Strategy strategy) {
			if (strategy.ScriptContextCurrent == null) return;
			base.Text = "ScriptContext :: [" + strategy.ScriptContextCurrent.Name + "] [" + strategy.Name + "]";
		}
	}
}