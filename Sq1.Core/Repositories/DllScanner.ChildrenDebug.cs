using System;

namespace Sq1.Core.Repositories {
	public partial class DllScanner<T> {
		protected string ChildrenDebug_DllExpected;

		bool childrenDebug_DllNameMatches(string dllAbsPath) {
			if (string.IsNullOrEmpty(this.ChildrenDebug_DllExpected)) return false;
			if (dllAbsPath.ToUpper().Contains(this.ChildrenDebug_DllExpected.ToUpper()) == false) return false;
			return true;
		}



		protected void Invoke_ChildrenDebug_onDllMarkedAsSkipDll(string dllMarkedAsSkipDll) {
			if (this.childrenDebug_DllNameMatches(dllMarkedAsSkipDll) == false) return;
			this.ChildrenDebug_onDllMarkedAsSkipDll(dllMarkedAsSkipDll);
		}
		protected void Invoke_ChildrenDebug_onExtraDllFoundButNotExplicitlyNeeded(string dllFoundButNotExplicitlyNeeded) {
			if (this.childrenDebug_DllNameMatches(dllFoundButNotExplicitlyNeeded) == false) return;
			this.ChildrenDebug_onFoundButNotExplicitlyNeeded(dllFoundButNotExplicitlyNeeded);
		}
		protected void Invoke_ChildrenDebug_onDllDoesntExistInFolder(string dllAbsPath) {
			if (this.childrenDebug_DllNameMatches(dllAbsPath) == false) return;
			this.ChildrenDebug_onDllDoesntExistInFolder();
		}		
		protected void Invoke_ChildrenDebug_onTypesFoundInDll(string dllAbsPath, Type[] typesFoundInDll) {
			if (this.childrenDebug_DllNameMatches(dllAbsPath) == false) return;
			this.ChildrenDebug_onTypesFoundInDll(dllAbsPath, typesFoundInDll);
		}
		protected void Invoke_ChildrenDebug_TypeAdded(string dllAbsPath, Type typeFound) {
			if (this.childrenDebug_DllNameMatches(dllAbsPath) == false) return;
			this.Invoke_ChildrenDebug_TypeAdded(dllAbsPath, typeFound);
		}
		protected void Invoke_ChildrenDebug_InstantiableInstanceForAssemblyAdded(string dllAbsPath, T classCastedInstance) {
			if (this.childrenDebug_DllNameMatches(dllAbsPath) == false) return;
			this.ChildrenDebug_InstantiableInstanceForAssemblyAdded(classCastedInstance);
		}
		protected void Invoke_ChildrenDebug_InstantiableInstanceByClassNameAdded(string dllAbsPath, string className, T classCastedInstance) {
			if (this.childrenDebug_DllNameMatches(dllAbsPath) == false) return;
			this.ChildrenDebug_InstantiableInstanceByClassNameAdded(className, classCastedInstance);
		}



		protected virtual void ChildrenDebug_onDllDoesntExistInFolder() {
			string msg = "breakpoint_here";
		}
		protected virtual void ChildrenDebug_onDllMarkedAsSkipDll(string dllMarkedAsSkipDll) {
			string msg = "breakpoint_here";
		}
		protected virtual void ChildrenDebug_onFoundButNotExplicitlyNeeded(string dllMarkedAsSkipDll) {
			string msg = "breakpoint_here";
		}
		protected virtual void ChildrenDebug_onTypesFoundInDll(string dllAbsPath, Type[] typesFoundInDll) {
			string msg = "breakpoint_here";
		}
		protected virtual void ChildrenDebug_TypeAdded(Type typeFound) {
			string msg = "breakpoint_here";
		}
		protected virtual void ChildrenDebug_InstantiableInstanceForAssemblyAdded(T classCastedInstance) {
			string msg = "breakpoint_here";
		}
		protected virtual void ChildrenDebug_InstantiableInstanceByClassNameAdded(string className, T classCastedInstance) {
			string msg = "breakpoint_here";
		}
		
	}
}
