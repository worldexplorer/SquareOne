using System;

using Sq1.Core.Support;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Livesim {
	//I_WANT_LIVESIM_STREAMING_BROKER_BE_AUTOASSIGNED_AND_VISIBLE_IN_DATASOURCE_EDITOR => SkipInstantiationAt(Startup = false)
	[SkipInstantiationAt(Startup = false)]
	public sealed class LivesimStreamingDefault : LivesimStreaming {

		public LivesimStreamingDefault() : base("USED_FOR_DATASOURCE_EDITOR_DUMMY") {
			string msg = "IM_HERE_WHEN_DLL_SCANNER_INSTANTIATES_DUMMY_STREAMING"
				//+ "IM_HERE_FOR_MY_CHILDREN_TO_HAVE_DEFAULT_CONSTRUCTOR"
		        + "_INVOKED_WHILE_REPOSITORY_SCANS_AND_INSTANTIATES_STREAMING_ADAPTERS_FOUND"
		        + " example:QuikLivesimStreaming()";	// activated on MainForm.ctor() if [SkipInstantiationAt(Startup = true)]
			base.Name = "LivesimStreamingDefault-child_ACTIVATOR_DLL-SCANNED";
		}

		public LivesimStreamingDefault(string reasonToExist) : base(reasonToExist) {
			base.Name						= "LivesimStreamingDefault_WILL_BE_RENAMED_IN-InitializeDataSource_inverse()";
		}

		public override void InitializeDataSource_inverse(DataSource dataSource, bool subscribeSolidifier = true) {
			base.InitializeDataSource_inverse(dataSource, subscribeSolidifier);
			base.Name						= "LivesimStreamingDefault";
			base.ReasonToExist				= "USED_FOR_LIVESIMMING_INITED_FROM_DATASOURCE";
		}
	}
}
