using System;

namespace Sq1.Core.Repositories {
	public partial class RepositoryDllScanner<T> {
		protected string ChildrenDebug_DllExpected;

		bool childrenDebug_DllNameMatches(string dllAbsPath) {
			if (string.IsNullOrEmpty(this.ChildrenDebug_DllExpected)) return false;
			if (dllAbsPath.ToUpper().Contains(this.ChildrenDebug_DllExpected.ToUpper()) == false) return false;
			return true;
		}



		void invoke_ChildrenDebug_onDllMarkedAsSkipDll(string dllMarkedAsSkipDll) {
			if (this.childrenDebug_DllNameMatches(dllMarkedAsSkipDll) == false) return;
			this.ChildrenDebug_onDllMarkedAsSkipDll(dllMarkedAsSkipDll);
		}
		void invoke_ChildrenDebug_onDllDoesntExistInFolder(string dllAbsPath) {
			if (this.childrenDebug_DllNameMatches(dllAbsPath) == false) return;
			this.ChildrenDebug_onDllDoesntExistInFolder();
		}		
		void invoke_ChildrenDebug_onTypesFoundInDll(string dllAbsPath, Type[] typesFoundInDll) {
			if (this.childrenDebug_DllNameMatches(dllAbsPath) == false) return;
			this.ChildrenDebug_onTypesFoundInDll(dllAbsPath, typesFoundInDll);
		}
		void invoke_ChildrenDebug_TypeAdded(string dllAbsPath, Type typeFound) {
			if (this.childrenDebug_DllNameMatches(dllAbsPath) == false) return;
			this.invoke_ChildrenDebug_TypeAdded(dllAbsPath, typeFound);
		}
		void invoke_ChildrenDebug_CloneableInstanceForAssemblyAdded(string dllAbsPath, T classCastedInstance) {
			if (this.childrenDebug_DllNameMatches(dllAbsPath) == false) return;
			this.ChildrenDebug_CloneableInstanceForAssemblyAdded(classCastedInstance);
		}
		void invoke_ChildrenDebug_CloneableInstanceByClassNameAdded(string dllAbsPath, string className, T classCastedInstance) {
			if (this.childrenDebug_DllNameMatches(dllAbsPath) == false) return;
			this.ChildrenDebug_CloneableInstanceByClassNameAdded(className, classCastedInstance);
		}



		protected virtual void ChildrenDebug_onDllDoesntExistInFolder() {
			string msg = "breakpoint_here";
		}
		protected virtual void ChildrenDebug_onDllMarkedAsSkipDll(string dllMarkedAsSkipDll) {
			string msg = "breakpoint_here";
		}
		protected virtual void ChildrenDebug_onTypesFoundInDll(string dllAbsPath, Type[] typesFoundInDll) {
			string msg = "breakpoint_here";
		}
		protected virtual void ChildrenDebug_TypeAdded(Type typeFound) {
			string msg = "breakpoint_here";
		}
		protected virtual void ChildrenDebug_CloneableInstanceForAssemblyAdded(T classCastedInstance) {
			string msg = "breakpoint_here";
		}
		protected virtual void ChildrenDebug_CloneableInstanceByClassNameAdded(string className, T classCastedInstance) {
			string msg = "breakpoint_here";
		}
		
	}
}
