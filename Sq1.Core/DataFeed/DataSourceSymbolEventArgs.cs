using System;

namespace Sq1.Core.DataFeed {
	public class DataSourceSymbolEventArgs : DataSourceEventArgs {
		public string Symbol { get; private set; }
		public DataSourceSymbolEventArgs(DataSource dataSource, string symbol) : base(dataSource) {
			this.Symbol = symbol;
		}
	}
}
