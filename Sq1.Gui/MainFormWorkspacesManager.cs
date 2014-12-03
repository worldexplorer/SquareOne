using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Gui {
	public class MainFormWorkspacesManager {
		private const string prefix = "workspace_";
		private MainForm mainForm;

		public string WorkspaceCurrentName { get { return this.WorkspaceCurrentMni.Text; } }
		public ToolStripMenuItem WorkspaceCurrentMni { get; protected set; }

		private List<ToolStripMenuItem> workspaceMenuItems;
		public ToolStripMenuItem[] WorkspaceMenuItemsWithHandlers { get { return this.workspaceMenuItems.ToArray(); } }

		public MainFormWorkspacesManager(MainForm mainForm) {
			this.mainForm = mainForm;
			this.workspaceMenuItems = this.initializeFromRepository();
		}
		
		private List<ToolStripMenuItem> initializeFromRepository() {
			var ret = new List<ToolStripMenuItem>();
			foreach (string workspace in Assembler.InstanceInitialized.WorkspacesRepository.FoldersWithin) {
				var mni = new ToolStripMenuItem();
				mni.Text = workspace;
				mni.Name = MainFormWorkspacesManager.prefix + workspace; 
				mni.Click += new EventHandler(mniWorkspace_Click);
				mni.CheckOnClick = true;
				ret.Add(mni);
			}
			return ret;
		}
		
		void mniWorkspace_Click(object sender, EventArgs e) {
			var mniWorkspace = sender as ToolStripMenuItem;
			if (mniWorkspace == null) {
				Assembler.PopupException("mniWorkspace_Click(): (sender as ToolStripMenuItem) = null");
				return;
			}
			this.WorkspaceCurrentMni.Checked = false;
			this.WorkspaceCurrentMni = mniWorkspace;
			this.mainForm.WorkspaceLoad(this.WorkspaceCurrentName);
		}

		public void WorkspaceDeleteCurrent_Click(object sender, EventArgs e) {
			var next = this.findNextOrPrevIfLastAfterDeletion();
			//Assembler.InstanceInitialized.WorkspacesRepository.ItemDelete(this.WorkspaceCurrent);
			if (next == null) return;
			next.Checked = true;
			this.mainForm.WorkspaceLoad(this.WorkspaceCurrentName);
		}

		public void WorkspaceCloneTo_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string workspaceCloneToName = e.StringUserTyped;
//			ScriptContext scriptContextToRename = this.ScriptContextFromMniTag(sender);
//			this.Strategy.ScriptContextRename(scriptContextToRename, workspaceCloneToName);
//			this.RaiseOnScriptContextRenamed(workspaceCloneToName);
//			this.ctxParameterBags_Opening(this, null);
			Assembler.PopupException("NotImplementedException WorkspaceCloneTo_UserTyped(" + e.StringUserTyped + ")");
		}
		public void WorkspaceNewBlank_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string workspaceNewBlankName = e.StringUserTyped;
			Assembler.PopupException("NotImplementedException WorkspaceNewBlank_UserTyped(" + e.StringUserTyped + ")");
		}
		public void WorkspaceRenameTo_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string workspaceRenameToName = e.StringUserTyped;
			Assembler.PopupException("NotImplementedException WorkspaceRenameTo_UserTyped(" + e.StringUserTyped + ")");
		}

		public void SelectWorkspaceLoaded(string workspaceLoaded) {
			var mni = this.FindMniByWorkspaceName(workspaceLoaded);
			if (mni == null) {
				Assembler.PopupException("SelectWorkspaceLoaded(" + workspaceLoaded + ") not found");
				return;
			}
			this.WorkspaceCurrentMni = mni;
			foreach (var every in this.workspaceMenuItems) every.Checked = false;
			this.WorkspaceCurrentMni.Checked = true;
		}
		
		public ToolStripMenuItem FindMniByWorkspaceName(string workspaceToFind) {
			ToolStripMenuItem ret = null;
			foreach (var mni in this.workspaceMenuItems) {
				if (mni.Text != workspaceToFind) continue;
				ret = mni;
				break;
			}
			return ret;
		}
		
		private ToolStripMenuItem findNextOrPrevIfLastAfterDeletion() {
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
	}
}
