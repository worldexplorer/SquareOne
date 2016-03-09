using System;

using Sq1.Core.StrategyBase;

namespace Sq1.Core.Repositories {
	public class RepositoryDllReporters : DllScannerExplicit<Reporter> {
		public RepositoryDllReporters(string rootPath) :
				base(rootPath,
					Assembler.InstanceUninitialized.AssemblerDataSnapshot.ReferencedNetAssemblyNames_Reporters) {
		}
	}
}
