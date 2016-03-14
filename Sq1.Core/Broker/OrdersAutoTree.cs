using System;
using System.Collections.Generic;

using Sq1.Core.Execution;

namespace Sq1.Core.Broker {
	public class OrdersAutoTree : OrderLane {
		OrderLane ordersAll;
		
		public OrdersAutoTree() : base() {
		}
		public void InitializeScanDeserialized_moveDerivedsInside_buildTreeShadow(OrderLane ordersAllDeserialized) {
			base.Clear();
			this.ordersAll = ordersAllDeserialized;
			this.scanDeserialized_moveDerivedsInside_buildTreeShadow(this.ordersAll);
		}
		public void InsertUnique_toRoot(Order orderAdded) {
			Order orderParent = orderAdded.DerivedFrom;
			if (orderParent != null) return;
			base.InsertUnique(orderAdded);
		}
		void scanDeserialized_moveDerivedsInside_buildTreeShadow(OrderLane ordersFlat) {
			string msig = " OrdersAutoTree::scanDeserializedMoveDerivedsInsideBuildTreeShadow(): ";
			int derivedsFound = 0;

			List<Order> foundSoRemoveFromRoot = new List<Order>();
			foreach (Order orderWithDeriveds in ordersFlat.SafeCopy) {
				if (orderWithDeriveds.DerivedOrdersGuids == null) continue;
				foreach (string guidToFind in orderWithDeriveds.DerivedOrdersGuids) {
					string ident = "orderWithDeriveds[" + orderWithDeriveds.GUID + "] has derivedGuid[" + guidToFind + "]";
					if (orderWithDeriveds.GUID == guidToFind) {
						string msg = ident + " DerivedOrdersGuids contains my own Guid: cyclic links ONE leveldeep (TODO: MANY leveldeep)";
						Assembler.PopupException(msg + msig);
						continue;
					}
					
					Order orderFound = null;
					foreach (Order orderEveryScanning in ordersFlat.SafeCopy) {
						if (orderEveryScanning.GUID != guidToFind) continue;
						orderFound = orderEveryScanning;
						break;
					}
					
					if (orderFound == null) {
						string msg = ident + " but I couldn't find it"
							+ " among ordersAllDeserialized; you won't see derived[" + guidToFind + "] in ExecutionForm";
						Assembler.PopupException(msg + msig, null, false);
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
			
			foreach (Order order in ordersFlat.SafeCopy) {
				if (foundSoRemoveFromRoot.Contains(order)) continue;
				base.InsertUnique(order);
			}

			string stats = "DERIVEDS_MOVED[" + derivedsFound + "] = ordersFlat.Count[" + ordersFlat.Count + "] - base.Count[" + base.Count + "]";
			//Assembler.PopupException(stats + msig);
		}

		public void Remove_fromRootLevel_keepOrderPointers(List<Order> ordersToRemove, bool popupIfDoesntContain = true) {
			foreach (Order orderRemoving in ordersToRemove) {
				if (orderRemoving.DerivedFrom != null) continue;
				if (base.ContainsGuid(orderRemoving) == false) {
					if (orderRemoving.Alert.MyBrokerIsLivesim) {
						string msg = "DID_I_CLEAR_LIVESIM_ORDERS_EARLIER??WHEN?";
						continue;
					}
					if (popupIfDoesntContain) {
						string msg = "ORDER_AUTOTREE_DOESNT_CONTAIN_ORDER_YOU_WILLING_TO_REMOVE " + this.ToStringCount() + " orderRemoving" + orderRemoving;
						Assembler.PopupException(msg);
					}
					continue;
				}
				base.Remove(orderRemoving);
			}
		}
	}
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
