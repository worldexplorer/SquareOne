using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Backtesting;
using Sq1.Core.Livesim;

namespace Sq1.Core.Streaming {
	public class SymbolChannel : IDisposable {
				string					symbol;
		public	string					ReasonIwasCreated_propagatedFromDistributor		{ get; private set; }
		public	Distributor				Distributor										{ get; private set; }

				object					lockStreamsDictionary;

		public	QueuePerSymbol<Quote>	QueueWhenBacktesting_PumpForLiveAndLivesim		{ get; protected set; }
		public	PumpPerSymbol<Quote>	QuotePump_nullUnsafe							{ get { return this.QueueWhenBacktesting_PumpForLiveAndLivesim as PumpPerSymbol<Quote>; } }
		public	bool					ImQueueNotPump_trueOnlyForBacktest				{ get { return this.QuotePump_nullUnsafe == null; } }
				bool					pump_separatePushingThread_enabled;

		public	Dictionary<BarScaleInterval, SymbolScaleStream>	StreamsByScaleInterval	{ get; protected set; }
		public	List<SymbolScaleStream>							AllStreams_safeCopy		{ get { lock (this.lockStreamsDictionary) {
			return new List<SymbolScaleStream>(this.StreamsByScaleInterval.Values);
		} } }

		~SymbolChannel() { this.Dispose(); }
		SymbolChannel() {
			lockStreamsDictionary					= new object();
			backtestersRunning_causingPumpingPause	= new List<Backtester>();
			StreamsByScaleInterval					= new Dictionary<BarScaleInterval, SymbolScaleStream>();
		}

		public SymbolChannel(Distributor dataDistributor, string symbol,
							bool quotePumpSeparatePushingThreadEnabled, string reasonIwasCreated) : this() {
			this.Distributor = dataDistributor;
			this.symbol = symbol;
			this.pump_separatePushingThread_enabled = quotePumpSeparatePushingThreadEnabled;

			if (string.IsNullOrEmpty(reasonIwasCreated)) {
				string msg = "WITHOUT_reasonIwasCreated_PUMP_AND_STREAMS_WILL_THROW";
				Assembler.PopupException(msg);
			}
			this.ReasonIwasCreated_propagatedFromDistributor = reasonIwasCreated;

			if (quotePumpSeparatePushingThreadEnabled) {
				this.QueueWhenBacktesting_PumpForLiveAndLivesim = new PumpPerSymbol<Quote>(this);
			} else {
				this.QueueWhenBacktesting_PumpForLiveAndLivesim = new QueuePerSymbol<Quote>(this);
			}
		}


		public void PushQuote_toStreams(Quote quoteDequeued_singleInstance_tillStreamBindsAll) {
			foreach (SymbolScaleStream stream in this.StreamsByScaleInterval.Values)  {
				try {
					stream.PushQuote_toConsumers(quoteDequeued_singleInstance_tillStreamBindsAll);
				} catch (Exception ex) {
					string msg = "STREAM_THREW [" + stream + "] WHILE_PUSHING quoteDequeued_singleInstance_tillStreamBindsAll[" + quoteDequeued_singleInstance_tillStreamBindsAll + "]";
					Assembler.PopupException(msg, ex);
				}
			}
		}

		public void PushQuote_viaPumpOrQueue(Quote quoteDequeued_singleInstance_tillStreamBindsAll) {
			Quote quote = quoteDequeued_singleInstance_tillStreamBindsAll;
			if (string.IsNullOrEmpty(quote.Symbol)) {
				Assembler.PopupException("quote.Symbol[" + quote.Symbol + "] is null or empty, returning");
				return;
			}
			if (quote.Symbol != this.symbol) {
				Assembler.PopupException("quote.Symbol[" + quote.Symbol + "] != this.Symbol[" + this.symbol + "], returning");
				return;
			}

			//v1 this.PushQuoteToConsumers(quoteSernoEnrichedWithUnboundStreamingBar);
			//v2 let the user re-backtest during live streaming using 1) QuotePump.OnHold=true; 2) RunBacktest(); 3) QuotePump.OnHold=false;
			int straightOrBuffered = this.QueueWhenBacktesting_PumpForLiveAndLivesim.Push_straightOrBuffered(quote);
			if (this.QueueWhenBacktesting_PumpForLiveAndLivesim.HasSeparatePushingThread) {
				int pushedBuffered = straightOrBuffered;
			} else {
				int pushedStraight = straightOrBuffered;
			}
		}

		public bool ConsumerQuoteAdd(BarScaleInterval scaleInterval, StreamingConsumer quoteConsumer) { lock (this.lockStreamsDictionary) {
			if (this.StreamsByScaleInterval.ContainsKey(scaleInterval) == false) {
				SymbolScaleStream newScaleChannel = new SymbolScaleStream(this, symbol, scaleInterval, this.ReasonIwasCreated_propagatedFromDistributor);
				this.StreamsByScaleInterval.Add(scaleInterval, newScaleChannel);
			}
			SymbolScaleStream stream = this.StreamsByScaleInterval[scaleInterval];
			return stream.ConsumerQuoteAdd(quoteConsumer);
		} }

		public bool ConsumerQuoteRemove(BarScaleInterval scaleInterval, StreamingConsumer quoteConsumer) { lock (this.lockStreamsDictionary) {
			if (this.StreamsByScaleInterval.ContainsKey(scaleInterval) == false) {
				string msg = "I_REFUSE_TO_REMOVE__SCALE_INTERVAL_NOT_SUBSCRIBED [" + scaleInterval + "] for [" + quoteConsumer + "]"
					+ " NOT_FOUND_AMONG_[" + this.ConsumersQuoteAsString + "]";
				Assembler.PopupException(msg);
				return false;
			}
			SymbolScaleStream stream = this.StreamsByScaleInterval[scaleInterval];
			bool removed = stream.ConsumerQuoteRemove(quoteConsumer);
			if (stream.ConsumersBarCount == 0 && stream.ConsumersQuoteCount == 0) {
				//Assembler.PopupException("QuoteConsumer [" + consumer + "] was the last one using [" + scaleInterval + "]; removing StreamsByScaleInterval[" + channel + "]");
				this.StreamsByScaleInterval.Remove(scaleInterval);
			}
			return removed;
		} }

		public bool ConsumerQuoteIsSubscribed(BarScaleInterval scaleInterval, StreamingConsumer quoteConsumer) { lock (this.lockStreamsDictionary) {
			bool ret = false;
			if (this.StreamsByScaleInterval.ContainsKey(scaleInterval) == false) {
				string msg = "I_REFUSE_TO_CHECK_SUBSCRIBED__SCALE_INTERVAL_NOT_SUBSCRIBED [" + scaleInterval + "] for [" + quoteConsumer + "]"
					+ " NOT_FOUND_AMONG_[" + this.ConsumersQuoteAsString + "]";
				Assembler.PopupException(msg);
				return false;
			}
			SymbolScaleStream stream = this.StreamsByScaleInterval[scaleInterval];
			ret = stream.ConsumersQuoteContains(quoteConsumer);
			return ret;
		} }

		public bool ConsumerQuoteIsSubscribed(StreamingConsumer quoteConsumer) { lock (this.lockStreamsDictionary) {
			bool ret = false;
			foreach (BarScaleInterval scaleInterval in this.StreamsByScaleInterval.Keys)  {
				ret &= this.ConsumerQuoteIsSubscribed(scaleInterval, quoteConsumer);
			}
			return ret;
		} }



		public bool ConsumerBarAdd(BarScaleInterval scaleInterval, StreamingConsumer barConsumer) { lock (this.lockStreamsDictionary) {
			if (this.StreamsByScaleInterval.ContainsKey(scaleInterval) == false) {
				SymbolScaleStream newScaleChannel = new SymbolScaleStream(this, symbol, scaleInterval, this.ReasonIwasCreated_propagatedFromDistributor);
				this.StreamsByScaleInterval.Add(scaleInterval, newScaleChannel);
			}
			SymbolScaleStream stream = this.StreamsByScaleInterval[scaleInterval];
			return stream.ConsumerBarAdd(barConsumer);
		} }

		public bool ConsumerBarRemove(BarScaleInterval scaleInterval, StreamingConsumer barConsumer) { lock (this.lockStreamsDictionary) {
			if (this.StreamsByScaleInterval.ContainsKey(scaleInterval) == false) {
				string msg = "I_REFUSE_TO_REMOVE__SCALE_INTERVAL_NOT_SUBSCRIBED [" + scaleInterval + "] for [" + barConsumer + "]"
					+ " NOT_FOUND_AMONG_[" + this.ConsumersBarAsString + "]";
				Assembler.PopupException(msg);
				return false;
			}
			SymbolScaleStream stream = this.StreamsByScaleInterval[scaleInterval];
			bool removed = stream.ConsumerBarRemove(barConsumer);
			if (stream.ConsumersBarCount == 0 && stream.ConsumersBarCount == 0) {
				//Assembler.PopupException("BarConsumer [" + consumer + "] was the last one using [" + scaleInterval + "]; removing StreamsByScaleInterval[" + channel + "]");
				this.StreamsByScaleInterval.Remove(scaleInterval);
			}
			return removed;
		} }

		public bool ConsumerBarIsSubscribed(BarScaleInterval scaleInterval, StreamingConsumer barConsumer) { lock (this.lockStreamsDictionary) {
			bool ret = false;
			if (this.StreamsByScaleInterval.ContainsKey(scaleInterval) == false) {
				string msg = "I_REFUSE_TO_CHECK_SUBSCRIBED__SCALE_INTERVAL_NOT_SUBSCRIBED [" + scaleInterval + "] for [" + barConsumer + "]"
					+ " NOT_FOUND_AMONG_[" + this.ConsumersBarAsString + "]";
				Assembler.PopupException(msg);
				return false;
			}
			SymbolScaleStream stream = this.StreamsByScaleInterval[scaleInterval];
			ret = stream.ConsumersBarContains(barConsumer);
			return ret;
		} }

		public bool ConsumerBarIsSubscribed(StreamingConsumer barConsumer) { lock (this.lockStreamsDictionary) {
			bool ret = false;
			foreach (BarScaleInterval scaleInterval in this.StreamsByScaleInterval.Keys)  {
				ret &= this.ConsumerBarIsSubscribed(scaleInterval, barConsumer);
			}
			return ret;
		} }



		public int ConsumersBarCount { get { lock (this.lockStreamsDictionary) {
			int ret = 0;
			foreach (SymbolScaleStream stream in this.StreamsByScaleInterval.Values)  {
				ret += stream.ConsumersBarCount;
			}
			return ret;
		} } }
		public int ConsumersQuoteCount { get { lock (this.lockStreamsDictionary) {
			int ret = 0;
			foreach (SymbolScaleStream stream in this.StreamsByScaleInterval.Values)  {
				ret += stream.ConsumersQuoteCount;
			}
			return ret;
		} } }


		public List<StreamingConsumer> ConsumersBar { get { lock (this.lockStreamsDictionary) {
			List<StreamingConsumer> ret = new List<StreamingConsumer>();
			foreach (SymbolScaleStream stream in this.StreamsByScaleInterval.Values)  {
				ret.AddRange(stream.ConsumersBar);
			}
			return ret;
		} } }

		public List<StreamingConsumer> ConsumersQuote { get { lock (this.lockStreamsDictionary) {
			List<StreamingConsumer> ret = new List<StreamingConsumer>();
			foreach (SymbolScaleStream stream in this.StreamsByScaleInterval.Values)  {
				ret.AddRange(stream.ConsumersQuote);
			}
			return ret;
		} } }
		public List<StreamingConsumer> Consumers { get { lock (this.lockStreamsDictionary) {
			List<StreamingConsumer> ret = new List<StreamingConsumer>(this.ConsumersQuote);
			foreach (StreamingConsumer consumer in this.ConsumersBar) {
				if (ret.Contains(consumer)) continue;
				ret.Add(consumer);
			}
			return ret;
		} } }


		public string ConsumersQuoteAsString { get { lock (this.lockStreamsDictionary) {
			string ret = "";
			foreach (StreamingConsumer consumer in this.ConsumersQuote) {
				if (ret != "") ret += ", ";
				ret += consumer.ToString();
			}
			return ret;
		} } }
		public string ConsumersBarAsString { get { lock (this.lockStreamsDictionary) {
			string ret = "";
			foreach (StreamingConsumer consumer in this.ConsumersBar) {
				if (ret != "") ret += ", ";
				ret += consumer.ToString();
			}
			return ret;
		} } }



		public string ConsumerNames { get { lock (this.lockStreamsDictionary) {
			return this.ConsumersQuoteNames + this.ConsumersBarNames;
		} } }

		public string ConsumersQuoteNames { get { lock (this.lockStreamsDictionary) {
			string ret = "";
			foreach (StreamingConsumer consumer in this.ConsumersQuote) {
				if (ret != "") ret += ",";
				ret += consumer.ReasonToExist;
			}
			return ret;
		} } }
		public string ConsumersBarNames { get { lock (this.lockStreamsDictionary) {
			string ret = "";
			foreach (StreamingConsumer consumer in this.ConsumersBar) {
				if (ret != "") ret += ",";
				ret += consumer.ReasonToExist;
			}
			return ret;
		} } }

		public string ToString() { lock (this.lockStreamsDictionary) {
			string ret = "";
			foreach (StreamingConsumer consumer in this.Consumers) {
				if (ret != "") ret += ",";
				ret += consumer.ReasonToExist;
			}
			return ret;
		} }



				List<Backtester>			backtestersRunning_causingPumpingPause;
		public void QueueOrPumpPause_addBacktesterLaunchingScript_eachQuote(Backtester backtesterOrLivesimAdding) {	// POTENTINALLY_THREAD_UNSAFE lock(this.lockPump) {
			bool addedFirstBacktester = false;
			// #1/3 add to backtesters running me
			if (this.backtestersRunning_causingPumpingPause.Contains(backtesterOrLivesimAdding)) {
				string msg = "ADD_BACKTESTER_ONLY_ONCE [" + backtesterOrLivesimAdding + "]"
					+ " 1)YOU_INVOKE_Script.OnNewQuote()_WITHOUT_WAITING_FOR_IT_TO_FINISH_PREV_INVOCATION (TRYING_TO_ASSURE_NON_REENTERABILITY_OF_SCRIPT_HOOKS_HERE)"
					+ " 2)Script.OnNewQuote()_THREW_EXCEPTION_AND_YOU_DIDNT_CATCH_IT_AND_DIDNT_REMOVE_BACKTESTER_POSSIBLY_DIDNT_UNPAUSE";
				Assembler.PopupException(msg);
				return;
			} else {
				this.backtestersRunning_causingPumpingPause.Add(backtesterOrLivesimAdding);
				if (this.backtestersRunning_causingPumpingPause.Count == 1) {
					addedFirstBacktester = true;
				}
			}
			// #2/3 livesim => don't pause, just exit
			if (backtesterOrLivesimAdding is Livesimulator) {
				//if (this.ImPumpNotQueue) {
					string msg = "YOU_RUINED_THE_CONCEPT_OF_HAVING_LIVESIM_AS_A_TEST_FOR_LIVE_STREAMING"
						//+ " YOU_SUBSCRIBED_LIVESIM_BARS_IN_SINGLE_THREADED_QUEUE__MUST_BE_A_PUMP_JUST_LIKE_FOR_QUIK_STREAMING"
						+ " SimulationPreBarsSubstitute_overrideable()<=Livesimulator.cs:105"
						+ " ANYWAY_NO_NEED_TO_PAUSE_COMPETITORS NO_COMPETITORS_FOR_LIVESIM_BAR_EVENTS";
					Assembler.PopupException(msg);
				//}
				return;
			}
			// #3/3 it's a backtest => pause
			if (this.QueueWhenBacktesting_PumpForLiveAndLivesim.Paused == true) {
				if (addedFirstBacktester) {
					string msg = "WE_ARE_ON_APP_RESTART_RIGHT??? ALL_PUMPS_ARE_BORN_PAUSED  backtesterAdding=[" + backtesterOrLivesimAdding + "]";
				} else {
					string msg = "PUMP_ALREADY_PAUSED_BY_ANOTHER_CONSUMERS_BACKTEST backtesterAdding=[" + backtesterOrLivesimAdding + "]";
					Assembler.PopupException(msg, null, false);
					//return;
				}
			} else {
				this.QueueWhenBacktesting_PumpForLiveAndLivesim.PusherPause_waitUntilPaused();
			}
		}
		public void QueueOrPumpResume_removeBacktesterFinishedScript_eachQuote(Backtester backtesterOrLivesimRemoving) {
			// #1/3 remove from backtesters running me, because backtest/livesim terminated anyway
			if (this.backtestersRunning_causingPumpingPause.Contains(backtesterOrLivesimRemoving)) {
				this.backtestersRunning_causingPumpingPause.Remove(backtesterOrLivesimRemoving);
			} else {
				string msg = "YOU_NEVER_ADDED_BACKTESTER [" + backtesterOrLivesimRemoving + "]";
				Assembler.PopupException(msg);
				//return;
			}
			// #2/3 livesim => don't unpause, just exit
			if (backtesterOrLivesimRemoving is Livesimulator) {
				//if (this.ImPumpNotQueue) {
					string msg = "YOU_RUINED_THE_CONCEPT_OF_HAVING_LIVESIM_AS_A_TEST_FOR_LIVE_STREAMING"
						//+ " YOU_SUBSCRIBED_LIVESIM_BARS_IN_SINGLE_THREADED_QUEUE__MUST_BE_A_PUMP_JUST_LIKE_FOR_QUIK_STREAMING"
						+ " SimulationPreBarsSubstitute_overrideable()<=Livesimulator.cs:105"
						+ " ANYWAY_NO_NEED_TO_UNPAUSE_COMPETITORS NO_COMPETITORS_FOR_LIVESIM_BAR_EVENTS";
					Assembler.PopupException(msg);
				//}
				return;
			}

			// #3/3 it's a backtest => pause
			if (this.QueueWhenBacktesting_PumpForLiveAndLivesim.Paused == false) {
				string msg = "YOU_RUINED_WHOLE_IDEA_OF_DISTRIBUTOR_CHANNEL_TO_AUTORESUME_ITS_OWN_PUMP backtesterRemoving=[" + backtesterOrLivesimRemoving + "]";
				Assembler.PopupException(msg);
				return;
			}
			if (this.backtestersRunning_causingPumpingPause.Count > 0) {
				string msg = "STILL_HAVE_ANOTHER_BACKTEST_RUNNING_IN_PARALLEL__WILL_UNPAUSE_PUMP_AFTER_LAST_TERMINATES"
					+ " backtestersRunningCausingPumpingPause.Count=[" + this.backtestersRunning_causingPumpingPause.Count + "]"
					+ " after backtesterRemoved[" + backtesterOrLivesimRemoving + "]";
				Assembler.PopupException(msg);
				return;
			}
			this.QueueWhenBacktesting_PumpForLiveAndLivesim.PusherUnpause_waitUntilUnpaused();
		}



		public bool IsDisposed { get; private set; }
		public void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE__" + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
			if (this.QuotePump_nullUnsafe != null) {
				this.QuotePump_nullUnsafe.Dispose();
			}
			this.IsDisposed = true;
		}
	}
}
