using System;
using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Static;
using Sq1.Core.StrategyBase;
using Sq1.Core.Streaming;

namespace Sq1.Gui.Forms {
	public class ChartFormStreamingConsumer : IStreamingConsumer {
		//public event EventHandler<BarEventArgs> NewBar;
		//public event EventHandler<QuoteEventArgs> NewQuote;
		//public event EventHandler<BarsEventArgs> BarsLocked;
		ChartFormManager chartFormManager;
		string msigForNpExceptions = "Failed to StartStreaming(): ";
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
				string symbol = this.ScriptContextCurrent.Symbol;
				if (String.IsNullOrEmpty(symbol)) {
					this.action("this.chartFormsManager.Executor.Strategy.ScriptContextCurrent.Symbol IsNullOrEmpty");
				}
				return symbol;
			} }
		BarScaleInterval ScaleInterval { get {
				var ret = this.ScriptContextCurrent.ScaleInterval;
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
		BarDataRange DataRange { get {
				var ret = this.ScriptContextCurrent.DataRange;
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Strategy.ScriptContextCurrent.DataRange=null");
				return ret;
			} }
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
				this.actionForNullPointer(ret, "this.chartFormsManager.Executor.Bars.StreamingBarSafeClone=null");
				return ret;
			} }
		Bar LastStaticBar { get {
				var ret = this.Bars.BarStaticLast;
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


		public ChartFormStreamingConsumer(ChartFormManager chartFormManager) {
			this.chartFormManager = chartFormManager;
		}
		public void StopStreaming() {
			this.msigForNpExceptions = "StartStreaming(): ";
			
			var executorSafe = this.Executor;
			var symbolSafe = this.Symbol;
			var streamingSafe = this.StreamingProvider;
			
			executorSafe.IsStreaming = false;
			streamingSafe.ConsumerUnregisterDead(this);
		}
		bool canStartStreaming() {
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
		public void StartStreaming() {
			//if (canStartStreaming() == false) return;
			
			this.msigForNpExceptions = "StartStreaming(): ";
			var executorSafe = this.Executor;
			var symbolSafe = this.Symbol;
			var scaleIntervalSafe = this.ScaleInterval;
			var streamingSafe = this.StreamingProvider;
			var streamingBarSafeCloneSafe = this.StreamingBarSafeClone;
			var lastStaticBarSafe = this.LastStaticBar;
			var chartFormSafe = this.ChartForm;
			
			string plug = "{this.ChartForm.Symbol[" + symbolSafe + "] + CHART[" + chartFormSafe.Text + "]'s (" + scaleIntervalSafe + ")} ";
			
			if (streamingSafe == null) {
				Assembler.PopupException("STREAMING_PROVIDER_NOT_ASSIGNED_IN_DATASOURCE [" + this + "] to " + plug);
				return;
			}
			
			executorSafe.IsStreaming = true;
			
			if (streamingSafe.ConsumerQuoteIsRegistered(symbolSafe, scaleIntervalSafe, this) == true) {
				Assembler.PopupException("ALREADY Subscribed QuoteConsumer [" + this + "] to " + plug);
			} else {
				//Assembler.PopupException("Subscribing QuoteConsumer [" + this + "]  to " + plug + "  (wasn't registered)");
				streamingSafe.ConsumerQuoteRegister(symbolSafe, scaleIntervalSafe, this);
			}

			if (streamingSafe.ConsumerBarIsRegistered(symbolSafe, scaleIntervalSafe, this) == true) {
				Assembler.PopupException("ALREADY Subscribed BarConsumer [" + this + "] to " + plug);
			} else {
				//Assembler.PopupException("Subscribing BarsConsumer [" + this + "] to " + plug + " (wasn't registered)");
				if (this.chartFormManager.Executor.Bars == null) {
					// in Initialize() this.ChartForm is requesting bars in a separate thread
					streamingSafe.ConsumerBarRegister(symbolSafe, scaleIntervalSafe, this);
				} else {
					// fully initialized, after streaming was stopped for a moment and resumed - append into PartialBar
					if (Double.IsNaN(streamingBarSafeCloneSafe.Open) == false) {
						//streamingSafe.ConsumerBarRegister(symbolSafe, scaleIntervalSafe, this, streamingBarSafeCloneSafe);
						streamingSafe.ConsumerBarRegister(symbolSafe, scaleIntervalSafe, this);
					} else {
						//streamingSafe.ConsumerBarRegister(symbolSafe, scaleIntervalSafe, this, lastStaticBarSafe);
						streamingSafe.ConsumerBarRegister(symbolSafe, scaleIntervalSafe, this);
					}
				}
			}
		}
		#region IStreamingConsumer
		Bars IStreamingConsumer.ConsumerBarsToAppendInto { get { return this.chartFormManager.Executor.Bars; } }
		void IStreamingConsumer.ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(Bar barLastFormed) {
			this.msigForNpExceptions = "ConsumeBarLastFormed(): ";
			var barsSafe = this.Bars;
			var chartFormSafe = this.ChartForm;
			var executorSafe = this.Executor;
			
			//CHART_CONTROL_REGISTERED_HIMSELF_FOR_BAR_EVENTS_THANX chartFormSafe.ChartControl.UpdateHorizontalScrollMaximumAfterBarAdd();
			//if (this.NewBar != null) this.NewBar(this, new BarEventArgs(barLastFormed));
			//this.chartFormsManager.Executor.onNewBarEnqueueExecuteStrategyInNewThread(this, new BarEventArgs(barLastFormed));
			executorSafe.ExecuteOnNewBarOrNewQuote(null);	//new Quote());
			chartFormSafe.ChartControl.InvalidateAllPanelsFolding();
		}
		void IStreamingConsumer.ConsumeQuoteOfStreamingBar(Quote quote) {
			this.msigForNpExceptions = "ConsumeFreshQuote(): ";
			var barsSafe = this.Bars;
			var streamingSafe = this.StreamingProvider;
			var chartFormSafe = this.ChartForm;
			var executorSafe = this.Executor;
			//var rendererSafe = this.Renderer;

			try {
				// COMPILATION_ERROR streamingSafe.InitializeStreamingOHLCVfromStreamingProvider(ConsumerBarsToAppendInto);
				streamingSafe.InitializeStreamingOHLCVfromStreamingProvider(this.chartFormManager.Executor.Bars);
			} catch (Exception e) {
				Assembler.PopupException("didn't merge with Partial, continuing", e);
			}

			//this.ChartForm.Chart.UpdatePartialAndHorizontalScrollMaximum(qd.BarFactory.CurrentBar);
			if (quote.ParentStreamingBar.ParentBarsIndex > quote.ParentStreamingBar.ParentBars.Count) {
				string msg = "should I add a bar into Chart.Bars?... NO !!! already added";
			}
			//if (this.NewQuote != null) this.NewQuote(this, new QuoteEventArgs(quote));
			//this.chartFormsManager.Executor.onNewQuoteEnqueueExecuteStrategyInNewThread(this, new QuoteEventArgs(quote));
			
			// launch update in GUI thread
			chartFormSafe.PrintQuoteTimestampsOnStreamingButtonBeforeExecution(quote);
			// execute strategy in the thread of a StreamingProvider (DDE server for MockQuickProvider)
			executorSafe.ExecuteOnNewBarOrNewQuote(quote);
			// trigger GUI to repaint the chart with new positions and bid/ask lines
			chartFormSafe.ChartControl.InvalidateAllPanelsFolding();
			//rendererSafe.DrawBidAskLines = true;
		}
		#endregion
		public override string ToString() {
			this.msigForNpExceptions = "ToString(): ";
			if (this.chartFormManager == null) return "ChartStreamingConsumer::chartFormsManager=null";
			if (this.chartFormManager.ChartForm == null) return "ChartStreamingConsumer::chartFormsManager.ChartForm=null";
			if (this.chartFormManager.ChartForm.IsDisposed) return "CHARTFORM_DISPOSED";
			if (this.chartFormManager.Executor == null) return "ChartStreamingConsumer::chartFormsManager.Executor=null";
			if (this.chartFormManager.Executor.Strategy == null) return "ChartStreamingConsumer::chartFormsManager.Executor.Strategy=null";
			if (this.chartFormManager.Executor.Strategy.ScriptContextCurrent == null) return "ChartStreamingConsumer::chartFormsManager.Executor.Strategy.ScriptContextCurrent=null";
			if (String.IsNullOrEmpty(this.chartFormManager.Executor.Strategy.ScriptContextCurrent.Symbol)) return "SYMBOL_EMPTY_NOT_SUBSCRIBED";
			return "StreamingChartManager: ";
//				+ "[" + this.chartFormsManager.Executor.Strategy.ScriptContextCurrent.Symbol + " " + this.chartFormsManager.Executor.Strategy.ScriptContextCurrent.ScaleInterval + "]"
//				//+ " chart[" + this.ChartContainer.Text + "]"
//				+ " streaming[" + this.chartFormsManager.Executor.DataSource.StreamingProvider.Name + "]"
//				+ " static[" + this.chartFormsManager.Executor.DataSource.StaticProvider.Name + "]";
		}
	}
}