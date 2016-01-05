using System;
using System.Collections.Generic;

namespace Sq1.Core.Charting {
	public class DictionaryManyToOne<CHART, ALERTS> {
		Dictionary<CHART, List<ALERTS>> Lookup;
		Dictionary<ALERTS, CHART> Reverse;

		public List<CHART> Keys { get {
			return new List<CHART>(this.Lookup.Keys);
		} }
		
		public DictionaryManyToOne() {
			this.Lookup = new Dictionary<CHART, List<ALERTS>>();
			this.Reverse = new Dictionary<ALERTS, CHART>();
		}
		
		public void Register(CHART chart) {
			string msig = " //DictionaryManyToOne::Register()";
			if (this.Lookup.ContainsKey(chart)) {
				string msg = "ALREADY_REGISTERED_IN_this.Lookup chart[" + chart + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
//			if (this.Reverse.ContainsValue(chart)) {
//				string msg = "chart[" + chart + "] is already registered in this.Reverse";
//				Assembler.PopupException(msg + msig);
//				return;
//			}
			this.Lookup.Add(chart, new List<ALERTS>());
		}
		
		public void UnRegister(CHART chart) {
			string msig = " //DictionaryManyToOne::UnRegister()";
			if (this.Lookup.ContainsKey(chart) == false) {
				string msg = "NEVER_REGISTERED_IN_this.Lookup chart[" + chart + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
//			if (this.Reverse.ContainsValue(chart)) {
//				string msg = "chart[" + chart + "] was never registered in this.Reverse";
//				Assembler.PopupException(msg + msig);
//				return;
//			}
			this.Lookup.Add(chart, new List<ALERTS>());
		}
		
		public void ClearDependantsFor(CHART chart) {
			string msig = " //DictionaryManyToOne::ClearDependantsFor()";
			if (this.Lookup.ContainsKey(chart) == false) {
				string msg = "NEVER_REGISTERED_IN_this.Lookup chart[" + chart + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			foreach (ALERTS dependant in this.Lookup[chart]) {
				if (this.Reverse.ContainsKey(dependant) == false) {
					string msg = "position[" + dependant + "] is not found in this.Reverse, you should've gotten an exception during Add()? AlmostImpossible happened";
					Assembler.PopupException(msg + msig);
					return;
				}
				this.Reverse.Remove(dependant);
			}
			this.Lookup[chart] = new List<ALERTS>();
		}
		
		public void Add(CHART chart, ALERTS alert) {
			ALERTS alertPointerCopy = alert;		// at some point alert=null and Dictionary.Insert() throws NullReferenceException
			string msig = " //DictionaryManyToOne::Add(chart[" + chart + "], alertPointerCopy[" + alertPointerCopy + "])";
			if (chart == null) {
				string msg = "I_REFUSE_TO_ADD_CHART_NULL__FILTER_ME_OUT_UPSTACK_OR_CREATE_EMPTY_CHART_SHADOW";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (alertPointerCopy == null) {
				string msg = "I_REFUSE_TO_ADD_alertPointerCopy_NULL__FILTER_ME_OUT_UPSTACK";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (this.Lookup.ContainsKey(chart) == false) {
				string msg = "NEVER_REGISTERED_IN_this.Lookup chart[" + chart + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (this.Lookup[chart].Contains(alertPointerCopy)) {
				string msg = "ALREADY_ADDED_INTO_this.Lookup[chart] alertPointerCopy[" + alertPointerCopy + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (this.Reverse.ContainsKey(alertPointerCopy)) {
				string msg = "ALREADY_ADDED_INTO_this.Reverse alertPointerCopy[" + alertPointerCopy + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			this.Lookup[chart].Add(alertPointerCopy);
			this.Reverse.Add(alertPointerCopy, chart);
		}
		
//		public void AddRange(CHART chart, List<ALERTS> alerts) {
//			foreach (ALERTS alert in alerts) {
//				this.Add(chart, alert);
//			}
//		}

		public void Remove(CHART chart, ALERTS alert) {
			string msig = " //DictionaryManyToOne::Remove(chart[" + chart + "], alert[" + alert + "])";
			if (this.Lookup.ContainsKey(chart) == false) {
				string msg = "CONTAINER_WAS_NEVER_REGISTERED_IN_this.Lookup chart[" + chart + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (this.Lookup[chart].Contains(alert) == false) {
				string msg = "LOOKUP_REFERENCE_WAS_NEVER_ADDED_FOR alert[" + alert + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (this.Reverse.ContainsKey(alert) == false) {
				string msg = "REVERSE_REFERENCE_WAS_NEVER_ADDED_FOR alert[" + alert + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			this.Lookup[chart].Remove(alert);
			this.Reverse.Remove(alert);
		}
		
		public bool IsItemRegisteredForAnyContainer(ALERTS alert) {
			string msig = " DictionaryManyToOne::IsItemRegistered(alert[" + alert + "]): ";
			return this.Reverse.ContainsKey(alert);
		}
		public CHART FindContainerFor_throws(ALERTS alert) {
			string msig = " //DictionaryManyToOne::FindContainerFor(alert[" + alert + "])";
			if (this.Reverse.ContainsKey(alert) == false) {
				string msg = "REVERSE_REFERENCE_WAS_NEVER_ADDED_FOR alert[" + alert + "]";
				throw new Exception(msg + msig);	// I hate nullable types alltogether; can't return null here koz CHART=null is nonsence for the Dictionary
				//v2
				//Assembler.PopupException(msg + msig);	// I hate nullable types alltogether; can't return null here koz CHART=null is nonsence for the Dictionary
				//return null;
			}
			return this.Reverse[alert];
		}
		public List<ALERTS> FindContentsOf_NullUnsafe(CHART chart) {
			string msig = " //DictionaryManyToOne::FindContents(chart[" + chart + "])";
			if (this.Lookup.ContainsKey(chart) == false) {
				string msg = "NEVER_REGISTERED_IN_this.Lookup chart[" + chart + "]";
				Assembler.PopupException(msg + msig);
				return null;
			}
			return this.Lookup[chart];
		}


		// added for DataSource.cs: public DictionaryManyToOne<SymbolOfDataSource, ChartShadow> ChartsOpenForSymbol
		internal void RenameKey(CHART oldSymbolName, CHART newSymbolName) {
			List<ALERTS> chartShadowsOpenForSymbolOfDataSource = this.FindContentsOf_NullUnsafe(oldSymbolName);
			foreach (ALERTS chartShadow in chartShadowsOpenForSymbolOfDataSource) {
				this.Add(newSymbolName, chartShadow);
				this.Remove(oldSymbolName, chartShadow);
			}
			if (this.Lookup[oldSymbolName].Count != 0) {
				string msg = "MUST_BE_NO_CONTENT_FOR_THE_OLD_KEY[" + oldSymbolName + "] //RenameKey(" + oldSymbolName + "=>" + newSymbolName + ")";
				Assembler.PopupException(msg);
				return;
			}
			this.Lookup.Remove(oldSymbolName);
		}
		internal CHART FindSimilarKey(CHART anotherInstance) {
			foreach (CHART existingKey in this.Lookup.Keys) {
				if (existingKey.ToString() == anotherInstance.ToString()) return existingKey;
			}
			return default(CHART);		// I_HOPE_IT_IS_NULL
		}
		internal List<ALERTS> FindContentsForSimilarKey_NullUnsafe(CHART anotherInstance) {
			string msig = " //DictionaryManyToOne::FindContentsForSimilarKey(chart[" + anotherInstance + "])";
			CHART existingKeyFound = this.FindSimilarKey(anotherInstance);
			if (existingKeyFound == null) {
				string msg = "NEVER_REGISTERED_IN_this.Lookup_WITH_SAME_.ToString() anotherInstance[" + anotherInstance.ToString() + "]";
				Assembler.PopupException(msg + msig);
			}
			if (this.Lookup.ContainsKey(existingKeyFound) == false) {
				string msg = "NEVER_REGISTERED_IN_this.Lookup existingKeyFound[" + existingKeyFound + "]";
				Assembler.PopupException(msg + msig);
				return null;
			}
			return this.Lookup[existingKeyFound];
		}
		internal void UnRegisterSimilar(CHART anotherInstance) {
			string msig = " //DictionaryManyToOne::UnRegisterSimilar(" + anotherInstance + ")";
			CHART existingKeyFound = this.FindSimilarKey(anotherInstance);
			if (existingKeyFound == null) {
				string msg = "NEVER_REGISTERED_IN_this.Lookup_WITH_SAME_.ToString() anotherInstance[" + existingKeyFound.ToString() + "]";
				Assembler.PopupException(msg + msig);
			}
			this.UnRegister(existingKeyFound);
		}
	}
}
