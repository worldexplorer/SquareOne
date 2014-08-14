//http://stackoverflow.com/questions/5549895/toolstriptextbox-customisation
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Sq1.Widgets.LabeledTextBox {
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip |
									   ToolStripItemDesignerAvailability.ContextMenuStrip)]
	public class MenuItemLabel : ToolStripControlHost {
		private Label internalLabel;
		
		[Browsable(true)]
		public Label InternalLabel {
			get { return this.internalLabel; }
			//DESIGNER_CREATES_CIRCULAR_REFERENCE set { this.internalLabel = value; }
		}
		
		[Browsable(true)]
		public new string Text {
			//YOULL_NEVER_SET_IF_YOU_GET 
			get { return this.internalLabel.Text; }
			set { this.internalLabel.Text = value; }
		}
		public MenuItemLabel() : base(new Label()) {
			this.internalLabel = this.Control as Label;
			this.BackColor = Color.Transparent;
		}
	}
}
