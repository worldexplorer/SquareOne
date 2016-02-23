using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public class LevelTwoHalfSortedFrozen : SortedDictionary<double, double> {
		public class  ASC : IComparer<double> { int IComparer<double>.Compare(double x, double y) { return x > y ? 1 : -1; } }
		public class DESC : IComparer<double> { int IComparer<double>.Compare(double x, double y) { return x < y ? 1 : -1; } }

		public	BidOrAsk					BidOrAsk			{ get; private set; }
		public	string						ReasonToExist		{ get; private set; }
		public	bool						ImSortedAscending	{ get; private set; }
		public	bool						ImFrozen			{ get; private set; }
		public	double						PriceMin			{ get; private set; }
		public	double						PriceMax			{ get; private set; }
		public	double						LotMin				{ get; private set; }
		public	double						LotMax				{ get; private set; }
		public	double						LotSum				{ get; private set; }
		public	Dictionary<double, double>	LotsCumulative		{ get; private set; }

		public LevelTwoHalfSortedFrozen(BidOrAsk bidOrAsk, string reasonToExist
					, Dictionary<double, double> level2half_safeClone, IComparer<double> orderby) : base(level2half_safeClone, orderby) {
			if (bidOrAsk != BidOrAsk.Ask && bidOrAsk != BidOrAsk.Bid) {
				throw new Exception("I_NEED_DIRECTION_TO_CALCULATE_LOTS_CUMULATIVE");
			}
			ImFrozen = true;
			BidOrAsk = bidOrAsk;
			ReasonToExist = reasonToExist;
			ImSortedAscending = orderby is LevelTwoHalfSortedFrozen.ASC;
			LotsCumulative = new Dictionary<double, double>();
			if (base.Count == 0) return;
			this.calcProperties_addingRemovingNotSupported();
		}

		void calcProperties_addingRemovingNotSupported() {
			List<KeyValuePair<double, double>> sortedFromSpreadToTheEdgeOfMarket_toCalculateLotsCumulative = new List<KeyValuePair<double, double>>();
			bool iShouldRevert = false;
			if (this.BidOrAsk == BidOrAsk.Ask) {
				// bigger values = edge of maket; smaller values => spread
				if (this.ImSortedAscending) {
					iShouldRevert = false;
				} else {
					iShouldRevert = true;
				}
			} else {
				// bigger values = spread; smaller values => edge of maket
				if (this.ImSortedAscending) {
					iShouldRevert = true;
				} else {
					iShouldRevert = false;
				}
			}

			if (iShouldRevert) {
				foreach (KeyValuePair<double, double> keyValue in this) {
					sortedFromSpreadToTheEdgeOfMarket_toCalculateLotsCumulative.Insert(0, keyValue);
				}
			} else {
				sortedFromSpreadToTheEdgeOfMarket_toCalculateLotsCumulative.AddRange(this);
			}

			double prevLot = 0;
			//foreach (double price in base.Keys) {
			//foreach (KeyValuePair<double, double> keyValue in this) {
			foreach (KeyValuePair<double, double> keyValue in sortedFromSpreadToTheEdgeOfMarket_toCalculateLotsCumulative) {
				double price = keyValue.Key;
				double lot = keyValue.Value;

				if (this.PriceMin == 0) this.PriceMin = price;
				if (this.PriceMax == 0) this.PriceMax = price;

				if (this.PriceMin > price) this.PriceMin = price;
				if (this.PriceMax < price) this.PriceMax = price;

				double thisLot = prevLot + lot;
				this.LotsCumulative.Add(price, thisLot);
				prevLot = thisLot;
				this.LotSum += lot;

				if (this.LotMin == 0) this.LotMin = lot;
				if (this.LotMax == 0) this.LotMax = lot;

				if (this.LotMin > lot) this.LotMin = lot;
				if (this.LotMax < lot) this.LotMax = lot;
			}
		}

		public new void Add(double key, double value) {
			if (this.ImFrozen) throw new Exception("FILL_ME_IN_CTOR()__FROZEN_MEANS_UNABLE_TO_MODIFY_AFTER_CONSTRUCTED__LAZY_TO_RECALCULATE_CUMULATIVES");
			base.Add(key, value);
		}

		public new void Remove(double key) {
			if (this.ImFrozen) throw new Exception("FILL_ME_IN_CTOR()__FROZEN_MEANS_UNABLE_TO_MODIFY_AFTER_CONSTRUCTED__LAZY_TO_RECALCULATE_CUMULATIVES");
			base.Remove(key);
		}

		public new void Clear() {
			if (this.ImFrozen) throw new Exception("FILL_ME_IN_CTOR()__FROZEN_MEANS_UNABLE_TO_MODIFY_AFTER_CONSTRUCTED__LAZY_TO_RECALCULATE_CUMULATIVES");
			base.Clear();
		}

		public override string ToString() {
			return this.ReasonToExist + ":[" + base.Count + "]";
		}

		public LevelTwoHalfSortedFrozen Clone_noDeeperThan(int depthFittingToDisplayedHeight = -1) {
			if (depthFittingToDisplayedHeight == -1) depthFittingToDisplayedHeight = this.Count;
			if (depthFittingToDisplayedHeight > this.Count) {
				string msg = "DONT_REQUEST_ASK/BID_DEPTH_LONGER_THAN_I_HAVE"
					+ " depthFittingToDisplayedHeight[" + depthFittingToDisplayedHeight + "] = this.Count[" + this.Count + "]";
				Assembler.PopupException(msg);
				depthFittingToDisplayedHeight = this.Count;
			}
			Dictionary<double, double> shorterHalf = new Dictionary<double,double>(depthFittingToDisplayedHeight);
			foreach (KeyValuePair<double, double> keyValue in this) {
				shorterHalf.Add(keyValue.Key, keyValue.Value);
				if (shorterHalf.Count >= depthFittingToDisplayedHeight) break;
			}

			LevelTwoHalfSortedFrozen ret = new LevelTwoHalfSortedFrozen(this.BidOrAsk
				, this.ReasonToExist  + " SHALLOW_CLONE[" + depthFittingToDisplayedHeight + "]"
				, shorterHalf, base.Comparer);
			return ret;
		}
	}
}
