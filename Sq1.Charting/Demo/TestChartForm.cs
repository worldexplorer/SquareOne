using System;
using System.Windows.Forms;

using Sq1.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace Sq1.Charting.Demo {
	//public partial class TestChartForm : Form {
	public partial class TestChartForm : DockContent {
		public TestChartForm() {
			ExceptionsForm exceptionsForm = new ExceptionsForm();
			exceptionsForm.Show();
			Assembler.InstanceUninitialized.Initialize(exceptionsForm, true);

			// I'm not loading workspace one-by-one; the size of parent form is set and innter TestChartControl sizes are firmly set => safe to say LaoyutComplete=true
			Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete = true;

			InitializeComponent();

			if (base.ClientRectangle.Width != this.chartControl1.ClientRectangle.Width) {
				string msg = "SIMULATING_Dock.Fill_ChartControl.Initialize(BARS_GENERATED_RANDOM)<=ctor() //TestChartForm()";
				Assembler.PopupException(msg);
			}

			//this.chartControl1.RangeBarCollapsed = true;
			//this.SplitterHeight = 6;
			//exceptionsForm.ExceptionControl.FlushExceptionsToOLVIfDockContentDeserialized_inGuiThread();
		}
	}
}
