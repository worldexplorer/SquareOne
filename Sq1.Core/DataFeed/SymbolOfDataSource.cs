using System;

namespace Sq1.Core.DataFeed {
	public class SymbolOfDataSource {
		public	string		Symbol		{ get; private set; }
		public	DataSource	DataSource	{ get; private set; }

		public SymbolOfDataSource(string symbol, DataSource dataSource) {
			Symbol = symbol;
			DataSource = dataSource;
		}

		public override string ToString() {
			return this.Symbol + "<=" + this.DataSource.Name;
		}
	}
}
