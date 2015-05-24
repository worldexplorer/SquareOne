using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Repositories;
using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Gui {
	public class MainFormWorkspacesManager {
		const string prefix = "workspace_";
		MainForm mainForm;
		RepositoryFoldersNoJson repository;

		public string WorkspaceCurrentName { get { return this.WorkspaceCurrentMni.Text; } }
		public ToolStripMenuItem WorkspaceCurrentMni { get; protected set; }

				List<ToolStripMenuItem>	workspaceMenuItems;
		public	ToolStripMenuItem[]		workspaceMenuItemsWithHandlers { get { return this.workspaceMenuItems.ToArray(); } }

		public MainFormWorkspacesManager(MainForm mainForm, RepositoryFoldersNoJson workspacesRepository) {
			this.mainForm = mainForm;
			this.repository = workspacesRepository;

			this.workspaceMenuItems = new List<ToolStripMenuItem>();

			// DOCK_CONTENT_SHOWS_ONLY_CHART_FORM_NO_SPLITTERS this.RescanRebuildWorkspacesMenu();

			this.mainForm.MniWorkspaceDeleteCurrent.Click		+= new EventHandler(this.mniltbDeleteCurrent_Click);
			this.mainForm.MniltbWorklspaceCloneTo.UserTyped		+= new EventHandler<LabeledTextBoxUserTypedArgs>(this.mniltbCloneTo_UserTyped);
			this.mainForm.MniltbWorklspaceRenameTo.UserTyped	+= new EventHandler<LabeledTextBoxUserTypedArgs>(this.mniltbRenameTo_UserTyped);
			this.mainForm.MniltbWorklspaceNewBlank.UserTyped	+= new EventHandler<LabeledTextBoxUserTypedArgs>(this.mniltbNewBlank_UserTyped);
		}
		
		void rescanFolders_BuildWorkspacesMenuItemsWithHandlersFromRepository() {
			foreach (ToolStripMenuItem mni in this.workspaceMenuItems) {
				mni.Click -= new EventHandler(mniWorkspaceLoad_Click);
			}
			this.workspaceMenuItems.Clear();
			this.repository.RescanFolders();
			foreach (string workspace in this.repository.FoldersWithin) {
				var mni = new ToolStripMenuItem();
				mni.Text = workspace;
				mni.Name = MainFormWorkspacesManager.prefix + workspace; 
				mni.Click += new EventHandler(mniWorkspaceLoad_Click);
				mni.CheckOnClick = true;
				this.workspaceMenuItems.Add(mni);
			}
		}
		
		void mniWorkspaceLoad_Click(object sender, EventArgs e) {
			var mniWorkspace = sender as ToolStripMenuItem;
			if (mniWorkspace == null) {
				Assembler.PopupException("mniWorkspace_Click(): (sender as ToolStripMenuItem) = null");
				return;
			}
			this.WorkspaceCurrentMni.Checked = false;
			this.WorkspaceCurrentMni = mniWorkspace;
			this.mainForm.WorkspaceLoad(this.WorkspaceCurrentName);
		}

		void mniltbDeleteCurrent_Click(object sender, EventArgs e) {
			var next = this.findNextOrPrevIfLastAfterDeletion();
			//Assembler.InstanceInitialized.WorkspacesRepository.ItemDelete(this.WorkspaceCurrent);
			if (next == null) return;
			next.Checked = true;
			this.mainForm.WorkspaceLoad(this.WorkspaceCurrentName);
		}

		void mniltbCloneTo_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string workspaceCloneToName = e.StringUserTyped;
//			ScriptContext scriptContextToRename = this.ScriptContextFromMniTag(sender);
//			this.Strategy.ScriptContextRename(scriptContextToRename, workspaceCloneToName);
//			this.RaiseOnScriptContextRenamed(workspaceCloneToName);
//			this.ctxParameterBags_Opening(this, null);
			Assembler.PopupException("NotImplementedException WorkspaceCloneTo_UserTyped(" + e.StringUserTyped + ")");
		}
		void mniltbNewBlank_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string workspaceNewBlankName = e.StringUserTyped;
			Assembler.PopupException("NotImplementedException WorkspaceNewBlank_UserTyped(" + e.StringUserTyped + ")");
		}
		void mniltbRenameTo_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string workspaceRenameToName = e.StringUserTyped;
			Assembler.PopupException("NotImplementedException WorkspaceRenameTo_UserTyped(" + e.StringUserTyped + ")");
			//this.repository.Ent
		}

		public void SelectWorkspaceLoaded(string workspaceLoaded) {
			var mni = this.findMniByWorkspaceName(workspaceLoaded);
			if (mni == null) {
				Assembler.PopupException("SelectWorkspaceLoaded(" + workspaceLoaded + ") not found");
				return;
			}
			this.WorkspaceCurrentMni = mni;
			foreach (var every in this.workspaceMenuItems) every.Checked = false;
			this.WorkspaceCurrentMni.Checked = true;
		}
		
		ToolStripMenuItem findMniByWorkspaceName(string workspaceToFind) {
			ToolStripMenuItem ret = null;
			foreach (var mni in this.workspaceMenuItems) {
				if (mni.Text != workspaceToFind) continue;
				ret = mni;
				break;
			}
			return ret;
		}
		
		ToolStripMenuItem findNextOrPrevIfLastAfterDeletion() {
			ToolStripMenuItem ret = null;
			if (this.WorkspaceCurrentMni == null) {
				string msg = "this.WorkspaceCurrentMni == null";
				Assembler.PopupException(msg);
				return ret;
			}
			int index = this.workspaceMenuItems.IndexOf(this.WorkspaceCurrentMni);
			if (index == -1) {
				string msg = "this.workspaceMenuItems.IndexOf(this.WorkspaceCurrentMni) == -1";
				Assembler.PopupException(msg);
				return ret;
			}
			index++;
			if (index >= this.workspaceMenuItems.Count) {
				string msg = "index[" + index + "] >= this.workspaceMenuItems.Count[" + this.workspaceMenuItems.Count + "]";
				Assembler.PopupException(msg);
				return ret;
			}
			return this.workspaceMenuItems[index];
		}


		public void RescanRebuildWorkspacesMenu(string currentWorkspaceName = null) {
			if (string.IsNullOrEmpty(currentWorkspaceName)) {
				currentWorkspaceName = Assembler.InstanceInitialized.AssemblerDataSnapshot.CurrentWorkspaceName;
			}

			this.rescanFolders_BuildWorkspacesMenuItemsWithHandlersFromRepository();

			//this.mainForm.CtxWorkspaces.Items.Clear();
			//this.mainForm.CtxWorkspaces.Items.AddRange(new ToolStripItem[] {
			//    this.mainForm.MniWorkspaceDeleteCurrent,
			//    this.mainForm.MniltbWorklspaceNewBlank,
			//    this.mainForm.MniltbWorklspaceCloneTo,
			//    this.mainForm.MniltbWorklspaceRenameTo,
			//    this.mainForm.MniWorkspacesToolStripSeparator});

			//this.SyncMniEnabledAndSuggestNames(currentWorkspaceName);
			this.mainForm.CtxWorkspaces.Items.AddRange(this.workspaceMenuItemsWithHandlers);
		}

		public void SyncMniEnabledAndSuggestNames(string currentWorkspaceName = null) {
			if (string.IsNullOrEmpty(currentWorkspaceName)) {
				currentWorkspaceName = Assembler.InstanceInitialized.AssemblerDataSnapshot.CurrentWorkspaceName;
			}

			if (currentWorkspaceName == "Default") currentWorkspaceName = "DEFAULT_MUST_STAY";
			this.mainForm.MniWorkspaceDeleteCurrent.Text = "Delete [" + currentWorkspaceName + "]";
			this.mainForm.MniltbWorklspaceCloneTo.InputFieldValue = currentWorkspaceName;
			this.mainForm.MniltbWorklspaceRenameTo.InputFieldValue = currentWorkspaceName;

			if (currentWorkspaceName == "DEFAULT_MUST_STAY") {
				this.mainForm.MniWorkspaceDeleteCurrent.Enabled = false;
				this.mainForm.MniltbWorklspaceRenameTo.Enabled = false;
			} else {
				this.mainForm.MniWorkspaceDeleteCurrent.Enabled = true;
				this.mainForm.MniltbWorklspaceRenameTo.Enabled = true;
			}
		}

	}
}
