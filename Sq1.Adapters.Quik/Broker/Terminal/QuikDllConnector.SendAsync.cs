using System;

using Sq1.Core;
using Sq1.Core.Execution;

namespace Sq1.Adapters.Quik.Broker.Terminal {
	public sealed partial class QuikDllConnector {
		public void OrderSubmit_sendTransaction_async(char opBuySell, char typeMarketLimitStop,
				string secCode, string classCode, double price, int quantity,
				string GUID, out int sernoSessionOut, out string msgSubmittedOut, out OrderState orderStateOut) {

			string msig = this.Ident + "OrderSubmit_sendTransaction_async(" + opBuySell + typeMarketLimitStop + quantity + "@" + price + ")";
			Assembler.SetThreadName(msig);

			sernoSessionOut = -1;
			if (this.checkIfConnectedBoth(out msgSubmittedOut, out orderStateOut, msig) == false) return;		// orderStateOut=OrderState.Unknown;

			if (this.IsSubscribed(secCode, classCode) == false) {
				try {
					string msg_subscribed = this.Subscribe(secCode, classCode);
					this.quikBroker.OrderProcessor.BrokerCallback_orderStateUpdate_byGuid_ifDifferent_dontPostProcess_appendPropagateMessage(GUID, OrderState.PreSubmit, msg_subscribed);
				} catch (Exception e) {
					msgSubmittedOut = msig + "Couldn't Subscribe(" + secCode + ", " + classCode + "), NOT going to Trans2Quik.SEND_ASYNC_TRANSACTION()";
					//this.BrokerQuik.StatusReporter.PopupException(new Exception(msgSubmittedOut, e));
					Assembler.PopupException(msgSubmittedOut, e);
					sernoSessionOut = -999;
					orderStateOut = OrderState.Error;
					return;
				}
			}

			string trans = this.getOrderCommand(opBuySell, typeMarketLimitStop, secCode, classCode, price, quantity, GUID, out sernoSessionOut);
			this.quikBroker.OrderProcessor.BrokerCallback_orderStateUpdate_byGuid_ifDifferent_dontPostProcess_appendPropagateMessage(GUID, OrderState.Submitting, trans);

			Trans2Quik.Result transSubmitted = Trans2Quik.SEND_ASYNC_TRANSACTION(trans, out this.anyCallback_error, this.anyCallback_msg, this.anyCallback_msg.Capacity);
			msgSubmittedOut = "transSubmitted[" + transSubmitted + "] callbackErrorMsg[" + this.anyCallback_msg + "] error[" + anyCallback_error + "]";
			if (transSubmitted == Trans2Quik.Result.SUCCESS) {
				orderStateOut = OrderState.Submitted;
			} else {
				orderStateOut = OrderState.ErrorSubmittingOrder;
			}
		}

		string getOrderCommand(char opBuySell, char typeMarketLimitStop,
				string secCode, string classCode, double price, int quantity, string GUID, out int SernoSessionOut) {
			// typeMarketLimitStop=='M' || typeMarketLimitStop='L'
			//string actionTypePriceStop = "ACTION=NEW_ORDER;PRICE=" + Price.GetRaw(price) + ";";
			//stopPrice += (opBuySell == 'S') ? SlippagesCommaSeparated : -SlippagesCommaSeparated;
			string actionTypePriceStop = "ACTION=NEW_ORDER;TYPE=" + typeMarketLimitStop + ";PRICE=" + price + ";";
			if (typeMarketLimitStop == 'S') {
				double stopPrice = price;
				actionTypePriceStop = "ACTION=NEW_STOP_ORDER;PRICE=" + price + ";STOPPRICE=" + stopPrice + ";";
			}
			SernoSessionOut = ++QuikDllConnector.transId;
			if (String.IsNullOrEmpty(classCode)) {
				int a = 1;
			}

			string trans = "OPERATION=" + opBuySell + ";"
				+ actionTypePriceStop
				+ "QUANTITY=" + quantity + ";"
				+ "SECCODE=" + secCode + ";" + "CLASSCODE=" + classCode + ";"
				+ "TRANS_ID=" + GUID + ";"
				//+ "ACCOUNT=" + "SPBFUTxxxxx" + ";"
				;
			trans += getAccountClientCode(classCode);
			return trans;
		}
		string getAccountClientCode(string classCode) {
			string ret = "";
			if (this.quikBroker == null) {
				Assembler.PopupException("can't set ACCOUNT=XXX and CLIENT_CODE=YYY: BrokerQuik=null");
			} else {
				if (this.quikBroker.AccountAutoPropagate == null) {
					Assembler.PopupException("can't set ACCOUNT=XXX: BrokerQuik[" + this.quikBroker + "].Account=null");
				} else {
					string account = this.quikBroker.AccountAutoPropagate.AccountNumber;
					if (classCode != "SPBFUT" && this.quikBroker.AccountMicexAutoPopulated != null) {
						account = this.quikBroker.AccountMicexAutoPopulated.AccountNumber;
					}
					ret += "ACCOUNT=" + account + ";";
				}
				ret += "CLIENT_CODE=" + this.quikBroker.QuikClientCode + ";";
			}
			return ret;
		}


		public void KillOrder_sendTransaction_async(string secCode, string classCode,
				string killerGUID, string victimGUID, long sernoExchangeVictim, bool victimWasStopOrder,
				out string msgSubmittedOut, out int sernoSessionOut, out OrderState orderStateOut) {

			string msig = this.Ident + ".KillOrder_sendTransaction_async("
				+ "killerGUID[" + killerGUID + "], victimGUID[" + victimGUID + "], sernoExchangeVictim[" + sernoExchangeVictim + "], victimWasStopOrder[" + victimWasStopOrder + "]"
				+ "): ";
			Assembler.SetThreadName(msig);

			sernoSessionOut = -1;
			if (this.checkIfConnectedBoth(out msgSubmittedOut, out orderStateOut, msig) == false) return;		// orderStateOut=OrderState.Unknown;

			if (this.IsSubscribed(secCode, classCode) == false) {
				string msg_subscribed = this.Subscribe(secCode, classCode);
				this.quikBroker.OrderProcessor.BrokerCallback_orderStateUpdate_byGuid_ifDifferent_dontPostProcess_appendPropagateMessage(killerGUID, OrderState.KillerPreSubmit, msg_subscribed);
			}

			string trans = this.getOrderKillCommand(secCode, classCode, victimWasStopOrder, victimGUID, sernoExchangeVictim, out sernoSessionOut);
			this.quikBroker.OrderProcessor.BrokerCallback_orderStateUpdate_byGuid_ifDifferent_dontPostProcess_appendPropagateMessage(killerGUID, OrderState.KillerSubmitting, trans);

			Trans2Quik.Result transSubmitted = Trans2Quik.SEND_ASYNC_TRANSACTION(trans, out anyCallback_error, this.anyCallback_msg, this.anyCallback_msg.Capacity);
			msgSubmittedOut = "transSubmitted[" + transSubmitted + "] callbackErrorMsg[" + this.anyCallback_msg + "] error[" + anyCallback_error + "]";
			if (transSubmitted == Trans2Quik.Result.SUCCESS) {
				orderStateOut = OrderState.KillerTransSubmittedOK;
			} else {
				orderStateOut = OrderState.ErrorSubmitting_KillerOrder;
			}
		}
		string getOrderKillCommand(string SecCode, string ClassCode, bool victimWasStopOrder,
				string GuidKiller, long SernoExchangeVictim, out int SernoSession) {
			SernoSession = ++QuikDllConnector.transId;
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

		bool checkIfConnectedBoth(out string msgSubmittedOut, out OrderState orderStateOut, string msig_invoker) {
			msgSubmittedOut = "CONTINUE__KOZ_RETURNED_TRUE_checkIfConnectedBoth_logOtherwize()";
			orderStateOut = OrderState.Unknown;

			bool ret = true;
			if (this.DllConnected && this.QuikConnected) return ret;

			if (this.DllConnected) {
				if (this.QuikConnected == false) {
					orderStateOut = OrderState.ErrorSubmitting_BrokerTerminalDisconnected;
					msgSubmittedOut = "QUIK_TERMINAL_DISCONNECTED " + msig_invoker;
					ret = false;
				}
			} else {
				orderStateOut = OrderState.ErrorSubmitting_BrokerDllDisconnected;
				msgSubmittedOut = "DLL_DISCONNECTED " + msig_invoker;
				ret = false;
			}
			return ret;
		}



		//public void KillAll_sendTransaction_async(string SecCode, string ClassCode, string GUID, out string msgSubmitted) {
		//	if (this.CanSend == false) {
		//		msgSubmitted = "DLL_OR_TERMINAL_DISCONNECTED " + this.Ident + "KillAll_sendTransaction_async()";
		//		return;
		//	}

		//	string ret = "";
		//	String trans = "";
		//	Trans2Quik.Result r;

		//	//transId++;
		//	//trans = ""
		//	//	+ "TRANS_ID=" + Order.newGUID() + ";"	//MddHHmmssfff
		//	//	+ "ACCOUNT=" + BrokerQuik.SettingsManager.Get(
		//	//		"QuikStreamingAdapter.QuikAccount", "SPBFUTxxxxx") + ";"
		//	//	//+ "CLIENT_CODE=" + BrokerQuik.SettingsManager.Get(
		//	//	//	"QuikStreamingAdapter.QuikClientCode", "") + ";"
		//	//	+ "SECCODE=" + SecCode + ";"
		//	//	+ "CLASSCODE=" + ClassCode + ";"
		//	//	+ "ACTION=KILL_ALL_FUTURES_ORDERS;";
		//	////quikTransactionsAttemptedLog.Put("KillAll(1/3) KILL_ALL_FUTURES_ORDERS=" + trans);
		//	//throw new Exception("NYI");
		//	//r = Trans2Quik.SEND_ASYNC_TRANSACTION(trans, out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
		//	//ret += this.Ident + " " + r + "	" + ((this.callbackErrorMsg.Length > 0) ? this.callbackErrorMsg.ToString() : " error[" + error + "]");

		//	//transId++;
		//	//trans = ""
		//	//	+ "TRANS_ID=" + Order.newGUID() + ";"
		//	//	+ "ACCOUNT=" + BrokerQuik.SettingsManager.Get(
		//	//		"QuikStreamingAdapter.QuikAccount", "SPBFUTxxxxx") + ";"
		//	//	//+ "CLIENT_CODE=" + BrokerQuik.SettingsManager.Get(
		//	//	//	"QuikStreamingAdapter.QuikClientCode", "") + ";"
		//	//	+ "SECCODE=" + SecCode + ";"
		//	//	+ "CLASSCODE=" + ClassCode + ";"
		//	//	+ "ACTION=KILL_ALL_STOP_ORDERS;";
		//	////quikTransactionsAttemptedLog.Put("KillAll(2/3) KILL_ALL_STOP_ORDERS=" + trans);
		//	//r = Trans2Quik.SEND_ASYNC_TRANSACTION(trans, out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
		//	//ret += this.Ident + " " + r + "	" + ((this.callbackErrorMsg.Length > 0) ? this.callbackErrorMsg.ToString() : " error[" + error + "]");

		//	//transId++;
		//	//trans = ""
		//	//	+ "TRANS_ID=" + Order.newGUID() + ";"
		//	//	+ "ACCOUNT=" + BrokerQuik.SettingsManager.Get(
		//	//		"QuikStreamingAdapter.QuikAccount", "SPBFUTxxxxx") + ";"
		//	//	//+ "CLIENT_CODE=" + BrokerQuik.SettingsManager.Get(
		//	//	//	"QuikStreamingAdapter.QuikClientCode", "") + ";"
		//	//	+ "SECCODE=" + SecCode + ";"
		//	//	+ "CLASSCODE=" + ClassCode + ";"
		//	//	+ "ACTION=KILL_ALL_ORDERS;";
		//	////quikTransactionsAttemptedLog.Put("KillAll(3/3) KILL_ALL_ORDERS=" + trans);
		//	//r = Trans2Quik.SEND_ASYNC_TRANSACTION(trans, out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
		//	//ret += this.Ident + " " + r + "	" + ((this.callbackErrorMsg.Length > 0) ? this.callbackErrorMsg.ToString() : " error[" + error + "]");

		//	msgSubmitted = (ret != "") ? ret : null;
		//}
	}
}
