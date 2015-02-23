using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
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
						this.scriptEditorFormFactory = new ScriptEditorFormFactory(this);
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
						this.optimizerFormFactory = new OptimizerFormFactory(this);
					}

					this.optimizerFormFactory.CreateOptimizerFormSubscribePushToManager(this);
					if (this.OptimizerForm == null) {
						throw new Exception("OptimizerFormFactory.CreateAndSubscribe() failed to create OptimizerForm in ChartFormsManager");
					}
				}
				return this.OptimizerForm;
			} }
		//LivesimFormFactory livesimFormFactory;
		public LivesimForm LivesimForm;
		public LivesimForm LivesimFormConditionalInstance { get {
				if (DockContentImproved.IsNullOrDisposed(this.LivesimForm)) {
					if (this.Strategy == null) return null;
					//v1 before I got rid of LivesimFormFactory - seemed useless, unlike ScriptEditorFormFactory which is useful enough to keep DigitalRune's library unmodified 
					//if (this.livesimFormFactory == null) {
					//	#if DEBUG
					//	Debugger.Break();
					//	#endif
					//	this.livesimFormFactory = new LivesimFormFactory(this);
					//}
					//this.livesimFormFactory.CreateLivesimFormSubscribePushToManager(this);
					//if (this.LivesimForm == null) {
					//	throw new Exception("LivesimFormFactory.CreateAndSubscribe() failed to create LivesimForm in ChartFormsManager");
					//}
					//v2
					this.LivesimForm = new LivesimForm(this);
				}
				return this.LivesimForm;
			} }
		public bool OptimizerFormIsNotDisposed { get { return (DockContentImproved.IsNullOrDisposed(this.OptimizerForm) == false); } }
		public bool OptimizerIsOnSurface { get {
				OptimizerForm optimizer = this.OptimizerForm;
				bool optimizerNotInstantiated = DockContentImproved.IsNullOrDisposed(optimizer);
				bool optimizerMustBeActivated = optimizerNotInstantiated ? true : optimizer.MustBeActivated;
				return !optimizerMustBeActivated;
			} }
		
		public ChartFormInterformEventsConsumer InterformEventsConsumer;
		public bool ScriptEditedNeedsSaving;
		public ChartFormStreamingConsumer ChartStreamingConsumer;
		
		public Dictionary<string, DockContentImproved> FormsAllRelated { get {
				var ret = new Dictionary<string, DockContentImproved>();
				if (this.ChartForm			!= null) ret.Add("Chart",		this.ChartForm);
				if (this.ScriptEditorForm	!= null) ret.Add("Source Code",	this.ScriptEditorForm);
				if (this.OptimizerForm		!= null) ret.Add("Optimizer",	this.OptimizerForm);
				if (this.LivesimForm		!= null) ret.Add("LiveSim",		this.LivesimForm);
				foreach (string textForMenuItem in this.ReportersFormsManager.FormsAllRelated.Keys) {
					ret.Add(textForMenuItem, this.ReportersFormsManager.FormsAllRelated[textForMenuItem]);
				}
				return ret;
			} }
		public string StreamingButtonIdent { get {
				if (this.ContextCurrentChartOrStrategy == null) {
					string msg = "StreamingButtonIdent_UNKNOWN__INVOKED_TOO_EARLY_ContextChart_WASNT_INITIALIZED_YET";
					//Assembler.PopupException(msg);
					return msg;
				}
				
				string emptyChartOrStrategy = this.ContextCurrentChartOrStrategy.Symbol;
				string onOff = this.ContextCurrentChartOrStrategy.IsStreaming ? " On" : " Off";

				if (this.Strategy != null) {
					emptyChartOrStrategy = this.Strategy.Name;
					onOff = this.Executor.IsStreamingTriggeringScript ? " On" : " Off";
				}

				return emptyChartOrStrategy + onOff;
			} }
		public ContextChart ContextCurrentChartOrStrategy { get { return (this.Strategy != null) ? this.Strategy.ScriptContextCurrent as ContextChart : this.DataSnapshot.ContextChart; } }

		private ChartFormManager() {
			this.StrategyFoundDuringDeserialization = false;
			// deserialization: ChartSerno will be restored; never use this constructor in your app!
			this.ScriptEditedNeedsSaving = false;
			//			this.Executor = new ScriptExecutor(Assembler.InstanceInitialized.ScriptExecutorConfig
			//				, this.ChartForm.ChartControl, null, Assembler.InstanceInitialized.OrderProcessor, Assembler.InstanceInitialized.StatusReporter);
			this.Executor = new ScriptExecutor();
			this.ReportersFormsManager = new ReportersFormsManager(this);
			this.ChartStreamingConsumer = new ChartFormStreamingConsumer(this);

			// never used in CHART_ONLY, but we have "Open In Current Chart" for Strategies
			this.scriptEditorFormFactory = new ScriptEditorFormFactory(this);
			this.optimizerFormFactory = new OptimizerFormFactory(this);
			//this.livesimFormFactory = new LivesimFormFactory(this);

			this.DataSnapshotSerializer = new Serializer<ChartFormDataSnapshot>();
		}
		public ChartFormManager(MainForm mainForm, int charSernoDeserialized = -1) : this() {
			this.MainForm = mainForm;
			if (charSernoDeserialized == -1) {
				this.DataSnapshot = new ChartFormDataSnapshot();
				return;
			}
			bool createdNewFile = this.DataSnapshotSerializer.Initialize(Assembler.InstanceInitialized.AppDataPath,
				"ChartFormDataSnapshot-" + charSernoDeserialized + ".json", "Workspaces",
				Assembler.InstanceInitialized.AssemblerDataSnapshot.CurrentWorkspaceName, true, true);
			this.DataSnapshot = this.DataSnapshotSerializer.Deserialize();
			if (this.DataSnapshot == null) {
				string msg = "I_REFUSE_CTOR_SERIALIZATION AVOIDING_NPE this.DataSnapshot[" + this.DataSnapshot + "]=null";
				Assembler.PopupException(msg);
				return;
			}
			this.DataSnapshot.ChartSerno = charSernoDeserialized;
			this.DataSnapshotSerializer.Serialize();
		}
		ChartForm chartFormFactory() {
			ChartForm ret = new ChartForm(this);
			
			// sequence of invocation matters otherwise "Delegate to an instance method cannot have null 'this'."
			// at Sq1.Gui.Forms.ChartForm.ChartFormEventsToChartFormManagerDetach() in C:\SquareOne\Sq1.Gui\Forms\ChartForm.cs:line 61
			this.InterformEventsConsumer = new ChartFormInterformEventsConsumer(this, ret);
			// MAKES_SENSE_IF_this.InterformEventsConsumer_WAS_INITIALIZED_IN_CTOR() this.ChartForm.ChartFormEventsToChartFormManagerDetach();
			ret.ChartFormEventsToChartFormManagerAttach();
			
			// DONT_MOVE_OUTSIDE_CHART_FORM_FACTORY you open a chartNoStrategy, then load a strategy, then another one => you get 3 invocations of ChartForm_FormClosed()  
			ret.FormClosed += this.MainForm.MainFormEventManager.ChartForm_FormClosed;
			ret.AppendReportersMenuItems(this.ReportersFormsManager.MenuItemsProvider.MenuItems.ToArray());
			return ret;
		}
		public void InitializeChartNoStrategy(ContextChart contextChart) {
			string msig = " //ChartFormsManager[" + this.ToString() + "].InitializeChartNoStrategy(" + contextChart + ")";

			if (this.DataSnapshot.ChartSerno == -1) {
				int charSernoNext = this.MainForm.GuiDataSnapshot.ChartSernoNextAvailable;
				bool createdNewFile = this.DataSnapshotSerializer.Initialize(Assembler.InstanceInitialized.AppDataPath,
					"ChartFormDataSnapshot-" + charSernoNext + ".json", "Workspaces",
					Assembler.InstanceInitialized.AssemblerDataSnapshot.CurrentWorkspaceName, true, true);
				this.DataSnapshot = this.DataSnapshotSerializer.Deserialize();	// will CREATE a new ChartFormDataSnapshot and keep the reference for further Serialize(); we should fill THIS object
				this.DataSnapshot.ChartSerno = charSernoNext;
				this.DataSnapshotSerializer.Serialize();
			}
			
			this.DataSnapshot.StrategyGuidJsonCheck		= "NO_STRATEGY_CHART_ONLY";
			this.DataSnapshot.StrategyNameJsonCheck		= "NO_STRATEGY_CHART_ONLY";
			this.DataSnapshot.StrategyAbsPathJsonCheck	= "NO_STRATEGY_CHART_ONLY";

			if (this.ChartForm == null) this.ChartForm = this.chartFormFactory();
			#region TODO move to chartFormFactory()
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
			#endregion
			this.ChartForm.Initialize(false);

			try {
				this.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(msig, true, true, false);
				//v1 if (this.DataSnapshot.ContextChart.IsStreaming) {
				//v2 universal for both InitializeWithStrategy() and InitializeChartNoStrategy()
				ContextChart ctx = this.ContextCurrentChartOrStrategy;
				if (ctx.IsStreaming) {
					string reason = "contextChart[" + ctx.ToString() + "].IsStreaming=true";
					this.ChartStreamingConsumer.StreamingSubscribe(reason);
				}
			} catch (Exception ex) {
				string msg = "PopulateCurrentChartOrScriptContext(): ";
				Assembler.PopupException(msg + msig, ex);
			}
		}
		public void InitializeWithStrategy(Strategy strategy, bool skipBacktestDuringDeserialization = true) {
			string msig = " //ChartFormsManager[" + this.ToString() + "].InitializeWithStrategy(" + strategy.ToString() + ")";
			this.Strategy = strategy;
			//this.Executor = new ScriptExecutor(mainForm.Assembler, this.Strategy);

			if (this.DataSnapshot.ChartSerno == -1) {
				int charSernoNext = this.MainForm.GuiDataSnapshot.ChartSernoNextAvailable;
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
			
			if (this.ChartForm == null) this.ChartForm = this.chartFormFactory();
			#region TODO merge this region from InitializeWithStrategy() and InitializeChartNoStrategy() into chartFormFactory()
			if (this.DataSnapshot.ChartSettings == null) {
				string msg = "SORRY_WHEN_EXACTLY_THIS_MAKES_SENSE?...";
				this.DataSnapshot.ChartSettings = this.ChartForm.ChartControl.ChartSettings;	// opening from Datasource => save
			} else {
				if (this.ChartForm.ChartControl.ChartSettings != this.DataSnapshot.ChartSettings) {
					string msg = "IM_HERE_AT_InitializeStrategyAfterDeserialization,...";
					this.ChartForm.ChartControl.ChartSettings = this.DataSnapshot.ChartSettings;	// otherwize JustDeserialized => propagate
				} else {
					string msg = "IM_LOADING_ANOTHER_STRATEGY_INTO_EXISTING_CHART";
				}
			}
			this.ChartForm.ChartControl.PropagateSplitterManorderDistanceIfFullyDeserialized();
			#endregion
			this.ChartForm.Initialize(true, this.Strategy.ActivatedFromDll);

			this.Executor.Initialize(this.ChartForm.ChartControl, this.Strategy);

			try {
				// Click on strategy should open new chart,  
				if (this.Strategy.ScriptContextCurrent.BacktestOnSelectorsChange == true && this.Strategy.ActivatedFromDll == false && this.Strategy.Script == null) {
					this.StrategyCompileActivatePopulateSlidersShow();
				}
				//I'm here via Persist.Deserialize() (=> Reporters haven't been restored yet => backtest should be postponed); will backtest in InitializeStrategyAfterDeserialization
				// STRATEGY_CLICK_TO_CHART_DOESNT_BACKTEST this.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(msig, true, true);
				// ALL_SORT_OF_STARTUP_ERRORS this.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(msig, true, false);
				this.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(msig, true, skipBacktestDuringDeserialization, false);
				if (skipBacktestDuringDeserialization == false) {
					this.OptimizerFormIfOpenPropagateTextboxesOrMarkStaleResults();
				}
				//v1 if (this.Strategy.ScriptContextCurrent.IsStreaming) {
				//v2 universal for both InitializeWithStrategy() and InitializeChartNoStrategy()
				ContextChart ctx = this.ContextCurrentChartOrStrategy;
				if (ctx.IsStreaming) {
					string reason = "contextChart[" + ctx.ToString() + "].IsStreaming=true;"
						+ " OnApprestartBacktest will launch in another thread and I can't postpone subscription until it finishes"
						+ " so the Pump should set paused now because UpstreamSubscribe should not invoke ChartFormStreamingConsumer"
						+ " whenever StreamingAdapter is ready, but only after all ScaleSymbol consuming backtesters are complete";
					this.ChartStreamingConsumer.StreamingSubscribe(reason);
				}
			} catch (Exception ex) {
				string msg = "PopulateCurrentChartOrScriptContext(): ";
				Assembler.PopupException(msg + msig, ex);
			}
		}
		public void PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(string msig, bool loadNewBars = true, bool skipBacktest = false, bool saveStrategyRequired = true) {
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
			
			//v1 COMMENTED_OUT ABORTED_UNFINISHED_BACKTEST should handle itself its own abortion;
			//if (context.ChartStreaming) {
			//	string msg = "CHART_STREAMING_FORCIBLY_DISABLED_SAVED POSSIBLY_ENABLED_BY_OPENCHART_OR_NOT_ABORTED_UNFINISHED_BACKTEST ContextCurrentChartOrStrategy.ChartStreaming=true"
			//		+ "; Selectors should've been Disable()d on chat[" + this.ChartForm + "].Activated() or StreamingOn()"
			//		+ " in MainForm.PropagateSelectorsForCurrentChart()";
			//	Assembler.PopupException(msg + msig);
			//	context.ChartStreaming = false;
			//	saveStrategyRequired = true;
			//}
			
			//v2: I closed opened the app while streaming from StreamingMock and I want it streaming after app restart!!!
			if (context.IsStreaming && context.IsStreamingTriggeringScript && saveStrategyRequired == true) {	// saveStrategyRequired=true for all user-clicked GUI events
				string msg = "NOT_AN_ERROR__I_REFUSE_TO_STOP_STREAMING__DISABLE_GUI_CONTROLS_THAT_TRIGGER_RELOADING_BARS";
				Assembler.PopupException(msg + msig, null, false);
			}
			
			if (context.ScaleInterval.Scale == BarScale.Unknown) {
				if (dataSource.ScaleInterval.Scale == BarScale.Unknown) {
					string msg1 = "EDIT_DATASOURCE_SET_SCALE_INTERVAL SCALE_INTERVAL_UNKNOWN_BOTH_CONTEXT_DATASOURCE WILL_NOT_INITIALIZE Executor.Init(Strategy->BarsLoaded)";
					throw new Exception(msg1 + msig);
				}
				context.ScaleInterval = dataSource.ScaleInterval;
				string msg2 = "CONTEXT_SCALE_INTERVAL_UNKNOWN__RESET_TO_DATASOURCE'S contextToPopulate.ScaleInterval[" + context.ScaleInterval + "]";
				Assembler.PopupException(msg2 + msig, null, false);
				saveStrategyRequired = true;	// don't move this write before you read above
			}

			bool wontBacktest = skipBacktest || (this.Strategy != null && this.Strategy.ScriptContextCurrent.BacktestOnSelectorsChange == false);
			bool willBacktest = !wontBacktest;
			if (loadNewBars) {
				string millisElapsedLoadCompress;
				Bars barsAll = dataSource.BarsLoadAndCompress(symbol, context.ScaleInterval, out millisElapsedLoadCompress);
				string stats = millisElapsedLoadCompress + " //dataSource[" + dataSource.ToString()
					+ "].BarsLoadAndCompress(" + symbol + ", " + context.ScaleInterval + ") ";
				//Assembler.PopupException(stats, null, false);

				if (barsAll.Count > 0) {
					this.ChartForm.ChartControl.RangeBar.Initialize(barsAll, barsAll);
				}

				if (context.DataRange == null) context.DataRange = new BarDataRange();
				Bars barsClicked = barsAll.SelectRange(context.DataRange);
				
				if (this.Executor.Bars != null) {
					this.Executor.Bars.DataSource.DataSourceEditedChartsDisplayedShouldRunBacktestAgain -=
						new EventHandler<DataSourceEventArgs>(chartFormManager_DataSourceEditedChartsDisplayedShouldRunBacktestAgain);
				}
				this.Executor.SetBars(barsClicked);
				this.Executor.Bars.DataSource.DataSourceEditedChartsDisplayedShouldRunBacktestAgain +=
						new EventHandler<DataSourceEventArgs>(chartFormManager_DataSourceEditedChartsDisplayedShouldRunBacktestAgain);

				bool invalidateAllPanels = wontBacktest;
				this.ChartForm.ChartControl.Initialize(barsClicked, invalidateAllPanels);
				//SCROLL_TO_SNAPSHOTTED_BAR this.ChartForm.ChartControl.ScrollToLastBarRight();
				this.ChartForm.PopulateBtnStreamingTriggersScriptAfterBarsLoaded();
			}

			// set original Streaming Icon before we lost in simulationPreBarsSubstitute() and launched backtester in another thread
			//V1 this.Executor.Performance.Initialize();
			//V2_REPORTERS_NOT_REFRESHED this.Executor.BacktesterRunSimulation();
			//var iconCanBeNull = this.Executor.DataSource.StreamingAdapter != null ? this.Executor.DataSource.StreamingAdapter.Icon : null;
			this.ChartForm.AbsorbContextBarsToGui();

			// v1 already in ChartRenderer.OnNewBarsInjected event - commented out DoInvalidate();
			// v2 NOPE, during DataSourcesTree_OnSymbolSelected() we're invalidating it here! - uncommented back
			//this.ChartForm.ChartControl.InvalidateAllPanelsFolding();	// WHEN_I_CHANGE_SMA_PERIOD_I_DONT_WANT_TO_SEE_CLEAR_CHART_BUT_REPAINTED_WITHOUT_2SEC_BLINK
			
			if (this.Strategy == null) {
				this.DataSnapshotSerializer.Serialize();
				return;
			}

			if (saveStrategyRequired) {
				// StrategySave is here koz I'm invoked for ~10 user-click GUI events; only Deserialization shouldn't save anything
				Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.Strategy);
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
			this.BacktesterRunSimulation();
		}
		void chartFormManager_DataSourceEditedChartsDisplayedShouldRunBacktestAgain(object sender, DataSourceEventArgs e) {
			if (this.Strategy == null) {
				//OUTDATED Assembler.PopupException("hey for ATRbandCompiled-DLL #D Debugger shows this=NullReferenceException, and Strategy=null while it was instantiated from DLL, right?....");
				return;
			}
			// Save datasource must fully unregister consumers and register again to avoid StreamingSolidifier dupes
			// ConsumerBarUnRegister()
			if (this.Strategy.ScriptContextCurrent.BacktestOnDataSourceSaved == false) return;
			this.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(
				"ChartFormManager_DataSourceEditedChartsDisplayedShouldRunBacktestAgain", true, false);
		}
		public void BacktesterRunSimulation(bool subscribeUpstreamOnWorkspaceRestore = false) {
			try {
				//AVOIDING_"Collection was modified; enumeration operation may not execute."_BY_SETTING___PaintAllowed=false
				//WILL_CALL_LATER_IN_Backtester.SimulationPreBarsSubstitute() this.Executor.ChartShadow.PaintAllowedDuringLivesimOrAfterBacktestFinished = false;	//WONT_BE_SET_IF_EXCEPTION_OCCURS_BEFORE_TASK_LAUNCH
				if (this.Executor.Strategy.ActivatedFromDll == false) {
					// ONLY_TO_MAKE_CHARTFORM_BACKTEST_NOW_WORK__FOR_F5_ITS_A_DUPLICATE__LAZY_TO_ENMESS_CHART_FORM_MANAGER_WITH_SCRIPT_EDITOR_FUNCTIONALITY
					this.StrategyCompileActivatePopulateSlidersShow();
				}
				if (subscribeUpstreamOnWorkspaceRestore == true) {
					this.Executor.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterCompleteOnceOnWorkspaceRestore), true);
				} else {
					this.Executor.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), true);
				}
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
			this.ChartForm.ChartControl.PositionsBacktestAdd(this.Executor.ExecutionDataSnapshot.PositionsMaster.InnerList);
			//this.ChartForm.ChartControl.PendingHistoryBacktestAdd(this.Executor.ExecutionDataSnapshot.AlertsPendingHistorySafeCopy);
			this.ChartForm.ChartControl.PendingHistoryBacktestAdd(this.Executor.ExecutionDataSnapshot.AlertsPending.ByBarPlaced);
			this.ChartForm.ChartControl.InvalidateAllPanels();
			
			//this.Executor.Performance.BuildStatsOnBacktestFinished(this.Executor.ExecutionDataSnapshot.PositionsMaster);
			// MOVED_TO_BacktesterRunSimulation() this.Executor.Performance.BuildStatsOnBacktestFinished();
			this.ReportersFormsManager.BuildReportFullOnBacktestFinishedAllReporters(this.Executor.Performance);
		}
		void afterBacktesterCompleteOnceOnWorkspaceRestore(ScriptExecutor myOwnExecutorIgnoring) {
			this.afterBacktesterComplete(myOwnExecutorIgnoring);
			
			if (this.Strategy == null) {
				Assembler.PopupException("this should never happen this.Strategy=null in afterBacktesterCompleteOnceOnRestart()");
				return;
			}
			//ONLY_ON_WORKSPACE_RESTORE??? this.ChartForm.PropagateContextChartOrScriptToLTB(this.Strategy.ScriptContextCurrent);
			//ALREADY_SUBSCRIBED_ON_WORKSPACE_RESTORE if (this.Strategy.ScriptContextCurrent.IsStreaming) this.ChartStreamingConsumer.StreamingSubscribe();
		}
		public void InitializeChartNoStrategyAfterDeserialization() {
			this.InitializeChartNoStrategy(null);
		}
		public void InitializeStrategyAfterDeserialization(string strategyGuid, string strategyName = "PLEASE_SUPPLY_FOR_USERS_CONVENIENCE") {
			this.StrategyFoundDuringDeserialization = false;
			Strategy strategyFound = null;
			if (String.IsNullOrEmpty(strategyGuid) == false) {
				strategyFound = Assembler.InstanceInitialized.RepositoryDllJsonStrategy.LookupByGuid(strategyGuid); 	// can return NULL here
			}
			if (strategyFound == null) {
				string msg = "STRATEGY_NOT_FOUND: RepositoryDllJsonStrategy.LookupByGuid(strategyGuid=" + strategyGuid + ")";
				Assembler.PopupException(msg);
				return;
			}
			this.InitializeWithStrategy(strategyFound, true);
			this.StrategyFoundDuringDeserialization = true;
			if (this.Executor.Bars == null) {
				string msg = "TYRINIG_AVOID_BARS_NULL_EXCEPTION: FIXME InitializeWithStrategy() didn't load bars";
				Assembler.PopupException(msg);
				return;
			}

			ContextChart ctxChart = this.ContextCurrentChartOrStrategy;
			bool streamingAsContinuationOfBacktest = ctxChart.IsStreaming && ctxChart.IsStreamingTriggeringScript;
			streamingAsContinuationOfBacktest &= this.Executor.DataSource.StreamingAdapter != null;
			bool willBacktest = streamingAsContinuationOfBacktest || this.Strategy.ScriptContextCurrent.BacktestOnRestart;

			if (willBacktest == false) {
				// COPYFROM_StrategyCompileActivatePopulateSlidersShow()
				if (this.Strategy.Script != null && this.Strategy.ActivatedFromDll) {
					this.Strategy.Script.PushRegisteredScriptParametersIntoCurrentContextSaveStrategy();
				}
				return;
			}

			this.Strategy.CompileInstantiate();
			if (this.Strategy.Script == null) {
				string msig = " InitializeStrategyAfterDeserialization(" + this.Strategy.ToString() + ")";
				string msg = "COMPILATION_FAILED_AFTER_DESERIALIZATION"
					+ " LET_USER_DEACTIVATE_FOR_NEXT_RESTART_NOT_TURNED_OFF: BacktestOnRestart,IsStreamingTriggeringScript";
				Assembler.PopupException(msg + msig);
				//v1: LET_USER_DEACTIVATE_FOR_NEXT_RESTART
				//this.Strategy.ScriptContextCurrent.BacktestOnRestart = false;
				//this.Strategy.ScriptContextCurrent.IsStreamingTriggeringScript = false;
				//Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.Strategy);
				return;
			}
			if (this.Strategy.Script.Executor == null) {
				//IM_GETTING_HERE_ON_STARTUP_AFTER_SUCCESFULL_COMPILATION_CHART_RELATED_STRATEGIES Debugger.Break();
				if (this.Strategy.ActivatedFromDll == true) {
					string msg = "you should never get here; a compiled script should've been already linked to Executor (without bars on deserialization)"
						+ " 10 lines above in this.InitializeWithStrategy(mainForm, strategy)";
					Assembler.PopupException(msg);
				}
				this.Strategy.Script.Initialize(this.Executor);
			}
			//v2 TOO_SPECIFIC this.StrategyActivateBeforeShow();

			
			//v1 this.Executor.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterCompleteOnceOnWorkspaceRestore), true);
			//NOPE_ALREADY_POPULATED_UPSTACK this.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsBacktestIfStrategy("InitializeStrategyAfterDeserialization()");
			//v2
			// same idea as in mniSubscribedToStreamingAdapterQuotesBars_Click();
			// I see StreamingSubscribe() happening down the road since quotes are drawn, just want to avoid YOU_JUST_RESTARTED_APP_AND_DIDNT_EXECUTE_BACKTEST_PRIOR_TO_CONSUMING_STREAMING_QUOTES
			if (willBacktest) {
				this.BacktesterRunSimulation(true);
			}

			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete) {
				string msg = "WILL_NEVER_HAPPEN?";
				Assembler.PopupException(msg);
				this.OptimizerFormShow(false);
				this.LivesimFormShow(false);
			}

			if (this.Strategy.ActivatedFromDll == false) {
				string msg = "DEBUG_ME_NOW this.Strategy.ActivatedFromDll == false";
				//Assembler.PopupException(msg);
				// MOVED_20_LINES_UP this.StrategyCompileActivateBeforeShow();	// if it was streaming at exit, we should have it ready
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
		public void LivesimFormShow(bool keepAutoHidden = true) {
			this.LivesimFormConditionalInstance.Initialize(this);

			DockPanel mainPanelOrAnotherLivesimsPanel = this.dockPanel;
			LivesimForm anotherOptimizer = null;
			foreach (DockContent form in this.dockPanel.Contents) {
				anotherOptimizer = form as LivesimForm;
				if (anotherOptimizer == null) continue;
				if (anotherOptimizer.Pane == null) continue;
				mainPanelOrAnotherLivesimsPanel = anotherOptimizer.Pane.DockPanel;
				break;
			}
			this.LivesimFormConditionalInstance.Show(mainPanelOrAnotherLivesimsPanel);
			this.LivesimFormConditionalInstance.ActivateDockContentPopupAutoHidden(keepAutoHidden, true);
			//this.LivesimFormConditionalInstance.OptimizerControl.Refresh();	// olvBacktest doens't repaint while having results?...
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
				//POPUP_ANYWAY_KOZ_THATS_THE_ONLY_WAY_TO_SHOW_AND_FIX_COMPILATION_ERRORS if (DockContentImproved.IsNullOrDisposed(this.ScriptEditorForm) == false) {
				this.EditorFormShow(false);
				this.ScriptEditorFormConditionalInstance.ScriptEditorControl.PopulateCompilerErrors(this.Strategy.ScriptCompiler.CompilerErrors);
				//}
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
			if (this.Strategy.ActivatedFromDll == false) {
				this.StrategyCompileActivateBeforeShow();
			} else {
				#if DEBUG
				Debugger.Break();
				#endif
			}

			if (this.Strategy.Script != null) {		// NULL if after restart the JSON Strategy.SourceCode was left with compilation errors/wont compile with MY_VERSION
				this.Strategy.Script.IndicatorsInitializeAbsorbParamsFromJsonStoreInSnapshot();
				this.Strategy.Script.PushRegisteredScriptParametersIntoCurrentContextSaveStrategy();
			}
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			this.PopulateSliders();
		}
		public void PopulateSliders() {
			//CAN_HANDLE_NULL_IN_SlidersForm.Instance.Initialize()  if (this.Strategy == null) return;
			// if you are tired of seeing "CHART_NO_STRATEGY" if (this.Strategy == null) return;
			if (SlidersForm.Instance.DockPanel == null) SlidersForm.Instance.Show(this.dockPanel);
			SlidersForm.Instance.Initialize(this.Strategy);		
			if (SlidersForm.Instance.Visible == false) {		// don't activate the tab if user has docked another Form on top of SlidersForm
				//FOR_CHART_NO_STRATEGY_BRINGS_EMPTY_SLIDERS_UP SlidersForm.Instance.Show(this.dockPanel);
				bool bringUp = this.Strategy != null;
				SlidersForm.Instance.ActivateDockContentPopupAutoHidden(!bringUp, bringUp);
			}
		}
		public override string ToString() {
			//v1: NullRef return "Strategy[" + this.Strategy.Name + "], Chart [" + this.ChartForm.ToString() + "]";
			return this.StreamingButtonIdent;
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
