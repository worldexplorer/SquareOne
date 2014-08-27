using System;

namespace Sq1.Core.Charting.OnChart {
	// these are successfull codes; anything besides these is an error;
	// errors are logged using Assembler.PopupException() by Sq1.Charting.ChartControl
	public enum ChartOperationStatus {
		Unknown = 0,
		JustCreated = 1,
//		CreatedButAdjustedToBarsAvailable = 2,
		Modified = 3,
//		ModifiedButAdjustedToBarsAvailable = 4,
		NotModifiedSinceParametersDidntChange = 5
	}
}
