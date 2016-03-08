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
nIsSell Тип: Long. Направление сделки: «0» еcли «Покупка», иначе «Продажа» 
dValue Тип: Double. Объем сделки
nTradeDescriptor Тип: Long. Дескриптор сделки, может использоваться для следующих специальных функций в функции обратного вызова:
*/
		void tradeStatus_callback(int nMode, double trade_id, double sernoExchange,
				string classCode, string secCode, double price, int filled, double msum, int isSell, int tradeDescriptor) {
			string comment = Marshal.PtrToStringAnsi(Trans2Quik.TRADE_BROKERREF(tradeDescriptor));
			string msg = "QuikDllConnector(" + this.DllName + ")::tradeStatus_callback():"
				+ " nMode[" + nMode + "]  trade_id[" + trade_id + "] sernoExchange[" + sernoExchange + "]"
				+ " classCode[" + classCode + "]  secCode[" + secCode + "] price[" + price + "] quantity[" + filled + "]"
				+ " msum[" + msum + "]  isSell[" + isSell + "] comment[" + comment + "] tradeDescriptor[" + tradeDescriptor + "]";
			if (nMode != 0) msg += "; ignoring nMode[" + nMode + "]!=0";

			double tradePrice2;
			double tradeTradeSysCommission;
			double tradeTScommission;
			DateTime tradeDate = DateTime.MinValue;
			if (tradeDescriptor == -111111) {
				tradePrice2 = Trans2Quik.TRADE_PRICE2(tradeDescriptor);
				tradeTradeSysCommission = Trans2Quik.TRADE_TRADING_SYSTEM_COMMISSION(tradeDescriptor);
				tradeTScommission = Trans2Quik.TRADE_TS_COMMISSION(tradeDescriptor);
				tradeDate = DateTime.Now;
			} else {
				tradePrice2 = 0;
				tradeTradeSysCommission = 0;
				tradeTScommission = 0;
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
			}

	
			string msg2 = "...tradePrice[" + tradePrice2 + "] tradeDate[" + tradeDate + "]"
				+ " tradeTradeSysCommission[" + tradeTradeSysCommission + "] tradeTScommission[" + tradeTScommission + "]";

			//Order orderExecuted = this.BrokerQuik.OrderProcessor.DataSnapshot.OrdersPending.FindBySernoExchange((long)SernoExchange);
			Order orderExecuted = this.quikBroker.OrderProcessor.DataSnapshot.OrdersAll.ScanRecentForSernoExchange((long)sernoExchange);
			if (orderExecuted == null) {
				msg += " orderExecuted=null";
			} else {
				msg += (orderExecuted == null) ? " orderExecuted=null" : " orderExecuted=[" + orderExecuted + "]";
				orderExecuted.AppendMessage(msg);
				orderExecuted.AppendMessage(msg2);
			}
			Assembler.PopupException(msg);
			Assembler.PopupException(msg2);

			if (nMode != 0) return; //&& ((comment != null && comment.EndsWith(cfg.FullProgName)) || cfg.u.AcceptAllTrades)

			if (orderExecuted == null) {
				//throw new Exception("CRITICAL!!! can't find order for SernoExchange=[" + SernoExchange + "]; " + msg);
				return;
			}

			//if (isSell != 0) quantity = -quantity;
			//BrokerQuik.PutOwnTrade(new OwnTrade(
			//	new DateTime(year, month, day, hour, min, sec),
			//	(long)SernoExchange, Price.GetInt(fillPrice), quantity));
			//	ThreadPool.QueueUserWorkItem(new WaitCallback(order.Alert.DataSource.BrokerAdapter.SubmitOrdersThreadEntry),
			//		new object[] { ordersFromAlerts });
			this.quikBroker.TradeState_callbackFromQuikDll((long)sernoExchange, tradeDate, 
				classCode, secCode, price, filled,
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
		void orderStatus_callback(int nMode, int GUID, double sernoExchange,
				string classCode, string secCode, double priceFilled, int balance,
				double msum, int isSell, int status, int orderDescriptor) {

			//int filled = Trans2Quik.ORDER_QTY(orderDescriptor);	// filled[" + filled + "]
			string msgDebug = "price[" + priceFilled + "] status[" + status + "]"
				+ " nMode[" + nMode + "] Guid[" + GUID + "] sernoExchange[" + sernoExchange + "]"
				+ " classCode[" + classCode + "] secCode[" + secCode + "]"
				+ " balance[" + balance + "] msum[" + msum + "] isSell[" + isSell + "]"
				;
			string msig = " QuikDllConnector(" + this.DllName + ").orderStatus_callback(" + msgDebug + ")";
			string msgError = "";

			OrderProcessorDataSnapshot snap = this.quikBroker.OrderProcessor.DataSnapshot;
			//v1
			string logOrEmpty = "";
			Order order = snap.ScanRecent_forGUID(GUID.ToString(), snap.LanesForCallbackOrderState, out logOrEmpty);
			//v2
			//Order order = snap.OrdersAll.ScanRecentForGUID(GUID.ToString());

			if (order == null) {
				msgError += "ORDER_NOT_FOUND_BY_GUID[" + GUID + "] [" + logOrEmpty + "] orderExecuted=[" + order + "]";
			} else {
				msgDebug += "ORDER_FOUND_BY_GUID[" + GUID + "] orderExecuted=[" + order + "]";
			}

			if (nMode == 1) {
				msgError = "IGNORING nMode[" + nMode + "]=1 " + msgDebug;
				Assembler.PopupException(msgError);
			}
			if (order == null) {
				Assembler.PopupException(msgError + msgDebug + msig);
				return;
			}
			if (string.IsNullOrEmpty(msgError) == false) {
				order.AppendMessage(msig + msgError);
			}
			if (nMode == 1) {
				return;
			}

			OrderState newOrderStateReceived = OrderState.Unknown;
			int qtyFilled = (int) (order.QtyRequested - (double)balance);
			switch (status) {
				case 1: //Значение «1» соответствует состоянию «Активна»
					newOrderStateReceived = OrderState.WaitingBrokerFill;
					//priceFilled = 0;
					break;
				case 2: //«2» - «Снята»
					//if (orderExecuted.State == OrderState.KillPending) {
					if (order.FindStateInOrderMessages(OrderState.KilledPending)) {
						newOrderStateReceived = OrderState.KillerDone;
					} else {
						// what was the state of a victim before you said Rejected? must be Killed!! TradeStatus!!! shit!
						newOrderStateReceived = OrderState.Rejected;
					}
					priceFilled = 0;
					break;
				default:	// иначе «Исполнена»
					if (balance > 0) {
						newOrderStateReceived = OrderState.FilledPartially;
					} else {
						newOrderStateReceived = OrderState.Filled;
					}
					break;
			}
			this.quikBroker.OrderState_callbackFromQuikDll(newOrderStateReceived, GUID.ToString(),
						(long)sernoExchange, classCode, secCode, priceFilled, qtyFilled);
			if (newOrderStateReceived == OrderState.FilledPartially || newOrderStateReceived == OrderState.Filled) {
				//v1 this.BrokerQuik.OrderProcessor.PostProcessOrderState(order, priceFilled, qtyFilled);
				OrderStateMessage omsg = new OrderStateMessage(order, "BrokerQuik_UpdateOrderState_postProcess:: " + newOrderStateReceived + "::" + this.DllName);
				this.quikBroker.OrderProcessor.UpdateOrderState_postProcess(order, omsg, priceFilled, qtyFilled);
			}
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
		void transactionReply_callback(Trans2Quik.Result r, int err, int rc, int GUID, double sernoExchange, string msgQuik) {
			string msg = "QuikDllConnector(" + this.DllName + ")::transactionReply_callback() "
				+ "msgQuik[" + msgQuik + "]"
				+ " r[" + r + "]  err[" + err + "] rc[" + rc + "]"
				+ " Guid[" + GUID + "] SernoExchange[" + sernoExchange + "]"
				;
			OrderLaneByState orders = this.quikBroker.OrderProcessor.DataSnapshot.OrdersPending;
			Order orderSubmitting = orders.ScanRecentForGUID(GUID.ToString());
			if (orderSubmitting == null) {
				msg += " Order not found Guid[" + GUID + "] ; orderSernos=["
					+ orders.SessionSernosAsString
					+ "] Count=[" + orders.SafeCopy.Count + "]";
				Assembler.PopupException(msg);
				return;
			}

			orderSubmitting.SernoExchange = (long)sernoExchange;
			OrderState newState = OrderState.Unknown;
			string msg2 = "";
			if (r == Trans2Quik.Result.SUCCESS && rc == 3) {
				msg2 = "TRANSACTION_SENT_OK: [" + msgQuik + "]";
				Assembler.PopupException(msg2 + msg);
				//quikTransactionsAttemptedLog.Put(msg);
				newState = OrderState.Submitted;
			} else {
				msg2 = "ERROR_SENDING_TRANSACTION: [" + msgQuik + "] r[" + r + "]  err[" + err + "] rc[" + rc + "]";
				Assembler.PopupException(msg2 + msg);
				newState = OrderState.ErrorSubmittingBroker;
			}
			OrderStateMessage newOrderState = new OrderStateMessage(orderSubmitting, newState, msg2);
			this.quikBroker.OrderProcessor.UpdateOrderState_postProcess(orderSubmitting, newOrderState);
		}
	}
}
