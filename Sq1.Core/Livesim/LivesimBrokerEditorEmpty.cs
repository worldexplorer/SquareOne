using System;

using Sq1.Core.Broker;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Livesim {
	public class LivesimBrokerEditorEmpty : BrokerEditor {
		public override void Initialize(BrokerAdapter brokerAdapter, IDataSourceEditor dataSourceEditor) {
			base.Initialize(brokerAdapter, dataSourceEditor);
		}
		public override void PullBrokerAdapterSettings_toEditor() {
			string msg = "INAPPLICABLE_FOR_A_LIVESIMSTREAMING_ENFORCED_TO_BE_MANDATORY_WHEN_NO_OTHER_STREAMING_IS_ASSIGNED_BY_USER_TO_A_DATASOURCE for streamingAdapter=[" + base.BrokerAdapter + "]";
		}
		public override void PushEditedSettings_toBrokerAdapter_serializeDataSource() {
			string msg = "INAPPLICABLE_FOR_A_LIVESIMSTREAMING_ENFORCED_TO_BE_MANDATORY_WHEN_NO_OTHER_STREAMING_IS_ASSIGNED_BY_USER_TO_A_DATASOURCE for streamingAdapter=[" + base.BrokerAdapter + "]";
		}

	}
}
