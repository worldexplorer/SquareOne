using System;

namespace Sq1.Core.DataFeed {
	public partial class DataSource {
		public event EventHandler<DataSourceEventArgs>				DataSourceEditedChartsDisplayedShouldRunBacktestAgain;
		public event EventHandler<DataSourceSymbolRenamedEventArgs> SymbolRenamedExecutorShouldRenameEachBarSaveStrategyNotBars;

		public void RaiseDataSourceEditedChartsDisplayedShouldRunBacktestAgain() {
			if (this.DataSourceEditedChartsDisplayedShouldRunBacktestAgain == null) return;
			this.DataSourceEditedChartsDisplayedShouldRunBacktestAgain(this, new DataSourceEventArgs(this));
		}
		public bool RaiseSymbolRenamedExecutorShouldRenameEachBarSaveStrategyNotBars(string oldSymbolName, string newSymbolName) {
			bool ret = false;	// No Obstacles by default; no charts open with oldSymbolName => cancel=true 
			if (this.SymbolRenamedExecutorShouldRenameEachBarSaveStrategyNotBars == null) return ret;
			DataSourceSymbolRenamedEventArgs args = new DataSourceSymbolRenamedEventArgs(this, newSymbolName, oldSymbolName);
			this.SymbolRenamedExecutorShouldRenameEachBarSaveStrategyNotBars(this, args);
			ret = args.CancelRepositoryRenameExecutorRefusedToRenameWasStreamingTheseBars;
			return ret;
		}
	}
}
