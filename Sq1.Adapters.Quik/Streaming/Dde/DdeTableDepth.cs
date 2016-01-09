using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Support;

using Sq1.Adapters.Quik.Streaming.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Dde {
	public class DdeTableDepth : XlDdeTable {
		protected override string DdeConsumerClassName { get { return "DdeTableDepth"; } }

		string			symbol;

		ConcurrentDictionaryGeneric<double, double> levelTwoAsks { get { return base.QuikStreaming.StreamingDataSnapshot.LevelTwoAsks; } }
		ConcurrentDictionaryGeneric<double, double> levelTwoBids { get { return base.QuikStreaming.StreamingDataSnapshot.LevelTwoBids; } }

		public DdeTableDepth(string topic, QuikStreaming quikStreaming, List<XlColumn> columns, string symbol) : base(topic, quikStreaming, columns) {
			this.symbol = symbol;
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
		}

		protected override void IncomingTableRow_convertToDataStructure(XlRowParsed row) {
			double bidVolume	= (double)row["SELL_VOLUME"];
			double price		= (double)row["PRICE"];
			double askVolume	= (double)row["BUY_VOLUME"];
			if (double.IsNaN(bidVolume) == false) {		// where Blank become NaN?
				base.QuikStreaming.StreamingDataSnapshot.LevelTwoBids.Add(price, bidVolume, this, "IncomingRowParsedPush");
			} else {
				base.QuikStreaming.StreamingDataSnapshot.LevelTwoAsks.Add(price, askVolume, this, "IncomingRowParsedPush");
			}
		}
	}
}