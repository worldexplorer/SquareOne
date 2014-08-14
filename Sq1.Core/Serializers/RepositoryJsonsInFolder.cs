using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using Sq1.Core.DataFeed;
using Sq1.Core.Support;

namespace Sq1.Core.Serializers {
	public class RepositoryJsonsInFolder<DATASOURCE>  /* hack for ItemRename() */ where DATASOURCE : NamedObjectJsonSerializable {
		public string OfWhat { get {
				string ret = "UNKNOWN";
				var type = this.GetType();
				var args = this.GetType().GetGenericArguments();
				if (args.Length > 0) ret = args[0].Name;
				return ret;
			} }

		public event EventHandler<NamedObjectJsonEventArgs<DATASOURCE>> OnItemAdded;
		public event EventHandler<NamedObjectJsonEventArgs<DATASOURCE>> OnItemCanBeRemoved;
		public event EventHandler<NamedObjectJsonEventArgs<DATASOURCE>> OnItemRemovedDone;
		public event EventHandler<NamedObjectJsonEventArgs<DATASOURCE>> OnItemRenamed;

		protected IStatusReporter StatusReporter;
		public string RootPath { get; protected set; }
		public string Subfolder { get; protected set; }
		public string AbsPath {
			get {
				//string ret = this.RootPath + this.Subfolder + this.WorkspaceName + Path.DirectorySeparatorChar;
				string ret = this.RootPath;
				if (String.IsNullOrEmpty(this.Subfolder) == false) {
					ret = Path.Combine(ret, this.Subfolder);
				}
				//if (ret.EndsWith(Path.DirectorySeparatorChar) == false) ret += Path.DirectorySeparatorChar;
				return ret;
			}
		}
		public string Mask { get; protected set; }
		public string MaskRel {
			get { return Path.Combine(this.Subfolder, Mask); }
		}
		public string MaskAbs {
			get { return Path.Combine(this.AbsPath, Mask); }
		}
		
		protected Dictionary<string, DATASOURCE> ItemsByName;
		public List<DATASOURCE> ItemsAsList { get {
				return new List<DATASOURCE>(this.ItemsByName.Values);
			} }
	
		public Func<string, DATASOURCE, bool> CheckIfValidAndShouldBeAddedAfterDeserialized;

		public RepositoryJsonsInFolder() {
			this.ItemsByName = new Dictionary<string, DATASOURCE>();			
		}
		public void Initialize(string rootPath,
		            string subfolder = "DataSources",
		            IStatusReporter statusReporter = null,
		            Func<string, DATASOURCE, bool> checkIfValidAndShouldBeAddedAfterDeserialized = null,
		            string mask = "*.json",
					bool createNonExistingPath = true, bool createNonExistingFile = true) {
			
			this.StatusReporter = statusReporter;
			
			string msig = "RepositoryJsonsInFolder<" + OfWhat + ">::Initialize("
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
				this.StatusReporter.PopupException(msg + msig);
				return;
			}
			string[] files = Directory.GetFiles(this.AbsPath, this.Mask);
			for (int i = 0; i < files.Length; i++) {
				string fileName = files[i];
				string thisOne = "[" + fileName + "]=[" + i + "]/[" + files.Length + "]";
				DATASOURCE deserialized = this.DeserializeSingle(fileName);
				if (deserialized == null) {
					string msg = "FAILED_TO_DESERIALIZE_ONE_OF_MANY_JSONS " + thisOne;
					this.StatusReporter.PopupException(msg + msig);
					continue;
				}
				string key = this.ExtractKeyFromJsonAbsname(fileName);
				this.ItemsByName.Add(key, deserialized);
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
				if (this.CheckIfValidAndShouldBeAddedAfterDeserialized != null) {
					bool valid = this.CheckIfValidAndShouldBeAddedAfterDeserialized(jsonAbsfile, ret);
					if (!valid) return null;
				}
			} catch (Exception ex) {
				string msig = " RepositoryJsonsInFolder<" + OfWhat + ">::DeserializeSingle(): ";
				string msg = "FAILED_DeserializeSingle_WITH_jsonAbsfile[" + jsonAbsfile + "]";
				Assembler.PopupException(msg + msig, ex);
				return ret;
			}
			return ret;
		}
		public virtual void SerializeSingle(DATASOURCE itemStored, string jsonRelname = null) {
			if (jsonRelname == null) jsonRelname = this.jsonRelnameForItem(itemStored);
			string jsonAbsname = Path.Combine(this.AbsPath, jsonRelname);
			try {
				string json = JsonConvert.SerializeObject(itemStored, Formatting.Indented,
					new JsonSerializerSettings {
						TypeNameHandling = TypeNameHandling.Objects
					});
				File.WriteAllText(jsonAbsname, json);
			} catch (Exception ex) {
				string msig = " RepositoryJsonsInFolder<" + OfWhat + ">::SerializeSingle(): ";
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
			return itemStored.Name + ".json";
		}
		public void ItemAdd(DATASOURCE itemCandidate, object sender = null) {
			if (sender == null) sender = this;
			string msig = " RepositoryJsonsInFolder<" + OfWhat + ">::ItemAdd(" + itemCandidate.Name + "): ";
			try {
				if (this.ItemsByName.ContainsKey(itemCandidate.Name)) {
					throw new Exception("ALREADY_EXISTS[" + itemCandidate.Name + "]");
				}
				this.ItemAddCascade(itemCandidate, sender);
				this.ItemsByName.Add(itemCandidate.Name, itemCandidate);
				this.SerializeSingle(itemCandidate);
				if (this.OnItemAdded == null) return;
				//this.OnItemAdded(this, new DataSourceEventArgs(itemCandidate));
				this.OnItemAdded(sender, new NamedObjectJsonEventArgs<DATASOURCE>(itemCandidate));
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			}
		}
		public virtual void ItemAddCascade(DATASOURCE itemCandidate, object sender = null) {
		}
		public void ItemDelete(DATASOURCE itemStored, object sender = null) {
			if (sender == null) sender = this;
			string msig = " RepositoryJsonsInFolder<" + OfWhat + ">::ItemDelete(" + itemStored.Name + "): ";
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
				if (this.OnItemRemovedDone == null) return;
				// since you answered DataSourceEventArgs.DoNotDeleteThisDataSourceBecauseItsUsedElsewhere=false,
				// you were aware that OnItemRemovedDone is invoked on a detached object
				this.OnItemRemovedDone(sender, args);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			}
		}
		public virtual void ItemDeleteCascade(DATASOURCE itemStored, object sender = null) {
		}
		public NamedObjectJsonEventArgs<DATASOURCE> ItemCanBeDeleted(DATASOURCE itemStored, object sender = null) {
			if (sender == null) sender = this;
			var args = new NamedObjectJsonEventArgs<DATASOURCE>(itemStored);
			if (this.OnItemCanBeRemoved == null) return args;
			this.OnItemCanBeRemoved(sender, args);
			this.ItemCanBeDeletedCascade(args, sender);
			return args;
		}
		public virtual void ItemCanBeDeletedCascade(NamedObjectJsonEventArgs<DATASOURCE> args, object sender = null) {
		}
		// very hacky, you should inherit DataSource from NamedObjectJsonSerializable with .Name property
		public void ItemRename(DATASOURCE itemStored, string newName, object sender = null) {
			if (sender == null) sender = this;
			string msig = " RepositoryJsonsInFolder<" + OfWhat + ">::ItemRename(" + itemStored.Name + ", " + newName + "): ";
			try {
				if (this.ItemsByName.ContainsKey(itemStored.Name) == false) {
					throw new Exception("ALREADY_DELETED[" + itemStored.Name + "]");
				}
				if (this.ItemsByName.ContainsKey(newName)) {
					throw new Exception("ALREADY_EXISTS[" + itemStored.Name + "]");
				}
				string jsonRelnameToDeleteBeforeRename = this.jsonRelnameForItem(itemStored);
				string oldName = itemStored.Name;
				itemStored.Name = newName;
				this.ItemRenameCascade(itemStored, oldName, sender);

				var items = new List<DATASOURCE>(this.ItemsByName.Values);
				this.ItemsByName.Clear();
				foreach (var item in items) {
					this.ItemsByName.Add(item.Name, item);
				}

				this.SerializeSingle(itemStored);
				this.JsonDeleteRelname(jsonRelnameToDeleteBeforeRename);
				if (this.OnItemRenamed == null) return;
				this.OnItemRenamed(sender, new NamedObjectJsonEventArgs<DATASOURCE>(itemStored));
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			}
		}
		public virtual void ItemRenameCascade(DATASOURCE itemRenamed, string oldName, object sender = null) {
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
