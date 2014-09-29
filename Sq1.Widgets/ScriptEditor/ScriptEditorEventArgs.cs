using System;

namespace Sq1.Widgets.ScriptEditor {
	public class ScriptEditorEventArgs : EventArgs {
		public string ScriptText;

		public ScriptEditorEventArgs(string p) {
			this.ScriptText = p;
		}
	}
}
