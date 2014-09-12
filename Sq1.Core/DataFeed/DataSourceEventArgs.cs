using System;

namespace Sq1.Core.DataFeed {
	public class DataSourceEventArgs : NamedObjectJsonEventArgs<DataSource>  {
		public DataSource DataSource { get { return base.Item; } }
		public DataSourceEventArgs(DataSource dataSource) : base(dataSource) { }
	}
}