using System;

namespace Sq1.Widgets.CsvImporter {
	public enum CsvFieldType {
		Ignore,
		Symbol,
		DateTime,
		Date,
		Time,
		Open,
		High,
		Low,
		Close,
		Volume,
		Interval_Min,
		Interval_Hrs,
		Interval_Days,
		Unknown
	}
}