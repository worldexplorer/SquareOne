using System;
using System.Collections.Generic;

using Sq1.Core.Execution;
using Sq1.Core.Support;

namespace Sq1.Core.Broker {
	//v1
	public class OrdersRootOnly {
	//v2 copypasted all I need from OrderLane - was hoping to inherit from ConcurrentListFiltered<Order> ...
	//public class OrdersRootOnly {

	//v3 this will never work for the tree - I will implement HasParent here for ExecutionTreeControl.Customiser
	//v3 (problematic also nesting two orders matching but related through unmatched chain between)
	//public class OrdersRootOnly : ConcurrentListFiltered<Order> {
	//v3
	//public object SafeCopy(object lockOwner, string lockReason) {
	//}
	//public List<Exception> InitKeywordsToExclude_AndSetPointer(string keywordsCsv_nullUnsafe) {
	//}
	//public List<Exception> SearchForKeywords_StaticSnapshotSubset(string keywordsCsv_nullUnsafe) {
	//}

				OrderLane							ordersAll;
		public	Order								MostRecent_ordersAll_nullUnsafe	{ get {	return this.ordersAll.MostRecent_nullUnsafe; } }

#region copypaste from OrderLane
		            object			ordersLock;
		readonly	string			reasonToExist;
		protected	List<Order>		InnerOrderList_recentFirst	{ get; private set; }
		            List<string>	ordersGuids_recentFirst;	//{ get; protected set; }
		public		List<Order>		SafeCopy					{ get { lock (this.ordersLock) { return new List<Order>(this.InnerOrderList_recentFirst); } } }
		
		OrdersRootOnly() {
		    this.InnerOrderList_recentFirst = new List<Order>();
		    this.ordersLock = new Object();
		    this.InnerOrderList_recentFirst.Capacity = 2000;
		    this.ordersGuids_recentFirst = new List<string>();
		}
		public OrdersRootOnly(string reasonToExist_passed) : this() {
		    this.reasonToExist = reasonToExist_passed;
		}

		protected virtual bool checkThrowAdd	(Order order) { return true; }
		protected virtual bool checkThrowRemove	(Order order) { return true; }
		public void InsertUnique(Order order) { lock (this.ordersLock) {
		    if (this.checkThrowAdd(order) == false) return;
		    if (this.ContainsGuid(order) == true) {
		        string msg = "Already in " + this.ToString() + ": " + order;
		        throw new Exception(msg);
		        //break;
		    }
		    order.AddedToOrdersListCounter++;
		    this.InnerOrderList_recentFirst.Insert(0, order);
		    this.ordersGuids_recentFirst.Insert(0, order.GUID);
		} }
		public int Count { get { lock (this.ordersLock) {
		    return this.InnerOrderList_recentFirst.Count;
		} } } 
		public bool ContainsGuid(Order order) { lock (this.ordersLock) {
		    //return base.Contains(order);
		    return this.ordersGuids_recentFirst.Contains(order.GUID);
		} }
		public int Clear() { lock (this.ordersLock) {
		    int ordersDropped = this.Count;
		    this.InnerOrderList_recentFirst.Clear();
		    this.ordersGuids_recentFirst.Clear();
		    return ordersDropped;
		} }
		public  bool RemoveUnique(Order order) { lock (this.ordersLock) {
		    if (this.checkThrowRemove(order) == false) return false;
		    if (this.ContainsGuid(order) == false) {
		        string msg2 = "REMOVING_ORDER_NOT_FOUND in " + this.ToString()
		            + ": (already removed or never stored before? broker callback Dupe?) " + order;
		        throw new Exception(msg2);
		        //break;
		    }
		    order.AddedToOrdersListCounter--;
		    this.ordersGuids_recentFirst.Remove(order.GUID);
		    return this.InnerOrderList_recentFirst.Remove(order);
		} }

		public override string ToString() {
		    return this.reasonToExist + ".Count=[" + this.Count + "]";
		}
#endregion


		public void InitializeScanDeserialized_moveDerivedsInside_buildTreeShadow(OrderLane ordersAllDeserialized) {
			this.Clear();
			this.ordersAll = ordersAllDeserialized;
			this.scanDeserialized_moveDerivedsInside_buildTreeShadow(this.ordersAll);
		}
		public void InsertUnique_onlyToRoot(Order orderAdded) {
			Order orderParent = orderAdded.DerivedFrom;
			if (orderParent != null) return;
			this.InsertUnique(orderAdded);
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
				this.InsertUnique(order);
			}

			string stats = "DERIVEDS_MOVED[" + derivedsFound + "] = ordersFlat.Count[" + ordersFlat.Count + "] - this.Count[" + this.Count + "]";
			//Assembler.PopupException(stats + msig);
		}

		public void Remove_fromRootLevel_keepOrderPointers(List<Order> ordersToRemove, bool popupIfDoesntContain = true) {
			foreach (Order orderRemoving in ordersToRemove) {
				if (orderRemoving.DerivedFrom != null) continue;
				if (this.ContainsGuid(orderRemoving) == false) {
					if (orderRemoving.Alert.MyBrokerIsLivesim) {
						string msg = "DID_I_CLEAR_LIVESIM_ORDERS_EARLIER??WHEN?";
						continue;
					}
					if (popupIfDoesntContain) {
						string msg = "ORDER_AUTOTREE_DOESNT_CONTAIN_ORDER_YOU_WILLING_TO_REMOVE " + this.ToString() + " orderRemoving" + orderRemoving;
						Assembler.PopupException(msg);
					}
					continue;
				}
				this.RemoveUnique(orderRemoving);
			}
		}

	}
}

