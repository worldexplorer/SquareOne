using System;

using Sq1.Gui.Forms;

namespace Sq1.Gui.FormFactories {
	public class LivesimFormFactory {	// REASON_TO_EXIST: way to refresh Sliders and run Chart.Backtest() for added ContextScript from Sq1.Widgets.dll:OptimizationControl
		ChartFormManager chartFormManager;

		public LivesimFormFactory(ChartFormManager chartFormsManager) {
			this.chartFormManager = chartFormsManager;
		}

		LivesimForm LivesimForm {
			get { return this.chartFormManager.LivesimFormConditionalInstance; }
			set { this.chartFormManager.LivesimForm = value; }
		}

		public void CreateLivesimFormSubscribePushToManager(ChartFormManager chartFormsManager) {
			this.LivesimForm = new LivesimForm(chartFormsManager);
			this.LivesimForm.Disposed += LivesimForm_Disposed;
			this.LivesimForm.LivesimControl.BtnStartLivesim.Click += new EventHandler(btnStartLivesim_Click);
		}

		void btnStartLivesim_Click(object sender, EventArgs e) {
			this.chartFormManager.Executor.Livesimulator.Start();
		}
		void LivesimForm_Disposed(object sender, EventArgs e) {
			// both at FormCloseByX and MainForm.onClose()
			this.chartFormManager.ChartForm.MniShowLivesim.Checked = false;
			this.LivesimForm = null;
		}
	}
}
