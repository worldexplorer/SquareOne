using System;
using System.Windows.Forms;

namespace Sq1.Charting.MultiSplit {
	public partial class MultiSplitContainer<PANEL_NAMED_FOLDING> {
		public EventHandler<MultiSplitterEventArgs> OnSplitterMoveStarted;
		public EventHandler<MultiSplitterEventArgs> OnSplitterMovingNow;
		public EventHandler<MultiSplitterEventArgs> OnSplitterMoveEnded;
		public EventHandler<MultiSplitterEventArgs> OnSplitterDragStarted;
		public EventHandler<MultiSplitterEventArgs> OnSplitterDraggingNow;
		public EventHandler<MultiSplitterEventArgs> OnSplitterDragEnded;
		
		public void RaiseOnSplitterMoveStarted(MultiSplitter multiSplitter) {
			if (this.OnSplitterMoveStarted == null) return;
			this.OnSplitterMoveStarted(this, new MultiSplitterEventArgs(multiSplitter));
		}
		public void RaiseOnSplitterMovingNow(MultiSplitter multiSplitter) {
			if (this.OnSplitterMovingNow == null) return;
			this.OnSplitterMovingNow(this, new MultiSplitterEventArgs(multiSplitter));
		}
		public void RaiseOnSplitterMoveEnded(MultiSplitter multiSplitter) {
			if (this.OnSplitterMoveEnded == null) return;
			this.OnSplitterMoveEnded(this, new MultiSplitterEventArgs(multiSplitter));
		}

		public void RaiseOnSplitterDragStarted(MultiSplitter multiSplitter) {
			if (this.OnSplitterDragStarted == null) return;
			this.OnSplitterDragStarted(this, new MultiSplitterEventArgs(multiSplitter));
		}
		public void RaiseOnSplitterDraggingNow(MultiSplitter multiSplitter) {
			if (this.OnSplitterDraggingNow == null) return;
			this.OnSplitterDraggingNow(this, new MultiSplitterEventArgs(multiSplitter));
		}
		public void RaiseOnSplitterDragEnded(MultiSplitter multiSplitter) {
			if (this.OnSplitterDragEnded == null) return;
			this.OnSplitterDragEnded(this, new MultiSplitterEventArgs(multiSplitter));
		}
	}
}
