using System;

using Sq1.Core.DataFeed;

namespace Sq1.Core.Repositories {
	// the hackiest class in the whole solution :(
	public partial class RepositoryJsonDataSources {
		public event EventHandler<DataSourceSymbolEventArgs>		OnSymbolAdded;
		public event EventHandler<DataSourceSymbolRenamedEventArgs>	OnSymbolRenamed;
		public event EventHandler<DataSourceSymbolEventArgs>		OnSymbolCanBeRemoved;
		public event EventHandler<DataSourceSymbolEventArgs>		OnSymbolRemovedDone;

		public void RaiseOnSymbolAdded(object sender, DataSource dataSource, string symbolToAdd) {
			if (this.OnSymbolAdded == null) return;
			this.OnSymbolAdded(sender, new DataSourceSymbolEventArgs(dataSource, symbolToAdd));
		}
		public void RaiseOnSymbolRenamed(object sender, DataSource dataSource, string newSymbolName, string oldSymbolName) {
			if (this.OnSymbolRenamed == null) return;
			this.OnSymbolRenamed(sender, new DataSourceSymbolRenamedEventArgs(dataSource, newSymbolName, oldSymbolName));
		}
		public void RaiseOnSymbolCanBeDeleted(object sender, DataSourceSymbolEventArgs args) {
			if (this.OnSymbolCanBeRemoved == null) return;
			this.OnSymbolCanBeRemoved(sender, args);
		}
		public void RaiseOnSymbolRemovedDone(object sender, DataSourceSymbolEventArgs args) {
			if (this.OnSymbolRemovedDone == null) return;
			this.OnSymbolRemovedDone(sender, args);
		}
	}
}