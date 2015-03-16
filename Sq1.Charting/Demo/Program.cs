using System;
using System.Windows.Forms;

namespace Sq1.Charting.Demo {
	internal sealed class Program {
		[STAThread]
		private static void Main(string[] args) {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new MultiSplitTest());
			Application.Run(new Panels());
		}
	}
}
