//http://stackoverflow.com/questions/5549895/toolstriptextbox-customisation
using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing;
using System.ComponentModel;

using Sq1.Widgets.SteppingSlider;

namespace Sq1.Widgets.ToolStripImproved {
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
	public class ToolStripItemNumericUpDownWithMouseEvents : ToolStripControlHost {
		public NumericUpDownWithMouseEvents NumericUpDownWithMouseEvents { get; private set; }

		[Browsable(true)]
		public Size DomainUpDownMinimumSize {
			get { return this.NumericUpDownWithMouseEvents.MinimumSize; }
			set { this.NumericUpDownWithMouseEvents.MinimumSize = value; }
		}
		//[Browsable(true)]
		//public Size DomainUpDownMaximumSize {
		//	get { return this.DomainUpDownWithMouseEvents.MaximumSize; }
		//	set { this.DomainUpDownWithMouseEvents.MaximumSize = value; }
		//}
		//[Browsable(true)]
		//public Size DomainUpDownSize {
		//	get { return this.DomainUpDownWithMouseEvents.Size; }
		//	set { this.DomainUpDownWithMouseEvents.Size = value; }
		//}
		//[Browsable(true)]
		//public Point DomainUpDownLocation {
		//	get { return this.DomainUpDownWithMouseEvents.Location; }
		//	set { this.DomainUpDownWithMouseEvents.Location = value; }
		//}

		//int offsetTop = 0;
		//[Browsable(true)]
		//public int OffsetTop {
		//	get { return this.offsetTop; }
		//	set {
		//		this.offsetTop = value;
		//		this.DomainUpDownWithMouseEvents.Location = new Point(this.DomainUpDownWithMouseEvents.Location.X, this.DomainUpDownWithMouseEvents.Location.Y + value);
		//		this.DomainUpDownWithMouseEvents.Size = new Size(this.DomainUpDownWithMouseEvents.Size.Width, this.DomainUpDownWithMouseEvents.Size.Height - value);
		//		//this.DomainUpDownWithMouseEvents.MaximumSize = new Size(this.DomainUpDownWithMouseEvents.MaximumSize.Width, this.DomainUpDownWithMouseEvents.MaximumSize.Height - value);
		//		this.DomainUpDownWithMouseEvents.PreferredSize = this.DomainUpDownWithMouseEvents.PreferredHeight - value;
		//	}
		//}

		public ToolStripItemNumericUpDownWithMouseEvents() : base(new NumericUpDownWithMouseEvents()) {
			this.NumericUpDownWithMouseEvents = this.Control as NumericUpDownWithMouseEvents;
			this.NumericUpDownWithMouseEvents.OnArrowUpStepAdd			+= new EventHandler(domainUpDownWithMouseEvents_OnArrowUpStepAdd);
			this.NumericUpDownWithMouseEvents.OnArrowDownStepSubstract	+= new EventHandler(domainUpDownWithMouseEvents_OnArrowDownStepSubstract);
		}

		public event EventHandler DomainUpDownArrowUpStepAdd;
		void domainUpDownWithMouseEvents_OnArrowUpStepAdd(object sender, EventArgs e) {
			if (this.DomainUpDownArrowUpStepAdd == null) return;
			this.DomainUpDownArrowUpStepAdd(this, null);
		}

		public event EventHandler DomainUpDownArrowDownStepSubstract;
		void domainUpDownWithMouseEvents_OnArrowDownStepSubstract(object sender, EventArgs e) {
			if (this.DomainUpDownArrowDownStepSubstract == null) return;
			this.DomainUpDownArrowDownStepSubstract(this, null);
		}

		// Add properties, events etc. you want to expose...
		
		//[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		//public string Text {
		//	get { return this.TextBox.Text; }
		//	set { this.TextBox.Text = value; }
		//}
	}
}
