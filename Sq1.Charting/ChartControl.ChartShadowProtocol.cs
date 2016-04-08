using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.Charting.OnChart;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;
using Sq1.Core.Streaming;

using Sq1.Charting.OnChart;

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
		public override bool SelectPosition(Position position) {
			bool tooltipPositionShown = false;
			
			this.ActivateParentForm();
			if (position == null) {
				string msg = "DONT_PASS_NULL_POSITION_TO_CHART_SHADOW ChartControl.SelectPosition()";
				Assembler.PopupException(msg);
				#if DEBUG
				Debugger.Break();
				#endif
				return tooltipPositionShown;
			}
			Bar barEntryOrExit = (position.ExitAlert != null) ? position.ExitAlert.PlacedBar : position.EntryAlert.PlacedBar;
			int barIndex = barEntryOrExit.ParentBarsIndex;
			//v1
			this.scrollToBarSafely(barIndex);
			//v2 TODO make whole position fit upon doubleclick
			//this.scrollToFitPositionSafely(position);
			
			
			// COPYPASTE_SOURCE=PanelNamedFolding.handleTooltipsPositionAndPrice()_DESTINATION=ChartShadowProtocol.SelectPosition() begin
			if (this.ChartSettings.TooltipPositionShow == false) return tooltipPositionShown;

			if (barIndex < this.VisibleBarLeft) return tooltipPositionShown;
			if (barIndex > this.VisibleBarRight) return tooltipPositionShown;	//Debugger.Break();

			Dictionary<int, List<AlertArrow>> alertArrowsListByBar = this.ExecutorObjects_frozenForRendering.AlertArrowsListByBar;
			if (alertArrowsListByBar.ContainsKey(barIndex) == false) {
				this.TooltipPositionHide();
				#if DEBUG
				//Debugger.Break();
				#endif
				return tooltipPositionShown;
			}

			List<AlertArrow> arrowsForBar = alertArrowsListByBar[barIndex];
			AlertArrow arrowFoundForPosition = null;
			foreach (AlertArrow arrow in arrowsForBar) {
				if (arrow.Position != position) continue;
				arrowFoundForPosition = arrow;
				break;
			}
			if (arrowFoundForPosition == null) {
				this.TooltipPositionHide();
				#if DEBUG
				//Debugger.Break();
				#endif
				return tooltipPositionShown;
			}
			Position positionToPopup = arrowFoundForPosition.Position;
			bool placeAtLeft = arrowFoundForPosition.ArrowIsForPositionEntry;

			if (this.PanelPrice == null) {
				#if DEBUG
				Debugger.Break();
				#endif
				string msg = "NEED_NON_NULL_PANEL_PRICE_TO_GET_X_OF_BAR this.panelPrice[" + this.PanelPrice + "]";
				return tooltipPositionShown;
			}
			
			int barX = this.PanelPrice.BarToX(barIndex);		// HACK NPE-vulnerable
			Rectangle rectangleYarrowXbar = new Rectangle();
			rectangleYarrowXbar.X		= barX;
			rectangleYarrowXbar.Width	= this.ChartSettings.BarWidthIncludingPadding;		//arrowFoundForMouse.Width;
			rectangleYarrowXbar.Y		= arrowFoundForPosition.Ytransient;
			rectangleYarrowXbar.Height	= arrowFoundForPosition.Height;

			this.TooltipPositionAndPriceShow(arrowFoundForPosition, barEntryOrExit, rectangleYarrowXbar);
			// COPYPASTE_SOURCE=PanelNamedFolding.handleTooltipsPositionAndPrice()_DESTINATION=ChartShadowProtocol.SelectPosition() end
			tooltipPositionShown = true;
			return tooltipPositionShown; 
		}
		public override void ClearAllScriptObjectsBeforeBacktest() {
			this.ExecutorObjects_frozenForRendering.ClearAllBeforeBacktest();
		}
		public override void PositionsBacktestAdd(List<Position> positionsMaster) {
			this.ExecutorObjects_frozenForRendering.PositionArrowsBacktestAdd(positionsMaster);
		}
		public override void PositionsRealtimeAdd(ReporterPokeUnit pokeUnit) {
			this.ExecutorObjects_frozenForRendering.PositionArrowsRealtimeAdd(pokeUnit);
		}
//		public override void PendingHistoryClearBacktestStarting() {
//			this.ScriptExecutorObjects.PendingHistoryClearBacktestStarting();
//		}
		public override void PendingHistoryBacktestAdd(Dictionary<int, AlertList> alertsPendingHistorySafeCopy) {
			this.ExecutorObjects_frozenForRendering.AlertsPlacedBacktestAdd(alertsPendingHistorySafeCopy);
		}
		public override void AlertsPlaced_addRealtime(List<Alert> alertsNewPlaced) {
			this.ExecutorObjects_frozenForRendering.AlertsPlacedRealtimeAdd(alertsNewPlaced);
		}
		public override void AlertsPendingStillNotFilledForBarAdd(int barIndex, List<Alert> alertsPendingAtCurrentBarSafeCopy) {
			this.ExecutorObjects_frozenForRendering.AlertsPendingStillNotFilledForBarAdd(barIndex, alertsPendingAtCurrentBarSafeCopy);
		}
		
		Dictionary<Indicator, PanelIndicator> PanelsByIndicator = new Dictionary<Indicator, PanelIndicator>();
		public override void HostPanelForIndicatorClear() {
			foreach (PanelIndicator panel in this.PanelsByIndicator.Values) {
				this.panelsForInvalidateAll_dontForgetIndicators.Remove(panel);
				this.multiSplitRowsVolumePrice.PanelRemove(panel);
			}
			this.PanelsByIndicator.Clear();
		}
		public override HostPanelForIndicator HostPanelForIndicatorGet(Indicator indicator) {
			bool needToReReadSplitterPositionsSinceIndicatorsWereAdded = false;
			switch (indicator.ChartPanelType) {
				case ChartPanelType.PanelPrice: return this.PanelPrice;
				case ChartPanelType.PanelVolume: return this.panelVolume;
				case ChartPanelType.PanelIndicatorSingle:
					PanelIndicator ret;
					if (this.PanelsByIndicator.ContainsKey(indicator) == false) {
						PanelIndicator panel = new PanelIndicator(indicator);
						
						panel.Initialize(this);		// WHICH_ONE_IS_APPROPRIATE??? ITS_LATE_INSTANTIATION_WE_MAY_ALREADY_HAVE_BARS_NOT_EMPTY
						if (this.Bars == null) {
							string msg = "DESERIALIZATION_RECREATES_LAYOUT_BARS_LOADING_FOLLOWS_LATER";
							panel.Initialize(this);
						} else {
							string msg = "I_COMPILED_SCRIPT_WHICH_ADDED_INDICATOR_PANEL";
							panel.InitializeWithNonEmptyBars(this);
						}
						
						this.PanelsByIndicator.Add(indicator, panel);
						this.panelsForInvalidateAll_dontForgetIndicators.Add(panel);
						this.multiSplitRowsVolumePrice.PanelAddSplitterCreateAdd(panel, true);		//, this.ChartSettings.MultiSplitterPropertiesByPanelName);
						needToReReadSplitterPositionsSinceIndicatorsWereAdded = true;
					}
					ret = this.PanelsByIndicator[indicator];
					return ret;
				case ChartPanelType.PanelIndicatorMultiple: 
				default:
					throw new NotImplementedException();
			}
		}
		public override void SetIndicators(Dictionary<string, Indicator> indicators) {
			this.ExecutorObjects_frozenForRendering.SetIndicators(indicators);
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
					Color color, int width, bool debugParametersDidntChange = false) {
			
			// OnChartLine = {Chart's implementation detail}, that's why it's in ChartControl.dll and is not exposed to the Script;
			// OnChartLine.Status = {AddOrModify's output}, so that the Script could base decisions upon status of the operation done to OnChartLine
			// HACK current implementation is based on responsibilities-vs-DLLs distribution which might not be optimal; returning only Status should be enough
			// TODO define what you want and how you distribute roles among DLLs before refactoring
			OnChartLine line = null;
			try {
				line = this.ExecutorObjects_frozenForRendering.LineAddOrModify(lineId, barStart, priceStart, barEnd, priceEnd, color, width, debugParametersDidntChange);
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
		public override bool BarBackgroundSet(int barIndex, Color colorBg) {
			bool ret = false;
			if (colorBg == Color.Empty) return ret; 
			try {
				ret = this.ExecutorObjects_frozenForRendering.BarBackgroundSet(barIndex, colorBg);
			} catch (Exception ex) {
				string msg = "EXECUTOROBJECTS_COULDNT_FIND_BAR";
				Assembler.PopupException(msg + " //BarBackgroundSet()");
			}
			return ret;
		}
		public override Color BarBackgroundGet(int barIndex) {
			return this.ExecutorObjects_frozenForRendering.BarBackgroundGet(barIndex);
		}
		public override bool BarForegroundSet(int barIndex, Color colorFg) {
			bool ret = false;
			try {
				ret = this.ExecutorObjects_frozenForRendering.BarForegroundSet(barIndex, colorFg);
			} catch (Exception ex) {
				string msg = "EXECUTOROBJECTS_COULDNT_FIND_BAR";
				Assembler.PopupException(msg + " //LineAddOrModify()");
			}
			return ret;
		}
		public override Color BarForegroundGet(int barIndex) {
			return this.ExecutorObjects_frozenForRendering.BarForegroundGet(barIndex);
		}
		public override OnChartObjectOperationStatus ChartLabelDrawOnNextLineModify(
				string labelId, string labelText, Font font, Color colorFore, Color colorBack) {
			OnChartObjectOperationStatus ret = OnChartObjectOperationStatus.Unknown;
			try {
				OnChartLabel label = this.ExecutorObjects_frozenForRendering.ChartLabelAddOrModify(labelId, labelText, font, colorFore, colorBack);
				ret = label.Status;
			} catch (Exception ex) {
				string msg = "DID_YOU_RUN_BACKTEST_TWICE??__MULTITHREADING_DUPLICATES_FOR_LABEL[" + labelId + "]";
				Assembler.PopupException(msg + " //LabelDrawMofify()", ex);
				return ret;
			}
			return ret;
		}

		public override OnChartObjectOperationStatus BarAnnotationDrawModify(
				int barIndex, string barAnnotationId, string barAnnotationText,
				Font font, Color colorForeground, Color colorBackground, bool aboveBar = true, 
				int verticalPadding = 5, bool popupParametersDidntChange = false) {
			OnChartBarAnnotation barAnnotation = null;
			try {
				if (colorBackground != Color.Empty) {
					colorBackground = Color.FromArgb(this.ChartSettings.BarsBackgroundTransparencyAlpha, colorBackground);
				}
				barAnnotation = this.ExecutorObjects_frozenForRendering.BarAnnotationAddOrModify(
					barIndex, barAnnotationId, barAnnotationText,
					font, colorForeground, colorBackground, aboveBar,
					verticalPadding, popupParametersDidntChange);
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
		public override void SyncBarsIdentDueToSymbolRename() {
			foreach (PanelBase panelFolding in this.panelsForInvalidateAll_dontForgetIndicators) {	// at least PanelPrice and PanelVolume
				panelFolding.InitializeWithNonEmptyBars(this);
				panelFolding.Invalidate();
			}
		}
		public override void PushQuote_LevelTwoFrozen_toExecutorObjects_fromStreamingDataSnapshot_triggerInvalidateAll() {
			if (this.Executor.BacktesterOrLivesimulator.ImRunningChartlessBacktesting) {
				string msg = "FOR_CHARTLESS_BACKTEST__THIS_CHART_SHOULD_HAVE_NOT_BEEN_SUBSCRIBED_TO__BARS_SIMULATING";
				Assembler.PopupException(msg, null, false);
				return;
			}

			// doing same thing from GUI thread at PanelLevel2.renderLevel2() got me even closer to realtime (after pausing a Livesim
			// and repainting Level2 whole thing was misplaced comparing to PanelPrice spread) but looked really random, not behind and not ahead;
			// but main reason is ConcurrentLocker was spitting messages (I dont remember what exactly but easy to move back to renderLevel2() and see)
			StreamingDataSnapshot snap = base.Bars.DataSource.StreamingAdapter.StreamingDataSnapshot;

			//v1 this.ScriptExecutorObjects.QuoteCurrent = this.Bars.QuoteLastClone_nullUnsafe;
			this.ExecutorObjects_frozenForRendering.QuoteCurrent = snap.GetQuoteCurrent_forSymbol_nullUnsafe(this.Bars.Symbol);
			this.ExecutorObjects_frozenForRendering.LevelTwo_frozen_forOnePaint =
				snap.GetLevelTwoFrozenSorted_forSymbol_nullUnsafe(this.Bars.Symbol,
					"CLONING_BIDS_ASKS_FOR_PAINTING_FOREGROUND_ON_PanelLevel2",
					"PanelLevel2");

			this.ExecutorObjectsReady_triggerRepaint__raiseOnBarStreamingUpdatedMerged_chartFormPrintsQuoteTimestamp();
		}
	}
}