using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;

namespace Sq1.Adapters.Quik.Streaming {
	class LevelTwoOlvEachLine {
		public	BidOrAsk	BidOrAsk		{ get; private set; }
		public	double		PriceLevel		{ get; private set; }
		public	bool		Colorify		{ get; private set; }

		public	double		AskCumulative	{ get; private set; }
		public	double		AskVolume		{ get; private set; }

		public	double		BidVolume		{ get; private set; }
		public	double		BidCumulative	{ get; private set; }

		LevelTwoOlvEachLine() {
			BidOrAsk		= BidOrAsk.UNKNOWN;		// which means "spread" and should be used only once per whole list published into OLV
			AskCumulative	= double.NaN;
			AskVolume		= double.NaN;
			BidVolume		= double.NaN;
			BidCumulative	= double.NaN;
		}

		internal LevelTwoOlvEachLine(BidOrAsk bidOrAsk, double priceLevelToCoverTheDistance, bool colorify = true)  : this() {
			this.BidOrAsk = bidOrAsk;
			this.PriceLevel = priceLevelToCoverTheDistance;
			this.Colorify = colorify;
		}

		internal void SetBidVolumes(double volumeBid, double volumeBidCumulative) {
			if (this.BidOrAsk != BidOrAsk.Bid) {
				throw new Exception("DONT_ERASE_NAN_BID_VOLUMES_FOR_ASK_OR_SPREAD");
			}
			this.BidVolume		= volumeBid;
			this.BidCumulative	= volumeBidCumulative;
		}

		internal void SetAskVolumes(double volumeAsk, double volumeAskCumulative) {
			if (this.BidOrAsk != BidOrAsk.Ask) {
				throw new Exception("DONT_ERASE_NAN_ASK_VOLUMES_FOR_BID_OR_SPREAD");
			}
			this.AskCumulative	= volumeAskCumulative;
			this.AskVolume		= volumeAsk;
		}
	}
}
