using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

#if NON_DOUBLE_BUFFERED_reverted_to_compulsory_UserControl_no_buffering
using System.Windows.Forms;
#endif

using Sq1.Core.Charting;
using Sq1.Core.Charting.OnChart;
using Sq1.Core.DoubleBuffered;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Charting {
	public abstract partial class ChartShadow : 
//COMMENTED_OUT_TO_MAKE_C#DEVELOPER_CLICK_THROUGH #if !NON_DOUBLE_BUFFERED_reverted_to_compulsory_UserControl_no_buffering
	UserControlDoubleBuffered
//#else
//	UserControl
//#endif
	{
		//REPLACED_BY_ScriptExecutorObjects public ScriptToChartCommunicator ScriptToChartCommunicator { get; protected set; }
		public event EventHandler<EventArgs> ChartSettingsChangedContainerShouldSerialize;
		public event EventHandler<EventArgs> ContextScriptChangedContainerShouldSerialize;

		// REASON_TO_EXIST: renderOnChartLines() was throwing "Dictionary.CopyTo target array wrong size" during backtest & chartMouseOver
		// push-type notification from Backtester: ChartControl:ChartShadow doen't have access to Core.ScriptExecutor.BacktestIsRunning so Backtester mimics it here 
		public ManualResetEvent BacktestIsRunning { get; private set; }
		public bool IsBacktestingNow { get { return this.BacktestIsRunning.WaitOne(0); } }

		public Bars Bars { get; private set; }
		public bool BarsEmpty { get { return this.Bars == null || this.Bars.Count == 0; } }
		public bool BarsNotEmpty { get { return this.Bars != null && this.Bars.Count > 0; } }

		public ChartShadow() : base() {
			BacktestIsRunning = new ManualResetEvent(false);
//			this.ScriptToChartCommunicator = new ScriptToChartCommunicator();
			this.Register();
		}
		public void Register(bool dontAccessAssemblerWhileInDesignMode = false) {
			if (base.DesignMode) return;
			if (dontAccessAssemblerWhileInDesignMode) return;
			if (Assembler.InstanceUninitialized.StatusReporter == null) return;
			Assembler.InstanceInitialized.AlertsForChart.Register(this);
		}
		public virtual void Initialize(Bars barsNotNull, bool invalidateAllPanels = true) {
			this.Bars = barsNotNull;
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
		public void RaiseChartSettingsChangedContainerShouldSerialize() {
			if (this.ChartSettingsChangedContainerShouldSerialize == null) return;
			try {
				this.ChartSettingsChangedContainerShouldSerialize(this, null);
			} catch (Exception ex) {
				Assembler.PopupException("RaiseChartSettingsChangedContainerShouldSerialize()", ex);
			}
		}
		public void RaiseContextScriptChangedContainerShouldSerialize() {
			if (this.ContextScriptChangedContainerShouldSerialize == null) return;
			try {
				this.ContextScriptChangedContainerShouldSerialize(this, null);
			} catch (Exception ex) {
				Assembler.PopupException("RaiseContextScriptChangedContainerShouldSerialize()", ex);
			}
		}
		
		
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
	}	
}
