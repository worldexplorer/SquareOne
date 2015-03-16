using System;

namespace Sq1.Charting.MultiSplit {
	// REASON_TO_EXIST: VS2010 Designer stupily says The type 'Sq1.Charting.MultiSplit.MultiSplitContainer' has no event named 'OnSplitterMoveEnded'.
	// "The variable 'ChartControl' is either undeclared or was never assigned." 
	public partial class MultiSplitContainerOfPanelBase {
// nope let MultiSplitContainerGeneric<PANEL_BASE> do stuff, VS2010 Designer could have picked events from base UserControl !!! if that's the reason of exception in Designer 
//		public new EventHandler<MultiSplitterEventArgs> OnSplitterMoveStarted;
//		public new EventHandler<MultiSplitterEventArgs> OnSplitterMovingNow;
//		public new EventHandler<MultiSplitterEventArgs> OnSplitterMoveEnded;
//		public new EventHandler<MultiSplitterEventArgs> OnSplitterDragStarted;
//		public new EventHandler<MultiSplitterEventArgs> OnSplitterDraggingNow;
//		public new EventHandler<MultiSplitterEventArgs> OnSplitterDragEnded;
	}
}
