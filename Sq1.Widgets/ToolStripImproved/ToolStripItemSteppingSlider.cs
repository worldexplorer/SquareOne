//http://stackoverflow.com/questions/5549895/toolstriptextbox-customisation
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Sq1.Widgets.SteppingSlider;

namespace Sq1.Widgets.ToolStripImproved {
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
	public class ToolStripItemSteppingSlider : ToolStripControlHost {
		public SteppingSliderComboControl SliderComboControl { get; private set; }

		public ToolStripItemSteppingSlider() : base(new SteppingSliderComboControl()) {
			this.SliderComboControl = this.Control as SteppingSliderComboControl;
		}

		// Add properties, events etc. you want to expose...
		
		//[DefaultValueAttribute(typeof(ProgressBar), null), Browsable(true)]
		public decimal ValueMin {
			get { return this.SliderComboControl.ValueMin; }
			set { this.SliderComboControl.ValueMin = value; }
		}

		//[DefaultValueAttribute(typeof(ProgressBar), null), Browsable(true)]
		public decimal ValueMax {
			get { return this.SliderComboControl.ValueMax; }
			set { this.SliderComboControl.ValueMax = value; }
		}

		//[DefaultValueAttribute(typeof(ProgressBar), null), Browsable(true)]
		public decimal ValueCurrent {
			get { return this.SliderComboControl.ValueCurrent; }
			set { this.SliderComboControl.ValueCurrent = value; }
		}

		//[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public string LabelText {
			get { return this.SliderComboControl.LabelText; }
			set { this.SliderComboControl.LabelText = value; }
		}
	}
}
