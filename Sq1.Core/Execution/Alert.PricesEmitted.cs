using System.Collections.Generic;

using Newtonsoft.Json;

namespace Sq1.Core.Execution {
	public partial class Alert  {
		// Json throws with Self referencing error > JsonIgnore
		[JsonIgnore]	public	List<Order>							OrdersFollowed_killedAndReplaced	{ get; private set; }

		[JsonProperty]	public	SortedDictionary<int, List<double>> PricesEmitted_byBarIndex			{ get; private set; }

		internal void SetNew_OrderFollowed_PriceEmitted_fromReplacementOrder(Order replacementOrder) {
			this.OrdersFollowed_killedAndReplaced.Add(this.OrderFollowed_orCurrentReplacement);
			this.OrderFollowed_orCurrentReplacement = replacementOrder;
			this.PriceEmitted = replacementOrder.PriceEmitted;

			int barLast_index = this.Bars.BarLast.ParentBarsIndex;

			//MOVED_TO_ScriptExecutor.InvokeScript_onNewBar_onNewQuote()
			//int howManyAdded = this.FillPriceEmitted_fromLastChangeTillBar(this.Bars.Count - 2);

			int barIndex_current = this.Bars.Count - 1;
			if (this.PricesEmitted_byBarIndex.ContainsKey(barIndex_current) == false) {
				this.PricesEmitted_byBarIndex.Add(barIndex_current, new List<double>());
			}
			List<double> pricesForSameBar = this.PricesEmitted_byBarIndex[barIndex_current];
			pricesForSameBar.Add(this.PriceEmitted);
		}
		public List<double> GetEmittedPrice_forBarIndex_nullUnsafe(int barIndex_beingPainted) {
			if (this.PricesEmitted_byBarIndex.Count == 0) {
				List<double> singlePriceForBar = new List<double>();
				singlePriceForBar.Add(this.PriceEmitted);
				return singlePriceForBar;
			}

			List<int> barIndexes = new List<int>(this.PricesEmitted_byBarIndex.Keys);
			//MOVED_TO_ScriptExecutor.InvokeScript_onNewBar_onNewQuote()
			//if (this.FilledOrKilled) {
			//    if (barIndexes.Contains(this.FilledOrKilled_barIndex) == false) {
			//        int howManyAdded = this.FillPriceEmitted_fromLastChangeTillBar(this.FilledOrKilled_barIndex);
			//        barIndexes = new List<int>(this.PricesEmitted_byBarIndex.Keys);
			//    }
			////} else {
			////    int howManyAdded = this.fillPriceEmitted_fromLastChangeTillBar();
			////    barIndexes = new List<int>(this.PricesEmitted_byBarIndex.Keys);
			//}

			if (barIndexes.Contains(barIndex_beingPainted) == false) {
			    string msg = "PANEL_PRICE_SHOULD_NOT_ASK_ME_ABOUT_Alert.PriceEmitted_FOR_BAR_WHERE_ALERT_DIDNT_EXIST";
			    //Assembler.PopupException(msg);
			    return null;
			}

			List<double> ret = this.PricesEmitted_byBarIndex[barIndex_beingPainted];
			return ret;
		}
		public int FillPriceEmitted_fromLastChangeTillBar(int barTill_streamingOrBarFilled = -1) {
		    int howManyAdded = 0;
		    if (this.Bars == null) return howManyAdded;

		    List<int> barIndexes = new List<int>(this.PricesEmitted_byBarIndex.Keys);
		    if (barIndexes.Count == 0) return howManyAdded;
		    int barIndex_last = barIndexes[barIndexes.Count-1];

		    List<double> pricesEmitted_last = this.PricesEmitted_byBarIndex[barIndex_last];

		    int barIndex_current = this.Bars.Count - 1;
		    if (barTill_streamingOrBarFilled == -1) barTill_streamingOrBarFilled = barIndex_current;
		    int barIndex_next = barIndex_last + 1;

		    if (barIndex_next > barTill_streamingOrBarFilled) {
				string msg = "ALREADY_EXTENDED_PRICE_EMITTED_FOR_CURRENT_BAR.COUNT__DONT_INVOKE_ME_TWICE_FOR_SAME_BAR";
				Assembler.PopupException(msg, null, false);
				return howManyAdded;
			}
			int mustBeOne = barTill_streamingOrBarFilled - barIndex_next;
		    if (mustBeOne > 1) {
				string msg = "I_EXTEND_PRICE_EMITTED_TO_NEXT_BAR__EACH_BAR__CAN_NOT_BE_TWO_BARS";
				Assembler.PopupException(msg, null, false);
				return howManyAdded;
			}

		    for (int i = barIndex_next; i <= barTill_streamingOrBarFilled; i++) {
		        if (this.PricesEmitted_byBarIndex.ContainsKey(i)) {
		            string msg = "ON_NEW_BAR__YOU_ADD_FROM_LAST_CHANGE_TO_NOW__BUT_JOB_IS_DONE_ONLY_FOR_FRESH_BAR_ADDED";
					Assembler.PopupException(msg);
		            continue;
		        }
		        List<double> addingSinglePrice = new List<double>();
		        //addingSinglePrice.AddRange(pricesEmitted_last);
				addingSinglePrice.Add(this.PriceEmitted);
		        this.PricesEmitted_byBarIndex.Add(i, addingSinglePrice);
		        howManyAdded++;
		    }
		    return howManyAdded;
		}
	}
}
