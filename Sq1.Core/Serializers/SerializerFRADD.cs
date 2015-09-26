using System;
using System.Collections.Generic;

namespace Sq1.Core.Serializers {
	public abstract class SerializerFRADD<T, U> : Serializer<List<T>> where T : new() {
		string REASON_TO_EXIST = "FRADD stands for Find,Rename,Add,Delete,Duplicate";
		public string OfWhat { get { return typeof(T).Name; } }
		
		public SerializerFRADD() : base() { }

		public virtual T FindNullUnsafe(string symbol) {
			//T ret = null;
			T mustBeNull = default(T);
			T ret = mustBeNull;
			if (string.IsNullOrEmpty(symbol)) return ret;
			foreach (T eachT in base.EntityDeserialized) {
				//if (eachT.Symbol.ToUpper() != symbol.ToUpper()) continue;
				if (eachT.ToString() != symbol) continue;
				ret = eachT;
				break;
			}
			return ret;
		}
		public T FindOrNew(string symbol) {
			T ret = this.FindNullUnsafe(symbol);
			if (ret == null) ret = this.Add(symbol);
			return ret;
		}

		protected abstract T Clone(T symbolInfo);
		public T Duplicate(T symbolInfo, string dupeSymbol) {
			if (this.FindNullUnsafe(dupeSymbol) != null) {
				string msg = "I_REFUSE_TO_DUPLICATE_SYMBOL_INFO__SYMBOL_ALREADY_EXISTS[" + dupeSymbol + "]";
				Assembler.PopupException(msg);
				//return null;
				T mustBeNull = default(T);
				return mustBeNull;
			}
			//T clone = symbolInfo.Clone();
			T clone = this.Clone(symbolInfo);
			clone.Symbol = dupeSymbol;
			base.EntityDeserialized.Add(clone);
			this.sort();
			base.Serialize();
			return clone;
		}
		public T Rename(T symbolInfo, string newSymbol) {
			if (this.FindNullUnsafe(newSymbol) != null) {
				string msg = "I_REFUSE_TO_RENAME_SYMBOL_INFO__SYMBOL_ALREADY_EXISTS[" + newSymbol + "]";
				Assembler.PopupException(msg);
				//return null;
				T mustBeNull = default(T);
				return mustBeNull;
			}
			symbolInfo.Symbol = newSymbol;
			this.sort();
			base.Serialize();
			return symbolInfo;
		}
		public T Add(string newSymbol) {
			if (this.FindNullUnsafe(newSymbol) != null) {
				string msg = "I_REFUSE_TO_ADD_SYMBOL_INFO__SYMBOL_ALREADY_EXISTS[" + newSymbol + "]";
				Assembler.PopupException(msg);
				//return null;
				T mustBeNull = default(T);
				return mustBeNull;
			}
			T adding = new T();
			adding.Symbol = newSymbol;
			base.EntityDeserialized.Add(adding);
			this.sort();
			base.Serialize();
			return adding;
		}
		public T Delete(T symbolInfo) {
			int index = base.EntityDeserialized.IndexOf(symbolInfo);
			if (index == -1) {
				string msg = "I_REFUSE_TO_DELETE_SYMBOL_INFO__NOT_FOUND[" + symbolInfo.ToString() + "]";
				Assembler.PopupException(msg);
				//return null;
				T mustBeNull = default(T);
				return mustBeNull;
			}
			base.EntityDeserialized.Remove(symbolInfo);
			this.sort();
			base.Serialize();
			if (index == 0) {
				string msg = "LAST_SYMBOL_DELETED[" + symbolInfo.ToString() + "]";
				Assembler.PopupException(msg);
				//return null;
				T mustBeNull = default(T);
				return mustBeNull;
			}
			T prior = base.EntityDeserialized[index - 1];
			return prior;
		}

		//public class ASC : IComparer<T> { int IComparer<T>.Compare(T x, T y) { return x.CompareTo(y); } }
		public virtual void sort() {
			base.EntityDeserialized.Sort();
		}
		internal void DeserializeAndSort() {
		    this.Deserialize();
		    this.sort();
		}

	}
}
