using System;
using System.ComponentModel;
using System.Windows.Forms;

using NDde.Client;
using Sq1.Core;
using Sq1.Core.Backtesting;
using NDde;
using Sq1.Adapters.QuikLivesim.Dde;
using Sq1.Core.DataTypes;

namespace Sq1.Adapters.QuikLivesim.DataFeed {
	public class QuikLivesimDdeClient : ISynchronizeInvoke {
				QuikLivesimStreaming	quikLivesimStreaming;
		public	DdeClient				DdeClient;
				DdeTableGeneratorQuotes ddeTableGeneratorQuotes;
		
				Form					syncContext		{ get { return this.quikLivesimStreaming.Livesimulator.Executor.ChartShadow.ParentForm; } }
				string					ddeService		{ get { return this.quikLivesimStreaming.QuikStreamingPuppet.DdeServiceName; } }
				string					ddeTopicQuotes	{ get { return this.quikLivesimStreaming.QuikStreamingPuppet.DdeSubscriptionManager.TableQuotes.Topic; } }

		public QuikLivesimDdeClient(QuikLivesimStreaming quikLivesimStreaming) {
			this.quikLivesimStreaming	= quikLivesimStreaming;
            this.DdeClient				= new DdeClient(this.ddeService, this.ddeTopicQuotes, this);
			this.DdeClient.Advise		+= new EventHandler<DdeAdviseEventArgs>(client_Advise);
			this.DdeClient.Disconnected	+= new EventHandler<DdeDisconnectedEventArgs>(client_Disconnected);
			this.ddeTableGeneratorQuotes = new DdeTableGeneratorQuotes(this.ddeTopicQuotes, this.quikLivesimStreaming);
		}

		internal void SendQuote(QuoteGenerated quote) {
			try {
				ddeTableGeneratorQuotes.OutgoingTableBegin();
				ddeTableGeneratorQuotes.OutgoingObjectBufferize_eachRow(quote);
				ddeTableGeneratorQuotes.OutgoingTableTerminate();

				byte[] bufferToSend = this.ddeTableGeneratorQuotes.GetXlDdeMessage();
				
			    IAsyncResult handle = this.DdeClient.BeginPoke("quote", bufferToSend, 0, null, this);
			    this.DdeClient.EndPoke(handle);
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

		void client_Advise(object sender, DdeAdviseEventArgs e) {
			throw new NotImplementedException();
		}

		void client_Disconnected(object sender, DdeDisconnectedEventArgs e) {
			string msig = " //client_Disconnected(" + e.ToString() + ")";
			string msg = "QuikLivesimDdeClient[" + this.quikLivesimStreaming.Name + "]";
			Assembler.PopupException(msg + msig, null, false);
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

	}
}
