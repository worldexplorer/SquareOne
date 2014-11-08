using System;

using Sq1.Core.DataFeed;

namespace Sq1.Core.Repositories {
	public partial class RepositoryJsonsInFolder<DATASOURCE> {
		public event EventHandler<NamedObjectJsonEventArgs<DATASOURCE>> OnItemAdded;
		public event EventHandler<NamedObjectJsonEventArgs<DATASOURCE>> OnItemRenamed;
		public event EventHandler<NamedObjectJsonEventArgs<DATASOURCE>> OnItemCanBeRemoved;
		public event EventHandler<NamedObjectJsonEventArgs<DATASOURCE>> OnItemRemovedDone;

		public void RaiseOnItemAdded(object sender, DATASOURCE itemCandidate) {
			if (this.OnItemAdded == null) return;
			//this.OnItemAdded(this, new DataSourceEventArgs(itemCandidate));
			this.OnItemAdded(sender, new NamedObjectJsonEventArgs<DATASOURCE>(itemCandidate));
		}
		public void RaiseOnItemRenamed(object sender, DATASOURCE itemStored) {
			if (this.OnItemRenamed == null) return;
			this.OnItemRenamed(sender, new NamedObjectJsonEventArgs<DATASOURCE>(itemStored));
		}
		public void RaiseOnItemCanBeRemoved(object sender, NamedObjectJsonEventArgs<DATASOURCE> args) {
			if (this.OnItemCanBeRemoved == null) return;
			this.OnItemCanBeRemoved(sender, args);
		}
		public void RaiseOnItemRemovedDone(object sender, NamedObjectJsonEventArgs<DATASOURCE> args) {
			if (this.OnItemRemovedDone == null) return;
			// since you answered DataSourceEventArgs.DoNotDeleteThisDataSourceBecauseItsUsedElsewhere=false,
			// you were aware that OnItemRemovedDone is invoked on a detached object
			this.OnItemRemovedDone(sender, args);
		}
	}
}
