//http://stackoverflow.com/questions/5549895/toolstriptextbox-customisation
using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Sq1.Widgets.SteppingSlider {
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip |
									   ToolStripItemDesignerAvailability.ContextMenuStrip)]
	public class MenuItemTrackBar : ToolStripControlHost {
		private TrackBar trackBar;

		public MenuItemTrackBar() : base(new TrackBar()) {
			this.trackBar = this.Control as TrackBar;
		}
	}
}
