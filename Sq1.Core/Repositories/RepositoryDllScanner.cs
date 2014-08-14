using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Sq1.Core.Support;

namespace Sq1.Core.Repositories {
	public class RepositoryDllScanner<T> {
		public List<Type> TypesFound;
		public Dictionary<string, T> CloneableInstanceByClassName;
		public Dictionary<Assembly, List<T>> CloneableInstancesByAssemblies;
		public List<string> NonEmptyDllsScanned;
		public string NonEmptyDllsScannedAsString {
			get {
				string ret = "";
				foreach (string dllPath in NonEmptyDllsScanned) {
					ret += "{" + dllPath + "}, ";
				}
				ret = ret.TrimEnd(", ".ToCharArray());
				return "[" + ret + "]";
			}
		}
		public List<Exception> ExceptionsWhileScanning;
		public string RootPath { get; protected set; }
		public string Subfolder;
		public string PathMask;
		public string AbsPath { get { return this.RootPath + Subfolder; } }
		public List<string> skipDlls = new List<string>() {
				"CsvHelper.dll", "DigitalRune.Windows.TextEditor.dll", "NDde.dll", "Newtonsoft.Json.dll",		//"log4net.dll", 
				"ObjectListView.dll", "Sq1.Charting.dll", "Sq1.Core.dll", "Sq1.Gui.dll", "Sq1.Widgets.dll",
				"WeifenLuo.WinFormsUI.Docking.dll"};

		public RepositoryDllScanner() {
			this.TypesFound = new List<Type>();
			this.CloneableInstanceByClassName = new Dictionary<string, T>();
			this.CloneableInstancesByAssemblies = new Dictionary<Assembly, List<T>>();
			this.NonEmptyDllsScanned = new List<string>();
			this.ExceptionsWhileScanning = new List<Exception>();
			this.Subfolder = "";
			this.PathMask = "*.dll";
		}
		public void InitializeAndScan(string rootPath, bool denyDuplicateShortNamesAndPreInstantiateToClone = true) {
			this.RootPath = rootPath;
			//if (this.RootPath.EndsWith(Path.DirectorySeparatorChar) == false) this.RootPath += Path.DirectorySeparatorChar;
			this.ScanDlls(denyDuplicateShortNamesAndPreInstantiateToClone);
		}
		public FileInfo[] DllsFoundMinusSkip(FileInfo[] foundInFolder) {
			List<FileInfo> ret = new List<FileInfo>();
			foreach (FileInfo fileInfo in foundInFolder) {
				if (skipDlls.Contains(fileInfo.Name)) continue;
				ret.Add(fileInfo);
			}
			return ret.ToArray();
		}
		public Dictionary<string, T> ScanDlls(bool denyDuplicateShortNamesAndPreInstantiateToClone = true) {
			Dictionary<string, T> ret = new Dictionary<string, T>();
			if (Directory.Exists(this.AbsPath) == false) return ret;
			DirectoryInfo directoryInfo = new DirectoryInfo(this.AbsPath);
			FileInfo[] dllsAllFoundInFolder = directoryInfo.GetFiles(this.PathMask);
			FileInfo[] dllsFiltered = DllsFoundMinusSkip(dllsAllFoundInFolder);
			foreach (FileInfo fileInfo in dllsFiltered) {
				//ALREADY_dllsFiltered if (skipDlls.Contains(fileInfo.Name)) continue;
				Type[] types;
				string dllAbsPath = Path.Combine(directoryInfo.FullName, fileInfo.Name);
				Assembly assembly;
				try {
					assembly = Assembly.LoadFile(dllAbsPath);
					if (assembly == null) continue;
					types = assembly.GetTypes();
					if (types == null) continue;
				} catch (ReflectionTypeLoadException ex) {
					foreach (var loaderEx in ex.LoaderExceptions) {
						// debugger-friendly
						string msg = loaderEx.ToString();
						// add topStack and continue
						this.ExceptionsWhileScanning.Add(loaderEx);
						break;
					}
					continue;
				} catch (Exception e) {
					this.ExceptionsWhileScanning.Add(e);
					continue;
				}
				this.NonEmptyDllsScanned.Add(dllAbsPath);
				foreach (Type type in types) {
					if (typeof(T).IsAssignableFrom(type) == false) continue;
					if (type.FullName == typeof(T).FullName) continue;
					if (denyDuplicateShortNamesAndPreInstantiateToClone == true) {
						bool wannaAvoidCloneableInsancesToThrow = this.checkShortNameAlreadyFound(type.Name);
						if (wannaAvoidCloneableInsancesToThrow) continue;
					}
					this.TypesFound.Add(type);

					if (denyDuplicateShortNamesAndPreInstantiateToClone == false) continue;
					if (SkipInstantiationAtAttribute.AtStartup(type) == true) continue;
					try {
						object instance = Activator.CreateInstance(type);	// Type.Assembly points to where the Type "lives"
						if (instance == null) continue;
						T classCastedInstance = (T)instance;
						//if (classCastedInstance.Name == null) continue;
						//this.CloneableInstanceByClassName.Add(classCastedInstance.Name, classCastedInstance);
						this.CloneableInstanceByClassName.Add(type.Name, classCastedInstance);

						if (this.CloneableInstancesByAssemblies.ContainsKey(assembly) == false) {
							this.CloneableInstancesByAssemblies.Add(assembly, new List<T>());
						}
						List<T> cloneableInstancesForAssembly = this.CloneableInstancesByAssemblies[assembly];
						cloneableInstancesForAssembly.Add(classCastedInstance);
					} catch (Exception ex) {
						string msig = "RepositoryDllScanner<" + typeof(T) + ">.ScanDlls(" + denyDuplicateShortNamesAndPreInstantiateToClone
							+ ", [" + dllAbsPath + "]): Activator.CreateInstance(" + type + "): ";
						string msg = "activation failed, or the instance can not be casted, or the duplicate found in another DLL";
						Exception dontThrowButLog = new Exception(msig + msg, ex);
						this.ExceptionsWhileScanning.Add(dontThrowButLog);
						continue;
					}
				}
			}
			return ret;
		}

		protected bool checkShortNameAlreadyFound(string typeNameShort) {
			List<Type> alreadyFound = this.FindTypesByShortName(typeNameShort);
			if (alreadyFound.Count == 0) return false;
			string dllNames = this.dllNamesForTypes(alreadyFound);
			string msg = "typeNameShort[" + typeNameShort + "] has duplicates "
				+ " containing in dllNames[" + dllNames + "]";
			this.ExceptionsWhileScanning.Add(new Exception(msg));
			return true;
		}
		protected string dllNamesForTypes(List<Type> types) {
			string ret = "";
			foreach (Type type in types) {
				ret += "{" + type.FullName + ":" + Path.GetFileName(type.Assembly.Location) + "}, ";
			}
			ret = ret.TrimEnd(", ".ToCharArray());
			return "[" + ret + "]";
		}
		[Obsolete]
		public T FindInstantiatedForCloning(string className) {
			T ret = default(T);
			if (this.CloneableInstanceByClassName.ContainsKey(className)) {
				ret = this.CloneableInstanceByClassName[className];
			}
			return ret;
		}
		public Type FindTypeByFullName(string fullTypeName) {
			Type ret = null;
			foreach (Type type in this.TypesFound) {
				if (type.FullName != fullTypeName) continue;
				if (ret == type) {
					string msg = "duplicate fullTypeName found... did you have a copy of a DLL?...";
					throw new Exception(msg);
				}
				ret = type;
				//break;	// commented out to let duplicate check 3 (lines above) work
			}
			return ret;
		}
		public T ActivateFromTypeName(string typeNameShortOrFullAutodetect) {
			T ret;
			if (typeNameShortOrFullAutodetect.Contains(".")) {
				ret = this.ActivateFromFullTypeName(typeNameShortOrFullAutodetect);
			} else {
				ret = this.ActivateFromShortTypeName(typeNameShortOrFullAutodetect);
			}
			return ret;
		}

		public T ActivateFromFullTypeName(string typeNameFull) {
			Type uniqueType = this.FindTypeByFullName(typeNameFull);
			if (uniqueType == null) {
				string msg = "ActivateFromFullTypeName(): typeNameFull[" + typeNameFull + "] doesn't exist among TypesFound["
					+ this.dllNamesForTypes(this.TypesFound) + "]";
				throw new Exception(msg);
			}
			object instance = Activator.CreateInstance(uniqueType);	// Type.Assembly points to where the Type "lives"
			T ret = (T)instance;
			return ret;
		}
		public T ActivateFromShortTypeName(string typeNameShort) {
			List<Type> uniqueType = this.FindTypesByShortName(typeNameShort);
			if (uniqueType.Count == 0) {
				string msg = "ActivateFromShortTypeName(): typeNameShort[" + typeNameShort + "] doesn't exist among TypesFound["
					+ this.dllNamesForTypes(this.TypesFound) + "]";
				throw new Exception(msg);
			}
			if (uniqueType.Count > 1) {
				string msg = "ActivateFromShortTypeName(): more than 1 typeNameShort[" + typeNameShort + "] exists among TypesFound["
					+ this.dllNamesForTypes(this.TypesFound) + "]";
				throw new Exception(msg);
			}

			object instance = Activator.CreateInstance(uniqueType[0]);	// Type.Assembly points to where the Type "lives"
			// there must be no exceptions; let it throw if DLL was deleted
			T ret = (T)instance;
			return ret;
		}
		public List<Type> FindTypesByShortName(string typeNameShortToFind) {
			List<Type> ret = new List<Type>();
			foreach (Type type in this.TypesFound) {
				if (type.Name != typeNameShortToFind) continue;
				ret.Add(type);
			}
			return ret;
		}
		public string ShrinkTypeName(string typeNameShortOrFullAutodetect) {
			string typeNameShort = typeNameShortOrFullAutodetect;
			int lastIndexDot = typeNameShortOrFullAutodetect.LastIndexOf('.');
			//The method returns -1 if the character or string is not found in this instance
			if (lastIndexDot != -1) {
				int indexAfterDot = lastIndexDot + 1;
				int fromDotTillEnd = typeNameShortOrFullAutodetect.Length - indexAfterDot;
				typeNameShort = typeNameShortOrFullAutodetect.Substring(indexAfterDot, fromDotTillEnd);
			}
			return typeNameShort;
		}
	}
}
