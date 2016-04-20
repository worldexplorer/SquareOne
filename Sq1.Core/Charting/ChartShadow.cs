using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

#if NON_DOUBLE_BUFFERED	//_reverted_to_compulsory_UserControl_no_buffering
using System.Windows.Forms;
#endif

using Sq1.Core.Charting.OnChart;
using Sq1.Core.DoubleBuffered;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;
using Sq1.Core.DataTypes;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Charting {
	public partial class ChartShadow : 
//COMMENTED_OUT_TO_MAKE_C#DEVELOPER_CLICK_THROUGH
#if NON_DOUBLE_BUFFERED //_reverted_to_compulsory_UserControl_no_buffering
	UserControl
#else
	UserControlDoubleBuffered
#endif
		, IDisposable
	{
		// REASON_TO_EXIST: renderOnChartLines() was throwing "Dictionary.CopyTo target array wrong size" during backtest & chartMouseOver
		// push-type notification from Backtester: ChartControl:ChartShadow doen't have access to Core.ScriptExecutor.BacktestIsRunning so Backtester mimics it here 
		public	ManualResetEvent		paintAllowed			{ get; private set; }
		public	bool					PaintAllowedDuringLivesimOrAfterBacktestFinished {
			get { return this.paintAllowed.WaitOne(0); }
			set {	if (value)	this.paintAllowed.Set();
					else		this.paintAllowed.Reset(); }
		}

		public	Bars					Bars					{ get; private set; }
		public	bool					BarsEmpty				{ get { return this.Bars == null || this.Bars.Count == 0; } }
		public	bool					BarsNotEmpty			{ get { return this.Bars != null && this.Bars.Count > 0; } }
		public	ScriptExecutor			Executor				{ get; private set; }
		public	StreamingConsumerChart	ChartStreamingConsumer	{ get; private set; }

		public	Color					ColorBackground_inDataSourceTree;
		public	ContextChart			CtxChart;

		public ChartShadow() : base() {
			paintAllowed = new ManualResetEvent(true);
			this.register();
			this.ColorBackground_inDataSourceTree	= Color.White;
			this.ChartStreamingConsumer				= new StreamingConsumerChart(this);
		}
		void register(bool dontAccessAssemblerWhileInDesignMode = false) {
			if (base.DesignMode) return;
			if (dontAccessAssemblerWhileInDesignMode) return;
			if (Assembler.InstanceUninitialized.StatusReporter == null) return;
			Assembler.InstanceInitialized.AlertsForChart.Register(this);
		}
		public virtual void SetExecutor(ScriptExecutor executor) {
			this.Executor = executor;
		}
		public virtual void Initialize(Bars barsNotNull, bool removeChartShadowFromOldSymbolAndAddToLoadingBars = false, bool invalidateAllPanels = true) {
			if (barsNotNull == null) {
				string msg = "AVOIDING_NPE LIVESIM_ABORTED_WITH_NO_BARS_TO_RESTORE //ChartShadow.Initialize(null)";
				Assembler.PopupException(msg, null, false);
				return;
			}
			#region I loaded bars by click on the DataSourceTree=>Symbol; I want the ChartName to move from previous symbol to barsNotNull.Symbol
			// 1) ChartDeserialization
			// 2) Backtester.InitializeAndRun_step1or2()
			// 3) LIVESIM_START Livesimulator.executor_BacktesterContextInitializedStep2of4()
			// 4) LIVESIM_END Livesimulator.afterBacktesterComplete()

			if (removeChartShadowFromOldSymbolAndAddToLoadingBars && this.Bars != null)	this.ChartShadow_RemoveFromDataSource();
			//OBSOLETE_NOW__USE_STREAMING_CONSUMERS_INSTEAD if (this.Bars != null) this.Bars.OnBarStreamingUpdatedMerged -= new EventHandler<BarEventArgs>(this.bars_OnBarStreamingUpdatedMerged_invokedOnlyWhenUserSubscribedChart_tunneledToChartForm);

			this.Bars = barsNotNull;
			if (removeChartShadowFromOldSymbolAndAddToLoadingBars)						this.ChartShadow_AddToDataSource();
			#endregion

			// ChartForm wants to update last received quote datetime; FOR_NON_CORE_CONSUMERS_ONLY CORE_DEFINED_CONSUMERS_IMPLEMENT_IStreamingConsumer.ConsumeQuoteOfStreamingBar()
			//OBSOLETE_NOW__USE_STREAMING_CONSUMERS_INSTEAD this.Bars.OnBarStreamingUpdatedMerged += new EventHandler<BarEventArgs>(this.bars_OnBarStreamingUpdatedMerged_invokedOnlyWhenUserSubscribedChart_tunneledToChartForm);
		}
		//void bars_OnBarStreamingUpdatedMerged_invokedOnlyWhenUserSubscribedChart_tunneledToChartForm(object sender, BarEventArgs e) {
		//    this.RaiseOnBarStreamingUpdatedMerged_chartFormPrintsQuoteTimestamp(e);
		//}
		
		public virtual bool SelectPosition(Position position) {
			string msg = "ChartShadow::SelectPosition() TODO: implement HIGHLIGHTING for a position[" + position + "]; chart[" + this + "]";
			Assembler.PopupException(msg);
			bool tooltipPositionShown = false;
			return tooltipPositionShown;
		}
		
		public virtual void SelectAlert(Alert alert) {
			string msg = "ChartShadow::SelectAlert() TODO: implement HIGHLIGHTING for a alert[" + alert + "]; chart[" + this + "]";
			Assembler.PopupException(msg);
		}

//#if !NON_DOUBLE_BUFFERED_reverted_to_compulsory_UserControl_no_buffering
//		protected override void OnPaintDoubleBuffered(PaintEventArgs paintEventArgs) {
//			if (base.DesignMode) return;
//			//throw new Exception("ChartShadow::OnPaintDoubleBuffered(): Implemented in Chart, make sure proper casting occurs");
//		}
//		protected override void OnPaintBackgroundDoubleBuffered(PaintEventArgs paintEventArgs) {
//			if (base.DesignMode) return;
//			//throw new Exception("ChartShadow::OnPaintBackgroundDoubleBuffered(): Implemented in Chart, make sure proper casting occurs");
//		}
//#else
//#endif
		
		/*
#region there is no graphics-related (PositionArrows / LinesDrawnOnChart) DataSnapshot in Core; ChartControl knows how to handle your wishes in terms of Core objects
		// "virtual" allows here to derive Control from ChartShadow and open it in Designer - it will be displayed without implementations; will start throwing in runtime and you'll see the stack then
		public abstract void ClearAllScriptObjectsBeforeBacktest();

		public abstract void PositionsBacktestAdd(List<Position> positionsMaster);
		public abstract void PositionsRealtimeAdd(ReporterPokeUnit pokeUnit);

		public abstract void PendingHistoryBacktestAdd(Dictionary<int, List<Alert>> alertsPendingHistorySafeCopy);
		public abstract void PendingRealtimeAdd(ReporterPokeUnit pokeUnit);

		public abstract OnChartObjectOperationStatus LineDrawModify(
				string id, int barStart, double priceStart, int barEnd, double priceEnd,
				Color color, int width, bool debugParametersDidntChange = false);
		public abstract bool BarBackgroundSet(int barIndex, Color colorBg);
		public abstract Color BarBackgroundGet(int barIndex);
		public abstract bool BarForegroundSet(int barIndex, Color colorFg);
		public abstract Color BarForegroundGet(int barIndex);
		public abstract OnChartObjectOperationStatus ChartLabelDrawOnNextLineModify(
				string labelId, string labelText,
				Font font, Color colorFore, Color colorBack);
		public abstract OnChartObjectOperationStatus BarAnnotationDrawModify(
				int barIndex, string barAnnotationId, string barAnnotationText,
				Font font, Color colorForeground, Color colorBackground, bool aboveBar = true,
				int verticalPadding = 5, bool reportDidntChangeStatus = false);
		public abstract void SyncBarsIdentDueToSymbolRename();
#endregion

		public abstract HostPanelForIndicator HostPanelForIndicatorGet(Indicator indicator);
		public abstract void HostPanelForIndicatorClear();
		public abstract void SetIndicators(Dictionary<string, Indicator> indicators);

		public abstract void RangeBarCollapseToAccelerateLivesim();

		// RELEASE_DOESNT_REPAINT_CHART_LIVESIM_DELAYED ALREADY_HANDLED_BY_chartControl_BarAddedUpdated_ShouldTriggerRepaint
		public abstract void InvalidateAllPanels();
		public abstract void RefreshAllPanelsNonBlockingRefreshNotYetStarted();
		*/

		#region VIRTUAL instead of ABSTRACT will let you open ChartControl in windows forms Designer (remove "abstract" from class declaration as well)
		public virtual void ClearAllScriptObjectsBeforeBacktest() { }

		public virtual void PositionsBacktestAdd(List<Position> positionsMaster) { }
		public virtual void PositionsRealtimeAdd(ReporterPokeUnit pokeUnit) { }
		public virtual void AlertsPendingStillNotFilledForBarAdd(int barIndex, List<Alert> alertsPendingSafeCopy) { }


		public virtual void PendingHistoryBacktestAdd(Dictionary<int, AlertList> alertsPendingHistorySafeCopy) { }
		public virtual void AlertsPlaced_addRealtime(List<Alert> alertsNewPlaced) { }

		public virtual OnChartObjectOperationStatus LineDrawModify(
				string id, int barStart, double priceStart, int barEnd, double priceEnd,
				Color color, int width, bool debugParametersDidntChange = false) { return OnChartObjectOperationStatus.Unknown; }
		public virtual bool BarBackgroundSet(int barIndex, Color colorBg) { return true; }
		public virtual Color BarBackgroundGet(int barIndex) { return Color.Black; }
		public virtual bool BarForegroundSet(int barIndex, Color colorFg) { return true; }
		public virtual Color BarForegroundGet(int barIndex) { return Color.Black; }
		public virtual OnChartObjectOperationStatus ChartLabelDrawOnNextLineModify(
				string labelId, string labelText,
				Font font, Color colorFore, Color colorBack) { return OnChartObjectOperationStatus.Unknown; }
		public virtual OnChartObjectOperationStatus BarAnnotationDrawModify(
				int barIndex, string barAnnotationId, string barAnnotationText,
				Font font, Color colorForeground, Color colorBackground, bool aboveBar = true,
				int verticalPadding = 5, bool reportDidntChangeStatus = false) { return OnChartObjectOperationStatus.Unknown; }
		public virtual void SyncBarsIdentDueToSymbolRename() { }

		public virtual HostPanelForIndicator HostPanelForIndicatorGet(Indicator indicator) { return null; }
		public virtual void HostPanelForIndicatorClear() { }
		public virtual void SetIndicators(Dictionary<string, Indicator> indicators) { }

		public virtual void RangeBarCollapseToAccelerateLivesim() { }

		// RELEASE_DOESNT_REPAINT_CHART_LIVESIM_DELAYED ALREADY_HANDLED_BY_chartControl_BarAddedUpdated_ShouldTriggerRepaint
		public virtual void InvalidateAllPanels() { }
		//public virtual void RefreshAllPanelsNonBlockingRefreshNotYetStarted() { }
		public virtual void PushQuote_toExecutorObjects_fromStreamingDataSnapshot_triggerInvalidateAll() { }
		public virtual void PushLevelTwoFrozen_toExecutorObjects_fromStreamingDataSnapshot_triggerInvalidateAll(LevelTwoFrozen levelTwoFrozen) { }
		#endregion


		public override string ToString() {
			string ret = null;
			if (this.Executor != null) {
				if (this.Executor.Strategy != null) {
					ret = this.Executor.Strategy.ToString();
				} else {
					if (this.CtxChart != null) {
						ret = this.CtxChart.ToString();
					}
				}
			}
			if (string.IsNullOrEmpty(ret) && base.InvokeRequired == false) ret = base.Name;
			return ret;
		}




		protected override void Dispose(bool disposing) {
			//this.Dispose();
			base.Dispose(disposing);
		}
		void IDisposable.Dispose() {
			if (this.ChartShadowResourcesDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE__" + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
			this.paintAllowed.Dispose();
			this.paintAllowed = null;
			this.ChartShadowResourcesDisposed = true;
		}
		public bool ChartShadowResourcesDisposed { get; private set; }



		string prePauseWindowsTitle = "";
		internal void PumpPaused_notification_switchLivesimmingThreadToGui() {
			if (base.ParentForm == null) {
				string msg = "CANT_SET_CHARTFORM_WINDOW_TITLE //PumpPaused_notification()";
				Assembler.PopupException(msg);
				return;
			}
			if (base.InvokeRequired) {
				//base.BeginInvoke((MethodInvoker) delegate { this.PumpPaused_notification_switchLivesimmingThreadToGui(); } );
				base.BeginInvoke(new MethodInvoker(this.PumpPaused_notification_switchLivesimmingThreadToGui));
				return;
			}
			if (base.ParentForm.Text.Contains("PAUSED")) {
				string msg = "I_PAUSE_TWICE__KOZ_EACH_CHART_HAS_PUMPQUOTE_AND_PUMPLEVEL2"
					//+ " 1) DID_UNDUPLICATION_WORK? 2) DONT_NOTIFY_CHART_IM_LIVESIMMING_YOU_PAUSED_REPLACED_DISTRIBUTOR"
					;
				//Assembler.PopupException(msg);
				return;
			}
			this.prePauseWindowsTitle = base.ParentForm.Text;
			base.ParentForm.Text = "PAUSED " + this.prePauseWindowsTitle;
			this.raiseOnPumpPaused();
		}
		internal void PumpUnPaused_notification_switchLivesimmingThreadToGui() {
			if (this.prePauseWindowsTitle == "") return;	// I wasn't paused from brother livesimming and I don't wanna reset my title
			if (base.ParentForm == null) {
				string msg = "CANT_SET_CHARTFORM_WINDOW_TITLE //PumpUnPaused_notification()";
				Assembler.PopupException(msg);
				return;
			}
			if (base.InvokeRequired) {
				base.BeginInvoke(new MethodInvoker(this.PumpUnPaused_notification_switchLivesimmingThreadToGui));
				return;
			}
			base.ParentForm.Text = this.prePauseWindowsTitle;
			this.prePauseWindowsTitle = "";
			this.raiseOnPumpUnPaused();
		}

	}
}
