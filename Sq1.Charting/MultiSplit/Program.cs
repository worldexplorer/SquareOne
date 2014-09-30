using System;
using System.Windows.Forms;

namespace Sq1.Charting.MultiSplit {
	internal sealed class RunMultiSplitTest {
		[STAThread]
		private static void Main(string[] args) {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MultiSplitTest());
		}
	}
}
