using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Support;
using Sq1.Core.Streaming;

using Sq1.Widgets.Level2;

using Sq1.Adapters.Quik.Streaming.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Dde {
	public class DdeTableDepth : XlDdeTableMonitoreable<LevelTwoOlv> {
		protected override string DdeConsumerClassName { get { return "DdeTableDepth"; } }

		string			symbol;
		LevelTwoHalf	levelTwoAsks;
		LevelTwoHalf	levelTwoBids;
		LevelTwoOlv		levelTwoProxy;

		public	SymbolInfo		SymbolInfo		{ get; private set; }

		public DdeTableDepth(string topic, QuikStreaming quikStreaming, List<XlColumn> columns, string symbol) : base(topic, quikStreaming, columns) {
			this.symbol = symbol;
			this.SymbolInfo = Assembler.InstanceInitialized.RepositorySymbolInfos.FindSymbolInfoOrNew(symbol);

			this.levelTwoAsks = base.QuikStreaming.StreamingDataSnapshot.LevelTwoAsks_getForSymbol(this.symbol);
			this.levelTwoBids = base.QuikStreaming.StreamingDataSnapshot.LevelTwoBids_getForSymbol(this.symbol);

			if (this.levelTwoBids != null && this.levelTwoAsks != null) {
				this.levelTwoProxy = new LevelTwoOlv(this.levelTwoBids, this.levelTwoAsks, this.SymbolInfo);
			}
		}
		protected override void IncomingTableBegun() {
			this.levelTwoAsks.Clear(this, "IncomingTableBegun");
			this.levelTwoBids.Clear(this, "IncomingTableBegun");
			this.levelTwoAsks.WaitAndLockFor(this, "IncomingTableBegun");
			this.levelTwoBids.WaitAndLockFor(this, "IncomingTableBegun");
		}
		protected override void IncomingTableTerminated() {
			this.levelTwoAsks.UnLockFor(this, "IncomingTableBegun");
			this.levelTwoBids.UnLockFor(this, "IncomingTableBegun");
			string msg = "send a notify event or Chart is already Wait(indefinitely)'ing to get a FrozenHalfs  and go draw them?...";

			// is ChartControl.chartControl_BarStreamingUpdatedMerged_ShouldTriggerRepaint_WontUpdateBtnTriggeringScriptTimeline() invoked?
			//base.QuikStreaming.(quikQuote);
			msg = "notifying DomResizeableUserControl now:";
			base.IncomingTableTerminated();
		}

		//protected override void IncomingTableRow_convertToDataStructure(XlRowParsed row) {
		protected override LevelTwoOlv IncomingTableRow_convertToDataStructure_monitoreable(XlRowParsed row) {
			double bidVolume	= (double)row["SELL_VOLUME"];
			double price		= (double)row["PRICE"];
			double askVolume	= (double)row["BUY_VOLUME"];
			if (double.IsNaN(bidVolume) == false) {		// where Blank become NaN?
				//base.QuikStreaming.StreamingDataSnapshot.LevelTwoBids_refactorBySymbol.Add(price, bidVolume, this, "IncomingRowParsedPush");
				levelTwoBids.Add(price, bidVolume, this, "IncomingRowParsedPush");
			} else {
				//base.QuikStreaming.StreamingDataSnapshot.LevelTwoAsks_refactorBySymbol.Add(price, askVolume, this, "IncomingRowParsedPush");
				levelTwoAsks.Add(price, askVolume, this, "IncomingRowParsedPush");
			}
			return this.levelTwoProxy;		// growing each time
		}
		public override string ToString() {
			string ret = "";

			if (string.IsNullOrEmpty(this.Topic) == false) ret += this.Topic;

			ret += " "	+ this.DdeMessagesReceived;
			ret += ":"	+ this.DdeRowsReceived;

			string connectionStatus = "NEVER_CONNECTED_DDE";
			if (this.DdeConnectedTimes > 0) {
				if (this.TopicConnected) {
					connectionStatus = "LISTENING_DDE#";
					if (this.DdeMessagesReceived > 0) connectionStatus = "RECEIVING_DDE#";
				} else {
					connectionStatus = "DISCONNECTED_DDE#";
				}
				connectionStatus += this.DdeConnectedTimes;
			}

			ret += " "	+ connectionStatus;
			ret += " " + (this.ColumnsIdentified ? "columnsIdentified" : "columnsNotIdentified");
			return ret;
		}
	}
}