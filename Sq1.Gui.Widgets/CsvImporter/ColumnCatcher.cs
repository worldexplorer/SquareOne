using System;
using System.Collections.Generic;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Newtonsoft.Json;
using Sq1.Core;

namespace Sq1.Widgets.CsvImporter {
	public class ColumnCatcher {
		public int ColumnSerno;
		public CsvTypeParser Parser;
		[JsonIgnore]
		public CsvImporterDataSnapshot DataSnapshot;
		[JsonIgnore]
		public CsvFieldType CsvTypeContentSuggested = CsvFieldType.Unknown;
			
		public ColumnCatcher() {
			this.Parser = new CsvTypeParser();
		}
		
		// JSON throws if default ctor is missing
		// NOPE_DEFAULT_FOR_FieldSetup:FieldSetup_WAS_NEEDED public ColumnCatcher() {}
		public ColumnCatcher(int i, CsvImporterDataSnapshot dataSnapshot) : this() {
			this.ColumnSerno = i;
			this.DataSnapshot = dataSnapshot;
		}
		public object AspectGetterParsedRaw(object shouldBeListOfStrings) {
			List<string> pi = shouldBeListOfStrings as List<string>;
			if (pi == null) {
				Assembler.PopupException("shouldBeListOfStrings is not List<string>");
				this.Parser.CsvType = this.CsvTypeContentSuggested = CsvFieldType.Ignore;
				return "NOT_LIST<STRING>";
			}
			if (pi.Count == 0) {
				Assembler.PopupException("shouldBeListOfStrings.Count=0");
				this.Parser.CsvType = this.CsvTypeContentSuggested = CsvFieldType.Ignore;
				return "LIST<STRING>.COUNT=0";
			}
			try {
				var ret = pi[this.ColumnSerno];
				//this.guessCsvFieldType(ret);
				return ret;
			} catch (Exception ex) {
				this.Parser.CsvType = this.CsvTypeContentSuggested = CsvFieldType.Ignore;
				return "OUT_OF_BOUNDARY";
			}
		}
		public object AspectGetterFieldSetup(object shouldBeListOfCatchers) {
			try {
				FieldSetup thisRowFieldSetup = shouldBeListOfCatchers as FieldSetup;
				if (thisRowFieldSetup == null) {
					Assembler.PopupException("shouldBeListOfCatchers is not FieldSetup");
					return "NOT_LIST<ColumnCatcher>";
				}
				
				switch (thisRowFieldSetup.RowIndexInObjectListView) {
					case 0:
						if (thisRowFieldSetup.Count == 0) {
							Assembler.PopupException("shouldBeListOfCatchers.Count=0");
							return "LIST<ColumnCatcher>.COUNT=0";
						}
						if (thisRowFieldSetup.Name != this.DataSnapshot.FieldSetupCurrent.Name) {
							return "NOT_FieldSetupCurrent.Name";
						}
						if (this.DataSnapshot.OLVModel.IndexOf(thisRowFieldSetup) != 0) {
							return "NOT_IN_OLVMODEL";
						}
						CsvTypeParser parser = thisRowFieldSetup[this.ColumnSerno].Parser;
						return parser.CsvType;
						//break;
					case 1:
						if (thisRowFieldSetup.Name != "STUB_DISPLAY_FORMATS") {
							return "NOT_STUB_DISPLAY_FORMATS";
						}
						FieldSetup prevRowFieldSetup = this.DataSnapshot.OLVModel[0];
						CsvTypeParser prevRowParser = prevRowFieldSetup[this.ColumnSerno].Parser;
						return prevRowParser.CsvTypeFormat;
						//break;
					default:
						return "2ROWS_MAX";
						//break;
				}
			} catch (Exception ex) {
				return "WRD" + ex.Message;
			}
		}
//		void guessCsvFieldType(string sample, bool forceGuessingAgain = false) {
//			if (this.CsvTypeContentSuggested != CsvFieldType.Unknown && forceGuessingAgain == false) return;
//			float floatParsed;
//			if (float.TryParse(sample, out floatParsed)) {
//				this.csvTypeUserSelected = this.CsvTypeContentSuggested = CsvFieldType.Open;
//				return;
//			}
//			DateTime dateTimeParsed;
//			if (DateTime.TryParse(sample, out dateTimeParsed)) {
//				this.csvTypeUserSelected = this.CsvTypeContentSuggested = CsvFieldType.DateTime;
//				return;
//			}
//			this.CsvTypeContentSuggested = CsvFieldType.Symbol;
//		}
		
		
		public ComboBox CreateEditorTypesAvailable() {
			ComboBox cb = new ComboBox();	//http://sourceforge.net/p/objectlistview/discussion/812922/thread/b2c57809
            List<ComboBoxItem> values = new List<ComboBoxItem>();
			foreach (object value in Enum.GetValues(typeof(CsvFieldType))) {
                values.Add(new ComboBoxItem(value, Enum.GetName(typeof(CsvFieldType), value)));
			}
            cb.Items.AddRange(values.ToArray());
			try {
				int index1 = (int) this.Parser.CsvType;
				//int index2 = cb.Items.IndexOf(this.CsvTypeContentSuggested);
            	cb.SelectedIndex = index1;
			} catch (Exception ex) {
				Assembler.PopupException("olvFieldSetup_CellEditStarting(): cb.SelectedIndex=[" + this.Parser.CsvType + "] failed");
			}
			return cb;
		}
		public ComboBox CreateEditorFormatsAvailable() {
			ComboBox cb = new ComboBox();	//http://sourceforge.net/p/objectlistview/discussion/812922/thread/b2c57809
            List<string> values = this.DataSnapshot.FormatsAvailableForType(this.Parser.CsvType);
            cb.Items.AddRange(values.ToArray());
			try {
            	string fmt = this.Parser.CsvTypeFormat;
            	if (string.IsNullOrEmpty(fmt)) fmt = CsvTypeParser.FORMAT_VISUALIZE_EMPTY_STRING;
				int indexToHighlight = values.IndexOf(fmt);
				if (indexToHighlight != -1) {
	            	cb.SelectedIndex = indexToHighlight;
				}
			} catch (Exception ex) {
				Assembler.PopupException("olvFieldSetup_CellEditStarting(): cb.SelectedIndex=[" + this.Parser.CsvType + "] failed");
			}
			return cb;
		}
	}
}
