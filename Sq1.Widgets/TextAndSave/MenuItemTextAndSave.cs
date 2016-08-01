using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Sq1.Widgets.TextAndSave {
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip |
									   ToolStripItemDesignerAvailability.ContextMenuStrip)]
	public class MenuItemTextAndSave : ToolStripControlHost {		//http://stackoverflow.com/questions/5549895/toolstriptextbox-customisation
		
		TextAndSaveControl TextAndSaveControl;
		public MenuItemTextAndSave() : base(new TextAndSaveControl()) {
			this.TextAndSaveControl = this.Control as TextAndSaveControl;
		}
	}
}
