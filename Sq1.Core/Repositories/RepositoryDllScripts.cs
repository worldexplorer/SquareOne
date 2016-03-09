using System;

using Sq1.Core.StrategyBase;

namespace Sq1.Core.Repositories {
	public class RepositoryDllScripts : DllScannerAllInFolder<Script> {
		public RepositoryDllScripts(string rootPath) :
				base(rootPath) {
			base.ChildrenDebug_DllExpected = "Sql.Core.dll";
		}
	}
}
