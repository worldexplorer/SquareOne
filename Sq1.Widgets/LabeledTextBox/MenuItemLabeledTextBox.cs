using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing;

namespace Sq1.Widgets.LabeledTextBox {
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip |
									   ToolStripItemDesignerAvailability.ContextMenuStrip)]
	public class MenuItemLabeledTextBox : ToolStripControlHost {		//http://stackoverflow.com/questions/5549895/toolstriptextbox-customisation
		public event EventHandler<LabeledTextBoxUserTypedArgs> UserTyped;
		public LabeledTextBoxControl LabeledTextBoxControl { get; private set; }
		
		[Browsable(true)]
		public string TextLeft {
			get { return this.LabeledTextBoxControl.TextLeft; }
			set { this.LabeledTextBoxControl.TextLeft = value; }
		}
		[Browsable(true)]
		public string TextRight {
			get { return this.LabeledTextBoxControl.TextRight; }
			set { this.LabeledTextBoxControl.TextRight = value; }
		}
		[Browsable(true)]
		public string InputFieldValue {
			get { return this.LabeledTextBoxControl.InputFieldValue; }
			set { this.LabeledTextBoxControl.InputFieldValue = value; }
		}
		[Browsable(true)]
		public bool TextRed {
			get { return this.LabeledTextBoxControl.TextRed; }
			set { this.LabeledTextBoxControl.TextRed = value; }
		}
		[Browsable(true)]
		public int InputFieldOffsetX {
			get { return this.LabeledTextBoxControl.InputFieldOffsetX; }
			set { this.LabeledTextBoxControl.InputFieldOffsetX = value; }
		}
		[Browsable(true)]
		public int TextLeftOffsetX {
			get { return this.LabeledTextBoxControl.TextLeftOffsetX; }
			set { this.LabeledTextBoxControl.TextLeftOffsetX = value; }
		}
		[Browsable(true)]
		public int TextLeftWidth {
			get { return this.LabeledTextBoxControl.TextLeftWidth; }
			set { this.LabeledTextBoxControl.TextLeftWidth = value; }
		}
		[Browsable(true)]
		public int TextRightOffsetX {
			get { return this.LabeledTextBoxControl.TextRightOffsetX; }
			set { this.LabeledTextBoxControl.TextRightOffsetX = value; }
		}
		[Browsable(true)]
		public int TextRightWidth {
			get { return this.LabeledTextBoxControl.TextRightWidth; }
			set { this.LabeledTextBoxControl.TextRightWidth = value; }
		}
		//[Browsable(true)]
		//public bool TextAutoSize {
		//	get { return this.LabeledTextBoxControl.TextAutoSize; }
		//	set { this.LabeledTextBoxControl.TextAutoSize = value; }
		//}
		[Browsable(true)]
		public int InputFieldWidth {
			get { return this.LabeledTextBoxControl.InputFieldWidth; }
			set { this.LabeledTextBoxControl.InputFieldWidth = value; }
		}
		[Browsable(true)]
		public bool InputFieldEditable {
			get { return this.LabeledTextBoxControl.InputFieldEditable; }
			set { this.LabeledTextBoxControl.InputFieldEditable = value; }
		}
		[Browsable(true)]
		public bool InputFieldAlignedRight {
			get { return this.LabeledTextBoxControl.InputFieldAlignedRight; }
			set { this.LabeledTextBoxControl.InputFieldAlignedRight = value; }
		}
		[Browsable(true)]
		public bool InputFieldMultiline {
			get { return this.LabeledTextBoxControl.TextBox.Multiline; }
			set { this.LabeledTextBoxControl.TextBox.Multiline = value; }
		}
		[Browsable(true)]
		public Color InputFieldBackColor {
			get { return this.LabeledTextBoxControl.TextBox.BackColor; }
			set { this.LabeledTextBoxControl.TextBox.BackColor = value; }
		}
		[Browsable(true)]
		public int OffsetTop {
			get { return this.LabeledTextBoxControl.LabelLeft.Padding.Top; }
			set {
				this.LabeledTextBoxControl.LabelLeft.Padding = new Padding(this.LabeledTextBoxControl.LabelLeft.Padding.Left, value
					, this.LabeledTextBoxControl.LabelLeft.Padding.Right, this.LabeledTextBoxControl.LabelLeft.Padding.Bottom);
				this.LabeledTextBoxControl.LabelRight.Padding = new Padding(this.LabeledTextBoxControl.LabelRight.Padding.Right, value
					, this.LabeledTextBoxControl.LabelRight.Padding.Right, this.LabeledTextBoxControl.LabelRight.Padding.Bottom);

				//this.LabeledTextBoxControl.TextBox.Margin = new Padding(this.LabeledTextBoxControl.TextBox.Margin.Left, value
				//	, this.LabeledTextBoxControl.TextBox.Margin.Right, this.LabeledTextBoxControl.TextBox.Margin.Bottom);
				this.LabeledTextBoxControl.TextBox.Location = new Point(this.LabeledTextBoxControl.TextBox.Location.X, this.LabeledTextBoxControl.TextBox.Location.Y + value);
				this.LabeledTextBoxControl.TextBox.Size = new Size(this.LabeledTextBoxControl.TextBox.Size.Width, this.LabeledTextBoxControl.TextBox.Size.Height - value);
			}
		}

		public MenuItemLabeledTextBox() : base(new LabeledTextBoxControl()) {
			this.LabeledTextBoxControl = this.Control as LabeledTextBoxControl;
			this.LabeledTextBoxControl.ParentToolStripControlHost = this;	// to calculate ((ToolStripControlHost as ToolStipItem).Tag as ScriptContext) from LabeledTextBox.TextBox.OnKeyPress 
			this.LabeledTextBoxControl.TextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyDown);
		}
		
		void TextBox_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode != Keys.Enter) return;
			e.Handled = e.SuppressKeyPress = true;
			string typed = this.LabeledTextBoxControl.TextBox.Text;
			typed = typed.Trim();
			typed = typed.Replace('\'', '~');	//  no single quotes against future SQL-injection: update name='456~~; DELETE *;' 
			typed = typed.Replace('"', '~');	//  no double quotes for current JSON consistency: "ScriptContextsByName": { "456~~": {} }
			var args = new LabeledTextBoxUserTypedArgs(typed);
			if (this.UserTyped != null) {
				this.UserTyped(this, args);
			}
			this.LabeledTextBoxControl.TextRed = args.HighlightTextWithRed;
			if (args.RootHandlerShouldCloseParentContextMenuStrip == false) return;
			ContextMenuStrip ctx = this.Owner as ContextMenuStrip;
			if (ctx == null) return;
			ctx.Close();
		}
		internal void InputFieldFocus() {
			this.LabeledTextBoxControl.TextBox.Focus();
		}
	}
}