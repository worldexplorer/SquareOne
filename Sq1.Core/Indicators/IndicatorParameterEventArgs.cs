using System;

namespace Sq1.Core.Indicators {
	public class IndicatorParameterEventArgs : EventArgs {
		public IndicatorParameter IndicatorParameter { get; protected set; }
		public IndicatorParameterEventArgs(IndicatorParameter indicatorParameter) {
			IndicatorParameter = indicatorParameter;
		}
	}
}
