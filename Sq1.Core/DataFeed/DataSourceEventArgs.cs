using System;

using Sq1.Core.DataTypes;

namespace Sq1.Core.DataFeed {
	public class DataSourceEventArgs : NamedObjectJsonEventArgs<DataSource>  {
		public DataSource DataSource { get { return base.Item; } }
		public DataSourceEventArgs(DataSource dataSource) : base(dataSource) { }
	}
}