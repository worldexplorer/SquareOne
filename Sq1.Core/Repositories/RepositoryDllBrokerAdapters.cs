using System;

using Sq1.Core.Broker;

namespace Sq1.Core.Repositories {
	public class RepositoryDllBrokerAdapters : DllScannerExplicit<BrokerAdapter> {
		public RepositoryDllBrokerAdapters(string rootPath) :
				base(rootPath,
					Assembler.InstanceUninitialized.AssemblerDataSnapshot.ReferencedNetAssemblyNames_StreamingBrokerAdapters) {
		}
	}
}
