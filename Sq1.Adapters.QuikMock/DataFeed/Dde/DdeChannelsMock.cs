using Sq1.Core;
using Sq1.Core.Support;

namespace Sq1.Adapters.QuikMock.Dde {
	public class DdeChannelsMock {
		public DdeChannelLastQuoteMock	ChannelQuote		{ get; protected set; }
		public string					Symbol				{ get; protected set; }
		public IStatusReporter			StatusReporter	{ get; protected set; }

		public DdeChannelsMock(StreamingMock receiver, string symbol, IStatusReporter statusReporter) {
			this.Symbol = symbol;
			this.ChannelQuote = new DdeChannelLastQuoteMock(receiver, symbol);
			//this.ChannelDepth = new DdeChannelDepth(streamingProvider, symbol);
			//this.ChannelHistory = new DdeChannelHistory(streamingProvider, quikTerminal, Symbol);
			this.StatusReporter = statusReporter;
		}
		public string DdeServerStart() {
			string ret = "DdeChannelsMock_HAVENT_STARTED";
			if ((ChannelQuote is DdeChannelLastQuoteMock) == false) return ret;
			
			this.ChannelQuote.MockStart();
			ret = "DdeChannelsMock: will generate quotes for symbol[" + Symbol + "]"
				+ " every [" + this.ChannelQuote.nextQuoteDelayMs + "]ms"
				+ " for " + this.ChannelQuote.ToString() + "]"
				+ " instead of registering a real DDE server";
			//Assembler.PopupException(msg, null, false);
			return ret;
		}
	}
}
