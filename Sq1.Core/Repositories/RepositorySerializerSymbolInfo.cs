using System.Collections.Generic;
using System.Text.RegularExpressions;

using Sq1.Core.DataTypes;
using Sq1.Core.Serializers;

namespace Sq1.Core.Repositories {
	public class RepositorySerializerSymbolInfo : Serializer<List<SymbolInfo>> {
		public RepositorySerializerSymbolInfo() {}
		public SymbolInfo FindSymbolInfo(string symbol) {
			SymbolInfo ret = null;
			if (string.IsNullOrEmpty(symbol)) return ret;
			foreach (SymbolInfo eachSymbolInfo in base.Entity) {
				if (eachSymbolInfo.Symbol.ToUpper() == symbol.ToUpper()) {
					ret = eachSymbolInfo;
					break;
				}
				if (Regex.IsMatch(symbol, "^" + eachSymbolInfo.Symbol + "$")) {
					ret = eachSymbolInfo;
					break;
				}
			}
			return ret;
		}
		public SymbolInfo FindSymbolInfoOrNew(string symbol) {
			SymbolInfo ret = this.FindSymbolInfo(symbol);
			if (ret == null) ret = new SymbolInfo();
			return ret;
		}
	}
}