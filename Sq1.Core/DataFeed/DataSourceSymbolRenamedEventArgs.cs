using System;

namespace Sq1.Core.DataFeed {
	public class DataSourceSymbolRenamedEventArgs : DataSourceSymbolEventArgs {
		public string SymbolOld { get; private set; }
		public bool CancelRepositoryRenameExecutorRefusedToRenameWasStreamingTheseBars;

		public DataSourceSymbolRenamedEventArgs(DataSource dataSource, string symbolNew, string symbolOld) : base(dataSource, symbolNew) {
			CancelRepositoryRenameExecutorRefusedToRenameWasStreamingTheseBars = false;
			SymbolOld = symbolOld;
		}
	}
}
