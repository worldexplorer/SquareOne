using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Sq1.Gui.Singletons {
	public partial class DataSourceEditorForm : DockContentSingleton<DataSourceEditorForm> {
		//!!!!!!!!!
		// DataSourceEditorForm.HideOnClose=true since on Close() we loose *ProvidersByName Dictionaries
		//!!!!!!!!!
		// 1) you specify HideOnClose=true  and do not override OnFormClosing(); OnClose: <Content IsHidden="True"> will	 be added into Layout.xml
		// 2) you specify HideOnClose=false and  you   override OnFormClosing(); OnClose: <Content IsHidden="True"> will NOT be added into Layout.xml
		// by HideOnClose=false and overriding with hide-cancelTrue-OnFormClosing-cancelFalse, I achieved:
		// onDockedMinimize - stay minimized after restart, onClose - removed from Layout.xml
		//protected override void OnFormClosing(FormClosingEventArgs e) {
		//	base.Hide();
		//	e.Cancel = true;
		//	base.OnFormClosing(e);
		//	e.Cancel = false;	// without it, <Content IsHidden="True"> will be added
		//}
		//public new bool Enabled {
		//	get { return base.Enabled; }
		//	set { base.Enabled = value; }
		//}

		public string windowTitleDefault;

		public DataSourceEditorForm() {
			this.windowTitleDefault = "New DataSource";
			this.InitializeComponent();
		}
	}
}