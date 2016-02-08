using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;
using Sq1.Core.Serializers;
using Sq1.Core.StrategyBase;
using Sq1.Core.Streaming;
using Sq1.Core.Livesim;
using Sq1.Core.Support;

using Sq1.Widgets.DataSourceEditor;

using Sq1.Gui.Forms;
using Sq1.Gui.Singletons;

namespace Sq1.Gui {
	public partial class MainForm : Form {
		const string GUI_THREAD_NAME = "GUI_THREAD"; // USED_WHEN_base.InvokeRequired_THROWS_#D SHARP_DEVELOP_THROWS_WHEN_TRYING_TO_POPUP_EXCEPTION_FROM_QUIK_TERMINAL_MOCK_THREAD

		public	MainFormEventManager			MainFormEventManager;
		public	MainFormWorkspacesManager		WorkspacesManager;
		public	GuiDataSnapshot					GuiDataSnapshot;
		public	Serializer<GuiDataSnapshot>		GuiDataSnapshotSerializer;
		public	bool							MainFormClosing_skipChartFormsRemoval_serializeExceptionsToPopupInNotepad;
		public	bool							dontSaveXml_ignoreActiveContentEvents_whileLoadingAnotherWorkspace { get; private set; }

		public ChartForm ChartFormActive_nullUnsafe { get {
				if (this.DockPanel.ActiveDocument == null) {
					string msg = "MainForm.DockPanel.ActiveDocument is not a ChartForm; no charts open or drag your chart into DOCUMENT docking area";
					Assembler.PopupException(msg, null, false);
					return null;
				}

				ChartForm ret = this.DockPanel.ActiveDocument as ChartForm;
				if (ret == null) {
					string msg = "NO_WORRIES__DATA_SOURCE_EDITOR_TAB_IS_CURRENTLY_DISPLAYED_IN_DOCUMENT_PANE"
						+ " MainForm.DockPanel.ActiveDocument is [" + this.DockPanel.ActiveDocument + "]";
					#if DEBUG_HEAVY
					Assembler.PopupException(msg2;
					#endif
					return null;
				}
				foreach (ChartFormManager chartFormDataSnap in this.GuiDataSnapshot.ChartFormManagers.Values) {
					if (chartFormDataSnap.ChartForm == ret) return ret;
				}
				string msg2 = "MainForm.DockPanel.ActiveDocument is [" + this.DockPanel.ActiveDocument + "] but it's not found among MainForm.ChartFormsManagers registry;"
					+ "1) did you forget to add? 2) MainForm.ChartFormsManagers doesn't have DockContent-restored Forms added?";
				Assembler.PopupException(msg2);
				return null;
			} }

		public MainForm() {
			InitializeComponent();
			#if DEBUG
			this.lblSpace.Text = "   ||   ";
			#endif

			try {
				Assembler.InstanceUninitialized.Initialize(this as IStatusReporter);
				this.GuiDataSnapshotSerializer = new Serializer<GuiDataSnapshot>();

				Assembler assemblerInstanceInitialized = Assembler.InstanceInitialized;
				DataSourceEditorControl dataSourceEditorControlInstance = DataSourceEditorForm.Instance.DataSourceEditorControl;
				dataSourceEditorControlInstance.InitializeContext(
					assemblerInstanceInitialized.RepositoryDllStreamingAdapters	.CloneableInstanceByClassName,
					assemblerInstanceInitialized.RepositoryDllBrokerAdapters	.CloneableInstanceByClassName,
					assemblerInstanceInitialized.RepositoryJsonDataSources,
					assemblerInstanceInitialized.RepositoryMarketInfos,
					assemblerInstanceInitialized.OrderProcessor);
	
				DataSourcesForm				.Instance.Initialize(Assembler.InstanceInitialized.RepositoryJsonDataSources);
				StrategiesForm				.Instance.Initialize(Assembler.InstanceInitialized.RepositoryDllJsonStrategies);
				ExecutionForm				.Instance.Initialize(Assembler.InstanceInitialized.OrderProcessor);
				CsvImporterForm				.Instance.Initialize(Assembler.InstanceInitialized.RepositoryJsonDataSources);
				SymbolInfoEditorForm		.Instance.Initialize(Assembler.InstanceInitialized.RepositorySymbolInfos, Assembler.InstanceInitialized.RepositoryJsonDataSources);

				this.WorkspacesManager = new MainFormWorkspacesManager(this, Assembler.InstanceInitialized.WorkspacesRepository);
			} catch (Exception ex) {
				Assembler.PopupException("ASSEMBLER_OR_SINGLETONS_FAILED //MainForm()", ex);
			}
		}
		public void WorkspaceLoad(string workspaceToLoad = null) {
			bool dockContentWillBeReCreated = true;
			if (string.IsNullOrEmpty(workspaceToLoad)) {
				workspaceToLoad = Assembler.InstanceInitialized.AssemblerDataSnapshot.WorkspaceCurrentlyLoaded;
				dockContentWillBeReCreated = false;
			}

			Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete = false;
			try {
				// it looks like ChartForm doesn't propagate its DockContent-set size to ChartControl =>
				// for wider than in Designer ChartConrtrol sizes I see gray horizontal lines and SliderOutOfBoundaries Exceptions for smaller than in Designer
				// (Disable Resize during DockContent XML deserialization and fire manually for each ChartForm (Document only?) )
				this.SuspendLayout();	// removes rounding error in propagateColumns/Rows


				////foreach (Form each in this.OwnedForms) each.Close();
				//foreach (IDockContent each in new List<IDockContent>(this.DockPanel.Documents)) {
				//	var form = each as DockContent;
				//	form.HideOnClose = false;
				//	form.Close();
				//}
				//foreach (FloatWindow each in new List<FloatWindow>(this.DockPanel.FloatWindows)) {
				//	each.Close(); 
				//}
				//foreach (DockWindow each in new List<DockWindow>(this.DockPanel.DockWindows)) {
				//	//each.Close();
				//}
				//if (this.DockPanel.Contents.Count > 0) {
				//	Debugger.Break();

				DockPanel disposePreviousDockPanel = null;
				if (dockContentWillBeReCreated) {
					this.dontSaveXml_ignoreActiveContentEvents_whileLoadingAnotherWorkspace = true;
					disposePreviousDockPanel = this.DockPanel;

					this.Controls.Remove(this.DockPanel);

					//v1 TOO_MESSY
					//foreach (IDockContent each in new List<IDockContent>(this.DockPanel.Contents)) {
					//	if (each.GetType().IsSubClassOfGeneric(typeof(DockContentSingleton<>))) continue;
					//	DockContent eachForm = each as DockContent;
					//	if (eachForm == null) continue;
					//	ChartForm eachChart = each as ChartForm;
					//	if (eachChart == null) continue;
					//	eachChart.HideOnClose = false;
					//	eachChart.Close();
					//	eachChart.Dispose();
					//}
					//v2
					foreach (ChartFormManager cfm in this.GuiDataSnapshot.ChartFormManagers.Values) {
						cfm.Dispose_workspaceReloading();
					}

					this.DockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
					this.DockPanel.BackColor = System.Drawing.SystemColors.ControlDark;
					this.DockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
					this.DockPanel.DockBottomPortion = 0.4D;
					this.DockPanel.DockLeftPortion = 0.15D;
					this.DockPanel.DockRightPortion = 0.35D;
					this.DockPanel.DockTopPortion = 0.3D;
					this.DockPanel.DocumentTabStripLocation = WeifenLuo.WinFormsUI.Docking.DocumentTabStripLocation.Bottom;
					this.DockPanel.Location = new System.Drawing.Point(0, 0);
					this.DockPanel.Name = "DockPanel";
					this.DockPanel.Size = new System.Drawing.Size(774, 423);
					this.Controls.Add(this.DockPanel);

					// ADDING_STATUS_STRIP_THE_LAST__OTHERWIZE_NEW_DOCK_CONTENT'S_BOTTOM_MINIMIZED_TABS_GO_UNDER_STATUS_STRIP
					this.Controls.Remove(this.mainFormStatusStrip);
					this.Controls.Add(this.mainFormStatusStrip);
				}



				if (Assembler.InstanceInitialized.AssemblerDataSnapshot.WorkspaceCurrentlyLoaded != workspaceToLoad) {
					Assembler.InstanceInitialized.AssemblerDataSnapshot.WorkspaceCurrentlyLoaded  = workspaceToLoad;
					Assembler.InstanceInitialized.AssemblerDataSnapshotSerializer.Serialize();
				}
				bool createdNewFile = this.GuiDataSnapshotSerializer.Initialize(Assembler.InstanceInitialized.AppDataPath,
					"Sq1.Gui.GuiDataSnapshot.json", "Workspaces",
					Assembler.InstanceInitialized.AssemblerDataSnapshot.WorkspaceCurrentlyLoaded);

				this.GuiDataSnapshot = this.GuiDataSnapshotSerializer.Deserialize();
				if (createdNewFile) {
					this.mainForm_LocationChanged(this, null);
					this.mainForm_ResizeEnd(this, null);
					this.GuiDataSnapshotSerializer.Serialize();
					
					//re-reading Workspaces\ since I just created one, and before it was empty; copy-paste from initializeWorkspacesManagerTrampoline()
					//v1
					//Assembler.InstanceInitialized.WorkspacesRepository.ScanFolders();
					//this.WorkspacesManager = new MainFormWorkspacesManager(this, Assembler.InstanceInitialized.WorkspacesRepository);
					//this.CtxWorkspaces.Items.AddRange(this.WorkspacesManager.WorkspaceMenuItemsWithHandlers);
					//v2
					this.WorkspacesManager.RescanRebuildWorkspacesMenu();
				}
				//this.WorkspacesManager.SyncMniEnabledAndSuggestNames();
				//this.DataSnapshot.RebuildDeserializedChartFormsManagers(this);


				string file = this.LayoutXml;
				if (File.Exists(file) == false) file = this.LayoutXmlInitial;
				if (File.Exists(file)) {
					DeserializeDockContent deserializeDockContent = new DeserializeDockContent(this.persistStringInstantiator);
					this.DockPanel.LoadFromXml(LayoutXml, deserializeDockContent);
				}

	
				this.initializeMainFromDeserializedDataSnapshot();
				this.mainFormEventManagerInitializeAfterDockingDeserialized();
	
				//this.PropagateSelectorsForCurrentChart();
				//WHY???this.MainFormEventManager.DockPanel_ActiveDocumentChanged(this, EventArgs.Empty);
				if (this.ChartFormActive_nullUnsafe != null) {
					this.ChartFormActive_nullUnsafe.ChartFormManager.PopulateThroughMainForm_symbolStrategyTree_andSliders();
					// onStartup, current chart is blank - MAY_FAIL when PANEL_HEIGHT_MUST_BE_POSITIVE but works otherwize
					//this.ChartFormActiveNullUnsafe.Invalidate();
					//BARS_ARE_STILL_NOT_PAINTER_ON_APPRESTART__MOVED_TO_SECOND_CFMGR_LOOP_180_LINES_BELOW this.ChartFormActiveNullUnsafe.ChartControl.InvalidateAllPanels();
					//DOESNT_HELP this.ChartFormActiveNullUnsafe.PerformLayout();
				}
	
				this.WorkspacesManager.SelectWorkspaceAfterLoaded(workspaceToLoad);

				if (ExceptionsForm.Instance.ExceptionControl.Exceptions.Count > 0) {
					ExceptionsForm.Instance.Show(this.DockPanel);
				}

				foreach (ChartFormManager cfmgr in this.GuiDataSnapshot.ChartFormManagers.Values) {
					if (cfmgr.ChartForm == null) continue;

					if (cfmgr.SequencerForm != null) {
						if (cfmgr.SequencerFormConditionalInstance.IsShown	== false) cfmgr.SequencerFormShow(true);
					} else {
						string msg = "SequencerForm wasn't mentioned in [" + this.LayoutXml + "]";
					}

					if (cfmgr.CorrelatorForm != null) {
						if (cfmgr.CorrelatorFormConditionalInstance.IsShown == false) cfmgr.CorrelatorFormShow(true);
					} else {
						string msg = "CorrelatorForm wasn't mentioned in [" + this.LayoutXml + "]";
					}

					if (cfmgr.LivesimForm != null) {
						if (cfmgr.LivesimFormConditionalInstance.IsShown	== false) cfmgr.LivesimFormShow(true);
					} else {
						string msg = "LivesimForm wasn't mentioned in [" + this.LayoutXml + "]";
					}

					// if sequencer instantiated first and fired, then correlator instantiated and didn't get the bullet; vice versa is even worse
					if (cfmgr.Executor.Strategy != null) {
						cfmgr.SequencerFormConditionalInstance.SequencerControl.RaiseOnCorrelatorShouldPopulate_usedByMainFormAfterBothAreInstantiated();
					}

					cfmgr.ChartFormShow();

					// INNER_DOCK_CONTENT_DOESNT_GET_FILLED_TO_THE_WINDOW_AREA???
					//cfmgr.ChartForm.ChartControl.InvalidateAllPanels();
				}
			
				if (disposePreviousDockPanel != null) {
					// MAKES_INNER_FORMS_CLUMSY_SIZED_AND_THROWS_INSIDE_WELFEN_LUO disposePreviousDockPanel.Dispose();		// doesn't heal memory,handles,GDI,UserObj leak on same-workspace load
				}

				foreach (ChartFormManager cfmgr in this.GuiDataSnapshot.ChartFormManagers.Values) {
					if (cfmgr.ChartForm == null) continue;
					if (cfmgr.ChartForm.MniShowSourceCodeEditor.Enabled) {		//set to true in InitializeWithStrategy() << DeserializeDockContent() 20 lines above
						cfmgr.ChartForm.MniShowSourceCodeEditor.Checked = cfmgr.ScriptEditorIsOnSurface;
					}

					//ADDED_ANOTHER_FOREACH_AFTER_ResumeLayout(true) cfmgr.ChartForm.ChartControl.PropagateSplitterManorderDistanceIfFullyDeserialized();

					Strategy chartStrategy = cfmgr.Executor.Strategy;
					if (chartStrategy == null) continue;
					if (chartStrategy.ActivatedFromDll == false) {
						string msg = "IRRELEVANT_FOR_EDITABLE_STRATEGIES WILL_PROCEED_WITHPANELS_COMPILATION_ETC editor-typed strategies already have indicators in SNAP after pre-backtest compilation";
						continue;
					}
					if (chartStrategy.Script == null) {
						string msg = "SCRIPT_COMPILES_OK_BUT_CTOR_THROWN_EXCEPTION_DURING_ACTIVATION_IN_RepositoryDllScanner<Script>.ScanDlls():107";
						continue;
					}
					
					//v1
					// INDICATORS_CLEARED_ADDED_AFTER_BACKTEST_STARTED "Collection was modified; enumeration operation may not execute."
					//if (chartStrategy.ScriptContextCurrent.BacktestOnRestart == false) {
					//	// need to instantiate all panels for all script indicators before distributing distances between them
					//	// COPIED_FROM ScriptExecutor.BacktesterRunSimulationTrampoline() FIXED "EnterEveryBar doesn't draw MAfast";
					//	chartStrategy.Script.IndicatorsInitializeMergeParamsFromJsonStoreInSnapshot();
					//}
					//v2
					#if DEBUG
					int indicatorsReflectedCount = chartStrategy.Script.IndicatorsByName_ReflectedCached.Count;
					if (indicatorsReflectedCount > 0) {
						bool mergedIfAny = chartStrategy.ScriptContextCurrent.IndicatorParametersByName.Count == indicatorsReflectedCount;
						if (mergedIfAny == false) Debugger.Break();
					}
					#endif

					//WILL_NEED_TO_COLLECT_QUOTES_FOR_SYMBOLS_WITHOUT_STREAMING_CHARTS_OPEN
					//if (cfmgr.DataSnapshot.ContextChart.IsStreaming == true) {
					//    string msg = "CHART_SUBSCRIBED__BUT_SHOULD_CONNECT_AFTER_BACKTEST";
					//} else {
						StreamingAdapter streaming = cfmgr.Executor.DataSource_fromBars.StreamingAdapter;
						if (streaming != null && (streaming is LivesimStreaming) == false) {
							streaming.UpstreamConnect();
						}
					//}
				}
				
				//NOPE ExecutionForm.Instance.ExecutionTreeControl.MoveStateColumnToLeftmost();
				if (ExecutionForm.Instance.IsShown) {
					// MOVED_TO_AFTER_MAINFORM_RESUMELAYOUT ExecutionForm.Instance.ExecutionTreeControl.PopulateDataSnapshotInitializeSplittersIfDockContentDeserialized();
					ExecutionForm.Instance.PopulateWindowText();
				}
				if (ExceptionsForm.Instance.IsShown) {
					// MOVED_TO_AFTER_MAINFORM_RESUMELAYOUT ExceptionsForm.Instance.ExceptionControl.PopulateDataSnapshotInitializeSplittersAfterDockContentDeserialized();
					// EXCEPTION_BORN_IN_GUI_THREAD_ARE_ALWAYS_ON_TIMER ExceptionsForm.Instance.ExceptionControl.FlushExceptionsToOLVIfDockContentDeserialized_inGuiThread();
				}
			} catch (Exception ex) {
				if (ex.Message == "The previous pane is invalid. It can not be null, and its docking state must not be auto-hide.") {
					foreach (DockPane eachPane in this.DockPanel.Panes) {
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
						Assembler.PopupException(msg, null, false);
					}
				}
				Assembler.PopupException("WorkspaceLoad#1()", ex);
			} finally {
				// it looks like ChartForm doesn't propagate its DockContent-set size to ChartControl =>
				// for wider than in Designer ChartControl sizes I see gray horizontal lines and SliderOutOfBoundaries Exceptions for smaller than in Designer
				// (Disable Resize during DockContent XML deserialization and fire manually for each ChartForm (Document only?) )
				this.ResumeLayout(true);	// removes rounding error in propagateColumns/Rows
				Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete = true;
				this.dontSaveXml_ignoreActiveContentEvents_whileLoadingAnotherWorkspace = false;
			}

			//NOT_NEEDED_AFTER_ChartForm.Size_FITS_DESIGNED_ChartControl.Size DOESNT_HELP base.PerformLayout();
			//NOT_NEEDED_AFTER_ChartForm.Size_FITS_DESIGNED_ChartControl.Size DOESNT_HELP this.DockPanel.PerformLayout();
			foreach (ChartFormManager cfmgr in this.GuiDataSnapshot.ChartFormManagers.Values) {
				if (cfmgr.ChartForm == null) {
					string msg = "CHART_FORM_MANAGER_MUST_HAVE_CHART_FORM_ASSIGNED_NOT_NULL";
					Assembler.PopupException(msg);
					continue;
				}

				//DOESNT_HELP cfmgr.ChartForm.SuspendLayout();
				//DOESNT_HELP
				if (cfmgr.ChartForm.ChartControl.Dock != DockStyle.Fill) {
					string msg = "YOU_FORGOT_TO_SET_ChartControl.Dock=DockStyle.Fill_IN_DESIGNER_OF_ChartForm";
					Assembler.PopupException(msg, null, false);
					//cfmgr.ChartForm.ChartControl.Dock = DockStyle.Fill;	// trying to trigger Dock.Fill to resize Control inside Form
				}
				//DOESNT_HELP cfmgr.ChartForm.ChartControl.PerformLayout();
				//DOESNT_HELP cfmgr.ChartForm.ResumeLayout(false);
				//MOVED_TO_cfmgr.ChartFormShow() cfmgr.ChartForm.PerformLayout();

				//WRONG_INVOKES_ON_LAYOUT 
				//if (cfmgr.ChartForm.ClientRectangle.Width != cfmgr.ChartForm.ChartControl.ClientRectangle.Width) {
				//    string msg = "YOU_FORGOT_TO_INVOKE_cfmgr.ChartForm[" + cfmgr.ChartForm + "].PerformLayout()";
				//    #if DEBUG_HEAVY
				//    Assembler.PopupException(msg, null, false);
				//    #endif
				//    //WRONG_INVOKES_ON_LAYOUT cfmgr.ChartForm.ChartControl.PerformLayout();
				//}

				if (cfmgr.ChartForm.ClientRectangle.Width != cfmgr.ChartForm.ChartControl.ClientRectangle.Width) {
					string msg = "SIZES_DONT_MATCH_DESPITE_INVOKED_EARLIER_BY_cfmgr.ChartFormShow() => cfmgr.ChartForm[" + cfmgr.ChartForm + "].PerformLayout() INVOKING_AGAIN_WILL_COMPLAIN_IF_NO_EFFECT";
					#if DEBUG_HEAVY
					Assembler.PopupException(msg, null, false);
					#endif
					cfmgr.ChartForm.PerformLayout();
					if (cfmgr.ChartForm.ClientRectangle.Width != cfmgr.ChartForm.ChartControl.ClientRectangle.Width) {
						string msg2 = "I_INVOKED_cfmgr.ChartForm[" + cfmgr.ChartForm + "].PerformLayout()_BUT_IT_DIDNT_HELP!!!";
						Assembler.PopupException(msg2, null, false);
					}
				}

				cfmgr.ChartForm.ChartControl.PropagateSplitterManorderDistanceIfFullyDeserialized();
			}
			if (this.ChartFormActive_nullUnsafe != null) {
				//+ainFrom.Deserializer on apprestart, Document.Active (ChartForm) doesn't paint Bars
				this.ChartFormActive_nullUnsafe.ChartControl.InvalidateAllPanels();
			}
			try {
				if (ExecutionForm.Instance.IsShown) {
					ExecutionForm.Instance.ExecutionTreeControl.PopulateDataSnapshotInitializeSplittersIfDockContentDeserialized();
				}
				if (ExceptionsForm.Instance.IsShown) {
					ExceptionsForm.Instance.ExceptionControl.PopulateDataSnapshotInitializeSplittersAfterDockContentDeserialized();
				}
				if (DataSourceEditorForm.Instance.IsShown) {
					// ON_WORKSPACE_LOAD__ActiveDocumentChanged_IS_NOT_INVOKED
					DataSourcesForm.Instance.ActivateDockContentPopupAutoHidden(false, true);
					string dsNameToSelect = DataSourceEditorForm.Instance.DataSourceEditorControl.DataSourceName;
					DataSourcesForm.Instance.DataSourcesTreeControl.SelectDatasource(dsNameToSelect);
				}

				//DOESNT_WORK trigger DataSourceTree to select the ActiveChart via full handler DockPanel_ActiveDocumentChanged();
				this.ChartFormActive_nullUnsafe.DockHandler.Pane.Activate();
			} catch (Exception ex) {
				Assembler.PopupException("WorkspaceLoad#2()", ex);
			}
		}
		void MainFormEventManagerInitializeWhenDockingIsNotNullAnymore() {
			// OK_SO_LUO_PLAYS_WITH_WINDOWS.FORMS.VISIBLE_I_SEE Debugger.Break();
			DataSourcesForm			.Instance.VisibleChanged	+= delegate { this.mniDataSources			.Checked = DataSourcesForm			.Instance.Visible; };
			ExceptionsForm			.Instance.VisibleChanged	+= delegate { this.mniExceptions			.Checked = ExceptionsForm			.Instance.Visible; };
			SlidersForm				.Instance.VisibleChanged	+= delegate { this.mniSliders				.Checked = SlidersForm				.Instance.Visible; };
			StrategiesForm			.Instance.VisibleChanged	+= delegate { this.mniStrategies			.Checked = StrategiesForm			.Instance.Visible; };
			ExecutionForm			.Instance.VisibleChanged	+= delegate { this.mniExecution				.Checked = ExecutionForm			.Instance.Visible; };
			CsvImporterForm			.Instance.VisibleChanged	+= delegate { this.mniCsvImporter			.Checked = CsvImporterForm			.Instance.Visible; };
			SymbolInfoEditorForm	.Instance.VisibleChanged	+= delegate { this.mniSymbolInfoEditor		.Checked = SymbolInfoEditorForm		.Instance.Visible; };
			ChartSettingsEditorForm	.Instance.VisibleChanged	+= delegate { this.mniChartSettingsEditor	.Checked = ChartSettingsEditorForm	.Instance.Visible; };

			this.MainFormEventManager = new MainFormEventManager(this);

			StrategiesForm.Instance.StrategiesTreeControl.OnStrategyOpenDefaultClicked		+= this.MainFormEventManager.StrategiesTree_OnStrategyOpenDefaultClicked_NewChart;
			StrategiesForm.Instance.StrategiesTreeControl.OnStrategyOpenSavedClicked		+= this.MainFormEventManager.StrategiesTree_OnStrategyLoadClicked;
			StrategiesForm.Instance.StrategiesTreeControl.OnStrategyRenamed					+= this.MainFormEventManager.StrategiesTree_OnStrategyRenamed;
			//StrategiesForm.Instance.StrategiesTreeControl.OnStrategySelected				+= this.MainFormEventManager.StrategiesTree_OnStrategySelected;
			StrategiesForm.Instance.StrategiesTreeControl.OnStrategyDoubleClicked			+= this.MainFormEventManager.StrategiesTree_OnStrategyDoubleClicked_NewChart;

			DataSourcesForm.Instance.DataSourcesTreeControl.OnSymbolSelected				+= this.MainFormEventManager.DataSourcesTree_OnSymbolSelected;
			DataSourcesForm.Instance.DataSourcesTreeControl.OnDataSourceSelected			+= this.MainFormEventManager.DataSourcesTree_OnDataSourceSelected;
			DataSourcesForm.Instance.DataSourcesTreeControl.OnNewChartForSymbolClicked		+= this.MainFormEventManager.DataSourcesTree_OnNewChartForSymbolClicked;
			DataSourcesForm.Instance.DataSourcesTreeControl.OnOpenStrategyForSymbolClicked	+= this.MainFormEventManager.DataSourcesTree_OnOpenStrategyForSymbolClicked;
			DataSourcesForm.Instance.DataSourcesTreeControl.OnBarsAnalyzerClicked			+= this.MainFormEventManager.DataSourcesTree_OnBarsAnalyzerClicked;
			DataSourcesForm.Instance.DataSourcesTreeControl.OnSymbolInfoEditorClicked		+= this.MainFormEventManager.DataSourcesTree_OnSymbolInfoEditorClicked;
			DataSourcesForm.Instance.DataSourcesTreeControl.OnDataSourceEditClicked			+= this.MainFormEventManager.DataSourcesTree_OnDataSourceEditClicked;
			//DataSourcesForm.Instance.DataSourcesTree.OnDataSourceDeleteClicked			+= this.MainFormEventManager.DataSourcesTree_OnDataSourceDeletedClicked;
			Assembler.InstanceInitialized.RepositoryJsonDataSources.OnItemCanBeRemoved		+= new EventHandler<NamedObjectJsonEventArgs<DataSource>>(this.MainFormEventManager.RepositoryJsonDataSources_OnDataSourceCanBeRemoved);
			Assembler.InstanceInitialized.RepositoryJsonDataSources.OnItemRemovedDone		+= new EventHandler<NamedObjectJsonEventArgs<DataSource>>(this.MainFormEventManager.RepositoryJsonDataSources_OnDataSourceRemoved);
			//DataSourcesForm.Instance.DataSourcesTreeControl.OnDataSourceNewClicked		+= this.MainFormEventManager.DataSourcesTree_OnDataSourceNewClicked;

			// TYPE_MANGLING_INSIDE_WARNING NOTICE_THAT_BOTH_PARAMETER_SCRIPT_AND_INDICATOR_VALUE_CHANGED_EVENTS_ARE_HANDLED_BY_SINGLE_HANDLER
			SlidersForm.Instance.SteppingSlidersAutoGrowControl.SliderChangedParameterValue			+= new EventHandler<ScriptParameterEventArgs>(this.MainFormEventManager.SlidersAutoGrow_SliderValueChanged);
			SlidersForm.Instance.SteppingSlidersAutoGrowControl.SliderChangedIndicatorValue			+= new EventHandler<IndicatorParameterEventArgs>(this.MainFormEventManager.SlidersAutoGrow_SliderValueChanged);
			SlidersForm.Instance.SteppingSlidersAutoGrowControl.ScriptContextLoadRequestedSubscriberImplementsCurrentSwitch += this.MainFormEventManager.SlidersAutoGrow_OnScriptContextLoadClicked;
			SlidersForm.Instance.SteppingSlidersAutoGrowControl.ScriptContextRenamed				+= this.MainFormEventManager.SlidersAutoGrow_OnScriptContextRenamed;

			DataSourceEditorForm.Instance.DataSourceEditorControl.DataSourceEdited_updateDataSourcesTreeControl += new EventHandler<DataSourceEventArgs>(this.MainFormEventManager.DataSourceEditorControl_DataSourceEdited_updateDataSourcesTreeControl);
		}
		void mainFormEventManagerInitializeAfterDockingDeserialized() {
			// too frequent
			//this.DockPanel.ActiveContentChanged += this.MainFormEventManager.DockPanel_ActiveContentChanged;
			// just as often as I needed!
			this.DockPanel.ActiveDocumentChanged += this.MainFormEventManager.DockPanel_ActiveDocumentChanged;
		}
		public void MainFormSerialize() {
			if (this.dontSaveXml_ignoreActiveContentEvents_whileLoadingAnotherWorkspace) return;
			this.DockPanel.SaveAsXml(this.LayoutXml);
			this.GuiDataSnapshotSerializer.Serialize();
			// nope, I'm dumping when ReporterShortNamesUserInvoked.Add() & Remove()
			//foreach (var chart in this.DataSnapshot.ChartFormsManagers) {
			//	chart.DumpCurrentReportersForSerialization();
			//}
		}
	}
}
