using System;
using System.ComponentModel;
using System.Windows.Forms;

using NDde;
using NDde.Client;

using Sq1.Core;
using Sq1.Core.Backtesting;
using Sq1.Core.DataTypes;

using Sq1.Adapters.Quik;
using Sq1.Adapters.QuikLivesim.Dde;
//using System.Threading;

namespace Sq1.Adapters.QuikLivesim.DataFeed {
	public class QuikLivesimDdeClient : ISynchronizeInvoke {
				QuikLivesimStreaming	quikLivesimStreaming;
		public	DdeClient				DdeClient;
				DdeTableGeneratorQuotes ddeTableGeneratorQuotes;
		
				Form					syncContext		{ get { return this.quikLivesimStreaming.Livesimulator.Executor.ChartShadow.ParentForm; } }
				string					ddeService		{ get { return this.quikLivesimStreaming.QuikStreamingPuppet.DdeServiceName; } }
				string					ddeTopicQuotes	{ get { return this.quikLivesimStreaming.QuikStreamingPuppet.DdeBatchSubscriber.TableQuotes.Topic; } }

		public QuikLivesimDdeClient(QuikLivesimStreaming quikLivesimStreaming) {
			this.quikLivesimStreaming	= quikLivesimStreaming;
			this.DdeClient				= new DdeClient(this.ddeService, this.ddeTopicQuotes, this);
			this.DdeClient.Advise		+= new EventHandler<DdeAdviseEventArgs>(client_Advise);
			this.DdeClient.Disconnected	+= new EventHandler<DdeDisconnectedEventArgs>(client_Disconnected);
			this.ddeTableGeneratorQuotes = new DdeTableGeneratorQuotes(this.ddeTopicQuotes, this.quikLivesimStreaming);
		}

		internal void SendQuote_DdeClientPokesDdeServer_waitServerProcessed(QuoteGenerated quote) {
			try {
				this.ddeTableGeneratorQuotes.OutgoingTableBegin();
				this.ddeTableGeneratorQuotes.OutgoingObjectBufferize_eachRow(quote);
				this.ddeTableGeneratorQuotes.OutgoingTableTerminate();

				byte[] bufferToSend = this.ddeTableGeneratorQuotes.GetXlDdeMessage();
				
				//IRANAI_DES IAsyncResult handle = this.DdeClient.BeginPoke("item-quote", bufferToSend, 0, new AsyncCallback(this.ddePokeAsyncCallback), this);
				IAsyncResult handle = this.DdeClient.BeginPoke("item-quote", bufferToSend, 0, null, this);
	
				bool isCompleted_hereNo		= handle.IsCompleted;
				// this.DdeClient.EndPoke(handle) is waiting for DdeServer.OnPoke() to return PokeResult.Processed;
				// with straight (non-threaded) QuotePump that means Strategy.OnQuote() has returned
				this.DdeClient.EndPoke(handle);		//SYNCHRONOUS_IS_EASIER_TO_DEBUG
				//bool completedSynchronously_hereFalse_noIdea	= handle.CompletedSynchronously;
				bool isCompleted_hereYes	= handle.IsCompleted;

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
			string msg = "QuikLivesimDdeClient[" + this.quikLivesimStreaming.Name + "]";
			Assembler.PopupException(msg + msig, null, false);
		}

		void client_Advise(object sender, DdeAdviseEventArgs e) {
			throw new NotImplementedException("QuikLivesimDdeClient.client_Advise()");
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
			return "QuikLivesimDdeClient[" + this.ddeService + "]";
		}
	}
}
