using System;
using System.Collections.Generic;

namespace Sq1.Core.DataTypes {
	public class LevelTwoFrozen {
		public	string						ReasonToExist		{ get; private set; }
		public	LevelTwoHalfSortedFrozen	Asks_sortedAsc		{ get; private set; }
		public	LevelTwoHalfSortedFrozen	Bids_sortedDesc		{ get; private set; }

				List<double>	ask_priceLevels_ASC_cached;
		public	List<double>	Ask_priceLevels_ASC		{ get {
			if (this.Asks_sortedAsc.ImFrozen == false) this.ask_priceLevels_ASC_cached = null;
			if (this.ask_priceLevels_ASC_cached == null) this.ask_priceLevels_ASC_cached = new List<double>(this.Asks_sortedAsc.Keys);
			return this.ask_priceLevels_ASC_cached;
		} }

				List<double>	bid_priceLevels_DESC_cached;
		public	List<double>	Bid_priceLevels_DESC	{ get {
			if (this.Asks_sortedAsc.ImFrozen == false) this.bid_priceLevels_DESC_cached = null;
			if (this.bid_priceLevels_DESC_cached == null) this.bid_priceLevels_DESC_cached = new List<double>(this.Bids_sortedDesc.Keys);
			return this.bid_priceLevels_DESC_cached;
		} }

		public	double	AskBest_lowest { get {
				double ret = double.NaN;
				if (this.Asks_sortedAsc.Count == 0) return ret;
				ret =  this.Ask_priceLevels_ASC[0];
				return ret;
			} }
		public	double	BidBest_highest { get {
				double ret = double.NaN;
				if (this.Bids_sortedDesc.Count == 0) return ret;
				ret =  this.Bid_priceLevels_DESC[this.Bid_priceLevels_DESC.Count-1];
				return ret;
			} }

		public bool MarketIsDead_reliablyKozSorted { get {
			if (double.IsNaN(this.AskBest_lowest))	return false;
			if (double.IsNaN(this.BidBest_highest))	return false;
			return this.AskBest_lowest <= this.BidBest_highest;
		} }

		public LevelTwoFrozen(LevelTwo levelTwo_toFreeze, string whoFrozeMe, string recipient) {
			ReasonToExist = whoFrozeMe;

			levelTwo_toFreeze.WaitAndLockFor(this, whoFrozeMe);

			Dictionary<double, double> asks_safeCopy = levelTwo_toFreeze.Asks_safeCopy(this, whoFrozeMe);

			this.Asks_sortedAsc = new LevelTwoHalfSortedFrozen(
				BidOrAsk.Ask, "asks_FOR_" + recipient,
				asks_safeCopy,
				new LevelTwoHalfSortedFrozen.ASC());

			Dictionary<double, double> bids_safeCopy = levelTwo_toFreeze.Bids_safeCopy(this, whoFrozeMe);

			this.Bids_sortedDesc = new LevelTwoHalfSortedFrozen(
				BidOrAsk.Bid, "bids_FOR_" + recipient,
				bids_safeCopy,
				new LevelTwoHalfSortedFrozen.DESC());

			//string msg1 = "CHECK_THE_LOCK_QUEUE__THAT_YOU_GOT_TWO_INVOKERS_NON_RECURSIVE";
			//TWO_IDENTICAL_IN_RECURSION_STACK_BUT_THEY_GOT_UNTANGLED_NOPROB Assembler.PopupException(msg1);

			levelTwo_toFreeze.UnLockFor(this, whoFrozeMe);

			#if DEBUG
			if (this.MarketIsDead_reliablyKozSorted) {
				string msg = "YOUR_MOUSTACHES_GOT_REVERTED_OR MarketIsDead_reliablyKozSorted";
				Assembler.PopupException(msg, null, false);
			}
			#endif

		}
	}
}
