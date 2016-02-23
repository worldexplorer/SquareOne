using System;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DoubleBuffered;

namespace Sq1.Charting.MultiSplit {
	public partial class MultiSplitContainerGeneric<PANEL_BASE>
// USING_NON_DOUBLE_BUFFERED_TO_PAINT_RED_SPLITTER_BACKGROUND_ON_DRAGGING_USING_GRIP...
#if NON_DOUBLE_BUFFERED
			: UserControl
#else
//SharpDevelopClickThroughAndF12
			: UserControlDoubleBuffered
#endif
			where PANEL_BASE : Control {
		
		protected override void OnResize(EventArgs e) {
			//I_WILL_INVOKE_MY_TENANTS_DistributePanelsAndSplitters/Invalidate()_ON_MY_OWN
			base.OnResize(e); // I NEED UserControlDoubleBuffered to rebuild its offscreen buffer size and sync !!! otherwize the canvas for chart etc stays the same ingoring MainForm.Resizes

			if (base.DesignMode) return;
			if (Assembler.IsInitialized == false) return;	// otherwize I couldn't drop ChartControl from Toolbox to TestChartControl form / designer
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			//if (Assembler.InstanceInitialized.SplitterEventsAreAllowedNsecAfterLaunchHopingInitialInnerDockResizingIsFinished) return;
			if (this.ignoreResizeImSettingWidthOrHeight) return;
			if (this.panels.Count == 0) return;

			try {
				this.DistributePanelsAndSplitters();		// otherwize internal Controls do not fit extended MainForm window surface
				int  baseWidthMinusVScroll = base.ClientRectangle.Width;
				int baseHeightMinusHScroll = base.ClientRectangle.Height;
				if (base.Height != base.ClientRectangle.Height) {
					string msg = "WAS_HSCROLL_HEIGHT_SUBTRACTED? AND DOCUMENT_TABS???";
				}
				if (this.VerticalizeAllLogic == false) {
					foreach (MultiSplitter splitter in this.splitters) {
				   		splitter.Width = baseWidthMinusVScroll;
				   		//NO_TRANSFER_TO_THEM_RESIZE_HOPING_THEY_WILL_REPAINT_FULL_NEW_SIZE splitter.Invalidate();	// base.Invalidate() below doesn't reposition the dots
					}
					foreach (Control panel in this.panels) {
				   		panel.Width = baseWidthMinusVScroll;
				   		//TESTME panel.Invalidate();
					}
				} else {
					foreach (MultiSplitter splitter in this.splitters) {
				   		splitter.Height = baseHeightMinusHScroll;
				   		//NO_TRANSFER_TO_THEM_RESIZE_HOPING_THEY_WILL_REPAINT_FULL_NEW_SIZE splitter.Invalidate();	// base.Invalidate() below doesn't reposition the dots
					}
					foreach (Control panel in this.panels) {
				   		panel.Height = baseHeightMinusHScroll;
				   		//TESTME panel.Invalidate();
					}
				}
				//NON_PAINTED_EXTENDED_AREAS_ON_SPLITTER base.Invalidate();
				// SLOWS_DOWN_BUT_WITHOUT_IT_LEVEL2_PAINTS_WITH_CHART_AND_VICE_VERSA!!! HELPS_onstartup
				// DOESNT_SLOW_DOWN_BUT_WITHOUT_IT_NON_PAINTED_EXTENDED_AREAS_ON_SPLITTER
				// NO_TRANSFER_TO_THEM_RESIZE_HOPING_THEY_WILL_REPAINT_FULL_NEW_SIZE
				//base.OnResize(e);	// hoping it will invoke all my tenants and they will repaint gray black splitters without mouseovers
			} catch (Exception ex) {
				Assembler.PopupException(" //MultiSplitContainerGeneric.OnResize()???", ex);
			}
		}
// USING_NON_DOUBLE_BUFFERED_TO_PAINT_RED_SPLITTER_BACKGROUND_ON_DRAGGING_USING_GRIP...
#if NON_DOUBLE_BUFFERED		//WHEN_INHERITED_FROM_REGULAR_USERCONTROL
		protected override void OnPaintBackground(PaintEventArgs e) {
			base.OnPaintBackground(e);
#else							//WHEN_INHERITED_FROM_USERCONTROL_DOUBLEBUFFERED
		protected override void OnPaintBackgroundDoubleBuffered(PaintEventArgs e) {
#endif

			if (base.DesignMode) return;
			return;
			
			//try {
			//	for (int i=0; i<this.panels.Count; i++) {
			//		Control panel = this.panels[i];
			//		//panel.Invalidate();	// base.OnPaint() did it already
			//	}
			//} catch (Exception ex) {
			//	string msg = "CONTROL.INVALIDATE()_IS_VERY_UNLIKELY_TO_THROW //MultiSplitContainerGeneric<PANEL_BASE>.OnPaintBackground()";
			//	Assembler.PopupException(msg, ex);
			//}
		}
// USING_NON_DOUBLE_BUFFERED_TO_PAINT_RED_SPLITTER_BACKGROUND_ON_DRAGGING_USING_GRIP...
#if NON_DOUBLE_BUFFERED		//WHEN_INHERITED_FROM_REGULAR_USERCONTROL
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);	//DOUBLEBUFFERED_PARENT_DID_THAT
#else							//WHEN_INHERITED_FROM_USERCONTROL_DOUBLEBUFFERED
		protected override void OnPaintDoubleBuffered(PaintEventArgs e) {
#endif
			if (base.DesignMode) return;
			if (Assembler.IsInitialized == false) return;	// otherwize I couldn't drop ChartControl from Toolbox to TestChartControl form / designer
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			//if (Assembler.InstanceInitialized.SplitterEventsAreAllowedNsecAfterLaunchHopingInitialInnerDockResizingIsFinished) return;
			if (this.ignoreResizeImSettingWidthOrHeight) return;
			if (this.panels.Count == 0) return;

			try {
				for (int i=0; i<this.panels.Count; i++) {
					//Control panel = this.panels[i];
					//panel.Invalidate();	// base.OnPaint() did it already
					
					if (i >= this.splitters.Count) {
						string msg = "YOU_GOT_MORE_(DESERIALIZED)_SPLITTERS_THAN_PANELS__MUST_BE_EQUAL  //MultiSplitContainerGeneric<PANEL_BASE>.OnPaint()";
						Assembler.PopupException(msg);
						continue;
					}
					MultiSplitter splitter = this.splitters[i];
					
					if (i == this.panelMouseIsOverNowIndexDropTarget) {
						// USING_NON_DOUBLE_BUFFERED_TO_PAINT_RED_SPLITTER_BACKGROUND_ON_DRAGGING_USING_GRIP...
						splitter.BackColor = ColorBackgroundSliderDroppingTarget;
					} else {
						splitter.BackColor = ColorBackgroundSliderRegular;
					}
					//splitter.Invalidate();
				}
				Point location = new Point(40, 10);
				e.Graphics.DrawString(this.panelText, this.Font, SystemBrushes.ControlText, location);
			} catch (Exception ex) {
				string msg = "CONTROL.INVALIDATE()_IS_VERY_UNLIKELY_TO_THROW //MultiSplitContainerGeneric<PANEL_BASE>.OnPaint()";
				Assembler.PopupException(msg, ex);
			}
		}
		//protected override void OnMouseMove(MouseEventArgs e) {
		//	// all my nested this.panels intercept their own OnMouseMove()s so I'm receiving only mouse-above-splitters here
		//	//Debugger.Break();
		//	//base.Invalidate();
		//}
		//protected override void OnMouseDown(MouseEventArgs e) {
		//	Debugger.Break();
		//}
		
		string panelText { get { return this.panelMouseIsOverNow != null ? this.panelMouseIsOverNow.Text : "<none>"; } }
		PANEL_BASE panelMouseIsOverNow;
		int panelMouseIsOverNowIndexDropTarget = -1;
		void panel_MouseEnter(object sender, EventArgs e) {
			if (this.panelMouseIsOverNow == sender) return;
			if (this.splitterStartedResizeOrDrag == null) { 
				//Debugger.Break();
				return;
			}
			PANEL_BASE panel = sender as PANEL_BASE;
			if (panel == null) {
				string msg = "I_MUST_BE_SUBSCRIBED_TO_TYPE " + typeof(PANEL_BASE) + "; got sender[" + sender.GetType() + "] //panel_MouseEnter()";
				Assembler.PopupException(msg);
				return;
			}
			this.panelMouseIsOverNow = panel;
			this.panelMouseIsOverNowIndexDropTarget = this.panels.IndexOf(this.panelMouseIsOverNow);
			MultiSplitter splitterForThisPanel = this.splitters[this.panelMouseIsOverNowIndexDropTarget];
			if (this.DebugSplitter) {
				//splitterAboveTarget.Text = this.splitterResizeOrDragStartedText + " targetIndex[" + this.panelMouseIsOverNowIndexDropTarget + "]";
				splitterForThisPanel.Text = " target[" + this.panelMouseIsOverNow.Text + "] index[" + this.panelMouseIsOverNowIndexDropTarget + "]";
				//splitterForThisPanel.Text += " :" + mousePositionFromSplitContainerUpperLeft;
			}
			//NOT_ENOUGH splitterForThisPanel.Invalidate();
			base.Invalidate();
		}
		void panel_MouseLeave(object sender, EventArgs e) {
			string msig = " //panel_MouseLeave()";
			this.panelMouseIsOverNow = null;
			this.panelMouseIsOverNowIndexDropTarget = -1;
			
			PANEL_BASE panel = sender as PANEL_BASE;
			if (panel == null) {
				string msg = "I_MUST_BE_SUBSCRIBED_TO_TYPE " + typeof(PANEL_BASE)
					+ "; got sender[" + sender.GetType() + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			int panelIndex = this.panels.IndexOf(panel);
			if (panelIndex == -1 || this.splitters.Count < panelIndex) {
				string msg = "I_REFUSE_TO_INVALIDATE_SPLITTER CANT_FIND_SPLITTER_FOR_PANEL";
				Assembler.PopupException(msg + msig);
				return;
			}
			MultiSplitter splitterForThisPanel = this.splitters[panelIndex];
			if (this.DebugSplitter) {
				splitterForThisPanel.Text = "<leftThisPanel>";
			}
			//NOT_ENOUGH splitterForThisPanel.Invalidate();
			base.Invalidate();
		}
		
		bool splitterIsDraggingNow = false;
		bool splitterIsMovingNow = false;
		MultiSplitter splitterStartedResizeOrDrag;
		Point splitterStartedResizeOrDragPoint;
		string splitterStartedResizeOrDragText { get {
				string ret = "<notResizingOrDragging>";
				if (this.splitterStartedResizeOrDrag == null) return ret;
				//if (this.splitters.Count <= splitterResizeOrDragStarted) return ret;
				//MultiSplitter splitterMouseOverResizingOrDragging = this.splitters[splitterResizeOrDragStarted];
				//Control panel = splitterStartedResizeOrDrag.PanelAbove as Control;
				
				int index = this.splitters.IndexOf(this.splitterStartedResizeOrDrag);
				Control panel = this.panels[index];
				ret = panel != null ? panel.Text : "<SplitterLostItsPanel>";
				if (this.splitterIsDraggingNow) ret += " PanelDragging";
				if (this.splitterIsMovingNow) ret += " PanelResizing";
				return ret;
			} }
		MultiSplitter splitterMouseIsOverNow;
		Point prevMousePointFilterNotMoving;
		void splitter_MouseMove(object sender, MouseEventArgs e) {			
			MultiSplitter splitter = sender as MultiSplitter;
			if (splitter == null) {
				string msg = "I_MUST_BE_SUBSCRIBED_TO_MultiSplitter, got sender[" + splitter.GetType() + "] //splitter_MouseMove()";
				Assembler.PopupException(msg);
				return;
			}
			if (this.DebugSplitter) {
				splitter.Text = this.splitterStartedResizeOrDragText;
			}
			
			// introducted this event filter koz 100% CPU when clicked on splitted and not moving (only when this.VerticalizeAllLogic=true)
			if (prevMousePointFilterNotMoving == null) prevMousePointFilterNotMoving = e.Location;
			if (prevMousePointFilterNotMoving == e.Location) return;
			prevMousePointFilterNotMoving = e.Location;
			
			if (this.splitterIsDraggingNow || this.splitterIsMovingNow) {
				Point mousePositionFromSplitContainerUpperLeft = new Point(
					e.X + this.splitterStartedResizeOrDragPoint.X,
					e.Y + this.splitterStartedResizeOrDragPoint.Y);
				if (this.splitterIsDraggingNow) this.splitterDraggingNow_PanelsSwap(splitter, mousePositionFromSplitContainerUpperLeft);
				if (this.splitterIsMovingNow)	this.splitterMovingNow_PanelsResize(splitter, mousePositionFromSplitContainerUpperLeft);
			} else {		//if (this.splitterIsDraggingNow == false && this.splitterIsResizingNow == false) {
				if (this.VerticalizeAllLogic == false) {
					base.Cursor = (e.X <= GrabHandleWidth) ? Cursors.Hand : Cursors.HSplit;
				} else {
					base.Cursor = (e.Y <= GrabHandleWidth) ? Cursors.Hand : Cursors.VSplit;
				}
			}

			if (this.DebugSplitter) {
				splitter.Text += " x:" + e.X + " y:" + e.Y;
				splitter.Invalidate();
				//base.Invalidate();
			}
			this.RaiseOnSplitterMovingNow(splitter);
		}
		void splitter_MouseEnter(object sender, EventArgs e) {
			MultiSplitter splitter = sender as MultiSplitter;
			if (splitter == null) {
				string msg = "I_MUST_BE_SUBSCRIBED_TO_MultiSplitter, got sender[" + splitter.GetType() + "] //splitter_MouseEnter()";
				Assembler.PopupException(msg);
				return;
			}
			this.splitterMouseIsOverNow = splitter;
			splitter.BackColor = ColorBackgroundSliderMouseOver;
			splitter.Invalidate();
		}
		void splitter_MouseLeave(object sender, EventArgs e) {
			MultiSplitter splitter = sender as MultiSplitter;
			if (splitter == null) {
				string msg = "I_MUST_BE_SUBSCRIBED_TO_MultiSplitter, got sender[" + splitter.GetType() + "] //splitter_MouseLeave()";
				Assembler.PopupException(msg);
				return;
			}
			if (this.splitterStartedResizeOrDrag != null) {
				if (this.DebugSplitter) {
					splitter.Text = "<leftThisSplitterWhileDragging>";
				}
				return;
			}

			this.splitterMouseIsOverNow = null;
			this.panelMouseIsOverNowIndexDropTarget = -1;
			base.Cursor = Cursors.Default;
			splitter.BackColor = ColorBackgroundSliderRegular;
			splitter.Invalidate();
				
			if (this.DebugSplitter) {
				splitter.Text = "<leftThisSplitter>";
				splitter.Invalidate();
				//base.Invalidate();
			}
		}

		void splitter_MouseDown(object sender, MouseEventArgs e) {
			MultiSplitter splitter = sender as MultiSplitter;
			if (splitter == null) {
				string msg = "I_MUST_BE_SUBSCRIBED_TO_MultiSplitter, got sender[" + splitter.GetType() + "] //splitter_MouseDown()";
				Assembler.PopupException(msg);
				return;
			}
			
			bool someCondition = this.VerticalizeAllLogic == false
				? e.X <= GrabHandleWidth
				: e.Y <= GrabHandleWidth;
			if (someCondition) {
				this.splitterIsDraggingNow = true;
			} else {
				this.splitterIsMovingNow = true;
			}
			this.splitterStartedResizeOrDrag = splitter;
			this.splitterStartedResizeOrDragPoint = this.VerticalizeAllLogic == false
				? new Point(splitter.Location.X + e.X, splitter.Location.Y + e.Y - splitter.Height)
				: new Point(splitter.Location.X + e.X - splitter.Width, splitter.Location.Y + e.Y);
			if (this.DebugSplitter) {
				splitter.Text = this.splitterStartedResizeOrDragText;
				splitter.Invalidate();		// makes the Text visible
			}
			if (this.splitterIsDraggingNow) {
				this.RaiseOnSplitterDragStarted(splitter);
			} else {
				this.RaiseOnSplitterMoveStarted(splitter);
			}
		}
		void splitter_MouseUp(object sender, MouseEventArgs e) {
			MultiSplitter splitter = sender as MultiSplitter;
			if (splitter == null) {
				string msg = "I_MUST_BE_SUBSCRIBED_TO_MultiSplitter, got sender[" + splitter.GetType() + "] //splitter_MouseUp()";
				Assembler.PopupException(msg);
				return;
			}
			if (this.splitterIsDraggingNow) {
				if (this.panelMouseIsOverNow == null) {
					string msg = "SINGLE_CLICK_ON_YELLOW_WITHOUT_DRAGGING?";
					Assembler.PopupException(msg, null, false);
					return;
				} else {
					int indexToMoveFrom = this.splitters.IndexOf(this.splitterStartedResizeOrDrag);
					int indexToMoveTo   = this.panels.IndexOf(this.panelMouseIsOverNow);
					if (indexToMoveFrom != indexToMoveTo) {
						this.panels		.Move(indexToMoveFrom, indexToMoveTo);
						this.splitters	.Move(indexToMoveFrom, indexToMoveTo);
						this.AssignPanelBelowAbove_setMinimalSize_fromPanelsList();
						this.DistributePanelsAndSplitters();
					}
				}
			}

			if (this.splitterIsDraggingNow) {
				this.RaiseOnSplitterDragEnded(splitter);
			} else {
				this.RaiseOnSplitterMoveEnded(splitter);
			}

			this.splitterStartedResizeOrDrag = null;
			this.splitterIsDraggingNow = false;
			this.splitterIsMovingNow = false;
			this.splitterStartedResizeOrDragPoint = new Point(-1, -1);

			if (this.DebugSplitter) {
				splitter.Text = this.splitterStartedResizeOrDragText;
				splitter.Invalidate();		// makes the Text visible
			}
			//WHAT_IF_I_DONT base.Invalidate();
			//base.Refresh();
		}

		protected override void OnInvalidated(InvalidateEventArgs e) {
			//base.OnInvalidated(e);
			foreach (var panel		in this.panels)		panel.Invalidate();
			foreach (var splitter	in this.splitters) splitter.Invalidate();
		}
		
		void splitterDraggingNow_PanelsSwap(MultiSplitter splitter, Point mousePositionFromSplitContainerUpperLeft) {
			//splitter.Text += " dragging";
			// I_HATE_HACKING_WINDOWS_FORMS mousedrag doesn't fire panel_MouseEnter()/panel_MouseLeave() even if the mouse is above them now, simulating it manually here
			Control panelMouseOvered = null;
			//foreach (Control panel in this.panels) {
			for (int i=0; i<this.panels.Count; i++) {
				Control panel = this.panels[i];
				MultiSplitter splitterAbove = this.splitters[i];
				Point offsetted = new Point(mousePositionFromSplitContainerUpperLeft.X - panel.Location.X,
								  mousePositionFromSplitContainerUpperLeft.Y - panel.Location.Y);
				bool beyondUpperLeft = offsetted.X < 0 || offsetted.Y < 0;
				Rectangle panelAndSplitterRect = this.VerticalizeAllLogic
					? new Rectangle(0, 0, panel.Width, panel.Height + splitterAbove.Height)
					: new Rectangle(0, 0, panel.Width + splitterAbove.Width, panel.Height);
				bool doesntContain = (beyondUpperLeft) ? true : (panelAndSplitterRect.Contains(offsetted) == false);
				if (beyondUpperLeft || doesntContain) {
					if (this.panelMouseIsOverNow != null) {
						string msg = "panel_MouseLeave should've set this.panelMouseIsOverNow=null";
						//Debugger.Break();
						continue;
					}
					//if (this.panelMouseIsOverNow == panel) {
						this.panel_MouseLeave(panel, null);
					//} else {
						//Debugger.Break();
					//}
				} else {
					if (panelMouseOvered != null) {
						string msg = "did you find the second panel mouseovered simultaneously???..";
						//Debugger.Break();
					}
					panelMouseOvered = panel;
				}
			}
			if (panelMouseOvered == null) return;
			if (this.panelMouseIsOverNow != null) {
				string msg = "panel_MouseLeave should've set this.panelMouseIsOverNow=null";
				//Debugger.Break();
				this.panel_MouseLeave(this.panelMouseIsOverNow, null);
				this.panelMouseIsOverNow = null;
			}
			this.panel_MouseEnter(panelMouseOvered, null);
			this.RaiseOnSplitterDraggingNow(splitter);
		}
		void splitterMovingNow_PanelsResize(MultiSplitter splitter, Point mousePositionFromSplitContainerUpperLeft) {
			int splitterIndex = this.splitters.IndexOf(splitter);
			if (splitterIndex == 0) return;
			
			Control panelBelow = this.panels[splitterIndex];
			Control panelAbove = this.panels[splitterIndex-1];

			int thingsIchanged = 0;
			if (this.VerticalizeAllLogic == false) {
				// mouseMovingUp CAN BE NEGATIVE
				int mouseMovingUp = this.splitterStartedResizeOrDragPoint.Y - mousePositionFromSplitContainerUpperLeft.Y;
				if (mouseMovingUp > 0 && panelAbove.Height <= 5) return;
				if (mouseMovingUp < 0 && panelBelow.Height <= 5) return;
				if (mouseMovingUp < 0 && splitterIndex == this.splitters.Count - 1) {
					int baseHeight = base.Height;
					if (splitter.Location.Y + splitter.Height >= base.Height - 5) {
						return;
					}
				}

				int panelAboveMinimumHeight = this.MinimumPanelHeight;
				//MultiSplitContainer multiSplitContainerAbove = panelAbove as MultiSplitContainer;
				//if (multiSplitContainerAbove != null) {		// I avoid setting MinimalSize for the MultiSplitContainer itself
				//	string msg = "CALCULATING_SUM_OF_MIN_HEIGHTS_AMONG_NESTED_PANELS__ABOVE_SPLITTER";
				//	panelAboveMinimumHeight = multiSplitContainerAbove.MinimalPanelHeights_SumIfHorizontal_MaxIfVertical;
				//} else {
					if (panelAbove.MinimumSize != default(Size)) {
						panelAboveMinimumHeight = panelAbove.MinimumSize.Height;
					}
				//}
				int panelAboveHeightProjected = panelAbove.Height - mouseMovingUp; 
				if (panelAboveHeightProjected < panelAboveMinimumHeight) {
					Cursor.Current = Cursors.No;
					return;
				}
				
				int panelBelowMinimumHeight = this.MinimumPanelHeight;
				//MultiSplitContainer multiSplitContainerBelow = panelBelow as MultiSplitContainer;
				//if (multiSplitContainerBelow != null) {		// I avoid setting MinimalSize for the MultiSplitContainer itself
				//	string msg = "CALCULATING_SUM_OF_MIN_HEIGHTS_AMONG_NESTED_PANELS__BELOW_SPLITTER";
				//	panelAboveMinimumHeight = multiSplitContainerBelow.MinimalPanelHeights_SumIfHorizontal_MaxIfVertical;
				//} else {
					if (panelBelow.MinimumSize != default(Size)) {
						panelBelowMinimumHeight = panelBelow.MinimumSize.Height;
					}
				//}
				int panelBelowHeightProjected = panelBelow.Height + mouseMovingUp;
				if (panelBelowHeightProjected < panelBelowMinimumHeight) {
					Cursor.Current = Cursors.No;
					return;
				}
				
				Cursor.Current = Cursors.HSplit;

				int panelBelowNewHeight = panelBelow.Height + mouseMovingUp;
				int panelAboveNewHeight = panelAbove.Height - mouseMovingUp;

				//PanelBase panelBaseBelow = panelBelow as PanelBase;
				//MultiSplitContainer multiSplitContainerBelow = panelBelow as MultiSplitContainer;
				//if (panelBaseBelow != null) {
				//	panelBaseBelow			.SetHeightIgnoreResize(panelBelowNewHeight);
				//} else if (multiSplitContainerBelow != null) {
				//	multiSplitContainerBelow.SetHeightIgnoreResize(panelBelowNewHeight);
				//} else {
					if (panelBelow.Height != panelBelowNewHeight) {
						panelBelow.Height  = panelBelowNewHeight;	// will invoke OnResize() => Distribute()
						thingsIchanged++;
					}
				//}

				//v2 REPLACEMENT_FOR_DistributePanelsAndSplitters()_BELOW
				Point panelBelowLocation = new Point(panelBelow.Location.X, panelBelow.Location.Y - mouseMovingUp);
				panelBelow.Location = panelBelowLocation;
				Point splitterLocation = new Point(splitter.Location.X, splitter.Location.Y - mouseMovingUp);
				splitter.Location = splitterLocation;
				thingsIchanged++;
				//v2 END

				//PanelBase panelBaseAbove = panelAbove as PanelBase;
				//MultiSplitContainer multiSplitContainerAbove = panelAbove as MultiSplitContainer;
				//if (panelBaseAbove != null) {
				//	panelBaseAbove			.SetHeightIgnoreResize(panelAboveNewHeight);
				//} else if (multiSplitContainerAbove != null) {
				//	multiSplitContainerAbove.SetHeightIgnoreResize(panelAboveNewHeight);
				//} else {
					if (panelAbove.Height != panelAboveNewHeight) {
						panelAbove.Height  = panelAboveNewHeight;	// will invoke OnResize() => Distribute()
						thingsIchanged++;
					}
				//}


			} else {
				int mouseMovedLeftFromPointClicked = this.splitterStartedResizeOrDragPoint.X - mousePositionFromSplitContainerUpperLeft.X;
				if (mouseMovedLeftFromPointClicked > 0 && panelAbove.Width <= 5) return;
				if (mouseMovedLeftFromPointClicked < 0 && panelBelow.Width <= 5) return;
				if (mouseMovedLeftFromPointClicked < 0 && splitterIndex == this.splitters.Count - 1) {
					int baseHeight = base.Width;
					if (splitter.Location.X + splitter.Width >= base.Width - 5) {
						return;
					}
				}
				int panelAboveMinimumWidth = this.MinimumPanelHeight;
				//MultiSplitContainer multiSplitContainerAbove = panelAbove as MultiSplitContainer;
				//if (multiSplitContainerAbove != null) {		// I avoid setting MinimalSize for the MultiSplitContainer itself
				//	string msg = "CALCULATING_SUM_OF_MIN_WIDTHS_AMONG_NESTED_PANELS__ABOVE_SPLITTER";
				//	panelAboveMinimumWidth = multiSplitContainerAbove.MinimalPanelWidths_SumIfVertical_MaxIfHorizontal;
				//} else {
					if (panelAbove.MinimumSize != default(Size)) {
						panelAboveMinimumWidth = panelAbove.MinimumSize.Width;
					}
				//}
				int panelAboveWidthProjected = panelAbove.Width - mouseMovedLeftFromPointClicked; 
				if (panelAboveWidthProjected < panelAboveMinimumWidth) {
					Cursor.Current = Cursors.No;
					return;
				}
				
				int panelBelowMinimumWidth = this.MinimumPanelHeight;
				//MultiSplitContainer multiSplitContainerBelow = panelBelow as MultiSplitContainer;
				//if (multiSplitContainerBelow != null) {		// I avoid setting MinimalSize for the MultiSplitContainer itself
				//	string msg = "CALCULATING_SUM_OF_MIN_WIDTHS_AMONG_NESTED_PANELS__BELOW_SPLITTER";
				//	panelBelowMinimumWidth = multiSplitContainerBelow.MinimalPanelWidths_SumIfVertical_MaxIfHorizontal;
				//} else {
					if (panelBelow.MinimumSize != default(Size)) {
						panelBelowMinimumWidth = panelBelow.MinimumSize.Width;
					}
				//}
				//panelBelow might be another MultiSplitContainer
				int panelBelowWidthProjected = panelBelow.Width + mouseMovedLeftFromPointClicked;
				if (panelBelowWidthProjected < panelBelowMinimumWidth) {
					Cursor.Current = Cursors.No;
					return;
				}
				
				Cursor.Current = Cursors.VSplit;

				int panelBelowNewWidth = panelBelow.Width + mouseMovedLeftFromPointClicked;
				int panelAboveNewWidth = panelAbove.Width - mouseMovedLeftFromPointClicked;

				//PanelBase panelBaseBelow = panelBelow as PanelBase;
				//MultiSplitContainer multiSplitContainerBelow = panelBelow as MultiSplitContainer;
				//if (panelBaseBelow != null) {
				//	panelBaseBelow			.SetWidthIgnoreResize(panelBelowNewWidth);
				//} else if (multiSplitContainerBelow != null) {
				//	multiSplitContainerBelow.SetWidthIgnoreResize(panelBelowNewWidth);
				//} else {
					if (panelBelow.Width != panelBelowNewWidth) {
						panelBelow.Width  = panelBelowNewWidth;	// will invoke OnResize() => Distribute()
						thingsIchanged++;
					}
				//}

				//v2 REPLACEMENT_FOR_DistributePanelsAndSplitters()_BELOW
				Point panelBelowLocation = new Point(panelBelow.Location.X - mouseMovedLeftFromPointClicked, panelBelow.Location.Y);
				panelBelow.Location = panelBelowLocation;
				Point splitterLocation = new Point(splitter.Location.X - mouseMovedLeftFromPointClicked, splitter.Location.Y);
				splitter.Location = splitterLocation;
				thingsIchanged++;
				//v2 END

				//PanelBase panelBaseAbove = panelAbove as PanelBase;
				//MultiSplitContainer multiSplitContainerAbove = panelAbove as MultiSplitContainer;
				//if (panelBaseAbove != null) {
				//	panelBaseAbove			.SetWidthIgnoreResize(panelAboveNewWidth);
				//} else if (multiSplitContainerAbove != null) {
				//	multiSplitContainerAbove.SetWidthIgnoreResize(panelAboveNewWidth);
				//} else {
					if (panelAbove.Width != panelAboveNewWidth) {
						panelAbove.Width  = panelAboveNewWidth;	// will invoke OnResize() => Distribute()
						thingsIchanged++;
					}
				//}
			}

			//v1 this.DistributePanelsAndSplitters();		// otherwize black rectangles flying unproportionally mouse move <= splitter must move and location of the right/bottom pane (above we set only the Widths)
			//v2 REPLACEMENT_FOR_DistributePanelsAndSplitters()

			if (thingsIchanged == 0) {
				string msg = "I_WAS_USELESS__REMOVE_MY_INVOCATION_FROM_UPSTACK //splitterMovingNow_PanelsResize()";
				#if DEBUG_HEAVY
				Assembler.PopupException(msg, null, false);
				#endif
			}

			base.Invalidate();

			// otherwize there are 1-3px painted black :(
			panelBelow.Invalidate();
			panelAbove.Invalidate();
			splitter.Invalidate();

			this.RaiseOnSplitterMovingNow(splitter);
		}
	}
}
