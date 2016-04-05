using System;
using System.Collections.Generic;

namespace Sq1.Core.DataTypes {
	public class LevelTwoFrozen {
		public	string						ReasonToExist		{ get; private set; }
		public	LevelTwoHalfSortedFrozen	Asks_sortedAsc		{ get; private set; }
		public	LevelTwoHalfSortedFrozen	Bids_sortedDesc		{ get; private set; }

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
			if (this.Asks_sortedAsc.Count > 0 && this.Bids_sortedDesc.Count > 0) {
				List<double> ask_priceLevels_ASC = new List<double>(this.Asks_sortedAsc.Keys);
				double askBest_lowest =  ask_priceLevels_ASC[0];
				List<double> bid_priceLevels_DESC = new List<double>(this.Bids_sortedDesc.Keys);
				double bidBest_highest =  bid_priceLevels_DESC[bid_priceLevels_DESC.Count-1];
				if (askBest_lowest < bidBest_highest) {
					string msg = "YOUR_MOUSTACHES_GOT_REVERTED";
					Assembler.PopupException(msg, null, false);
				}
			}
			#endif

		}
	}
}
