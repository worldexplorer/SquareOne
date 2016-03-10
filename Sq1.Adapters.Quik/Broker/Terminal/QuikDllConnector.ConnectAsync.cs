using System;
using System.Threading;

using Sq1.Core;
using Sq1.Core.DataTypes;

namespace Sq1.Adapters.Quik.Broker.Terminal {
	public sealed partial class QuikDllConnector {
					int				connectionAttempts = 0;
					bool			stopTryingToConnect;
					Timer			connectionTimer;
		public		bool			DllConnected	{ get; private set; }

		public void ConnectDll() {
			this.stopTryingToConnect = false;
			this.connectionTimer.Change(0, this.quikBroker.ReconnectTimeoutMillis);
		}
		public void DisconnectDll() {
			this.stopTryingToConnect = true;
			this.connectionTimer.Change(Timeout.Infinite, Timeout.Infinite);
			if (this.DllConnected == false) {
				string msg = "I_REFUSE_TO_DISCONNECT ALREADY_DISCONNECTED";
				Assembler.PopupException(msg, null, false);
			}
			Trans2Quik.DISCONNECT(out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
			this.DllConnected = false;
		}
		void tryConnect_rescheduled(Object state) {
			if (this.connectionAttempts == 0) Thread.Sleep(this.quikBroker.ReconnectTimeoutMillis);
			this.connectionAttempts++;
			if (this.stopTryingToConnect) {
				this.connectionTimer.Change(Timeout.Infinite, Timeout.Infinite);
				return;
			}
			this.CurrentStatus = "0/3: DLL not connected, Quik Not Connected, No Symbols subscribed";
			Trans2Quik.Result error;
			string msg;
			try {
				error = this.invokeDll_async(out msg);
				this.connectionTimer.Change(Timeout.Infinite, Timeout.Infinite);
				msg = "tryConnect_rescheduled#" + connectionAttempts + "//timeoutInfinite QuikDllConnector(" + this.DllName + ") 2/2 " + this.quikBroker.UpstreamConnectionState;
				Assembler.PopupException(msg, null, false);
			} catch (Exception ex) {
				this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.ErrorConnectingNoRetriesAnymore, ex.Message);
				Assembler.PopupException("Should we set a limit/timeout on trials?", ex);
				return;
			}
		}

		Trans2Quik.Result invokeDll_async(out string msg) {
			msg = "NO_DLL_METHOD_WAS_INVOKED_YET";

			if (symbolClassesSubscribed.Count > 0) {
				this.symbolClassesSubscribed.Clear();
				string msg1 = "CLEARED SymbolClassSubscribers.Count=" + symbolClassesSubscribed.Count + "; most likely QUIK got disconnected";
				Assembler.PopupException(msg1, null, false);
			}

			Trans2Quik.Result status = Trans2Quik.Result.FAILED;

			status = Trans2Quik.SET_CONNECTION_STATUS_CALLBACK(this.connectionStatus_callback, out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
			if (status != Trans2Quik.Result.SUCCESS) {
				this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.UnknownConnectionState, this.callbackErrorMsg.ToString());
				msg = "1/5 FAILED: SET_CONNECTION_STATUS_CALLBACK(): error[" + error + "][" + callbackErrorMsg + "] status[" + status + "] != SUCCESS";
				Assembler.PopupException(msg);
				//throw new Exception(msg);
				return status;
			}

			status = Trans2Quik.SET_TRANSACTIONS_REPLY_CALLBACK(this.transactionReply_callback, out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
			if (status != Trans2Quik.Result.SUCCESS) {
				this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.UnknownConnectionState, this.callbackErrorMsg.ToString());
				msg = "2/5 FAILED: SET_TRANSACTIONS_REPLY_CALLBACK(): error[" + error + "][" + callbackErrorMsg + "] status[" + status + "] != SUCCESS";
				Assembler.PopupException(msg);
				//throw new Exception(msg);
				return status;
			}

			status = Trans2Quik.CONNECT(this.quikBroker.QuikFolder, out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
			if (status != Trans2Quik.Result.SUCCESS && status != Trans2Quik.Result.ALREADY_CONNECTED_TO_QUIK) {
				this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.UnknownConnectionState, this.callbackErrorMsg.ToString());
				msg = "3/5 FAILED Quik=>Menu=>Torgovlya=>VneshnieTransakcii=>NachatObrabotku: CONNECT(" + this.quikBroker.QuikFolder + "): error[" + error + "][" + callbackErrorMsg + "] status[" + status + "] != SUCCESS";
				Assembler.PopupException(msg);
				//throw new Exception(msg);
				return status;
			}

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
			return status;
		}

		void connectionStatus_callback(Trans2Quik.Result status, int err, string m) {
			string msig = " //QuikDllConnector(" + this.DllName + ")::connectionStatus_callback([" + status + "] m[" + m + "] err[" + err + "])";
			switch (status) {
				case Trans2Quik.Result.DLL_CONNECTED:
					this.DllConnected = true;
					this.CurrentStatus = "1/3: DLL Connected, Quik NOT connected, No Symbols subscribed";
					this.connectionTimer.Change(Timeout.Infinite, Timeout.Infinite);
					string msg1 = status + " WONT_RESCHEDULE_RECONNECT";
					Assembler.PopupException(msg1 + msig, null, false);
					this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.UpstreamConnected_downstreamUnsubscribed, msg1);
					break;

				case Trans2Quik.Result.QUIK_CONNECTED:
					this.CurrentStatus = "2/3: DLL Connected, Quik Connected, No Symbols subscribed";
					this.connectionTimer.Change(0, this.quikBroker.ReconnectTimeoutMillis);
					string msg4 = status + " RECONNECT_RESCHEDULED [" + this.quikBroker.ReconnectTimeoutMillis + "]ms";
					Assembler.PopupException(msg4 + msig, null, false);
					this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.UpstreamConnected_downstreamUnsubscribed, msg4);
					break;

				case Trans2Quik.Result.QUIK_DISCONNECTED:
					this.CurrentStatus = "2/3: DLL Connected, Quik Disconnected, No Symbols subscribed";
					this.connectionTimer.Change(0, this.quikBroker.ReconnectTimeoutMillis);
					string msg2 = status + " RECONNECT_RESCHEDULED [" + this.quikBroker.ReconnectTimeoutMillis + "]ms";
					Assembler.PopupException(msg2 + msig, null, false);
					this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.UpstreamDisconnected_downstreamUnsubscribed, msg2);
					break;

				case Trans2Quik.Result.DLL_DISCONNECTED:
					this.CurrentStatus = "1/3: DLL Disonnected, Quik Disconnected, No Symbols subscribed";
					this.DllConnected = false;
					this.connectionTimer.Change(0, this.quikBroker.ReconnectTimeoutMillis);
					string msg3 = status + " RECONNECT_RESCHEDULED [" + this.quikBroker.ReconnectTimeoutMillis + "]ms";
					Assembler.PopupException(msg3 + msig, null, false);
					this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.UpstreamDisconnected_downstreamUnsubscribed, msg3);
					break;

				default:
					Assembler.PopupException("NO_HANDLER" + msig);
					break;
			}
		}

		//string errorAsString(int error) {
		//    return Enum.GetName(typeof(Trans2Quik.Result), error);
		//}
	}
}
