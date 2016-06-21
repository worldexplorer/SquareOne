using System;
using System.Threading;

namespace Sq1.Core.Livesim {
	public class LivesimBrokerSpoiler {
				LivesimBroker livesimBroker;

		public	int		DelayBeforeFill		{ get; private set; }
		public	int		DelayBeforeKill		{ get; private set; }

		public	bool	RejectOrderNow		{ get; private set; }

		public	int		DelayTransactionStatusAfterOrderStatus		{ get; private set; }

		LivesimBrokerSpoiler() {
			this.DelayBeforeFill = 0;
			this.RejectOrderNow = false;
		}

		public LivesimBrokerSpoiler(LivesimBroker livesimBroker) : this() {
			this.livesimBroker = livesimBroker;
		}

		public int DelayBeforeFill_calculate() {
			this.DelayBeforeFill = 0;
			if (this.livesimBroker.LivesimBrokerSettings.DelayBeforeFillEnabled == false) return this.DelayBeforeFill;
			this.DelayBeforeFill = this.livesimBroker.LivesimBrokerSettings.DelayBeforeFillMillisMin;
			if (this.livesimBroker.LivesimBrokerSettings.DelayBeforeFillMillisMax > 0) {
				int range = Math.Abs(this.livesimBroker.LivesimBrokerSettings.DelayBeforeFillMillisMin -
										this.livesimBroker.LivesimBrokerSettings.DelayBeforeFillMillisMax);
				double rnd0to1 = new Random().NextDouble();
				int rangePart = (int)Math.Round(range * rnd0to1);
				this.DelayBeforeFill += rangePart;
			}
			return this.DelayBeforeFill;
		}
		public void DelayBeforeFill_threadSleep() {
			if (this.DelayBeforeFill <= 0) return;
			//Application.DoEvents();
			Thread.Sleep(this.DelayBeforeFill);
		}

		public int DelayBeforeKill_calculate() {
			this.DelayBeforeKill = 0;
			if (this.livesimBroker.LivesimBrokerSettings.KillPendingDelayEnabled == false) return this.DelayBeforeKill;
			this.DelayBeforeKill = this.livesimBroker.LivesimBrokerSettings.KillPendingDelayMillisMin;
			if (this.livesimBroker.LivesimBrokerSettings.KillPendingDelayMillisMax > 0) {
				int range = Math.Abs(this.livesimBroker.LivesimBrokerSettings.KillPendingDelayMillisMin -
										this.livesimBroker.LivesimBrokerSettings.KillPendingDelayMillisMax);
				double rnd0to1 = new Random().NextDouble();
				int rangePart = (int)Math.Round(range * rnd0to1);
				this.DelayBeforeKill += rangePart;
			}
			return this.DelayBeforeKill;
		}
		public void DelayBeforeKill_threadSleep() {
			if (this.DelayBeforeKill <= 0) return;
			Thread.Sleep(this.DelayBeforeKill);
		}

	
		int howManyOrders_wereNotRejected = 0;
		public bool RejectNow_calculate() {
			this.RejectOrderNow = false;
			if (this.livesimBroker.LivesimBrokerSettings.OrderRejectionEnabled == false) return this.RejectOrderNow;

			int planned_nonRejectedLimit = this.livesimBroker.LivesimBrokerSettings.OrderRejectionHappensOncePerXordersMin;
			if (planned_nonRejectedLimit == 0) return this.RejectOrderNow;

			if (this.livesimBroker.LivesimBrokerSettings.OrderRejectionHappensOncePerXordersMax > 0) {
				int range = Math.Abs(this.livesimBroker.LivesimBrokerSettings.OrderRejectionHappensOncePerXordersMin -
									 this.livesimBroker.LivesimBrokerSettings.OrderRejectionHappensOncePerXordersMax);
				double rnd0to1 = new Random().NextDouble();
				int rangePart = (int)Math.Round(range * rnd0to1);
				planned_nonRejectedLimit += rangePart;
			}

			if (this.howManyOrders_wereNotRejected >= planned_nonRejectedLimit) {
				this.RejectOrderNow = true;
				this.howManyOrders_wereNotRejected = 0;
			}
			this.howManyOrders_wereNotRejected++;
			return this.RejectOrderNow;
		}




	
		bool delayTransactionStatusAfterOrderStatus_happensNow = false;
		int howManyOrders_wereNotTransactionStatusAfterOrderStatus = 0;
		bool delayTransactionStatusAfterOrderStatus_happensNow_calculate() {
			this.delayTransactionStatusAfterOrderStatus_happensNow = false;
			
			int planned_notHappeningNow = this.livesimBroker.LivesimBrokerSettings.TransactionStatusAfterOrderStatusHappensOncePerOrdersMin;
			if (planned_notHappeningNow == 0) return this.delayTransactionStatusAfterOrderStatus_happensNow;

			if (this.livesimBroker.LivesimBrokerSettings.OrderRejectionHappensOncePerXordersMax > 0) {
				int range = Math.Abs(this.livesimBroker.LivesimBrokerSettings.TransactionStatusAfterOrderStatusHappensOncePerOrdersMin -
									 this.livesimBroker.LivesimBrokerSettings.TransactionStatusAfterOrderStatusHappensOncePerOrdersMax);
				double rnd0to1 = new Random().NextDouble();
				int rangePart = (int)Math.Round(range * rnd0to1);
				planned_notHappeningNow += rangePart;
			}

			if (this.howManyOrders_wereNotTransactionStatusAfterOrderStatus >= planned_notHappeningNow) {
				this.delayTransactionStatusAfterOrderStatus_happensNow = true;
				this.howManyOrders_wereNotTransactionStatusAfterOrderStatus = 0;
			}
			this.howManyOrders_wereNotTransactionStatusAfterOrderStatus++;
			return this.delayTransactionStatusAfterOrderStatus_happensNow;
		}
		public int DelayTransactionStatusAfterOrderStatus_calculate() {
			this.DelayTransactionStatusAfterOrderStatus = 0;

			if (this.livesimBroker.LivesimBrokerSettings.TransactionStatusAfterOrderStatusEnabled == false)
				return this.DelayTransactionStatusAfterOrderStatus;

			if (this.delayTransactionStatusAfterOrderStatus_happensNow_calculate() == false) 
				return this.DelayTransactionStatusAfterOrderStatus;

			this.DelayTransactionStatusAfterOrderStatus = this.livesimBroker.LivesimBrokerSettings.TransactionStatusAfterOrderStatusDelayAfterFillMin;
			if (this.livesimBroker.LivesimBrokerSettings.TransactionStatusAfterOrderStatusDelayAfterFillMax > 0) {
				int range = Math.Abs(this.livesimBroker.LivesimBrokerSettings.TransactionStatusAfterOrderStatusDelayAfterFillMin -
										this.livesimBroker.LivesimBrokerSettings.TransactionStatusAfterOrderStatusDelayAfterFillMax);
				double rnd0to1 = new Random().NextDouble();
				int rangePart = (int)Math.Round(range * rnd0to1);
				this.DelayTransactionStatusAfterOrderStatus += rangePart;
			}
			return this.DelayTransactionStatusAfterOrderStatus;
		}
		public void DelayTransactionStatusAfterOrderStatus_threadSleep() {
			if (this.DelayTransactionStatusAfterOrderStatus <= 0) return;
			//Application.DoEvents();
			Thread.Sleep(this.DelayTransactionStatusAfterOrderStatus);
		}


	

	
		bool delayKillerTransactionCallbackAfterVictimFilled_happensNow = false;
		int howManyOrders_wereNotKillerTransactionCallbackAfterVictimFilled = 0;
		private int DelayKillerTransactionCallbackAfterVictimFilled;
		bool delayKillerTransactionCallbackAfterVictimFilled_happensNow_calculate() {
			this.delayKillerTransactionCallbackAfterVictimFilled_happensNow = false;
			
			int planned_notHappeningNow = this.livesimBroker.LivesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledHappensOncePerKillersMin;
			if (planned_notHappeningNow == 0) return this.delayKillerTransactionCallbackAfterVictimFilled_happensNow;

			if (this.livesimBroker.LivesimBrokerSettings.OrderRejectionHappensOncePerXordersMax > 0) {
				int range = Math.Abs(this.livesimBroker.LivesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledHappensOncePerKillersMin -
									 this.livesimBroker.LivesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledHappensOncePerKillersMax);
				double rnd0to1 = new Random().NextDouble();
				int rangePart = (int)Math.Round(range * rnd0to1);
				planned_notHappeningNow += rangePart;
			}

			if (this.howManyOrders_wereNotKillerTransactionCallbackAfterVictimFilled >= planned_notHappeningNow) {
				this.delayKillerTransactionCallbackAfterVictimFilled_happensNow = true;
				this.howManyOrders_wereNotKillerTransactionCallbackAfterVictimFilled = 0;
			}
			this.howManyOrders_wereNotKillerTransactionCallbackAfterVictimFilled++;
			return this.delayKillerTransactionCallbackAfterVictimFilled_happensNow;
		}
		public int DelayKillerTransactionCallbackAfterVictimFilled_calculate() {
			this.DelayKillerTransactionCallbackAfterVictimFilled = 0;

			if (this.livesimBroker.LivesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledEnabled == false)
				return this.DelayKillerTransactionCallbackAfterVictimFilled;

			if (this.delayKillerTransactionCallbackAfterVictimFilled_happensNow_calculate() == false) 
				return this.DelayKillerTransactionCallbackAfterVictimFilled;

			this.DelayKillerTransactionCallbackAfterVictimFilled = this.livesimBroker.LivesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledDelayMin;
			if (this.livesimBroker.LivesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledDelayMax > 0) {
				int range = Math.Abs(this.livesimBroker.LivesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledDelayMin -
										this.livesimBroker.LivesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledDelayMax);
				double rnd0to1 = new Random().NextDouble();
				int rangePart = (int)Math.Round(range * rnd0to1);
				this.DelayKillerTransactionCallbackAfterVictimFilled += rangePart;
			}
			return this.DelayKillerTransactionCallbackAfterVictimFilled;
		}
		public void DelayKillerTransactionCallbackAfterVictimFilled_threadSleep() {
			if (this.DelayKillerTransactionCallbackAfterVictimFilled <= 0) return;
			//Application.DoEvents();
			Thread.Sleep(this.DelayKillerTransactionCallbackAfterVictimFilled);
		}



	
		public bool Disconnect_happensNow = false;
		int howManyOrders_wereNotDisconnected = 0;
		public int DelayReconnect_USELESS;
		public bool Disconnect_happensNow_calculate() {
			this.Disconnect_happensNow = false;
			if (this.livesimBroker.LivesimBrokerSettings.AdapterDisconnectEnabled == false) return this.Disconnect_happensNow;
			
			int planned_notHappeningNow = this.livesimBroker.LivesimBrokerSettings.AdapterDisconnectHappensOncePerOrderMin;
			if (planned_notHappeningNow == 0) return this.Disconnect_happensNow;

			if (this.livesimBroker.LivesimBrokerSettings.OrderRejectionHappensOncePerXordersMax > 0) {
				int range = Math.Abs(this.livesimBroker.LivesimBrokerSettings.AdapterDisconnectHappensOncePerOrderMin -
									 this.livesimBroker.LivesimBrokerSettings.AdapterDisconnectHappensOncePerOrderMax);
				double rnd0to1 = new Random().NextDouble();
				int rangePart = (int)Math.Round(range * rnd0to1);
				planned_notHappeningNow += rangePart;
			}

			if (this.howManyOrders_wereNotDisconnected >= planned_notHappeningNow) {
				this.Disconnect_happensNow = true;
				this.howManyOrders_wereNotDisconnected = 0;
			}
			this.howManyOrders_wereNotDisconnected++;
			return this.Disconnect_happensNow;
		}
		//public int DelayDisconnect_calculate() {
		//    this.DelayReconnect_USELESS = 0;
		//    //will be part of disconnect_happensNow_calculate()
		//    //if (this.livesimBroker.LivesimBrokerSettings.AdapterDisconnectEnabled == false)
		//    //    return this.DelayReconnect_USELESS;
		//    if (this.disconnect_happensNow_calculate() == false)
		//        return this.DelayReconnect_USELESS;
		//    this.DelayReconnect_USELESS = this.livesimBroker.LivesimBrokerSettings.AdapterDisconnectReconnectsAfterMillisMin;
		//    if (this.livesimBroker.LivesimBrokerSettings.AdapterDisconnectReconnectsAfterMillisMax > 0) {
		//        int range = Math.Abs(this.livesimBroker.LivesimBrokerSettings.AdapterDisconnectReconnectsAfterMillisMin -
		//                                this.livesimBroker.LivesimBrokerSettings.AdapterDisconnectReconnectsAfterMillisMax);
		//        double rnd0to1 = new Random().NextDouble();
		//        int rangePart = (int)Math.Round(range * rnd0to1);
		//        this.DelayReconnect_USELESS += rangePart;
		//    }
		//    return this.DelayReconnect_USELESS;
		//}
		//public void Reconnect_threadSleep() {
		//    if (this.DelayReconnect_USELESS <= 0) return;
		//    //Application.DoEvents();
		//    Thread.Sleep(this.DelayReconnect_USELESS);
		//}

	}
}
