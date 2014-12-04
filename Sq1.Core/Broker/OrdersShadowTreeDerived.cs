using System;
using System.Collections.Generic;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public class OrdersShadowTreeDerived : OrderLane {
		OrderLane ordersAll;
		public OrderEventDistributor OrderEventDistributor;
		
		public OrdersShadowTreeDerived() : base() {
			this.OrderEventDistributor = new OrderEventDistributor();
		}
		public void InitializeScanDeserializedMoveDerivedsInsideBuildTreeShadow(OrderLane ordersAllDeserialized) {
			base.Clear();
			this.ordersAll = ordersAllDeserialized;
			this.scanDeserializedMoveDerivedsInsideBuildTreeShadow(this.ordersAll);
		}
//		public Order FindOrderGuidOnRootLevel(string Guid) {
//			Order orderParent = null;
//			foreach (var each in this.ordersAll) {
//				if (each.GUID != Guid) continue; 
//				orderParent = each;
//				break;
//			}
//			return orderParent;
//		}
//		public Order FindOrderGuidAmongDerivedsRecursively(string Guid) {
//			Order ret = null
//			foreach (var each in this.ordersAll) {
//				var found = each.FindOrderGuidAmongDerivedsRecursively(Guid);
//				if (found == null) continue;
//				ret = found;
//				break;
//			}
//			return ret;
//		}
		//InsertToShadowTreeAfterAddingThisOrderToAnotherOrdersDerivedSlow
		public void InsertToShadowTreeRaiseExecutionFormNotification(int indexInsertToIfOrderNotDerived, Order orderAdded) {
			Order orderParent = orderAdded.DerivedFrom;
			if (orderParent == null) {
				base.Insert(indexInsertToIfOrderNotDerived, orderAdded);
			}
			this.OrderEventDistributor.RaiseOrderAddedExecutionFormNotification(this, orderAdded);
		}
		void scanDeserializedMoveDerivedsInsideBuildTreeShadow(OrderLane ordersFlat) {
			string msig = " OrdersShadowTreeDerived::scanDeserializedMoveDerivedsInsideBuildTreeShadow(): ";
			int derivedsFound = 0;

			List<Order> foundSoRemoveFromRoot = new List<Order>();
			foreach (Order orderWithDeriveds in ordersFlat.InnerOrderList) {
				if (orderWithDeriveds.DerivedOrdersGuids == null) continue;
				foreach (string guidToFind in orderWithDeriveds.DerivedOrdersGuids) {
					string ident = "orderWithDeriveds[" + orderWithDeriveds.GUID + "] has derivedGuid[" + guidToFind + "]";
					if (orderWithDeriveds.GUID == guidToFind) {
						string msg = ident + " DerivedOrdersGuids contains my own Guid: cyclic links ONE leveldeep (TODO: MANY leveldeep)";
						Assembler.PopupException(msg + msig);
						continue;
					}
					
					Order orderFound = null;
					foreach (Order orderEveryScanning in ordersFlat.InnerOrderList) {
						if (orderEveryScanning.GUID != guidToFind) continue;
						orderFound = orderEveryScanning;
						break;
					}
					
					if (orderFound == null) {
						string msg = ident + " but I couldn't find it"
							+ " among ordersAllDeserialized; you won't see derived[" + guidToFind + "] in ExecutionForm";
						Assembler.PopupException(msg + msig);
						continue;
					}
					
					derivedsFound++;
					
					if (orderWithDeriveds.DerivedOrders.Contains(orderFound)) {
						string msg = "ALREADY_UNFLATTENED_INTO_SHADOWTREE: you already restored DerivedOrders from DerivedOrdersGuids"
							+ "; no need for the second pass; you don't want duplicates in DerivedOrders"
							+ "; get rid of this redundant invocation upstack";
						throw new Exception(msg + msig);
					}
				
					orderWithDeriveds.DerivedOrders.Add(orderFound);
					orderFound.DerivedFrom = orderWithDeriveds;
					foundSoRemoveFromRoot.Add(orderFound);
				}
			}
			
			foreach (Order order in ordersFlat.InnerOrderList) {
				if (foundSoRemoveFromRoot.Contains(order)) continue;
				base.Insert(0, order);
			}

			string stats = "DERIVEDS_MOVED[" + derivedsFound + "] = ordersFlat.Count[" + ordersFlat.InnerOrderList.Count + "] - base.Count[" + base.InnerOrderList.Count + "]";
			//Assembler.PopupException(stats + msig);
		}
		
		public void RemoveFromShadowTree(List<Order> ordersToRemove) {
			//base.RemoveRange(ordersToRemove);
			//throw new NotImplementedException("ExecutionTree.HideCompletedOrders] is a shaky ground; why do you need to remove? tree scan is a PITA");
		}
		public void AppendOrderMessageAndRaiseOrderMessageAddedExecutionFormNotification(object sender, OrderStateMessage orderStateMessage) {
			orderStateMessage.Order.AppendMessageSynchronized(orderStateMessage);
			this.OrderEventDistributor.RaiseOrderMessageAddedExecutionFormNotification(sender, orderStateMessage);
		}
	}
}
