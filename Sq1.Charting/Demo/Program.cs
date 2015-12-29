using System;
using System.Windows.Forms;

using Sq1.Core;

namespace Sq1.Charting.Demo {
	internal sealed class Program {
		[STAThread]
		private static void Main(string[] args) {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new MultiSplitTest());
			//Application.Run(new TestPanels());
			//Application.Run(new TestChartForm());
			Application.Run(new TestDockedChart());
		}
	}
}
