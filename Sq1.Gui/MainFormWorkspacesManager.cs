using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Repositories;

using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Gui {
	public class MainFormWorkspacesManager {
		const string prefix = "workspace_";
		const string NAME_DEFAULT = "Default";

		MainForm					mainForm;
		RepositoryFoldersNoJson		repository;
		List<ToolStripMenuItem>		workspaceMenuItemsWithHandlers;

		public string				WorkspaceCurrentName			{ get { return this.WorkspaceCurrentMni.Text; } }
		public ToolStripMenuItem	WorkspaceCurrentMni				{ get; protected set; }

		public MainFormWorkspacesManager(MainForm mainForm, RepositoryFoldersNoJson workspacesRepository) {
			this.mainForm = mainForm;
			this.repository = workspacesRepository;

			this.workspaceMenuItemsWithHandlers = new List<ToolStripMenuItem>();

			// DOCK_CONTENT_SHOWS_ONLY_CHART_FORM_NO_SPLITTERS this.RescanRebuildWorkspacesMenu();

			this.mainForm.MniltbWorklspaceDuplicateTo.UserTyped	+= new EventHandler<LabeledTextBoxUserTypedArgs>(this.mniltbWorkspaceDuplicateTo_UserTyped);
			this.mainForm.MniltbWorklspaceRenameTo.UserTyped	+= new EventHandler<LabeledTextBoxUserTypedArgs>(this.mniltbWorkspaceRenameTo_UserTyped);
			this.mainForm.MniltbWorklspaceNewBlank.UserTyped	+= new EventHandler<LabeledTextBoxUserTypedArgs>(this.mniltbWorkspaceNewBlank_UserTyped);
			this.mainForm.MniWorkspaceDelete.Click				+= new EventHandler(this.mniWorkspaceDelete_Click);
		}
		
		void rescanFolders_BuildWorkspacesMenuItemsWithHandlersFromRepository() {
			foreach (ToolStripMenuItem mni in this.workspaceMenuItemsWithHandlers) {
				mni.Click -= new EventHandler(this.mniWorkspaceLoad_Click);
			}
			this.workspaceMenuItemsWithHandlers.Clear();
			this.repository.RescanFoldersAndSort();
			foreach (string workspace in this.repository.FoldersWithin) {
				var mni = new ToolStripMenuItem();
				mni.Text = workspace;
				mni.Name = MainFormWorkspacesManager.prefix + workspace; 
				mni.Click += new EventHandler(this.mniWorkspaceLoad_Click);
				mni.CheckOnClick = true;
				mni.DropDownOpening += new EventHandler(this.mniWorkspaceItem_DropDownOpening);
				mni.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { new ToolStripMenuItem("WILL_BE_CLEARED_IN_this.mniWorkspaceItem_DropDownOpening()__JUST_ADDS_A_TRIANGLE") });
				this.workspaceMenuItemsWithHandlers.Add(mni);
			}
		}

		void mniWorkspaceItem_DropDownOpening(object sender, EventArgs e) {
			ToolStripMenuItem mni = sender as ToolStripMenuItem;
			if (mni == null) {
				string msg = "I_REFUSE_TO_LOAD sender as ToolStripMenuItem = null";
				Assembler.PopupException(msg);
				return;
			}
			mni.DropDown = this.mainForm.CtxWorkspacesModify;

			string workspace = mni.Text;
			this.mainForm.MniltbWorklspaceDuplicateTo	.InputFieldValue	= workspace;
			this.mainForm.MniltbWorklspaceRenameTo		.InputFieldValue	= workspace;
			this.mainForm.MniltbWorklspaceNewBlank		.InputFieldValue	= workspace;
			this.mainForm.MniWorkspaceDelete			.Text				= "Delete [" + workspace + "]";

			this.mainForm.CtxWorkspacesModify.Tag = workspace;

			if (mni.Text == NAME_DEFAULT) {
				this.mainForm.MniltbWorklspaceRenameTo.Enabled = false;
				this.mainForm.MniWorkspaceDelete.Text = "Delete [DEFAULT_MUST_STAY]";
				this.mainForm.MniWorkspaceDelete.Enabled = false;
			} else {
				this.mainForm.MniltbWorklspaceRenameTo.Enabled = true;
				this.mainForm.MniWorkspaceDelete.Enabled = true;

				string workspaceCurrentlyLoaded = Assembler.InstanceInitialized.AssemblerDataSnapshot.WorkspaceCurrentlyLoaded;
				if (workspace == workspaceCurrentlyLoaded) {
					this.mainForm.MniWorkspaceDelete.Text = "Delete [LOAD_ANOTHER_TO_DELETE_THIS]";
					this.mainForm.MniWorkspaceDelete.Enabled = false;
				}
			}
		}
		
		void mniWorkspaceLoad_Click(object sender, EventArgs e) {
			var mniWorkspace = sender as ToolStripMenuItem;
			if (mniWorkspace == null) {
				Assembler.PopupException("mniWorkspace_Click(): (sender as ToolStripMenuItem) = null");
				return;
			}
			if (this.WorkspaceCurrentMni != null) this.mainForm.MainFormSerialize();
			this.WorkspaceCurrentMni.Checked = false;
			this.WorkspaceCurrentMni = mniWorkspace;
			this.mainForm.WorkspaceLoad(this.WorkspaceCurrentName);
		}
		void mniltbWorkspaceDuplicateTo_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string msig = " //mniltbWorkspaceDuplicateTo_UserTyped()";
			try {
				string workspace = this.mainForm.CtxWorkspacesModify.Tag as string;
				string workspaceDupe = e.StringUserTyped;
				this.repository.Duplicate(workspace, workspaceDupe);
				this.RescanRebuildWorkspacesMenu();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			}
		}
		void mniltbWorkspaceRenameTo_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string workspace = this.mainForm.CtxWorkspacesModify.Tag as string;
			string workspaceRenameTo = e.StringUserTyped;
			string workspaceCurrentlyLoaded = Assembler.InstanceInitialized.AssemblerDataSnapshot.WorkspaceCurrentlyLoaded;
			string msig = " //mniltbWorkspaceRenameTo_UserTyped(workspaceCurrentlyLoaded[" + workspaceCurrentlyLoaded + "]=> workspaceRenameTo[" + workspaceRenameTo + "])";

			try {
				this.repository.Rename(workspace, workspaceRenameTo);
				if (workspace == workspaceCurrentlyLoaded) {
					Assembler.InstanceInitialized.AssemblerDataSnapshot.WorkspaceCurrentlyLoaded = workspaceRenameTo;

					bool createdNewFile = this.mainForm.GuiDataSnapshotSerializer.Initialize(Assembler.InstanceInitialized.AppDataPath,
						"Sq1.Gui.GuiDataSnapshot.json", "Workspaces",
						Assembler.InstanceInitialized.AssemblerDataSnapshot.WorkspaceCurrentlyLoaded);
					bool mustContainNow_workspaceRenamed = this.mainForm.LayoutXml.Contains(workspaceRenameTo);
					if (mustContainNow_workspaceRenamed == false) {
						string msg = "mainForm.LayoutXml[" + this.mainForm.LayoutXml + "]"
							+ " mustContainNow_workspaceRenamed[" + mustContainNow_workspaceRenamed + "]";
						Assembler.PopupException(msg + msig);
					}
				}
				this.RescanRebuildWorkspacesMenu();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			}
		}
		void mniltbWorkspaceNewBlank_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string workspace = this.mainForm.CtxWorkspacesModify.Tag as string;
			string workspaceNewBlank = e.StringUserTyped;
			string msig = " //mniWorkspaceDelete_Click()";
			try {
				this.repository.Add(workspaceNewBlank);
				this.RescanRebuildWorkspacesMenu();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			}
		}
		void mniWorkspaceDelete_Click(object sender, EventArgs e) {
			string workspace = this.mainForm.CtxWorkspacesModify.Tag as string;
			string workspaceCurrentlyLoaded = Assembler.InstanceInitialized.AssemblerDataSnapshot.WorkspaceCurrentlyLoaded;
			string msig = " //mniWorkspaceDelete_Click()";
			try {
				if (workspace == workspaceCurrentlyLoaded) {
					string msg = "I_REFUSE_TO_DELETE_workspaceCurrentlyLoaded[" + workspaceCurrentlyLoaded + "]";
					Assembler.PopupException(msg);
					return;
				}
				this.repository.Delete(workspace);
				this.RescanRebuildWorkspacesMenu();
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			}
		}

		public void SelectWorkspaceAfterLoaded(string workspaceLoaded) {
			string msig = " //SelectWorkspaceAfterLoaded(" + workspaceLoaded + ")";
			if (string.IsNullOrEmpty(workspaceLoaded)) {
				string msg = "I_REFUSE_TO_CHECK_EMPTY_WORKSPACE string.IsNullOrEmpty(" + workspaceLoaded + ") == true";
				Assembler.PopupException(msg + msig);
				return;
			}
			var mni = this.findMniByWorkspaceName(workspaceLoaded);
			if (mni == null) {
				Assembler.PopupException("SelectWorkspaceLoaded(" + workspaceLoaded + ") not found");
				return;
			}
			if (this.WorkspaceCurrentMni == mni) {
				string msg = "I_REFUSE_TO_CHECK_THIS_AND_UNCHECK_OTHERS__YOU_CHECKED_THE_SAME this.WorkspaceCurrentMni=mni[" + mni.Text + "]";
				//Assembler.PopupException(msg + msig);
				return;
			}
			this.WorkspaceCurrentMni = mni;
			foreach (var every in this.workspaceMenuItemsWithHandlers) every.Checked = false;
			this.WorkspaceCurrentMni.Checked = true;
		}
		
		ToolStripMenuItem findMniByWorkspaceName(string workspaceToFind) {
			ToolStripMenuItem ret = null;
			foreach (var mni in this.workspaceMenuItemsWithHandlers) {
				if (mni.Text != workspaceToFind) continue;
				ret = mni;
				break;
			}
			return ret;
		}

		public void RescanRebuildWorkspacesMenu(string currentWorkspaceName = null) {
			if (string.IsNullOrEmpty(currentWorkspaceName)) {
				currentWorkspaceName = Assembler.InstanceInitialized.AssemblerDataSnapshot.WorkspaceCurrentlyLoaded;
			}

			this.rescanFolders_BuildWorkspacesMenuItemsWithHandlersFromRepository();

			this.mainForm.CtxWorkspaces.Items.Clear();
			this.mainForm.CtxWorkspaces.Items.AddRange(this.workspaceMenuItemsWithHandlers.ToArray());
			this.SelectWorkspaceAfterLoaded(currentWorkspaceName);
		}
	}
}
