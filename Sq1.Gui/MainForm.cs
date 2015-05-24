using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.Indicators;
using Sq1.Core.Serializers;
using Sq1.Core.StrategyBase;
using Sq1.Core.Support;
using Sq1.Gui.Forms;
using Sq1.Gui.Singletons;
using WeifenLuo.WinFormsUI.Docking;

namespace Sq1.Gui {
	public partial class MainForm : Form {
		const string GUI_THREAD_NAME = "GUI_THREAD"; // USED_WHEN_base.InvokeRequired_THROWS_#D SHARP_DEVELOP_THROWS_WHEN_TRYING_TO_POPUP_EXCEPTION_FROM_QUIK_TERMINAL_MOCK_THREAD

		public	MainFormEventManager			MainFormEventManager;
		public	MainFormWorkspacesManager		WorkspacesManager;
		public	GuiDataSnapshot					GuiDataSnapshot;
		public	Serializer<GuiDataSnapshot>		GuiDataSnapshotSerializer;
		public	bool							MainFormClosingSkipChartFormsRemoval;
		public	bool							dontSaveXml_ignoreActiveContentEvents_whileLoadingAnotherWorkspace { get; private set; }

		public ChartForm ChartFormActiveNullUnsafe { get {
				var ret = this.DockPanel.ActiveDocument as ChartForm;
				if (ret == null) {
					string msg = "MainForm.DockPanel.ActiveDocument is not a ChartForm; no charts open or drag your chart into DOCUMENT docking area";
					//throw new Exception(msg);
					return null;
				}
				foreach (ChartFormsManager chartFormDataSnap in this.GuiDataSnapshot.ChartFormsManagers.Values) {
					if (chartFormDataSnap.ChartForm == ret) return ret;
				}
				string msg2 = "MainForm.DockPanel.ActiveDocument is [" + ret.ToString() + "] but it's not found among MainForm.ChartFormsManagers registry;"
					+ "1) did you forget to add? 2) MainForm.ChartFormsManagers doesn't have DockContent-restored Forms added?";
				throw new Exception(msg2);
			} }

		public MainForm() {
			InitializeComponent();
			#if DEBUG
			this.lblSpace.Text = "   ||   ";
			#endif

			try {
				Assembler.InstanceUninitialized.Initialize(this as IStatusReporter);
				this.GuiDataSnapshotSerializer = new Serializer<GuiDataSnapshot>();
	
				DataSourceEditorForm.Instance.DataSourceEditorControl.InitializeContext(Assembler.InstanceInitialized);
				DataSourceEditorForm.Instance.DataSourceEditorControl.InitializeAdapters(
					Assembler.InstanceInitialized.RepositoryDllStreamingAdapter	.CloneableInstanceByClassName,
					Assembler.InstanceInitialized.RepositoryDllBrokerAdapter	.CloneableInstanceByClassName);
	
				DataSourcesForm		.Instance.Initialize(Assembler.InstanceInitialized.RepositoryJsonDataSource);
				StrategiesForm		.Instance.Initialize(Assembler.InstanceInitialized.RepositoryDllJsonStrategy);
				ExecutionForm		.Instance.Initialize(Assembler.InstanceInitialized.OrderProcessor);
				CsvImporterForm		.Instance.Initialize(Assembler.InstanceInitialized.RepositoryJsonDataSource);
				SymbolEditorForm	.Instance.Initialize(Assembler.InstanceInitialized.RepositorySymbolInfo);

				this.WorkspacesManager = new MainFormWorkspacesManager(this, Assembler.InstanceInitialized.WorkspacesRepository);
			} catch (Exception ex) {
				Assembler.PopupException("ASSEMBLER_OR_SINGLETONS_FAILED //MainForm()", ex);
			}
		}
		public void WorkspaceLoad(string workspaceToLoad = null) {
			bool dockContentWillBeReCreated = true;
			if (string.IsNullOrEmpty(workspaceToLoad)) {
				workspaceToLoad = Assembler.InstanceInitialized.AssemblerDataSnapshot.CurrentWorkspaceName;
				dockContentWillBeReCreated = false;
			}

			Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete = false;
			try {
				// it looks like ChartForm doesn't propagate its DockContent-set size to ChartControl =>
				// for wider than in Designer ChartConrtrol sizes I see gray horizontal lines and SliderOutOfBoundaries Exceptions for smaller than in Designer
				// (Disable Resize during DockContent XML deserialization and fire manually for each ChartForm (Document only?) )
				this.SuspendLayout();


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
					//    if (each.GetType().IsSubClassOfGeneric(typeof(DockContentSingleton<>))) continue;
					//    DockContent eachForm = each as DockContent;
					//    if (eachForm == null) continue;
					//    ChartForm eachChart = each as ChartForm;
					//    if (eachChart == null) continue;
					//    eachChart.HideOnClose = false;
					//    eachChart.Close();
					//    eachChart.Dispose();
					//}
					//v2
					foreach (ChartFormsManager cfm in this.GuiDataSnapshot.ChartFormsManagers.Values) {
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



				if (Assembler.InstanceInitialized.AssemblerDataSnapshot.CurrentWorkspaceName != workspaceToLoad) {
					Assembler.InstanceInitialized.AssemblerDataSnapshot.CurrentWorkspaceName  = workspaceToLoad;
					Assembler.InstanceInitialized.AssemblerDataSnapshotSerializer.Serialize();
				}
				bool createdNewFile = this.GuiDataSnapshotSerializer.Initialize(Assembler.InstanceInitialized.AppDataPath,
					"Sq1.Gui.GuiDataSnapshot.json", "Workspaces",
					Assembler.InstanceInitialized.AssemblerDataSnapshot.CurrentWorkspaceName);

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
				this.WorkspacesManager.SyncMniEnabledAndSuggestNames();
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
				if (this.ChartFormActiveNullUnsafe != null) {
					//v1
					//this.ChartFormActive.ChartFormManager.PopulateSliders();
					//if (this.ChartFormActive.ChartFormManager.Strategy == null) {
					//	StrategiesForm.Instance.StrategiesTreeControl.UnSelectStrategy();
					//} else {
					//	StrategiesForm.Instance.StrategiesTreeControl.SelectStrategy(this.ChartFormActive.ChartFormManager.Strategy);
					//}
					this.ChartFormActiveNullUnsafe.ChartFormManager.PopulateMainFormSymbolStrategyTreesScriptParameters();
					this.ChartFormActiveNullUnsafe.Invalidate();	// onStartup, current chart is blank - MAY_FAIL when PANEL_HEIGHT_MUST_BE_POSITIVE but works otherwize
				}
	
				this.WorkspacesManager.SelectWorkspaceLoaded(workspaceToLoad);

				if (ExceptionsForm.Instance.ExceptionControl.Exceptions.Count > 0) {
					ExceptionsForm.Instance.Show(this.DockPanel);
				}

			
				Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete = true;
				if (disposePreviousDockPanel != null) disposePreviousDockPanel.Dispose();		// doesn't heal memory,handles,GDI,UserObj leak on same-workspace load

				foreach (ChartFormsManager cfmgr in this.GuiDataSnapshot.ChartFormsManagers.Values) {
					if (cfmgr.ChartForm == null) continue;
					if (cfmgr.ChartForm.MniShowSourceCodeEditor.Enabled) {		//set to true in InitializeWithStrategy() << DeserializeDockContent() 20 lines above
						cfmgr.ChartForm.MniShowSourceCodeEditor.Checked = cfmgr.ScriptEditorIsOnSurface;
					}

					cfmgr.ChartForm.ChartControl.PropagateSplitterManorderDistanceIfFullyDeserialized();

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

					//if (cfmgr.DataSnapshot.ContextChart.IsStreaming == true) {
					//	string msg = "CHART_SUBSCRIBED__BUT_SHOULD_CONNECT_AFTER_BACKTEST";
					//} else {
					if (cfmgr.Executor.DataSource.StreamingAdapter != null) {
						cfmgr.Executor.DataSource.StreamingAdapter.UpstreamConnect();
					}
					//}

					cfmgr.SequencerFormShow(true);
					cfmgr.CorrelatorFormShow(true);
					cfmgr.LivesimFormShow(true);
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
				Assembler.PopupException("WorkspaceLoad#1()", ex);
			} finally {
				// it looks like ChartForm doesn't propagate its DockContent-set size to ChartControl =>
				// for wider than in Designer ChartConrtrol sizes I see gray horizontal lines and SliderOutOfBoundaries Exceptions for smaller than in Designer
				// (Disable Resize during DockContent XML deserialization and fire manually for each ChartForm (Document only?) )
				this.ResumeLayout(true);
				this.dontSaveXml_ignoreActiveContentEvents_whileLoadingAnotherWorkspace = false;
			}
			try {
				if (ExecutionForm.Instance.IsShown) {
					ExecutionForm.Instance.ExecutionTreeControl.PopulateDataSnapshotInitializeSplittersIfDockContentDeserialized();
				}
				if (ExceptionsForm.Instance.IsShown) {
					ExceptionsForm.Instance.ExceptionControl.PopulateDataSnapshotInitializeSplittersAfterDockContentDeserialized();
				}
			} catch (Exception ex) {
				Assembler.PopupException("WorkspaceLoad#2()", ex);
			}
		}
		void MainFormEventManagerInitializeWhenDockingIsNotNullAnymore() {
			// OK_SO_LUO_PLAYS_WITH_WINDOWS.FORMS.VISIBLE_I_SEE Debugger.Break();
			DataSourcesForm	.Instance.VisibleChanged	+= delegate { this.mniDataSources	.Checked = DataSourcesForm	.Instance.Visible; };
			ExceptionsForm	.Instance.VisibleChanged	+= delegate { this.mniExceptions	.Checked = ExceptionsForm	.Instance.Visible; };
			SlidersForm		.Instance.VisibleChanged	+= delegate { this.mniSliders		.Checked = SlidersForm		.Instance.Visible; };
			StrategiesForm	.Instance.VisibleChanged	+= delegate { this.mniStrategies	.Checked = StrategiesForm	.Instance.Visible; };
			ExecutionForm	.Instance.VisibleChanged	+= delegate { this.mniExecution		.Checked = ExecutionForm	.Instance.Visible; };
			CsvImporterForm	.Instance.VisibleChanged	+= delegate { this.mniCsvImporter	.Checked = CsvImporterForm	.Instance.Visible; };
			SymbolEditorForm.Instance.VisibleChanged	+= delegate { this.mniSymbolsEditor	.Checked = SymbolEditorForm	.Instance.Visible; };

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
			Assembler.InstanceInitialized.RepositoryJsonDataSource.OnItemCanBeRemoved		+= new EventHandler<NamedObjectJsonEventArgs<DataSource>>(this.MainFormEventManager.RepositoryJsonDataSource_OnDataSourceCanBeRemoved);
			Assembler.InstanceInitialized.RepositoryJsonDataSource.OnItemRemovedDone		+= new EventHandler<NamedObjectJsonEventArgs<DataSource>>(this.MainFormEventManager.RepositoryJsonDataSource_OnDataSourceRemoved);
			//DataSourcesForm.Instance.DataSourcesTreeControl.OnDataSourceNewClicked		+= this.MainFormEventManager.DataSourcesTree_OnDataSourceNewClicked;

			// TYPE_MANGLING_INSIDE_WARNING NOTICE_THAT_BOTH_PARAMETER_SCRIPT_AND_INDICATOR_VALUE_CHANGED_EVENTS_ARE_HANDLED_BY_SINGLE_HANDLER
			SlidersForm.Instance.SlidersAutoGrowControl.SliderChangedParameterValue			+= new EventHandler<ScriptParameterEventArgs>(this.MainFormEventManager.SlidersAutoGrow_SliderValueChanged);
			SlidersForm.Instance.SlidersAutoGrowControl.SliderChangedIndicatorValue			+= new EventHandler<IndicatorParameterEventArgs>(this.MainFormEventManager.SlidersAutoGrow_SliderValueChanged);
			SlidersForm.Instance.SlidersAutoGrowControl.ScriptContextLoadRequestedSubscriberImplementsCurrentSwitch += this.MainFormEventManager.SlidersAutoGrow_OnScriptContextLoadClicked;
			SlidersForm.Instance.SlidersAutoGrowControl.ScriptContextRenamed				+= this.MainFormEventManager.SlidersAutoGrow_OnScriptContextRenamed;
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
