//http://stackoverflow.com/questions/5549895/toolstriptextbox-customisation
using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Sq1.Core;
using Sq1.Widgets.WalkForward;

namespace Sq1.Widgets.ToolStripImproved {
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
	public class ToolStripItemWalkForward : ToolStripControlHost {
		public WalkForwardControl WalkForwardControl { get; private set; }

		public ToolStripItemWalkForward() : base(new WalkForwardControl()) {
			this.WalkForwardControl = this.Control as WalkForwardControl;
			this.WalkForwardControl.SliderComboControl.ValueCurrentChanged += new EventHandler<EventArgs>(SliderComboControl_ValueCurrentChanged);
			this.WalkForwardControl.Cbx_Walkforward.CheckedChanged += new EventHandler(Cbx_Walkforward_CheckedChanged);
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
		    get { return this.WalkForwardControl.SliderComboControl.ValueCurrent; }
		    set { this.WalkForwardControl.SliderComboControl.ValueCurrent = value; }
		}

		public bool WalkForwardChecked {
			get { return this.WalkForwardControl.Cbx_Walkforward.Checked; }
			set { this.WalkForwardControl.Cbx_Walkforward.Checked = value; }
		}

		public bool WalkForwardEnabled {
			get { return this.WalkForwardControl.Cbx_Walkforward.Enabled; }
			set { this.WalkForwardControl.Cbx_Walkforward.Enabled = value; }
		}

		////[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public string LabelText {
		    get { return this.WalkForwardControl.SliderComboControl.LabelText; }
		    set { this.WalkForwardControl.SliderComboControl.LabelText = value; }
		}

		public bool FillFromCurrentToMax {
			get { return this.WalkForwardControl.SliderComboControl.FillFromCurrentToMax; }
			set { this.WalkForwardControl.SliderComboControl.FillFromCurrentToMax = value; }
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
