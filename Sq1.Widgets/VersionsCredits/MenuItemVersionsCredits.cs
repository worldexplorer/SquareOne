using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Sq1.Widgets.VersionsCredits {
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip |
									   ToolStripItemDesignerAvailability.ContextMenuStrip)]
	public class MenuItemVersionsCredits : ToolStripControlHost {		//http://stackoverflow.com/questions/5549895/toolstriptextbox-customisation
		
		VersionsCreditsUserControl VersionsCreditsUserControl;
		public MenuItemVersionsCredits() : base(new VersionsCreditsUserControl()) {
			this.VersionsCreditsUserControl = this.Control as VersionsCreditsUserControl;
		}
	}
}
