using Sq1.Core;
using Sq1.Core.Support;

namespace Sq1.Adapters.QuikMock.Dde {
	public class DdeChannelsMock {
		public DdeChannelLastQuoteMock	ChannelQuote		{ get; protected set; }
		public string					Symbol				{ get; protected set; }
		public string					Ident				{ get { return "DDE_MOCK_SINEWAVE_GENERATOR[" + Symbol + "/" + this.ChannelQuote.nextQuoteDelayMs + "ms] "; } }


		public DdeChannelsMock(StreamingMock receiver, string symbol) {
			this.Symbol = symbol;
			this.ChannelQuote = new DdeChannelLastQuoteMock(receiver, symbol);
			//this.ChannelDepth = new DdeChannelDepth(streamingProvider, symbol);
			//this.ChannelHistory = new DdeChannelHistory(streamingProvider, quikTerminal, Symbol);
		}
		public string AllChannelsForSymbolStart() {
			string ret = "START_STATUS_UNKONWN";
			if ((this.ChannelQuote is DdeChannelLastQuoteMock) == false) {
				ret += " (this.ChannelQuote is DdeChannelLastQuoteMock) == false";
				return this.Ident + ret;
			}
			if (this.ChannelQuote.MockRunning) {
				ret = "CHANNEL_QUOTE_ALREADY_STARTED " + this.ChannelQuote.ToString();
				return this.Ident + ret;
			}
			this.ChannelQuote.MockStart();
			ret = "STARTED";
			//+ " for " + this.ChannelQuote.ToString() + "]"
			//+ " instead of registering a real DDE server"
			return this.Ident + ret;
		}
		public string AllChannelsForSymbolStop() {
			string ret = "STOP_STATUS_UNKONWN";
			if (this.ChannelQuote.MockRunning == false) {
				ret = "CHANNEL_QUOTE_ALREADY_STOPPED " + this.ChannelQuote.ToString();
				return this.Ident + ret;
			}
			this.ChannelQuote.MockStop();
			ret = "STOPPED";
			return this.Ident + ret;
		}
		public override string ToString() {
			string ret = Symbol + ":";
			ret += " ChannelQuote{" + this.ChannelQuote.MockRunningAsString + "}[" + this.ChannelQuote.ToString() + "]";
			ret += " ChannelLevel2Snap{}[...]";
			ret += " ChannelOrderLog{}[...]";
			return ret;
		}
	}
}
