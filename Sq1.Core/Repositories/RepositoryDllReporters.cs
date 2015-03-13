using System;

using Sq1.Core.StrategyBase;

namespace Sq1.Core.Repositories {
	public class RepositoryDllReporters : RepositoryDllScanner<Reporter> {
		// don't forget to initialize with RootPath!!!
		public RepositoryDllReporters() : base() {}
	}
}
