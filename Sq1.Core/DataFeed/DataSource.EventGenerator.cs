using System;

namespace Sq1.Core.DataFeed {
	public partial class DataSource {
		public event EventHandler<DataSourceEventArgs>				OnDataSourceEditedChartsDisplayedShouldRunBacktestAgain;
		public event EventHandler<DataSourceSymbolRenamedEventArgs> OnSymbolRenamed_eachExecutorShouldRenameItsBars_saveStrategyIfNotNull;

		public void RaiseOnDataSourceEdited_chartsDisplayedShouldRunBacktestAgain() {
			if (this.OnDataSourceEditedChartsDisplayedShouldRunBacktestAgain == null) return;
			this.OnDataSourceEditedChartsDisplayedShouldRunBacktestAgain(this, new DataSourceEventArgs(this));
		}
		public bool RaiseOnSymbolRenamed_eachExecutorShouldRenameItsBars_saveStrategyIfNotNull(string oldSymbolName, string newSymbolName) {
			bool ret = false;	// No Obstacles by default; no charts open with oldSymbolName => cancel=true 
			if (this.OnSymbolRenamed_eachExecutorShouldRenameItsBars_saveStrategyIfNotNull == null) return ret;
			DataSourceSymbolRenamedEventArgs args = new DataSourceSymbolRenamedEventArgs(this, newSymbolName, oldSymbolName);
			this.OnSymbolRenamed_eachExecutorShouldRenameItsBars_saveStrategyIfNotNull(this, args);
			ret = args.CancelRepositoryRename_oneExecutorRefusedToRename_wasStreamingTheseBars;
			return ret;
		}
	}
}
