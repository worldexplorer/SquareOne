//http://stackoverflow.com/questions/5549895/toolstriptextbox-customisation
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Sq1.Core;

namespace Sq1.Widgets.ToolStripImproved {
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
	public class ToolStripItemCheckBox : ToolStripControlHost {
		public CheckBox CheckBox { get; private set; }

		[Browsable(true)]
		public string CheckBoxText {
		    get { return this.CheckBox.Text; }
		    set { this.CheckBox.Text = value; }
		}
		[Browsable(true)]
		public bool CheckBoxChecked {
			get { return this.CheckBox.Checked; }
			set { this.CheckBox.Checked = value; }
		}


		public ToolStripItemCheckBox() : base(new CheckBox()) {
			this.CheckBox = this.Control as CheckBox;
			this.CheckBox.CheckedChanged += new EventHandler(this.checkBox_CheckedChanged);
		}

		void checkBox_CheckedChanged(object sender, EventArgs e) {
			this.raiseCheckBoxCheckedChanged();
		}
		public event EventHandler<EventArgs> CheckBoxCheckedChanged;
		void raiseCheckBoxCheckedChanged() {
			if (this.CheckBoxCheckedChanged == null) return;
			try {	// downstack backtest throwing won't crash Release (Debug will halt) 
				this.CheckBoxCheckedChanged(this, EventArgs.Empty);
			} catch (Exception ex) {
				Assembler.PopupException(null, ex);
			}
		}

	}
}
