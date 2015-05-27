using System;
using System.Collections.Generic;

namespace Sq1.Core.DataFeed {
	public class NamedObjectJsonSerializableList : List<NamedObjectJsonSerializable> {
		public bool ContainsName(string anotherName) {
			bool ret = false;
			foreach (NamedObjectJsonSerializable each in base.ToArray()) {
				if (each.Name != anotherName) continue;
				ret = true;
				break;
			}
			return ret;
		}
	}
}
