using System;

using Sq1.Core.DataFeed;

namespace Sq1.Widgets.DataSourcesTree {
	public partial class DataSourcesTreeControl {
		void dataSourceTreeListViewCustomize() {
			//v2	http://stackoverflow.com/questions/9802724/how-to-create-a-multicolumn-treeview-like-this-in-c-sharp-winforms-app/9802753#9802753
			this.tree.CanExpandGetter = delegate(object o) {
				var dataSource = o as DataSource;
				if (dataSource == null) return false;
				return dataSource.Symbols.Count > 0;
			};
			this.tree.ChildrenGetter = delegate(object o) {
				var dataSource = o as DataSource;
				if (dataSource == null) return null;
				return dataSource.Symbols.ToArray();
			};

			this.olvColumnName.AspectGetter = delegate(object o) {
				var dataSource = o as DataSource;
				if (dataSource == null) return o.ToString();
				string dsName = dataSource.Name;
				if (this.ShowScaleIntervalInsteadOfMarket) {
					dsName += " :: " + dataSource.ScaleInterval;
				} else {
					if (dataSource.MarketInfo != null) {
						dsName += " :: " + dataSource.MarketInfo.Name;
					}
				}
				return dsName;
			};
			this.olvColumnName.ImageGetter = delegate(object o) {
				var dataSource = o as DataSource;
				if (dataSource == null) return null;
				return this.getProviderImageIndexForDataSource(dataSource);
			};
		}
	}
}