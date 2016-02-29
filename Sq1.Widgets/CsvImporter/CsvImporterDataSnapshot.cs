using System;
using System.Collections.Generic;
using System.IO;

using CsvHelper.Configuration;
using Newtonsoft.Json;

namespace Sq1.Widgets.CsvImporter {
	public class CsvImporterDataSnapshot {
		[JsonProperty]	public	string				DownloadButtonURL;
		[JsonProperty]	public	string				PathCsv;
		[JsonProperty]	public	string				FileSelected;
		[JsonIgnore]	public	string				FileSelectedAbsname { get { return Path.Combine(this.PathCsv, this.FileSelected); } }
		[JsonIgnore]	public	CsvConfiguration	CsvConfiguration;				//jSON.deserialize() throws blaming CultureInfo
		[JsonProperty]	public	string				CsvDelimiter {
			get { return this.CsvConfiguration.Delimiter; }
			set { this.CsvConfiguration.Delimiter = value; }
		}

		[JsonProperty]			string				fieldSetupCurrentName;
		[JsonProperty]	public	string				FieldSetupCurrentName {
			// if you restrict SET, serializer won't be able to restore from JSON { get; private set; }
			get { return this.fieldSetupCurrentName; }
			set {
				this.fieldSetupCurrentName = value;
				this.olvModel = null;
			}
		}
		
		[JsonProperty]	public Dictionary<string, FieldSetup> FieldSetupsByName;
		[JsonIgnore]	public FieldSetup FieldSetupCurrent { get {
				if (this.FieldSetupsByName.ContainsKey(FieldSetupCurrentName) == false)  {
					string msg = "ScriptContextCurrentName[" + FieldSetupCurrentName + "] doesn't exist in CsvImporterDataSnapshot";
					throw new Exception(msg);
				}
				return this.FieldSetupsByName[this.FieldSetupCurrentName];
			} }
		
		[JsonIgnore]			List<FieldSetup>					olvModel;
		[JsonIgnore]	public	List<FieldSetup>					OLVModel { get {
				if (this.olvModel == null) {
					this.olvModel = new List<FieldSetup>();
					//ALWAYS_ZERO_FOR_FieldSetupsByName this.FieldSetupCurrent.Serno = 0;
					this.olvModel.Add(this.FieldSetupCurrent);
					this.olvModel.Add(new FieldSetup(FieldSetup.STUB_DISPLAY_FORMATS, 1));		// provokes OLV to display second row but AspectGetter shows CsvFormat instead
				}
				return this.olvModel;
			} }
		[JsonProperty]	public	Dictionary<string, List<string>>	CsvFieldTypeFormatsAvailable;
		
		
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
			this.DownloadButtonURL = "http://www.finam.ru/analysis/profile0000300007/";
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

C:\SquareOne\Data-debug\Workspaces\Default\Sq1.Widgets.CsvImporter.CsvImporterDataSnapshot.json

{
  "$type": "Sq1.Widgets.CsvImporter.CsvImporterDataSnapshot, Sq1.Widgets",
  "DownloadButtonURL": "http://www.finam.ru/analysis/profile0000300007/",
  "PathCsv": "C:\\SquareOne\\_finam",
  "FileSelected": "SPFB.RTS-3.16_151201_160225.txt",
  "fieldSetupCurrentName": "Default",
  "FieldSetupsByName": {
    "$type": "System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Sq1.Widgets.CsvImporter.FieldSetup, Sq1.Widgets]], mscorlib",
    "Default": [
      {
        "$type": "Sq1.Widgets.CsvImporter.ColumnCatcher, Sq1.Widgets",
        "ColumnSerno": 0,
        "Parser": {
          "$type": "Sq1.Widgets.CsvImporter.CsvTypeParser, Sq1.Widgets",
          "CsvType": 1,
          "CsvTypeFormat": ""
        }
      },
      {
        "$type": "Sq1.Widgets.CsvImporter.ColumnCatcher, Sq1.Widgets",
        "ColumnSerno": 1,
        "Parser": {
          "$type": "Sq1.Widgets.CsvImporter.CsvTypeParser, Sq1.Widgets",
          "CsvType": 10,
          "CsvTypeFormat": ""
        }
      },
      {
        "$type": "Sq1.Widgets.CsvImporter.ColumnCatcher, Sq1.Widgets",
        "ColumnSerno": 2,
        "Parser": {
          "$type": "Sq1.Widgets.CsvImporter.CsvTypeParser, Sq1.Widgets",
          "CsvType": 3,
          "CsvTypeFormat": "yyyyMMdd"
        }
      },
      {
        "$type": "Sq1.Widgets.CsvImporter.ColumnCatcher, Sq1.Widgets",
        "ColumnSerno": 3,
        "Parser": {
          "$type": "Sq1.Widgets.CsvImporter.CsvTypeParser, Sq1.Widgets",
          "CsvType": 4,
          "CsvTypeFormat": "HHmmss"
        }
      },
      {
        "$type": "Sq1.Widgets.CsvImporter.ColumnCatcher, Sq1.Widgets",
        "ColumnSerno": 4,
        "Parser": {
          "$type": "Sq1.Widgets.CsvImporter.CsvTypeParser, Sq1.Widgets",
          "CsvType": 5,
          "CsvTypeFormat": ""
        }
      },
      {
        "$type": "Sq1.Widgets.CsvImporter.ColumnCatcher, Sq1.Widgets",
        "ColumnSerno": 5,
        "Parser": {
          "$type": "Sq1.Widgets.CsvImporter.CsvTypeParser, Sq1.Widgets",
          "CsvType": 6,
          "CsvTypeFormat": ""
        }
      },
      {
        "$type": "Sq1.Widgets.CsvImporter.ColumnCatcher, Sq1.Widgets",
        "ColumnSerno": 6,
        "Parser": {
          "$type": "Sq1.Widgets.CsvImporter.CsvTypeParser, Sq1.Widgets",
          "CsvType": 7,
          "CsvTypeFormat": ""
        }
      },
      {
        "$type": "Sq1.Widgets.CsvImporter.ColumnCatcher, Sq1.Widgets",
        "ColumnSerno": 7,
        "Parser": {
          "$type": "Sq1.Widgets.CsvImporter.CsvTypeParser, Sq1.Widgets",
          "CsvType": 8,
          "CsvTypeFormat": ""
        }
      },
      {
        "$type": "Sq1.Widgets.CsvImporter.ColumnCatcher, Sq1.Widgets",
        "ColumnSerno": 8,
        "Parser": {
          "$type": "Sq1.Widgets.CsvImporter.CsvTypeParser, Sq1.Widgets",
          "CsvType": 9,
          "CsvTypeFormat": ""
        }
      }
    ]
  },
  "CsvFieldTypeFormatsAvailable": {
    "$type": "System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib]], mscorlib",
    "Open,High,Low,Close,Volume": [
      "#,###.##",
      "#.###,##"
    ],
    "Date": [
      "<TRY_ALL>",
      "d",
      "D",
      "d/M/yyyy",
      "dd/MM/yy",
      "dd/MM/yyyy",
      "MM-dd-yy",
      "yyyyMMdd",
      "yyyy-MM-dd",
      "yyyy-MMM-dd"
    ],
    "Time": [
      "<TRY_ALL>",
      "h:mm tt",
      "h:mm tt",
      "h:mm:ss tt",
      "HH':'mm':'ss 'GMT'",
      "HH':'mm':'ss'Z'",
      "HHmmss",
      "hmmss",
      "t",
      "T",
      "'T'HH':'mm':'ss",
      "THHmmssZ"
    ]
  },
  "CsvDelimiter": ",",
  "FieldSetupCurrentName": "Default"
}
*/