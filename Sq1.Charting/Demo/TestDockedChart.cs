using System;
using System.Windows.Forms;

using Sq1.Core;

namespace Sq1.Charting.Demo {
	public partial class TestDockedChart : Form {
		public TestDockedChart() {
			InitializeComponent();

			TestChartForm chart = new TestChartForm();
			chart.Show(this.dockPanel1);
		}
	}
}
