using System;
using System.Collections.Generic;

using Newtonsoft.Json;


namespace Sq1.Core.Execution {
	public partial class Order {		
		[JsonIgnore]	public	List<Order>		DerivedOrders				{ get; protected set; }		// rebuilt on app restart from	DerivedOrdersGuids 
		[JsonProperty]	public	List<string>	DerivedOrdersGuids			{ get; protected set; }
		[JsonProperty]	public	Order			DerivedFrom	;				// SET_IN_OrdersShadowTreeDerived	{ get; protected set; }		// one parent with possibly its own parent, but not too deep; lazy to restore from DerivedFromGui only to rebuild Tree after restart


		public Order DeriveKillerOrder() {
			if (this.Alert == null) {
				string msg = "DeriveKillerOrder(): Alert=null (serializer will get upset) for " + this.ToString();
				throw new Exception(msg);
			}
			Order killer = new Order(this.Alert, this.EmittedByScript, false);
			killer.State = OrderState.JustConstructed;
			killer.PriceEmitted = 0;
			killer.PriceFilled = 0;
			killer.Qty = 0;
			killer.QtyFill = 0;

			killer.VictimToBeKilled = this;
			killer.VictimGUID = this.GUID;
			killer.Alert.SignalName = "IAM_KILLER_FOR " + this.Alert.SignalName;
			killer.IsKiller = true;

			this.KillerOrder = killer;
			this.KillerGUID = killer.GUID;
			this.IsVictim = true;
			
			this.DerivedOrdersAdd(killer);
			
			DateTime serverTimeNow = this.Alert.Bars.MarketInfo.ServerTimeNow;
			killer.CreatedBrokerTime = serverTimeNow;

			return killer;
		}
		public Order DeriveReplacementOrder() {
			if (this.Alert == null) {
				string msg = "DeriveReplacementOrder(): Alert=null (serializer will get upset) for " + this.ToString();
				throw new Exception(msg);
			}
			Order replacement = new Order(this.Alert, this.EmittedByScript, true);
			replacement.State = OrderState.JustConstructed;
			replacement.SlippageAppliedIndex = this.SlippageAppliedIndex;
			replacement.ReplacementForGUID = this.GUID;
			this.ReplacedByGUID = replacement.GUID;
			
			this.DerivedOrdersAdd(replacement);

			DateTime serverTimeNow = this.Alert.Bars.MarketInfo.ServerTimeNow;
			replacement.CreatedBrokerTime = serverTimeNow;

			return replacement;
		}
		public void DerivedOrdersAdd(Order killerReplacementPositionclose) {
			if (this.DerivedOrdersGuids.Contains(killerReplacementPositionclose.GUID)) {
				string msg = "ALREADY_ADDED DerivedOrder.GUID[" + killerReplacementPositionclose.GUID + "]";
				Assembler.PopupException(msg);
				return;
			}
			this.DerivedOrdersGuids.Add(killerReplacementPositionclose.GUID);
			this.DerivedOrders.Add(killerReplacementPositionclose);
			killerReplacementPositionclose.DerivedFrom = this;
		}
		public bool RebuildDerivedOrdersGuids() {
			List<string> backup = this.DerivedOrdersGuids;
			this.DerivedOrdersGuids = new List<string>();
			foreach (Order order in this.DerivedOrders) {
				this.DerivedOrdersGuids.Add(order.GUID);
			}
			return backup.Count != this.DerivedOrders.Count;
		}
		
		public Order FindOrderGuidAmongDerivedsRecursively(string Guid) {
			Order ret = null;
			foreach (Order derived in this.DerivedOrders) {
				if (derived.GUID != Guid) continue;
				ret = derived;
				break;
			}
			
			Order foundAmongChildrenOfDerived = null;
			if (ret == null) {
				foreach (Order derived in this.DerivedOrders) {
					foundAmongChildrenOfDerived = derived.FindOrderGuidAmongDerivedsRecursively(Guid);
					if (foundAmongChildrenOfDerived == null) continue;
					break;
				}
				if (foundAmongChildrenOfDerived != null) ret = foundAmongChildrenOfDerived; 
			}
			return ret;
		}
	}
}