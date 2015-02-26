using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Repositories {
	public partial class RepositoryJsonsInFolderSimple<SYSTEM_PERFORMANCE_RESTORE_ABLE>  /* hack for ItemRename() */ where SYSTEM_PERFORMANCE_RESTORE_ABLE : NamedObjectJsonSerializable {
		public string	OfWhat						{ get { return typeof(SYSTEM_PERFORMANCE_RESTORE_ABLE).Name; } }

		public string	RootPath					{ get; protected set; }
		public string	Subfolder					{ get; protected set; }
		public string	AbsPath						{ get {
				//string ret = this.RootPath + this.Subfolder + this.WorkspaceName + Path.DirectorySeparatorChar;
				string ret = this.RootPath;
				if (String.IsNullOrEmpty(this.Subfolder) == false) {
					ret = Path.Combine(ret, this.Subfolder);
				}
				//if (ret.EndsWith(Path.DirectorySeparatorChar) == false) ret += Path.DirectorySeparatorChar;
				return ret;
			} }
		public string	Mask						{ get; protected set; }
		public string	MaskRel						{ get { return Path.Combine(this.Subfolder, Mask); } }
		public string	MaskAbs						{ get { return Path.Combine(this.AbsPath, Mask); } }
		public string	Extension					{ get { return Path.GetExtension(this.Mask); } }
		
		public List<FnameDateSize> ItemsFound { get; protected set; }

		public RepositoryJsonsInFolderSimple() {
			ItemsFound = new List<FnameDateSize>();
		}

		public void Initialize(string rootPath,
					string subfolder = "OptimizationResults",
					string mask = "*.json",
					bool createNonExistingPath = true, bool createNonExistingFile = true) {

			string msig = "RepositoryJsonsInFolder<" + this.OfWhat + ">::Initialize("
					+ "rootPath=[" + rootPath + "], subfolder=[" + subfolder + "],"
					+ " mask=[" + mask + "],"
					+ " createNonExistingPath=[" + createNonExistingPath + "],"
					+ " createNonExistingFile=[" + createNonExistingFile + "]): ";

			if (string.IsNullOrEmpty(rootPath)) {
				string msg = "rootPath.IsNullOrEmpty(" + rootPath + ")";
				Assembler.PopupException(msg + msig);
				return;
			}

			//if (string.IsNullOrEmpty(subfolder) == false && subfolder.EndsWith(Path.DirectorySeparatorChar) == false) subfolder += Path.DirectorySeparatorChar;
			this.Subfolder = subfolder;

			//if (rootPath.EndsWith(Path.DirectorySeparatorChar) == false) rootPath += Path.DirectorySeparatorChar;
			this.RootPath = rootPath;
			this.Mask = mask;

			if (Directory.Exists(this.AbsPath) == false) {
				if (createNonExistingPath == false) {
					string msg = "Directory.Exists(" + this.AbsPath + ")=false AND createPath=false";
					throw new Exception(msig + msg);
					return;
				}
				try {
					Directory.CreateDirectory(this.AbsPath);
				} catch (Exception e) {
					string msg = "FAILED_TO Directory.CreateDirectory(" + this.AbsPath + ")";
					throw new Exception(msig + msg);
					return;
				}
			}
			this.RescanFolderStoreNamesFound();
		}
		public void RescanFolderStoreNamesFound() {
			this.ItemsFound.Clear();
			string[] absFileNames = Directory.GetFiles(this.AbsPath, this.Mask);
			for (int i = 0; i < absFileNames.Length; i++) {
				string absFileName = absFileNames[i];
				string thisOne = "[" + absFileName + "]=[" + i + "]/[" + absFileNames.Length + "]";
				FileInfo finfo = new FileInfo(absFileName);
				DateTime dateModified = finfo.LastWriteTime;
				long size = finfo.Length;
				string fname = Path.GetFileNameWithoutExtension(absFileName);
				FnameDateSize fnameDateSize = new FnameDateSize(fname, dateModified, size);
				this.ItemsFound.Add(fnameDateSize);
			}
		}
		public bool ItemsFoundContainsName(string name) {
			bool ret = false;
			foreach (FnameDateSize each in this.ItemsFound) {
				if (each.Name != name) continue;
				ret = true;
				break;
			}
			return ret;
		}
		public string AbsFnameFor(string fname) {
			return Path.Combine(this.AbsPath, fname, this.Extension);
		}
		public int ItemsFoundDeleteAll() {
			int ret = 0;
			foreach (FnameDateSize each in this.ItemsFound) {
				string absFname = this.AbsFnameFor(each.Name);
				File.Delete(absFname);
				ret++;
			}
			this.RescanFolderStoreNamesFound();
			return ret;
		}
		public virtual List<SYSTEM_PERFORMANCE_RESTORE_ABLE> DeserializeList(string fname) {
			List<SYSTEM_PERFORMANCE_RESTORE_ABLE> tmp = null;
			List<SYSTEM_PERFORMANCE_RESTORE_ABLE> ret = null;
			string jsonAbsfile = Path.Combine(this.AbsPath, fname + this.Extension);
			if (File.Exists(jsonAbsfile) == false) return ret;
			try {
				string json = File.ReadAllText(jsonAbsfile);
				ret = JsonConvert.DeserializeObject<List<SYSTEM_PERFORMANCE_RESTORE_ABLE>>(json, new JsonSerializerSettings {
					TypeNameHandling = TypeNameHandling.Objects
				});
			} catch (Exception ex) {
				string msig = " RepositoryJsonsInFolder<" + this.OfWhat + ">::DeserializeList(): ";
				string msg = "FAILED_DeserializeList_WITH_jsonAbsfile[" + jsonAbsfile + "]";
				Assembler.PopupException(msg + msig, ex);
				return ret;
			}
			return ret;
		}
		public virtual void SerializeList(List<SYSTEM_PERFORMANCE_RESTORE_ABLE> backtests, string jsonRelname) {
			jsonRelname += this.Extension;
			string jsonAbsname = Path.Combine(this.AbsPath, jsonRelname);
			try {
				string json = JsonConvert.SerializeObject(backtests, Formatting.Indented,
					new JsonSerializerSettings {
						TypeNameHandling = TypeNameHandling.Objects
					});
				File.WriteAllText(jsonAbsname, json);
			} catch (Exception ex) {
				string msig = " RepositoryJsonsInFolder<" + this.OfWhat + ">::SerializeList(): ";
				string msg = "FAILED_SerializeList_WITH_this.jsonAbsname[" + jsonAbsname + "]";
				Assembler.PopupException(msg + msig, ex);
				return;
			}
		}
	}
}
