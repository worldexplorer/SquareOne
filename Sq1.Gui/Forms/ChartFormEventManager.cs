using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.StrategyBase;
using Sq1.Core.Execution;
using Sq1.Widgets.RangeBar;
using Sq1.Gui.Singletons;
using WeifenLuo.WinFormsUI.Docking;

namespace Sq1.Gui.Forms {
	public class ChartFormEventManager {
		ChartFormManager chartFormManager;

		public ChartFormEventManager(ChartFormManager chartFormManager) {
			this.chartFormManager = chartFormManager;
			this.chartFormManager.ChartForm.FormClosing += ChartForm_FormClosing;
			this.chartFormManager.ChartForm.Load += ChartForm_Load;
		}
		void ChartForm_Load(object sender, EventArgs e) {
			// ON_DESERIALIZATION_BACKTESTER_LAUNCHES_FASTER_THAN_CHART_FORM_GETS_LOADED; see ON_REQUESTING_ABORT_TASK_DIES_WITHOUT_INVOKING_CONTINUE_WITH
			this.chartFormManager.Executor.EventGenerator.BacktesterContextInitializedStep2of4 += this.Executor_BacktesterContextInitializedStep2of4;
			this.chartFormManager.Executor.EventGenerator.BacktesterSimulatedChunkStep3of4 += this.Executor_BacktesterChunkSimulatedStep3of4;
			this.chartFormManager.Executor.EventGenerator.BacktesterSimulatedAllBarsStep4of4 += this.Executor_BacktesterSimulatedAllBarsStep4of4;
			//this.chartFormsManager.Executor.EventGenerator.BacktesterBarsChanged += this.Executor_BacktesterChangedQuotesWillGenerate;
		}
		void ChartForm_FormClosing(object sender, FormClosingEventArgs e) {
			this.chartFormManager.Executor.EventGenerator.BacktesterContextInitializedStep2of4 -= this.Executor_BacktesterContextInitializedStep2of4;
			this.chartFormManager.Executor.EventGenerator.BacktesterSimulatedChunkStep3of4 -= this.Executor_BacktesterChunkSimulatedStep3of4;
			this.chartFormManager.Executor.EventGenerator.BacktesterSimulatedAllBarsStep4of4 -= this.Executor_BacktesterSimulatedAllBarsStep4of4;
			//this.chartFormsManager.Executor.EventGenerator.BacktesterBarsChanged -= this.Executor_BacktesterChangedQuotesWillGenerate;
			
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
			} catch (Exception ex) {
				Assembler.PopupException("DataSourcesTree_OnSymbolSelected()", ex);
			}
		}
//		internal void BarScaleIntervalSelector_OnBarDataScaleChangedUserMouse(object sender, BarScaleIntervalEventArgs e) {
//			try {
//				ContextChart context = this.chartFormManager.ContextCurrentChartOrStrategy;
//				context.ScaleInterval = e.BarScaleInterval;
//				this.chartFormManager.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsBacktestIfStrategy("BarScaleIntervalSelector_OnBarDataScaleChangedUserMouse");
//			} catch (Exception ex) {
//				Assembler.PopupException("BarScaleIntervalSelector_OnBarDataScaleChangedUserMouse", ex);
//			}
//		}
//		internal void BarDataRangeSelector_OnBarDataRangeChangedUserMouse(object sender, BarDataRangeEventArgs e) {
//			try {
//				ContextChart context = this.chartFormManager.ContextCurrentChartOrStrategy;
//				context.DataRange = e.BarDataRange;
//				this.chartFormManager.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsBacktestIfStrategy("BarDataRangeSelector_OnBarDataRangeChangedUserMouse");
//			} catch (Exception ex) {
//				Assembler.PopupException("BarDataRangeSelector_OnBarDataRangeChangedUserMouse", ex);
//			}
//		}
//		internal void PositionSizeSelector_OnPositionChangedUserMouse(object sender, PositionSizeEventArgs e) {
//			try {
//				ContextChart context = this.chartFormManager.ContextCurrentChartOrStrategy;
//				context.PositionSize = e.PositionSize;
//				this.chartFormManager.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsBacktestIfStrategy("PositionSizeSelector_OnPositionChangedUserMouse", false);
//			} catch (Exception ex) {
//				Assembler.PopupException("PositionSizeSelector_OnPositionChangedUserMouse", ex);
//			}
//		}

		internal void MainForm_ActivatedDocumentPane_WithChart(object sender, EventArgs e) {
			//v1
			//ScriptEditorForm  scriptEditorFormCorresponding = chartFormClicked.ChartFormsManager.ScriptEditorFormConditionalInstance;
			//if (DockHelper.IsDockStateAutoHide(scriptEditorFormCorresponding.DockState)) {
			//	DockState newState = DockHelper.ToggleAutoHideState(scriptEditorFormCorresponding.Pane.DockState);
			//	scriptEditorFormCorresponding.Pane.SetDockState(newState);
			//}
			//scriptEditorFormCorresponding.Activate();
			//v2
			if (this.chartFormManager.Strategy != null && this.chartFormManager.Strategy.ActivatedFromDll == false) {
				DockHelper.ActivateDockContentPopupAutoHidden(this.chartFormManager.ScriptEditorFormConditionalInstance);
			}
			//during the update to the next DockContent version, copy&paste into DockHelper.cs:
			//public static void ActivateDockContentPopupAutoHidden(DockContent form) {
			//	if (DockHelper.IsDockStateAutoHide(form.DockState)) {
			//		DockState newState = DockHelper.ToggleAutoHideState(form.Pane.DockState);
			//		form.Pane.SetDockState(newState);
			//	}
			//	form.Activate();
			//}

			this.chartFormManager.ReportersFormsManager.ParentChart_OnActivated(sender, e);
			this.chartFormManager.ChartForm.ChartControl.RangeBar.Enabled = false;
		}
		internal void Executor_BacktesterContextInitializedStep2of4(object sender, EventArgs e) {
			if (this.chartFormManager.ChartForm == null) return;
			if (this.chartFormManager.Executor == null) return;
			if (this.chartFormManager.Executor.Backtester.QuotesGenerator == null) return;
			if (this.chartFormManager.ChartForm.InvokeRequired) {
				this.chartFormManager.ChartForm.BeginInvoke(new MethodInvoker(delegate { this.Executor_BacktesterContextInitializedStep2of4(sender, e); }));
				return;
			}

			this.chartFormManager.ChartForm.TsiProgressBarETA.ETALabelText = this.chartFormManager.Executor.Backtester.ProgressStats;

			int quotesTotal = this.chartFormManager.Executor.Backtester.QuotesTotalToGenerate;
			if (quotesTotal == -1) {
				string msg = "Executor_BacktesterSimulationStarted: Backtester.QuotesTotalToGenerate=-1 due to Backtester.BarsOriginal=null";
				Assembler.PopupException(msg);
				return;
			}

			//if (this.chartFormsManager.ChartForm.TsiProgressBarETA.ETAProgressBarMaximum != quotesTotal) {
			this.chartFormManager.ChartForm.TsiProgressBarETA.ETAProgressBarMaximum = quotesTotal;
			//}

			this.chartFormManager.ChartForm.TsiProgressBarETA.Visible = true;
			
			this.chartFormManager.ChartForm.btnAutoSubmit.Visible = false;
			this.chartFormManager.ChartForm.btnStreaming.Visible = false;
			this.chartFormManager.ChartForm.PropagateSelectorsDisabledIfStreamingForCurrentChart();
		}
		internal void Executor_BacktesterChunkSimulatedStep3of4(object sender, EventArgs e) {
			if (this.chartFormManager.Executor == null) {
				string msg = "invoked by Backtester.SubstituteAndRunSimulation() I don't remember whether Tag=null is ok or not...";
				return;
			}
			if (sender != this.chartFormManager.Executor.EventGenerator) return;
			if (this.chartFormManager.ChartForm.InvokeRequired) {
				this.chartFormManager.ChartForm.BeginInvoke(new MethodInvoker(delegate { this.Executor_BacktesterChunkSimulatedStep3of4(sender, e); }));
				return;
			}

			this.chartFormManager.ChartForm.TsiProgressBarETA.ETALabelText = this.chartFormManager.Executor.Backtester.ProgressStats;

			//int quotesTotal = this.chartFormsManager.Executor.BacktesterFacade.Backtester.QuotesTotalToGenerate;
			//if (quotesTotal == -1) {
			//	string msg = "Executor_BacktesterSimulationStarted: Backtester.QuotesTotalToGenerate=-1 due to Backtester.BarsOriginal=null";
			//	this.chartFormsManager.MainForm.PopupException(msg);
			//	return;
			//}

			int currentValue = this.chartFormManager.Executor.Backtester.QuotesGeneratedSoFar;
			if (currentValue > this.chartFormManager.ChartForm.TsiProgressBarETA.ETAProgressBarMaximum) return;
			this.chartFormManager.ChartForm.TsiProgressBarETA.ETAProgressBarValue = currentValue;

			// ETALabelText isn't refreshed fast enough; windows don't feel mouse clicks&moves, GUI freezes; REMOVE after backtester goes to its own thread!
			////DEBUGGER_SHOWS_RECURSIVE_CALLS_TO_BuildOnceAllReports
			/// uncommented to make GUI more responsive during backtests; GUI didn't fully unhalt, lagging.... FIXME  
			//Application.DoEvents();
		}
		internal void Executor_BacktesterSimulatedAllBarsStep4of4(object sender, EventArgs e) {
			if (this.chartFormManager.Executor == null) return;
			if (sender != this.chartFormManager.Executor.EventGenerator) return;
			if (this.chartFormManager.ChartForm.InvokeRequired) {
				this.chartFormManager.ChartForm.BeginInvoke(new MethodInvoker(delegate { this.Executor_BacktesterSimulatedAllBarsStep4of4(sender, e); }));
				return;
			}

			this.chartFormManager.ChartForm.TsiProgressBarETA.ETALabelText = this.chartFormManager.Executor.Backtester.ProgressStats;
			this.chartFormManager.ChartForm.TsiProgressBarETA.ETAProgressBarValue = 0;
			this.chartFormManager.ChartForm.TsiProgressBarETA.Visible = false;
			
			this.chartFormManager.ChartForm.btnAutoSubmit.Visible = true;
			this.chartFormManager.ChartForm.btnStreaming.Visible = true;
			this.chartFormManager.ChartForm.PropagateSelectorsDisabledIfStreamingForCurrentChart();
		}
		internal void ChartRangeBar_AnyValueChanged(object sender, RangeArgs<DateTime> e) {
			BarDataRange newRange = new BarDataRange(e.ValueMin.Date, e.ValueMax.Date);
			try {
				this.chartFormManager.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy("ChartRangeBar_AnyValueChanged");
			} catch (Exception ex) {
				Assembler.PopupException("ChartRangeBar_AnyValueChanged", ex);
			}
		}
		internal void ChartForm_StreamingButtonStateChanged(object sender, EventArgs e) {
//			bool streamingChecked = this.chartFormManager.ChartForm.btnStreaming.Checked;
//			SlidersForm.Instance.Enabled = !streamingChecked;
//			SelectorsForm.Instance.Enabled = !streamingChecked;
			try {
				this.chartFormManager.ChartForm.PropagateSelectorsDisabledIfStreamingForCurrentChart();
			} catch (Exception ex) {
				Assembler.PopupException("ChartForm_StreamingButtonStateChanged", ex);
			}
		}

	}
}
