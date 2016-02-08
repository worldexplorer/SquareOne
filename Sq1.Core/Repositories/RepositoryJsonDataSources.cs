using System;
using System.Collections.Generic;

using Sq1.Core.Broker;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Repositories {
	// the hackiest class in the whole solution :(
	public partial class RepositoryJsonDataSources : RepositoryJsonsInFolder<DataSource> {
		public RepositorySerializerMarketInfos	MarketInfoRepository;
		public OrderProcessor					OrderProcessor;

		public RepositoryJsonDataSources() : base() {
			base.CheckIfValidAndShouldBeAddedAfterDeserialized = this.dataSourceDeserializedInitializePriorToAdding;
		}
		
		public void Initialize(string rootPath, string subfolder,
				RepositorySerializerMarketInfos marketInfoRepository, OrderProcessor orderProcessor) {
			base.Initialize(rootPath, subfolder, this.dataSourceDeserializedInitializePriorToAdding);
			this.MarketInfoRepository = marketInfoRepository;
			this.OrderProcessor = orderProcessor;
		}
		
		bool dataSourceDeserializedInitializePriorToAdding(string thisOne, DataSource dsDeserialized) {
			if (dsDeserialized.MarketInfo == null && string.IsNullOrEmpty(dsDeserialized.MarketName) == false) {
				dsDeserialized.MarketInfo = MarketInfoRepository.FindMarketInfoOrNew(dsDeserialized.MarketName);
			}
			try {
				dsDeserialized.Initialize(base.AbsPath, this.OrderProcessor);
			} catch (Exception ex) {
				string msg = "FAILED to dsDeserialized.InitializeAndScan(" + base.AbsPath + ")"
					//+ "; //Exception shouldn't break all DataSourceManager.DataSourcesDeserialize()"
					//+ " and make other *Adaptders unusable"
					+ " from " + thisOne + "; still adding to the Tree";
				//base.StatusReporter.PopupException(msg, ex);
				Assembler.PopupException(msg, ex);
			}
			return true;
		}

		public override void ItemAddCascade(DataSource itemCandidate, object sender = null) {
			itemCandidate.Initialize(base.AbsPath, this.OrderProcessor);
			//v1 WILL_DELETES_BAR_FILE_IF_RENAME_FAILS itemCandidate.CreateDeleteBarFilesToSymbolsDeserialized();
			//v2 
			itemCandidate.SymbolsRebuildReadDataSourceSubFolderAfterDeserialization();
		}
		public override void ItemDeleteCascade(DataSource itemStored, object sender = null) {
			itemStored.DataSourceFolderDeleteWithSymbols();
		}
		public override void ItemCanBeDeletedCascade(NamedObjectJsonEventArgs<DataSource> args, object sender = null) {
		}
		public override bool ItemRenameCascade(DataSource itemToRename, string newName, object sender = null) {
			return itemToRename.DataSourceFolderRename(newName);
		}

		public DataSourceSymbolEventArgs SymbolCanBeDeleted(DataSource dataSource, string symbolToDelete, object sender = null) {
			if (sender == null) sender = this;
			var args = new DataSourceSymbolEventArgs(dataSource, symbolToDelete);
			this.RaiseOnSymbolCanBeDeleted(sender, args);
			return args;
		}
		public void SymbolAdd(DataSource dataSource, string symbolToAdd, object sender = null) {
			if (sender == null) sender = this;
			string msig = " RepositoryJsonDataSource.SymbolAdd(" + dataSource + ", " + symbolToAdd + "): ";
			try {
				dataSource.SymbolAdd(symbolToAdd);
				base.SerializeSingle(dataSource);
				this.RaiseOnSymbolAdded(sender, dataSource, symbolToAdd);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			}
		}
		public void SymbolRename(DataSource dataSource, string oldSymbolName, string newSymbolName, object sender = null) {
			if (sender == null) sender = this;
			string msig = " RepositoryJsonDataSource.SymbolRename(" + dataSource + ", " + oldSymbolName + "): ";
			try {
				dataSource.SymbolRename(oldSymbolName, newSymbolName);
				base.SerializeSingle(dataSource);
				// invoking the callback for DataSourcesTreeControl to repaint successfully renamed symbol
				this.RaiseOnSymbolRenamed(sender, dataSource, newSymbolName, oldSymbolName);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			}
		}
		public void SymbolRemove(DataSource dataSource, string symbolToDelete, object sender = null) {
			if (sender == null) sender = this;
			string msig = " RepositoryJsonDataSource.SymbolRemove(" + symbolToDelete + "): ";
			DataSourceSymbolEventArgs args = null;
			try {
				args = this.SymbolCanBeDeleted(dataSource, symbolToDelete, sender);
				if (args.DoNotDeleteItsUsedElsewhere) {
					string msg = "DoNotDeleteReason=[" + args.DoNotDeleteReason + "] for ItemDelete(" + dataSource.ToString() + ")";
					throw new Exception(msg);
				}
				dataSource.SymbolRemove(symbolToDelete);		// can throw FILE_ALREADY_DELETED
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			}
			// sorry for the mess
			try {
				base.SerializeSingle(dataSource);
				if (args != null) {
					// since you answered DataSourceEventArgs.DoNotDeleteThisDataSourceBecauseItsUsedElsewhere=false,
					// you were aware that OnItemRemovedDone is invoked on a detached object
					this.RaiseOnSymbolRemovedDone(sender, args);
				}
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			}
		}
		public DataSource DataSourceFindNullUnsafe(string name) {
			DataSource ret = null;
			string nameUpper = name.ToUpper();
			foreach (DataSource current in base.ItemsAsList) {
				if (nameUpper != current.Name.ToUpper()) continue;
				ret = current;
			}
			return ret;
		}
		public int SameMarketInfoInHowManyDataSources(MarketInfo marketInfo) {
			int ret = 0;
			foreach (DataSource ds in base.ItemsAsList) {
				if (ds.MarketInfo == marketInfo) ret++;
			}
			return ret;
		}

		public void SymbolCopyOrCompressFrom(DataSource dataSourceFrom, string symbolToCopy, DataSource dataSourceTo, object sender = null) {
			if (sender == null) sender = this;
			string msig = " RepositoryJsonDataSource.SymbolCopy(" + dataSourceFrom.Name + ", " + symbolToCopy + ", " + dataSourceTo.Name + "): ";
			try {
				dataSourceTo.SymbolCopyOrCompressFrom(dataSourceFrom, symbolToCopy, dataSourceTo);
				base.SerializeSingle(dataSourceTo);
				this.RaiseOnSymbolAdded(sender, dataSourceTo, symbolToCopy);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			}
		}
		public List<DataSource> SameSymbolInHowManyDataSources(string symbol) {
			List<DataSource> ret = new List<DataSource>();
			foreach (DataSource ds in base.ItemsAsList) {
				if (ds.Symbols.Contains(symbol)) ret.Add(ds);
			}
			return ret;
		}

		//SNAP_IS_NOT_SERIALIZED_ANYMORE 
		//internal void ReattachDataSnaphotsToOwnersStreamingAdapters() {
		//    foreach (DataSource ds in base.ItemsAsList) {
		//        if (ds.StreamingAdapter == null) continue;
		//        ds.StreamingAdapter.StreamingDataSnapshot.InitializeWithStreaming(ds.StreamingAdapter);
		//    }
		//}
	}
}