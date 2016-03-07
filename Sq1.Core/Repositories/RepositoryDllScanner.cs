using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Sq1.Core.Support;

namespace Sq1.Core.Repositories {
	public partial class RepositoryDllScanner<T> {
		public string OfWhat { get { return typeof(T).Name; } }
		
		public List<Type>						TypesFound;
		public Dictionary<string, T>			CloneableInstanceByClassName;
		public Dictionary<Assembly, List<T>>	CloneableInstancesByAssemblies;
		public string							AssembliesWithCloneableTypesFoundAsString { get {
				string ret = "";
				foreach (Assembly asm in this.CloneableInstancesByAssemblies.Keys) {
					ret += "{" + Path.GetFileName(asm.Location) + "}, ";
				}
				ret = ret.TrimEnd(", ".ToCharArray());
				return "[" + ret + "]";
			} }

		public List<string>						NonEmptyDllsScanned;
		public string							NonEmptyDllsScannedAsString { get {
				string ret = "";
				foreach (string dllPath in this.NonEmptyDllsScanned) {
					ret += "{" + Path.GetFileName(dllPath) + "}, ";
				}
				ret = ret.TrimEnd(", ".ToCharArray());
				return "[" + ret + "]";
			} }
		public List<Exception>					ExceptionsWhileScanning;
		public string							RootPath	{ get; private set; }
		public string							Subfolder	{ get; private set; }
		public string							PathMask	{ get; private set; }
		public string							AbsPath		{ get { return this.RootPath + Subfolder; } }
		public List<string>						skipDlls = new List<string>() {
				"CsvHelper35.dll", "DigitalRune.Windows.TextEditor.dll", "NDde.dll", "Newtonsoft.Json.dll",		//"log4net.dll", 
				"ObjectListView.dll", "WeifenLuo.WinFormsUI.Docking.dll", "Sq1.Charting.dll", "Sq1.Gui.dll", "Sq1.Widgets.dll",
				"TRANS2QUIK.dll"
				// I_WANT_LIVESIM_STREAMING_BROKER_BE_AUTOASSIGNED_AND_VISIBLE_IN_DATASOURCE_EDITOR, "Sq1.Core.dll"
				};
		private bool							includeGrandChildren;

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
		public FileInfo[] DllsFoundMinusSkip(DirectoryInfo rootFolder, FileInfo[] dllsAllFoundInRootFolder) {
			List<FileInfo> ret = new List<FileInfo>();
			foreach (FileInfo fileInfo in dllsAllFoundInRootFolder) {
				if (skipDlls.Contains(fileInfo.Name)) {
					#if DEBUG
					this.invoke_ChildrenDebug_onDllMarkedAsSkipDll(fileInfo.Name);
					#endif
					continue;
				}
				#if DEBUG
				string dllAbsPath = Path.Combine(rootFolder.FullName, fileInfo.Name);
				this.invoke_ChildrenDebug_onDllDoesntExistInFolder(dllAbsPath);
				#endif
				ret.Add(fileInfo);
			}
			return ret.ToArray();
		}
		public Dictionary<string, T> ScanDlls(bool denyDuplicateShortNames = true) {
			Dictionary<string, T> ret = new Dictionary<string, T>();
			if (Directory.Exists(this.AbsPath) == false) return ret;
			DirectoryInfo rootFolder = new DirectoryInfo(this.AbsPath);
			FileInfo[] dllsAllFoundInRootFolder = rootFolder.GetFiles(this.PathMask);
			FileInfo[] dllsFiltered = this.DllsFoundMinusSkip(rootFolder, dllsAllFoundInRootFolder);
			foreach (FileInfo fileInfo in dllsFiltered) {
				//ALREADY_dllsFiltered if (skipDlls.Contains(fileInfo.Name)) continue;
				Type[] typesFoundInDll;
				string dllAbsPath = Path.Combine(rootFolder.FullName, fileInfo.Name);
				Assembly assembly;
				string msig = " Assembly.LoadFile(" + dllAbsPath + ") << RepositoryDllScanner<" + typeof(T) + ">.ScanDlls()";
				try {
					assembly = Assembly.LoadFile(dllAbsPath);
					if (assembly == null) continue;
					typesFoundInDll = assembly.GetTypes();
#if DEBUG
					this.invoke_ChildrenDebug_onTypesFoundInDll(dllAbsPath, typesFoundInDll);
#endif
					if (typesFoundInDll == null) continue;
				} catch (ReflectionTypeLoadException ex) {
					foreach (var loaderEx in ex.LoaderExceptions) {
						string msg = "ReflectionTypeLoadException/LOADFILE_FAILED";
						// add topStack and continue
						//this.ExceptionsWhileScanning.Add(loaderEx);
						Assembler.PopupException(msg + msig, loaderEx);
						break;
					}
					continue;
				} catch (Exception ex) {
					//this.ExceptionsWhileScanning.Add(ex);
					string msg = "Exception/LOADFILE_FAILED";
					Assembler.PopupException(msg + msig, ex);
					continue;
				}
				this.NonEmptyDllsScanned.Add(dllAbsPath);
				foreach (Type typeFound in typesFoundInDll) {
					msig = " Activator.CreateInstance(" + typeFound + ") << RepositoryDllScanner<" + typeof(T) + ">.ScanDlls("
						+ denyDuplicateShortNames + ", [" + dllAbsPath + "])";
						
					string typeFoundName = typeFound.FullName;
					string typeMustBeParent = typeof(T).FullName;

					if (typeFound.Name == "QuikLivesimStreaming"
							&& typeMustBeParent == "Sq1.Core.Streaming.StreamingAdapter"
							//&& typeMustBeParent.Contains("StreamingAdapter")
						) {
						string breakpoint = "here";
					}
					bool isAssignable = typeof(T).IsAssignableFrom(typeFound);
					if (isAssignable == false) {
						string msg = "typeFoundName[" + typeFoundName+ "].IsNotValidTORepresent[" + typeMustBeParent + "]";
						continue;
					}

					bool imInstantiatingTheParent_itMustBeAbstract = typeFoundName == typeMustBeParent;
					if (imInstantiatingTheParent_itMustBeAbstract) continue;
		
					if (denyDuplicateShortNames == true) {
						bool wannaAvoidCloneableInsancesToThrow = this.checkShortNameAlreadyFound(typeFound.Name);
						if (wannaAvoidCloneableInsancesToThrow) continue;
					}
					this.TypesFound.Add(typeFound);
#if DEBUG
					this.invoke_ChildrenDebug_TypeAdded(dllAbsPath, typeFound);
#endif

					//if (denyDuplicateShortNames == false) continue;
					if (SkipInstantiationAtAttribute.AtStartup(typeFound) == true) continue;
					
					// 1/3
					object instance;
					try {
						instance = Activator.CreateInstance(typeFound);	// Type.Assembly points to where the Type "lives"
						if (instance == null) throw new Exception("INSTANCE_RETURNED_IS_NULL");
					} catch (Exception ex) {
						string msg = "ACTIVATION_FAILED";
						Assembler.PopupException(msg + msig, ex);
						continue;
					}
					
					// 2/3
					T classCastedInstance;
					try {
						classCastedInstance = (T)instance;
						//if (classCastedInstance.Name == null) continue;
						if (classCastedInstance == null) throw new Exception("CLASSCASTEDINSTANCE_IS_NULL");
					} catch (Exception ex) {
						string msg = "INSTANCE_CANT_BE_CASTED";	 //this check is redundant due to if (typeof(T).IsAssignableFrom(type) == false) continue;
						Assembler.PopupException(msg + msig, ex);
						continue;
					}
					
					// 3/3
					try {
						//this.CloneableInstanceByClassName.Add(classCastedInstance.Name, classCastedInstance);
						this.CloneableInstanceByClassName.Add(typeFound.Name, classCastedInstance);
#if DEBUG
						this.invoke_ChildrenDebug_CloneableInstanceByClassNameAdded(dllAbsPath, typeFound.Name, classCastedInstance);
#endif

						if (this.CloneableInstancesByAssemblies.ContainsKey(assembly) == false) {
							this.CloneableInstancesByAssemblies.Add(assembly, new List<T>());
						}
						List<T> cloneableInstancesForAssembly = this.CloneableInstancesByAssemblies[assembly];
						cloneableInstancesForAssembly.Add(classCastedInstance);
#if DEBUG
						this.invoke_ChildrenDebug_CloneableInstanceForAssemblyAdded(dllAbsPath, classCastedInstance);
#endif
					} catch (Exception ex) {
						string msg = "DUPLICATE_FOUND_IN_ANOTHER_DLL";
						Assembler.PopupException(msg + msig, ex);
						continue;
					}
				}
			}
			return ret;
		}

		bool checkShortNameAlreadyFound(string typeNameShort) {
			List<Type> alreadyFound = this.FindTypesByShortName(typeNameShort);
			if (alreadyFound.Count == 0) return false;
			string dllNames = this.dllNamesForTypes(alreadyFound);
			string msg = "typeNameShort[" + typeNameShort + "] has duplicates "
				+ " containing in dllNames[" + dllNames + "]";
			//this.ExceptionsWhileScanning.Add(new Exception(msg));
			Assembler.PopupException(msg);
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
		public override string ToString() {
			return "RepositoryDllScanner<" + this.OfWhat + ">(" + this.RootPath + "): " + this.AssembliesWithCloneableTypesFoundAsString;
		}
	}
}
