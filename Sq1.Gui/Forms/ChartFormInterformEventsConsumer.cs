using System;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.StrategyBase;
using Sq1.Core.Charting;
using Sq1.Core.Streaming;

using Sq1.Widgets.RangeBar;
using Sq1.Widgets;

using Sq1.Gui.Singletons;


namespace Sq1.Gui.Forms {
	public class ChartFormInterformEventsConsumer {
		ChartFormManager chartFormManager;
		private bool backtestAlreadyFinished;

		public ChartFormInterformEventsConsumer(ChartFormManager chartFormManager, ChartForm chartFormNotAssignedToManagerInTheFactoryYet = null) {
			this.chartFormManager = chartFormManager;
			chartFormNotAssignedToManagerInTheFactoryYet.FormClosing += ChartForm_FormClosing;
			chartFormNotAssignedToManagerInTheFactoryYet.Load += ChartForm_Load;
		}
		void ChartForm_Load(object sender, EventArgs e) {
			// ON_DESERIALIZATION_BACKTESTER_LAUNCHES_FASTER_THAN_CHART_FORM_GETS_LOADED; see ON_REQUESTING_ABORT_TASK_DIES_WITHOUT_INVOKING_CONTINUE_WITH
			this.chartFormManager.Executor.EventGenerator.OnBacktesterContextInitialized_step2of4 += this.Executor_BacktesterContextInitialized_step2of4;
			this.chartFormManager.Executor.EventGenerator.OnBacktesterSimulatedChunk_step3of4 += this.Executor_BacktesterChunkSimulated_step3of4;
			this.chartFormManager.Executor.EventGenerator.OnBacktesterContextRestoredAfterExecutingAllBars_step4of4 += this.Executor_BacktesterSimulatedAllBars_step4of4;
			//this.chartFormsManager.Executor.EventGenerator.OnBacktesterBarsChanged += this.Executor_BacktesterChangedQuotesWillGenerate;

			this.chartFormManager.Executor.EventGenerator.OnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_tunnelToInterChartForm -= new EventHandler<QuoteEventArgs>(eventGenerator_OnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_tunnelToInterChartForm);
			this.chartFormManager.Executor.EventGenerator.OnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_tunnelToInterChartForm += new EventHandler<QuoteEventArgs>(eventGenerator_OnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_tunnelToInterChartForm);
		}
		void eventGenerator_OnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_tunnelToInterChartForm(object sender, QuoteEventArgs e) {
			string msg = "DEAD_END DataSourceTree.olvTree_FormatRow() gets BackColor from ChartControl; if there is no ChartControl I have no way to transfer it; unblinker should be there, too";
		}

		void ChartForm_FormClosing(object sender, FormClosingEventArgs e) {
			this.chartFormManager.Executor.EventGenerator.OnBacktesterContextInitialized_step2of4 -= this.Executor_BacktesterContextInitialized_step2of4;
			this.chartFormManager.Executor.EventGenerator.OnBacktesterSimulatedChunk_step3of4 -= this.Executor_BacktesterChunkSimulated_step3of4;
			this.chartFormManager.Executor.EventGenerator.OnBacktesterContextRestoredAfterExecutingAllBars_step4of4 -= this.Executor_BacktesterSimulatedAllBars_step4of4;
			//this.chartFormsManager.Executor.EventGenerator.OnBacktesterBarsChanged -= this.Executor_BacktesterChangedQuotesWillGenerate;
			
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;
			string reporterCurrent = "";
			try {
				foreach (DockContent reporterWrapper in this.chartFormManager.ReportersFormsManager.FormsAllRelated.Values) {
					reporterCurrent = reporterWrapper.Text; 
					reporterWrapper.Close();
				}
				this.chartFormManager.MainForm.MainFormSerialize();
			} catch (Exception ex) {
				string msg = "REPORTER_CLOSING_FAILED reporterCurrent[" + reporterCurrent + "] COULDNT_TUNNEL_EXCEPTION_INTO_FORM_CLOSING";
				Assembler.PopupException(msg, ex);
			}
		}
		internal void DataSourcesTree_OnSymbolSelected(object sender, DataSourceSymbolEventArgs e) {
			string msig = " //DataSourcesTree_OnSymbolSelected(" + e.Symbol + ")";

			if (	this.chartFormManager.Executor.DataSource_fromBars.StreamingAdapter != null
				 && this.chartFormManager.Executor.Strategy != null
				 //&& this.chartFormManager.Executor.IsStreamingTriggeringScript
				 && this.chartFormManager.ContextCurrentChartOrStrategy.DownstreamSubscribed == true
				) {
				string msg = "I_REFUSE_TO_CHANGE_SYMBOL"
					//+ " CURRENT_CHART_HAS_STRATEGY_RUNNING_ON_STREAMING"
					+ " CURRENT_CHART_HAS_STRATEGY_SUBSCRIBED"
					+ " to prevent occasional order execution, click [" + this.chartFormManager.StreamingButtonIdent + "] button on ChartForm to unsubscribe";
				Assembler.PopupException(msg + msig, null, false);
				return;
			}

			try {
				ContextChart contextChart = this.chartFormManager.ContextCurrentChartOrStrategy;

				#region subscribed strategies are not allowed to swap the horses; chartsWithoutStrategies are updated by Bars.OnBarStreamingUpdatedMerged
				//string symbol_beforeChange			= contextChart.Symbol;
				//string dataSource_beforeChange		= contextChart.DataSourceName;
				//string reasonToSubscribeUnsubscribe = "USER_MADE_CHART?_CHANGE_BARS[" + symbol_beforeChange + "@" + dataSource_beforeChange + "]=>[" + e.Symbol + "@" + e.DataSource.Name + "]";
				//ChartStreamingConsumer	amIsubscribed				= this.chartFormManager.ChartForm.ChartControl.ChartStreamingConsumer;
				//DataDistributor			hopefullyLiveStreaming		= this.chartFormManager.Executor.DataSource_fromBars.StreamingAdapter.DataDistributor_replacedForLivesim;
				//bool chartMustBeSubscribedToNewBars = contextChart.DownstreamSubscribed;
				//if (chartMustBeSubscribedToNewBars) {
				//	BarScaleInterval		scaleInterval_beforeChange	= contextChart.ScaleInterval;
				//	//bool barsUnsubscribed	= hopefullyLiveStreaming.ConsumerBarUnsubscribe		(symbol_beforeChange, scaleInterval_beforeChange, amIsubscribed);
				//	//bool quotesUnsubscribed = hopefullyLiveStreaming.ConsumerQuoteUnsubscribe	(symbol_beforeChange, scaleInterval_beforeChange, amIsubscribed);
				//	amIsubscribed.StreamingUnsubscribe(reasonToSubscribeUnsubscribe);
				//}
				#endregion
				
				if (contextChart.DataSourceName != e.DataSource.Name)	contextChart.DataSourceName = e.DataSource.Name; 
				if (contextChart.Symbol			!= e.Symbol) 			contextChart.Symbol 		= e.Symbol;
				this.chartFormManager.PopulateSelectors_fromCurrentChartOrScriptContext_loadBars_saveStrategyOrCtx_backtestIfStrategy("DataSourcesTree_OnSymbolSelected");

				#region subscribed strategies are not allowed to swap the horses; chartsWithoutStrategies are updated by Bars.OnBarStreamingUpdatedMerged
				//if (chartMustBeSubscribedToNewBars) {
				//	string					symbol_afterChange			= contextChart.Symbol;
				//	BarScaleInterval		scaleInterval_afterChange	= contextChart.ScaleInterval;
				//	//bool barsSubscribed		= hopefullyLiveStreaming.ConsumerBarSubscribe	(symbol_afterChange, scaleInterval_afterChange, amIsubscribed, true);
				//	//bool quotesSubscribed	= hopefullyLiveStreaming.ConsumerQuoteSubscribe	(symbol_afterChange, scaleInterval_afterChange, amIsubscribed, true);
				//	amIsubscribed.StreamingSubscribe(reasonToSubscribeUnsubscribe);
				//}
				#endregion

				DataSourcesForm.Instance.DataSourcesTreeControl.Refresh();

				this.chartFormManager.SequencerFormIfOpenPropagateTextboxesOrMarkStaleResultsAndDeleteHistory();

				//copypaste from MainFormEventManager.DockPanel_ActiveDocumentChanged()
				ChartForm chartFormCurrentlyOpen = this.chartFormManager.ChartForm;
				ChartSettingsEditorForm.Instance.PopulateWithChartSettings(chartFormCurrentlyOpen.ChartControl.ChartSettings);
				if (chartFormCurrentlyOpen.ChartFormManager.Executor.Bars != null) {
					SymbolInfoEditorForm.Instance.SymbolEditorControl.PopulateWithSymbolInfo(chartFormCurrentlyOpen.ChartFormManager.Executor.Bars.SymbolInfo);
				}

			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			}
		}
		internal void MainForm_ActivateDocumentPane_WithChart(object sender, EventArgs e) {
			if (this.chartFormManager.Strategy != null && this.chartFormManager.Strategy.ActivatedFromDll == false) {
				this.chartFormManager.ScriptEditorFormConditionalInstance.ActivateDockContentPopupAutoHidden(true, true);
			}

			this.chartFormManager.ReportersFormsManager.PopupReporters_OnParentChartActivated(sender, e);
			//this.chartFormManager.ChartForm.ChartControl.RangeBar.Enabled = false;	// WHY false?? YOU_SHOULD_NOT_CHANGE_VISIBILITY_OF_RANGEBAR
			this.chartFormManager.ChartForm.ChartControl.InvalidateAllPanels();		// CHART_WAS_INVOKED_WITH_SIZE_DIFFERENT_ON_START__HELPS_TO_STRETCH_CHART_TO_ACTUAL_SIZE__COVERED_WAS_FIRST_TIME_SHOWN__ACTIVE_IS_OK
			
			//if (this.chartFormManager.SequencerForm == null) {
			if (DockContentImproved.IsNullOrDisposed(this.chartFormManager.SequencerForm) == true) {
				string msg = "don't even try to access SequencerConditionalInstance if user didn't click implicitly; TODO where to can I incapsulate it?";
				//Assembler.PopupException(msg, null, false);
			} else {
				if (this.chartFormManager.SequencerForm.IsShown) this.chartFormManager.SequencerFormShow(true);
			}

			//if (this.chartFormManager.CorrelatorForm == null) {
			if (DockContentImproved.IsNullOrDisposed(this.chartFormManager.CorrelatorForm) == true) {
				string msg = "don't even try to access CorrelatorConditionalInstance if user didn't click implicitly; TODO where to can I incapsulate it?";
				//Assembler.PopupException(msg, null, false);
			} else {
				if (this.chartFormManager.CorrelatorForm.IsShown) this.chartFormManager.CorrelatorFormShow(true);
			}

			//if (this.chartFormManager.LivesimForm == null) {
			if (DockContentImproved.IsNullOrDisposed(this.chartFormManager.LivesimForm) == true) {
				string msg = "don't even try to access LivesimConditionalInstance if user didn't click implicitly; TODO where to can I incapsulate it?";
				//Assembler.PopupException(msg, null, false);
			} else {
				if (this.chartFormManager.LivesimForm.IsShown) this.chartFormManager.LivesimFormShow(true);
			}
		}
		internal void Executor_BacktesterContextInitialized_step2of4(object sender, EventArgs e) {
			string msig = " Executor_BacktesterContextInitialized_step2of4()" + this.chartFormManager.ToString();
			if (this.chartFormManager.ChartForm == null) return;
			if (this.chartFormManager.Executor == null) return;
			if (this.chartFormManager.Executor.BacktesterOrLivesimulator.BarsOriginal == null) {
				string msg = "I_RESTORED_CONTEXT__END_OF_BACKTEST_ORIGINAL_BECAME_NULL";
				if (this.chartFormManager.ChartForm.InvokeRequired == false) {
					msg = "NO_NEED_TO_REPORT_ITS_NOT_AN_ERROR  I_REFUSE_TO_CALCULATE_PERCENTAGE_COMPLETED BACKTEST_ALREADY_FINISHED_WHILE_SWTICHING_TO_GUI_THREAD";
					//Assembler.PopupException(msg + msig, null, false);
				} else {
					Assembler.PopupException(msg + msig, null, false);
				}
				return;
			}

			if (this.chartFormManager.Executor.BacktesterOrLivesimulator.QuotesGenerator == null) return;
			int quotesTotal = this.chartFormManager.Executor.BacktesterOrLivesimulator.QuotesTotalToGenerate;
			if (quotesTotal == -1) {
				string msg = "I_RESTORED_CONTEXT__END_OF_BACKTEST_ORIGINAL_BECAME_NULL: Backtester.QuotesTotalToGenerate=-1 due to Backtester.BarsOriginal=null";
				Assembler.PopupException(msg);
				return;
			}

			this.backtestAlreadyFinished = false;
			if (this.chartFormManager.ChartForm.InvokeRequired) {
				this.chartFormManager.ChartForm.BeginInvoke(new MethodInvoker(delegate { this.Executor_BacktesterContextInitialized_step2of4(sender, e); }));
				return;
			}

			this.chartFormManager.ChartForm.TsiProgressBarETA.ETALabelText = this.chartFormManager.Executor.BacktesterOrLivesimulator.ProgressStats;
			
			// CHART_NOT_NOTIFIED_OF_BACKTEST_PROGRESS_AFTER_DESERIALIZATION_BACKTESTER_LAUNCHES_BEFORE_IM_SUBSCRIBED BEGIN

			//if (this.chartFormsManager.ChartForm.TsiProgressBarETA.ETAProgressBarMaximum != quotesTotal) {
			this.chartFormManager.ChartForm.TsiProgressBarETA.ETAProgressBarMaximum = quotesTotal;
			//}

			this.chartFormManager.ChartForm.TsiProgressBarETA.Visible = true;
			
			//this.chartFormManager.ChartForm.btnStrategyEmittingOrders.Visible = false;
			//this.chartFormManager.ChartForm.btnStreamingTriggersScript.Visible = false;
			this.chartFormManager.ChartForm.PropagateSelectorsDisabledIfStreaming_forCurrentChart();
			// CHART_NOT_NOTIFIED_OF_BACKTEST_PROGRESS_AFTER_DESERIALIZATION_BACKTESTER_LAUNCHES_BEFORE_IM_SUBSCRIBED END
			this.chartFormManager.ChartForm.AbsorbContextBarsToGui();
		}
		internal void Executor_BacktesterChunkSimulated_step3of4(object sender, EventArgs e) {
			string msig = " //Executor_BacktesterChunkSimulated_step3of4() " + this.chartFormManager.ToString();
			if (sender != this.chartFormManager.Executor.EventGenerator) return;
			if (this.chartFormManager.Executor == null) {
				string msg = "invoked by Backtester.SubstituteAndRunSimulation() I don't remember whether Tag=null is ok or not...";
				return;
			}
			//if (this.chartFormManager.Executor.Backtester.IsBacktestingNoLivesimNow == false) {
			//if (this.chartFormManager.ChartForm.ChartControl.PaintAllowedDuringLivesimOrAfterBacktestFinished == false) {
			if (this.backtestAlreadyFinished) {
				string msg = "Livesimulator.afterBacktesterComplete()_ALREADY_RESTORED_BACKTESTER_WHILE_SWITCHING_TO_GUI_THREAD [base.Executor.Backtester = this.BacktesterBackup]";
				return;
			}
			if (this.chartFormManager.Executor.BacktesterOrLivesimulator.QuotesGenerator == null) {
				string msg = "YOU_DIDNT_INVOKE_Backtester.Initialize() AVOIDING_EXCEPTIONS_IN_QuotesGeneratedSoFar";
				Assembler.PopupException(msg, null, false);
				return;
			}

			int quotesTotal = this.chartFormManager.Executor.BacktesterOrLivesimulator.QuotesTotalToGenerate;
			if (quotesTotal == -1) {
				string msg = "CANT_CALCULATE_PERCENTAGE_KOZ_BARS_ORIGINAL_NULL"
					+ " : Backtester.QuotesTotalToGenerate=-1 due to Backtester.BarsOriginal=null";
				//Assembler.PopupException(msg + msig, null, false);
				return;
			}

			if (this.chartFormManager.ChartForm.InvokeRequired) {
				this.chartFormManager.ChartForm.BeginInvoke(new MethodInvoker(delegate { this.Executor_BacktesterChunkSimulated_step3of4(sender, e); }));
				return;
			}

			// HACK FOR CHART_NOT_NOTIFIED_OF_BACKTEST_PROGRESS_AFTER_DESERIALIZATION_BACKTESTER_LAUNCHES_BEFORE_IM_SUBSCRIBED BEGIN COPYPASTE
			if (this.chartFormManager.ChartForm.TsiProgressBarETA.Visible == false) {
				//int quotesTotal = this.chartFormManager.Executor.Backtester.QuotesTotalToGenerate;
				//if (quotesTotal == -1) {
				//	string msg = "Backtester.QuotesTotalToGenerate=-1 due to Backtester.BarsOriginal=null";
				//	Assembler.PopupException(msg + msig);
				//	return;
				//}
				this.chartFormManager.ChartForm.TsiProgressBarETA.ETAProgressBarMaximum = quotesTotal;
				this.chartFormManager.ChartForm.TsiProgressBarETA.Visible = true;
				//this.chartFormManager.ChartForm.btnStrategyEmittingOrders.Visible = false;
				//this.chartFormManager.ChartForm.btnStreamingTriggersScript.Visible = false;
				this.chartFormManager.ChartForm.PropagateSelectorsDisabledIfStreaming_forCurrentChart();
			}
			// HACK FOR CHART_NOT_NOTIFIED_OF_BACKTEST_PROGRESS_AFTER_DESERIALIZATION_BACKTESTER_LAUNCHES_BEFORE_IM_SUBSCRIBED END COPYPASTE

			this.chartFormManager.ChartForm.TsiProgressBarETA.ETALabelText = this.chartFormManager.Executor.BacktesterOrLivesimulator.ProgressStats;
			

			int currentValue = this.chartFormManager.Executor.BacktesterOrLivesimulator.QuotesGeneratedSoFar;
			if (currentValue > this.chartFormManager.ChartForm.TsiProgressBarETA.ETAProgressBarMaximum) return;
			this.chartFormManager.ChartForm.TsiProgressBarETA.ETAProgressBarValue = currentValue;

			// ETALabelText isn't refreshed fast enough; windows don't feel mouse clicks&moves, GUI freezes; REMOVE after backtester goes to its own thread!
			////DEBUGGER_SHOWS_RECURSIVE_CALLS_TO_BuildOnceAllReports
			/// uncommented to make GUI more responsive during backtests; GUI didn't fully unhalt, lagging.... FIXME  
			//Application.DoEvents();
		}
		internal void Executor_BacktesterSimulatedAllBars_step4of4(object sender, EventArgs e) {
			if (this.chartFormManager.Executor == null) return;
			if (sender != this.chartFormManager.Executor.EventGenerator) return;
			this.backtestAlreadyFinished = true;

			if (this.chartFormManager.ChartForm.InvokeRequired) {
				this.chartFormManager.ChartForm.BeginInvoke(new MethodInvoker(delegate { this.Executor_BacktesterSimulatedAllBars_step4of4(sender, e); }));
				return;
			}

			this.chartFormManager.ChartForm.TsiProgressBarETA.ETALabelText = this.chartFormManager.Executor.BacktesterOrLivesimulator.ProgressStats;
			this.chartFormManager.ChartForm.TsiProgressBarETA.ETAProgressBarValue = 0;
			this.chartFormManager.ChartForm.TsiProgressBarETA.Visible = false;
			
			//this.chartFormManager.ChartForm.btnStrategyEmittingOrders.Visible = true;
			//this.chartFormManager.ChartForm.btnStreamingTriggersScript.Visible = true;
			this.chartFormManager.ChartForm.PropagateSelectorsDisabledIfStreaming_forCurrentChart();
			this.chartFormManager.ChartForm.AbsorbContextBarsToGui();

			this.chartFormManager.OnBacktestedOrLivesimmed();
		}
		internal void ChartRangeBar_OnAnyValueChanged(object sender, RangeArgs<DateTime> e) {
			BarDataRange newRange = new BarDataRange(e.ValueMin, e.ValueMax);
			try {
				this.chartFormManager.UserSelectedRange_loadBars_backtest_populate(newRange);
			} catch (Exception ex) {
				Assembler.PopupException("ChartRangeBar_AnyValueChanged", ex);
			}
		}
	}
}
