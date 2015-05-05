//http://stackoverflow.com/questions/5549895/toolstriptextbox-customisation
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Sq1.Core;

namespace Sq1.Widgets.ToolStripTrackBar {
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
	public class ToolStripItemTrackBar : ToolStripControlHost {
		public TrackBarControl TrackBarControl { get; private set; }

		public ToolStripItemTrackBar() : base(new TrackBarControl()) {
			this.TrackBarControl = this.Control as TrackBarControl;
			this.TrackBarControl.SliderComboControl.ValueCurrentChanged += new EventHandler<EventArgs>(SliderComboControl_ValueCurrentChanged);
			this.TrackBarControl.Cbx_Walkforward.CheckedChanged += new EventHandler(Cbx_Walkforward_CheckedChanged);
		}

		void Cbx_Walkforward_CheckedChanged(object sender, EventArgs e) {
			this.RaiseWalkForwardCheckedChanged();
		}

		void  SliderComboControl_ValueCurrentChanged(object sender, EventArgs e) {
			this.RaiseValueCurrentChanged();
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
		public decimal ValueCurrent {
		    get { return this.TrackBarControl.SliderComboControl.ValueCurrent; }
		    set { this.TrackBarControl.SliderComboControl.ValueCurrent = value; }
		}

		public bool WalkForwardChecked {
			get { return this.TrackBarControl.Cbx_Walkforward.Checked; }
			set { this.TrackBarControl.Cbx_Walkforward.Checked = value; }
		}

		////[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public string LabelText {
		    get { return this.TrackBarControl.SliderComboControl.LabelText; }
		    set { this.TrackBarControl.SliderComboControl.LabelText = value; }
		}

		public event EventHandler<EventArgs> ValueCurrentChanged;
		public event EventHandler<EventArgs> WalkForwardCheckedChanged;
		public void RaiseValueCurrentChanged() {
			if (this.ValueCurrentChanged == null) return;
			try {	// downstack backtest throwing won't crash Release (Debug will halt) 
				this.ValueCurrentChanged(this, EventArgs.Empty);
			} catch (Exception ex) {
				Assembler.PopupException(null, ex);
			}
		}
		public void RaiseWalkForwardCheckedChanged() {
			if (this.WalkForwardCheckedChanged == null) return;
			try {	// downstack backtest throwing won't crash Release (Debug will halt) 
				this.WalkForwardCheckedChanged(this, EventArgs.Empty);
			} catch (Exception ex) {
				Assembler.PopupException(null, ex);
			}
		}

	}
}
