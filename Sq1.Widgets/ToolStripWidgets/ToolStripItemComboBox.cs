//http://stackoverflow.com/questions/5549895/toolstriptextbox-customisation
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Sq1.Core;

namespace Sq1.Widgets.ToolStripImproved {
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
	public class ToolStripItemComboBox : ToolStripControlHost {
		public ComboBox ComboBox { get; private set; }

		[Browsable(true)]
		public ComboBoxStyle ComboBoxDropDownStyle {
			get { return this.ComboBox.DropDownStyle; }
			set { this.ComboBox.DropDownStyle = value; }
		}
		[Browsable(true)]
		public bool ComboBoxFormattingEnabled {
			get { return this.ComboBox.FormattingEnabled; }
			set { this.ComboBox.FormattingEnabled = value; }
		}
		//[Browsable(true)]
		//public Size ComboBoxMinimumSize {
		//    get { return this.ComboBox.MinimumSize; }
		//    set { this.ComboBox.MinimumSize = value; }
		//}
		[Browsable(true)]
		public Size ComboBoxSize {
			get { return this.ComboBox.Size; }
			set { this.ComboBox.Size = value; }
		}
		[Browsable(true)]
		public Point ComboBoxLocation {
			get { return this.ComboBox.Location; }
			set { this.ComboBox.Location = value; }
		}
		[Browsable(true)]
		public bool ComboBoxSorted {
		    get { return this.ComboBox.Sorted; }
		    set { this.ComboBox.Sorted = value; }
		}
		[Browsable(true)]
		[Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ComboBox.ObjectCollection ComboBoxItems {
		    get { return this.ComboBox.Items; }
		}

		[Browsable(true)]
		public int ComboBoxSelectedIndex {
		    get { return this.ComboBox.SelectedIndex; }
		    set { this.ComboBox.SelectedIndex = value; }
		}

		//int offsetTop = 0;
		//[Browsable(true)]
		//public int OffsetTop {
		//    get { return this.offsetTop; }
		//    set {
		//        this.offsetTop = value;
		//        //this.ComboBox.Location = new Point(this.ComboBox.Location.X, this.ComboBox.Location.Y + value);
		//        //this.ComboBox.Size = new Size(this.ComboBox.Size.Width, this.ComboBox.Size.Height - value);
		//        //this.ComboBox.ItemHeight = this.ComboBox.ItemHeight - value;
		//        this.ComboBox.PreferredSize = this.ComboBox.PreferredHeight - value;
		//    }
		//}
		

		public ToolStripItemComboBox() : base(new ComboBox()) {
			this.ComboBox = this.Control as ComboBox;
			//ComboBox.SelectedValueChanged is bad because when ComboBox.Sort=true, figuring out the Index by Value will lead you to an error and hours lost for debugging
			this.ComboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
			this.ComboBox.DropDown += new EventHandler(comboBox_DropDown);
		}

		void comboBox_DropDown(object sender, EventArgs e) {
		    this.raiseDropDown();
		}
		public event EventHandler<EventArgs> ComboBoxDropDown;
		void raiseDropDown() {
		    if (this.ComboBoxDropDown == null) return;
		    try {	// downstack backtest throwing won't crash Release (Debug will halt) 
		        this.ComboBoxDropDown(this, EventArgs.Empty);
		    } catch (Exception ex) {
		        Assembler.PopupException(null, ex);
		    }
		}


		void comboBox_SelectedIndexChanged(object sender, EventArgs e) {
		    this.raiseSelectedIndexChanged();
		}
		public event EventHandler<EventArgs> ComboBoxSelectedIndexChanged;
		void raiseSelectedIndexChanged() {
		    if (this.ComboBoxSelectedIndexChanged == null) return;
		    try {	// downstack backtest throwing won't crash Release (Debug will halt) 
		        this.ComboBoxSelectedIndexChanged(this, EventArgs.Empty);
		    } catch (Exception ex) {
		        Assembler.PopupException(null, ex);
		    }
		}

		// USAGE:
		//public ChartSettingsEditorControl() {
		//    InitializeComponent();
		////    // DESIGNER_RESETS_TO_EDITABLE__LAZY_TO_TUNNEL_PROPERTIES_AND_EVENTS_IN_ToolStripItemComboBox.cs
		////    this.toolStripItemComboBox1.ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
		////    this.toolStripItemComboBox1.ComboBox.Sorted = true;
		////    this.toolStripItemComboBox1.ComboBox.SelectedIndexChanged += new EventHandler(this.toolStripItemComboBox1_SelectedIndexChanged);
		//}

	}
}
