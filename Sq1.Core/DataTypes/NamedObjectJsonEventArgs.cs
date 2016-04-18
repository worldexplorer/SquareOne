using System;

namespace Sq1.Core.DataTypes {
	public class NamedObjectJsonEventArgs<T> : EventArgs {
		//public string Name;
		public bool DoNotDeleteItsUsedElsewhere;
		public string DoNotDeleteReason;
		public T Item;

		public NamedObjectJsonEventArgs(T dataSource) {
			this.DoNotDeleteReason = "";
			this.Item = dataSource;
		}
	}
}
