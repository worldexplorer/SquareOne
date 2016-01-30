using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Sq1.Core.DataTypes;
using Sq1.Core.Serializers;
using Newtonsoft.Json;

namespace Sq1.Core.Repositories {
	public class RepositorySerializerSymbolInfos : Serializer<SymbolInfoList> {
		public RepositorySerializerSymbolInfos() : base() {}

		public SymbolInfoList SymbolInfos { get { return base.EntityDeserialized; } }

		public SymbolInfo FindSymbolInfoNullUnsafe(string symbol) {
			SymbolInfo ret = null;
			if (string.IsNullOrEmpty(symbol)) return ret;
			foreach (SymbolInfo eachSymbolInfo in this.SymbolInfos) {
				if (eachSymbolInfo.Symbol == null) {
					string msg = "DELETE_Symbol=null_IN_SYMBOL_INFO_EDITOR";
					continue;
				}
				if (eachSymbolInfo.Symbol.ToUpper() != symbol.ToUpper()) continue;
				ret = eachSymbolInfo;
				break;
			}
			return ret;
		}
		public SymbolInfo FindSymbolInfoOrNew(string symbol) {
			SymbolInfo ret = this.FindSymbolInfoNullUnsafe(symbol);
			if (ret == null) ret = this.Add(symbol);
			return ret;
		}
		public SymbolInfo Duplicate(SymbolInfo symbolInfo, string dupeSymbol) {
			if (this.FindSymbolInfoNullUnsafe(dupeSymbol) != null){
				string msg = "I_REFUSE_TO_DUPLICATE_SYMBOL_INFO__SYMBOL_ALREADY_EXISTS[" + dupeSymbol + "]";
				Assembler.PopupException(msg);
				return null;
			}
			SymbolInfo clone = symbolInfo.Clone();
			clone.Symbol = dupeSymbol;
			this.SymbolInfos.Add(clone);
			this.sort();
			base.Serialize();
			return clone;
		}
		public SymbolInfo Rename(SymbolInfo symbolInfo, string newSymbol) {
			if (this.FindSymbolInfoNullUnsafe(newSymbol) != null) {
				string msg = "I_REFUSE_TO_RENAME_SYMBOL_INFO__SYMBOL_ALREADY_EXISTS[" + newSymbol + "]";
				Assembler.PopupException(msg);
				return null;
			}
			symbolInfo.Symbol = newSymbol;
			this.sort();
			base.Serialize();
			return symbolInfo;
		}
		public SymbolInfo Add(string newSymbol) {
			if (newSymbol == null) {
				string msg = "I_REFUSE_TO_ADD_SYMBOL_NULL[" + newSymbol + "]";
				Assembler.PopupException(msg);
				return null;
			}
			if (this.FindSymbolInfoNullUnsafe(newSymbol) != null) {
				string msg = "I_REFUSE_TO_ADD_SYMBOL_INFO__SYMBOL_ALREADY_EXISTS[" + newSymbol + "]";
				Assembler.PopupException(msg);
				return null;
			}
			SymbolInfo adding = new SymbolInfo();
			adding.Symbol = newSymbol;
			this.SymbolInfos.Add(adding);
			this.sort();
			base.Serialize();
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
			this.sort();
			base.Serialize();
			if (index == 0) {
				string msg = "LAST_SYMBOL_DELETED[" + symbolInfo.ToString() + "]";
				Assembler.PopupException(msg);
				return null;
			}
			SymbolInfo prior = this.SymbolInfos[index-1];
			return prior;
		}


		public class ASC : IComparer<SymbolInfo> {
			int IComparer<SymbolInfo>.Compare(SymbolInfo x, SymbolInfo y) { return x.Symbol.CompareTo(y.Symbol); }
		}
		void sort() {
			this.SymbolInfos.Sort(new ASC());
		}
		internal void DeserializeAndSort() {
			this.Deserialize();
			this.sort();
		}


		bool deserializedOnce_nowSyncOnly = false;
		public override SymbolInfoList Deserialize() {
			if (base.EntityDeserialized == null) base.EntityDeserialized = new SymbolInfoList();
			SymbolInfoList backup = base.EntityDeserialized;
			base.Deserialize();

			if (this.deserializedOnce_nowSyncOnly == false) {
				this.deserializedOnce_nowSyncOnly = true;
				return base.EntityDeserialized;
			}

			SymbolInfoList dontOverwriteInstancesKozSubscribersWillLooseEventGenerators = base.EntityDeserialized;
			base.EntityDeserialized = backup;

			string msg = "YOU_SHOULD_NOT_DESERIALIZE_TWICE__RepositorySerializerSymbolInfo.Deserialize()";
			Assembler.PopupException(msg, null, false);
			// if you never saw the Exception above, the code below is never invoked; keeping it to eternify my stupidity

			List<SymbolInfo> toBeAdded = new List<SymbolInfo>();
			List<SymbolInfo> toBeDeleted = new List<SymbolInfo>();
			foreach (SymbolInfo eachExisting in base.EntityDeserialized) {
				if (backup.ContainsSymbol(eachExisting.Symbol)) continue;
				toBeAdded.Add(eachExisting);
			}
			foreach (SymbolInfo eachDeserialized in dontOverwriteInstancesKozSubscribersWillLooseEventGenerators) {
				if (this.SymbolInfos.ContainsSymbol(eachDeserialized.Symbol)) continue;
				toBeDeleted.Add(eachDeserialized);
			}
			foreach (var deleteEach in toBeDeleted) {
				this.SymbolInfos.Remove(deleteEach);
			}
			base.EntityDeserialized.AddRange(toBeAdded);
			return base.EntityDeserialized;
		}

	}
}