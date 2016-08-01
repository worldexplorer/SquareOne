using System;
using System.Collections.Generic;

using Sq1.Core.Execution;

namespace Sq1.Core.Support {
	public class TimerSimplifiedThreading_withAlert : TimerSimplifiedThreading {
		public	Alert		Alert										{ get; private set; }
		public	List<Order>	ReplacementOrders_attempted			{ get; private set; }
		public	string		ReplacementOrders_alreadyEmitted_asGuidCSV	{ get {
			string ret = "";
			foreach (Order eachReplacement_alreadyEmitted in this.ReplacementOrders_attempted) {
				if (ret != "") ret += ",";
				ret += eachReplacement_alreadyEmitted.GUID;
			}
			return ret;
		} }

		public TimerSimplifiedThreading_withAlert(Alert alert, int millisToGetCallback_WaitingForOrderFill)
				: base("TIMER_FOR_ALERT__BASE_ORDER" + alert.OrderFollowed_orCurrentReplacement, millisToGetCallback_WaitingForOrderFill) {
			this.Alert = alert;
			this.ReplacementOrders_attempted = new List<Order>();
		}
	}
}