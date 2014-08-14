using System;
namespace Sq1.Core.DataFeed {
	public class DataSourceSymbolEventArgs : NamedObjectJsonEventArgs<DataSource> {
		//public DataSource DataSource { get; private set; }
		//public string Symbol { get; private set; }

		//public DataSourceSymbolEventArgs(DataSource dataSource, string symbol) {
		//    this.DataSource = dataSource;
		//    this.Symbol = symbol;
		//}

		public DataSource DataSource { get { return base.Item; } }
		public string Symbol { get; private set; }

		public DataSourceSymbolEventArgs(DataSource dataSource, string symbol) : base(dataSource) {
			this.Symbol = symbol;
		}
	}
}
