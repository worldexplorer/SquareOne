using System;

using Sq1.Core.Streaming;

namespace Sq1.Core.Repositories {
	public class RepositoryDllStreamingAdapters : DllScannerExplicit<StreamingAdapter> {
		public RepositoryDllStreamingAdapters(string rootPath) :
				base(rootPath,
					Assembler.InstanceUninitialized.AssemblerDataSnapshot.ReferencedNetAssemblyNames_StreamingBrokerAdapters) {
			base.ChildrenDebug_DllExpected = "Sql.Core.dll";
		}

		protected override void ChildrenDebug_onDllDoesntExistInFolder() {
			string msg = "breakpoint_here";
		}
		protected override void ChildrenDebug_onDllMarkedAsSkipDll(string dllMarkedAsSkipDll) {
			string msg = "breakpoint_here";
		}
		protected override void ChildrenDebug_onTypesFoundInDll(string dllAbsPath, Type[] typesFoundInDll) {
			string msg = "breakpoint_here";
		}
		protected override void ChildrenDebug_TypeAdded(Type typeFound) {
			string msg = "breakpoint_here";
		}
		protected override void ChildrenDebug_CloneableInstanceForAssemblyAdded(StreamingAdapter classCastedInstance) {
			string msg = "breakpoint_here";
		}
		protected override void ChildrenDebug_CloneableInstanceByClassNameAdded(string className, StreamingAdapter classCastedInstance) {
			string msg = "breakpoint_here";
		}


	}
}
