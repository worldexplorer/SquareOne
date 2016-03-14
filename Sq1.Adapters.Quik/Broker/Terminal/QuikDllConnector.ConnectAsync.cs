using System;
using System.Threading;

using Sq1.Core;
using Sq1.Core.DataTypes;

namespace Sq1.Adapters.Quik.Broker.Terminal {
	public sealed partial class QuikDllConnector {
					int				connectionAttempts = 0;

					//Timer			connectionTimer;
					//bool			ignoreTimerInvocation;
					//bool			rescheduleToConnect { set {
					//    if (value == true) {
					//        this.ignoreTimerInvocation = false;
					//        this.connectionTimer.Change(0, this.quikBroker.ReconnectTimeoutMillis);
					//    } else {
					//        this.ignoreTimerInvocation = true;
					//        this.connectionTimer.Change(Timeout.Infinite, Timeout.Infinite);
					//    }
					//} }

		public		bool			DllConnected	{ get; private set; }
		public		bool			QuikConnected	{ get; private set; }
		public		bool			CanSend			{ get { return this.DllConnected && this.QuikConnected; } }

		public void ConnectDll() {
			if (this.DllConnected) {
				string msg = "I_REFUSE_TO_CONNECT DLL_ALREADY_CONNECTED ALT+TAB_TO_YOUR_TERMINAL_AND_CLICK_CONNECT_ENTER_PASSWORD"
					+ " CONNECT_BUTTON_IN_DATASOURCE_EDITOR_WILL_TOGGLE_AFTER_QUIK_ESTABLISHES_ITS_OWN_TCP_CONNECTION"
					+ " CHECK_YOUR_NETWORK_CONNECTIVITY_TOO";
				Assembler.PopupException(msg, null, false);
				return;
			}

			//this.rescheduleToConnect = true;
			this.tryConnect_timeredEntryPoint(null);
		}
		public void DisconnectDll() {
			//this.rescheduleToConnect = false;
			if (this.DllConnected == false) {
				string msg = "I_REFUSE_TO_DISCONNECT ALREADY_DISCONNECTED";
				Assembler.PopupException(msg, null, false);
			}
			Trans2Quik.DISCONNECT(out anyCallback_error, this.anyCallback_msg, this.anyCallback_msg.Capacity);
			this.DllConnected = false;
			this.QuikConnected = false;
		}
		void tryConnect_timeredEntryPoint(object state_notUsed) {
			//if (this.connectionAttempts == 0) Thread.Sleep(this.quikBroker.ReconnectTimeoutMillis);
			//if (this.ignoreTimerInvocation) return;
			//this.rescheduleToConnect = false;

			string msg = "NO_DLL_METHOD_WAS_INVOKED_YET";
			this.connectionAttempts++;

			if (this.symbolClassesSubscribed.Count > 0) {
				this.symbolClassesSubscribed.Clear();
				string msg1 = "MUST_BE_EMPTY__CLEARED SymbolClassSubscribers.Count=" + this.symbolClassesSubscribed.Count + "; most likely QUIK got disconnected => bring back connectionTimer";
				Assembler.PopupException(msg1, null, false);
			}

			Trans2Quik.Result status_setCallback = Trans2Quik.SET_CONNECTION_STATUS_CALLBACK(this.connectionStatus_callback, out anyCallback_error, this.anyCallback_msg, this.anyCallback_msg.Capacity);
			if (status_setCallback != Trans2Quik.Result.SUCCESS) {
				this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.UnknownConnectionState, "QUIK_SAID[" + this.anyCallback_msg.ToString() + "]");
				msg = "FAILED_TO_SET_CONNECTION_STATUS_CALLBACK [" + status_setCallback + "][" + anyCallback_msg + "] error[" + anyCallback_error + "]";
				Assembler.PopupException(msg);
				return;
			}

			Trans2Quik.Result status_connect = Trans2Quik.CONNECT(this.quikBroker.QuikFolder, out anyCallback_error, this.anyCallback_msg, this.anyCallback_msg.Capacity);
			if (status_connect != Trans2Quik.Result.SUCCESS && status_connect != Trans2Quik.Result.ALREADY_CONNECTED_TO_QUIK) {
				this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.FailedToConnect, "QUIK_SAID[" + this.anyCallback_msg.ToString() + "]");
				msg = "QUIK_IS_NOT_RUNNING__LAUNCH_info.exe"	// OR DOESNT_ACCEPT_TRANSACTIONS_VIA_DLL
					//+ " Quik=>Menu=>Торговля=>ВнешниеТранзакции=>НачатьОбработку"
					+ " [" + status_connect + "][" + anyCallback_msg + "]"
					+ " CONNECT(" + this.quikBroker.QuikFolder + ") error[" + anyCallback_error + "]";
				Assembler.PopupException(msg, null, false);
				//throw new Exception(msg);
			}
			//this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.ErrorConnectingNoRetriesAnymore, ex.Message);
		}

		void connectionStatus_callback(Trans2Quik.Result callbackStatus, int err, string quikMsg) {
			string msigHead = "[" + callbackStatus + "] [" + quikMsg + "]";
			string msigTail = " //QuikDllConnector(" + this.DllName + ")::connectionStatus_callback(err[" + err + "])";
			string msig = " " + msigHead + msigTail;

			try {
				switch (callbackStatus) {
					case Trans2Quik.Result.DLL_CONNECTED:
						this.DllConnected = true;
						//this.rescheduleToConnect = false;
						string msg = "WONT_RESCHEDULE_RECONNECT";
						Assembler.PopupException(msg + msig, null, false);
						this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.Broker_DllConnected, msg + msigHead);

						callbackStatus = Trans2Quik.SET_TRANSACTIONS_REPLY_CALLBACK(this.transactionReply_callback, out anyCallback_error, this.anyCallback_msg, this.anyCallback_msg.Capacity);
						if (callbackStatus != Trans2Quik.Result.SUCCESS) {
							this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.UnknownConnectionState, this.anyCallback_msg.ToString());
							string msg4 = "FAILED_TO_SET_TRANSACTIONS_REPLY_CALLBACK [" + this.anyCallback_msg + "] error[" + this.anyCallback_error + "]";
							Assembler.PopupException(msg4 + msigHead, null, false);
							//throw new Exception(msg);
							//return status;
						}
						break;

					case Trans2Quik.Result.QUIK_CONNECTED:
						//this.rescheduleToConnect = true;
						this.QuikConnected = true;
						string msg1 = "__RECONNECT_RESCHEDULED [" + this.quikBroker.ReconnectTimeoutMillis + "]ms";
						Assembler.PopupException(msg1 + msig, null, false);
						this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.Broker_TerminalConnected, msg1);
						break;

					case Trans2Quik.Result.QUIK_DISCONNECTED:
						//this.rescheduleToConnect = true;
						this.QuikConnected = false;
						//string msg2 = "__RECONNECT_RESCHEDULED [" + this.quikBroker.ReconnectTimeoutMillis + "]ms";
						string msg2 = "TERMINAL_WAS_TRYING_TO_RECONNECT";
						Assembler.PopupException(msg2 + msig, null, false);
						this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.Broker_TerminalDisonnected, msg2);

						return;
						Trans2Quik.Result status_unsubscribeOrders = Trans2Quik.UNSUBSCRIBE_ORDERS();
						if (status_unsubscribeOrders != Trans2Quik.Result.SUCCESS) {
							//this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.TermialDisonnected, "can not UNSUBSCRIBE_ORDERS()");
							msg2 = "FAILED_TO_UNSUBSCRIBE_ORDERS() [" + status_unsubscribeOrders + "]";
							Assembler.PopupException(msg2 + msig, null, false);
							//return;
						}

						Trans2Quik.Result status_unsubscribeTrades = Trans2Quik.UNSUBSCRIBE_TRADES();
						if (status_unsubscribeTrades != Trans2Quik.Result.SUCCESS) {
							//this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(QuikTerminalQuikConnectionState.DllConnected, "can not UNSUBSCRIBE_TRADES()");
							msg2 = "FAILED_TO_UNSUBSCRIBE_TRADES() ret[" + status_unsubscribeTrades + "]";
							Assembler.PopupException(msg2 + msig, null, false);
							//return;
						}

						break;

					case Trans2Quik.Result.DLL_DISCONNECTED:
						//this.rescheduleToConnect = true;
						this.DllConnected = false;
						string msg3 = "__RECONNECT_RESCHEDULED [" + this.quikBroker.ReconnectTimeoutMillis + "]ms";
						Assembler.PopupException(msg3 + msig, null, false);
						this.quikBroker.ConnectionStateUpdated_callbackFromQuikDll(ConnectionState.Broker_DllDisonnected, msg3);
						break;

					default:
						Assembler.PopupException("NO_HANDLER" + msigTail);
						break;
				}
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			}
		}

		//string errorAsString(int error) {
		//    return Enum.GetName(typeof(Trans2Quik.Result), error);
		//}
	}
}
