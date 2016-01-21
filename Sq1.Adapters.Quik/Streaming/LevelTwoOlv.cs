using System;
using System.Collections.Generic;

using Sq1.Core.Support;
using Sq1.Core.Streaming;
using Sq1.Core.DataTypes;

namespace Sq1.Adapters.Quik.Streaming {
	public class LevelTwoOlv {
		public LevelTwoHalf Asks { get; private set; }
		public LevelTwoHalf Bids { get; private set; }

		public LevelTwoOlv(LevelTwoHalf levelTwoBids, LevelTwoHalf levelTwoAsks) {
			this.Bids = levelTwoBids;
			this.Asks = levelTwoAsks;
		}

		internal List<LevelTwoOlvEachLine> FreezeSortAndFlatten() {
			LevelTwoHalf bidsProxied = this.Bids;
			LevelTwoHalfSortedFrozen bidsFrozen = new LevelTwoHalfSortedFrozen(
				"BIDS_FROZEN",
				bidsProxied.SafeCopy(this, "FREEZING_PROXIED_BIDS_TO_PUSH_TO_DomResizeableUserControl"),
				new LevelTwoHalfSortedFrozen.ASC());

			LevelTwoHalf asksProxied = this.Asks;
			LevelTwoHalfSortedFrozen asksFrozen = new LevelTwoHalfSortedFrozen(
				"ASKS_FROZEN",
				asksProxied.SafeCopy(this, "FREEZING_PROXIED_ASKS_TO_PUSH_TO_DomResizeableUserControl"),
				new LevelTwoHalfSortedFrozen.DESC());

			List<LevelTwoOlvEachLine> ret = new List<LevelTwoOlvEachLine>();

			foreach (KeyValuePair<double, double> keyValue in asksFrozen) {
				LevelTwoOlvEachLine eachAsk = new LevelTwoOlvEachLine();
				eachAsk.BidOrAsk	= BidOrAsk.Ask;
				eachAsk.Ask			= keyValue.Value;
				eachAsk.Price		= keyValue.Key;
				eachAsk.Bid			= double.NaN;
				ret.Add(eachAsk);
			}

			foreach (KeyValuePair<double, double> keyValue in bidsFrozen) {
				LevelTwoOlvEachLine eachBid = new LevelTwoOlvEachLine();
				eachBid.BidOrAsk	= BidOrAsk.Bid;
				eachBid.Ask			= double.NaN;
				eachBid.Price		= keyValue.Key;
				eachBid.Bid			= keyValue.Value;
				ret.Add(eachBid);
			}

			return ret;
		}
	}
}
