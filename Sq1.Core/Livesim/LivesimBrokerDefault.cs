using System;

using Sq1.Core.Support;
using Sq1.Core.DataFeed;
using Sq1.Core.Streaming;
using Sq1.Core.Broker;

namespace Sq1.Core.Livesim {
	//I_WANT_LIVESIM_STREAMING_BROKER_BE_AUTOASSIGNED_AND_VISIBLE_IN_DATASOURCE_EDITOR => SkipInstantiationAt(Startup = false)
	[SkipInstantiationAt(Startup = false)]
	public sealed class LivesimBrokerDefault : LivesimBroker {

		public LivesimBrokerDefault() : base("USED_FOR_DATASOURCE_EDITOR_DUMMY") {
			string msg = "IM_HERE_WHEN_DLL_SCANNER_INSTANTIATES_DUMMY_STREAMING"
				//+ "IM_HERE_FOR_MY_CHILDREN_TO_HAVE_DEFAULT_CONSTRUCTOR"
		        + "_INVOKED_WHILE_REPOSITORY_SCANS_AND_INSTANTIATES_STREAMING_ADAPTERS_FOUND"
		        + " example:QuikLivesimBroker()";	// activated on MainForm.ctor() if [SkipInstantiationAt(Startup = true)]
			base.Name = "LivesimBrokerDefault-child_ACTIVATOR_DLL-SCANNED";
		}

		public LivesimBrokerDefault(string reasonToExist) : base(reasonToExist) {
			base.Name						= "LivesimBrokerDefault_WILL_BE_RENAMED_IN-InitializeDataSource_inverse()";
		}

		public override void InitializeLivesim(LivesimDataSource livesimDataSource, OrderProcessor orderProcessor) {
			base.Name						= "LivesimBrokerDefault";
			base.ReasonToExist				= "USED_FOR_LIVESIMMING_INITED_FROM_DATASOURCE";
			base.InitializeLivesim(livesimDataSource, orderProcessor);
		}
	}
}
