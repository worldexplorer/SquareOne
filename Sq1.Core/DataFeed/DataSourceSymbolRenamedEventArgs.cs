using System;
namespace Sq1.Core.DataFeed {
	public class DataSourceSymbolRenamedEventArgs : DataSourceSymbolEventArgs {
		public string SymbolOld { get; private set; }

		public DataSourceSymbolRenamedEventArgs(DataSource dataSource, string symbolNew, string symbolOld) : base(dataSource, symbolNew) {
			this.SymbolOld = symbolOld;
		}
	}
}
