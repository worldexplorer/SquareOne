using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using NDde.Client;

using Sq1.Core;

using Sq1.Adapters.Quik.Streaming.Dde.XlDde;
using NDde;

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

		protected XlDdeTableGenerator(string ddeService, string ddeTopic, QuikStreamingLivesim quikLivesimStreaming) {
			this.ddeService				= ddeService;
			this.ddeTopic				= ddeTopic;
			this.DdeClient				= new DdeClient(this.ddeService, this.ddeTopic, this);
			this.DdeClient.Advise		+= new EventHandler<DdeAdviseEventArgs>(client_Advise);
			this.DdeClient.Disconnected	+= new EventHandler<DdeDisconnectedEventArgs>(client_Disconnected);

			this.QuikStreamingLivesim = quikLivesimStreaming;
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
		}
		internal void Disconnect() {
			this.DdeClient.Disconnect();
			string msg = "DDE_CLIENT_DISCONNECTED [" + this.DdeClient.Topic + "]";
			Assembler.PopupException(msg, null, false);
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
				byte[] bufferToSend = this.XlWriter.ConvertToXlDdeMessage();
				IAsyncResult handle = this.DdeClient.BeginPoke(itemName_noClueHowIuseIt, bufferToSend, 0, null, this);
				if (this.QuikStreamingLivesim.DdePokerShouldSyncWaitForDdeServerToReceiveMessage_falseToAvoidDeadlocks) {
					this.DdeClient.EndPoke(handle);		//SYNCHRONOUS
				}
			} catch (ArgumentNullException ex) {
				Assembler.PopupException("This is thrown when item or data is a null reference.", ex);
			} catch (ArgumentException ex) {
				Assembler.PopupException("This is thown when item exceeds 255 characters.", ex);
			} catch (InvalidOperationException ex) {
				Assembler.PopupException("This is thrown when the client is not connected.", ex);
			} catch (DdeException ex) {
				Assembler.PopupException("This is thrown when the asynchronous operation could not begin.", ex);
			} catch (Exception ex) {
				Assembler.PopupException("UNKNOWN_ERROR_DDE_CLIENT_BEGIN_POKE", ex);
			}
		}
	}
}