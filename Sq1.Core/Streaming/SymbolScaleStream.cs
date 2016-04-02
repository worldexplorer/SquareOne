using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Backtesting;
using Sq1.Core.Livesim;
using Sq1.Core.Charting;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStream {
		public	string						ReasonIwasCreated_propagatedFromDistributor;
				SymbolChannel				symbolChannel_parent;
		public	string						Symbol							{ get; protected set; }
		public	BarScaleInterval			ScaleInterval					{ get; protected set; }

		//NB#1	QuotePump.PushStraightOrBuffered replaced this.PushQuoteToConsumers to:
		//		1) set Streaming free without necessity to wait for Script.OnNewQuote/Bar and deliver the next quote ASAP;
		//		2) pause the Live trading and re-Backtest with new parameters imported from Sequencer, and continue Live with them (handling open positions at the edge NYI)
		//NB#2	QuotePump.PushConsumersPaused will freeze max all opened charts and one Solidifier per DataSource:Symbol:ScaleInterval;
		//		ability to control on per-consumer level costs more, including dissync between Solidifier.BarsStored and Executor.BarsInMemory
		//public	QuoteQueuePerSymbol			QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim	{ get; protected set; }
		//public	QuotePumpPerSymbol			QuotePump_nullUnsafe									{ get { return this.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim as QuotePumpPerSymbol; } }
		//public	bool						ImQueueNotPump_trueOnlyForBacktest						{ get { return this.QuotePump_nullUnsafe == null; } }

		SymbolScaleStream() {
			lockConsumersQuote	= new object();
			lockConsumersBar	= new object();
			ConsumersQuote		= new List<StreamingConsumer>();
			ConsumersBar		= new List<StreamingConsumer>();
			binderPerConsumer	= new Dictionary<StreamingConsumer, BinderAttacherPerConsumer>();
			//QuotePump = new QuotePump(this);
			// avoiding YOU_FORGOT_TO_INVOKE_INDICATOR.INITIALIZE()_OR_WAIT_UNTIL_ITLLBE_INVOKED_LATER
			// Assembler instantiates StreamingAdapters early enough so these horses 
			// NOPE_ON_APP_RESTART_BACKTESTER_COMPLAINS_ITS_ALREADY_PAUSED
			// moved BacktesterRunningAdd() to QuotePump.PushConsumersPaused = true;
		}
		public SymbolScaleStream(SymbolChannel symbolChannel, string symbol, BarScaleInterval scaleInterval /*, bool quotePumpSeparatePushingThreadEnabled*/
						, string reasonIwasCreated = "REASON_UNKNOWN") : this() {
			this.symbolChannel_parent = symbolChannel;
			this.Symbol = symbol;
			this.ScaleInterval = scaleInterval;
			this.ReasonIwasCreated_propagatedFromDistributor = reasonIwasCreated;

			this.UnattachedStreamingBar_factory = new UnattachedStreamingBar_factory(symbol, ScaleInterval);

			//v1
			//// delayed start to 1) give BacktestStreaming-created channels to not start PumpThread (both architecturally and for linear call stack in debugger)
			//// 2) set Thread.CurrentThread.Name = QuotePump.channel.ToString() ( == this[SymbolScaleDistributionChannel].ToString), without subscribers it looks lame now
			//if (this.QuotePump.SeparatePushingThreadEnabled != quotePumpSeparatePushingThreadEnabled) {
			//	string msg = "I_MADE_IT_PROTECTED_TO_SET_THREAD_NAME_FOR_EASIER_DEBUGGING_IN_VISUAL_STUDIO";
			//	//this.QuotePump.SeparatePushingThreadEnabled  = quotePumpSeparatePushingThreadEnabled;
			//}
			//v2 : SET_THREAD_NAME_FOR_EASIER_DEBUGGING_IN_VISUAL_STUDIO 1) default constructor this() made private, 2) QuotePump knows if it should launch pusherEntryPoint() now
			//QuotePump = new QuotePumpPerChannel(this, quotePumpSeparatePushingThreadEnabled);
			//v3 UNMESSING_QuotePump
			//if (quotePumpSeparatePushingThreadEnabled) {
			//    this.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim = new QuotePumpPerSymbol(this);
			//} else {
			//    this.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim = new QuoteQueuePerSymbol(this);
			//}
		}


		public string SymbolScaleInterval { get { return this.Symbol + "_" + this.ScaleInterval; } }
		public override string ToString() { return this.SymbolScaleInterval + ":Quotes[" + this.ConsumersQuoteAsString + "],Bars[" + this.ConsumersBarAsString + "]"; }
		public string ToStringShort { get { return this.SymbolScaleInterval + "(b" + this.ConsumersBar.Count + "+" + this.ConsumersQuote.Count + "q)"; } }
		public string ToStringNames { get { return this.SymbolScaleInterval + ":Quotes[" + this.ConsumersQuoteNames + "],Bars[" + this.ConsumersBarNames + "]"; } }

		internal void UpstreamSubscribedToSymbol_pokeConsumers(string symbol) {
			foreach (StreamingConsumer quoteConsumer	in this.ConsumersQuote)	quoteConsumer.UpstreamSubscribed_toSymbol_streamNotifiedMe(null);
			foreach (StreamingConsumer barConsumer		in this.ConsumersBar)	  barConsumer.UpstreamSubscribed_toSymbol_streamNotifiedMe(null);
		}
		internal void UpstreamUnSubscribedFromSymbolPokeConsumers(string symbol, Quote quoteLastReceived) {
			foreach (StreamingConsumer quoteConsumer in this.ConsumersQuote) {
				quoteConsumer.UpstreamUnsubscribed_fromSymbol_streamNotifiedMe(quoteLastReceived);
			}
			foreach (StreamingConsumer barConsumer in this.ConsumersBar) {
				if (barConsumer is StreamingSolidifier == false) {
					quoteLastReceived.Replace_myStreamingBar_withConsumersStreamingBar(barConsumer.ConsumerBars_toAppendInto.BarStreaming_nullUnsafe);
				}
				barConsumer.UpstreamUnsubscribed_fromSymbol_streamNotifiedMe(quoteLastReceived);
			}
		}

		internal SymbolScaleStream CloneFullyFunctional_withNewDictioniariesAndLists_toPossiblyRemoveMatchingConsumers() {
			SymbolScaleStream ret			= new SymbolScaleStream();
			ret.Symbol									= this.Symbol;
			ret.ScaleInterval							= this.ScaleInterval;
			ret.UnattachedStreamingBar_factory			= this.UnattachedStreamingBar_factory;
			ret.binderPerConsumer						= new Dictionary<StreamingConsumer, BinderAttacherPerConsumer>(this.binderPerConsumer);
			ret.ConsumersQuote							= new List<StreamingConsumer>(this.ConsumersQuote);
			ret.ConsumersBar							= new List<StreamingConsumer>(this.ConsumersBar);
			//ret.backtestersRunning_causingPumpingPause	= new List<Backtester>(this.backtestersRunning_causingPumpingPause);
			//ret.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim								= this.QuoteQueue_onlyWhenBacktesting_quotePumpForLiveAndSim;
			return ret;
		}
	}
}
