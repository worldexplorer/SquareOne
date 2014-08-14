using System;

namespace Sq1.Widgets.LabeledTextBox {
	public class LabeledTextBoxUserTypedArgs : EventArgs {
		public string StringUserTyped; 
		public bool RootHandlerShouldCloseParentContextMenuStrip = false;
		public bool HighlightTextWithRed = false;
		public LabeledTextBoxUserTypedArgs(string stringUserTyped) {
			this.StringUserTyped = stringUserTyped;
		}
	}
}