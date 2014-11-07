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
using Sq1.Widgets;
using WeifenLuo.WinFormsUI.Docking;

namespace Sq1.Gui.Forms {
	public class ChartFormManager {
		public MainForm MainForm;
		public ChartFormDataSnapshot DataSnapshot;
		public Serializer<ChartFormDataSnapshot> DataSnapshotSerializer;
		
		public bool StrategyFoundDuringDeserialization { get; private set; }
		// private shortcuts
		DockPanel dockPanel { get { return this.MainForm.DockPanel; } }

		public Strategy Strategy;
		public ScriptExecutor Executor;
		public ReportersFormsManager ReportersFormsManager;

		public ChartForm ChartForm;
		
		
		public ScriptEditorForm ScriptEditorForm;
		public ScriptEditorForm ScriptEditorFormConditionalInstance { get {
				if (DockContentImproved.IsNullOrDisposed(this.ScriptEditorForm)) {
					if (this.Strategy == null) return null;
					if (this.Strategy.ActivatedFromDll == true) return null;
						#if DEBUG
						//Debugger.Break();
						#endif
					if (this.scriptEditorFormFactory == null) {
						this.scriptEditorFormFactory = new ScriptEditorFormFactory(this, Assembler.InstanceInitialized.RepositoryDllJsonStrategy);
					}
					this.scriptEditorFormFactory.CreateEditorFormSubscribePushToManager(this);
					if (this.ScriptEditorForm == null) {
						throw new Exception("ScriptEditorFormFactory.CreateAndSubscribe() failed to create ScriptEditorForm in ChartFormsManager");
					}
				}
				return this.ScriptEditorForm;
			} }
		public bool EditorFormIsNotDisposed { get { return (DockContentImproved.IsNullOrDisposed(this.ScriptEditorForm) == false); } }
		public bool ScriptEditorIsOnSurface { get {
				bool editorMustBeActivated = true;
				ScriptEditorForm editor = this.ScriptEditorForm;
				bool editorNotInstantiated = DockContentImproved.IsNullOrDisposed(editor);
				if (editorNotInstantiated) return editorMustBeActivated;
				
				//bool hidden = editor.IsHidden;
				//SHOULD_INCLUDE_FLOATWINDOW_OR_DOCKED_BUT_COVERED_BY_OTHER_FELLAS bool undockToOpen = editor.IsDocked || editor.IsDockedAutoHide;
				//for DockedRightAutoHide+Folded, Control.Active=true (seems illogical)
				//for DockedRightAutoHide+Folded, DockContent.IsHidden=false (seems illogical)
				editorMustBeActivated = editor.IsCoveredOrAutoHidden;
				return !editorMustBeActivated;
			} }
		ScriptEditorFormFactory scriptEditorFormFactory;
		
		
		OptimizerFormFactory optimizerFormFactory;
		public OptimizerForm OptimizerForm;
		public OptimizerForm OptimizerFormConditionalInstance { get {
				if (DockContentImproved.IsNullOrDisposed(this.OptimizerForm)) {
					if (this.Strategy == null) return null;
					if (this.optimizerFormFactory == null) {
						#if DEBUG
						Debugger.Break();
						#endif
						this.optimizerFormFactory = new OptimizerFormFactory(this, Assembler.InstanceInitialized.RepositoryDllJsonStrategy);
					}

					this.optimizerFormFactory.CreateOptimizerFormSubscribePushToManager(this);
					if (this.OptimizerForm == null) {
						throw new Exception("OptimizerFormFactory.CreateAndSubscribe() failed to create OptimizerForm in ChartFormsManager");
					}
				}
				return this.OptimizerForm;
			} }
		public bool OptimizerFormIsNotDisposed { get { return (DockContentImproved.IsNullOrDisposed(this.OptimizerForm) == false); } }
		public bool OptimizerIsOnSurface { get {
				bool optimizerMustBeActivated = true;
				OptimizerForm optimizer = this.OptimizerForm;
				bool optimizerNotInstantiated = DockContentImproved.IsNullOrDisposed(optimizer);
				if (optimizerNotInstantiated) return optimizerMustBeActivated;
				optimizerMustBeActivated = optimizer.IsCoveredOrAutoHidden;
				return !optimizerMustBeActivated;
			} }
		
		public ChartFormEventManager EventManager;
		public bool ScriptEditedNeedsSaving;
		public ChartFormStreamingConsumer ChartStreamingConsumer;
		
		public Dictionary<string, DockContent> FormsAllRelated { get {
				var ret = new Dictionary<string, DockContent>();
				if (this.ChartForm != null) ret.Add("Chart", this.ChartForm);
				if (this.ScriptEditorForm != null) ret.Add("Source Code", this.ScriptEditorForm);
				if (this.OptimizerForm != null) ret.Add("Optimizer", this.OptimizerForm);
				foreach (string textForMenuItem in this.ReportersFormsManager.FormsAllRelated.Keys) {
					ret.Add(textForMenuItem, this.ReportersFormsManager.FormsAllRelated[textForMenuItem]);
				}
				return ret;
			} }
		public ChartFormManager(int charSernoDeserialized = -1) {
			this.StrategyFoundDuringDeserialization = false;
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
			if (this.DataSnapshot == null) {
				Debugger.Break();
			}
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
			this.DataSnapshot.StrategyGuidJsonCheck		= "NO_STRATEGY_CHART_ONLY";
			this.DataSnapshot.StrategyNameJsonCheck		= "NO_STRATEGY_CHART_ONLY";
			this.DataSnapshot.StrategyAbsPathJsonCheck	= "NO_STRATEGY_CHART_ONLY";


			if (this.DataSnapshot.ChartSettings == null) {
				// delete "ChartSettings": {} from JSON to reset to ChartControl>Design>ChartSettings>Properties
				this.DataSnapshot.ChartSettings = this.ChartForm.ChartControl.ChartSettings;	// opening from Datasource => save
			} else {
				this.ChartForm.ChartControl.ChartSettings = this.DataSnapshot.ChartSettings;	// otherwize JustDeserialized => propagate
				this.ChartForm.ChartControl.PropagateSplitterManorderDistanceIfFullyDeserialized();
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
		public void InitializeWithStrategy(MainForm mainForm, Strategy strategy, bool skipBacktestDuringDeserialization = true) {
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
			this.DataSnapshot.StrategyNameJsonCheck = strategy.Name;
			this.DataSnapshot.StrategyAbsPathJsonCheck = strategy.StoredInJsonAbspath;
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
				this.optimizerFormFactory = new OptimizerFormFactory(this, Assembler.InstanceInitialized.RepositoryDllJsonStrategy);
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
				this.ChartForm.ChartControl.PropagateSplitterManorderDistanceIfFullyDeserialized();
			}

			//this.ChartForm.CtxReporters.Enabled = true;
			this.ChartForm.DdbReporters.Enabled = true;
			this.ChartForm.DdbStrategy.Enabled = true;
			this.ChartForm.DdbBacktest.Enabled = true;
			this.ChartForm.MniShowSourceCodeEditor.Enabled = !this.Strategy.ActivatedFromDll;

			try {
				// Click on strategy should open new chart,  
				if (this.Strategy.ScriptContextCurrent.BacktestOnSelectorsChange == true && this.Strategy.ActivatedFromDll == false && this.Strategy.Script == null) {
					this.StrategyCompileActivatePopulateSlidersShow();
				}
				//I'm here via Persist.Deserialize() (=> Reporters haven't been restored yet => backtest should be postponed); will backtest in InitializeStrategyAfterDeserialization
				// STRATEGY_CLICK_TO_CHART_DOESNT_BACKTEST this.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(msig, true, true);
				// ALL_SORT_OF_STARTUP_ERRORS this.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(msig, true, false);
				this.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(msig, true, skipBacktestDuringDeserialization);
				if (skipBacktestDuringDeserialization == false) {
					this.OptimizerFormIfOpenPropagateTextboxesOrMarkStaleResults();
				}
			} catch (Exception ex) {
				string msg = "PopulateCurrentChartOrScriptContext(): ";
				Assembler.PopupException(msg + msig, ex);
				#if DEBUG
				Debugger.Break();
				#endif
			}
		}
		public ContextChart ContextCurrentChartOrStrategy { get {
				return (this.Strategy != null) ? this.Strategy.ScriptContextCurrent as ContextChart : this.DataSnapshot.ContextChart;
			} }
		public void PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(string msig, bool loadNewBars = true, bool skipBacktest = false) {
			//TODO abort backtest here if running!!! (wait for streaming=off) since ChartStreaming wrongly sticks out after upstack you got "Selectors should've been disabled" Exception
			this.Executor.BacktesterAbortIfRunningRestoreContext();

			ContextChart context = this.ContextCurrentChartOrStrategy;
			if (context == null) {
				string msg = "WONT_POPULATE_NULL_CONTEXT: strategy JSON/DLL was removedBetweenRestart / deserializedWithExceptionDueToDataFormatChange" +
					" + chart for strategy doesn't contain Context; expect also Bars=NULL exception";
				Assembler.PopupException(msg);
				return;
			}
			this.PopulateWindowTitlesFromChartContextOrStrategy();
			msig += (this.Strategy != null) ?
				" << PopulateCurrentScriptContext(): Strategy[" + this.Strategy.ToString() + "].ScriptContextCurrent[" + context.Name + "]"
				:	" << PopulateCurrentScriptContext(): this.ChartForm[" + this.ChartForm.Text + "].ChartControl.ContextChart[" + context.Name + "]";

			if (string.IsNullOrEmpty(context.DataSourceName)) {
				string msg = "DataSourceName.IsNullOrEmpty; WILL_NOT_INITIALIZE Executor.Init(Strategy->BarsLoaded)";
				Assembler.PopupException(msg + msig);
				return;
			}
			DataSource dataSource = Assembler.InstanceInitialized.RepositoryJsonDataSource.DataSourceFindNullUnsafe(context.DataSourceName);
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
				string msg = "CHART_STREAMING_DISABLED_FORCIBLY_POSSIBLY_ENABLED_BY_OPENCHART_OR_NOT_ABORTED_UNFINISHED_BACKTEST ContextCurrentChartOrStrategy.ChartStreaming=true"
					+ "; Selectors should've been Disable()d on chat[" + this.ChartForm + "].Activated() or StreamingOn()"
					+ " in MainForm.PropagateSelectorsForCurrentChart()";
				#if DEBUG
				//Debugger.Break();
				#endif
				Assembler.PopupException(msg + msig);
				context.ChartStreaming = false;
			}
			
			if (context.ScaleInterval.Scale == BarScale.Unknown) {
				if (dataSource.ScaleInterval.Scale == BarScale.Unknown) {
					string msg1 = "SCALE_INTERVAL_UNKNOWN_BOTH_CONTEXT_DATASOURCE WILL_NOT_INITIALIZE Executor.Init(Strategy->BarsLoaded)";
					throw new Exception(msg1 + msig);
				}
				context.ScaleInterval = dataSource.ScaleInterval;
				string msg2 = "CONTEXT_SCALE_INTERVAL_UNKNOWN_FIXED_TO_DATASOURCE contextToPopulate.ScaleInterval[" + context.ScaleInterval + "]";
				Assembler.PopupException(msg2 + msig);
			}

			bool wontBacktest = skipBacktest || (this.Strategy != null && this.Strategy.ScriptContextCurrent.BacktestOnSelectorsChange == false);
			bool willBacktest = !wontBacktest;
			if (loadNewBars) {
				Bars barsAll = dataSource.BarsLoadAndCompress(symbol, context.ScaleInterval);
				if (barsAll.Count > 0) {
					this.ChartForm.ChartControl.RangeBar.Initialize(barsAll, barsAll);
				}

				if (context.DataRange == null) context.DataRange = new BarDataRange();
				Bars barsClicked = barsAll.SelectRange(context.DataRange);
				
				if (this.Executor.Bars != null) {
					this.Executor.Bars.DataSource.DataSourceEditedChartsDisplayedShouldRunBacktestAgain -=
						new EventHandler<DataSourceEventArgs>(ChartFormManager_DataSourceEditedChartsDisplayedShouldRunBacktestAgain);
				}
				this.Executor.SetBars(barsClicked);
				this.Executor.Bars.DataSource.DataSourceEditedChartsDisplayedShouldRunBacktestAgain +=
						new EventHandler<DataSourceEventArgs>(ChartFormManager_DataSourceEditedChartsDisplayedShouldRunBacktestAgain);

				bool invalidateAllPanels = wontBacktest;
				this.ChartForm.ChartControl.Initialize(barsClicked, invalidateAllPanels);
				//SCROLL_TO_SNAPSHOTTED_BAR this.ChartForm.ChartControl.ScrollToLastBarRight();
				this.ChartForm.PopulateBtnStreamingClickedAndText();
			}

			// set original Streaming Icon before we lost in simulationPreBarsSubstitute() and launched backtester in another thread
			//V1 this.Executor.Performance.Initialize();
			//V2_REPORTERS_NOT_REFRESHED this.Executor.BacktesterRunSimulation();
			var iconCanBeNull = this.Executor.DataSource.StreamingProvider != null ? this.Executor.DataSource.StreamingProvider.Icon : null;
			this.ChartForm.PropagateContextChartOrScriptToLTB(context, iconCanBeNull);

			// v1 already in ChartRenderer.OnNewBarsInjected event - commented out DoInvalidate();
			// v2 NOPE, during DataSourcesTree_OnSymbolSelected() we're invalidating it here! - uncommented back
			//this.ChartForm.ChartControl.InvalidateAllPanelsFolding();	// WHEN_I_CHANGE_SMA_PERIOD_I_DONT_WANT_TO_SEE_CLEAR_CHART_BUT_REPAINTED_WITHOUT_2SEC_BLINK
			
			if (this.Strategy == null) {
				this.DataSnapshotSerializer.Serialize();
				return;
			}

			//bool wontBacktest = skipBacktest || this.Strategy.ScriptContextCurrent.BacktestOnSelectorsChange == false;
			if (willBacktest == false) {
				this.Executor.ChartShadow.ClearAllScriptObjectsBeforeBacktest();
				return;
			}
			if (this.Strategy.Script == null) {
				// WRONG_PLACE_TO_FIX "EnterEveryBar doesn't draw MAfast" this.StrategyCompileActivatePopulateSlidersShow();
				string msg = "legitimate ways to get here:"
					+ " 1) after compilation failed at Editor-F5;"
					+ " 2) an existing strategy.BacktestOnSelectorsChange=true (opened in a new chartform) failed compile upstack;"
					+ " 3) an exisitng strategy.BacktestOnSelectorsChange=true (loaded non-default ScriptContext) failed to compile upstack;"
					;
				Assembler.PopupException(msg);
				return;
			}
			this.BacktesterRunSimulationRegular();
		}
		void ChartFormManager_DataSourceEditedChartsDisplayedShouldRunBacktestAgain(object sender, DataSourceEventArgs e) {
			if (this.Strategy == null) return;
			if (this.Strategy.ScriptContextCurrent.BacktestOnDataSourceSaved == false) return;
			this.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(
				"ChartFormManager_DataSourceEditedChartsDisplayedShouldRunBacktestAgain", true, false);
		}
		public void BacktesterRunSimulationRegular() {
			try {
				this.Executor.ChartShadow.BacktestIsRunning.Set();	//WONT_BE_RESET_IF_EXCEPTION_OCCURS_BEFORE_TASK_LAUNCH
				if (this.Executor.Strategy.ActivatedFromDll == false) {
					// ONLY_TO_MAKE_CHARTFORM_BACKTEST_NOW_WORK__FOR_F5_ITS_A_DUPLICATE__LAZY_TO_ENMESS_CHART_FORM_MANAGER_WITH_SCRIPT_EDITOR_FUNCTIONALITY
					this.StrategyCompileActivatePopulateSlidersShow();
				}
				this.Executor.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), true);
			} catch (Exception ex) {
				string msg = "RUN_SIMULATION_TRAMPOLINE_FAILED for Strategy[" + this.Strategy + "] on Bars[" + this.Executor.Bars + "]";
				Assembler.PopupException(msg, ex);
			}
		}
		void afterBacktesterComplete(ScriptExecutor myOwnExecutorIgnoring) {
			if (this.Executor.Bars == null) {
				string msg = "DONT_RUN_BACKTEST_BEFORE_BARS_ARE_LOADED";
				Assembler.PopupException(msg);
				return;
			}
			if (this.ChartForm.InvokeRequired) {
				this.ChartForm.BeginInvoke((MethodInvoker)delegate { this.afterBacktesterComplete(myOwnExecutorIgnoring); });
				return;
			}
			//this.clonePositionsForChartPickupBacktest(this.Executor.ExecutionDataSnapshot.PositionsMaster);
			this.ChartForm.ChartControl.PositionsBacktestAdd(this.Executor.ExecutionDataSnapshot.PositionsMaster);
			this.ChartForm.ChartControl.PendingHistoryBacktestAdd(this.Executor.ExecutionDataSnapshot.AlertsPendingHistorySafeCopy);
			this.ChartForm.ChartControl.InvalidateAllPanels();
			
			//this.Executor.Performance.BuildStatsOnBacktestFinished(this.Executor.ExecutionDataSnapshot.PositionsMaster);
			// MOVED_TO_BacktesterRunSimulation() this.Executor.Performance.BuildStatsOnBacktestFinished();
			this.ReportersFormsManager.BuildOnceAllReports(this.Executor.Performance);
		}
		void afterBacktesterCompleteOnceOnRestart(ScriptExecutor myOwnExecutorIgnoring) {
			this.afterBacktesterComplete(myOwnExecutorIgnoring);
			
			if (this.Strategy == null) {
				Assembler.PopupException("this should never happen this.Strategy=null in afterBacktesterCompleteOnceOnRestart()");
				return;
			}
			//ONLY_ON_WORKSPACE_RESTORE??? this.ChartForm.PropagateContextChartOrScriptToLTB(this.Strategy.ScriptContextCurrent);
			if (this.Strategy.ScriptContextCurrent.ChartStreaming) this.ChartStreamingConsumer.StartStreaming();
		}
		public void InitializeChartNoStrategyAfterDeserialization(MainForm mainForm) {
			this.InitializeChartNoStrategy(mainForm, null);
		}
		public void InitializeStrategyAfterDeserialization(MainForm mainForm, string strategyGuid, string strategyName = "PLEASE_SUPPLY_FOR_USERS_CONVENIENCE") {
			this.StrategyFoundDuringDeserialization = false;
			Strategy strategyFound = null;
			if (String.IsNullOrEmpty(strategyGuid) == false) {
				strategyFound = Assembler.InstanceInitialized.RepositoryDllJsonStrategy.LookupByGuid(strategyGuid); 	// can return NULL here
			}
			if (strategyFound == null) {
				string msg = "STRATEGY_NOT_FOUND: RepositoryDllJsonStrategy.LookupByGuid(strategyGuid=" + strategyGuid + ")";
				#if DEBUG
				Debugger.Break();
				#endif
				Assembler.PopupException(msg);
				return;
			}
			this.InitializeWithStrategy(mainForm, strategyFound, true);
			this.StrategyFoundDuringDeserialization = true;
			if (this.Executor.Bars == null) {
				string msg = "TYRINIG_AVOID_BARS_NULL_EXCEPTION: FIXME InitializeWithStrategy() didn't load bars";
				Assembler.PopupException(msg);
				#if DEBUG
				Debugger.Break();
				#endif
				return;
			}
			if (this.Strategy.ScriptContextCurrent.BacktestOnRestart == false) {
				// COPYFROM_StrategyCompileActivatePopulateSlidersShow()
				if (this.Strategy.Script != null && this.Strategy.ActivatedFromDll) {
					this.Strategy.Script.PullParametersFromCurrentContextSaveStrategyIfAbsorbedFromScript();
				}
				return;
			}

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
				//IM_GETTING_HERE_ON_STARTUP_AFTER_SUCCESFULL_COMPILATION_CHART_RELATED_STRATEGIES Debugger.Break();
				string msg = "you should never get here; a compiled script should've been already linked to Executor (without bars on deserialization) 10 lines above in this.InitializeWithStrategy(mainForm, strategy);";
				#if DEBUG
				if (this.Strategy.ActivatedFromDll == true) {
					Debugger.Break();
				}
				#endif
				this.Strategy.Script.Initialize(this.Executor);
			}

			//FIX_FOR: TOO_SMART_INCOMPATIBLE_WITH_LIFE_SPENT_4_HOURS_DEBUGGING DESERIALIZED_STRATEGY_HAD_PARAMETERS_NOT_INITIALIZED INITIALIZED_BY_SLIDERS_AUTO_GROW_CONTROL
			string msg2 = "DONT_UNCOMMENT_ITS_LIKE_METHOD_BUT_USED_IN_SLIDERS_AUTO_GROW_CONTROL_4_HOURS_DEBUGGING";
			// MOVED_TO_StrategyCompileActivatePopulateSlidersShow() this.Strategy.Script.PullCurrentContextParametersFromStrategyTwoWayMergeSaveStrategy();

			// MOVED_TO_StrategyCompileActivatePopulateSlidersShow(), we definitely will be running it later due to BacktestOnRestart.true tested 20 lines above this.Strategy.Script.IndicatorsInitializeMergeParamsfromJsonStoreInSnapshot();
			
			this.Executor.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterCompleteOnceOnRestart), true);
			//NOPE_ALREADY_POPULATED_UPSTACK this.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsBacktestIfStrategy("InitializeStrategyAfterDeserialization()");

			//NOT_SURE
			Debugger.Break();
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete) {
				this.OptimizerFormShow(false);
			}
		}
		public void ReportersDumpCurrentForSerialization() {
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) {
				#if DEBUG
				//Debugger.Break();
				#endif
				return;
			}
			if (this.Strategy == null) return;
			this.Strategy.ScriptContextCurrent.ReporterShortNamesUserInvokedJSONcheck =
				new List<string>(this.ReportersFormsManager.ReporterShortNamesUserInvoked.Keys);
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

			DockPanel mainPanelOrAnotherEditorsPanel = this.dockPanel;
			ScriptEditorForm anotherEditor = null;
			foreach (DockContent form in this.dockPanel.Contents) {
				anotherEditor = form as ScriptEditorForm;
				if (anotherEditor == null) continue;
				if (anotherEditor.Pane == null) continue;
				mainPanelOrAnotherEditorsPanel = anotherEditor.Pane.DockPanel;
				break;
			}

			this.ScriptEditorFormConditionalInstance.Show(mainPanelOrAnotherEditorsPanel);
			this.ScriptEditorFormConditionalInstance.ActivateDockContentPopupAutoHidden(keepAutoHidden, true);
			//this.ScriptEditorFormConditionalInstance.Show(this.dockPanel, DockState.Document);
			//useless: will be re-calculated in ctxStrategy_Opening(); this.ChartForm.MniShowSourceCodeEditor.Checked = this.ScriptEditorIsOnSurface;
		}
		public void OptimizerFormShow(bool keepAutoHidden = true) {
			this.OptimizerFormConditionalInstance.Initialize(this);

			DockPanel mainPanelOrAnotherOptimizersPanel = this.dockPanel;
			OptimizerForm anotherOptimizer = null;
			foreach (DockContent form in this.dockPanel.Contents) {
				anotherOptimizer = form as OptimizerForm;
				if (anotherOptimizer == null) continue;
				if (anotherOptimizer.Pane == null) continue;
				mainPanelOrAnotherOptimizersPanel = anotherOptimizer.Pane.DockPanel;
				break;
			}
			this.OptimizerFormConditionalInstance.Show(mainPanelOrAnotherOptimizersPanel);
			this.OptimizerFormConditionalInstance.ActivateDockContentPopupAutoHidden(keepAutoHidden, true);
			this.OptimizerFormConditionalInstance.OptimizerControl.Refresh();	// olvBacktest doens't repaint while having results?...
		}
		
		const string prefixWhenNeedsToBeSaved = "* ";
		internal void PopulateWindowTitlesFromChartContextOrStrategy() {
			if (this.Strategy == null) {
				//string msg = "ChartFormsManager doesn't have a pointer to Strategy; Opening a Chart without Strategy is NYI";
				//throw new Exception(msg);
				if (this.DataSnapshot.ContextChart == null) {
					string msg = "CHART_WITHOUT_STRATEGY_MUST_HAVE_DataSnapshot.ContextChart; Sq1.Gui.Layout.xml and ChartFormDataSnapshot-*.json aren't in sync";
					this.ChartForm.Text = msg;
					Assembler.PopupException(msg);
					return;
				}
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
		public void StrategyCompileActivateBeforeShow() {
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
				if (DockContentImproved.IsNullOrDisposed(this.ScriptEditorForm) == false) {
					this.ScriptEditorFormConditionalInstance.ScriptEditorControl.PopulateCompilerErrors(this.Strategy.ScriptCompiler.CompilerErrors);
				}
			} else {
				if (DockContentImproved.IsNullOrDisposed(this.ScriptEditorForm) == false) {
					this.ScriptEditorFormConditionalInstance.ScriptEditorControl.PopulateCompilerSuccess();
				}
				this.Strategy.Script.Initialize(this.Executor);
                this.Executor.Optimizer.RaiseScriptRecompiledUpdateHeaderPostponeColumnsRebuild();
			}
			// moved to StrategyCompileActivatePopulateSlidersShow() because no need to PopulateSliders during Deserialization
			//SlidersForm.Instance.Initialize(this.Strategy);
			this.Executor.ChartShadow.HostPanelForIndicatorClear();		//non-DLL-strategy multiple F5s add PanelIndicator endlessly
			this.Executor.Optimizer.Initialize();						// removes "optimizerInitializedProperly == false" on app restart => Optimizer fills up with Script&Indicator Prarmeters for a JSON-based strategy
		}
		public void StrategyCompileActivatePopulateSlidersShow() {
			if (this.Strategy.ActivatedFromDll == false) this.StrategyCompileActivateBeforeShow();
			else Debugger.Break();

			if (this.Strategy.Script != null) {		// NULL if after restart the JSON Strategy.SourceCode was left with compilation errors/wont compile with MY_VERSION
				this.Strategy.Script.IndicatorsInitializeAbsorbParamsFromJsonStoreInSnapshot();
				this.Strategy.Script.PullParametersFromCurrentContextSaveStrategyIfAbsorbedFromScript();
			}
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			this.PopulateSliders();
		}
		public void PopulateSliders() {
			//CAN_HANDLE_NULL_IN_SlidersForm.Instance.Initialize()  if (this.Strategy == null) return;
			SlidersForm.Instance.Initialize(this.Strategy);
			if (SlidersForm.Instance.Visible == false) {		// don't activate the tab if user has docked another Form on top of SlidersForm
				SlidersForm.Instance.Show(this.dockPanel);
			}
		}
		public override string ToString() {
			return "Strategy[" + this.Strategy.Name + "], Chart [" + this.ChartForm.ToString() + "]";
		}
		public void PopulateMainFormSymbolStrategyTreesScriptParameters() {
			ContextChart ctxScript = this.ContextCurrentChartOrStrategy;
			if (ctxScript == null) {
				#if DEBUG
				Debugger.Break();
				#endif
				return;
			}
			DataSourcesForm.Instance.DataSourcesTreeControl.SelectSymbol(ctxScript.DataSourceName, ctxScript.Symbol);
			if (this.Strategy != null) {
				StrategiesForm.Instance.StrategiesTreeControl.SelectStrategy(this.Strategy);
			} else {
				StrategiesForm.Instance.StrategiesTreeControl.UnSelectStrategy();
			}
			this.PopulateSliders();
		}

		
		public void OptimizerFormIfOpenPropagateTextboxesOrMarkStaleResults() {
			if (this.OptimizerForm == null) {
				string msg = "ADDED_CONDITION_UPSTACK_TO_AVOID JUST_WANNA_KNOW_IF_I_EVER_CHECK_FOR_STALE_BEFORE_FORM_IS_CREATED";
				#if DEBUG
				//Assembler.PopupException(msg);
				#endif
				return;
			}
			
			string staleReason = null;
			staleReason = this.OptimizerFormConditionalInstance.OptimizerControl.PopulateTextboxesFromExecutorsState();
			
			bool clearFirstBeforeClickingAnotherSymbolScaleIntervalRangePositionSize = string.IsNullOrEmpty(staleReason) == false;
			if (clearFirstBeforeClickingAnotherSymbolScaleIntervalRangePositionSize == false) {
				int a = 1;
				//return;
			}
			this.OptimizerFormConditionalInstance.OptimizerControl
				.NormalizeBackgroundOrMarkIfBacktestResultsAreForDifferentSymbolScaleIntervalRangePositionSize();
		}
	}
}
