using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sq1.Widgets.LabeledTextBox {
	public partial class LabeledTextBoxControl : UserControl {
		public ToolStripControlHost ParentToolStripControlHost;
		
//		[Browsable(true)]
//		public new Size Size {
//			get { return new Size(base.Size.Width, base.Size.Height - 2); }
//			set { base.Size = new Size(value.Width, value.Height + 2); }
//		}

		[Browsable(true)]
		public TextBox InternalTextBox {
			get { return this.TextBox; }
			set { this.TextBox = value; }
		}
		[Browsable(true)]
		public Label InternalLabelLeft {
			get { return this.LabelLeft; }
			set { this.LabelLeft = value; }
		}
		[Browsable(true)]
		public Label InternalLabelRight {
			get { return this.LabelRight; }
			set { this.LabelRight = value; }
		}

		[Browsable(true)]
		public string TextLeft {
			get { return this.LabelLeft.Text; }
			set { this.LabelLeft.Text = value; }
		}
		[Browsable(true)]
		public string TextRight {
			get { return this.LabelRight.Text; }
			set { this.LabelRight.Text = value; this.LabelRight.Visible = true; }
		}
		[Browsable(true)]
		public bool TextRed {
			get { return this.LabelLeft.ForeColor == Color.Red; }
			set { this.LabelLeft.ForeColor = value ? Color.Red : Color.Black; }
		}
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public int TextLeftOffsetX {
			get { return this.LabelLeft.Location.X; }
			set { this.LabelLeft.Location = new Point(value, this.LabelLeft.Location.Y); }
		}
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public int TextLeftWidth {
			get { return this.LabelLeft.Width; }
			set { this.LabelLeft.Width = value; }
		}
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public int TextRightOffsetX {
			get { return this.LabelRight.Location.X; }
			set { this.LabelRight.Location = new Point(value, this.LabelRight.Location.Y); }
		}
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public int TextRightWidth {
			get { return this.LabelRight.Width; }
			set { this.LabelRight.Width = value; }
		}
		//[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		//public bool TextAutoSize {
		//	get { return this.Label.AutoSize; }
		//	set { this.Label.AutoSize = value; }
		//}
		[Browsable(true)]
		public string InputFieldValue {
			get { return this.TextBox.Text; }
			set { this.TextBox.Text = value; }
		}
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public int InputFieldOffsetX {
			get { return this.TextBox.Location.X; }
			set { this.TextBox.Location = new Point(value, this.TextBox.Location.Y); }
		}
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public int InputFieldWidth {
			get { return this.TextBox.Width; }
			set { this.TextBox.Width = value;
				  this.TextRightOffsetX = this.InputFieldOffsetX + this.InputFieldWidth + 3; }
		}
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public bool InputFieldEditable {
			get { return this.TextBox.Enabled; }
			set { this.TextBox.Enabled = value; }
		}
		[DefaultValueAttribute(typeof(TextBox), null), Browsable(true)]
		public bool InputFieldAlignedRight {
			get { return this.TextBox.TextAlign == HorizontalAlignment.Right; }
			set { this.TextBox.TextAlign = (value == true) ? HorizontalAlignment.Right : HorizontalAlignment.Left; }
		}
		
		public LabeledTextBoxControl() {
			InitializeComponent();
			//base.Size = new Size(base.Width, base.Height - 3);
			this.LabelRight.Visible = false;
		}
		
	}
}
