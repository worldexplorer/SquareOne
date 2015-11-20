using Sq1.Core;
using Sq1.Core.DataTypes;

using Sq1.Adapters.Quik.Dde.XlDde;
using Sq1.Core.Support;

namespace Sq1.Adapters.Quik.Dde {
	public class DdeTableDepth : XlDdeTable {
		protected override string DdeConsumerClassName { get { return "DdeTableDepth"; } }

		string			symbol;

		ConcurrentDictionaryGeneric<double, double> levelTwoAsks { get { return base.QuikStreaming.StreamingDataSnapshot.LevelTwoAsks; } }
		ConcurrentDictionaryGeneric<double, double> levelTwoBids { get { return base.QuikStreaming.StreamingDataSnapshot.LevelTwoBids; } }

		public DdeTableDepth(string topic, QuikStreaming quikStreaming, string symbol) : base(topic, quikStreaming) {
			this.symbol = symbol;
		}
		protected override void IncomingTableBegun() {
			this.levelTwoAsks.Clear(this, "IncomingTableBegun");
			this.levelTwoBids.Clear(this, "IncomingTableBegun");
			this.levelTwoAsks.WaitAndLockFor(this, "IncomingTableBegun");
			this.levelTwoBids.WaitAndLockFor(this, "IncomingTableBegun");
		}
		protected override void IncomingTableTerminated() {
			this.levelTwoAsks.UnLockFor(this, "IncomingTableTerminated");
			this.levelTwoBids.UnLockFor(this, "IncomingTableTerminated");
			string msg = "send a notify event or Chart is already Wait(indefinitely)'ing to get a FrozenHalfs  and go draw them?...";
		}

		protected override void IncomingRowParsedDelivered(XlRowParsed row) {
			double bidVolume	= (double)row["SELL_VOLUME"];
			double askVolume	= (double)row["BUY_VOLUME"];
			double price		= (double)row["PRICE"];
			if (double.IsNaN(bidVolume) == false) {
				base.QuikStreaming.StreamingDataSnapshot.LevelTwoBids.Add(price, bidVolume, this, "IncomingRowParsedPush");
			} else {
				base.QuikStreaming.StreamingDataSnapshot.LevelTwoAsks.Add(price, bidVolume, this, "IncomingRowParsedPush");
			}
		}
	}
}