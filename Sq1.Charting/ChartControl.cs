using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;
using Sq1.Charting.MultiSplit;

namespace Sq1.Charting {
	public partial class ChartControl {
		public Bars Bars { get; private set; }
		public bool BarsEmpty { get { return this.Bars == null || this.Bars.Count == 0; } }
		public bool BarsNotEmpty { get { return this.Bars != null && this.Bars.Count > 0; } }
		List<PanelBase> panels;
		public bool RangeBarCollapsed {
			get { return this.splitContainerChartVsRange.Panel2Collapsed; }
			set { this.splitContainerChartVsRange.Panel2Collapsed = value; }
			//Designer was complaining about NullReference; I was fixing NullPointer here....
			//get { return this.splitContainerChartVsRange == null ? false : this.splitContainerChartVsRange.Panel2Collapsed; }
			//set { if (this.splitContainerChartVsRange == null) return; this.splitContainerChartVsRange.Panel2Collapsed = value; }
		}

		// all cached variables must be calculated once per frame (once per ChartControl.PaintBackgroundWholeSurfaceBarsNotEmpty)
	 	public int GutterRightWidth_cached = -1;
		public int GutterBottomHeight_cached = -1;
		public ChartSettings ChartSettings;
		public ScriptExecutorObjects ScriptExecutorObjects;

		public int HeightMinusBottomHscrollbar { get { return base.Height - this.hScrollBar.Height; } }
		public int BarIndexMouseIsOverNow;

		public ChartControl() {
			this.ChartSettings = new ChartSettings(); // was a component, used at InitializeComponent() (to draw SampleBars)
			this.ScriptExecutorObjects = new ScriptExecutorObjects();

			InitializeComponent();
			//when previous line doesn't help and Designer still throws exceptions return;
			
			// moved here from Designer to Make ChartForm work is Designer (third button)  
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.panelVolume);
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.panelPrice);

			// doesn't live well in InitializeComponent(), designer barks 
			this.multiSplitContainer.OnSplitterMoveEnded += new EventHandler<MultiSplitterEventArgs>(multiSplitContainer_OnResizing_OnSplitterMoveOrDragEnded);
			this.multiSplitContainer.OnSplitterDragEnded += new EventHandler<MultiSplitterEventArgs>(multiSplitContainer_OnResizing_OnSplitterMoveOrDragEnded);


			this.AutoScroll = false;
			//this.HScroll = true;
			this.hScrollBar.SmallChange = this.ChartSettings.ScrollNBarsPerOneKeyPress;

			panels = new List<PanelBase>();
			panels.Add(this.panelPrice);
			panels.Add(this.panelVolume);

			//v1 1/2 making ChartForm editable in Designer: exit to avoid InitializeCreateSplittersDistributeFor() below throw
			//if (base.DesignMode) return; // Generics in InitializeComponent() cause Designer to throw up (new Sq1.Charting.MultiSplit.MultiSplitContainer<PanelBase>())
			//v2 uncomment if you are opening ChartForm in Designer to make it editable; v1 doens't work!?!!
			// return;	// ENABLE_CHART_CONTROL_DESIGNER <= keyword for easy find

			this.multiSplitContainer.InitializeCreateSplittersDistributeFor(panels);

			// Splitter might still notify them for Resize() during startup (while only multiSplitter's size should generate Resize in nested controls)  
			// trying to leave only multiSplitter to react on Panel2.Collapsed = true/false;
			//this.splitContainerChartVsRange.Panel1.Controls.Remove(this.panelVolume);
			//this.splitContainerChartVsRange.Panel1.Controls.Remove(this.panelPrice);
			this.splitContainerChartVsRange.Panel1.Controls.Remove(this.lblWinFormDesignerComment);

			// TOO_EARLY_MOVED_TO_PropagateSettingSplitterDistancePriceVsVolume this.multiSplitContainer.SplitterPositionsByManorder = this.ChartSettings.SplitterPositionsByManorder;
			
			this.panelPrice.Initialize(this);
			this.panelVolume.Initialize(this);

			BarIndexMouseIsOverNow = -1;	//if I didn't mouseover after app start, streaming values don't show up

			#region 1/2 making ChartForm editable in Designer; I left that cumbersome because haven't finished investigation how to make ChartForm show ChartControl
			// this.Initialize()_BELOW_MAKES_DESIGNER_THROW_DUE_TO_ASSEMBLER_NON_INSTANTIATED
			return;

			//this.chartRenderer.Initialize(this);
			BarScaleInterval chartShouldntCare = new BarScaleInterval(BarScale.Minute, 5);
			//REFLECTION_FAILS_FOR_DESIGNER BarsBasic.GenerateRandom(chartShouldntCare)
			//this.Initialize(BarsBasic.GenerateRandom(chartShouldntCare));
			Bars generated = new Bars("RANDOM", chartShouldntCare, "test-ChartControl-DesignMode");
			generated.GenerateAppend();
			this.Initialize(generated);
			#endregion
		}
		public void Initialize(Bars barsNotNull, bool invalidateAllPanels = true) {
			this.barEventsDetach();
			this.Bars = barsNotNull;
			if (this.BarsNotEmpty == false) {
				string msg = "DONT_PASS_EMPTY_BARS_TO_CHART_CONTROL ATTACHING_EVENTS_THOUGH";
				Assembler.PopupException(msg);
				//return;
			}
			this.barEventsAttach();
			this.SyncHorizontalScrollToBarsCount();
			//this.hScrollBar.ValueCurrent = this.hScrollBar.Maximum;	// I just sync'ed this.hScrollBar.Maximum = this.Bars.Count - 1
			// after I reduced BarRange{500bars => 100bars} in MainForm, don't set this.hScrollBar.Value here, I'll invoke ScrollToLastBarRight() upstack
			//v1 if (this.ChartSettings.ScrollPositionAtBarIndex >= this.hScrollBar.Minimum && this.ChartSettings.ScrollPositionAtBarIndex <= this.hScrollBar.Maximum) {
			//	// I'm here 1) at ChartControl startup; 2) after I changed BarRange in MainForm 
			//	this.hScrollBar.Value = this.ChartSettings.ScrollPositionAtBarIndex;
			//} else {
			//	string msg = "HSCROLL_POSITION_VALUE_OUT_OF_RANGE; fix deserialization upstack";
			//}
			//v1 STREAMING_GROWS_BARS.COUNT_AND_YOU_SAVE_IT_AS_520__ON_RESTART_YOU_GO_BEYOND_BARS_LOADED_500
			//v2
			if (this.ChartSettings.ScrollPositionAtBarIndex < 0) {
				string msg = "CATCH_AND_FIX_WHEN_YOU_ASSIGN_ChartSettings.ScrollPositionAtBarIndex_TO_NEGATIVE_VALUE";
				Assembler.PopupException(msg);
				return;
			}
			if (this.ChartSettings.ScrollPositionAtBarIndex > this.hScrollBar.Maximum) {
				this.ChartSettings.ScrollPositionAtBarIndex = this.hScrollBar.Maximum;
			}
			// I'm here 1) at ChartControl startup; 2) after I changed BarRange in MainForm 
			this.hScrollBar.Value = this.ChartSettings.ScrollPositionAtBarIndex;
			foreach (PanelBase panel in this.panels) {	// at least PanelPrice and PanelVolume
				panel.InitializeWithNonEmptyBars(this);
			}
			if (invalidateAllPanels == false) return;
			this.InvalidateAllPanels();
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
		public void InvalidateAllPanels() {
			if (base.InvokeRequired) {
				base.BeginInvoke(new MethodInvoker(this.InvalidateAllPanels));
				return;
			}
			if (this.IsBacktestingNow) return;
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			this.hScrollBar.Minimum = this.BarsCanFitForCurrentWidth;
			foreach (PanelBase panel in this.panels) {
				panel.Invalidate();
			}
			//if (this.InvalidatedByStreamingKeepTooltipsOpen == true) return;
			//this.TooltipPriceHide();
			//this.TooltipPositionHide();
			//this.InvalidatedByStreamingKeepTooltipsOpen = false;
		}
		void scrollToBarSafely(int bar) {
			if (bar > this.hScrollBar.Maximum) bar = this.hScrollBar.Maximum;
			if (bar < this.hScrollBar.Minimum) bar = this.hScrollBar.Minimum;
			this.ChartSettings.ScrollPositionAtBarIndex = bar;
			this.RaiseChartSettingsChangedContainerShouldSerialize();
			this.hScrollBar.Value = bar;
			this.InvalidateAllPanels();
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
			if (this.Bars == null) {
				#if DEBUG
				string msg = "POSSIBLY_DISABLE_SCROLLBAR_WHEN_CHART_HAS_NO_BARS? OR MAKE_CHART_ALWAYS_DISPLAY_BARS";
				Debugger.Break();
				#endif
				return;
			}

			if (this.hScrollBar.Value != scrollEventArgs.NewValue) {	// FILTER_OUT_UNNECESSARY_INVOCATIONS
				//ALREADY_THERE_AFTER_EVENT_HANDLER_TERMINATES this.hScrollBar.Value = scrollEventArgs.NewValue;
				this.InvalidateAllPanels();
			}

			if (scrollEventArgs.Type == ScrollEventType.ThumbPosition || scrollEventArgs.Type == ScrollEventType.ThumbTrack) {
				// dragging: ThumbPosition -> ThumbTrack -> EndScroll; EndScroll will follow 100% and we'll serialize
				return;
			}
			// single-click input (arrows, direct position) or EndScroll after ThumbPosition
			if (this.ChartSettings.ScrollPositionAtBarIndex != this.hScrollBar.Value) {
				this.ChartSettings.ScrollPositionAtBarIndex  = this.hScrollBar.Value;
				this.RaiseChartSettingsChangedContainerShouldSerialize();	//scrollbar should have OnDragCompleteMouseReleased event!!!
			}
		}
		void barEventsAttach() {
			if (this.Bars == null) return; 
			this.Bars.BarStaticAdded			+= new EventHandler<BarEventArgs>(chartControl_BarAddedUpdated_ShouldTriggerRepaint);		// quite useless since I don't plan to append-statically to displayed-bars 
			this.Bars.BarStreamingAdded			+= new EventHandler<BarEventArgs>(chartControl_BarAddedUpdated_ShouldTriggerRepaint); 
			this.Bars.BarStreamingUpdatedMerged	+= new EventHandler<BarEventArgs>(chartControl_BarAddedUpdated_ShouldTriggerRepaint);  
		}
		void barEventsDetach() {
			if (this.Bars == null) return; 
			this.Bars.BarStreamingUpdatedMerged -= new EventHandler<BarEventArgs>(chartControl_BarAddedUpdated_ShouldTriggerRepaint);  
			this.Bars.BarStreamingAdded			-= new EventHandler<BarEventArgs>(chartControl_BarAddedUpdated_ShouldTriggerRepaint);  
			this.Bars.BarStaticAdded			-= new EventHandler<BarEventArgs>(chartControl_BarAddedUpdated_ShouldTriggerRepaint); 
		}
		void chartControl_BarAddedUpdated_ShouldTriggerRepaint(object sender, BarEventArgs e) {
			// if I was designing events for WinForms, I would switch to GUI thread automatically
			if (base.InvokeRequired == true) {
				base.BeginInvoke((MethodInvoker)delegate { this.chartControl_BarAddedUpdated_ShouldTriggerRepaint(sender, e); });
				return;
			}
			if (this.VisibleBarRight != this.Bars.Count) return;	// move slider if only last bar is visible 
			this.SyncHorizontalScrollToBarsCount();
			this.InvalidateAllPanels();
			this.RangeBar.Invalidate();
		}

		public void BarWidthIncrementAtKeyPressRate() {
			if (this.ChartSettings.BarWidthIncludingPadding >= this.ChartSettings.BarWidthIncludingPaddingMax) return; 
			this.ChartSettings.BarWidthIncludingPadding += this.ChartSettings.SqueezeHorizontalKeyOnePressReceivedToOneStep;
			this.InvalidateAllPanels();
			base.RaiseChartSettingsChangedContainerShouldSerialize();
		}
		public void BarWidthDecrementAtKeyPressRate() {
			if (this.ChartSettings.BarWidthIncludingPadding <= this.ChartSettings.SqueezeHorizontalKeyOnePressReceivedToOneStep) return;	// <= since BarWidth mustn't be zero (ZeroDivException)
			this.ChartSettings.BarWidthIncludingPadding -= this.ChartSettings.SqueezeHorizontalKeyOnePressReceivedToOneStep;
			this.InvalidateAllPanels();
			base.RaiseChartSettingsChangedContainerShouldSerialize();
		}
		public void DragDownSqueeze() {
			this.ChartSettings.SqueezeVerticalPaddingPx += this.ChartSettings.SqueezeVerticalPaddingStep;
//			if (this.ChartSettings.BarWidthIncludingPadding >= this.ChartSettings.SqueezeHorizontalStep) {
//				this.BarWidthDecrement();
//				return;
//			}
			this.panelPrice.Invalidate();
			//base.RaiseChartSettingsChangedContainerShouldSerialize();
		}
		public void DragUpUnsqueeze() {
			if (this.ChartSettings.SqueezeVerticalPaddingPx < this.ChartSettings.SqueezeVerticalPaddingStep) return;
			this.ChartSettings.SqueezeVerticalPaddingPx -= this.ChartSettings.SqueezeVerticalPaddingStep;
//			this.BarWidthIncrement();
			this.panelPrice.Invalidate();
			//base.RaiseChartSettingsChangedContainerShouldSerialize();
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

		void tooltipPositionShowXY(AlertArrow alertArrowToPopup, int xPosition, int yPosition) {
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
		void tooltipPriceShowXY(Bar barToPopulate, int x, int y) {
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
			this.InvalidateAllPanels();
			return wasInvalidated;
		}
		public bool TooltipPositionHide() {
			bool wasInvalidated = false;
			if (this.ChartSettings.TooltipPositionShow == false) return wasInvalidated;
			if (this.tooltipPosition.Visible == false) return wasInvalidated;	// DO_NOT_DELETE__HERE_WE_BREAK_INFINITE_LOOP
			this.tooltipPosition.Visible = false;
			wasInvalidated = true;
			this.InvalidateAllPanels();
			return wasInvalidated;
		}
		
		public void PropagateSplitterManorderDistanceIfFullyDeserialized() {
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) {
				return;
			}
			this.multiSplitContainer.SplitterPropertiesByPanelNameSet(this.ChartSettings.MultiSplitterPropertiesByPanelName);
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
//		public Rectangle TooltipPriceClientRectangleOrEmptyIfInvisible { get {
//				Rectangle ret = Rectangle.Empty;
//				if (this.tooltipPrice.Visible == false) return ret;
//				return this.tooltipPrice.ClientRectangle;
//			} }
		public override string ToString() {
			string ret = "NO_PARENT_INFO for " + this.Name;
			Form parentForm = this.Parent as Form;
			if (parentForm != null) {
				ret = parentForm.Text;
			} else {
				if (this.Parent != null) ret = "Parent[" + this.Parent.ToString() + "]";
			}
			return ret;
		}

	}
}
