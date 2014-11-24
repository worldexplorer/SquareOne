using System;
using System.Collections.Generic;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.Repositories;
using Sq1.Core.Serializers;
using Sq1.Core.Streaming;

namespace Sq1.Widgets.DataSourcesTree {
	public partial class DataSourcesTreeControl : UserControl {
		RepositoryJsonDataSource dataSourceRepository;
		Dictionary<Type, int> imageIndexByStaticProviderType = new Dictionary<Type, int>();

		public DataSource DataSourceSelected;
		public string SymbolSelected;

		bool ignoreExpandCollapseEventsDuringInitializationOrUninitialized;
		DataSourceTreeDataSnapshot dataSnapshot;
		Serializer<DataSourceTreeDataSnapshot> dataSnapshotSerializer;

		public string TreeFirstColumnNameText {
			get { return this.olvColumnName.Text; }
			set { this.olvColumnName.Text = value; }
		}
		private bool showScaleIntervalInsteadOfMarket;
		public bool ShowScaleIntervalInsteadOfMarket {
			get { return this.showScaleIntervalInsteadOfMarket; }
			set { this.showScaleIntervalInsteadOfMarket = value; }
		}
		public DataSourcesTreeControl() {
			this.InitializeComponent();
			this.dataSourceTreeListViewCustomize();

			this.dataSnapshotSerializer = new Serializer<DataSourceTreeDataSnapshot>();
			this.tree.Expanded += new EventHandler<TreeBranchExpandedEventArgs>(tree_Expanded);
			this.tree.Collapsed += new EventHandler<TreeBranchCollapsedEventArgs>(tree_Collapsed);
			this.ignoreExpandCollapseEventsDuringInitializationOrUninitialized = true;
		}
		public void Initialize(RepositoryJsonDataSource dataSourceRepository) {
			this.dataSourceRepository = dataSourceRepository;

			try {
				bool createdNewFile = this.dataSnapshotSerializer.Initialize(this.dataSourceRepository.RootPath,
					"Sq1.Widgets.DataSourcesTree.DataSourceTreeDataSnapshot.json", "Workspaces",
					Assembler.InstanceInitialized.AssemblerDataSnapshot.CurrentWorkspaceName, true, true);
				this.dataSnapshot = this.dataSnapshotSerializer.Deserialize();
				if (createdNewFile) {
					this.dataSnapshotSerializer.Serialize();
				}
			} catch (Exception ex) {
				Assembler.PopupException(" DataSourcesTreeControl.Initialize()", ex);
			}
			
			this.populateDataSourcesIntoTreeListView();
			this.populateDataSnapshotDeserialized();
			
			// TODO MULTIPLE_INITIALIZATIONS_WILL_INVOKE_YOUR_HANDLERS_MULTIPLE_TIMES
			this.dataSourceRepository.OnItemAdded += new EventHandler<NamedObjectJsonEventArgs<DataSource>>(dataSourceRepository_OnDataSourceAdded);
			this.dataSourceRepository.OnItemRenamed += new EventHandler<NamedObjectJsonEventArgs<DataSource>>(dataSourceRepository_OnDataSourceRenamed);
			this.dataSourceRepository.OnItemCanBeRemoved += new EventHandler<NamedObjectJsonEventArgs<DataSource>>(dataSourceRepository_OnDataSourceCanBeRemoved);
			this.dataSourceRepository.OnItemRemovedDone += new EventHandler<NamedObjectJsonEventArgs<DataSource>>(dataSourceRepository_OnDataSourceRemovedDone);
			this.dataSourceRepository.OnSymbolAdded += new EventHandler<DataSourceSymbolEventArgs>(dataSourceRepository_OnSymbolAdded);
			this.dataSourceRepository.OnSymbolRenamed += new EventHandler<DataSourceSymbolEventArgs>(dataSourceRepository_OnSymbolRenamed);
			this.dataSourceRepository.OnSymbolCanBeRemoved += new EventHandler<DataSourceSymbolEventArgs>(dataSourceRepository_OnSymbolCanBeRemoved);
			this.dataSourceRepository.OnSymbolRemovedDone += new EventHandler<DataSourceSymbolEventArgs>(dataSourceRepository_OnSymbolRemovedDone);
		}
		void populateDataSnapshotDeserialized() {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.populateDataSnapshotDeserialized(); });
				return;
			}
			try {
				this.tree.HeaderStyle = this.dataSnapshot.ShowHeader ? ColumnHeaderStyle.Clickable : ColumnHeaderStyle.None;
				this.mniShowHeader.Checked = this.dataSnapshot.ShowHeader;
				
				this.tableLayoutPanel1.Visible = this.dataSnapshot.ShowSearchBar;
				this.mniShowSearchBar.Checked = this.dataSnapshot.ShowSearchBar;
			} catch (Exception ex) {
				string msg = "SHOULD_NEVER_HAPPEN StrategiesTreeControl.populateDataSnapshotDeserialized() ";
				Assembler.PopupException(msg, ex);
			}
		}
		void populateDataSourcesIntoTreeListView() {
			var dataSources = dataSourceRepository.ItemsAsList;
			this.imageList.Images.Clear();
			foreach (DataSource ds in dataSources) {
				StreamingProvider provider = ds.StreamingProvider;
				if (provider == null) continue;
				this.populateIconForDataSource(ds);
			}
			this.tree.SetObjects(dataSources);
			this.ignoreExpandCollapseEventsDuringInitializationOrUninitialized = true;
			foreach (var dsEach in dataSources) {
				if (this.dataSnapshot.DataSourceFoldersExpanded.Contains(dsEach.Name) == false) continue;
				this.tree.Expand(dsEach);
			}
			this.tree.RebuildAll(true);
			this.tree.RefreshObjects(dataSources);		// #1 may be icons for recently added/edited datasources will show up/update?
			this.tree.SmallImageList = this.imageList; 	// #2 may be icons for recently added/edited datasources will show up/update?
			this.ignoreExpandCollapseEventsDuringInitializationOrUninitialized = false;
		}
		void populateIconForDataSource(DataSource ds) {
			if (ds == null) return;
			StreamingProvider provider = ds.StreamingProvider;
			if (provider == null) return;
			this.imageList.Images.Add(provider.Icon);
			int providerIconImageIndex = this.imageList.Images.Count - 1;
			if (this.imageIndexByStaticProviderType.ContainsKey(provider.GetType()) == false) {
				this.imageIndexByStaticProviderType.Add(provider.GetType(), providerIconImageIndex);
			}
		}
		int getProviderImageIndexForDataSource(DataSource dataSource) {
			var provider = dataSource.StreamingProvider;
			if (provider == null) return -1;
			if (this.imageIndexByStaticProviderType.ContainsKey(provider.GetType()) == false) return -1;
			return this.imageIndexByStaticProviderType[provider.GetType()];
		}
		void syncSymbolAndDataSourceSelectedFromRowIndexClicked(int itemRowIndex) {
			string msig = " //DataSourcesTreeControl.syncSymbolAndDataSourceSelectedFromRowIndexClicked(" + itemRowIndex + ")";
			if (this.tree.SelectedObject == null) {
				string msg = "IM_HERE_WHEN_I_CLICKED_PLUS_TO_EXPAND_COLLAPSE";
				Assembler.PopupException(msg + msig, null, false);
				return;
			}
			try {
			//if ((this.tree.SelectedObject is NullReferenceException) == false) {
				// first time loaded, nothing is selected event after right click; (SelectedObject as DataSource) was NullReferenceException 
				DataSource dataSourceClicked = this.tree.SelectedObject as DataSource;
				if (dataSourceClicked != null) {
					this.DataSourceSelected = dataSourceClicked;
					this.SymbolSelected = null;
					return;
				}
			//}
			} catch (NullReferenceException) {
				string msg = "OLV_INTERNAL_EXCEPTION?...";
				Assembler.PopupException(msg + msig, null, false);
			}
			string symbol = null;
			DataSource dataSourceParent = this.tree.SelectedObject as DataSource;
			int indexCurrent = 0;
			// stop by breakpoint to see what's inside ObjectsForClustering:
			// [0] => DataSource1
			// [1] => DataSource1.Symbol1
			// [2] => DataSource1.Symbol2
			// [3] => DataSource1.Symbol3
			// [4] => DataSource2
			// [5] => DataSource2.Symbol1
			// [6] => DataSource2.Symbol2
			foreach (object dsOrSymbol in this.tree.ObjectsForClustering) {
				if (dsOrSymbol is DataSource) {
					if (dataSourceParent == null || dataSourceParent != dsOrSymbol) dataSourceParent = (DataSource)dsOrSymbol;
				} else {
					if (indexCurrent > itemRowIndex) break;
					if (indexCurrent == itemRowIndex) {
						symbol = dsOrSymbol.ToString();
						break;
					}
				}
				indexCurrent++;
			}
			this.DataSourceSelected = dataSourceParent;
			this.SymbolSelected = symbol;
		}
		public void SelectSymbol(string dataSourceName, string symbol) {
			string msig = " SelectSymbol([" + dataSourceName + "], [" + symbol + "])";

			DataSource dataSourceFound = null;
			int indexForDataSource = 0;
			int indexForSymbol = -1;
			//foreach (object dsOrSymbol in this.tree.ObjectsForClustering) {
			foreach (object shouldBeDataSource in this.tree.Objects) {
				DataSource dataSourceEach = shouldBeDataSource as DataSource;
				//if (dataSourceEach == null) continue;	//that was Symbol1-2-3
				if (dataSourceEach.Name == dataSourceName) {
					dataSourceFound = dataSourceEach;
					break;
				}
				indexForDataSource++;
				if (this.tree.IsExpanded(dataSourceEach)) {
					indexForDataSource += dataSourceEach.Symbols.Count;
				}
			}
			if (dataSourceFound == null) {
				string msg = "DATASOURCE_NOT_FOUND_IN_TREE_OBJECTS dataSourceName[" + dataSourceName + "]"
					+ "; you may have removed from DataSources before application restart"
					+ ", and one of the strategies requested the deleted datasource";
				Assembler.PopupException(msg);
				return;
			}
			if (string.IsNullOrEmpty(symbol) == false) {
				indexForSymbol = dataSourceFound.Symbols.IndexOf(symbol);
				if (indexForSymbol == -1) {
					this.tree.Expand(dataSourceFound);
					this.tree.EnsureModelVisible(dataSourceFound);
					this.tree.SelectObject(dataSourceFound);

					string msg = "SYMBOL_NOT_FOUND_IN_TREE_OBJECTS symbol[" + symbol + "]"
						+ " you may have removed from DataSources before application restart"
						+ ", and one of the strategies requested the deleted datasource";
					Assembler.PopupException(msg);
					return;
				}
			}
			
			//DOESNT_WORK_FOR_SAME_SYMBOLS_FOUND_IN_TWO_DATASOURCES this.tree.EnsureModelVisible(symbolFound);
			//DOESNT_WORK_FOR_SAME_SYMBOLS_FOUND_IN_TWO_DATASOURCES this.tree.SelectObject(symbolFound);
			int indexToSelect = indexForDataSource + indexForSymbol + 1;
			try {
				//throws when I point into Symbol folded inside a collapsed datasource this.tree.EnsureVisible(indexToSelect);
				this.tree.Expand(dataSourceName);	// doesn't really expand the collapsed "Qmock" but let the selected row go "under" it; whatever, if I collapsed then I don't need the content
				this.tree.SelectedIndex = indexToSelect;
				this.tree.RefreshSelectedObjects();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			}

			this.DataSourceSelected = dataSourceFound;
			this.SymbolSelected = symbol;
		}
	}
}