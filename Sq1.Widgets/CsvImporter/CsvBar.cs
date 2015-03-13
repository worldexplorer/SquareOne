using System;

using BrightIdeasSoftware;

namespace Sq1.Widgets.CsvImporter {
	public class CsvBar {
		//public string Symbol;
		public DateTime DateTime = DateTime.MinValue;
		public float Open = 0;
		public float High = 0;
		public float Low = 0;
		public float Close = 0;
		public float Volume = -1;

		[OLVIgnore]
		public string DateAsString {get;set;}
		public string TimeAsString {get;set;}
		
		public bool IsFullyFilledDOHLCV {
			get {
				return this.DateTime != DateTime.MinValue
					&& this.Open > 0
					&& this.High > 0
					&& this.Low > 0
					&& this.Close > 0
					&& this.Volume > -1;
			}
		}
		public bool IsValueValid(CsvFieldType fieldType) {
			switch(fieldType) {
				case CsvFieldType.DateTime: 	return this.DateTime != DateTime.MinValue;
				case CsvFieldType.Open: 		return this.Open > 0;
				case CsvFieldType.High: 		return this.High > 0;
				case CsvFieldType.Low: 			return this.Low > 0;
				case CsvFieldType.Close: 		return this.Close > 0;
				case CsvFieldType.Volume: 		return this.Volume > -1;
				default:
					return true;
			}
		}
		
		public void AbsorbOneValueFromStringThrows(object value, CsvFieldType fieldType) {
			if (value == null) {
				throw new Exception("CsvBar.AbsorbOneValueFromStringThrows(): don't make me absorb value=NULL");
			}
			switch(fieldType) {
				case CsvFieldType.DateTime: 	DateTime		= (DateTime)	value;	break;
				case CsvFieldType.Date:			DateAsString	= (string)		value;	break;
				case CsvFieldType.Time:			TimeAsString	= (string)		value;	break;
				case CsvFieldType.Open: 		Open			= (float)		value;	break;
				case CsvFieldType.High: 		High			= (float)		value;	break;
				case CsvFieldType.Low: 			Low				= (float)		value;	break;
				case CsvFieldType.Close: 		Close			= (float)		value;	break;
				case CsvFieldType.Volume: 		Volume			= (float)		value;	break;

				case CsvFieldType.Unknown:
				case CsvFieldType.Ignore:
				case CsvFieldType.Symbol:
				case CsvFieldType.Interval_Min:
				case CsvFieldType.Interval_Hrs:
				case CsvFieldType.Interval_Days:
					break;

				default:
					throw new Exception("CsvBar.AbsorbOneValueFromString():\r\nadd handler for [" + fieldType + "]");
			}
		}
		public override string ToString() {
			return "T[" + DateTime + "]"
				+ "O[" + Math.Round(Open, 3) + "]"
				+ "H[" + Math.Round(High, 3) + "]"
				+ "L[" + Math.Round(Low, 3) + "]"
				+ "C[" + Math.Round(Close, 3) + "]"
				+ "V[" + Math.Round(Volume, 3) + "]"
				;
		}
	}
}
