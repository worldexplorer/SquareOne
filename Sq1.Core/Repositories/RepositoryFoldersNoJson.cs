using System;
using System.Collections.Generic;
using System.IO;

using Sq1.Core.Support;

namespace Sq1.Core.Repositories {
	public class RepositoryFoldersNoJson {
		protected IStatusReporter StatusReporter;
		public string RootPath { get; protected set; }
		public string Subfolder { get; protected set; }
		public string AbsPath { get {
				//string ret = this.RootPath + this.Subfolder + this.WorkspaceName + Path.DirectorySeparatorChar;
				string ret = this.RootPath;
				if (String.IsNullOrEmpty(this.Subfolder) == false) {
					ret = Path.Combine(ret, this.Subfolder);
				}
				//if (ret.EndsWith(Path.DirectorySeparatorChar).ToString() == false) ret += Path.DirectorySeparatorChar;
				return ret;
			} }
		public List<string> FoldersWithin { get; protected set; }
		public RepositoryFoldersNoJson() { this.FoldersWithin = new List<string>(); }
		public void Initialize(string rootPath,
		            string subfolder = "DataSources",
		            IStatusReporter statusReporter = null,
					bool createNonExistingPath = true) {
			
			this.StatusReporter = statusReporter;
			
			string msig = "RepositoryFoldersNoJson::Initialize("
					+ "rootPath=[" + rootPath + "], subfolder=[" + subfolder + "]: ";
			
			if (string.IsNullOrEmpty(rootPath)) {
				string msg = "rootPath.IsNullOrEmpty(" + rootPath + ")";
				this.ThrowOrPopup(msig + msg);
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
		public void ScanFolders() {
			this.FoldersWithin.Clear();
			if (Directory.Exists(this.AbsPath) == false) {
				string msg = "ScanFolders() path[" + this.AbsPath + "] doesn't exist; returning";
				this.StatusReporter.PopupException(msg);
				return;
			}
			string[] folders = Directory.GetDirectories(this.AbsPath);
			for (int i=0; i<folders.Length; i++) {
				string folderAbspath = folders[i];
				string folderName = Path.GetFileName(folderAbspath);
				this.FoldersWithin.Add(folderName);
			}
		}
		public void ThrowOrPopup(string msg, Exception ex = null) {
			ex = new Exception(msg, ex); 
			if (this.StatusReporter == null) {
				throw ex;
			}
			this.StatusReporter.PopupException(msg, ex);
		}
	}
}
