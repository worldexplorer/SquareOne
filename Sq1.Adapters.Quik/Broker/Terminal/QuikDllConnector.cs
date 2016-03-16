using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.Execution;
using Sq1.Core.DataTypes;

namespace Sq1.Adapters.Quik.Broker.Terminal {
	public sealed partial class QuikDllConnector {
					QuikBroker				quikBroker;
					static int				transId;
					int						anyCallback_error;
					StringBuilder			anyCallback_msg;
		public		string					DllName				{ get { return "TRANS2QUIK.dll"; } }
		public		string					DllUrl				{ get { return "http://www.quik.ru"; } }

					Dictionary<string, int>	symbolClassesSubscribed = new Dictionary<string, int>();
		public		string					SymbolClassSubscribed_AsString { get {
			string ret = "";
			foreach (string symbolColonClass in this.symbolClassesSubscribed.Keys) {
				int subscribersCount = this.symbolClassesSubscribed[symbolColonClass];
				ret += symbolColonClass + ":" + subscribersCount + ",";
			}
			ret.TrimEnd(",".ToCharArray());
			return ret;
		} }

		public		string					Ident				{ get { return " //QuikDllConnector(" + this.quikBroker.DataSource.Name + " [" + this.quikBroker.Trans2QuikDllAbsPath + "])::"; } }


		QuikDllConnector() {
			this.anyCallback_msg			= new StringBuilder(256);
			this.symbolClassesSubscribed	= new Dictionary<string, int>();
		}
		public QuikDllConnector(QuikBroker quikBroker_passed) : this() {
			this.quikBroker				= quikBroker_passed;
		}

		public bool IsSubscribed(string symbol, string classCode) {
			string key = symbol + ":" + classCode;
			bool subscribed = this.symbolClassesSubscribed.ContainsKey(key);
			return subscribed;
		}
		public string Subscribe(string symbol, string classCode) {
			string msigHead = " (" + symbol + ":" + classCode + ") ";
			string msigTail = this.Ident + ".Subscribe()";

			string ret = "";
			if (this.DllConnected == false) {
				ret += " I_REFUSE_TO_SUBSCRIBE__DLL_MUST_BE_CONNECTED_FIRST " + msigHead + msigTail;
				Assembler.PopupException(ret);
				//BrokerQuik.ExceptionDialog.PopupException(new Exception(ret));
				throw new Exception(ret);
			}

			string key = symbol + ":" + classCode;
			if (this.symbolClassesSubscribed.ContainsKey(key)) {
				this.symbolClassesSubscribed[key]++;
				ret += " WAS_ALREADY_SUBSCRIBED_BEFORE now" + msigHead + ".Count=" + this.symbolClassesSubscribed[key];
				Assembler.PopupException(ret, null, false);
				return ret;
			}

			Trans2Quik.Result status_subscribeOrders = Trans2Quik.SUBSCRIBE_ORDERS(classCode, symbol);
			if (status_subscribeOrders != Trans2Quik.Result.SUCCESS) {
				ret += " FAILED_TO_SUBSCRIBE_ORDERS [" + status_subscribeOrders + "]" + msigHead;
				Assembler.PopupException(ret);
				throw new Exception(ret);
			} else {
				ret += " SUBSCRIBED_ORDERS";
				//Assembler.PopupException(ret + msigHead, null, false);
			}

			Trans2Quik.Result status_startOrders = Trans2Quik.START_ORDERS(dllCallback_orderStatus);
			if (status_startOrders != Trans2Quik.Result.SUCCESS) {
				 ret += " FAILED_TO_START_ORDERS [" + status_startOrders + "]" + msigHead;
				Assembler.PopupException(ret);
				throw new Exception(ret);
			} else {
				ret += " STARTED_ORDERS";
				//Assembler.PopupException(ret + msigHead, null, false);
			}

			Trans2Quik.Result status_subscribeTrades = Trans2Quik.SUBSCRIBE_TRADES(classCode, symbol);
			if (status_subscribeTrades != Trans2Quik.Result.SUCCESS) {
				ret += " FAILED_TO_SUBSCRIBE_TRADES [" + status_subscribeTrades + "]" + msigHead;
				Assembler.PopupException(ret);
				throw new Exception(ret);
			} else {
				ret += " SUBSCRIBED_TRADES";
				//Assembler.PopupException(ret + msigHead, null, false);
			}

			Trans2Quik.Result status_startTrades = Trans2Quik.START_TRADES(dllCallback_tradeStatus);
			if (status_startTrades != Trans2Quik.Result.SUCCESS) {
				ret += " FAILED_TO_START_TRADES [" + status_startTrades + "]" + msigHead;;
				Assembler.PopupException(ret);
				throw new Exception(ret);
			} else {
				ret += " STARTED_TRADES";
				//Assembler.PopupException(ret + msigHead, null, false);
			}

			this.symbolClassesSubscribed[key] = 1;
			//Assembler.PopupException("Subscribed to Quik Execution Callbacks for [" + key + "]", null, false);
	
			ConnectionState state = ConnectionState.Broker_Connected_SymbolSubscribed;
			this.quikBroker.CallbackFromQuikDll_ConnectionStateUpdated(state, "SUBSCRIBED_OK " + ret);

			return ret;
		}
		public void Unsubscribe(string symbol, string classCode) {
			string msigHead = " (" + symbol + ":" + classCode + ")";
			string msigTail = this.Ident + "Unsubscribe()";

			if (this.DllConnected == false) {
				string msg = "I_REFUSE_TO_UNSUBSCRIBE__DLL_MUST_BE_CONNECTED_FIRST" + msigHead + msigTail;
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}

			string key = symbol + ":" + classCode;
			if (this.symbolClassesSubscribed.ContainsKey(key) == false) {
					string msg = "I_REFUSE_TO_UNSUBSCRIBE__WAS_NOT_SUBSCRIBED_BEFORE" + msigHead;
					Assembler.PopupException(msg);
					throw new Exception(msg);
			}

			Trans2Quik.Result status_unsubscribeOrders = Trans2Quik.UNSUBSCRIBE_ORDERS();
			if (status_unsubscribeOrders != Trans2Quik.Result.SUCCESS) {
				string msg = "FAILED_TO_UNSUBSCRIBE_ORDERS [" + status_unsubscribeOrders + "] " + msigHead;
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			Trans2Quik.Result status_unsubscribeTrades = Trans2Quik.UNSUBSCRIBE_TRADES();
			if (status_unsubscribeTrades != Trans2Quik.Result.SUCCESS) {
				string msg = "FAILED_TO_UNSUBSCRIBE_TRADES [" + status_unsubscribeTrades + "] " + msigHead;
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			
			this.symbolClassesSubscribed[key]--;
		
			ConnectionState state = this.symbolClassesSubscribed[key] > 0
				? ConnectionState.Broker_Connected_SymbolUnsubscribed
				: ConnectionState.Broker_Connected_SymbolsUnsubscribedAll;
			this.quikBroker.CallbackFromQuikDll_ConnectionStateUpdated(state, "UNSUBSCRIBED_OK " + msigHead);
		}

	}
}
