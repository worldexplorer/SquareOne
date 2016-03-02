using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public class LevelTwo {
								string			symbol;
		[JsonIgnore]	public	Quote			LastQuote_unbound_notCloned;
		[JsonIgnore]	public	LevelTwoHalf	Asks;
		[JsonIgnore]	public	LevelTwoHalf	Bids;

		public LevelTwo(string symbolPassed) {
			this.symbol = symbolPassed;
			this.Asks = new LevelTwoHalf("LevelTwoAsks[" + this.symbol + "]");
			this.Bids = new LevelTwoHalf("LevelTwoBids[" + this.symbol + "]");
		}

		internal Quote Clear() {
			Quote ret = this.LastQuote_unbound_notCloned;
			this.LastQuote_unbound_notCloned = null;
			this.Asks.Clear(this, "livesimEnded");
			this.Bids.Clear(this, "livesimEnded");
			return ret;
		}

		#region merge from Widgets.Level2

								SymbolInfo		symbolInfo;

		public LevelTwo(LevelTwoHalf levelTwoBids, LevelTwoHalf levelTwoAsks, SymbolInfo symbolInfoPassed) {
			this.Bids = levelTwoBids;
			this.Asks = levelTwoAsks;
			//if (symbolInfoPassed == null) symbolInfoPassed = new SymbolInfo();	// just for cleaning DomControl after manual user-dde-stop; nothing is gonna be outputted so I don't care; avoiding NPE
			this.symbolInfo = symbolInfoPassed;
		}

		public List<LevelTwoEachLine> FrozenSortedFlattened_priceLevelsInserted { get {
			List<LevelTwoEachLine> ret = new List<LevelTwoEachLine>();
			if (this.symbolInfo == null) return ret;	// just for cleaning DomControl after manual user-dde-stop; nothing is gonna be outputted


			Dictionary<double, double> asksSafeCopy_orEmpty = this.Asks != null
				? this.Asks.SafeCopy(this, "FREEZING_PROXIED_asks_TO_PUSH_TO_DomResizeableUserControl")
				: new Dictionary<double, double>();

			LevelTwoHalfSortedFrozen asksFrozen_asc = new LevelTwoHalfSortedFrozen(
				BidOrAsk.Ask, "asks_FOR_DomResizeableUserControl",
				asksSafeCopy_orEmpty,
				new LevelTwoHalfSortedFrozen.ASC());


			Dictionary<double, double> bidsafeCopy_orEmpty = this.Bids != null
				? this.Bids.SafeCopy(this, "FREEZING_PROXIED_bids_TO_PUSH_TO_DomResizeableUserControl")
				: new Dictionary<double, double>();

			LevelTwoHalfSortedFrozen bidsFrozen_desc = new LevelTwoHalfSortedFrozen(
				BidOrAsk.Bid, "bids_FOR_DomResizeableUserControl",
				bidsafeCopy_orEmpty,
				new LevelTwoHalfSortedFrozen.DESC());

			#if DEBUG
			if (asksFrozen_asc.Count > 1 && bidsFrozen_desc.Count > 1) {
				List<double> AsksPriceLevels_ASC = new List<double>(asksFrozen_asc.Keys);
				double askBest_lowest =  AsksPriceLevels_ASC[0];
				List<double> BidsPriceLevels_DESC = new List<double>(bidsFrozen_desc.Keys);
				double bidBest_highest =  BidsPriceLevels_DESC[BidsPriceLevels_DESC.Count-1];
				if (askBest_lowest < bidBest_highest) {
					string msg = "YOUR_MOUSTACHES_GOT_REVERTED";
					Assembler.PopupException(msg, null, false);
				}
			}
			#endif

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
			foreach (KeyValuePair<double, double> keyValue in asksFrozen_asc) {
				double priceLevel			= keyValue.Key;
				double volumeAsk			= keyValue.Value;
				double volumeAskCumulative	= asksFrozen_asc.LotsCumulative[priceLevel];

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
			double highestAsk = asksFrozen_asc.PriceMax;
			for (int i = 0; i < howManyAsksToInsertArtificially; i++) {
				highestAsk += this.symbolInfo.PriceStep;
				LevelTwoEachLine askArtificial = new LevelTwoEachLine(BidOrAsk.Ask, highestAsk, false);
				ret.Insert(0, askArtificial);
			}

			if (this.symbolInfo.Level2ShowSpread) {
				double spread = double.NaN;
				if (double.IsNaN(priceLastAdded) == false) {
					List<double> priceLevelsAsks = new List<double>(asksFrozen_asc.Keys);
					List<double> priceLevelsBids = new List<double>(bidsFrozen_desc.Keys);
					if (priceLevelsAsks.Count > 0 && priceLevelsBids.Count > 0) {
						double bestAsk = priceLevelsAsks[0];
						double bestBid = priceLevelsBids[priceLevelsBids.Count - 1];
						spread = bestAsk - bestBid;
					}
				}
				LevelTwoEachLine spreadRow = new LevelTwoEachLine(BidOrAsk.UNKNOWN, spread);
				ret.Add(spreadRow);
			}

			priceLastAdded = double.NaN;
			int bidRowsIncludingAdded = 0;
			foreach (KeyValuePair<double, double> keyValue in bidsFrozen_desc) {
				double priceLevel			= keyValue.Key;
				double volumeBid			= keyValue.Value;
				double volumeBidCumulative	= bidsFrozen_desc.LotsCumulative[priceLevel];

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
			double lowestBid = bidsFrozen_desc.PriceMin;
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
