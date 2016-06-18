using System;
using System.CodeDom.Compiler;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using DigitalRune.Windows.TextEditor;
using DigitalRune.Windows.TextEditor.Completion;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Formatting;
using DigitalRune.Windows.TextEditor.Highlighting;
using DigitalRune.Windows.TextEditor.Insight;

using Sq1.Core;

namespace Sq1.Widgets.ScriptEditor {
	public partial class ScriptEditor : UserControl {
		public event EventHandler<ScriptEditorEventArgs> OnSave;
		public event EventHandler<ScriptEditorEventArgs> OnCompile;
		public event EventHandler<ScriptEditorEventArgs> OnRun;
		public event EventHandler<ScriptEditorEventArgs> OnDebug;
		public event EventHandler<ScriptEditorEventArgs> OnTextNotSaved;
		public bool DocumentChangedIgnoredDuringInitialization;
		const string SCRIPT_COMPILED_OK = "Script compiled OK";

		public string ScriptSourceCode {
			get { return this.TextEditorControl.Text; }
			set { this.TextEditorControl.Text = value; }
		}

		public ScriptEditor() {
			InitializeComponent();
			this.splitContainer1.Panel2Collapsed = true;
			
			// taken_from_BEGIN DigitalRune.Windows.Editor
			// made visual, moved to ~.Designer.cs, check Properties window
			//this.TextEditorControl.TextAreaKeyPress += TextEditorControl_TextAreaKeyPress;
			//this.TextEditorControl.TextAreaDialogKeyPress += TextEditorControl_TextAreaDialogKeyPress;
			//this.TextEditorControl.CompletionRequest += this.TextEditorControl_CompletionRequest;
			//this.TextEditorControl.InsightRequest += this.TextEditorControl_InsightRequest;
			//this.TextEditorControl.ToolTipRequest += this.TextEditorControl_ToolTipRequest;
			//this.timer.Enabled = true;
			//this.timer.Interval = 2000;
			//this.timer.Tick += new System.EventHandler(this.UpdateFoldings);

			// Set the syntax-highlighting for C#
			this.TextEditorControl.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("C#");

			// Set the formatting for C#
			this.TextEditorControl.Document.FormattingStrategy = new CSharpFormattingStrategy();

			// Set a simple folding strategy that folds all "{ ... }" blocks
			this.TextEditorControl.Document.FoldingManager.FoldingStrategy = new CodeFoldingStrategy();

			// ----- Use the following settings for XML content instead of C#
			//this.TextEditorControl.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("XML");
			//this.TextEditorControl.Document.FormattingStrategy = new XmlFormattingStrategy();
			//this.TextEditorControl.Document.FoldingManager.FoldingStrategy = new XmlFoldingStrategy();
			// -----

			// Try to set font "Consolas", because it's a lot prettier:
			Font consolasFont = new Font("Consolas", 9.75f);
			if (consolasFont.Name == "Consolas")		// Set font if it is available on this machine.
				this.TextEditorControl.Font = consolasFont;
			// taken_from_END
		}
		
		// taken_from_BEGIN DigitalRune.Windows.Editor
		void TextEditorControl_UpdateFoldings(object sender, EventArgs e) {
		  // The foldings needs to be manually updated:
		  // In this example a timer updates the foldings every 2 seconds.
		  // You should manually update the foldings when
		  // - a new document is loaded
		  // - content is added (paste)
		  // - the parse-info is updated
		  // - etc.
		  if (base.Visible == false) return;
		  this.TextEditorControl.Document.FoldingManager.UpdateFolds(null, null);
		}
		void TextEditorControl_CompletionRequest(object sender, CompletionEventArgs e) {
			if (this.TextEditorControl.CompletionWindowVisible) return;
			// e.Key contains the key that the user wants to insert and which triggered
			// the CompletionRequest.e.Key == '\0' means that the user triggered the CompletionRequest by pressing <Ctrl> + <Space>.
			if (e.Key == '\0') {
				// The user has requested the completion window by pressing <Ctrl> + <Space>.
				this.TextEditorControl.ShowCompletionWindow(new CodeCompletionDataProvider(), e.Key, false);
			} else if (char.IsLetter(e.Key)) {
				// The user is typing normally. 
				// -> Show the completion to provide suggestions. Automatically close the window if the 
				// word the user is typing does not match the completion data. (Last argument.)
				this.TextEditorControl.ShowCompletionWindow(new CodeCompletionDataProvider(), e.Key, true);
			}
		}
		void TextEditorControl_InsightRequest(object sender, InsightEventArgs e) {
			this.TextEditorControl.ShowInsightWindow(new MethodInsightDataProvider());
		}
		void TextEditorControl_ToolTipRequest(object sender, ToolTipRequestEventArgs e) {
			if (!e.InDocument || e.ToolTipShown) return;
			// Get word under cursor
			TextLocation position = e.LogicalPosition;
			LineSegment line = this.TextEditorControl.Document.GetLineSegment(position.Y);
			if (line != null) {
				TextWord word = line.GetWord(position.X);
				if (word != null && !String.IsNullOrEmpty(word.Word))
					e.ShowToolTip("Current word: \"" + word.Word + "\"\n" + "\nRow: " + (position.Y + 1) + " Column: " + (position.X + 1));
			}
		}
		// taken_from_END
		
		void optionsToolStripMenuItem_Click(object sender, EventArgs e) {
			OptionsDialog optionsDialog = new OptionsDialog(TextEditorControl);
			optionsDialog.ShowDialog(this);
		}
		void mniSave_Click(object sender, EventArgs e) {
			if (this.OnSave == null) return;
			this.OnSave(this, new ScriptEditorEventArgs(this.ScriptSourceCode));
		}
		void mniCompile_Click(object sender, EventArgs e) {
			if (this.OnCompile == null) return;
			this.OnCompile(this, new ScriptEditorEventArgs(this.ScriptSourceCode));
		}
		void mniRun_Click(object sender, EventArgs e) {
			if (this.OnRun == null) return;
			this.OnRun(this, new ScriptEditorEventArgs(this.ScriptSourceCode));
		}
		void mniDebug_Click(object sender, EventArgs e) {
			if (this.OnDebug == null) return;
			this.OnDebug(this, new ScriptEditorEventArgs(this.ScriptSourceCode));
		}
		public void PopulateCompilerSuccess() {
			int distanceFromBottom = this.splitContainer1.Panel2MinSize;
			//int distanceFromBottom = this.splitContainer1.SplitterIncrement;
			this.txtCompilerErrors.Text = SCRIPT_COMPILED_OK;
			this.txtCompilerErrors.BackColor = Color.LightGreen;
			if (base.Height > 0) {
				distanceFromBottom += 11;
				//distanceFromBottom += this.splitContainer1.SplitterWidth;
				int distanceFromTop = base.Height - distanceFromBottom;
				try {
					this.splitContainer1.SplitterDistance = distanceFromTop;
				} catch (Exception ex) {
					string msg = "TRYING_TO_LOCALIZE_SPLITTER_MUST_BE_BETWEEN_0_AND_PANEL_MIN";
					Assembler.PopupException(msg);
				}
			}
			if (this.splitContainer1.Panel2Collapsed) this.splitContainer1.Panel2Collapsed = false;
		}
		public void PopulateCompilerErrors(CompilerErrorCollection compilerErrorCollection, bool popupFromAutoHidden = true) {
			int errorsIgnoreWarnings = 0;
			string errorsPlainText = "";
			foreach (var error in compilerErrorCollection) {
				string errormsg = error.ToString();
				if (errormsg.ToLower().IndexOf("warning ") == 0) continue;
				errorsIgnoreWarnings++;
				int indexLastSlash = errormsg.LastIndexOf(Path.DirectorySeparatorChar.ToString());
				string noPath = errormsg.Substring(indexLastSlash + 14);
				if (errorsPlainText.Length > 0) errorsPlainText += System.Environment.NewLine;
				errorsPlainText += noPath;
			}

			//int showLines = (compilerErrorCollection.Count >= 5) ? 5 : compilerErrorCollection.Count;
			//int showLines = (errorsIgnoreWarnings >= 5) ? 5 : errorsIgnoreWarnings;
			int showLines = errorsIgnoreWarnings;
			int distanceFromBottom = this.splitContainer1.Panel2MinSize * showLines;
			//int distanceFromBottom = this.splitContainer1.SplitterIncrement * showLines;
			if (base.Height > 0) {
				distanceFromBottom += 11;
				int distanceFromTop = base.Height - distanceFromBottom;
				this.splitContainer1.SplitterDistance = distanceFromTop;
			}
			if (this.splitContainer1.Panel2Collapsed) this.splitContainer1.Panel2Collapsed = false;

			this.txtCompilerErrors.Text = errorsPlainText;
			//this.txtCompilerErrors.BackColor = Color.LightCoral;
			this.txtCompilerErrors.BackColor = Color.FromArgb(225, 200, 200);
			this.parseError_setEditorCaret(popupFromAutoHidden);
		}
		void mniWordWrap_Click(object sender, EventArgs e) {
			this.mniWordWrap.Checked = !this.mniWordWrap.Checked;
			this.txtCompilerErrors.WordWrap = this.mniWordWrap.Checked;
		}
		void mniHide_Click(object sender, EventArgs e) {
			this.splitContainer1.Panel2Collapsed = true;
		}
		void TextEditorControl_DocumentChanged(object sender, DigitalRune.Windows.TextEditor.Document.DocumentEventArgs e) {
			if (this.DocumentChangedIgnoredDuringInitialization) return;
			if (this.OnTextNotSaved == null) return;
			this.OnTextNotSaved(this, new ScriptEditorEventArgs(this.ScriptSourceCode));
		}
		void TextEditorControl_TextAreaKeyPress(object sender, KeyPressEventArgs e) {
			string msg = "Regular alfanumeric key has been pressed";
			//throw new NotImplementedException();
		}
		void TextEditorControl_TextAreaDialogKeyPress(object sender, KeyEventArgs e) {
			switch (e.KeyCode) {
				case Keys.F5: this.mniRun_Click(sender, e); return;
				case Keys.F6: this.mniCompile_Click(sender, e); return;
				case Keys.S:
					if (e.Control == false) return;
					this.mniSave_Click(sender, e);
					return;
			}
		}

		void txtCompilerErrors_KeyUp(object sender, KeyEventArgs e) {
			switch (e.KeyCode) {
				case Keys.Up:
				case Keys.Down:
				case Keys.PageUp:
				case Keys.PageDown:
				case Keys.Home:
				case Keys.End:
					this.parseError_setEditorCaret();
					break;
			}
		}
		void txtCompilerErrors_MouseUp(object sender, MouseEventArgs e) {
			this.parseError_setEditorCaret();
		}
		void parseError_setEditorCaret(bool popupFromAutoHidden = true) {
			if (this.txtCompilerErrors.Text == SCRIPT_COMPILED_OK) return;
			int errorStartIndex = this.txtCompilerErrors.GetFirstCharIndexOfCurrentLine();
			int errorLength = this.txtCompilerErrors.Text.Length - errorStartIndex;
			if (errorLength <= 0) return;
			try {
				int fromBegToFirstEOL = this.txtCompilerErrors.Text.IndexOf(System.Environment.NewLine, errorStartIndex);
				if (fromBegToFirstEOL >= 0) {
					errorLength -= this.txtCompilerErrors.Text.Length - fromBegToFirstEOL;
				}
			} catch (Exception ex) {
				string msg = "last line may not have CRLF...";
			}
			string errorLine = this.txtCompilerErrors.Text.Substring(errorStartIndex, errorLength);
			Regex regex = new Regex(@"\((\d+),(\d+)\)");
			MatchCollection matches = regex.Matches(errorLine);
			string row = "ROW_NOT_FOUND";
			string column = "COLUMN_NOT_FOUND";
			try {
				Match rowColumn = matches[0];
				row		= rowColumn.Groups[1].ToString();
				column	= rowColumn.Groups[2].ToString();
			} catch (Exception ex) {
				string msg = "regex[" + regex + "] didn't find matches for errorLine[" + errorLine + "]";
			}

			if (popupFromAutoHidden) {
				this.TextEditorControl.Focus();
			}
			Caret caret = this.TextEditorControl.ActiveTextAreaControl.Caret;
			try {
				caret.Position = new TextLocation(Int32.Parse(column) - 1, Int32.Parse(row) - 1);
			} catch (Exception ex) {
				string msg = "column[" + column + "] or row[" + row + "] are not Int32";
			}
			caret.UpdateCaretPosition();
		}
		public void AppendRunErrorToCompilerErrors(string msg) {
			this.txtCompilerErrors.Text += System.Environment.NewLine + msg;
		}
	}
}
