using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sq1.Core.DataTypes;
using Sq1.Core.Serializers;

namespace Sq1.Core.Repositories {
	public class RepositoryCustomSymbolInfo : Serializer<List<SymbolInfo>> {
		public RepositoryCustomSymbolInfo() {
		}

		public SymbolInfo FindSymbolInfo(string symbol) {
			if (string.IsNullOrEmpty(symbol)) return null;
			foreach (SymbolInfo current in base.Entity) {
				if (current.Symbol.ToUpper() == symbol.ToUpper()) return current;
				if (Regex.IsMatch(symbol, "^" + current.Symbol + "$")) return current;
			}
			return null;
		}
		public SymbolInfo FindSymbolInfoOrNew(string symbol) {
			SymbolInfo ret = this.FindSymbolInfo(symbol);
			if (ret == null) ret = new SymbolInfo();
			return ret;
		}
	}
}