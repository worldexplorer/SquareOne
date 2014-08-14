using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;

namespace Sq1.Charting {
	public partial class ChartControl : ChartShadow {
		public Bars Bars { get; private set; }
		public bool BarsEmpty { get { return this.Bars == null || this.Bars.Count == 0; } }
		public bool BarsNotEmpty { get { return this.Bars != null && this.Bars.Count > 0; } }
		public int BarsDecimalsPrice { get { return (this.Bars.SymbolInfo != null) ? this.Bars.SymbolInfo.DecimalsPrice : 5; } }
		public int BarsDecimalsVolume { get { return (this.Bars.SymbolInfo != null) ? this.Bars.SymbolInfo.DecimalsVolume : 0; } }
		List<PanelNamedFolding> panelsFolding;
		public bool RangeBarCollapsed {
			get { return this.splitContainerChartVsRange.Panel2Collapsed; }
			set { this.splitContainerChartVsRange.Panel2Collapsed = value; }
		}

		// all cached variables must be calculated once per frame (once per ChartControl.PaintBackgroundWholeSurfaceBarsNotEmpty)
	 	public int GutterRightWidth_cached = -1;
		public int GutterBottomHeight_cached = -1;
		public ChartSettings ChartSettings;
		public ScriptExecutorObjects ScriptExecutorObjects;

		public int HeightMinusBottomHscrollbar { get { return base.Height - this.hScrollBar.Height; } }

		public ChartControl() {
			this.ChartSettings = new ChartSettings(); // was a component, used at InitializeComponent() (to draw SampleBars)
			this.ScriptExecutorObjects = new ScriptExecutorObjects();
			InitializeComponent();
			//if (this.ChartSettings == null) 

			this.AutoScroll = false;
			//this.HScroll = true;
			this.hScrollBar.SmallChange = this.ChartSettings.ScrollNBarsPerOneKeyPress;

			panelsFolding = new List<PanelNamedFolding>();
			panelsFolding.Add(this.panelPrice);
			panelsFolding.Add(this.panelVolume);
			
			this.panelPrice.Initialize(this);
			this.panelVolume.Initialize(this);

			if (base.DesignMode == false) return;
			//this.chartRenderer.Initialize(this);
			BarScaleInterval chartShouldntCare = new BarScaleInterval(BarScale.Minute, 5);
			//REFLECTION_FAILS_FOR_DESIGNER BarsBasic.GenerateRandom(chartShouldntCare)
			//this.Initialize(BarsBasic.GenerateRandom(chartShouldntCare));
			Bars generated = new Bars("RANDOM", chartShouldntCare, "test-ChartControl-DesignMode");
			generated.GenerateAppend();
			this.Initialize(generated);
		}
		public void Initialize(Bars barsNotNull) {
			this.barEventsDetach();
			this.Bars = barsNotNull;
			if (this.BarsNotEmpty) {
				this.barEventsAttach();
				this.SyncHorizontalScrollToBarsCount();
				//this.hScrollBar.ValueCurrent = this.hScrollBar.Maximum;	// I just sync'ed this.hScrollBar.Maximum = this.Bars.Count - 1
				// after I reduced BarRange{500bars => 100bars} in MainForm, don't set this.hScrollBar.Value here, I'll invoke ScrollToLastBarRight() upstack
				if (this.ChartSettings.ScrollPositionAtBarIndex >= this.hScrollBar.Minimum && this.ChartSettings.ScrollPositionAtBarIndex <= this.hScrollBar.Maximum) {
					// I'm here 1) at ChartControl startup; 2) after I changed BarRange in MainForm 
					this.hScrollBar.Value = this.ChartSettings.ScrollPositionAtBarIndex;
				} else {
					string msg = "HSCROLL_POSITION_VALUE_OUT_OF_RANGE; fix deserialization upstack";

				}
				foreach (PanelNamedFolding panelFolding in this.panelsFolding) {	// at least PanelPrice and PanelVolume
					panelFolding.InitializeWithNonEmptyBars(this);
				}
			}
			this.InvalidateAllPanelsFolding();
		}
		protected override void OnResize(EventArgs e) {
			if (this.ScrollLargeChange <= 0) {
				//Debugger.Break();	// HAPPENS_WHEN_WINDOW_IS_MINIMIZED... how to disable any OnPaint when app isn't visible?...
				return;
			}
		    this.hScrollBar.LargeChange = this.ScrollLargeChange;
		    base.OnResize(e);	// will invoke UserControlDoubleBuffered.OnResize() if you inherited so here you are DoubleBuffer-safe
		}
		public void SyncHorizontalScrollToBarsCount() {
			// this.HorizontalScroll represents the scrolling window for the content, useful in UserControl.Autoscroll when an innerPanel is wider or has Top|Left < 0 
			// this.hScrollBar represents a sensor which accepts user clicks on a visual surface
			// I'm not using Panel.HorizontalScroll because I'll never have anything "inner" larger; I'm painting the sliding bar window on Graphics
			// I don't use the concept of scrollable content; I'm gonna use this.hScrollBar everywhere because it produces events
			if (this.hScrollBar.Maximum == this.Bars.Count - 1) return;
			this.hScrollBar.Minimum = 0;						// index of first available Bar in this.Bars 
			this.hScrollBar.Maximum = this.Bars.Count - 1;		// index of  last available Bar in this.Bars
		}
		public void InvalidateAllPanelsFolding() {
			if (base.InvokeRequired) {
				base.BeginInvoke(new MethodInvoker(this.InvalidateAllPanelsFolding));
				return;
			}
			this.hScrollBar.Minimum = this.BarsCanFitForCurrentWidth;
			foreach (PanelNamedFolding panelFolding in this.panelsFolding) {
				panelFolding.Invalidate();
			}
			this.TooltipPriceHide();
			this.TooltipPositionHide();
		}
		private void scrollToBarSafely(int bar) {
			if (bar > this.hScrollBar.Maximum) bar = this.hScrollBar.Maximum;
			if (bar < this.hScrollBar.Minimum) bar = this.hScrollBar.Minimum;
			this.ChartSettings.ScrollPositionAtBarIndex = bar;
			this.RaiseChartSettingsChangedContainerShouldSerialize();
			this.hScrollBar.Value = bar;
			this.InvalidateAllPanelsFolding();
		}
		public void ScrollOneBarLeftAtKeyPressRate() {
			this.scrollToBarSafely(this.hScrollBar.Value - this.ChartSettings.ScrollNBarsPerOneKeyPress);
		}
		public void ScrollOneBarRightAtKeyPressRate() {
			this.scrollToBarSafely(this.hScrollBar.Value + this.ChartSettings.ScrollNBarsPerOneKeyPress);
		}
		public void DragNBarsLeft() {
			this.scrollToBarSafely(this.hScrollBar.Value - this.ChartSettings.ScrollNBarsPerOneDragMouseEvent);
		}
		public void DragNBarsRight() {
			this.scrollToBarSafely(this.hScrollBar.Value + this.ChartSettings.ScrollNBarsPerOneDragMouseEvent);
		}
		public void ScrollOnePageLeft() {
			this.scrollToBarSafely(this.hScrollBar.Value - this.hScrollBar.LargeChange);
		}
		public void ScrollOnePageRight() {
			this.scrollToBarSafely(this.hScrollBar.Value + this.hScrollBar.LargeChange);
		}
		public void ScrollToLastBarRight() {
			this.scrollToBarSafely(this.hScrollBar.Maximum);
		}
		public void ScrollToFirstBarLeft() {
			this.scrollToBarSafely(this.hScrollBar.Minimum);
		}
		void hScrollBar_Scroll(object sender, ScrollEventArgs scrollEventArgs) {
			if (this.Bars == null) return;
			
			if (scrollEventArgs.Type == ScrollEventType.EndScroll && scrollEventArgs.NewValue > this.Bars.Count - 1) {
				scrollEventArgs.NewValue = this.Bars.Count - 1;
			}
			this.ChartSettings.ScrollPositionAtBarIndex = scrollEventArgs.NewValue;
			this.RaiseChartSettingsChangedContainerShouldSerialize();
			this.hScrollBar.Value = scrollEventArgs.NewValue;	// USE_DEBUGGER_SETTING_SENSOR_TO_THE_VALUE_IT_HAS_JUST_NOTIFIED_YOU_MAKES_SENSE
			this.InvalidateAllPanelsFolding();
		}
		
		protected override void OnMouseMove(MouseEventArgs e) {
			// it looks like parent should get mouse updates from the Panels?...
			int a = 1;
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			if (e.Delta == 0) return;
			if (e.Delta > 0) {
				this.ScrollOnePageLeft();
			} else {
				this.ScrollOnePageRight();
			}
		}
		void barEventsAttach() {
			if (this.Bars == null) return; 
			this.Bars.BarStaticAdded			+= new EventHandler<BarEventArgs>(ChartControl_BarAddedUpdated_ShouldTriggerRepaint);		// quite useless since I don't plan to append-statically to displayed-bars 
			this.Bars.BarStreamingAdded			+= new EventHandler<BarEventArgs>(ChartControl_BarAddedUpdated_ShouldTriggerRepaint); 
			this.Bars.BarStreamingUpdatedMerged	+= new EventHandler<BarEventArgs>(ChartControl_BarAddedUpdated_ShouldTriggerRepaint);  
		}
		void barEventsDetach() {
			if (this.Bars == null) return; 
			this.Bars.BarStreamingUpdatedMerged -= new EventHandler<BarEventArgs>(ChartControl_BarAddedUpdated_ShouldTriggerRepaint);  
			this.Bars.BarStreamingAdded			-= new EventHandler<BarEventArgs>(ChartControl_BarAddedUpdated_ShouldTriggerRepaint);  
			this.Bars.BarStaticAdded			-= new EventHandler<BarEventArgs>(ChartControl_BarAddedUpdated_ShouldTriggerRepaint); 
		}
		void ChartControl_BarAddedUpdated_ShouldTriggerRepaint(object sender, BarEventArgs e) {
			// if I was designing events for WinForms, I would switch to GUI thread automatically
//			if (InvokeRequired) {
//				BeginInvoke(new MethodInvoker(UpdateHorizontalScrollMaximumAfterBarAdd));
//				return;
//			}
			if (this.VisibleBarRight != this.Bars.Count) return;	// move slider if only last bar is visible 
			this.SyncHorizontalScrollToBarsCount();
			this.InvalidateAllPanelsFolding();
			this.RangeBar.Invalidate();
		}
		#region IsInputKey is a filter OnKeyDown should go together
		protected override bool IsInputKey(Keys keyData) {
			switch (keyData) {
				case Keys.Right:
				case Keys.Left:
				case Keys.Up:
				case Keys.Down:
					return true;
//				case Keys.Shift | Keys.Right:
//				case Keys.Shift | Keys.Left:
//				case Keys.Shift | Keys.Up:
//				case Keys.Shift | Keys.Down:
//					return true;
			}
			return base.IsInputKey(keyData);
		}
		protected override void OnKeyDown(KeyEventArgs keyEventArgs) {
			if (this.BarsEmpty) return;
			switch (keyEventArgs.KeyCode) {
				case Keys.Up:
					this.BarWidthIncrementAtKeyPressRate();
					break;
				case Keys.Down:
					this.BarWidthDecrementAtKeyPressRate();
					break;
				case Keys.Left:
					this.ScrollOneBarLeftAtKeyPressRate();
					break;
				case Keys.Right:
					this.ScrollOneBarRightAtKeyPressRate();
					break;
				case Keys.Home:
					this.scrollToBarSafely(0);
					break;
				case Keys.End:
					this.scrollToBarSafely(this.Bars.Count - 1);
					break;
				case Keys.PageDown:
					this.ScrollOnePageRight();
					break;
				case Keys.PageUp:
					this.ScrollOnePageLeft();
					break;
			}
			base.OnKeyDown(keyEventArgs);
		}
		#endregion
		
		public void BarWidthIncrementAtKeyPressRate() {
			if (this.ChartSettings.BarWidthIncludingPadding >= 50) return; 
			this.ChartSettings.BarWidthIncludingPadding += this.ChartSettings.SqueezeHorizontalKeyOnePressReceivedToOneStep;
			this.InvalidateAllPanelsFolding();
			base.RaiseChartSettingsChangedContainerShouldSerialize();
		}
		public void BarWidthDecrementAtKeyPressRate() {
			if (this.ChartSettings.BarWidthIncludingPadding <= this.ChartSettings.SqueezeHorizontalKeyOnePressReceivedToOneStep) return;	// <= since BarWidth mustn't be zero (ZeroDivException)
			this.ChartSettings.BarWidthIncludingPadding -= this.ChartSettings.SqueezeHorizontalKeyOnePressReceivedToOneStep;
			this.InvalidateAllPanelsFolding();
			base.RaiseChartSettingsChangedContainerShouldSerialize();
		}
		public void DragDownSqueeze() {
			this.ChartSettings.SqueezeVerticalPaddingPx += this.ChartSettings.SqueezeVerticalPaddingStep;
//			if (this.ChartSettings.BarWidthIncludingPadding >= this.ChartSettings.SqueezeHorizontalStep) {
//				this.BarWidthDecrement();
//				return;
//			}
			this.panelPrice.Invalidate();
			base.RaiseChartSettingsChangedContainerShouldSerialize();
		}
		public void DragUpUnsqueeze() {
			if (this.ChartSettings.SqueezeVerticalPaddingPx < this.ChartSettings.SqueezeVerticalPaddingStep) return;
			this.ChartSettings.SqueezeVerticalPaddingPx -= this.ChartSettings.SqueezeVerticalPaddingStep;
//			this.BarWidthIncrement();
			this.panelPrice.Invalidate();
			base.RaiseChartSettingsChangedContainerShouldSerialize();
		}
		public void TooltipPriceShowAlone(Bar barToPopulate, Rectangle barWithShadowsRectangle) {
			if (this.ChartSettings.TooltipPriceShow == false) return;
			// MouseX will never go over tooltip => PanelNamedFolding.OnMouseLeave() never invoked
			int awayFromBarXpx = this.ChartSettings.TooltipsPaddingFromBarLeftRightEdgesToAvoidMouseLeave;
			int x = barWithShadowsRectangle.Left - this.tooltipPrice.Width - awayFromBarXpx;
			if (x < 0) x = barWithShadowsRectangle.Right + awayFromBarXpx;
			int y = barWithShadowsRectangle.Top - this.ChartSettings.TooltipBordersMarginToKeepBordersVisible;
			if (y < 0) y = 0;
			if (y + this.tooltipPrice.Height > this.HeightMinusBottomHscrollbar)
				y = this.HeightMinusBottomHscrollbar - this.tooltipPrice.Height - this.ChartSettings.TooltipBordersMarginToKeepBordersVisible;
			this.tooltipPriceShowXY(barToPopulate, x, y);
		}
		public void TooltipPositionAndPriceShow(AlertArrow alertArrow, Bar barToPopulate, Rectangle rectangleYarrowXbar) {
			if (this.ChartSettings.TooltipPriceShow == false) return;
			// MouseX will never go over tooltip => PanelNamedFolding.OnMouseLeave() never invoked ???
			int awayFromBarXpx = this.ChartSettings.TooltipsPaddingFromBarLeftRightEdgesToAvoidMouseLeave;
			int xPosition	= -1;
			int xPrice		= -1;

			int twoTooltipsVerticalDistance = 4;
			int twoTooltipsCombinedHeight = this.tooltipPosition.Height + twoTooltipsVerticalDistance + this.tooltipPrice.Height;

			if (alertArrow.ArrowIsForPositionEntry) {
				xPosition	= rectangleYarrowXbar.Left - this.tooltipPosition.Width - awayFromBarXpx;
				xPrice		= rectangleYarrowXbar.Left - this.tooltipPrice.Width - awayFromBarXpx;
// LET_POSITION_TOOLTIP_GO_BEHIND_LEFT_EDGE_AND_PARTIALLY_OVERLAP_I_NEED_POSITION_LINE_TO_BE_FULLY_VISIBLE_TO_HIGHLIGHT
//				if (xPrice < 0)	xPrice = rectangleYarrowXbar.Right + awayFromBarXpx;
//				if (xPosition < 0) {	// positionTooltip is wider, dont squeeze priceTooltip but take the same side as the big brother
//					xPosition	= rectangleYarrowXbar.Right + awayFromBarXpx;
//					xPrice		= rectangleYarrowXbar.Right + awayFromBarXpx;
//				}
			} else {
				xPosition	= rectangleYarrowXbar.Right + awayFromBarXpx;
				xPrice		= xPosition;
			}

			int yPrice = rectangleYarrowXbar.Top - twoTooltipsCombinedHeight / 2;
			if (yPrice <= 0) yPrice = this.ChartSettings.TooltipBordersMarginToKeepBordersVisible;
			if (yPrice + twoTooltipsCombinedHeight > this.HeightMinusBottomHscrollbar) {
				yPrice = this.HeightMinusBottomHscrollbar - twoTooltipsCombinedHeight - this.ChartSettings.TooltipBordersMarginToKeepBordersVisible;
				if (yPrice <= 0) yPrice = this.ChartSettings.TooltipBordersMarginToKeepBordersVisible;
			}
			int yPosition = yPrice + this.tooltipPrice.Height + twoTooltipsVerticalDistance;
			
			this.tooltipPriceShowXY(barToPopulate, xPrice, yPrice);
			this.tooltipPositionShowXY(alertArrow, xPosition, yPosition);
		}

		private void tooltipPositionShowXY(AlertArrow alertArrowToPopup, int xPosition, int yPosition) {
			Point newLocation = new Point(xPosition, yPosition);
			//DOESNT_SHOWUP_MOUSE_BETWEEN_ARROW_AND_BAR
			//if (this.tooltipPosition.Location == newLocation) {	//Point is a structure with Equals() overriden => we are safe to compare
			//	this.tooltipPrice.Visible = true;
			//	return;	
			//}

			this.tooltipPosition.PopulateTooltip(alertArrowToPopup);
			//this.tooltipPosition.Capture = false;
			this.tooltipPosition.Location = newLocation;
			this.tooltipPosition.Visible = true;
		}
		private void tooltipPriceShowXY(Bar barToPopulate, int x, int y) {
			Point newLocation = new Point(x, y);
			//DOESNT_SHOWUP_MOUSE_BETWEEN_ARROW_AND_BAR
			if (this.tooltipPrice.Location == newLocation) {	//REMOVES_FLICKERING Point is a structure with Equals() overriden => we are safe to compare
				this.tooltipPrice.Visible = true;
				return;
			}

			//if (barToPopulate.IsStreamingBar)...
			List<Alert> tooltipAlertsForBar = new List<Alert>();
			//			Dictionary<int, List<Alert>> alertsForBarsFromExecution = this.executor.ExecutionDataSnapshot
			//				.AlertsPendingHistorySafeCopyForRenderer(mouseBar, mouseBar);
			//			if (alertsForBarsFromExecution.ContainsKey(mouseBar)) tooltipAlertsForBar = alertsForBarsFromExecution[mouseBar];
			Dictionary<string, Indicator> indicators = this.ScriptExecutorObjects.Indicators;
			this.tooltipPrice.PopulateTooltip(barToPopulate, indicators, tooltipAlertsForBar);
			//this.tooltipPrice.Capture = false;
			this.tooltipPrice.Location = newLocation;
			this.tooltipPrice.Visible = true;
		}
		public bool TooltipPriceHide() {
			bool wasInvalidated = false;
			if (this.ChartSettings.TooltipPriceShow == false) return wasInvalidated;
			if (this.tooltipPrice.Visible == false) return wasInvalidated;	// DO_NOT_DELETE__HERE_WE_BREAK_INFINITE_LOOP
			this.tooltipPrice.Visible = false;
			wasInvalidated = true;
			this.InvalidateAllPanelsFolding();
			return wasInvalidated;
		}
		public bool TooltipPositionHide() {
			bool wasInvalidated = false;
			if (this.ChartSettings.TooltipPositionShow == false) return wasInvalidated;
			if (this.tooltipPosition.Visible == false) return wasInvalidated;	// DO_NOT_DELETE__HERE_WE_BREAK_INFINITE_LOOP
			this.tooltipPosition.Visible = false;
			wasInvalidated = true;
			this.InvalidateAllPanelsFolding();
			return wasInvalidated;
		}
		public override void PositionsClearBacktestStarting() {
			this.ScriptExecutorObjects.PositionsClearBacktestStarting();
		}
		public override void PositionsBacktestAdd(List<Position> positionsMaster) {
			this.ScriptExecutorObjects.PositionArrowsBacktestAdd(positionsMaster);
		}
		public override void PositionsRealtimeAdd(ReporterPokeUnit pokeUnit) {
			this.ScriptExecutorObjects.PositionArrowsRealtimeAdd(pokeUnit);
		}
		public override void PendingHistoryClearBacktestStarting() {
			this.ScriptExecutorObjects.PendingHistoryClearBacktestStarting();
		}
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
		
		public void PropagateSettingSplitterDistancePriceVsVolume() {
			if (this.ChartSettings.PriceVsVolumeSplitterDistance == 0) {
				this.ChartSettings.PriceVsVolumeSplitterDistance = this.splitContainerPriceVsVolume.SplitterDistance; 
			} else {
				this.splitContainerPriceVsVolume.SplitterDistance = this.ChartSettings.PriceVsVolumeSplitterDistance;
			}
		}
		void splitContainerPriceVsVolume_SplitterMoved(object sender, SplitterEventArgs e) {
			//v1 this.ChartSettings.PriceVsVolumeSplitterDistance = this.splitContainerPriceVsVolume.SplitterDistance;
			//v2
			if (base.DesignMode) return;
			if (this.ChartSettings == null) return;	// may be redundant
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			//if (this.ChartSettings.PriceVsVolumeSplitterDistance == e.SplitY) return;
			//this.ChartSettings.PriceVsVolumeSplitterDistance = e.SplitY;
			if (this.ChartSettings.PriceVsVolumeSplitterDistance == this.splitContainerPriceVsVolume.SplitterDistance) return;
			this.ChartSettings.PriceVsVolumeSplitterDistance = this.splitContainerPriceVsVolume.SplitterDistance;
			this.RaiseChartSettingsChangedContainerShouldSerialize();
		}
		public AlertArrow TooltipPositionShownForAlertArrow { get {
				AlertArrow ret = null;
				if (this.tooltipPosition.Visible == false) return ret;
				ret = this.tooltipPosition.AlertArrow;
				return ret;
			} }
		public int TooltipPositionShownForBarIndex { get {
				int ret = -1;
				AlertArrow arrowNullIfNotDisplayed = this.TooltipPositionShownForAlertArrow;
				if (arrowNullIfNotDisplayed == null) return ret;
				ret = arrowNullIfNotDisplayed.BarIndexFilled;
				return ret;
			} }
	}
}
//		public bool TooltipPriceShownAndMouseOverIt { get { return this.TooltipPrice.Visible && this.TooltipPrice.Capture; } }
//		public bool TooltipPositionShownAndMouseOverIt { get { return this.tooltipPosition.Visible && this.tooltipPosition.Capture; } }
