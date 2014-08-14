using System;

namespace Sq1.Core.DataFeed {
	public class DataSourceEventArgs : NamedObjectJsonEventArgs<DataSource>  {
		//public DataSource DataSource { get; private set; }
		//public DataSourceEventArgs(DataSource dataSource) {
		//    this.DataSource = dataSource;
		//}

		public DataSource DataSource { get { return base.Item; } }
		public DataSourceEventArgs(DataSource dataSource) : base(dataSource) { }
	}
}