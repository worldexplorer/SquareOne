using System.Collections.Generic;

using DigitalRune.Windows.TextEditor;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Folding;

namespace Sq1.Widgets.ScriptEditor {
	class CodeFoldingStrategy : IFoldingStrategy {
		public List<Fold> GenerateFolds(IDocument document, string fileName, object parseInformation) {
			// This is a simple folding strategy. It searches for matching brackets ('{', '}') and creates folds for each region.
			List<Fold> folds = new List<Fold>();
			for (int offset = 0; offset < document.TextLength; ++offset) {
				char marker = document.GetCharAt(offset);
				switch (marker) {
					case '{':
						int offsetOfClosingBracket = TextHelper.FindClosingBracket(document, offset + 1, '{', '}');
						if (offsetOfClosingBracket <= 0) continue;
						int length = offsetOfClosingBracket - offset + 1;
						folds.Add(new Fold(document, offset, length, "{...}", false));
						break;
					//case '#':
					//	string lookingForRegion = document.GetText(offset, "region".Length);
					//	if (lookingForRegion.ToLower() != "region") continue;
					//	int offsetOfEndRegion = document.FormattingStrategy.SearchBracketForward TextHelper.Find(document, offset + 1, '{', '}');
					//	if (offsetOfEndRegion <= 0) continue;
					//	int length = offsetOfEndRegion - offset + 1;
					//	folds.Add(new Fold(document, offset, length, "#REGION_COMMENT_HERE#", false));
					//	break;
				}
			}
			return folds;
		}
	}
}
