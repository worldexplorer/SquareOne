using System;
using System.Threading;

using Sq1.Core;
using Sq1.Core.DataTypes;

namespace Sq1.Adapters.Quik.Broker.Terminal {
	public sealed partial class QuikDllConnector {
					int				connectionAttempts = 0;
					bool			stopTryingToConnectDll;
					Timer			connectionTimer;
		public		bool			DllConnected	{ get; private set; }

		public void ConnectDll() {
			this.stopTryingToConnectDll = false;
			this.connectionTimer.Change(0, this.quikBroker.ReconnectTimeoutMillis);
		}
		public void DisconnectDll() {
			this.stopTryingToConnectDll = true;
			this.connectionTimer.Change(Timeout.Infinite, Timeout.Infinite);
			if (this.DllConnected == false) {
				string msg = "I_REFUSE_TO_DISCONNECT ALREADY_DISCONNECTED";
				Assembler.PopupException(msg);
			}
			Trans2Quik.DISCONNECT(out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
			this.DllConnected = false;
		}
		void tryConnectPeriodical(Object state) {
			if (this.connectionAttempts == 0) Thread.Sleep(this.quikBroker.ReconnectTimeoutMillis);
			this.connectionAttempts++;
			if (this.stopTryingToConnectDll) {
				this.connectionTimer.Change(Timeout.Infinite, Timeout.Infinite);
				return;
			}
			this.CurrentStatus = "0/2: DLL not connected, No Symbols subscribed";
			Trans2Quik.Result error;
			string msg;
			try {
				error = this.invokeDll_async(out msg);
				this.connectionTimer.Change(Timeout.Infinite, Timeout.Infinite);
				msg = "#" + connectionAttempts + "//timeoutInfinite QuikDllConnector(" + this.DllName + ") 2/2 " + this.quikBroker.UpstreamConnectionState;
				Assembler.PopupException(msg, null, false);
			} catch (Exception e) {
				this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.ErrorConnectingNoRetriesAnymore, e.Message);
				Assembler.PopupException("Should we set a limit/timeout on trials?", e);
				return;
			}
		}

		Trans2Quik.Result invokeDll_async(out string msg) {
			msg = "NO_DLL_METHOD_WAS_INVOKED_YET";

			if (symbolClassesSubscribed.Count > 0) {
				Assembler.PopupException("Clearing SymbolClassSubscribers.Count=" + symbolClassesSubscribed.Count + "; most likely QUIK got disconnected");
				this.symbolClassesSubscribed.Clear();
			}

			Trans2Quik.Result ret = Trans2Quik.Result.FAILED;

			ret = Trans2Quik.SET_CONNECTION_STATUS_CALLBACK(this.connectionStatus_callback, out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
			if (ret != Trans2Quik.Result.SUCCESS) {
				this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.UnknownConnectionState, this.callbackErrorMsg.ToString());
				msg = "1/5 FAILED: SET_CONNECTION_STATUS_CALLBACK(): error[" + errorAsString(error) + "][" + callbackErrorMsg + "] ret[" + ret + "] != SUCCESS";
				Assembler.PopupException(msg);
				//throw new Exception(msg);
				return ret;
			}

			ret = Trans2Quik.SET_TRANSACTIONS_REPLY_CALLBACK(this.transactionReply_callback, out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
			if (ret != Trans2Quik.Result.SUCCESS) {
				this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.UnknownConnectionState, this.callbackErrorMsg.ToString());
				msg = "2/5 FAILED: SET_TRANSACTIONS_REPLY_CALLBACK(): error[" + errorAsString(error) + "][" + callbackErrorMsg + "] ret[" + ret + "] != SUCCESS";
				Assembler.PopupException(msg);
				//throw new Exception(msg);
				return ret;
			}

			ret = Trans2Quik.CONNECT(this.quikBroker.QuikFolder, out error, this.callbackErrorMsg, this.callbackErrorMsg.Capacity);
			if (ret != Trans2Quik.Result.SUCCESS && ret != Trans2Quik.Result.ALREADY_CONNECTED_TO_QUIK) {
				this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.UnknownConnectionState, this.callbackErrorMsg.ToString());
				msg = "3/5 FAILED Quik=>Menu=>Torgovlya=>VneshnieTransakcii=>NachatObrabotku: CONNECT(" + this.quikBroker.QuikFolder + "): error[" + errorAsString(error) + "][" + callbackErrorMsg + "] ret[" + ret + "] != SUCCESS";
				Assembler.PopupException(msg);
				//throw new Exception(msg);
				return ret;
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
			return ret;
		}

		void connectionStatus_callback(Trans2Quik.Result status, int err, string m) {
			string msig = " //connectionStatus_callback[" + status + "] err[" + err + "] m[" + m + "]";
			switch (status) {
				case Trans2Quik.Result.DLL_CONNECTED:
					this.DllConnected = true;
					this.CurrentStatus = "1/2: DLL Connected, No Symbols subscribed";
					this.connectionTimer.Change(Timeout.Infinite, Timeout.Infinite);
					string msg1 = "DLL_CONNECTED WONT_RESCHEDULE connectionTimer";
					Assembler.PopupException(msg1 + msig, null, false);
					this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.UpstreamConnected_downstreamSubscribed, msg1);
					break;

				case Trans2Quik.Result.QUIK_DISCONNECTED:
					this.connectionTimer.Change(0, this.quikBroker.ReconnectTimeoutMillis);
					string msg2 = "QUIK_DISCONNECTED connectionTimer.Change(0, " + this.quikBroker.ReconnectTimeoutMillis + ")";
					Assembler.PopupException(msg2 + msig, null, false);
					this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.UpstreamDisconnected_downstreamUnsubscribed, msg2);
					break;

				case Trans2Quik.Result.DLL_DISCONNECTED:
					this.DllConnected = false;
					this.connectionTimer.Change(0, this.quikBroker.ReconnectTimeoutMillis);
					string msg3 = "DLL_DISCONNECTED connectionTimer.Change(0, " + this.quikBroker.ReconnectTimeoutMillis + ")";
					Assembler.PopupException(msg3 + msig, null, false);
					this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.UpstreamDisconnected_downstreamUnsubscribed, msg3);
					break;

				default:
					Assembler.PopupException("NO_HANDLER" + msig);
					break;
			}
		}
	}
}
