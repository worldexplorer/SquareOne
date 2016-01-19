using System;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core.Support;

using Sq1.Adapters.Quik.Streaming.Dde;

namespace Sq1.Adapters.Quik.Streaming.Monitor {
	public partial class QuikStreamingMonitorDomUserControl {
		void olvDomCustomize() {
			this.olvAsk.AspectGetter = delegate(object o) {
				LevelTwoOlvEachLine askPriceBid = o as LevelTwoOlvEachLine;
				if (askPriceBid == null) return "olvAsk.AspectGetter: askPriceBid=null";
				if (double.IsNaN(askPriceBid.Ask)) return null;
				string formatVolume = this.tableLevel2.FormatVolume;
				return string.Format(formatVolume, askPriceBid.Ask);
			};
			this.olvPrice.AspectGetter = delegate(object o) {
				LevelTwoOlvEachLine askPriceBid = o as LevelTwoOlvEachLine;
				if (askPriceBid == null) return "olvPrice.AspectGetter: askPriceBid=null";
				string formatPrice = this.tableLevel2.FormatPrice;
				return string.Format(formatPrice, askPriceBid.Price);
			};
			this.olvBid.AspectGetter = delegate(object o) {
				LevelTwoOlvEachLine askPriceBid = o as LevelTwoOlvEachLine;
				if (askPriceBid == null) return "olvBid.AspectGetter: askPriceBid=null";
				if (double.IsNaN(askPriceBid.Bid)) return null;
				string formatVolume = this.tableLevel2.FormatVolume;
				return string.Format(formatVolume, askPriceBid.Bid);
			};
		}
	}
}
