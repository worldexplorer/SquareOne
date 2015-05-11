using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core;

namespace Sq1.Charting {
	public partial class PanelBase {
		protected bool	mouseOver;						// HELPS_DRAWING_INDICATOR_VALUE_ON_GUTTER_WHILE_HOVERING_PANEL_PRICE
		protected int	barIndexMouseIsOverPrev;		// barIndexMouseIsOverPrev is needed for a mouse over Volume to paint CurrentBar'sClose on Price (try & see)
		public	  int	BarIndexMouseIsOverNow = -1;	// if I didn't mouseover after app start, streaming values don't show up
		
		protected bool	dragButtonPressed;
		
		protected bool	scrollingHorizontally;
		protected int	scrollHorizontalXstarted;
		protected int	scrollHorizontalXprev;
		
		protected bool	squeezingVertically;
		protected int	squeezeVerticalYstarted;
		protected int	squeezeVerticalYprev;

		protected bool	squeezingHorizontally;
		protected int	squeezeHorizontalXstarted;
		protected int	squeezeHorizontalXprev;
		
		protected int	moveHorizontalXprev;
		protected int	moveHorizontalYprev = -1;	//if I didn't mouseover after app start, streaming values don't show up

		protected override void OnMouseEnter(EventArgs e) {
			if (base.DesignMode) return;
			base.OnMouseEnter(e);
			if (this.ChartControl == null) return;
			this.scrollingHorizontally = false;
			this.squeezingHorizontally = false;
			this.squeezingVertically = false;
			this.mouseOver = true;
			if (this.ChartControl.ChartSettings.MousePositionTrackOnGutters) {
				this.ChartControl.InvalidateAllPanels();
			}
		}
		protected override void OnMouseLeave(EventArgs e) {
			if (base.DesignMode) return;
			base.OnMouseLeave(e);
			if (this.ChartControl == null) return;
			
			//this.ChartControl.TooltipPrice.ClientRectangle.Contains(e.
			//if (this.ChartControl.TooltipPriceShown && ) {
			//if (this.ChartControl.TooltipPriceShownAndMouseOverIt == true) {
			//	return;
			//}
			//if (this.ChartControl.TooltipPositionShownAndMouseOverIt == true) {
			//	return;
			//}
			
			this.scrollingHorizontally = false;
			this.squeezingHorizontally = false;
			this.squeezingVertically = false;

			// MouseTrack uses this to remove MouseLines from Panel Price & MousePositionLabels from Gutters
			this.moveHorizontalXprev = -1;
			this.moveHorizontalYprev = -1;
			
			this.mouseOver = false;
			if (this.ChartControl.ChartSettings.MousePositionTrackOnGutters) {
				this.ChartControl.InvalidateAllPanels();
			}
			
			barIndexMouseIsOverPrev = -1;
			BarIndexMouseIsOverNow = -1;
			this.ChartControl.BarIndexMouseIsOverNow = -1;

			this.ChartControl.TooltipPriceHide();
			this.ChartControl.TooltipPositionHide();

			this.ChartControl.InvalidateAllPanels();	//	DRAWING_CURRENT_JUMPING_STREAMING_VALUE_ON_GUTTER_SINCE_MOUSE_WENT_OUT_OF_BOUNDARIES
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			if (this.ChartControl == null) return;
			if (e.Button != MouseButtons.Left) return;
			this.dragButtonPressed = true;
			this.scrollingHorizontally = false;
			this.squeezingHorizontally = false;
			this.squeezingVertically = false;
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			if (this.ChartControl == null) return;
			if (e.Button != MouseButtons.Left) return;
			if (this.scrollingHorizontally || this.squeezingHorizontally || this.squeezingVertically) {
				this.ChartControl.RaiseChartSettingsChangedContainerShouldSerialize();
			}
			this.dragButtonPressed = false;
			this.scrollingHorizontally = false;
			this.squeezingHorizontally = false;
			this.squeezingVertically = false;
			this.Cursor = Cursors.Default;
			base.OnMouseUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			if (base.DesignMode) return;
			if (this.ChartControl == null) return;
			
			if (this.ChartControl.PaintAllowedDuringLivesimOrAfterBacktestFinished == false) {
				//this.Cursor = Cursors.WaitCursor;
				return;
			}
			//if (this.Cursor == Cursors.WaitCursor) this.Cursor = Cursors.Default;
			try {
				//if (this.DesignMode) return;				// so that Designer works
				// if (base.DesignMode) this.ChartControl will be NULL
				if (this.ChartControl == null) return;		// so that Designer works
				if (this.ChartControl.BarsEmpty) return;	// finally {} will invoke base.OnMouseMove()
				
				BarIndexMouseIsOverNow = this.XToBar(e.X);
				this.ChartControl.BarIndexMouseIsOverNow = BarIndexMouseIsOverNow; 
				
				if (this.dragButtonPressed == true
						&& this.scrollingHorizontally == false
						&& this.squeezingHorizontally == false
						&& this.squeezingVertically == false) {
					// coordinates inverted: (x=0;y=0) is upper left corner
					if (e.X > this.PanelWidthMinusRightPriceGutter) {
						// first move after this.mousePressed means DRAG!! wasn't in UserControl due to ambiguity between OnMouseMove and OnMouseDrag, probably
						this.squeezeVerticalYstarted = e.Y;
						this.squeezeVerticalYprev = e.Y;
						this.squeezingVertically = true;
					} else if (e.Y > this.PanelHeightMinusGutterBottomHeight_cached) {
						// first move after this.mousePressed means DRAG!! wasn't in UserControl due to ambiguity between OnMouseMove and OnMouseDrag, probably
						this.squeezeHorizontalXstarted = e.Y;
						this.squeezeHorizontalXprev = e.Y;
						this.squeezingHorizontally = true;
					} else {
						// first move after this.mousePressed means DRAG!! wasn't in UserControl due to ambiguity between OnMouseMove and OnMouseDrag, probably
						this.scrollHorizontalXstarted = e.X;
						this.scrollHorizontalXprev = e.X;
						this.scrollingHorizontally = true;
					}
					this.ChartControl.TooltipPriceHide();
					this.ChartControl.TooltipPositionHide();
				}

				if (this.scrollingHorizontally) {
					// should drag a little for me to consider the user is really dragging anything
					if (Math.Abs(this.scrollHorizontalXstarted - e.X) <=
						this.ChartControl.ChartSettings.ScrollSqueezeMouseDragSensitivityPx) return;
					
					int XnotBeyond0width = e.X;
					//COMMENTED_OUT_TO_ALLOW_SCROLL_BEYOND_CONTROL/APPLICATION_VISIBLE_SURFACE_AREA
					//if (e.X < 0) XnotBeyond0width = 0;
					//if (e.X > base.Width) XnotBeyond0width = base.Width;

					if (this.scrollHorizontalXprev == XnotBeyond0width) return;		// ignore vertical mouse movements while dragging (and all other 1-sec-frequency garbage events)
					bool draggingToTheLeft = (this.scrollHorizontalXstarted > XnotBeyond0width) ? true : false;
					if (draggingToTheLeft) {
						if (this.scrollHorizontalXprev < XnotBeyond0width) {	// we reversed drag direction without releasing the mouse button
							this.scrollHorizontalXstarted = XnotBeyond0width;
							this.scrollHorizontalXprev = XnotBeyond0width;
							return;
						}
					} else {
						if (this.scrollHorizontalXprev > XnotBeyond0width) {	// we reversed drag direction without releasing the mouse button
							this.scrollHorizontalXstarted = XnotBeyond0width;
							this.scrollHorizontalXprev = XnotBeyond0width;
							return;
						}
					}
					this.scrollHorizontalXprev = XnotBeyond0width;	// continue scrolling since we are still dragging to the same direction as for event-received-previously
					if (draggingToTheLeft) {
						//if (this.VisibleMin_cached <= 0) return;		//this.HorizontalScroll IS NOT USED!!! use Parent's instead! ChartControl.HorizontalScroll
						//if (this.ChartControl.hScrollBar.ValueCurrent >= this.ChartControl.HorizontalScroll.Maximum) return;
						this.Cursor = Cursors.PanWest;
						this.ChartControl.DragNBarsRight();
					} else {
						//if (this.VisibleMax_cached >= this.ChartControl.Bars.Count) return;
						//if (this.ChartControl.hScrollBar.ValueCurrent <= this.ChartControl.HorizontalScroll.Minimum) return;
						this.Cursor = Cursors.PanEast;
						this.ChartControl.DragNBarsLeft();
					}
				}
				
				if (this.squeezingHorizontally) {
					if (this.ThisPanelIsPricePanel == false) return;
					// should drag a little for me to consider the user is really dragging anything
					if (Math.Abs(this.squeezeHorizontalXstarted - e.X) <=
						this.ChartControl.ChartSettings.ScrollSqueezeMouseDragSensitivityPx) return;
					
					int XnotBeyond0height = e.X;
					//COMMENTED_OUT_TO_ALLOW_SCROLL_BEXOND_CONTROL/APPLICATION_VISIBLE_SURFACE_AREA
					//if (e.X < 0) XnotBeyond0height = 0;
					//if (e.X > base.Height) XnotBeyond0height = base.Height;
					if (Math.Abs(this.squeezeHorizontalXprev - XnotBeyond0height) <
						this.ChartControl.ChartSettings.SqueezeHorizontalMouse1pxDistanceReceivedToOneStep) return;

					if (this.squeezeHorizontalXprev == XnotBeyond0height) return;		// ignore vertical mouse movements while dragging (and all other 1-sec-frequency garbage events)
					bool squeezingToTheLeft = (this.squeezeHorizontalXstarted > XnotBeyond0height) ? true : false;
					if (squeezingToTheLeft) {
						if (this.squeezeHorizontalXprev < XnotBeyond0height) {	// we reversed drag direction without releasing the mouse button
							this.squeezeHorizontalXstarted = XnotBeyond0height;
							this.squeezeHorizontalXprev = XnotBeyond0height;
							return;
						}
					} else {
						if (this.squeezeHorizontalXprev > XnotBeyond0height) {	// we reversed drag direction without releasing the mouse button
							this.squeezeHorizontalXstarted = XnotBeyond0height;
							this.squeezeHorizontalXprev = XnotBeyond0height;
							return;
						}
					}
					this.squeezeHorizontalXprev = XnotBeyond0height;	// continue scrolling since we are still dragging to the same direction as for event-received-previously
					if (squeezingToTheLeft) {
						this.Cursor = Cursors.PanWest;
						this.ChartControl.BarWidthIncrementAtKeyPressRate();
					} else {
						this.Cursor = Cursors.PanEast;
						this.ChartControl.BarWidthDecrementAtKeyPressRate();
					}
				}
				
				if (this.squeezingVertically) {
					if (this.ThisPanelIsPricePanel == false) return;
					// should drag a little for me to consider the user is really dragging anything
					if (Math.Abs(this.squeezeVerticalYstarted - e.Y) <=
						this.ChartControl.ChartSettings.ScrollSqueezeMouseDragSensitivityPx) return;
					
					int YnotBeyond0height = e.Y;
					//COMMENTED_OUT_TO_ALLOW_SCROLL_BEYOND_CONTROL/APPLICATION_VISIBLE_SURFACE_AREA
					//if (e.Y < 0) YnotBeyond0height = 0;
					//if (e.Y > base.Height) YnotBeyond0height = base.Height;

					if (this.squeezeVerticalYprev == YnotBeyond0height) return;		// ignore vertical mouse movements while dragging (and all other 1-sec-frequency garbage events)
					bool draggingUp = (this.squeezeVerticalYstarted > YnotBeyond0height) ? true : false;
					if (draggingUp) {
						if (this.squeezeVerticalYprev < YnotBeyond0height) {	// we reversed drag direction without releasing the mouse button
							this.squeezeVerticalYstarted = YnotBeyond0height;
							this.squeezeVerticalYprev = YnotBeyond0height;
							return;
						}
					} else {
						if (this.squeezeVerticalYprev > YnotBeyond0height) {	// we reversed drag direction without releasing the mouse button
							this.squeezeVerticalYstarted = YnotBeyond0height;
							this.squeezeVerticalYprev = YnotBeyond0height;
							return;
						}
					}
					this.squeezeVerticalYprev = YnotBeyond0height;	// continue scrolling since we are still dragging to the same direction as for event-received-previously
					if (draggingUp) {
						this.Cursor = Cursors.PanNorth;
						this.ChartControl.DragDownSqueeze();
					} else {
						this.Cursor = Cursors.PanSouth;
						this.ChartControl.DragUpUnsqueeze();
					}
					this.RaisePanelPriceSqueezed();
				}
				
				if (this.moveHorizontalXprev == e.X && this.moveHorizontalYprev == e.Y) {
					return;
				}
				this.moveHorizontalXprev = e.X;
				this.moveHorizontalYprev = e.Y;

				if (this.ChartControl.ChartSettings.MousePositionTrackOnGutters) {
					//JONNY_BE_GOOD__DONT_BE_THIS_RUDE base.Refresh();
					base.Invalidate();

					//v1 if (this.ChartControl.BarIndexMouseIsOverNow != this.BarIndexMouseIsOverNow) {
					if (barIndexMouseIsOverPrev != this.BarIndexMouseIsOverNow) {
						barIndexMouseIsOverPrev  = this.BarIndexMouseIsOverNow;		// barIndexMouseIsOverPrev is needed for a mouse over Volume to paint CurrentBar'sClose on Price (try & see)
						this.ChartControl.BarIndexMouseIsOverNow  = this.BarIndexMouseIsOverNow;
						this.ChartControl.InvalidateAllPanels();
					}
				}

				if (this.ThisPanelIsPricePanel && this.scrollingHorizontally == false) {
					this.TooltipPositionShown = this.handleTooltipsPositionAndPrice(e);
					if (this.TooltipPositionShown == false) this.handleTooltipPrice(e);
				}
			} catch (Exception ex) {
				string msg = "WindProc won't catch your exceptions; //PanelBase.OnMouseMove()";
				Assembler.PopupException(msg, ex);
			} finally {
				base.OnMouseMove(e);
			}
		}
		public bool TooltipPositionShown;
		bool handleTooltipsPositionAndPrice(MouseEventArgs e) {
			// COPYPASTE_SOURCE=PanelNamedFolding.handleTooltipsPositionAndPrice()_DESTINATION=ChartShadowProtocol.SelectPosition() begin 
			bool tooltipPositionShown = false;
			if (this.ChartControl.ChartSettings.TooltipPositionShow == false) return tooltipPositionShown;

			int barIndexMouseIsOverNow = this.XToBar(e.X);
			if (barIndexMouseIsOverNow < this.VisibleBarLeft_cached) return tooltipPositionShown;
			if (barIndexMouseIsOverNow > this.VisibleBarRight_cached) return tooltipPositionShown;	//Debugger.Launch();

			Dictionary<int, List<AlertArrow>> alertArrowsListByBar = this.ChartControl.ScriptExecutorObjects.AlertArrowsListByBar;
			if (alertArrowsListByBar.ContainsKey(barIndexMouseIsOverNow) == false) {
				this.ChartControl.TooltipPositionHide();
				return tooltipPositionShown;
			}

			List<AlertArrow> arrowsForBar = alertArrowsListByBar[barIndexMouseIsOverNow];
			AlertArrow arrowFoundForMouse = null;
			foreach (AlertArrow arrow in arrowsForBar) {
				int yInverted = e.Y;
				int arrowYinvertedHigher = arrow.Ytransient;
				int arrowYinvertedLower = arrowYinvertedHigher + arrow.Height;
				if (yInverted < arrowYinvertedHigher || yInverted > arrowYinvertedLower) continue;
				arrowFoundForMouse = arrow;
				break;
			}
			if (arrowFoundForMouse == null) {
				this.ChartControl.TooltipPositionHide();
				return tooltipPositionShown;
			}
			Position positionToPopup = arrowFoundForMouse.Position;
			bool placeAtLeft = arrowFoundForMouse.ArrowIsForPositionEntry;
			Bar bar = arrowFoundForMouse.ArrowIsForPositionEntry ? positionToPopup.EntryBar : positionToPopup.ExitBar;

			int barX = this.BarToX(barIndexMouseIsOverNow);
			Rectangle rectangleYarrowXbar = new Rectangle();
			rectangleYarrowXbar.X		= barX;
			rectangleYarrowXbar.Width	= this.BarWidthIncludingPadding_cached;		//arrowFoundForMouse.Width;
			rectangleYarrowXbar.Y		= arrowFoundForMouse.Ytransient;
			rectangleYarrowXbar.Height	= arrowFoundForMouse.Height;

			bool mouseIsOverArrowIconRectangle = rectangleYarrowXbar.Contains(e.X, e.Y);

			if (mouseIsOverArrowIconRectangle) {
				this.ChartControl.TooltipPositionAndPriceShow(arrowFoundForMouse, bar, rectangleYarrowXbar);
				tooltipPositionShown = true;
			} else {
				this.ChartControl.TooltipPositionHide();
				this.ChartControl.TooltipPriceHide();
			}
			// COPYPASTE_SOURCE=PanelNamedFolding.handleTooltipsPositionAndPrice()_DESTINATION=ChartShadowProtocol.SelectPosition() end 

			return tooltipPositionShown;
		}
		void handleTooltipPrice(MouseEventArgs e) {
			if (this.ChartControl.ChartSettings.TooltipPriceShow == false) return;
			//if (this.ThisPanelIsPricePanel == false) return;
			//if (this.dragButtonPressed == true) return;
			//if (this.scrollingHorizontally == false || this.squeezingHorizontally == false || this.squeezingVertically == false) return;

			int barIndexMouseIsOverNow = this.XToBar(e.X);
			if (barIndexMouseIsOverNow < this.VisibleBarLeft_cached) return;
			if (barIndexMouseIsOverNow > this.VisibleBarRight_cached) return;	//Debugger.Launch();

			Bar barMouseIsOverNow = this.ChartControl.Bars[barIndexMouseIsOverNow];
			if (double.IsNaN(barMouseIsOverNow.Low)) return;
			if (double.IsNaN(barMouseIsOverNow.High)) return;

			int yLow  = this.ValueToYinverted(barMouseIsOverNow.Low);
			int yHigh = this.ValueToYinverted(barMouseIsOverNow.High);

			Rectangle rectangleBarWithShadows = new Rectangle();
			rectangleBarWithShadows.X = this.BarToX(barIndexMouseIsOverNow);
			rectangleBarWithShadows.Y = yHigh;
			rectangleBarWithShadows.Width = this.BarWidthIncludingPadding_cached;
			rectangleBarWithShadows.Height = yLow - yHigh;	// due to upperLeft=(0:0), inverted yLow > yHigh
			if (this.ChartControl.ChartSettings.TooltipPriceShowOnlyWhenMouseTouchesCandle) {
// FAILED		hide the priceTooltip when mouse hovers <= chart drag is "blocked", mouse "cross" doesn't update (I want to see the price of current mouse position)  
// WHY			TooltipPriceClientRectangleOrEmptyIfInvisible.Location is (always) 0:0, I'm lazy to offset TooltipPosition to parent's (ChartControl's) coordinates;
// ALTERNATIVE	TooltipPrice.Designer.cs: ouseMove += new MouseEventHandler(TooltipPrice_MouseMove);
//				Rectangle clientRectangle = this.ChartControl.TooltipPriceClientRectangleOrEmptyIfInvisible;
//				if (clientRectangle != null) {
//					bool mouseIsOverTooltipPriceRectangle = clientRectangle.Contains(e.X, e.Y);
//					if (mouseIsOverTooltipPriceRectangle) {
//						this.ChartControl.TooltipPriceHide();
//						return;
//					}
//				}
				//int verticalSensitivityIncreased = 4;	// doesn't cause flickering, keep TooltipSurfacePaddingFromBarLeftRightToAvoidMouseLeave > 2
				int verticalSensitivityIncreased = 0;	//MOVE_TO_SETTINGS_SO_ARROWS_WONT_OVERLAP
				bool mouseIsOverBarRectangle = e.Y >= rectangleBarWithShadows.Top - verticalSensitivityIncreased
											&& e.Y <= rectangleBarWithShadows.Bottom + verticalSensitivityIncreased;
				if (mouseIsOverBarRectangle) {
					this.ChartControl.TooltipPriceShowAlone(barMouseIsOverNow, rectangleBarWithShadows);
				} else {
					this.ChartControl.TooltipPriceHide();
				}
			} else {
				this.ChartControl.TooltipPriceShowAlone(barMouseIsOverNow, rectangleBarWithShadows);
			}
		}
		//public new void Invalidate() {
			// for unwanted user-initiated Paint()s, watch invoker in the stack; otherwize OS invoked Paint() and you have to deal with it
		//	base.Invalidate();
		//}
	}
}
