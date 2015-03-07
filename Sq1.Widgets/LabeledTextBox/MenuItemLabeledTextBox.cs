using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Sq1.Widgets.LabeledTextBox {
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip |
									   ToolStripItemDesignerAvailability.ContextMenuStrip)]
	public class MenuItemLabeledTextBox : ToolStripControlHost {		//http://stackoverflow.com/questions/5549895/toolstriptextbox-customisation
		public event EventHandler<LabeledTextBoxUserTypedArgs> UserTyped;
		private LabeledTextBoxControl LabeledTextBoxControl; // { get; private set; }
		public MenuItemLabeledTextBox() : base(new LabeledTextBoxControl()) {
			this.LabeledTextBoxControl = this.Control as LabeledTextBoxControl;
			this.LabeledTextBoxControl.ParentToolStripControlHost = this;	// to calculate ((ToolStripControlHost as ToolStipItem).Tag as ScriptContext) from LabeledTextBox.TextBox.OnKeyPress 
			this.LabeledTextBoxControl.TextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyDown);
		}
		[Browsable(true)]
		public new string Text {
			get { return this.LabeledTextBoxControl.Text; }
			set { this.LabeledTextBoxControl.Text = value; }
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
		public int TextOffsetX {
			get { return this.LabeledTextBoxControl.TextOffsetX; }
			set { this.LabeledTextBoxControl.TextOffsetX = value; }
		}
		[Browsable(true)]
		public int TextWidth {
			get { return this.LabeledTextBoxControl.TextWidth; }
			set { this.LabeledTextBoxControl.TextWidth = value; }
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
	}
}