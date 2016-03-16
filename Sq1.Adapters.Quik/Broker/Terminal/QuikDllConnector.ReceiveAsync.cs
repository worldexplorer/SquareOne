using System;
using System.Runtime.InteropServices;

using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.Execution;

namespace Sq1.Adapters.Quik.Broker.Terminal {
	public sealed partial class QuikDllConnector {

/*Функция TRANS2QUIK_TRADE_STATUS_CALLBACK
Функция обратного вызова для получения информации о сделке.
void TRANS2QUIK_TRADE_STATUS_CALLBACK (long nMode, double dNumber, double dOrderNum, LPSTR lpstrClassCode, LPSTR lpstrSecCode, double dPrice, long nQty, double dValue, long nIsSell, long nTradeDescriptor)

nMode Тип: Long. Признак того, идет ли начальное получение сделок или нет, возможные значения: «0» – новая сделка, «1» - идет начальное получение сделок, «2» – получена последняя сделка из начальной рассылки 
dNumber Тип: Double. Номер сделки 
dOrderNum Тип: Double. Номер заявки, породившей сделку 
lpstrClassCode Тип: указатель на переменную типа Строка. Код класса 
lpstrSecCode Тип: указатель на переменную типа Строка. Код бумаги 
dPrice Тип: Double. Цена сделки 
nQty Тип: Long. Количество сделки 
dValue Тип: Double. Объем сделки
nIsSell Тип: Long. Направление сделки: «0» еcли «Покупка», иначе «Продажа» 
nTradeDescriptor Тип: Long. Дескриптор сделки, может использоваться для следующих специальных функций в функции обратного вызова:
*/
		void dllCallback_tradeStatus(int nMode, double trade_id, double sernoExchange, string classCode, string secCode,
									double priceFilled_forMarket_zeroForLimit, int qtyFilled_forMarket_zeroForLimit,
									double cost, int isSell, int tradeDescriptor) {
			string comment = Marshal.PtrToStringAnsi(Trans2Quik.TRADE_BROKERREF(tradeDescriptor));

			string nMode_asString = "";		//NEW";
			if (nMode != 0) nMode_asString = nMode == 1 ? "INIT_LIST_USE_ME " : "LAST_ORDER_UPDATE ";

			string msg_dupeIgnored = "";
			if (nMode != 0) {
				msg_dupeIgnored = "DUPE_IGNORED ";
				if (nMode != 2) msg_dupeIgnored = "IM_USEFUL_NYI";
				msg_dupeIgnored += " nMode[" + nMode + "]!=0 " + nMode_asString;
			}

			string msigHead = msg_dupeIgnored + nMode_asString + "[" + secCode + "][" + classCode + "]"
				+ " qnty[" + qtyFilled_forMarket_zeroForLimit + "]@[" + priceFilled_forMarket_zeroForLimit + "] comment[" + comment + "]"
				+ " sernoExchange[" + sernoExchange + "] trade_id[" + trade_id + "]";
			string msigTail = " tradeDescriptor[" + tradeDescriptor + "] cost[" + cost + "] isSell[" + isSell + "]";

			string msig = msigHead + this.Ident + "tradeStatus_callback(" + msigTail + ")";
			Assembler.SetThreadName(msig);

			string msg_tradeCommission = "";
			double tradePrice2				= double.NaN;
			double tradeTradeSysCommission	= double.NaN;
			double tradeTScommission		= double.NaN;
			DateTime tradeDate				= DateTime.MinValue;
			try {
				tradePrice2 = Trans2Quik.TRADE_PRICE2(tradeDescriptor);
				tradeTradeSysCommission = Trans2Quik.TRADE_TRADING_SYSTEM_COMMISSION(tradeDescriptor);
				tradeTScommission = Trans2Quik.TRADE_TS_COMMISSION(tradeDescriptor);
				int date = Trans2Quik.TRADE_DATE(tradeDescriptor);
				int time = Trans2Quik.TRADE_TIME(tradeDescriptor);
				int year, month, day;
				int hour, min, sec;
				year = date / 10000;
				month = (day = date - year * 10000) / 100;
				day -= month * 100;
				hour = time / 10000;
				min = (sec = time - hour * 10000) / 100;
				sec -= min * 100;
				tradeDate = new DateTime(year, month, day, hour, min, sec);

				msg_tradeCommission = "tradeTradeSysCommission[" + tradeTradeSysCommission + "] tradeTScommission[" + tradeTScommission + "]"
					+ "tradePrice2[" + tradePrice2 + "] tradeDate[" + tradeDate + "]";
		    } catch (Exception ex) {
				msg_tradeCommission = " CANT_EXTRACT_Commission__TRY_MOVE_TO_orderStatus_callback() ex[" + ex.Message + "]";
		        //Assembler.PopupException(msg_tradeCommission + msig, ex);
		    }


			string msg_findingOrder = "";

			Order orderExecuted = null;
			OrderProcessorDataSnapshot snap = this.quikBroker.OrderProcessor.DataSnapshot;
			OrderLane laneToSearch = nMode == 0 ? snap.OrdersPending : snap.OrdersAll;	// you may want to wrap this method in lock(){} <= despite all Lanes are sync'ed, two async callbacks may not find the order while moving
			OrderLane	suggestedLane = null;
			string		suggestion = "PASS_suggestLane=TRUE";
			try {
				orderExecuted = laneToSearch.ScanRecent_forSernoExchange((long)sernoExchange, out suggestedLane, out suggestion);
			} catch (Exception ex) {
				msg_findingOrder = " sernoExchange_NOT_FOUND_IN_PENDINGS ex[" + ex.Message + "]"
						+ " suggestedLane[" + suggestedLane + "] suggestion[" + suggestion + "]";
		        //Assembler.PopupException(msg_findingOrder + msig, ex);
		    }

			if (orderExecuted == null) {
				Assembler.PopupException(msigHead + msg_findingOrder + msg_tradeCommission, null, false);
				return;
			}

			OrderStateMessage osm = new OrderStateMessage(orderExecuted, OrderState._TradeStatus, msigHead + msg_tradeCommission);
			orderExecuted.AppendMessageSynchronized(osm);

			if (nMode != 0) return;

			this.quikBroker.CallbackFromQuikDll_TradeState((long)sernoExchange, tradeDate, 
				classCode, secCode, priceFilled_forMarket_zeroForLimit, qtyFilled_forMarket_zeroForLimit,
				tradePrice2, tradeTradeSysCommission, tradeTScommission);
		}

/*Функция TRANS2QUIK_ORDER_STATUS_CALLBACK
Функция обратного вызова для получения информации о параметрах заявки.
void TRANS2QUIK_ORDER_STATUS_CALLBACK (long nMode, DWORD dwTransID, double dNumber, LPSTR lpstrClassCode, LPSTR lpstrSecCode, double dPrice, long nBalance, double dValue, long nIsSell, long nStatus, long nOrderDescriptor)

Параметр Описание 
nMode Тип: Long. Признак того, идет ли начальное получение заявок или нет, возможные значения: «0» – новая заявка, «1» - идет начальное получение заявок, «2» – получена последняя заявка из начальной рассылки 
dwTransID Тип: Long. TransID транзакции, породившей заявку. Имеет значение «0», если заявка не была порождена транзакцией из файла, либо если TransID неизвестен 
dNumber Тип: Double. Номер заявки 
lpstrClassCode Тип: указатель на переменную типа Строка. Код класса 
lpstrSecCode Тип: указатель на переменную типа Строка. Код бумаги 
dPrice Тип: Double. Цена заявки 
nBalance Тип: Long. Неисполненный остаток заявки 
dValue Тип: Double. Объем заявки 
nIsSell Тип: Long. Направление заявки: «0» еcли «Покупка», иначе «Продажа» 
nStatus Тип: Long. Состояние исполнения заявки: Значение «1» соответствует состоянию «Активна», «2» - «Снята», иначе «Исполнена» 
nOrderDescriptor Тип: Long. Дескриптор заявки, может использоваться для следующих специальных функций в теле функции обратного вызова:
*/
		void dllCallback_orderStatus(int nMode, int transId, double sernoExchange, string classCode, string secCode,
									double priceFilled_forLimit_zerForMarket, int qtyLeftUnfilled_forLimit_zeroForMarket,
									double cost, int isSell, int status, int orderDescriptor) {

			//int filled = Trans2Quik.ORDER_QTY(orderDescriptor);	// filled[" + filled + "]
			//string msgParams = "price[" + priceFilled + "] status[" + status + "]"
			//    + " nMode[" + nMode + "] Guid[" + GUID + "] sernoExchange[" + sernoExchange + "]"
			//    + " classCode[" + classCode + "] secCode[" + secCode + "]"
			//    + " leftUnfilled[" + balance + "] msum[" + msum + "] isSell[" + isSell + "]";

			string nMode_asString = "";		//"NEW";
			if (nMode != 0) nMode_asString = nMode == 1 ? "INIT_LIST_USE_ME " : "LAST_ORDER_UPDATE ";

			string status_asString = "FILLED";
			if (status == 1 || status == 2) status_asString = status == 1 ? "PENDING" : "KILLED";

			string msg_dupeIgnored = "";
			if (nMode != 0) {
				msg_dupeIgnored = "DUPE_IGNORED ";
				if (nMode != 2) msg_dupeIgnored = "IM_USEFUL_NYI";
				msg_dupeIgnored += " nMode[" + nMode + "]!=0 " + nMode_asString;
			}

			string msigHead = msg_dupeIgnored + nMode_asString + status_asString + " [" + secCode + "][" + classCode + "]"
				+ " leftUnfilled[" + qtyLeftUnfilled_forLimit_zeroForMarket + "]@[" + priceFilled_forLimit_zerForMarket + "]"
				+ " sernoExchange[" + sernoExchange + "] transId[" + transId + "]";
			string msigTail = " cost[" + cost + "] isSell[" + isSell + "]";

			string msig = msigHead + this.Ident + "orderStatus_callback(" + msigTail + ")";
			Assembler.SetThreadName(msig);


			string msg_findingOrder = "";

			OrderProcessorDataSnapshot snap = this.quikBroker.OrderProcessor.DataSnapshot;
			//v1
			string logOrEmpty = "";
			Order order = snap.ScanRecent_forGUID(transId.ToString(), snap.LanesForCallbackOrderState, out logOrEmpty);
			//v2
			//Order order = snap.OrdersAll.ScanRecentForGUID(GUID.ToString());
			if (order == null) {
				msg_findingOrder += "NOT_FOUND_IN_LANES__LOOKING_IN_FILLED ";
				var hopefullyCemeteryHealthy = new System.Collections.Generic.List<OrderLane>(){
					snap.SuggestLaneByOrderState_nullUnsafe(OrderState.Filled)
				};
				order = snap.ScanRecent_forGUID(transId.ToString(), hopefullyCemeteryHealthy, out logOrEmpty);
			}

			OrderLane	suggestedLane = null;
			string		suggestion = "PASS_suggestLane=TRUE";

			if (order == null) {
				msg_findingOrder += "NOT_FOUND_IN_LANES__LOOKING_IN_ALL ";
				order = snap.OrdersAll.ScanRecent_forGuid(transId.ToString(), out suggestedLane, out suggestion);
			}
			if (order == null) {
				msg_findingOrder += "NOT_FOUND_BY_GUID[" + transId + "] [" + logOrEmpty + "]"
						+ " suggestedLane[" + suggestedLane + "] suggestion[" + suggestion + "]";
			} else {
				msg_findingOrder += "FOUND_BY_GUID[" + transId + "] [" + logOrEmpty + "] order[" + order + "]";
			}


			if (order == null) {
				Assembler.PopupException(msigHead + msg_dupeIgnored + msg_findingOrder, null, false);
				return;
			}

			OrderStateMessage osm = new OrderStateMessage(order, OrderState._OrderStatus, msigHead + msg_dupeIgnored + msg_findingOrder);
			order.AppendMessageSynchronized(osm);

			if (nMode != 0) return;

			OrderState newState_orderReceives = OrderState.Unknown;
			int qtyFilled_forLimit_zeroForMarket = (int) (order.QtyRequested - (double)qtyLeftUnfilled_forLimit_zeroForMarket);
			switch (status) {
				case 1:		//PENDING	Значение «1» соответствует состоянию «Активна»
					newState_orderReceives = OrderState.WaitingBrokerFill;
					break;

				case 2:		//KILLED	«2» - «Снята»
					bool killersJob = order.State == OrderState.VictimsBulletFlying;
					if (killersJob) {
						newState_orderReceives = OrderState.VictimKilled;
					} else {
						newState_orderReceives = OrderState.Rejected;		// state of a victim must be Killed - before you said Rejected; TradeStatus
						string msg = "STUCK_ON_THE_CHART?REMOVE_ORDER_FROM_PEDINIGS " + transId.ToString();
						Assembler.PopupException(msg, null, false);
						//newOrderStateReceived = OrderState._OrderStatus;
					}
					priceFilled_forLimit_zerForMarket = 0;
					break;

				default:	//FILLED	иначе «Исполнена»
					if (qtyLeftUnfilled_forLimit_zeroForMarket > 0) {
						newState_orderReceives = OrderState.FilledPartially;
					} else {
						newState_orderReceives = OrderState.Filled;
					}
					break;
			}

			this.quikBroker.CallbackFromQuikDll_OrderState(newState_orderReceives, transId.ToString(),
						(long)sernoExchange, classCode, secCode, priceFilled_forLimit_zerForMarket, qtyFilled_forLimit_zeroForMarket);
		}

/* Функция TRANS2QUIK_TRANSACTIONS_REPLY_CALLBACK
Описание прототипа функции обратного вызова для обработки полученной информации об отправленной транзакции.
void TRANS2QUIK_TRANSACTION_REPLY_CALLBACK (long nTransactionResult, long nTransactionExtendedErrorCode, long nTransactionReplyCode, DWORD dwTransId, double dOrderNum, LPSTR lpstrTransactionReplyMessage)

Параметр Описание 
nTransactionResult Тип: Long. Возвращаемое число может принимать следующие значения:

TRANS2QUIK_SUCCESS – транзакция передана успешно, 
TRANS2QUIK_DLL_NOT_CONNECTED – отсутствует соединение между библиотекой Trans2QUIK.dll и терминалом QUIK, 
TRANS2QUIK_QUIK_NOT_CONNECTED – отсутствует соединение между терминалом QUIK и сервером, 
TRANS2QUIK_FAILED – транзакцию передать не удалось. В этом случае в переменную pnExtendedErrorCode может передаваться дополнительный код ошибки
 
nTransactionExtendedErrorCode Тип: Long. В случае возникновения проблемы при выходе из функции обратного вызова в переменную может быть помещен расширенный код ошибки 
nTransactionReplyCode Тип: Long. Указатель для получения статуса выполнения транзакции. Значения статусов те же самые, что и при подаче заявок через файл 
dwTransId Тип: Long. Содержимое параметра TransId, который получила зарегистрированная транзакция 
dOrderNum Тип: Double. Номер заявки, присвоенный торговой системой в результате выполнения транзакции 
lpstrTransactionReplyMessage Тип: указатель на переменную типа Строка. Сообщение от торговой системы или сервера QUIK 
*/
		void dllCallback_transactionReply(Trans2Quik.Result transResult, int err, int replyCode, int trans_id, double sernoExchange, string msgQuik) {
			string msg_transactionState = "";
			OrderState newState = OrderState.Unknown;
			if (transResult == Trans2Quik.Result.SUCCESS && replyCode == 3) {
				msg_transactionState = "TRANSACTION_SENT_OK ";	//: [" + msgQuik + "]";
				newState = OrderState.Submitted;
			} else {
				msg_transactionState = "TRANSACTION_FAILED ";	 //: [" + msgQuik + "] r[" + r + "]  err[" + err + "] rc[" + rc + "]";
				newState = OrderState.ErrorSubmitting_BrokerTerminalDisconnected;
			}

			string msigHead = msg_transactionState + transResult + " msgQuik[" + msgQuik + "] sernoExchange[" + sernoExchange + "]";
			string msigTail = " transId[" + trans_id + "] err[" + err + "] replyCode?[" + replyCode + "]";
			string msig = this.Ident + "transactionReply_callback(" + msigTail + ")";

			Assembler.SetThreadName(msig);


			string msg_findingOrder = "";

			OrderLaneByState orders = this.quikBroker.OrderProcessor.DataSnapshot.OrdersPending;

			OrderLane	suggestedLane = null;
			string		suggestion = "PASS_suggestLane=TRUE";
			Order orderSubmitting = orders.ScanRecent_forGuid(trans_id.ToString(), out suggestedLane, out suggestion);
			if (orderSubmitting == null) {
				msg_findingOrder = "NOT_FOUND Guid[" + trans_id + "] ; orderSernos=[" + orders.SessionSernosAsString + "] Count=[" + orders.Count + "]"
						+ " suggestedLane[" + suggestedLane + "] suggestion[" + suggestion + "]";
				Assembler.PopupException(msigHead + msg_findingOrder, null, false);
				return;
			}

			string msg = "";

			long sernoExchange_asLong = (long)sernoExchange;
			if (orderSubmitting.SernoExchange != sernoExchange_asLong) {
				msg = " ASSIGNED_SernoExchange[" + sernoExchange_asLong + "] was[" + orderSubmitting.SernoExchange + "]";
				orderSubmitting.SernoExchange  = sernoExchange_asLong;
			}

			OrderStateMessage osm = new OrderStateMessage(orderSubmitting, OrderState._TransactionStatus, msigHead + msg_findingOrder);
			orderSubmitting.AppendMessageSynchronized(osm);

			if (orderSubmitting.State == OrderState.WaitingBrokerFill && orderSubmitting.IsVictim) {
				string msg_dontPostProcess = "SUBMITTED_TRANSACTION_FOR_ME__BUT_LOGGING_TO_MY_KILLER" + msigHead + msg_findingOrder;
				orderSubmitting.AppendMessage(msg_dontPostProcess);

				Order killer = orderSubmitting.KillerOrder;
				OrderStateMessage newOrderState = new OrderStateMessage(killer, newState, msigHead + msg_findingOrder);
				this.quikBroker.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderState);

				return;
			}

			if (orderSubmitting.State != newState) {
				string todo = "DELETE_ME_IF_BREAKPOINT_HERE_WILL_BE_NEVER_HIT__MOST_LIKELY";
				OrderStateMessage newOrderState = new OrderStateMessage(orderSubmitting, newState, msigHead + msg_findingOrder);
				this.quikBroker.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(newOrderState);
			}

			// IT'S_NOT_YOUR_BUSINESS_HERE BULLETS_FLYING_ARE_SET_IN_Emit_killOrderPending_usingKiller();
			//if (orderSubmitting.IsKiller == false) return;
			//Order orderKiller = orderSubmitting;
			//if (orderKiller.VictimToBeKilled == null) {
			//    string msg1 = "";
			//    var omsg1 = new OrderStateMessage(orderKiller, OrderState.KillerPreSubmit, "KILLING_HOPE[" + orderKiller + "]");
			//    orderKiller.AppendMessage(msg1);
			//    //Assembler.PopupException(msg);
			//}
			//var omsg = new OrderStateMessage(orderKiller, OrderState.KillerSubmitting, "KILLING_HOPE[" + orderKiller + "]");
			//this.quikBroker.OrderProcessor.BrokerCallback_orderStateUpdate_mustBeDifferent_postProcess(omsg);
			//this.quikBroker.OrderProcessor.BrokerCallback_pendingKilled_withKiller_postProcess_removeAlertsPending_fromExecutorDataSnapshot(orderKiller, msig);
			//Order orderVictim = orderSubmitting.VictimToBeKilled;
			////orderVictim.AppendMessage
		}
	}
}
