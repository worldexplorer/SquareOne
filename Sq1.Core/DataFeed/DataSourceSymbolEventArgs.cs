using System;

namespace Sq1.Core.DataFeed {
	public class DataSourceSymbolEventArgs : DataSourceEventArgs {
		public bool		ReScanFolderForBarsFiles;
		public string	Symbol			{ get; private set; }

		public DataSourceSymbolEventArgs(DataSource dataSource, string symbol, bool reScanFolderForBarsFiles = false) : base(dataSource) {
			this.Symbol = symbol;
			this.ReScanFolderForBarsFiles = reScanFolderForBarsFiles;
		}
	}
}
