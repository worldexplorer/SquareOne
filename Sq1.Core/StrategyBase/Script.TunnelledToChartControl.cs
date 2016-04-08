using System;
using System.Drawing;

using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Indicators;

using Sq1.Core.Charting.OnChart;

namespace Sq1.Core.StrategyBase {
	public abstract partial class Script {
		// REASON_TO_EXIST: tunnelled to ChartShadow through Executor; grouped and separated to share with for python integrators
		public void ChartConditionalBarAnnotationDrawModify(
				int barIndex, string barAnnotationId, string barAnnotationText,
				Font font, Color colorForeground, Color colorBackground, bool aboveBar = true, 
				int verticalPadding = 5, bool reportDidntChangeStatus = false) {
			this.Executor.ChartConditional_barAnnotationDrawModify(barIndex, barAnnotationId, barAnnotationText,
				font, colorForeground, colorBackground, aboveBar, 
				verticalPadding, reportDidntChangeStatus);
		}
		public OnChartObjectOperationStatus ChartConditionalLineDrawModify(
				string id, int barStart, double priceStart, int barEnd, double priceEnd,
				Color color, int width, bool debugParametersDidntChange = false) {
			return this.Executor.ChartConditional_lineDrawModify(id, barStart, priceStart, barEnd, priceEnd,
				color, width, debugParametersDidntChange);
		}
		public void ChartConditionalBarBackgroundSet(int barsIndex, Color colorBg) {
			this.Executor.ChartConditional_barBackgroundSet(barsIndex, colorBg);
		}

		public void ChartConditionalBarBackgroundSet(Bar bar, Color colorBg) {
			this.Executor.ChartConditional_barBackgroundSet(bar.ParentBarsIndex, colorBg);
		}
		public Color ChartConditionalBarBackgroundGet(Bar bar) {
			return this.Executor.ChartConditional_barBackgroundGet(bar.ParentBarsIndex);
		}
		public bool ChartConditionalBarForegroundSet(Bar bar, Color colorFg) {
			return this.Executor.ChartConditional_barForegroundSet(bar.ParentBarsIndex, colorFg);
		}
		public Color ChartConditionalBarForegroundGet(Bar bar) {
			return this.Executor.ChartConditional_barForegroundGet(bar.ParentBarsIndex);
		}

		public OnChartObjectOperationStatus ChartConditionalChartLabelDrawOnNextLineModify(
				string labelId, string labelText,
				Font font, Color colorFore, Color colorBack) {
			return this.Executor.ChartConditional_chartLabelDrawOnNextLineModify(labelId, labelText, font, colorFore, colorBack);
		}
		public HostPanelForIndicator ChartConditionalHostPanelForIndicatorGet(Indicator indicatorInstance) {
			return this.Executor.ChartConditional_hostPanelForIndicatorGet(indicatorInstance);
		}
	}
}
