//http://stackoverflow.com/questions/5549895/toolstriptextbox-customisation
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections;

using Sq1.Core;

namespace Sq1.Widgets.ToolStripImproved {
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
	public class ToolStripItemComboBox : ToolStripControlHost {
		public ComboBox ComboBox { get; private set; }

		public ToolStripItemComboBox() : base(new ComboBox()) {
			this.ComboBox = this.Control as ComboBox;
			//v1   ComboBox.SelectedValueChanged is bad because when ComboBox.Sort=true, figuring out the Index by Value will lead you to an error and hours lost for debugging
			//v2 NO!!_SUBSCRIBE_TO_ComboBox_DIRECTLY_WHICH_IS_PUBLIC_NOW
			//this.ComboBox.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
		}

		//void ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
		//    this.RaiseSelectedIndexChanged();
		//}

		//// Add properties, events etc. you want to expose...

		//public IList ComboBoxItems {
		//    get { return this.ComboBox.Items; }
		//    set { this.ComboBox.Items = value; }
		//}

		//public int SelectedIndex {
		//    get { return this.ComboBox.SelectedIndex; }
		//    set { this.ComboBox.SelectedIndex = value; }
		//}
		

		// NO!!_SUBSCRIBE_TO_ComboBox_DIRECTLY_WHICH_IS_PUBLIC_NOW
		//public event EventHandler<EventArgs> SelectedIndexChanged;
		//public void RaiseSelectedIndexChanged() {
		//    if (this.SelectedIndexChanged == null) return;
		//    try {	// downstack backtest throwing won't crash Release (Debug will halt) 
		//        this.SelectedIndexChanged(this, EventArgs.Empty);
		//    } catch (Exception ex) {
		//        Assembler.PopupException(null, ex);
		//    }
		//}


		// USAGE:
		//public ChartSettingsEditorControl() {
		//    InitializeComponent();
		//    // DESIGNER_RESETS_TO_EDITABLE__LAZY_TO_TUNNEL_PROPERTIES_AND_EVENTS_IN_ToolStripItemComboBox.cs
		//    this.toolStripItemComboBox1.ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
		//    this.toolStripItemComboBox1.ComboBox.Sorted = true;
		//    this.toolStripItemComboBox1.ComboBox.SelectedIndexChanged += new EventHandler(this.toolStripItemComboBox1_SelectedIndexChanged);
		//}

	}
}
