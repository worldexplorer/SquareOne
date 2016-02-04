using System;
using System.IO;

using Newtonsoft.Json;

namespace Sq1.Core.Serializers {
	// http://stackoverflow.com/questions/1727346/what-is-the-use-of-default-keyword-in-c
	public class Serializer<T> where T : new() {
		public	string			OfWhat			{ get { return typeof(T).Name; } }
		
		public	string			RootPath		{ get; protected set; }
		public	string			Subfolder		{ get; protected set; }
		public	string			WorkspaceName	{ get; protected set; }
		public	string			AbsPath			{ get {
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
			} }
		public		string		FnameRelpath				{ get; protected set; }
		public		string		JsonAbsFile					{ get { return Path.Combine(this.AbsPath, FnameRelpath); } }

		//RENAME_IN_CHILDREN__AVOID_MAUVAIS_TONE__INHERIT_RESPONSIBILITY_FOR_ITS_LIFECYCLE
		public		T			EntityDeserialized			{ get; protected set; }
		public		Action<T>	ActionAfterDeserialized;

		public Serializer() {
			this.EntityDeserialized = new T();
		}
		public bool Initialize(string rootPath, string relFname,
					string subfolder = "Workspaces", string workspaceName = "Default",
					bool createNonExistingPath = true, bool createNonExistingFile = true) {
			
			bool createdNewEmptyFile = false;
			string msig = " //" + this.ToString() + ".Initialize("
					+ "rootPath=[" + rootPath + "], subfolder=[" + subfolder + "],"
					+ " workspaceName=[" + workspaceName + "], relFname=[" + relFname + "],"
					+ " createNonExistingPath=[" + createNonExistingPath + "],"
					+ " createNonExistingFile=[" + createNonExistingFile + "]): ";
			
			if (string.IsNullOrEmpty(rootPath)) {
				string msg = "rootPath.IsNullOrEmpty(" + rootPath + ")";
				Assembler.PopupException(msg + msig);
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
					Assembler.PopupException(msg + msig);
				}
				try {
					Directory.CreateDirectory(this.AbsPath);
				} catch (Exception e) {
					string msg = "FAILED_TO Directory.CreateDirectory(" + this.AbsPath + ")";
					Assembler.PopupException(msg + msig);
				}
			}
			if (File.Exists(this.JsonAbsFile) == false) {
				if (createNonExistingFile == false) {
					string msg = "File.Exists(this.JsonAbsFile[" + this.JsonAbsFile + "])=false";
					Assembler.PopupException(msg + msig);
				}
				try {
					using (FileStream stream = File.Create(this.JsonAbsFile)) {
						createdNewEmptyFile = true;
						stream.Close();
					}
				} catch (Exception ex) {
					string msg = "FAILED_TO File.Create(" + this.JsonAbsFile + ")";
					Assembler.PopupException(msg + msig, ex);
				}
			}
			return createdNewEmptyFile;
		}

		public virtual T Deserialize() {
			if (File.Exists(this.JsonAbsFile) == false) return this.EntityDeserialized;
			try {
				string json = File.ReadAllText(this.JsonAbsFile);
				if (string.IsNullOrEmpty(json) == false) {
					this.EntityDeserialized = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings {
						TypeNameHandling = TypeNameHandling.Objects});
				}
				if (this.EntityDeserialized == null) this.EntityDeserialized = new T();
				if (this.ActionAfterDeserialized != null) this.ActionAfterDeserialized(this.EntityDeserialized);
			} catch (Exception ex) {
				string msig = " //" + this.ToString() + ".Deserialize";
				string msg = "FAILED_Deserialize_WITH_this.JsonAbsFile[" + this.JsonAbsFile + "]";
				Assembler.PopupException(msg + msig, ex);
			}
			return this.EntityDeserialized;
		}
		public virtual void Serialize() {
			string json;
			try {
				json = JsonConvert.SerializeObject(this.EntityDeserialized, Formatting.Indented,
					new JsonSerializerSettings {
						TypeNameHandling = TypeNameHandling.Objects
					});
				File.WriteAllText(this.JsonAbsFile, json);
			} catch (Exception ex) {
				string msig = " //" + this.ToString() + ".Serialize()";
				string msg = "FAILED_Serialize";
				if (this.AbsPath != null) msg += "_WITH_this.JsonAbsFile[" + this.JsonAbsFile + "]";
				Assembler.PopupException(msg + msig, ex);
			}
		}
		public virtual void DeleteJsonFile() {
			try {
				File.Delete(this.JsonAbsFile);
			} catch (Exception ex) {
				string msig = " //" + this.ToString() + ".DeleteJsonFile()";
				string msg = "FAILED_DELETE_JSON_FILE_WITH_this.JsonAbsFile[" + this.JsonAbsFile + "]";
				Assembler.PopupException(msg + msig, ex);
			}
		}

		public override string ToString() {
			return "Serializer<" + this.OfWhat + ">";
		}
	}
}