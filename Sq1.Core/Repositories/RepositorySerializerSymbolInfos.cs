using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Repositories {
	public class RepositorySerializerSymbolInfos : SerializerList<SymbolInfo> {
		public class ASC : IComparer<SymbolInfo> {
			int IComparer<SymbolInfo>.Compare(SymbolInfo x, SymbolInfo y) { return x.Symbol.CompareTo(y.Symbol); }
		}

		public	List<SymbolInfo>				SymbolInfos { get { return base.EntityDeserialized; } }
				Dictionary<string, SymbolInfo>	lookup_symbolInfos_bySymbol;
				Dictionary<string, SymbolInfo>	lookup_symbolInfos_bySymbol_UPPERCASE;
				IComparer<SymbolInfo>			sorter;

		RepositorySerializerSymbolInfos() : base() {
			lookup_symbolInfos_bySymbol			= new Dictionary<string, SymbolInfo>();
			lookup_symbolInfos_bySymbol_UPPERCASE	= new Dictionary<string, SymbolInfo>();
		}
		public RepositorySerializerSymbolInfos(IComparer<SymbolInfo> sorter_passed = null) : this() {
			if (sorter_passed == null) sorter_passed = new ASC();
			sorter = sorter_passed;
		}

		public SymbolInfo FindSymbolInfo_nullUnsafe(string symbol, bool caseSensitive = false) {
			SymbolInfo ret = null;
			if (string.IsNullOrEmpty(symbol)) return ret;

			//v1 STARTUP_TOO_SLOW__6Bln_TIMES_strings_are_compared
			//foreach (SymbolInfo eachSymbolInfo in this.SymbolInfos) {
			//    if (eachSymbolInfo.Symbol == null) {
			//        string msg = "DELETE_Symbol=null_IN_SYMBOL_INFO_EDITOR";
			//        continue;
			//    }
			//    bool iFoundIt = caseSensitive
			//        ? eachSymbolInfo.Symbol.ToUpper()	== symbol.ToUpper()
			//        : eachSymbolInfo.Symbol				== symbol;
			//    if (iFoundIt == false) continue;
			//    ret = eachSymbolInfo;
			//    break;
			//}

			//v2
			Dictionary<string, SymbolInfo> lookup = lookup_symbolInfos_bySymbol;
			if (caseSensitive == false) {
				lookup = lookup_symbolInfos_bySymbol_UPPERCASE;
				symbol = symbol.ToUpper();
			}
			try {
				ret = lookup[symbol];
			} catch (Exception ex) {
				string msg = "CHEAPER_TO_RETRIEVE_THAT_.Contains()+RETRIEVE";
			}
			return ret;
		}
		public SymbolInfo FindSymbolInfoOrNew(string symbol) {
			if (string.IsNullOrEmpty(symbol)) {
				string msg = "MUST_NEVER_HAPPEN__LEADS_TO_FATAL_ERRORS__CREATING_EMPTY_SYMBOL_INFO FindSymbolInfoOrNew(NULL)";
				Assembler.PopupException(msg);
				return new SymbolInfo();
			}
			SymbolInfo ret = this.FindSymbolInfo_nullUnsafe(symbol);
			if (ret == null) ret = this.Add(symbol);
			return ret;
		}
		public SymbolInfo Duplicate(SymbolInfo symbolInfo, string dupeSymbol) {
			if (this.FindSymbolInfo_nullUnsafe(dupeSymbol) != null){
				string msg = "I_REFUSE_TO_DUPLICATE_SYMBOL_INFO__SYMBOL_ALREADY_EXISTS[" + dupeSymbol + "]";
				Assembler.PopupException(msg);
				return null;
			}
			SymbolInfo clone = symbolInfo.Clone();
			clone.Symbol = dupeSymbol;
			this.SymbolInfos.Add(clone);
			this.sortSerialize_rebuildLookup();
			return clone;
		}
		public SymbolInfo Rename(SymbolInfo symbolInfo, string newSymbol) {
			if (this.FindSymbolInfo_nullUnsafe(newSymbol) != null) {
				string msg = "I_REFUSE_TO_RENAME_SYMBOL_INFO__SYMBOL_ALREADY_EXISTS[" + newSymbol + "]";
				Assembler.PopupException(msg);
				return null;
			}
			symbolInfo.Symbol = newSymbol;
			this.sortSerialize_rebuildLookup();
			return symbolInfo;
		}
		public SymbolInfo Add(string newSymbol) {
			if (newSymbol == null) {
				string msg = "I_REFUSE_TO_ADD_SYMBOL_NULL[" + newSymbol + "]";
				Assembler.PopupException(msg);
				return null;
			}
			if (this.FindSymbolInfo_nullUnsafe(newSymbol) != null) {
				string msg = "I_REFUSE_TO_ADD_SYMBOL_INFO__SYMBOL_ALREADY_EXISTS[" + newSymbol + "]";
				Assembler.PopupException(msg);
				return null;
			}
			SymbolInfo adding = new SymbolInfo();
			adding.Symbol = newSymbol;
			this.SymbolInfos.Add(adding);
			this.sortSerialize_rebuildLookup();
			return adding;
		}
		public SymbolInfo Delete(SymbolInfo symbolInfo) {
			int index = this.SymbolInfos.IndexOf(symbolInfo);
			if (index == -1) {
				string msg = "I_REFUSE_TO_DELETE_SYMBOL_INFO__NOT_FOUND[" + symbolInfo.ToString() + "]";
				Assembler.PopupException(msg);
				return null;
			}
			this.SymbolInfos.Remove(symbolInfo);
			this.sortSerialize_rebuildLookup();
			if (index == 0) {
				string msg = "LAST_SYMBOL_DELETED[" + symbolInfo.ToString() + "]";
				Assembler.PopupException(msg);
				return null;
			}
			SymbolInfo prior = this.SymbolInfos[index-1];
			return prior;
		}

		void sortSerialize_rebuildLookup() {
			this.SymbolInfos.Sort(this.sorter);
			base.Serialize();
			this.rebuild_lookupDictionaries();
		}
		//internal void DeserializeAndSort() {
		//    this.Deserialize();
		//    this.sort();
		//}

		public override int Serialize() {
			base.Serialize();
			return base.EntityDeserialized.Count;
		}

		bool deserializedOnce_nowSyncOnly = false;
		public override List<SymbolInfo> Deserialize() {
			if (base.EntityDeserialized == null) base.EntityDeserialized = new List<SymbolInfo>();
			List<SymbolInfo> backup = base.EntityDeserialized;
			base.Deserialize();
			this.rebuild_lookupDictionaries(true);

			if (this.deserializedOnce_nowSyncOnly == false) {
				this.deserializedOnce_nowSyncOnly = true;
				return base.EntityDeserialized;
			}

			List<SymbolInfo> dontOverwriteInstances_kozSubscribers_willLoose_eventGenerators = base.EntityDeserialized;
			base.EntityDeserialized = backup;

			string msg = "YOU_SHOULD_NOT_DESERIALIZE_TWICE__RepositorySerializerSymbolInfo.Deserialize()";
			Assembler.PopupException(msg, null, false);
			// if you never saw the Exception above, the code below is never invoked; keeping it to eternify my stupidity

			List<SymbolInfo> toBeAdded = new List<SymbolInfo>();
			List<SymbolInfo> toBeDeleted = new List<SymbolInfo>();
			foreach (SymbolInfo eachExisting in base.EntityDeserialized) {
				//if (backup.ContainsSymbol(eachExisting.Symbol)) continue;
				if (backup.Contains(eachExisting)) continue;
				toBeAdded.Add(eachExisting);
			}
			foreach (SymbolInfo eachDeserialized in dontOverwriteInstances_kozSubscribers_willLoose_eventGenerators) {
				//if (this.SymbolInfos.ContainsSymbol(eachDeserialized.Symbol)) continue;
				if (this.SymbolInfos.Contains(eachDeserialized)) continue;
				toBeDeleted.Add(eachDeserialized);
			}
			foreach (var deleteEach in toBeDeleted) {
				this.SymbolInfos.Remove(deleteEach);
			}
			base.EntityDeserialized.AddRange(toBeAdded);

			return base.EntityDeserialized;
		}

		void rebuild_lookupDictionaries(bool deduplicateSource = false) {
			this.lookup_symbolInfos_bySymbol.Clear();
			this.lookup_symbolInfos_bySymbol_UPPERCASE.Clear();
			List<SymbolInfo> duplicates_toRemove = new List<SymbolInfo>();

			foreach (SymbolInfo symbolInfo_each in this.SymbolInfos) {
				string symbol = symbolInfo_each.Symbol;
				string symbol_UPPERCASE = symbol.ToUpper();

				if (this.lookup_symbolInfos_bySymbol.ContainsKey(symbol)) {
					string msg = "LOOKUP_ALREADY_CONTAINS symbol[" + symbol + "]=>lookup_symbolInfos_bySymbol";
					Assembler.PopupException(msg);
					if (duplicates_toRemove.Contains(symbolInfo_each) == false) {
						duplicates_toRemove.Add(symbolInfo_each);
					}
				} else {
					this.lookup_symbolInfos_bySymbol.Add(symbol, symbolInfo_each);
				}

				if (this.lookup_symbolInfos_bySymbol_UPPERCASE.ContainsKey(symbol_UPPERCASE)) {
					string msg = "LOOKUP_ALREADY_CONTAINS symbol_UPPERCASE[" + symbol_UPPERCASE + "]=>lookup_symbolInfos_bySymbol_UPPERCASE";
					Assembler.PopupException(msg);
					if (duplicates_toRemove.Contains(symbolInfo_each) == false) {
						duplicates_toRemove.Add(symbolInfo_each);
					}
				} else {
					this.lookup_symbolInfos_bySymbol_UPPERCASE.Add(symbol_UPPERCASE, symbolInfo_each);
				}
			}

			int removed = 0;
			foreach (SymbolInfo symbolInfo_each in duplicates_toRemove) {
				if (SymbolInfos.Contains(symbolInfo_each) == false) continue;
				this.SymbolInfos.Remove(symbolInfo_each);
				removed++;
			}
			if (removed > 0) this.Serialize();
		}

	}
}