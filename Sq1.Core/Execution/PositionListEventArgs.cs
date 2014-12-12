using System;
using System.Collections.Generic;

namespace Sq1.Core.Execution {
	public class PositionListEventArgs : EventArgs {
		public List<Position> PositionsOpenedNow;

		public PositionListEventArgs(List<Position> positionsUpdatedDueToStreamingNewQuote) {
			this.PositionsOpenedNow = positionsUpdatedDueToStreamingNewQuote;
		}
	}
}
