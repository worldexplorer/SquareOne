using System;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core.StrategyBase;

namespace Sq1.Gui.Singletons {
	public partial class SlidersForm : DockContentSingleton<SlidersForm> {
		public SlidersForm() {
			InitializeComponent();
//			WindowsFormsUtils.SetDoubleBuffered(this);
//			this.SetStyle(
//				ControlStyles.UserPaint |
//				ControlStyles.AllPaintingInWmPaint |
//				ControlStyles.OptimizedDoubleBuffer, true);
		}
		public void Initialize(Strategy strategy) {
			try {
				base.SuspendLayout();
				if (strategy.Script == null || strategy.Script.ParametersById.Count == 0) {
					this.SlidersAutoGrowControl.Hide();
					this.pnlNoParametersInScript.Show();
					this.lblScriptName.Text = (strategy.Script == null) ? "NO_SCRIPT" : strategy.Name;
					this.Size = new Size(this.Size.Width, this.pnlNoParametersInScript.Height);
					return;
				}
				this.pnlNoParametersInScript.Hide();
				this.SlidersAutoGrowControl.Show();
				this.SlidersAutoGrowControl.Initialize(strategy);
			} finally {
				base.ResumeLayout(true);
			}
			this.Size = new Size(this.Size.Width, this.SlidersAutoGrowControl.Height);
			//if (this.Pane == null) {
			//	string msg = "SlidersForm wasn't added to MainForm.DockContent; not activated; use chartFormsManager.StrategyCompileActivatePopulateSliders() later";
			//	return;
			//}
			//this.Pane.Activate();
			
			this.PopulateFormTitle(strategy);
		}
		public void PopulateFormTitle(Strategy strategy) {
			if (strategy.ScriptContextCurrent == null) return;
			base.Text = "[" + strategy.ScriptContextCurrent.Name + "] ScriptContext";
		}
	}
}