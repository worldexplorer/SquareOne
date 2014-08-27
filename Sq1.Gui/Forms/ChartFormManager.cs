using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Serializers;
using Sq1.Core.StrategyBase;
using Sq1.Gui.FormFactories;
using Sq1.Gui.Forms;
using Sq1.Gui.ReportersSupport;
using Sq1.Gui.Singletons;
using WeifenLuo.WinFormsUI.Docking;

namespace Sq1.Gui.Forms {
	public class ChartFormManager {
		public MainForm MainForm;
		public ChartFormDataSnapshot DataSnapshot;
		public Serializer<ChartFormDataSnapshot> DataSnapshotSerializer;
		
		bool FOR_DEBUGGING_initializedWithStrategyAfterDeserialization;
		// private shortcuts
		DockPanel dockPanel { get { return this.MainForm.DockPanel; } }

		public Strategy Strategy;
		public ScriptExecutor Executor;
		public ReportersFormsManager ReportersFormsManager;

		public ScriptEditorForm ScriptEditorForm;
		public ChartForm ChartForm;

		ScriptEditorFormFactory scriptEditorFormFactory;
		public ChartFormEventManager EventManager;
		public bool ScriptEditedNeedsSaving;
		public ChartFormStreamingConsumer ChartStreamingConsumer;
		
		public Dictionary<string, DockContent> FormsAllRelated {
			get {
				var ret = new Dictionary<string, DockContent>();
				if (this.ChartForm != null) ret.Add("Chart", this.ChartForm);
				if (this.ScriptEditorForm != null) ret.Add("Source Code", this.ScriptEditorForm);
				foreach (string textForMenuItem in this.ReportersFormsManager.FormsAllRelated.Keys) {
					ret.Add(textForMenuItem, this.ReportersFormsManager.FormsAllRelated[textForMenuItem]);
				}
				return ret;
			}
		}
		public ChartFormManager(int charSernoDeserialized = -1) {
			// deserialization: ChartSerno will be restored; never use this constructor in your app!
			this.ScriptEditedNeedsSaving = false;
//			this.Executor = new ScriptExecutor(Assembler.InstanceInitialized.ScriptExecutorConfig
//				, this.ChartForm.ChartControl, null, Assembler.InstanceInitialized.OrderProcessor, Assembler.InstanceInitialized.StatusReporter);
			this.Executor = new ScriptExecutor();
			this.ReportersFormsManager = new ReportersFormsManager(this, Assembler.InstanceInitialized.RepositoryDllReporters);
			this.ChartStreamingConsumer = new ChartFormStreamingConsumer(this);
			
			this.DataSnapshotSerializer = new Serializer<ChartFormDataSnapshot>();
			if (charSernoDeserialized == -1) {
				this.DataSnapshot = new ChartFormDataSnapshot();
				return;
			}
			bool createdNewFile = this.DataSnapshotSerializer.Initialize(Assembler.InstanceInitialized.AppDataPath,
				"ChartFormDataSnapshot-" + charSernoDeserialized + ".json", "Workspaces",
				Assembler.InstanceInitialized.AssemblerDataSnapshot.CurrentWorkspaceName, true, true);
			this.DataSnapshot = this.DataSnapshotSerializer.Deserialize();
			this.DataSnapshot.ChartSerno = charSernoDeserialized;
			this.DataSnapshotSerializer.Serialize();
		}
		public void InitializeChartNoStrategy(MainForm mainForm, ContextChart contextChart) {
			string msig = "ChartFormsManager.InitializeChartNoStrategy(" + contextChart + "): ";
			this.MainForm = mainForm;

			if (this.DataSnapshot.ChartSerno == -1) {
				int charSernoNext = mainForm.GuiDataSnapshot.ChartSernoNextAvailable;
				bool createdNewFile = this.DataSnapshotSerializer.Initialize(Assembler.InstanceInitialized.AppDataPath,
					"ChartFormDataSnapshot-" + charSernoNext + ".json", "Workspaces",
					Assembler.InstanceInitialized.AssemblerDataSnapshot.CurrentWorkspaceName, true, true);
				this.DataSnapshot = this.DataSnapshotSerializer.Deserialize();	// will CREATE a new ChartFormDataSnapshot and keep the reference for further Serialize(); we should fill THIS object
				this.DataSnapshot.ChartSerno = charSernoNext;
				this.DataSnapshotSerializer.Serialize();
			}

			this.ChartForm = new ChartForm(this);
			this.DataSnapshot.StrategyGuidJsonCheck = "NO_STRATEGY_CHART_ONLY";
			if (this.DataSnapshot.ChartSettings == null) {
				// delete "ChartSettings": {} from JSON to reset to ChartControl>Design>ChartSettings>Properties 
				this.DataSnapshot.ChartSettings = this.ChartForm.ChartControl.ChartSettings;	// opening from Datasource => save
			} else {
				this.ChartForm.ChartControl.ChartSettings = this.DataSnapshot.ChartSettings;	// otherwize JustDeserialized => propagate
				this.ChartForm.ChartControl.PropagateSettingSplitterDistancePriceVsVolume();
			}
			if (contextChart != null) {
				// contextChart != null when opening from Datasource; contextChart == null when JustDeserialized
				this.DataSnapshot.ContextChart = contextChart;
			}
			this.DataSnapshotSerializer.Serialize();

			this.ChartForm.FormClosed += this.MainForm.MainFormEventManager.ChartForm_FormClosed;

			//this.ChartForm.CtxReporters.Enabled = false;
			this.ChartForm.DdbReporters.Enabled = false;
			this.ChartForm.DdbBacktest.Enabled = false;
			this.ChartForm.DdbStrategy.Enabled = false;
			this.ChartForm.MniShowSourceCodeEditor.Enabled = false;
			
			this.EventManager = new ChartFormEventManager(this);
			this.ChartForm.AttachEventsToChartFormsManager();
						
			try {
				this.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(msig);
			} catch (Exception ex) {
				string msg = "PopulateCurrentChartOrScriptContext(): ";
				Assembler.PopupException(msg + msig, ex);
			}
		}
		public void InitializeWithStrategy(MainForm mainForm, Strategy strategy) {
			string msig = "ChartFormsManager.InitializeWithStrategy(" + strategy + "): ";

			this.MainForm = mainForm;
			this.Strategy = strategy;
			//this.Executor = new ScriptExecutor(mainForm.Assembler, this.Strategy);

			if (this.DataSnapshot.ChartSerno == -1) {
				int charSernoNext = mainForm.GuiDataSnapshot.ChartSernoNextAvailable;
				bool createdNewFile = this.DataSnapshotSerializer.Initialize(Assembler.InstanceInitialized.AppDataPath,
					"ChartFormDataSnapshot-" + charSernoNext + ".json", "Workspaces",
					Assembler.InstanceInitialized.AssemblerDataSnapshot.CurrentWorkspaceName, true, true);
				this.DataSnapshot = this.DataSnapshotSerializer.Deserialize();
				this.DataSnapshot.ChartSerno = charSernoNext;
			}
			this.DataSnapshot.StrategyGuidJsonCheck = strategy.Guid.ToString();
			this.DataSnapshotSerializer.Serialize();
			
			if (this.ChartForm == null) {
				// 1. create ChartForm.Chart.Renderer
				this.ChartForm = new ChartForm(this);
				this.ChartForm.FormClosed += this.MainForm.MainFormEventManager.ChartForm_FormClosed;
				// 2. create Executor with Renderer
				this.Executor.Initialize(this.ChartForm.ChartControl as ChartShadow,
				                         this.Strategy, Assembler.InstanceInitialized.OrderProcessor,
				                         Assembler.InstanceInitialized.StatusReporter);
				// 3. initialize Chart with Executor (I don't know why it should be so crazy)
				//this.ChartForm.Chart.Initialize(this.Executor);
				//ScriptExecutor.DataSource: you should not access DataSource before you've set Bars
				//this.ChartForm.ChartStreamingConsumer.Initialize(this);
	
				this.scriptEditorFormFactory = new ScriptEditorFormFactory(this, Assembler.InstanceInitialized.RepositoryDllJsonStrategy);
				this.ChartForm.CtxReporters.Items.AddRange(this.ReportersFormsManager.MenuItemsProvider.MenuItems.ToArray());
	
				this.EventManager = new ChartFormEventManager(this);
				this.ChartForm.AttachEventsToChartFormsManager();
			} else {
				// we had chart already opened with bars loaded; then we clicked on a strategy and we want strategy to be backtested on these bars
				this.Executor.Initialize(this.ChartForm.ChartControl as ChartShadow,
				                         this.Strategy, Assembler.InstanceInitialized.OrderProcessor,
				                         Assembler.InstanceInitialized.StatusReporter);
				if (this.ChartForm.CtxReporters.Items.Count == 0) {
					this.ChartForm.CtxReporters.Items.AddRange(this.ReportersFormsManager.MenuItemsProvider.MenuItems.ToArray());
				}
			}
			
			if (this.DataSnapshot.ChartSettings == null) {
				this.DataSnapshot.ChartSettings = this.ChartForm.ChartControl.ChartSettings;	// opening from Datasource => save
			} else {
				this.ChartForm.ChartControl.ChartSettings = this.DataSnapshot.ChartSettings;	// otherwize JustDeserialized => propagate
				this.ChartForm.ChartControl.PropagateSettingSplitterDistancePriceVsVolume();
			}

			//this.ChartForm.CtxReporters.Enabled = true;
			this.ChartForm.DdbReporters.Enabled = true;
			this.ChartForm.DdbStrategy.Enabled = true;
			this.ChartForm.DdbBacktest.Enabled = true;
			this.ChartForm.MniShowSourceCodeEditor.Enabled = !this.Strategy.ActivatedFromDll;

			try {
				//I'm here via Persist.Deserialize() (=> Reporters haven't been restored yet => backtest should be postponed); will backtest in InitializeStrategyAfterDeserialization
				this.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(msig, true, true);
			} catch (Exception ex) {
				string msg = "PopulateCurrentChartOrScriptContext(): ";
				Assembler.PopupException(msg + msig, ex);
			}
		}

		
		public ContextChart ContextCurrentChartOrStrategy { get {
				return (this.Strategy != null) ? this.Strategy.ScriptContextCurrent as ContextChart : this.DataSnapshot.ContextChart;
			} }

		public void PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(string msig, bool loadNewBars = true, bool skipBacktest = false) {
			//TODO abort backtest here if running!!! (wait for streaming=off) since ChartStreaming wrongly sticks out after upstack you got "Selectors should've been disabled" Exception 
			ContextChart context = this.ContextCurrentChartOrStrategy;
			msig += (this.Strategy != null) ?
					" << PopulateCurrentScriptContext(): Strategy[" + this.Strategy + "].ScriptContextCurrent[" + context.Name + "]"
				:	" << PopulateCurrentScriptContext(): this.ChartForm[" + this.ChartForm.Text + "].ChartControl.ContextChart[" + context.Name + "]";

			//BarScaleInterval barScaleInterval = (contextToPopulate.ScaleInterval != null) ? contextToPopulate.ScaleInterval : new BarScaleInterval();
			if (context.ScaleInterval.Scale == BarScale.Unknown) {
				string msg = "contextToPopulate.ScaleInterval[" + context.ScaleInterval + "] has BarScale.Unknown #1" + "; WILL_NOT_INITIALIZE Executor.Init(Strategy->BarsLoaded)";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (string.IsNullOrEmpty(context.DataSourceName)) {
				string msg = "DataSourceName.IsNullOrEmpty; WILL_NOT_INITIALIZE Executor.Init(Strategy->BarsLoaded)";
				Assembler.PopupException(msg + msig);
				return;
			}
			DataSource dataSource = Assembler.InstanceInitialized.RepositoryJsonDataSource.DataSourceFind(context.DataSourceName);
			if (dataSource == null) {
				string msg = "DataSourceName[" + context.DataSourceName + "] not found; WILL_NOT_INITIALIZE Executor.Init(Strategy->BarsLoaded)";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (string.IsNullOrEmpty(context.Symbol)) {
				string msg = "Strategy[" + this.Strategy + "].ScriptContextCurrent.Symbol.IsNullOrEmpty; WILL_NOT_INITIALIZE Executor.Init(Strategy->BarsLoaded)";
				Assembler.PopupException(msg + msig);
				return;
			}
			string symbol = context.Symbol;
			
			if (context.ChartStreaming) {
				string msg = "strategyToSave[" + this.Strategy + "].ScriptContextCurrent[].ChartStreaming=true"
					+ "; Selectors should've been Disable()d on chat[" + this.ChartForm + "].Activated() or StreamingOn()"
					+ " in MainForm.PropagateSelectorsForCurrentChart()";
				throw new Exception(msg + msig);
			}
			
			BarScaleInterval barScaleInterval = context.ScaleInterval;
			if (barScaleInterval.Scale == BarScale.Unknown) barScaleInterval = dataSource.ScaleInterval;
			if (barScaleInterval.Scale == BarScale.Unknown) {
				string msg = "barScaleInterval.ScaleInterval[" + barScaleInterval + "] has BarScale.Unknown #3"
					+ "; WILL_NOT_INITIALIZE Executor.Init(Strategy->BarsLoaded)";
				throw new Exception(msg + msig);
			}
			BarDataRange barRange = (context.DataRange != null) ? context.DataRange : new BarDataRange();
			//PositionSize posSize = (contextToPopulate.PositionSize != null) ? contextToPopulate.PositionSize : new PositionSize();
			
			if (loadNewBars) {
				Bars barsAll = dataSource.BarsLoadAndCompress(symbol, barScaleInterval);
				if (barsAll.Count > 0) {
					this.ChartForm.ChartControl.RangeBar.Initialize(barsAll, barsAll);
				}

				Bars barsClicked = barsAll.SelectRange(barRange);
				
				this.Executor.SetBars(barsClicked);
				this.ChartForm.ChartControl.Initialize(barsClicked);
				//SCROLL_TO_SNAPSHOTTED_BAR this.ChartForm.ChartControl.ScrollToLastBarRight();
				this.ChartForm.PopulateBtnStreamingClickedAndText();

				context.ScaleInterval = barScaleInterval;
				context.DataRange = barRange;
			}

			// set original Streaming Icon before we lost in simulationPreBarsSubstitute() and launched backtester in another thread
			//V1 this.Executor.Performance.Initialize();
			//V2_REPORTERS_NOT_REFRESHED this.Executor.BacktesterRunSimulation();
			var iconCanBeNull = this.Executor.DataSource.StreamingProvider != null ? this.Executor.DataSource.StreamingProvider.Icon : null;
			this.ChartForm.PropagateContextChartOrScriptToLTB(context, iconCanBeNull);

			// v1 already in ChartRenderer.OnNewBarsInjected event - commented out DoInvalidate();
			// v2 NOPE, during DataSourcesTree_OnSymbolSelected() we're invalidating it here! - uncommented back
			this.ChartForm.ChartControl.InvalidateAllPanelsFolding();
			
			if (this.Strategy == null) {
				this.DataSnapshotSerializer.Serialize();
				return;
			}
			this.Strategy.ScriptContextCurrent.PositionSize = this.Strategy.ScriptContextCurrent.PositionSize;
			Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.Strategy);
			
			if (skipBacktest) return;
			if (this.Strategy.ScriptContextCurrent.BacktestOnSelectorsChange == false) return;
			if (this.Strategy.Script == null) {
				//this.StrategyCompileActivatePopulateSlidersShow();
				string msg = "1) will compile it upstack in InitializeStrategyAfterDeserialization() or 2) compilation failed at Editor-F5";
				Debugger.Break();
				return;
			}
			this.BacktesterRunSimulationRegular();
		}
		public void BacktesterRunSimulationRegular() {
			try {
				this.Executor.BacktesterRunSimulationTrampoline(new Action(this.afterBacktesterComplete), true);
			} catch (Exception ex) {
				string msg = "RUN_SIMULATION_TRAMPOLINE_FAILED for Strategy[" + this.Strategy + "] on Bars[" + this.Executor.Bars + "]";
				Assembler.PopupException(msg, ex);
			}
		}
		void afterBacktesterComplete() {
			if (this.ChartForm.InvokeRequired) {
				this.ChartForm.BeginInvoke((MethodInvoker)delegate { this.afterBacktesterComplete(); });
				return;
			}
			//this.clonePositionsForChartPickupBacktest(this.Executor.ExecutionDataSnapshot.PositionsMaster);
			this.ChartForm.ChartControl.PositionsBacktestAdd(this.Executor.ExecutionDataSnapshot.PositionsMaster);
			this.ChartForm.ChartControl.PendingHistoryBacktestAdd(this.Executor.ExecutionDataSnapshot.AlertsPendingHistorySafeCopy);
			this.ChartForm.ChartControl.InvalidateAllPanelsFolding();
			
			this.Executor.Performance.BuildStatsOnBacktestFinished(this.Executor.ExecutionDataSnapshot.PositionsMaster);
			this.ReportersFormsManager.BuildOnceAllReports(this.Executor.Performance);
		}
		void afterBacktesterCompleteOnceOnRestart() {
			this.afterBacktesterComplete();
			
			if (this.Strategy == null) {
				Assembler.PopupException("this should never happen this.Strategy=null in afterBacktesterCompleteOnceOnRestart()");
				return;
			}
			this.ChartForm.PropagateContextChartOrScriptToLTB(this.Strategy.ScriptContextCurrent);
			if (this.Strategy.ScriptContextCurrent.ChartStreaming) this.ChartStreamingConsumer.StartStreaming();
		}
		public void InitializeChartNoStrategyAfterDeserialization(MainForm mainForm) {
			this.InitializeChartNoStrategy(mainForm, null);
		}
		public void InitializeStrategyAfterDeserialization(MainForm mainForm, string strategyGuid) {
			Strategy strategyFound = null;
			if (String.IsNullOrEmpty(strategyGuid) == false) {
				strategyFound = Assembler.InstanceInitialized.RepositoryDllJsonStrategy.LookupByGuid(strategyGuid); 	// can return NULL here
			}
			this.InitializeWithStrategy(mainForm, strategyFound);
			this.FOR_DEBUGGING_initializedWithStrategyAfterDeserialization = true;
			if (strategyFound == null) {
				string msg = "STRATEGY_NOT_FOUND: RepositoryDllJsonStrategy.LookupByGuid(strategyGuid=" + strategyGuid + ")";
				Assembler.PopupException(msg);
				return;
			}
			// ALREADY_DONE_BY_InitializeWithStrategy()_ABOVE this.Strategy = strategyFound;
			if (this.Strategy.ScriptContextCurrent.BacktestOnRestart == false) return;
			
			this.Strategy.CompileInstantiate();	//this.Strategy is initialized in this.Initialize(); mess comes from StrategiesTree_OnStrategyOpenSavedClicked() (TODO: reduce multiple paths)
			if (this.Strategy.Script == null) {
				string msig = " InitializeStrategyAfterDeserialization(" + this.Strategy.ToString() + ")";
				string msg = "COMPILATION_FAILED_AFTER_DESERIALIZATION BACKTEST_ON_RESTART_TURNED_OFF";
				Assembler.PopupException(msg + msig);
				this.Strategy.ScriptContextCurrent.BacktestOnRestart = false;
				Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.Strategy);
#if DEBUG
				Debugger.Break();
#endif
				return;
			}
			if (this.Strategy.Script.Executor == null) {
				//IM_GETTING_HERE_ON_STARTUP_AFTER_SUCCESFULL_COMPILATION_CHART_RELATED_STRATEGIES Debugger.Break();	// you should never get here; a compiled script should've been already linked to Executor (without bars on deserialization) 10 lines above in this.InitializeWithStrategy(mainForm, strategy);
				this.Strategy.Script.Initialize(this.Executor);
			}
			// make sure so that reporters will get poked
			this.Executor.BacktesterRunSimulationTrampoline(new Action(this.afterBacktesterCompleteOnceOnRestart), true);
			//NOPE_ALREADY_POPULATED_UPSTACK this.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsBacktestIfStrategy("InitializeStrategyAfterDeserialization()");
		}
		public void ReportersDumpCurrentForSerialization() {
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			if (this.Strategy == null) return;
			this.Strategy.ScriptContextCurrent.ReporterShortNamesUserInvokedJSONcheck =
				new List<string>(this.ReportersFormsManager.ReporterShortNamesUserInvoked.Keys);
		}
		public ScriptEditorForm ScriptEditorFormConditionalInstance {
			get {
				if (this.formIsNullOrDisposed(this.ScriptEditorForm)) {
					if (this.Strategy.ActivatedFromDll == true) return null;
					this.scriptEditorFormFactory.CreateEditorFormAndSubscribeFactoryMethod(this);
					if (this.ScriptEditorForm == null) {
						throw new Exception("ScriptEditorFormFactory.CreateAndSubscribe() failed to create ScriptEditorForm in ChartFormsManager");
					}
				}
				return this.ScriptEditorForm;
			}
		}
		public bool EditorFormIsNotDisposed {
			get { return this.formIsNullOrDisposed(this.ScriptEditorForm); }
		}
		bool formIsNullOrDisposed(Form form) {
			if (form == null) return true;
			if (form.IsDisposed) return true;
			return false;	//!this.chartForm.IsHidden;
		}
		public void ChartFormShow(string scriptContext = null) {
			this.ChartForm.ShowAsDocumentTabNotPane(this.dockPanel);
			if (this.Strategy != null) {
				this.PopulateWindowTitlesFromChartContextOrStrategy();
				this.EditorFormShow();
			}
		}
		public void EditorFormShow(bool keepAutoHidden = true) {
			if (this.Strategy.ActivatedFromDll == true) return;
			this.ScriptEditorFormConditionalInstance.Initialize(this);

			DockPanel mainPanelOranotherEditorsPanel = this.dockPanel;
			ScriptEditorForm anotherEditor = null;
			foreach (DockContent form in this.dockPanel.Contents) {
				anotherEditor = form as ScriptEditorForm;
				if (anotherEditor == null) continue;
				mainPanelOranotherEditorsPanel = anotherEditor.Pane.DockPanel;
				break;
			}

			this.ScriptEditorFormConditionalInstance.Show(mainPanelOranotherEditorsPanel);
			DockHelper.ActivateDockContentPopupAutoHidden(this.ScriptEditorFormConditionalInstance, keepAutoHidden);
			//this.ScriptEditorFormConditionalInstance.Show(this.dockPanel, DockState.Document);
			this.ChartForm.MniShowSourceCodeEditor.Checked = !keepAutoHidden;
		}

		const string prefixWhenNeedsToBeSaved = "* ";
		internal void PopulateWindowTitlesFromChartContextOrStrategy() {
			if (this.Strategy == null) {
				//string msg = "ChartFormsManager doesn't have a pointer to Strategy; Opening a Chart without Strategy is NYI";
				//throw new Exception(msg);
				this.ChartForm.Text = this.DataSnapshot.ContextChart.ToString();
				return;
			}
			string windowTitle = this.Strategy.Name;
			if (this.ScriptEditedNeedsSaving) windowTitle = prefixWhenNeedsToBeSaved + windowTitle;
			if (this.ScriptEditorForm != null) {
				this.ScriptEditorForm.Text = windowTitle;
			}
			//ALWAYS_NOT_NULL REDUNDANT if (this.ChartForm != null) {
				this.ChartForm.Text = windowTitle;
				if (this.Strategy.ActivatedFromDll == true) this.ChartForm.Text += "-DLL";
				this.ChartForm.IsHidden = false;
			//}
		}
		public void StrategyCompileActivatePopulateSlidersBeforeShow() {
			if (this.Strategy.ActivatedFromDll) {
				string msg = "WONT_COMPILE_STRATEGY_ACTIVATED_FROM_DLL_SHOULD_HAVE_NO_OPTION_IN_UI_TO_COMPILE_IT " + this.Strategy.ToString();
				Assembler.PopupException(msg);
				return;
			}
			if (string.IsNullOrEmpty(this.Strategy.ScriptSourceCode)) {
				string msg = "WONT_COMPILE_STRATEGY_HAS_EMPTY_SOURCE_CODE_PLEASE_TYPE_SOMETHING";
				Assembler.PopupException(msg);
				return;
			}
			this.Strategy.CompileInstantiate();
			if (this.Strategy.Script == null) {
				this.ScriptEditorFormConditionalInstance.ScriptEditorControl.PopulateCompilerErrors(this.Strategy.ScriptCompiler.CompilerErrors);
			} else {
				this.ScriptEditorFormConditionalInstance.ScriptEditorControl.PopulateCompilerSuccess();
				this.Strategy.Script.Initialize(this.Executor);
			}
			// moved to StrategyCompileActivatePopulateSlidersShow() because no need to PopulateSliders during Deserialization
			//SlidersForm.Instance.Initialize(this.Strategy);
		}
		public void StrategyCompileActivatePopulateSlidersShow() {
			if (this.Strategy.ActivatedFromDll == false) {
				this.StrategyCompileActivatePopulateSlidersBeforeShow();
			}
			SlidersForm.Instance.Initialize(this.Strategy);
			SlidersForm.Instance.Show(this.dockPanel);
		}
		public override string ToString() {
			return "Strategy[" + this.Strategy.Name + "], Chart [" + this.ChartForm.ToString() + "]";
		}
	}
}
