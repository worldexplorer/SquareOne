using System;
using System.Collections.Generic;

namespace Sq1.Core.Charting {
	public class DictionaryManyToOne<CHART, ALERTS> {
		Dictionary<CHART, List<ALERTS>> lookup;
		Dictionary<ALERTS, CHART> reverse;

		public List<CHART> Keys { get {
			return new List<CHART>(this.lookup.Keys);
		} }
		
		public DictionaryManyToOne() {
			this.lookup = new Dictionary<CHART, List<ALERTS>>();
			this.reverse = new Dictionary<ALERTS, CHART>();
		}
		
		public void Register(CHART chart) {
			string msig = " //DictionaryManyToOne::Register()";
			if (this.lookup.ContainsKey(chart)) {
				string msg = "ALREADY_REGISTERED_IN_this.Lookup chart[" + chart + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
//			if (this.Reverse.ContainsValue(chart)) {
//				string msg = "chart[" + chart + "] is already registered in this.Reverse";
//				Assembler.PopupException(msg + msig);
//				return;
//			}
			this.lookup.Add(chart, new List<ALERTS>());
		}
		
		public void UnRegister(CHART chart) {
			string msig = " //DictionaryManyToOne::UnRegister()";
			if (this.lookup.ContainsKey(chart) == false) {
				string msg = "NEVER_REGISTERED_IN_this.Lookup chart[" + chart + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
//			if (this.Reverse.ContainsValue(chart)) {
//				string msg = "chart[" + chart + "] was never registered in this.Reverse";
//				Assembler.PopupException(msg + msig);
//				return;
//			}
			this.lookup.Add(chart, new List<ALERTS>());
		}
		
		public void ClearDependantsFor(CHART chart) {
			string msig = " //DictionaryManyToOne::ClearDependantsFor()";
			if (this.lookup.ContainsKey(chart) == false) {
				string msg = "NEVER_REGISTERED_IN_this.Lookup chart[" + chart + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			foreach (ALERTS dependant in this.lookup[chart]) {
				if (this.reverse.ContainsKey(dependant) == false) {
					string msg = "position[" + dependant + "] is not found in this.Reverse, you should've gotten an exception during Add()? AlmostImpossible happened";
					Assembler.PopupException(msg + msig);
					return;
				}
				this.reverse.Remove(dependant);
			}
			this.lookup[chart] = new List<ALERTS>();
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
			if (this.lookup.ContainsKey(chart) == false) {
				string msg = "NEVER_REGISTERED_IN_this.Lookup chart[" + chart + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (this.lookup[chart].Contains(alertPointerCopy)) {
				string msg = "ALREADY_ADDED_INTO_this.Lookup[chart] alertPointerCopy[" + alertPointerCopy + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (this.reverse.ContainsKey(alertPointerCopy)) {
				string msg = "ALREADY_ADDED_INTO_this.Reverse alertPointerCopy[" + alertPointerCopy + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			this.lookup[chart].Add(alertPointerCopy);
			this.reverse.Add(alertPointerCopy, chart);
		}
		
//		public void AddRange(CHART chart, List<ALERTS> alerts) {
//			foreach (ALERTS alert in alerts) {
//				this.Add(chart, alert);
//			}
//		}

		public void Remove(CHART chart, ALERTS alert) {
			string msig = " //DictionaryManyToOne::Remove(chart[" + chart + "], alert[" + alert + "])";
			if (this.lookup.ContainsKey(chart) == false) {
				string msg = "CONTAINER_WAS_NEVER_REGISTERED_IN_this.Lookup chart[" + chart + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (this.lookup[chart].Contains(alert) == false) {
				string msg = "LOOKUP_REFERENCE_WAS_NEVER_ADDED_FOR alert[" + alert + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (this.reverse.ContainsKey(alert) == false) {
				string msg = "REVERSE_REFERENCE_WAS_NEVER_ADDED_FOR alert[" + alert + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			this.lookup[chart].Remove(alert);
			this.reverse.Remove(alert);
		}
		
		public bool IsItemRegisteredForAnyContainer(ALERTS alert) {
			string msig = " DictionaryManyToOne::IsItemRegistered(alert[" + alert + "]): ";
			return this.reverse.ContainsKey(alert);
		}
		public CHART FindContainerFor_throws(ALERTS alert) {
			string msig = " //DictionaryManyToOne::FindContainerFor(alert[" + alert + "])";
			if (this.reverse.ContainsKey(alert) == false) {
				string msg = "REVERSE_REFERENCE_WAS_NEVER_ADDED_FOR alert[" + alert + "]";
				throw new Exception(msg + msig);	// I hate nullable types alltogether; can't return null here koz CHART=null is nonsence for the Dictionary
				//v2
				//Assembler.PopupException(msg + msig);	// I hate nullable types alltogether; can't return null here koz CHART=null is nonsence for the Dictionary
				//return null;
			}
			return this.reverse[alert];
		}
		public List<ALERTS> FindContentsOf_nullUnsafe(CHART chart) {
			string msig = " //DictionaryManyToOne::FindContents(chart[" + chart + "])";
			if (this.lookup.ContainsKey(chart) == false) {
				string msg = "NEVER_REGISTERED_IN_this.Lookup chart[" + chart + "]";
				Assembler.PopupException(msg + msig);
				return null;
			}
			return this.lookup[chart];
		}


		// added for DataSource.cs: public DictionaryManyToOne<SymbolOfDataSource, ChartShadow> ChartsOpenForSymbol
		internal List<ALERTS> RenameKey(CHART oldSymbolName, CHART newSymbolName) {
			List<ALERTS> chartShadowsAffected_alreadySavedStrategyOrCtx_invokedRaiseChartSettingsChangedContainerShouldSerialize = new List<ALERTS>();

			List<ALERTS> chartShadowsOpenForSymbolOfDataSource = this.FindContentsOf_nullUnsafe(oldSymbolName);
			List<ALERTS> avoidingCollectionModified = new List<ALERTS>(chartShadowsOpenForSymbolOfDataSource);
			foreach (ALERTS chartShadow in avoidingCollectionModified) {
				if (this.IsRegistered(newSymbolName)) {
					string msg = "I_REFUSE_TO_RENAME SYMBOL_ALREADY_EXISTS";
					Assembler.PopupException(msg, null, false);
					return chartShadowsAffected_alreadySavedStrategyOrCtx_invokedRaiseChartSettingsChangedContainerShouldSerialize;
				}
				this.Register(newSymbolName);
				//looks atomic, removal before adding => should NOT throw
				this.Remove(oldSymbolName, chartShadow);
				this.Add(newSymbolName, chartShadow);
				chartShadowsAffected_alreadySavedStrategyOrCtx_invokedRaiseChartSettingsChangedContainerShouldSerialize.Add(chartShadow);
			}
			if (this.lookup[oldSymbolName].Count != 0) {
				string msg = "MUST_BE_NO_CONTENT_FOR_THE_OLD_KEY[" + oldSymbolName + "] //RenameKey(" + oldSymbolName + "=>" + newSymbolName + ")";
				Assembler.PopupException(msg);
				return chartShadowsAffected_alreadySavedStrategyOrCtx_invokedRaiseChartSettingsChangedContainerShouldSerialize;
			}
			this.lookup.Remove(oldSymbolName);
#if DEBUG
			string msig = " //RenameKey(oldSymbolName[" + oldSymbolName + "] => newSymbolName[" + newSymbolName + "]";
			if (this.lookup.ContainsKey(oldSymbolName)) {
				string msg = "lookup must not contain oldSymbolName[" + oldSymbolName + "]" + msig;
			}
			if (this.lookup.ContainsKey(newSymbolName) == false) {
				string msg = "lookup must contain newSymbolName[" + newSymbolName + "]" + msig;
			}
			if (this.reverse.ContainsValue(oldSymbolName)) {
				string msg = "reverse must not contain oldSymbolName[" + oldSymbolName + "]" + msig;
			}
			if (this.reverse.ContainsValue(newSymbolName) == false) {
				string msg = "reverse must contain newSymbolName[" + newSymbolName + "]" + msig;
			}
#endif
			return chartShadowsAffected_alreadySavedStrategyOrCtx_invokedRaiseChartSettingsChangedContainerShouldSerialize;
		}

		internal bool IsRegistered(CHART newSymbolName) {
			return this.lookup.ContainsKey(newSymbolName);
		}
		internal CHART FindSimilarKey(CHART anotherInstance) {
			foreach (CHART existingKey in this.lookup.Keys) {
				if (existingKey.ToString() == anotherInstance.ToString()) return existingKey;
			}
			return default(CHART);		// I_HOPE_IT_IS_NULL
		}
		internal List<ALERTS> FindContentsForSimilarKey__nullUnsafe(CHART anotherInstance) {
			string msig = " //DictionaryManyToOne::FindContentsForSimilarKey(chart[" + anotherInstance + "])";
			CHART existingKeyFound = this.FindSimilarKey(anotherInstance);
			if (existingKeyFound == null) {
				string msg = "NEVER_REGISTERED_IN_this.Lookup_WITH_SAME_.ToString() anotherInstance[" + anotherInstance.ToString() + "]";
				Assembler.PopupException(msg + msig);
			}
			if (this.lookup.ContainsKey(existingKeyFound) == false) {
				string msg = "NEVER_REGISTERED_IN_this.Lookup existingKeyFound[" + existingKeyFound + "]";
				Assembler.PopupException(msg + msig);
				return null;
			}
			return this.lookup[existingKeyFound];
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
