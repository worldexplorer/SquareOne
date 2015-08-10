using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Repositories;
using Sq1.Core.Serializers;
using Sq1.Core.StrategyBase;
using Sq1.Core.Livesim;
using Sq1.Gui.Forms;
using Sq1.Gui.FormFactories;
using Sq1.Gui.ReportersSupport;
using Sq1.Gui.Singletons;
using Sq1.Widgets.Sequencing;
using Sq1.Widgets;
using Sq1.Core.Sequencing;
using Sq1.Widgets.Correlation;

namespace Sq1.Gui.Forms {
	public class ChartFormsManager {
		public	MainForm							MainForm;
		public	ChartFormDataSnapshot				DataSnapshot;
		public	Serializer<ChartFormDataSnapshot>	DataSnapshotSerializer;
		public	bool								StrategyFoundDuringDeserialization		{ get; private set; }
		public	Strategy							Strategy;
		public	ScriptExecutor						Executor;
		public	ReportersFormsManager				ReportersFormsManager;
		public	ChartForm							ChartForm;
				DockPanel							dockPanel								{ get { return this.MainForm.DockPanel; } }
				ScriptEditorFormFactory				scriptEditorFormFactory;
				
		public ScriptEditorForm						ScriptEditorForm;
		public ScriptEditorForm						ScriptEditorFormConditionalInstance		{ get {
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
		public	bool								ScriptEditorIsOnSurface					{ get {
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
		
		
				SequencerFormFactory				sequencerFormFactory;
		public	SequencerForm						SequencerForm;
		public	SequencerForm						SequencerFormConditionalInstance		{ get {
				if (DockContentImproved.IsNullOrDisposed(this.SequencerForm)) {
					if (this.Strategy == null) return null;
					if (this.sequencerFormFactory == null) {
						string msg = "SHOULD_NEVER_HAPPEN_this.sequencerFormFactory=null";
						Assembler.PopupException(msg);
						this.sequencerFormFactory = new SequencerFormFactory(this);
					}

					this.SequencerForm = this.sequencerFormFactory.CreateSequencerFormSubscribe();
					if (this.SequencerForm == null) {
						throw new Exception("SequencerFormFactory.CreateAndSubscribe() failed to create SequencerForm in ChartFormsManager");
					}
				}
				return this.SequencerForm;
			} }
		public	bool								SequencerIsOnSurface				{ get {
				SequencerForm sequencer = this.SequencerForm;
				bool sequencerNotInstantiated = DockContentImproved.IsNullOrDisposed(sequencer);
				bool sequencerMustBeActivated = sequencerNotInstantiated ? true : sequencer.MustBeActivated;
				return !sequencerMustBeActivated;
			} }

		public	LivesimForm LivesimForm;
		public	LivesimForm LivesimFormConditionalInstance { get {
				if (DockContentImproved.IsNullOrDisposed(this.LivesimForm)) {
					if (this.Strategy == null) return null;
					this.LivesimForm = new LivesimForm(this);
				}
				return this.LivesimForm;
			} }
		public	bool								LivesimFormIsOnSurface				{ get {
				LivesimForm livesim = this.LivesimForm;
				bool livesimNotInstantiated = DockContentImproved.IsNullOrDisposed(livesim);
				bool livesimMustBeActivated = livesimNotInstantiated ? true : livesim.MustBeActivated;
				return !livesimMustBeActivated;
			} }

		public	CorrelatorForm CorrelatorForm;
		public	CorrelatorForm CorrelatorFormConditionalInstance { get {
				if (DockContentImproved.IsNullOrDisposed(this.CorrelatorForm)) {
					if (this.Strategy == null) return null;
					this.CorrelatorForm = new CorrelatorForm(this);
				}
				return this.CorrelatorForm;
			} }
		public	bool								CorrelatorFormIsOnSurface				{ get {
				CorrelatorForm Correlator = this.CorrelatorForm;
				bool CorrelatorNotInstantiated = DockContentImproved.IsNullOrDisposed(Correlator);
				bool CorrelatorMustBeActivated = CorrelatorNotInstantiated ? true : Correlator.MustBeActivated;
				return !CorrelatorMustBeActivated;
			} }
		
		public	ChartFormInterformEventsConsumer	InterformEventsConsumer;
		public	ChartFormStreamingConsumer			ChartStreamingConsumer;
		
		public	Dictionary<string, DockContentImproved>	FormsAllRelated						{ get {
				var ret = new Dictionary<string, DockContentImproved>();
				if (this.ChartForm			!= null) ret.Add("Chart",		this.ChartForm);
				if (this.ScriptEditorForm	!= null) ret.Add("Source Code",	this.ScriptEditorForm);
				if (this.SequencerForm		!= null) ret.Add("Sequencer",	this.SequencerForm);
				if (this.CorrelatorForm		!= null) ret.Add("Correlator",	this.CorrelatorForm);
				if (this.LivesimForm		!= null) ret.Add("LiveSim",		this.LivesimForm);
				foreach (string textForMenuItem in this.ReportersFormsManager.FormsAllRelated.Keys) {
					ret.Add(textForMenuItem, this.ReportersFormsManager.FormsAllRelated[textForMenuItem]);
				}
				return ret;
			} }
		public	string								StreamingButtonIdent				{ get {
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
		public	ContextChart						ContextCurrentChartOrStrategy		{ get {
				return (this.Strategy != null) ? this.Strategy.ScriptContextCurrent as ContextChart : this.DataSnapshot.ContextChart; } }

		// WHATTTTT???? I dont want it "internal" when "private" is omitted
		ChartFormsManager() {
			this.StrategyFoundDuringDeserialization = false;
			// deserialization: ChartSerno will be restored; never use this constructor in your app!
			//			this.Executor = new ScriptExecutor(Assembler.InstanceInitialized.ScriptExecutorConfig
			//				, this.ChartForm.ChartControl, null, Assembler.InstanceInitialized.OrderProcessor, Assembler.InstanceInitialized.StatusReporter);
			this.Executor = new ScriptExecutor("EXECUTOR_FOR_AN_OPENED_CHART_UNCLONED");
			this.ReportersFormsManager = new ReportersFormsManager(this);
			this.ChartStreamingConsumer = new ChartFormStreamingConsumer(this);

			// never used in CHART_ONLY, but we have "Open In Current Chart" for Strategies
			this.scriptEditorFormFactory = new ScriptEditorFormFactory(this);
			this.sequencerFormFactory = new SequencerFormFactory(this);
			//this.livesimFormFactory = new LivesimFormFactory(this);

			this.DataSnapshotSerializer = new Serializer<ChartFormDataSnapshot>();
		}
		public ChartFormsManager(MainForm mainForm, int charSernoDeserialized = -1) : this() {
			this.MainForm = mainForm;
			if (charSernoDeserialized == -1) {
				this.DataSnapshot = new ChartFormDataSnapshot();
				return;
			}
			bool createdNewFile = this.DataSnapshotSerializer.Initialize(Assembler.InstanceInitialized.AppDataPath,
				"ChartFormDataSnapshot-" + charSernoDeserialized + ".json", "Workspaces",
				Assembler.InstanceInitialized.AssemblerDataSnapshot.WorkspaceCurrentlyLoaded, true, true);
			this.DataSnapshot = this.DataSnapshotSerializer.Deserialize();
			if (this.DataSnapshot == null) {
				string msg = "I_REFUSE_CTOR_SERIALIZATION AVOIDING_NPE this.DataSnapshot[" + this.DataSnapshot + "]=null";
				Assembler.PopupException(msg);
				return;
			}
			if (this.DataSnapshot.ChartSerno != charSernoDeserialized) {
				this.DataSnapshot.ChartSerno = charSernoDeserialized;
				if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == true) {
					string msg = "NEVER_INVOKED_SINCE_CHART_FORM_MANAGER_GOT_SERNO_FROM_XML_WHICH_MUST_BE_EQUAL_TO_SNAP";
					Assembler.PopupException(msg);
					this.DataSnapshotSerializer.Serialize();
				}
			}
		}
		ChartForm chartFormFactory() {
			ChartForm ret = new ChartForm(this);
			//MOVED_TO_Executor.Initialize()
			ret.ChartControl.SetExecutor(this.Executor);
			
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
					Assembler.InstanceInitialized.AssemblerDataSnapshot.WorkspaceCurrentlyLoaded, true, true);
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
				string msg = "WHAT_SELECTORS???__IS_THAT_THE_WAY_TO_CLEAR_SLIDERS_AFTER_REMOVING_STRATEGY_FROM_THE_CHART???";
				Assembler.PopupException(msg, null, false);
				bool loadNewBars = true;
				bool skipBacktest = true;
				bool saveStrategyRequired = false;
				this.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(msig, loadNewBars, skipBacktest, saveStrategyRequired);
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
					Assembler.InstanceInitialized.AssemblerDataSnapshot.WorkspaceCurrentlyLoaded, true, true);
				this.DataSnapshot = this.DataSnapshotSerializer.Deserialize();
				this.DataSnapshot.ChartSerno = charSernoNext;
			}
			this.DataSnapshot.StrategyGuidJsonCheck = strategy.Guid.ToString();
			this.DataSnapshot.StrategyNameJsonCheck = strategy.Name;
			this.DataSnapshot.StrategyAbsPathJsonCheck = strategy.StoredInJsonAbspath;
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == true) {
				this.DataSnapshotSerializer.Serialize();
			}
			
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

			this.Executor.Initialize(this.Strategy, this.ChartForm.ChartControl, false);

			try {
				// Click on strategy should open new chart,  
				if (this.Strategy.ScriptContextCurrent.BacktestOnSelectorsChange == true && this.Strategy.Script == null) {		// && this.Strategy.ActivatedFromDll == false
					this.StrategyCompileActivatePopulateSlidersShow();
				}
				//I'm here via Persist.Deserialize() (=> Reporters haven't been restored yet => backtest should be postponed); will backtest in InitializeStrategyAfterDeserialization
				// STRATEGY_CLICK_TO_CHART_DOESNT_BACKTEST this.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(msig, true, true);
				// ALL_SORT_OF_STARTUP_ERRORS this.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(msig, true, false);
				bool loadNewBars = true;
				bool saveStrategyRequired = false;
				this.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(msig, loadNewBars, skipBacktestDuringDeserialization, saveStrategyRequired);
				if (skipBacktestDuringDeserialization == false) {
					string msg = "YOU_DID_NOT_SWITCH_TO_GUI_THREAD...";
					this.SequencerFormIfOpenPropagateTextboxesOrMarkStaleResultsAndDeleteHistory();
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

		void executor_OnBacktesterContextRestoredAfterExecutingAllBars_step4of4(object sender, EventArgs e) {
			throw new NotImplementedException();
		}
		public void PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy(
					string msig, bool loadNewBars = true, bool skipBacktest = false, bool saveStrategyRequired = true) {
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

				string strategyName = this.Strategy == null ? "CHART_ONLY" : this.Strategy.Name;
				this.ChartForm.ChartControl.Initialize(barsClicked, strategyName, invalidateAllPanels);
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
				this.Strategy.Serialize();
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
					//v1 LOSING_ARROWS_AND_REPORTERS_SINCE_INVOKED_TWICE
					this.Executor.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterCompleteOnceOnWorkspaceRestore), true);
					//this.Executor.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), true);
				} else {
					//v1 this.Executor.BacktesterRunSimulationTrampoline(new Action<ScriptExecutor>(this.afterBacktesterComplete), true);
					//v2 
					this.Executor.BacktesterRunSimulationTrampoline(null, true);
				}
			} catch (Exception ex) {
				string msg = "RUN_SIMULATION_TRAMPOLINE_FAILED for Strategy[" + this.Strategy + "] on Bars[" + this.Executor.Bars + "]";
				Assembler.PopupException(msg, ex);
			}
		}
		public void OnBacktestedOrLivesimmed() {
			this.afterBacktestComplete(this.Executor);
		}
		void afterBacktestComplete(ScriptExecutor myOwnExecutorIgnoring) {
			try {
				if (this.Executor.Bars == null) {
					string msg = "DONT_RUN_BACKTEST_BEFORE_BARS_ARE_LOADED";
					Assembler.PopupException(msg);
					return;
				}
				if (this.ChartForm.IsDisposed) {
					string msg = "CREATE_CHART_FORM_FIRST__ON_APPRESTART_AND WORKSPACE_RELOAD";
					Assembler.PopupException(msg);
					return;
				}
				if (this.ChartForm.InvokeRequired) {
					if (this.ChartForm.ChartFormIsLoaded_NonBlocking == false) {
						bool loadedIwaited = this.ChartForm.ChartFormIsLoaded_Blocking;
						string msg = "CHART_FORM_IS_LOADED__NOW_IT_CAN_DRAW_BACKTESTED_POSITIONS " + this.Executor.ToString();
						Assembler.PopupException(msg, null, false);
					}

					this.ChartForm.BeginInvoke((MethodInvoker)delegate { this.afterBacktestComplete(myOwnExecutorIgnoring); });
					//v2 after worspace is loaded (app restart / workspace change), I don't hit the breakpoint 4 lines below; it looks like ChartFormsManager gets destroyed?
					//v2 making Invoke linear (in the same thread instead of postponed BeginInvoke) allowed to catch the ChartForm.Disposed Exception...
					//this.ChartForm.Invoke((MethodInvoker)delegate { this.afterBacktestComplete(myOwnExecutorIgnoring); });
					return;
				}
				if (this.ChartForm.IsDisposed) {
					string msg = "CREATE_CHART_FORM_FIRST__ON_APPRESTART_AND WORKSPACE_RELOAD";
					Assembler.PopupException(msg);
					return;
				}
				//this.clonePositionsForChartPickupBacktest(this.Executor.ExecutionDataSnapshot.PositionsMaster);
				this.ChartForm.ChartControl.PositionsBacktestAdd(this.Executor.ExecutionDataSnapshot.PositionsMaster.SafeCopy(this, "afterBacktestComplete(WAIT"));
				//this.ChartForm.ChartControl.PendingHistoryBacktestAdd(this.Executor.ExecutionDataSnapshot.AlertsPendingHistorySafeCopy);
				this.ChartForm.ChartControl.PendingHistoryBacktestAdd(this.Executor.ExecutionDataSnapshot.AlertsPending.ByBarPlacedSafeCopy(this, "afterBacktestComplete(WAIT"));
				this.ChartForm.ChartControl.InvalidateAllPanels();
			
				//this.Executor.Performance.BuildStatsOnBacktestFinished(this.Executor.ExecutionDataSnapshot.PositionsMaster);
				// MOVED_TO_BacktesterRunSimulation() this.Executor.Performance.BuildStatsOnBacktestFinished();
				//this.ReportersFormsManager.BuildReportFullOnBacktestFinishedAllReporters(this.Executor.Performance);
				this.ReportersFormsManager.BuildReportFullOnBacktestFinishedAllReporters();

				Assembler.DisplayStatus(this.Executor.LastBacktestStatus);
			} catch (Exception ex) {
				string msig = "Strategy[" + this.Strategy + "] on Bars[" + this.Executor.Bars + "]"
						+ " //afterBacktesterComplete(" + myOwnExecutorIgnoring + ")";
				string msg = "THREAD_ISOLATED_EXCEPTION";
				Assembler.PopupException(msg + msig, ex);
			}
		}
		void afterBacktesterCompleteOnceOnWorkspaceRestore(ScriptExecutor myOwnExecutorIgnoring) {
			try {
				this.afterBacktestComplete(myOwnExecutorIgnoring);
			
				if (this.Strategy == null) {
					Assembler.PopupException("this should never happen this.Strategy=null in afterBacktesterCompleteOnceOnRestart()");
					return;
				}
				//ONLY_ON_WORKSPACE_RESTORE??? this.ChartForm.PropagateContextChartOrScriptToLTB(this.Strategy.ScriptContextCurrent);
				//ALREADY_SUBSCRIBED_ON_WORKSPACE_RESTORE if (this.Strategy.ScriptContextCurrent.IsStreaming) this.ChartStreamingConsumer.StreamingSubscribe();
			} catch (Exception ex) {
				string msig = "Strategy[" + this.Strategy + "] on Bars[" + this.Executor.Bars + "]"
						+ " //afterBacktesterCompleteOnceOnWorkspaceRestore(" + myOwnExecutorIgnoring + ")";
				string msg = "THREAD_ISOLATED_EXCEPTION";
				Assembler.PopupException(msg + msig, ex);
			}
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
				if (this.Strategy.Script == null) {		 //&& this.Strategy.ActivatedFromDll
					string msg = "COMPILE_AND_INITIALIZE_SCRIPT_THEN??...";
					Assembler.PopupException(msg);
				} else {
					this.Strategy.ScriptAndIndicatorParametersReflectedAbsorbMergeFromCurrentContext_SaveStrategy();
				}
				// candidate to be moved to MainForm.cs:156 into foreach (ChartFormsManager cfmgr in this.GuiDataSnapshot.ChartFormManagers.Values) {
				//this.SequencerFormShow(true);
				//this.CorrelatorFormShow(true);
				//this.LivesimFormShow(true);
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
				this.SequencerFormShow(false);
				this.CorrelatorFormShow(false);
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
		public void SequencerFormShow(bool keepAutoHidden = true) {
			if (this.SequencerFormConditionalInstance.Initialized == false) {
				this.SequencerFormConditionalInstance.Initialize(this);
			} else {
				this.SequencerFormConditionalInstance.SequencerControl.SelectHistoryPopulateBacktestsAndPushToCorellatorWithSequencedResultsBySymbolScaleRange();
				// I dont know why it's a second call (must be first) during app startup, but I wanna get CHECKED to shrink full backtests list
				//since I just restored CHECKED in Correlator, I want only sequencer's backtests checked in Correlator!
				//v1 DOESNT_SHOW_ALL_AFTER_RESTART this.CorrelatorForm.CorrelatorControl.Correlator.RaiseOnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt();
				//v2  copypaste from correlator_OnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt(object sender, SequencedBacktestsEventArgs e)
				SequencerControl sequencerControl = this.SequencerFormConditionalInstance.SequencerControl;
				//v2 MOVED_TO_BacktestsReplaceWithCorrelated()
				//if (sequencerControl.ShowOnlyCorrelatorChosenBacktests) {
				//    CorrelatorControl correlatorControl = this.CorrelatorFormConditionalInstance.CorrelatorControl;
				//    SequencedBacktests chosenOnly = correlatorControl.Correlator.SequencedBacktestsOriginalMinusParameterValuesUnchosen;
				//    sequencerControl.BacktestsReplaceWithCorrelated(chosenOnly.BacktestsReadonly);
				//} else {
				//    sequencerControl.BacktestsShowAll_regardlessWhatIsChosenInCorrelator();
				//}
				CorrelatorControl correlatorControl = this.CorrelatorFormConditionalInstance.CorrelatorControl;
				SequencedBacktests chosenOnly = correlatorControl.Correlator.SequencedBacktestsOriginalMinusParameterValuesUnchosen;
				sequencerControl.BacktestsReplaceWithCorrelated(chosenOnly);
			}

			if (this.SequencerFormConditionalInstance.MustBeActivated) {
				this.SequencerFormConditionalInstance.ActivateDockContentPopupAutoHidden(keepAutoHidden, true);
				return;			// NOT_SURE
			}
			DockPanel mainPanelOrAnotherSequencerPanel = this.dockPanel;
			DockPane			 anotherSequencerPane  = null;
			SequencerForm anotherSequencer = null;
			foreach (DockContent form in this.dockPanel.Contents) {
				anotherSequencer = form as SequencerForm;
				if (anotherSequencer == null) continue;
				if (anotherSequencer.Pane == null) continue;
				mainPanelOrAnotherSequencerPanel = anotherSequencer.Pane.DockPanel;		// will point to Main's this.dockPanel
				anotherSequencerPane = anotherSequencer.Pane;
				break;
			}
			if (anotherSequencerPane != null) {
				this.SequencerFormConditionalInstance.Show(anotherSequencerPane, null);	// will place new sequencer into the same panel as (at right of) the previous & found
			} else {
				this.SequencerFormConditionalInstance.Show(mainPanelOrAnotherSequencerPanel);
			}
			this.SequencerFormConditionalInstance.ActivateDockContentPopupAutoHidden(keepAutoHidden, true);
			//this.SequencerFormConditionalInstance.SequencerControl.Refresh();	// olvBacktest doens't repaint while having results?...
			//this.SequencerFormConditionalInstance.SequencerControl.Invalidate();	// olvBacktest doens't repaint while having results?...
		}
		public void CorrelatorFormShow(bool keepAutoHidden = true) {
			//MUST_BE_INVOKED_SINCE_SECOND_INITIALIZATION_WILL_POPULATE_DESERIALIZED if (this.CorrelatorFormConditionalInstance.Initialized == false) {
				this.CorrelatorFormConditionalInstance.Initialize(this);
			//}

			if (this.CorrelatorFormConditionalInstance.MustBeActivated) {
				this.CorrelatorFormConditionalInstance.ActivateDockContentPopupAutoHidden(keepAutoHidden, true);
				return;			// NOT_SURE
			}

			DockPanel mainPanelOrAnotherCorrelatorPanel = this.dockPanel;
			DockPane			 anotherCorrelatorPane  = null;
			CorrelatorForm anotherCorrelator = null;
			foreach (DockContent form in this.dockPanel.Contents) {
				anotherCorrelator = form as CorrelatorForm;
				if (anotherCorrelator == null) continue;
				if (anotherCorrelator.Pane == null) continue;
				mainPanelOrAnotherCorrelatorPanel = anotherCorrelator.Pane.DockPanel;		// will point to Main's this.dockPanel
				anotherCorrelatorPane = anotherCorrelator.Pane;
				//this.CorrelatorFormConditionalInstance.Show(mainPanelOrAnotherCorrelatorPanel, DockState.Document);
				break;
			}
			if (anotherCorrelatorPane != null) {
				this.CorrelatorFormConditionalInstance.Show(anotherCorrelatorPane, null);	// will place new correlator into the same panel as (at right of) the previous & found
			} else {
				if (mainPanelOrAnotherCorrelatorPanel == this.dockPanel) {
					if (this.SequencerFormConditionalInstance.Pane == null) {
						this.SequencerFormShow(false);
					}
					if (this.SequencerFormConditionalInstance.Pane == null) {
						string msg = "STILL_NULL_AFTER_OPENING_SEQUENCER__AVOIDING_NPE"
							+ " this.SequencerFormConditionalInstance.Pane=null ATER this.SequencerFormShow(false)";
						Assembler.PopupException(msg);
						return;
					}
					DockPane landingPane = this.SequencerFormConditionalInstance.Pane;
					bool autoHide = landingPane.DockState == DockState.DockBottomAutoHide
								 || landingPane.DockState == DockState.DockLeftAutoHide
								 || landingPane.DockState == DockState.DockRightAutoHide
								 || landingPane.DockState == DockState.DockTopAutoHide;

					if (autoHide) {
						string msg = "CANT_ADD_PANE_RELATIVELY_TO_AUTOHIDE_PANE ADDING_TO_RIGHT__NOT_BEST_SOLUTION_BUT_DOESNT_THROW";
						Assembler.PopupException(msg, null, false);
						this.CorrelatorFormConditionalInstance.Show(this.dockPanel, DockState.DockRight);
					} else {
						this.CorrelatorFormConditionalInstance.Show(this.SequencerFormConditionalInstance.Pane, DockAlignment.Bottom, 0.6);
					}
				} else {
					string msg = "UNKNOWN_CASE__REMOVE_ME_IF_YOU_NEVER_SEE_ME";
					Assembler.PopupException(msg);
					this.CorrelatorFormConditionalInstance.Show(mainPanelOrAnotherCorrelatorPanel);
				}
			}
			this.CorrelatorFormConditionalInstance.ActivateDockContentPopupAutoHidden(keepAutoHidden, !keepAutoHidden);

			// WILL_RAISE_BUT_AND_CORRELATOR_ALREADY_CATCHES_IT this.SequencerFormConditionalInstance.SequencerControl.SelectHistoryPopulateBacktestsAndPushToCorellatorWithSequencedResultsBySymbolScaleRange();
			//NEXT_LINE_RAISE_WILL_PUSH_IT_BUT_SECOND_CLICK_WILL_SHOW_ZEROES this.CorrelatorFormConditionalInstance.PopulateSequencedHistory(this.SequencerFormConditionalInstance.SequencerControl.PushToCorrelator);
			//DONT_INIT_IF_I_HAVE_NON_DEFAULT_SCALEINTERVAL_SELECTED this.SequencerFormShow(false);
			//REMOVED_GUI_OPTIMIZATION__WHY_IS_IT_HERE?? this.CorrelatorFormConditionalInstance.CorrelatorControl.Correlator.RaiseOnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt();
			//this.SequencerFormConditionalInstance.ActivateDockContentPopupAutoHidden(false, true);
		}
		public void LivesimFormShow(bool keepAutoHidden = true) {
			this.LivesimFormConditionalInstance.Initialize(this);

			DockPanel mainPanelOrAnotherLivesimsPanel = this.dockPanel;
			LivesimForm anotherLivesim = null;
			foreach (DockContent form in this.dockPanel.Contents) {
				anotherLivesim = form as LivesimForm;
				if (anotherLivesim == null) continue;
				if (anotherLivesim.Pane == null) continue;
				mainPanelOrAnotherLivesimsPanel = anotherLivesim.Pane.DockPanel;
				break;
			}
			this.LivesimFormConditionalInstance.Show(mainPanelOrAnotherLivesimsPanel);
			this.LivesimFormConditionalInstance.ActivateDockContentPopupAutoHidden(keepAutoHidden, true);
			//this.LivesimFormConditionalInstance.SequencerControl.Refresh();	// olvBacktest doens't repaint while having results?...
			//this.LivesimFormConditionalInstance.SequencerControl.Invalidate();	// olvBacktest doens't repaint while having results?...
		}
		
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
			this.ChartForm.Text = this.Strategy.WindowTitle;
			this.ChartForm.IsHidden = false;
			if (this.ScriptEditorForm != null) {
				this.ScriptEditorForm.Text = this.ChartForm.Text;
			}

			this.ReportersFormsManager.WindowTitlePullFromStrategy_allReporterWrappers();
			if (DockContentImproved.IsNullOrDisposed(this.SequencerForm) == false) {
				this.SequencerForm.WindowTitlePullFromStrategy();
			}
			if (DockContentImproved.IsNullOrDisposed(this.LivesimForm) == false) {
				this.LivesimForm.WindowTitlePullFromStrategy();
			}
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
				this.Executor.Sequencer.RaiseScriptRecompiledUpdateHeaderPostponeColumnsRebuild();
				this.Executor.Sequencer.Initialize();						// removes "sequencerInitializedProperly == false" on app restart => Sequencer fills up with Script&Indicator Prarmeters for a JSON-based strategy
			}
			// moved to StrategyCompileActivatePopulateSlidersShow() because no need to PopulateSliders during Deserialization
			//SlidersForm.Instance.Initialize(this.Strategy);
			this.Executor.ChartShadow.HostPanelForIndicatorClear();		//non-DLL-strategy multiple F5s add PanelIndicator endlessly
		}
		public void StrategyCompileActivatePopulateSlidersShow() {
			if (this.Strategy.ActivatedFromDll == false) {
				this.StrategyCompileActivateBeforeShow();
			} else {
				#if DEBUG
				Debugger.Break();
				#endif
			}

			if (this.Strategy.Script == null) {		// NULL if after restart the JSON Strategy.SourceCode was left with compilation errors/wont compile with MY_VERSION
				string msg = "COMPILE_AND_INITIALIZE_SCRIPT_THEN??...";
				Assembler.PopupException(msg);
			} else {
				this.Strategy.ScriptAndIndicatorParametersReflectedAbsorbMergeFromCurrentContext_SaveStrategy();
			}
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			this.PopulateSliders();
		}
		public void PopulateSliders() {
			//CAN_HANDLE_NULL_IN_SlidersForm.Instance.Initialize()  if (this.Strategy == null) return;
			// if you are tired of seeing "CHART_NO_STRATEGY" if (this.Strategy == null) return;
			if (SlidersForm.Instance.DockPanel == null) SlidersForm.Instance.Show(this.dockPanel);
			SlidersForm.Instance.Initialize(this.Strategy);		
			if (SlidersForm.Instance.IsDockedAutoHide) return;	// don't activate the tab if minimized (by user click or during WorkspaceLoad()))
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
			DataSourcesForm.Instance.DataSourcesTreeControl.RaiseOnSymbolInfoEditorClicked();
			if (this.Strategy != null) {
				StrategiesForm.Instance.StrategiesTreeControl.SelectStrategy(this.Strategy);
			} else {
				StrategiesForm.Instance.StrategiesTreeControl.UnSelectStrategy();
			}
			this.PopulateSliders();
		}

		
		public void SequencerFormIfOpenPropagateTextboxesOrMarkStaleResultsAndDeleteHistory(bool deleteSequencedBacktest = false) {
			if (deleteSequencedBacktest) {
				//v1 this.Strategy.OptimizationResultsByContextIdent.Clear();
				if (this.SequencerForm != null) {
					this.SequencerForm.SequencerControl.RepositoryJsonSequencer.ItemsFoundDeleteAll();
				} else {
					try {
						RepositoryJsonsInFolderSimpleDictionarySequencer repositoryJsonSequencer = new RepositoryJsonsInFolderSimpleDictionarySequencer();
						repositoryJsonSequencer.Initialize(Assembler.InstanceInitialized.AppDataPath,
							Path.Combine("Sequencer", this.Strategy.RelPathAndNameForSequencerResults));
						int deleted = repositoryJsonSequencer.ItemsFoundDeleteAll();
						string msg = "RECOMPILATION_ERASED_SEQUENCER_HISTORY [" + deleted + "] SequencedBacktests deleted (with many backtests each)";
						Assembler.DisplayStatus(msg);
					} catch (Exception ex) {
						string msg = "RECOMPILATION_COULD_NOT_ERASE_SEQUENCER_HISTORY";
						Assembler.PopupException(msg, ex);
					}
				}
			}

			if (this.SequencerForm == null) {
				string msg = "JUST_WANNA_KNOW_IF_I_EVER_CHECK_FOR_STALE_BEFORE_FORM_IS_CREATED";
				//Assembler.PopupException(msg);
				return;
			}

			//v1
			SequencerControl control = this.SequencerFormConditionalInstance.SequencerControl;
			control.SelectHistoryPopulateBacktestsAndPushToCorellatorWithSequencedResultsBySymbolScaleRange();
			//v2 NO!!! I_SELECTED_SIXTEEN_STROKE_AND_I_EXPECT_this.populateTextboxesFromExecutorsState() olvHistory_ItemActivate will do a better job

			//string staleReason = control.PopulateTextboxesFromExecutorsState();
			//bool clearFirstBeforeClickingAnotherSymbolScaleIntervalRangePositionSize = string.IsNullOrEmpty(staleReason) == false;
			//if (clearFirstBeforeClickingAnotherSymbolScaleIntervalRangePositionSize == true) {
			//	control.NormalizeBackgroundOrMarkIfBacktestResultsAreForDifferentSymbolScaleIntervalRangePositionSize();
			//}
		}

		public void LivesimStartedOrUnpaused_AutoHiddeExecutionAndReporters() {
			LivesimStreamingSettings stream = this.ChartForm.ChartFormManager.Executor.Strategy.LivesimStreamingSettings;
			if (stream.DelayBetweenSerialQuotesEnabled && stream.DelayBetweenSerialQuotesMin >= 50) return;
			ExecutionForm exec = ExecutionForm.Instance;
			if (exec.IsCoveredOrAutoHidden == false) exec.ToggleAutoHide();
			this.ReportersFormsManager.LivesimStartedOrUnpaused_AutoHideReporters();
		}
		public void LivesimEndedOrStoppedOrPaused_RestoreAutoHiddenExecutionAndReporters() {
			ExecutionForm exec = ExecutionForm.Instance;
			if (exec.IsCoveredOrAutoHidden == true) exec.ToggleAutoHide();
			exec.ExecutionTreeControl.RebuildAllTreeFocusOnTopmost();
			this.ReportersFormsManager.LivesimEndedOrStoppedOrPaused_RestoreAutoHiddenReporters();
		}
		public void Dispose_workspaceReloading() {
			string msig = " //ChartFormsManager.Dispose_workspaceReloading()";
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE__" + this.ToString();
				Assembler.PopupException(msg + msig);
				return;
			}
			this.ChartStreamingConsumer.StreamingUnsubscribe(msig);
			this.ReportersFormsManager	.Dispose_workspaceReloading();
			this.ChartForm				.Dispose();
			//this.SequencerForm			.Dispose();
			//this.CorrelatorForm			.Dispose();
			//this.LivesimForm			.Dispose();
			this.Executor				.Dispose();

			this.ReportersFormsManager	= null;
			this.ChartForm				= null;
			this.SequencerForm			= null;
			this.CorrelatorForm			= null;
			this.LivesimForm			= null;
			this.Executor 				= null;

			this.IsDisposed = true;
		}
		public bool IsDisposed { get; private set; }
	}
}
