using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;

namespace Sq1.Charting {
	public partial class ChartControl	{
		protected override void OnResize(EventArgs e) {
			if (base.DesignMode) return;
			if (Assembler.IsInitialized == false) return;
			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) return;
			if (this.ScrollLargeChange <= 0) {
				//Debugger.Break();	// HAPPENS_WHEN_WINDOW_IS_MINIMIZED OR BEFORE_FIRST_PAINT_SETS_GutterRightWidth_cached... how to disable any OnPaint when app isn't visible?... 
				return;
			}
			this.hScrollBar.LargeChange = this.ScrollLargeChange;
			base.OnResize(e);	// will invoke UserControlDoubleBuffered.OnResize() if you inherited so here you are DoubleBuffer-safe
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
		protected override void OnMouseLeave(EventArgs e) {
			if (base.DesignMode) return;
			base.OnMouseLeave(e);

			// DOESNT_WORK this.InvalidateAllPanels();	//	DRAWING_CURRENT_JUMPING_STREAMING_VALUE_ON_GUTTER_SINCE_MOUSE_WENT_OUT_OF_BOUNDARIES
		}
		#region ProcessCmdKey is a filter OnKeyDown should go together
//DUE_TO_STOPPED_WORKING_REPLACED_BY_ProcessCmdKey	BEGIN
//		protected override bool IsInputKey(Keys keyData) {
//			switch (keyData) {
//				case Keys.Right:
//				case Keys.Left:
//				case Keys.Up:
//				case Keys.Down:
//					return true;
////				case Keys.Shift | Keys.Right:
////				case Keys.Shift | Keys.Left:
////				case Keys.Shift | Keys.Up:
////				case Keys.Shift | Keys.Down:
////					return true;
//			}
//			return base.IsInputKey(keyData);
//		}
//		protected override void OnKeyDown(KeyEventArgs keyEventArgs) {
//			Debugger.Break();
//			if (this.BarsEmpty) return;
//			this.keysToReaction(keyEventArgs.KeyCode);
//			base.OnKeyDown(keyEventArgs);
//		}
//		public void OnKeyDownPush(KeyEventArgs keyEventArgs) {
//			this.OnKeyDown(keyEventArgs);
//		}
//DUE_TO_STOPPED_WORKING_REPLACED_BY_ProcessCmdKey	END
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			this.keysToReaction(keyData);
			return base.ProcessCmdKey(ref msg, keyData);
		}
		
		void keysToReaction(Keys keyData) {
			//Debugger.Break();
			switch (keyData) {
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
				default:
					break;
			}
		}
		#endregion

		void hScrollBar_Scroll(object sender, ScrollEventArgs scrollEventArgs) {
			if (this.Bars == null) {
				#if DEBUG
				string msg = "POSSIBLY_DISABLE_SCROLLBAR_WHEN_CHART_HAS_NO_BARS? OR MAKE_CHART_ALWAYS_DISPLAY_BARS";
				Assembler.PopupException(msg, null, false);
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
				this.RaiseOnChartSettingsChanged_containerShouldSerialize();	//scrollbar should have OnDragCompleteMouseReleased event!!!
			}
		}
		void bars_symbolInfo_PriceDecimalsChanged(object sender, EventArgs e) {
			this.InvalidateAllPanels();
		}

		//void chartControl_BarStreamingUpdatedMerged_ShouldTriggerRepaint_WontUpdateBtnTriggeringScriptTimeline(object sender, BarEventArgs e) {
		public void ExecutorObjectsReady_triggerRepaint__raiseOnBarStreamingUpdatedMerged_chartFormPrintsQuoteTimestamp() {
			// if I was designing events for WinForms, I would switch to GUI thread automatically
			if (base.InvokeRequired == true) {
			    //base.BeginInvoke((MethodInvoker)delegate { this.ChartControl_BarStreamingUpdatedMerged_ShouldTriggerRepaint_WontUpdateBtnTrigeringScriptTimeline(); });
			    base.BeginInvoke(new MethodInvoker(this.ExecutorObjectsReady_triggerRepaint__raiseOnBarStreamingUpdatedMerged_chartFormPrintsQuoteTimestamp));
			    return;
			}
	
			//if (this.VisibleBarRight != this.Bars.Count - 1) {
			//    string msg = "I_WILL_MOVE_SLIDER_IF_ONLY_LAST_BAR_IS_VISIBLE";
			//    //I_WILL_MOVE_ANYWAYS__WE_ARE_HERE_WHEN_PAUSED_LIVESIM_WAS_HSCROLLED_BACKWARDS return;
			//}
			//string msg1 = "IM_MOVING_SLIDER_TO_THE_RIGHTMOST_BAR_KOZ_WE_ARE_ON_LAST_BAR";
			
			//PAS_BESION??? this.SyncHorizontal_scrollToBarsCount();
			this.InvalidateAllPanels();
			// UPDATED_VIA_ PrintQuoteTimestamp_onStrategyTriggeringButton_beforeExecution_switchToGuiThread() updating 00:00:00.000 on ChartForm.btnStreamingTriggersScript
			base.RaiseOnBarStreamingUpdatedMerged_chartFormPrintsQuoteTimestamp(new BarEventArgs(base.Bars.BarStreaming_nullUnsafeCloneReadonly));

			if (this.splitContainerChartVsRange.Panel2Collapsed == true) {
				string msg = "YES_splitContainerChartVsRange.Panel2Collapsed_WAS_THE_ONE WAS_THAT_THE_RIGHT_VISIBILITY_CRITERION???";
				//Assembler.PopupException(msg);
				return;
			}
			this.RangeBar.Invalidate();
		}

		void multiSplitContainerColumns_OnResizing_OnSplitterMoveOrDragEnded(object sender, EventArgs e) {
			if (base.DesignMode) return;
			if (this.ChartSettings == null) return;	// MAY_BE_REDUNDANT
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;
			//WORKS_WHEN_COMMENTED_SURPRISE if (Assembler.InstanceInitialized.SplitterEventsAreAllowedNsecAfterLaunchHopingInitialInnerDockResizingIsFinished == false) return;
			this.SerializeSplitterDistanceOrPanelName();
		}
		// WHERE_IS_RESIZE_ENDED_IN_F_WINDOWS_FORMS?? SAVING_CHART_SETTINGS_ON_EACH_TINY_RESIZE_FOR_ALL_OPEN_CHARTS this.multiSplitContainer.Resize += new EventHandler(multiSplitContainer_OnResizing_OnSplitterMoveOrDragEnded);
		void multiSplitContainerRows_OnResizing_OnSplitterMoveOrDragEnded(object sender, EventArgs e) {		//MultiSplitterEventArgs e
			if (base.DesignMode) return;
			if (this.ChartSettings == null) return;	// MAY_BE_REDUNDANT
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;
			//WORKS_WHEN_COMMENTED_SURPRISE if (Assembler.InstanceInitialized.SplitterEventsAreAllowedNsecAfterLaunchHopingInitialInnerDockResizingIsFinished == false) return;
			this.SerializeSplitterDistanceOrPanelName();
		}
		public void SerializeSplitterDistanceOrPanelName() {
			//if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) {
			//	return;
			//}
			this.ChartSettings.MultiSplitterRowsPropertiesByPanelName		= this.multiSplitRowsVolumePrice		.SplitterPropertiesByPanelNameGet();
			this.ChartSettings.MultiSplitterColumnsPropertiesByPanelName	= this.multiSplitColumns_Level2_PriceVolumeMultisplit	.SplitterPropertiesByPanelNameGet();
			// that will show that 10s delay actually makes better sense than relying on MainFormDockFormsFullyDeserializedLayoutComplete in ChartControl.PropagateSplitterManorderDistanceIfFullyDeserialized()
			//try {
			//	int justCurious = this.ChartSettings.MultiSplitterPropertiesByPanelName[this.panelVolume.PanelName].Distance;
			//	Debugger.Break();
			//} catch (Exception ex) {
			//	Assembler.PopupException(null, ex);
			//}
			this.RaiseOnChartSettingsChanged_containerShouldSerialize();
		}

		void repositoryJsonDataSources_OnSymbolRemoved_clearChart(object sender, DataSourceSymbolEventArgs e) {
			string msig = " //ChartControl.repositoryJsonDataSource_OnSymbolRemoved_clearChart(" + e.Symbol + ") chart[" + this.ToString() + "]";
			if (this.Bars.DataSource != e.DataSource) {
				string msg1 = "IGNORING_DELETION_OTHER_DATASOURCE_NOT_IM_ACTUALLY_DISPLAYING"
					+ " this.Bars.DataSource[" + this.Bars.DataSource + "] != e.DataSource[" + e.DataSource + "]";
				//I_REALLY_DONT_CARE__WHY_SHOULD_I_KNOW?? Assembler.PopupException(msg1 + msig, null, false);
				return;
			}
			if (this.Bars.Symbol != e.Symbol) {
				string msg1 = "IGNORING_DELETION_OTHER_SYMBOL_NOT_IM_ACTUALLY_DISPLAYING"
					+ " this.Bars.Symbol[" + this.Bars.Symbol + "] != e.Symbol[" + e.Symbol + "]";
				//I_REALLY_DONT_CARE__WHY_SHOULD_I_KNOW?? Assembler.PopupException(msg1 + msig, null, false);
				return;
			}
			string msg = "MY_SYMBOL_DELETED__INITIALIZING_WITH_BARS_NULL_TO_RESET";
			Assembler.PopupException(msg + msig, null, false);
			this.Initialize(null, this.ChartSettings.StrategyName);
		}

		protected override void OnLayout(System.Windows.Forms.LayoutEventArgs levent) {
			// INVOKERS: base.AutoScroll=false, ChartControl.PerformLayout()
			base.OnLayout(levent);
			this.simulateDockFill();
		}

		void simulateDockFill() {
			string msig = " //simulateDockFill()";
			Form parentForm = base.Parent as Form;
			if (parentForm == null) {
				string msg = "YOU_INVOKED_ChartControl.Initialize()_FROM_ChartControl.ctor(RANDOM_GENERATED_BARS) CHART_CONTROL_NOT_ADDED_TO_ANY_FORM";
				#if DEBUG_HEAVY
				Assembler.PopupException(msg + msig, null, false);
				#endif
				return;
			}
			if (base.ClientRectangle.Width == parentForm.ClientRectangle.Width) return;		// looks already FILLed
			string msg2 = "HAPPENS_WHEN_??? ";
			#if DEBUG_HEAVY
			Assembler.PopupException(msg2 + msig, null, false);
			#endif
			parentForm.PerformLayout();		// did it help to get the Control stretched to fill the surface of the form?
		}
	}
}
