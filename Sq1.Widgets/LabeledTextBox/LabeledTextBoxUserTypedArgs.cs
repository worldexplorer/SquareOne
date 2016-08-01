using System;

namespace Sq1.Widgets.LabeledTextBox {
	public class LabeledTextBoxUserTypedArgs : EventArgs {
		public	string	StringUserTyped; 
		public	bool	RootHandlerShouldCloseParentContextMenuStrip	= false;
		public	bool	HighlightTextWithRed							= false;

		public LabeledTextBoxUserTypedArgs(string stringUserTyped) {
			this.StringUserTyped = stringUserTyped;
		}

		public int UserTyped_asInteger { get {
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
		public decimal UserTyped_asDecimal { get {
			decimal userTyped_asDecimal;
			bool validInteger = Decimal.TryParse(this.StringUserTyped, out userTyped_asDecimal);
			if (validInteger == false) {
				this.HighlightTextWithRed = true;
				throw new Exception("ISNT_AN_INTEGER__I_HIGHLIGHTED_WITH_RED");
			}
			if (userTyped_asDecimal <= 0) {
				this.HighlightTextWithRed = true;
				throw new Exception("ISNT_AN_INTEGER__I_HIGHLIGHTED_WITH_RED");
			}
			return userTyped_asDecimal;
		} }
		public double UserTyped_asDouble { get {
			return (double)this.UserTyped_asDecimal;
		} }
		public float UserTyped_asFloat { get {
			return (float)this.UserTyped_asDecimal;
		} }
	}
}