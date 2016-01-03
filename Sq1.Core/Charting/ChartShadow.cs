using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

#if NON_DOUBLE_BUFFERED	//_reverted_to_compulsory_UserControl_no_buffering
using System.Windows.Forms;
#endif

using Sq1.Core.Charting.OnChart;
using Sq1.Core.DoubleBuffered;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;
using Sq1.Core.DataTypes;
using Sq1.Core.StrategyBase;
using Sq1.Core.DataFeed;

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
		public SymbolOfDataSource SymbolOfDataSource { get; private set; }
		// REASON_TO_EXIST: renderOnChartLines() was throwing "Dictionary.CopyTo target array wrong size" during backtest & chartMouseOver
		// push-type notification from Backtester: ChartControl:ChartShadow doen't have access to Core.ScriptExecutor.BacktestIsRunning so Backtester mimics it here 
		public ManualResetEvent paintAllowed { get; private set; }
		public bool PaintAllowedDuringLivesimOrAfterBacktestFinished {
			get { return this.paintAllowed.WaitOne(0); }
			set {	if (value)	this.paintAllowed.Set();
					else		this.paintAllowed.Reset(); }
		}

		public Bars				Bars			{ get; private set; }
		public bool				BarsEmpty		{ get { return this.Bars == null || this.Bars.Count == 0; } }
		public bool				BarsNotEmpty	{ get { return this.Bars != null && this.Bars.Count > 0; } }
		public ScriptExecutor	Executor		{ get; private set; }

		public ChartShadow() : base() {
			paintAllowed = new ManualResetEvent(true);
//			this.ScriptToChartCommunicator = new ScriptToChartCommunicator();
			this.Register();
		}
		public void Register(bool dontAccessAssemblerWhileInDesignMode = false) {
			if (base.DesignMode) return;
			if (dontAccessAssemblerWhileInDesignMode) return;
			if (Assembler.InstanceUninitialized.StatusReporter == null) return;
			Assembler.InstanceInitialized.AlertsForChart.Register(this);
		}
		public virtual void SetExecutor(ScriptExecutor executor) {
			this.Executor = executor;
		}
		public virtual void Initialize(Bars barsNotNull, string strategySavedInChartSettings, bool invalidateAllPanels = true) {
			if (this.Bars != null) this.ChartShadow_RemoveFromDataSource();
			this.Bars = barsNotNull;
			this.ChartShadow_AddToDataSource();

			// ChartForm wants to update last received quote datetime; FOR_NON_CORE_CONSUMERS_ONLY CORE_DEFINED_CONSUMERS_IMPLEMENT_IStreamingConsumer.ConsumeQuoteOfStreamingBar()
			this.Bars.BarStreamingUpdatedMerged -= new EventHandler<BarEventArgs>(bars_BarStreamingUpdatedMerged);
			this.Bars.BarStreamingUpdatedMerged += new EventHandler<BarEventArgs>(bars_BarStreamingUpdatedMerged);
		}
		
		void bars_BarStreamingUpdatedMerged(object sender, BarEventArgs e) {
			this.RaiseBarStreamingUpdatedMerged(e);
		}

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

		// will let you open ChartControl in windows forms Designer (remove "abstract" from class declaration as well) 
		public virtual void ClearAllScriptObjectsBeforeBacktest() { }

		public virtual void PositionsBacktestAdd(List<Position> positionsMaster) { }
		public virtual void PositionsRealtimeAdd(ReporterPokeUnit pokeUnit) { }
		public virtual void AlertsPendingStillNotFilledForBarAdd(int barIndex, List<Alert> alertsPendingSafeCopy) { }


		public virtual void PendingHistoryBacktestAdd(Dictionary<int, AlertList> alertsPendingHistorySafeCopy) { }
		public virtual void AlertsPlacedRealtimeAdd(List<Alert> alertsNewPlaced) { }

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

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			this.Dispose();
		}
		public void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE__" + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
			this.paintAllowed.Dispose();
			this.paintAllowed = null;
			this.IsDisposed = true;
		}
		public bool IsDisposed { get; private set; }



		public void ChartShadow_AddToDataSource() {
			string msig = " //ChartShadow_AddToDataSource("  + this.ToString() + ")";
			if (this.Bars == null) {
				string msg = "DONT_ALLOW_CHART_WITHOUT_BARS";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (this.Bars.DataSource == null) {
				string msg = "IM_LOADING_RANDOM_GENERATED_BARS_250";
				//Assembler.PopupException(msg);
				return;
			}
			string symbol = this.Bars.Symbol;
			DictionaryManyToOne<SymbolOfDataSource, ChartShadow> chartsOpenForSymbol = this.Bars.DataSource.ChartsOpenForSymbol;
			SymbolOfDataSource symbolOfDataSourceFound = chartsOpenForSymbol.FindSimilarKey(new SymbolOfDataSource(symbol, this.Bars.DataSource));
			if (symbolOfDataSourceFound != null) {
				List<ChartShadow> addingToList = chartsOpenForSymbol.FindContentsOf_NullUnsafe(symbolOfDataSourceFound);
				if (addingToList.Contains(this) == false) {
					chartsOpenForSymbol.Add(symbolOfDataSourceFound, this);
				} else {
					string msg = "YOU_ALREADY_ADDED_CHART_SHADOW_TO_DATASOURCE this.Bars.DataSource[" + this.Bars.DataSource + "].ChartsOpenForSymbol[" + symbolOfDataSourceFound + "]";
					Assembler.PopupException(msg + msig);
				}
			} else {
				string msg = "YOU_DIDNT_ADD_SYMBOL_TO_DATASOURCE.ChartsOpenForSymbol.Keys this.Bars.DataSource[" + this.Bars.DataSource + "].ChartsOpenForSymbol[" + symbolOfDataSourceFound + "]";
				Assembler.PopupException(msg + msig);
			}
			this.SymbolOfDataSource = this.Bars.DataSource.ChartsOpenForSymbol.FindContainerFor_throws(this);
		}
		public void ChartShadow_RemoveFromDataSource() {
			string msig = " //ChartShadow_RemoveFromDataSource("  + this.ToString() + ")";
			if (this.Bars == null) {
				string msg = "DONT_ALLOW_CHART_WITHOUT_BARS";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (this.Bars.DataSource == null) {
				string msg = "IM_REPLACING_RANDOM_GENERATED_BARS_250_WITH_SELECTED_FROM_DATASOURCE";
				//Assembler.PopupException(msg);
				return;
			}
			string symbol = this.Bars.Symbol;
			DictionaryManyToOne<SymbolOfDataSource, ChartShadow> chartsOpenForSymbol = this.Bars.DataSource.ChartsOpenForSymbol;
			SymbolOfDataSource symbolOfDataSourceFound = chartsOpenForSymbol.FindSimilarKey(new SymbolOfDataSource(symbol, this.Bars.DataSource));
			if (symbolOfDataSourceFound != null) {
				List<ChartShadow> iMustBeHere = chartsOpenForSymbol.FindContentsOf_NullUnsafe(symbolOfDataSourceFound);
				if (iMustBeHere.Contains(this)) {
					chartsOpenForSymbol.Remove(symbolOfDataSourceFound, this);
				} else {
					string msg = "YOU_DIDNT_ADD_CHART_SHADOW_TO_DATASOURCE this.Bars.DataSource[" + this.Bars.DataSource + "].ChartsOpenForSymbol[" + symbol + "]";
					Assembler.PopupException(msg + msig);
				}
			} else {
				string msg = "YOU_DIDNT_ADD_SYMBOL_TO_DATASOURCE.ChartsOpenForSymbol.Keys this.Bars.DataSource[" + this.Bars.DataSource + "].ChartsOpenForSymbol[" + symbol + "]";
				Assembler.PopupException(msg + msig);
			}
			this.SymbolOfDataSource = null;
		}

	}
}
