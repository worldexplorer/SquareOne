using System;

using Sq1.Core.StrategyBase;

namespace Sq1.Core.Repositories {
	public class RepositoryDllScripts : DllScannerAllInFolder<Script> {
		public RepositoryDllScripts(string rootPath) :
				base(rootPath) {
			base.ChildrenDebug_DllExpected = "Sq1.Strategies.Demo.dll";
		}
		protected override void ChildrenDebug_onTypesFoundInDll(string dllAbsPath, Type[] typesFoundInDll) {
			string msg = "breakpoint_here";
		}
		protected override void ChildrenDebug_TypeAdded(Type typeFound) {
			string msg = "breakpoint_here";
		}
	}
}
