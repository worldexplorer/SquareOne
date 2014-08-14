using System;
using System.Collections.Generic;
using System.Text;

namespace Sq1.Core.DataFeed {
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
