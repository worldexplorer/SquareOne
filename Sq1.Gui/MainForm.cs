using System;
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

		public MainFormEventManager			MainFormEventManager;
		public MainFormWorkspacesManager	WorkspacesManager;
		public GuiDataSnapshot				GuiDataSnapshot;
		public Serializer<GuiDataSnapshot>	GuiDataSnapshotSerializer;
		public bool							MainFormClosingSkipChartFormsRemoval;

		public ChartForm ChartFormActiveNullUnsafe { get {
				var ret = this.DockPanel.ActiveDocument as ChartForm;
				if (ret == null) {
					string msg = "MainForm.DockPanel.ActiveDocument is not a ChartForm; no charts open or drag your chart into DOCUMENT docking area";
					//throw new Exception(msg);
					return null;
				}
				foreach (ChartFormsManager chartFormDataSnap in this.GuiDataSnapshot.ChartFormManagers.Values) {
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

			//string currentThreadName;
			//try {
			//	currentThreadName = Thread.CurrentThread.Name;	// SharpDevelop4.4 Debugger doesn't evaluate, so I assign to visualize it 
			//	Thread.CurrentThread.Name = GUI_THREAD_NAME;
			//	currentThreadName = Thread.CurrentThread.Name;
			//} catch (Exception ex) {
			//	string msg = "FAILED_TO_SET_GUI_THREAD_NAME Thread.CurrentThread.Name=[" + GUI_THREAD_NAME + "] //MainForm()";
			//	Assembler.PopupException(msg, ex);
			//}

			try {
				Assembler.InstanceUninitialized.Initialize(this as IStatusReporter);
				this.GuiDataSnapshotSerializer = new Serializer<GuiDataSnapshot>();
	
				DataSourceEditorForm.Instance.DataSourceEditorControl.InitializeContext(Assembler.InstanceInitialized);
				DataSourceEditorForm.Instance.DataSourceEditorControl.InitializeAdapters(
					Assembler.InstanceInitialized.RepositoryDllStreamingAdapter.CloneableInstanceByClassName,
					Assembler.InstanceInitialized.RepositoryDllBrokerAdapter.CloneableInstanceByClassName);
	
				DataSourcesForm		.Instance.Initialize(Assembler.InstanceInitialized.RepositoryJsonDataSource);
				StrategiesForm		.Instance.Initialize(Assembler.InstanceInitialized.RepositoryDllJsonStrategy);
				ExecutionForm		.Instance.Initialize(Assembler.InstanceInitialized.OrderProcessor);
				CsvImporterForm		.Instance.Initialize(Assembler.InstanceInitialized.RepositoryJsonDataSource);
				SymbolEditorForm	.Instance.Initialize(Assembler.InstanceInitialized.RepositorySymbolInfo);
			} catch (Exception ex) {
				Assembler.PopupException("ASSEMBLER_OR_SINGLETONS_FAILED //MainForm()", ex);
			}
		}

		void createWorkspacesManager() {
			this.WorkspacesManager = new MainFormWorkspacesManager(this);
			//this.CtxWorkspaces.Items.Clear();
			this.CtxWorkspaces.Items.AddRange(this.WorkspacesManager.WorkspaceMenuItemsWithHandlers);
			this.mniWorkspaceDeleteCurrent.Click		+= new EventHandler(this.WorkspacesManager.WorkspaceDeleteCurrent_Click);
			this.mniltbWorklspaceCloneTo.UserTyped		+= new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.WorkspacesManager.WorkspaceCloneTo_UserTyped);
			this.mniltbWorklspaceRenameTo.UserTyped		+= new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.WorkspacesManager.WorkspaceRenameTo_UserTyped);
			this.mniltbWorklspaceNewBlank.UserTyped		+= new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.WorkspacesManager.WorkspaceNewBlank_UserTyped);
		}
		public void WorkspaceLoad(string workspaceToLoad) {
			try {
				// it looks like ChartForm doesn't propagate its DockContent-set size to ChartControl =>
				// for wider than in Designer ChartConrtrol sizes I see gray horizontal lines and SliderOutOfBoundaries Exceptions for smaller than in Designer
				// (Disable Resize during DockContent XML deserialization and fire manually for each ChartForm (Document only?) )
				this.SuspendLayout();
				
				if (Assembler.InstanceInitialized.AssemblerDataSnapshot.CurrentWorkspaceName != workspaceToLoad) {
					Assembler.InstanceInitialized.AssemblerDataSnapshot.CurrentWorkspaceName = workspaceToLoad;
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
					
					// re-reading Workspaces\ since I just created one, and before it was empty; copy-paste from initializeWorkspacesManagerTrampoline()
					Assembler.InstanceInitialized.WorkspacesRepository.ScanFolders();
					this.WorkspacesManager = new MainFormWorkspacesManager(this);
					this.CtxWorkspaces.Items.AddRange(this.WorkspacesManager.WorkspaceMenuItemsWithHandlers);
				}
				//this.DataSnapshot.RebuildDeserializedChartFormsManagers(this);
				
				//foreach (Form each in this.OwnedForms) each.Close();
				foreach (IDockContent each in this.DockPanel.Documents) {
					var form = each as DockContent;
					form.Close();
				}
				foreach (FloatWindow each in this.DockPanel.FloatWindows) each.Close(); 
				foreach (DockWindow each in this.DockPanel.DockWindows) {
					//each.Close();
				}
	
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

				foreach (ChartFormsManager cfmgr in this.GuiDataSnapshot.ChartFormManagers.Values) {
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
			this.DockPanel.SaveAsXml(this.LayoutXml);
			this.GuiDataSnapshotSerializer.Serialize();
			// nope, I'm dumping when ReporterShortNamesUserInvoked.Add() & Remove()
			//foreach (var chart in this.DataSnapshot.ChartFormsManagers) {
			//	chart.DumpCurrentReportersForSerialization();
			//}
		}

	}
}
