using System;

namespace Sq1.Charting.MultiSplit {
	public class MultiSplitterEventArgs : EventArgs {
		MultiSplitter MultiSplitter;
		public MultiSplitterEventArgs(MultiSplitter multiSplitter) {
			MultiSplitter = multiSplitter;
		}
	}
}
