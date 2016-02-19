using System;
using System.Collections.Generic;

using Sq1.Core.Support;
using Sq1.Core.Streaming;
using Sq1.Core.DataTypes;
using Sq1.Core;

namespace Sq1.Widgets.Level2 {
	public class LevelTwoOlv {
		SymbolInfo symbolInfo;
		LevelTwoHalf asks;
		LevelTwoHalf bids;

		public LevelTwoOlv(LevelTwoHalf levelTwoBids, LevelTwoHalf levelTwoAsks, SymbolInfo symbolInfoPassed) {
			this.bids = levelTwoBids;
			this.asks = levelTwoAsks;
			//if (symbolInfoPassed == null) symbolInfoPassed = new SymbolInfo();	// just for cleaning DomControl after manual user-dde-stop; nothing is gonna be outputted so I don't care; avoiding NPE
			this.symbolInfo = symbolInfoPassed;
		}

		internal List<LevelTwoOlvEachLine> FrozenSortedFlattened_priceLevelsInserted { get {
			List<LevelTwoOlvEachLine> ret = new List<LevelTwoOlvEachLine>();
			if (this.symbolInfo == null) return ret;	// just for cleaning DomControl after manual user-dde-stop; nothing is gonna be outputted


			Dictionary<double, double> asksSafeCopy_orEmpty = this.asks != null
				? this.asks.SafeCopy(this, "FREEZING_PROXIED_ASKS_TO_PUSH_TO_DomResizeableUserControl")
				: new Dictionary<double, double>();

			LevelTwoHalfSortedFrozen asksFrozen = new LevelTwoHalfSortedFrozen(
				BidOrAsk.Ask, "ASKS_FOR_DomResizeableUserControl",
				asksSafeCopy_orEmpty,
				new LevelTwoHalfSortedFrozen.DESC());


			Dictionary<double, double> bidSafeCopy_orEmpty = this.bids != null
				? this.bids.SafeCopy(this, "FREEZING_PROXIED_BIDS_TO_PUSH_TO_DomResizeableUserControl")
				: new Dictionary<double, double>();

			LevelTwoHalfSortedFrozen bidsFrozen = new LevelTwoHalfSortedFrozen(
				BidOrAsk.Bid, "BIDS_FOR_DomResizeableUserControl",
				bidSafeCopy_orEmpty,
				new LevelTwoHalfSortedFrozen.DESC());

			#if DEBUG
			if (asksFrozen.Count > 1 && bidsFrozen.Count > 1) {
				List<double> asksPriceLevels_ASC = new List<double>(asksFrozen.Keys);
				double askBest_lowest =  asksPriceLevels_ASC[0];
				List<double> bidsPriceLevels_DESC = new List<double>(bidsFrozen.Keys);
				double bidBest_highest =  bidsPriceLevels_DESC[bidsPriceLevels_DESC.Count-1];
				if (askBest_lowest < bidBest_highest) {
					string msg = "YOUR_MOUSTACHES_GOT_REVERTED";
					Assembler.PopupException(msg, null, false);
				}
			}
			#endif

			double priceStep = this.symbolInfo.PriceStep;

			double priceLastAdded = double.NaN;
			int askRowsIncludingAdded = 0;
			foreach (KeyValuePair<double, double> keyValue in asksFrozen) {
				double priceLevel			= keyValue.Key;
				double volumeAsk			= keyValue.Value;
				double volumeAskCumulative	= asksFrozen.LotsCumulative[priceLevel];

				LevelTwoOlvEachLine eachAsk = new LevelTwoOlvEachLine(BidOrAsk.Ask, priceLevel);
				eachAsk.SetAskVolumes(volumeAsk, volumeAskCumulative);

				if (double.IsNaN(priceLastAdded) == false && this.symbolInfo.Level2AskShowHoles) {
					// SORTED_DESCENDING_BOTH_BIDS_AND_ASKS 1 = (140723 - 140722 ) / 1
					double distance = priceLastAdded - priceLevel;
					distance -= this.symbolInfo.PriceStep;
					int priceLevelsMissing = (int)Math.Floor(distance / (double) priceStep);
					double priceLevelToCoverTheDistance = priceLastAdded - priceStep;
					for (int i = 0; i < priceLevelsMissing; i++) {
						LevelTwoOlvEachLine eachAskEmpty = new LevelTwoOlvEachLine(BidOrAsk.Ask, priceLevelToCoverTheDistance);
						ret.Add(eachAskEmpty);
						askRowsIncludingAdded++;
						priceLevelToCoverTheDistance += priceStep;
					}
				}

				priceLastAdded = priceLevel;
				ret.Add(eachAsk);
				askRowsIncludingAdded++;
			}

			int howManyAsksToInsertArtificially = this.symbolInfo.Level2PriceLevels - askRowsIncludingAdded;
			double highestAsk = asksFrozen.PriceMax;
			for (int i = 0; i < howManyAsksToInsertArtificially; i++) {
				highestAsk += this.symbolInfo.PriceStep;
				LevelTwoOlvEachLine askArtificial = new LevelTwoOlvEachLine(BidOrAsk.Ask, highestAsk, false);
				ret.Insert(0, askArtificial);
			}

			if (double.IsNaN(priceLastAdded) == false && this.symbolInfo.Level2ShowSpread) {
				List<double> priceLevelsBids = new List<double>(bidsFrozen.Keys);
				if (priceLevelsBids.Count > 0) {
					double firstBid = priceLevelsBids[0];
					double spread = priceLastAdded - firstBid;
					if (spread > this.symbolInfo.PriceStep) {
						LevelTwoOlvEachLine spreadRow = new LevelTwoOlvEachLine(BidOrAsk.UNKNOWN, spread);
						ret.Add(spreadRow);
					}
				}
			}

			priceLastAdded = double.NaN;
			int bidRowsIncludingAdded = 0;
			foreach (KeyValuePair<double, double> keyValue in bidsFrozen) {
				double priceLevel			= keyValue.Key;
				double volumeBid			= keyValue.Value;
				double volumeBidCumulative	= bidsFrozen.LotsCumulative[priceLevel];

				LevelTwoOlvEachLine eachBid = new LevelTwoOlvEachLine(BidOrAsk.Bid, priceLevel);
				eachBid.SetBidVolumes(volumeBid, volumeBidCumulative);

				if (double.IsNaN(priceLastAdded) == false && this.symbolInfo.Level2BidShowHoles) {
					double distance = priceLastAdded - priceLevel;
					distance -= this.symbolInfo.PriceStep;
					int priceLevelsMissing = (int)Math.Floor(distance / (double) priceStep);
					double priceLevelToCoverTheDistance = priceLastAdded + priceStep;
					for (int i = 0; i < priceLevelsMissing; i++) {
						LevelTwoOlvEachLine eachBidEmpty = new LevelTwoOlvEachLine(BidOrAsk.Bid, priceLevelToCoverTheDistance);
						ret.Add(eachBidEmpty);
						bidRowsIncludingAdded++;
						priceLevelToCoverTheDistance += priceStep;
					}
				}

				priceLastAdded = priceLevel;
				ret.Add(eachBid);
				bidRowsIncludingAdded++;
			}

			int howManyBidsToAddArtificially = this.symbolInfo.Level2PriceLevels - bidRowsIncludingAdded;
			double lowestBid = bidsFrozen.PriceMin;
			for (int i = 0; i < howManyAsksToInsertArtificially; i++) {
				lowestBid -= this.symbolInfo.PriceStep;
				LevelTwoOlvEachLine bidArtificial = new LevelTwoOlvEachLine(BidOrAsk.Bid, lowestBid, false);
				ret.Add(bidArtificial);
			}

			return ret;
		} }
	}
}
