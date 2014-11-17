using Sq1.Core;
using Sq1.Core.Support;

namespace Sq1.Adapters.QuikMock.Dde {
	public class DdeChannelsMock {
		public DdeChannelLastQuoteMock	ChannelQuote		{ get; protected set; }
		public string					Symbol				{ get; protected set; }
		public IStatusReporter			ConnectionStatus	{ get; protected set; }

		public DdeChannelsMock(StreamingMock receiver, string symbol, IStatusReporter ConnectionStatus) {
			this.Symbol = symbol;
			this.ChannelQuote = new DdeChannelLastQuoteMock(receiver, symbol);
			//this.ChannelDepth = new DdeChannelDepth(streamingProvider, symbol);
			//this.ChannelHistory = new DdeChannelHistory(streamingProvider, quikTerminal, Symbol);
			this.ConnectionStatus = ConnectionStatus;
		}
		public void StartDdeServer() {
			if ((ChannelQuote is DdeChannelLastQuoteMock) == false) return;
			string msg = "MOCK: will generate quotes for symbol[" + Symbol + "]"
				+ " every [" + ChannelQuote.nextQuoteDelayMs + "]ms"
				+ " for " + ChannelQuote + "]"
				+ " instead of registering real DDE server";
			Assembler.PopupException(msg, null, false);
		}
	}
}
