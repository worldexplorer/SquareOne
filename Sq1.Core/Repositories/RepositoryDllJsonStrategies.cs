using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Newtonsoft.Json;

using Sq1.Core.StrategyBase;

namespace Sq1.Core.Repositories {
	// TODO: inherit from RepositoryDllScanner<Strategy>
	public class RepositoryDllJsonStrategies {
		public Dictionary<string, List<Strategy>>	StrategiesInFolders			{ get; private set; }
		public Dictionary<string, List<Script>>		ScriptsInDlls				{ get; private set; }
		public List<Strategy>						AllStrategiesAvailable		{ get {
				var ret = new List<Strategy>();
				foreach (string folder in this.StrategiesInFolders.Keys) ret.AddRange(this.StrategiesInFolders[folder]);
				//foreach (string key2 in this.ScriptsInDlls.Keys) ret.AddRange(this.ScriptsInDlls[key2]);
				return ret;
			} }
		public List<string>							AllFoldersAvailable			{ get {
				var ret = new List<string>();
				foreach (string folderJson in this.FoldersPurelyJson) ret.Add(folderJson);
				foreach (string folderDllShadow in this.FoldersDllShadows) ret.Add(folderDllShadow);
				return ret;
			} }
		public List<string>							FoldersPurelyJson			{ get {
				var ret = new List<string>();
				foreach (string folder in this.StrategiesInFolders.Keys) {
					if (folder.ToUpper().EndsWith(".DLL")) continue;
					ret.Add(folder);
				}
				return ret;
			} }
		public List<string>							FoldersDllShadows			{ get {
				var ret = new List<string>();
				foreach (string folder in this.StrategiesInFolders.Keys) {
					if (folder.ToUpper().EndsWith(".DLL") == false) continue;
					string folderPrefixed = folder;	// avoiding "Cannot assign to 'folder' because it is a 'foreach iteration variable'" compiler message
					bool dll_not_found_for_existing_folder = true;
					foreach (string dllFound in this.ScriptsInDlls.Keys) {
						if (folder.ToUpper() != dllFound.ToUpper()) continue;
						dll_not_found_for_existing_folder = false;
					}
					if (dll_not_found_for_existing_folder) {
						folderPrefixed = "DLL_NOT_FOUND: " + folderPrefixed;
						string msg = folderPrefixed + " STRATEGIES_FOLDER_CONTAINS_SHADOW_FOR_DLL_REMOVED_FROM_APPROOT_OR_STRATEGIES_FOLDER";
						Assembler.PopupException(null, new Exception(msg));
					}
					ret.Add(folderPrefixed);
				}
				return ret;
			} }

		public string								RootPath						{ get; private set; }
		public string								Subfolder						{ get; private set; }
		public string								PathMask						{ get; private set; }
		public string								AbsPath							{ get { return Path.Combine(this.RootPath, Subfolder); } }
		public List<Exception>						ExceptionsWhileInstantiating	{ get {
				List<Exception> ret = new List<Exception>();
				if (this.ScriptRepositoryFoundInFolderDataStrategies != null) {
					ret.AddRange(this.ScriptRepositoryFoundInFolderDataStrategies.ExceptionsWhileScanning);
				}
				if (this.ScriptRepositoryFoundInFolderAppStartup != null) {
					ret.AddRange(this.ScriptRepositoryFoundInFolderAppStartup.ExceptionsWhileScanning);
				}
				return ret;
			} }

		public RepositoryDllScripts					ScriptRepositoryFoundInFolderDataStrategies;
		public RepositoryDllScripts					ScriptRepositoryFoundInFolderAppStartup;
				string								AppStartupPath;

		public RepositoryDllJsonStrategies() {
			this.Subfolder = "Strategies" + Path.DirectorySeparatorChar + "";
			this.PathMask = "*.dll";
			this.StrategiesInFolders = new Dictionary<string, List<Strategy>>();
			this.ScriptsInDlls = new Dictionary<string, List<Script>>();
			//this.ScriptRepositoryFoundInFolderDataStrategies = new RepositoryDllScripts();
			//this.ScriptRepositoryFoundInFolderAppStartup = new RepositoryDllScripts();
		}
		public void RootPathCheckThrow(string rootPath) {
			if (string.IsNullOrEmpty(rootPath)) {
				throw new ArgumentException("FOLDER_REQUIRED: StrategyRepository.InitializeAndScan(string rootPath IsNullOrEmpty)");
			}
			if (Directory.Exists(rootPath) == false) {
				throw new ArgumentException("FOLDER_DOESNT_EXIST: StrategyRepository.InitializeAndScan(rootPath=[" + rootPath + "]");
			}
		}
		public void Initialize(string rootPath, string appStartupPath) {
			this.RootPathCheckThrow(rootPath);
			this.RootPath = rootPath;
			//if (this.RootPath.EndsWith(Path.DirectorySeparatorChar) == false) this.RootPath += Path.DirectorySeparatorChar;
			//if (this.Subfolder.EndsWith(Path.DirectorySeparatorChar) == false) this.Subfolder += Path.DirectorySeparatorChar;
			this.AppStartupPath = appStartupPath;
			this.StrategiesScanFoldersAndDlls();
		}
		public void StrategiesScanFoldersAndDlls() {
			this.StrategiesInFolders.Clear();
			this.ScriptsInDlls.Clear();
			
			if (Directory.Exists(this.AbsPath) == false) Directory.CreateDirectory(this.AbsPath);
			this.StrategiesInFolders = this.StrategiesScanFolders();
			Dictionary<Assembly, List<Script>> strategiesScanDllsInitDeserializedFromRootPath = this.StrategiesScanDlls_initDeserialized(this.RootPath);
			this.storeInScriptsInDlls(strategiesScanDllsInitDeserializedFromRootPath);
			Dictionary<Assembly, List<Script>> strategiesScanDllsInitDeserializedFromAppStartupPath = StrategiesScanDlls_initDeserialized(this.AppStartupPath);
			this.storeInScriptsInDlls(strategiesScanDllsInitDeserializedFromAppStartupPath);
		}
		void storeInScriptsInDlls(Dictionary<Assembly, List<Script>> strategiesInitedFromScannedDll) {
			foreach (Assembly assembly in strategiesInitedFromScannedDll.Keys) {
				AssemblyName name = assembly.GetName();
				string dllName = name.Name + ".dll";
				List<Script> scripts = strategiesInitedFromScannedDll[assembly];
				if (this.ScriptsInDlls.ContainsKey(dllName)) {
					string msg = "DUPLICATE_DLL_NAME_IN_APPROOT_AND_DATA_FOLDERS_RENAME_RESTART dllName[" + dllName + "]";
					Assembler.PopupException(msg);
					continue;
				}
				this.ScriptsInDlls.Add(dllName, scripts);
			}
		}

		protected Dictionary<string, List<Strategy>> StrategiesScanFolders() {
			var ret = new Dictionary<string, List<Strategy>>();
			string[] directories = Directory.GetDirectories(this.AbsPath);
			foreach (string subfolderAbsPath in directories) {
				string subfolderOnly = subfolderAbsPath.Substring(this.AbsPath.Length);
				if (ret.ContainsKey(subfolderOnly) == false) {
					ret.Add(subfolderOnly, new List<Strategy>());
				}
				string[] jsonFiles = Directory.GetFiles(subfolderAbsPath, "*.json");
				foreach (string strategyJsonAbsFile in jsonFiles) {
					try {
						Strategy strategy = this.StrategyDeserialize_fromJsonFile(strategyJsonAbsFile);
						strategy.StoredInJsonAbspath = strategyJsonAbsFile;
						ret[subfolderOnly].Add(strategy);
					} catch (Exception ex) {
						string msg = "STRATEGY_unJSON_FAILED: StrategyDeserializeFromJsonFile(" + strategyJsonAbsFile + ")";
						Assembler.PopupException(msg, ex);
//						var sample = new Strategy("sampleStrategy");
//						sample.ScriptSourceCode =
//							"namespace sample.Strategies {\r\n"
//								+ "public class MyScript : Script {\r\n"
//									+ "public override void ExecuteOnNewQuote(Quote quote) {\r\n"
//										+ "log(\"ExecuteOnNewQuote(): [\" + quote.ToString() + \"]\");\r\n"
//									+ "}\r\n"
//								+ "}\r\n"
//							+ "}\r\n";
//						this.StrategyAdd(sample, "sampleFolder");
					}
				}
			}
			return ret;
		}
		protected Dictionary<Assembly, List<Script>> StrategiesScanDlls_initDeserialized(string dataOrStartupPath) {
			RepositoryDllScripts repo = new RepositoryDllScripts(dataOrStartupPath);
			repo.ScanDlls();
			Dictionary<Assembly, List<Script>> ret = repo.TypesByAssemblies;
			foreach (Assembly assembly in ret.Keys) {
				List<Script> cloneableInstancesForAssembly = ret[assembly];
				// WONT_BE_EMPTY if (cloneableInstancesForAssembly.Count == 0) continue;
				string assemblyName = Path.GetFileName(assembly.Location);
				if (this.StrategiesInFolders.ContainsKey(assemblyName) == false) {
					//this.StrategiesInFolders.Add(assemblyName, new List<Strategy>());
					this.FolderAdd(assemblyName, false);
				//} else {
				//	string msg = "StrategyRepository::StrategiesScanDllsInitDeserialized(" + dataOrStartupPath + "):"
				//		+ " Assembly [" + assembly.Location + "] wasn't added to this.StrategiesInFolders"
				//		+ " because already existed among AllFoldersAvailable[" + AllFoldersAvailable + "]";
				//	Assembler.Constructed.StatusReporter.PopupException(msg);
				//	continue;
				}
				List<Strategy> strategiesForAssmbly = this.StrategiesInFolders[assemblyName];
				foreach (Script script in cloneableInstancesForAssembly) {
					string scriptName = script.GetType().Name;
					Strategy strategyFound = this.LookupByName(scriptName, assemblyName);
					if (strategyFound == null) {
						strategyFound = new Strategy(scriptName);
						strategyFound.StoredInJsonAbspath = Path.Combine(this.FolderAbsPathFor(assemblyName), scriptName + ".json");
						strategiesForAssmbly.Add(strategyFound);
					}
					strategyFound.Script = script;
					strategyFound.DllPathIfNoSourceCode = assembly.Location;
					// NO_POINT_FOR_SAVING_ONLY_StoredInJsonAbspath this.StrategySave(strategyFound);
				}
			}
			return ret;
		}
		Strategy StrategyDeserialize_fromJsonFile(string strategyJsonAbsFile) {
			Strategy ret = null;
			if (File.Exists(strategyJsonAbsFile) == false) return null;
			string json = File.ReadAllText(strategyJsonAbsFile);
			ret = JsonConvert.DeserializeObject<Strategy>(json, new JsonSerializerSettings {
				TypeNameHandling = TypeNameHandling.Objects});
			return ret;
		}
		public void StrategySerialize_toJsonFile(Strategy strategy, string strategyJsonAbsFile) {
			string json = JsonConvert.SerializeObject(strategy, Formatting.Indented, new JsonSerializerSettings {
				TypeNameHandling = TypeNameHandling.Objects});
			File.WriteAllText(strategyJsonAbsFile, json);
		}
		public Strategy LookupByName(string strategyName, string specificFolderToLook = null) {
			List<Strategy> whereToLook;
			if (String.IsNullOrEmpty(specificFolderToLook)) {
				whereToLook = this.AllStrategiesAvailable;
			} else {
				if (this.StrategiesInFolders.ContainsKey(specificFolderToLook)) {
					whereToLook = this.StrategiesInFolders[specificFolderToLook];
				} else {
					string msg = "LookupByName(" + strategyName + "):"
						+ " specificFolderToLook[" + specificFolderToLook + "] is not found"
						+ " among AllFoldersAvailable:[" + this.AllFoldersAvailable + "]";
					throw new Exception(msg);
				}
			}
			string strategyNameUpper = strategyName.ToUpper();
			foreach (Strategy strategy in whereToLook) {
				if (strategy.Name.ToUpper() != strategyNameUpper) continue;
				return strategy;
			}
			return null;
		}
		public Strategy LookupByGuid(string strategyGuid) {
			List<Strategy> whereToLook = this.AllStrategiesAvailable;
			Guid guid = new Guid(strategyGuid);
			foreach (Strategy strategy in whereToLook) {
				if (strategy.Guid != guid) continue;
				return strategy;
			}
			return null;
		}
		public void StrategySave(Strategy strategy, bool lastModIsNow = true, bool creating = false) {
			if (strategy.HasChartOnly) {
				throw new Exception("CREATE_CHART_CONTEXT_AND_SERIALIZE_IT_IN_CHART_FORM_DATASNAPSHOT");
				return;		//stub for a strategy-less chart
			}
			//if (lastModIsNow) strategy.LastModified = DateTime.Now;
			string strategyJsonAbsFile = this.StrategyJsonAbsNameFor(strategy);
			try {
				this.StrategySerialize_toJsonFile(strategy, strategy.StoredInJsonAbspath);
			} catch (Exception e) {
				string msg = "STRATEGY_SERIALIZE_JSON_FAILED: StrategySerializeToJsonFile(" + strategyJsonAbsFile + ")";
				throw new Exception(msg, e);
			}

			if (creating == false && this.LookupByName(strategy.Name) == null) {
				throw new Exception("StrategyFolder MUST have been already included in internal dictionnaries");
				//this.AllStrategiesAvailable.Add(strategy);
			}
		}
		public void FolderAdd(string folderName, bool throwWhenNonExistent = true) {
			string folderAbsPath = this.AbsPath + folderName;
			string msig = "StrategyRepository.FolderAdd(" + folderAbsPath + "): ";
			if (Directory.Exists(folderAbsPath)) {
				if (throwWhenNonExistent == false) return;
				string msg = "Folder already exists";
				throw new Exception(msig + msg);
			}
			try {
				Directory.CreateDirectory(folderAbsPath);
			} catch (Exception ex) {
				string msg = ex.Message;
				throw new Exception(msig + msg);
			}
			this.StrategiesInFolders.Add(folderName, new List<Strategy>());
		}
		public string FolderRename_modifyName_tillNoException(string folderNameFrom, string folderNameTo, bool creating = false) {
			if (this.FolderRename_isLikelyToSucceed(folderNameFrom, folderNameTo, creating) == true) {
				return folderNameTo;
			}
			int limit = 10;
			Exception lastException = null;
			for (int i = 1; i <= limit; i++) {
				string ret2 = folderNameTo + i;
				//if (FolderRenameIsLikelyToSucceed(folderNameFrom, folderNameTo) == true) return ret2;
				try {
					this.FolderRename_checkThrow(folderNameFrom, ret2, creating);
				} catch (Exception e) {
					lastException = e;
					continue;
				}
				return ret2;
			}
			//string msg = "strategies [" + folderNameTo + "] ... [" + (folderNameTo + limit) + "] already exist, can't generate name";
			//throw new Exception(msg, lastException);
			throw lastException;
		}
		public bool FolderRename_isLikelyToSucceed(string folderNameFrom, string folderNameTo, bool creating = false) {
			try {
				this.FolderRename_checkThrow(folderNameFrom, folderNameTo, creating);
			} catch (Exception e) {
				return false;
			}
			return true;
		}
		public void FolderRename_checkThrow(string folderNameFrom, string folderNameTo, bool creating = false) {
			string folderAbsPathFrom = this.AbsPath + folderNameFrom;
			string folderAbsPathTo = this.AbsPath + folderNameTo;
			string msig = "StrategyRepository.FolderRenameCheckThrow([" + folderNameFrom + "]=>[" + folderAbsPathTo + "]): ";

			if (folderNameFrom.Length > 4) {
				string substr = folderNameFrom.Substring(folderNameFrom.Length - 4);
				if (substr.ToUpper() == ".DLL") {
					string msg = "Adjust your GUI so that folder.ActivatedFromDll is not rename-able and its folder";
					throw new Exception(msig + msg);
				}
			}
			if (creating == false && this.StrategiesInFolders.ContainsKey(folderNameFrom) == false) {
				string msg = "FolderFrom is not in the this.StrategiesInFolders dictionary; re-read by StrategiesScanFoldersDlls()?";
				throw new Exception(msig + msg);
			}
			if (String.IsNullOrWhiteSpace(folderNameTo)) {
				string msg = "Destination folder IsNullOrWhiteSpace";
				throw new Exception(msig + msg);
			}
			if (creating == false && this.StrategiesInFolders.ContainsKey(folderNameTo) == true) {
				string msg = "folderNameTo is alreadyin the this.StrategiesInFolders dictionary; add '1' at the end";
				throw new Exception(msig + msg);
			}
			if (Directory.Exists(folderAbsPathTo)) {
				string msg = "Destination folder already exists";
				throw new Exception(msig + msg);
			}
		}
		public void FolderRename(string folderNameFrom, string folderNameTo) {
			string folderAbsPathFrom = this.AbsPath + folderNameFrom;
			string folderAbsPathTo = this.AbsPath + folderNameTo;
			string msig = "StrategyRepository.FolderRename([" + folderNameFrom + "]=>[" + folderAbsPathTo + "]): ";
			this.FolderRename_checkThrow(folderNameFrom, folderNameTo);
			try {
				DirectoryInfo directoryInfo = new DirectoryInfo(folderAbsPathFrom);
				directoryInfo.MoveTo(folderAbsPathTo);
			} catch (Exception ex) {
				string msg = ex.Message;
				throw new Exception(msig + msg);
			}
			this.StrategiesInFolders.Add(folderNameTo, this.StrategiesInFolders[folderNameFrom]);
			this.StrategiesInFolders.Remove(folderNameFrom);
		}
		public void FolderDelete(string folderName) {
			string toBeDeleted = this.AbsPath + folderName;
			string msig = "FolderDelete(" + toBeDeleted + "): ";

			if (folderName.Length > 4) {
				string substr = folderName.Substring(folderName.Length - 4);
				if (substr.ToUpper() == ".DLL") {
					string msg = "Adjust your GUI so that folder.ActivatedFromDll is not rename-able and its folder";
					throw new Exception(msig + msg);
				}
			}
			if (Directory.Exists(toBeDeleted) == false) {
				string msg = "Folder was (manually/externally) deleted from file system before you hit DELETE here";
				throw new Exception(msig + msg);
			}
			if (this.StrategiesInFolders.ContainsKey(folderName) == false) {
				string msg = "I don't have this folder even loaded; re-read by StrategiesScanFoldersDlls()?";
				//this.StrategiesScanFolders = this.StrategiesScanFolders();
				throw new Exception(msig + msg);
			}
			int count = this.StrategiesInFolders[folderName].Count;
			if (count > 0) {
				string msg = "Delete all " + count + " strategies first";
				throw new Exception(msig + msg);
			}

			DirectoryInfo directoryInfo = new DirectoryInfo(toBeDeleted);
			FileInfo[] files = directoryInfo.GetFiles(this.PathMask);
			if (files.Length > 0) {
				string msg = "";
				foreach (FileInfo fileInfo in files) {
					if (String.IsNullOrEmpty(msg) == false) msg += ",";
					msg += fileInfo.Name;
				}
				msg = "Can't delete folder: Some non-folder files left: [" + msg + "]";
				throw new Exception(msig + msg);
			}

			try {
				Directory.Delete(toBeDeleted);
			} catch (Exception e) {
				string msg = "Directory.Delete(" + toBeDeleted + "): System exception occured";
				throw new Exception(msig + msg, e);
			}

			this.StrategiesInFolders.Remove(folderName);
		}
		public void StrategyAdd(Strategy strategy, string folderName) {
			string msig = "StrategyAdd(" + strategy.Name + ", " + folderName + "): ";
			strategy.StoredInJsonAbspath = this.StrategyJsonAbsNameFor(strategy, null, folderName);

			string path = this.AbsPath + folderName + strategy.Name;
			if (File.Exists(path)) {
				string msg = "StrategyFolder already saved as [" + path + "]";
				throw new Exception(msig + msg);
			}
			var strategyFound = this.LookupByName(strategy.Name);
			if (strategyFound != null && strategyFound.StoredInJsonAbspath.ToUpper() == folderName.ToUpper()) {
				string msg = "StrategyFolder is not saved as [" + path + "] but exists in this.StrategiesInFolders as "
					+ "[" + strategyFound.StoredInJsonAbspath + Path.DirectorySeparatorChar + strategyFound.Name + "]";
				throw new Exception(msig + msg);
			}

			try {
				this.FolderAdd(folderName, false);
				if (this.StrategiesInFolders.ContainsKey(folderName) == false) {
					this.StrategiesInFolders.Add(folderName, new List<Strategy>());
				}
				if (this.StrategiesInFolders[folderName].Contains(strategy) == false) {
					this.StrategiesInFolders[folderName].Add(strategy);
				}
				this.StrategySave(strategy, false);
			} catch (Exception e) {
				string msg = "Permission denied in [" + strategy.StoredInJsonAbspath + "]?";
				throw new Exception(msig + msg, e);
			}
		}
		public void StrategyDelete(Strategy strategy) {
			string currentJsonAbspath = strategy.StoredInJsonAbspath;
			string msig = "StrategyDelete(" + currentJsonAbspath + "): ";
			this.currentJsonAbspathCheckThrow(strategy, msig);

			try {
				File.Delete(currentJsonAbspath);
			} catch (Exception e) {
				string msg = "Permission denied in [" + strategy.StoredInJsonAbspath + "]?";
				throw new Exception(msig + msg, e);
			}
			this.StrategiesInFolders[strategy.StoredInFolderRelName].Remove(strategy);
		}
		public void StrategyMoveToFolder(Strategy strategy, string folderTo) {
			string currentJsonAbspath = strategy.StoredInJsonAbspath;
			string folderFrom = strategy.StoredInFolderRelName;
			string msig = "StrategyMoveToFolder(" + currentJsonAbspath + "=>" + folderTo + "): ";

			string newJsonAbspath = this.StrategyJsonAbsNameFor(strategy, null, folderTo);
			strategy.StoredInJsonAbspath = newJsonAbspath;
			string strategyName = strategy.Name;
			strategy.Name = "$$$DUMMY_STRATEGY_NAME";
			strategy.Name = this.StrategyRenameModifyNameTillNoException(strategy, strategyName);
			strategy.StoredInJsonAbspath = this.StrategyJsonAbsNameFor(strategy, null, folderTo);
			//if (File.Exists(newJsonAbspath)) {
			//	string msg = "File.Exists(newJsonAbspath=" + newJsonAbspath + ") = true";
			//	throw new Exception(msig + msg);
			//}

			try {
				//this.currentJsonAbspathCheckThrow(strategy, msig, strategy.Name);
				File.Move(currentJsonAbspath, strategy.StoredInJsonAbspath);
			} catch (Exception e) {
				strategy.StoredInJsonAbspath = currentJsonAbspath;
				strategy.Name = strategyName;
				string msg = "Permission denied in [" + strategy.StoredInJsonAbspath + "]?";
				throw new Exception(msig + msg, e);
			}
			StrategiesInFolders[folderTo].Add(strategy);
			StrategiesInFolders[folderFrom].Remove(strategy);
		}
		public string StrategyRenameModifyNameTillNoException(Strategy strategy, string strategyNameTo) {
			if (StrategyRenameIsLikelyToSucceed(strategy, strategyNameTo) == true) {
				return strategyNameTo;
			}
			int limit = 10;
			Exception lastException = null;
			for (int i = 1; i <= limit; i++) {
				string ret2 = strategyNameTo + i;
				//if (StrategyRenameIsLikelyToSucceed(strategy, strategyNameTo) == true) return ret2;
				try {
					this.StrategyRenameCheckThrow(strategy, ret2);
				} catch (Exception e) {
					lastException = e;
					continue;
				}
				return ret2;
			}
			//string msg = "strategies [" + folderNameTo + "] ... [" + (folderNameTo + limit) + "] already exist, can't generate name";
			//throw new Exception(msg, lastException);
			throw lastException;
		}
		public bool StrategyRenameIsLikelyToSucceed(Strategy strategy, string strategyNameTo) {
			try {
				this.StrategyRenameCheckThrow(strategy, strategyNameTo);
			} catch (Exception e) {
				return false;
			}
			return true;
		}
		public void StrategyRenameCheckThrow(Strategy strategy, string strategyNameTo) {
			string msig = "StrategyRepository.StrategyRenameCheckThrow([" + strategy.Name + "]=>[" + strategyNameTo + "]): ";

			if (String.IsNullOrWhiteSpace(strategyNameTo)) {
				string msg = "Destination strategyNameTo IsNullOrWhiteSpace";
				throw new Exception(msig + msg);
			}
			var strategyNamesInThisFolder = new List<string>();
			foreach (Strategy strategySibling in this.StrategiesInFolders[strategy.StoredInFolderRelName]) {
				strategyNamesInThisFolder.Add(strategySibling.Name);
			}
			if (strategyNamesInThisFolder.Contains(strategyNameTo)) {
				string msg = "strategyNameTo is alreadyin the this.StrategiesInFolders["
					+ strategy.StoredInFolderRelName + "] list; add '1' at the end";
				throw new Exception(msig + msg);
			}
			string newJsonAbspath = this.StrategyJsonAbsNameFor(strategy, strategyNameTo);
			if (File.Exists(newJsonAbspath)) {
				string msg = "File.Exists(newJsonAbspath=" + newJsonAbspath + ") = true";
				throw new Exception(msig + msg);
			}
		}
		public void StrategyRename(Strategy strategy, string strategyNameTo) {
			string currentJsonAbspath = strategy.StoredInJsonAbspath;
			string msig = "StrategyRename(" + currentJsonAbspath + "=>" + strategyNameTo + "): ";
			this.currentJsonAbspathCheckThrow(strategy, msig);
			this.StrategyRenameCheckThrow(strategy, strategyNameTo);

			string newJsonAbspath = this.StrategyJsonAbsNameFor(strategy, strategyNameTo);
			try {
				File.Move(currentJsonAbspath, newJsonAbspath);
			} catch (Exception e) {
				string msg = "Permission denied in [" + strategy.StoredInJsonAbspath + "]?";
				throw new Exception(msig + msg, e);
			}
			strategy.Name = strategyNameTo;
			strategy.StoredInJsonAbspath = newJsonAbspath;
		}
		void currentJsonAbspathCheckThrow(Strategy strategy, string msig, string renamingTo = null) {
			if (renamingTo == null && strategy.StoredInJsonAbspath != this.StrategyJsonAbsNameFor(strategy)) {
				string msg = "folder.JsonAbsFile[" + strategy.StoredInJsonAbspath + "] != "
					+ "this.StrategyJsonAbsNameFor(" + strategy.Name + ")[" + this.StrategyJsonAbsNameFor(strategy) + "]";
				throw new Exception(msig + msg);
			}
			
			if (strategy.ActivatedFromDll == true) {
				string msg = "Adjust your GUI so that folder.ActivatedFromDll is not rename-able and its folder";
				throw new Exception(msig + msg);
			}

			if (this.StrategiesInFolders.ContainsKey(strategy.StoredInFolderRelName) == false) {
				string msg = "this.StrategiesInFolders should contain Folder[" + strategy.StoredInFolderRelName
					+ "] before manipulating; re-read by StrategiesScanFoldersDlls()?";
				throw new Exception(msig + msg);
			}
			if (this.StrategiesInFolders[strategy.StoredInFolderRelName].Contains(strategy) == false) {
				string msg = "this.StrategiesInFolders[" + strategy.StoredInFolderRelName + "] should contain StrategyFolder["
					+ strategy.Name + "] before manipulating; re-read by StrategiesScanFoldersDlls()?";
				throw new Exception(msig + msg);
			}
			string currentJsonAbspath = this.StrategyJsonAbsNameFor(strategy, renamingTo);
			if (File.Exists(currentJsonAbspath) == false) {
				string msg = "File.Exists(currentJsonAbspath=" + currentJsonAbspath + ") = false";
				throw new Exception(msig + msg);
			}

		}
		protected string FolderAbsPathFor(string folderName) {
			//if (folderName.EndsWith(Path.DirectorySeparatorChar) == false) folderName += Path.DirectorySeparatorChar;
			return Path.Combine(this.AbsPath, folderName);
		}
		public string StrategyJsonAbsNameFor(Strategy strategy, string newStrategyName = null, string newFolderName = null) {
			string jsonCurrentOrRenamed = (newStrategyName == null) ? strategy.Name : newStrategyName;
			string folderCurrentOrRenamed = (newFolderName == null) ? strategy.StoredInFolderRelName : newFolderName;
			return Path.Combine(this.FolderAbsPathFor(folderCurrentOrRenamed), jsonCurrentOrRenamed + ".json");
		}

		//public List<string> StrategyNamesInFolder(string folder) {
		//	List<string> ret = new List<string>();
		//	foreach (Strategy strategy in this.StrategiesInFolders[folder]) {
		//		ret.Add(strategy.Name);
		//	}
		//	return ret;
		//}

		public string GenerateStrategyName() {
			string ret = "NewStrategy";
			if (this.LookupByName(ret) == null) return ret;
			int limit = 100;
			for (int i=1; i<=limit; i++) {
				string ret2 = ret + i;
				if (this.LookupByName(ret2) == null) return ret2;
			}
			string msg = "strategies [" + ret + "] ... [" + (ret + limit) + "] already exist, can't generate name";
			throw new Exception(msg);
		}

		public string GenerateFolderName() {
			string ret = "NewFolder";
			return this.FolderRename_modifyName_tillNoException("DUMMY_FOLDER_NAME_TO_RENAME", ret, true);
		}

		public Strategy StrategyDuplicate(Strategy strategy, string strategyNameUserTyped = "") {
			//string nameToRestore = strategy.Name;
			Strategy strategyShallowCopy = strategy.CloneWithNewGuid();
			//strategyShallowCopy.Name = "$$$STRATEGY_DUNNY_NAME_TO_BE_RENAMED";
			//strategyShallowCopy.Name = this.StrategyRenameModifyNameTillNoException(strategyShallowCopy, strategy.Name);
			if (string.IsNullOrEmpty(strategyNameUserTyped)) strategyNameUserTyped = strategy.Name;
			strategyShallowCopy.Name = this.StrategyRenameModifyNameTillNoException(strategyShallowCopy, strategyNameUserTyped);
			this.StrategySave(strategyShallowCopy, true, true);
			Strategy strategyDeepCopy = this.StrategyDeserialize_fromJsonFile(strategyShallowCopy.StoredInJsonAbspath);
			this.StrategyAdd(strategyDeepCopy, strategyDeepCopy.StoredInFolderRelName);
			//strategy.Name = nameToRestore;
			return strategyDeepCopy;
		}

	}
}
