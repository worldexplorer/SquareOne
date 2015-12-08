using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataFeed;

namespace Sq1.Charting {
	public partial class ChartControl	{
		protected override void OnResize(EventArgs e) {
			if (base.DesignMode) return;
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
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
			this.ChartSettings.MultiSplitterRowsPropertiesByPanelName		= this.multiSplitContainerRows		.SplitterPropertiesByPanelNameGet();
			this.ChartSettings.MultiSplitterColumnsPropertiesByPanelName	= this.multiSplitContainerColumns	.SplitterPropertiesByPanelNameGet();
			// that will show that 10s delay actually makes better sense than relying on MainFormDockFormsFullyDeserializedLayoutComplete in ChartControl.PropagateSplitterManorderDistanceIfFullyDeserialized()
			//try {
			//	int justCurious = this.ChartSettings.MultiSplitterPropertiesByPanelName[this.panelVolume.PanelName].Distance;
			//	Debugger.Break();
			//} catch (Exception ex) {
			//	Assembler.PopupException(null, ex);
			//}
			this.RaiseChartSettingsChangedContainerShouldSerialize();
		}

		void repositoryJsonDataSource_OnSymbolRemoved_clearChart(object sender, DataSourceSymbolEventArgs e) {
			string msig = " //repositoryJsonDataSource_OnSymbolRemoved_clearChart(" + e.Symbol + ") chart[" + this.ToString() + "]";
			if (this.Bars.DataSource != e.DataSource) {
				string msg = "IGNORING_DELETION_OTHER_DATASOURCE_NOT_IM_ACTUALLY_DISPLAYING"
					+ " this.Bars.DataSource[" + this.Bars.DataSource + "] != e.DataSource[" + e.DataSource + "]";
				#if DEBUG
				Assembler.PopupException(msg + msig, null, false);
				#endif
				return;
			}
			if (this.Bars.Symbol != e.Symbol) {
				string msg = "IGNORING_DELETION_OTHER_SYMBOL_NOT_IM_ACTUALLY_DISPLAYING"
					+ " this.Bars.Symbol[" + this.Bars.Symbol + "] != e.Symbol[" + e.Symbol + "]";
				#if DEBUG
				Assembler.PopupException(msg + msig);
				#endif
				return;
			}
			this.Initialize(null, this.ChartSettings.StrategyName);
		}
	}
}
