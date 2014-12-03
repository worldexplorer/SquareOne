using System;
using System.Threading;

using Sq1.Core;
using Sq1.Core.Execution;
using Sq1.Core.DataTypes;
using Sq1.Adapters.Quik.Terminal;

namespace Sq1.Adapters.QuikMock.Terminal {
	public class QuikTerminalMock : QuikTerminal {
		public override string DllName { get { return "QUIK_MOCK"; } }
		
		bool simulateUnexistingGUID				= false;
		bool simulateUnexistingSernoExchange	= false;
		bool simulateTradeStatus				= false;
		bool simulateOrderStatusDupes			= false;
		
		BrokerMock mockBrokerProvider { get { return base.BrokerQuik as BrokerMock; } }

		public QuikTerminalMock() {
			throw new Exception("FOR_JSON_DESERIALIZER QuikTerminalMock doesn't support default constructor, use QuikTerminalMock(BrokerMock)");
		}
		public QuikTerminalMock(BrokerMock mockBrokerProvider) : base(mockBrokerProvider) {
			base.BrokerQuik = mockBrokerProvider;
			base.BrokerQuik.callbackTerminalConnectionStateUpdated(QuikConnectionState.DllConnected, "Connected to " + this.DllName);
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
			if (this.SymbolClassSubscribers.ContainsKey(key)) {
				this.SymbolClassSubscribers[key]++;
			} else {
				this.SymbolClassSubscribers.Add(key, 1);
				if (this.SymbolClassSubscribers.Count == 0) CurrentStatus = "";
				CurrentStatus = SymbolClassSubscribersAsString;

				base.BrokerQuik.callbackTerminalConnectionStateUpdated(QuikConnectionState.ConnectedSubscribed,
					"QuikTerminal(" + this.DllName + ") 2/2 " + QuikConnectionState.ConnectedSubscribed
					+ " for SecCode[" + SecCode + "] ClassCode[" + ClassCode + "]");
			}
		}

		static double SernoExchange = 1000000;
		static readonly Random Rng = new Random();

		int CONST_nMode = 0;
		int CONST_msum = 9999191;
		int CONST_descriptor = -111111;

		void orderSimulateFillLoopStateThreadEntryPoint(object o) {
			string msig = " //QuikTerminalMock(" + this.DllName + ").orderSimulateFillLoopStateThreadEntryPoint()";
			QuikTerminalMockThreadParam tp = (QuikTerminalMockThreadParam)o;

			//Debugger.Break();
			string msg = "";
			Order order = base.BrokerQuik.OrderProcessor.DataSnapshot.OrdersSubmitting.FindByGUID(tp.GUID.ToString());
			if (order == null) {
				order = base.BrokerQuik.OrderProcessor.DataSnapshot.OrdersPending.FindByGUID(tp.GUID.ToString());
			}
			if (order == null) {
				order = base.BrokerQuik.OrderProcessor.DataSnapshot.OrdersAll.FindByGUID(tp.GUID.ToString());
			}
			if (order == null) {
				msg = "OrdersPending.FindByGUID(" + tp.GUID.ToString() + "=null, can't update with Active/Filled";
				Assembler.PopupException(msg, null, false);
				return;
			}

			try {
				// setting status to WaitingFillBroker
				//nStatus Тип: Long. Состояние исполнения заявки: Значение «1» соответствует состоянию «Активна», «2» - «Снята», иначе «Исполнена» 
				int nStatus = 1;
				base.CallbackOrderStatus(CONST_nMode, tp.GUID, tp.SernoExchange, tp.ClassCode, tp.SecCode,
					-999.99, tp.Balance, 9999191, tp.IsSell, nStatus, 111111);
				// let order "become" WaitingFillBroker (why State didn't change  immediately after base.CallbackOrderStatus???)
				Thread.Sleep(this.mockBrokerProvider.ExecutionDelayMillis);

				// CallbackOrderStatus sets the status to WaitingFillBroker immediately, check it here
				if (OrderStatesCollections.NoInterventionRequired.Contains(order.State) == false) {
					string allowed = OrderStatesCollections.NoInterventionRequired.ToString();
					msg = "MOCK_CHECK: order.State[" + order.State + "] NOT_IN (" + allowed + ") after QuikTerminal::CallbackOrderStatus(), won't run the loop!";
					base.BrokerQuik.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msg + msig);
					//order.Alert.Strategy.Script.Executor.RemovePendingAlertClosePosition(order.Alert, msig);
					order.Alert.Strategy.Script.Executor.CreatedOrderWontBePlacedPastDueInvokeScript(order.Alert, order.Alert.Bars.Count);
					return;
				}

				while (this.mockBrokerProvider.SignalToTerminateAllOrderTryFillLoopsInAllMocks == false) {
					bool abortThread = this.orderSimulateFillLoopStep(order, tp);
					if (abortThread) break;
				}
			} catch (Exception ex) {
				//KEPT_FOR_FUTURE_PER_STRATEGY_OR_PER_PROVIDER_SEPARATE_EXCEPTION_LIST
				//order.Alert.Strategy.Script.Executor.PopupException("QuikTerminalMock::OrderTryFillLoopEachOrderNewThread()", e);
				//base.BrokerQuik.OrderProcessor.PopupException("QuikTerminalMock::OrderTryFillLoopEachOrderNewThread()", e);
				Assembler.PopupException(msig, ex);
			}
		}

		bool orderSimulateFillLoopStep(Order order, QuikTerminalMockThreadParam tp) {
			string msig = " //QuikTerminalMock(" + this.DllName + ").orderSimulateFillLoopStep(order[" + order + "] tp[" + tp + "])";
			string msg = "";

			bool abortThread = false;
			Exception exCaught = null;
			//Thread.Sleep(this.mockBrokerProvider.ExecutionDelayMillis / 2);

			switch (order.State) {
				case OrderState.WaitingBrokerFill:
					Quote quoteToFillAgainst;
					try {
						if (order.Alert.Strategy == null) {
							msg += "JSON_DESERIALIZED_ORDERS_ONLY_HAVE_ALERT.STRATEGY_NULL BUT_WHY_WOULD_YOU_FILL_IT_HERE? ";
							abortThread = true;
							break;
						}
						quoteToFillAgainst = order.Alert.QuoteCreatedThisAlert;
						if (order.Alert.QuoteCreatedThisAlert.ParentBarStreaming.ParentBarsIndex == -1) {
							msg += "EARLY_BINDER_DIDNT_DO_ITS_JOB#2 order.Alert.QuoteCreatedThisAlert.ParentStreamingBar.ParentBarsIndex=-1 ";
							abortThread = true;
							break;
						}
						Quote quoteLast = this.mockBrokerProvider.StreamingProvider.StreamingDataSnapshot.LastQuoteCloneGetForSymbol(order.Alert.Symbol);
						if (quoteLast.SameBidAsk(order.Alert.QuoteCreatedThisAlert)) {
							msg += "SURE_CURRENT_QUOTE_AND_QuoteCreatedThisAlert_MUST_BE_DIFFERENT_HERE that's why QuikBrokerEditor has ExecutionDelay!"
								+ "; TODO: split it to two 1) DelayOrderEmittedToFilled (Limit came too late))"
								+ ", 2) DelayOrderFilledCallback (Strategy gets callback 1 sec later after filled by the Market) ";
							//abortThread = true;
							//break;
						}
						//if (quoteLast.ParentBarStreaming.ParentBarsIndex == -1) {
						//    msg += "EARLY_BINDER_DIDNT_DO_ITS_JOB#1 quote.ParentStreamingBar.ParentBarsIndex=-1 ";
						//    abortThread = true;
						//    break;
						//}
						//quoteToFillAgainst = quoteLast;
					} catch (Exception ex) {
						msg += "FAILED_INNER_mockBrokerProvider.StreamingProvider.StreamingDataSnapshot.LastQuoteGetForSymbol( " + order.Alert.Symbol + ")";
						exCaught = ex;
						break;
					}

					bool filled = false;
					bool abortTryFill = false;
					string abortTryFillReason = "NO_ABORT_TRY_FILL_MESSAGE";
					try {
						filled = order.Alert.Strategy.Script.Executor.MarketSim.SimulateFillLive(order.Alert, quoteToFillAgainst, out abortTryFill, out abortTryFillReason);
					} catch (Exception ex) {
						msg = "FAILED_INNER_MarketSim.SimulateFillLive(" + quoteToFillAgainst + ") " + msg;
						exCaught = ex;
						break;
					}
					if (abortTryFill == true) {
						msg = "FAILED_INNER_MarketSim.SimulateFillLive(): " + abortTryFillReason + msg;
						abortThread = true;
						break;
					}
					if (filled) {
						try {
							this.OrderFilledNotifyLikeTerminalCallback(order, tp);
							msg = "THREAD_FILLED_ORDER__SUCCESSFULLY_COMPLETED_OrderFilledNotifyLikeTerminalCallback() " + msg;
							base.BrokerQuik.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msg + msig);
							abortThread = true;
							break;
						} catch (Exception ex) {
							msg = "FAILED_INNER_OrderFilledNotifyLikeTerminalCallback() " + msg;
							exCaught = ex;
							abortThread = true;
							break;
						}
					} else {
						//msg = " order.State[" + order.State + "] == WaitingFillBroker but not filled @[" + tp.TryFillInvokedTimes + "] attempt;"
						//	+ " waiting for the price to reach alert.PriceScript[" + order.Alert.PriceScript + "]";
					}
					break;
				case OrderState.AlertCreatedOnPreviousBarNotAutoSubmitted:
					msg = "SHOULD_NEVER_BE_HERE: BrokerProvider should never get PastExecutionDueNotAutoSubmitted orders"
						+ ";  check CreateOrdersSubmitToBrokerProvider() once again";
					base.BrokerQuik.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msg + msig);
					//order.Alert.Strategy.Script.Executor.RemovePendingEntryAlertCloneFromEntryClosePositionBacktestEnded(order.Alert);
					// I'm suggesting a wrapper instead of low-level hanlder
					//order.Alert.Strategy.Script.Executor.CreatedOrderWontBePlacedPastDueInvokeScript(order.Alert, order.Alert.Bars.Count);
					abortThread = true;
					break;
				//case OrderState.Submitting:
				//	break;
				case OrderState.KillPending:
				case OrderState.Killed:
				case OrderState.TPAnnihilated:
				case OrderState.SLAnnihilated:
					msg = "THREAD_TERMINATING_ANNIHILATED: [" + order.State + "] is expected and handled";
					base.BrokerQuik.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msg + msig);
					abortThread = true;
					break;
				default:
					msg = "THREAD_TERMINATING_WEIRD: got [" + order.State + "] while WaitingFillBroker should've been set!!!";
					base.BrokerQuik.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msg + msig);
					abortThread = true;
					break;
			}

			if (abortThread || exCaught != null) {
				if (exCaught != null) {
					msg = "THREAD_ABORTING_BY_EXCEPTION: " + msg;
					base.BrokerQuik.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msg + msig);
					//Assembler.PopupException(msg + msig, exCaught, false);
				}
				return abortThread;
			}

			if (msg != "") {
				msg += "; sleeping [" + this.mockBrokerProvider.ExecutionDelayMillis + "]ms"
					+ " and continuing OrderTryFillLoopEachOrderNewThread";
				base.BrokerQuik.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msg + msig);

			}

			tp.TryFillInvokedTimes++;
			Thread.Sleep(this.mockBrokerProvider.ExecutionDelayMillis);
			//Thread.Sleep(this.mockBrokerProvider.ExecutionDelayMillis / 2);
			
			return abortThread;
		}

		protected void OrderFilledNotifyLikeTerminalCallback(Order order, QuikTerminalMockThreadParam tp) {
			string msig = " //QuikTerminalMock(" + this.DllName + ")::OrderFilled()";
			string msg = "";

			int sleepMs = this.mockBrokerProvider.ExecutionDelayMillis + Rng.Next(100, 1000);
			msg = "Sleeping random [" + (sleepMs) + "]ms; status=>[" + tp.QuikStatus + "]";
			base.BrokerQuik.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(order, msg + msig);
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

			// 3 identical excessive (spamming) ORDER callbacks like QUIK does; "Filled/Rejected"; subject for OrderCallbackDupesCheckerQuik
			base.CallbackOrderStatus(CONST_nMode, tp.GUID, tp.SernoExchange, tp.ClassCode, tp.SecCode,
				tp.Price, tp.Balance, CONST_msum, tp.IsSell, tp.QuikStatus, CONST_descriptor);
			if (this.simulateOrderStatusDupes == true) {
				base.CallbackOrderStatus(CONST_nMode, tp.GUID, tp.SernoExchange, tp.ClassCode, tp.SecCode,
					tp.Price, tp.Balance, CONST_msum, tp.IsSell, tp.QuikStatus, CONST_descriptor);
				base.CallbackOrderStatus(CONST_nMode, tp.GUID, tp.SernoExchange, tp.ClassCode, tp.SecCode,
					tp.Price, tp.Balance, CONST_msum, tp.IsSell, tp.QuikStatus, CONST_descriptor);
			}

			if (this.simulateUnexistingSernoExchange == true) {
				this.simulateUnexistingSernoExchange = false;
				base.CallbackOrderStatus(CONST_nMode, tp.GUID, tp.SernoExchange * 2, tp.ClassCode, tp.SecCode,
					tp.Price, tp.Balance, CONST_msum, tp.IsSell, tp.QuikStatus, CONST_descriptor);
			}

			if (this.simulateUnexistingGUID == true) {
				this.simulateUnexistingGUID = false;
				base.CallbackOrderStatus(CONST_nMode, -tp.GUID, tp.SernoExchange, tp.ClassCode, tp.SecCode,
					tp.Price, tp.Balance, CONST_msum, tp.IsSell, tp.QuikStatus, CONST_descriptor);
			}
		}

		public override void SendTransactionOrderAsync(char opBuySell, char typeMarketLimitStop,
				string SecCode, string ClassCode, double price, int quantity,
				string GUID, out int SernoSession, out string msgSumbittedOut, out OrderState orderStateOut) {

			//Debugger.Break();
			if (!this.IsSubscribed(SecCode, ClassCode)) this.Subscribe(SecCode, ClassCode);
			string trans = base.getOrderCommand(opBuySell, typeMarketLimitStop, SecCode, ClassCode, price, quantity, GUID, out SernoSession);
			Order orderFound = base.BrokerQuik.OrderProcessor.UpdateOrderStateByGuidNoPostProcess(GUID, OrderState.Submitting, trans);
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

			Thread th = new Thread(new ParameterizedThreadStart(this.orderSimulateFillLoopStateThreadEntryPoint));
			//Thread th = new Thread(new ParameterizedThreadStart(OrderTryFillLoop));
			th.Name = Thread.CurrentThread.ManagedThreadId + " >> MOCK.OrderTryFillLoop(" + opBuySell + typeMarketLimitStop + quantity + "@" + price + ")";
			//Debugger.Break();
			th.Start(tp);
			// STICKY_ORDERS_NOT_GETTING_EXECUTED the new thread should wait for Submitted to get updated, MreActiveCanCome to signal and then continue;
			// otherwize WaitingFillBroker magically happens before the upperStack OrderSubmit() sets Submitted returned here and Submitted stays forever
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

			Order orderKiller = base.BrokerQuik.OrderProcessor.DataSnapshot.OrdersSubmitting.FindByGUID(KillerGUID.ToString());
			//if (orderKiller == null) {
			//	Order orderKiller = base.BrokerQuik.OrderProcessor.DataSnapshot.OrdersPending.FindByGUID(KillerGUID.ToString());
			//}
			if (orderKiller == null) {
				 orderKiller = base.BrokerQuik.OrderProcessor.DataSnapshot.OrdersAll.FindByGUID(KillerGUID.ToString());
			}
			int a = 1;
			if (orderKiller == null) {
				msg = "order==null, can't update with KillSubmittingAsync";
				base.BrokerQuik.OrderProcessor.UpdateOrderStateDontPostProcess(orderKiller,
					new OrderStateMessage(orderKiller, OrderState.Error, victimMsgSumbitted + msg));
				return;
			}
			if (orderKiller.KillerOrder != null) {
				msg = "use ParametrizedCallbackOrderStatus() for labourOrder fill simulation!";
				base.BrokerQuik.OrderProcessor.UpdateOrderStateDontPostProcess(orderKiller,
					new OrderStateMessage(orderKiller, OrderState.Error, victimMsgSumbitted + msg));
				return;
			}
			if (orderKiller.State != OrderState.KillerPreSubmit) {
				msg = "orderKiller.State[" + orderKiller.State + "] != KillerPreSubmit (victim kill failed?), won't update with KillSubmittingAsync";
				base.BrokerQuik.OrderProcessor.UpdateOrderStateDontPostProcess(orderKiller,
					new OrderStateMessage(orderKiller, OrderState.Error, victimMsgSumbitted + msg));
				return;
			}

			string trans = this.getOrderKillCommand(SecCode, ClassCode, victimWasStopOrder, VictimGUID, VictimSernoExchange, out SernoSession);
			base.BrokerQuik.OrderProcessor.UpdateOrderStateDontPostProcess(orderKiller,
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
			Order orderKiller = base.BrokerQuik.OrderProcessor.DataSnapshot.OrdersSubmitting.FindByGUID(t.KillerGUID.ToString());
			if (orderKiller == null) {
				orderKiller = base.BrokerQuik.OrderProcessor.DataSnapshot.OrdersPending.FindByGUID(t.KillerGUID.ToString());
			}
			if (orderKiller == null) {
				 orderKiller = base.BrokerQuik.OrderProcessor.DataSnapshot.OrdersAll.FindByGUID(t.KillerGUID.ToString());
			}
			int a =	1;
			try {
				if (orderKiller == null) {
					msg = "orderKiller==null, can't update with KillerBulletFlying";
					base.BrokerQuik.OrderProcessor.UpdateOrderStateDontPostProcess(orderKiller,
						new OrderStateMessage(orderKiller, OrderState.Error, msg + msig));
					return;
				}
				if (orderKiller.KillerOrder != null) {
					msg = " use ParametrizedCallbackOrderStatus() for labourOrder fill simulation!";
					base.BrokerQuik.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(orderKiller, msg + msig);
					return;
				}

				int sleepMs = this.mockBrokerProvider.ExecutionDelayMillis + Rng.Next(100, 1000);
				msg = "Sleeping random [" + sleepMs + "]ms; status=>[" + t.QuikStatus + "]";
				base.BrokerQuik.OrderProcessor.UpdateOrderStateDontPostProcess(orderKiller,
					new OrderStateMessage(orderKiller, OrderState.KillerBulletFlying, msg + msig));
				Thread.Sleep(sleepMs);

				if (orderKiller.State != OrderState.KillerBulletFlying) {
					msg = "orderKiller.State[" + orderKiller.State + "] != KillerBulletFlying (victim kill failed?), won't update victim.Killed <=> killerFilled;"
						+ " t.t_status[" + t.QuikStatus + "]?=2)";
					base.BrokerQuik.OrderProcessor.AppendOrderMessageAndPropagateCheckThrowOrderNull(orderKiller, msg + msig);
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
			} catch (Exception ex) {
				//KEPT_FOR_FUTURE_PER_STRATEGY_OR_PER_PROVIDER_SEPARATE_EXCEPTION_LIST
				//orderKiller.Alert.Strategy.Script.Executor.PopupException("QuikTerminalMock::KillOrderEachOrderNewThread()", e);
				Assembler.PopupException(msig, ex);
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