using System;
using System.Collections.Generic;
using System.Drawing;

namespace Sq1.Core.Support {
	public class FontCache {
		public	Font							FontForeground { get; private set;}
				Dictionary<FontStyle, Font>		fontsByStyle_dontDisposeReusableGDI;

		public	Font Bold { get {
			return this.GetCachedFontWithStyle(FontStyle.Bold);
		} }


		FontCache() {
			this.fontsByStyle_dontDisposeReusableGDI = new Dictionary<FontStyle, Font>();
		}
		public FontCache(Font font) : this() {
			this.FontForeground = font;
			this.fontsByStyle_dontDisposeReusableGDI.Add(this.FontForeground.Style, this.FontForeground);
		}

		public Font GetCachedFontWithStyle(FontStyle labelFontStyle) {
			if (this.fontsByStyle_dontDisposeReusableGDI.ContainsKey(labelFontStyle) == false) {
				Font font = new Font(this.FontForeground, labelFontStyle);
				this.fontsByStyle_dontDisposeReusableGDI.Add(labelFontStyle, font);
			}
			Font ret = this.fontsByStyle_dontDisposeReusableGDI[labelFontStyle];
			return ret;
		}

	}
}
