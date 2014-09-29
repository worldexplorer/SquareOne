using System;
using System.Globalization;

namespace Sq1.Widgets.CsvImporter {
	public class CsvTypeParser {
		public const string FORMAT_VISUALIZE_EMPTY_STRING = "<NO_FORMAT>";
		public const string FORMAT_TRY_ALL = "<TRY_ALL>";

		public CsvFieldType CsvType;
		public string CsvTypeFormat;
		
		public CsvTypeParser() {
			this.CsvType = CsvFieldType.Unknown;
			this.CsvTypeFormat = "";
		}

		public void ParseAndPushThrows(string csvValue, CsvBar targetCsvBar, string formatForDateTimeGlued) {
			switch(this.CsvType) {
				case CsvFieldType.Date:
					this.glueDateParts(csvValue, targetCsvBar);
					break;

				case CsvFieldType.Time:
					string dateAndTime = this.glueDateParts(csvValue, targetCsvBar);
					DateTime dateTimeParsed;
					try {
						dateTimeParsed = DateTime.ParseExact(dateAndTime, formatForDateTimeGlued, null);
					} catch (Exception ex) {
						string msg = "DateTime.ParseExact(" + dateAndTime + ", " + formatForDateTimeGlued + ", null)\r\n" + ex.Message;
						throw new Exception(msg, ex);
					}
					try {
						targetCsvBar.AbsorbOneValueFromStringThrows(dateTimeParsed, CsvFieldType.DateTime);
					} catch (Exception ex) {
						string msg = "AbsorbOneValueFromStringThrows(" + dateAndTime + ", " + this.CsvType + ")\r\n" + ex.Message;
						throw new Exception(msg, ex);
					}
					break;
					
				case CsvFieldType.Unknown:
				case CsvFieldType.Ignore:
				case CsvFieldType.Symbol:
				case CsvFieldType.Interval_Min:
				case CsvFieldType.Interval_Hrs:
				case CsvFieldType.Interval_Days:
					break;

				default:
					object csvValueParsed = this.parse(csvValue);
					targetCsvBar.AbsorbOneValueFromStringThrows(csvValueParsed, this.CsvType);
					break;
			}
		}
		string glueDateParts(string csvValue, CsvBar targetCsvBar) {
			if (this.CsvType == CsvFieldType.Date) {
				targetCsvBar.DateAsString = csvValue;
				return null;
			}
			if (this.CsvType == CsvFieldType.Time) targetCsvBar.TimeAsString = csvValue;
			return targetCsvBar.DateAsString + " " + targetCsvBar.TimeAsString;
		}
		object parse(string value, CsvFieldType typeOverridden = CsvFieldType.Unknown) {
			object ret = null;
			
			if (typeOverridden == CsvFieldType.Unknown) typeOverridden = this.CsvType;
			switch(typeOverridden) {
				case CsvFieldType.DateTime:
					if (this.CsvTypeFormat == FORMAT_VISUALIZE_EMPTY_STRING) {
						ret = DateTime.Parse(value);
					} else {
						ret = DateTime.ParseExact(value, this.CsvTypeFormat, CultureInfo.InvariantCulture);
					}
					break;
					
				case CsvFieldType.Open:
				case CsvFieldType.High:
				case CsvFieldType.Low:
				case CsvFieldType.Close:
				case CsvFieldType.Volume:
					//http://stackoverflow.com/questions/1354924/how-do-i-parse-a-string-with-a-decimal-point-to-a-double
					ret	= float.Parse(value);
					break;

				case CsvFieldType.Date:
				case CsvFieldType.Time:
				case CsvFieldType.Unknown:
				case CsvFieldType.Ignore:
				case CsvFieldType.Symbol:
				case CsvFieldType.Interval_Min:
				case CsvFieldType.Interval_Hrs:
				case CsvFieldType.Interval_Days:
					break;

				default:
					throw new Exception("CsvTypeParser.parse():\r\nadd handler for [" + typeOverridden + "]");
			}
			return ret;
		}
	}
}
