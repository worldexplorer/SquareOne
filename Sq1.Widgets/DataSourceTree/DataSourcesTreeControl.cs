using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;	//[Browsable(true)]
using System.Drawing;

using BrightIdeasSoftware;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.Repositories;
using Sq1.Core.Serializers;
using Sq1.Core.Streaming;
using Sq1.Core.Charting;

namespace Sq1.Widgets.DataSourcesTree {
	public partial class DataSourcesTreeControl : UserControl {
				RepositoryJsonDataSources				dataSourceRepository;
				Dictionary<Type, int>					imageIndexByStreamingAdapterType = new Dictionary<Type, int>();

		public	DataSource								DataSourceSelected	{ get; private set; }
		public	string									SymbolSelected		{ get; private set; }
		public	ChartShadow								ChartSelected		{ get; private set; }

				bool									ignoreExpandCollapseEventsDuringInitializationOrUninitialized;
				DataSourceTreeDataSnapshot				dataSnapshot;
				Serializer<DataSourceTreeDataSnapshot>	dataSnapshotSerializer;

		public	string TreeFirstColumnNameText {
			get { return this.olvcName.Text; }
			set { this.olvcName.Text = value; }
		}

		public	bool									DisplayingThirdLevel_withChartsOpen { get; private set; }


		[Browsable(true)]
		public bool AppendMarketToDataSourceName { get; set; }

		public List<ToolStripMenuItem> DataSourcesAsMniList { get {
			List<ToolStripMenuItem> ret = new List<ToolStripMenuItem>();
			foreach (DataSource ds in Assembler.InstanceInitialized.RepositoryJsonDataSources.ItemsAsList) {
				ToolStripMenuItem mni = new ToolStripMenuItem();
				mni.Text = ds.Name;
				mni.Tag = ds;
				//mni.Image = ...
				ret.Add(mni);
			}
			return ret;
		} }
		public DataSourcesTreeControl() {
			this.InitializeComponent();
			this.AppendMarketToDataSourceName = true;
			this.dataSourceTreeListViewCustomize();

			this.dataSnapshotSerializer = new Serializer<DataSourceTreeDataSnapshot>();
			this.OlvTree.Expanded += new EventHandler<TreeBranchExpandedEventArgs>(tree_Expanded);
			this.OlvTree.Collapsed += new EventHandler<TreeBranchCollapsedEventArgs>(tree_Collapsed);
			this.ignoreExpandCollapseEventsDuringInitializationOrUninitialized = true;
			this.mniSymbolCopyToAnotherDataSource.DropDownItems.Add("CAUSING_SUBMENU_TRIANGLE_TO_APPEAR__WILL_BE_CLEARED_ON_OPENING");
		}
		public void Initialize(RepositoryJsonDataSources dataSourceRepository, bool addThirdLevel_withChartsOpen = true) {
			this.dataSourceRepository = dataSourceRepository;
			this.DisplayingThirdLevel_withChartsOpen = addThirdLevel_withChartsOpen;

			try {
				bool createdNewFile = this.dataSnapshotSerializer.Initialize(this.dataSourceRepository.RootPath,
					"Sq1.Widgets.DataSourcesTree.DataSourceTreeDataSnapshot.json", "Workspaces",
					Assembler.InstanceInitialized.AssemblerDataSnapshot.WorkspaceCurrentlyLoaded, true, true);
				this.dataSnapshot = this.dataSnapshotSerializer.Deserialize();
				if (createdNewFile) {
					this.dataSnapshotSerializer.Serialize();
				}
			} catch (Exception ex) {
				Assembler.PopupException(" DataSourcesTreeControl.Initialize()", ex);
			}
			
			this.PopulateDataSourcesIntoTreeListView();
			this.populateDataSnapshotDeserialized();
			
			// TODO MULTIPLE_INITIALIZATIONS_WILL_INVOKE_YOUR_HANDLERS_MULTIPLE_TIMES
			this.dataSourceRepository.OnItemAdded			+= new EventHandler<NamedObjectJsonEventArgs<DataSource>>(	dataSourceRepository_OnDataSourceAdded);
			this.dataSourceRepository.OnItemRenamed			+= new EventHandler<NamedObjectJsonEventArgs<DataSource>>(	dataSourceRepository_OnDataSourceRenamed);
			this.dataSourceRepository.OnItemCanBeRemoved	+= new EventHandler<NamedObjectJsonEventArgs<DataSource>>(	dataSourceRepository_OnDataSourceCanBeRemoved);
			this.dataSourceRepository.OnItemRemovedDone		+= new EventHandler<NamedObjectJsonEventArgs<DataSource>>(	dataSourceRepository_OnDataSourceRemovedDone);
			this.dataSourceRepository.OnSymbolAdded			+= new EventHandler<DataSourceSymbolEventArgs>(				dataSourceRepository_OnSymbolAdded);
			this.dataSourceRepository.OnSymbolRenamed		+= new EventHandler<DataSourceSymbolRenamedEventArgs>(		dataSourceRepository_OnSymbolRenamed);
			this.dataSourceRepository.OnSymbolCanBeRemoved	+= new EventHandler<DataSourceSymbolEventArgs>(				dataSourceRepository_OnSymbolCanBeRemoved);
			this.dataSourceRepository.OnSymbolRemovedDone	+= new EventHandler<DataSourceSymbolEventArgs>(				dataSourceRepository_OnSymbolRemovedDone);
		}
		void populateDataSnapshotDeserialized() {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.populateDataSnapshotDeserialized(); });
				return;
			}
			try {
				this.OlvTree.HeaderStyle = this.dataSnapshot.ShowHeader ? ColumnHeaderStyle.Clickable : ColumnHeaderStyle.None;
				this.mniShowHeader.Checked = this.dataSnapshot.ShowHeader;
				
				this.pnlSearch			.Visible = this.dataSnapshot.ShowSearchBar;
				this.mniShowSearchBar	.Checked = this.dataSnapshot.ShowSearchBar;

				this.AppendMarketToDataSourceName									= this.dataSnapshot.AppendMarketToDataSourceName;
				this.mniAppendMarketNameToDataSourceToolStripMenuItem	.Checked	= this.dataSnapshot.AppendMarketToDataSourceName;
			} catch (Exception ex) {
				string msg = "SHOULD_NEVER_HAPPEN StrategiesTreeControl.populateDataSnapshotDeserialized() ";
				Assembler.PopupException(msg, ex);
			}
		}
		public void PopulateDataSourcesIntoTreeListView() {
			List<DataSource> dataSources = dataSourceRepository.ItemsAsList;
			this.imageList.Images.Clear();
			foreach (DataSource ds in dataSources) {
				StreamingAdapter provider = ds.StreamingAdapter;
				if (provider == null) continue;
				this.PopulateIconForDataSource(ds);
			}
			this.OlvTree.SetObjects(dataSources);
			this.ignoreExpandCollapseEventsDuringInitializationOrUninitialized = true;
			foreach (DataSource dsEach in dataSources) {
				if (this.dataSnapshot.DataSourceFoldersExpanded.Contains(dsEach.Name) == false) continue;
				this.OlvTree.Expand(dsEach);
			}
			this.OlvTree.RebuildAll(true);
			this.OlvTree.RefreshObjects(dataSources);		// #1 may be icons for recently added/edited datasources will show up/update?
			this.OlvTree.SmallImageList = this.imageList; 	// #2 may be icons for recently added/edited datasources will show up/update?
			this.ignoreExpandCollapseEventsDuringInitializationOrUninitialized = false;
		}
		public void PopulateIconForDataSource(DataSource ds) {
			string msig = " //PopulateIconForDataSource()";
			if (ds == null) {
				string msg = "ds==null";
				Assembler.PopupException(msg + msig);
				return;
			}
			msig = " //PopulateIconForDataSource(" + ds.ToString() + ")";

			StreamingAdapter adapter = ds.StreamingAdapter;
			if (adapter == null) {
				string msg = "LOOKS_LIKE_YOU_REMOVED_STREAMING_IN_DataSourceEditor_AND_SAVED_DataSource?";
				//Assembler.PopupException(msg + msig);
				return;
			}

			if (adapter.Icon == null) {
				string streamingFullName = adapter.GetType().FullName;
				if (streamingFullName == "Sq1.Core.Livesim.LivesimStreamingDefault") return;
				string msg = "ds.StreamingAdapter[" + adapter + "].Icon==null";
				Assembler.PopupException(msg + msig);
				return;
			}

			this.imageList.Images.Add(adapter.Icon);
			int providerIconImageIndex = this.imageList.Images.Count - 1;
			if (this.imageIndexByStreamingAdapterType.ContainsKey(adapter.GetType()) == false) {
				this.imageIndexByStreamingAdapterType.Add(adapter.GetType(), providerIconImageIndex);
			}
		}
		int getProviderImageIndexForDataSource(DataSource dataSource) {
			StreamingAdapter adapter = dataSource.StreamingAdapter;
			if (adapter == null) return -1;
			if (this.imageIndexByStreamingAdapterType.ContainsKey(adapter.GetType()) == false) return -1;
			return this.imageIndexByStreamingAdapterType[adapter.GetType()];
		}
		void syncSymbolAndDataSourceSelectedFromRowIndexClicked(int itemRowIndex) {
			string msig = " //DataSourcesTreeControl.syncSymbolAndDataSourceSelectedFromRowIndexClicked(" + itemRowIndex + ")";
			if (this.OlvTree.SelectedObject == null) {
				string msg = "IM_HERE_WHEN_I_CLICKED_PLUS_TO_EXPAND_COLLAPSE";
				Assembler.PopupException(msg + msig, null, false);
				return;
			}
			try {
				// first time loaded, nothing is selected event after right click; (SelectedObject as DataSource) was NullReferenceException 
				ChartShadow chartShadowClicked = this.OlvTree.SelectedObject as ChartShadow;
				if (chartShadowClicked != null) {
					this.ChartSelected = chartShadowClicked;
					this.DataSourceSelected = null;
					this.SymbolSelected = null;
					return;
				}

				DataSource dataSourceClicked = this.OlvTree.SelectedObject as DataSource;
				if (dataSourceClicked != null) {
					this.DataSourceSelected = dataSourceClicked;
					this.SymbolSelected = null;
					this.ChartSelected = null;
					return;
				}
			} catch (NullReferenceException) {
				string msg = "OLV_INTERNAL_EXCEPTION?...";
				Assembler.PopupException(msg + msig, null, false);
			}

			this.ChartSelected = null;
			string symbol = null;
			DataSource dataSourceParent = this.OlvTree.SelectedObject as DataSource;
			int indexCurrent = 0;
			// stop by breakpoint to see what's inside ObjectsForClustering:
			// [0] => DataSource1
			// [1] => DataSource1.Symbol1
			// [2] => DataSource1.Symbol2
			// [3] => DataSource1.Symbol3
			// [4] => DataSource2
			// [5] => DataSource2.Symbol1
			// [6] => DataSource2.Symbol2
			foreach (object dsOrSymbol in this.OlvTree.ObjectsForClustering) {
				DataSource dataSource = dsOrSymbol as DataSource;
				SymbolOfDataSource symbolOfDataSource = dsOrSymbol as SymbolOfDataSource;
				if (dataSource != null) {
					if (dataSourceParent == null || dataSourceParent != dataSource) dataSourceParent = dataSource;
				} else if (symbolOfDataSource != null) {
					if (indexCurrent > itemRowIndex) break;
					if (indexCurrent == itemRowIndex) {
						symbol = symbolOfDataSource.Symbol;
						break;
					}
				}
				indexCurrent++;
			}
			this.DataSourceSelected = dataSourceParent;
			this.SymbolSelected = symbol;
		}
		public void SelectDatasource(string dataSourceName) {
			string msig = " SelectDatasource([" + dataSourceName + "])";
			DataSource dataSourceFound = null;
			int indexForDataSource = 0;
			foreach (object shouldBeDataSource in this.OlvTree.Objects) {
				DataSource dataSourceEach = shouldBeDataSource as DataSource;
				//if (dataSourceEach == null) continue;	//that was Symbol1-2-3
				if (dataSourceEach.Name == dataSourceName) {
					dataSourceFound = dataSourceEach;
					break;
				}
				indexForDataSource++;
				if (this.OlvTree.IsExpanded(dataSourceEach)) {
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
			this.OlvTree.Expand(dataSourceFound);
			this.OlvTree.EnsureModelVisible(dataSourceFound);
			this.OlvTree.SelectObject(dataSourceFound);

			this.DataSourceSelected = dataSourceFound;
			this.SymbolSelected = null;
		}
		public void SelectSymbol(string dataSourceName, string symbol) {
			string msig = " SelectSymbol([" + dataSourceName + "], [" + symbol + "])";

			DataSource dataSourceFound = null;
			int indexForDataSource = 0;
			int indexForSymbol = -1;
			//foreach (object dsOrSymbol in this.tree.ObjectsForClustering) {
			foreach (object shouldBeDataSource in this.OlvTree.Objects) {
				DataSource dataSourceEach = shouldBeDataSource as DataSource;
				//if (dataSourceEach == null) continue;	//that was Symbol1-2-3
				if (dataSourceEach.Name == dataSourceName) {
					dataSourceFound = dataSourceEach;
					break;
				}
				indexForDataSource++;
				if (this.OlvTree.IsExpanded(dataSourceEach)) {
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
					this.OlvTree.Expand(dataSourceFound);
					this.OlvTree.EnsureModelVisible(dataSourceFound);
					this.OlvTree.SelectObject(dataSourceFound);

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
				this.OlvTree.Expand(dataSourceName);	// doesn't really expand the collapsed "Qmock" but let the selected row go "under" it; whatever, if I collapsed then I don't need the content
				if (this.OlvTree.SelectedIndex != indexToSelect) {
					this.OlvTree.SelectedIndex  = indexToSelect;
					this.OlvTree.RefreshSelectedObjects();
				}
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			}

			this.DataSourceSelected = dataSourceFound;
			this.SymbolSelected = symbol;
		}

		public void ChartShadow_Select_HideSelectionFalse(ChartShadow chartControl) {
			int indexFoundForSelectObject = this.OlvTree.VirtualListDataSource.GetObjectIndex(chartControl);

			//if (indexFoundForSelectObject == -1) {
			//	//Sq1.Core.DataSource has to notify Sq1.Widgets.DataSourcesTreeControl about a new chart added;
			//	//I'm lazy to loop it through JsonRepository like OnDataSourceAdded / OnSymbolAdded
			//	//so I'm just rebuilding the list making this.OlvTree.ChildrenGetter to DataSource.ChartsOpenForSymbol.FindContentsOf__nullUnsafe(symbol)
			//	//var model = this.OlvTree.TreeModel;
			//	this.OlvTree.RebuildAll(true);		// without Rebuild, OlvTree.SelectObject(chartControl) will also find index=-1
			//	this.OlvTree.Expand(chartControl.SymbolOfDataSource);
			//	this.OlvTree.Expand(chartControl);
			//	this.OlvTree.EnsureModelVisible(chartControl);
			//	indexFoundForSelectObject = this.OlvTree.VirtualListDataSource.GetObjectIndex(chartControl);
			//	//var model2 = this.OlvTree.TreeModel;
			//	//var lvi = this.OlvTree.ModelToItem(chartControl);
			//}

			//MUST_BE_HERE_ALL_3_LINES__WILL_100%_EXPAND
			this.OlvTree.Expand(chartControl.SymbolOfDataSource);
			this.OlvTree.Expand(chartControl);
			this.OlvTree.EnsureModelVisible(chartControl);

			//this.OlvTree.RefreshObject(chartControl);		// REQUESTS_this.OlvTree.ChildrenGetter

			//this.OlvTree.SelectedIndex = -1;
			this.OlvTree.SelectedObject = chartControl;		//this.OlvTree.SelectObject(chartControl, true);
			//this.OlvTree.RefreshSelectedObjects();

			//HideSelection=FALSE FIXED_IT_NOW_STAYS_SELECTED
			//HideSelection=FALSE FIXED_IT_NOW_STAYS_SELECTED
			//HideSelection=FALSE FIXED_IT_NOW_STAYS_SELECTED (you may need to get the DocumentPane focused upstack, and this one will be pale blue)
			//this.OlvTree.Focus();	// DIDNT_FIX_IT
			//HideSelection=FALSE FIXED_IT_NOW_STAYS_SELECTED
			//HideSelection=FALSE FIXED_IT_NOW_STAYS_SELECTED
			//HideSelection=FALSE FIXED_IT_NOW_STAYS_SELECTED

			//if (this.OlvTree.SelectedIndex == -1) {
			//	string msg = "TRY_HARDER#1";
			//}
			//object selectedObject = this.OlvTree.GetSelectedObject();
			//if (selectedObject != chartControl) {
			//	string msg = "TRY_HARDER#2";
			//}
		}
	}
}