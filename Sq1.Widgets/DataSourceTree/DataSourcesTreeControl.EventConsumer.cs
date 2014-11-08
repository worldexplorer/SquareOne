using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Widgets.DataSourcesTree {
	public partial class DataSourcesTreeControl {
		void tree_CellClick(object sender, CellClickEventArgs e) {
			if (e.RowIndex == -1) return;
			this.syncSymbolAndDataSourceSelectedFromRowIndexClicked(e.RowIndex);
			if (this.SymbolSelected != null) {
				this.RaiseOnSymbolSelected();
			} else {
				this.RaiseOnDataSourceSelected();
			}
		}
		void tree_CellRightClick(object sender, CellRightClickEventArgs e) {
			if (e.RowIndex == -1) { // empty tree, no datasources added yet
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
				this.mniNewChartSymbol.Text = newExisting + " Chart for " + this.SymbolSelected;
				this.mniBarsAnalyzerSymbol.Text = "Bars Analyzer for " + this.SymbolSelected;
				this.mniOpenStrategySymbol.Text = "New Strategy for " + this.SymbolSelected;
				this.mniRemoveSymbol.Text = "Remove [" + this.SymbolSelected + "] from [" + this.DataSourceSelected.Name + "]";
				DataSourceSymbolEventArgs subscribersPolled =
					this.dataSourceRepository.SymbolCanBeDeleted(this.DataSourceSelected, this.SymbolSelected, this);
				this.mniRemoveSymbol.Enabled = (subscribersPolled.DoNotDeleteItsUsedElsewhere == false);
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
				this.mniDataSourceEdit.Enabled = (subscribersPolled.DoNotDeleteItsUsedElsewhere == false);
				this.mniDataSourceDelete.Enabled = (subscribersPolled.DoNotDeleteItsUsedElsewhere == false);
				this.mniDataSourceEdit.Text = "Edit DataSource [" + this.DataSourceSelected.Name + "]";
				this.mniDataSourceDelete.Text = "Delete DataSource [" + this.DataSourceSelected.Name + "]";
				this.ContextMenuStrip = this.ctxDataSource;
			}
		}
		void tree_MouseDoubleClick(object sender, MouseEventArgs e) {
			//if (this.SymbolSelected == null) return;
			//this.mniNewChartSymbol_Click(this, null);
		}
		void mniNewChartSymbol_Click(object sender, EventArgs e) {
			this.RaiseOnNewChartForSymbolClicked();
		}
		void mniBarsAnalyzerSymbol_Click(object sender, EventArgs e) {
			this.RaiseOnBarsAnalyzerClicked();
		}
		void mniOpenStrategySymbol_Click(object sender, EventArgs e) {
			this.RaiseOnOpenStrategyForSymbolClicked();
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
			this.populateDataSourcesIntoTreeListView();
		}
//		void mniDataSourceCreate_Click(object sender, EventArgs e) {
//			this.RaiseOnDataSourceCreateClicked();
//		}
		void mniDataSourceEdit_Click(object sender, EventArgs e) {
			this.RaiseOnDataSourceEditClicked();
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
		void txtFilterSymbol_TextChanged(object sender, EventArgs e) {
			this.btnClear.Enabled = string.IsNullOrEmpty(textBoxFilterTree.Text) ? false : true;
			this.TimedFilter(this.tree, textBoxFilterTree.Text);
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
				this.toolTip1.SetToolTip(this.textBoxFilterTree, msg);
			} else {
				string msg = String.Format("Filtered {0} items down to {1} items in {2}ms",
										   objects.Count,
										   olv.Items.Count,
										   stopWatch.ElapsedMilliseconds);
				this.toolTip1.SetToolTip(this.textBoxFilterTree, msg);
			}
		}
		void btnClear_Click(object sender, EventArgs e) {
			this.textBoxFilterTree.Text = "";
		}

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
			if (this.DataSourceSelected == null) {
				Assembler.PopupException("mnitlbSymbolRenameTo_UserTyped(): this.DataSourceSelected=null");
				return;
			}
			// repository has no idea who loaded the bars that are being renamed now {BTABRN}
			// but DataSource.SymbolRenamedExecutorShouldRenameEachBarAndSave() will notify Executors using BTABRN and their Chart will repaint after mouseover  
			this.dataSourceRepository.SymbolRename(this.DataSourceSelected, this.SymbolSelected, e.StringUserTyped, this);
			//REPOSITORY_WILL_NOTIFY_ME_USING_EVENT this.SelectSymbol(this.DataSourceSelected.Name, e.StringUserTyped);
			e.RootHandlerShouldCloseParentContextMenuStrip = true;
		}

		void mniltbDataSourceAddNew_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string newDataSourceName = e.StringUserTyped;
			DataSource foundWithSameName = this.dataSourceRepository.ItemFind(newDataSourceName);
			if (foundWithSameName != null) {
				Assembler.InstanceInitialized.StatusReporter.DisplayStatus("DataSource[" + newDataSourceName + "] already exists");
				this.tree.EnsureModelVisible(foundWithSameName);
				this.tree.SelectObject(foundWithSameName);
				e.HighlightTextWithRed = true;
				//e.RootHandlerShouldCloseParentContextMenuStrip = true;
				return;
			}
			// literally: create, add, make it visible, emulate a click on the newborn, popup editor 
			var dataSourceNewborn = new DataSource(newDataSourceName);
			this.dataSourceRepository.ItemAdd(dataSourceNewborn, this);
			// all the rest was already done in dataSourceRepository.ItemAdd() => dataSourceRepository_OnDataSourceAdded(),
//			this.populateDataSourcesIntoTreeListView();
//			this.tree.EnsureModelVisible(foundWithSameName);
//			this.tree.SelectObject(foundWithSameName);
//			this.SelectSymbol(ds.Name);
			// but now user has selected the static provider and I want the provider's icon in the tree
			this.populateIconForDataSource(dataSourceNewborn);
			this.RaiseOnDataSourceEditClicked();	//ds
		}

		void dataSourceRepository_OnSymbolAdded(object sender, DataSourceSymbolEventArgs e) {
			if (sender == this) {
				this.tree.RefreshObject(this.DataSourceSelected);
				this.tree.Expand(e.DataSource);
			} else {
				//this.populateDataSourcesIntoTreeListView();
				this.tree.RebuildAll(true);
				this.tree.Expand(e.DataSource);
				this.SelectSymbol(e.DataSource.Name, e.Symbol);
			}
		}
		void dataSourceRepository_OnSymbolRenamed(object sender, DataSourceSymbolEventArgs e) {
			this.tree.RefreshObject(this.DataSourceSelected);
			if (sender != this) {
				//RefreshObject()WORKS_FINE this.populateDataSourcesIntoTreeListView();
				this.tree.Expand(e.DataSource);
				this.SelectSymbol(e.DataSource.Name, e.Symbol);
			}
		}
		void dataSourceRepository_OnSymbolCanBeRemoved(object sender, DataSourceSymbolEventArgs e) {
			//e.DoNotDeleteItsUsedElsewhere = !e.DataSource.StaticProvider.CanModifySymbols;
			e.DoNotDeleteItsUsedElsewhere = false;
		}
		void dataSourceRepository_OnSymbolRemovedDone(object sender, DataSourceSymbolEventArgs e) {
			if (sender == this) {
				this.tree.RebuildAll(true);
				this.tree.Expand(this.DataSourceSelected);
				this.SelectSymbol(this.DataSourceSelected.Name, null);
			} else {
				this.tree.RebuildAll(true);	//roots not changed, no need to call this.populateDataSourcesIntoTreeListView();
				this.tree.Expand(e.DataSource);
				this.SelectSymbol(e.DataSource.Name, null);
			}
		}
		
		void dataSourceRepository_OnDataSourceAdded(object sender, NamedObjectJsonEventArgs<DataSource> e) {
			this.populateDataSourcesIntoTreeListView();	// roots changed => this.tree.RebuildAll(true) isn't enough
			this.SelectSymbol(e.Item.Name, null);
		}
		void dataSourceRepository_OnDataSourceRenamed(object sender, NamedObjectJsonEventArgs<DataSource> e) {
			if (sender == this) {
				this.tree.RefreshObject(this.DataSourceSelected);
			} else {
				this.tree.RefreshObject(e.Item);
				this.SelectSymbol(e.Item.Name, null);
			}
		}
		void dataSourceRepository_OnDataSourceRemovedDone(object sender, NamedObjectJsonEventArgs<DataSource> e) {
			this.populateDataSourcesIntoTreeListView();
		}
		void dataSourceRepository_OnDataSourceCanBeRemoved(object sender, NamedObjectJsonEventArgs<DataSource> e) {
			e.DoNotDeleteItsUsedElsewhere = false;
		}
		void mniRefresh_Click(object sender, EventArgs e) {
			Cursor.Current = Cursors.WaitCursor;
			this.populateDataSourcesIntoTreeListView();
			Cursor.Current = Cursors.Arrow;
		}
	}
}