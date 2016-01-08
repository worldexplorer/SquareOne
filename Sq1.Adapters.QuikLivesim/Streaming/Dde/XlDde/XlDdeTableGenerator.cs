using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using NDde.Client;

using Sq1.Core;

using Sq1.Adapters.Quik.Dde.XlDde;
using Sq1.Adapters.QuikLivesim;
using Sq1.Adapters.QuikLivesim.Dde.XlDde;

namespace Sq1.Adapters.QuikLiveism.Dde {
	public abstract class XlDdeTableGenerator  : ISynchronizeInvoke {
		protected abstract	string					DdeGeneratorClassName		{ get; }
		public		DateTime						LastDataReceived			{ get; protected set; }

					Form							syncContext					{ get { return this.QuikLivesimStreaming.Livesimulator.Executor.ChartShadow.ParentForm; } }
					string							ddeTopic;
					string							ddeService;
		public		DdeClient						DdeClient					{ get; private set; }
		protected	QuikLivesimStreaming			QuikLivesimStreaming		{ get; private set; }

		public		List<XlColumn>					Columns						{ get; private set; }
		public		Dictionary<string, XlColumn>	ColumnsLookup				{ get; private set; }

		protected	XlWriter						XlWriter					{ get; private set; }

		protected XlDdeTableGenerator(string ddeService, string ddeTopic, QuikLivesimStreaming quikLivesimStreaming) {
			this.ddeService				= ddeService;
			this.ddeTopic				= ddeTopic;
			this.DdeClient				= new DdeClient(this.ddeService, this.ddeTopic, this);
			this.DdeClient.Advise		+= new EventHandler<DdeAdviseEventArgs>(client_Advise);
			this.DdeClient.Disconnected	+= new EventHandler<DdeDisconnectedEventArgs>(client_Disconnected);

			this.QuikLivesimStreaming = quikLivesimStreaming;
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
			string msg = "DDE_CLIENT_CONNECTED [" + this.DdeClient.Topic + "]";
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
			string msig = " //client_Disconnected(" + e.ToString() + ")";
			string msg = "QuikLivesimDdeClient[" + this.QuikLivesimStreaming.Name + "]";
			Assembler.PopupException(msg + msig, null, false);
		}

		void client_Advise(object sender, DdeAdviseEventArgs e) {
			throw new NotImplementedException("XlDdeTableGenerator.client_Advise()");
		}


		#region 1) requirement of new DdeClient(a, b, HERE); 2) 100% displayed form is the Chart
		IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args) {
			return syncContext.BeginInvoke(method, args);
		}

		object ISynchronizeInvoke.EndInvoke(IAsyncResult result) {
			return syncContext.EndInvoke(result);
		}

		object ISynchronizeInvoke.Invoke(Delegate method, object[] args) {
			return syncContext.Invoke(method, args);
		}

		bool ISynchronizeInvoke.InvokeRequired {
			get { return syncContext.InvokeRequired; }
		}
		#endregion


		public override string ToString() {
			return "Service[" + this.ddeService + "] Topic[" + this.ddeTopic + "]";
		}
	}
}