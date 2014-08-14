//http://stackoverflow.com/questions/5549895/toolstriptextbox-customisation
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Sq1.Widgets.SteppingSlider {
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip |
									   ToolStripItemDesignerAvailability.ContextMenuStrip)]
	public class MenuItemSlidersAutoGrowPreview : ToolStripControlHost {
		public SlidersAutoGrowControl SlidersAutoGrow { get; private set; }

		public MenuItemSlidersAutoGrowPreview() : base(new SlidersAutoGrowControl()) {
			this.SlidersAutoGrow = this.Control as SlidersAutoGrowControl;
		}

		// Add properties, events etc. you want to expose...

		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public SlidersAutoGrowControl SlidersPreview {
			get { return this.SlidersAutoGrow; }
			set { this.SlidersAutoGrow = value; }
		}
	}
}
