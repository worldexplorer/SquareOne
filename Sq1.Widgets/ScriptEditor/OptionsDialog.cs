using System.Windows.Forms;

using DigitalRune.Windows.TextEditor;

namespace Sq1.Widgets.ScriptEditor {
	public partial class OptionsDialog : Form {
		public OptionsDialog(TextEditorControl textEditorControl) {
			InitializeComponent();

			// Show the properties of the TextEditorControl
			propertyGrid.SelectedObject = textEditorControl;
		}
	}
}