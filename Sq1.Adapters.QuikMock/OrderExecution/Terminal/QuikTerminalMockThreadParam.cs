using System;

using Sq1.Core.Execution;

namespace Sq1.Adapters.QuikMock.Terminal {
	public class QuikTerminalMockThreadParam {
		public int SernoSession, GUID, Balance, QuikStatus, IsSell, Filled;
		public string ClassCode, SecCode;
		public double Price, SernoExchange;
		public OrderState OrderStatus;
		public int TryFillInvokedTimes;
		public bool IsKillCallback;
		public string KillerGUID;
	}
}
