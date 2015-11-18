using System;
using System.ComponentModel;
using System.Windows.Forms;

using NDde.Client;
using Sq1.Core;

namespace Sq1.Adapters.QuikLivesim.DataFeed {
	class QuikLivesimDdeClient : ISynchronizeInvoke {
				QuikLivesimStreaming quikLivesimStreaming;
		public	DdeClient DdeClient;
		
				Form	syncContext		{ get { return this.quikLivesimStreaming.Livesimulator.Executor.ChartShadow.ParentForm; } }
				string	ddeService		{ get { return this.quikLivesimStreaming.QuikStreamingPuppet.DdeServiceName; } }
				string	ddeTopicQuotes	{ get { return this.quikLivesimStreaming.QuikStreamingPuppet.DdeSubscriptionManager.TableQuotes.Topic; } }

		public QuikLivesimDdeClient(QuikLivesimStreaming quikLivesimStreaming) {
			this.quikLivesimStreaming = quikLivesimStreaming;
            DdeClient = new DdeClient(this.ddeService, this.ddeTopicQuotes, this);
			DdeClient.Advise += new EventHandler<DdeAdviseEventArgs>(client_Advise);
			DdeClient.Disconnected += new EventHandler<DdeDisconnectedEventArgs>(client_Disconnected);
		}

		void client_Advise(object sender, DdeAdviseEventArgs e) {
			throw new NotImplementedException();
		}

		void client_Disconnected(object sender, DdeDisconnectedEventArgs e) {
			string msig = " //client_Disconnected(" + e.ToString() + ")";
			string msg = "QuikLivesimDdeClient[" + this.quikLivesimStreaming.Name + "]";
			Assembler.PopupException(msg + msig, null, false);
		}

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
	}
}
