using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Sq1.Widgets.DllVersions {
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip |
									   ToolStripItemDesignerAvailability.ContextMenuStrip)]
	public class MenuItemDllVersions : ToolStripControlHost {		//http://stackoverflow.com/questions/5549895/toolstriptextbox-customisation
		
		DllVersionsUserControl DllVersionsUserControl;
		public MenuItemDllVersions() : base(new DllVersionsUserControl()) {
			this.DllVersionsUserControl = this.Control as DllVersionsUserControl;
		}
	}
}
