using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using Sq1.Core.Accounting;
using Sq1.Core.Broker;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Serializers;
using Sq1.Core.Support;

namespace Sq1.Core.Repositories {
	// the hackiest implementation in the whole project :(
	public class RepositoryJsonDataSource : RepositoryJsonsInFolder<DataSource> {
		public RepositoryCustomMarketInfo MarketInfoRepository;
		public OrderProcessor OrderProcessor;

		public event EventHandler<DataSourceSymbolEventArgs> OnSymbolAdded;
		public event EventHandler<DataSourceSymbolEventArgs> OnSymbolCanBeRemoved;
		public event EventHandler<DataSourceSymbolEventArgs> OnSymbolRemovedDone;
		public event EventHandler<DataSourceSymbolEventArgs> OnSymbolRenamed;

		public RepositoryJsonDataSource() : base() {
			base.CheckIfValidAndShouldBeAddedAfterDeserialized = this.dataSourceDeserializedInitializePriorToAdding;
		}
		
		public void Initialize(string rootPath, string subfolder,
				IStatusReporter statusReporter,
				RepositoryCustomMarketInfo marketInfoRepository, OrderProcessor orderProcessor) {
			base.Initialize(rootPath, subfolder, statusReporter, this.dataSourceDeserializedInitializePriorToAdding);
			this.MarketInfoRepository = marketInfoRepository;
			this.OrderProcessor = orderProcessor;
		}
		
		private bool dataSourceDeserializedInitializePriorToAdding(string thisOne, DataSource dsDeserialized) {
			if (dsDeserialized.MarketInfo == null && string.IsNullOrEmpty(dsDeserialized.MarketName) == false) {
				dsDeserialized.MarketInfo = MarketInfoRepository.FindMarketInfoOrNew(dsDeserialized.MarketName);
			}
			// dsDeserialized.Initialize() knows better what's best for unconfigured DS
//			if (dsDeserialized.StaticProvider == null) {
//				string msg = "dsAdding[" + dsDeserialized.ToString() + "] has StaticProvider=null;"
//					+ " FAILED to deserialize from " + thisOne + "; continuing with next DataSource";
//				base.StatusReporter.PopupException(msg);
//				return false;
//			}

			//string barDataStorePath = Path.Combine(base.RootPath, dsDeserialized.StaticProvider.GetType().Name);
			try {
				dsDeserialized.Initialize(base.AbsPath, this.OrderProcessor, base.StatusReporter);
			} catch (Exception ex) {
				string msg = "FAILED to dsDeserialized.InitializeAndScan(" + base.AbsPath + ")"
					//+ "; //Exception shouldn't break all DataSourceManager.DataSourcesDeserialize()"
					//+ " and make other *Providers unusable"
					+ " from " + thisOne + "; still adding to the Tree";
				//base.StatusReporter.PopupException(msg, ex);
				Assembler.PopupException(msg, ex);
			}
			return true;
		}

		public override void ItemAddCascade(DataSource itemCandidate, object sender = null) {
			itemCandidate.Initialize(base.AbsPath, this.OrderProcessor, base.StatusReporter);
			//v1 WILL_DELETES_BAR_FILE_IF_RENAME_FAILS itemCandidate.CreateDeleteBarFilesToSymbolsDeserialized();
			//v2 
			itemCandidate.SymbolsRebuildReadDataSourceSubFolderAfterDeserialization();
		}
		public override void ItemDeleteCascade(DataSource itemStored, object sender = null) {
			itemStored.DataSourceFolderDeleteWithSymbols();
		}
		public override void ItemCanBeDeletedCascade(NamedObjectJsonEventArgs<DataSource> args, object sender = null) {
		}
		public override void ItemRenameCascade(DataSource itemRenamed, string oldName, object sender = null) {
			itemRenamed.DataSourceFolderRename(oldName);
		}

		public DataSourceSymbolEventArgs SymbolCanBeDeleted(DataSource dataSource, string symbolToDelete, object sender = null) {
			if (sender == null) sender = this;
			var args = new DataSourceSymbolEventArgs(dataSource, symbolToDelete);
			if (this.OnSymbolCanBeRemoved == null) return args;
			this.OnSymbolCanBeRemoved(sender, args);
			return args;
		}
		public void SymbolAdd(DataSource dataSource, string symbolToAdd, object sender = null) {
			if (sender == null) sender = this;
			string msig = " RepositoryJsonDataSource.SymbolAdd(" + dataSource + ", " + symbolToAdd + "): ";
			try {
				dataSource.SymbolAdd(symbolToAdd);
				base.SerializeSingle(dataSource);
				if (this.OnSymbolAdded == null) return;
				this.OnSymbolAdded(sender, new DataSourceSymbolEventArgs(dataSource, symbolToAdd));
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
				if (this.OnSymbolRenamed == null) return;
				this.OnSymbolRenamed(sender, new DataSourceSymbolRenamedEventArgs(dataSource, newSymbolName, oldSymbolName));
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
					if (this.OnSymbolRemovedDone == null) return;
					// since you answered DataSourceEventArgs.DoNotDeleteThisDataSourceBecauseItsUsedElsewhere=false,
					// you were aware that OnItemRemovedDone is invoked on a detached object
					this.OnSymbolRemovedDone(sender, args);
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
		public List<Account> AccountsFromUnderlyingBrokerProviders { get {
				List<Account> ret = new List<Account>();
				foreach (DataSource ds in base.ItemsAsList) {
					if (ds.BrokerProvider == null) continue;
					if (ds.BrokerProvider.AccountAutoPropagate == null) continue;
					if (ret.Contains(ds.BrokerProvider.AccountAutoPropagate)) continue;
					ret.Add(ds.BrokerProvider.AccountAutoPropagate);
				}
				return ret;
			} }
		public string AccountsFromUnderlyingBrokerProvidersAsString { get {
				string ret = "";
				foreach (Account account in AccountsFromUnderlyingBrokerProviders) {
					ret += "[" + account.ToString() + "] ";
				}
				ret.TrimEnd(' ');
				return ret;
			} }
		public ToolStripMenuItem[] CtxAccountsAllCheckedFromUnderlyingBrokerProviders { get {
				List<Account> accts = this.AccountsFromUnderlyingBrokerProviders;
				var ret = new List<ToolStripMenuItem>();
				foreach (Account acct in accts) {
					ToolStripMenuItem mni = new ToolStripMenuItem();
					mni.Checked = true;
					mni.Name = "mniAccount_" + acct.AccountNumber;
					mni.Text = acct.AccountNumber;
					if (acct.BrokerProvider != null) {
						mni.Image = acct.BrokerProvider.Icon;
					}
					//mni.Size = new System.Drawing.Size(236, 22);
					//mni.Click += new System.EventHandler(this.btnEdit_Click);
					ret.Add(mni);
				}
				return ret.ToArray();
			} }
		public List<string> AccountNumbersFromUnderlyingBrokerProviders { get {
				List<string> ret = new List<string>();
				foreach (Account account in AccountsFromUnderlyingBrokerProviders) {
					if (ret.Contains(account.AccountNumber)) continue;
					ret.Add(account.AccountNumber);
				}
				return ret;
			} }
		public Account FindAccount(string accountNumber) {
			foreach (Account current in AccountsFromUnderlyingBrokerProviders) {
				if (current.AccountNumber == accountNumber) return current;
			}
			return null;
		}
		public int UsedTimes(MarketInfo marketInfo) {
			int ret = 0;
			foreach (DataSource ds in base.ItemsAsList) {
				if (ds.MarketInfo == marketInfo) ret++;
			}
			return ret;
		}		
	}
}