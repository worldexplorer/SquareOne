using System;
using System.Text;

namespace Sq1.Core.Execution {
	public class PositionEventArgs : EventArgs {
		public Position Position;

		public PositionEventArgs(Execution.Position position) {
			this.Position = position;
		}

	}
}
