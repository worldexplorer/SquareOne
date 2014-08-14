using System;
using System.Windows.Forms;
using Sq1.Gui.Singletons;

namespace Sq1.Gui {
	internal sealed class Program {
		[STAThread]
		private static void Main(string[] args) {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
