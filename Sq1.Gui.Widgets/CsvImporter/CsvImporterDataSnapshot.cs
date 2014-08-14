using System;
using System.Collections.Generic;
using System.IO;

using CsvHelper.Configuration;
using Newtonsoft.Json;

namespace Sq1.Widgets.CsvImporter {
	public class CsvImporterDataSnapshot {
		public string PathCsv;
		public string FileSelected;
		
		[JsonIgnore]
		public string FileSelectedAbsname { get { return Path.Combine(this.PathCsv, this.FileSelected); } }
		[JsonIgnore]	//jSON.deserialize() throws blaming CultureInfo
		public CsvConfiguration CsvConfiguration;
		public string CsvDelimiter {
			get { return this.CsvConfiguration.Delimiter; }
			set { this.CsvConfiguration.Delimiter = value; }
		}

		string fieldSetupCurrentName;
		public string FieldSetupCurrentName {
			// if you restrict SET, serializer won't be able to restore from JSON { get; private set; }
			get { return this.fieldSetupCurrentName; }
			set {
				this.fieldSetupCurrentName = value;
				this.olvModel = null;
			}
		}
		
		public Dictionary<string, FieldSetup> FieldSetupsByName;

		[JsonIgnore]
		public FieldSetup FieldSetupCurrent {
			get {
				if (this.FieldSetupsByName.ContainsKey(FieldSetupCurrentName) == false)  {
					string msg = "ScriptContextCurrentName[" + FieldSetupCurrentName + "] doesn't exist in CsvImporterDataSnapshot";
					throw new Exception(msg);
				}
				return this.FieldSetupsByName[this.FieldSetupCurrentName];
			}
		}
		
		[JsonIgnore]
		private List<FieldSetup> olvModel;
		[JsonIgnore]
		public List<FieldSetup> OLVModel {
			get {
				if (this.olvModel == null) {
					this.olvModel = new List<FieldSetup>();
					//ALWAYS_ZERO_FOR_FieldSetupsByName this.FieldSetupCurrent.Serno = 0;
					this.olvModel.Add(this.FieldSetupCurrent);
					this.olvModel.Add(new FieldSetup(FieldSetup.STUB_DISPLAY_FORMATS, 1));		// provokes OLV to display second row but AspectGetter shows CsvFormat instead
				}
				return this.olvModel;
			}
		}
		
		public Dictionary<string, List<string>> CsvFieldTypeFormatsAvailable;
		public string GetAppropriateFormatKeyForType(CsvFieldType type) {
			string ret = null;
			foreach (string key in this.CsvFieldTypeFormatsAvailable.Keys) {
				if (key.Contains(type.ToString()) == false) continue;
				ret = key;
				break;
			}
			return ret;
		}
		public List<string> FormatsAvailableForType(CsvFieldType type) {
			string storedUnderKey = this.GetAppropriateFormatKeyForType(type);
			if (storedUnderKey == null) {
				return new List<string>() { CsvTypeParser.FORMAT_VISUALIZE_EMPTY_STRING };
			}
			return this.CsvFieldTypeFormatsAvailable[storedUnderKey];
		}
		
		public CsvImporterDataSnapshot() {
			this.PathCsv = "C:\\";
			this.FieldSetupCurrentName = "Default";
			this.FieldSetupsByName = new Dictionary<string, FieldSetup>();
			this.FieldSetupsByName.Add(this.FieldSetupCurrentName, new FieldSetup(this.FieldSetupCurrentName));
			
			this.CsvConfiguration = new CsvConfiguration();
			this.CsvConfiguration.Delimiter = ";";
			this.CsvConfiguration.AllowComments = true;
			this.CsvConfiguration.TrimFields = true;
			
			this.CsvFieldTypeFormatsAvailable = new Dictionary<string, List<string>>();
			this.CsvFieldTypeFormatsAvailable.Add("Open,High,Low,Close,Volume", new List<string>() {"#,###.##", "#.###,##"});
			
			//http://www.csharp-examples.net/string-format-datetime/ http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx http://msdn.microsoft.com/en-us/library/az4se3k1.aspx
			this.CsvFieldTypeFormatsAvailable.Add("Date", new List<string>() {CsvTypeParser.FORMAT_TRY_ALL,
				"d", "D", "yyyyMMdd", "yyyy-MM-dd", "yyyy-MMM-dd",
				"d/M/yyyy", "dd/MM/yyyy", "dd/MM/yy",
				"MM-dd-yy"});
			this.CsvFieldTypeFormatsAvailable["Date"].Sort();

			this.CsvFieldTypeFormatsAvailable.Add("Time", new List<string>() {CsvTypeParser.FORMAT_TRY_ALL,
				"t", "T", "HHmmss", "hmmss", "h:mm tt", "h:mm:ss tt", "h:mm tt",
				"HH':'mm':'ss 'GMT'", "'T'HH':'mm':'ss", "HH':'mm':'ss'Z'",
				"THHmmssZ"});
			this.CsvFieldTypeFormatsAvailable["Time"].Sort();
		}
		public void AddFormatForTypeUnique(string selectedFormat, CsvFieldType csvFieldType) {
			string storedUnderKey = this.GetAppropriateFormatKeyForType(csvFieldType);
			if (storedUnderKey == null) {
				//throw new Exception("CANT_GUESS_KEY_FOR_FORMATS_AVAILABLE_FOR_TYPE dataSnapshot.GetAppropriateFormatKeyForType(" + iCatcherEdited.Parser.CsvType + ")=null");
				storedUnderKey = csvFieldType.ToString();
				this.CsvFieldTypeFormatsAvailable.Add(storedUnderKey, new List<string>());
			}
			List<string> typeFormats = this.CsvFieldTypeFormatsAvailable[storedUnderKey];
			if (typeFormats.Contains(selectedFormat) == false) {
				typeFormats.Add(selectedFormat);
				typeFormats.Sort();
			}
		}
	}
}

/*
  "CsvConfiguration": {
    "$type": "CsvHelper.Configuration.CsvConfiguration, CsvHelper",
    "Maps": {
      "$type": "CsvHelper.Configuration.CsvClassMapCollection, CsvHelper"
    },
    "PropertyBindingFlags": 20,
    "HasHeaderRecord": true,
    "HasExcelSeparator": false,
    "WillThrowOnMissingField": true,
    "DetectColumnCountChanges": false,
    "IsHeaderCaseSensitive": true,
    "IgnoreHeaderWhiteSpace": false,
    "IgnoreReferences": false,
    "TrimHeaders": false,
    "TrimFields": true,
    "Delimiter": ",",
    "Quote": "\"",
    "QuoteString": "\"",
    "DoubleQuoteString": "\"\"",
    "QuoteRequiredChars": [
      "\r",
      "\n",
      ","
    ],
    "Comment": "#",
    "AllowComments": true,
    "BufferSize": 2048,
    "QuoteAllFields": false,
    "QuoteNoFields": false,
    "CountBytes": false,
    "Encoding": {
      "$type": "System.Text.UTF8Encoding, mscorlib",
      "BodyName": "utf-8",
      "EncodingName": "Unicode (UTF-8)",
      "HeaderName": "utf-8",
      "WebName": "utf-8",
      "WindowsCodePage": 1200,
      "IsBrowserDisplay": true,
      "IsBrowserSave": true,
      "IsMailNewsDisplay": true,
      "IsMailNewsSave": true,
      "IsSingleByte": false,
      "EncoderFallback": {
        "$type": "System.Text.EncoderReplacementFallback, mscorlib",
        "DefaultString": "�",
        "MaxCharCount": 1
      },
      "DecoderFallback": {
        "$type": "System.Text.DecoderReplacementFallback, mscorlib",
        "DefaultString": "�",
        "MaxCharCount": 1
      },
      "IsReadOnly": true,
      "CodePage": 65001
    },
    "SkipEmptyRecords": false,
    "IgnoreQuotes": false,
    "IgnorePrivateAccessor": false,
    "IgnoreBlankLines": true,
    "PrefixReferenceHeaders": false,
    "IgnoreReadingExceptions": false,
    "ReadingExceptionCallback": null
  },
*/