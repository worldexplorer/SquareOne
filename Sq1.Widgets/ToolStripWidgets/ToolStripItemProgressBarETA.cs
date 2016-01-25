//http://stackoverflow.com/questions/5549895/toolstriptextbox-customisation
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Sq1.Widgets.ProgressBacktestETA;

namespace Sq1.Widgets.ToolStripImproved {
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
	public class ToolStripItemProgressBarETA : ToolStripControlHost {
		public ProgressBarAndLabelControl ProgressBarAndLabel { get; private set; }

		public ToolStripItemProgressBarETA() : base(new ProgressBarAndLabelControl()) {
			this.ProgressBarAndLabel = this.Control as ProgressBarAndLabelControl;
		}

		// Add properties, events etc. you want to expose...
		
		[DefaultValueAttribute(typeof(ProgressBar), null), Browsable(true)]
		public int ETAProgressBarMinimum {
			get { return this.ProgressBarAndLabel.ProgressBarETA.Minimum; }
			set { this.ProgressBarAndLabel.ProgressBarETA.Minimum = value; }
		}

		[DefaultValueAttribute(typeof(ProgressBar), null), Browsable(true)]
		public int ETAProgressBarMaximum {
			get { return this.ProgressBarAndLabel.ProgressBarETA.Maximum; }
			set { this.ProgressBarAndLabel.ProgressBarETA.Maximum = value; }
		}

		[DefaultValueAttribute(typeof(ProgressBar), null), Browsable(true)]
		public int ETAProgressBarValue {
			get { return this.ProgressBarAndLabel.ProgressBarETA.Value; }
			set { this.ProgressBarAndLabel.ProgressBarETA.Value = value; }
		}

		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public string ETALabelText {
			get { return this.ProgressBarAndLabel.LabelETA.Text; }
			set { this.ProgressBarAndLabel.LabelETA.Text = value; }
		}

	}
}
