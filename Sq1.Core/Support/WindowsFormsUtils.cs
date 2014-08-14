namespace Sq1.Support  {
	public class WindowsFormsUtils {
		//http://stackoverflow.com/questions/76993/how-to-double-buffer-net-controls-on-a-form
		public static void SetDoubleBuffered(System.Windows.Forms.Control c) {
			//Taxes: Remote Desktop Connection and painting
			//http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
			if (System.Windows.Forms.SystemInformation.TerminalServerSession)
				return;

			System.Reflection.PropertyInfo aProp =
				  typeof(System.Windows.Forms.Control).GetProperty(
						"DoubleBuffered",
						System.Reflection.BindingFlags.NonPublic |
						System.Reflection.BindingFlags.Instance);

			aProp.SetValue(c, true, null);
		}
	}
}
