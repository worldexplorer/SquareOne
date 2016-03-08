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
					QuikBroker		quikBroker;
					static int		transId;
					int				error;
					StringBuilder	callbackErrorMsg;
		protected	string			CurrentStatus = "1/2 DLL not connected, 2/2 No Symbols subscribed";
		public		string			DllName			{ get { return "TRANS2QUIK.dll"; } }
		public		string			DllUrl			{ get { return "http://www.quik.ru"; } }

		QuikDllConnector() {
			this.callbackErrorMsg			= new StringBuilder(256);
			this.connectionTimer			= new Timer(this.tryConnectPeriodical);
			this.symbolClassesSubscribed	= new Dictionary<string, int>();
		}
		public QuikDllConnector(QuikBroker quikBroker_passed) : this() {
			this.quikBroker				= quikBroker_passed;
		}

		string errorAsString(int error) {
			return Enum.GetName(typeof(Trans2Quik.Result), error);
		}

				Dictionary<string, int> symbolClassesSubscribed = new Dictionary<string, int>();
		public	string SymbolClassSubscribed_AsString { get {
			string ret = "";
			foreach (string symbolColonClass in this.symbolClassesSubscribed.Keys) {
				int subscribersCount = this.symbolClassesSubscribed[symbolColonClass];
				ret += symbolColonClass + ":" + subscribersCount + ",";
			}
			ret.TrimEnd(",".ToCharArray());
			return ret;
		} }
		public bool IsSubscribed(string Symbol, string ClassCode) {
			string key = Symbol + ":" + ClassCode;
			bool subscribed = symbolClassesSubscribed.ContainsKey(key);
			return subscribed;
		}
		public void Subscribe(string Symbol, string ClassCode) {
			if (DllConnected == false) {
				string msg = "can not RegisterQuoteConsumer(" + Symbol + "," + ClassCode + "): DLL[" + DllName + "] must be connected first [" + this.quikBroker.QuikFolder + "]";
				Assembler.PopupException(msg);
				//BrokerQuik.ExceptionDialog.PopupException(new Exception(msg));
				throw new Exception(msg);
			}

			string key = Symbol + ":" + ClassCode;
			if (symbolClassesSubscribed.ContainsKey(key)) {
				this.symbolClassesSubscribed[key]++;
				Assembler.PopupException("Was already subscribed to Quik Execution Callbacks for [" + key + "].Count=" + symbolClassesSubscribed[key]);
				return;
			}

			Trans2Quik.Result ret = Trans2Quik.SUBSCRIBE_ORDERS(ClassCode, Symbol);
			if (ret != Trans2Quik.Result.SUCCESS) {
				string msg = "can not SUBSCRIBE_ORDERS(" + ClassCode + "," + Symbol + "): ret[" + ret + "] != SUCCESS";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			} else {
				string msg = "SUBSCRIBE_ORDERS(" + ClassCode + "," + Symbol + "): SUCCESS";
				Assembler.PopupException(msg);
			}

			ret = Trans2Quik.START_ORDERS(orderStatus_callback);
			if (ret != Trans2Quik.Result.SUCCESS) {
				string msg = "can not START_ORDERS(): ret" + ret + "] != SUCCESS";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			} else {
				string msg = "START_ORDERS(): SUCCESS";
				Assembler.PopupException(msg);
			}

			ret = Trans2Quik.SUBSCRIBE_TRADES(ClassCode, Symbol);
			if (ret != Trans2Quik.Result.SUCCESS) {
				string msg = "can not SUBSCRIBE_TRADES(" + ClassCode + "," + Symbol + "): ret[" + ret + "] != SUCCESS";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			} else {
				string msg = "SUBSCRIBE_TRADES(" + ClassCode + "," + Symbol + "): SUCCESS";
				Assembler.PopupException(msg);
			}

			ret = Trans2Quik.START_TRADES(tradeStatus_callback);
			if (ret != Trans2Quik.Result.SUCCESS) {
				string msg = "can not START_TRADES(): ret[" + ret + "] != SUCCESS";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			} else {
				string msg = "START_TRADES(): SUCCESS";
				Assembler.PopupException(msg);
			}

			this.symbolClassesSubscribed[key] = 1;
			if (symbolClassesSubscribed.Count == 0) CurrentStatus = "";
			this.CurrentStatus = symbolClassesSubscribed.ToString();
			Assembler.PopupException("Subscribed to Quik Execution Callbacks for [" + key + "]");
	
			this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.SymbolSubscribed,
				"QuikDllConnector(" + this.DllName + ") 2/2 " + ConnectionState.SymbolSubscribed +
					" for SecCode[" + Symbol + "] ClassCode[" + ClassCode + "]");
		}
		public void Unsubscribe(string Symbol, string ClassCode) {
			if (!DllConnected) {
				string msg = "can not Unsubscribe(" + Symbol + "," + ClassCode + "): DLL must be connected first";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}

			string key = Symbol + ":" + ClassCode;
			if (!symbolClassesSubscribed.ContainsKey(key)) {
					string msg = "can not Unsubscribe(" + Symbol + "," + ClassCode + "): was not subscribed before";
					Assembler.PopupException(msg);
					throw new Exception(msg);
			}
			Trans2Quik.Result ret = Trans2Quik.UNSUBSCRIBE_ORDERS();
			if (ret != Trans2Quik.Result.SUCCESS) {
				string msg = "can not UNSUBSCRIBE_ORDERS(): ret[" + ret + "] != SUCCESS for SecCode[" + Symbol + "] ClassCode[" + ClassCode + "]";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			ret = Trans2Quik.UNSUBSCRIBE_TRADES();
			if (ret != Trans2Quik.Result.SUCCESS) {
				string msg = "can not UNSUBSCRIBE_TRADES(): ret[" + ret + "] != SUCCESS for SecCode[" + Symbol + "] ClassCode[" + ClassCode + "]";
				Assembler.PopupException(msg);
				throw new Exception(msg);
			}
			
			this.symbolClassesSubscribed[key]--;
			this.CurrentStatus = symbolClassesSubscribed.ToString();
		
			ConnectionState state = (symbolClassesSubscribed[key] > 0)
				? ConnectionState.SymbolSubscribed : ConnectionState.SymbolUnsubscribed;
			this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(state,
				"QuikDllConnector(" + this.DllName + ") " + state + " for SecCode[" + Symbol + "] ClassCode[" + ClassCode + "]");
		}

	}
}
