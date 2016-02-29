using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

using BrightIdeasSoftware;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;

using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Widgets.DataSourcesTree {
	public partial class DataSourcesTreeControl {
		void tree_CellClick(object sender, CellClickEventArgs e) {
			if (e.RowIndex == -1) return;
			this.syncSymbolAndDataSourceSelectedFromRowIndexClicked(e.RowIndex);

			if (this.SymbolSelected != null) {
				this.raiseOnSymbolSelected();
				//this.tree.RebuildAll(true);
			} else if (this.OnDataSourceSelected != null) {
				this.raiseOnDataSourceSelected();
			} else if (this.ChartSelected != null) {
				this.raiseOnChartSelected();
			} else {
				string msg = "USER_EXPANDED_THE_TREE NOTHING_VALUABLE_SELECTED__CANT_RAISE_EVENT //DataSourcesTreeControl.tree_CellClick()";
				//Assembler.PopupException(msg);
			}

			this.OlvTree.RebuildAll(true);		// needed for Chart to get removed from old Symbol and move to the Symbol clicked; not needed without level3
		}
		void tree_CellRightClick(object sender, CellRightClickEventArgs e) {
			if (e.RowIndex == -1) { // 1) empty tree, no datasources added yet; 2) clicked on blank area of some tree
				this.mniDataSourceBrief		.Visible = false;
				this.mniDataSourceEdit		.Visible = false;
				this.mniltbDataSourceRename	.Visible = false;
				this.mniDataSourceDelete	.Visible = false;
				this.mniltbSymbolAdd		.Visible = false;
				this.toolStripSeparator1	.Visible = false;
				this.ContextMenuStrip = this.ctxDataSource;
				return;
			}
			this.syncSymbolAndDataSourceSelectedFromRowIndexClicked(e.RowIndex);
			if (this.SymbolSelected != null) {
				if (this.DataSourceSelected == null) {
					throw new Exception("DATASOURCE_NOT_SELECTED_WHILE_SYMBOL_SELECTED");
				}
				this.ContextMenuStrip = this.ctxSymbol;
				//string dataSourceName = base.SelectedNode.Parent.Text;
				//string chartTitlePartial = " - " + symbol + " (" + dataSourceName + ")";
				//ChartForm chart = MainModule.Instance.MainForm.findByChartTitlePartialIfOpen(chartTitlePartial);
				string newExisting = (true) ? "New" : "Existing";	//chart != null
				this.mniNewChartSymbol.Text = newExisting + " Chart for [" + this.SymbolSelected + "]";
				this.mniOpenStrategySymbol	.Text = "New Strategy for [" + this.SymbolSelected + "]";
				this.mniSymbolBarsEditor	.Text = "Bars Editor for [" + this.SymbolSelected + "]";
				this.mniSymbolInfoEditor	.Text = "Symbol Editor for [" + this.SymbolSelected + "]";
				this.mniSymbolBarsEditor	.Text = "Bar Editor for [" + this.SymbolSelected + "]";
				this.mniSymbolFuturesMerger	.Text = "Futures Merger for [" + this.SymbolSelected + "]";
				this.mniSymbolRemove		.Text = "Remove [" + this.SymbolSelected + "] from [" + this.DataSourceSelected.Name + "] DataSource";
				this.mniltbSymbolRenameTo	.TextLeft = "Rename [" + this.SymbolSelected + "] to";
				this.mniltbSymbolRenameTo	.InputFieldValue = this.SymbolSelected;
				this.mniSymbolCopyToAnotherDataSource	.Text = "Copy [" + this.SymbolSelected + "] to another DataSource";
				DataSourceSymbolEventArgs subscribersPolled =
					this.dataSourceRepository.SymbolCanBeDeleted(this.DataSourceSelected, this.SymbolSelected, this);
				this.mniSymbolRemove.Enabled = (subscribersPolled.DoNotDeleteItsUsedElsewhere == false);
				//int imageIndex = getProviderImageIndexForDataSource(this.DataSourceSelected);
				//if (imageIndex == -1) return;
				//Image providerIcon = this.tree.SmallImageList.Images[imageIndex];
				//if (providerIcon != null) {
				//	this.mniNewChartSymbol.Image = providerIcon;
				//	this.mniOpenStrategySymbol.Image = providerIcon;
				//}
			} else {
				NamedObjectJsonEventArgs<DataSource> subscribersPolled =
					this.dataSourceRepository.ItemCanBeDeleted(this.DataSourceSelected, this);
				// REVERSING_AFTER_OUT_OF_TREE_CLICK
				this.mniDataSourceBrief		.Visible = true;
				this.mniDataSourceEdit		.Visible = true;
				this.mniltbDataSourceRename	.Visible = true;
				this.mniDataSourceDelete	.Visible = true;
				this.mniltbSymbolAdd		.Visible = true;
				this.toolStripSeparator1	.Visible = true;

				this.mniDataSourceBrief		.Text = "DataSource [" + this.DataSourceSelected.Name + "][" + this.DataSourceSelected.ScaleInterval + "], [" + this.DataSourceSelected.Symbols.Count + "]Symbols";
				this.mniDataSourceEdit		.Enabled = (subscribersPolled.DoNotDeleteItsUsedElsewhere == false);
				this.mniDataSourceEdit		.Text = "Edit [" + this.DataSourceSelected.Name + "] DataSource";
				this.mniltbDataSourceRename	.TextLeft = "Rename [" + this.DataSourceSelected.Name + "] to";
				this.mniltbSymbolRenameTo	.InputFieldValue = this.SymbolSelected;
				this.mniDataSourceDelete	.Enabled = (subscribersPolled.DoNotDeleteItsUsedElsewhere == false);
				this.mniDataSourceDelete	.Text = "Delete [" + this.DataSourceSelected.Name + "] DataSource";
				this.mniltbSymbolAdd		.TextLeft = "Add Symbol to [" + this.DataSourceSelected.Name + "]";
				this.ContextMenuStrip = this.ctxDataSource;
			}
		}
		void tree_MouseDoubleClick(object sender, MouseEventArgs e) {
			//if (this.SymbolSelected == null) return;
			//this.mniNewChartSymbol_Click(this, null);
		}
		void tree_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
			if (this.pnlSearch.Visible) return;
			this.mniShowSearchBar.Checked = true;
			this.mniShowSearchBar_Click(sender, null);
		}

		void mniNewChartSymbol_Click(object sender, EventArgs e) {
			this.raiseOnNewChartForSymbolClicked();
		}
		void mniBarsEditorSymbol_Click(object sender, EventArgs e) {
			this.raiseOnBarsEditorClicked();
		}
		void mniOpenStrategySymbol_Click(object sender, EventArgs e) {
			this.raiseOnOpenStrategyForSymbolClicked();
		}
		void mniDataSourceDelete_Click(object sender, EventArgs e) {
			if (this.DataSourceSelected == null) {
				string msg = "DataSourcesTree.mniRemoveSymbol_Click(): event OnDataSourceNewClicked: no subscribers";
				Assembler.PopupException(msg);
				return;
			}
			this.dataSourceRepository.ItemDelete(this.DataSourceSelected, this);
		}
		void mniRemoveSymbol_Click(object sender, EventArgs e) {
			if (this.DataSourceSelected == null) {
				string msg = "DataSourcesTree.ctxDataSource_Opening(): this.DataSourceSelected = null";
				Assembler.PopupException(msg);
				return;
			}
			if (this.SymbolSelected == "") return;
			this.dataSourceRepository.SymbolRemove(this.DataSourceSelected, this.SymbolSelected, this);		//blocking!
			this.PopulateDataSourcesIntoTreeListView();
		}
//		void mniDataSourceCreate_Click(object sender, EventArgs e) {
//			this.RaiseOnDataSourceCreateClicked();
//		}
		void mniDataSourceEdit_Click(object sender, EventArgs e) {
			this.raiseOnDataSourceEditClicked();
		}
		void ctxDataSource_Opening(object sender, CancelEventArgs e) {
			if (this.DataSourceSelected == null) {
				// ALREADY_ASSIGNED_IN_DESIGNER_USEFUL_WHEN_TREE_IS_EMPTY_OR_CLICKING_ON_EMPTY_AREA
				//this.tree.ContextMenuStrip = this.ctxDataSource;
				this.mniltbDataSourceRename.Enabled = false;
				this.mniltbSymbolAdd.Enabled = false;
				this.mniDataSourceEdit.Enabled = false;
				this.mniDataSourceDelete.Enabled = false;
			} else {
				this.mniltbDataSourceRename.Enabled = true;
				this.mniltbSymbolAdd.Enabled = true;
				this.mniDataSourceEdit.Enabled = true;
				this.mniDataSourceDelete.Enabled = true;
				this.mniDataSourceDelete.Text = "Delete [" + this.DataSourceSelected.Name + "]";
			}
		}
		#region inline search, taken from ObjectListViewDemo
		void txtSearch_TextChanged(object sender, EventArgs e) {
			this.btnClear.Enabled = string.IsNullOrEmpty(txtSearch.Text) ? false : true;
			this.TimedFilter(this.OlvTree, txtSearch.Text);
		}
		void TimedFilter(ObjectListView olv, string txt) {
			this.TimedFilter(olv, txt, 0);
		}
		void TimedFilter(ObjectListView olv, string txt, int matchKind) {
			TextMatchFilter filter = null;
			if (!String.IsNullOrEmpty(txt)) {
				switch (matchKind) {
					case 0:
					default:
						filter = TextMatchFilter.Contains(olv, txt);
						break;
					case 1:
						filter = TextMatchFilter.Prefix(olv, txt);
						break;
					case 2:
						filter = TextMatchFilter.Regex(olv, txt);
						break;
				}
			}
			// Setup a default renderer to draw the filter matches
			if (filter == null) {
				olv.DefaultRenderer = null;
			}  else {
				olv.DefaultRenderer = new HighlightTextRenderer(filter);
				// Uncomment this line to see how the GDI+ rendering looks
				//olv.DefaultRenderer = new HighlightTextRenderer { Filter = filter, UseGdiTextRendering = false };
			}

			// Some lists have renderers already installed
			HighlightTextRenderer highlightingRenderer = olv.GetColumn(0).Renderer as HighlightTextRenderer;
			if (highlightingRenderer != null) {
				highlightingRenderer.Filter = filter;
			}
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			olv.AdditionalFilter = filter;
			olv.Invalidate();
			stopWatch.Stop();

			IList objects = olv.Objects as IList;
			if (objects == null) {
				string msg = String.Format("Filtered in {0}ms", stopWatch.ElapsedMilliseconds);
				this.toolTip1.SetToolTip(this.txtSearch, msg);
			} else {
				string msg = String.Format("Filtered {0} items down to {1} items in {2}ms",
										   objects.Count,
										   olv.Items.Count,
										   stopWatch.ElapsedMilliseconds);
				this.toolTip1.SetToolTip(this.txtSearch, msg);
			}
		}
		void btnClear_Click(object sender, EventArgs e) {
			this.txtSearch.Text = "";
		}
		#endregion
		void tree_Expanded(object sender, TreeBranchExpandedEventArgs e) {
			if (this.ignoreExpandCollapseEventsDuringInitializationOrUninitialized) return;
			DataSource dataSourceExpanded = e.Model as DataSource;
			if (dataSourceExpanded == null) return;
			if (this.dataSnapshot == null) return;
			string dataSourceExpandedName = dataSourceExpanded.Name;
			if (this.dataSnapshot.DataSourceFoldersExpanded.Contains(dataSourceExpandedName)) return;
			this.dataSnapshot.DataSourceFoldersExpanded.Add(dataSourceExpandedName);
			this.dataSnapshotSerializer.Serialize();
		}
		void tree_Collapsed(object sender, TreeBranchCollapsedEventArgs e) {
			if (this.ignoreExpandCollapseEventsDuringInitializationOrUninitialized) return;
			DataSource dataSourceCollapsed = e.Model as DataSource;
			if (dataSourceCollapsed == null) return;
			if (this.dataSnapshot == null) return;
			string dataSourceCollapsedName = dataSourceCollapsed.Name;
			if (this.dataSnapshot.DataSourceFoldersExpanded.Contains(dataSourceCollapsedName) == false) return;
			this.dataSnapshot.DataSourceFoldersExpanded.Remove(dataSourceCollapsedName);
			this.dataSnapshotSerializer.Serialize();
		}
		void mniltbSymbolAdd_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			//this.dataSourceRepository.SymbolAdd(this.DataSourceSelected, e.StringUserTyped);
			if (this.DataSourceSelected == null) {
				Assembler.PopupException("mniltbSymbolAdd_UserTyped(): this.DataSourceSelected=null");
				return;
			}
			this.dataSourceRepository.SymbolAdd(this.DataSourceSelected, e.StringUserTyped, this);
			this.SelectSymbol(this.DataSourceSelected.Name, e.StringUserTyped);
			e.RootHandlerShouldCloseParentContextMenuStrip = true;
		}
		void mniltbDataSourceRename_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			if (this.DataSourceSelected == null) {
				Assembler.PopupException("mniltbDataSourceRename_UserTyped(): this.DataSourceSelected=null");
				return;
			}
			this.dataSourceRepository.ItemRename(this.DataSourceSelected, e.StringUserTyped, this);
			e.RootHandlerShouldCloseParentContextMenuStrip = true;
		}
		void mniltbSymbolRenameTo_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string sourceSymbol = this.SymbolSelected;
			string targetSymbol = e.StringUserTyped;

			string msig = " //mnitlbSymbolRenameTo_UserTyped(" + sourceSymbol + "=>" + targetSymbol + ")";

			if (this.DataSourceSelected == null) {
				Assembler.PopupException("this.DataSourceSelected=null" + msig);
				return;
			}

			try {
				List<DataSource> dataSourcesHavingSymbolToBeRenamed = this.dataSourceRepository.SameSymbolInHowManyDataSources(sourceSymbol);
				int numberOfDataSourcesHavingSymbolToBeRenamed = dataSourcesHavingSymbolToBeRenamed.Count;
				if (numberOfDataSourcesHavingSymbolToBeRenamed < 1) {
					string msg = "I_REFUSE_TO_RENAME SYMBOL_MUST_EXIST_AT_LEAST_IN_1_DATASOURCE [" + this.DataSourceSelected + "] numberOfDataSourcesHavingSymbolToBeRenamed[" + numberOfDataSourcesHavingSymbolToBeRenamed + "] < 1";
					Assembler.PopupException(msg + msig);
					return;
				}
				bool renameSymbolInfoKozNoOtherDataSourceHasSameSymbol = numberOfDataSourcesHavingSymbolToBeRenamed == 1;

				SymbolInfo sourceSymbolInfo = Assembler.InstanceInitialized.RepositorySymbolInfos.FindSymbolInfo_nullUnsafe(sourceSymbol);
				SymbolInfo targetSymbolInfo = Assembler.InstanceInitialized.RepositorySymbolInfos.FindSymbolInfo_nullUnsafe(targetSymbol);

				if (targetSymbolInfo == null) {
					if (renameSymbolInfoKozNoOtherDataSourceHasSameSymbol) {
						Assembler.InstanceInitialized.RepositorySymbolInfos.Rename(sourceSymbolInfo, targetSymbol);
					} else {
						Assembler.InstanceInitialized.RepositorySymbolInfos.Duplicate(sourceSymbolInfo, targetSymbol);	// targetSymbolInfoAlreadyExists
					}
				}

				// repository has no idea who loaded the bars that are being renamed now {BTABRN}
				// but DataSource.SymbolRenamedExecutorShouldRenameEachBarAndSave() will notify Executors using BTABRN and their Chart will repaint after mouseover  
				this.dataSourceRepository.SymbolRename(this.DataSourceSelected, this.SymbolSelected, e.StringUserTyped, this);
			} catch (Exception ex) {
				Assembler.PopupException("REPOSITORIES_FILE_ACCESS__OR_OnSymbolRenamed_EVENT_CONSUMERS" + msig, ex);
			}

			// I right-clicked on Symbol to rename it; it is still Selected and I typed new Symbol name in a popup-menu
			if (this.OlvTree.SelectedObject != null) {
				this.OlvTree.Expand(this.OlvTree.SelectedObject);
			}
			//REPOSITORY_WILL_NOTIFY_ME_USING_EVENT this.SelectSymbol(this.DataSourceSelected.Name, e.StringUserTyped);
			e.RootHandlerShouldCloseParentContextMenuStrip = true;
		}

		void mniltbDataSourceAddNew_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string newDataSourceName = e.StringUserTyped;
			DataSource foundWithSameName = this.dataSourceRepository.ItemFind(newDataSourceName);
			if (foundWithSameName != null) {
				Assembler.InstanceInitialized.StatusReporter.DisplayStatus("DataSource[" + newDataSourceName + "] already exists");
				this.OlvTree.EnsureModelVisible(foundWithSameName);
				this.OlvTree.SelectObject(foundWithSameName);
				e.HighlightTextWithRed = true;
				//e.RootHandlerShouldCloseParentContextMenuStrip = true;
				return;
			}
			// literally: create, add, make it visible, emulate a click on the newborn, popup editor 
			var dataSourceNewborn = new DataSource(newDataSourceName);
			dataSourceNewborn.Initialize(this.dataSourceRepository.AbsPath, Assembler.InstanceInitialized.OrderProcessor);
			this.dataSourceRepository.ItemAdd(dataSourceNewborn, this, true);
			// all the rest was already done in dataSourceRepository.ItemAdd() => dataSourceRepository_OnDataSourceAdded(),
//			this.populateDataSourcesIntoTreeListView();
//			this.tree.EnsureModelVisible(foundWithSameName);
//			this.tree.SelectObject(foundWithSameName);
//			this.SelectSymbol(ds.Name);
			// but now user has selected the static provider and I want the provider's icon in the tree
			this.PopulateIconForDataSource(dataSourceNewborn);
			this.raiseOnDataSourceEditClicked();	//ds
		}

		void dataSourceRepository_OnSymbolAdded(object sender, DataSourceSymbolEventArgs e) {
			if (sender == this) {
				this.OlvTree.RefreshObject(this.DataSourceSelected);
				this.OlvTree.Expand(e.DataSource);
			} else {
				//this.populateDataSourcesIntoTreeListView();
				this.OlvTree.RebuildAll(true);
				this.OlvTree.Expand(e.DataSource);
				this.SelectSymbol(e.DataSource.Name, e.Symbol);
			}
		}
		void dataSourceRepository_OnSymbolRenamed(object sender, DataSourceSymbolRenamedEventArgs e) {
			this.OlvTree.RefreshObject(this.DataSourceSelected);
			if (sender != this) {
				//RefreshObject()WORKS_FINE this.populateDataSourcesIntoTreeListView();
				this.OlvTree.Expand(e.DataSource);
				this.SelectSymbol(e.DataSource.Name, e.Symbol);
			}
		}
		void dataSourceRepository_OnSymbolCanBeRemoved(object sender, DataSourceSymbolEventArgs e) {
			//e.DoNotDeleteItsUsedElsewhere = !e.DataSource.StaticProvider.CanModifySymbols;
			e.DoNotDeleteItsUsedElsewhere = false;
		}
		void dataSourceRepository_OnSymbolRemovedDone(object sender, DataSourceSymbolEventArgs e) {
			if (sender == this) {
				this.OlvTree.RebuildAll(true);
				this.OlvTree.Expand(this.DataSourceSelected);
				this.SelectSymbol(this.DataSourceSelected.Name, null);
			} else {
				this.OlvTree.RebuildAll(true);	//roots not changed, no need to call this.populateDataSourcesIntoTreeListView();
				this.OlvTree.Expand(e.DataSource);
				this.SelectSymbol(e.DataSource.Name, null);
			}
		}
		
		void dataSourceRepository_OnDataSourceAdded(object sender, NamedObjectJsonEventArgs<DataSource> e) {
			this.PopulateDataSourcesIntoTreeListView();	// roots changed => this.tree.RebuildAll(true) isn't enough
			this.SelectSymbol(e.Item.Name, null);
		}
		void dataSourceRepository_OnDataSourceRenamed(object sender, NamedObjectJsonEventArgs<DataSource> e) {
			if (sender == this) {
				this.OlvTree.RefreshObject(this.DataSourceSelected);
			} else {
				this.OlvTree.RefreshObject(e.Item);
				this.SelectSymbol(e.Item.Name, null);
			}
		}
		void dataSourceRepository_OnDataSourceRemovedDone(object sender, NamedObjectJsonEventArgs<DataSource> e) {
			this.PopulateDataSourcesIntoTreeListView();
		}
		void dataSourceRepository_OnDataSourceCanBeRemoved(object sender, NamedObjectJsonEventArgs<DataSource> e) {
			e.DoNotDeleteItsUsedElsewhere = false;
		}
		void mniRefresh_Click(object sender, EventArgs e) {
			Cursor.Current = Cursors.WaitCursor;
			this.PopulateDataSourcesIntoTreeListView();
			Cursor.Current = Cursors.Arrow;
		}
		void mniShowHeader_Click(object sender, EventArgs e) {
			//v1 ColumnHeaderStyle newStyle = this.tree.HeaderStyle == ColumnHeaderStyle.None ? ColumnHeaderStyle.Clickable : ColumnHeaderStyle.None;
			try {
				this.dataSnapshot.ShowHeader = this.mniShowHeader.Checked;
				this.dataSnapshotSerializer.Serialize();
				this.OlvTree.HeaderStyle = this.dataSnapshot.ShowHeader ? ColumnHeaderStyle.Clickable : ColumnHeaderStyle.None;
			} catch (Exception ex) {
				Assembler.PopupException("mniShowHeader_Click", ex);
			}
		}
		void mniShowSearchBar_Click(object sender, EventArgs e){
			try {
				this.dataSnapshot.ShowSearchBar = this.mniShowSearchBar.Checked;
				this.dataSnapshotSerializer.Serialize();
				this.pnlSearch.Visible = this.dataSnapshot.ShowSearchBar;
				this.txtSearch.Focus();
			} catch (Exception ex) {
				Assembler.PopupException("mniShowHeader_Click", ex);
			}
		}
		void mniAppendMarketNameToDataSourceToolStripMenuItem_Click(object sender, EventArgs e) {
			try {
				this.AppendMarketToDataSourceName				= this.mniAppendMarketNameToDataSourceToolStripMenuItem.Checked;
				this.dataSnapshot.AppendMarketToDataSourceName	= this.mniAppendMarketNameToDataSourceToolStripMenuItem.Checked;
				this.dataSnapshotSerializer.Serialize();
				//this.tree.RebuildAll(true);		// otherwize mouseover will trigger repaint
				this.OlvTree.Invalidate();		// otherwize mouseover will trigger repaint
			} catch (Exception ex) {
				Assembler.PopupException("mniAppendMarketNameToDataSourceToolStripMenuItem_Click", ex);
			}
		}
		void mniSymbolInfoEditor_Click(object sender, EventArgs e) {
			this.RaiseOnSymbolInfoEditorClicked();
		}
		void mniSymbolCopyToAnotherDataSource_DropDownOpening(object sender, EventArgs e) {
			ToolStripMenuItem mniDataSource = sender as ToolStripMenuItem;
			if (mniDataSource == null) {
				string msg = "MUST_BE_OF_TYPE_ToolStripMenuItem mniDataSource[" + sender + "]";
				Assembler.PopupException(msg);
				return;
			}
			//DataSource ds = mniDataSource.Tag as DataSource;
			DataSource dsSource = this.DataSourceSelected;
			if (dsSource == null) {
				string msg = "MUST_BE_OF_TYPE_DataSource mniDataSource.Tag[" + mniDataSource.Tag + "]";
				Assembler.PopupException(msg);
				return;
			}
			string dsSourceName = dsSource.Name;
			if (this.mniSymbolCopyToAnotherDataSource.DropDownItems.Count > 0) this.mniSymbolCopyToAnotherDataSource.DropDownItems.Clear(); //CAUSING_SUBMENU_TRIANGLE_TO_APPEAR__WILL_BE_CLEARED_ON_OPENING
			foreach (ToolStripMenuItem mni in this.DataSourcesAsMniList) {
				this.mniSymbolCopyToAnotherDataSource.DropDownItems.Add(mni);
				DataSource dsTarget = mni.Tag as DataSource;
				if (dsTarget == null) {
					string msg = "MUST_BE_OF_TYPE_DataSource mni[" + mni.Text+ "].Tag[" + mni.Tag + "]";
					Assembler.PopupException(msg);
					continue;
				}
				string dsTargetName = dsTarget.Name;
				if (dsSourceName == mni.Text) {
					mni.Enabled = false;
					mni.Text += " [Im copying from]";
					continue;
				}
				if (dsTarget.Symbols.Contains(this.SymbolSelected)) {
					mni.Enabled = false;
					mni.Text += " [already has " + this.SymbolSelected + "]";
					continue;
				}
				if (dsSource.ScaleInterval.CanConvertTo(dsTarget.ScaleInterval) == false) {
					mni.Enabled = false;
					mni.Text += " [CANT_INCREASE_GRANULARITY " + dsSource.Name + "[" + dsSource.ScaleInterval + "]=>" + dsTarget.Name + "[" + dsTarget.ScaleInterval + "]";
					continue;
				}
				
				// REPLACED_BY_mniSymbolCopy_DropDownItemClicked mni.Click += new EventHandler(mniSymbolCopy_Click);
			}
		}
		void mniSymbolCopyToAnotherDataSource_DropDownClosed(object sender, EventArgs e) {
			// REPLACED_BY_mniSymbolCopy_DropDownItemClicked 
			//foreach (ToolStripMenuItem mni in this.mniSymbolCopyToAnotherDataSource.DropDownItems) {
			//	mni.Click -= mniSymbolCopy_DropDownItemClicked;
			//}
			this.mniSymbolCopyToAnotherDataSource.DropDownItems.Clear();
			this.mniSymbolCopyToAnotherDataSource.DropDownItems.Add("CAUSING_SUBMENU_TRIANGLE_TO_APPEAR__WILL_BE_CLEARED_ON_OPENING");
		}
		// REPLACED_BY_mniSymbolCopy_DropDownItemClicked
		//void mniSymbolCopy_Click(object sender, EventArgs e) {
		//	ToolStripMenuItem mniDataSourceDestination = sender as ToolStripMenuItem;
		void mniSymbolCopyToAnotherDataSource_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			ToolStripMenuItem mniDsDestination = e.ClickedItem as ToolStripMenuItem;
			if (mniDsDestination == null) {
				string msg = "MUST_BE_OF_TYPE_ToolStripMenuItem mniDataSourceDestination[" + mniDsDestination.GetType() + "]";
				Assembler.PopupException(msg);
				return;
			}
			DataSource dsDestination = mniDsDestination.Tag as DataSource;
			if (dsDestination == null) {
				string msg = "MUST_BE_OF_TYPE_ToolStripMenuItem mniDataSource[" + dsDestination + "]";
				Assembler.PopupException(msg);
				return;
			}
			Assembler.InstanceInitialized.RepositoryJsonDataSources.SymbolCopyOrCompressFrom(this.DataSourceSelected, this.SymbolSelected, dsDestination, this);
			this.PopulateDataSourcesIntoTreeListView();
		}
	}
}