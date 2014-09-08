using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.StrategyBase;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Support;
using Sq1.Core.Execution;
using Sq1.Gui.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Sq1.Gui.Singletons {
	public class MainFormEventManager {
		private MainForm mainForm;

		private DockPanel dockPanel {
			get { return this.mainForm.DockPanel; }
		}
		private IStatusReporter statusReporter {
			get { return this.mainForm as IStatusReporter; }
		}

		public MainFormEventManager(MainForm mainForm) {
			this.mainForm = mainForm;
		}

		#region StrategiesTree
		internal void StrategiesTree_OnStrategyOpenDefaultClicked(object sender, StrategyEventArgs e) {
			//v1
			//Strategy strategy = e.Strategy;
			////strategy.ContextMarkNone();
			//strategy.ScriptContextCurrent.DataRange = SelectorsForm.Instance.BarDataRangeSelector.Popup.BarDataRange;
			//strategy.ScriptContextCurrent.ScaleInterval = SelectorsForm.Instance.BarScaleIntervalSelector.Popup.BarScaleInterval;
			//strategy.ScriptContextCurrent.PositionSize = SelectorsForm.Instance.PositionSizeSelector.Popup.PositionSize;
			//this.chartCreateShowPopulateSelectorsSliders(strategy);
			//v2
			e.scriptContextName = "Default";
			this.StrategiesTree_OnStrategyOpenNewChartClicked(sender, e);
		}
		internal void StrategiesTree_OnStrategyOpenNewChartClicked(object sender, StrategyEventArgs e) {
			Strategy strategy = e.Strategy;
			strategy.ContextSwitchCurrentToNamedAndSerialize(e.scriptContextName);
			this.chartCreateShowPopulateSelectorsSlidersFromStrategy(strategy);
		}
		internal void StrategiesTree_OnStrategyRenamed(object sender, StrategyEventArgs e) {
			foreach (ChartFormManager chartFormsManager in this.mainForm.GuiDataSnapshot.ChartFormManagers.Values) {
				if (chartFormsManager.Strategy != e.Strategy) continue;
				if (chartFormsManager.ScriptEditorFormConditionalInstance != null) {
					chartFormsManager.ScriptEditorFormConditionalInstance.Text = e.Strategy.Name;
				}
				if (chartFormsManager.ChartForm != null) {
					chartFormsManager.ChartForm.Text = e.Strategy.Name;
				}
			}
		}
		ChartFormManager chartCreateShowPopulateSelectorsSlidersFromStrategy(Strategy strategy) {
			ChartFormManager chartFormManager = new ChartFormManager();
			chartFormManager.InitializeWithStrategy(this.mainForm, strategy, false);
			this.mainForm.GuiDataSnapshot.ChartFormManagers.Add(chartFormManager.DataSnapshot.ChartSerno, chartFormManager);
			chartFormManager.ChartFormShow();
			chartFormManager.StrategyCompileActivatePopulateSlidersShow();
			return chartFormManager;
		}
		void chartCreateShowPopulateSelectorsSlidersNoStrategy(ContextChart contextChart) {
			ChartFormManager chartFormManager = new ChartFormManager();
			chartFormManager.InitializeChartNoStrategy(this.mainForm, contextChart);
			this.mainForm.GuiDataSnapshot.ChartFormManagers.Add(chartFormManager.DataSnapshot.ChartSerno, chartFormManager);
			chartFormManager.ChartFormShow();
		}
		public void StrategiesTree_OnStrategySelected(object sender, StrategyEventArgs e) {
			ChartForm active = this.mainForm.ChartFormActive;
			if (active == null) {
				e.NoActiveChartFoundToAccomodateStrategy = true;
				//USELESS Assembler.PopupException("NO_ACTIVE_CHART_FORMS_IN_DOCUMENT_PANE_TO_LOAD_STRATEGY_INTO this.mainForm.ChartFormActive=null //StrategiesTree_OnStrategySelected(" + e.Strategy + ")");
				return;
			}
			active.ChartFormManager.InitializeWithStrategy(this.mainForm, e.Strategy, false);
		}
		#endregion
		
		#region ChartForm
		internal void ChartForm_FormClosed(object sender, FormClosedEventArgs e) {
			if (this.mainForm.MainFormClosingSkipChartFormsRemoval) return;
			try {
				ChartForm chartFormClosed = sender as ChartForm;
				ChartFormManager chartFormManager = chartFormClosed.ChartFormManager;
				// chartFormsManager lifecycle ends here
				this.mainForm.GuiDataSnapshot.ChartFormManagers.Remove(chartFormManager.DataSnapshot.ChartSerno);

				if (chartFormManager.EditorFormIsNotDisposed == false) chartFormManager.ScriptEditorFormConditionalInstance.Close();
				//foreach (Reporter reporter in chartFormsManager.Reporters.Values) {
				//	Control reporterAsControl = reporter as Control;
				//	Control reporterParent = reporterAsControl.Parent;
				//	DockContent reporterParentForm = reporterParent as DockContent;
				//	if (chartFormsManager.FormIsNullOrDisposed(reporterParentForm)) reporterParentForm.Close();
				//}
				if (StrategiesForm.Instance.IsActivated == false) {
					StrategiesForm.Instance.Activate();
				}
			} catch (Exception ex) {
				string msg = "ChartFormsManagers.Remove() didn't go trought? duplicates";
			}
		}
		#endregion

		internal void DockPanel_ActiveDocumentChanged(object sender, EventArgs e) {
			if (this.mainForm.MainFormClosingSkipChartFormsRemoval) {
				string msg = "onAppClose getting invoked for each [mosaically] visible document, right? nope just once per Close()";
				return;
			}

			//DONT_FORGET_TO_RENAME_CHART2_TO_CHART_WHEN_REPLACING_SQ1.WIDGETS.CHARTING_<_SQ1.CHARTING
			ChartForm chartFormClicked = this.mainForm.DockPanel.ActiveDocument as ChartForm;
			if (chartFormClicked == null) {
				this.mainForm.GuiDataSnapshot.ChartSernoHasFocus = -1;
				string msg = "focus might have moved away from a document to Docked Panel"
					+ "; I'm here after having focused on ExceptionsForm docked into Documents pane";
				return;
			}
			//if (chartFormClicked.IsActivated == false) return;	//NOUP ActiveDocumentChanged is invoked twice: 1) for a form loosing control, 2) for a form gaining control
			try {
				chartFormClicked.ChartFormManager.EventManager.MainForm_ActivatedDocumentPane_WithChart(sender, e);
				this.mainForm.GuiDataSnapshot.ChartSernoHasFocus = chartFormClicked.ChartFormManager.DataSnapshot.ChartSerno;
				this.mainForm.GuiDataSnapshotSerializer.Serialize();
				
				//v1: DOESNT_POPULATE_SYMBOL_AND_SCRIPT_PARAMETERS 
				//if (chartFormClicked.ChartFormManager.Strategy == null) {
				//	StrategiesForm.Instance.StrategiesTreeControl.UnSelectStrategy();
				//} else {
				//	StrategiesForm.Instance.StrategiesTreeControl.SelectStrategy(chartFormClicked.ChartFormManager.Strategy);
				//}
				chartFormClicked.ChartFormManager.PopulateMainFormSymbolStrategyTreesScriptParameters();
			} catch (Exception ex) {
				this.mainForm.PopupException(ex);
			}
		}

		#region DataSourcesTree
		internal void DataSourcesTree_OnBarsAnalyzerClicked(object sender, DataSourceSymbolEventArgs e) {
		}
		internal void DataSourcesTree_OnDataSourceDeletedClicked(object sender, DataSourceEventArgs e) {
		}
		internal void RepositoryJsonDataSource_OnDataSourceCanBeRemoved(object sender, NamedObjectJsonEventArgs<DataSource> e) {
			int a = 1;
			// ask them before deleting using another event and check if DataSourceEventArgs.DoNotDeleteThisDataSourceBecauseItsUsedElsewhere
		}
		internal void RepositoryJsonDataSource_OnDataSourceRemoved(object sender, NamedObjectJsonEventArgs<DataSource> e) {
			int a = 1;
			//if a running optimizer / backtester / streaming chart had DataSource, possibly shut them down?
		}
		internal void DataSourcesTree_OnDataSourceEditClicked(object sender, DataSourceEventArgs e) {
			DataSourceEditorForm.Instance.DataSourceEditorControl.Initialize(e.DataSource);
			try {
				DataSourceEditorForm.Instance.ShowAsDocumentTabNotPane(this.mainForm.DockPanel);
			} catch (Exception exc) {
				string msg = "DataSourceEditorForm(DataSource[" + e.DataSource + "]): internal Exception";
				this.statusReporter.PopupException(new Exception(msg, exc));
				return;
			}
		}
		internal void DataSourcesTree_OnDataSourceRenameClicked(object sender, DataSourceEventArgs e) {
		}
		internal void DataSourcesTree_OnDataSourceSelected(object sender, DataSourceEventArgs e) {
		}
		internal void DataSourcesTree_OnNewChartForSymbolClicked(object sender, DataSourceSymbolEventArgs e) {
			ContextChart contextChart = new ContextChart("CHART_" + e.Symbol);
			contextChart.DataSourceName = e.DataSource.Name;
			contextChart.Symbol = e.Symbol;
			contextChart.ScaleInterval = e.DataSource.ScaleInterval;
			//this.chartCreateShowPopulateSelectorsSliders(contextChart);
			this.chartCreateShowPopulateSelectorsSlidersNoStrategy(contextChart);
		}
		internal void DataSourcesTree_OnOpenStrategyForSymbolClicked(object sender, DataSourceSymbolEventArgs e) {
		}
		internal void DataSourcesTree_OnSymbolSelected(object sender, DataSourceSymbolEventArgs e) {
			try {
				if ((this.mainForm.DockPanel.ActiveDocument is ChartForm) == false) {
					return;
				}
				// mainForm.ChartFormActive will already throw if Documents have no Charts selected; no need to check
				this.mainForm.ChartFormActive.ChartFormManager.EventManager.DataSourcesTree_OnSymbolSelected(sender, e);
			} catch (Exception ex) {
				this.mainForm.PopupException(ex);
			}
		}
		#endregion DataSourcesTree


		#region SlidersForm.Instance.SlidersAutoGrow
		internal void SlidersAutoGrow_OnScriptContextLoadClicked(object sender, StrategyEventArgs e) {
			Strategy strategy = e.Strategy;
			strategy.ContextSwitchCurrentToNamedAndSerialize(e.scriptContextName);
			SlidersForm.Instance.PopulateFormTitle(strategy);
			try {
				this.mainForm.ChartFormActive.ChartFormManager.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy("StrategiesTree_OnScriptContextLoadClicked()");
			} catch (Exception ex) {
				Assembler.PopupException("StrategiesTree_OnScriptContextLoadClicked()", ex);
			}
		}
		internal void SlidersAutoGrow_OnScriptContextRenamed(object sender, StrategyEventArgs e) {
			Strategy strategy = e.Strategy;
			if (strategy.ScriptContextCurrentName != e.scriptContextName) return;	//refresh FormTitle only when renaming current context
			SlidersForm.Instance.PopulateFormTitle(strategy);
		}
		internal void SlidersAutoGrow_SliderValueChanged(object sender, ScriptParameterEventArgs e) {
			try {
// MOVED_TO: SlidersAutoGrow.slider_ValueCurrentChanged
//				// mainForm.ChartFormActive will already throw if Documents have no Charts selected; no need to check
//				Strategy strategyToSave = this.mainForm.ChartFormActive.ChartFormManager.Strategy;
//				if (strategyToSave == null) {
//					string msg = "You should rebuild SlidersForm on ChartFormActive switch"
//						+ "; parameterless ChartFormActive[" + this.mainForm.ChartFormActive.Name + "] doesn't have a strategy and should not display ScriptParameters that user might click"
//						+ "; if you see this exception then SlidersForm wasn't cleaned up and displays Sliders for another Chart";
//					throw new Exception(msg);
//				}
//				strategyToSave.SetCurrentValueToCurrentContextAndScriptParameters(e.ScriptParameter);
//				Assembler.InstanceInitialized.RepositoryDllJsonStrategy.SaveStrategy(strategyToSave);
// /MOVED_TO: SlidersAutoGrow.slider_ValueCurrentChanged
// MOVED_FROM: SlidersAutoGrow.slider_ValueCurrentChanged
				Strategy strategyToSaveAndRun = this.mainForm.ChartFormActive.ChartFormManager.Strategy;
				if (strategyToSaveAndRun.Script.Executor == null) {
					string msg = "slider_ValueCurrentChanged(): did you forget to assign Script.Executor after compilation?...";
					Assembler.PopupException(msg);
					return;
				}
				//v1 too little strategyToSaveAndRun.Script.Executor.BacktesterFacade.RunSimulation();
				//v2 too little strategyToSaveAndRun.Script.Executor.BacktesterRunSimulation();
				//v3 too little this.mainForm.ChartFormActive.ChartFormManager.BacktesterRunSimulationRegular();
				//v4 merged into PopulateSelectors this.mainForm.ChartFormActive.ChartFormManager.EventManager.LoadNewBarsAndPassToExecutor(false);
				this.mainForm.ChartFormActive.ChartFormManager.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy("SlidersAutoGrow_SliderValueChanged", false);
				//v5 REDUNDANT, ChartControl.Initialize() added to LoadNewBarsAndPassToExecutor() this.mainForm.ChartFormActive.ChartFormManager.PopulateCurrentChartOrScriptContext("SlidersAutoGrow_SliderValueChanged()");

// /MOVED_FROM: SlidersAutoGrow.slider_ValueCurrentChanged
			} catch (Exception ex) {
				Assembler.PopupException("SlidersAutoGrow_SliderValueChanged()", ex);
			}
		}
		#endregion SlidersForm.Instance.SlidersAutoGrow

	}
}