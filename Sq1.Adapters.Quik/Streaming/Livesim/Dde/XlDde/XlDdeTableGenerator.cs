using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using NDde;
using NDde.Client;

using Sq1.Core;

using Sq1.Adapters.Quik.Streaming.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Livesim.Dde.XlDde {
	public abstract class XlDdeTableGenerator  : ISynchronizeInvoke {
		protected abstract	string					DdeGeneratorClassName		{ get; }
		public		DateTime						LastDataReceived			{ get; protected set; }

					Form							syncContext					{ get { return this.QuikStreamingLivesim.Livesimulator.Executor.ChartShadow.ParentForm; } }
					string							ddeTopic;
					string							ddeService;
		public		DdeClient						DdeClient					{ get; private set; }
		protected	QuikStreamingLivesim			QuikStreamingLivesim		{ get; private set; }

		public		List<XlColumn>					Columns						{ get; private set; }
		public		Dictionary<string, XlColumn>	ColumnsLookup				{ get; private set; }

		protected	XlWriter						XlWriter					{ get; private set; }

		public		bool							DdeWillDeliver_updatesForEachRow_inSeparateMessages			{ get; private set; }
		public		bool							HeaderAlreadySentForThisSession								{ get; private set; }


		protected XlDdeTableGenerator(string ddeService, string ddeTopic, QuikStreamingLivesim quikLivesimStreaming, bool oneRowUpdates) {
			this.ddeService				= ddeService;
			this.ddeTopic				= ddeTopic;
			this.DdeClient				= new DdeClient(this.ddeService, this.ddeTopic, this);
			this.DdeClient.Advise		+= new EventHandler<DdeAdviseEventArgs>(client_Advise);
			this.DdeClient.Disconnected	+= new EventHandler<DdeDisconnectedEventArgs>(client_Disconnected);

			this.QuikStreamingLivesim = quikLivesimStreaming;
			DdeWillDeliver_updatesForEachRow_inSeparateMessages = oneRowUpdates;
		}
		protected void Initialize(List<XlColumn> columns) {
			if (columns == null || columns.Count == 0) {
				string msg = "DONT_FEED_ME_WITH_EMPTY_COLUMNS //XlWriter(" + columns + ")";
				Assembler.PopupException(msg);
				return;
			}

			this.Columns = columns;
			this.ColumnsLookup = new Dictionary<string, XlColumn>();
			foreach (XlColumn col in this.Columns) {
				this.ColumnsLookup.Add(col.Name, col);
			}

			this.XlWriter = new XlWriter(this);
		}

		public virtual void OutgoingTableBegin() {
			this.XlWriter.StartNewTable();
		}
		public virtual void OutgoingTableTerminate() {
		}

		//public abstract void OutgoingObjectBufferize_eachRow(object objectToSendAsDdeMessage);

		
		internal void Connect() {
			this.DdeClient.Connect();
			string msg = "DDE_CLIENT_CONNECTED [" + this.DdeClient.Topic + "]";
			Assembler.PopupException(msg, null, false);
			if (this.HeaderAlreadySentForThisSession != false) {
				string msg1 = "MUST_HAVE_BEEN_RESET_IN_client_Disconnected(): this.HeaderAlreadySentForThisSession[" + this.HeaderAlreadySentForThisSession + "]=>false //Connect()";
				Assembler.PopupException(msg1);
				this.HeaderAlreadySentForThisSession = false;
			}
		}
		internal void Disconnect() {
			this.DdeClient.Disconnect();
			string msg = "DDE_CLIENT_DISCONNECTED [" + this.DdeClient.Topic + "]";
			Assembler.PopupException(msg, null, false);
			if (this.HeaderAlreadySentForThisSession != false) {
				string msg1 = "MUST_HAVE_BEEN_RESET_IN_client_Disconnected(): this.HeaderAlreadySentForThisSession[" + this.HeaderAlreadySentForThisSession + "]=>false //Disconnect()";
				Assembler.PopupException(msg1);
				this.HeaderAlreadySentForThisSession = false;
			}
		}
		internal void Dispose() {
			string topic = this.DdeClient.Topic;
			this.DdeClient.Dispose();
			string msg = "DDE_CLIENT_DISPOSED [" + topic + "]";
			Assembler.PopupException(msg, null, false);
		}

		// IRANAI_DES
		//void ddePokeAsyncCallback(IAsyncResult ar) {
		//    bool isCompleted_hopeNo = ar.IsCompleted;
		//    bool completedSynchronously = ar.CompletedSynchronously;
		//    bool isCompleted_hopeYes = ar.IsCompleted;
			
		//    string msg = "INVOKED_AT_END_POKE__I_WILL_USE_this.DdeClient.EndPoke(handle) which waits for the PokeResult.Complete";
		//    string msig = " //QuikLivesimDdeClient.ddePokeAsyncCallback(" + ar.ToString() + ")";
		//    //Assembler.PopupException(msg + msig, null, false);
		//}

		void client_Disconnected(object sender, DdeDisconnectedEventArgs e) {
			string msig = " //XlDdeTableGenerator.client_Disconnected(" + e.ToString() + ")";
			string msg = "QuikLivesimDdeClient[" + this.QuikStreamingLivesim.Name + "]";
			Assembler.PopupException(msg + msig, null, false);
			this.HeaderAlreadySentForThisSession = false;
		}

		void client_Advise(object sender, DdeAdviseEventArgs e) {
			throw new NotImplementedException("XlDdeTableGenerator.client_Advise()");
		}


		#region 1) requirement of new DdeClient(a, b, HERE); 2) 100% displayed form is the Chart
		IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args) {
			IAsyncResult ret = null;
			ret = syncContext.BeginInvoke(method, args);
			return ret;
		}

		object ISynchronizeInvoke.EndInvoke(IAsyncResult result) {
			object ret = null;
			ret = syncContext.EndInvoke(result);
			return ret;
		}

		object ISynchronizeInvoke.Invoke(Delegate method, object[] args) {
			object ret = null;
			ret = syncContext.Invoke(method, args);
			return ret;
		}

		bool ISynchronizeInvoke.InvokeRequired {
			get { return syncContext.InvokeRequired; }
		}
		#endregion


		public override string ToString() {
			string connectedPaused = "DISCONNECTED";
			if (this.DdeClient.IsConnected) connectedPaused = this.DdeClient.IsPaused ? "PAUSED" : "CONNECTED";
			return "Topic[" + this.ddeTopic + "]:" + connectedPaused;
		}

		protected void Send_DdeClientPokesDdeServer_asynControlledByLivesim(string itemName_noClueHowIuseIt) {
			try {
				bool sendHeader = true;
				if (this.DdeWillDeliver_updatesForEachRow_inSeparateMessages) {
					if (this.HeaderAlreadySentForThisSession) {
						sendHeader = false;
					} else {
						this.HeaderAlreadySentForThisSession =  true;	// will be sent again only at the beginning of next Livesimming session (after disconnect)
						sendHeader = true;
					}
				}
				byte[] bufferToSend = this.XlWriter.ConvertToXlDdeMessage(sendHeader);
				IAsyncResult handle = this.DdeClient.BeginPoke(itemName_noClueHowIuseIt, bufferToSend, 0, null, this);
				if (this.QuikStreamingLivesim.DdePokerShouldSyncWaitForDdeServerToReceiveMessage_falseToAvoidDeadlocks) {
					this.DdeClient.EndPoke(handle);		//SYNCHRONOUS
				}
				return;
			} catch (ArgumentNullException ex) {
				string msg = "This is thrown when item or data is a null reference.";
				Assembler.PopupException(msg, ex);
				throw new Exception(msg, ex);
			} catch (ArgumentException ex) {
				string msg = "This is thown when item exceeds 255 characters.";
				Assembler.PopupException(msg, ex);
				throw new Exception(msg, ex);
			} catch (InvalidOperationException ex) {
				string msg = "This is thrown when the client is not connected.";
				Assembler.PopupException(msg, ex);
				throw new Exception(msg, ex);
			} catch (DdeException ex) {
				string msg = "This is thrown when the asynchronous operation could not begin.";
				Assembler.PopupException(msg, ex);
				throw new Exception(msg, ex);
			} catch (Exception ex) {
				string msg = "UNKNOWN_ERROR_DDE_CLIENT_BEGIN_POKE";
				Assembler.PopupException(msg, ex);
				throw new Exception(msg, ex);
			}
		}
	}
}