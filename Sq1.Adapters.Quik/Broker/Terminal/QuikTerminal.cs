using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.Execution;
using Sq1.Core.DataTypes;

namespace Sq1.Adapters.Quik.Broker.Terminal {
	public class QuikTerminal {
		protected QuikBroker BrokerQuik;
		protected static int transId;
		protected int error;
		protected StringBuilder callbackErrorMsg;
		int connectionAttempts = 0;
		bool stopTryingToConnectDll;
		object tryingToConnectDllLock;
		Timer connectionTimer;
		protected bool DllConnected;
		protected string CurrentStatus = "1/2 DLL not connected, 2/2 No Symbols subscribed";
		public virtual string DllName { get { return "TRANS2QUIK.DLL"; } }
		public QuikTerminal() {
			throw new Exception("QuikTerminal doesn't support default constructor, use QuikTerminal(BrokerQuik)");
		}
		public QuikTerminal(QuikBroker BrokerQuik) {
			tryingToConnectDllLock = new Object();
			callbackErrorMsg = new StringBuilder(256);
			this.BrokerQuik = BrokerQuik;
			this.connectionTimer = new Timer(this.tryConnectPeriodical);
		}
		public virtual void ConnectDll() {
			this.stopTryingToConnectDll = false;
			connectionTimer.Change(0, BrokerQuik.ReconnectTimeoutMillis);
		}
		public virtual void DisconnectDll() {
			this.stopTryingToConnectDll = true;
			connectionTimer.Change(Timeout.Infinite, Timeout.Infinite);
			if (DllConnected) {
				Trans2Quik.DISCONNECT(out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
				DllConnected = false;
			}
		}
		void tryConnectPeriodical(Object state) {
			lock (tryingToConnectDllLock) {
				if (connectionAttempts == 0) Thread.Sleep(3000);
				connectionAttempts++;
				//if (StatusReporter == null) {
				//	connectionTimer.Change(0, BrokerQuik.ReconnectTimeoutMillis);
				//	return;
				//}
				if (this.stopTryingToConnectDll) {
					connectionTimer.Change(Timeout.Infinite, Timeout.Infinite);
					return;
				}
				CurrentStatus = "0/2: DLL not connected, No Symbols subscribed";
				Trans2Quik.Result ret;
				string msg;
				try {
					if (SymbolClassSubscribers == null) {
						SymbolClassSubscribers = new Dictionary<string, int>();
					}
					if (SymbolClassSubscribers.Count > 0) {
						Assembler.PopupException("Clearing SymbolClassSubscribers.Count=" + SymbolClassSubscribers.Count + "; most likely QUIK got disconnected");
						SymbolClassSubscribers.Clear();
					}

					ret = Trans2Quik.SET_CONNECTION_STATUS_CALLBACK(this.callbackConnectionStatus, out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
					if (ret != Trans2Quik.Result.SUCCESS) {
						BrokerQuik.callbackTerminalConnectionStateUpdated(ConnectionState.Unknown, this.callbackErrorMsg.ToString());
						msg = "1/5 FAILED: SET_CONNECTION_STATUS_CALLBACK(): error[" + error + "][" + callbackErrorMsg + "] ret[" + ret + "] != SUCCESS";
						Assembler.PopupException(msg);
						//throw new Exception(msg);
						return;
					}

					ret = Trans2Quik.SET_TRANSACTIONS_REPLY_CALLBACK(this.CallbackTransactionReply, out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
					if (ret != Trans2Quik.Result.SUCCESS) {
						BrokerQuik.callbackTerminalConnectionStateUpdated(ConnectionState.Unknown, this.callbackErrorMsg.ToString());
						msg = "2/5 FAILED: SET_TRANSACTIONS_REPLY_CALLBACK(): error[" + error + "][" + callbackErrorMsg + "] ret[" + ret + "] != SUCCESS";
						Assembler.PopupException(msg);
						//throw new Exception(msg);
						return;
					}

					ret = Trans2Quik.CONNECT(BrokerQuik.QuikFolder, out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
					if (ret != Trans2Quik.Result.SUCCESS && ret != Trans2Quik.Result.ALREADY_CONNECTED_TO_QUIK) {
						BrokerQuik.callbackTerminalConnectionStateUpdated(ConnectionState.Unknown, this.callbackErrorMsg.ToString());
						msg = "3/5 FAILED: CONNECT(" + BrokerQuik.QuikFolder + "): error[" + error + "][" + callbackErrorMsg + "] ret[" + ret + "] != SUCCESS";
						Assembler.PopupException(msg);
						//throw new Exception(msg);
						return;
					}

					this.DllConnected = true;
					CurrentStatus = "1/2: DLL Connected, No Symbols subscribed";
					/*
					ret = Trans2Quik.UNSUBSCRIBE_ORDERS();
					if (ret != Trans2Quik.Result.SUCCESS) {
						BrokerQuik.callbackTerminalConnectionStateUpdated(QuikTerminalQuikConnectionState.DllConnected, "can not UNSUBSCRIBE_ORDERS()");
						msg = "4/5 FAILED: UNSUBSCRIBE_ORDERS() ret[" + ret + "] != SUCCESS";
						Assembler.PopupException(msg);
						//throw new Exception(msg);
						//return;
					}

					ret = Trans2Quik.UNSUBSCRIBE_TRADES();
					if (ret != Trans2Quik.Result.SUCCESS) {
						BrokerQuik.callbackTerminalConnectionStateUpdated(QuikTerminalQuikConnectionState.DllConnected, "can not UNSUBSCRIBE_TRADES()");
						msg = "5/5 FAILED: UNSUBSCRIBE_TRADES() ret[" + ret + "] != SUCCESS";
						Assembler.PopupException(msg);
						//throw new Exception(msg);
						//return;
					}
					*/
					connectionTimer.Change(Timeout.Infinite, Timeout.Infinite);
					msg = "#" + connectionAttempts + "//timeoutInfinite QuikTerminal(" + this.DllName + ") 2/2 " + ConnectionState.DllNotConnectedUnsubscribed;
					BrokerQuik.callbackTerminalConnectionStateUpdated(ConnectionState.DllNotConnectedUnsubscribed, msg);
					Assembler.PopupException(msg);
				} catch (Exception e) {
					BrokerQuik.callbackTerminalConnectionStateUpdated(ConnectionState.ErrorConnectingNoRetriesAnymore, e.Message);
					Assembler.PopupException("Should we set a limit/timeout on trials?", e);
					return;
				}
			}
		}

		protected Dictionary<string, int> SymbolClassSubscribers = new Dictionary<string, int>();
		protected string SymbolClassSubscribersAsString { get {
				string ret = "";
				foreach (string symbolColonClass in this.SymbolClassSubscribers.Keys) {
					int subscribersCount = this.SymbolClassSubscribers[symbolColonClass];
					ret += symbolColonClass + ":" + subscribersCount + ",";
				}
				ret.TrimEnd(",".ToCharArray());
				return ret;
			} }
		public bool IsSubscribed(string SecCode, string ClassCode) {
			string key = SecCode + ":" + ClassCode;
			bool subscribed = SymbolClassSubscribers.ContainsKey(key);
			return subscribed;
		}
		public void Subscribe(string SecCode, string ClassCode) {
			if (DllConnected == false) {
				string msg = "can not RegisterQuoteConsumer(" + SecCode + "," + ClassCode + "): DLL[" + DllName + "] must be connected first [" + BrokerQuik.QuikFolder + "]";
				Assembler.PopupException(msg);
				//BrokerQuik.ExceptionDialog.PopupException(new Exception(msg));
				throw new Exception(msg);
			}

			string key = SecCode + ":" + ClassCode;
			if (SymbolClassSubscribers.ContainsKey(key)) {
				SymbolClassSubscribers[key]++;
				Assembler.PopupException("Was already subscribed to Quik Execution Callbacks for [" + key + "].Count=" + SymbolClassSubscribers[key]);
				return;
			}

			Trans2Quik.Result ret = Trans2Quik.SUBSCRIBE_ORDERS(ClassCode, SecCode);
			if (ret != Trans2Quik.Result.SUCCESS) {
				string msg = "can not SUBSCRIBE_ORDERS(" + ClassCode + "," + SecCode + "): ret[" + ret + "] != SUCCESS";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			} else {
				string msg = "SUBSCRIBE_ORDERS(" + ClassCode + "," + SecCode + "): SUCCESS";
				Assembler.PopupException(msg);
			}

			ret = Trans2Quik.START_ORDERS(CallbackOrderStatus);
			if (ret != Trans2Quik.Result.SUCCESS) {
				string msg = "can not START_ORDERS(): ret" + ret + "] != SUCCESS";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			} else {
				string msg = "START_ORDERS(): SUCCESS";
				Assembler.PopupException(msg);
			}

			ret = Trans2Quik.SUBSCRIBE_TRADES(ClassCode, SecCode);
			if (ret != Trans2Quik.Result.SUCCESS) {
				string msg = "can not SUBSCRIBE_TRADES(" + ClassCode + "," + SecCode + "): ret[" + ret + "] != SUCCESS";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			} else {
				string msg = "SUBSCRIBE_TRADES(" + ClassCode + "," + SecCode + "): SUCCESS";
				Assembler.PopupException(msg);
			}

			ret = Trans2Quik.START_TRADES(CallbackTradeStatus);
			if (ret != Trans2Quik.Result.SUCCESS) {
				string msg = "can not START_TRADES(): ret[" + ret + "] != SUCCESS";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			} else {
				string msg = "START_TRADES(): SUCCESS";
				Assembler.PopupException(msg);
			}

			SymbolClassSubscribers[key] = 1;
			if (SymbolClassSubscribers.Count == 0) CurrentStatus = "";
			CurrentStatus = SymbolClassSubscribers.ToString();
			Assembler.PopupException("Subscribed to Quik Execution Callbacks for [" + key + "]");
	
			BrokerQuik.callbackTerminalConnectionStateUpdated(ConnectionState.SymbolSubscribed,
				"QuikTerminal(" + this.DllName + ") 2/2 " + ConnectionState.SymbolSubscribed +
					" for SecCode[" + SecCode + "] ClassCode[" + ClassCode + "]");
		}
		public void Unsubscribe(string SecCode, string ClassCode) {
			if (!DllConnected) {
				string msg = "can not Unsubscribe(" + SecCode + "," + ClassCode + "): DLL must be connected first";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}

			string key = SecCode + ":" + ClassCode;
			if (!SymbolClassSubscribers.ContainsKey(key)) {
					string msg = "can not Unsubscribe(" + SecCode + "," + ClassCode + "): was not subscribed before";
					Assembler.PopupException(msg);
					throw new Exception(msg);
			}
			Trans2Quik.Result ret = Trans2Quik.UNSUBSCRIBE_ORDERS();
			if (ret != Trans2Quik.Result.SUCCESS) {
				string msg = "can not UNSUBSCRIBE_ORDERS(): ret[" + ret + "] != SUCCESS for SecCode[" + SecCode + "] ClassCode[" + ClassCode + "]";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			ret = Trans2Quik.UNSUBSCRIBE_TRADES();
			if (ret != Trans2Quik.Result.SUCCESS) {
				string msg = "can not UNSUBSCRIBE_TRADES(): ret[" + ret + "] != SUCCESS for SecCode[" + SecCode + "] ClassCode[" + ClassCode + "]";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			
			SymbolClassSubscribers[key]--;
			CurrentStatus = SymbolClassSubscribers.ToString();
		
			ConnectionState state = (SymbolClassSubscribers[key] > 0)
				? ConnectionState.SymbolSubscribed : ConnectionState.SymbolUnsubscribed;
			BrokerQuik.callbackTerminalConnectionStateUpdated(state,
				"QuikTerminal(" + this.DllName + ") " + state + " for SecCode[" + SecCode + "] ClassCode[" + ClassCode + "]");
		}
		void callbackConnectionStatus(Trans2Quik.Result evnt, int err, string m) {
			switch (evnt) {
				case Trans2Quik.Result.QUIK_DISCONNECTED:
					connectionTimer.Change(0, BrokerQuik.ReconnectTimeoutMillis);
					Assembler.PopupException("CallbackConnectionStatus[" + evnt + "]  err[" + err + "] m[" + m + "]: connectionTimer.Change(0, " + BrokerQuik.ReconnectTimeoutMillis + ")");
					break;
				case Trans2Quik.Result.DLL_DISCONNECTED:
					DllConnected = false;
					connectionTimer.Change(0, BrokerQuik.ReconnectTimeoutMillis);
					Assembler.PopupException("CallbackConnectionStatus[" + evnt + "]  err[" + err + "] m[" + m + "]: connectionTimer.Change(0, " + BrokerQuik.ReconnectTimeoutMillis + ")");
					break;
				default:
					Assembler.PopupException("CallbackConnectionStatus[" + evnt + "]  err[" + err + "] m[" + m + "]: NO_HANDLER");
					break;
			}
		}
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
		protected void CallbackTradeStatus(int nMode, double trade_id, double SernoExchange,
				string classCode, string secCode, double price, int filled, double msum, int isSell, int tradeDescriptor) {
			string comment = Marshal.PtrToStringAnsi(Trans2Quik.TRADE_BROKERREF(tradeDescriptor));
			string msg = "QuikTerminal(" + this.DllName + ")::CallbackTradeStatus():"
				+ " nMode[" + nMode + "]  trade_id[" + trade_id + "] SernoExchange[" + SernoExchange + "]"
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
			Order orderExecuted = this.BrokerQuik.OrderProcessor.DataSnapshot.OrdersAll.ScanRecentForSernoExchange((long)SernoExchange);
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
			BrokerQuik.CallbackTradeStateReceivedQuik((long)SernoExchange, tradeDate, 
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
		protected virtual void CallbackOrderStatus(int nMode, int GUID, double SernoExchange,
				string classCode, string secCode, double priceFilled, int balance,
				double msum, int isSell, int status, int orderDescriptor) {

			//int filled = Trans2Quik.ORDER_QTY(orderDescriptor);	// filled[" + filled + "]
			string msgDebug = "price[" + priceFilled + "] status[" + status + "]"
				+ " nMode[" + nMode + "] Guid[" + GUID + "] SernoExchange[" + SernoExchange + "]"
				+ " classCode[" + classCode + "] secCode[" + secCode + "]"
				+ " balance[" + balance + "] msum[" + msum + "] isSell[" + isSell + "]"
				;
			string msig = " QuikTerminal(" + this.DllName + ").CallbackOrderStatus(" + msgDebug + ")";
			string msgError = "";

			OrderProcessorDataSnapshot snap = this.BrokerQuik.OrderProcessor.DataSnapshot;
			//v1
			string logOrEmpty = "";
			Order order = snap.ScanRecentForGUID(GUID.ToString(), snap.LanesForCallbackOrderState, out logOrEmpty);
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
			this.BrokerQuik.CallbackOrderStateReceivedQuik(newOrderStateReceived, GUID.ToString(),
						(long)SernoExchange, classCode, secCode, priceFilled, qtyFilled);
			if (newOrderStateReceived == OrderState.FilledPartially || newOrderStateReceived == OrderState.Filled) {
				//v1 this.BrokerQuik.OrderProcessor.PostProcessOrderState(order, priceFilled, qtyFilled);
				OrderStateMessage omsg = new OrderStateMessage(order, "BrokerQuik_UpdateOrderStateAndPostProcess:: " + newOrderStateReceived + "::" + this.DllName);
				this.BrokerQuik.OrderProcessor.UpdateOrderStateAndPostProcess(order, omsg, priceFilled, qtyFilled);
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
		protected void CallbackTransactionReply(Trans2Quik.Result r, int err, int rc, int GUID, double SernoExchange, string msgQuik) {
			string msg = "QuikTerminal(" + this.DllName + ")::CallbackTransactionReply() "
				+ "msgQuik[" + msgQuik + "]"
				+ " r[" + r + "]  err[" + err + "] rc[" + rc + "]"
				+ " Guid[" + GUID + "] SernoExchange[" + SernoExchange + "]"
				;
			var orders = BrokerQuik.OrderProcessor.DataSnapshot.OrdersPending;
			Order orderSubmitting = orders.ScanRecentForGUID(GUID.ToString());
			if (orderSubmitting == null) {
				msg += " Order not found Guid[" + GUID + "] ; orderSernos=["
					+ orders.SessionSernosAsString
					+ "] Count=[" + orders.SafeCopy.Count + "]";
				Assembler.PopupException(msg);
				return;
			}

			orderSubmitting.SernoExchange = (long)SernoExchange;
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
			BrokerQuik.OrderProcessor.UpdateOrderStateAndPostProcess(orderSubmitting, newOrderState);
		}
		public virtual void SendTransactionOrderAsync(char opBuySell, char typeMarketLimitStop,
				string SecCode, string ClassCode, double price, int quantity,
				string GUID, out int SernoSession, out string msgSubmittedOut, out OrderState orderStateOut) {

			string msig = "QuikTerminal(" + this.DllName + ").SendTransactionOrderAsync(" + opBuySell + typeMarketLimitStop + quantity + "@" + price + ")";
			try {
				if (Thread.CurrentThread.Name != msig) Thread.CurrentThread.Name = msig;
			} catch (Exception e) {
				//Assembler.PopupException("can not set Thread.CurrentThread.Name=[" + msig + "]", e);
			}

			/*if (!connected) {
				sernoSessionOut = 0;
				orderStatus = OrderStatus.Error;
				msgSubmittedOut = Name + "::sendTransactionOrder(): " + CurrentStatus;
				return;
			}*/
			if (!IsSubscribed(SecCode, ClassCode)) {
				try {
					Subscribe(SecCode, ClassCode);
				} catch (Exception e) {
					msgSubmittedOut = msig + "Couldn't Subscribe(" + SecCode + ", " + ClassCode + "), NOT going to Trans2Quik.SEND_ASYNC_TRANSACTION()";
					//this.BrokerQuik.StatusReporter.PopupException(new Exception(msgSubmittedOut, e));
					Assembler.PopupException(msgSubmittedOut, e);
					SernoSession = -999;
					orderStateOut = OrderState.Error;
					return;
				}
			}

			orderStateOut = OrderState.PreSubmit;
			string trans = this.getOrderCommand(opBuySell, typeMarketLimitStop, SecCode, ClassCode, price, quantity, GUID, out SernoSession);
			this.BrokerQuik.OrderProcessor.UpdateOrderStateByGuidNoPostProcess(GUID, orderStateOut, trans);

			Trans2Quik.Result r = Trans2Quik.SEND_ASYNC_TRANSACTION(trans, out this.error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
			msgSubmittedOut = "r[" + r + "] callbackErrorMsg[" + this.callbackErrorMsg + "] error[" + error + "]";
			if (r == Trans2Quik.Result.SUCCESS) {
				orderStateOut = OrderState.Submitted;
			} else {
				orderStateOut = OrderState.ErrorSubmittingBroker;
			}
		}
		protected string getOrderCommand(char opBuySell, char typeMarketLimitStop,
				string SecCode, string ClassCode, double price, int quantity, string GUID, out int SernoSession) {
			// typeMarketLimitStop=='M' || typeMarketLimitStop='L'
			//string actionTypePriceStop = "ACTION=NEW_ORDER;PRICE=" + Price.GetRaw(price) + ";";
			//stopPrice += (opBuySell == 'S') ? SlippagesCommaSeparated : -SlippagesCommaSeparated;
			string actionTypePriceStop = "ACTION=NEW_ORDER;TYPE=" + typeMarketLimitStop + ";PRICE=" + price + ";";
			if (typeMarketLimitStop == 'S') {
				double stopPrice = price;
				actionTypePriceStop = "ACTION=NEW_STOP_ORDER;PRICE=" + price + ";STOPPRICE=" + stopPrice + ";";
			}
			SernoSession = ++QuikTerminal.transId;
			if (String.IsNullOrEmpty(ClassCode)) {
				int a = 1;
			}

			string trans = "OPERATION=" + opBuySell + ";"
				+ actionTypePriceStop
				+ "QUANTITY=" + quantity + ";"
				+ "SECCODE=" + SecCode + ";" + "CLASSCODE=" + ClassCode + ";"
				+ "TRANS_ID=" + GUID + ";"
				//+ "ACCOUNT=" + "SPBFUTxxxxx" + ";"
				;
			trans += getAccountClientCode(ClassCode);
			return trans;
		}
		protected virtual string getAccountClientCode(string ClassCode) {
			string ret = "";
			if (BrokerQuik == null) {
				Assembler.PopupException("can't set ACCOUNT=XXX and CLIENT_CODE=YYY: BrokerQuik=null");
			} else {
				if (BrokerQuik.AccountAutoPropagate == null) {
					Assembler.PopupException("can't set ACCOUNT=XXX: BrokerQuik[" + BrokerQuik + "].Account=null");
				} else {
					string account = BrokerQuik.AccountAutoPropagate.AccountNumber;
					if (ClassCode != "SPBFUT" && BrokerQuik.AccountMicexAutoPopulated != null) {
						account = BrokerQuik.AccountMicexAutoPopulated.AccountNumber;
					}
					ret += "ACCOUNT=" + account + ";";
				}
				ret += "CLIENT_CODE=" + BrokerQuik.QuikClientCode + ";";
			}
			return ret;
		}
		public virtual void SendTransactionOrderKillAsync(string SecCode, string ClassCode,
				string KillerGUID, string VictimGUID, long SernoExchangeVictim, bool victimWasStopOrder,
				out string msgSubmitted, out int SernoSession, out OrderState orderState) {

			string msig = "QuikTerminal(" + this.DllName + ").SendTransactionOrderKillAsync("
				+ "KillerGUID[" + KillerGUID + "], VictimGUID[" + VictimGUID + "], SernoExchangeVictim[" + SernoExchangeVictim + "], victimWasStopOrder[" + victimWasStopOrder + "]"
				+ "): ";
			try {
				if (Thread.CurrentThread.Name != msig) Thread.CurrentThread.Name = msig;
			} catch (Exception e) {
				Assembler.PopupException("can not set Thread.CurrentThread.Name=[" + msig + "]", e);
			}

			/*if (!connected) {
				sernoSessionOut = 0;
				orderStatus = OrderStatus.Error;
				msgSubmittedOut = Name + "::sendTransactionOrder(): " + CurrentStatus;
				return;
			}*/
			if (!IsSubscribed(SecCode, ClassCode)) {
				try {
					Subscribe(SecCode, ClassCode);
				} catch (Exception e) {
					msgSubmitted = msig + "Couldn't Subscribe(" + SecCode + ", " + ClassCode + "), NOT going to Trans2Quik.SEND_ASYNC_TRANSACTION()";
					Assembler.PopupException(msgSubmitted, e);
					SernoSession = -999;
					orderState = OrderState.Error;
					return;
				}
			}

			string trans = this.getOrderKillCommand(SecCode, ClassCode, victimWasStopOrder, VictimGUID, SernoExchangeVictim, out SernoSession);
			this.BrokerQuik.OrderProcessor.UpdateOrderStateByGuidNoPostProcess(KillerGUID, OrderState.KillerSubmitting, trans);

			Trans2Quik.Result r = Trans2Quik.SEND_ASYNC_TRANSACTION(trans, out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
			msgSubmitted = msig + r + "    " + ((this.callbackErrorMsg.Length > 0) ? this.callbackErrorMsg.ToString() : " error[" + error + "]");
			if (r == Trans2Quik.Result.SUCCESS) {
				orderState = OrderState.KilledPending;
			} else {
				orderState = OrderState.Error;
			}
		}
		protected string getOrderKillCommand(string SecCode, string ClassCode, bool victimWasStopOrder,
				string GuidKiller, long SernoExchangeVictim, out int SernoSession) {
			SernoSession = ++QuikTerminal.transId;
			if (String.IsNullOrEmpty(ClassCode)) {
				int a = 1;
			}

			string actionKey = victimWasStopOrder
				? "ACTION=KILL_STOP_ORDER;STOP_ORDER_KEY=" + SernoExchangeVictim + ";"
				: "ACTION=KILL_ORDER;ORDER_KEY=" + SernoExchangeVictim + ";";
			string trans = "TRANS_ID=" + GuidKiller + ";"		//"MddHHmmssfff"
				+ actionKey
				+ "SECCODE=" + SecCode + ";" + "CLASSCODE=" + ClassCode + ";"
				;
			trans += getAccountClientCode(ClassCode);
			return trans;
		}
		public virtual void sendTransactionKillAll(string SecCode, string ClassCode, string GUID, out string msgSubmitted) {
			if (!DllConnected) {
				msgSubmitted = "QuikTerminal(" + this.DllName + ")::sendTransactionKillAll(): " + CurrentStatus;
				return;
			}

			string ret = "";
			String trans = "";
			Trans2Quik.Result r;

			//transId++;
			//trans = ""
			//	+ "TRANS_ID=" + Order.newGUID() + ";"	//MddHHmmssfff
			//	+ "ACCOUNT=" + BrokerQuik.SettingsManager.Get(
			//		"QuikStreamingAdapter.QuikAccount", "SPBFUTxxxxx") + ";"
			//	//+ "CLIENT_CODE=" + BrokerQuik.SettingsManager.Get(
			//	//	"QuikStreamingAdapter.QuikClientCode", "") + ";"
			//	+ "SECCODE=" + SecCode + ";"
			//	+ "CLASSCODE=" + ClassCode + ";"
			//	+ "ACTION=KILL_ALL_FUTURES_ORDERS;";
			////quikTransactionsAttemptedLog.Put("KillAll(1/3) KILL_ALL_FUTURES_ORDERS=" + trans);
			//throw new Exception("NYI");
			//r = Trans2Quik.SEND_ASYNC_TRANSACTION(trans, out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
			//ret += "QuikTerminal(" + this.DllName + "):: " + r + "    " + ((this.callbackErrorMsg.Length > 0) ? this.callbackErrorMsg.ToString() : " error[" + error + "]");

			//transId++;
			//trans = ""
			//	+ "TRANS_ID=" + Order.newGUID() + ";"
			//	+ "ACCOUNT=" + BrokerQuik.SettingsManager.Get(
			//		"QuikStreamingAdapter.QuikAccount", "SPBFUTxxxxx") + ";"
			//	//+ "CLIENT_CODE=" + BrokerQuik.SettingsManager.Get(
			//	//	"QuikStreamingAdapter.QuikClientCode", "") + ";"
			//	+ "SECCODE=" + SecCode + ";"
			//	+ "CLASSCODE=" + ClassCode + ";"
			//	+ "ACTION=KILL_ALL_STOP_ORDERS;";
			////quikTransactionsAttemptedLog.Put("KillAll(2/3) KILL_ALL_STOP_ORDERS=" + trans);
			//r = Trans2Quik.SEND_ASYNC_TRANSACTION(trans, out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
			//ret += "QuikTerminal(" + this.DllName + "):: " + r + "    " + ((this.callbackErrorMsg.Length > 0) ? this.callbackErrorMsg.ToString() : " error[" + error + "]");

			//transId++;
			//trans = ""
			//	+ "TRANS_ID=" + Order.newGUID() + ";"
			//	+ "ACCOUNT=" + BrokerQuik.SettingsManager.Get(
			//		"QuikStreamingAdapter.QuikAccount", "SPBFUTxxxxx") + ";"
			//	//+ "CLIENT_CODE=" + BrokerQuik.SettingsManager.Get(
			//	//	"QuikStreamingAdapter.QuikClientCode", "") + ";"
			//	+ "SECCODE=" + SecCode + ";"
			//	+ "CLASSCODE=" + ClassCode + ";"
			//	+ "ACTION=KILL_ALL_ORDERS;";
			////quikTransactionsAttemptedLog.Put("KillAll(3/3) KILL_ALL_ORDERS=" + trans);
			//r = Trans2Quik.SEND_ASYNC_TRANSACTION(trans, out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
			//ret += "QuikTerminal(" + this.DllName + "):: " + r + "    " + ((this.callbackErrorMsg.Length > 0) ? this.callbackErrorMsg.ToString() : " error[" + error + "]");

			msgSubmitted = (ret != "") ? ret : null;
		}
	}
}