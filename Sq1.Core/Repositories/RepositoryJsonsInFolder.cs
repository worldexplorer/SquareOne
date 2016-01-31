using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using Sq1.Core.DataFeed;

namespace Sq1.Core.Repositories {
	public partial class RepositoryJsonsInFolder<DATASOURCE>  /* hack for ItemRename() */ where DATASOURCE : NamedObjectJsonSerializable {
		public string	OfWhat						{ get { return typeof(DATASOURCE).Name; } }

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
		
		protected Dictionary<string, DATASOURCE>	ItemsByName;
		public List<DATASOURCE>						ItemsAsList		{ get {
				return new List<DATASOURCE>(this.ItemsByName.Values);
			} }
	
		public Func<string, DATASOURCE, bool>		CheckIfValidAndShouldBeAddedAfterDeserialized;

		public RepositoryJsonsInFolder() {
			ItemsByName 	= new Dictionary<string, DATASOURCE>();			
		}
		public virtual void Initialize(string rootPath,
					string subfolder = "DataSources",
					Func<string, DATASOURCE, bool> checkIfValidAndShouldBeAddedAfterDeserialized = null,
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
   			this.CheckIfValidAndShouldBeAddedAfterDeserialized = checkIfValidAndShouldBeAddedAfterDeserialized;
		}
		
		public void DeserializeJsonsInFolder() {
			string msig = " DeserializeJsonsInFolder() path[" + this.AbsPath + "]";
			this.ItemsByName.Clear();
			if (Directory.Exists(this.AbsPath) == false) {
				string msg = "doesn't exist, no {" + this.MaskRel + "}s deserialized; returning";
				Assembler.PopupException(msg + msig);
				return;
			}
			string[] absFileNames = Directory.GetFiles(this.AbsPath, this.Mask);
			for (int i = 0; i < absFileNames.Length; i++) {
				string absFileName = absFileNames[i];
				string thisOne = "[" + absFileName + "]=[" + i + "]/[" + absFileNames.Length + "]";
				DATASOURCE deserialized = this.DeserializeSingle(absFileName);
				if (deserialized == null) {
					string msg = "FAILED_TO_DESERIALIZE_ONE_OF_MANY_JSONS " + thisOne;
					Assembler.PopupException(msg + msig);
					continue;
				}
				string key = this.ExtractKeyFromJsonAbsname(absFileName);
				//v1 this.ItemsByName.Add(key, deserialized);
				//v2
				this.ItemAdd(deserialized);
			}
		}
		public virtual DATASOURCE DeserializeSingle(string jsonAbsfile) {
			DATASOURCE ret = null;
			if (File.Exists(jsonAbsfile) == false) return ret;
			try {
				string json = File.ReadAllText(jsonAbsfile);
				ret = JsonConvert.DeserializeObject<DATASOURCE>(json, new JsonSerializerSettings {
					TypeNameHandling = TypeNameHandling.Objects});
				//if (ret == null) ret = new T();
				if (ret == null) return null;
				ret.NameImStoredUnder_asUniqueKeyForRename = ret.Name;
				if (this.CheckIfValidAndShouldBeAddedAfterDeserialized != null) {
					bool valid = this.CheckIfValidAndShouldBeAddedAfterDeserialized(jsonAbsfile, ret);
					if (!valid) return null;
				}
			} catch (Exception ex) {
				string msig = " RepositoryJsonsInFolder<" + this.OfWhat + ">::DeserializeSingle(): ";
				string msg = "FAILED_DeserializeSingle_WITH_jsonAbsfile[" + jsonAbsfile + "]"
					+ " check if you have DLL with the failed type in the same folder next to Sq1.Gui.exe";
				Assembler.PopupException(msg + msig, ex);
				return ret;
			}
			return ret;
		}
		public virtual void SerializeSingle(DATASOURCE itemStored, string jsonRelname = null) {
			bool shouldRenameAndSave =
				string.IsNullOrEmpty(	  itemStored.NameImStoredUnder_asUniqueKeyForRename) == false
					&& itemStored.Name != itemStored.NameImStoredUnder_asUniqueKeyForRename;

			if (shouldRenameAndSave) {
				string newName = itemStored.Name;
				itemStored.Name = itemStored.NameImStoredUnder_asUniqueKeyForRename;
				itemStored.NameImStoredUnder_asUniqueKeyForRename = null;
				this.ItemRename(itemStored, newName);	// SerializeSingle_WILL_BE_INVOKED_AGAIN_BUT_WILL_SERIALIZE_WITHOUT_MAKING_A_COPY
				return;
			}
			if (jsonRelname == null) jsonRelname = this.jsonRelnameForItem(itemStored);
			string jsonAbsname = Path.Combine(this.AbsPath, jsonRelname);
			try {
				string json = JsonConvert.SerializeObject(itemStored, Formatting.Indented,
					new JsonSerializerSettings {
						TypeNameHandling = TypeNameHandling.Objects
					});
				File.WriteAllText(jsonAbsname, json);
				// NO__USE_DeserializeJsonsInFolder()_MANUALLY_UPSTACK__AFTER_EACH_SerializeSingle();
				//if (this.ItemsByName.ContainsKey(itemStored.Name) == false) {
				//    this.ItemAdd(itemStored);
				//}
			} catch (Exception ex) {
				string msig = " RepositoryJsonsInFolder<" + this.OfWhat + ">::SerializeSingle(): ";
				string msg = "FAILED_SerializeSingle_WITH_this.jsonAbsname[" + jsonAbsname + "]";
				Assembler.PopupException(msg + msig, ex);
				return;
			}
		}
		public void SerializeAll() {
			foreach (DATASOURCE current in this.ItemsByName.Values) {
				this.SerializeSingle(current);
			}
		}
		
		public virtual string ExtractKeyFromJsonAbsname(string jsonAbsfile) {
			return Path.GetFileNameWithoutExtension(jsonAbsfile);
		}
		public string jsonRelnameForItem(DATASOURCE itemStored) {
			return itemStored.Name + this.Extension;
		}
		public void ItemAdd(DATASOURCE itemCandidate, object sender = null, bool serialize = false) {
			if (sender == null) sender = this;
			string msig = " RepositoryJsonsInFolder<" + this.OfWhat + ">::ItemAdd(" + itemCandidate.Name + "): ";
			try {
				if (this.ItemsByName.ContainsKey(itemCandidate.Name)) {
					throw new Exception("ALREADY_EXISTS[" + itemCandidate.Name + "]");
				}
				//this.ItemAddCascade(itemCandidate, sender);
				this.ItemsByName.Add(itemCandidate.Name, itemCandidate);
				if (serialize) this.SerializeSingle(itemCandidate);
				this.RaiseOnItemAdded(sender, itemCandidate);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			}
		}
		public virtual void ItemAddCascade(DATASOURCE itemCandidate, object sender = null) {
		}
		public void ItemDelete(DATASOURCE itemStored, object sender = null) {
			if (sender == null) sender = this;
			string msig = " RepositoryJsonsInFolder<" + this.OfWhat + ">::ItemDelete(" + itemStored.Name + "): ";
			try {
				if (this.ItemsByName.ContainsKey(itemStored.Name) == false) {
					throw new Exception("ALREADY_DELETED[" + itemStored.Name + "]");
				}
				var args = this.ItemCanBeDeleted(itemStored, sender);
				if (args.DoNotDeleteItsUsedElsewhere) {
					string msg = "DoNotDeleteReason=[" + args.DoNotDeleteReason + "] for ItemDelete(" + itemStored.ToString() + ")";
					throw new Exception(msg);
				}
				this.ItemDeleteCascade(itemStored, sender);
				this.ItemsByName.Remove(itemStored.Name);
				this.JsonDeleteItem(itemStored);
				this.RaiseOnItemRemovedDone(sender, args);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			}
		}
		public virtual void ItemDeleteCascade(DATASOURCE itemStored, object sender = null) {
		}
		public NamedObjectJsonEventArgs<DATASOURCE> ItemCanBeDeleted(DATASOURCE itemStored, object sender = null) {
			if (sender == null) sender = this;
			var args = new NamedObjectJsonEventArgs<DATASOURCE>(itemStored);
			this.RaiseOnItemCanBeRemoved(sender, args);
			this.ItemCanBeDeletedCascade(args, sender);
			return args;
		}
		public virtual void ItemCanBeDeletedCascade(NamedObjectJsonEventArgs<DATASOURCE> args, object sender = null) {
		}
		// very hacky, you should inherit DataSource from NamedObjectJsonSerializable with .Name property
		public void ItemRename(DATASOURCE itemStored, string newName, object sender = null) {
			if (sender == null) sender = this;
			string msig = " RepositoryJsonsInFolder<" + this.OfWhat + ">::ItemRename(" + itemStored.Name + ", " + newName + "): ";
			try {
				if (this.ItemsByName.ContainsKey(itemStored.Name) == false) {
					string msg = "ALREADY_DELETED[" + itemStored.Name + "]";
					// throw new Exception(msg);
					Assembler.PopupException(msg + msig);
				}
				if (this.ItemsByName.ContainsKey(newName)) {
					string msg = "ALREADY_EXISTS[" + itemStored.Name + "]";
					// throw new Exception(msg);
					Assembler.PopupException(msg + msig);
				}
				bool dataSourceFolderRenamed = this.ItemRenameCascade(itemStored, newName, sender);

				if (dataSourceFolderRenamed) {
					var items = new List<DATASOURCE>(this.ItemsByName.Values);
					this.ItemsByName.Clear();
					foreach (var item in items) {
						this.ItemsByName.Add(item.Name, item);
					}

					//string jsonRelnameToDeleteBeforeRename = this.jsonRelnameForItem(itemStored);
					string jsonRelnameBeforeRename	= this.jsonRelnameForItem(itemStored);
					string jsonRelnameAfterRename	= newName + this.Extension;
					bool jsonRenamed = this.jsonRename(jsonRelnameBeforeRename, jsonRelnameAfterRename);
					if (jsonRenamed) {
						itemStored.Name = newName;
						this.SerializeSingle(itemStored);
						//this.JsonDeleteRelname(jsonRelnameToDeleteBeforeRename);
						this.RaiseOnItemRenamed(sender, itemStored);
					} else {
						string msg = "I_REFUSE_TO_SerializeSingle(itemStored): jsonRenamed[" + jsonRenamed + "] NOT_RAISING_OnItemRenamed_EVENT";
						Assembler.PopupException(msg + msig);
					}
				} else {
					string msg = "I_REFUSE_TO_SerializeSingle(itemStored)_AND_REBUILD_RepositoryJsonsInFolder.ItemsByName dataSourceFolderRenamed[" + dataSourceFolderRenamed + "] NOT_RAISING_OnItemRenamed_EVENT";
					Assembler.PopupException(msg + msig);
				}
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			}
		}

		bool jsonRename(string jsonRelnameBeforeRename, string jsonRelnameAfterRename) {
			bool ret = false;
			string msig = " JsonRename(" + jsonRelnameBeforeRename + "=>" + jsonRelnameAfterRename + ")";
			if (Directory.Exists(this.AbsPath) == false) {
				throw new Exception("DATASOURCE_OLD_FOLDER_DOESNT_EXIST this.FolderForBarDataStore[" + this.AbsPath + "]" + msig);
			}
			string abspathOldJsonName = Path.Combine(this.AbsPath, jsonRelnameBeforeRename);
			string abspathNewJsonName = Path.Combine(this.AbsPath, jsonRelnameAfterRename);
			if (Directory.Exists(abspathNewJsonName)) {
				string abspathNewRandomFolderName = Path.Combine(this.AbsPath, jsonRelnameAfterRename + "-OutOfMyWay-" + new Random().Next(1000000, 9999999));
				int newNameGenTrialsDone = 0;
				int newNameGenTrialsLimit = 1000;
				while (Directory.Exists(abspathNewRandomFolderName)) {
					abspathNewRandomFolderName = Path.Combine(this.AbsPath, jsonRelnameAfterRename + "-OutOfMyWay-" + new Random().Next(1000000, 9999999));
					newNameGenTrialsDone++;
					if (newNameGenTrialsDone >= newNameGenTrialsLimit) {
						string fatal = "CHECK_YOUR_FOLDER_IT_HAS_WAY_TOO_MANY_-OutOfMyWay-_FOLDERS LAST_EXISTING[" + abspathNewRandomFolderName + "]"
							+ " newNameGenTrialsDone[" + newNameGenTrialsDone + "] >= newNameGenTrialsLimit[" +  newNameGenTrialsLimit + "]";
						//throw new Exception(fatal + msig);
						Assembler.PopupException(fatal + msig);
						return ret;
					}
				}
				string msg = "DATASOURCE_NEW_FOLDER_ALREADY_EXISTS abspathNewFolderName[" + abspathNewJsonName + "]=>renamingToRandom[" + abspathNewRandomFolderName + "] TO_GET_CLEAR_WAY";
				try {
					Directory.Move(abspathNewJsonName, abspathNewRandomFolderName);
					Assembler.PopupException(msg + msig);
				} catch (Exception ex) {
					msg = "RENAME_FAILED__GRANT_YOURSELF_FULL_CONTROL_TO_FOLDER this.AbsPath[" + this.AbsPath + "] " + msg;
					Assembler.PopupException(msg + msig, ex);
					return ret;
				}
			}
			try {
				Directory.Move(abspathOldJsonName, abspathNewJsonName);
			} catch (Exception ex) {
				string msg = "RENAME_FAILED__GRANT_YOURSELF_FULL_CONTROL_TO_FOLDER this.AbsPath[" + this.AbsPath + "] "
					+ " Directory.Move(" + abspathOldJsonName + "=>" + abspathNewJsonName + ")";
				Assembler.PopupException(msg + msig, ex);
				return ret;
			}
			ret = true;
			return ret;
		}
		public virtual bool ItemRenameCascade(DATASOURCE itemToRename, string newName, object sender = null) {
			return true;	// returning "yes renamed okay"
		}
		public void JsonDeleteItem(DATASOURCE itemStored) {
			string jsonRelname = this.jsonRelnameForItem(itemStored);
			this.JsonDeleteRelname(jsonRelname);
		}
		protected void JsonDeleteRelname(string jsonRelname) {
			string jsonAbsname = Path.Combine(this.AbsPath, jsonRelname);
			if (File.Exists(jsonAbsname)) File.Delete(jsonAbsname);
		}
		public DATASOURCE ItemFind(string dataSourceName) {
			if (this.ItemsByName.ContainsKey(dataSourceName) == false) return null;
			return ItemsByName[dataSourceName];
		}
	}
}
