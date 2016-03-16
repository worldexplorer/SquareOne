using System;
using System.Threading;

using Sq1.Core;
using Sq1.Core.DataTypes;

namespace Sq1.Adapters.Quik.Broker.Terminal {
	public sealed partial class QuikDllConnector {
					int				connectionAttempts = 0;

					Timer			connectionTimer;
					bool			ignoreTimerInvocation;
					bool			rescheduleToConnect { set {
					    if (value == true) {
					        this.ignoreTimerInvocation = false;
							if (this.connectionAttempts == 0) {
								// we start it in ConnectDll() - invoke now, infinite amount of times, with 5 sec delay
								this.connectionTimer.Change(0, Timeout.Infinite);	//this.quikBroker.ReconnectTimeoutMillis);
							} else {
								// we reschedule it in tryConnect_timeredEntryPoint() - wait 5 sec and re-invoke once
								this.connectionTimer.Change(this.quikBroker.ReconnectTimeoutMillis, Timeout.Infinite);
							}
					    } else {
					        this.ignoreTimerInvocation = true;
					        this.connectionTimer.Change(Timeout.Infinite, Timeout.Infinite);
					    }
					} }

		public		bool			DllConnected	{ get; private set; }
		public		bool			QuikConnected	{ get; private set; }
		public		bool			CanSend			{ get { return this.DllConnected && this.QuikConnected; } }

		public void ConnectDll() {
			if (this.connectionTimer == null) this.connectionTimer = new Timer(this.tryConnect_timeredEntryPoint);

			if (this.DllConnected) {
				string msg = "I_REFUSE_TO_CONNECT DLL_ALREADY_CONNECTED ALT+TAB_TO_YOUR_TERMINAL_AND_CLICK_CONNECT_ENTER_PASSWORD"
					+ " CONNECT_BUTTON_IN_DATASOURCE_EDITOR_WILL_TOGGLE_AFTER_QUIK_ESTABLISHES_ITS_OWN_TCP_CONNECTION"
					+ " CHECK_YOUR_NETWORK_CONNECTIVITY_TOO";
				Assembler.PopupException(msg, null, false);
				return;
			}

			this.connectionAttempts = 0;
			this.rescheduleToConnect = true;
			//this.tryConnect_timeredEntryPoint(null);
		}
		public void DisconnectDll() {
			this.rescheduleToConnect = false;
			if (this.DllConnected == false) {
				string msg = "DLL_ALREADY_DISCONNECTED";
				Assembler.PopupException(msg, null, false);
			}
			Trans2Quik.DISCONNECT(out anyCallback_error, this.anyCallback_msg, this.anyCallback_msg.Capacity);
			this.DllConnected = false;
			this.QuikConnected = false;
			this.quikBroker.CallbackFromQuikDll_ConnectionStateUpdated(ConnectionState.BrokerErrorConnectingNoRetriesAnymore, this.Ident + "DisconnectDll()");
		}
		void tryConnect_timeredEntryPoint(object state_notUsed) {
			//if (this.connectionAttempts == 0) Thread.Sleep(this.quikBroker.ReconnectTimeoutMillis);
			if (this.ignoreTimerInvocation) return;
			//this.rescheduleToConnect = false;

			string msig = this.Ident + "tryConnect_timeredEntryPoint()";
			Assembler.SetThreadName(msig);

			string msg = "NO_DLL_METHOD_WAS_INVOKED_YET";
			this.connectionAttempts++;

			this.quikBroker.CallbackFromQuikDll_ConnectionStateUpdated(ConnectionState.Broker_DllConnecting, msig);

			if (this.symbolClassesSubscribed.Count > 0) {
				this.symbolClassesSubscribed.Clear();
				string msg1 = "MUST_BE_EMPTY__CLEARED SymbolClassSubscribers.Count=" + this.symbolClassesSubscribed.Count + "; most likely QUIK got disconnected => bring back connectionTimer";
				Assembler.PopupException(msg1, null, false);
			}

			Trans2Quik.Result status_setCallback = Trans2Quik.SET_CONNECTION_STATUS_CALLBACK(this.dllCallback_connectionStatus, out anyCallback_error, this.anyCallback_msg, this.anyCallback_msg.Capacity);
			if (status_setCallback != Trans2Quik.Result.SUCCESS) {
				this.quikBroker.CallbackFromQuikDll_ConnectionStateUpdated(ConnectionState.UnknownConnectionState, "QUIK_SAID[" + this.anyCallback_msg.ToString() + "]");
				msg = "FAILED_TO_SET_CONNECTION_STATUS_CALLBACK [" + status_setCallback + "][" + anyCallback_msg + "] error[" + anyCallback_error + "]";
				Assembler.PopupException(msg);
				return;
			}

			Trans2Quik.Result status_connect = Trans2Quik.CONNECT(this.quikBroker.QuikFolder, out anyCallback_error, this.anyCallback_msg, this.anyCallback_msg.Capacity);
			if (status_connect == Trans2Quik.Result.SUCCESS) return;
			if (status_connect == Trans2Quik.Result.ALREADY_CONNECTED_TO_QUIK) return;

			// NOPE_ITS_ALREADY_PERIODIC this.rescheduleToConnect = true;

			msg = "QUIK_IS_NOT_RUNNING__LAUNCH_info.exe"	// OR DOESNT_ACCEPT_TRANSACTIONS_VIA_DLL
				//+ " Quik=>Menu=>Торговля=>ВнешниеТранзакции=>НачатьОбработку"
				+ " RECONNECT_RESCHEDULED[" + this.quikBroker.ReconnectTimeoutMillis + "]ms"
				+ " [" + status_connect + "][" + anyCallback_msg + "]"
				+ this.Ident
				+ "CONNECT() error[" + anyCallback_error + "]";
			Assembler.PopupException(msg, null, false);

			this.quikBroker.CallbackFromQuikDll_ConnectionStateUpdated(ConnectionState.Broker_DllConnecting, "QUIK_SAID[" + this.anyCallback_msg.ToString() + "]");		//ConnectionState.FailedToConnect
			this.rescheduleToConnect = true;
		}

		void dllCallback_connectionStatus(Trans2Quik.Result callbackStatus, int err, string quikMsg) {
			string msigHead = "[" + callbackStatus + "] [" + quikMsg + "] attempted[" + this.connectionAttempts + "]";
			string msigTail = " " + this.Ident + "dllCallback_connectionStatus(err[" + err + "])"
				+ " QuikFolder[" + this.quikBroker.QuikFolder + "]";
			string msig = " " + msigHead + msigTail;

			try {
				switch (callbackStatus) {
					case Trans2Quik.Result.DLL_CONNECTED:
						this.DllConnected = true;
						this.rescheduleToConnect = false;
						string msg = "WONT_RESCHEDULE_DLL_RECONNECT";
						Assembler.PopupException(msg + msig, null, false);
						this.quikBroker.CallbackFromQuikDll_ConnectionStateUpdated(ConnectionState.Broker_DllConnected, msg + msigHead);

						callbackStatus = Trans2Quik.SET_TRANSACTIONS_REPLY_CALLBACK(this.dllCallback_transactionReply, out anyCallback_error, this.anyCallback_msg, this.anyCallback_msg.Capacity);
						if (callbackStatus != Trans2Quik.Result.SUCCESS) {
							this.quikBroker.CallbackFromQuikDll_ConnectionStateUpdated(ConnectionState.UnknownConnectionState, this.anyCallback_msg.ToString());
							string msg4 = "FAILED_TO_SET_TRANSACTIONS_REPLY_CALLBACK [" + this.anyCallback_msg + "] error[" + this.anyCallback_error + "]";
							Assembler.PopupException(msg4 + msigHead, null, false);
							//throw new Exception(msg);
							//return status;
						}

						// launched & connected QUIK first; connected to DLL to see if we CanSend
						int pnExtendedErrorCode;
						Trans2Quik.Result status_quikConnected = Trans2Quik.IS_QUIK_CONNECTED(out pnExtendedErrorCode, this.anyCallback_msg, this.anyCallback_msg.Capacity);
						if (status_quikConnected == Trans2Quik.Result.QUIK_CONNECTED) {
							this.QuikConnected = true;
							this.quikBroker.CallbackFromQuikDll_ConnectionStateUpdated(ConnectionState.Broker_TerminalConnected, msg + msigHead);
						}

						break;

					case Trans2Quik.Result.QUIK_CONNECTED:
						//this.rescheduleToConnect = false;
						this.QuikConnected = true;
						//string msg1 = "__RECONNECT_RESCHEDULED[" + this.quikBroker.ReconnectTimeoutMillis + "]ms";
						string msg1 = "TERMINAL_CONNECTED";
						Assembler.PopupException(msg1 + msig, null, false);
						this.quikBroker.CallbackFromQuikDll_ConnectionStateUpdated(ConnectionState.Broker_TerminalConnected, msg1);
						break;

					case Trans2Quik.Result.QUIK_DISCONNECTED:
						//this.rescheduleToConnect = true;
						this.QuikConnected = false;
						//string msg2 = "__RECONNECT_RESCHEDULED[" + this.quikBroker.ReconnectTimeoutMillis + "]ms";
						string msg2 = "TERMINAL_WAS_TRYING_TO_RECONNECT";
						Assembler.PopupException(msg2 + msig, null, false);
						this.quikBroker.CallbackFromQuikDll_ConnectionStateUpdated(ConnectionState.Broker_TerminalDisconnected, msg2);

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
						this.DllConnected = false;
						this.connectionAttempts = 0;
						this.rescheduleToConnect = true;
						string msg3 = "RECONNECT_RESCHEDULED[" + this.quikBroker.ReconnectTimeoutMillis + "]ms";
						Assembler.PopupException(msg3 + msig, null, false);
						this.quikBroker.CallbackFromQuikDll_ConnectionStateUpdated(ConnectionState.Broker_DllConnecting, msg3);	//Broker_DllDisonnected
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
