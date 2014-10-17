using System;
using System.IO;
using Newtonsoft.Json;
using Sq1.Core.Support;

namespace Sq1.Core.Serializers {
	// http://stackoverflow.com/questions/1727346/what-is-the-use-of-default-keyword-in-c
	public class Serializer<T> where T : new() {
		public string OfWhat { get { return typeof(T).Name; } }
		
		private IStatusReporter StatusReporter;
		public string RootPath { get; protected set; }
		public string Subfolder { get; protected set; }
		public string WorkspaceName { get; protected set; }
		public string AbsPath {
			get {
				//string ret = this.RootPath + this.Subfolder + this.WorkspaceName + Path.DirectorySeparatorChar;
				string ret = this.RootPath;
				if (String.IsNullOrEmpty(this.Subfolder) == false) {
					ret = Path.Combine(ret, this.Subfolder);
				}
				if (String.IsNullOrEmpty(this.WorkspaceName) == false) {
					ret = Path.Combine(ret, this.WorkspaceName);
				}
				//if (ret.EndsWith(Path.DirectorySeparatorChar) == false) ret += Path.DirectorySeparatorChar;
				return ret;
			}
		}
		public string FnameRelpath { get; protected set; }
		public string JsonAbsFile {
			get { return Path.Combine(this.AbsPath, FnameRelpath); }
		}
		
		public T Entity { get; protected set; }
		
		public Action<T> ActionAfterDeserialized;

		public Serializer(IStatusReporter statusReporter = null) {
			this.Entity = new T();
			this.StatusReporter = statusReporter;
		}
		public bool Initialize(string rootPath, string relFname,
		            string subfolder = "Workspaces", string workspaceName = "Default",
					bool createNonExistingPath = true, bool createNonExistingFile = true) {
			
			bool createdNewEmptyFile = false;
			string msig = " Serializer<" + OfWhat + ">::Initialize("
					+ "rootPath=[" + rootPath + "], subfolder=[" + subfolder + "],"
					+ " workspaceName=[" + workspaceName + "], relFname=[" + relFname + "],"
					+ " createNonExistingPath=[" + createNonExistingPath + "],"
					+ " createNonExistingFile=[" + createNonExistingFile + "]): ";
			
			if (string.IsNullOrEmpty(rootPath)) {
				string msg = "rootPath.IsNullOrEmpty(" + rootPath + ")";
				this.ThrowOrPopup(msg + msig);
			}

			//if (string.IsNullOrEmpty(subfolder) == false && subfolder.EndsWith(Path.DirectorySeparatorChar) == false) subfolder += Path.DirectorySeparatorChar;
			this.Subfolder = subfolder;
			
			//if (string.IsNullOrEmpty(workspaceName) == false && workspaceName.EndsWith(Path.DirectorySeparatorChar) == false) workspaceName += Path.DirectorySeparatorChar;
			this.WorkspaceName = workspaceName;
			
			//if (rootPath.EndsWith(Path.DirectorySeparatorChar) == false) rootPath += Path.DirectorySeparatorChar;
			this.RootPath = rootPath;
			this.FnameRelpath = relFname;

			if (Directory.Exists(this.AbsPath) == false) {
				if (createNonExistingPath == false) {
					string msg = "DIRECTORY_DOESNT_EXIST_AND_YOU_DIDNT_SAY_CREATE Directory.Exists(" + this.AbsPath + ")=false AND createNonExistingPath=false";
					this.ThrowOrPopup(msg + msig);
				}
				try {
					Directory.CreateDirectory(this.AbsPath);
				} catch (Exception e) {
					string msg = "FAILED_TO Directory.CreateDirectory(" + this.AbsPath + ")";
					this.ThrowOrPopup(msg + msig);
				}
			}
			if (File.Exists(this.JsonAbsFile) == false) {
				if (createNonExistingFile == false) {
					string msg = "File.Exists(this.JsonAbsFile[" + this.JsonAbsFile + "])=false";
					this.ThrowOrPopup(msg + msig);
				}
				try {
					using (FileStream stream = File.Create(this.JsonAbsFile)) {
						createdNewEmptyFile = true;
						stream.Close();
					}
				} catch (Exception ex) {
					string msg = "FAILED_TO File.Create(" + this.JsonAbsFile + ")";
					this.ThrowOrPopup(msg + msig, ex);
				}
			}
			return createdNewEmptyFile;
		}

		public virtual T Deserialize() {
			if (File.Exists(this.JsonAbsFile) == false) return this.Entity;
			try {
				string json = File.ReadAllText(this.JsonAbsFile);
				if (string.IsNullOrEmpty(json) == false) {
					this.Entity = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings {
						TypeNameHandling = TypeNameHandling.Objects});
				}
				if (this.Entity == null) this.Entity = new T();
				if (this.ActionAfterDeserialized != null) this.ActionAfterDeserialized(this.Entity);
			} catch (Exception ex) {
				string msig = " Serializer<" + OfWhat + ">::Deserialize(): ";
				string msg = "FAILED_Deserialize_WITH_this.JsonAbsFile[" + this.JsonAbsFile + "]";
				this.ThrowOrPopup(msg + msig, ex);
			}
			return this.Entity;
		}
		public virtual void Serialize() {
			string json;
			try {
				json = JsonConvert.SerializeObject(this.Entity, Formatting.Indented,
					new JsonSerializerSettings {
						TypeNameHandling = TypeNameHandling.Objects
					});
				File.WriteAllText(this.JsonAbsFile, json);
			} catch (Exception ex) {
				string msig = " Serializer<" + OfWhat + ">::Serialize(): ";
				string msg = "FAILED_Serialize_WITH_this.JsonAbsFile[" + this.JsonAbsFile + "]";
				this.ThrowOrPopup(msg + msig, ex);
			}
		}
		public virtual void DeleteJsonFile() {
			try {
				File.Delete(this.JsonAbsFile);
			} catch (Exception ex) {
				string msig = " Serializer<" + OfWhat + ">::DeleteJsonFile(): ";
				string msg = "FAILED_DELETE_JSON_FILE_WITH_this.JsonAbsFile[" + this.JsonAbsFile + "]";
				this.ThrowOrPopup(msg + msig, ex);
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