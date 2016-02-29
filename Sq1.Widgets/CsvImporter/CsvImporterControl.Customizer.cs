using System;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;

namespace Sq1.Widgets.CsvImporter {
	public partial class CsvImporterControl  {
		void olvParsedByFormat_customize() {
			this.olvcSerno.AspectGetter = delegate(object o) {
				Bar bar = o as Bar;
				if (bar == null) return "olvcSerno.AspectGetter: bar=null";
				return bar.ParentBarsIndex;
			};
			this.olvcDateTime.AspectGetter = delegate(object o) {
				Bar bar = o as Bar;
				if (bar == null) return "olvcDateTime.AspectGetter: bar=null";
				return bar.DateTimeOpen_formatted;
			};
			this.olvcOpen.AspectGetter = delegate(object o) {
				Bar bar = o as Bar;
				if (bar == null) return "olvcOpen.AspectGetter: bar=null";
				return bar.Open_formatted;
			};
			this.olvcHigh.AspectGetter = delegate(object o) {
				Bar bar = o as Bar;
				if (bar == null) return "olvcHigh.AspectGetter: bar=null";
				return bar.High_formatted;
			};
			this.olvcLow.AspectGetter = delegate(object o) {
				Bar bar = o as Bar;
				if (bar == null) return "olvcLow.AspectGetter: bar=null";
				return bar.Low_formatted;
			};
			this.olvcClose.AspectGetter = delegate(object o) {
				Bar bar = o as Bar;
				if (bar == null) return "olvcClose.AspectGetter: bar=null";
				return bar.Close_formatted;
			};
			this.olvcVolume.AspectGetter = delegate(object o) {
				Bar bar = o as Bar;
				if (bar == null) return "olvcVolume.AspectGetter: bar=null";
				return bar.Volume_formatted;
			};
		}
	}
}
