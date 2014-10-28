using System;
using System.Threading;
using Sq1.Core;
using Sq1.Core.Execution;

namespace Sq1.QuikAdapter.BrokerInteropApi {
	public class QuikTerminalMock : QuikTerminal {
		public override string DllName { get { return "QUIK_MOCK"; } }
		bool simulateUnexistingGUID = false;
		bool simulateUnexistingSernoExchange = false;
		bool simulateTradeStatus = false;
		bool simulateOrderStatusDupes = false;
		
		MockBrokerProvider mockBrokerProvider {
			get { return base.quikBrokerProvider as MockBrokerProvider; }
		}

		public QuikTerminalMock() {
			throw new Exception("QuikTerminalMock doesn't support default constructor, use QuikTerminalMock(MockBrokerProvider)");
		}
		public QuikTerminalMock(MockBrokerProvider mockBrokerProvider) : base(mockBrokerProvider) {
			// base needs its own....
			base.quikBrokerProvider = mockBrokerProvider;
			// this is needs its own....
			//this.mockBrokerProvider = mockBrokerProvider;
			base.quikBrokerProvider.callbackTerminalConnectionStateUpdated(
				QuikTerminalConnectionState.DllConnected, "Соединение с " + this.DllName + " установлено");
		}
		public override void ConnectDll() {
			DllConnected = true;
		}
		public override void DisconnectDll() {
			DllConnected = false;
		}
		public void Subscribe(string SecCode, string ClassCode) {
			if (!DllConnected) {
				string msg = "can not RegisterQuoteConsumer(" + SecCode + "," + ClassCode + "): DLL must be connected first";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}

			string key = SecCode + ":" + ClassCode;
			if (SymbolClassSubscribers.ContainsKey(key)) {
				SymbolClassSubscribers[key]++;
			} else {
				SymbolClassSubscribers[key] = 1;
				if (SymbolClassSubscribers.Count == 0) CurrentStatus = "";
				CurrentStatus = SymbolClassSubscribers.ToString();

				base.quikBrokerProvider.callbackTerminalConnectionStateUpdated(QuikTerminalConnectionState.ConnectedSubscribed,
					"QuikTerminal(" + this.DllName + ") 2/2 " + QuikTerminalConnectionState.ConnectedSubscribed
					+ " for SecCode[" + SecCode + "] ClassCode[" + ClassCode + "]");
			}
		}

		static double SernoExchange = 1000000;
		static readonly Random Rng = new Random();

		int CONST_nMode = 0;
		int CONST_msum = 9999191;
		int CONST_descriptor = -111111;

		protected void OrderTryFillLoopEachOrderNewThread(object o) {
			string msig = "QuikTerminal(" + this.DllName + ")::OrderTryFillLoopEachOrderNewThread(): ";
			QuikTerminalMockThreadParam tp = (QuikTerminalMockThreadParam)o;

			string msg = "";
			Order order = base.quikBrokerProvider.OrderProcessor.DataSnapshot.OrdersSubmitting.FindByGUID(tp.GUID.ToString());
			if (order == null) {
				order = base.quikBrokerProvider.OrderProcessor.DataSnapshot.OrdersPending.FindByGUID(tp.GUID.ToString());
			}
			if (order == null) {
				order = base.quikBrokerProvider.OrderProcessor.DataSnapshot.OrdersAll.FindByGUID(tp.GUID.ToString());
			}
			if (order == null) {
				msg = "OrdersPending.FindByGUID(" + tp.GUID.ToString() + "=null, can't update with Active/Filled";
				base.quikBrokerProvider.OrderProcessor.PopupException(new Exception(msg));
				return;
			}

			try {
				// let all the Submitting status reach the order (ManualResetEvent?...) - impossible tend to happen
				// (avoiding Active have the same timestamp as Submitting and come after Submitting - screenshot)
				//Thread.Sleep(100);
				//bool signalled = order.MreActiveCanCome.WaitOne(-1);

				//if (order.State != OrderState.Submitted) {
				//	msg = " order.State[" + order.State + "] != Submitted, won't update to Active, won't run the loop!";
				//	mockBrokerProvider.OrderManager.AppendMessageAndPropagate(order, msig + msg);
				//	order.Alert.Strategy.Script.Executor.RemovePendingAlertClosePosition(order.Alert, msig);
				//	return;
				//}

				//if (OrderStatesCollections.AllowedForSubmissionToBrokerProvider.Contains(order.State) == false) {
				//	string allowed = OrderStatesCollections.AllowedForSubmissionToBrokerProvider.ToString();
				//	msg = "MOCK_CHECK: order.State[" + order.State + "] NOT_IN (" + allowed + ") after QuikTerminal::CallbackOrderStatus(), won't run the loop!"
				//		+ "; just wanna make sure all pre-Active statuses so while() loop wont need to deal with them";
				//	base.quikBrokerProvider.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msig + msg);
				//	//order.Alert.Strategy.Script.Executor.RemovePendingAlertClosePosition(order.Alert, msig);
				//	order.Alert.Strategy.Script.Executor.CreatedOrderWontBePlacedPastDueInvokeScript(order.Alert, order.Alert.Bars.Count);
				//	return;
				//}

				// set status to active "Active"
				//nStatus Тип: Long. Состояние исполнения заявки: Значение «1» соответствует состоянию «Активна», «2» - «Снята», иначе «Исполнена» 
				int nStatus = 1;
				base.CallbackOrderStatus(CONST_nMode, tp.GUID, tp.SernoExchange, tp.ClassCode, tp.SecCode,
					-999.99, tp.Balance, 9999191, tp.IsSell, nStatus, 111111);
				// let order "become" Active (why State didn't change  immediately after base.CallbackOrderStatus???)
				//WTF?... Thread.Sleep(this.mockBrokerProvider.ExecutionDelayMillis);

				// CallbackOrderStatus sets the status to Active immediately, check it here
				if (OrderStatesCollections.NoInterventionRequired.Contains(order.State) == false) {
					string allowed = OrderStatesCollections.NoInterventionRequired.ToString();
					msg = "MOCK_CHECK: order.State[" + order.State + "] NOT_IN (" + allowed + ") after QuikTerminal::CallbackOrderStatus(), won't run the loop!";
					base.quikBrokerProvider.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msig + msg);
					//order.Alert.Strategy.Script.Executor.RemovePendingAlertClosePosition(order.Alert, msig);
					order.Alert.Strategy.Script.Executor.CreatedOrderWontBePlacedPastDueInvokeScript(order.Alert, order.Alert.Bars.Count);
					return;
				}

				while (this.mockBrokerProvider.SignalToTerminateAllOrderTryFillLoopsInAllMocks == false) {
					switch (order.State) {
						case OrderState.Active:
							bool filled = false;
							bool abortTryFill = false;
							string abortTryFillReason = "NO_ABORT_TRY_FILL_MESSAGE";
							try {
								//double priceFill = -1;
								//double slippageFill = -1;
								filled = order.Alert.Strategy.Script.Executor.MarketSimStreaming.SimulateFill(order.Alert, out abortTryFill, out abortTryFillReason);	//, out priceFill, out slippageFill);
							} catch (Exception e) {
								msg = "SimulateRealtimeOrderFill() THREAD_TERMINATING_EXCEPTION: [" + order.State + "]"
									+ ": " + e.ToString();
								base.quikBrokerProvider.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msig + msg);
								order.Alert.DataSource.BrokerProvider.StatusReporter.PopupException(e);
								return;
							}
							if (abortTryFill == true) {
								msg = "OrderFilledNotifyLikeTerminalCallback(): THREAD_ABORTED_TERMINATING: [" + order.State + "]"
									+ ": " + abortTryFillReason;
								base.quikBrokerProvider.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msig + msg);
								return;
							}
							if (filled) {
								try {
									this.OrderFilledNotifyLikeTerminalCallback(order, tp);
									msg = "THREAD_TERMINATING_OK: [" + order.State + "] OrderFilledNotifyLikeTerminalCallback() complete";
									base.quikBrokerProvider.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msig + msg);
								} catch (Exception e) {
									msg = "OrderFilledNotifyLikeTerminalCallback(): THREAD_TERMINATING_EXCEPTION: [" + order.State + "]"
										+ ": " + e.ToString();
									base.quikBrokerProvider.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msig + msg);
									order.Alert.DataSource.BrokerProvider.StatusReporter.PopupException(e);
									return;
								}
								return;
							} else {
								//msg = " order.State[" + order.State + "] == Active but not filled @[" + tp.TryFillInvokedTimes + "] attempt;"
								//	+ " waiting for the price to reach alert.PriceScript[" + order.Alert.PriceScript + "]";
							}
							break;
						case OrderState.AlertCreatedOnPreviousBarNotAutoSubmitted:
							msg = "SHOULD_NEVER_LAND_HERE: BrokerProvider should never get PastExecutionDueNotAutoSubmitted orders"
								+ ";  check CreateOrdersSubmitToBrokerProvider() once again";
							base.quikBrokerProvider.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msig + msg);
							//order.Alert.Strategy.Script.Executor.RemovePendingEntryAlertCloneFromEntryClosePositionBacktestEnded(order.Alert);
							// I'm suggesting a wrapper instead of low-level hanlder
							//order.Alert.Strategy.Script.Executor.CreatedOrderWontBePlacedPastDueInvokeScript(order.Alert, order.Alert.Bars.Count);
							return;
						//case OrderState.Submitting:
						//	break;
						case OrderState.KillPending:
						case OrderState.Killed:
						case OrderState.TPAnnihilated:
						case OrderState.SLAnnihilated:
							msg = "THREAD_TERMINATING_ANNIHILATED: [" + order.State + "] is expected and handled";
							base.quikBrokerProvider.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msig + msg);
							return;
						default:
							msg = "THREAD_TERMINATING_WEIRD: got [" + order.State + "] while Active should've been set!!!";
							base.quikBrokerProvider.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msig + msg);
							return;
					}

					if (msg != "") {
						msg += "; sleeping [" + this.mockBrokerProvider.ExecutionDelayMillis + "]ms"
							+ " and continuing OrderTryFillLoopEachOrderNewThread";
						base.quikBrokerProvider.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msig + msg);
					}

					tp.TryFillInvokedTimes++;
					Thread.Sleep(this.mockBrokerProvider.ExecutionDelayMillis);
				}
			} catch (Exception e) {
				order.Alert.Strategy.Script.Executor.PopupException("QuikTerminalMock::OrderTryFillLoopEachOrderNewThread()", e);
				//base.quikBrokerProvider.OrderProcessor.PopupException("QuikTerminalMock::OrderTryFillLoopEachOrderNewThread()", e);
			}
		}

		protected void OrderFilledNotifyLikeTerminalCallback(Order order, QuikTerminalMockThreadParam tp) {
			string msig = "QuikTerminal(" + this.DllName + ")::OrderFilled(): ";
			string msg = "";

			int sleepMs = this.mockBrokerProvider.ExecutionDelayMillis + Rng.Next(100, 1000);
			msg = " Sleeping random [" + (sleepMs) + "]ms; status=>[" + tp.QuikStatus + "]";
			base.quikBrokerProvider.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msig + msg);
			Thread.Sleep(sleepMs);

			if (this.mockBrokerProvider.RejectAllUpcoming) {
				tp.QuikStatus = 2;
			}
			if (tp.QuikStatus != 2 && this.mockBrokerProvider.RejectFirstNOrders > 0
					&& tp.SernoSession <= this.mockBrokerProvider.RejectFirstNOrders) {
				tp.QuikStatus = 2;	// always rejected
			}
			if (tp.QuikStatus != 2 && this.mockBrokerProvider.RejectRandomly) {
				tp.QuikStatus = Rng.Next(0, 2);
				if (tp.QuikStatus == 1) tp.QuikStatus = 2;
			}

			// 1 TRADE callbacks like QUIK does
			if (this.simulateTradeStatus == true) {
				this.CallbackTradeStatus(CONST_nMode, tp.GUID, tp.SernoExchange, tp.ClassCode, tp.SecCode,
					tp.Price, tp.Filled, tp.Balance, tp.IsSell, CONST_descriptor);
				Thread.Sleep(sleepMs);
			}

			// 3 identical ORDER callbacks like QUIK does; "Filled/Rejected"; subject for OrderCallbackDupesCheckerQuik
			base.CallbackOrderStatus(CONST_nMode, tp.GUID, tp.SernoExchange, tp.ClassCode, tp.SecCode,
				tp.Price, tp.Balance, CONST_msum, tp.IsSell, tp.QuikStatus, CONST_descriptor);
			if (this.simulateOrderStatusDupes == true) {
				base.CallbackOrderStatus(CONST_nMode, tp.GUID, tp.SernoExchange, tp.ClassCode, tp.SecCode,
					tp.Price, tp.Balance, CONST_msum, tp.IsSell, tp.QuikStatus, CONST_descriptor);
				base.CallbackOrderStatus(CONST_nMode, tp.GUID, tp.SernoExchange, tp.ClassCode, tp.SecCode,
					tp.Price, tp.Balance, CONST_msum, tp.IsSell, tp.QuikStatus, CONST_descriptor);
			}

			if (simulateUnexistingSernoExchange == true) {
				simulateUnexistingSernoExchange = false;
				base.CallbackOrderStatus(CONST_nMode, tp.GUID, tp.SernoExchange * 2, tp.ClassCode, tp.SecCode,
					tp.Price, tp.Balance, CONST_msum, tp.IsSell, tp.QuikStatus, CONST_descriptor);
			}

			if (simulateUnexistingGUID == true) {
				simulateUnexistingGUID = false;
				base.CallbackOrderStatus(CONST_nMode, -tp.GUID, tp.SernoExchange, tp.ClassCode, tp.SecCode,
					tp.Price, tp.Balance, CONST_msum, tp.IsSell, tp.QuikStatus, CONST_descriptor);
			}
		}

		public override void SendTransactionOrderAsync(char opBuySell, char typeMarketLimitStop,
				string SecCode, string ClassCode, double price, int quantity,
				string GUID, out int SernoSession, out string msgSumbittedOut, out OrderState orderStateOut) {

			if (!IsSubscribed(SecCode, ClassCode)) Subscribe(SecCode, ClassCode);
			string trans = base.getOrderCommand(opBuySell, typeMarketLimitStop, SecCode, ClassCode, price, quantity, GUID, out SernoSession);
			Order orderFound = base.quikBrokerProvider.OrderProcessor.UpdateOrderStateByGuidNoPostProcess(GUID, OrderState.Submitting, trans);
			orderStateOut = OrderState.Submitted;

			msgSumbittedOut = "QuikTerminal(" + this.DllName + ")::SendTransactionOrderAsync() Trans2Quik.Result.SUCCESS    "
				+ ((this.callbackErrorMsg.Length > 0) ? this.callbackErrorMsg.ToString() : " error[" + error + "]");
			//this.OrderManager.AppendMessageForOrderGuidAndPropagate(GUID, orderState, msgSumbitted);

			QuikTerminalMockThreadParam tp = new QuikTerminalMockThreadParam();
			tp.ClassCode = ClassCode;
			tp.SecCode = SecCode;
			tp.Price = price;
			tp.Balance = 0;
			tp.GUID = Convert.ToInt32(GUID);
			tp.SernoSession = SernoSession;
			tp.SernoExchange = SernoExchange++;
			tp.QuikStatus = 0;	//Значение «1» соответствует состоянию «Активна», «2» - «Снята», иначе «Исполнена» 
			tp.Filled = quantity;
			tp.IsSell = (opBuySell == 'S') ? 1 : 0;
			tp.OrderStatus = orderStateOut;
			tp.TryFillInvokedTimes = 0;

			Thread th = new Thread(new ParameterizedThreadStart(OrderTryFillLoopEachOrderNewThread));
			//Thread th = new Thread(new ParameterizedThreadStart(OrderTryFillLoop));
			th.Name = Thread.CurrentThread.ManagedThreadId + " >> MOCK.OrderTryFillLoop(" + opBuySell + typeMarketLimitStop + quantity + "@" + price + ")";
			th.Start(tp);
			// STICKY_ORDERS_NOT_GETTING_EXECUTED the new thread should wait for Submitted to get updated, MreActiveCanCome to signal and then continue;
			// otherwize Active magically happens before the upperStack OrderSubmit() sets Submitted returned here and Submitted stays forever
		}


		public override void SendTransactionOrderKillAsync(string SecCode, string ClassCode,
				string KillerGUID, string VictimGUID, long VictimSernoExchange, bool victimWasStopOrder,
				out string victimMsgSumbitted, out int SernoSession, out OrderState victimOrderState) {

			string msig = "QuikTerminal(" + this.DllName + ")::SendTransactionOrderKillAsync(): ";
			string msg = "";

			if (!IsSubscribed(SecCode, ClassCode)) Subscribe(SecCode, ClassCode);
			victimMsgSumbitted = msig + " Trans2Quik.Result.PRE_CHECK_FAILED";
			victimOrderState = OrderState.Error;
			SernoSession = -9999;

			Order orderKiller = base.quikBrokerProvider.OrderProcessor.DataSnapshot.OrdersSubmitting.FindByGUID(KillerGUID.ToString());
			//if (orderKiller == null) {
			//	Order orderKiller = base.quikBrokerProvider.OrderProcessor.DataSnapshot.OrdersPending.FindByGUID(KillerGUID.ToString());
			//}
			if (orderKiller == null) {
				 orderKiller = base.quikBrokerProvider.OrderProcessor.DataSnapshot.OrdersAll.FindByGUID(KillerGUID.ToString());
			}
			int a = 1;
			if (orderKiller == null) {
				msg = "order==null, can't update with KillSubmittingAsync";
				base.quikBrokerProvider.OrderProcessor.UpdateOrderStateNoPostProcess(orderKiller,
					new OrderStateMessage(orderKiller, OrderState.Error, victimMsgSumbitted + msg));
				return;
			}
			if (orderKiller.KillerOrder != null) {
				msg = "use ParametrizedCallbackOrderStatus() for labourOrder fill simulation!";
				base.quikBrokerProvider.OrderProcessor.UpdateOrderStateNoPostProcess(orderKiller,
					new OrderStateMessage(orderKiller, OrderState.Error, victimMsgSumbitted + msg));
				return;
			}
			if (orderKiller.State != OrderState.KillerPreSubmit) {
				msg = "orderKiller.State[" + orderKiller.State + "] != KillerPreSubmit (victim kill failed?), won't update with KillSubmittingAsync";
				base.quikBrokerProvider.OrderProcessor.UpdateOrderStateNoPostProcess(orderKiller,
					new OrderStateMessage(orderKiller, OrderState.Error, victimMsgSumbitted + msg));
				return;
			}

			string trans = this.getOrderKillCommand(SecCode, ClassCode, victimWasStopOrder, VictimGUID, VictimSernoExchange, out SernoSession);
			base.quikBrokerProvider.OrderProcessor.UpdateOrderStateNoPostProcess(orderKiller,
				new OrderStateMessage(orderKiller, OrderState.KillerSubmitting, trans));
			
			victimMsgSumbitted = msig + "Trans2Quik.Result.SUCCESS    "
				+ ((this.callbackErrorMsg.Length > 0) ? this.callbackErrorMsg.ToString() : " error[" + error + "]");
			victimOrderState = OrderState.KillPending;

			QuikTerminalMockThreadParam tp = new QuikTerminalMockThreadParam();
			tp.ClassCode = ClassCode;
			tp.SecCode = SecCode;
			tp.Balance = 0;
			tp.GUID = Convert.ToInt32(VictimGUID);
			tp.SernoSession = SernoSession;
			tp.SernoExchange = SernoExchange++;
			tp.KillerGUID = KillerGUID;
			tp.IsKillCallback = true;
			tp.QuikStatus = 2;	//Значение «1» соответствует состоянию «Активна», «2» - «Снята», иначе «Исполнена» 
			//tp.Filled = quantity;
			tp.OrderStatus = victimOrderState;

			Thread th = new Thread(new ParameterizedThreadStart(KillOrderEachOrderNewThread));
			th.Name = Thread.CurrentThread.ManagedThreadId + " >> MOCK.SendTransactionOrderAsync(" + VictimSernoExchange + ")";
			th.Start(tp);
		}
		protected void KillOrderEachOrderNewThread(object o) {
			string msig = "QuikTerminal(" + this.DllName + ")::ParametrizedCallbackKillOrderStatus(): ";
			string msg = "";

			QuikTerminalMockThreadParam t = (QuikTerminalMockThreadParam)o;
			Order orderKiller = base.quikBrokerProvider.OrderProcessor.DataSnapshot.OrdersSubmitting.FindByGUID(t.KillerGUID.ToString());
			if (orderKiller == null) {
				orderKiller = base.quikBrokerProvider.OrderProcessor.DataSnapshot.OrdersPending.FindByGUID(t.KillerGUID.ToString());
			}
			if (orderKiller == null) {
				 orderKiller = base.quikBrokerProvider.OrderProcessor.DataSnapshot.OrdersAll.FindByGUID(t.KillerGUID.ToString());
			}
			int a =	1;
			try {
				if (orderKiller == null) {
					msg = "orderKiller==null, can't update with KillerBulletFlying";
					base.quikBrokerProvider.OrderProcessor.UpdateOrderStateNoPostProcess(orderKiller,
						new OrderStateMessage(orderKiller, OrderState.Error, msig + msg));
					return;
				}
				if (orderKiller.KillerOrder != null) {
					msg = " use ParametrizedCallbackOrderStatus() for labourOrder fill simulation!";
					base.quikBrokerProvider.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(orderKiller, msig + msg);
					return;
				}

				int sleepMs = this.mockBrokerProvider.ExecutionDelayMillis + Rng.Next(100, 1000);
				msg = "Sleeping random [" + sleepMs + "]ms; status=>[" + t.QuikStatus + "]";
				base.quikBrokerProvider.OrderProcessor.UpdateOrderStateNoPostProcess(orderKiller,
					new OrderStateMessage(orderKiller, OrderState.KillerBulletFlying, msig + msg));
				Thread.Sleep(sleepMs);

				if (orderKiller.State != OrderState.KillerBulletFlying) {
					msg = "orderKiller.State[" + orderKiller.State + "] != KillerBulletFlying (victim kill failed?), won't update victim.Killed <=> killerFilled;"
						+ " t.t_status[" + t.QuikStatus + "]?=2)";
					base.quikBrokerProvider.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(orderKiller, msig + msg);
					return;
				}

				int nMode = 0;
				int msum = 9999191;
				int descriptor = -111111;

				// 1 TRADE callbacks like QUIK does
				if (this.simulateTradeStatus == true) {
					this.CallbackTradeStatus(nMode, t.GUID, t.SernoExchange, t.ClassCode, t.SecCode,
						t.Price, t.Filled, t.Balance, t.IsSell, descriptor);
					Thread.Sleep(sleepMs);
				}

				// notify the labourOrder about t_status=2 (you are killed)
				base.CallbackOrderStatus(nMode, t.GUID, t.SernoExchange, t.ClassCode, t.SecCode,
					t.Price, t.Balance, msum, t.IsSell, t.QuikStatus, descriptor);
				// 3 identical ORDER callbacks like QUIK does; "Filled/Rejected"; subject for OrderCallbackDupesCheckerQuik
				if (this.simulateOrderStatusDupes == true) {
					base.CallbackOrderStatus(nMode, t.GUID, t.SernoExchange, t.ClassCode, t.SecCode,
						t.Price, t.Balance, msum, t.IsSell, t.QuikStatus, descriptor);
					base.CallbackOrderStatus(nMode, t.GUID, t.SernoExchange, t.ClassCode, t.SecCode,
						t.Price, t.Balance, msum, t.IsSell, t.QuikStatus, descriptor);
				}

				if (simulateUnexistingSernoExchange == true) {
					simulateUnexistingSernoExchange = false;
					base.CallbackOrderStatus(nMode, t.GUID, t.SernoExchange * 2, t.ClassCode, t.SecCode,
						t.Price, t.Balance, msum, t.IsSell, t.QuikStatus, descriptor);
				}

				if (simulateUnexistingGUID == true) {
					simulateUnexistingGUID = false;
					base.CallbackOrderStatus(nMode, -t.GUID, t.SernoExchange, t.ClassCode, t.SecCode,
						t.Price, t.Balance, msum, t.IsSell, t.QuikStatus, descriptor);
				}
			} catch (Exception e) {
				orderKiller.Alert.Strategy.Script.Executor.PopupException("QuikTerminalMock::KillOrderEachOrderNewThread()", e);
			}
		}
		public override void sendTransactionKillAll(string SecCode, string ClassCode, string GUID, out string msgSumbitted) {
			transId++;
			String trans = ""
				+ "TRANS_ID=" + Order.newGUID() + ";"
				+ "SECCODE=" + SecCode + ";"
				+ "CLASSCODE=" + ClassCode + ";"
				+ "ACTION=KILL_ALL_FUTURES_ORDERS;";
			//quikTransactionsAttemptedLog.Put("QuikTerminal(" + this.DllName + ")::sendTransactionKillAll(): " + trans);
			throw new Exception("NYI");
			Trans2Quik.Result r = Trans2Quik.SEND_ASYNC_TRANSACTION(trans, out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
			msgSumbitted = "QuikTerminal(" + this.DllName + "):: " + r + "    " + ((this.callbackErrorMsg.Length > 0)
				? this.callbackErrorMsg.ToString() : " error[" + error + "]");

			QuikTerminalMockThreadParam tp = new QuikTerminalMockThreadParam();
			tp.ClassCode = ClassCode;
			tp.SecCode = SecCode;
			tp.Price = -5;
			tp.Balance = 0;
			tp.SernoSession = 0;
			tp.QuikStatus = 0;

			Thread th = new Thread(new ParameterizedThreadStart(KillOrderEachOrderNewThread));
			th.Name = Thread.CurrentThread.ManagedThreadId + " >> MOCK.sendTransactionKillAll(" + SecCode + ")";
			th.Start(tp);
		}
	}
}