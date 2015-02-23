using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.StrategyBase;
using Sq1.Widgets.RangeBar;
using WeifenLuo.WinFormsUI.Docking;

namespace Sq1.Gui.Forms {
	public class ChartFormInterformEventsConsumer {
		ChartFormManager chartFormManager;

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
			//v1
			//Strategy strategyToSave = this.chartFormManager.Strategy;
			//if (strategyToSave.ScriptContextCurrent.Symbol == e.Symbol) {
			//if (contextChart.Symbol == e.Symbol) {
			//	string msg = "DebuggableCallback invokes ObjectListView.OnMouseUp():8266 twice, and ObjectListView.WndProc():4996 doesn't have a filter";
			//	Assembler.PopupException(msg);
			//	return;
			//}
			try {
				//v2
				ContextChart contextChart = this.chartFormManager.ContextCurrentChartOrStrategy;
				if (contextChart.DataSourceName != e.DataSource.Name)	contextChart.DataSourceName = e.DataSource.Name; 
				if (contextChart.Symbol			!= e.Symbol) 			contextChart.Symbol 		= e.Symbol;
				this.chartFormManager.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy("DataSourcesTree_OnSymbolSelected");
				this.chartFormManager.OptimizerFormIfOpenPropagateTextboxesOrMarkStaleResults();
			} catch (Exception ex) {
				Assembler.PopupException("DataSourcesTree_OnSymbolSelected()", ex);
			}
		}
		internal void MainForm_ActivateDocumentPane_WithChart(object sender, EventArgs e) {
			//v1
			//ScriptEditorForm  scriptEditorFormCorresponding = chartFormClicked.ChartFormsManager.ScriptEditorFormConditionalInstance;
			//if (DockHelper.IsDockStateAutoHide(scriptEditorFormCorresponding.DockState)) {
			//	DockState newState = DockHelper.ToggleAutoHideState(scriptEditorFormCorresponding.Pane.DockState);
			//	scriptEditorFormCorresponding.Pane.SetDockState(newState);
			//}
			//scriptEditorFormCorresponding.Activate();
			//v2
			if (this.chartFormManager.Strategy != null && this.chartFormManager.Strategy.ActivatedFromDll == false) {
				this.chartFormManager.ScriptEditorFormConditionalInstance.ActivateDockContentPopupAutoHidden(true, true);
			}
			//during the update to the next DockContent version, copy&paste into DockHelper.cs:
			//public static void ActivateDockContentPopupAutoHidden(DockContent form) {
			//	if (DockHelper.IsDockStateAutoHide(form.DockState)) {
			//		DockState newState = DockHelper.ToggleAutoHideState(form.Pane.DockState);
			//		form.Pane.SetDockState(newState);
			//	}
			//	form.Activate();
			//}

			this.chartFormManager.ReportersFormsManager.PopupReporters_OnParentChartActivated(sender, e);
			this.chartFormManager.ChartForm.ChartControl.RangeBar.Enabled = false;
			
			if (this.chartFormManager.OptimizerForm == null) {
				string msg = "don't even try to access OptimizationConditionalInstance if user didn't click implicitly; TODO where to can I incapsulate it?";
			} else {
				this.chartFormManager.OptimizerFormShow(true);
			}
			if (this.chartFormManager.LivesimForm == null) {
				string msg = "don't even try to access OptimizationConditionalInstance if user didn't click implicitly; TODO where to can I incapsulate it?";
			} else {
				this.chartFormManager.LivesimFormShow(true);
			}
		}
		internal void Executor_BacktesterContextInitialized_step2of4(object sender, EventArgs e) {
			string msig = " Executor_BacktesterContextInitialized_step2of4()" + this.chartFormManager.ToString();
			if (this.chartFormManager.ChartForm == null) return;
			if (this.chartFormManager.Executor == null) return;
			if (this.chartFormManager.Executor.Backtester.BarsOriginal == null) {
				string msg = "I_RESTORED_CONTEXT__END_OF_BACKTEST_ORIGINAL_BECAME_NULL";
				if (this.chartFormManager.ChartForm.InvokeRequired == false) {
					msg = "NO_NEED_TO_REPORT_ITS_NOT_AN_ERROR  I_REFUSE_TO_CALCULATE_PERCENTAGE_COMPLETED BACKTEST_ALREADY_FINISHED_WHILE_SWTICHING_TO_GUI_THREAD";
					//Assembler.PopupException(msg + msig, null, false);
				} else {
					Assembler.PopupException(msg + msig, null, false);
				}
				return;
			}

			if (this.chartFormManager.Executor.Backtester.QuotesGenerator == null) return;
			int quotesTotal = this.chartFormManager.Executor.Backtester.QuotesTotalToGenerate;
			if (quotesTotal == -1) {
				string msg = "I_RESTORED_CONTEXT__END_OF_BACKTEST_ORIGINAL_BECAME_NULL: Backtester.QuotesTotalToGenerate=-1 due to Backtester.BarsOriginal=null";
				Assembler.PopupException(msg);
				return;
			}

			if (this.chartFormManager.ChartForm.InvokeRequired) {
				this.chartFormManager.ChartForm.BeginInvoke(new MethodInvoker(delegate { this.Executor_BacktesterContextInitialized_step2of4(sender, e); }));
				return;
			}

			this.chartFormManager.ChartForm.TsiProgressBarETA.ETALabelText = this.chartFormManager.Executor.Backtester.ProgressStats;
			
			// CHART_NOT_NOTIFIED_OF_BACKTEST_PROGRESS_AFTER_DESERIALIZATION_BACKTESTER_LAUNCHES_BEFORE_IM_SUBSCRIBED BEGIN

			//if (this.chartFormsManager.ChartForm.TsiProgressBarETA.ETAProgressBarMaximum != quotesTotal) {
			this.chartFormManager.ChartForm.TsiProgressBarETA.ETAProgressBarMaximum = quotesTotal;
			//}

			this.chartFormManager.ChartForm.TsiProgressBarETA.Visible = true;
			
			this.chartFormManager.ChartForm.btnStrategyEmittingOrders.Visible = false;
			this.chartFormManager.ChartForm.btnStreamingTriggersScript.Visible = false;
			this.chartFormManager.ChartForm.PropagateSelectorsDisabledIfStreamingForCurrentChart();
			// CHART_NOT_NOTIFIED_OF_BACKTEST_PROGRESS_AFTER_DESERIALIZATION_BACKTESTER_LAUNCHES_BEFORE_IM_SUBSCRIBED END
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
			//    string msg = "Livesimulator.afterBacktesterComplete()_ALREADY_RESTORED_BACKTESTER_WHILE_SWITCHING_TO_GUI_THREAD [base.Executor.Backtester = this.BacktesterBackup]";
			//    return;
			//}
			if (this.chartFormManager.Executor.Backtester.QuotesGenerator == null) {
				string msg = "YOU_DIDNT_INVOKE_Backtester.Initialize() AVOIDING_EXCEPTIONS_IN_QuotesGeneratedSoFar";
				Assembler.PopupException(msg, null, false);
				return;
			}

			int quotesTotal = this.chartFormManager.Executor.Backtester.QuotesTotalToGenerate;
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
				//    string msg = "Backtester.QuotesTotalToGenerate=-1 due to Backtester.BarsOriginal=null";
				//    Assembler.PopupException(msg + msig);
				//    return;
				//}
				this.chartFormManager.ChartForm.TsiProgressBarETA.ETAProgressBarMaximum = quotesTotal;
				this.chartFormManager.ChartForm.TsiProgressBarETA.Visible = true;
				this.chartFormManager.ChartForm.btnStrategyEmittingOrders.Visible = false;
				this.chartFormManager.ChartForm.btnStreamingTriggersScript.Visible = false;
				this.chartFormManager.ChartForm.PropagateSelectorsDisabledIfStreamingForCurrentChart();
			}
			// HACK FOR CHART_NOT_NOTIFIED_OF_BACKTEST_PROGRESS_AFTER_DESERIALIZATION_BACKTESTER_LAUNCHES_BEFORE_IM_SUBSCRIBED END COPYPASTE

			this.chartFormManager.ChartForm.TsiProgressBarETA.ETALabelText = this.chartFormManager.Executor.Backtester.ProgressStats;
			

			int currentValue = this.chartFormManager.Executor.Backtester.QuotesGeneratedSoFar;
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
			if (this.chartFormManager.ChartForm.InvokeRequired) {
				this.chartFormManager.ChartForm.BeginInvoke(new MethodInvoker(delegate { this.Executor_BacktesterSimulatedAllBars_step4of4(sender, e); }));
				return;
			}

			this.chartFormManager.ChartForm.TsiProgressBarETA.ETALabelText = this.chartFormManager.Executor.Backtester.ProgressStats;
			this.chartFormManager.ChartForm.TsiProgressBarETA.ETAProgressBarValue = 0;
			this.chartFormManager.ChartForm.TsiProgressBarETA.Visible = false;
			
			this.chartFormManager.ChartForm.btnStrategyEmittingOrders.Visible = true;
			this.chartFormManager.ChartForm.btnStreamingTriggersScript.Visible = true;
			this.chartFormManager.ChartForm.PropagateSelectorsDisabledIfStreamingForCurrentChart();
		}
		internal void ChartRangeBar_AnyValueChanged(object sender, RangeArgs<DateTime> e) {
			BarDataRange newRange = new BarDataRange(e.ValueMin.Date, e.ValueMax.Date);
			try {
				this.chartFormManager.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy("ChartRangeBar_AnyValueChanged");
				this.chartFormManager.OptimizerFormIfOpenPropagateTextboxesOrMarkStaleResults();
			} catch (Exception ex) {
				Assembler.PopupException("ChartRangeBar_AnyValueChanged", ex);
			}
		}
	}
}
