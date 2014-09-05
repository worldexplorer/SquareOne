using System;

namespace Sq1.Core.Charting.OnChart {
	// these are successfull codes; anything besides these is an error;
	// errors are logged using Assembler.PopupException() by Sq1.Charting.ChartControl
	public enum OnChartObjectOperationStatus {
		Unknown = 0,
		OnChartObjectJustCreated = 1,
//		CreatedButAdjustedToBarsAvailable = 2,
		OnChartObjectModified = 3,
//		ModifiedButAdjustedToBarsAvailable = 4,
		OnChartObjectNotModifiedSinceParametersDidntChange = 5
	}
}
