using System;

namespace Sq1.Core.DataFeed {
	public class DataSourceSymbolRenamedEventArgs : DataSourceSymbolEventArgs {
		public string SymbolOld { get; private set; }
		public bool CancelRepositoryRename_oneExecutorRefusedToRename_wasStreamingTheseBars;

		public DataSourceSymbolRenamedEventArgs(DataSource dataSource, string symbolNew, string symbolOld) : base(dataSource, symbolNew) {
			CancelRepositoryRename_oneExecutorRefusedToRename_wasStreamingTheseBars = false;
			SymbolOld = symbolOld;
		}
	}
}
