using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Sq1.Core.DataTypes;
using Sq1.Core.Support;

namespace Sq1.Core.Streaming {
	public class LevelTwo : ConcurrentWatchdog {
								string			symbol;
		[JsonIgnore]	public	Quote			QuoteCurrent_unbound_notCloned_validAbsno_invalidIntrabarSerno;
		[JsonIgnore]	public	Quote			QuotePrev_unbound_notCloned;
		[JsonIgnore]	private	LevelTwoHalf	asks;		//	{ get; private set; }
		[JsonIgnore]	private	LevelTwoHalf	bids;		//	{ get; private set; }

		// forced by ConcurrentWatchdog
		//LevelTwo() {}

		public LevelTwo(string symbolPassed, string reasonToExist) : base(reasonToExist) {
			this.symbol = symbolPassed;
			this.asks = new LevelTwoHalf("LevelTwoAsks[" + this.symbol + "] " + reasonToExist);
			this.bids = new LevelTwoHalf("LevelTwoBids[" + this.symbol + "] " + reasonToExist);
		}

		public Quote Clear_QuoteLastPrev() {
			Quote ret = this.QuoteCurrent_unbound_notCloned_validAbsno_invalidIntrabarSerno;
			this.QuoteCurrent_unbound_notCloned_validAbsno_invalidIntrabarSerno = null;
			this.QuotePrev_unbound_notCloned = null;
			return ret;
		}

		#region the price I pay to keep LevelTwo atomic: GUI and other threads will wait until DdeTableDepth.IncomingTableTerminated() will invoke LevelTwo.Unlock()
		public void Clear_LevelTwo(object lockOwner, string lockPurpose) {
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose);
				this.asks.Clear(lockOwner, lockPurpose);
				this.bids.Clear(lockOwner, lockPurpose);
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}

		public void AddBid(double price, double bidVolume, object lockOwner, string lockPurpose) {
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose);
				this.bids.Add(price, bidVolume, lockOwner, lockPurpose);
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}

		public void AddAsk(double price, double askVolume, object lockOwner, string lockPurpose) {
			try {
				base.WaitAndLockFor(lockOwner, lockPurpose);
				this.asks.Add(price, askVolume, lockOwner, lockPurpose);
			} finally {
				base.UnLockFor(lockOwner, lockPurpose);
			}
		}

		public Dictionary<double, double> Asks_safeCopy(object lockOwner, string lockPurpose) {
			return this.asks.SafeCopy(lockOwner, lockPurpose);
		}

		public Dictionary<double, double> Bids_safeCopy(object lockOwner, string lockPurpose) {
			return this.asks.SafeCopy(lockOwner, lockPurpose);
		}
		#endregion

		public override string ToString() {
			return this.symbol + " " + base.ReasonToExist;
		}

		#region merge from Widgets.Level2

								SymbolInfo		symbolInfo;

		public LevelTwo(LevelTwoHalf levelTwoBids, LevelTwoHalf levelTwoAsks, SymbolInfo symbolInfoPassed, string reasonToExist) : base(reasonToExist) {
			if (levelTwoBids == null || levelTwoAsks == null || symbolInfoPassed == null) {
				string msg = ""
					+ " ref:DdeTableDepth_MANUALLY_RAISED_EVENT_WITH_EMPTY_LIST_TO_CLEAR_EVERYTHING_IN_DDE_MONITOR_(QUOTES/LEVEL2/TRADES)_RIGHT_AFTER_USER_STOPPED_DDE_FEED";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			this.bids = levelTwoBids;
			this.asks = levelTwoAsks;
			//if (symbolInfoPassed == null) symbolInfoPassed = new SymbolInfo();	// just for cleaning DomControl after manual user-dde-stop; nothing is gonna be outputted so I don't care; avoiding NPE
			this.symbolInfo = symbolInfoPassed;
		}

		public List<LevelTwoEachLine> FrozenSortedFlattened_priceLevelsInserted_forOlv { get {
			List<LevelTwoEachLine> ret = new List<LevelTwoEachLine>();
			if (this.symbolInfo == null) return ret;	// just for cleaning DomControl after manual user-dde-stop; nothing is gonna be outputted

			#region moved to LevelTwoFrozen()
			//Dictionary<double, double> asksSafeCopy_orEmpty = this.Asks != null
			//    ? this.Asks.SafeCopy(this, "FREEZING_PROXIED_asks_TO_PUSH_TO_LevelTwoUserControl_titledResizeableUserControl")
			//    : new Dictionary<double, double>();

			//LevelTwoHalfSortedFrozen AsksFrozen_sortedAsc = new LevelTwoHalfSortedFrozen(
			//    BidOrAsk.Ask, "asks_FOR_LevelTwoUserControl_titledResizeableUserControl",
			//    asksSafeCopy_orEmpty,
			//    new LevelTwoHalfSortedFrozen.ASC());


			//Dictionary<double, double> bidsafeCopy_orEmpty = this.Bids != null
			//    ? this.Bids.SafeCopy(this, "FREEZING_PROXIED_bids_TO_PUSH_TO_LevelTwoUserControl_titledResizeableUserControl")
			//    : new Dictionary<double, double>();

			//LevelTwoHalfSortedFrozen bidsFrozen_desc = new LevelTwoHalfSortedFrozen(
			//    BidOrAsk.Bid, "bids_FOR_LevelTwoUserControl_titledResizeableUserControl",
			//    bidsafeCopy_orEmpty,
			//    new LevelTwoHalfSortedFrozen.DESC());

			//#if DEBUG
			//if (AsksFrozen_sortedAsc.Count > 0 && bidsFrozen_desc.Count > 0) {
			//    List<double> ask_priceLevels_ASC = new List<double>(AsksFrozen_sortedAsc.Keys);
			//    double askBest_lowest =  ask_priceLevels_ASC[0];
			//    List<double> bid_priceLevels_DESC = new List<double>(bidsFrozen_desc.Keys);
			//    double bidBest_highest =  bid_priceLevels_DESC[bid_priceLevels_DESC.Count-1];
			//    if (askBest_lowest < bidBest_highest) {
			//        string msg = "YOUR_MOUSTACHES_GOT_REVERTED";
			//        Assembler.PopupException(msg, null, false);
			//    }
			//}
			//#endif
			#endregion

			LevelTwoFrozen levelTwoFrozen = new LevelTwoFrozen(this,
				"FREEZING_PROXIED_bids/asks_TO_PUSH_TO_LevelTwoUserControl_titledResizeableUserControl",
				"LevelTwoUserControl_titledResizeableUserControl");

			
			//v1
			//if (this.symbolInfo.Level2PriceLevels < bidsFrozen.Count) {
			//    bidsFrozen = bidsFrozen.Clone_noDeeperThan(this.symbolInfo.Level2PriceLevels);
			//}
			//if (this.symbolInfo.Level2PriceLevels < asksFrozen.Count) {
			//    asksFrozen = asksFrozen.Clone_noDeeperThan(this.symbolInfo.Level2PriceLevels, true);
			//}
			//v2 don't cut anything but just display from spread up and down

			double priceStep = this.symbolInfo.PriceStep;

			double priceLastAdded = double.NaN;
			int askRowsIncludingAdded = 0;
			foreach (KeyValuePair<double, double> keyValue in levelTwoFrozen.Asks_sortedAsc) {
				double priceLevel			= keyValue.Key;
				double volumeAsk			= keyValue.Value;
				double volumeAskCumulative	= levelTwoFrozen.Asks_sortedAsc.LotsCumulative[priceLevel];

				LevelTwoEachLine eachAsk = new LevelTwoEachLine(BidOrAsk.Ask, priceLevel);
				eachAsk.SetAskVolumes(volumeAsk, volumeAskCumulative);

				if (double.IsNaN(priceLastAdded) == false && this.symbolInfo.Level2AskShowHoles) {
					// SORTED_DESCENDING_BOTH_Bids_AND_Asks 1 = (140723 - 140722 ) / 1
					double distance = priceLastAdded - priceLevel;
					distance -= this.symbolInfo.PriceStep;
					int priceLevelsMissing = (int)Math.Floor(distance / (double) priceStep);
					double priceLevelToCoverTheDistance = priceLastAdded - priceStep;
					for (int i = 0; i < priceLevelsMissing; i++) {
						LevelTwoEachLine eachAskEmpty = new LevelTwoEachLine(BidOrAsk.Ask, priceLevelToCoverTheDistance);
						ret.Insert(0, eachAskEmpty);
						askRowsIncludingAdded++;
						priceLevelToCoverTheDistance += priceStep;
						if (askRowsIncludingAdded >= this.symbolInfo.Level2PriceLevels) break;		//v2 don't cut anything but just display from spread up and down
					}
					if (askRowsIncludingAdded >= this.symbolInfo.Level2PriceLevels) break;			//v2 don't cut anything but just display from spread up and down
				}

				priceLastAdded = priceLevel;
				ret.Insert(0, eachAsk);
				askRowsIncludingAdded++;
				if (askRowsIncludingAdded >= this.symbolInfo.Level2PriceLevels) break;			
				//v2 don't cut anything but just display from spread up and down
			}

			int howManyAsksToInsertArtificially = this.symbolInfo.Level2PriceLevels - askRowsIncludingAdded;
			double highestAsk = levelTwoFrozen.Asks_sortedAsc.PriceMax;
			for (int i = 0; i < howManyAsksToInsertArtificially; i++) {
				highestAsk += this.symbolInfo.PriceStep;
				LevelTwoEachLine askArtificial = new LevelTwoEachLine(BidOrAsk.Ask, highestAsk, false);
				ret.Insert(0, askArtificial);
			}

			if (this.symbolInfo.Level2ShowSpread) {
				double spread = double.NaN;
				if (double.IsNaN(priceLastAdded) == false) {
					List<double> priceLevelsAsks = new List<double>(levelTwoFrozen.Asks_sortedAsc.Keys);
					List<double> priceLevelsBids = new List<double>(levelTwoFrozen.Bids_sortedDesc.Keys);
					if (priceLevelsAsks.Count > 0 && priceLevelsBids.Count > 0) {
						double bestAsk = priceLevelsAsks[0];
						double bestBid = priceLevelsBids[0];
						spread = bestAsk - bestBid;
					}
				}
				LevelTwoEachLine rowSpread = new LevelTwoEachLine(BidOrAsk.UNKNOWN, spread);
				ret.Add(rowSpread);
			}

			priceLastAdded = double.NaN;
			int bidRowsIncludingAdded = 0;
			foreach (KeyValuePair<double, double> keyValue in levelTwoFrozen.Bids_sortedDesc) {
				double priceLevel			= keyValue.Key;
				double volumeBid			= keyValue.Value;
				double volumeBidCumulative	= levelTwoFrozen.Bids_sortedDesc.LotsCumulative[priceLevel];

				LevelTwoEachLine eachBid = new LevelTwoEachLine(BidOrAsk.Bid, priceLevel);
				eachBid.SetBidVolumes(volumeBid, volumeBidCumulative);

				if (double.IsNaN(priceLastAdded) == false && this.symbolInfo.Level2BidShowHoles) {
					double distance = priceLastAdded - priceLevel;
					distance -= this.symbolInfo.PriceStep;
					int priceLevelsMissing = (int)Math.Floor(distance / (double) priceStep);
					double priceLevelToCoverTheDistance = priceLastAdded + priceStep;
					for (int i = 0; i < priceLevelsMissing; i++) {
						LevelTwoEachLine eachBidEmpty = new LevelTwoEachLine(BidOrAsk.Bid, priceLevelToCoverTheDistance);
						ret.Add(eachBidEmpty);
						bidRowsIncludingAdded++;
						priceLevelToCoverTheDistance += priceStep;
						if (bidRowsIncludingAdded >= this.symbolInfo.Level2PriceLevels) break;		//v2 don't cut anything but just display from spread up and down
					}
					if (bidRowsIncludingAdded >= this.symbolInfo.Level2PriceLevels) break;			//v2 don't cut anything but just display from spread up and down
				}

				priceLastAdded = priceLevel;
				ret.Add(eachBid);
				bidRowsIncludingAdded++;
				if (bidRowsIncludingAdded >= this.symbolInfo.Level2PriceLevels) break;				//v2 don't cut anything but just display from spread up and down
			}

			int howManyBidsToAddArtificially = this.symbolInfo.Level2PriceLevels - bidRowsIncludingAdded;
			double lowestBid = levelTwoFrozen.Bids_sortedDesc.PriceMin;
			for (int i = 0; i < howManyAsksToInsertArtificially; i++) {
				lowestBid -= this.symbolInfo.PriceStep;
				LevelTwoEachLine bidArtificial = new LevelTwoEachLine(BidOrAsk.Bid, lowestBid, false);
				ret.Add(bidArtificial);
			}

			return ret;
		} }
		#endregion
	}
}
