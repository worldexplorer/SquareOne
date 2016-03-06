using System;
using System.Drawing;

using Sq1.Core.Charting;
using Sq1.Core.Charting.OnChart;
using Sq1.Core.Indicators;

namespace Sq1.Core.StrategyBase {
	public partial class ScriptExecutor {	// the following wrappers only make sense for Sequencing, when Executor.ChartShadow=null
		// COMPILATION_ERROR??? public => internal block access from Script-derived and to enforce usage of Script's wrappers for the same methods
//		public  void ChartConditionalSetIndicators(Dictionary<string, Indicator> indicators) {
//			if (this.ChartShadow == null) return;
//			this.ChartShadow.SetIndicators(indicators);
//		}
//		public void ChartConditionalClearAllScriptObjectsBeforeBacktest() {
//			if (this.ChartShadow == null) return;
//			this.ChartShadow.ClearAllScriptObjectsBeforeBacktest();
//		}
//		public void ChartConditionalPositionsBacktestAdd(List<Position> positionsMaster) {
//			if (this.ChartShadow == null) return;
//			this.ChartShadow.PositionsBacktestAdd(positionsMaster);
//		}
//		public void ChartConditionalPositionsRealtimeAdd(ReporterPokeUnit pokeUnit) {
//			if (this.ChartShadow == null) return;
//			this.ChartShadow.PositionsRealtimeAdd(pokeUnit);
//		}
//		public void ChartConditionalPendingHistoryBacktestAdd(Dictionary<int, List<Alert>> alertsPendingHistorySafeCopy) {
//			if (this.ChartShadow == null) return;
//			this.ChartShadow.PendingHistoryBacktestAdd(alertsPendingHistorySafeCopy);
//		}
//		public void ChartConditionalPendingRealtimeAdd(ReporterPokeUnit pokeUnit) {
//			if (this.ChartShadow == null) return;
//			this.ChartShadow.PendingRealtimeAdd(pokeUnit);
//		}
		public void ChartConditional_barAnnotationDrawModify(
				int barIndex, string barAnnotationId, string barAnnotationText,
				Font font, Color colorForeground, Color colorBackground, bool aboveBar = true, 
				int verticalPadding = 5, bool reportDidntChangeStatus = false) {
			if (this.ChartShadow == null) return;
			this.ChartShadow.BarAnnotationDrawModify(barIndex, barAnnotationId, barAnnotationText,
				font, colorForeground, colorBackground, aboveBar, 
				verticalPadding, reportDidntChangeStatus);
		}
		public OnChartObjectOperationStatus ChartConditional_lineDrawModify(
				string id, int barStart, double priceStart, int barEnd, double priceEnd,
				Color color, int width, bool debugParametersDidntChange = false) {
			if (this.ChartShadow == null) return OnChartObjectOperationStatus.OnChartObjectJustCreated;
			return this.ChartShadow.LineDrawModify(id, barStart, priceStart, barEnd, priceEnd,
				color, width, debugParametersDidntChange);
		}
		public void ChartConditional_barBackgroundSet(int barsIndex, Color colorBg) {
			if (this.ChartShadow == null) return;
			this.ChartShadow.BarBackgroundSet(barsIndex, colorBg);
		}
		public Color ChartConditional_barBackgroundGet(int barIndex) {
			if (this.ChartShadow == null) return Color.Empty;
			return this.ChartShadow.BarBackgroundGet(barIndex);
		}
		public bool ChartConditional_barForegroundSet(int barIndex, Color colorFg) {
			if (this.ChartShadow == null) return true;
			return this.ChartShadow.BarForegroundSet(barIndex, colorFg);
		}
		public Color ChartConditional_barForegroundGet(int barIndex) {
			if (this.ChartShadow == null) return Color.Empty;
			return this.ChartShadow.BarForegroundGet(barIndex);
		}
		public OnChartObjectOperationStatus ChartConditional_chartLabelDrawOnNextLineModify(
				string labelId, string labelText,
				Font font, Color colorFore, Color colorBack) {
			if (this.ChartShadow == null) return OnChartObjectOperationStatus.OnChartObjectJustCreated;
			return this.ChartShadow.ChartLabelDrawOnNextLineModify(labelId, labelText, font, colorFore, colorBack);
		}
		public HostPanelForIndicator ChartConditional_hostPanelForIndicatorGet(Indicator indicatorInstance) {
			if (this.ChartShadow == null) return null;
			return this.ChartShadow.HostPanelForIndicatorGet(indicatorInstance);
		}
	}
}
