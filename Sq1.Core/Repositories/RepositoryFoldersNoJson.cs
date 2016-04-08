using System;
using System.Collections.Generic;
using System.IO;

namespace Sq1.Core.Repositories {
	public class RepositoryFoldersNoJson {
		public	string			RootPath		{ get; protected set; }
		public	string			Subfolder		{ get; protected set; }
		public	string			AbsPath			{ get {
				//string ret = this.RootPath + this.Subfolder + this.WorkspaceName + Path.DirectorySeparatorChar;
				string ret = this.RootPath;
				if (String.IsNullOrEmpty(this.Subfolder) == false) {
					ret = Path.Combine(ret, this.Subfolder);
				}
				//if (ret.EndsWith(Path.DirectorySeparatorChar).ToString() == false) ret += Path.DirectorySeparatorChar;
				return ret;
			} }
		public	List<string>	FoldersWithin	{ get; protected set; }

		public	RepositoryFoldersNoJson() { this.FoldersWithin = new List<string>(); }

		public void Initialize(string rootPath,
					string subfolder = "DataSources",
					bool createNonExistingPath = true) {
			
			string msig = "RepositoryFoldersNoJson::Initialize("
					+ "rootPath=[" + rootPath + "], subfolder=[" + subfolder + "]: ";
			
			if (string.IsNullOrEmpty(rootPath)) {
				string msg = "rootPath.IsNullOrEmpty(" + rootPath + ")";
				Assembler.PopupException(msig + msg);
			}

			//if (string.IsNullOrEmpty(subfolder) == false && subfolder.EndsWith(Path.DirectorySeparatorChar) == false) subfolder += Path.DirectorySeparatorChar;
			this.Subfolder = subfolder;
			
			//if (rootPath.EndsWith(Path.DirectorySeparatorChar) == false) rootPath += Path.DirectorySeparatorChar;
			this.RootPath = rootPath;

			if (Directory.Exists(this.AbsPath) == false) {
				if (createNonExistingPath == false) {
					string msg = "Directory.Exists(" + this.AbsPath + ")=false AND createPath=false";
					throw new Exception(msig + msg);
				}
				try {
					Directory.CreateDirectory(this.AbsPath);
				} catch (Exception e) {
					string msg = "FAILED_TO Directory.CreateDirectory(" + this.AbsPath + ")";
					throw new Exception(msig + msg);
				}
			}
		}
		public void RescanFolders() {
			this.FoldersWithin.Clear();
			if (Directory.Exists(this.AbsPath) == false) {
				string msg = "ScanFolders() path[" + this.AbsPath + "] doesn't exist; returning";
				Assembler.PopupException(msg);
				return;
			}
			string[] folders = Directory.GetDirectories(this.AbsPath);
			for (int i=0; i<folders.Length; i++) {
				string folderAbspath = folders[i];
				string folderName = Path.GetFileName(folderAbspath);
				this.FoldersWithin.Add(folderName);
			}
		}
		protected int DeleteFolderAndInnerFiles(string workspace) {
			int filesDeleted = 0;
			string folderAbsname = Path.Combine(this.AbsPath, workspace);
			if (Directory.Exists(folderAbsname) == false) {
				string msg = "I_REFUSE_TO_DELETE_NON-EXISTING_WORKSPACE Directory.Exists(" + folderAbsname + ")=false";
				Assembler.PopupException(msg);
				return filesDeleted;
			}

			// an exception during deletion of files inside a folder => points where exactly was problem with permissions
			foreach (string eachFile in Directory.EnumerateFiles(folderAbsname)) {
				string fileAbsname = Path.Combine(folderAbsname, eachFile);
				File.Delete(fileAbsname);
				filesDeleted++;
			}

			Directory.Delete(folderAbsname);
			return filesDeleted;
		}
		protected void CreateFolder(string workspace) {
			string folderAbsname = Path.Combine(this.AbsPath, workspace);
			if (Directory.Exists(folderAbsname)) {
				string msg = "I_REFUSE_TO_CREATE_ALREADY-EXISTING_WORKSPACE Directory.Exists(" + folderAbsname + ")=true";
				Assembler.PopupException(msg);
				return;
			}
			Directory.CreateDirectory(folderAbsname);
		}
		protected void RenameFolder(string workspace, string workspaceNew) {
			string folderAbsname = Path.Combine(this.AbsPath, workspace);
			if (Directory.Exists(folderAbsname) == false) {
				string msg = "I_REFUSE_TO_CREATE_ALREADY-EXISTING_WORKSPACE Directory.Exists(" + folderAbsname + ")=true";
				Assembler.PopupException(msg);
				return;
			}
			string folderAbsnameNew = Path.Combine(this.AbsPath, workspaceNew);
			if (workspace.ToUpper() != workspaceNew.ToUpper()) {
				Directory.Move(folderAbsname, folderAbsnameNew);
			} else {
				//WINDOWS_TROWS_DIRECTORY_ALREADY_EXISTS__WHEN_RENAMING_goofy_to_Goofy
				string tmp = folderAbsname + "_tmp";
				Directory.Move(folderAbsname, tmp);
				Directory.Move(tmp, folderAbsnameNew);
			}
		}
		protected int CopyInnerFiles(string workspaceFrom, string workspaceTo) {
			int filesCopied = 0;
			string folderAbsnameFrom = Path.Combine(this.AbsPath, workspaceFrom);
			if (Directory.Exists(folderAbsnameFrom) == false) {
				string msg = "I_REFUSE_TO_COPY__WORKSPACE_FROM__DOESNT_EXIST Directory.Exists(" + folderAbsnameFrom + ")=false";
				Assembler.PopupException(msg);
				return filesCopied;
			}
			string folderAbsnameTo = Path.Combine(this.AbsPath, workspaceTo);
			if (Directory.Exists(folderAbsnameTo) == false) {
				this.CreateFolder(workspaceTo);
			}

			foreach (string eachFrom in Directory.EnumerateFiles(folderAbsnameTo)) {
				string fileAbsnameTo = Path.Combine(folderAbsnameTo, eachFrom);
				File.Delete(fileAbsnameTo);
			}
		
			foreach (string eachFrom in Directory.EnumerateFiles(folderAbsnameFrom)) {
				string eachFromNoPath = Path.GetFileName(eachFrom);
				string fileAbsnameFrom = Path.Combine(folderAbsnameFrom, eachFromNoPath);
				string fileAbsnameTo   = Path.Combine(folderAbsnameTo,   eachFromNoPath);
				File.Copy(fileAbsnameFrom, fileAbsnameTo);
				filesCopied++;
			}
			return filesCopied;
		}




		// copypaste & refactor from RepositorySerializerSymbolInfo.cs
		public string FindWorkspaceName_nullUnsafe(string workspace) {
			string ret = null;
			if (string.IsNullOrEmpty(workspace)) return ret;
			foreach (string eachWorkspaceName in this.FoldersWithin) {
				//NOT!!! FORCASE_INSENSITIVE_FOLDERS if (eachWorkspaceName.ToUpper() != workspace.ToUpper()) continue;
				if (eachWorkspaceName != workspace) continue;
				ret = eachWorkspaceName;
				break;
			}
			return ret;
		}
		public string FindWorkspaceNameOrNew(string workspace) {
			string ret = this.FindWorkspaceName_nullUnsafe(workspace);
			if (ret == null) ret = this.Add(workspace);
			return ret;
		}
		public string Duplicate(string workspace, string workspaceDupe) {
			if (this.FindWorkspaceName_nullUnsafe(workspaceDupe) != null) {
				string msg = "I_REFUSE_TO_DUPLICATE_WORKSPACE__SYMBOL_ALREADY_EXISTS[" + workspaceDupe + "]";
				Assembler.PopupException(msg);
				return null;
			}
			this.CopyInnerFiles(workspace, workspaceDupe);
			this.FoldersWithin.Add(workspace);
			this.sort();
			return workspaceDupe;
		}
		public string Rename(string workspace, string workspaceNew) {
			if (this.FindWorkspaceName_nullUnsafe(workspaceNew) != null) {
				string msg = "I_REFUSE_TO_RENAME_WORKSPACE__SYMBOL_ALREADY_EXISTS[" + workspaceNew + "]";
				Assembler.PopupException(msg);
				return null;
			}
			this.RenameFolder(workspace, workspaceNew);
			this.FoldersWithin.Add(workspaceNew);
			this.FoldersWithin.Remove(workspace);
			this.sort();
			return workspace;
		}
		public string Add(string workspaceNew) {
			if (this.FindWorkspaceName_nullUnsafe(workspaceNew) != null) {
				string msg = "I_REFUSE_TO_ADD_WORKSPACE__SYMBOL_ALREADY_EXISTS[" + workspaceNew + "]";
				Assembler.PopupException(msg);
				return null;
			}
			this.CreateFolder(workspaceNew);
			this.FoldersWithin.Add(workspaceNew);
			this.sort();
			return workspaceNew;
		}
		public string Delete(string workspace) {
			int index = this.FoldersWithin.IndexOf(workspace);
			if (index == -1) {
				string msg = "I_REFUSE_TO_DELETE_WORKSPACE__NOT_FOUND[" + workspace.ToString() + "]";
				Assembler.PopupException(msg);
				return null;
			}
			this.DeleteFolderAndInnerFiles(workspace);
			this.FoldersWithin.Remove(workspace);
			this.sort();
			if (index == 0) {
				string msg = "LAST_WORKSPACE_DELETED[" + workspace.ToString() + "]";
				Assembler.PopupException(msg);
				return null;
			}
			string prior = this.FoldersWithin[index - 1];
			return prior;
		}


		public class ASC : IComparer<string> { int IComparer<string>.Compare(string x, string y) { return x.CompareTo(y); } }
		public void sort() {
			this.FoldersWithin.Sort(new ASC());
		}
		public void RescanFoldersAndSort() {
			this.RescanFolders();
			this.sort();
		}
	}
}
