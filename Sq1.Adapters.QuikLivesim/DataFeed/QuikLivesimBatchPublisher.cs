using System;
using System.Windows.Forms;

using NDde;
using NDde.Client;

using Sq1.Core;
using Sq1.Core.Backtesting;
using Sq1.Core.DataTypes;
using Sq1.Core.Support;

using Sq1.Adapters.Quik;
using Sq1.Adapters.QuikLivesim.Dde;
//using System.Threading;

namespace Sq1.Adapters.QuikLivesim.DataFeed {
	public class QuikLivesimBatchPublisher {
				QuikLivesimStreaming			quikLivesimStreaming;

				DdeClient						ddeClientDepth;
				DdeClient						ddeClientTrades;

				DdeTableGeneratorQuotes			ddeTableGeneratorQuotes;
				DdeTableGeneratorDepth			ddeTableGeneratorDepth;
				string							symbolSingleImLivesimmingDepth;
		
				string							ddeService		{ get { return this.quikLivesimStreaming.QuikStreamingPuppet.DdeServiceName; } }
				string							ddeTopicQuotes	{ get { return this.quikLivesimStreaming.QuikStreamingPuppet.DdeBatchSubscriber.TableQuotes.Topic; } }
				string							ddeTopicDepth	{ get { return this.quikLivesimStreaming.QuikStreamingPuppet.DdeBatchSubscriber.GetDomTopicForSymbol(this.symbolSingleImLivesimmingDepth); } }

		public QuikLivesimBatchPublisher(QuikLivesimStreaming quikLivesimStreaming) {
			this.quikLivesimStreaming	= quikLivesimStreaming;

			this.ddeTableGeneratorQuotes		= new DdeTableGeneratorQuotes(this.ddeService,	this.ddeTopicQuotes	, this.quikLivesimStreaming);

			if (this.quikLivesimStreaming.DataSource.Symbols.Count != 1) {
				string msg = "LIVESIM_DATASOURCE_MUST_CONTAIN_ONE_SYMBOL_YOU_ARE_BACKTESTING";	// and in the future many symbols, for multi-symbol-within-same-datasource strategies
				Assembler.PopupException(msg);
			} else {
				this.symbolSingleImLivesimmingDepth	= this.quikLivesimStreaming.DataSource.Symbols[0];
				this.ddeTableGeneratorDepth			= new DdeTableGeneratorDepth (this.ddeService,	this.ddeTopicDepth	, this.quikLivesimStreaming);
			}
		}

		internal void SendLevelTwo_DdeClientPokesDdeServer_waitServerProcessed(
				ConcurrentDictionaryGeneric<double, double> levelTwoAsks,
				ConcurrentDictionaryGeneric<double, double> levelTwoBids) {

			if (this.ddeTableGeneratorDepth == null) {
				string msg = "I_REFUSE_TO_SendLevelTwo()__DATASOURCE_DIDNT_CONTAIN_ANY_SYMBOLS_TO_LAUNCH_DdeClient_FOR";
				Assembler.PopupException(msg);
				return;
			}
			this.ddeTableGeneratorDepth.Send_DdeClientPokesDdeServer_waitServerProcessed(levelTwoAsks, levelTwoBids);
		}

		internal void SendQuote_DdeClientPokesDdeServer_waitServerProcessed(QuoteGenerated quote) {
			this.ddeTableGeneratorQuotes.Send_DdeClientPokesDdeServer_waitServerProcessed(quote);
		}

		public override string ToString() {
			return "QuikLivesimDdeClient[" + this.ddeService + "]";
		}

		internal void ConnectAll() {
			this.ddeTableGeneratorQuotes	.Connect();
			this.ddeTableGeneratorDepth		.Connect();
			//this.ddeClientTrades	.Connect();
		}

		internal void DisconnectAll() {
			this.ddeTableGeneratorQuotes	.Disconnect();
			this.ddeTableGeneratorDepth		.Disconnect();
			//this.ddeClientTrades	.Disconnect();
		}

		internal void DisposeAll() {
			this.ddeTableGeneratorQuotes	.Dispose();
			this.ddeTableGeneratorDepth		.Dispose();
			//this.ddeClientTrades	.Dispose();
		}
	}
}
