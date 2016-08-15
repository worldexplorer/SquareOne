using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public class OrderProcessorDataViewProxy {
		OrderProcessorDataSnapshot Snap;

		public bool			ShowingTreeTrue_FlatFalse;
		//{ get {
		//	this.Snap.OrdersSearchable_forGui.SafeCopy
		//} }

		public List<Order>	OrdersList_switchable { get {
			string msig = " //OrderProcessorDataViewProxy.OrdersList_switchable_get";
			List<Order> ret = null;
			if (this.ShowingTreeTrue_FlatFalse) {
				ret = this.Snap.OrdersRootOnly.SafeCopy;
			} else {
				if (foundForKeywords != null) {
					ret = foundForKeywords;
				} else {
					ret = this.Snap.OrdersSearchable_forGui.SafeCopy(this, msig);
					//ret = this.Snap.OrdersByState.Orders_withStatesDisplayed(this, msig);
				}
			}
			return ret;
		} }
		public OrdersSearchable	OrdersSearchable_forGui { get {
			return this.Snap.OrdersSearchable_forGui;
		} }

		OrderProcessorDataViewProxy() {
			this.ShowingTreeTrue_FlatFalse = true;
		}

		public OrderProcessorDataViewProxy(OrderProcessorDataSnapshot orderProcessorDataSnapshot) : this() {
			this.Snap = orderProcessorDataSnapshot;
		}

		public Order MostRecent_ordersAll_nullUnsafe { get {
			Order ret = null;
			List<Order>	currentOrdersShown = this.OrdersList_switchable;
			if (currentOrdersShown.Count == 0) return ret;
			ret = currentOrdersShown[0];
			return ret;
		} }

		List<Order> foundForKeywords;
		public void SearchForKeywords(string keywordsCsv_nullUnsafe) {
			if (string.IsNullOrEmpty(keywordsCsv_nullUnsafe)) {
				foundForKeywords = null;
				return;
			}
			foundForKeywords = this.Snap.OrdersSearchable_forGui.SearchForKeywords_StaticSnapshotSubset(keywordsCsv_nullUnsafe);
		}
	}
}
