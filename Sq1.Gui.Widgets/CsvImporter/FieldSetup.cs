using System;
using System.Collections.Generic;

namespace Sq1.Widgets.CsvImporter {
	public class FieldSetup : List<ColumnCatcher>	{
		public const string STUB_DISPLAY_FORMATS = "STUB_DISPLAY_FORMATS";
		public bool IsStubDisplayFormats { get { return this.Name == STUB_DISPLAY_FORMATS; } }
		
		public string Name;
		public int RowIndexInObjectListView { get; private set; }
		
		public string DateAndTimeFormatsGlued {
			get {
				string ret = "";
				foreach (ColumnCatcher i in this) {
					if (i.Parser.CsvType != CsvFieldType.Date) continue;
					ret += i.Parser.CsvTypeFormat;
					//ret += "yyyyMMdd";
					break;
				}
				foreach (ColumnCatcher i in this) {
					if (i.Parser.CsvType != CsvFieldType.Time) continue;
					ret += " " + i.Parser.CsvTypeFormat;
					//ret += " HHmmss";
				break;
				}
				return ret;
			}
		}
		
		// JSON throws if default ctor is missing
		public FieldSetup() : this("JUST_DESERIALIZED") {
		}
		public FieldSetup(string name = "UNDEFINED", int serno = 0) : base() {
			this.Name = name;
			this.RowIndexInObjectListView = serno;
		}
	}
}
