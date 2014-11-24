using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.StrategyBase;
using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Widgets.StrategiesTree {
	public partial class StrategiesTreeControl {

		void treeListView_MouseDoubleClick(object sender, MouseEventArgs e) {
			if (this.StrategySelected == null) return;
			if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
			this.mniStrategyOpen_Click(sender, e);
		}
		void treeListView_KeyDown(object sender, KeyEventArgs e) {
			if (this.StrategySelected == null) return;
			switch (e.KeyCode) {
					case Keys.Enter:	this.mniStrategyOpen_Click(sender, e); break;
					case Keys.Delete:	this.mniStrategyDelete_Click(sender, e); break;
					case Keys.F2:		this.mniStrategyRename_Click(sender, e); break;
			}
		}
		void treeListView_CellClick(object sender, CellClickEventArgs e) {
			if (e.RowIndex == -1) return;
			this.syncFolderStrategySelectedFromRowIndexClicked(e.RowIndex);
			
			Strategy mayBeStrategy = e.Model as Strategy;
			if (mayBeStrategy != null) {
				this.RaiseOnStrategySelected();
				return;
			}

			string mayBeFolder = e.Model as string;
			if (mayBeFolder != null) {
				bool folderWasSelectedOrExpandedOrCollapsed = true;
				// it's more expensive to provide notification sync from Repository to here, than just get the list 
				List<string> folderPurelyJson = Assembler.InstanceInitialized.RepositoryDllJsonStrategy.FoldersPurelyJson;
				folderWasSelectedOrExpandedOrCollapsed = folderPurelyJson.Contains(mayBeFolder);

				if (folderWasSelectedOrExpandedOrCollapsed) {
					if (mayBeFolder != this.FolderSelected) return;	//	folder was Just ExpandedOrCollapsed => don't invoke FolderSelected  
					this.RaiseOnFolderSelected();
				}
				return;
			}
		}
		void treeListView_CellRightClick(object sender, CellRightClickEventArgs e) {
			if (e.RowIndex == -1) return;
			this.syncFolderStrategySelectedFromRowIndexClicked(e.RowIndex);
			if (this.StrategySelected != null) {
				bool clickable = (this.StrategySelected.ActivatedFromDll == false);
				this.mniStrategyDelete.Enabled = clickable;
				this.mniStrategyEdit.Enabled = clickable;
				//v1 - invisible until deleted
				this.mniStrategyDuplicate.Enabled = clickable;
				this.mniStrategyRename.Enabled = clickable;
				//v2
				this.mniltbStrategyDuplicateTo.Enabled = clickable;
				this.mniltbStrategyRenameTo.Enabled = clickable;
				
				this.ContextMenuStrip = this.ctxStrategy;
				
				List<ToolStripItem> scriptContextsMnis = this.CreateMnisForScriptContexts(this.StrategySelected);
				if (scriptContextsMnis.Count == 0) {
					this.mniStrategyOpenWith.Enabled = false;
				} else {
					this.mniStrategyOpenWith.Enabled = true;
					this.mniStrategyOpenWith.DropDownItems.Clear();
					this.mniStrategyOpenWith.DropDownItems.AddRange(scriptContextsMnis.ToArray());
				}
				
				if (clickable) {
					List<ToolStripItem> strategyFoldersMnis = this.CreateMnisForFolders(this.StrategySelected.StoredInFolderRelName);
					if (strategyFoldersMnis.Count == 0) {
						// can only happen we read Strategies/aaa.json and there is no other folders in Strategies/
						this.mniStrategyMoveToAnotherFolder.Enabled = false;
					} else {
						this.mniStrategyMoveToAnotherFolder.Enabled = true;
						this.mniStrategyMoveToAnotherFolder.DropDownItems.Clear();
						this.mniStrategyMoveToAnotherFolder.DropDownItems.AddRange(strategyFoldersMnis.ToArray());
					}
				} else {
					this.mniStrategyMoveToAnotherFolder.Enabled = false;
				}
			} else {
				bool clickable = this.FolderSelected != null
					&& (this.FolderSelected.ToUpper().EndsWith(".DLL") == false);
				this.mniFolderDelete.Enabled = clickable;
				//v1 - invisible until deleted
				this.mniFolderCreate.Enabled = clickable;
				this.mniFolderRename.Enabled = clickable;
				this.mniFolderCreateStrategy.Enabled = clickable;
				//v2
				this.mniltbFolderRename.Enabled = clickable;
				this.mniltbStrategyCreate.Enabled = clickable;
				this.mniltbFolderCreate.Enabled = clickable;
				
				this.ContextMenuStrip = this.ctxFolder;
			}
		}

		// keyboard arrowUp/Down should notify me for selection changes as well as mouse clicks
		void treeListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
			try {
				this.syncFolderStrategySelectedFromRowIndexClicked(e.ItemIndex);
			} catch (Exception ex) {
				Assembler.PopupException(null, ex);
			}
		}

		// ctxFolder BEGIN
		void mniFolderCreateStrategy_Click(object sender, EventArgs e) {
			string msig = "StrategiesTree.mniFolderCreateStrategy_Click(): ";
			if (this.FolderSelected == null) {
				string msg = "this.FolderSelected==null; please set mniFolderCreate.Enabled=false when right-clicked not on the folder";
				Assembler.PopupException(msg + msig);
				return;
			}
			Strategy strategyNew = null;
			try {
				string strategyNameGenerated = strategyRepository.GenerateStrategyName();
				strategyNew = new Strategy(strategyNameGenerated);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
				return;
			}

			strategyRepository.StrategyAdd(strategyNew, this.FolderSelected);

			this.tree.RebuildAll(true);
			this.tree.SelectObject(strategyNew, true); // does it work??
			this.StrategySelected = strategyNew;
			//var olvStrategy = this.treeListView.FindItemWithText(strategyNew.Name, true, 0); // finds first occurency, not what I've inserted!
			var olvStrategy = this.tree.ModelToItem(strategyNew);
			this.tree.EditSubItem(olvStrategy as OLVListItem, 0);
			this.tree.Expand(this.FolderSelected);

			this.RaiseOnStrategyCreated(msig);
		}
		void mniltbStrategyCreate_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string msig = "StrategiesTree.mniltbStrategyCreate_UserTyped(): ";
			if (this.FolderSelected == null) {
				string msg = "this.FolderSelected==null; please set mniFolderCreate.Enabled=false when right-clicked not on the folder";
				Assembler.PopupException(msg + msig);
				return;
			}
			Strategy strategyNew = null;
			try {
				string strategyNameGenerated = e.StringUserTyped;
				if (string.IsNullOrEmpty(strategyNameGenerated)) strategyNameGenerated = strategyRepository.GenerateStrategyName();
				strategyNew = new Strategy(strategyNameGenerated);
			} catch (Exception ex) {
				Assembler.PopupException(null, ex);
				return;
			}

			strategyRepository.StrategyAdd(strategyNew, this.FolderSelected);

			this.tree.RebuildAll(true);
			this.tree.SelectObject(strategyNew, true); // does it work??
			this.StrategySelected = strategyNew;
			//var olvStrategy = this.treeListView.FindItemWithText(strategyNew.Name, true, 0); // finds first occurency, not what I've inserted!
			//var olvStrategy = this.tree.ModelToItem(strategyNew);
			//this.tree.EditSubItem(olvStrategy as OLVListItem, 0);
			this.tree.Expand(this.FolderSelected);
			e.RootHandlerShouldCloseParentContextMenuStrip = true;

			this.RaiseOnStrategyCreated(msig);
		}
		void mniFolderRename_Click(object sender, EventArgs e) {
			//v1 var olv = this.treeListView.FindItemWithText(this.FolderSelected, false, 0);
			var olv = this.tree.ModelToItem(this.FolderSelected);
			this.tree.EditSubItem(olv as OLVListItem, 0);
		}
		void mniltbFolderRename_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string stringUserTyped = e.StringUserTyped;
			if (this.FolderSelected == stringUserTyped) return;
			try {
				stringUserTyped = this.strategyRepository.FolderRenameModifyNameTillNoException(this.FolderSelected, stringUserTyped);
				this.strategyRepository.FolderRename(this.FolderSelected, stringUserTyped);
				this.FolderSelected = stringUserTyped;
				this.tree.SetObjects(this.strategyRepository.AllFoldersAvailable);
				this.tree.RebuildAll(true);
				e.RootHandlerShouldCloseParentContextMenuStrip = true;
			} catch (Exception ex1) {
				e.HighlightTextWithRed = true;
				Assembler.PopupException(null, ex1);
			}
		}
		void treeListView_CellEditValidating(object sender, CellEditEventArgs e) {
			if (e.NewValue == null) {
				e.Cancel = true;
				this.tree.CancelCellEdit();
				string msg = "treeListView_CellEditValidating(): How come e.NewValue==null?";
				//Assembler.PopupException(msg);
				Assembler.DisplayStatus(msg);
				return;
			}
			var strategyEditedName = e.RowObject as Strategy;
			if (strategyEditedName != null) {
				// used just edited strategy name
				if (strategyEditedName.Name == e.NewValue.ToString()) return;
				if (strategyEditedName != this.StrategySelected) {
					e.Cancel = true;
					string msg = "how come you edited a non-selected strategy?...";
					Assembler.PopupException(msg);
					this.tree.CancelCellEdit();
				}
				string beforeSuggested = "";
				try {
					beforeSuggested = e.NewValue.ToString();
					e.NewValue = this.strategyRepository.StrategyRenameModifyNameTillNoException(strategyEditedName, e.NewValue.ToString());
					this.strategyRepository.StrategyRename(strategyEditedName, e.NewValue.ToString());
					this.RaiseOnStrategyRenamed();
					//this.treeListView.RebuildAll(true);
					//this.treeListView.SelectObject(strategyEditedName, true); // doesnt work, same item highlighted, but keyboard position jumps to index=0
				} catch (Exception ex1) {
					e.Cancel = true;
					Assembler.PopupException(null, ex1);
				}
				return;
			} else {
				// used just edited folder name
				if (this.FolderSelected == e.NewValue.ToString()) return;
				try {
					e.NewValue = this.strategyRepository.FolderRenameModifyNameTillNoException(this.FolderSelected, e.NewValue.ToString());
					this.strategyRepository.FolderRename(this.FolderSelected, e.NewValue.ToString());
					this.FolderSelected = e.NewValue.ToString();
					this.tree.SetObjects(this.strategyRepository.AllFoldersAvailable);
					//this.tree.RebuildAll(true);
				} catch (Exception ex1) {
					e.Cancel = true;
					Assembler.PopupException(null, ex1);
				}
			}
		}
		void mniFolderCreate_Click(object sender, EventArgs e) {
			string msig = "StrategiesTree.mniFolderCreate_Click(): ";
			string folderNew = "";
			try {
				folderNew = strategyRepository.GenerateFolderName();
				strategyRepository.FolderAdd(folderNew, false);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
				return;
			}

			this.FolderSelected = folderNew;
			this.tree.SetObjects(this.strategyRepository.AllFoldersAvailable);
			this.tree.SelectObject(folderNew, true); // does it work??
			//var olvStrategy = this.treeListView.FindItemWithText(strategyNew.Name, true, 0); // finds first occurency, not what I've inserted!
			//var olvStrategy = this.tree.ModelToItem(folderNew);
			//this.tree.EditSubItem(olvStrategy as OLVListItem, 0);
			this.RaiseOnFolderCreated(msig);
		}

		void mniltbFolderCreate_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string msig = "StrategiesTree.mniltbFolderCreate_UserTyped(): ";
			string folderNew = e.StringUserTyped;
			try {
				if (string.IsNullOrEmpty(folderNew)) folderNew = strategyRepository.GenerateFolderName();
				strategyRepository.FolderAdd(folderNew, false);
				e.RootHandlerShouldCloseParentContextMenuStrip = true;
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
				return;
			}

			this.FolderSelected = folderNew;
			this.tree.SetObjects(this.strategyRepository.AllFoldersAvailable);
			this.tree.SelectObject(folderNew, true); // does it work??
			//var olvStrategy = this.treeListView.FindItemWithText(strategyNew.Name, true, 0); // finds first occurency, not what I've inserted!
			//var olvStrategy = this.tree.ModelToItem(folderNew);
			//this.tree.EditSubItem(olvStrategy as OLVListItem, 0);

			this.RaiseOnFolderCreated(msig);
		}
		void mniFolderDelete_Click(object sender, EventArgs e) {
			try {
				if (this.FolderSelected == null) {
					string msg = "StrategiesTree.ctxStrategy_Opening(): this.FolderSelected = null";
					Assembler.PopupException(msg);
				} else {
					this.strategyRepository.FolderDelete(this.FolderSelected);
				}
			} catch (Exception ex) {
				Assembler.PopupException(ex.Message);
			}
			this.tree.SetObjects(this.strategyRepository.AllFoldersAvailable);
		}

		// ctxFolder END



		
		//ctxStrategy BEGIN
		void mniStrategyOpen_Click(object sender, EventArgs e) {
			string msig = "StrategiesTree.mniStrategyOpen_Click(): ";
			this.RaiseOnStrategyOpenClicked(msig);
		}
		void mniStrategyOpenWithScriptContext_Click(object sender, EventArgs e) {
			string msig = "StrategiesTree.mniScriptContext_Click(): ";
			var mniClicked = sender as ToolStripMenuItem;
			if (mniClicked == null) {
				string msg = "Can not fire OnStrategyOpenSavedClicked() because sender is not ToolStripMenuItem";
				Assembler.PopupException(msg + msig);
				return;
			}
			this.RaiseOnStrategyOpenSavedClicked(msig, mniClicked);
		}
		void mniStrategyMoveToAnotherFolder_Click(object sender, EventArgs e) {
			string msig = "StrategiesTree.mniStrategyMoveToAnotherFolder_Click(): ";
			var mniClicked = sender as ToolStripMenuItem;
			if (mniClicked == null) {
				string msg = "Can not fire mniStrategyMoveToAnotherFolder_Click() because sender is not ToolStripMenuItem";
				Assembler.PopupException(msg + msig);
				return;
			}
			this.strategyRepository.StrategyMoveToFolder(this.StrategySelected, mniClicked.Text);
			this.tree.RebuildAll(true);
			// in a long multipage list, I want to show where the strategy resides NOW after it's been moved 
			this.tree.Expand(this.StrategySelected.StoredInFolderRelName);
			this.tree.SelectObject(this.StrategySelected, true); // does it work??
			//var olvStrategy = this.tree.ModelToItem(strategyNew);
			//this.tree.EditSubItem(olvStrategy as OLVListItem, 0);
			
			this.RaiseOnStrategyMovedToAnotherFolderClicked(msig, mniClicked);
		}
		void mniStrategyEdit_Click(object sender, EventArgs e) {
			string msig = "StrategiesTree.mniScriptContext_Click(): ";
			this.RaiseOnStrategyEditClicked(msig);
		}
		void mniStrategyRename_Click(object sender, EventArgs e) {
			//v1 var olv = this.treeListView.FindItemWithText(this.FolderSelected, false, 0);
			var olv = this.tree.ModelToItem(this.StrategySelected);
			this.tree.EditSubItem(olv as OLVListItem, 0);
		}
		void mniltbStrategyRenameTo_UserTyped(object sender, LabeledTextBox.LabeledTextBoxUserTypedArgs e) {
			string msig = "StrategiesTree.mniltbStrategyRenameTo_UserTyped(): ";
			if (this.FolderSelected == null) {
				string msg = "this.FolderSelected==null; please set mniFolderCreate.Enabled=false when right-clicked not on the folder";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (this.StrategySelected == null) {
				string msg = "this.StrategySelected==null";
				Assembler.PopupException(msg + msig);
				return;
			}

			string strategyNameUserTyped = e.StringUserTyped;
			try {
				string strategyNameSucceeded = strategyRepository.StrategyRenameModifyNameTillNoException(this.StrategySelected, strategyNameUserTyped);
				strategyRepository.StrategyRename(this.StrategySelected, strategyNameSucceeded);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
				return;
			} finally {
				this.tree.SetObjects(this.strategyRepository.AllFoldersAvailable);
				this.tree.RebuildAll(true);
			}

			//this.tree.SelectObject(strategyNew, true); // does it work??
			//var olvStrategy = this.treeListView.FindItemWithText(strategyNew.Name, true, 0); // finds first occurency, not what I've inserted!
			//var olvStrategy = this.tree.ModelToItem(strategyNew);
			//this.tree.EditSubItem(olvStrategy as OLVListItem, 0);
			e.RootHandlerShouldCloseParentContextMenuStrip = true;

			this.RaiseOnStrategyRenamed();
		}

		void mniStrategyDuplicate_Click(object sender, EventArgs e) {
			string msig = "StrategiesTree.mniStrategyDuplicate_Click(): ";
			if (this.FolderSelected == null) {
				string msg = "this.FolderSelected==null; please set mniFolderCreate.Enabled=false when right-clicked not on the folder";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (this.StrategySelected == null) {
				string msg = "this.StrategySelected==null";
				Assembler.PopupException(msg + msig);
				return;
			}

			Strategy strategyNew = null;
			try {
				strategyNew = strategyRepository.StrategyDuplicate(this.StrategySelected);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
				return;
			} finally {
				this.tree.RebuildAll(true);
			}

			this.tree.SelectObject(strategyNew, true); // does it work??
			this.StrategySelected = strategyNew;
			//var olvStrategy = this.treeListView.FindItemWithText(strategyNew.Name, true, 0); // finds first occurency, not what I've inserted!
			var olvStrategy = this.tree.ModelToItem(strategyNew);
			this.tree.EditSubItem(olvStrategy as OLVListItem, 0);

			this.RaiseOnStrategyDuplicated(msig);
		}
		void mniltbStrategyDuplicateTo_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string msig = "StrategiesTree.mniStrategyDuplicate_Click(): ";
			if (this.FolderSelected == null) {
				string msg = "this.FolderSelected==null; please set mniFolderCreate.Enabled=false when right-clicked not on the folder";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (this.StrategySelected == null) {
				string msg = "this.StrategySelected==null";
				Assembler.PopupException(msg + msig);
				return;
			}

			Strategy strategyNew = null;
			string strategyNameUserTyped = e.StringUserTyped;
			try {
				strategyNew = strategyRepository.StrategyDuplicate(this.StrategySelected, strategyNameUserTyped);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
				return;
			} finally {
				this.tree.RebuildAll(true);
			}

			this.tree.SelectObject(strategyNew, true); // does it work??
			this.StrategySelected = strategyNew;
			//var olvStrategy = this.treeListView.FindItemWithText(strategyNew.Name, true, 0); // finds first occurency, not what I've inserted!
			//var olvStrategy = this.tree.ModelToItem(strategyNew);
			//this.tree.EditSubItem(olvStrategy as OLVListItem, 0);
			e.RootHandlerShouldCloseParentContextMenuStrip = true;

			this.RaiseOnStrategyDuplicated(msig);
		}

		void mniStrategyDelete_Click(object sender, EventArgs e) {
			//foreach (var item in this.treeListView.SelectedObjects) {
			//Strategy strategy = item as Strategy;
			//if (strategy == null) continue;
			Strategy strategy = this.StrategySelected;
			if (strategy == null) return;

			try {
				this.strategyRepository.StrategyDelete(strategy);
				this.RaiseOnStrategyDeleteClicked();
			} catch (Exception ex) {
				Assembler.PopupException(null, ex);
				//continue;
			} finally {
				this.tree.RebuildAll(true);
			}
		}
		//ctxStrategy END

		void ctxStrategy_Opening(object sender, CancelEventArgs e) {
			if (this.FolderSelected == null) {
				string msg = "StrategiesTree.ctxStrategy_Opening(): this.FolderSelected = null";
				Assembler.PopupException(msg);
				return;
			}
			//this.mniAddSymbols.Text = "Add Symbols to this Strategy[" + this.StrategySelected.Name + "]...";
		}

		void treeListView_ModelCanDrop(object sender, ModelDropEventArgs e) {
			//e.Effect = DragDropEffects.None;
			//if (e.TargetModel == null) return;
			//if (e.TargetModel is Strategy) {
			//	e.InfoMessage = "Can only drop on directories";
			//} else {
			e.Effect = e.StandardDropActionFromKeys;
			//}
		}
		void treeListView_ModelDropped(object sender, ModelDropEventArgs e) {
			//String msg = String.Format("{2} items were dropped on '{1}' as a {0} operation.",
			//	e.Effect, (string)e.TargetModel, e.SourceModels.Count);
			//MessageBox.Show(msg, "OLV Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
			string folderTo = (e.TargetModel is Strategy)
				? ((Strategy)e.TargetModel).StoredInFolderRelName
				: (string)e.TargetModel;
			foreach (Strategy strategyDropped in e.SourceModels) {
				if (strategyDropped.StoredInFolderRelName == folderTo) continue;	// dont wake up the lion
				try {
					this.strategyRepository.StrategyMoveToFolder(strategyDropped, folderTo);
				} catch (Exception ex) {
					string msg = "Looks like folderTo[" + folderTo + "] has a strategy with the same name[" + strategyDropped.Name + "]";
					Assembler.PopupException(msg, ex);
					continue;
				}
			}
			this.tree.SetObjects(strategyRepository.AllFoldersAvailable);
		}
		#region inline search, taken from ObjectListViewDemo
		void txtFilterSymbol_TextChanged(object sender, EventArgs e) {
			this.btnClear.Enabled = string.IsNullOrEmpty(this.textBoxFilterTree.Text) ? false : true;
			this.TimedFilter(this.tree, this.textBoxFilterTree.Text);
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
		#endregion
		void tree_Expanded(object sender, TreeBranchExpandedEventArgs e) {
			if (this.ignoreExpandCollapseEventsDuringInitializationOrUninitialized) return;
			if (this.dataSnapshot == null) return;
			string folderExpanded = e.Model.ToString();
			if (this.dataSnapshot.StrategyFoldersExpanded.Contains(folderExpanded)) return;
			this.dataSnapshot.StrategyFoldersExpanded.Add(folderExpanded);
			this.dataSnapshotSerializer.Serialize();
		}
		void tree_Collapsed(object sender, TreeBranchCollapsedEventArgs e) {
			if (this.ignoreExpandCollapseEventsDuringInitializationOrUninitialized) return;
			if (this.dataSnapshot == null) return;
			string folderCollapsed = e.Model.ToString();
			if (this.dataSnapshot.StrategyFoldersExpanded.Contains(folderCollapsed) == false) return;
			this.dataSnapshot.StrategyFoldersExpanded.Remove(folderCollapsed);
			this.dataSnapshotSerializer.Serialize();
		}
		void mniStrategyOpenWith_Click(object sender, System.EventArgs e) {
			string mniNameToFindDefaultScriptContent = PREFIX_SCRIPT_CONTEXT_MNI_NAME + ContextScript.DEFAULT_NAME;
			ToolStripItem[] found = this.mniStrategyOpenWith.DropDownItems.Find(mniNameToFindDefaultScriptContent, false);
			if (found.Length <= 0) {
				string msg = "FAILED_TO_FIND_DEFAULT_SCRIPT_CONTEXT_AMONG_SUBMENU_ITEMS_TO_OPEN_REPLACE_IN_CURRENT_CHART"
					+ " mniStrategyOpenWith.DropDownItems.Find(" + mniNameToFindDefaultScriptContent + ").Length[" + found + "] <= 0 //mniStrategyOpenWith_Click()";
				Assembler.PopupException(msg);
				return;
			}
			ToolStripMenuItem mniClicked = found[0] as ToolStripMenuItem;
			string msig = "REPLACE_CURRENT_CHART_WITH_DEFAULT_mniStrategyOpenWith_Click";
			this.RaiseOnStrategyOpenSavedClicked(msig, mniClicked);
		}
		void mniShowHeader_Click(object sender, EventArgs e) {
			//v1 ColumnHeaderStyle newStyle = this.tree.HeaderStyle == ColumnHeaderStyle.None ? ColumnHeaderStyle.Clickable : ColumnHeaderStyle.None;
			try {
				this.dataSnapshot.ShowHeader = this.mniShowHeader.Checked;
				this.dataSnapshotSerializer.Serialize();
				this.tree.HeaderStyle = this.dataSnapshot.ShowHeader ? ColumnHeaderStyle.Clickable : ColumnHeaderStyle.None;
			} catch (Exception ex) {
				Assembler.PopupException("mniShowHeader_Click", ex);
			}
		}
		void mniShowSearchBar_Click (object sender, EventArgs e){
			try {
				this.dataSnapshot.ShowSearchBar = this.mniShowSearchBar.Checked;
				this.dataSnapshotSerializer.Serialize();
				this.tableLayoutPanel1.Visible = this.dataSnapshot.ShowSearchBar;
			} catch (Exception ex) {
				Assembler.PopupException("mniShowHeader_Click", ex);
			}
		}
	}
}