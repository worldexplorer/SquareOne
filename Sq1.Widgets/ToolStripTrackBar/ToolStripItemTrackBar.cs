//http://stackoverflow.com/questions/5549895/toolstriptextbox-customisation
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Sq1.Widgets.ToolStripTrackBar {
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
	public class ToolStripItemTrackBar : ToolStripControlHost {
		public TrackBarControl TrackBarControl { get; private set; }

		public ToolStripItemTrackBar() : base(new TrackBarControl()) {
			this.TrackBarControl = this.Control as TrackBarControl;
		}

		//// Add properties, events etc. you want to expose...
		
		////[DefaultValueAttribute(typeof(ProgressBar), null), Browsable(true)]
		//public decimal ValueMin {
		//    get { return this.TrackBarControl.ValueMin; }
		//    set { this.TrackBarControl.ValueMin = value; }
		//}

		////[DefaultValueAttribute(typeof(ProgressBar), null), Browsable(true)]
		//public decimal ValueMax {
		//    get { return this.TrackBarControl.ValueMax; }
		//    set { this.TrackBarControl.ValueMax = value; }
		//}

		////[DefaultValueAttribute(typeof(ProgressBar), null), Browsable(true)]
		//public decimal ValueCurrent {
		//    get { return this.TrackBarControl.ValueCurrent; }
		//    set { this.TrackBarControl.ValueCurrent = value; }
		//}

		////[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		//public string LabelText {
		//    get { return this.TrackBarControl.LabelText; }
		//    set { this.TrackBarControl.LabelText = value; }
		//}
	}
}
