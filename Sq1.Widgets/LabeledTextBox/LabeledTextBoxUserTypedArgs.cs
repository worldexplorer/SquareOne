using System;

namespace Sq1.Widgets.LabeledTextBox {
	public class LabeledTextBoxUserTypedArgs : EventArgs {
		public	string	StringUserTyped; 
		public	bool	RootHandlerShouldCloseParentContextMenuStrip	= false;
		public	bool	HighlightTextWithRed							= false;

		public LabeledTextBoxUserTypedArgs(string stringUserTyped) {
			this.StringUserTyped = stringUserTyped;
		}

		public int IntegerUserTyped { get {
			int userTyped_asInteger;
			bool validInteger = Int32.TryParse(this.StringUserTyped, out userTyped_asInteger);
			if (validInteger == false) {
				this.HighlightTextWithRed = true;
				throw new Exception("ISNT_AN_INTEGER__I_HIGHLIGHTED_WITH_RED");
			}
			if (userTyped_asInteger <= 0) {
				this.HighlightTextWithRed = true;
				throw new Exception("ISNT_AN_INTEGER__I_HIGHLIGHTED_WITH_RED");
			}
			return userTyped_asInteger;
		} }
	}
}