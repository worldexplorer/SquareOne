using System;
using System.Collections.Generic;

namespace Sq1.Core.DataTypes {
	public class SymbolInfoList : List<SymbolInfo> {

		// MY_WAY_TO_AVOID_OVERRIDING_EQUALS
		public bool ContainsSymbol(string anotherSymbol) {
			bool ret = false;
			foreach (SymbolInfo each in base.ToArray()) {
				if (each.Symbol != anotherSymbol) continue;
				ret = true;
				break;
			}
			return ret;
		}
	}
}
