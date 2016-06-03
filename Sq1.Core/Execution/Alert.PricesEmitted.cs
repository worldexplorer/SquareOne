using System.Collections.Generic;

using Newtonsoft.Json;

namespace Sq1.Core.Execution {
	public partial class Alert  {
		[JsonProperty]	public	List<Order>							OrdersFollowed_killedAndReplaced	{ get; private set; }
		[JsonProperty]	public	SortedDictionary<int, List<double>> PricesEmitted_byBarIndex			{ get; private set; }

		internal void SetNewPriceEmitted_fromReplacementOrder(Order replacementOrder) {
			this.OrdersFollowed_killedAndReplaced.Add(this.OrderFollowed);
			this.OrderFollowed = replacementOrder;

			double replacementOrder_PriceRequested = replacementOrder.PriceEmitted;

			this.PriceEmitted = replacementOrder_PriceRequested;

			// POSITION_DOESNT_EXIST_UNTIL_ENTRY_ALERT_GOT_FILL
			//if (this.PositionAffected == null) {
			//    string msg = "POSITION_AFFECTED_MUST_NOT_BE_NULL WHEN_ORDER_IS_REPLACED_INSTEAD_OF_REJECTED alert[" + this.ToString() + "]";
			//    Assembler.PopupException(msg);
			//    return;
			//}
			//if (this.IsEntryAlert) {
			//    this.PositionAffected.EntryEmitted_price = this.PriceEmitted;
			//} else {
			//    this.PositionAffected.ExitEmitted_price = this.PriceEmitted;
			//}

			int howManyAdded = this.fillPriceEmitted_fromLastChangeTillBar(this.Bars.Count - 2);

			int barIndex_current = this.Bars.Count - 1;
			if (this.PricesEmitted_byBarIndex.ContainsKey(barIndex_current) == false) {
				this.PricesEmitted_byBarIndex.Add(barIndex_current, new List<double>());
			}
			List<double> pricesForSameBar = this.PricesEmitted_byBarIndex[barIndex_current];
			pricesForSameBar.Add(this.PriceEmitted);
		}
		public List<double> GetEmittedPrice_forBarIndex(int barIndex_beingPainted) {
			List<double> ret = new List<double>();
			ret.Add(this.PriceEmitted);

			if (this.PricesEmitted_byBarIndex.Count == 0) return ret;

			List<int> barIndexes = new List<int>(this.PricesEmitted_byBarIndex.Keys);
			if (this.FilledBarIndex != -1) {
				if (barIndexes.Contains(this.FilledBarIndex) == false) {
					int howManyAdded = this.fillPriceEmitted_fromLastChangeTillBar(this.FilledBarIndex);
					barIndexes = new List<int>(this.PricesEmitted_byBarIndex.Keys);
				}
			} else {
				int howManyAdded = this.fillPriceEmitted_fromLastChangeTillBar();
				barIndexes = new List<int>(this.PricesEmitted_byBarIndex.Keys);
			}

			if (barIndexes.Contains(barIndex_beingPainted) == false) {
				string msg = "PANEL_PRICE_SHOULD_NOT_ASK_ME_ABOUT_Alert.PriceEmitted_FOR_BAR_WHERE_ALERT_DIDNT_EXIST";
				Assembler.PopupException(msg);
				return ret;
			}

			ret = this.PricesEmitted_byBarIndex[barIndex_beingPainted];
			return ret;
		}
		int fillPriceEmitted_fromLastChangeTillBar(int barTill_streamingOrBarFilled = -1) {
			int howManyAdded = 0;
			if (this.Bars == null) return howManyAdded;

			List<int> barIndexes = new List<int>(this.PricesEmitted_byBarIndex.Keys);
			if (barIndexes.Count == 0) return howManyAdded;
			int barIndex_last = barIndexes[barIndexes.Count-1];

			List<double> pricesEmitted_forLastBar = this.PricesEmitted_byBarIndex[barIndex_last];

			int barIndex_current = this.Bars.Count - 1;
			if (barTill_streamingOrBarFilled == -1) barTill_streamingOrBarFilled = barIndex_current;
			//lastBarIndex++;
			if (barIndex_last >= barTill_streamingOrBarFilled) return howManyAdded;
			for (int i = barIndex_last; i <= barTill_streamingOrBarFilled; i++) {
				if (this.PricesEmitted_byBarIndex.ContainsKey(i)) {
					string msg = "";
					continue;
				}
				List<double> addingSinglePrice = new List<double>();
				addingSinglePrice.AddRange(pricesEmitted_forLastBar);
				this.PricesEmitted_byBarIndex.Add(i, addingSinglePrice);
				howManyAdded++;
			}
			return howManyAdded;
		}
	}
}
