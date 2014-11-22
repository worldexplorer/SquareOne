using System;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Static;
using Sq1.Core.StrategyBase;
using Sq1.Core.Streaming;

namespace Sq1.Gui.Forms {
	// ANY_STRATEGY_WILL_RUN_WITH_A_CHART_ITS_NOT_A_SERVER_APPLICATION
	public class ChartFormStreamingConsumer : IStreamingConsumer {
		//public event EventHandler<BarEventArgs> NewBar;
		//public event EventHandler<QuoteEventArgs> NewQuote;
		//public event EventHandler<BarsEventArgs> BarsLocked;
		ChartFormManager chartFormManager;
		string msigForNpExceptions = "Failed to StreamingSubscribe(): ";

		#region CASCADED_INITIALIZATION_ALL_CHECKING_CONSISTENCY_FROM_ONE_METHOD begin
		ChartFormManager ChartFormManager { get {
				var ret = this.chartFormManager;
				this.actionForNullPointer(ret, "this.chartFormsManager=null");
				return ret;
			} }
		ScriptExecutor Executor { get {
				var ret = this.ChartFormManager.Executor;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor=null");
				return ret;
			} }
		Strategy Strategy { get {
				var ret = this.Executor.Strategy;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Strategy=null");
				return ret;
			} }
		ContextScript ScriptContextCurrent { get {
				var ret = this.Strategy.ScriptContextCurrent;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Strategy.ScriptContextCurrent=null");
				return ret;
			} }
		string Symbol { get {
				string symbol = (this.Executor.Strategy == null) ? this.Executor.Bars.Symbol : this.ScriptContextCurrent.Symbol;
				if (String.IsNullOrEmpty(symbol)) {
					this.action("this.chartFormsManager.Executor.Strategy.ScriptContextCurrent.Symbol IsNullOrEmpty");
				}
				return symbol;
			} }
		BarScaleInterval ScaleInterval { get {
				var ret = (this.Executor.Strategy == null) ? this.Executor.Bars.ScaleInterval : this.ScriptContextCurrent.ScaleInterval;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Strategy.ScriptContextCurrent.ScaleInterval=null");
				return ret;
			} }
		BarScale Scale { get {
				var ret = this.ScaleInterval.Scale;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Strategy.ScriptContextCurrent.ScaleInterval.Scale=null");
				if (ret == BarScale.Unknown) {
					this.action("this.chartFormsManager.Executor.Strategy.ScriptContextCurrent.ScaleInterval.Scale=Unknown");
				}
				return ret;
			} }
		//		BarDataRange DataRange { get {
		//				var ret = this.ScriptContextCurrent.DataRange;
		//				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Strategy.ScriptContextCurrent.DataRange=null");
		//				return ret;
		//			} }
		DataSource DataSource { get {
				var ret = this.Executor.DataSource;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.DataSource=null");
				return ret;
			} }
		StaticProvider StaticProvider { get {
				var ret = this.DataSource.StaticProvider;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.DataSource.StaticProvider=null");
				return ret;
			} }
		StreamingProvider StreamingProvider { get {
				StreamingProvider ret = this.DataSource.StreamingProvider;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.DataSource.StreamingProvider=null");
				return ret;
			} }
		StreamingSolidifier StreamingSolidifierDeep { get {
				var ret = this.StreamingProvider.StreamingSolidifier;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.DataSource.StreamingProvider.StreamingSolidifier=null");
				return ret;
			} }
		ChartForm ChartForm { get {
				var ret = this.ChartFormManager.ChartForm;
				this.actionForNullPointer(ret, "this.chartFormsManager.ChartForm=null");
				return ret;
			} }
		Bars Bars { get {
				var ret = this.Executor.Bars;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Bars=null");
				return ret;
			} }
		Bar StreamingBarSafeClone { get {
				var ret = this.Bars.BarStreamingCloneReadonly;
				//this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Bars.StreamingBarSafeClone=null");
				if (ret == null) ret = new Bar();
				return ret;
			} }
		Bar LastStaticBar { get {
				var ret = this.Bars.BarStaticLastNullUnsafe;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Bars.LastStaticBar=null");
				return ret;
			} }
		void actionForNullPointer(object mustBeInstance, string msgIfNull) {
			if (mustBeInstance != null) return;
			this.action(msgIfNull);
		}
		void action(string msgIfNull) {
			string msg = msigForNpExceptions + msgIfNull;
			Assembler.PopupException(msg);
			//throw new Exception(msg);
		}
		bool canSubscribeToStreamingProvider() {
			try {
				var symbolSafe = this.Symbol;
				var scaleSafe = this.Scale;
				var staticSafe = this.StaticProvider;
				var streamingSafe = this.StreamingProvider;
				var staticDeepSafe = this.StreamingSolidifierDeep;
			} catch (Exception e) {
				return false;
			}
			return true;
		}
		#endregion

		public ChartFormStreamingConsumer(ChartFormManager chartFormManager) {
			this.chartFormManager = chartFormManager;
		}
		public void StreamingSubscribe(string reason = "NO_REASON_FOR_STREAMING_SUBSCRIBE") {
			this.msigForNpExceptions = " //ChartFormStreamingConsumer.StreamingSubscribe(" + this.ToString() + ")";
			var executorSafe = this.Executor;
			var symbolSafe = this.Symbol;
			var streamingSafe = this.StreamingProvider;

			streamingSafe.ConsumerUnregisterDead(this);

			//v1 this.ChartFormManager.ContextCurrentChartOrStrategy.IsStreaming = false;
			//v2
			bool subscribed = this.Subscribed;
			if (subscribed == true) {
				string msg = "STREAMING_STILL_HAS_TWO_CONSUMERS_REGISTER StreamingProvider[" + streamingSafe.ToString() + "]";
				Assembler.PopupException(msg + this.msigForNpExceptions);
				return;
			} else {
				string msg = "STREAMING_SUBSCRIBED[" + subscribed + "] due to [" + reason + "]";
				Assembler.PopupException(msg+ this.msigForNpExceptions, null, false);
			}
			this.ChartFormManager.ContextCurrentChartOrStrategy.IsStreaming = subscribed;

			var chartFormSafe = this.ChartForm;
			chartFormSafe.ChartControl.ScriptExecutorObjects.QuoteLast = null;
		}
		public void StreamingUnsubscribe(string reason = "NO_REASON_FOR_STREAMING_UNSUBSCRIBE") {
			if (this.canSubscribeToStreamingProvider() == false) return;	// NULL_POINTERS_ARE_ALREADY_REPORTED_TO_EXCEPTIONS_FORM

			this.msigForNpExceptions = " //ChartFormStreamingConsumer.StreamingUnsubscribe(" + this.ToString() + ")";
			var executorSafe = this.Executor;
			var symbolSafe = this.Symbol;
			var scaleIntervalSafe = this.ScaleInterval;
			var streamingSafe = this.StreamingProvider;
			var streamingBarSafeCloneSafe = this.StreamingBarSafeClone;
			var chartFormSafe = this.ChartForm;

			if (streamingSafe == null) {
				Assembler.PopupException("STREAMING_PROVIDER_NOT_ASSIGNED_IN_DATASOURCE" + this.msigForNpExceptions);
				return;
			}

			bool subscribed = this.Subscribed;
			if (subscribed == true) {
				Assembler.PopupException("ALREADY_STREAMING_OR_FORGOT_TO_DISCONNECT tthis.Subscribed=true " + this.msigForNpExceptions);
				return;
			}

			//StreamingStarted_UNWRAPPED
			if (streamingSafe.ConsumerQuoteIsRegistered(symbolSafe, scaleIntervalSafe, this) == true) {
				Assembler.PopupException("ALREADY_REGISTERED_CONSUMER_QUOTE" + this.msigForNpExceptions);
			} else {
				//Assembler.PopupException("Subscribing QuoteConsumer [" + this + "]  to " + plug + "  (wasn't registered)");
				streamingSafe.ConsumerQuoteRegister(symbolSafe, scaleIntervalSafe, this);
			}

			if (streamingSafe.ConsumerBarIsRegistered(symbolSafe, scaleIntervalSafe, this) == true) {
				Assembler.PopupException("ALREADY_REGISTERED_CONSUMER_BAR" + this.msigForNpExceptions);
			} else {
				//Assembler.PopupException("Subscribing BarsConsumer [" + this + "] to " + this.ToString() + " (wasn't registered)");
				if (this.chartFormManager.Executor.Bars == null) {
					// in Initialize() this.ChartForm is requesting bars in a separate thread
					streamingSafe.ConsumerBarRegister(symbolSafe, scaleIntervalSafe, this);
				} else {
					// fully initialized, after streaming was stopped for a moment and resumed - append into PartialBar
					if (double.IsNaN(streamingBarSafeCloneSafe.Open) == false) {
						//streamingSafe.ConsumerBarRegister(symbolSafe, scaleIntervalSafe, this, streamingBarSafeCloneSafe);
						streamingSafe.ConsumerBarRegister(symbolSafe, scaleIntervalSafe, this);
					} else {
						//streamingSafe.ConsumerBarRegister(symbolSafe, scaleIntervalSafe, this, lastStaticBarSafe);
						streamingSafe.ConsumerBarRegister(symbolSafe, scaleIntervalSafe, this);
					}
				}
			}

			//v1 this.ChartFormManager.ContextCurrentChartOrStrategy.IsStreaming = true;
			//v2
			subscribed = this.Subscribed;
			if (subscribed == false) {
				string msg = "STREAMING_DIDNT_REGISTER_TWO_CONSUMERS StreamingProvider[" + streamingSafe.ToString() + "]";
				Assembler.PopupException(msg + this.msigForNpExceptions);
				return;
			} else {
				string msg = "STREAMING_SUBSCRIBED[" + subscribed + "] due to [" + reason + "]";
				Assembler.PopupException(msg+ this.msigForNpExceptions, null, false);
			}
			this.ChartFormManager.ContextCurrentChartOrStrategy.IsStreaming = subscribed;

			if (chartFormSafe.ChartControl.ScriptExecutorObjects.QuoteLast != null) {
				string msg = "SHOULD_I_CLEANUP_QUOTE_LAST?";
				Assembler.PopupException(msg + this.msigForNpExceptions);
			}
		}
		public bool Subscribed { get {
				if (this.canSubscribeToStreamingProvider() == false) return false;	// NULL_POINTERS_ARE_ALREADY_REPORTED_TO_EXCEPTIONS_FORM

				var streamingSafe = this.StreamingProvider;
				var symbolSafe = this.Symbol;
				var scaleIntervalSafe = this.ScaleInterval;

				bool ret = streamingSafe.ConsumerQuoteIsRegistered(symbolSafe, scaleIntervalSafe, this)
						&& streamingSafe.ConsumerBarIsRegistered(symbolSafe, scaleIntervalSafe, this);
				return ret;
			}}

		public void StreamingTriggeringScriptStop() {
			this.Executor.IsStreamingTriggeringScript = false;
		}
		public void StreamingTriggeringScriptStart() {
			this.Executor.IsStreamingTriggeringScript = true;
		}

		#region IStreamingConsumer
		Bars IStreamingConsumer.ConsumerBarsToAppendInto { get { return this.Bars; } }
		void IStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(Bar barLastFormed) {
			this.msigForNpExceptions = " //ChartFormStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(" + barLastFormed.ToString() + ")";

			#if DEBUG	// TEST_INLINE
			var barsSafe = this.Bars;
			if (barsSafe.ScaleInterval != barLastFormed.ScaleInterval) {
				string msg = "SCALEINTERVAL_RECEIVED_DOESNT_MATCH_CHARTS ChartForm[" + this.ChartForm.Text + "]"
					+ " bars[" + barsSafe.ScaleInterval + "] barLastFormed[" + barLastFormed.ScaleInterval + "]";
				Assembler.PopupException(msg + this.msigForNpExceptions);
				return;
			}
			if (barsSafe.Symbol != barLastFormed.Symbol) {
				string msg = "SYMBOL_RECEIVED_DOESNT_MATCH_CHARTS ChartForm[" + this.ChartForm.Text + "]"
					+ " bars[" + barsSafe.Symbol + "] barLastFormed[" + barLastFormed.Symbol + "]";
				Assembler.PopupException(msg + this.msigForNpExceptions);
				return;
			}
			#endif

			var chartFormSafe = this.ChartForm;
			var executorSafe = this.Executor;

			if (executorSafe.Strategy != null && executorSafe.IsStreamingTriggeringScript) {
				executorSafe.ExecuteOnNewBarOrNewQuote(null);	//new Quote());
			}

			if (this.ChartFormManager.ContextCurrentChartOrStrategy.IsStreaming) {
				chartFormSafe.ChartControl.InvalidateAllPanels();
			}
		}
		void IStreamingConsumer.ConsumeQuoteOfStreamingBar(Quote quote) {
			this.msigForNpExceptions = " //ChartFormStreamingConsumer.ConsumeQuoteOfStreamingBar(" + quote.ToString() + ")";

			#if DEBUG	// TEST_INLINE
			var barsSafe = this.Bars;
			if (barsSafe.ScaleInterval != quote.ParentStreamingBar.ScaleInterval) {
				string msg = "SCALEINTERVAL_RECEIVED_DOESNT_MATCH_CHARTS ChartForm[" + this.ChartForm.Text + "]"
					+ " bars[" + barsSafe.ScaleInterval + "] quote.ParentStreamingBar[" + quote.ParentStreamingBar.ScaleInterval + "]";
				Assembler.PopupException(msg + this.msigForNpExceptions);
				return;
			}
			if (barsSafe.Symbol != quote.ParentStreamingBar.Symbol) {
				string msg = "SYMBOL_RECEIVED_DOESNT_MATCH_CHARTS ChartForm[" + this.ChartForm.Text + "]"
					+ " bars[" + barsSafe.Symbol + "] quote.ParentStreamingBar[" + quote.ParentStreamingBar.Symbol + "]";
				Assembler.PopupException(msg + this.msigForNpExceptions);
				return;
			}
			#endif

			var streamingSafe = this.StreamingProvider;
			var chartFormSafe = this.ChartForm;
			var executorSafe = this.Executor;

			try {
				streamingSafe.InitializeStreamingOHLCVfromStreamingProvider(this.chartFormManager.Executor.Bars);
			} catch (Exception e) {
				Assembler.PopupException("didn't merge with Partial, continuing", e, false);
			}

			if (quote.ParentStreamingBar.ParentBarsIndex > quote.ParentStreamingBar.ParentBars.Count) {
				string msg = "should I add a bar into Chart.Bars?... NO !!! already added";
			}

			// #1/3 launch update in GUI thread
			chartFormSafe.PrintQuoteTimestampOnStrategyTriggeringButtonBeforeExecution(quote);
			chartFormSafe.ChartControl.ScriptExecutorObjects.QuoteLast = quote.Clone();

			// #2/3 execute strategy in the thread of a StreamingProvider (DDE server for MockQuickProvider)
			if (executorSafe.Strategy != null && executorSafe.IsStreamingTriggeringScript) {
				executorSafe.ExecuteOnNewBarOrNewQuote(quote);
			}

			// #3/3 trigger ChartControl to repaint candles with new positions and bid/ask lines
			if (this.ChartFormManager.ContextCurrentChartOrStrategy.IsStreaming) {
				chartFormSafe.ChartControl.InvalidateAllPanels();
			}
		}
		#endregion

		public override string ToString() {
			//v1
			//this.msigForNpExceptions = "ToString(): ";
			//if (this.chartFormManager == null) return "ChartStreamingConsumer::chartFormsManager=null";
			//if (this.chartFormManager.ChartForm == null) return "ChartStreamingConsumer::chartFormsManager.ChartForm=null";
			//if (this.chartFormManager.ChartForm.IsDisposed) return "CHARTFORM_DISPOSED";
			//if (this.chartFormManager.Executor == null) return "ChartStreamingConsumer::chartFormsManager.Executor=null";
			//if (this.chartFormManager.Executor.Strategy == null) return "ChartStreamingConsumer::chartFormsManager.Executor.Strategy=null";
			//if (this.chartFormManager.Executor.Strategy.ScriptContextCurrent == null) return "ChartStreamingConsumer::chartFormsManager.Executor.Strategy.ScriptContextCurrent=null";
			//if (String.IsNullOrEmpty(this.chartFormManager.Executor.Strategy.ScriptContextCurrent.Symbol)) return "SYMBOL_EMPTY_NOT_SUBSCRIBED";
			//return this.ChartFormManager.StreamingButtonIdent
			//    //+ " [" + this.Strategy.ScriptContextCurrent.Symbol + " " + this.Strategy.ScriptContextCurrent.ScaleInterval + "]"
			//    ////+ " chart[" + this.ChartContainer.Text + "]"
			//    //+ " streaming[" + this.chartFormsManager.Executor.DataSource.StreamingProvider.Name + "]"
			//    //+ " static[" + this.chartFormsManager.Executor.DataSource.StaticProvider.Name + "]"
			//    ;
			//v2
			var symbolSafe = this.Symbol;
			var chartFormSafe = this.ChartForm;
			var scaleIntervalSafe = this.ScaleInterval;
			string ident = "{this.ChartForm.Symbol[" + symbolSafe + "] + CHART[" + chartFormSafe.Text + "]'s (" + scaleIntervalSafe + ")}";
			return ident;
		}
	}
}