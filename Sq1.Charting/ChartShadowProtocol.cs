using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Charting.OnChart;
using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.Charting.OnChart;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;

namespace Sq1.Charting {
	public partial class ChartControl : ChartShadow {
		public void ActivateParentForm() {
			Form parent = this.Parent as Form;
			if (parent != null) {
				parent.Activate();
			} else {
				string msg = "Chart::ActivateParentForm() chart[" + this + "].Parent is not a Form, can not Activate()";
				Assembler.PopupException(msg);
			}
		}
		public override void SelectAlert(Alert alert) {
			this.ActivateParentForm();
			if (alert == null) {
				string msg = "DONT_PASS_ALERT=NULL_TO_CHART_SHADOW ChartControl.SelectAlert()";
				Assembler.PopupException(msg);
				#if DEBUG
				Debugger.Break();
				#endif
				return;
			}
			if (alert.PlacedBar == null) {
				string msg = "DONT_PASS_ALERT.PLACEDBAR=NULL_TO_CHART_SHADOW ChartControl.SelectAlert()";
				Assembler.PopupException(msg);
				#if DEBUG
				Debugger.Break();
				#endif
				return;
			}
			if (alert.PlacedBar.HasParentBars == false) {
				string msg = "DONT_PASS_ALERT.PLACEDBAR.HASPARENTBARS=FALSE_TO_CHART_SHADOW ChartControl.SelectAlert()";
				Assembler.PopupException(msg);
				#if DEBUG
				Debugger.Break();
				#endif
				return;
			}
			int bar = alert.PlacedBar.ParentBarsIndex;		
			this.scrollToBarSafely(bar);
		}		
		public override void SelectPosition(Position position) {
			this.ActivateParentForm();
			if (position == null) {
				string msg = "DONT_PASS_NULL_POSITION_TO_CHART_SHADOW ChartControl.SelectPosition()";
				Assembler.PopupException(msg);
				#if DEBUG
				Debugger.Break();
				#endif
				return;
			}
			int bar = (position.ExitAlert != null)
				? position.ExitAlert.PlacedBar.ParentBarsIndex
				: position.EntryAlert.PlacedBar.ParentBarsIndex;
			this.scrollToBarSafely(bar);
		}
		public override void ClearAllScriptObjectsBeforeBacktest() {
			this.ScriptExecutorObjects.ClearAllBeforeBacktest();
		}
		public override void PositionsBacktestAdd(List<Position> positionsMaster) {
			this.ScriptExecutorObjects.PositionArrowsBacktestAdd(positionsMaster);
		}
		public override void PositionsRealtimeAdd(ReporterPokeUnit pokeUnit) {
			this.ScriptExecutorObjects.PositionArrowsRealtimeAdd(pokeUnit);
		}
//		public override void PendingHistoryClearBacktestStarting() {
//			this.ScriptExecutorObjects.PendingHistoryClearBacktestStarting();
//		}
		public override void PendingHistoryBacktestAdd(Dictionary<int, List<Alert>> alertsPendingHistorySafeCopy) {
			this.ScriptExecutorObjects.PendingHistoryBacktestAdd(alertsPendingHistorySafeCopy);
		}
		public override void PendingRealtimeAdd(ReporterPokeUnit pokeUnit) {
			this.ScriptExecutorObjects.PendingRealtimeAdd(pokeUnit);
		}
		public override HostPanelForIndicator GetHostPanelForIndicator(Indicator indicator) {
			switch (indicator.ChartPanelType) {
				case ChartPanelType.PanelPrice: return this.panelPrice;
				case ChartPanelType.PanelVolume: return this.panelVolume;
				case ChartPanelType.PanelIndicatorSingle:
				case ChartPanelType.PanelIndicatorMultiple: 
				default:
					throw new NotImplementedException();
			}
		}
		public override void SetIndicators(Dictionary<string, Indicator> indicators) {
			this.ScriptExecutorObjects.SetIndicators(indicators);
		}
		public override OnChartObjectOperationStatus LineDrawModify (
					// parameters could be a class, but I didnt introduce class Sq1.Core.Line because:
					// 1) I don't want Sq1.Charting to depend on Sq1.Core too much
					// 		so that other developers could take only Sq1.ChartShadow and adapt it for another trading application
					// 2) I'd be tempted to inherit Sq1.Core.Line in Sq1.Charting.Line and both would carry ambigous functionality
					// 3) I don't need classes as grouped parameters; I intoduce a class only when:
					//		3.1) grouped parameters are used in Lists or Dictionaries;
					//		3.1) grouped parameters have a STATE and class is a container for STATE related to group of members;
					//		here I just pass the parameters for constructor; the STATE is kept in Sq1.Charting.Line
					//		and Sq1.Core shouldn't have access to the on-chart-drawn objects;
					// 4) if the command to CreateOrModify succeeds => you have ChartOperationStatus; if it fails you have an Assembler.PopupException()
 					//		nothing else I want to do in Script with LineDrawModify() => KISS
					string lineId, int barStart, double priceStart, int barEnd, double priceEnd,
					Color color, int width) {
			
			// OnChartLine = {Chart's implementation detail}, that's why it's in ChartControl.dll and is not exposed to the Script;
			// OnChartLine.Status = {AddOrModify's output}, so that the Script could base decisions upon status of the operation done to OnChartLine
			// HACK current implementation is based on responsibilities-s-DLLs distribution which might not be optimal; returning only Status should be enough
			// TODO define what you want and how you distribute roles among DLLs before refactoring
			OnChartLine line = null;
			try {
				line = this.ScriptExecutorObjects.LineAddOrModify(lineId, barStart, priceStart, barEnd, priceEnd, color, width);
			} catch (Exception ex) {
				if (line != null) {
					Assembler.PopupException(line.ToString() + " //LineAddOrModify()");
				} else {
					string msg = "EXECUTOROBJECTS_DIDNT_EVEN_RETURN_LINE_SOMETHING_SERIOUS";
					Assembler.PopupException(msg + " //LineAddOrModify()");
				}
				return OnChartObjectOperationStatus.Unknown;
			}
			return line.Status;
		}
		public override bool BarBackgroundSet(int barIndex, Color color) {
			bool ret = false;
			try {
				ret = this.ScriptExecutorObjects.BarBackgroundSet(barIndex, color);
			} catch (Exception ex) {
				string msg = "EXECUTOROBJECTS_COULDNT_FIND_BAR";
				Assembler.PopupException(msg + " //BarBackgroundSet()");
			}
			return ret;
		}
		public override Color BarBackgroundGet(int barIndex) {
			return this.ScriptExecutorObjects.BarBackgroundGet(barIndex);
		}
		public override bool BarForegroundSet(int barIndex, Color color) {
			bool ret = false;
			try {
				ret = this.ScriptExecutorObjects.BarForegroundSet(barIndex, color);
			} catch (Exception ex) {
				string msg = "EXECUTOROBJECTS_COULDNT_FIND_BAR";
				Assembler.PopupException(msg + " //LineAddOrModify()");
			}
			return ret;
		}
		public override Color BarForegroundGet(int barIndex) {
			return this.ScriptExecutorObjects.BarForegroundGet(barIndex);
		}
		public override OnChartObjectOperationStatus ChartLabelDrawOnNextLineModify(
				string labelId, string labelText, Font font, Color colorFore, Color colorBack) {
			OnChartLabel label = null;
			try {
				label = this.ScriptExecutorObjects.ChartLabelAddOrModify(labelId, labelText, font, colorFore, colorBack);
			} catch (Exception ex) {
				if (label != null) {
					Assembler.PopupException(label.ToString() + " //LabelDrawMofify()");
				} else {
					string msg = "EXECUTOROBJECTS_DIDNT_EVEN_RETURN_LINE_SOMETHING_SERIOUS";
					Assembler.PopupException(msg + " //LabelDrawMofify()");
				}
				return OnChartObjectOperationStatus.Unknown;
			}
			return label.Status;
		}
		public override OnChartObjectOperationStatus BarAnnotationDrawModify(
				int barIndex, string barAnnotationId, string barAnnotationText,
				Font font, Color colorFore, Color colorBack, bool aboveBar = true, bool debugStatus = false) {
			OnChartBarAnnotation barAnnotation = null;
			try {
				barAnnotation = this.ScriptExecutorObjects.BarAnnotationAddOrModify(
					barIndex, barAnnotationId, barAnnotationText, font, colorFore, colorBack, aboveBar, debugStatus);
			} catch (Exception ex) {
				if (barAnnotation != null) {
					Assembler.PopupException(barAnnotation.ToString() + " //BarAnnotationDrawModify()");
				} else {
					string msg = "EXECUTOROBJECTS_DIDNT_EVEN_RETURN_LINE_SOMETHING_SERIOUS";
					Assembler.PopupException(msg + " //BarAnnotationDrawMofify()");
				}
				return OnChartObjectOperationStatus.Unknown;
			}
			return barAnnotation.Status;
		}
	}
}