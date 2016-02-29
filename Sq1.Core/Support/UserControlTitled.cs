using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sq1.Core.Support {
	public partial class UserControlTitled : UserControlResizeable {
		[Browsable(true)]
		public string WindowTitle {
			get { return this.lblWindowTitle.Text; }
			set { this.lblWindowTitle.Text = value; }
		}

		// so that Designer will add everything into the WindowTitle
		//public new Control.ControlCollection Controls {
		//    //get { return this.isInitialized ? base.Controls : this.pnlWindowContent.Controls; }
		//    get { return this.UserControlInner.Controls; }
		//    //set { this.pnlWindowContent.Controls = value; }
		//}

		bool isInitialized = false;
		public UserControlTitled() {
			InitializeComponent();
			isInitialized = true;
		}
	}
}
