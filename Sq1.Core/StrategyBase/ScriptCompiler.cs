using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using Microsoft.CSharp;

namespace Sq1.Core.StrategyBase {
	public class ScriptCompiler {
		public CompilerErrorCollection CompilerErrors;
		public string TempFolderInAppData;
		public string TempFolderAbsPath { get {
			string ret = null;
			if (string.IsNullOrEmpty(this.TempFolderInAppData)) return ret;
			ret = Path.Combine(Assembler.InstanceInitialized.RepositoryDllJsonStrategy.RootPath, this.TempFolderInAppData);
			return ret;
		} }
		object avoidingMessIfInstantiatedInAssembler;

		public ScriptCompiler() {
			avoidingMessIfInstantiatedInAssembler = new object();
		}

		//NOT USED
		private ScriptCompiler(string tmpFolder = "_StrategiesCompiledOnTheFlyInsecure") : this() {

			//DISABLED_SINCE_DOESNT_HELP_FOR_EXCEPTIONS_TO_HAVE_LINE_NUMBER_IN_EXCEPTIONS_CONTROL
			tmpFolder = null;

			TempFolderInAppData = tmpFolder;
			if (string.IsNullOrWhiteSpace(this.TempFolderInAppData)) return;
			bool attemptedToCreateDirectory = false;
			try {
				if (Directory.Exists(this.TempFolderAbsPath) == false) {
					attemptedToCreateDirectory = true;
					Directory.CreateDirectory(this.TempFolderAbsPath);
				}
			} catch (Exception ex) {
				string msg = "I_CANNOT_CREATE_TEMP_DIRECTORY_CHECK_PERMISSIONS_AND_DISK_QUOTE tmpAbsPath["
					+ this.TempFolderAbsPath + "]";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg + " ScriptCompiler.CompileSourceReturnInstance()", ex);
			}
			if (Directory.Exists(this.TempFolderAbsPath) == false) {
				string msg = "TEMP_DIRECTORY_DOES_NOT_EXISTS attemptedToCreateDirectory["
					+ attemptedToCreateDirectory + "] tmpAbsPath[" + this.TempFolderAbsPath + "]";
				#if DEBUG
				Debugger.Break();
				#endif
				throw new Exception(msg + " ScriptCompiler.CompileSourceReturnInstance()");
			}
			this.CleanTempFolder();
		}

		public int CleanTempFolder(bool keepDllsUndeleted = false) {
			int ret = 0;
			DirectoryInfo directoryInfo = new DirectoryInfo(this.TempFolderAbsPath);
			FileInfo[] allFoundFiles = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in allFoundFiles) {
				if (keepDllsUndeleted == true && fileInfo.Name.ToUpper().EndsWith(".DLL")) {
					continue;
				}
				File.Delete(fileInfo.FullName);
				ret++;
			}
			return ret;
		}

		public Script CompileSourceReturnInstance(string sourceCode, string dotNetReferences) { lock (this.avoidingMessIfInstantiatedInAssembler) {
			CodeDomProvider codeDomProvider = new CSharpCodeProvider();
			CompilerParameters compilerParameters = new CompilerParameters();
			compilerParameters.GenerateExecutable = false;
			compilerParameters.GenerateInMemory = true;
			if (string.IsNullOrWhiteSpace(this.TempFolderInAppData) == false) {
				// http://stackoverflow.com/questions/875723/how-to-debug-break-in-codedom-compiled-code
				compilerParameters.GenerateInMemory = false;
				string tmpAbsPath = Path.Combine(Assembler.InstanceInitialized.RepositoryDllJsonStrategy.RootPath, this.TempFolderInAppData);
				compilerParameters.TempFiles = new TempFileCollection(tmpAbsPath, true);
				compilerParameters.IncludeDebugInformation = true;
			}
			compilerParameters.IncludeDebugInformation = false;
			compilerParameters.ReferencedAssemblies.Add("System.dll");
			compilerParameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
			compilerParameters.ReferencedAssemblies.Add("System.Drawing.dll");
			this.addApplicationAssemblies(compilerParameters);
			if (string.IsNullOrEmpty(dotNetReferences) == false) {
				string[] referencesSplitted = dotNetReferences.Split(new char[] { ';' });
				foreach (string reference in referencesSplitted) {
					string referenceTrimmed = reference.Trim();
					if (referenceTrimmed != "") {
						if (referenceTrimmed.ToUpper().EndsWith(".DLL") == false) {
							referenceTrimmed += ".dll";
						}
						if (compilerParameters.ReferencedAssemblies.Contains(referenceTrimmed) == false) {
							compilerParameters.ReferencedAssemblies.Add(referenceTrimmed);
						}
					}
				}
			}
			CompilerResults compilerResults = codeDomProvider.CompileAssemblyFromSource(compilerParameters, sourceCode);
			this.CompilerErrors = compilerResults.Errors;
			if (compilerResults.Errors.HasErrors) {
				return null;
			}
			Assembly compiledAssembly = compilerResults.CompiledAssembly;
			Script result = null;
			Type[] types = compiledAssembly.GetTypes();
			foreach (Type type in types) {
				if (type.IsAbstract) continue;
				if (type.BaseType == null) continue;
				//&& (type.BaseType.Name == "ScriptBase" || type.BaseType.Name == "ScriptBaseTL")) {
				if (type.BaseType.Name != typeof(Script).Name) continue;
				object typeInstance = Activator.CreateInstance(type);
				result = typeInstance as Script;
				if (result == null) {
					string msg = "Activator.CreateInstance(" + type + ") as Script == null";
					throw new Exception();
				}
				break;
			}
			return result;
		} }
		void addApplicationAssemblies(CompilerParameters compilerParameters) {
			//Debugger.Break();	//TESTED
			List<string> dllsFound = new List<string>();
			List<Assembly> assembliesFound = new List<Assembly>();
			DirectoryInfo directoryInfo = new DirectoryInfo(Assembler.InstanceInitialized.AppStartupPath);		//Application.ExecutablePath
			FileInfo[] dllsAllFoundInFolder = directoryInfo.GetFiles("*.dll");
			foreach (FileInfo fileInfo in dllsAllFoundInFolder) {
				string dllAbsPath = Path.Combine(directoryInfo.FullName, fileInfo.Name);
				Assembly assembly;
				try {
					assembly = Assembly.LoadFile(dllAbsPath);
					if (assembly == null) continue;
					assembliesFound.Add(assembly);
					dllsFound.Add(fileInfo.Name);
				} catch (Exception ex) {
					string msg = "ASSEMBLY_NOT_FOUND_IN_APP_FOLDER Assembly.LoadFile(" + dllAbsPath + ")";
					Assembler.PopupException(msg, ex);
					throw ex;
				}
			}
			foreach (string dll in dllsFound) {
				if (compilerParameters.ReferencedAssemblies.Contains(dll)) continue;
				compilerParameters.ReferencedAssemblies.Add(dll);
			}
		}
	}
}
