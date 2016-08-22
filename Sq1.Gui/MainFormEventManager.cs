using System;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

using Sq1.Widgets;
using Sq1.Widgets.SteppingSlider;

using Sq1.Gui.Forms;
using Sq1.Gui.Singletons;

namespace Sq1.Gui {
	public class MainFormEventManager {
		MainForm	mainForm;

		public MainFormEventManager(MainForm mainForm) {
			this.mainForm = mainForm;
		}

		#region StrategiesTree
		public void StrategiesTree_OnStrategyDoubleClicked_NewChart(object sender, StrategyEventArgs e) {
			Strategy strategy = e.Strategy;
			if (strategy.Script == null) {
				string msg = "YES_OR_AFTER_RESTART YOU_OPENED_A_STRATEGY_YOU_JUST_ADDED_TO_REPOSITORY ?";
				//Assembler.PopupException(msg);
				//return;
			} else {
				if (strategy.Script.Executor == null) {
					string msg = "OPENED_FROM_STRATEGIES_TREE strategy.Script.Executor=null";
					Assembler.PopupException(msg, null, false);
				} else {
					strategy.ContextSwitch_currentToNamed_serialize(e.scriptContextName);
				}
			}
			this.chartCreateShow_populateSelectorsSliders_fromStrategy(strategy);
		}
		internal void StrategiesTree_OnStrategyOpenDefaultClicked_NewChart(object sender, StrategyEventArgs e) {
			//v1
			//Strategy strategy = e.Strategy;
			////strategy.ContextMarkNone();
			//strategy.ScriptContextCurrent.DataRange = SelectorsForm.Instance.BarDataRangeSelector.Popup.BarDataRange;
			//strategy.ScriptContextCurrent.ScaleInterval = SelectorsForm.Instance.BarScaleIntervalSelector.Popup.BarScaleInterval;
			//strategy.ScriptContextCurrent.PositionSize = SelectorsForm.Instance.PositionSizeSelector.Popup.PositionSize;
			//this.chartCreateShowPopulateSelectorsSliders(strategy);
			//v2
			e.scriptContextName = ContextScript.DEFAULT_NAME;
			this.StrategiesTree_OnStrategyDoubleClicked_NewChart(sender, e);
		}
		internal void StrategiesTree_OnStrategyLoadClicked(object sender, StrategyEventArgs e) {
			Strategy strategy = e.Strategy;
			ChartForm active = this.mainForm.ChartForm_lastActivatedContent_nullUnsafe;
			if (active == null) {
				ChartFormManager msg = this.chartCreateShow_populateSelectorsSliders_fromStrategy(strategy);
				active = msg.ChartForm;
			}
			active.ChartFormManager.InitializeWithStrategy(strategy, false, true);
			if (strategy.Script != null && strategy.Script.Executor != null) {
				strategy.ContextSwitch_currentToNamed_serialize(e.scriptContextName);
			} else {
				string msg = "SCRIPT_DIDNT_COMPILE__CANT_SWITCH_CONTEXT_SCRIPT strategy[" + strategy.ToString() + "]";
				Assembler.PopupException(msg);
			}
		}
		internal void StrategiesTree_OnStrategyRenamed(object sender, StrategyEventArgs e) {
			foreach (ChartFormManager chartFormManager in this.mainForm.GuiDataSnapshot.ChartFormManagers.Values) {
				if (chartFormManager.Strategy != e.Strategy) continue;
				if (chartFormManager.ScriptEditorFormSingletonized_nullUnsafe != null) {
					chartFormManager.ScriptEditorFormSingletonized_nullUnsafe.Text = e.Strategy.Name;
				}
				if (chartFormManager.ChartForm != null) {
					//v1 chartFormsManager.ChartForm.Text = e.Strategy.Name;
					chartFormManager.PopulateWindowTitles_fromChartContext_orStrategy();
				}
			}
		}
		ChartFormManager chartCreateShow_populateSelectorsSliders_fromStrategy(Strategy strategy) {
			ChartFormManager chartFormManager = new ChartFormManager(this.mainForm);
			chartFormManager.InitializeWithStrategy(strategy, false, true);
			this.mainForm.GuiDataSnapshot.ChartFormManagers.Add(chartFormManager.DataSnapshot.ChartSerno, chartFormManager);

			this.mainForm.GuiDataSnapshot.ChartControls_AllCurrentlyOpen.Add(chartFormManager.ChartForm.ChartControl);
			ChartSettingsEditorForm.Instance.RebuildChartsDropDown_dueToChartFormAddedOrRemoved();

			chartFormManager.ChartFormShow();
			chartFormManager.StrategyCompileActivate_populateSlidersShow();
			return chartFormManager;
		}
		void chartCreateShow_populateSelectorsSliders_noStrategy(ContextChart contextChart) {
			ChartFormManager chartFormManager = new ChartFormManager(this.mainForm);
			chartFormManager.InitializeWithoutStrategy(contextChart, true);
			this.mainForm.GuiDataSnapshot.ChartFormManagers.Add(chartFormManager.DataSnapshot.ChartSerno, chartFormManager);

			this.mainForm.GuiDataSnapshot.ChartControls_AllCurrentlyOpen.Add(chartFormManager.ChartForm.ChartControl);
			ChartSettingsEditorForm.Instance.RebuildChartsDropDown_dueToChartFormAddedOrRemoved();

			chartFormManager.ChartFormShow();
		}
		#endregion
		
		#region ChartForm
		internal void ChartForm_FormClosed(object sender, FormClosedEventArgs e) {
			if (this.mainForm.MainFormClosing_skipChartFormsRemoval_serializeExceptionsToPopupInNotepad) return;
			try {
				// chartFormsManager lifecycle ends here
				ChartForm chartFormClosed = sender as ChartForm;
				ChartFormManager chartFormManager = chartFormClosed.ChartFormManager;

				if (DockContentImproved.IsNullOrDisposed(chartFormManager.ScriptEditorForm) == false) chartFormManager.ScriptEditorFormSingletonized_nullUnsafe.Close();

				string msg = null;
				var mgrs = this.mainForm.GuiDataSnapshot.ChartFormManagers;
				if (mgrs.Count <= 0) {
					msg += "ChartFormsManagers.Count=0 ";
				} else {
					if (this.mainForm.GuiDataSnapshot.ChartFormManagers.ContainsValue(chartFormManager) == false) {
						msg += "chartFormManager_NOT_FOUND[" + chartFormManager.ToString() + "] ";
					} else {
						if (mgrs.ContainsKey(chartFormManager.DataSnapshot.ChartSerno) == false) {
							msg += "ChartSerno_NOT_FOUND[" + chartFormManager.DataSnapshot.ChartSerno + "] ";
						}
					}
				}

				if (string.IsNullOrEmpty(msg) == false) {
					Assembler.PopupException("CHART_FORMS_MANAGER_MUST_HAVE_BEEN_ADDED " + msg);
				} else {
					this.mainForm.GuiDataSnapshot.ChartFormManagers.Remove(chartFormManager.DataSnapshot.ChartSerno);
					this.mainForm.GuiDataSnapshot.ChartControls_AllCurrentlyOpen.Remove(chartFormClosed.ChartControl);
					ChartSettingsEditorForm.Instance.RebuildChartsDropDown_dueToChartFormAddedOrRemoved();
				}

				chartFormManager.Dispose_workspaceReloading();

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
				string msg = "ChartFormsManagers.Remove() didn't go throught? duplicates";
				Assembler.PopupException(msg, ex);
			}
		}

		public void ChartForm_OnBarsEditorClicked(object sender, DataSourceSymbolEventArgs e) {
			string msig = " //ChartForm_OnBarsEditorClicked()";
			if (BarsEditorForm.Instance.IsShown) {
				BarsEditorForm.Instance.ActivateDockContent_popupAutoHidden(false, true);
			} else {
				BarsEditorForm.Instance.ShowStackedHinted(this.mainForm.DockPanel);
			}
			BarsEditorForm.Instance.BarsEditorUserControl.LoadBars(e.DataSource.Name, e.Symbol);
		}
		#endregion

		//bool ignoreActiveDocumentChanged_imActivatingChart_afterLastReporterPoppedUp_preventingStackOverflow = false;
		//internal void DockPanel_ActiveDocumentChanged(object sender, EventArgs e) {
		//	string msig = "DockPanel_ActiveDocumentChanged()";
		//	if (this.ignoreActiveDocumentChanged_imActivatingChart_afterLastReporterPoppedUp_preventingStackOverflow) {
		//		this.ignoreActiveDocumentChanged_imActivatingChart_afterLastReporterPoppedUp_preventingStackOverflow = false;
		//		return;
		//	}

		//	if (this.mainForm.MainFormClosing_skipChartFormsRemoval_serializeExceptionsToPopupInNotepad) {
		//		string msg = "onAppClose getting invoked for each [mosaically] visible document, right? nope just once per Close()";
		//		return;
		//	}
		//	if (this.mainForm.dontSaveXml_ignoreActiveContentEvents_whileLoadingAnotherWorkspace) {
		//		string msg = "onAppClose getting invoked for each [mosaically] visible document, right? nope just once per Close()";
		//		return;
		//	}
		//	if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) {
		//		string msg = "dont save ChartSernoLastKnownHadFocus";
		//		return;
		//	}

		//	ChartForm chartFormClicked = this.mainForm.DockPanel.ActiveDocument as ChartForm;
		//	if (chartFormClicked == null) {
		//		this.mainForm.GuiDataSnapshot.ChartSernoLastKnownHadFocus = -1;
		//		string msg = "focus might have moved away from a document to Docked Panel"
		//			+ "; I'm here after having focused on ExceptionsForm docked into Documents pane";
		//		// ON_WORKSPACE_LOAD__ActiveDocumentChanged_IS_NOT_INVOKED_NO_NEED_TO_MOVE_5_LINES_UP
		//		DataSourceEditorForm dataSourceEditorFormClicked = this.mainForm.DockPanel.ActiveDocument as DataSourceEditorForm;
		//		if (dataSourceEditorFormClicked != null) {
		//			DataSourcesForm.Instance.ActivateDockContent_popupAutoHidden(false, true);
		//			string dsNameToSelect = dataSourceEditorFormClicked.DataSourceEditorControl.DataSourceName;
		//			DataSourcesForm.Instance.DataSourcesTreeControl.SelectDatasource(dsNameToSelect);
		//		}
		//		return;
		//	}
		//	//if (chartFormClicked.IsActivated == false) return;	//NOUP ActiveDocumentChanged is invoked twice: 1) for a form loosing control, 2) for a form gaining control
		//	try {
		//		chartFormClicked.ChartFormManager.InterformEventsConsumer.MainForm_ActivateDocumentPane_WithChart(sender, e);
		//		if (this.mainForm.GuiDataSnapshot.ChartSernoLastKnownHadFocus != chartFormClicked.ChartFormManager.DataSnapshot.ChartSerno) {
		//			this.mainForm.GuiDataSnapshot.ChartSernoLastKnownHadFocus  = chartFormClicked.ChartFormManager.DataSnapshot.ChartSerno;
		//			//v1 should be enough
		//			this.mainForm.GuiDataSnapshotSerializer.Serialize();
		//			//v2 EXCESSIVE_FOR_SAVING.ChartSernoLastKnownHadFocus this.mainForm.MainFormSerialize();		// serialises Snap and XML
		//		}
				
		//		//v1: DOESNT_POPULATE_SYMBOL_AND_SCRIPT_PARAMETERS 
		//		//if (chartFormClicked.ChartFormManager.Strategy == null) {
		//		//	StrategiesForm.Instance.StrategiesTreeControl.UnSelectStrategy();
		//		//} else {
		//		//	StrategiesForm.Instance.StrategiesTreeControl.SelectStrategy(chartFormClicked.ChartFormManager.Strategy);
		//		//}
		//		chartFormClicked.ChartFormManager.PopulateThroughMainForm_symbolStrategyTree_andSliders();
		//		if (this.mainForm.DockPanel.ActiveDocument != chartFormClicked) {
		//			this.ignoreActiveDocumentChanged_imActivatingChart_afterLastReporterPoppedUp_preventingStackOverflow = true;
		//			chartFormClicked.Activate();	// IF_REPORTERS_ARE_ALSO_IN_DOCUMENT_PANE__THEY_GOT_ACTIVE__WITH_BOLDED_TAB INFINITE_LOOP__IT_IS_ALREADY_ACTIVE
		//			this.ignoreActiveDocumentChanged_imActivatingChart_afterLastReporterPoppedUp_preventingStackOverflow = false;
		//			chartFormClicked.Focus();		// FLOATING_FORM_CANT_BE_RESIZED_WITHOUT_FOCUS FOCUS_WAS_PROBABLY_STOLEN_BY_SOME_OTHER_FORM(MAIN?)_LAZY_TO_DEBUG
		//		}
		//		ChartSettingsEditorForm.Instance.PopulateWithChartSettings(chartFormClicked.ChartControl);
		//		if (chartFormClicked.ChartFormManager.Executor.Bars != null) {
		//			SymbolInfoEditorForm.Instance.SymbolEditorControl.PopulateWithSymbolInfo(chartFormClicked.ChartFormManager.Executor.Bars.SymbolInfo);
		//		}
		//	} catch (Exception ex) {
		//		if (ex.Message == "The previous pane is invalid. It can not be null, and its docking state must not be auto-hide.") {
		//			foreach (DockPane eachPane in this.mainForm.DockPanel.Panes) {
		//				bool autoHide = eachPane.DockState == DockState.DockBottomAutoHide
		//							 || eachPane.DockState == DockState.DockLeftAutoHide
		//							 || eachPane.DockState == DockState.DockRightAutoHide
		//							 || eachPane.DockState == DockState.DockTopAutoHide;
		//				if (autoHide == false) continue;
		//				string whosInside = "";
		//				foreach (IDockContent outlaw in eachPane.DisplayingContents) {
		//					if (whosInside != "") whosInside += ",";
		//					whosInside += outlaw.ToString();
		//				}
		//				string msg = "FIXED_AS:CANT_ADD_PANE_RELATIVELY_TO_AUTOHIDE_PANE ADD_THOSE_AS_NON_AUTO_HIDDENS [" + whosInside + "]";
		//				Assembler.PopupException(msg + msig, null, false);
		//			}
		//		}
		//		Assembler.PopupException(msig, ex);
		//	}
		//}

		bool ignoreActiveContentChanged_imActivatingChart_afterLastReporterPoppedUp_preventingStackOverflow = false;
		internal void DockPanel_ActiveContentChanged(object sender, EventArgs e) {
			string msig = "DockPanel_ActiveContentChanged()";

			if (this.ignoreActiveContentChanged_imActivatingChart_afterLastReporterPoppedUp_preventingStackOverflow) {
				this.ignoreActiveContentChanged_imActivatingChart_afterLastReporterPoppedUp_preventingStackOverflow = false;
				return;
			}

			if (this.mainForm.MainFormClosing_skipChartFormsRemoval_serializeExceptionsToPopupInNotepad) {
				string msg = "onAppClose getting invoked for each [mosaically] visible content, right? nope just once per Close()";
				return;
			}
			if (this.mainForm.dontSaveXml_ignoreActiveContentEvents_whileLoadingAnotherWorkspace) {
				string msg = "onAppClose getting invoked for each [mosaically] visible content, right? nope just once per Close()";
				return;
			}
			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) {
				string msg = "TOO_EARLY_TO_HANDLE_THIS_EVENT I_NEED_TO_SET_GuiDataSnapshot.ChartSernoLastKnownHadFocus";
				return;
			}

			ChartForm chartFormClicked = this.mainForm.DockPanel.ActiveContent as ChartForm;
			//if (chartFormClicked == null) {
			//    string msg = "focus might have moved away from a document to Docked Panel"
			//        + "; I'm here after having focused on ExceptionsForm docked into Documents pane";
			//    // ON_WORKSPACE_LOAD__ActiveDocumentChanged_IS_NOT_INVOKED_NO_NEED_TO_MOVE_5_LINES_UP
			//    DataSourceEditorForm dataSourceEditorFormClicked = this.mainForm.DockPanel.ActiveDocument as DataSourceEditorForm;
			//    if (dataSourceEditorFormClicked != null) {
			//        DataSourcesForm.Instance.ActivateDockContent_popupAutoHidden(false, true);
			//        string dsNameToSelect = dataSourceEditorFormClicked.DataSourceEditorControl.DataSourceName;
			//        DataSourcesForm.Instance.DataSourcesTreeControl.SelectDatasource(dsNameToSelect);
			//    }
			//    return;
			//}
			if (chartFormClicked == null) {
				//this.mainForm.GuiDataSnapshot.ChartSernoLastKnownHadFocus = -1;
				string msg = "DockContent-derived activated by user isn't a ChartForm; I won't sync DataSourcesTree,StrategiesTree,Splitters to hightlight relevant";
				return;
			}
			if (chartFormClicked == this.mainForm.ChartForm_lastActivatedContent_nullUnsafe) {
				string msg = "THIS_IS_A_GARBAGE_EVENT_INVOCATIOIN__DOCK_CONTENT_GENERATES_TOO_MANY";
				return;
			}

			try {
				//string whoIsActive = sender.ToString();
				//DockPanel panelActivated = sender as DockPanel;
				//if (panelActivated != null) {
				//    if (panelActivated.ActiveDocument != null) {
				//        whoIsActive = panelActivated.ActiveDocument.ToString();
				//    } else {
				//        whoIsActive = "WHERE_ARE_YOU_THEN?? panelActivated.ActiveContent[" + panelActivated.ActiveContent + "]";
				//    }
				//}
				//Assembler.PopupException("lastChartActive[" + whoIsActive + "] " + msig, null, false);

				this.mainForm.ChartForm_lastActivatedContent_nullUnsafe = chartFormClicked;
				chartFormClicked.ChartFormManager.InterformEventsConsumer.MainForm_ActivateDocumentPane_WithChart(sender, e);
				if (this.mainForm.GuiDataSnapshot.ChartSernoLastKnownHadFocus != chartFormClicked.ChartFormManager.DataSnapshot.ChartSerno) {
					this.mainForm.GuiDataSnapshot.ChartSernoLastKnownHadFocus  = chartFormClicked.ChartFormManager.DataSnapshot.ChartSerno;
					//v1 should be enough
					this.mainForm.GuiDataSnapshotSerializer.Serialize();
					//v2 EXCESSIVE_FOR_SAVING.ChartSernoLastKnownHadFocus this.mainForm.MainFormSerialize();		// serialises Snap and XML
				}
				
				//v1: DOESNT_POPULATE_SYMBOL_AND_SCRIPT_PARAMETERS 
				//if (chartFormClicked.ChartFormManager.Strategy == null) {
				//	StrategiesForm.Instance.StrategiesTreeControl.UnSelectStrategy();
				//} else {
				//	StrategiesForm.Instance.StrategiesTreeControl.SelectStrategy(chartFormClicked.ChartFormManager.Strategy);
				//}
				chartFormClicked.ChartFormManager.PopulateThroughMainForm_symbolStrategyTree_andSliders();
				if (this.mainForm.DockPanel.ActiveDocument != chartFormClicked) {
					this.ignoreActiveContentChanged_imActivatingChart_afterLastReporterPoppedUp_preventingStackOverflow = true;
					chartFormClicked.Activate();	// STACK_OVERFLOW IF_REPORTERS_ARE_ALSO_IN_DOCUMENT_PANE__THEY_GOT_ACTIVE__WITH_BOLDED_TAB INFINITE_LOOP__IT_IS_ALREADY_ACTIVE
					chartFormClicked.Focus();		// STACK_OVERFLOW FLOATING_FORM_CANT_BE_RESIZED_WITHOUT_FOCUS FOCUS_WAS_PROBABLY_STOLEN_BY_SOME_OTHER_FORM(MAIN?)_LAZY_TO_DEBUG
					this.ignoreActiveContentChanged_imActivatingChart_afterLastReporterPoppedUp_preventingStackOverflow = false;
				}
				ChartSettingsEditorForm.Instance.PopulateWithChartSettings(chartFormClicked.ChartControl);
				if (chartFormClicked.ChartFormManager.Executor.Bars != null) {
					SymbolInfoEditorForm.Instance.SymbolEditorControl.PopulateWithSymbolInfo(chartFormClicked.ChartFormManager.Executor.Bars.SymbolInfo);
				}
			} catch (Exception ex) {
				if (ex.Message == "The previous pane is invalid. It can not be null, and its docking state must not be auto-hide.") {
					foreach (DockPane eachPane in this.mainForm.DockPanel.Panes) {
						bool autoHide = eachPane.DockState == DockState.DockBottomAutoHide
									 || eachPane.DockState == DockState.DockLeftAutoHide
									 || eachPane.DockState == DockState.DockRightAutoHide
									 || eachPane.DockState == DockState.DockTopAutoHide;
						if (autoHide == false) continue;
						string whosInside = "";
						foreach (IDockContent outlaw in eachPane.DisplayingContents) {
							if (whosInside != "") whosInside += ",";
							whosInside += outlaw.ToString();
						}
						string msg = "FIXED_AS:CANT_ADD_PANE_RELATIVELY_TO_AUTOHIDE_PANE ADD_THOSE_AS_NON_AUTO_HIDDENS [" + whosInside + "]";
						Assembler.PopupException(msg + msig, null, false);
					}
				}
				Assembler.PopupException(msig, ex);
			}
		}

		#region DataSourcesTree
		internal void DataSourcesTree_OnBarsEditorClicked(object sender, DataSourceSymbolEventArgs e) {
			string msig = " //DataSourcesTree_OnBarsEditorClicked()";
			if (BarsEditorForm.Instance.IsShown) {
				BarsEditorForm.Instance.ActivateDockContent_popupAutoHidden(false, true);
			} else {
				BarsEditorForm.Instance.ShowStackedHinted(this.mainForm.DockPanel);
			}
			BarsEditorForm.Instance.BarsEditorUserControl.LoadBars(e.DataSource.Name, e.Symbol);
		}
		internal void DataSourcesTree_OnSymbolInfoEditorClicked(object sender, DataSourceSymbolEventArgs e) {
			string msig = " //DataSourcesTree_OnSymbolInfoEditorClicked()";
			if (SymbolInfoEditorForm.Instance.IsShown) {
				SymbolInfoEditorForm.Instance.ActivateDockContent_popupAutoHidden(false, true);
			} else {
				SymbolInfoEditorForm.Instance.Show();
			}
			SymbolInfoEditorForm.Instance.SymbolEditorControl.PopulateWithSymbol_findOrCreateSymbolInfo(e.Symbol);
		}
		internal void DataSourcesTree_OnDataSourceDeletedClicked(object sender, DataSourceEventArgs e) {
		}
		internal void RepositoryJsonDataSources_OnDataSourceCanBeRemoved(object sender, NamedObjectJsonEventArgs<DataSource> e) {
			int a = 1;
			// ask them before deleting using another event and check if DataSourceEventArgs.DoNotDeleteThisDataSourceBecauseItsUsedElsewhere
		}
		internal void RepositoryJsonDataSources_OnDataSourceRemoved(object sender, NamedObjectJsonEventArgs<DataSource> e) {
			int a = 1;
			//if a running sequencer / backtester / streaming chart had DataSource, possibly shut them down?
		}
		internal void DataSourcesTree_OnDataSourceEditClicked(object sender, DataSourceEventArgs e) {
			//DataSourceEditorForm.Instance.DataSourceEditorControl.Initialize(e.DataSource);
			DataSourceEditorForm.Instance.Initialize(e.DataSource.Name, this.mainForm.DockPanel);
			try {
				if (DataSourceEditorForm.Instance.IsCoveredOrAutoHidden) {
					DataSourceEditorForm.Instance.ShowPopupSwitchToGuiThreadRunDelegateInIt();
				}
				//v1 IT_WILL_FOLLOW_DockContent.ShowHint__REMOVED_DockPanel_ActiveDocumentChanged() DataSourceEditorForm.Instance.ShowAsDocumentTabNotPane(this.mainForm.DockPanel);
				DataSourceEditorForm.Instance.Show(this.mainForm.DockPanel);		// as DockRight (Designer)
				this.mainForm.MainFormSerialize();	// after appRestart, DataSourceEditor was loosing last edited datasource
			} catch (Exception exc) {
				string msg = "DataSourceEditorForm(DataSource[" + e.DataSource + "]): internal Exception";
				Assembler.PopupException(msg, exc);
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
			this.chartCreateShow_populateSelectorsSliders_noStrategy(contextChart);
		}
		internal void DataSourcesTree_OnOpenStrategyForSymbolClicked(object sender, DataSourceSymbolEventArgs e) {
		}
		internal void DataSourcesTree_OnSymbolSelected(object sender, DataSourceSymbolEventArgs e) {
			try {
				if ((this.mainForm.DockPanel.ActiveDocument is ChartForm) == false) {
					return;
				}
				// mainForm.ChartFormActive will already throw if Documents have no Charts selected; no need to check
				this.mainForm.ChartForm_lastActivatedContent_nullUnsafe.ChartFormManager.InterformEventsConsumer.DataSourcesTree_OnSymbolSelected(sender, e);
			} catch (Exception ex) {
				Assembler.PopupException(null, ex);
			}
		}
		#endregion DataSourcesTree


		#region SlidersForm.Instance.SlidersAutoGrow
		internal void SlidersAutoGrow_OnScriptContextLoadClicked(object sender, StrategyEventArgs e) {
			Strategy strategy = e.Strategy;
			strategy.ContextSwitch_currentToNamed_serialize(e.scriptContextName);
			//v1 SlidersForm.Instance.PopulateFormTitle(strategy);
			//v2 WILLBEDONE_BY_PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy() SlidersForm.Instance.Initialize(strategy);
			try {
				this.mainForm.ChartForm_lastActivatedContent_nullUnsafe.ChartFormManager
					.PopulateSelectors_fromCurrentChartOrScriptContext_loadBars_saveStrategyOrCtx_backtestIfStrategy("StrategiesTree_OnScriptContextLoadClicked()");
			} catch (Exception ex) {
				Assembler.PopupException("StrategiesTree_OnScriptContextLoadClicked()", ex);
			}
		}
		internal void SlidersAutoGrow_OnScriptContextRenamed(object sender, StrategyEventArgs e) {
			Strategy strategy = e.Strategy;
			if (strategy.ScriptContextCurrentName != e.scriptContextName) return;	//refresh FormTitle only when renaming current context
			SlidersForm.Instance.PopulateFormTitle(strategy);
		}
		// TYPE_MANGLING_INSIDE_WARNING NOTICE_THAT_BOTH_PARAMETER_SCRIPT_AND_INDICATOR_VALUE_CHANGED_EVENTS_ARE_HANDLED_BY_SINGLE_HANDLER
		internal void SlidersAutoGrow_SliderValueChanged(object sender, IndicatorParameterEventArgs indicatorParamChangedArg) {
			ChartForm chartFormActive = this.mainForm.ChartForm_lastActivatedContent_nullUnsafe;
			if (chartFormActive == null) {
				string msg = "DRAG_CHART_INTO_DOCUMENT_AREA";
				Assembler.PopupException(msg);
				return;
			}
			Strategy strategyToSaveAndRun = chartFormActive.ChartFormManager.Strategy;
			if (strategyToSaveAndRun.Script.Executor == null) {
				string msg = "slider_ValueCurrentChanged(): did you forget to assign Script.Executor after compilation?...";
				Assembler.PopupException(msg);
				return;
			}

			// CHANGING_SLIDERS_ALREADY_AFFECTS_SCRIPT_AND_INDICATOR_PARAMS_KOZ_THERE_ARE_NO_CLONES_ANYMORE
			// SAME_OBJECTS_BETWEEN_SLIDER_AND_CURRENT_SCRIPT_CONTEXT_BUT_SCRIPT_HAS_ITS_OWN_ACCESSIBLE_THROUGH_REFLECTED
			bool dontRecalculate = indicatorParamChangedArg.IndicatorParameter is ScriptParameter;
			if (dontRecalculate == false && strategyToSaveAndRun.ScriptContextCurrent.BacktestOnSelectorsChange == false) {
				strategyToSaveAndRun.RecalculateIndicator(indicatorParamChangedArg.IndicatorParameter);
			}

			chartFormActive.ChartFormManager.
				PopulateSelectors_fromCurrentChartOrScriptContext_loadBars_saveStrategyOrCtx_backtestIfStrategy(
					"SlidersAutoGrow_SliderValueChanged", false);

			//ScriptContext.Sliders => Sequencer's.Parameters.Current
			//WHY??? chartFormActive.ChartFormManager.SequencerForm.SequencerControl.OlvParameterPopulate();
		}
		#endregion SlidersForm.Instance.SlidersAutoGrow


		//internal void DataSourceEditorControl_DataSourceEdited_updateDataSourcesTreeControl(object sender, DataSourceEventArgs e) {
		//	// mouseover DataSource tree refreshes the OLV and the icon disappears after DataSourceEditor => change Streaming (even without Save, but Save is an official "event trigger")
		//	// WEIRD_BUT_NOT_ENOUGH DataSourcesForm.Instance.DataSourcesTreeControl.Invalidate();
		//	DataSourcesForm.Instance.DataSourcesTreeControl.Refresh();
		//}
	}
}
