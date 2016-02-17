using System;
using System.Collections.Generic;

using Sq1.Core.Support;
using Sq1.Core.Streaming;
using Sq1.Core.DataTypes;

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
			LevelTwoHalfSortedFrozen asksFrozen = new LevelTwoHalfSortedFrozen(
				BidOrAsk.Ask, "ASKS_FOR_DomResizeableUserControl",
				this.asks.SafeCopy(this, "FREEZING_PROXIED_ASKS_TO_PUSH_TO_DomResizeableUserControl"),
				new LevelTwoHalfSortedFrozen.DESC());

			LevelTwoHalfSortedFrozen bidsFrozen = new LevelTwoHalfSortedFrozen(
				BidOrAsk.Bid, "BIDS_FOR_DomResizeableUserControl",
				this.bids.SafeCopy(this, "FREEZING_PROXIED_BIDS_TO_PUSH_TO_DomResizeableUserControl"),
				new LevelTwoHalfSortedFrozen.DESC());

			List<LevelTwoOlvEachLine> ret = new List<LevelTwoOlvEachLine>();
			if (this.symbolInfo == null && asksFrozen.Count == 0 && bidsFrozen.Count == 0) return ret;	// just for cleaning DomControl after manual user-dde-stop; nothing is gonna be outputted


			double priceStep = this.symbolInfo.PriceStepFromDecimal;

			double priceLastAdded = double.NaN;
			int askRowsIncludingAdded = 0;
			foreach (KeyValuePair<double, double> keyValue in asksFrozen) {
				double priceLevel			= keyValue.Key;
				double volumeAsk			= keyValue.Value;
				double volumeAskCumulative	= asksFrozen.LotsCumulative[priceLevel];

				LevelTwoOlvEachLine eachAsk = new LevelTwoOlvEachLine(BidOrAsk.Ask, priceLevel);
				eachAsk.SetAskVolumes(volumeAsk, volumeAskCumulative);

				if (double.IsNaN(priceLastAdded) == false && this.symbolInfo.Level2AskFillHoles) {
					// SORTED_DESCENDING_BOTH_BIDS_AND_ASKS 1 = (140723 - 140722 ) / 1
					double distance = priceLastAdded - priceLevel;
					distance -= this.symbolInfo.PriceStepFromDecimal;
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
				highestAsk += this.symbolInfo.PriceStepFromDecimal;
				LevelTwoOlvEachLine askArtificial = new LevelTwoOlvEachLine(BidOrAsk.Ask, highestAsk, false);
				ret.Insert(0, askArtificial);
			}

			if (double.IsNaN(priceLastAdded) == false && this.symbolInfo.Level2ShowSpread) {
				List<double> priceLevelsBids = new List<double>(bidsFrozen.Keys);
				if (priceLevelsBids.Count > 0) {
					double firstBid = priceLevelsBids[0];
					double spread = priceLastAdded - firstBid;
					if (spread > this.symbolInfo.PriceStepFromDecimal) {
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

				if (double.IsNaN(priceLastAdded) == false && this.symbolInfo.Level2BidFillHoles) {
					double distance = priceLastAdded - priceLevel;
					distance -= this.symbolInfo.PriceStepFromDecimal;
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
				lowestBid -= this.symbolInfo.PriceStepFromDecimal;
				LevelTwoOlvEachLine bidArtificial = new LevelTwoOlvEachLine(BidOrAsk.Bid, lowestBid, false);
				ret.Add(bidArtificial);
			}

			return ret;
		} }
	}
}
