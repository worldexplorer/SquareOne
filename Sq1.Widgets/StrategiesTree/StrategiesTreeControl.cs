using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.Repositories;
using Sq1.Core.Serializers;
using Sq1.Core.StrategyBase;

namespace Sq1.Widgets.StrategiesTree {
	public partial class StrategiesTreeControl {
		RepositoryDllJsonStrategy strategyRepository;

		public string FolderSelected;
		public Strategy StrategySelected;
		
		bool ignoreExpandCollapseEventsDuringInitializationOrUninitialized;
		StrategiesTreeDataSnapshot dataSnapshot;
		Serializer<StrategiesTreeDataSnapshot> dataSnapshotSerializer;

		public StrategiesTreeControl() {
			this.InitializeComponent();
			this.strategyTreeListViewCustomize();

			this.dataSnapshotSerializer = new Serializer<StrategiesTreeDataSnapshot>();
			this.tree.Expanded += new EventHandler<TreeBranchExpandedEventArgs>(tree_Expanded);
			this.tree.Collapsed += new EventHandler<TreeBranchCollapsedEventArgs>(tree_Collapsed);
			this.ignoreExpandCollapseEventsDuringInitializationOrUninitialized = true;
		}
		public void Initialize(RepositoryDllJsonStrategy strategyRepository) {
			this.strategyRepository = strategyRepository;
			
			bool createdNewFile = this.dataSnapshotSerializer.Initialize(this.strategyRepository.RootPath,
				"Sq1.Widgets.StrategiesTree.StrategiesTreeDataSnapshot.json", "Workspaces",
				Assembler.InstanceInitialized.AssemblerDataSnapshot.CurrentWorkspaceName, false, true);
			this.dataSnapshot = this.dataSnapshotSerializer.Deserialize();
			
			this.populateStrategyRepositoryIntoTreeListView();			
			this.populateDataSnapshotDeserialized();
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
		void populateStrategyRepositoryIntoTreeListView() {
			List<string> strategyFolders = strategyRepository.AllFoldersAvailable;
			this.tree.SmallImageList = this.imageList;
			this.tree.SetObjects(strategyFolders);
			this.ignoreExpandCollapseEventsDuringInitializationOrUninitialized = true;
			foreach (string folder in strategyFolders) {
				if (this.dataSnapshot.StrategyFoldersExpanded.Contains(folder) == false) continue;
				this.tree.Expand(folder);
			}
			this.tree.RebuildAll(true);
			this.ignoreExpandCollapseEventsDuringInitializationOrUninitialized = false;
		}
		void syncFolderStrategySelectedFromRowIndexClicked(int itemIndexSelected) {
			string msig = " StrategiesTreeControl.syncFolderStrategySelectedFromRowIndexClicked(" + itemIndexSelected + ")";
			if (this.tree.SelectedObject == null) {
				string msg = "IM_HERE_WHEN_I_CLICKED_PLUS_TO_EXPAND_COLLAPSE";
				Assembler.PopupException(msg + msig, null, false);
				return;
			}
			string folder = this.tree.SelectedObject as string;
			if (folder != null) {
				this.FolderSelected = folder;
				this.StrategySelected = null;
				return;
			}
			Strategy strategy = this.tree.SelectedObject as Strategy;
			if (strategy == null) {
				string msg = "this.tree.SelectedObject is not a Strategy and wasn't a Folder either";
				Assembler.PopupException(msg + msig);
				return;
			}
			this.StrategySelected = strategy;
			this.FolderSelected = strategy.StoredInFolderRelName;
		}
		
		const string PREFIX_SCRIPT_CONTEXT_MNI_NAME = "mniContextScript_";
		List<ToolStripItem> CreateMnisForScriptContexts(Strategy strategy) {
			var ret = new List<ToolStripItem>();
			foreach (string scriptContextName in strategy.ScriptContextsByName.Keys) {
				string mniName = PREFIX_SCRIPT_CONTEXT_MNI_NAME + scriptContextName;
				ToolStripMenuItem mni = new ToolStripMenuItem(scriptContextName, null, this.mniStrategyOpenWithScriptContext_Click, mniName);
				if (scriptContextName == ContextScript.DEFAULT_NAME) {
					mni.Font = new Font(mni.Font.FontFamily, mni.Font.Size, FontStyle.Bold);
				}
				ret.Add(mni);
			}
			return ret;
		}
		List<ToolStripItem> CreateMnisForFolders(string excludeStrategysOwnFolder) {
			var ret = new List<ToolStripItem>();
			foreach (string folderPurelyJson in Assembler.InstanceInitialized.RepositoryDllJsonStrategy.FoldersPurelyJson) {
				var mni = new ToolStripMenuItem(folderPurelyJson, null, this.mniStrategyMoveToAnotherFolder_Click, "mni" + folderPurelyJson);
				if (string.IsNullOrEmpty(excludeStrategysOwnFolder) == false && excludeStrategysOwnFolder == folderPurelyJson) {
					mni.Enabled = false;
				}
				ret.Add(mni);
			}
			return ret;
		}
		public void UnSelectStrategy() {
			//this.tree.SelectSelectObject(null);
			this.tree.SelectedIndex = -1;
		}
		public void SelectStrategy(Strategy strategy) {
			if (strategy == null) {
				string msg = "why would you pass strategy[" + strategy+ "]=null???";
				Assembler.PopupException(msg);
				return;
			}
			
			if (Assembler.InstanceInitialized.RepositoryDllJsonStrategy.AllStrategiesAvailable.Contains(strategy) == false) {
				string msg = "you may have removed {strategy[" + strategy+ "]"
					+ "from tree before application restart"
					+ ", but an open chart with this strategy became Active and requests SelectStrategy()";
				Assembler.PopupException(msg);
				return;
			}
			if (this.tree.IsExpanded(strategy.StoredInFolderRelName) == false) {
				this.tree.Expand(strategy.StoredInFolderRelName);
				this.tree_Expanded(this, new TreeBranchExpandedEventArgs(strategy.StoredInFolderRelName, null));
			}
			this.tree.EnsureModelVisible(strategy);
			this.tree.SelectObject(strategy);
			//int index = this.tree.IndexOf(strategy);
			//this.tree.SelectedIndex = index; 
			this.StrategySelected = strategy;
		}
	}
}