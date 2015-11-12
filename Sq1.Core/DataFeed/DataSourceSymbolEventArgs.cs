using System;

namespace Sq1.Core.DataFeed {
	public class DataSourceSymbolEventArgs : DataSourceEventArgs {
		public bool		RebuildDropDown;
		public string	Symbol			{ get; private set; }

		public DataSourceSymbolEventArgs(DataSource dataSource, string symbol, bool rebuildDropDown = false) : base(dataSource) {
			this.Symbol = symbol;
			this.RebuildDropDown = rebuildDropDown;
		}
	}
}
